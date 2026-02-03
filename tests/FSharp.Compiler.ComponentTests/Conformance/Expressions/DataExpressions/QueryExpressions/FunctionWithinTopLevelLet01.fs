// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions within a top level let in queries
// Dev11:182874

let add x y = x + y
let addG (x : 'a list) y = y :: x
let inline addInline x y = x + y
let rec fact x = if x = 0 then 1 else x * fact(x - 1)

let ie = [1;2;3;4;5]
let iq = System.Linq.Queryable.AsQueryable([1;2;3;4;5])

// Defining a function within a top level let
let q1 (ds : seq<int>) =
    query {
        for i in ds do
        let aFunc =
            let f x = x + 1
            f
        select (aFunc i)
    } |> Seq.toList
if q1 ie <> [2;3;4;5;6] then printfn "q1 failed"; exit 1
if q1 iq <> [2;3;4;5;6] then printfn "q1 failed"; exit 1

let q1' (ds : seq<int>) =
    query {
        for i in ds do
        let aFunc =
            let inline f x = x + 1
            f
        select (aFunc i)
    } |> Seq.toList
if q1' ie <> [2;3;4;5;6] then printfn "q1' failed"; exit 1
if q1' iq <> [2;3;4;5;6] then printfn "q1' failed"; exit 1

let q1''' (ds : seq<int>) =
    query {
        for i in ds do
        let aFunc =
            let rec fact1 x = if x = 0 then 1 else x * fact1(x - 1)
            fact1
        select (aFunc i)
    } |> Seq.toList
if q1''' ie <> [1;2;6;24;120] then printfn "q1''' failed"; exit 1
if q1''' iq <> [1;2;6;24;120] then printfn "q1''' failed"; exit 1

exit 0