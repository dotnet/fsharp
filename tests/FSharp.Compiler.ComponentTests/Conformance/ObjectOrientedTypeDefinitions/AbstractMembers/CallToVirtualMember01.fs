// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for TFS bug Dev10:834160
//<Expects status="success"></Expects>

[<AbstractClass>]
type B() = 
    abstract M : int -> float
    abstract M : string -> unit
and [<AbstractClass>]
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = base.M(3) }
    default x.M(a:int) : float  = 1.0
    default x.M(a:string) : unit = (a:string) |> ignore
