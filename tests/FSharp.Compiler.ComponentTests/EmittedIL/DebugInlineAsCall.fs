namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module DebugInlineAsCall =

    [<Fact>]
    let ``Call 01 - Release`` () =
        FSharp """
let inline f (x: int) =
    x + x

let i = f 5
"""
        |> asExe
        |> compile
        |> verifyILContains ["ldc.i4.s   10"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 02 - Debug`` () =
        FSharp """
let inline f (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = f 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::f(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 03 - Two args`` () =
        FSharp """

let inline add a b =
    a + b

[<EntryPoint>]
let main _ =
    let i = add 1 2
    if i = 3 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@8'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 04 - Function arg`` () =
        FSharp """
let inline apply (f: 'a -> 'b -> 'c) (x: 'a) (y: 'b) : 'c =
    f x y

[<EntryPoint>]
let main _ =
    let i = apply (+) 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       !!2 Test::apply<int32,int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>,"]
        |> shouldSucceed


    [<Fact>]
    let ``Call 05 - Nested inline`` () =
        FSharp """
let inline double (x: int) =
    x + x

let inline quadruple (x: int) =
    double (double x)

[<EntryPoint>]
let main _ =
    let i = quadruple 3
    if i = 12 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            ["call       int32 Test::double(int32)"
             "call       int32 Test::quadruple(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 06 - Multiple calls`` () =
        FSharp """
let inline double (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = double 1 + double 2
    if i = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains [ "call       int32 Test::double(int32)" ]

    [<Fact>]
    let ``Call 07 - Local function`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline double (x: int) = x + x
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 08 - Local generic function`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline apply (f: 'a -> 'b) (x: 'a) : 'b = f x
    let i = apply (fun x -> x + 1) 5
    if i = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,int32>::InvokeFast<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 09 - FSharp.Core not`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let b = not true
    if b = false then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILNotPresent ["call       bool [FSharp.Core]Microsoft.FSharp.Core.Operators::Not(bool)"]

    [<Fact>]
    let ``Call 10 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline triple (x: int) = x + x + x
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary
            |> withName "Lib"

        FSharp """
open MyLib

[<EntryPoint>]
let main _ =
    let i = triple 3
    if i = 9 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 [Lib]MyLib::triple(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 11 - Measure`` () =
        FSharp """
[<Measure>] type cm

let inline scale (x: float<'u>) = x * 2.0

[<EntryPoint>]
let main _ =
    let v = scale 5.0<cm>
    if v = 10.0<cm> then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       float64 Test::scale(float64)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 12 - No inner optimization`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline f (x: int) =
        let i = 5 + 10
        x + i

    let i = f 20
    if i = 35 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "ldc.i4.5"
              "ldc.i4.s   10"
              "callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)" ]
        |> shouldSucceed

    [<Fact>]
    let ``Call 13`` () =
        FSharp """
[<NoDynamicInvocation>]
let inline f x = x + 1

let inline g x = f x

g 1 |> ignore
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun

    [<Fact>]
    let ``Call 14`` () =
        FSharp """
[<NoDynamicInvocation>]
let inline f x = x + 1

let inline g (x: ^T) (y: ^T) = f (x + y)

g 1 2 |> ignore
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun

    [<Fact>]
    let ``Call 15`` () =
        FSharp """
[<NoDynamicInvocation>]
let inline f (x: ^T) = x

let inline g (x: ^T) (y: ^T) = f (x + y)

g 1 2 |> ignore
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun

    [<Fact>]
    let ``SRTP 01`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) =
    x + y

[<EntryPoint>]
let main _ =
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@7'(int32,"]
        |> shouldSucceed
    
    [<Fact>]
    let ``SRTP 02 - Local `` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline add (x: ^T) (y: ^T) = x + y
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@5'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 03 - Different type arguments`` () =
        FSharp """
let inline getLength (x: ^T) =
    (^T : (member Length : int) x)

[<EntryPoint>]
let main _ =
    let i = getLength "hello"
    let j = getLength [1; 2; 3]
    if i = 5 && j = 3 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<getLength>__debug@7'(string)"
              "call       int32 Test::'<getLength>__debug@8-1'(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)" ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 04 - Multiple calls`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let i = add 1 2 + add 3 4
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<add>__debug@6'(int32,"
              "call       int32 Test::'<add>__debug@6-1'(int32," ]
        |> shouldSucceed
    

    [<Fact>]
    let ``SRTP 05 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline add (x: ^T) (y: ^T) = x + y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open MyLib

[<EntryPoint>]
let main _ =
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@6'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 06 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline add (x: ^T) (y: ^T) = x + y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open MyLib

let inline double (x: ^T) = add x x

[<EntryPoint>]
let main _ =
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<double>__debug@8'(int32)"
              "call       int32 Test::'<add>__debug@4'(int32," ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 07 - Nested - Same project`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

let inline double (x: ^T) = add x x

[<EntryPoint>]
let main _ =
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<double>__debug@8'(int32)"
              "call       int32 Test::'<add>__debug@4'(int32," ]
        |> shouldSucceed

    

    [<Fact>]
    let ``SRTP 08 - Nested - Different type arguments`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

let inline addBoth (x: ^A) (y: ^B) =
    let a = add x x
    let b = add y y
    (a, b)

[<EntryPoint>]
let main _ =
    let (a, b) = addBoth 2 3.0
    if a = 4 && b = 6.0 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<add>__debug@5'(int32,"
              "call       float64 Test::'<add>__debug@6-1'(float64," ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 09 - Witness`` () =
        FSharp """
let check s (b1: 'a) (b2: 'a) = if b1 = b2 then () else failwith s

let inline add (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    check "int" (add 3 4) 7
    check "float" (add 1.0 2.0) 3.0
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 10 - Witness`` () =
        FSharp """
type MyNum =
    { Value: float }
    static member FromFloat (_: MyNum) = fun (x: float) -> { Value = x }

type T =
    static member inline Invoke(x: float) : 'Num =
        let inline call (a: ^a) = (^a: (static member FromFloat : _ -> _) a)
        call Unchecked.defaultof<'Num> x

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(3.14)
    if result.Value = 3.14 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains [ "call       class Test/MyNum Test::'<Invoke>__debug@13'(float64)" ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 11 - Witness`` () =
        FSharp """
type MyNum =
    { Value: float }
    static member FromFloat (_: MyNum, _: T) = fun (x: float) -> { Value = x }

and T =
    { Dummy: int }
    static member inline Invoke(x: float) : 'Num =
        let inline call2 (a: ^a, b: ^b) = ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
        let inline call (a: 'a) = fun (x: 'x) -> call2 (a, Unchecked.defaultof<'r>) x : 'r
        call Unchecked.defaultof<T> x

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(2.71)
    if result.Value = 2.71 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains [ "call       class Test/MyNum Test::'<Invoke>__debug@15'(float64)" ]
        |> shouldSucceed


    [<Fact>]
    let ``SRTP 12 - Witness - Struct with partially resolved type args`` () =
        FSharp """
[<Struct>]
type S =
    member _.M() = ()

type T() =
    member _.N() = ()

let inline f (a: ^A when ^A: (member M: unit -> unit)) (_b: ^B when ^B: (member N: unit -> unit)) =
    (^A: (member M: unit -> unit) a)

let inline g b = f (S()) b

[<EntryPoint>]
let main _ =
    g (T())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 13 - Witness - Struct with ResumableCode and partially resolved type args`` () =
        FSharp """
open Microsoft.FSharp.Core.CompilerServices

[<Struct>]
type S = member this.Foo() = ()

type D = member _.Bar() = true

let inline f (x: ^A) =
    ResumableCode< ^B, _>(fun sm ->
        (^A: (member Foo: unit -> unit) x)
        (^B: (member Bar: unit -> bool) sm.Data)
    )

let inline g () = f (S())

[<EntryPoint>]
let main _ =
    let _ : ResumableCode<D, _> = g ()
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 14 - StateMachine with unresolved trait from composed inline function`` () =
        FSharp """
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline f<'A, 'R, 'B when 'A: (member M: int -> 'R) and 'B: (member M: 'R -> unit)> (_a: 'A) : 'B = Unchecked.defaultof<_>

let inline g (_: S<'T>) =
    if __useResumableCode then
        __stateMachine<S<'T>, int>
            (MoveNextMethodImpl<_>(fun _ -> ()))
            (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
            (AfterCode<_, _>(fun _ -> 0))
    else 0

let inline h a = g (f a)

[<EntryPoint>]
let main _ =
    let _ = h (S<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 15 - Composed inline with linked constraints`` () =
        FSharp """
[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline f<'A, 'R, 'B when 'A: (member M: int -> 'R) and 'B: (member M: 'R -> unit)> (_a: 'A) : 'B = Unchecked.defaultof<_>

let inline g (_: S<'T>) = 42

let inline h a = g (f a)

[<EntryPoint>]
let main _ =
    let i = h (S<int>())
    if i = 42 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 16 - Same line`` () =
        let additionalSource = FsSourceWithFileName "Program.fs" """
module Program

open Module

let inline foo (a: 'a) = U.F(a, 0); fun () -> ()

[<EntryPoint>]
let main _ =
    let _ = foo (T())
    0
"""
        FSharpWithFileName "Module.fs" """
module Module

type T() = member _.M() = ()

type U = static member inline F<'a, 'b when 'a: (member M: unit -> unit)>(_a: 'a, _b: 'b) = ()
"""
        |> withAdditionalSourceFile additionalSource
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 17 - Same line`` () =
        let library =
            FSharp """
module Module

let inline add<'a, 'b, 'c when 'a: (static member (+): 'a * 'a -> 'b)> (x: 'a) (y: 'a) =
    x + y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open Module

let inline foo x y = add x y |> ignore; fun () -> ()

[<EntryPoint>]
let main _ =
    let _ = foo 1 2
    0
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 18 - Type abbreviation with constraint`` () =
        let additionalSource = FsSourceWithFileName "Program.fs" """
module Program

open Module

type T() = member _.M() = 42

let inline foo (a: 'a) = C.F(a); fun () -> ()

[<EntryPoint>]
let main _ =
    let _ = foo (T())
    0
"""
        FSharpWithFileName "Module.fs" """
module Module

type C<'a, 'b when 'a: (member M: unit -> 'b)> = 'a

type C = static member inline F<'a, 'b, 'c when C<'a, 'b>>(_a: 'a) = ()
"""
        |> withAdditionalSourceFile additionalSource
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 19 - byref`` () =
        FSharp """
let inline f<'T, 'U when 'T: (member M: byref<'U> -> unit)> (x: byref<'T>, y: byref<'U>) =
    x.M(&y)

[<Struct>]
type S =
    member _.M(x: byref<int>) = x <- 42

[<EntryPoint>]
let main _ =
    let mutable s = S()
    let mutable v = 0
    f(&s, &v)
    if v = 42 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 20 - byref`` () =
        let library =
            FSharp """
module MyLib

let inline f<'T, 'U when 'T: (member M: byref<'U> -> unit)> (x: byref<'T>, y: byref<'U>) =
    x.M(&y)
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open MyLib

[<Struct>]
type S =
    member _.M(x: byref<int>) = x <- 42

[<EntryPoint>]
let main _ =
    let mutable s = S()
    let mutable v = 0
    f(&s, &v)
    if v = 42 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 21 - byref`` () =
        let library =
            FSharp """
module MyLib

let inline f<'T, 'U when 'T: (member M: byref<'U> -> unit)> (x: byref<'T>, y: byref<'U>) =
    x.M(&y)

let inline g<'T, 'U when 'T: (member M: byref<'U> -> unit)> (x: byref<'T>, y: byref<'U>) =
    f(&x, &y)
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open MyLib

[<Struct>]
type S =
    member _.M(x: byref<int>) = x <- 42

[<EntryPoint>]
let main _ =
    let mutable s = S()
    let mutable v = 0
    g(&s, &v)
    if v = 42 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Member 01 - Non-generic`` () =
        FSharp """
type T() =
    member inline _.Double(x: int) = x + x

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Double(5)
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["callvirt   instance int32 Test/T::Double(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Member 02 - Generic`` () =
        FSharp """
type T() =
    member inline _.Apply(f: 'a -> 'b, x: 'a) : 'b = f x

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Apply((fun x -> x + 1), 5)
    if i = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["callvirt   instance !!1 Test/T::Apply<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,"]
        |> shouldSucceed

    [<Fact>]
    let ``Member 03 - SRTP`` () =
        FSharp """
type T() =
    member inline _.Add(x: ^T, y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Add(3, 4)
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<Add>__debug@8'(class Test/T,"]
        |> shouldSucceed

    [<Fact>]
    let ``Operator 01 - Top-level`` () =
        FSharp """
let inline (++) (x: int) (y: int) = x + y + 1

[<EntryPoint>]
let main _ =
    let i = 3 ++ 4
    if i = 8 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::op_PlusPlus(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``Operator 02 - Top-level SRTP`` () =
        FSharp """
let inline (++) (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let i = 3 ++ 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<op_PlusPlus>__debug@6'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``Operator 03 - Local`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline (++) (x: int) (y: int) = x + y + 1
    let i = 3 ++ 4
    if i = 8 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::InvokeFast<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,"]
        |> shouldSucceed

    [<Fact>]
    let ``Operator 04 - Local SRTP`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline (++) (x: ^T) (y: ^T) = x + y
    let i = 3 ++ 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<op_PlusPlus>__debug@5'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 01`` () =
        FSharp """
module Module

let inline internal fInternal () = ()
let inline f () = fInternal ()
"""
        |> withDebug
        |> withNoOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 02`` () =
        FSharp """
module Module

type T() =
    member inline internal this.InternalMethod() =
        ()

    member inline this.Method() =
        this.InternalMethod()
"""
        |> withDebug
        |> withNoOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 03`` () =
        let library =
            FSharp """
module Lib

type T() =
    member inline internal _.F(x) = x
    member inline this.G(x) = this.F(x)
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
module App
let r = Lib.T().G(1)
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> compile
        |> shouldSucceed


    [<Fact>]
    let ``Accessibility 04`` () =
        let library =
            FSharp """
module MyLib

let inline internal addInternal (x: ^T) (y: ^T) = x + y
let inline addPublic (x: ^T) (y: ^T) = addInternal x y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open MyLib

[<EntryPoint>]
let main _ =
    let i = addPublic 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 05`` () =
        let library =
            FSharp """
module Module

type MyNum =
    { Value: float }
    static member FromFloat (_: MyNum) = fun (x: float) -> { Value = x }

type T =
    static member inline internal InvokeInternal(x: float) : 'Num =
        let inline call (a: ^a) = (^a: (static member FromFloat : _ -> _) a)
        call Unchecked.defaultof<'Num> x

    static member inline Invoke(x: float) : 'Num =
        T.InvokeInternal<'Num>(x)
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open Module

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(3.14)
    if result.Value = 3.14 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 06`` () =
        let impl =
            FsSource """
module Lib

module internal Impl =
    let inline implFn (x: int) =
        x * x

let inline publicFn (x: int) =
    Impl.implFn x + 1
"""
        let fsi = Fsi """
module Lib

val inline publicFn: x: int -> int
"""
        let library =
            fsi
            |> withAdditionalSourceFile impl
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open Lib

[<EntryPoint>]
let main _ =
    let i = publicFn 3
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Accessibility 07`` () =
        let impl =
            FsSource """
module Lib

module Impl =
    let inline implFn (x: int) =
        x * x

let inline publicFn (x: int) =
    Impl.implFn x + 1
"""
        let fsi = Fsi """
module Lib

val inline publicFn: x: int -> int
"""
        let library =
            fsi
            |> withAdditionalSourceFile impl
            |> withDebug
            |> withNoOptimize
            |> asLibrary

        FSharp """
open Lib

[<EntryPoint>]
let main _ =
    let i = publicFn 3
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
