module FSharp.Compiler.ComponentTests.Language.ModuleAsTypeTests

open Xunit
open FSharp.Test.Compiler

[<Theory>]
[<InlineData("typeof")>]
[<InlineData("typedefof")>]
let ``Module can be used in typeof in lang preview`` (typeofKeyword: string) =
    Fsx $"""
let actual = {typeofKeyword}<module FSharp.Core.LanguagePrimitives>.Name
let expected = "LanguagePrimitives"
if actual <> expected then failwith $"Expected '{{expected}}', but got '{{actual}}'"
    """
    |> asExe
    |> withLangVersionPreview
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``Module cannot be used in typeof in lang version70`` () =
    Fsx """
let actual = typeof<module FSharp.Core.LanguagePrimitives>
    """
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 2, Col 21, Line 2, Col 27, "Feature 'Allow modules to be used in type application' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
    ]

[<Fact>]
let ``Module cannot be used in type annotation in lang preview`` () =
    Fsx """
let actual: module LanguagePrimitives = Unchecked.defaultof<_>
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3561, Line 2, Col 13, Line 2, Col 38, "Module references are intended to be used in 'typeof'.")
    ]

[<Fact>]
let ``Module can be used as generic type argument in lang preview`` () =
    Fsx """
let actual = ResizeArray<module LanguagePrimitives>().GetType().FullName
let expected = "System.Collections.Generic.List`1[[Microsoft.FSharp.Core.LanguagePrimitives"
if not (actual.StartsWith expected) then failwith $"Expected to start with '{expected}', but got '{actual}'"
    """
    |> asExe
    |> withLangVersionPreview
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``Namespaces, types, values and non-existent modules cannot be used as modules in typeof in lang preview`` () =
    Fsx """
module rec Ns.A

module B =
    let value = ()

type R = { F: int }

let _ = typeof<module Ns>
let _ = typeof<module Nonexistent>
let _ = typeof<module NonexistentNs.NonexistentMod>
let _ = typeof<module Ns.Nonexistent>
let _ = typeof<module Ns.NonexistentNs.NonexistentMod>
let _ = typeof<module Ns.A.Nonexistent>
let _ = typeof<module Ns.A.B.value>
let _ = typeof<module R>
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 39, Line 9, Col 23, Line 9, Col 25, "The module 'Ns' is not defined.")
        (Error 39, Line 10, Col 23, Line 10, Col 34, "The module 'Nonexistent' is not defined.")
        (Error 39, Line 11, Col 23, Line 11, Col 36, "The namespace or module 'NonexistentNs' is not defined.")
        (Error 39, Line 12, Col 26, Line 12, Col 37, "The module 'Nonexistent' is not defined.")
        (Error 39, Line 13, Col 26, Line 13, Col 39, "The namespace or module 'NonexistentNs' is not defined.")
        (Error 39, Line 14, Col 28, Line 14, Col 39, "The module 'Nonexistent' is not defined.")
        (Error 39, Line 15, Col 30, Line 15, Col 35, "The module 'value' is not defined.")
        (Error 39, Line 16, Col 23, Line 16, Col 24, "The module 'R' is not defined.")
    ]