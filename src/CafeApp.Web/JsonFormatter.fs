module CafeApp.Web.JsonFormatter

open Newtonsoft.Json.Linq

open Suave
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

open CafeApp.Commands.Api.CommandHandler
open CafeApp.Core.Domain
open CafeApp.Core.Events
open CafeApp.Core.States
open CafeApp.Persistence.ReadModels

let (.=) key (value: obj) = JProperty(key, value)

let jObj (jProperties: JProperty list) =
    let jObject = JObject ()
    jProperties |> List.iter jObject.Add
    jObject

let jArray jObjects =
    let jArray = JArray ()
    jObjects |> List.iter jArray.Add
    jArray

let tabIdJObj tabId =
    jObj [
        "tabId" .= tabId
    ]

let tabJObj tab =
    jObj [
        "id" .= tab.Id
        "tableNumber" .= tab.TableNumber
    ]

let itemJObj item =
    jObj [
        "menuNumber" .= item.MenuNumber
        "name" .= item.Name
        "price" .= item.Price
    ]

let foodJObj (Food food) = itemJObj food

let drinkJObj (Drink drink) = itemJObj drink

let foodJArray foods =
    foods |> List.map foodJObj |> jArray

let drinkJArray drinks =
    drinks |> List.map drinkJObj |> jArray

let orderJObj (order: Order) =
    jObj [
        "tabId" .= order.Tab.Id
        "tableNumber" .= order.Tab.TableNumber
        "foods" .= foodJArray order.Foods
        "drinks" .= drinkJArray order.Drinks
    ]

let inProgressOrderJObj ipo =
    jObj [
        "tabId" .= ipo.PlacedOrder.Tab.Id
        "tableNumber" .= ipo.PlacedOrder.Tab.TableNumber
        "preparedFoods" .= foodJArray ipo.PreparedFoods
        "servedFoods" .= foodJArray ipo.ServedFoods
        "servedDrinks" .= drinkJArray ipo.ServedDrinks
    ]

let stateJObj = function
    | ClosedTab tabId ->
        let state = "state" .= "ClosedTab"
        match tabId with
        | Some tabId -> jObj [ state; "data" .= tabIdJObj tabId ]
        | None -> jObj [ state ]
    | OpenedTab tab ->
        jObj [
            "state" .= "OpenedTab"
            "data" .= tabJObj tab
        ]
    | PlacedOrder order ->
        jObj [
            "state" .= "PlacedOrder"
            "data" .= orderJObj order
        ]
    | OrderInProgress ipo ->
        jObj [
            "state" .= "OrderInProgress"
            "data" .= inProgressOrderJObj ipo
        ]
    | ServedOrder order ->
        jObj [
            "state" .= "ServedOrder"
            "data" .= orderJObj order
        ]

let statusJObj = function
    | Open tabId ->
        "status" .= jObj [
            "open" .= tabId.ToString ()
        ]
    | InService tabId ->
        "status" .= jObj [
            "inService" .= tabId.ToString ()
        ]
    | Closed -> "status" .= "closed"

let tableJObj table =
    jObj [
        "number" .= table.Number
        "waiter" .= table.Waiter
        statusJObj table.Status
    ]

let chefToDoJObj (todo: ChefToDo) =
    jObj [
        "tabId" .= todo.Tab.Id
        "tableNumber" .= todo.Tab.TableNumber
        "foods" .= foodJArray todo.Foods
    ]

let waiterToDoJObj (todo: WaiterToDo) =
    jObj [
        "tabId" .= todo.Tab.Id
        "tableNumber" .= todo.Tab.TableNumber
        "foods" .= foodJArray todo.Foods
        "drinks" .= drinkJArray todo.Drinks
    ]

let cashierToDoJObj (payment: Payment) =
    jObj [
        "tabId" .= payment.Tab.Id
        "tableNumber" .= payment.Tab.TableNumber
        "paymentAmount" .= payment.Amount
    ]

let eventJObj = function
    | TabOpened tab ->
        jObj [
            "event" .= "TabOpened"
            "data" .= tabJObj tab
        ]
    | OrderPlaced order ->
        jObj [
            "event" .= "OrderPlaced"
            "data" .= jObj [
                "order" .= orderJObj order
            ]
        ]
    | DrinkServed (drink, tabId) ->
        jObj [
            "event" .= "DrinkServed"
            "data" .= jObj [
                "drink" .= drinkJObj drink
                "tabId" .= tabId
            ]
        ]
    | FoodPrepared (food, tabId) ->
        jObj [
            "event" .= "FoodPrepared"
            "data" .= jObj [
                "food" .= foodJObj food
                "tabId" .= tabId
            ]
        ]
    | FoodServed (food, tabId) ->
        jObj [
            "event" .= "FoodServed"
            "data" .= jObj [
                "food" .= foodJObj food
                "tabId" .= tabId
            ]
        ]
    | OrderServed (order, payment) ->
        jObj [
            "event" .= "OrderServed"
            "data" .= jObj [
                "order" .= orderJObj order
                "tabId" .= order.Tab.Id
                "tableNumber" .= order.Tab.TableNumber
                "amount" .= payment.Amount
            ]
        ]
    | TabClosed payment ->
        jObj [
            "event" .= "TabClosed"
            "data" .= jObj [
                "amountPaid" .= payment.Amount
                "tabId" .= payment.Tab.Id
                "tableNumber" .= payment.Tab.TableNumber
            ]
        ]

let JSON webpart jsonString (context: HttpContext) = async {
    let wp =
        webpart jsonString >=> setMimeType "application/json; charset=utf-8"
    return! wp context
}

let toStateJson state =
    state |> stateJObj |> string |> JSON OK

let toErrorJson err =
    jObj [ "error" .= err.Message ] |> string |> JSON BAD_REQUEST

let toReadModelsJson toJObj key models =
    models
    |> List.map toJObj
    |> jArray
    |> (.=) key
    |> List.singleton
    |> jObj
    |> string
    |> JSON OK

let toTablesJson = toReadModelsJson tableJObj "tables"

let toChefToDosJson = toReadModelsJson chefToDoJObj "chefToDos"

let toWaiterToDosJson = toReadModelsJson waiterToDoJObj "waiterToDos"

let toCashierToDosJson = toReadModelsJson cashierToDoJObj "cashierToDos"

let toFoodsJson = toReadModelsJson foodJObj "foods"

let toDrinksJson = toReadModelsJson drinkJObj "drinks"
