module FSharp.Compiler.ComponentTests.Signatures.MissingDiagnostic

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let implementation = """
module Foo

let a (b: int) : int = 'x'
"""

let signature = """
module Foo

val a: b: int -> int
"""

[<Fact>]
let ``Compile gives errors`` () =
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> compile
    |> shouldFail
    |> withSingleDiagnostic (Error 1, Line 4, Col 24, Line 4,Col 27, "This expression was expected to have type
    'int'    
but here has type
    'char'    ")

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Type check project with signature file doesn't get the diagnostic`` useTransparentCompiler =
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> typecheckProject false useTransparentCompiler
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Type check project without signature file does get the diagnostic`` useTransparentCompiler =
    Fs implementation
    |> typecheckProject false useTransparentCompiler
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Enabling enablePartialTypeChecking = true doesn't change the problem`` useTransparentCompiler =
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> typecheckProject true useTransparentCompiler
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)