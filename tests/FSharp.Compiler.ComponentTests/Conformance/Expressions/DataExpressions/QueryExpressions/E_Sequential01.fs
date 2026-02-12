// #Conformance #DataExpressions #Query #Regression
// DevDiv:179747, sequential statements in queries
//<Expects status="error" span="(10,9-10,16)" id="FS3145">This is not a known query operator\. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type\.$</Expects>
//<Expects status="error" span="(17,9-17,16)" id="FS3145">This is not a known query operator\. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type\.$</Expects>
//<Expects status="error" span="(25,9-25,16)" id="FS3145">This is not a known query operator\. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type\.$</Expects>

let q1 ds =
    query {
        for c in ds do
        printfn "%A" c
        select c
    } 

let q1' ds =
    query {
        for c in ds do
        printfn "%A" c
        yield c
    } 

let q2 ds =
    query {
        for c in ds do
        join(for d in ds -> c = d)
        printfn "%A" c
        select c
    }

exit 1