// #Conformance #DataExpressions #Query
// Yield and Select are interchangeable

let q1 =
    query {
        for i in [1..10] do
        where (i % 2 = 0)
        yield i
    }

let q2 =
    query {
        for i in [1..10] do
        where (i % 2 = 0)
        select i
    }

if not ((sprintf "%A" q1) = (sprintf "%A" q2)) then exit 1 else exit 0