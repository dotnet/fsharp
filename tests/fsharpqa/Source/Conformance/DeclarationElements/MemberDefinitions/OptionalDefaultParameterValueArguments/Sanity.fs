// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments 

open System.Runtime.InteropServices
open System

type Class() =
    // check that all known supported primitive types work.
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
    // Check a reference type - only null possible. We need to give this the correct type or our new compiler error triggers?
    static member Method14 ([<Optional;DefaultParameterValue(null:obj)>]i:obj) = i
    // Check a value type - only default ctor possible.
    static member Method15 ([<Optional;DefaultParameterValue(new DateTime())>]i:DateTime) = i

    // Check nullables
    static member MethodNullable1 ([<Optional;DefaultParameterValue(Nullable<int>())>]i:Nullable<int>) = i
    static member MethodNullable2 ([<Optional;DefaultParameterValue(Nullable<bool>())>]i:Nullable<bool>) = i
    static member MethodNullable3 ([<Optional;DefaultParameterValue(Nullable<DateTime>())>]i:Nullable<DateTime>) = i

    // Sanity checks with a mix of optional/non-optional parameters.
    static member Mix1(a:int, b:string, [<Optional;DefaultParameterValue(-12)>]c:int) = c
    // can omit optional in the middle of the arg list; this works in C# too.
    static member Mix2(a:int, b:string, [<Optional;DefaultParameterValue(-12)>]c:int, d: int) = c
    static member Mix3(a:int, [<Optional;DefaultParameterValue("str")>]b:string, 
                       [<Optional;DefaultParameterValue(-12)>]c:int, [<Optional;DefaultParameterValue(-123)>]d: int) = (b,c,d)

    // compiler should be able to figure out default to pass to Optional parameters without DefaultPaarameterValue.
    static member Optional1([<Optional>]a: int) = a
    static member Optional2([<Optional>]a: obj) = a
    static member Optional3([<Optional>]a: DateTime) = a
    static member Optional4([<Optional>]a: Nullable<int>) = a
    static member Optional5([<Optional>]a: string) = a

    // Insanity checks - basically make sure the compiler does not crash.
    static member OnlyDefault([<DefaultParameterValueAttribute(1)>]a: int) = a

let checkMethod (f:'a->'a) value (defaultFun:unit->'a) defaultValue =
    let result = f value
    if result <> value then printfn "normal case failed for type %s. Expected %A <> %A" typeof<'a>.Name value result; exit 1
    let result = defaultFun() 
    if defaultFun() <> defaultValue then printf "default case failed for type %s. Expected %A <> %A" typeof<'a>.Name defaultValue result; exit 1

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

do checkMethod (fun v -> Class.MethodNullable1(v)) (Nullable<_>(1))    (fun () -> Class.MethodNullable1()) (Nullable<_>())
do checkMethod (fun v -> Class.MethodNullable2(v)) (Nullable<_>(true))  (fun () -> Class.MethodNullable2()) (Nullable<_>())
do checkMethod (fun v -> Class.MethodNullable3(v)) (Nullable<_>(new DateTime(12300L)))  (fun () -> Class.MethodNullable3()) (Nullable<_>())

do checkMethod (fun v -> Class.Mix1(1, "1", v)) 100 (fun () -> Class.Mix1(2, "2")) -12
do checkMethod (fun v -> Class.Mix2(1, "1", v, 101)) 100 (fun () -> Class.Mix2(2, "2", d=12)) -12
do if (Class.Mix3(1, "1")) <> ("1",-12,-123) then exit 1
do if (Class.Mix3(1)) <> ("str",-12,-123) then exit 1
do if (Class.Mix3(1, c=1, b="123")) <> ("123",1,-123) then exit 1

do checkMethod (fun v -> Class.Optional1(v)) 1    (fun () -> Class.Optional1()) 0
do checkMethod (fun v -> Class.Optional2(v)) (new obj())  (fun () -> Class.Optional2()) (upcast System.Reflection.Missing.Value)
do checkMethod (fun v -> Class.Optional3(v)) DateTime.Now  (fun () -> Class.Optional3()) (DateTime())
do checkMethod (fun v -> Class.Optional4(v)) (Nullable<_>(12300))  (fun () -> Class.Optional4()) (Nullable<_>())
do checkMethod (fun v -> Class.Optional5(v)) "123"  (fun () -> Class.Optional5()) null

// can't omit the argument, but can still call the method.
do if Class.OnlyDefault(2) <> 2 then exit 1

exit 0
