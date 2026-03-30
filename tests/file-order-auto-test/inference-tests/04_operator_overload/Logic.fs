module OperatorTest.Logic

open OperatorTest.Types

let totalPrice (items: Money list) =
    items |> List.reduce (+)

let applyDiscount (rate: decimal) (price: Money) =
    (1.0m - rate) * price

let calculateChange (paid: Money) (cost: Money) =
    paid - cost
