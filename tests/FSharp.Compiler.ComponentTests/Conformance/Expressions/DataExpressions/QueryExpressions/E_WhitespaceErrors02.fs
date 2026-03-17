// #Conformance #DataExpressions #Query
// DevDiv:188241, just baselines errors around spacing
//<Expects status="error" span="(16,17-16,22)" id="FS0010">Unexpected yield in expression\. Expected incomplete structured construct at or before this point or other token\.$</Expects>

let q3 = // no errors
    query {
        for d in [1..10] do
        where (d > 3)
        select d
    }

let q4 = 
    query {
        for d in [1..10] do
            where (d > 3)
                yield d
    }

