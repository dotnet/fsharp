module Tests

open System
open Xunit
open FSharp.Compiler.Compilation
open Microsoft.CodeAnalysis

let workspace = new AdhocWorkspace ()
let compilationService = CompilationService (3, 8, workspace)

[<Fact>]
let ``Compilation works`` () =
    Assert.True(true)
