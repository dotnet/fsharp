﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddInstanceMemberParameter); Shared>]
type internal FSharpAddInstanceMemberParameterCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingInstanceMemberParameter()

    interface IFSharpCodeFix with
        member _.GetChangesAsync _ span =
            let changes = [ TextChange(TextSpan(span.Start, 0), "x.") ]
            CancellableTask.singleton (title, changes)

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0673")

    override this.RegisterCodeFixesAsync context : Task =
        cancellableTask {
            let! title, changes = (this :> IFSharpCodeFix).GetChangesAsync context.Document context.Span
            context.RegisterFsharpFix(CodeFix.AddInstanceMemberParameter, title, changes)
        }
        |> CancellableTask.startAsTask context.CancellationToken
