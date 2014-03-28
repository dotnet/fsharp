// #Regression #Conformance #ApplicationExpressions 
// Regression test for FSHARP1.0:5525
// Deprecate postfix type application in "new" and "inherit" constructs
//<Expects status="error" span="(9,27-9,32)" id="FS0035">This construct is deprecated: The use of the type syntax 'int C' and 'C  <int>' is not permitted here\. Consider adjusting this type to be written in the form 'C<int>'$</Expects>
#nowarn "0988"
type T<'t> = System.Collections.Generic.List<'t>

type C<'t> () = class
                  inherit int T ()
                end

