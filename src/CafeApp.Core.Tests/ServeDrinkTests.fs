module CafeApp.Tests.ServeDrinkTests

open NUnit.Framework

open CafeApp.Commands
open CafeApp.Domain
open CafeApp.Errors
open CafeApp.Events
open CafeApp.States

open CafeAppTestsDSL
open TestData

[<Test>]
let ``Can Serve Drink`` () =
    let order = { order with Drinks = [ coke; lemonade ] }
    let expected = {
        PlacedOrder = order;
        ServedDrinks = [ coke ];
        PreparedFoods = [];
        ServedFoods = []
    }

    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ DrinkServed (coke, order.Tab.Id) ]

[<Test>]
let ``Cannot Serve Non Ordered Drink`` () =
    let order = { order with Drinks = [ coke ] }
    Given (PlacedOrder order)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonOrderedDrink lemonade)

[<Test>]
let ``Cannot Serve Drink for Already Served Order`` () =
    Given (ServedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Test>]
let ``Cannot Serve Drinks for Non Placed Order`` () =
    Given (OpenedTab tab)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CannotServeForNonPlacedOrder

[<Test>]
let ``Cannot Serve with Closed Tab`` () =
    Given (ClosedTab None)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CannotServeWithClosedTab

[<Test>]
let ``Can Serve Drink for Order Containing Only One Drink`` () =
    let order = { order with Drinks = [ coke ] }
    let payment = { Tab = order.Tab; Amount = drinkPrice coke }
    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (ServedOrder order)
    |> WithEvents [
        DrinkServed (coke, order.Tab.Id)
        OrderServed (order, payment)
    ]
