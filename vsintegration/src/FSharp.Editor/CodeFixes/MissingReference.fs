// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.IO

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open CancellableTasks

type private ReferenceType =
    | AddProjectRef of ProjectReference
    | AddMetadataRef of MetadataReference

// This code fix only works for the legacy, non-SDK projects.
// In SDK projects, transitive references do this trick.
// See: https://github.com/dotnet/fsharp/pull/2743

// Because this code fix is barely applicable anymore
// and because it's very different from other code fixes
// (applies to the projects files, not code itself),
// it's not implemented via IFSharpCodeFix interfaces
// and not tested automatically either.
// If we happen to create similar code fixes,
// it'd make sense to create some testing framework for them.
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.MissingReference); Shared>]
type internal MissingReferenceCodeFixProvider() =
    inherit CodeFixProvider()

    let createCodeFix (title: string, context: CodeFixContext, addReference: ReferenceType) =
        CodeAction.Create(
            title,
            (fun cancellationToken ->
                cancellableTask {
                    let project = context.Document.Project
                    let solution = project.Solution

                    match addReference with
                    | AddProjectRef projectRef ->
                        let references = project.AllProjectReferences
                        let newReferences = references |> Seq.append [ projectRef ]
                        return solution.WithProjectReferences(project.Id, newReferences)

                    | AddMetadataRef metadataRef ->
                        let references = project.MetadataReferences
                        let newReferences = references |> Seq.append [ metadataRef ]
                        return solution.WithProjectMetadataReferences(project.Id, newReferences)
                }
                |> CancellableTask.start cancellationToken)
        )

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0074"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let solution = context.Document.Project.Solution

            let diagnostic = context.Diagnostics[0]
            let message = diagnostic.GetMessage()
            let parts = message.Split([| '\'' |], StringSplitOptions.None)

            match parts with
            | [| _; _type; _; assemblyName; _ |] ->

                let exactProjectMatches =
                    solution.Projects
                    |> Seq.tryFindV (fun project ->
                        String.Compare(project.AssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase) = 0)

                match exactProjectMatches with
                | ValueSome refProject ->
                    let codefix =
                        createCodeFix (
                            String.Format(SR.AddProjectReference(), refProject.Name),
                            context,
                            AddProjectRef(ProjectReference refProject.Id)
                        )

                    context.RegisterCodeFix(codefix, ImmutableArray.Create diagnostic)
                | ValueNone ->
                    let metadataReferences =
                        solution.Projects
                        |> Seq.collect (fun project -> project.MetadataReferences)
                        |> Seq.tryFindV (fun ref ->
                            let referenceAssemblyName = Path.GetFileNameWithoutExtension(ref.Display)
                            String.Compare(referenceAssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase) = 0)

                    match metadataReferences with
                    | ValueSome metadataRef ->
                        let codefix =
                            createCodeFix (String.Format(SR.AddAssemblyReference(), assemblyName), context, AddMetadataRef metadataRef)

                        context.RegisterCodeFix(codefix, ImmutableArray.Create diagnostic)
                    | ValueNone -> ()
            | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
