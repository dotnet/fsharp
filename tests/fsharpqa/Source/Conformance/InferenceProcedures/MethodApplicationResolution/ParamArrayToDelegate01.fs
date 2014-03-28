// #Conformance #ObjectOrientedTypes #Delegates #TypeInference
// Regression for Dev11:33760
// Previously param arrays didn't work with implicit delegate conversions

open System
open System.Threading.Tasks

type C() = 
    static member M1([<ParamArray>] arg: System.Action []) = ()
    static member M2([<ParamArray>] arg: System.Func<int> []) = ()
    member x.M3([<ParamArray>] arg: System.Action []) = ()
    member x.M4([<ParamArray>] arg: System.Func<int> []) = ()

module Extensions =
    type C with
        static member M5(x, [<ParamArray>] arg: System.Action []) = ()
        static member M6(x, [<ParamArray>] arg: System.Func<int> []) = ()


C.M1(fun () -> ())
C.M2(fun () -> 1)
C().M3(fun () -> ())
C().M4(fun () -> 1)

open Extensions

C.M5(1, fun () -> ())
C.M6(1, fun () -> 1)

exit 0