
open ConsumeFromCS
open System.Runtime.InteropServices
open System

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