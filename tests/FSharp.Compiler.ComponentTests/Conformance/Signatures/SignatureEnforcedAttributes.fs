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
