// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text.Editor

/// helper class which provides token information
type internal TokenContext (serviceProvider : SVsServiceProvider, adapterService : IVsEditorAdaptersFactoryService) =
    let fsLangService = serviceProvider.GetService(typeof<FSharpLanguageService>) :?> FSharpLanguageService
    
    /// Returns token info for given position.
    /// If trialString is provided, will return token info for given position, assuming trialString has been inserted at that position
    member  this.GetContextAt(textView : ITextView, lineNum : int, tokenColumn : int, trialString : string, trialStringInsertionColumn : int) =
        let vsTextView = adapterService.GetViewAdapter(textView)

        let hr,buffer = vsTextView.GetBuffer()
        ErrorHandler.ThrowOnFailure(hr) |> ignore
        
        let hr,colorizer = (fsLangService :> IVsLanguageInfo).GetColorizer(buffer)
        ErrorHandler.ThrowOnFailure(hr) |> ignore

        let fsColorizer = colorizer :?> FSharpColorizer

        fsColorizer.GetTokenInfoAt(VsTextLines.TextColorState buffer, lineNum, tokenColumn, trialString, trialStringInsertionColumn)
