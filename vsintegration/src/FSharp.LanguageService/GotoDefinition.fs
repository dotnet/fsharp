// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.IO
open System.Collections.Generic
open System.Diagnostics
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.SourceCodeServices

module internal OperatorToken =
    
    let asIdentifier (token : TokenInfo) =
        // Typechecker reports information about all values in the same fashion no matter whether it is named value (let binding) or operator
        // here we piggyback on this fact and just pretend that we need data time for identifier
        let tagOfIdentToken = Microsoft.FSharp.Compiler.Parser.tagOfToken(Microsoft.FSharp.Compiler.Parser.IDENT "")
        let endCol = token.EndIndex + 1 // EndIndex from GetTokenInfoAt points to the last operator char, but here it should point to column 'after' the last char 
        tagOfIdentToken, token.StartIndex, endCol

module internal GotoDefinition =
    
    let GotoDefinition (colourizer: FSharpColorizer, typedResults : FSharpCheckFileResults, textView : IVsTextView, line : int, col : int) : GotoDefinitionResult =
        
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
                      match typedResults.GetDeclarationLocationAlternate (line+1, colIdent, lineStr, qualId, false) |> Async.RunSynchronously with
                      | FSharpFindDeclResult.DeclFound m -> 
                          let span = TextSpan (iStartLine = m.StartLine-1, iEndLine = m.StartLine-1, iStartIndex = m.StartColumn, iEndIndex = m.StartColumn) 
                          GotoDefinitionResult.MakeSuccess(m.FileName, span)
                      | FSharpFindDeclResult.DeclNotFound(reason) ->
                          if makeAnotherAttempt then gotoDefinition true
                          else
                          Trace.Write("LanguageService", sprintf "Goto definition failed: Reason %+A" reason)
                          let text = 
                              match reason with                    
                              | FSharpFindDeclFailureReason.Unknown -> Strings.Errors.GotoDefinitionFailed()
                              | FSharpFindDeclFailureReason.NoSourceCode -> Strings.Errors.GotoDefinitionFailed_NoSourceCode()
                              | FSharpFindDeclFailureReason.ProvidedType(typeName) -> Strings.Errors.GotoDefinitionFailed_ProvidedType(typeName)
                              | FSharpFindDeclFailureReason.ProvidedMember(name) -> Strings.Errors.GotoFailed_ProvidedMember(name)
                          GotoDefinitionResult.MakeError text
                else 
                    Trace.Write("LanguageService", "Goto definition: No 'TypeCheckInfo' available")
                    Strings.Errors.GotoDefinitionFailed_NoTypecheckInfo()
                    |> GotoDefinitionResult.MakeError
            
        gotoDefinition false
