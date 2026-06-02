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
        |> compile

    [<Fact>]
    let ``NoDynamicInvocation in impl but not sig raises error`` () =
        let sigSrc = """
module M
val f: x: int -> int
"""
        let implSrc = """
module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldFail
        |> withDiagnosticMessageMatches "NoDynamicInvocation"

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
    let ``NoDynamicInvocation on type member in impl but not sig raises error`` () =
        let sigSrc = """
module M
type T =
    new: unit -> T
    member F: x: int -> int
"""
        let implSrc = """
module M
type T() =
    [<NoDynamicInvocationAttribute>]
    member inline _.F(x: int) = x + 1
"""
        compileSigImpl sigSrc implSrc
        |> shouldFail
        |> withDiagnosticMessageMatches "NoDynamicInvocation"
