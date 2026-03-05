namespace Conformance.Types

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for RFC FS-1043: Extension members become available to solve operator trait constraints.
module ExtensionConstraintsTests =

    let private testFileDir = Path.Combine(__SOURCE_DIRECTORY__, "testFiles")

    /// Compile and run a test file with --langversion:preview. No warnings allowed.
    let private compileAndRunPreview fileName =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, fileName)))
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    // ========================================================================
    // Positive tests: compile AND run cleanly, zero warnings
    // ========================================================================

    [<Fact>]
    let ``Extension operators solve SRTP constraints`` () =
        compileAndRunPreview "BasicExtensionOperators.fs"

    [<Fact>]
    let ``Most recently opened extension wins`` () =
        compileAndRunPreview "ExtensionPrecedence.fs"

    [<Fact>]
    let ``Extension operators respect accessibility`` () =
        compileAndRunPreview "ExtensionAccessibility.fs"

    [<Fact>]
    let ``Extensions captured at call site not definition site`` () =
        compileAndRunPreview "ScopeCapture.fs"

    [<Fact>]
    let ``Sequentialized InvokeMap pattern compiles and runs`` () =
        compileAndRunPreview "WeakResolution.fs"

    [<Fact>]
    let ``op_Explicit return type disambiguation`` () =
        compileAndRunPreview "OpExplicitReturnType.fs"

    [<Fact>]
    let ``Issue 9382 and 9416 regressions compile and run`` () =
        compileAndRunPreview "IssueRegressions.fs"

    [<Fact>]
    let ``DateTime plus y compiles and runs with preview`` () =
        // Prior to RFC-1043, weak resolution eagerly resolved this to
        // DateTime -> TimeSpan -> DateTime. Now it stays generic because
        // weak resolution is deferred for inline code.
        FSharp """
module WeakResDateTime
open System
let inline f1 (x: DateTime) y = x + y
let r = f1 DateTime.MinValue (TimeSpan.FromHours(1.0))
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Built-in operator wins over extension on same type`` () =
        FSharp """
module Test
type System.Int32 with
    static member (+) (a: int, b: int) = a * b  // deliberately wrong

let r1 = 1 + 2  // built-in must win, not the extension
if r1 <> 3 then failwith (sprintf "Expected 3, got %d" r1)

let inline addGeneric (x: ^T) (y: ^T) = x + y
let r2 = addGeneric 1 2  // built-in must win even through SRTP
if r2 <> 3 then failwith (sprintf "Expected 3, got %d" r2)
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    // ========================================================================
    // Negative tests: assert specific diagnostics
    // ========================================================================

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

    [<Fact>]
    let ``Issue 8794 - Shadowing member return type produces ambiguity error`` () =
        // When Daughter shadows Mother.Hello() with a different return type,
        // the member constraint finds both overloads and reports ambiguity.
        // Not directly RFC FS-1043 — documents current member constraint behavior.
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

    [<Fact>]
    let ``Extension not in scope is not resolved`` () =
        FSharp """
module Exts =
    type System.Int32 with
        static member Zing(x: int) = x + 999

module Consumer =
    let inline zing (x: ^T) = (^T : (static member Zing: ^T -> ^T) x)
    let r = zing 5  // Exts not opened — should fail
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
