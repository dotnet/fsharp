namespace EmittedIL

open System.Diagnostics
open System.Runtime.CompilerServices
open Xunit
open FSharp.Test.Compiler

module DebugInlineAsCall =

    let private baseline = SequencePointsBaseline(__SOURCE_DIRECTORY__)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private verifySequencePoints result =
        baseline.verifyResult(StackTrace().GetFrame(1), result)

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

    [<Fact>]
    let ``Call 16 - Debug lib called from optimized app`` () =
        let lib =
            FSharp """
module Module

let inline double (x: int) = x + x

let inline quadruple (x: int) = double (double x)
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary
            |> withName "Lib"

        lib
            |> compile
            |> verifyILContains ["call       int32 Module::double(int32)"]
            |> shouldSucceed
            |> ignore

        FSharp """
open Module

[<EntryPoint>]
let main (args: string[]) =
    let i = quadruple args.Length
    i
"""
        |> withOptimize
        |> withReferences [lib]
        |> asExe
        |> compile
        |> verifyILContains ["add"] |> shouldSucceed
        |> verifyILNotPresent ["quadruple"; "double"]

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
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 02 - Local`` () =
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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 22 - Recursive inline with different type arg`` () =
        FSharp """
type T = T with
    static member ($) (T, _:int) = (+)
    static member ($) (T, _:decimal) = (+)

let inline sum (i:'a) (x:'a) :'r = (T $ Unchecked.defaultof<'r>) i x

type T with
    static member inline ($) (T, _:'t -> 'rest) = fun (a:'t) x -> sum (x + a)

[<EntryPoint>]
let main _ =
    let y:int = sum 2 3 4
    if y = 9 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 23 - Cross-file specialization with nested closures at same line`` () =
        // Two specializations in wrap's body each contain nested closures whose ranges are in the
        // other file. Without bucketing the closure-name counter by the enclosing type's file, the
        // two nested closures end up with identical names ('wrap@12-1') and ilwrite rejects them
        // with FS2014 duplicate entry in type index table.
        let additionalSource = FsSourceWithFileName "Program.fs" """
module Program
open Module

let inline wrap source =
    let mutable state = false
    monad' {
        match state with
        | true -> return false
        | _ ->
            let! _ = source
            state <- true
            return true }
"""
        FSharpWithFileName "Module.fs" """
module Module
type Bind =
    static member inline Invoke (s: 'M) (b: 'T -> 'U) =
        ((^M or ^U) : (static member (>>=) : _*_ -> _) s, b)

type Return =
    static member Return (_: 'T list, _: Return) : 'T -> 'T list = Unchecked.defaultof<_>
    static member inline Invoke (x: 'T) =
        (^A : (static member Return : 'T -> ^A) x)

type MonadBuilder () =
    member inline _.Return x = Return.Invoke x
    member inline _.Bind (p, r) = Bind.Invoke p r
let monad' = MonadBuilder ()
"""
        |> withAdditionalSourceFile additionalSource
        |> withDebug
        |> withNoOptimize
        |> asLibrary
        |> compile
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 24 - byref with free typar at callsite`` () =
        FSharp """
type MyBuilder<'T>() =
    member _.M(a: byref<int>, b: byref<int>) = ()

let inline callMember<'Builder, 'A
    when 'Builder: (member M: byref<'A> * byref<'A> -> unit)>
    (builder: 'Builder, a: byref<'A>) =
    builder.M(&a, &a)

let runDynamic (builder: MyBuilder<'T>) =
    let mutable x = 0
    callMember (builder, &x)

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 25 - byref with free typar at callsite - large tuple`` () =
        // F# represents tuples with 8+ elements as nested System.Tuple. When building a closure
        // for such a call the compiler would otherwise pack args into a reference Tuple shape
        // that cannot contain byrefs. The debug-call path must flatten to a method regardless
        // of arity.
        FSharp """
type MyBuilder<'T>() =
    member _.M(a1: byref<int>, a2: byref<int>, a3: byref<int>,
               a4: byref<int>, a5: byref<int>, a6: byref<int>,
               a7: byref<int>, a8: byref<int>) =
        a1 <- 1

let inline callMember<'Builder, 'A
    when 'Builder: (member M: byref<'A> * byref<'A> * byref<'A> * byref<'A>
                            * byref<'A> * byref<'A> * byref<'A> * byref<'A> -> unit)>
    (builder: 'Builder,
     a1: byref<'A>, a2: byref<'A>, a3: byref<'A>, a4: byref<'A>,
     a5: byref<'A>, a6: byref<'A>, a7: byref<'A>, a8: byref<'A>) =
    builder.M(&a1, &a2, &a3, &a4, &a5, &a6, &a7, &a8)

let runDynamic (builder: MyBuilder<'T>) =
    let mutable x = 0
    callMember (builder, &x, &x, &x, &x, &x, &x, &x, &x)

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 26 - byref with free typar at callsite - three tupled args`` () =
        FSharp """
type MyBuilder<'T>() =
    member _.M(a: byref<int>, b: byref<int>) = ()

let inline callMember<'Builder, 'A
    when 'Builder: (member M: byref<'A> * byref<'A> -> unit)>
    (builder: 'Builder, a: byref<'A>, b: byref<'A>) =
    builder.M(&a, &b)

let runDynamic (builder: MyBuilder<'T>) =
    let mutable x = 0
    callMember (builder, &x, &x)

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 27 - non-typar tyarg to SRTP callee - non-inline caller`` () =
        // Non-typar tyargs (MyBuilder<'T>) cannot satisfy the callee's SRTP via witness
        // propagation; they must go through the specialization path so the trait resolves
        // statically to MyBuilder<T>.M.
        FSharp """
type MyBuilder<'T>() =
    member _.M() = ()

let inline callMember<'Builder when 'Builder: (member M: unit -> unit)> (builder: 'Builder) =
    builder.M()

let runDynamic (builder: MyBuilder<'T>) =
    callMember builder

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 28 - non-typar tyarg to SRTP callee - inline caller`` () =
        // Same pattern as SRTP 27 but the caller is itself inline. The non-$W variant of the
        // inline caller would otherwise contain a direct call to the SRTP callee's non-$W
        // stub and throw at runtime when invoked from non-inline code.
        FSharp """
type MyBuilder<'T>() =
    member _.M() = ()

let inline callMember<'Builder when 'Builder: (member M: unit -> unit)> (builder: 'Builder) =
    builder.M()

let inline outerInline<'T> (builder: MyBuilder<'T>) =
    callMember builder

[<EntryPoint>]
let main _ =
    outerInline (MyBuilder<int>())
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``SRTP 29 - byref callee under SRTP witnesses, nested in delegate bodies`` () =
        // The specialized callee shape has byref in a tupled-arg position and the enclosing
        // scope needs a trait witness. Neither a byref-bearing closure (invalid System.Tuple
        // instantiation) nor a non-$W stub method works here, so the optimizer must leave the
        // call as-is and let IlxGen's $W rewriting pick up the witness from the inline caller.
        FSharp """
type Aw(value: int) =
    member _.GetResult() = value

type Code<'T> = delegate of byref<'T> -> unit

let inline bindDynamic<'TResult1, 'TOverall, 'Awaiter
    when 'Awaiter: (member GetResult: unit -> 'TResult1)>
    (d: byref<'TOverall>, awaiter: 'Awaiter, continuation: 'TResult1 -> Code<'TOverall>) =
    (continuation (awaiter.GetResult())).Invoke(&d)

let inline wrap<'T1, 'T2, 'Awaiter1, 'Awaiter2
    when 'Awaiter1: (member GetResult: unit -> 'T1)
    and 'Awaiter2: (member GetResult: unit -> 'T2)>
    (left: 'Awaiter1) (right: 'Awaiter2) : Code<'T1 * 'T2> =
    Code<'T1 * 'T2>(fun d ->
        bindDynamic (&d, left, fun leftR ->
            Code<'T1 * 'T2>(fun d2 ->
                bindDynamic (&d2, right, fun rightR ->
                    Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))))

[<EntryPoint>]
let main _ =
    let code = wrap (Aw(1)) (Aw(2))
    let mutable result = (0, 0)
    code.Invoke(&result)
    printfn "Result = %A" result
    0
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
        |> verifySequencePoints

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
            |> withName "Lib"

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
        |> verifySequencePoints

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
            |> withName "Lib"

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
        |> verifySequencePoints

    [<Fact>]
    let ``Accessibility 08`` () =
        FSharp """
module Module

type T() =
    static member private PrivateMethod() = 1
    static member inline private InlinePrivateMethod() = T.PrivateMethod()
    static member inline PublicMethod() = T.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T.PublicMethod() = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 1113

    [<Fact>]
    let ``Accessibility 09`` () =
        FSharp """
module Module

type T() =
    member private this.PrivateMethod() = 1
    member inline private this.InlinePrivateMethod() = this.PrivateMethod()
    member inline this.PublicMethod() = this.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T().PublicMethod() = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 1113

    [<Fact>]
    let ``Accessibility 10`` () =
        FSharp """
module Module

type T() =
    static member internal InternalMethod() = 1
    static member inline internal InlineInternalMethod() = T.InternalMethod()
    static member inline PublicMethod() = T.InlineInternalMethod()

[<EntryPoint>]
let main _ =
    if T.PublicMethod() = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 1113

    [<Fact>]
    let ``Accessibility 11`` () =
        FSharp """
module Module

type T() =
    static let i = 1
    static member inline private InlinePrivateMethod() = i
    static member inline PublicMethod() = T.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T.PublicMethod() = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``Accessibility 12`` () =
        FSharp """
module Module

type T() =
    let i = 1
    member inline private this.InlinePrivateMethod() = i
    member inline this.PublicMethod() = this.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T().PublicMethod() = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``Resumable 01`` () =
        FSharp """
open System.Threading.Tasks

[<EntryPoint>]
let main _ =
    let t = task { return 1 }
    if t.Result = 1 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``Resumable 02`` () =
        FSharp """
open System.Threading.Tasks

[<EntryPoint>]
let main _ =
    let t = task {
        let! x = Task.FromResult(1)
        let! y = Task.FromResult(2)
        return x + y
    }
    if t.Result = 3 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``Resumable 03`` () =
        FSharp """
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline g (_: S<'T>) =
    if __useResumableCode then
        __stateMachine<S<'T>, int>
            (MoveNextMethodImpl<_>(fun _ -> ()))
            (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
            (AfterCode<_, _>(fun _ -> 42))
    else 42

[<EntryPoint>]
let main _ =
    let r = g (S<int>())
    if r = 42 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``InlineIfLambda 01 - Debug`` () =
        FSharp """
let inline apply ([<InlineIfLambda>] f: int -> int) (x: int) : int =
    f x

[<EntryPoint>]
let main _ =
    let result = apply (fun i -> i + 1) 5
    if result = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifySequencePoints

    [<Fact>]
    let ``InlineIfLambda 02 - Release`` () =
        FSharp """
let inline apply ([<InlineIfLambda>] f: int -> int) (x: int) : int =
    f x

[<EntryPoint>]
let main _ =
    let result = apply (fun i -> i + 1) 5
    if result = 6 then 0 else 1
"""
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent ["call       int32 Test::apply(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,"]

