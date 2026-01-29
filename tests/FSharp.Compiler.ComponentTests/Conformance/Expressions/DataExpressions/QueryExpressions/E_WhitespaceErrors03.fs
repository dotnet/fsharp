// #Conformance #DataExpressions #Query
// DevDiv:188241, just baselines errors around spacing
//<Expects status="error" span="(15,13-15,17)" id="FS3098">'join' must come after a 'for' selection clause and be followed by the rest of the query\. Syntax: \.\.\. join var in collection on \(outerKey = innerKey\)\. Note that parentheses are required after 'on' \.\.\.$</Expects>
//<Expects status="error" span="(21,9-21,13)" id="FS3098">'join' must come after a 'for' selection clause and be followed by the rest of the query\. Syntax: \.\.\. join var in collection on \(outerKey = innerKey\)\. Note that parentheses are required after 'on' \.\.\.$</Expects>
let q5 = // no errors
    query { 
        for d in [1..10] do 
            where (d > 3)
            select d
    }

let q6 =
    query {
        for d in [1..10] do
            join(for c in [1..10] -> d = c)
                select d
    }
let q7 =
    query {
        for d in [1..10] do
        join(for c in [1..10] -> d = c)
            select d
    }

exit 1