// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddInstanceMemberParameter); Shared>]
type internal FSharpAddInstanceMemberParameterCodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set [ "FS0673" ]
    static let title = SR.AddMissingInstanceMemberParameter()

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do context.RegisterFsharpFix(CodeFix.AddInstanceMemberParameter, title, [| TextChange(TextSpan(context.Span.Start, 0), "x.") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
