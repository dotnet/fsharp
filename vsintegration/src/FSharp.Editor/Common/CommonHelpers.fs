// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal ISetThemeColors = abstract member SetColors: unit -> unit

[<RequireQualifiedAccess>]
type internal LexerSymbolKind =
    | Ident
    | Operator
    | Punctuation
    | GenericTypeParameter
    | StaticallyResolvedTypeParameter
    | Other

type internal LexerSymbol =
    { Kind: LexerSymbolKind
      /// Last part of `LongIdent`
      Ident: Ident
      /// All parts of `LongIdent`
      FullIsland: string list }
    member x.Range: Range.range = x.Ident.idRange

[<RequireQualifiedAccess>]
type internal SymbolLookupKind =
    /// Position must lay inside symbol range.
    | Precise
    /// Position may lay one column outside of symbol range to the right.
    | Greedy

module internal CommonHelpers =
    type private SourceLineData(lineStart: int, lexStateAtStartOfLine: FSharpTokenizerLexState, lexStateAtEndOfLine: FSharpTokenizerLexState, 
                                hashCode: int, classifiedSpans: IReadOnlyList<ClassifiedSpan>, tokens: FSharpTokenInfo list) =
        member val LineStart = lineStart
        member val LexStateAtStartOfLine = lexStateAtStartOfLine
        member val LexStateAtEndOfLine = lexStateAtEndOfLine
        member val HashCode = hashCode
        member val ClassifiedSpans = classifiedSpans
        member val Tokens = tokens
    
        member data.IsValid(textLine: TextLine) =
            data.LineStart = textLine.Start && 
            let lineContents = textLine.Text.ToString(textLine.Span)
            data.HashCode = lineContents.GetHashCode() 
    
    type private SourceTextData(approxLines: int) =
        let data = ResizeArray<SourceLineData option>(approxLines)
        let extendTo i =
            if i >= data.Count then 
                data.Capacity <- i + 1
                for j in data.Count .. i do
                    data.Add(None)
        member x.Item 
          with get (i:int) = extendTo  i; data.[i]
          and set (i:int) v = extendTo  i; data.[i] <- v
    
        member x.ClearFrom(n) =
            let mutable i = n
            while i < data.Count && data.[i].IsSome do
                data.[i] <- None
                i <- i + 1

    let private dataCache = ConditionalWeakTable<DocumentId, SourceTextData>()

    let internal compilerTokenToRoslynToken(colorKind: FSharpTokenColorKind) : string = 
        match colorKind with
        | FSharpTokenColorKind.Comment -> ClassificationTypeNames.Comment
        | FSharpTokenColorKind.Identifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Keyword -> ClassificationTypeNames.Keyword
        | FSharpTokenColorKind.String -> ClassificationTypeNames.StringLiteral
        | FSharpTokenColorKind.Text -> ClassificationTypeNames.Text
        | FSharpTokenColorKind.UpperIdentifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Number -> ClassificationTypeNames.NumericLiteral
        | FSharpTokenColorKind.InactiveCode -> ClassificationTypeNames.ExcludedCode 
        | FSharpTokenColorKind.PreprocessorKeyword -> ClassificationTypeNames.PreprocessorKeyword 
        | FSharpTokenColorKind.Operator -> ClassificationTypeNames.Operator
        | FSharpTokenColorKind.Punctuation -> ClassificationTypeNames.Punctuation
        | FSharpTokenColorKind.Default | _ -> ClassificationTypeNames.Text

    let private scanSourceLine(sourceTokenizer: FSharpSourceTokenizer, textLine: TextLine, lineContents: string, lexState: FSharpTokenizerLexState) : SourceLineData =
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.Text
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(lineContents)
        let tokens = ResizeArray()
            
        let scanAndColorNextToken(lineTokenizer: FSharpLineTokenizer, lexState: Ref<FSharpTokenizerLexState>) : Option<FSharpTokenInfo> =
            let tokenInfoOption, nextLexState = lineTokenizer.ScanToken(lexState.Value)
            lexState.Value <- nextLexState
            if tokenInfoOption.IsSome then
                let classificationType = compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
                for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                    Array.set colorMap i classificationType
                tokens.Add tokenInfoOption.Value
            tokenInfoOption

        let previousLexState = ref lexState
        let mutable tokenInfoOption = scanAndColorNextToken(lineTokenizer, previousLexState)
        while tokenInfoOption.IsSome do
            tokenInfoOption <- scanAndColorNextToken(lineTokenizer, previousLexState)

        let mutable startPosition = 0
        let mutable endPosition = startPosition
        let classifiedSpans = new List<ClassifiedSpan>()

        while startPosition < colorMap.Length do
            let classificationType = colorMap.[startPosition]
            endPosition <- startPosition
            while endPosition < colorMap.Length && classificationType = colorMap.[endPosition] do
                endPosition <- endPosition + 1
            let textSpan = new TextSpan(textLine.Start + startPosition, endPosition - startPosition)
            classifiedSpans.Add(new ClassifiedSpan(classificationType, textSpan))
            startPosition <- endPosition

        SourceLineData(textLine.Start, lexState, previousLexState.Value, lineContents.GetHashCode(), classifiedSpans, List.ofSeq tokens)

    let getColorizationData(documentKey: DocumentId, sourceText: SourceText, textSpan: TextSpan, fileName: string option, defines: string list, 
                            cancellationToken: CancellationToken) : List<ClassifiedSpan> =
         try
             let sourceTokenizer = FSharpSourceTokenizer(defines, fileName)
             let lines = sourceText.Lines
             // We keep incremental data per-document.  When text changes we correlate text line-by-line (by hash codes of lines)
             let sourceTextData = dataCache.GetValue(documentKey, fun key -> SourceTextData(lines.Count))
 
             let startLine = lines.GetLineFromPosition(textSpan.Start).LineNumber
             let endLine = lines.GetLineFromPosition(textSpan.End).LineNumber
             // Go backwards to find the last cached scanned line that is valid
             let scanStartLine = 
                 let mutable i = startLine
                 while i > 0 && (match sourceTextData.[i] with Some data -> not (data.IsValid(lines.[i])) | None -> true)  do
                     i <- i - 1
                 i
             // Rescan the lines if necessary and report the information
             let result = new List<ClassifiedSpan>()
             let mutable lexState = if scanStartLine = 0 then 0L else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine
 
             for i = scanStartLine to endLine do
                 cancellationToken.ThrowIfCancellationRequested()
                 let textLine = lines.[i]
                 let lineContents = textLine.Text.ToString(textLine.Span)
 
                 let lineData = 
                     // We can reuse the old data when 
                     //   1. the line starts at the same overall position
                     //   2. the hash codes match
                     //   3. the start-of-line lex states are the same
                     match sourceTextData.[i] with 
                     | Some data when data.IsValid(textLine) && data.LexStateAtStartOfLine = lexState -> 
                         data
                     | _ -> 
                         // Otherwise, we recompute
                         let newData = scanSourceLine(sourceTokenizer, textLine, lineContents, lexState)
                         sourceTextData.[i] <- Some newData
                         newData
                     
                 lexState <- lineData.LexStateAtEndOfLine
 
                 if startLine <= i then
                     result.AddRange(lineData.ClassifiedSpans |> Seq.filter(fun token ->
                         textSpan.Contains(token.TextSpan.Start) ||
                         textSpan.Contains(token.TextSpan.End - 1) ||
                         (token.TextSpan.Start <= textSpan.Start && textSpan.End <= token.TextSpan.End)))

             // If necessary, invalidate all subsequent lines after endLine
             if endLine < lines.Count - 1 then 
                 match sourceTextData.[endLine+1] with 
                 | Some data  -> 
                     if data.LexStateAtStartOfLine <> lexState then
                          sourceTextData.ClearFrom (endLine+1)
                 | None -> ()
             result
         with 
         | :? System.OperationCanceledException -> reraise()
         |  ex -> 
             Assert.Exception(ex)
             List<ClassifiedSpan>()

    type private DraftToken =
        { Kind: LexerSymbolKind
          Token: FSharpTokenInfo 
          RightColumn: int }
        static member inline Create kind token = 
            { Kind = kind; Token = token; RightColumn = token.LeftColumn + token.FullMatchedLength - 1 }
    
    /// Returns symbol at a given position.
    let private getSymbolFromTokens (fileName: string, tokens: FSharpTokenInfo list, linePos: LinePosition, lineStr: string, lookupKind: SymbolLookupKind) : LexerSymbol option =
        let isIdentifier t = t.CharClass = FSharpTokenCharKind.Identifier
        let isOperator t = t.ColorClass = FSharpTokenColorKind.Operator
        let isPunctuation t = t.ColorClass = FSharpTokenColorKind.Punctuation
    
        let inline (|GenericTypeParameterPrefix|StaticallyResolvedTypeParameterPrefix|Other|) (token: FSharpTokenInfo) =
            if token.Tag = FSharpTokenTag.QUOTE then GenericTypeParameterPrefix
            elif token.Tag = FSharpTokenTag.INFIX_AT_HAT_OP then
                 // The lexer return INFIX_AT_HAT_OP token for both "^" and "@" symbols.
                 // We have to check the char itself to distinguish one from another.
                 if token.FullMatchedLength = 1 && lineStr.[token.LeftColumn] = '^' then 
                    StaticallyResolvedTypeParameterPrefix
                 else Other
            else Other
       
        // Operators: Filter out overlapped operators (>>= operator is tokenized as three distinct tokens: GREATER, GREATER, EQUALS. 
        // Each of them has FullMatchedLength = 3. So, we take the first GREATER and skip the other two).
        //
        // Generic type parameters: we convert QUOTE + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "'type" which is tokenized as (QUOTE, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        //
        // Statically resolved type parameters: we convert INFIX_AT_HAT_OP + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "^type" which is tokenized as (INFIX_AT_HAT_OP, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        let tokens = 
            let tokensCount = tokens.Length
            tokens
            |> List.foldi (fun (acc, lastToken) index (token: FSharpTokenInfo) ->
                match lastToken with
                | Some t when token.LeftColumn <= t.RightColumn -> acc, lastToken
                | _ ->
                    let isLastToken = index = tokensCount - 1
                    match token with
                    | GenericTypeParameterPrefix when not isLastToken -> acc, Some (DraftToken.Create LexerSymbolKind.GenericTypeParameter token)
                    | StaticallyResolvedTypeParameterPrefix when not isLastToken -> acc, Some (DraftToken.Create LexerSymbolKind.StaticallyResolvedTypeParameter token)
                    | _ ->
                        let draftToken =
                            match lastToken with
                            | Some { Kind = LexerSymbolKind.GenericTypeParameter | LexerSymbolKind.StaticallyResolvedTypeParameter as kind } when isIdentifier token ->
                                DraftToken.Create kind { token with LeftColumn = token.LeftColumn - 1
                                                                    FullMatchedLength = token.FullMatchedLength + 1 }
                            // ^ operator                                                
                            | Some { Kind = LexerSymbolKind.StaticallyResolvedTypeParameter } ->
                                DraftToken.Create LexerSymbolKind.Operator { token with LeftColumn = token.LeftColumn - 1
                                                                                        FullMatchedLength = 1 }
                            | _ -> 
                                let kind = 
                                    if isOperator token then LexerSymbolKind.Operator 
                                    elif isIdentifier token then LexerSymbolKind.Ident 
                                    elif isPunctuation token then LexerSymbolKind.Punctuation
                                    else LexerSymbolKind.Other

                                DraftToken.Create kind token
                        draftToken :: acc, Some draftToken
                ) ([], None)
            |> fst
           
        // One or two tokens that in touch with the cursor (for "let x|(g) = ()" the tokens will be "x" and "(")
        let tokensUnderCursor = 
            let rightColumnCorrection = 
                match lookupKind with 
                | SymbolLookupKind.Precise -> 0
                | SymbolLookupKind.Greedy -> 1
            
            tokens |> List.filter (fun x -> x.Token.LeftColumn <= linePos.Character && (x.RightColumn + rightColumnCorrection) >= linePos.Character)
                
        // Select IDENT token. If failed, select OPERATOR token.
        tokensUnderCursor
        |> List.tryFind (fun { DraftToken.Kind = k } -> 
            match k with 
            | LexerSymbolKind.Ident 
            | LexerSymbolKind.GenericTypeParameter 
            | LexerSymbolKind.StaticallyResolvedTypeParameter -> true 
            | _ -> false) 
        |> Option.orElseWith (fun _ -> tokensUnderCursor |> List.tryFind (fun { DraftToken.Kind = k } -> k = LexerSymbolKind.Operator))
        |> Option.map (fun token ->
            let plid, _ = QuickParse.GetPartialLongNameEx(lineStr, token.RightColumn)
            let identStr = lineStr.Substring(token.Token.LeftColumn, token.Token.FullMatchedLength)
            { Kind = token.Kind
              Ident = 
                Ident 
                    (identStr, 
                     Range.mkRange 
                        fileName 
                        (Range.mkPos (linePos.Line + 1) token.Token.LeftColumn)
                        (Range.mkPos (linePos.Line + 1) (token.RightColumn + 1))) 
              FullIsland = plid @ [identStr] })

    let private getCachedSourceLineData(documentKey: DocumentId, sourceText: SourceText, position: int, fileName: string, defines: string list) = 
        let textLine = sourceText.Lines.GetLineFromPosition(position)
        let textLinePos = sourceText.Lines.GetLinePosition(position)
        let lineNumber = textLinePos.Line + 1 // FCS line number
        let sourceTokenizer = FSharpSourceTokenizer(defines, Some fileName)
        let lines = sourceText.Lines
        // We keep incremental data per-document. When text changes we correlate text line-by-line (by hash codes of lines)
        let sourceTextData = dataCache.GetValue(documentKey, fun key -> SourceTextData(lines.Count))
        // Go backwards to find the last cached scanned line that is valid
        let scanStartLine = 
            let mutable i = min (lines.Count - 1) lineNumber
            while i > 0 &&
               (match sourceTextData.[i] with 
                | Some data -> not (data.IsValid(lines.[i])) 
                | None -> true
               ) do  
               i <- i - 1
            i
        let lexState = if scanStartLine = 0 then 0L else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine
        let lineContents = textLine.Text.ToString(textLine.Span)
        
        // We can reuse the old data when 
        //   1. the line starts at the same overall position
        //   2. the hash codes match
        //   3. the start-of-line lex states are the same
        match sourceTextData.[lineNumber] with 
        | Some data when data.IsValid(textLine) && data.LexStateAtStartOfLine = lexState -> 
            data, textLinePos, lineContents
        | _ -> 
            // Otherwise, we recompute
            let newData = scanSourceLine(sourceTokenizer, textLine, lineContents, lexState)
            sourceTextData.[lineNumber] <- Some newData
            newData, textLinePos, lineContents
           
    let tokenizeLine (documentKey, sourceText, position, fileName, defines) =
        try
            let lineData, _, _ = getCachedSourceLineData(documentKey, sourceText, position, fileName, defines)
            lineData.Tokens   
        with 
        |  ex -> 
            Assert.Exception(ex)
            []

    let getSymbolAtPosition(documentKey: DocumentId, sourceText: SourceText, position: int, fileName: string, defines: string list, lookupKind: SymbolLookupKind) : LexerSymbol option =
        try
            let lineData, textLinePos, lineContents = getCachedSourceLineData(documentKey, sourceText, position, fileName, defines)
            getSymbolFromTokens(fileName, lineData.Tokens, textLinePos, lineContents, lookupKind)
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            None

    /// Fix invalid span if it appears to have redundant suffix and prefix.
    let fixupSpan (sourceText: SourceText, span: TextSpan) : TextSpan =
        let text = sourceText.GetSubText(span).ToString()
        match text.LastIndexOf '.' with
        | -1 | 0 -> span
        | index -> TextSpan(span.Start + index + 1, text.Length - index - 1)

    let isValidNameForSymbol (lexerSymbolKind: LexerSymbolKind, symbol: FSharpSymbol, name: string) : bool =
        let doubleBackTickDelimiter = "``"
        
        let isDoubleBacktickIdent (s: string) =
            let doubledDelimiter = 2 * doubleBackTickDelimiter.Length
            if s.StartsWith(doubleBackTickDelimiter) && s.EndsWith(doubleBackTickDelimiter) && s.Length > doubledDelimiter then
                let inner = s.Substring(doubleBackTickDelimiter.Length, s.Length - doubledDelimiter)
                not (inner.Contains(doubleBackTickDelimiter))
            else false
        
        let isIdentifier (ident: string) =
            if isDoubleBacktickIdent ident then
                true
            else
                ident 
                |> Seq.mapi (fun i c -> i, c)
                |> Seq.forall (fun (i, c) -> 
                     if i = 0 then PrettyNaming.IsIdentifierFirstCharacter c 
                     else PrettyNaming.IsIdentifierPartCharacter c) 
        
        let isFixableIdentifier (s: string) = 
            not (String.IsNullOrEmpty s) && Lexhelp.Keywords.NormalizeIdentifierBackticks s |> isIdentifier
        
        let forbiddenChars = [| '.'; '+'; '$'; '&'; '['; ']'; '/'; '\\'; '*'; '\'' |]
        
        let isTypeNameIdent (s: string) =
            not (String.IsNullOrEmpty s) && s.IndexOfAny forbiddenChars = -1 && isFixableIdentifier s 
        
        let isUnionCaseIdent (s: string) =
            isTypeNameIdent s && Char.IsUpper(s.Replace(doubleBackTickDelimiter, "").[0])
        
        let isTypeParameter (prefix: char) (s: string) =
            s.Length >= 2 && s.[0] = prefix && isIdentifier s.[1..]
        
        let isGenericTypeParameter = isTypeParameter '''
        let isStaticallyResolvedTypeParameter = isTypeParameter '^'
        
        match lexerSymbolKind, symbol with
        | _, :? FSharpUnionCase -> isUnionCaseIdent name
        | _, :? FSharpActivePatternCase -> 
            // Different from union cases, active patterns don't accept double-backtick identifiers
            isFixableIdentifier name && not (String.IsNullOrEmpty name) && Char.IsUpper(name.[0]) 
        | LexerSymbolKind.Operator, _ -> PrettyNaming.IsOperatorName name
        | LexerSymbolKind.Punctuation, _ -> PrettyNaming.IsPunctuation name
        | LexerSymbolKind.GenericTypeParameter, _ -> isGenericTypeParameter name
        | LexerSymbolKind.StaticallyResolvedTypeParameter, _ -> isStaticallyResolvedTypeParameter name
        | (LexerSymbolKind.Ident | LexerSymbolKind.Other), _ ->
            match symbol with
            | :? FSharpEntity as e when e.IsClass || e.IsFSharpRecord || e.IsFSharpUnion || e.IsValueType || e.IsFSharpModule || e.IsInterface -> isTypeNameIdent name
            | _ -> isFixableIdentifier name

[<RequireQualifiedAccess; NoComparison>] 
type internal SymbolDeclarationLocation = 
    | CurrentDocument
    | Projects of Project list * isLocalForProject: bool

[<AutoOpen>]
module internal Extensions =
    open System
    open System.IO
    open Microsoft.FSharp.Compiler.Ast
    open Microsoft.FSharp.Compiler.Lib
    open Microsoft.VisualStudio.FSharp.Editor.Logging

    type System.IServiceProvider with
        member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
        member x.GetService<'T, 'S>() = x.GetService(typeof<'S>) :?> 'T

    type Path with
        static member GetFullPathSafe path =
            try Path.GetFullPath path
            with _ -> path

    type CheckResults =
        | Ready of (FSharpParseFileResults * FSharpCheckFileResults) option
        | StillRunning of Async<(FSharpParseFileResults * FSharpCheckFileResults) option>

    let getFreshFileCheckResultsTimeoutMillis = GetEnvInteger "VFT_GetFreshFileCheckResultsTimeoutMillis" 1000
    
    type FSharpChecker with
        member this.ParseDocument(document: Document, options: FSharpProjectOptions, sourceText: string) =
            asyncMaybe {
                let! fileParseResults = this.ParseFileInProject(document.FilePath, sourceText, options) |> liftAsync
                return! fileParseResults.ParseTree
            }

        member this.ParseDocument(document: Document, options: FSharpProjectOptions, ?sourceText: SourceText) =
            asyncMaybe {
                let! sourceText =
                    match sourceText with
                    | Some x -> Task.FromResult x
                    | None -> document.GetTextAsync()
                return! this.ParseDocument(document, options, sourceText.ToString())
            }

        member this.ParseAndCheckDocument(filePath: string, textVersionHash: int, sourceText: string, options: FSharpProjectOptions, allowStaleResults: bool) : Async<(FSharpParseFileResults * Ast.ParsedInput * FSharpCheckFileResults) option> =
            let parseAndCheckFile =
                async {
                    let! parseResults, checkFileAnswer = this.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText, options)
                    return
                        match checkFileAnswer with
                        | FSharpCheckFileAnswer.Aborted -> 
                            None
                        | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                            Some (parseResults, checkFileResults)
                }

            let tryGetFreshResultsWithTimeout() : Async<CheckResults> =
                async {
                    try
                        let! worker = Async.StartChild(parseAndCheckFile, getFreshFileCheckResultsTimeoutMillis)
                        let! result = worker 
                        return Ready result
                    with :? TimeoutException ->
                        return StillRunning parseAndCheckFile
                }

            let bindParsedInput(results: (FSharpParseFileResults * FSharpCheckFileResults) option) =
                match results with
                | Some(parseResults, checkResults) ->
                    match parseResults.ParseTree with
                    | Some parsedInput -> Some (parseResults, parsedInput, checkResults)
                    | None -> None
                | None -> None

            if allowStaleResults then
                async {
                    let! freshResults = tryGetFreshResultsWithTimeout()
                    
                    let! results =
                        match freshResults with
                        | Ready x -> async.Return x
                        | StillRunning worker ->
                            async {
                                match allowStaleResults, this.TryGetRecentCheckResultsForFile(filePath, options) with
                                | true, Some (parseResults, checkFileResults, _) ->
                                    return Some (parseResults, checkFileResults)
                                | _ ->
                                    return! worker
                            }
                    return bindParsedInput results
                }
            else parseAndCheckFile |> Async.map bindParsedInput

        member this.ParseAndCheckDocument(document: Document, options: FSharpProjectOptions, allowStaleResults: bool, ?sourceText: SourceText) : Async<(FSharpParseFileResults * Ast.ParsedInput * FSharpCheckFileResults) option> =
            async {
                let! cancellationToken = Async.CancellationToken
                let! sourceText =
                    match sourceText with
                    | Some x -> Task.FromResult x
                    | None -> document.GetTextAsync()
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                return! this.ParseAndCheckDocument(document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options, allowStaleResults)
            }

    type FSharpSymbol with
        member this.IsInternalToProject =
            match this with 
            | :? FSharpParameter -> true
            | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || not m.Accessibility.IsPublic
            | :? FSharpEntity as m -> not m.Accessibility.IsPublic
            | :? FSharpGenericParameter -> true
            | :? FSharpUnionCase as m -> not m.Accessibility.IsPublic
            | :? FSharpField as m -> not m.Accessibility.IsPublic
            | _ -> false

    type FSharpSymbolUse with
        member this.GetDeclarationLocation (currentDocument: Document) : SymbolDeclarationLocation option =
            if this.IsPrivateToFile then
                Some SymbolDeclarationLocation.CurrentDocument
            else
                let isSymbolLocalForProject = this.Symbol.IsInternalToProject
                
                let declarationLocation = 
                    match this.Symbol.ImplementationLocation with
                    | Some x -> Some x
                    | None -> this.Symbol.DeclarationLocation
                
                match declarationLocation with
                | Some loc ->
                    let filePath = Path.GetFullPathSafe loc.FileName
                    let isScript = String.Equals(Path.GetExtension(filePath), ".fsx", StringComparison.OrdinalIgnoreCase)
                    if isScript && filePath = currentDocument.FilePath then 
                        Some SymbolDeclarationLocation.CurrentDocument
                    elif isScript then
                        // The standalone script might include other files via '#load'
                        // These files appear in project options and the standalone file 
                        // should be treated as an individual project
                        Some (SymbolDeclarationLocation.Projects ([currentDocument.Project], isSymbolLocalForProject))
                    else
                        let projects =
                            currentDocument.Project.Solution.GetDocumentIdsWithFilePath(filePath)
                            |> Seq.map (fun x -> x.ProjectId)
                            |> Seq.distinct
                            |> Seq.map currentDocument.Project.Solution.GetProject
                            |> Seq.toList
                        match projects with
                        | [] -> None
                        | projects -> Some (SymbolDeclarationLocation.Projects (projects, isSymbolLocalForProject))
                | None -> None

        member this.IsPrivateToFile = 
            let isPrivate =
                match this.Symbol with
                | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || m.Accessibility.IsPrivate
                | :? FSharpEntity as m -> m.Accessibility.IsPrivate
                | :? FSharpGenericParameter -> true
                | :? FSharpUnionCase as m -> m.Accessibility.IsPrivate
                | :? FSharpField as m -> m.Accessibility.IsPrivate
                | _ -> false
            
            let declarationLocation =
                match this.Symbol.SignatureLocation with
                | Some x -> Some x
                | _ ->
                    match this.Symbol.DeclarationLocation with
                    | Some x -> Some x
                    | _ -> this.Symbol.ImplementationLocation
            
            let declaredInTheFile = 
                match declarationLocation with
                | Some declRange -> declRange.FileName = this.RangeAlternate.FileName
                | _ -> false
            
            isPrivate && declaredInTheFile   

    type FSharpMemberOrFunctionOrValue with
        
        member x.IsConstructor = x.CompiledName = ".ctor"
        
        member x.IsOperatorOrActivePattern =
            let name = x.DisplayName
            if name.StartsWith "( " && name.EndsWith " )" && name.Length > 4
            then name.Substring (2, name.Length - 4) |> String.forall (fun c -> c <> ' ')
            else false

        member x.EnclosingEntitySafe =
            try
                Some x.EnclosingEntity
            with :? InvalidOperationException -> None

    type FSharpEntity with
        member x.AllBaseTypes =
            let rec allBaseTypes (entity:FSharpEntity) =
                [
                    match entity.TryFullName with
                    | Some _ ->
                        match entity.BaseType with
                        | Some bt ->
                            yield bt
                            if bt.HasTypeDefinition then
                                yield! allBaseTypes bt.TypeDefinition
                        | _ -> ()
                    | _ -> ()
                ]
            allBaseTypes x

    type FSharpNavigationDeclarationItem with
        member x.RoslynGlyph : Glyph =
            match x.Glyph with
            | FSharpGlyph.Class
            | FSharpGlyph.Typedef
            | FSharpGlyph.Type
            | FSharpGlyph.Exception ->
                match x.Access with
                | Some SynAccess.Private -> Glyph.ClassPrivate
                | Some SynAccess.Internal -> Glyph.ClassInternal
                | _ -> Glyph.ClassPublic
            | FSharpGlyph.Constant -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.ConstantPrivate
                | Some SynAccess.Internal -> Glyph.ConstantInternal
                | _ -> Glyph.ConstantPublic
            | FSharpGlyph.Delegate -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.DelegatePrivate
                | Some SynAccess.Internal -> Glyph.DelegateInternal
                | _ -> Glyph.DelegatePublic
            | FSharpGlyph.Union
            | FSharpGlyph.Enum -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.EnumPrivate
                | Some SynAccess.Internal -> Glyph.EnumInternal
                | _ -> Glyph.EnumPublic
            | FSharpGlyph.EnumMember
            | FSharpGlyph.Variable
            | FSharpGlyph.Field -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.FieldPrivate
                | Some SynAccess.Internal -> Glyph.FieldInternal
                | _ -> Glyph.FieldPublic
            | FSharpGlyph.Event -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.EventPrivate
                | Some SynAccess.Internal -> Glyph.EventInternal
                | _ -> Glyph.EventPublic
            | FSharpGlyph.Interface -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.InterfacePrivate
                | Some SynAccess.Internal -> Glyph.InterfaceInternal
                | _ -> Glyph.InterfacePublic
            | FSharpGlyph.Method
            | FSharpGlyph.OverridenMethod -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.MethodPrivate
                | Some SynAccess.Internal -> Glyph.MethodInternal
                | _ -> Glyph.MethodPublic
            | FSharpGlyph.Module -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.ModulePrivate
                | Some SynAccess.Internal -> Glyph.ModuleInternal
                | _ -> Glyph.ModulePublic
            | FSharpGlyph.NameSpace -> Glyph.Namespace
            | FSharpGlyph.Property -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.PropertyPrivate
                | Some SynAccess.Internal -> Glyph.PropertyInternal
                | _ -> Glyph.PropertyPublic
            | FSharpGlyph.Struct -> 
                match x.Access with
                | Some SynAccess.Private -> Glyph.StructurePrivate
                | Some SynAccess.Internal -> Glyph.StructureInternal
                | _ -> Glyph.StructurePublic
            | FSharpGlyph.ExtensionMethod ->
                match x.Access with
                | Some SynAccess.Private -> Glyph.ExtensionMethodPrivate
                | Some SynAccess.Internal -> Glyph.ExtensionMethodInternal
                | _ -> Glyph.ExtensionMethodPublic
            | FSharpGlyph.Error -> Glyph.Error

/// Active patterns over `FSharpSymbolUse`.
module internal SymbolUse =
    let (|ActivePatternCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpActivePatternCase as ap-> ActivePatternCase(ap) |> Some
        | _ -> None

    let private attributeSuffixLength = "Attribute".Length

    let (|Entity|_|) (symbol : FSharpSymbolUse) : (FSharpEntity * (* cleanFullNames *) string list) option =
        match symbol.Symbol with
        | :? FSharpEntity as ent -> 
            // strip generic parameters count suffix (List`1 => List)
            let cleanFullName =
                // `TryFullName` for type aliases is always `None`, so we have to make one by our own
                if ent.IsFSharpAbbreviation then
                    [ent.AccessPath + "." + ent.DisplayName]
                else
                    ent.TryFullName
                    |> Option.toList
                    |> List.map (fun fullName ->
                        if ent.GenericParameters.Count > 0 && fullName.Length > 2 then
                            fullName.[0..fullName.Length - 3]
                        else fullName)
            
            let cleanFullNames =
                cleanFullName
                |> List.collect (fun cleanFullName ->
                    if ent.IsAttributeType then
                        [cleanFullName; cleanFullName.[0..cleanFullName.Length - attributeSuffixLength - 1]]
                    else [cleanFullName]
                   )
            Some (ent, cleanFullNames)
        | _ -> None

    let (|Field|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpField as field-> Some field
        |  _ -> None

    let (|GenericParameter|_|) (symbol: FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpGenericParameter as gp -> Some gp
        | _ -> None

    let (|MemberFunctionOrValue|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> Some func
        | _ -> None

    let (|ActivePattern|_|) = function
        | MemberFunctionOrValue m when m.IsActivePattern -> Some m | _ -> None

    let (|Parameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpParameter as param -> Some param
        | _ -> None

    let (|StaticParameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpStaticParameter as sp -> Some sp
        | _ -> None

    let (|UnionCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpUnionCase as uc-> Some uc
        | _ -> None

    //let (|Constructor|_|) = function
    //    | MemberFunctionOrValue func when func.IsConstructor || func.IsImplicitConstructor -> Some func
    //    | _ -> None

    let (|TypeAbbreviation|_|) = function
        | Entity (entity, _) when entity.IsFSharpAbbreviation -> Some entity
        | _ -> None

    let (|Class|_|) = function
        | Entity (entity, _) when entity.IsClass -> Some entity
        | Entity (entity, _) when entity.IsFSharp &&
            entity.IsOpaque &&
            not entity.IsFSharpModule &&
            not entity.IsNamespace &&
            not entity.IsDelegate &&
            not entity.IsFSharpUnion &&
            not entity.IsFSharpRecord &&
            not entity.IsInterface &&
            not entity.IsValueType -> Some entity
        | _ -> None

    let (|Delegate|_|) = function
        | Entity (entity, _) when entity.IsDelegate -> Some entity
        | _ -> None

    let (|Event|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsEvent -> Some symbol
        | _ -> None

    let (|Property|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsProperty || symbol.IsPropertyGetterMethod || symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let inline private notCtorOrProp (symbol:FSharpMemberOrFunctionOrValue) =
        not symbol.IsConstructor && not symbol.IsPropertyGetterMethod && not symbol.IsPropertySetterMethod

    let (|Method|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            symbol.IsModuleValueOrMember  &&
            not symbolUse.IsFromPattern &&
            not symbol.IsOperatorOrActivePattern &&
            not symbol.IsPropertyGetterMethod &&
            not symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let (|Function|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol  &&
            symbol.IsModuleValueOrMember &&
            not symbol.IsOperatorOrActivePattern &&
            not symbolUse.IsFromPattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Operator|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbolUse.IsFromPattern &&
            not symbol.IsActivePattern &&
            symbol.IsOperatorOrActivePattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Pattern|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbol.IsOperatorOrActivePattern &&
            symbolUse.IsFromPattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType ->Some symbol
            | _ -> None
        | _ -> None


    let (|ClosureOrNestedFunction|_|) = function
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbol.IsOperatorOrActivePattern &&
            not symbol.IsModuleValueOrMember ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    
    let (|Val|_|) = function
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbol.IsOperatorOrActivePattern ->
            match symbol.FullTypeSafe with
            | Some _fullType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Enum|_|) = function
        | Entity (entity, _) when entity.IsEnum -> Some entity
        | _ -> None

    let (|Interface|_|) = function
        | Entity (entity, _) when entity.IsInterface -> Some entity
        | _ -> None

    let (|Module|_|) = function
        | Entity (entity, _) when entity.IsFSharpModule -> Some entity
        | _ -> None

    let (|Namespace|_|) = function
        | Entity (entity, _) when entity.IsNamespace -> Some entity
        | _ -> None

    let (|Record|_|) = function
        | Entity (entity, _) when entity.IsFSharpRecord -> Some entity
        | _ -> None

    let (|Union|_|) = function
        | Entity (entity, _) when entity.IsFSharpUnion -> Some entity
        | _ -> None

    let (|ValueType|_|) = function
        | Entity (entity, _) when entity.IsValueType && not entity.IsEnum -> Some entity
        | _ -> None

    let (|ComputationExpression|_|) (symbol:FSharpSymbolUse) =
        if symbol.IsFromComputationExpression then Some symbol
        else None
        
    let (|Attribute|_|) = function
        | Entity (entity, _) when entity.IsAttributeType -> Some entity
        | _ -> None