// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons


module internal SymbolHelpers =
    let getSymbolUsesInSolution (symbol: FSharpSymbol, declLoc: SymbolDeclarationLocation, checkFileResults: FSharpCheckFileResults,
                                 projectInfoManager: ProjectInfoManager, checker: FSharpChecker, solution: Solution) =
        async {
            let! symbolUses =
                match declLoc with
                | SymbolDeclarationLocation.CurrentDocument ->
                    checkFileResults.GetUsesOfSymbolInFile(symbol)
                | SymbolDeclarationLocation.Projects (projects, isInternalToProject) -> 
                    let projects =
                        if isInternalToProject then projects
                        else 
                            [ for project in projects do
                                yield project
                                yield! project.GetDependentProjects() ]
                            |> List.distinctBy (fun x -> x.Id)

                    projects
                    |> Seq.map (fun project ->
                        async {
                            match projectInfoManager.TryGetOptionsForProject(project.Id) with
                            | Some options ->
                                let! projectCheckResults = checker.ParseAndCheckProject(options)
                                return! projectCheckResults.GetUsesOfSymbol(symbol)
                            | None -> return [||]
                        })
                    |> Async.Parallel
                    |> Async.map Array.concat
            
            return
                (symbolUses 
                 |> Seq.collect (fun symbolUse -> 
                      solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                 |> Seq.groupBy fst
                ).ToImmutableDictionary(
                    (fun (id, _) -> id), 
                    fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
        }


