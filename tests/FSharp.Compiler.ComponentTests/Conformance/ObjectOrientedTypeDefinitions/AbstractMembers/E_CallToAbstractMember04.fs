// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS bug Dev10:834160
// This test would fail if we don't set the IsImplemented bit for the abstract slot
//<Expects status="error" span="(14,55-14,64)" id="FS1201">Cannot call an abstract base member: 'M'$</Expects>
//<Expects status="error" span="(15,57-15,68)" id="FS1201">Cannot call an abstract base member: 'M'$</Expects>
[<AbstractClass>]
type B() = 
    abstract M : int -> float
    abstract M : string -> unit
and [<AbstractClass>]
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = base.M(3) 
                         member x.M(a:string) : unit  = base.M("3") }
