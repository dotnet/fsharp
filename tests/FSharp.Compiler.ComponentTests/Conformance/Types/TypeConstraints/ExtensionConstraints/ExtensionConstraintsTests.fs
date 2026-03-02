namespace Conformance.Types

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for RFC FS-1043: Extension members become available to solve operator trait constraints.
module ExtensionConstraintsTests =

    let private testFileDir = Path.Combine(__SOURCE_DIRECTORY__, "testFiles")

    let private loadAndRun fileName =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, fileName)))
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    // ---- Basic Extension Operators ----

    [<Fact>]
    let ``Extension operators solve SRTP constraints`` () =
        loadAndRun "BasicExtensionOperators.fs"

    // ---- Extension Precedence ----

    [<Fact>]
    let ``Most recently opened extension wins`` () =
        loadAndRun "ExtensionPrecedence.fs"

    // ---- Extension Accessibility ----

    [<Fact>]
    let ``Extension operators respect accessibility`` () =
        loadAndRun "ExtensionAccessibility.fs"

    // ---- Weak Resolution ----
    // ignoreWarnings: f1 (DateTime + y) stays generic, producing FS3882
    // ("constraint could not be statically resolved") — this is expected,
    // the function works when called with concrete types at the call site.

    [<Fact>]
    let ``Weak resolution deferred for inline code with extensions`` () =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, "WeakResolution.fs")))
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    // ---- AllowOverloadOnReturnType ----
    // ignoreWarnings: FS0077 about op_Explicit member constraints having
    // special compiler treatment. The test still exercises the resolution correctly.

    [<Fact>]
    let ``AllowOverloadOnReturnType enables return type disambiguation`` () =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, "AllowOverloadOnReturnType.fs")))
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    // ---- Issue Regressions ----
    // These compile with warnings (FS3882: constraint not statically resolved) but
    // no longer produce internal errors (FS0073), which was the original bug.

    [<Fact>]
    let ``Issue regressions compile and run`` () =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, "IssueRegressions.fs")))
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    // ---- FS1215 warning suppression ----

    [<Fact>]
    let ``FS1215 warning suppressed when ExtensionConstraintSolutions is active`` () =
        Fsx """
type System.String with
    static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FS1215 warning emitted without ExtensionConstraintSolutions`` () =
        Fsx """
type System.String with
    static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)
        """
        |> withLangVersion80
        |> compile
        |> withDiagnostics [
            Warning 1215, Line 3, Col 19, Line 3, Col 22, "Extension members cannot provide operator overloads.  Consider defining the operator as part of the type definition instead."
        ]

    // ---- Negative: FSharpPlus-style ambiguity should fail ----

    [<Fact>]
    let ``FSharpPlus Sequence pattern fails to compile`` () =
        Fsx """
let inline CallReturn< ^M, ^R, 'T when (^M or ^R) : (static member Return : unit -> ('T -> ^R))> () =
    ((^M or ^R) : (static member Return : unit -> ('T -> ^R)) ())

let inline CallApply< ^M, ^I1, ^I2, ^R when (^M or ^I1 or ^I2) : (static member Apply : ^I1 * ^I2 -> ^R)> (input1: ^I1, input2: ^I2) =
    ((^M or ^I1 or ^I2) : (static member Apply : ^I1 * ^I2 -> ^R) input1, input2)

let inline CallMap< ^M, ^F, ^I, ^R when (^M or ^I or ^R) : (static member Map : ^F * ^I -> ^R)> (mapping: ^F, source: ^I) : ^R =
    ((^M or ^I or ^R) : (static member Map : ^F * ^I -> ^R) mapping, source)

let inline CallSequence< ^M, ^I, ^R when (^M or ^I) : (static member Sequence : ^I -> ^R)> (b: ^I) : ^R =
    ((^M or ^I) : (static member Sequence : ^I -> ^R) b)

type Return = class end
type Apply = class end
type Map = class end
type Sequence = class end

let inline InvokeReturn (x: 'T) : ^R = CallReturn< Return, ^R, 'T> () x
let inline InvokeApply (f: ^I1) (x: ^I2) : ^R = CallApply<Apply, ^I1, ^I2, ^R>(f, x)
let inline InvokeMap (mapping: ^F) (source: ^I) : ^R = CallMap<Map, ^F, ^I, ^R> (mapping, source)

type Sequence with
    static member inline Sequence (t: list<option<'t>>) : ^R =
        List.foldBack (fun (x: 't option) (ys: ^R) -> InvokeApply (InvokeMap (fun x y -> x :: y) x) ys) t (InvokeReturn [])

type Map with
    static member Map (f: 'T->'U, x: option<_>) = Option.map f x

type Apply with
    static member Apply (f: option<_>, x: option<'T>) : option<'U> = failwith ""

type Return with
    static member Return () = fun x -> Some x : option<'a>

let res = CallSequence<Sequence, _, _> [Some 3; Some 2; Some 1]
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail

    // ---- Issue #8794: Return type in member constraint with shadowing ----
    // When Daughter shadows Mother.Hello() with a different return type,
    // the member constraint `(member Hello : unit -> string)` finds both
    // overloads and reports ambiguity. This is not directly an RFC FS-1043
    // issue — it's about return-type-based disambiguation in member constraints
    // generally. Test documents current behavior.

    [<Fact>]
    let ``Issue 8794 - Shadowing member return type produces ambiguity error`` () =
        Fsx """
type Mother() =
    member this.Hello() = Unchecked.defaultof<int>

type Daughter() =
    inherit Mother()
    member this.Hello() = Unchecked.defaultof<string>

type SomeoneHolder<'Someone when 'Someone: (member Hello : unit -> string)> =
    { Someone: 'Someone }

let someoneHolder = { Someone = Daughter() }
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
