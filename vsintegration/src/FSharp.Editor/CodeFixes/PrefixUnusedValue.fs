// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.PrefixUnusedValue); Shared>]
type internal PrefixUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let getTitle (symbolName: string) =
        String.Format(SR.PrefixValueNameWithUnderscore(), symbolName)

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS1182"

    override this.RegisterCodeFixesAsync context =
        if context.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
            context.RegisterFsharpFix this
        else
            Task.CompletedTask

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()
                let! symbol = UnusedCodeFixHelper.getUnusedSymbol context.Span context.Document sourceText CodeFix.PrefixUnusedValue

                return
                    symbol
                    |> ValueOption.map (fun symbol ->
                        {
                            Name = CodeFix.PrefixUnusedValue
                            Message = getTitle symbol.DisplayName
                            Changes = [ TextChange(TextSpan(context.Span.Start, 0), "_") ]
                        })
            }
