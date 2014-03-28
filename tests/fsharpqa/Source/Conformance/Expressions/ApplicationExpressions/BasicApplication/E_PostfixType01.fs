// #Regression #Conformance #ApplicationExpressions 
// Regression test for FSHARP1.0:5525
// Deprecate postfix type application in "new" and "inherit" constructs
//<Expects status="error" span="(7,13-7,18)" id="FS0035">This construct is deprecated: The use of the type syntax 'int C' and 'C  <int>' is not permitted here\. Consider adjusting this type to be written in the form 'C<int>'$</Expects>

type T<'t> = System.Collections.Generic.List<'t>
let o = new int T ()
