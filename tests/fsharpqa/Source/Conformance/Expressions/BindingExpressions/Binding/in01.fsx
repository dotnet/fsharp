// #in #BindingExpressions
//
// These are pretty pathological cases related to ";;" and "in"
// I'm adding these cases to make sure we do not accidentally change the behavior from version to version
// Eventually, we will deprecated them - and the specs will be updated.
//
//<Expects status="error" span="(10,1-10,2)" id="FS0039">The value or constructor 'a' is not defined</Expects>

let a = 3 in a + 1 |> ignore;;
a > 4;;

