// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Symbols

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RenameUnusedValue); Shared>]
type internal RenameUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let getTitle (symbolName: string) =
        String.Format(SR.RenameValueToUnderscore(), symbolName)

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
                let! symbol = CodeFixHelpers.getUnusedSymbol context.Span context.Document CodeFix.RenameUnusedValue

                return
                    symbol
                    |> Option.filter (fun symbol ->
                        match symbol with
                        | :? FSharpMemberOrFunctionOrValue as x when x.IsConstructorThisValue -> false
                        | _ -> true)
                    |> Option.map (fun symbol ->
                        {
                            Name = CodeFix.RenameUnusedValue
                            Message = getTitle symbol.DisplayName
                            Changes = [ TextChange(context.Span, "_") ]
                        })
            }
