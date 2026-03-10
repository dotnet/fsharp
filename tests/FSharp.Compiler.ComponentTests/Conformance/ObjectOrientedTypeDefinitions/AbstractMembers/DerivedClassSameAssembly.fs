// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS#834683
// Virtual interface implementation (same assembly) 
//<Expects status="success"></Expects>

// Interface with a member
type I = abstract M : unit -> unit

// Class that implements the interface I _and_ also has a 
type C = 
    new() = {}
    abstract member M : unit -> unit
    default this.M() = System.Console.WriteLine("I am my method")
    interface I with
        member this.M() = 
            System.Console.WriteLine("I am going via the interface")
            this.M()
type D = 
    inherit C
    new() = {}
    override this.M() = System.Console.WriteLine("I am MyMethod in MyDerivedClass")
                        base.M()

let d = D()
d.M()
let di = d :> I
di.M()
