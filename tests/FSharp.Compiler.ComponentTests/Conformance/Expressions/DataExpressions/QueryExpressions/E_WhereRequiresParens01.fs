// #Conformance #DataExpressions #Query
// Where expressions require parenthesis
//<Expects status="error" span="(8,21-8,22)" id="FS3145">This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.</Expects>
//<Expects status="error" span="(8,9-8,14)" id="FS3095">'where' is not used correctly\. This is a custom operation in this query or computation expression\.$</Expects>
let query =
    query {
        for i in [1..10] do
        where i % 2 = 0
        sortByDescending i
        select i
    }

exit 1