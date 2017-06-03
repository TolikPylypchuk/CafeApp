module CafeApp.Web.Program

open System.Text

open Chessie.ErrorHandling

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

open CafeApp.Commands.Api.CommandApi
open CafeApp.Core.Events
open CafeApp.Persistence.Projections
open CafeApp.Persistence.InMemory.InMemory
open CafeApp.Web.JsonFormatter

let eventsStream = new Control.Event<Event list>()

let project event =
    projectReadModel inMemoryActions event
    |> Async.RunSynchronously |> ignore

let projectEvents = project |> List.iter

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

[<EntryPoint>]
let main argv =
    let app =
        let eventStore = inMemoryEventStore ()
        choose [
            commandApi eventStore
        ]

    let config = {
        defaultConfig with bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8083 ]
    }

    eventsStream.Publish.Add projectEvents

    startWebServer config app

    0
