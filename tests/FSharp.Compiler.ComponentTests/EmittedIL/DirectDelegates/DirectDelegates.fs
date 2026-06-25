module EmittedIL.RealInternalSignature.DirectDelegates

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let private coreOptions compilation =
    compilation
    |> withOptions [ "--test:EmitFeeFeeAs100001" ]
    |> asExe
    |> withEmbeddedPdb
    |> withEmbedAllSource
    |> ignoreWarnings

let verifyCompilation compilation =
    compilation
    |> coreOptions
    |> compile
    |> shouldSucceed
    |> verifyPEFileWithSystemDlls
    |> verifyILBaseline

// Redirect the IL baseline to a distinct *.Preview.il.bsl path so the preview variant can reuse the
// very same input .fs file (no input duplication / drift) without clobbering the default baseline.
let private withPreviewBaseline (cUnit: CompilationUnit) : CompilationUnit =
    match cUnit with
    | FS src ->
        let baseline =
            src.Baseline
            |> Option.map (fun bsl ->
                let path = bsl.ILBaseline.BslSource.Replace(".il.bsl", ".Preview.il.bsl")
                let content = if File.Exists path then Some(File.ReadAllText path) else None
                { bsl with ILBaseline = { bsl.ILBaseline with BslSource = path; Content = content } })
        FS { src with Baseline = baseline }
    | other -> other

let verifyPreviewCompilation compilation =
    compilation
    |> coreOptions
    |> withLangVersionPreview
    |> withPreviewBaseline
    |> compile
    |> shouldSucceed
    |> verifyPEFileWithSystemDlls
    |> verifyILBaseline

[<Theory; FileInlineData("DelegateKnownFunction.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateKnownFunction_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateKnownFunction.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateKnownFunction_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateStaticMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateStaticMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateStaticMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateStaticMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateGenericStaticMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateGenericStaticMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateInstanceMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateInstanceMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateInstanceMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateInstanceMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateGenericInstanceMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateGenericInstanceMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateUnitArg.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateUnitArg_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateUnitArg.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateUnitArg_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateNegativeCases.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateNegativeCases_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateNegativeCases.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateNegativeCases_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateNonInlinable.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateNonInlinable_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateNonInlinable.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateNonInlinable_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegatePartialApplication.fs", Optimize=BooleanOptions.Both)>]
let ``DelegatePartialApplication_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegatePartialApplication.fs", Optimize=BooleanOptions.Both)>]
let ``DelegatePartialApplication_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateUnitReturn.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateUnitReturn_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateUnitReturn.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateUnitReturn_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateStructTarget.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateStructTarget_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateStructTarget.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateStructTarget_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateExtensionMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateExtensionMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateExtensionMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateExtensionMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateILMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateILMethod_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateILMethod.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateILMethod_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Theory; FileInlineData("DelegateCustomType.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateCustomType_fs`` compilation =
    compilation |> getCompilation |> verifyCompilation

[<Theory; FileInlineData("DelegateCustomType.fs", Optimize=BooleanOptions.Both)>]
let ``DelegateCustomType_fs preview`` compilation =
    compilation |> getCompilation |> verifyPreviewCompilation

[<Fact>]
let ``Direct delegates target the real method and dispatch correctly (preview)`` () =
    FSharp """
module DirectDelegateExecution

open System

[<NoCompilerInlining>]
let add (x: int) (y: int) : int = x + y

type G<'U> =
    [<NoCompilerInlining>]
    static member Pick<'T>(x: 'T) (y: 'T) : 'T = x

[<AbstractClass>]
type Base() =
    abstract M: int -> int

type Derived() =
    inherit Base()
    override _.M x = x + 100

[<EntryPoint>]
let main _ =
    // Non-eta known function: the delegate points directly at 'add'.
    let d = Func<int, int, int>(add)
    if d.Invoke(2, 3) <> 5 then failwith "add: wrong result"
    if d.Method.Name <> "add" then failwithf "add: expected Method.Name 'add' but got '%s'" d.Method.Name

    // Non-eta generic method on a generic type: the delegate points directly at the fully instantiated method.
    let gd = Func<int, int, int>(G<string>.Pick<int>)
    if gd.Invoke(7, 9) <> 7 then failwithf "generic: expected 7 but got %d" (gd.Invoke(7, 9))
    if gd.Method.Name <> "Pick" then failwithf "generic: expected Method.Name 'Pick' but got '%s'" gd.Method.Name

    // Non-eta virtual instance method: dup; ldvirtftn must preserve override dispatch.
    let b: Base = Derived()
    let vd = Func<int, int>(b.M)
    if vd.Invoke 1 <> 101 then failwithf "virtual: expected 101 but got %d" (vd.Invoke 1)
    if vd.Method.Name <> "M" then failwithf "virtual: expected Method.Name 'M' but got '%s'" vd.Method.Name
    if not (obj.ReferenceEquals(vd.Target, b)) then failwith "virtual: Target is not the receiver"

    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Without the feature the delegate goes through a closure (default langversion)`` () =
    FSharp """
module ClosureDelegateExecution

open System

[<NoCompilerInlining>]
let add (x: int) (y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(add)
    if d.Invoke(2, 3) <> 5 then failwith "add: wrong result"
    // Without the feature the delegate is built over a generated closure method named 'Invoke'.
    if d.Method.Name <> "Invoke" then failwithf "expected closure Method.Name 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> compileExeAndRun
    |> shouldSucceed

// IL (BCL) method target: compiled as TOp.ILCall. With ILCall recognition the optimized eta-expanded
// delegate points directly at the BCL method, so Method.Name is the real method ('Max'), not a closure
// 'Invoke'. Compiled with --optimize+ so the eta forwarding call survives to codegen.
[<Fact>]
let ``IL method targets are emitted directly when optimized (preview)`` () =
    FSharp """
module IlMethodDelegate

open System

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(fun a b -> Math.Max(a, b))
    if d.Invoke(3, 7) <> 7 then failwith "il: wrong result"
    if d.Method.Name <> "Max" then failwithf "il: expected direct 'Max' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// A closure built from an explicit eta-lambda re-evaluates the receiver on every Invoke; a direct
// delegate would evaluate it once at construction. When the receiver has an effect (here a counter-
// bumping call) that difference is observable, so the closure must be kept even under optimization.
[<Fact>]
let ``Side-effecting receiver keeps the closure so it is re-evaluated per invoke (preview)`` () =
    FSharp """
module ReceiverEffectDelegate

open System

let mutable calls = 0

type Box(tag: int) =
    member _.Read (_: int) : int = tag

let getBox () =
    calls <- calls + 1
    Box(calls)

[<EntryPoint>]
let main _ =
    // The receiver 'getBox()' has an effect, so it must run on each invocation, not once at construction.
    let d = Func<int, int>(fun a -> (getBox()).Read a)
    let r1 = d.Invoke 0
    let r2 = d.Invoke 0
    if calls <> 2 then failwithf "receiver should be re-evaluated per invoke; calls=%d" calls
    if r1 = r2 then failwithf "expected distinct boxes per invoke but got %d and %d" r1 r2
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Instance IL method target: a BCL instance method bound directly. The delegate's Target must be the
// receiver and Method.Name the real method.
[<Fact>]
let ``Instance IL method targets are emitted directly when optimized (preview)`` () =
    FSharp """
module IlInstanceMethodDelegate

open System
open System.Text

[<EntryPoint>]
let main _ =
    let sb = StringBuilder()
    // StringBuilder.Append(string) is an instance method on a reference type.
    let d = Func<string, StringBuilder>(fun s -> sb.Append(s))
    d.Invoke "hello" |> ignore
    if sb.ToString() <> "hello" then failwith "il-instance: wrong result"
    if d.Method.Name <> "Append" then failwithf "il-instance: expected direct 'Append' but got '%s'" d.Method.Name
    if not (obj.ReferenceEquals(d.Target, sb)) then failwith "il-instance: Target is not the receiver"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Custom, F#-declared delegate types (not just BCL Func/Action) point directly at the target method.
// Non-eta targets, so direct in both debug and release. Covers a static target (null Target) and an
// instance target (Target = receiver) through a user-defined delegate.
[<Fact>]
let ``Custom F# delegate targets the real method and dispatch correctly (preview)`` () =
    FSharp """
module CustomDelegateExecution

open System

type DTupled = delegate of int * int -> int

[<NoCompilerInlining>]
let acc (x: int) (y: int) : int = x + y

type C() =
    [<NoCompilerInlining>]
    member _.M (x: int) (y: int) : int = x * y

[<EntryPoint>]
let main _ =
    let ds = DTupled(acc)
    if ds.Invoke(2, 3) <> 5 then failwith "static: wrong result"
    if ds.Method.Name <> "acc" then failwithf "static: expected 'acc' but got '%s'" ds.Method.Name
    if not (isNull ds.Target) then failwith "static: Target should be null"

    let c = C()
    let di = DTupled(c.M)
    if di.Invoke(4, 5) <> 20 then failwith "instance: wrong result"
    if di.Method.Name <> "M" then failwithf "instance: expected 'M' but got '%s'" di.Method.Name
    if not (obj.ReferenceEquals(di.Target, c)) then failwith "instance: Target is not the receiver"
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Cases 33-38: a tupled application forwards a single tuple, not the Invoke parameters verbatim, so the
// shape is not recognized and a closure is kept even in release.
[<Fact>]
let ``Tupled application stays a closure (preview)`` () =
    FSharp """
module TupledClosure

open System

[<NoCompilerInlining>]
let accT (x: int, y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(fun a b -> accT (a, b))
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Cases 39-43: a partial application leaves the target with more parameters than the delegate's Invoke,
// so there is no closed direct form and a closure is kept.
[<Fact>]
let ``Partial application stays a closure (preview)`` () =
    FSharp """
module PartialClosure

open System

[<NoCompilerInlining>]
let add3 (x: int) (y: int) (z: int) : int = x + y + z

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(add3 1)
    if d.Invoke(2, 3) <> 6 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Cases 48-49: the synthesized unit argument is seen as a leading argument for a static target, so a
// unit-argument delegate stays a closure (and still runs).
[<Fact>]
let ``Unit-argument delegate stays a closure (preview)`` () =
    FSharp """
module UnitArgClosure

open System

let mutable ran = false
let handler () : unit = ran <- true

[<EntryPoint>]
let main _ =
    let d = Action(handler)
    d.Invoke()
    if not ran then failwith "did not run"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Cases 50-51: a value-type receiver would need boxing (a delegate Target is object); not implemented, so
// it stays a closure and still dispatches correctly.
[<Fact>]
let ``Struct value-type receiver stays a closure (preview)`` () =
    FSharp """
module StructClosure

open System

[<Struct>]
type S =
    member _.Add (x: int) (y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let s = S()
    let d = Func<int, int, int>(s.Add)
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Case 52: an extension member compiles to a static method whose first parameter is the receiver, so it is
// bound as a leading argument (a partial application) and stays a closure.
[<Fact>]
let ``Extension member stays a closure (preview)`` () =
    FSharp """
module ExtensionClosure

open System
open System.Runtime.CompilerServices

type Holder() = class end

[<Extension>]
type HolderExtensions =
    [<Extension>]
    static member Combine (h: Holder, x: int, y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let h = Holder()
    let d = Func<int, int, int>(fun a b -> h.Combine(a, b))
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Case 53: a byref Invoke parameter with a mutating body is not a transparent forwarding call, so it stays
// a closure and mutates through the byref correctly.
[<Fact>]
let ``Byref-parameter delegate stays a closure and mutates (preview)`` () =
    FSharp """
module ByrefClosure

open System

type D = delegate of byref<int> -> unit

[<EntryPoint>]
let main _ =
    let d = D(fun x -> x <- x + 1)
    let mutable v = 10
    d.Invoke(&v)
    if v <> 11 then failwithf "expected 11 but got %d" v
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed
