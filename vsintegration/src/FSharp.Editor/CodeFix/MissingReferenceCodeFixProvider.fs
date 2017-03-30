// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "MissingReference"); Shared>]
type internal MissingReferenceCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticId = "FS0074"
        
    let createCodeFix (title: string, context: CodeFixContext, addReference: Project) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let project = context.Document.Project
                    let solution = project.Solution
                    let references = project.AllProjectReferences
                    let newReferences = references |> Seq.append [ProjectReference(addReference.Id)]
                    return solution.WithProjectReferences(project.Id, newReferences)
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
                ),
            title)

    override __.FixableDiagnosticIds = Seq.toImmutableArray [fixableDiagnosticId]

    override __.RegisterCodeFixesAsync context : Task =
        async { 
            let solution = context.Document.Project.Solution

            context.Diagnostics 
            |> Seq.filter (fun x -> x.Id = fixableDiagnosticId)
            |> Seq.iter (fun diagnostic ->
                let message = diagnostic.GetMessage()
                let parts = message.Split([| '\'' |], StringSplitOptions.None)

                let assembly = parts.[3]
                
                match solution.Projects |> Seq.tryFind (fun project -> project.AssemblyName = assembly) with
                | Some addReference ->
                    let codefix = 
                        createCodeFix(
                            sprintf "Add a project reference to '%s'" addReference.Name, // TODO: localise
                            context,
                            addReference)

                    context.RegisterCodeFix (codefix, ImmutableArray.Create diagnostic)
                | None ->
                    ()
                )
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
