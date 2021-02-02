// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//------- DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS ---------------

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Diagnostics
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop 
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Tokenization

module internal OperatorToken =
    
    let asIdentifier_DEPRECATED (token : TokenInfo) =
        // Typechecker reports information about all values in the same fashion no matter whether it is named value (let binding) or operator
        // here we piggyback on this fact and just pretend that we need data time for identifier

        let tagOfIdentToken = FSharpTokenTag.IDENT

        let endCol = token.EndIndex + 1 // EndIndex from GetTokenInfoAt points to the last operator char, but here it should point to column 'after' the last char 
        tagOfIdentToken, token.StartIndex, endCol

module internal GotoDefinition =
    
    //
    // Note: DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS. 
    //
    // Note: Tests using this code should either be adjusted to test the corresponding feature in
    // FSharp.Editor, or deleted.  However, the tests may be exercising underlying F# Compiler 
    // functionality and thus have considerable value, they should ony be deleted if we are sure this 
    // is not the case.
    //
    let GotoDefinition_DEPRECATED (colourizer: FSharpColorizer_DEPRECATED, typedResults : FSharpCheckFileResults, textView : IVsTextView, line : int, col : int) : GotoDefinitionResult_DEPRECATED =
        
        let ls = textView.GetBuffer() |> Com.ThrowOnFailure1
        let len = ls.GetLengthOfLine line |> Com.ThrowOnFailure1
        let lineStr = ls.GetLineText(line, 0, line, len) |> Com.ThrowOnFailure1

        // in many cases we assume gotodef should work even if we don't stand on the identifier directly
        // treatTokenAsIdentifier governs if we want to have raw value of token (possibly operator) or if should always be considered as identifier
        let rec gotoDefinition alwaysTreatTokenAsIdentifier =
            let token = colourizer.GetTokenInfoAt(VsTextLines.TextColorState(VsTextView.Buffer textView), line, col)
            let identInfo, makeAnotherAttempt =
                if token.Type = TokenType.Operator && not alwaysTreatTokenAsIdentifier then
                    let tag, _, endCol = OperatorToken.asIdentifier_DEPRECATED token
                    Some(endCol, tag, [""]), true
                else
                    match QuickParse.GetCompleteIdentifierIsland true lineStr col with 
                    | None -> 
                        Trace.Write("LanguageService", "Goto definition: No identifier found")
                        None, false
                    | Some (s, colIdent, isQuoted) -> 
                        let qualId  = if isQuoted then [s] else s.Split '.' |> Array.toList // chop it up (in case it's a qualified ident)
                        // this is a bit irratiting: `GetTokenInfoAt` won't handle just-past-the-end, so we take the just-past-the-end position and adjust it by the `magicalAdjustmentConstant` to just-*at*-the-end
                        let colIdentAdjusted = colIdent - QuickParse.MagicalAdjustmentConstant
            
                        // Corrrect the identifier (e.g. to correctly handle active pattern names that end with "BAR" token)
                        let tag = colourizer.GetTokenInfoAt(VsTextLines.TextColorState(VsTextView.Buffer textView), line, colIdentAdjusted).Token
                        let tag = QuickParse.CorrectIdentifierToken s tag
                        Some(colIdent, tag, qualId), false
            match identInfo with
            | None ->
                Strings.GotoDefinitionFailed_NotIdentifier()
                |> GotoDefinitionResult_DEPRECATED.MakeError
            | Some(colIdent, tag, qualId) ->
                if typedResults.HasFullTypeCheckInfo then 
                    // Used to be the Parser's internal definition, now hard-coded to avoid an IVT into the parser itsef.
                    // Dead code (aside from legacy tests), ignore
                    if tag <> FSharpTokenTag.IDENT then
                        Strings.GotoDefinitionFailed_NotIdentifier()
                        |> GotoDefinitionResult_DEPRECATED.MakeError
                    else
                      match typedResults.GetDeclarationLocation (line+1, colIdent, lineStr, qualId, false) with
                      | FindDeclResult.DeclNotFound(reason) ->
                          if makeAnotherAttempt then gotoDefinition true
                          else
                          Trace.Write("LanguageService", sprintf "Goto definition failed: Reason %+A" reason)
                          let text = 
                              match reason with                    
                              | FindDeclFailureReason.Unknown _message -> Strings.GotoDefinitionFailed_Generic()
                              | FindDeclFailureReason.NoSourceCode -> Strings.GotoDefinitionFailed_NotSourceCode()
                              | FindDeclFailureReason.ProvidedType(typeName) -> String.Format(Strings.GotoDefinitionFailed_ProvidedType(), typeName)
                              | FindDeclFailureReason.ProvidedMember(name) -> String.Format(Strings.GotoDefinitionFailed_ProvidedMember(), name)
                          GotoDefinitionResult_DEPRECATED.MakeError text
                      | FindDeclResult.DeclFound m when System.IO.File.Exists m.FileName -> 
                          let span = TextSpan (iStartLine = m.StartLine-1, iEndLine = m.StartLine-1, iStartIndex = m.StartColumn, iEndIndex = m.StartColumn) 
                          GotoDefinitionResult_DEPRECATED.MakeSuccess(m.FileName, span)
                      | FindDeclResult.DeclFound _ (* File does not exist *) -> 
                          GotoDefinitionResult_DEPRECATED.MakeError(Strings.GotoDefinitionFailed_NotSourceCode())
                      | FindDeclResult.ExternalDecl _ -> 
                          GotoDefinitionResult_DEPRECATED.MakeError(Strings.GotoDefinitionFailed_NotSourceCode())
                else
                    Trace.Write("LanguageService", "Goto definition: No 'TypeCheckInfo' available")
                    Strings.GotoDefinitionFailed_NoTypecheckInfo()
                    |> GotoDefinitionResult_DEPRECATED.MakeError
            
        gotoDefinition false
