module CafeApp.Persistence.InMemory.Items

open System.Collections.Generic

open CafeApp.Core.Domain
open CafeApp.Persistence.Queries

let private foods =
    let dict = new Dictionary<int, Food>()
    dict.Add(8, Food {
        MenuNumber = 8
        Price = 5M
        Name = "Salad"
    })
    dict.Add(9, Food {
        MenuNumber = 8
        Price = 10M
        Name = "Pizza"
    })
    dict

let private drinks =
    let dict = new Dictionary<int, Drink>()
    dict.Add(8, Drink {
        MenuNumber = 10
        Price = 2.5M
        Name = "Coke"
    })
    dict.Add(9, Drink {
        MenuNumber = 11
        Price = 1.5M
        Name = "Lemonade"
    })
    dict

let private getItem<'a> (dict: Dictionary<int, 'a>) key =
    if dict.ContainsKey key then
        dict.[key] |> Some
    else 
        None

let private getItems<'a> (dict: Dictionary<int, 'a>) keys =
    let invalidKeys = keys |> Array.except dict.Keys
    if Array.isEmpty invalidKeys then
        keys
        |> Array.map (fun n -> dict.[n])
        |> Array.toList
        |> Choice1Of2
    else
        invalidKeys |> Choice2Of2

let getFoods () =
    foods.Values |> Seq.toList |> async.Return

let getDrinks () =
    drinks.Values |> Seq.toList |> async.Return

let getFoodByMenuNumber key =
    getItem foods key |> async.Return

let getFoodsByMenuNumbers keys =
    getItems foods keys |> async.Return

let getDrinkByMenuNumber key =
    getItem drinks key |> async.Return

let getDrinksByMenuNumbers keys =
    getItems drinks keys |> async.Return

let foodQueries = {
    GetFoods = getFoods
    GetFoodByMenuNumber = getFoodByMenuNumber
    GetFoodsByMenuNumbers = getFoodsByMenuNumbers
}

let drinkQueries = {
    GetDrinks = getDrinks
    GetDrinkByMenuNumber = getDrinkByMenuNumber
    GetDrinksByMenuNumbers = getDrinksByMenuNumbers
}
