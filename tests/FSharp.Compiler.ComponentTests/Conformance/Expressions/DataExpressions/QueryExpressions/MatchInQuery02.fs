// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions as a top level let in queries
// Dev11:182874

let ie = [1;2;3;4;5]
let iq = System.Linq.Queryable.AsQueryable([1;2;3;4;5])

// Use function instead of match
let q5 (ds : seq<int>) =
    query {
        for i in ds do
        let x = 
            function
            | 0 -> i
            | _ -> i * i
        select (x i)
    } |> Seq.toList
if q5 ie <> [1;4;9;16;25] then printfn "q5 failed"; exit 1
if q5 iq <> [1;4;9;16;25] then printfn "q5 failed"; exit 1

let q5' (ds : seq<int>) =
    query {
        for i in ds do
        select 
            ((function
             | 0 -> i
             | _ -> i * i) i)             
    } |> Seq.toList
if q5' ie <> [1;4;9;16;25] then printfn "q5' failed"; exit 1
if q5' iq <> [1;4;9;16;25] then printfn "q5' failed"; exit 1

exit 0