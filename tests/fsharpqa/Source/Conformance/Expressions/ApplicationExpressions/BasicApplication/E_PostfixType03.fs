// #Regression #Conformance #ApplicationExpressions 
// Regression test for FSHARP1.0:5525
// Deprecate postfix type application in "new" and "inherit" constructs
//<Expects status="error" span="(9,31-9,32)" id="FS0010">Unexpected identifier in member definition$</Expects>
#nowarn "0988"
type T<'t> = System.Collections.Generic.List<'t>

type C<'t> () = class
                  inherit int T ()
                end

