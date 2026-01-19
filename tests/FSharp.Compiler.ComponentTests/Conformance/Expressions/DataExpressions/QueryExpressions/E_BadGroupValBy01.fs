// #Conformance #DataExpressions #Query
// DevDiv:179889, groupValBy was hard to use with poor diagnostics
//<Expects status="error" span="(9,9-9,19)" id="FS3099">'groupValBy' is used with an incorrect number of arguments\. This is a custom operation in this query or computation expression\. Expected 2 argument\(s\), but given 0\.$</Expects>
//<Expects status="error" span="(15,9-15,19)" id="FS3099">'groupValBy' is used with an incorrect number of arguments\. This is a custom operation in this query or computation expression\. Expected 2 argument\(s\), but given 1\.$</Expects>

let q1 = 
    query {
        for c in [1..10] do
        groupValBy         
    }

let q2 =
    query {
        for c in [1..10] do
        groupValBy c        
    }

let q3 = // no error
    query {
        for c in [1..10] do
        groupValBy c c
    }

exit 1
