// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions as a top level let in queries
// Dev11:182874

let add x y = x + y
let addG (x : 'a list) y = y :: x
let inline addInline x y = x + y
let rec fact x = if x = 0 then 1 else x * fact(x - 1)

let ie = [1;2;3;4;5]
let iq = System.Linq.Queryable.AsQueryable([1;2;3;4;5])

// Using a function defined outside the query
let q3 (ds : seq<int>) =
    query {
        for i in ds do
        select (add i i)
    } |> Seq.toList
if q3 ie <> [2;4;6;8;10] then printfn "q3 failed"; exit 1
if q3 iq <> [2;4;6;8;10] then printfn "q3 failed"; exit 1

let q3' (ds : seq<int>) =
    query {
        for i in ds do
        select (addInline i i)
    } |> Seq.toList
if q3' ie <> [2;4;6;8;10] then printfn "q3' failed"; exit 1
if q3' iq <> [2;4;6;8;10] then printfn "q3' failed"; exit 1

let q3'' (ds : seq<int>) =
    query {
        for i in ds do
        select (fact i)
    } |> Seq.toList
if q3'' ie <> [1;2;6;24;120] then printfn "q3'' failed"; exit 1
if q3'' iq <> [1;2;6;24;120] then printfn "q3'' failed"; exit 1

exit 0