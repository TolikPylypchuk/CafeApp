module CafeApp.Web.QueriesApi

open Suave
open Suave.Filters
open Suave.Operators

open CafeApp.Core.CommandHandlers
open CafeApp.Persistence.Queries

open JsonFormatter

let readModels getReadModels wp (context: HttpContext) =
    async {
        let! models = getReadModels ()
        return! wp models context
    }

let queriesApi queries eventStore =
    GET >=> choose [
        path "/tables" >=>
            readModels queries.Table.GetTables toTablesJson
    ]
