// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System

open NUnit.Framework

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open FsUnit

[<TestFixture>]
[<Category "Roslyn Services">]
type UnusedOpensDiagnosticAnalyzer()  =
    
    let filePath = "C:\\test.fs"
    
    let projectOptions : FSharpProjectOptions = 
        { ProjectFileName = "C:\\test.fsproj"
          SourceFiles =  [| filePath |]
          ReferencedProjects = [| |]
          OtherOptions = [| |]
          IsIncompleteTypeCheckEnvironment = true
          UseScriptResolutionRules = false
          LoadTime = DateTime.MaxValue
          OriginalLoadReferences = []
          UnresolvedReferences = None
          ExtraProjectInfo = None
          Stamp = None }

    let mutable checker = lazy (FSharpChecker.Create())

    let (=>) (source: string) (expectedRanges: (int * (int * int)) list) =
        let sourceText = SourceText.From(source)

        let parsedInput, checkFileResults =
            let parseResults, checkFileAnswer = checker.Value.ParseAndCheckFileInProject(filePath, 0, source, projectOptions) |> Async.RunSynchronously
            match checkFileAnswer with
            | FSharpCheckFileAnswer.Aborted -> failwithf "ParseAndCheckFileInProject aborted"
            | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                match parseResults.ParseTree with
                | None -> failwith "Parse returns None ParseTree"
                | Some parsedInput -> parsedInput, checkFileResults

        let allSymbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> Async.RunSynchronously
        let unusedOpenRanges = UnusedOpens.getUnusedOpens sourceText parsedInput allSymbolUses
        
        unusedOpenRanges 
        |> List.map (fun x -> x.StartLine, (x.StartColumn, x.EndColumn))
        |> shouldEqual expectedRanges

    [<Test>]
    member __.``top level module``() =
    """
module TopModule
open System
open System.IO
let _ = DateTime.Now
"""
    => [ 4, (5, 14) ]
