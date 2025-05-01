// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.SimplifyName); Shared>]
type internal SimplifyNameCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds =
        ImmutableArray.Create FSharpIDEDiagnosticIds.SimplifyNamesDiagnosticId

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let propertyBag = context.Diagnostics[0].Properties

                let title =
                    match propertyBag.TryGetValue SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey with
                    | true, longIdent -> sprintf "%s '%s'" (SR.SimplifyName()) longIdent
                    // TODO: figure out if this can happen in the real world
                    | _ -> SR.SimplifyName()

                return
                    ValueSome
                        {
                            Name = CodeFix.SimplifyName
                            Message = title
                            Changes = [ TextChange(context.Span, "") ]
                        }
            }
