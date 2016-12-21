// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Text.RegularExpressions
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

open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

[<Shared>]
[<ExportSignatureHelpProvider("FSharpSignatureHelpProvider", FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpSignatureHelpProvider 
    [<ImportingConstructor>]
    (
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static let oneColAfter (lp: LinePosition) = LinePosition(lp.Line,lp.Character+1)
    static let oneColBefore (lp: LinePosition) = LinePosition(lp.Line,max 0 (lp.Character-1))

    // Unit-testable core rutine
    static member internal ProvideMethodsAsyncAux(checker: FSharpChecker, documentationBuilder: IDocumentationBuilder, sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, triggerIsTypedChar: char option, filePath: string, textVersionHash: int) = async {
        let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
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

        // Compute the argument index by working out where the caret is between the various commas.
        let argumentIndex = 
            let computedTextSpans =
                tupleEnds 
                |> Array.pairwise 
                |> Array.map (fun (lp1, lp2) -> textLines.GetTextSpan(LinePositionSpan(lp1, lp2)))
                
            match (computedTextSpans|> Array.tryFindIndex (fun t -> t.Contains(caretPosition))) with 
            | None -> 
                // Because 'TextSpan.Contains' only succeeeds if 'TextSpan.Start <= caretPosition < TextSpan.End' is true,
                // we need to check if the caret is at the very last position in the TextSpan.
                //
                // We default to 0, which is the first argument, if the caret position was nowhere to be found.
                if computedTextSpans.[computedTextSpans.Length-1].End = caretPosition then
                    computedTextSpans.Length-1 
                else 0
            | Some n -> n
         
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
            let mainDescription = List()
            let documentation = List()
            XmlDocumentation.BuildMethodOverloadTipText(documentationBuilder, CommonRoslynHelpers.CollectTaggedText mainDescription, CommonRoslynHelpers.CollectTaggedText documentation, method.Description, false)

            let parameters = 
                let parameters = if isStaticArgTip then method.StaticParameters else method.Parameters
                [| for p in parameters do 
                      let doc = List()
                      // FSROSLYNTODO: compute the proper help text for parameters, c.f. AppendParameter in XmlDocumentation.fs
                      XmlDocumentation.BuildMethodParamText(documentationBuilder, CommonRoslynHelpers.CollectTaggedText doc, method.XmlDoc, p.ParameterName) 
                      //let doc = if String.IsNullOrWhiteSpace(paramDoc) then [||]
                      //          else [| TaggedText(TextTags.Text, paramDoc) |]
                      //let parameterParts =
                      //    if isStaticArgTip then
                      //        [| TaggedText(TextTags.Class, p.Display) |]                                
                      //    else
                      //        let str = p.Display
                      //        match str.IndexOf(':') with
                      //        | -1 -> [| TaggedText(TextTags.Parameter, str) |]                                
                      //        | 0 -> 
                      //          [| TaggedText(TextTags.Punctuation, ":"); 
                      //             TaggedText(TextTags.Class, str.[1..]) |]
                      //        | i -> 
                      //          [| TaggedText(TextTags.Parameter, str.[..i-1]); 
                      //             TaggedText(TextTags.Punctuation, ":"); 
                      //             TaggedText(TextTags.Class, str.[i+1..]) |]
                      let parts = List()
                      renderL (taggedTextListR (CommonRoslynHelpers.CollectTaggedText parts)) p.Display |> ignore
                      yield (p.ParameterName, p.IsOptional, doc, parts) 
                |]

            //let hasParamComments (pcs: (string*bool*TaggedText[]*TaggedText[])[]) =
            //    pcs |> Array.exists (fun (_, _, doc, _) -> doc.Length > 0)

            //let summaryText =                 
            //    let doc = 
            //        if String.IsNullOrWhiteSpace summaryDoc then
            //            String.Empty
            //        elif hasParamComments parameters then 
            //            summaryDoc + "\n" 
            //        else 
            //            summaryDoc
            //    [| TaggedText(TextTags.Text, doc) |]
                
            //// Prepare the text to display
            //let descriptionParts = 
            //    let str = method.TypeText
            //    if str.StartsWith(":", StringComparison.OrdinalIgnoreCase) then
            //        [| TaggedText(TextTags.Punctuation, ":"); 
            //           TaggedText(TextTags.Class, str.[1..]) |]
            //    else
            //        [| TaggedText(TextTags.Text, str) |]
            let prefixParts = 
                [| TaggedText(TextTags.Method, methodGroup.MethodName);  
                   TaggedText(TextTags.Punctuation, (if isStaticArgTip then "<" else "(")) |]
            let separatorParts = [| TaggedText(TextTags.Punctuation, ","); TaggedText(TextTags.Space, " ") |]
            let suffixParts = [| TaggedText(TextTags.Punctuation, (if isStaticArgTip then ">" else ")")) |]

            let completionItem = (method.HasParamArrayArg, documentation, prefixParts, separatorParts, suffixParts, parameters, mainDescription)
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
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask

                    let triggerTypedChar = 
                        if triggerInfo.TriggerCharacter.HasValue && triggerInfo.TriggerReason = SignatureHelpTriggerReason.TypeCharCommand then
                            Some triggerInfo.TriggerCharacter.Value
                        else None

                    let! methods = FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(checkerProvider.Checker, documentationBuilder, sourceText, position, options, triggerTypedChar, document.FilePath, textVersion.GetHashCode())
                    match methods with 
                    | None -> return null
                    | Some (results,applicableSpan,argumentIndex,argumentCount,argumentName) -> 
                        let items = 
                            results 
                            |> Array.map (fun (hasParamArrayArg, doc, prefixParts, separatorParts, suffixParts, parameters, descriptionParts) ->
                                    let parameters = parameters 
                                                     |> Array.map (fun (paramName, isOptional, paramDoc, displayParts) -> 
                                                        SignatureHelpParameter(paramName,isOptional,documentationFactory=(fun _ -> paramDoc :> seq<_>),displayParts=displayParts))
                                    SignatureHelpItem(isVariadic=hasParamArrayArg, documentationFactory=(fun _ -> doc :> seq<_>),prefixParts=prefixParts,separatorParts=separatorParts,suffixParts=suffixParts,parameters=parameters,descriptionParts=descriptionParts))

                        return SignatureHelpItems(items,applicableSpan,argumentIndex,argumentCount,Option.toObj argumentName)
                | None -> 
                    return null 
              with ex -> 
                Assert.Exception(ex)
                return null
            } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

open System.ComponentModel.Composition
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.SignatureHelp.Presentation

// Enable colorized signature help for F# buffers

[<Export(typeof<IClassifierProvider>)>]
[<ContentType(FSharpCommonConstants.FSharpSignatureHelpContentTypeName)>]
type internal FSharpSignatureHelpClassifierProvider [<ImportingConstructor>] (typeMap) =
    interface IClassifierProvider with
        override __.GetClassifier (buffer: ITextBuffer) =
            buffer.Properties.GetOrCreateSingletonProperty(fun _ -> SignatureHelpClassifier(buffer, typeMap) :> _)
