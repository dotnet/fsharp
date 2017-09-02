// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "1182"

open System.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open System.Threading.Tasks
open System.Collections.Generic

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpFormattingService() =
    let emptyChange = Task.FromResult<IList<TextChange>> [||]

    interface IEditorFormattingService with
        member __.SupportsFormatDocument = true
        member __.SupportsFormatSelection = true
        member __.SupportsFormatOnPaste = false
        member __.SupportsFormatOnReturn = false
        member __.SupportsFormattingOnTypedCharacter (_document, _ch) = false

        member __.GetFormattingChangesAsync (document, textSpan, cancellationToken) = emptyChange
        member __.GetFormattingChangesOnPasteAsync (document, textSpan, cancellationToken) = emptyChange
        member __.GetFormattingChangesAsync (document, typedChar, position, cancellationToken) = emptyChange
        member __.GetFormattingChangesOnReturnAsync (document, position, cancellationToken) = emptyChange