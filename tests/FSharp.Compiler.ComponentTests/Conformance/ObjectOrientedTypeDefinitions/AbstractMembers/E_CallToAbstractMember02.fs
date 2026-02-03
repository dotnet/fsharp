// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS bug Dev10:834160
// This test would fail if we don't set the IsImplemented bit for the abstract slot
//<Expects status="error" span="(13,20-14,60)" id="FS0365">No implementation was given for 'abstract B\.M: string -> unit'$</Expects>

[<AbstractClass>]
type B() = 
    abstract M : int -> float
    abstract M : string -> unit
and [<AbstractClass>]
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = 1.0 }
    default x.M(a:int) : float  = 1.0
    //default x.M(a:string) : unit = (a:string) |> ignore
