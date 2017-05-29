module CafeApp.Core.Tests.PrepareFoodTests

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
let ``Can Prepare Food``() =
    let order = { order with Foods = [ salad ] }
    let expected = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = [ salad ]
        ServedFoods = []
    }

    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ FoodPrepared (salad, order.Tab.Id) ]

[<Fact>]
let ``Can Prepare Food During Order in Progress``() =
    let order = { order with Foods = [ salad; pizza ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = [ pizza ]
        ServedFoods = []
    }

    let expected = { inProgressOrder with PreparedFoods = [ salad; pizza ] }

    Given (OrderInProgress inProgressOrder)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ FoodPrepared (salad, order.Tab.Id) ]

[<Fact>]
let ``Cannot Prepare Non-Ordered Food``() =
    let order = { order with Foods = [ pizza ] }

    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith (CannotPrepareNonOrderedFood salad)

[<Fact>]
let ``Cannot Prepare Food for Served Order``() =
    Given (ServedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Cannot Prepare Non-Ordered Food During Order in Progress``() =
    let order = { order with Foods = [ salad ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = []
        ServedFoods = []
    }

    Given (OrderInProgress inProgressOrder)
    |> When (PrepareFood (pizza, order.Tab.Id))
    |> ShouldFailWith (CannotPrepareNonOrderedFood pizza)

[<Fact>]
let ``Cannot Prepare Already Prepared Food During Order in Progress``() =
    let order = { order with Foods = [ salad ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = [ salad ]
        ServedFoods = []
    }

    Given (OrderInProgress inProgressOrder)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith (CannotPrepareAlreadyPreparedFood salad)

[<Fact>]
let ``Cannot Prepare with Closed Tab``() =
    Given (ClosedTab None)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith CannotPrepareWithClosedTab
