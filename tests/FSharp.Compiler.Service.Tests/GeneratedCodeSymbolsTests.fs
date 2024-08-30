module FSharp.Compiler.Service.Tests.GeneratedCodeSymbolsTests

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols

[<Literal>]
let dirName = "GeneratedCodeSymbolsTests"

[<Fact>]
let ``IsUnionCaseTester in generated file`` () =
    let source = """
module Lib

type T () =
    member x.IsM = 1
"""
    let cleanup, options = createProjectOptions dirName [ source ] [ "--langversion:preview" ]
    use _holder = cleanup
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=false)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate

    let mfvs =
        seq {
            for implFile in wholeProjectResults.AssemblyContents.ImplementationFiles do
                for decl in implFile.Declarations do
                    match decl with
                    | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                        for d in ds do
                            match d with
                            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (mfv, args, body) ->
                                yield mfv
                            | _ -> ()
                    | _ -> ()
        }

    Assert.Contains(mfvs, fun x -> x.LogicalName = "get_IsM")
    let mfv = mfvs |> Seq.filter (fun x -> x.LogicalName = "get_IsM") |> Seq.exactlyOne
    Assert.False(mfv.IsUnionCaseTester, $"get_IsM has IsUnionCaseTester = {mfv.IsUnionCaseTester}")

[<Fact>]
let ``IsUnionCaseTester in generated file 2`` () =
    let source = """
module Lib

type T = A | B
"""
    let cleanup, options = createProjectOptions dirName [ source ] [ "--langversion:preview" ]
    use _holder = cleanup
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=false)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate

    let mfvs =
        seq {
            for implFile in wholeProjectResults.AssemblyContents.ImplementationFiles do
                for decl in implFile.Declarations do
                    match decl with
                    | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                        for d in ds do
                            match d with
                            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (mfv, args, body) ->
                                yield mfv
                            | _ -> ()
                    | _ -> ()
        }

    Assert.Contains(mfvs, fun x -> x.LogicalName = "get_IsA")
    let mfv = mfvs |> Seq.filter (fun x -> x.LogicalName = "get_IsA") |> Seq.exactlyOne
    Assert.True(mfv.IsUnionCaseTester, $"get_IsA has IsUnionCaseTester = {mfv.IsUnionCaseTester}")