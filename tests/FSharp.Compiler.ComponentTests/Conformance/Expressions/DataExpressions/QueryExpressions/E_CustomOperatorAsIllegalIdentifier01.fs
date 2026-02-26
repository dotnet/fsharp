// #Conformance #DataExpressions #Query
// Errors for a couple cases where query custom operators are used as identifiers which we'd like to disallow, see DevDiv:187448
// <Expects id="FS3095" status="error" span="(7,16-7,21)">'minBy' is not used correctly\. This is a custom operation in this query or computation expression\.</Expects>
let q1 =
    query {
        for d in [1..10] do
        select minBy
    }

let q2 =
    query {
        for d in [1..10] do
        let averageBy = d // no error, let binding shadows the previously named averageBy
        sumBy averageBy
    }

exit 1