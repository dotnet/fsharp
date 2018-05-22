// #Conformance #DataExpressions #Query
// DevDiv:188241, just baselines errors around spacing
//<Expects status="error" span="(15,13-16,25)" id="FS0501">The member or object constructor 'Where' takes 2 argument\(s\) but is here given 4\. The required signature is 'member Linq\.QueryBuilder\.Where : source:Linq\.QuerySource<'T,'Q> \* predicate:\('T \-> bool\) \-> Linq\.QuerySource<'T,'Q>'\.$</Expects>

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
