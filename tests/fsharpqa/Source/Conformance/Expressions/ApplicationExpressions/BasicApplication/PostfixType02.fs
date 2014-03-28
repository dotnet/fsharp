// #Regression #Conformance #ApplicationExpressions 
// Regression test for FSHARP1.0:5525
// Deprecate postfix type application in "new" and "inherit" constructs
//<Expects status="success"></Expects>

type I<'i> = interface
             end

type CC<'t> () = class
                    interface int I     // nothing wrong here
                 end
