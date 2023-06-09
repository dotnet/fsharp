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

[<Fact>]
let ``Type check project with signature file doesn't get the diagnostic`` () =        
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> typecheckProject false
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)

[<Fact>]
let ``Type check project without signature file does get the diagnostic`` () =        
    Fs implementation
    |> typecheckProject false
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)

[<Fact>]
let ``Enabling enablePartialTypeChecking = true doesn't change the problem`` () =        
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> typecheckProject true
    |> fun projectResults ->
        projectResults.Diagnostics |> ignore
        Assert.False (projectResults.Diagnostics |> Array.isEmpty)