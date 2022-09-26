// #in #BindingExpressions
//
// These are pretty pathological cases related to ";;" and "in"
// I'm adding these cases to make sure we do not accidentally change the behavior from version to version
// Eventually, we will deprecated them - and the specs will be updated.
//
//<Expects status="error" span="(11,5-11,6)" id="FS0039">The value or constructor 'a' is not defined</Expects>
//<Expects status="error" span="(11,5-11,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
module A
    let a = 3 in a + 1 |> ignore;;
    a > 4;;

