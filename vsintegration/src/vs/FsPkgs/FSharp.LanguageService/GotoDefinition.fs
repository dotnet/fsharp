// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.IO
open System.Collections.Generic
open System.Collections
open System.Diagnostics
open System.Globalization
open System.ComponentModel.Design
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.Lib
open Internal.Utilities.Debug

module internal OperatorToken =
    
    let asIdentifier (token : TokenInfo) =
        // Typechecker reports information about all values in the same fashion no matter whether it is named value (let binding) or operator
        // here we piggyback on this fact and just pretend that we need data time for identifier
        let tagOfIdentToken = Microsoft.FSharp.Compiler.Parser.tagOfToken(Microsoft.FSharp.Compiler.Parser.IDENT "")
        let endCol = token.EndIndex + 1 // EndIndex from GetTokenInfoAt points to the last operator char, but here it should point to column 'after' the last char 
        tagOfIdentToken, token.StartIndex, endCol

module internal GotoDefinition =
    
    module Parser = Microsoft.FSharp.Compiler.Parser  

    let GotoDefinition (colourizer: FSharpColorizer, typedResults : TypeCheckResults, textView : IVsTextView, line : int, col : int) : GotoDefinitionResult =
        
        let ls = textView.GetBuffer() |> Com.ThrowOnFailure1
        let len = ls.GetLengthOfLine line |> Com.ThrowOnFailure1
        let lineStr = ls.GetLineText(line, 0, line, len) |> Com.ThrowOnFailure1
        
        // in many cases we assume gotodef should work even if we don't stand on the identifier directly
        // treatTokenAsIdentifier governs if we want to have raw value of token (possibly operator) or if should always be considered as identifier
        let rec gotoDefinition alwaysTreatTokenAsIdentifier =
            let token = colourizer.GetTokenInfoAt(VsTextLines.TextColorState(VsTextView.Buffer textView), line, col)
            let identInfo, makeAnotherAttempt =
                if token.Type = TokenType.Operator && not alwaysTreatTokenAsIdentifier then
                    let tag, _, endCol = OperatorToken.asIdentifier token
                    Some(endCol, tag, [""]), true
                else
                    match QuickParse.GetCompleteIdentifierIsland true lineStr col with 
                    | None -> 
                        Trace.Write("LanguageService", "Goto definition: No identifier found")
                        None, false
                    | Some (s, colIdent, isQuoted) -> 
                        let qualId  = if isQuoted then [s] else s.Split '.' |> Array.toList // chop it up (in case it's a qualified ident)
                        // this is a bit irratiting: `GetTokenInfoAt` won't handle just-past-the-end, so we take the just-past-the-end position and adjust it by the `magicalAdjustmentConstant` to just-*at*-the-end
                        let colIdentAdjusted = colIdent - QuickParse.magicalAdjustmentConstant
            
                        // Corrrect the identifier (e.g. to correctly handle active pattern names that end with "BAR" token)
                        let tag = colourizer.GetTokenInfoAt(VsTextLines.TextColorState(VsTextView.Buffer textView), line, colIdentAdjusted).Token
                        let tag = QuickParse.CorrectIdentifierToken s tag
                        Some(colIdent, tag, qualId), false
            match identInfo with
            | None ->
                Strings.Errors.GotoDefinitionFailed_NotIdentifier ()
                |> GotoDefinitionResult.MakeError
            | Some(colIdent, tag, qualId) ->
                if typedResults.HasFullTypeCheckInfo then 
                    if Parser.tokenTagToTokenId tag <> Parser.TOKEN_IDENT then 
                        Strings.Errors.GotoDefinitionFailed_NotIdentifier ()
                        |> GotoDefinitionResult.MakeError
                    else
                      match typedResults.GetDeclarationLocation ((line, colIdent), lineStr, qualId, tag, false) with
                      | DeclFound ((r, c), file) -> 
                          let span = TextSpan (iStartLine = r, iEndLine = r, iStartIndex = c, iEndIndex = c) 
                          GotoDefinitionResult.MakeSuccess(file, span)
                      | FindDeclResult.DeclNotFound(reason) ->
                          if makeAnotherAttempt then gotoDefinition true
                          else
                          Trace.Write("LanguageService", sprintf "Goto definition failed: Reason %+A" reason)
                          let text = 
                              match reason with                    
                              | FindDeclFailureReason.Unknown -> Strings.Errors.GotoDefinitionFailed()
                              | FindDeclFailureReason.NoSourceCode -> Strings.Errors.GotoDefinitionFailed_NoSourceCode()
                              | FindDeclFailureReason.ProvidedType(typeName) -> Strings.Errors.GotoDefinitionFailed_ProvidedType(typeName)
                              | FindDeclFailureReason.ProvidedMember(name) -> Strings.Errors.GotoFailed_ProvidedMember(name)
                          GotoDefinitionResult.MakeError text
                else 
                    Trace.Write("LanguageService", "Goto definition: No 'TypeCheckInfo' available")
                    Strings.Errors.GotoDefinitionFailed_NoTypecheckInfo()
                    |> GotoDefinitionResult.MakeError
            
        gotoDefinition false
