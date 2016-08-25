// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.LanguageService

module internal CommonRoslynHelpers =

    // Create F# project options for a Roslyn project.
    let rec GetFSharpProjectOptionsForRoslynProject(project: Project) : FSharpProjectOptions = { 
        ProjectFileName = project.FilePath
        ProjectFileNames = project.Documents
            |> Seq.map (fun document -> document.Name)
            |> Seq.toArray
        ReferencedProjects = project.ProjectReferences
            |> Seq.map(fun reference -> project.Solution.Projects |> Seq.find(fun otherProject -> otherProject.Id = reference.ProjectId))
            |> Seq.map(fun otherProject -> (otherProject.FilePath, GetFSharpProjectOptionsForRoslynProject(otherProject)))
            |> Seq.toArray

        // FSROSLYNTODO: add defines flags if available from project sites and files
        OtherOptions = [| |]

        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        UnresolvedReferences = None
    }

    let FSharpRangeToTextSpan(sourceText: SourceText, range: range) =
        // Roslyn TextLineCollection is zero-based, F# range lines are one-based
        let startPosition = sourceText.Lines.[range.StartLine - 1].Start + range.StartColumn
        let endPosition = sourceText.Lines.[range.EndLine - 1].Start + range.EndColumn
        TextSpan(startPosition, endPosition - startPosition)

    let GetCompletedTaskResult(task: Task<'TResult>) =
        if task.Status = TaskStatus.RanToCompletion then
            task.Result
        else
            Assert.Exception(task.Exception.GetBaseException())
            raise(task.Exception.GetBaseException())
