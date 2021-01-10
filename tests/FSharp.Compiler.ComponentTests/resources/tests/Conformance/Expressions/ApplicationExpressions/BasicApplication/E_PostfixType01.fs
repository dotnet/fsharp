// #Regression #Conformance #ApplicationExpressions 
// Regression test for FSHARP1.0:5525
// Deprecate postfix type application in "new" and "inherit" constructs
//<Expects status="error" span="(7,17-7,18)" id="FS0010">Unexpected identifier in expression$</Expects>

type T<'t> = System.Collections.Generic.List<'t>
let o = new int T ()
