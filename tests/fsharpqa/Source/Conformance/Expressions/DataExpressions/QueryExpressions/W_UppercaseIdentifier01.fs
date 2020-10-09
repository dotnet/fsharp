// Regression test for DevDiv:305886
// [QueryExpressions] Identifiers for range variables in for-join queries cannot be uppercase!
// We expect a simple warning.
//<Expects status="warning" span="(10,9-10,18)" id="FS0049">Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name\.$</Expects>

module M
// Warning
let _ =
   query {
    for UpperCase in [1..10] do
       join b in [1..2] on (UpperCase = b)
       select b
}

