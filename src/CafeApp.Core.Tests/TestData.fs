module CafeApp.Tests.TestData

open System

open CafeApp.Domain

let tab = { Id = Guid.NewGuid(); TableNumber = 1 }

let coke = Drink {
    MenuNumber = 1;
    Name = "Coke";
    Price = 1.5M
}

let lemonade = Drink {
    MenuNumber = 3;
    Name = "Lemonade";
    Price = 1.0M
}

let appleJuice = Drink {
    MenuNumber = 5;
    Name = "Apple Juice";
    Price = 1.0M
}

let order = { Tab = tab; Foods = []; Drinks = [] }

let salad = Food {
    MenuNumber = 2;
    Name = "Salad";
    Price = 2.5M
}

let pizza = Food {
    MenuNumber = 4;
    Name = "Pizza";
    Price = 6.5M
}

let foodPrice (Food food) = food.Price

let drinkPrice (Drink drink) = drink.Price
