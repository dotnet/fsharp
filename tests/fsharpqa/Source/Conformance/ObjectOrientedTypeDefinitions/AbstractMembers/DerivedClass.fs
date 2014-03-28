// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS#834683
//<Expects status="success"></Expects>

#if CSLIBRARY
open CSLibrary
#else
open FSLibrary
#endif

type D = 
    inherit C
    new() = {}
    override this.M() = System.Console.WriteLine("I am method M in D")
                        base.M()

let d = D()
d.M()
let di = d :> I
di.M()

exit 0
