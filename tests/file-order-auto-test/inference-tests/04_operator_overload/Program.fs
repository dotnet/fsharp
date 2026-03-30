module OperatorTest.Program

open OperatorTest.Types
open OperatorTest.Logic

[<EntryPoint>]
let main _argv =
    let items = [
        { Amount = 10.00m; Currency = "USD" }
        { Amount = 25.50m; Currency = "USD" }
        { Amount = 3.99m; Currency = "USD" }
    ]
    let total = totalPrice items
    let discounted = applyDiscount 0.1m total
    let change = calculateChange { Amount = 50.0m; Currency = "USD" } discounted
    printfn "Total: %M %s" total.Amount total.Currency
    printfn "After 10%% discount: %M %s" discounted.Amount discounted.Currency
    printfn "Change from 50: %M %s" change.Amount change.Currency
    0
