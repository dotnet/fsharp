// Regression test for DevDiv:305886
// [QueryExpressions] Identifiers for range variables in for-join queries cannot be uppercase!
// We expect a simple warning.
//<Expects status="warning" span="(10,9-10,18)" id="FS3874">Variable patterns should be lowercase\.$</Expects>

module M
// Warning
let _ =
   query {
    for UpperCase in [1..10] do
    groupJoin UpperCase2 in [|1..2|] on (UpperCase = UpperCase2) into g
    for k in g do
    select (k + 1)
}
