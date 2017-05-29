module CafeApp.Core.Tests.ServeFoodTests

open System

open Xunit

open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Core.Errors
open CafeApp.Core.Events
open CafeApp.Core.States

open CafeAppTestsDSL
open TestData

[<Fact>]
let ``Can Maintain the Order in Progress State by Serving Food``() =
    let order = { order with Foods = [ salad; pizza ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = [ salad; pizza ]
    }

    let expected = { inProgressOrder with ServedFoods = [ salad ] }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ FoodServed (salad, order.Tab.Id) ]

[<Fact>]
let ``Can Serve Only Prepared Food``() =
    let order = { order with Foods = [ salad; pizza ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = [ salad ]
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeFood (pizza, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonPreparedFood pizza)

[<Fact>]
let ``Can Serve Food for Order Containing Only One Food``() =
    let order = { order with Foods = [ salad ] }
    let inProgressOrder = {
        PlacedOrder = order
        PreparedFoods = [ salad ]
        ServedDrinks = []
        ServedFoods = []
    }

    let payment = { Tab = order.Tab; Amount = foodPrice salad }
    
    Given (OrderInProgress inProgressOrder)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (ServedOrder order)
    |> WithEvents [
        FoodServed (salad, order.Tab.Id)
        OrderServed (order, payment)
    ]

[<Fact>]
let ``Cannot Serve Non-Ordered Food``() =
    let order = { order with Foods = [ salad ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = [ salad ]
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeFood (pizza, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonOrderedFood pizza)

[<Fact>]
let ``Cannot Serve Already Served Food``() =
    let order = { order with Foods = [ salad; pizza ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedFoods = [ salad ]
        ServedDrinks = []
        PreparedFoods = [ salad; pizza ]
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith (CannotServeAlreadyServedFood salad)

[<Fact>]
let ``Cannot Serve for Placed Order``() =
    Given (PlacedOrder order)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonPreparedFood salad)

[<Fact>]
let ``Cannot Serve for Non Placed Order``() =
    Given (OpenedTab tab)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith CannotServeForNonPlacedOrder

[<Fact>]
let ``Cannot Serve for Already Served Order``() =
    Given (ServedOrder order)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Cannot Serve with Closed Tab``() =
    Given (ClosedTab None)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith CannotServeWithClosedTab
