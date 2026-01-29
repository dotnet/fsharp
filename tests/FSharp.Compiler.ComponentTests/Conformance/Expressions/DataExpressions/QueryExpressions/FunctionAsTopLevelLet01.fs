// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions as a top level let in queries
// Dev11:182874

let add x y = x + y
let ie = [1;2;3;4;5]
let iq = System.Linq.Queryable.AsQueryable([1;2;3;4;5])

// Defining a 0 argument function at a top level let
let q2 (ds : seq<int>)=
    query {
        for i in ds do
        let add3 = add 3
        select (add3 i)
    } |> Seq.toList
if q2 ie <> [4;5;6;7;8] then printfn "q2 failed"; exit 1
if q2 iq <> [4;5;6;7;8] then printfn "q2 failed"; exit 1

exit 0