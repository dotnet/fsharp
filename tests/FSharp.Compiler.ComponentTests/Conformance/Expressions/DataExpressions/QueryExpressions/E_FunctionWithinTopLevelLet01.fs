// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions within a top level let in queries
// Dev11:182874
// <Expects status="error" id="FS1230" span="(9,13-9,18)">Inner generic functions are not permitted in quoted expressions\. Consider adding some type constraints until this function is no longer generic\.</Expects>

let q1'' (ds : seq<int>) =
    query {
        for i in ds do
        let aFunc =
            let addG (x : 'a list) y = y :: x
            addG
        select (aFunc [] i)
    } |> Seq.toList