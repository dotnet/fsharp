// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.SignatureHelp

open Microsoft.VisualStudio.Shell

open FSharp.Compiler
open FSharp.Compiler.Layout
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices

type SignatureHelpParameterInfo =
    { ParameterName: string
      IsOptional: bool
      CanonicalTypeTextForSorting: string
      Documentation: ResizeArray<Microsoft.CodeAnalysis.TaggedText>
      DisplayParts: ResizeArray<Microsoft.CodeAnalysis.TaggedText> }

type SignatureHelpItem =
    { HasParamArrayArg: bool
      Documentation: ResizeArray<Microsoft.CodeAnalysis.TaggedText>
      PrefixParts: Microsoft.CodeAnalysis.TaggedText[]
      SeparatorParts: Microsoft.CodeAnalysis.TaggedText[]
      SuffixParts: Microsoft.CodeAnalysis.TaggedText[]
      Parameters: SignatureHelpParameterInfo[]
      MainDescription: ResizeArray<Microsoft.CodeAnalysis.TaggedText> }

type SignatureHelpData =
    { SignatureHelpItems: SignatureHelpItem[]
      ApplicableSpan: TextSpan
      ArgumentIndex: int
      ArgumentCount: int
      ArgumentName: string option }

[<Shared>]
[<Export(typeof<IFSharpSignatureHelpProvider>)>]
type internal FSharpSignatureHelpProvider 
    [<ImportingConstructor>]
    (
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    static let userOpName = "SignatureHelpProvider"
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(serviceProvider.XMLMemberIndexService)

    static let oneColAfter (lp: LinePosition) = LinePosition(lp.Line,lp.Character+1)
    static let oneColBefore (lp: LinePosition) = LinePosition(lp.Line,max 0 (lp.Character-1))

    static member internal ProvideMethodsAsyncAux
        (
            caretLinePos: LinePosition,
            caretLineColumn: int,
            paramLocations: FSharpNoteworthyParamInfoLocations,
            checkFileResults: FSharpCheckFileResults,
            documentationBuilder: IDocumentationBuilder,
            sourceText: SourceText,
            caretPosition: int,
            triggerIsTypedChar: char option
        ) =
        asyncMaybe {
            let textLines = sourceText.Lines
            let names = paramLocations.LongId
            let lidEnd = paramLocations.LongIdEndLocation

            let methodGroup = checkFileResults.GetMethods(lidEnd.Line, lidEnd.Column, "", Some names)

            let methods = methodGroup.Methods

            do! Option.guard (methods.Length > 0 && not(methodGroup.MethodName.EndsWith("> )")))

            let isStaticArgTip =
                let parenLine, parenCol = Pos.toZ paramLocations.OpenParenLocation 
                assert (parenLine < textLines.Count)
                let parenLineText = textLines.[parenLine].ToString()
                parenCol < parenLineText.Length && parenLineText.[parenCol] = '<'

            let filteredMethods =
                [|
                    for m in methods do 
                        if (isStaticArgTip && m.StaticParameters.Length > 0) ||
                            (not isStaticArgTip && m.HasParameters) then   // need to distinguish TP<...>(...)  angle brackets tip from parens tip
                            m
                |]

            do! Option.guard (filteredMethods.Length > 0)

            let posToLinePosition pos = 
                let (l,c) = Pos.toZ  pos
                let result = LinePosition(l,c)
                let lastPosInDocument = textLines.GetLinePosition(textLines.[textLines.Count-1].End)
                if lastPosInDocument.CompareTo(result) > 0 then result else lastPosInDocument

            let startPos = paramLocations.LongIdStartLocation |> posToLinePosition
            let endPos = 
                let last = paramLocations.TupleEndLocations.[paramLocations.TupleEndLocations.Length-1] |> posToLinePosition
                (if paramLocations.IsThereACloseParen then oneColBefore last else last)  

            assert (startPos.CompareTo(endPos) <= 0)

            let applicableSpan = 
                textLines.GetTextSpan(LinePositionSpan(startPos, endPos))

            let startOfArgs = paramLocations.OpenParenLocation |> posToLinePosition |> oneColAfter 

            let tupleEnds = 
                [|
                    startOfArgs
                    for i in 0..paramLocations.TupleEndLocations.Length-2 do
                        paramLocations.TupleEndLocations.[i] |> posToLinePosition
                    endPos 
                |]

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
            | Some('<' | '(' | ',') when not (tupleEnds |> Array.exists (fun lp -> lp.Character = caretLineColumn))  -> 
                return! None // comma or paren at wrong location = remove help display
            | _ -> 

                // Compute the argument index by working out where the caret is between the various commas.
                let argumentIndex = 
                    let computedTextSpans =
                        tupleEnds 
                        |> Array.pairwise 
                        |> Array.map (fun (lp1, lp2) -> textLines.GetTextSpan(LinePositionSpan(lp1, lp2)))
                
                    match (computedTextSpans|> Array.tryFindIndex (fun t -> t.Contains(caretPosition))) with 
                    | None -> 
                        // Because 'TextSpan.Contains' only succeeds if 'TextSpan.Start <= caretPosition < TextSpan.End' is true,
                        // we need to check if the caret is at the very last position in the TextSpan.
                        //
                        // We default to 0, which is the first argument, if the caret position was nowhere to be found.
                        if computedTextSpans.[computedTextSpans.Length-1].End = caretPosition then
                            computedTextSpans.Length-1 
                        else 0
                    | Some n -> n
         
                let argumentCount = 
                    match paramLocations.TupleEndLocations.Length with 
                    | 1 when caretLinePos.Character = startOfArgs.Character -> 0  // count "WriteLine(" as zero arguments
                    | n -> n

                // Compute the current argument name if it is named.
                let argumentName = 
                    if argumentIndex < paramLocations.NamedParamNames.Length then 
                        paramLocations.NamedParamNames.[argumentIndex] 
                    else 
                        None

                let results =
                    [|
                        for method in methods do
                            let mainDescription = ResizeArray()
                            let documentation = ResizeArray()
                            XmlDocumentation.BuildMethodOverloadTipText(
                                documentationBuilder,
                                RoslynHelpers.CollectTaggedText mainDescription,
                                RoslynHelpers.CollectTaggedText documentation,
                                method.StructuredDescription, false)

                            let parameters = 
                                let parameters = if isStaticArgTip then method.StaticParameters else method.Parameters
                                [|
                                    for p in parameters do 
                                        let doc = ResizeArray()
                                        let parts = ResizeArray()
                                        XmlDocumentation.BuildMethodParamText(documentationBuilder, RoslynHelpers.CollectTaggedText doc, method.XmlDoc, p.ParameterName)
                                        renderL (taggedTextListR (RoslynHelpers.CollectTaggedText parts)) p.StructuredDisplay |> ignore
                                        { ParameterName = p.ParameterName
                                          IsOptional = p.IsOptional
                                          CanonicalTypeTextForSorting = p.CanonicalTypeTextForSorting
                                          Documentation = doc
                                          DisplayParts = parts }
                                |]

                            let prefixParts = 
                                [| TaggedText(TextTags.Method, methodGroup.MethodName);  
                                   TaggedText(TextTags.Punctuation, (if isStaticArgTip then "<" else "(")) |]

                            let separatorParts = [| TaggedText(TextTags.Punctuation, ","); TaggedText(TextTags.Space, " ") |]
                            let suffixParts = [| TaggedText(TextTags.Punctuation, (if isStaticArgTip then ">" else ")")) |]

                            { HasParamArrayArg = method.HasParamArrayArg
                              Documentation = documentation
                              PrefixParts = prefixParts
                              SeparatorParts = separatorParts
                              SuffixParts = suffixParts
                              Parameters = parameters
                              MainDescription = mainDescription }
                        |]

                let data =
                    { SignatureHelpItems = results
                      ApplicableSpan = applicableSpan
                      ArgumentIndex = argumentIndex
                      ArgumentCount = argumentCount
                      ArgumentName = argumentName }

                return! Some data
    }

    static member internal ProvideParametersAsyncAux
        (
            parseResults: FSharpParseFileResults,
            checkFileResults: FSharpCheckFileResults,
            documentId: DocumentId,
            defines: string list,
            documentationBuilder: IDocumentationBuilder,
            sourceText: SourceText,
            caretPosition: int,
            filePath: string
        ) =
        asyncMaybe {
            // Backtrack to find a non-whitespace character to get curried arg infos (if present) and a symbol to inspect.
            let adjustedColumnInSource =
                let rec loop s c =
                    if String.IsNullOrWhiteSpace(s.ToString()) then
                        loop (sourceText.GetSubText(c - 1)) (c - 1)
                    else
                        c
                let startText =
                    if caretPosition = sourceText.Length then
                        sourceText.GetSubText(caretPosition)
                    else
                        sourceText.GetSubText(TextSpan(caretPosition, 1))
                
                loop startText caretPosition

            let textLine = sourceText.Lines.GetLineFromPosition(adjustedColumnInSource)
            let textLinePos = sourceText.Lines.GetLinePosition(adjustedColumnInSource)
            let pos = mkPos (Line.fromZ textLinePos.Line) textLinePos.Character
            let textLinePos = sourceText.Lines.GetLinePosition(adjustedColumnInSource)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
                
            let! possibleApplicableSymbolEndColumn =
                maybe {
                    if parseResults.IsPosContainedInApplication pos then
                        let! funcRange = parseResults.TryRangeOfFunctionOrMethodBeingApplied pos 
                        let! funcSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, funcRange)
                        return funcSpan.End
                    else
                        return adjustedColumnInSource
                }

            let! lexerSymbol = Tokenizer.getSymbolAtPosition(documentId, sourceText, possibleApplicableSymbolEndColumn, filePath, defines, SymbolLookupKind.Greedy, false, false)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)

            let isValid (mfv: FSharpMemberOrFunctionOrValue) =
                not (PrettyNaming.IsOperatorName mfv.DisplayName) &&
                not mfv.IsProperty &&
                mfv.CurriedParameterGroups.Count > 0

            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when isValid mfv ->
                let tooltip = checkFileResults.GetStructuredToolTipText(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, FSharpTokenTag.IDENT)
                match tooltip with
                | FSharpToolTipText []
                | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                | _ ->                    
                    let possiblePipelineIdent = parseResults.TryIdentOfPipelineContainingPosAndNumArgsApplied symbolUse.RangeAlternate.Start
                    let numArgsAlreadyApplied =
                        match possiblePipelineIdent with
                        | None -> 0
                        | Some (_, numArgsApplied) -> numArgsApplied

                    let definedArgs = mfv.CurriedParameterGroups |> Seq.concat |> Array.ofSeq
                        
                    let numDefinedArgs = definedArgs.Length

                    let curriedArgsInSource =
                        parseResults.GetAllArgumentsForFunctionApplicationAtPostion symbolUse.RangeAlternate.Start
                        |> Option.defaultValue []
                        |> Array.ofList

                    do! Option.guard (numDefinedArgs >= curriedArgsInSource.Length)

                    (*
                       Calculate the argument index for fun and profit! It's a doozy...
                   
                       Firstly, we need to use the caret position unlike before.
                   
                       If the caret position is exactly in range of an existing argument, pick its index.
                   
                       The rest answers the question of, "what is the NEXT index to show?", because
                       when you're not cycling through parameters with the caret, you're typing,
                       and you want to know what the next argument should be.
                   
                       A possibility is you've deleted a parameter and want to enter a new one that
                       corresponds to the argument you're "at". We need to find the correct next index.
                       This could also correspond to an existing argument application. Buuuuuut that's okay.
                       If you want the "used to be 3rd arg, but is now 2nd arg" to remain, when you cycle
                       past the "now 2nd arg", it will calculate the 3rd arg as the next argument.
                   
                       If none of that applies, then we apply the magic of arithmetic
                       to find the next index if we're not at the max defined args for the application.
                       Otherwise, we're outa here!
                   *)
                    let! argumentIndex =
                        let caretTextLinePos = sourceText.Lines.GetLinePosition(caretPosition)
                        let caretPos = mkPos (Line.fromZ caretTextLinePos.Line) caretTextLinePos.Character

                        let possibleExactIndex =
                            curriedArgsInSource
                            |> Array.tryFindIndex(fun argRange -> rangeContainsPos argRange caretPos)

                        match possibleExactIndex with
                        | Some index -> Some index
                        | None ->
                            let possibleNextIndex =
                                curriedArgsInSource
                                |> Array.tryFindIndex(fun argRange -> Range.posGeq argRange.Start caretPos)

                            match possibleNextIndex with
                            | Some index -> Some index
                            | None ->
                                if numDefinedArgs - numArgsAlreadyApplied > curriedArgsInSource.Length then
                                    Some (numDefinedArgs - (numDefinedArgs - curriedArgsInSource.Length))
                                else
                                    None

                    let mainDescription, documentation, typeParameterMap, usage, exceptions =
                        ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()

                    XmlDocumentation.BuildDataTipText(
                        documentationBuilder,
                        mainDescription.Add,
                        documentation.Add,
                        typeParameterMap.Add,
                        usage.Add,
                        exceptions.Add,
                        tooltip)

                    let fsharpDocs = RoslynHelpers.joinWithLineBreaks [documentation; typeParameterMap; usage; exceptions]
                                       
                    let docs = ResizeArray()
                    for fsharpDoc in fsharpDocs do
                        RoslynHelpers.CollectTaggedText docs fsharpDoc

                    let parts = ResizeArray()
                    for part in mainDescription do
                        RoslynHelpers.CollectTaggedText parts part

                    let displayArgs = ResizeArray()

                    // Offset by 1 here until we support reverse indexes in this codebase
                    for argument in definedArgs.[.. definedArgs.Length - 1 - numArgsAlreadyApplied] do
                        let taggedText = ResizeArray()
                        let tt = ResizeArray()
                        let layout = argument.Type.FormatLayout symbolUse.DisplayContext
                        Layout.renderL (Layout.taggedTextListR taggedText.Add) layout |> ignore
                        for part in taggedText do
                            RoslynHelpers.CollectTaggedText tt part

                        let display =
                            [|
                                TaggedText(TextTags.Local, argument.DisplayName)
                                TaggedText(TextTags.Punctuation, ":")
                                TaggedText(TextTags.Space, " ")
                            |]
                            |> ResizeArray

                        display.AddRange(tt)

                        let info =
                            { ParameterName = argument.DisplayName
                              IsOptional = false
                              CanonicalTypeTextForSorting = argument.FullName
                              Documentation = ResizeArray()
                              DisplayParts = display }

                        displayArgs.Add(info)

                    do! Option.guard (displayArgs.Count > 0)

                    let prefixParts =
                        [|
                            if mfv.IsMember then
                                TaggedText(TextTags.Keyword, "member")
                            else
                                TaggedText(TextTags.Keyword, "val")
                            TaggedText(TextTags.Space, " ")
                            TaggedText(TextTags.Method, mfv.DisplayName)
                            TaggedText(TextTags.Punctuation, ":")
                            TaggedText(TextTags.Space, " ")
                        |]

                    let separatorParts =
                        [|
                            TaggedText(TextTags.Space, " ")
                            TaggedText(TextTags.Operator, "->")
                            TaggedText(TextTags.Space, " ")
                        |]

                    let sigHelpItem =
                        { HasParamArrayArg = false
                          Documentation = docs
                          PrefixParts = prefixParts
                          SeparatorParts = separatorParts
                          SuffixParts = [||]
                          Parameters = displayArgs.ToArray()
                          MainDescription = ResizeArray() }

                    let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)

                    let data =
                        { SignatureHelpItems = [| sigHelpItem |]
                          ApplicableSpan = TextSpan(symbolSpan.End, caretPosition - symbolSpan.End)
                          ArgumentIndex = argumentIndex
                          ArgumentCount = displayArgs.Count
                          ArgumentName = None }

                    return! Some data
            | _ ->
                return! None
        }

    static member ProvideSignatureHelp
        (
            document: Document,
            defines: string list,
            checker: FSharpChecker,
            documentationBuilder: IDocumentationBuilder,
            sourceText: SourceText,
            caretPosition: int,
            options: FSharpProjectOptions,
            filePath: string,
            textVersionHash: int,
            triggerTypedChar: char option
        ) =
        asyncMaybe {
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(caretPosition)
            let caretLineColumn = caretLinePos.Character
            let perfOptions = document.FSharpOptions.LanguageServicePerformance

            let! parseResults, _, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, options, perfOptions, userOpName = userOpName)
            match parseResults.FindNoteworthyParamInfoLocations(Pos.fromZ caretLinePos.Line caretLineColumn) with
            | Some paramInfoLocations ->
                return!
                    FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                        caretLinePos,
                        caretLineColumn,
                        paramInfoLocations,
                        checkFileResults,
                        documentationBuilder,
                        sourceText,
                        caretPosition,
                        triggerTypedChar)
            | None ->
                return!
                    FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
                        parseResults,
                        checkFileResults,
                        document.Id,
                        defines,
                        documentationBuilder,
                        sourceText,
                        caretPosition,
                        filePath)
        }

    interface IFSharpSignatureHelpProvider with
        member _.IsTriggerCharacter(c) = c ='(' || c = '<' || c = ',' || c = ' '
        member _.IsRetriggerCharacter(c) = c = ')' || c = '>' || c = '='

        member _.GetItemsAsync(document, position, triggerInfo, cancellationToken) = 
            asyncMaybe {
                let! _, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let checker = checkerProvider.Checker

                let triggerTypedChar = 
                    if triggerInfo.TriggerCharacter.HasValue && triggerInfo.TriggerReason = FSharpSignatureHelpTriggerReason.TypeCharCommand then
                        Some triggerInfo.TriggerCharacter.Value
                    else None

                let! signatureHelpData =
                    FSharpSignatureHelpProvider.ProvideSignatureHelp(
                        document,
                        defines,
                        checker,
                        documentationBuilder,
                        sourceText,
                        position,
                        projectOptions,
                        document.FilePath,
                        textVersion.GetHashCode(),
                        triggerTypedChar)
                let items =
                    signatureHelpData.SignatureHelpItems
                    |> Array.map (fun item ->
                        let parameters =
                            item.Parameters
                            |> Array.map (fun paramInfo ->
                                FSharpSignatureHelpParameter(
                                    paramInfo.ParameterName,
                                    paramInfo.IsOptional,
                                    documentationFactory = (fun _ -> paramInfo.Documentation :> seq<_>),
                                    displayParts = paramInfo.DisplayParts))
                                            
                        FSharpSignatureHelpItem(
                            isVariadic=item.HasParamArrayArg,
                            documentationFactory=(fun _ -> item.Documentation :> seq<_>),
                            prefixParts=item.PrefixParts,
                            separatorParts=item.SeparatorParts,
                            suffixParts=item.SuffixParts,
                            parameters=parameters,
                            descriptionParts=item.MainDescription))
                                    
                return
                    FSharpSignatureHelpItems(
                        items,
                        signatureHelpData.ApplicableSpan,
                        signatureHelpData.ArgumentIndex,
                        signatureHelpData.ArgumentCount,
                        Option.toObj signatureHelpData.ArgumentName)
            } 
            |> Async.map Option.toObj
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
