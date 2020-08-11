// #Conformance #DataExpressions #Query
// DevDiv:188241, just baselines errors around spacing
//<Expects status="error" span="(15,13-15,18)" id="FS3099">'where' is used with an incorrect number of arguments\. This is a custom operation in this query or computation expression\. Expected 1 argument\(s\), but given 3\.$</Expects>

let q1 = // no errors
    query {
        for d in [1..10] do
            if d > 3 then
                select d
    }

let q2 = 
    query {
        for d in [1..10] do
            where (d > 3)
                select d
    }

exit 1
