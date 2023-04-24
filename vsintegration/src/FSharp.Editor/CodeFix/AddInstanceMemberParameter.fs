// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddInstanceMemberParameter); Shared>]
type internal FSharpAddInstanceMemberParameterCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingInstanceMemberParameter()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0673")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do context.RegisterFsharpFix(CodeFix.AddInstanceMemberParameter, title, [| TextChange(TextSpan(context.Span.Start, 0), "x.") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
