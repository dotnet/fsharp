// #Conformance #DataExpressions #Query
// Where expressions require parenthesis

let query =
    query {
        for i in [1..10] do
        where (i % 2 = 0)
        sortByDescending i
        select i
    }

let r = "seq [10; 8; 6; 4; ...]" = sprintf "%A" query 
if not r then exit 1 else exit 0