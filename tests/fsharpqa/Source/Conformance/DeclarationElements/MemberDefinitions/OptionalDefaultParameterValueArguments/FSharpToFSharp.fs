// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments 

open System.Runtime.InteropServices
open System

type Class() =
    static member Method1  ([<Optional;DefaultParameterValue(42y)>]i:sbyte) = i
    static member Method2  ([<Optional;DefaultParameterValue(42uy)>]i:byte) = i
    static member Method3  ([<Optional;DefaultParameterValue(42s)>]i:int16) = i
    static member Method4  ([<Optional;DefaultParameterValue(42us)>]i:uint16) = i
    static member Method5  ([<Optional;DefaultParameterValue(42)>]i:int32) = i
    static member Method6  ([<Optional;DefaultParameterValue(42u)>]i:uint32) = i
    static member Method7  ([<Optional;DefaultParameterValue(42L)>]i:int64) = i
    static member Method8  ([<Optional;DefaultParameterValue(42UL)>]i:uint64) = i
    static member Method9  ([<Optional;DefaultParameterValue(42.42f)>]i:single) = i
    static member Method10 ([<Optional;DefaultParameterValue(42.42)>]i:double) = i
    static member Method11 ([<Optional;DefaultParameterValue(true)>]i:bool) = i
    static member Method12 ([<Optional;DefaultParameterValue('q')>]i:char) = i
    static member Method13 ([<Optional;DefaultParameterValue("ono")>]i:string) = i
    //reference type
    static member Method14 ([<Optional;DefaultParameterValue(null)>]i:obj) = i
    //struct type
    static member Method15 ([<Optional;DefaultParameterValue(new DateTime())>]i:DateTime) = i

let checkMethod f value defaultFun defaultValue =
    if f value <> value then exit 1
    if defaultFun() <> defaultValue then exit 1

do checkMethod (fun v -> Class.Method1 (v)) 1y       (fun () -> Class.Method1 ()) 42y
do checkMethod (fun v -> Class.Method2 (v)) 1uy      (fun () -> Class.Method2 ()) 42uy
do checkMethod (fun v -> Class.Method3 (v)) 1s       (fun () -> Class.Method3 ()) 42s
do checkMethod (fun v -> Class.Method4 (v)) 1us      (fun () -> Class.Method4 ()) 42us
do checkMethod (fun v -> Class.Method5 (v)) 1        (fun () -> Class.Method5 ()) 42
do checkMethod (fun v -> Class.Method6 (v)) 1u       (fun () -> Class.Method6 ()) 42u
do checkMethod (fun v -> Class.Method7 (v)) 1L       (fun () -> Class.Method7 ()) 42L
do checkMethod (fun v -> Class.Method8 (v)) 1UL      (fun () -> Class.Method8 ()) 42UL
do checkMethod (fun v -> Class.Method9 (v)) 1.1f     (fun () -> Class.Method9 ()) 42.42f
do checkMethod (fun v -> Class.Method10(v)) 1.1      (fun () -> Class.Method10()) 42.42
do checkMethod (fun v -> Class.Method11(v)) false    (fun () -> Class.Method11()) true
do checkMethod (fun v -> Class.Method12(v)) 'c'      (fun () -> Class.Method12()) 'q'
do checkMethod (fun v -> Class.Method13(v)) "noo"    (fun () -> Class.Method13()) "ono"
do checkMethod (fun v -> Class.Method14(v)) (obj())  (fun () -> Class.Method14()) null
do checkMethod (fun v -> Class.Method15(v)) (new DateTime(12300L))  (fun () -> Class.Method15()) (new DateTime())

exit 0
