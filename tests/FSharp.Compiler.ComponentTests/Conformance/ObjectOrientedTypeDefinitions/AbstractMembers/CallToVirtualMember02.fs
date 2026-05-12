// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS bug Dev10:834160
// This test would fail if we don't set the IsImplemented bit for the abstract slot
//<Expects status="success"></Expects>

[<AbstractClass>]
type B() = 
    abstract M : int -> float
    default x.M(a:int) : float  = 1.0
and [<AbstractClass>]
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = base.M(3) }
