// #Conformance #DataExpressions #Query
// Where expressions require parenthesis
//<Expects status="error" span="(8,9-8,14)" id="FS3095">'where' is not used correctly\. This is a custom operation in this query or computation expression\.$</Expects>
//<Expects status="error" span="(8,9-8,24)" id="FS0020">The result of this expression is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
let query =
    query {
        for i in [1..10] do
        where i % 2 = 0
        sortByDescending i
        select i
    }

exit 1