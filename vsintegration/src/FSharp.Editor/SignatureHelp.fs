// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.SignatureHelp
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

[<Shared>]
[<ExportSignatureHelpProvider("FSharpSignatureHelpProvider", FSharpCommonConstants.FSharpLanguageName)>]
type FSharpSignatureHelpProvider [<ImportingConstructor>]  (serviceProvider: SVsServiceProvider) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static let oneColAfter (lp: LinePosition) = LinePosition(lp.Line,lp.Character+1)
    static let oneColBefore (lp: LinePosition) = LinePosition(lp.Line,max 0 (lp.Character-1))

    // Unit-testable core rutine
    static member internal ProvideMethodsAsyncAux(documentationBuilder: IDocumentationBuilder, sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, triggerIsTypedChar: char option, filePath: string, textVersionHash: int) = async {
        let! parseResults, checkFileAnswer = FSharpLanguageService.Checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
        match checkFileAnswer with
        | FSharpCheckFileAnswer.Aborted -> return None
        | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 

        let textLines = sourceText.Lines
        let caretLinePos = textLines.GetLinePosition(caretPosition)
        let caretLineColumn = caretLinePos.Character

        // Get the parameter locations
        let paramLocations = parseResults.FindNoteworthyParamInfoLocations(Pos.fromZ caretLinePos.Line caretLineColumn)

        match paramLocations with
        | None -> return None // no locations = no help
        | Some nwpl -> 
        let names = nwpl.LongId
        let lidEnd = nwpl.LongIdEndLocation

        // Get the methods
        let! methodGroup = checkFileResults.GetMethodsAlternate(lidEnd.Line, lidEnd.Column, "", Some names)

        let methods = methodGroup.Methods

        if (methods.Length = 0 || methodGroup.MethodName.EndsWith("> )")) then return None else                    

        let isStaticArgTip =
            let parenLine, parenCol = Pos.toZ nwpl.OpenParenLocation 
            assert (parenLine < textLines.Count)
            let parenLineText = textLines.[parenLine].ToString()
            parenCol < parenLineText.Length && parenLineText.[parenCol] = '<'

        let filteredMethods =
            [| for m in methods do 
                  if (isStaticArgTip && m.StaticParameters.Length > 0) ||
                      (not isStaticArgTip && m.HasParameters) then   // need to distinguish TP<...>(...)  angle brackets tip from parens tip
                      yield m |]

        if filteredMethods.Length = 0 then return None else

        let posToLinePosition pos = 
            let (l,c) = Pos.toZ  pos
            // FSROSLYNTODO: FCS gives back line counts that are too large. Really, this shouldn't happen
            let result =LinePosition(l,c)
            let lastPosInDocument = textLines.GetLinePosition(textLines.[textLines.Count-1].End)
            if lastPosInDocument.CompareTo(result) > 0 then result else lastPosInDocument

        // Compute the start position
        let startPos = nwpl.LongIdStartLocation |> posToLinePosition

        // Compute the end position
        let endPos = 
            let last = nwpl.TupleEndLocations.[nwpl.TupleEndLocations.Length-1] |> posToLinePosition
            (if nwpl.IsThereACloseParen then oneColBefore last else last)  

        assert (startPos.CompareTo(endPos) <= 0)

        // Compute the applicable span between the parentheses
        let applicableSpan = 
            textLines.GetTextSpan(LinePositionSpan(startPos, endPos))

        let startOfArgs = nwpl.OpenParenLocation |> posToLinePosition |> oneColAfter 

        let tupleEnds = 
            [| yield startOfArgs
               for i in 0..nwpl.TupleEndLocations.Length-2 do
                   yield nwpl.TupleEndLocations.[i] |> posToLinePosition
               yield endPos  |]

        // If we are pressing "(" or "<" or ",", then only pop up the info if this is one of the actual, real detected positions in the detected promptable call
        //
        // For example the last "(" in 
        //    List.map (fun a -> (
        // should not result in a prompt.
        //
        // Likewise the last "," in 
        //    Console.WriteLine( [(1, 
        // should not result in a prompt, whereas this one will:
        //    Console.WriteLine( [(1,2)],

        match triggerIsTypedChar with 
        | Some ('<' | '(' | ',') when not (tupleEnds |> Array.exists (fun lp -> lp.Character = caretLineColumn))  -> 
            return None // comma or paren at wrong location = remove help display
        | _ -> 

        // Compute the argument index by working out where the caret is between the various commas
        let argumentIndex = 
            tupleEnds
            |> Array.pairwise 
            |> Array.tryFindIndex (fun (lp1,lp2) -> textLines.GetTextSpan(LinePositionSpan(lp1, lp2)).Contains(caretPosition)) 
            |> (function None -> 0 | Some n -> n)
         
        // Compute the overall argument count
        let argumentCount = 
            match nwpl.TupleEndLocations.Length with 
            | 1 when caretLinePos.Character = startOfArgs.Character -> 0  // count "WriteLine(" as zero arguments
            | n -> n

        // Compute the current argument name, if any
        let argumentName = 
            if argumentIndex < nwpl.NamedParamNames.Length then 
                nwpl.NamedParamNames.[argumentIndex] 
            else 
                None  // not a named argument

        // Prepare the results
        let results = List<_>()

        for method in methods do
            // Create the documentation. Note, do this on the background thread, since doing it in the documentationBuild fails to build the XML index
            let methodDocs = XmlDocumentation.BuildMethodOverloadTipText(documentationBuilder, method.Description, true)

            let parameters = 
                let parameters = if isStaticArgTip then method.StaticParameters else method.Parameters
                [| for p in parameters do 
                      // FSROSLYNTODO: compute the proper help text for parameters, c.f. AppendParameter in XmlDocumentation.fs
                      let paramDoc = XmlDocumentation.BuildMethodParamText(documentationBuilder, method.XmlDoc, p.ParameterName) 
                      let doc = [| TaggedText(TextTags.Text, paramDoc);  |] 
                      yield (p.ParameterName,p.IsOptional,doc,[| TaggedText(TextTags.Text,p.Display) |]) |]

            let doc = [| TaggedText(TextTags.Text, methodDocs + "\n") |] 

            // Prepare the text to display
            let descriptionParts = [| TaggedText(TextTags.Text, method.TypeText) |]
            let prefixParts = [| TaggedText(TextTags.Text, methodGroup.MethodName);  TaggedText(TextTags.Punctuation,  (if isStaticArgTip then "<" else "(")) |]
            let separatorParts = [| TaggedText(TextTags.Punctuation, ", ") |]
            let suffixParts = [| TaggedText(TextTags.Text, (if isStaticArgTip then ">" else ")")) |]
            let completionItem =  (method.HasParamArrayArg ,doc,prefixParts,separatorParts,suffixParts,parameters,descriptionParts)
            // FSROSLYNTODO: Do we need a cache like for completion?
            //declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
            //declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
            results.Add(completionItem)


        let items = (results.ToArray(),applicableSpan,argumentIndex,argumentCount,argumentName)
        return Some items
    }

    interface ISignatureHelpProvider with
        member this.IsTriggerCharacter(c) = c ='(' || c = '<' || c = ',' 
        member this.IsRetriggerCharacter(c) = c = ')' || c = '>' || c = '='

        member this.GetItemsAsync(document, position, triggerInfo, cancellationToken) = 
            async {
              try
                match FSharpLanguageService.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask

                    let triggerTypedChar = 
                        if triggerInfo.TriggerCharacter.HasValue && triggerInfo.TriggerReason = SignatureHelpTriggerReason.TypeCharCommand then
                            Some triggerInfo.TriggerCharacter.Value
                        else None

                    let! methods = FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(documentationBuilder, sourceText, position, options, triggerTypedChar, document.FilePath, textVersion.GetHashCode())
                    match methods with 
                    | None -> return null
                    | Some (results,applicableSpan,argumentIndex,argumentCount,argumentName) -> 

                        let items = 
                            results |> Array.map (fun (hasParamArrayArg,doc,prefixParts,separatorParts,suffixParts,parameters,descriptionParts) ->
                                    let parameters = parameters |> Array.map (fun (paramName, isOptional, paramDoc, displayParts) -> SignatureHelpParameter(paramName,isOptional,documentationFactory=(fun _ -> paramDoc :> seq<_>),displayParts=displayParts))
                                    SignatureHelpItem(isVariadic=hasParamArrayArg ,documentationFactory=(fun _ -> doc :> seq<_>),prefixParts=prefixParts,separatorParts=separatorParts,suffixParts=suffixParts,parameters=parameters,descriptionParts=descriptionParts))

                        return SignatureHelpItems(items,applicableSpan,argumentIndex,argumentCount,Option.toObj argumentName)
                | None -> 
                    return null 
              with ex -> 
                Assert.Exception(ex)
                return null
            } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

