// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS bug Dev10:834160
// This test would fail if we don't set the IsImplemented bit for the abstract slot
//<Expects status="error" span="(11,5-11,6)" id="FS0365">No implementation was given for 'abstract B\.M: int -> float'$</Expects>
//<Expects status="error" span="(11,5-11,6)" id="FS0054">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>
[<AbstractClass>]
type B() = 
    abstract M : int -> float
    abstract M : string -> unit
and 
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = 1.0 }
    //default x.M(a:int) : float  = 1.0
    default x.M(a:string) : unit = (a:string) |> ignore
