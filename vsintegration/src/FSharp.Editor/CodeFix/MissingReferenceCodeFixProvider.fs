// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.IO

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

type private ReferenceType =
| AddProjectRef of ProjectReference
| AddMetadataRef of MetadataReference

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "MissingReference"); Shared>]
type internal MissingReferenceCodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticId = "FS0074"
        
    let createCodeFix (title: string, context: CodeFixContext, addReference: ReferenceType) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let project = context.Document.Project
                    let solution = project.Solution

                    match addReference with
                    | AddProjectRef projectRef -> 
                        let references = project.AllProjectReferences
                        let newReferences = references |> Seq.append [projectRef]
                        return solution.WithProjectReferences(project.Id, newReferences)

                    | AddMetadataRef metadataRef ->
                        let references = project.MetadataReferences
                        let newReferences = references |> Seq.append [metadataRef]
                        return solution.WithProjectMetadataReferences(project.Id, newReferences)
                }
                |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
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

                match parts with
                | [| _; _type; _; assemblyName; _ |] ->

                    let exactProjectMatches = solution.Projects |> Seq.tryFind (fun project -> project.AssemblyName = assemblyName)
                
                    match exactProjectMatches with
                    | Some refProject ->
                        let codefix = 
                            createCodeFix(
                                sprintf "Add a project reference to '%s'" refProject.Name, // TODO: localise
                                context,
                                AddProjectRef (ProjectReference refProject.Id)
                                )

                        context.RegisterCodeFix (codefix, ImmutableArray.Create diagnostic)
                    | None ->
                        let metadataReferences =
                            solution.Projects
                            |> Seq.collect (fun project -> project.MetadataReferences)
                            |> Seq.tryFind (fun ref ->
                                Path.GetFileNameWithoutExtension(ref.Display) = assemblyName
                                )
                        
                        match metadataReferences with
                        | Some metadataRef ->
                            let codefix = 
                                createCodeFix(
                                    sprintf "Add an assembly reference to '%s'" metadataRef.Display, // TODO: localise
                                    context,
                                    AddMetadataRef metadataRef
                                    )

                            context.RegisterCodeFix (codefix, ImmutableArray.Create diagnostic)
                        | None ->
                            ()
                | _ -> ()
                )
        }
        |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
