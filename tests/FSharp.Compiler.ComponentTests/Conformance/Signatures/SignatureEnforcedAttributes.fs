// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.Signatures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SignatureEnforcedAttributes =

    let private fsi src = SourceCodeFileKind.Create("Library.fsi", src)
    let private fs  src = SourceCodeFileKind.Create("Library.fs",  src)

    let private compileSigImpl (sigSrc: string) (implSrc: string) =
        fsFromString (fsi sigSrc)
        |> FS
        |> withAdditionalSourceFile (fs implSrc)
        |> asLibrary
        |> ignoreWarnings
        |> compile

    [<Fact>]
    let ``NoDynamicInvocation in impl but not sig produces warning`` () =
        let sigSrc = """
module M
val inline f: x: int -> int
"""
        let implSrc = """
module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "NoDynamicInvocation"
        |> withDiagnosticMessageMatches "will become an error"

    [<Fact>]
    let ``NoDynamicInvocation in both impl and sig compiles clean`` () =
        let sigSrc = """
module M
[<NoDynamicInvocationAttribute>]
val inline f: x: int -> int
"""
        let implSrc = """
module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed

    [<Fact>]
    let ``Regular attribute in impl but not sig does NOT raise enforcement error`` () =
        let sigSrc = """
module M
val f: x: int -> int
"""
        let implSrc = """
module M
[<System.Obsolete("old")>]
let f (x: int) = x + 1
"""
        // Obsolete is NOT in the enforced list - compilation must succeed.
        compileSigImpl sigSrc implSrc
        |> shouldSucceed

    [<Fact>]
    let ``InlineIfLambda in sig but not impl still raises (existing behavior preserved)`` () =
        let sigSrc = """
module M
val run: f: (int -> int) -> int
"""
        let implSrc = """
module M
let run ([<InlineIfLambda>] f: int -> int) = f 42
"""
        // Pre-existing FS3518 path. Must still fire.
        compileSigImpl sigSrc implSrc
        |> shouldFail
        |> withDiagnosticMessageMatches "InlineIfLambda"

    [<Fact>]
    let ``Attribute absent from both impl and sig is fine`` () =
        let sigSrc = """
module M
val f: x: int -> int
"""
        let implSrc = """
module M
let f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed

    [<Fact>]
    let ``NoDynamicInvocation on type member in impl but not sig produces warning`` () =
        let sigSrc = """
module M
type T =
    new: unit -> T
    member inline F: x: int -> int
"""
        let implSrc = """
module M
type T() =
    [<NoDynamicInvocationAttribute>]
    member inline _.F(x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "NoDynamicInvocation"
        |> withDiagnosticMessageMatches "will become an error"

    [<Fact>]
    let ``RequiresExplicitTypeArguments in impl but not sig produces warning`` () =
        let sigSrc = """
module M
val f: x: int -> int
"""
        let implSrc = """
module M
[<RequiresExplicitTypeArguments>]
let f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "RequiresExplicitTypeArguments"

    [<Fact>]
    let ``Conditional in impl but not sig produces warning`` () =
        let sigSrc = """
module M
type T =
    new: unit -> T
    member F: x: int -> unit
"""
        let implSrc = """
module M
type T() =
    [<System.Diagnostics.Conditional("DEBUG")>]
    member _.F(x: int) = ()
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "Conditional"

    [<Fact>]
    let ``RequireQualifiedAccess on union in impl but not sig produces warning`` () =
        let sigSrc = """
module M
type U = A | B
"""
        let implSrc = """
module M
[<RequireQualifiedAccess>]
type U = A | B
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "RequireQualifiedAccess"

    [<Fact>]
    let ``AutoOpen on nested module in impl but not sig produces warning`` () =
        let sigSrc = """
module M
module Inner =
    val x: int
"""
        let implSrc = """
module M
[<AutoOpen>]
module Inner =
    let x = 42
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "AutoOpen"

    [<Fact>]
    let ``CLIMutable on record in impl but not sig produces warning`` () =
        let sigSrc = """
module M
type R = { mutable X: int }
"""
        let implSrc = """
module M
[<CLIMutable>]
type R = { mutable X: int }
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "CLIMutable"

    [<Fact>]
    let ``AllowNullLiteral on type in impl but not sig produces warning`` () =
        let sigSrc = """
module M
type C =
    new: unit -> C
"""
        let implSrc = """
module M
[<AllowNullLiteral>]
type C() = class end
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "AllowNullLiteral"

    [<Fact>]
    let ``NoEquality on record in impl but not sig produces warning`` () =
        // The mismatch also triggers FS293 (signature requires IStructuralEquatable
        // but implementation has NoEquality). That's a separate, existing diagnostic.
        // We verify the new warning is included regardless.
        let sigSrc = """
module M
type R = { X: int }
"""
        let implSrc = """
module M
[<NoEquality; NoComparison>]
type R = { X: int }
"""
        compileSigImpl sigSrc implSrc
        |> withDiagnosticMessageMatches "NoEquality"
        |> withDiagnosticMessageMatches "will become an error"

    [<Fact>]
    let ``Multiple enforced attributes on same val produce multiple warnings`` () =
        let sigSrc = """
module M
val inline f: x: int -> int
"""
        let implSrc = """
module M
[<NoDynamicInvocationAttribute>]
[<RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withDiagnosticMessageMatches "NoDynamicInvocation"
        |> withDiagnosticMessageMatches "RequiresExplicitTypeArguments"

    [<Fact>]
    let ``Warning is suppressible with nowarn 3888`` () =
        let sigSrc = """
module M
val inline f: x: int -> int
"""
        let implSrc = """
module M
#nowarn "3888"
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
        fsFromString (fsi sigSrc)
        |> FS
        |> withAdditionalSourceFile (fs implSrc)
        |> asLibrary
        |> compile
        |> shouldSucceed

    // -----------------------------------------------------------------------
    // Module-level attribute
    // -----------------------------------------------------------------------

    [<Fact>]
    let ``AutoOpen on top-level module in impl but not sig produces warning`` () =
        let sigSrc = """
module M.Sub
val x: int
"""
        let implSrc = """
[<AutoOpen>]
module M.Sub
let x = 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "AutoOpen"

    // -----------------------------------------------------------------------
    // Diagnostic placement and range
    // -----------------------------------------------------------------------

    [<Fact>]
    let ``Diagnostic squiggle is placed on the offending attribute in the .fs`` () =
        let sigSrc = """
module M
val inline f: x: int -> int
"""
        // Line numbers (1-based) — line 1 is empty, line 2 `module M`, line 3 the attribute.
        let implSrc = """
module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
        let result = compileSigImpl sigSrc implSrc |> shouldSucceed
        // Verify a single FS3888 diagnostic and that its range targets the
        // attribute on line 3, not the value identifier on line 4.
        let diagnostics =
            match result with
            | CompilationResult.Success r -> r.Diagnostics
            | CompilationResult.Failure r -> r.Diagnostics
        let attribDiag =
            diagnostics
            |> List.filter (fun d -> match d.Error with Warning n -> n = 3888 | _ -> false)
            |> List.exactlyOne
        Assert.Equal(3, attribDiag.Range.StartLine)
        Assert.Equal(3, attribDiag.Range.EndLine)

    [<Fact>]
    let ``Diagnostic on entity attribute targets the attribute in the .fs`` () =
        let sigSrc = """
module M
type U = A | B
"""
        let implSrc = """
module M
[<RequireQualifiedAccess>]
type U = A | B
"""
        let result = compileSigImpl sigSrc implSrc |> shouldSucceed
        let diagnostics =
            match result with
            | CompilationResult.Success r -> r.Diagnostics
            | CompilationResult.Failure r -> r.Diagnostics
        let attribDiag =
            diagnostics
            |> List.filter (fun d -> match d.Error with Warning n -> n = 3888 | _ -> false)
            |> List.exactlyOne
        // Attribute is on line 3 (1-based, after the leading empty line + `module M`).
        Assert.Equal(3, attribDiag.Range.StartLine)

    // -----------------------------------------------------------------------
    // -----------------------------------------------------------------------
    // Note: opt-in escalation of FS3888 to error (via the
    // ErrorOnMissingSignatureAttribute language feature) is deferred -
    // FSharp.Profiles.props sets <LangVersion>preview</LangVersion> for the
    // F# self-build, which would unconditionally turn FS3888 into a hard
    // error during the bootstrap build. The escalation should be gated on
    // a project property that does NOT inherit from <LangVersion>preview</>.
    // -----------------------------------------------------------------------
