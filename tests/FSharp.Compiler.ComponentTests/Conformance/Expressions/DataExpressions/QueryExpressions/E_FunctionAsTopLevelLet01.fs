// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions within a top level let in queries
// Dev11:182874
// <Expects status="error" id="FS3147" span="(10,20-10,24)">This 'let' definition may not be used in a query\. Only simple value definitions may be used in queries\.</Expects>
// <Expects status="error" id="FS3147" span="(18,17-18,21)">This 'let' definition may not be used in a query\. Only simple value definitions may be used in queries\.</Expects>

let q2' =
    query {
        for i in [1..5] do
        let inline add3 = add 3
        select (add3 i)
    } |> Seq.toList


let q2'' =
    query {
        for i in [1..5] do
        let rec add3 = add 3
        select (add3 i)
    } |> Seq.toList

exit 1