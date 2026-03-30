module OperatorTest.Types

type Money = {
    Amount: decimal
    Currency: string
}
with
    static member (+) (a: Money, b: Money) =
        if a.Currency <> b.Currency then failwith "Currency mismatch"
        { Amount = a.Amount + b.Amount; Currency = a.Currency }
    static member (-) (a: Money, b: Money) =
        if a.Currency <> b.Currency then failwith "Currency mismatch"
        { Amount = a.Amount - b.Amount; Currency = a.Currency }
    static member (*) (scalar: decimal, m: Money) =
        { Amount = scalar * m.Amount; Currency = m.Currency }
