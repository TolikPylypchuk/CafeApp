module CafeApp.Web.Program

open System.Text

open Chessie.ErrorHandling

open Suave
open Suave.Control
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Sockets
open Suave.Sockets.Control
open Suave.Sockets.SocketOp
open Suave.Successful
open Suave.WebSocket

open CafeApp.Commands.Api.CommandApi
open CafeApp.Core.Events
open CafeApp.Persistence.Projections
open CafeApp.Persistence.InMemory.InMemory

open JsonFormatter
open QueriesApi

let eventsStream = new Event<Event list>()

let project event =
    projectReadModel inMemoryActions event
    |> Async.RunSynchronously
    |> ignore

let projectEvents = List.iter project

let commandApiHandler eventStore (context: HttpContext) = async {
    let payload = Encoding.UTF8.GetString context.request.rawForm
    let! response = handleCommandRequest inMemoryQueries eventStore payload
    match response with
    | Ok ((state, events), _) ->
        do! eventStore.SaveEvents state events
        eventsStream.Trigger events
        return! context |> toStateJson state
    | Bad err ->
        return! context |> toErrorJson err.Head
}

let commandApi eventStore =
    path "/command"
        >=> POST
        >=> commandApiHandler eventStore

let socketHandler (ws: WebSocket) context = socket {
    printfn "In socket handler"
    
    while true do
        let! events =
            Async.AwaitEvent eventsStream.Publish |> ofAsync

        printfn "Received events"

        for event in events do
            printfn "%A" event
            let eventData =
                event
                |> eventJObj
                |> string
                |> Encoding.UTF8.GetBytes
                |> ByteSegment
            do! ws.send Text eventData true
}

[<EntryPoint>]
let main argv =
    let app =
        let eventStore = inMemoryEventStore ()
        choose [
            path "/websocket" >=>
                handShake socketHandler
            commandApi eventStore
            queriesApi inMemoryQueries eventStore
        ]

    let config = {
        defaultConfig with
            bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8083 ]
    }

    eventsStream.Publish.Add projectEvents

    startWebServer config app

    0
