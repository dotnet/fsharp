module EmittedIL.RealInternalSignature.DirectDelegates

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.ProjectGeneration

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

let add (x: int) (y: int) : int = x + y

type G<'U> =
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

let acc (x: int) (y: int) : int = x + y

type C() =
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

// Cases 31-35: a tupled application carries each tupled group as a single tuple node, exactly the shape the
// code generator de-tuples by the target's arity when it emits the call. The recognizer de-tuples the same
// way, so a tupled target is as direct-able as its curried counterpart and points at the real method.
[<Fact>]
let ``Tupled application targets the real method (preview)`` () =
    FSharp """
module TupledDirect

open System

let accT (x: int, y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(fun a b -> accT (a, b))
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "accT" then failwithf "expected direct 'accT' but got '%s'" d.Method.Name
    if not (isNull d.Target) then failwith "Target should be null for a static target"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Cases 37-41: the CLR's closed delegate binds exactly one leading argument as the Target, so a partial
// application that fixes two or more arguments (or also fixes a receiver) has no closed direct form and stays
// a closure. A one-argument partial application could be closed, but only if that argument is a reference type
// (a value-type Target would need boxing - the same gap as a value-type receiver), so fixing a value-type
// argument keeps a closure too.
[<Fact>]
let ``Partial application stays a closure (preview)`` () =
    FSharp """
module PartialClosure

open System

let add3 (x: int) (y: int) (z: int) : int = x + y + z

[<EntryPoint>]
let main _ =
    // One fixed argument, but it is a value type: a value-type Target would need boxing, so a closure is kept.
    let d = Func<int, int, int>(add3 1)
    if d.Invoke(2, 3) <> 6 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// A one-argument partial application whose fixed argument is a reference type is expressible as a closed
// delegate: the argument is bound as the Target and the delegate points directly at the static method.
[<Fact>]
let ``Reference-type single-argument partial application is direct (preview)`` () =
    FSharp """
module PartialDirect

open System

let prepend (prefix: string) (x: int) (y: int) : string = sprintf "%s%d%d" prefix x y

[<EntryPoint>]
let main _ =
    let p = "p"
    let d = Func<int, int, string>(prepend p)
    if d.Invoke(2, 3) <> "p23" then failwith "wrong result"
    if d.Method.Name <> "prepend" then failwithf "expected direct 'prepend' but got '%s'" d.Method.Name
    if not (obj.ReferenceEquals(d.Target, p)) then failwith "Target is not the fixed argument"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Cases 46-49: the forwarded unit argument is stripped, so a unit-argument delegate points directly at the
// target - a static target carries a null Target, an instance target carries the receiver.
[<Fact>]
let ``Unit-argument delegate targets the real method (preview)`` () =
    FSharp """
module UnitArgDirect

open System

let mutable ran = 0

let handler () : unit = ran <- ran + 1

type C() =
    member _.M () : unit = ran <- ran + 10

[<EntryPoint>]
let main _ =
    // Static unit-argument target: direct, null Target, real Method.Name.
    let ds = Action(handler)
    ds.Invoke()
    if ds.Method.Name <> "handler" then failwithf "static: expected 'handler' but got '%s'" ds.Method.Name
    if not (isNull ds.Target) then failwith "static: Target should be null"

    // Instance unit-argument target: direct, Target is the receiver.
    let c = C()
    let di = Action(c.M)
    di.Invoke()
    if di.Method.Name <> "M" then failwithf "instance: expected 'M' but got '%s'" di.Method.Name
    if not (obj.ReferenceEquals(di.Target, c)) then failwith "instance: Target is not the receiver"

    if ran <> 11 then failwithf "expected both targets to run (ran=%d)" ran
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Cases 50-51: a value-type receiver is boxed (a copy) and stored as the delegate's Target; the runtime binds
// the unboxing stub, so the delegate points at the real struct method and dispatches correctly, with the boxed
// copy carrying the receiver's value. The receiver must be effect-free, so it comes from a (non-mutable)
// parameter here. (By-value capture cannot be observed via external mutation on a *direct* struct delegate: a
// mutable receiver - or the defensive copy it forces - reads a mutable value, which counts as an effect, so it
// is kept as a closure instead. The boxing itself guarantees the by-value copy.)
[<Fact>]
let ``Struct value-type receiver targets the real method (preview)`` () =
    FSharp """
module StructDirect

open System

[<Struct>]
type S =
    val V : int
    new (v: int) = { V = v }
    member this.AddV (x: int) (y: int) : int = this.V + x + y
    
let makeAdder (s: S) = Func<int, int, int>(s.AddV)

[<EntryPoint>]
let main _ =
    let d = makeAdder (S(100))
    if d.Invoke(2, 3) <> 105 then failwithf "wrong result: %d" (d.Invoke(2, 3))
    if d.Method.Name <> "AddV" then failwithf "expected direct 'AddV' but got '%s'" d.Method.Name
    if isNull d.Target then failwith "Target should be the boxed receiver, not null"
    if not (d.Target :? S) then failwith "Target should be a boxed S"
    0
        """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// Case 52: an extension member compiles to a static method whose first parameter is the receiver. The CLR's
// "closed over the first argument" delegate binds that receiver as the Target, so the delegate points directly
// at the static extension method (in release, where the eta-lambda does not need to survive for debugging).
[<Fact>]
let ``Extension member targets the real method (preview)`` () =
    FSharp """
module ExtensionDirect

open System
open System.Runtime.CompilerServices

type Holder() = class end

[<Extension>]
type Extensions =
    [<Extension>]
    static member Combine (h: Holder, x: int, y: int) : int = x + y

[<EntryPoint>]
let main _ =
    let h = Holder()
    let d = Func<int, int, int>(fun a b -> h.Combine(a, b))
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "Combine" then failwithf "expected direct 'Combine' but got '%s'" d.Method.Name
    if not (obj.ReferenceEquals(d.Target, h)) then failwith "Target is not the extension receiver"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// A generic extension member whose receiver type uses the method's type parameter ('T list) still binds the
// receiver as the Target: the type argument is threaded through as a method instantiation (an extension member
// has no enclosing type arguments), and the receiver - a reference type - is the closed-over first argument.
[<Fact>]
let ``Generic extension member receiver targets the real method (preview)`` () =
    FSharp """
module GenericExtensionDirect

open System
open System.Runtime.CompilerServices

[<Extension>]
type ListExtensions =
    [<Extension>]
    static member CountWith<'T> (xs: 'T list, x: int, y: int) : int = List.length xs + x + y

[<EntryPoint>]
let main _ =
    let xs = [ "a"; "b"; "c" ]
    let d = Func<int, int, int>(fun a b -> xs.CountWith(a, b))
    if d.Invoke(2, 3) <> 8 then failwithf "wrong result: %d" (d.Invoke(2, 3))
    if d.Method.Name <> "CountWith" then failwithf "expected direct 'CountWith' but got '%s'" d.Method.Name
    if not (obj.ReferenceEquals(d.Target, xs)) then failwith "Target is not the extension receiver"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// A static method on a value type whose first argument is a reference type: that argument becomes the CLR's
// closed-over Target (a reference), not a value-type instance receiver, so it must be passed as-is and must
// NOT be boxed as the declaring struct.
[<Fact>]
let ``Static method on a value type with a reference first argument is not boxed (preview)`` () =
    FSharp """
module StaticValueTypeFirstArg

open System

[<Struct>]
type V =
    static member Pick (s: string, n: int) : int = s.Length + n

[<EntryPoint>]
let main _ =
    // F# static member on a struct: the leading arg "abc" is a reference, closed over as the Target.
    let d = Func<int, int>(fun n -> V.Pick("abc", n))
    if d.Invoke 10 <> 13 then failwithf "fsharp: expected 13 but got %d" (d.Invoke 10)
    if d.Method.Name <> "Pick" then failwithf "fsharp: expected direct 'Pick' but got '%s'" d.Method.Name
    if not (d.Target :? string) then failwith "fsharp: Target should be the reference first argument, not a boxed struct"

    // BCL static method on a struct (System.Int32): the leading arg "41" is a reference, closed over as the Target.
    let b = Func<int>(fun () -> Int32.Parse "41")
    if b.Invoke() <> 41 then failwithf "il: expected 41 but got %d" (b.Invoke())
    if b.Method.Name <> "Parse" then failwithf "il: expected direct 'Parse' but got '%s'" b.Method.Name
    if not (b.Target :? string) then failwith "il: Target should be the reference first argument, not a boxed struct"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
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

// Case 55: an over-application - the target's *result* consumes the delegate argument(s) - is not a saturated
// call to the target, so it must stay a closure with per-invocation evaluation of the function position. A
// direct delegate here would be doubly wrong: it would point at the wrong method (with an incompatible IL
// return) and would stop re-evaluating the function position on each invocation.
[<Fact>]
let ``Over-application stays a closure and evaluates per invocation (preview)`` () =
    FSharp """
module OverApplicationClosure

open System

let mutable calls = 0

let makeHandler (tag: string) : unit -> unit =
    calls <- calls + 1
    fun () -> ()

[<EntryPoint>]
let main _ =
    // 'makeHandler "h"' returns the function that consumes the Invoke argument list, so the closure must
    // re-evaluate it on every invocation, not bind 'makeHandler' at construction.
    let d = Action(makeHandler "h")
    if calls <> 0 then failwith "over-application was evaluated at construction"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    d.Invoke()
    d.Invoke()
    if calls <> 2 then failwithf "expected per-invocation evaluation, calls=%d" calls

    // A throwing function position likewise stays a closure and faults at invocation, not construction.
    let f = Action(failwith "boom")
    if f.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" f.Method.Name

    try
        f.Invoke()
        failwith "expected the lazy 'failwith' to throw on Invoke"
    with Failure "boom" ->
        ()

    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

let private crossAssemblyLibrary =
    FSharp """
module DelegateLib

let add (x: int) (y: int) : int = x + y

type Calc(k: int) =
    member _.Scale (x: int) (y: int) : int = (x + y) * k

// A small inline function: its body is serialized into the referenced assembly and is always inlined at the
// use site (independent of --optimize), so a delegate over it can never see a forwarding call.
let inline addInline (x: int) (y: int) : int = x + y
    """
    |> asLibrary

[<Fact>]
let ``Cross-assembly F# target is emitted directly (preview)`` () =
    FSharp """
module CrossAsmDirect

open System
open DelegateLib

[<EntryPoint>]
let main _ =
    // Static module function imported from another assembly: direct, null Target, real Method.Name.
    let ds = Func<int, int, int>(add)
    if ds.Invoke(2, 3) <> 5 then failwith "static: wrong result"
    if ds.Method.Name <> "add" then failwithf "static: expected 'add' but got '%s'" ds.Method.Name
    if not (isNull ds.Target) then failwith "static: Target should be null"

    // Instance member imported from another assembly: direct, Target is the receiver.
    let c = Calc(10)
    let di = Func<int, int, int>(c.Scale)
    if di.Invoke(2, 3) <> 50 then failwithf "instance: expected 50 but got %d" (di.Invoke(2, 3))
    if di.Method.Name <> "Scale" then failwithf "instance: expected 'Scale' but got '%s'" di.Method.Name
    if not (obj.ReferenceEquals(di.Target, c)) then failwith "instance: Target is not the receiver"
    0
        """
    |> withReferences [ crossAssemblyLibrary ]
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// An 'inline' target from a referenced assembly is always inlined (mandatory inlining takes precedence over
// the forwarding-call preservation), so the forwarding call vanishes and a closure is kept even in release.
[<Fact>]
let ``Cross-assembly inline target stays a closure (preview)`` () =
    FSharp """
module CrossAsmInline

open System
open DelegateLib

[<EntryPoint>]
let main _ =
    let d = Func<int, int, int>(fun a b -> addInline a b)
    if d.Invoke(2, 3) <> 5 then failwith "wrong result"
    if d.Method.Name <> "Invoke" then failwithf "expected closure 'Invoke' but got '%s'" d.Method.Name
    0
        """
    |> withReferences [ crossAssemblyLibrary ]
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// An [<InlineIfLambda>] function parameter yields a direct delegate only when full inlining leaves a
// forwarding call to a named method as the delegate body: when the inlined lambda body is arbitrary code
// there is no method to point at, and when either inlining does not happen, the parameter is a first-class
// function value (case 42) - both keep a closure.
[<Fact>]
let ``InlineIfLambda function parameter keeps a closure unless inlining exposes a forwarding call (preview)`` () =
    FSharp """
module InlineIfLambdaClosure

open System

let mutable acc = 0

let inline makeAction ([<InlineIfLambda>] f: int -> int -> unit) = Action<int, int>(fun a b -> f a b)

// Read through a mutable so no inlining step can turn the argument back into a lambda.
let mutable handler : int -> int -> unit = fun a b -> acc <- acc + a * 100 + b

let bump (a: int) (b: int) : unit = acc <- acc + a * 1000 + b

[<EntryPoint>]
let main _ =
    // Lambda argument: 'makeAction' and the lambda both inline, leaving inlined code as the delegate body.
    let k = 7
    let d = makeAction (fun a b -> acc <- acc + a * 10 + b + k)
    d.Invoke(1, 2)
    if acc <> 19 then failwithf "lambda: wrong result %d" acc
    if d.Method.Name <> "Invoke" then failwithf "lambda: expected closure 'Invoke' but got '%s'" d.Method.Name

    // First-class argument: there is no lambda to inline, so 'f' is a function value.
    acc <- 0
    let d2 = makeAction handler
    d2.Invoke(1, 2)
    if acc <> 102 then failwithf "value: wrong result %d" acc
    if d2.Method.Name <> "Invoke" then failwithf "value: expected closure 'Invoke' but got '%s'" d2.Method.Name

    // Forwarding lambda argument: after both inline, the delegate body is a forwarding call to 'bump',
    // which the recognizer binds directly.
    acc <- 0
    let d3 = makeAction (fun a b -> bump a b)
    d3.Invoke(1, 2)
    if acc <> 1002 then failwithf "forwarding: wrong result %d" acc
    if d3.Method.Name <> "bump" then failwithf "forwarding: expected direct 'bump' but got '%s'" d3.Method.Name
    if not (isNull d3.Target) then failwith "forwarding: Target should be null for a static target"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// A static method's closed-over first argument must be a *known* reference type: the CLR stores it as the
// delegate's 'object' Target and passes it unboxed into the method's first by-value parameter, so a value type
// has no closed form. A type parameter is not known to be a reference type (it could be instantiated with a
// value type), so closing over a type-parameter-typed first argument stays a closure - a direct delegate would
// push an unboxed !!T where an object Target is expected (invalid IL, InvalidProgramException at runtime).
[<Fact>]
let ``Static method with a type-parameter first argument stays a closure (preview)`` () =
    FSharp """
module GenericStaticFirstArgClosure

open System

let pick<'T> (tag: 'T) (n: int) : int = n + 1

let make<'T> (v: 'T) = Func<int, int>(fun n -> pick v n)

[<EntryPoint>]
let main _ =
    // 'T = int (value type): must be a closure, not a direct delegate closing over an unboxed int.
    let di = make 100
    if di.Invoke 5 <> 6 then failwithf "int: wrong result %d" (di.Invoke 5)
    if di.Method.Name <> "Invoke" then failwithf "int: expected closure 'Invoke' but got '%s'" di.Method.Name

    // 'T = string (reference type): also a closure, since the recognizer cannot know 'T is a reference type.
    let ds = make "abc"
    if ds.Invoke 5 <> 6 then failwithf "string: wrong result %d" (ds.Invoke 5)
    if ds.Method.Name <> "Invoke" then failwithf "string: expected closure 'Invoke' but got '%s'" ds.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// An instance receiver typed as a bare type parameter has no direct form either: generic code shares one body
// across reference instantiations but specializes value ones, so pushing an unboxed !!T as the 'object' Target
// is invalid IL for a value-type instantiation (and unverifiable even for a reference one). Both a struct and a
// class instantiation must therefore stay a closure.
[<Fact>]
let ``Type-parameter instance receiver stays a closure (preview)`` () =
    FSharp """
module TyparInstanceReceiverClosure

open System

type IFoo =
    abstract M : int -> int

[<Struct>]
type SFoo =
    interface IFoo with
        member _.M x = x + 1

type CFoo() =
    interface IFoo with
        member _.M x = x + 1

let make<'T when 'T :> IFoo> (x: 'T) = Func<int, int>(x.M)

[<EntryPoint>]
let main _ =
    // 'T = struct implementing IFoo: a direct delegate would emit invalid IL, so a closure is kept.
    let ds = make (SFoo())
    if ds.Invoke 5 <> 6 then failwithf "struct: wrong result %d" (ds.Invoke 5)
    if ds.Method.Name <> "Invoke" then failwithf "struct: expected closure 'Invoke' but got '%s'" ds.Method.Name

    // 'T = class implementing IFoo: also a closure, since the receiver type is a bare type parameter.
    let dc = make (CFoo())
    if dc.Invoke 5 <> 6 then failwithf "class: wrong result %d" (dc.Invoke 5)
    if dc.Method.Name <> "Invoke" then failwithf "class: expected closure 'Invoke' but got '%s'" dc.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// A BCL instance method on a value type is reached via the ILCall path and boxes the receiver as the Target
// (the same box logic as an F# struct instance receiver, exercised through imported metadata).
[<Fact>]
let ``BCL value-type instance method targets the real method (preview)`` () =
    FSharp """
module BclStructInstanceDirect

open System

[<EntryPoint>]
let main _ =
    // Int32.CompareTo(int) is an instance method on a value type.
    let d = Func<int, int>(fun x -> (42).CompareTo(x))
    if d.Invoke 42 <> 0 then failwithf "compare-eq: %d" (d.Invoke 42)
    if d.Invoke 100 >= 0 then failwithf "compare-lt: %d" (d.Invoke 100)
    if d.Invoke 1 <= 0 then failwithf "compare-gt: %d" (d.Invoke 1)
    if d.Method.Name <> "CompareTo" then failwithf "expected direct 'CompareTo' but got '%s'" d.Method.Name
    if isNull d.Target then failwith "Target should be the boxed receiver"
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

// Property accessors compile to get_/set_ methods and are direct instance targets like any other member; the
// setter additionally exercises a void-returning instance target.
[<Fact>]
let ``Property getter and setter are emitted directly (preview)`` () =
    FSharp """
module PropertyAccessorDirect

open System

type C() =
    let mutable v = 7
    member _.Value with get () = v and set x = v <- x

[<EntryPoint>]
let main _ =
    let c = C()
    let g = Func<int>(fun () -> c.Value)
    if g.Invoke() <> 7 then failwithf "getter: %d" (g.Invoke())
    if g.Method.Name <> "get_Value" then failwithf "getter: expected 'get_Value' but got '%s'" g.Method.Name

    let s = Action<int>(fun x -> c.Value <- x)
    s.Invoke 99
    if c.Value <> 99 then failwithf "setter did not run: %d" c.Value
    if s.Method.Name <> "set_Value" then failwithf "setter: expected 'set_Value' but got '%s'" s.Method.Name
    0
        """
    |> withLangVersionPreview
    |> withOptions [ "--optimize+" ]
    |> compileExeAndRun
    |> shouldSucceed

#if NETCOREAPP
[<Fact>]
let ``Minimal API binds a direct delegate handler by parameter name (preview)`` () =
    let aspNetFrameworkReferences =
        ReferenceHelpers.getFrameworkReference { Name = "Microsoft.AspNetCore.App"; Version = None }

    Fsx (aspNetFrameworkReferences + """
module X =
    open System
    open System.Net
    open System.Net.Http
    open System.Net.Sockets
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Http

    let divide (first: int) (second: int) : int = first / second

    let run () =
        let port =
            let listener = new TcpListener(IPAddress.Loopback, 0)
            listener.Start()
            let p = (listener.LocalEndpoint :?> IPEndPoint).Port
            listener.Stop()
            p

        let url = sprintf "http://127.0.0.1:%d" port
        let app = WebApplication.CreateBuilder().Build()
        
        // Route parameters {second}/{first} bind to the handler's parameters by name, which requires delegate.Method to be the
        // real 'divide' (a direct delegate), not a synthesized closure 'Invoke'.
        app.MapGet("/divide/{second}/{first}", Func<int, int, int>(fun z w -> divide z w)) |> ignore
        app.Urls.Add url
        app.StartAsync().GetAwaiter().GetResult()

        try
            let client = new HttpClient()
            let body = client.GetStringAsync(url + "/divide/2/6").GetAwaiter().GetResult()
            if body.Trim() <> "3" then failwithf "minimal API returned '%s', expected '3'" body
        finally
            app.StopAsync().GetAwaiter().GetResult()

X.run () """)
    |> withLangVersionPreview
    |> runFsi
    |> shouldSucceed
#endif
