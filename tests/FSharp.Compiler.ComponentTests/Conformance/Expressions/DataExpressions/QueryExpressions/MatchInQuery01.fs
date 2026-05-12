// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions as a top level let in queries
// Dev11:182874

let ie = [1;2;3;4;5]
let iq = System.Linq.Queryable.AsQueryable([1;2;3;4;5])

// Use match within a let binding
let q4 (ds : seq<int>) =
    query {
        for i in ds do
        let x = 
            match (i % 2) with
            | 0 -> i
            | _ -> i * i
        select x
    } |> Seq.toList
if q4 ie <> [1;2;9;4;25] then printfn "q4 failed"; exit 1
if q4 iq <> [1;2;9;4;25] then printfn "q4 failed"; exit 1

let q4' (ds : seq<int>) =
    query {
        for i in ds do
        select (
            match (i % 2) with
            | 0 -> i
            | _ -> i * i
        )
    } |> Seq.toList
if q4' ie <> [1;2;9;4;25] then printfn "q4' failed"; exit 1
if q4' iq <> [1;2;9;4;25] then printfn "q4' failed"; exit 1

exit 0