﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingEqualsToTypeDefinition); Shared>]
type internal AddMissingEqualsToTypeDefinitionCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingEqualsToTypeDefinition()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3360"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! range = context.GetErrorRangeAsync()

                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof AddMissingEqualsToTypeDefinitionCodeFixProvider)

                if parseResults.IsTypeName range then
                    return None

                else
                    return
                        Some
                            {
                                Name = CodeFix.AddMissingEqualsToTypeDefinition
                                Message = title
                                Changes = [ TextChange(TextSpan(context.Span.Start, 0), "= ") ]
                            }
            }
