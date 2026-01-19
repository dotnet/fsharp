// #Conformance #DataExpressions #Query #Regression
// Various ways of defining functions as a top level let in queries
// Dev11:182874
//<Expects status="error" span="(9,9-11,21)" id="FS3145">This is not a known query operator\. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type\.</Expects>

let q4' (ds : seq<int>) =
    query {
        for i in ds do
        match (i % 2) with
        | 0 -> i
        | _ -> i * i
        select i
    } |> Seq.toList