// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.SimplifyName); Shared>]
type internal FSharpSimplifyNameCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticId = FSharpIDEDiagnosticIds.SimplifyNamesDiagnosticId

    override _.FixableDiagnosticIds = ImmutableArray.Create(fixableDiagnosticId)

    override _.RegisterCodeFixesAsync(context: CodeFixContext) : Task =
        async {
            for diagnostic in context.Diagnostics |> Seq.filter (fun x -> x.Id = fixableDiagnosticId) do
                let title =
                    match diagnostic.Properties.TryGetValue(SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey) with
                    | true, longIdent -> sprintf "%s '%s'" (SR.SimplifyName()) longIdent
                    | _ -> SR.SimplifyName()

                let codefix =
                    CodeFixHelpers.createTextChangeCodeFix (
                        CodeFix.SimplifyName,
                        title,
                        context,
                        (fun () -> asyncMaybe.Return [| TextChange(context.Span, "") |])
                    )

                context.RegisterCodeFix(codefix, ImmutableArray.Create(diagnostic))
        }
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
