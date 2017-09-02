// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "1182"

open System.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open System.Threading.Tasks
open System.Collections.Generic
open Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting
open System.IO.Ports

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpFormattingService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    static let userOpName = "Formatting"
    let emptyChange = Task.FromResult<IList<TextChange>> [||]

    interface IEditorFormattingService with
        member __.SupportsFormatDocument = true
        member __.SupportsFormatSelection = false
        member __.SupportsFormatOnPaste = false
        member __.SupportsFormatOnReturn = false
        member __.SupportsFormattingOnTypedCharacter (_, _) = false
        member __.GetFormattingChangesOnPasteAsync (_, _, _) = emptyChange
        member __.GetFormattingChangesAsync (_, _, _, _) = emptyChange
        member __.GetFormattingChangesOnReturnAsync (_, _, _) = emptyChange
        
        member __.GetFormattingChangesAsync (document, textSpan, cancellationToken) =
            asyncMaybe {
                match Option.ofNullable textSpan with
                | None -> 
                    let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                    let! sourceText = document.GetTextAsync(cancellationToken)
                    let! parsedInput = checkerProvider.Checker.ParseDocument(document, options, sourceText, userOpName)
                    let changedSource = CodeFormatter.FormatAST(parsedInput, document.FilePath, Some (sourceText.ToString()), FormatConfig.FormatConfig.Default)
                    return [| TextChange(TextSpan(0, sourceText.Length), changedSource) |]
                | Some _ -> 
                    return [||]
            }
            |> Async.map (fun xs -> (match xs with Some changes -> changes | None -> [||]) :> IList<TextChange>)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken