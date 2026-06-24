namespace EmittedIL.RealInternalSignature

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DirectDelegates =

    let private coreOptions compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings

    // Default langversion: direct delegates must NOT be emitted (a closure is still generated).
    // Baselines: <source>.<suffix>.il.bsl  -- these should remain unchanged when the feature lands.
    let verifyCompilation compilation =
        compilation
        |> coreOptions
        |> compile
        |> shouldSucceed
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

    // Preview langversion: direct delegates ARE emitted once the feature lands.
    // Baselines: <source>.<suffix>.Preview.il.bsl  -- these are the ones that will change.
    let verifyPreviewCompilation compilation =
        compilation
        |> coreOptions
        |> withLangVersionPreview
        |> withPreviewBaseline
        |> compile
        |> shouldSucceed
        |> verifyILBaseline

    [<Theory; FileInlineData("DelegateKnownFunction.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateKnownFunction_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateKnownFunction.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateKnownFunction_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStaticMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStaticMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericStaticMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericStaticMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateInstanceMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateInstanceMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericInstanceMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericInstanceMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateUnitArg.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitArg_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateUnitArg.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitArg_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateNegativeCases.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNegativeCases_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateNegativeCases.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNegativeCases_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateNonInlinable.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNonInlinable_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateNonInlinable.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNonInlinable_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegatePartialApplication.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegatePartialApplication_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegatePartialApplication.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegatePartialApplication_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateUnitReturn.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitReturn_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateUnitReturn.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitReturn_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateStructTarget.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStructTarget_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateStructTarget.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStructTarget_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateExtensionMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateExtensionMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateExtensionMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateExtensionMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateILMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateILMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateILMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateILMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateCustomType.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateCustomType_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateCustomType.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
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
