// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions within a top level let in queries
// Dev11:182874
// <Expects status="error" id="FS1230" span="(9,13-9,17)">Inner generic functions are not permitted in quoted expressions. Consider adding some type constraints until this function is no longer generic.</Expects>

let q2''' =
    query {
        for i in [1..5] do
        let addG (x : 'a list) y = y :: x
        select (addG [] i)
    } |> Seq.toList