﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.SignatureHelp

open Microsoft.VisualStudio.Shell

open Microsoft.VisualStudio.FSharp.Editor.Logging

open FSharp.Compiler.Layout
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree

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

module internal SynExprAppLocationsImpl =
    let rec private searchSynArgExpr traverseSynExpr expr ranges =
        match expr with
        | SynExpr.Const(SynConst.Unit, _) ->
            None, None

        | SynExpr.Paren(SynExpr.Tuple (_, exprs, _commas, _tupRange), _, _, _parenRange) ->
            let rec loop (exprs: SynExpr list) ranges =
                match exprs with
                | [] -> ranges
                | h::t ->
                    loop t (h.Range :: ranges)

            let res = loop exprs ranges
            Some (res), None

        | SynExpr.Paren(SynExpr.Paren(_, _, _, _) as synExpr, _, _, _parenRange) -> 
            let r, _cacheOpt = searchSynArgExpr traverseSynExpr synExpr ranges
            r, None

        | SynExpr.Paren(SynExpr.App (_, _isInfix, _, _, _range), _, _, parenRange) ->
            Some (parenRange :: ranges), None

        | e -> 
            let inner = traverseSynExpr e
            match inner with
            | None ->
                Some (e.Range :: ranges), Some inner
            | _ -> None, Some inner

    let getAllCurriedArgsAtPosition pos parseTree =
        AstTraversal.Traverse(pos, parseTree, { new AstTraversal.AstVisitorBase<_>() with
            member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                match expr with
                | SynExpr.App (_exprAtomicFlag, _isInfix, funcExpr, argExpr, range) when posEq pos range.Start ->
                    let isInfixFuncExpr =
                        match funcExpr with
                        | SynExpr.App (_, isInfix, _, _, _) -> isInfix
                        | _ -> false

                    if isInfixFuncExpr then
                        traverseSynExpr funcExpr
                    else
                        let workingRanges =
                            match traverseSynExpr funcExpr with
                            | Some ranges -> ranges
                            | None -> []

                        let xResult, cacheOpt = searchSynArgExpr traverseSynExpr argExpr workingRanges
                        match xResult, cacheOpt with
                        | Some ranges, _ -> Some ranges
                        | None, Some cache -> cache
                        | _ -> traverseSynExpr argExpr
                | _ -> defaultTraverse expr })
        |> Option.map List.rev

[<AutoOpen>]
module Poop =
    type FSharpParseFileResults with
        member scope.GetAllArgumentsForFunctionApplicationAtPostion pos =
            match scope.ParseTree with
            | Some input -> SynExprAppLocationsImpl.getAllCurriedArgsAtPosition pos input
            | None -> None
        
        member scope.TryRangeOfFunctionInApplication pos =
            match scope.ParseTree with
            | Some input ->
                AstTraversal.Traverse(pos, input, { new AstTraversal.AstVisitorBase<_>() with
                    member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                        match expr with
                        | SynExpr.App (_flag, false, funcExpr, _argExpr, range) when rangeContainsPos range pos ->
                            Some funcExpr.Range
                        | _ -> defaultTraverse expr
                })
            | None -> None

        member scope.IsPosWithinAFunctionApplication pos =
            match scope.ParseTree with
            | Some input ->
                let result =
                    AstTraversal.Traverse(pos, input, { new AstTraversal.AstVisitorBase<_>() with
                        member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                            match expr with
                            | SynExpr.App (_flag, _isInfix, _funcExpr, _argExpr, range) when rangeContainsPos range pos ->
                                Some range
                            | _ -> defaultTraverse expr
                    })
                result.IsSome

            | None -> false

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

    static let joinWithLineBreaks segments =
        let lineBreak = TaggedTextOps.Literals.lineBreak
        match segments |> List.filter (Seq.isEmpty >> not) with
        | [] -> Seq.empty
        | xs -> xs |> List.reduce (fun acc elem -> seq { yield! acc; yield lineBreak; yield! elem })

    // Unit-testable core routine
    static member internal ProvideMethodsAsyncAux(checker: FSharpChecker, documentationBuilder: IDocumentationBuilder, sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, triggerIsTypedChar: char, filePath: string, textVersionHash: int) =
        async {
            let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToFSharpSourceText(), options, userOpName = userOpName)
            match checkFileAnswer with
            | FSharpCheckFileAnswer.Aborted ->
                return None
            | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 
                let textLines = sourceText.Lines
                let caretLinePos = textLines.GetLinePosition(caretPosition)
                let caretLineColumn = caretLinePos.Character

                // Get the parameter locations
                let paramLocations = parseResults.FindNoteworthyParamInfoLocations(Pos.fromZ caretLinePos.Line caretLineColumn)
                match paramLocations with
                | None ->
                    return None // no locations = no help
                | Some nwpl -> 
                    let names = nwpl.LongId
                    let lidEnd = nwpl.LongIdEndLocation

                    // Get the methods
                    let methodGroup = checkFileResults.GetMethods(lidEnd.Line, lidEnd.Column, "", Some names)

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
                    | '<' | '(' | ',' when not (tupleEnds |> Array.exists (fun lp -> lp.Character = caretLineColumn))  -> 
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
                                // Because 'TextSpan.Contains' only succeeds if 'TextSpan.Start <= caretPosition < TextSpan.End' is true,
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
                        let results =
                            [|
                                for method in methods do
                                    // Create the documentation. Note, do this on the background thread, since doing it in the documentationBuild fails to build the XML index
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

                                                // FSROSLYNTODO: compute the proper help text for parameters, c.f. AppendParameter in XmlDocumentation.fs
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
                        return Some data
        }

    static member internal ProvideParametersAsyncAux
        (
            document: Document,
            defines: string list,
            checker: FSharpChecker,
            documentationBuilder: IDocumentationBuilder,
            sourceText: SourceText,
            caretPosition: int,
            options: FSharpProjectOptions,
            filePath: string,
            textVersionHash: int
        ) =
            asyncMaybe {
                let adjustedColumnInSource =
                    let rec loop s c =
                        if String.IsNullOrWhiteSpace(s) then
                            loop (sourceText.GetSubText(c - 1).ToString()) (c - 1)
                        else
                            c
                    loop (sourceText.GetSubText(caretPosition).ToString()) caretPosition

                // For function applications, we need to offset this by 1 because
                // otherwise it won't display. TODO better explanation lmao
                let tooltipPosition = caretPosition - 1

                let perfOptions = document.FSharpOptions.LanguageServicePerformance
                let textLine = sourceText.Lines.GetLineFromPosition(adjustedColumnInSource)
                let textLinePos = sourceText.Lines.GetLinePosition(adjustedColumnInSource)
                let pos = mkPos (Line.fromZ textLinePos.Line) textLinePos.Character
                let textLinePos = sourceText.Lines.GetLinePosition(adjustedColumnInSource)
                let fcsTextLineNumber = Line.fromZ textLinePos.Line
                let! parseResults, _, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, options, perfOptions, userOpName = userOpName)
                
                let! possibleFuncPosition =
                    maybe {
                        if parseResults.IsPosWithinAFunctionApplication pos then
                            let! funcRange = parseResults.TryRangeOfFunctionInApplication pos 
                            let! funcSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, funcRange)
                            return funcSpan.Start
                        else
                            return adjustedColumnInSource
                    }

                if parseResults.IsPosWithinAFunctionApplication pos then
                    let! funcRange = parseResults.TryRangeOfFunctionInApplication pos
                    let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, possibleFuncPosition, filePath, defines, SymbolLookupKind.Greedy, false, false)
                    let! funcSymbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)
                    match funcSymbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsFunction ->
                        let tooltip = checkFileResults.GetStructuredToolTipText(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, FSharpTokenTag.IDENT)
                        match tooltip with
                        | FSharpToolTipText []
                        | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                        | _ ->
                            let numDefinedArgs = mfv.CurriedParameterGroups |> Seq.concat |> Seq.length
                            let! curriedArgsInSource = parseResults.GetAllArgumentsForFunctionApplicationAtPostion funcRange.Start

                            // Don't show past the last one. Also offset by one, because we get sig help when we have none defined.
                            do! Option.guard (curriedArgsInSource.Length <= (numDefinedArgs - 1))

                            let! argumentIndex =
                                curriedArgsInSource
                                |> List.indexed
                                |> List.tryFind(fun (_, argRanage) -> rangeContainsPos argRanage pos)
                                |> Option.map (fun (index, _) -> index + 1) // TODO: explain why offsetting here

                            for x in curriedArgsInSource do
                                logInfof "%A" x

                            logInfof "Index: %d" argumentIndex

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

                            let fsharpDocs = joinWithLineBreaks [documentation; typeParameterMap; usage; exceptions]
                        
                            let docs = ResizeArray()
                            for fsharpDoc in fsharpDocs do
                                RoslynHelpers.CollectTaggedText docs fsharpDoc

                            let parts = ResizeArray()
                            for part in mainDescription do
                                RoslynHelpers.CollectTaggedText parts part

                            let args = ResizeArray()
                            for group in mfv.CurriedParameterGroups do
                                for argument in group do
                                    let ret =
                                        if argument.Type.HasTypeDefinition then
                                            argument.Type.TypeDefinition.DisplayName
                                        elif argument.Type.IsGenericParameter then
                                            "'" + argument.Type.GenericParameter.DisplayName
                                        else
                                            logInfof "uhhh %A" mfv.ReturnParameter
                                            ""

                                    let display =
                                        [|
                                            TaggedText(TextTags.Local, argument.DisplayName)
                                            TaggedText(TextTags.Punctuation, ":")
                                            TaggedText(TextTags.Space, " ")
                                            TaggedText(TextTags.Class, ret)
                                        |]

                                    let info =
                                        { ParameterName = argument.DisplayName
                                          IsOptional = false
                                          CanonicalTypeTextForSorting = argument.FullName
                                          Documentation = ResizeArray()
                                          DisplayParts = ResizeArray(display) }

                                    args.Add(info)

                            let prefixParts =
                                [|
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

                            let ret =
                                if mfv.ReturnParameter.Type.HasTypeDefinition then
                                    mfv.ReturnParameter.Type.TypeDefinition.DisplayName
                                elif mfv.ReturnParameter.Type.IsGenericParameter then
                                    "'" + mfv.ReturnParameter.Type.GenericParameter.DisplayName
                                else
                                    logInfof "uhhh %A" mfv.ReturnParameter
                                    ""

                            let suffixParts =
                                [|
                                    TaggedText(TextTags.Space, " ")
                                    TaggedText(TextTags.Operator, "->")
                                    TaggedText(TextTags.Space, " ")
                                    TaggedText(TextTags.Class, ret)
                                |]

                            let sigHelpItem =
                                { HasParamArrayArg = false
                                  Documentation = docs
                                  PrefixParts = prefixParts
                                  SeparatorParts = separatorParts
                                  SuffixParts = suffixParts
                                  Parameters = args.ToArray()
                                  MainDescription = ResizeArray() }

                            let data =
                                { SignatureHelpItems = [| sigHelpItem |]
                                  ApplicableSpan = TextSpan(tooltipPosition, 1)
                                  ArgumentIndex = argumentIndex
                                  ArgumentCount = args.Count
                                  ArgumentName = None }

                            return! Some data
                    | _ ->
                        return! None
                else
                    let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, possibleFuncPosition, filePath, defines, SymbolLookupKind.Greedy, false, false)
                    let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)
                    match symbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsFunction ->
                        let tooltip = checkFileResults.GetStructuredToolTipText(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, FSharpTokenTag.IDENT)
                        match tooltip with
                        | FSharpToolTipText []
                        | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                        | _ ->
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

                            let fsharpDocs = joinWithLineBreaks [documentation; typeParameterMap; usage; exceptions]
                        
                            let docs = ResizeArray()
                            for fsharpDoc in fsharpDocs do
                                RoslynHelpers.CollectTaggedText docs fsharpDoc

                            let parts = ResizeArray()
                            for part in mainDescription do
                                RoslynHelpers.CollectTaggedText parts part

                            let args = ResizeArray()
                            for group in mfv.CurriedParameterGroups do
                                for argument in group do
                                    let ret =
                                        if argument.Type.HasTypeDefinition then
                                            argument.Type.TypeDefinition.DisplayName
                                        elif argument.Type.IsGenericParameter then
                                            "'" + argument.Type.GenericParameter.DisplayName
                                        else
                                            logInfof "uhhh %A" mfv.ReturnParameter
                                            ""

                                    let display =
                                        [|
                                            TaggedText(TextTags.Local, argument.DisplayName)
                                            TaggedText(TextTags.Punctuation, ":")
                                            TaggedText(TextTags.Space, " ")
                                            TaggedText(TextTags.Class, ret)
                                        |]

                                    let info =
                                        { ParameterName = argument.DisplayName
                                          IsOptional = false
                                          CanonicalTypeTextForSorting = argument.FullName
                                          Documentation = ResizeArray()
                                          DisplayParts = ResizeArray(display) }

                                    args.Add(info)

                            let prefixParts =
                                [|
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

                            let ret =
                                if mfv.ReturnParameter.Type.HasTypeDefinition then
                                    mfv.ReturnParameter.Type.TypeDefinition.DisplayName
                                elif mfv.ReturnParameter.Type.IsGenericParameter then
                                    "'" + mfv.ReturnParameter.Type.GenericParameter.DisplayName
                                else
                                    logInfof "uhhh %A" mfv.ReturnParameter
                                    ""

                            let suffixParts =
                                [|
                                    TaggedText(TextTags.Space, " ")
                                    TaggedText(TextTags.Operator, "->")
                                    TaggedText(TextTags.Space, " ")
                                    TaggedText(TextTags.Class, ret)
                                |]

                            let sigHelpItem =
                                { HasParamArrayArg = false
                                  Documentation = docs
                                  PrefixParts = prefixParts
                                  SeparatorParts = separatorParts
                                  SuffixParts = suffixParts
                                  Parameters = args.ToArray()
                                  MainDescription = ResizeArray() }

                            let data =
                                { SignatureHelpItems = [| sigHelpItem |]
                                  ApplicableSpan = TextSpan(tooltipPosition, 1)
                                  ArgumentIndex = 0
                                  ArgumentCount = args.Count
                                  ArgumentName = None }

                            return! Some data
                    | _ ->
                        return! None
            }

    interface IFSharpSignatureHelpProvider with
        member _.IsTriggerCharacter(c) = c ='(' || c = '<' || c = ',' || c = ' '
        member _.IsRetriggerCharacter(c) = c = ')' || c = '>' || c = '=' || c = ' '

        member _.GetItemsAsync(document, position, triggerInfo, cancellationToken) = 
            asyncMaybe {
                try
                    let! _, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                    let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)
                    let! sourceText = document.GetTextAsync(cancellationToken)
                    let! textVersion = document.GetTextVersionAsync(cancellationToken)

                    let! triggerTypedChar = 
                        if triggerInfo.TriggerCharacter.HasValue && triggerInfo.TriggerReason = FSharpSignatureHelpTriggerReason.TypeCharCommand then
                            Some triggerInfo.TriggerCharacter.Value
                        else None
                    
                    if triggerTypedChar = ' ' then
                        let! signatureHelpData =
                            FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
                                document,
                                defines,
                                checkerProvider.Checker,
                                documentationBuilder,
                                sourceText,
                                position,
                                projectOptions,
                                document.FilePath,
                                textVersion.GetHashCode())
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
                                    
                        return FSharpSignatureHelpItems(
                            items,
                            signatureHelpData.ApplicableSpan,
                            signatureHelpData.ArgumentIndex,
                            signatureHelpData.ArgumentCount,
                            Option.toObj signatureHelpData.ArgumentName)
                    else
                        let! signatureHelpData =
                            FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                                checkerProvider.Checker,
                                documentationBuilder,
                                sourceText,
                                position,
                                projectOptions,
                                triggerTypedChar,
                                document.FilePath,
                                textVersion.GetHashCode())
                        let items = 
                            signatureHelpData.SignatureHelpItems 
                            |> Array.map (fun item ->
                                    let parameters =
                                        item.Parameters 
                                        |> Array.map (fun paramInfo -> 
                                            FSharpSignatureHelpParameter(
                                                paramInfo.ParameterName,
                                                paramInfo.IsOptional,
                                                documentationFactory=(fun _ -> paramInfo.Documentation :> seq<_>),
                                                displayParts=paramInfo.DisplayParts))
                            
                                    FSharpSignatureHelpItem(
                                        isVariadic=item.HasParamArrayArg,
                                        documentationFactory=(fun _ -> item.Documentation :> seq<_>),
                                        prefixParts=item.PrefixParts,
                                        separatorParts=item.SeparatorParts,
                                        suffixParts=item.SuffixParts,
                                        parameters=parameters,
                                        descriptionParts=item.MainDescription))

                        return FSharpSignatureHelpItems(
                            items,
                            signatureHelpData.ApplicableSpan,
                            signatureHelpData.ArgumentIndex,
                            signatureHelpData.ArgumentCount,
                            Option.toObj signatureHelpData.ArgumentName)
                with ex -> 
                    Assert.Exception(ex)
                    return! None
            } 
            |> Async.map Option.toObj
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
