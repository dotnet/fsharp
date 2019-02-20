// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open SymbolHelpers
open FSharp.Compiler.SourceCodeServices.Keywords

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ReplaceWithSuggestion"); Shared>]
type internal FSharpReplaceWithSuggestionCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set ["FS0039"; "FS1129"; "FS0495"]
    let maybeString = FSComp.SR.undefinedNameSuggestionsIntro()
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        async { 
            context.Diagnostics 
            |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
            |> Seq.iter (fun diagnostic ->
                let message = diagnostic.GetMessage()
                let parts = message.Split([| maybeString |], StringSplitOptions.None)
                if parts.Length > 1 then
                    let suggestions = 
                        parts.[1].Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries) 
                        |> Array.map (fun s -> s.Trim())
                    
                    let diagnostics = ImmutableArray.Create diagnostic

                    for suggestion in suggestions do
                        let replacement = QuoteIdentifierIfNeeded suggestion
                        let codefix = 
                            createTextChangeCodeFix(
                                FSComp.SR.replaceWithSuggestion suggestion, 
                                context,
                                (fun () -> asyncMaybe.Return [| TextChange(context.Span, replacement) |]))
                        context.RegisterCodeFix(codefix, diagnostics))
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
