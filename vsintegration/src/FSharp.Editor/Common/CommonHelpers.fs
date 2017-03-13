// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
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
open Microsoft.VisualStudio.FSharp.Editor

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
           
    let tokenizeLine (documentKey, sourceText, position, fileName, defines) : FSharpTokenInfo list =
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
module internal SymbolExtensions =

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


[<AutoOpen>]
module internal UntypedAstUtils =
    open Microsoft.FSharp.Compiler.Range

    type Range.range with
        member inline x.IsEmpty = x.StartColumn = x.EndColumn && x.StartLine = x.EndLine 

    type internal ShortIdent = string
    type internal Idents = ShortIdent[]

    let internal longIdentToArray (longIdent: Ident list): Idents =
        longIdent |> Seq.map string |> Seq.toArray   


    /// An recursive pattern that collect all sequential expressions to avoid StackOverflowException
    let rec (|Sequentials|_|) = function
        | SynExpr.Sequential(_, _, e, Sequentials es, _) ->
            Some(e::es)
        | SynExpr.Sequential(_, _, e1, e2, _) ->
            Some [e1; e2]
        | _ -> None

    let (|ConstructorPats|) = function
        | SynConstructorArgs.Pats ps -> ps
        | SynConstructorArgs.NamePatPairs(xs, _) -> List.map snd xs


    /// Returns all Idents and LongIdents found in an untyped AST.
    let internal getLongIdents (input: ParsedInput option) : IDictionary<Range.pos, Idents> =
        let identsByEndPos = Dictionary<Range.pos, Idents>()

        let addLongIdent (longIdent: Ident list) =
            let idents = longIdentToArray longIdent
            for ident in longIdent do
                identsByEndPos.[ident.idRange.End] <- idents

        let addLongIdentWithDots (LongIdentWithDots (longIdent, lids) as value) =
            match longIdentToArray longIdent with
            | [||] -> ()
            | [|_|] as idents -> identsByEndPos.[value.Range.End] <- idents
            | idents ->
                for dotRange in lids do
                    identsByEndPos.[Range.mkPos dotRange.EndLine (dotRange.EndColumn - 1)] <- idents
                identsByEndPos.[value.Range.End] <- idents

        let addIdent (ident: Ident) =
            identsByEndPos.[ident.idRange.End] <- [|ident.idText|]

        let rec walkImplFileInput (ParsedImplFileInput(_, _, _, _, _, moduleOrNamespaceList, _)) =
            List.iter walkSynModuleOrNamespace moduleOrNamespaceList

        and walkSynModuleOrNamespace (SynModuleOrNamespace(_, _, _, decls, _, attrs, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkSynModuleDecl decls

        and walkAttribute (attr: SynAttribute) =
            addLongIdentWithDots attr.TypeName
            walkExpr attr.ArgExpr

        and walkTyparDecl (SynTyparDecl.TyparDecl (attrs, typar)) =
            List.iter walkAttribute attrs
            walkTypar typar

        and walkTypeConstraint = function
            | SynTypeConstraint.WhereTyparIsValueType (t, _)
            | SynTypeConstraint.WhereTyparIsReferenceType (t, _)
            | SynTypeConstraint.WhereTyparIsUnmanaged (t, _)
            | SynTypeConstraint.WhereTyparSupportsNull (t, _)
            | SynTypeConstraint.WhereTyparIsComparable (t, _)
            | SynTypeConstraint.WhereTyparIsEquatable (t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparDefaultsToType (t, ty, _)
            | SynTypeConstraint.WhereTyparSubtypeOfType (t, ty, _) -> walkTypar t; walkType ty
            | SynTypeConstraint.WhereTyparIsEnum (t, ts, _)
            | SynTypeConstraint.WhereTyparIsDelegate (t, ts, _) -> walkTypar t; List.iter walkType ts
            | SynTypeConstraint.WhereTyparSupportsMember (ts, sign, _) -> List.iter walkType ts; walkMemberSig sign

        and walkPat = function
            | SynPat.Tuple (pats, _)
            | SynPat.ArrayOrList (_, pats, _)
            | SynPat.Ands (pats, _) -> List.iter walkPat pats
            | SynPat.Named (pat, ident, _, _, _) ->
                walkPat pat
                addIdent ident
            | SynPat.Typed (pat, t, _) ->
                walkPat pat
                walkType t
            | SynPat.Attrib (pat, attrs, _) ->
                walkPat pat
                List.iter walkAttribute attrs
            | SynPat.Or (pat1, pat2, _) -> List.iter walkPat [pat1; pat2]
            | SynPat.LongIdent (ident, _, typars, ConstructorPats pats, _, _) ->
                addLongIdentWithDots ident
                typars
                |> Option.iter (fun (SynValTyparDecls (typars, _, constraints)) ->
                     List.iter walkTyparDecl typars
                     List.iter walkTypeConstraint constraints)
                List.iter walkPat pats
            | SynPat.Paren (pat, _) -> walkPat pat
            | SynPat.IsInst (t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> ()

        and walkTypar (Typar (_, _, _)) = ()

        and walkBinding (SynBinding.Binding (_, _, _, _, attrs, _, _, pat, returnInfo, e, _, _)) =
            List.iter walkAttribute attrs
            walkPat pat
            walkExpr e
            returnInfo |> Option.iter (fun (SynBindingReturnInfo (t, _, _)) -> walkType t)

        and walkInterfaceImpl (InterfaceImpl(_, bindings, _)) = List.iter walkBinding bindings

        and walkIndexerArg = function
            | SynIndexerArg.One e -> walkExpr e
            | SynIndexerArg.Two (e1, e2) -> List.iter walkExpr [e1; e2]

        and walkType = function
            | SynType.Array (_, t, _)
            | SynType.HashConstraint (t, _)
            | SynType.MeasurePower (t, _, _) -> walkType t
            | SynType.Fun (t1, t2, _)
            | SynType.MeasureDivide (t1, t2, _) -> walkType t1; walkType t2
            | SynType.LongIdent ident -> addLongIdentWithDots ident
            | SynType.App (ty, _, types, _, _, _, _) -> walkType ty; List.iter walkType types
            | SynType.LongIdentApp (_, _, _, types, _, _, _) -> List.iter walkType types
            | SynType.Tuple (ts, _) -> ts |> List.iter (fun (_, t) -> walkType t)
            | SynType.WithGlobalConstraints (t, typeConstraints, _) ->
                walkType t; List.iter walkTypeConstraint typeConstraints
            | _ -> ()

        and walkClause (Clause (pat, e1, e2, _, _)) =
            walkPat pat
            walkExpr e2
            e1 |> Option.iter walkExpr

        and walkSimplePats = function
            | SynSimplePats.SimplePats (pats, _) -> List.iter walkSimplePat pats
            | SynSimplePats.Typed (pats, ty, _) -> 
                walkSimplePats pats
                walkType ty

        and walkExpr = function
            | SynExpr.Paren (e, _, _, _)
            | SynExpr.Quote (_, _, e, _, _)
            | SynExpr.Typed (e, _, _)
            | SynExpr.InferredUpcast (e, _)
            | SynExpr.InferredDowncast (e, _)
            | SynExpr.AddressOf (_, e, _, _)
            | SynExpr.DoBang (e, _)
            | SynExpr.YieldOrReturn (_, e, _)
            | SynExpr.ArrayOrListOfSeqExpr (_, e, _)
            | SynExpr.CompExpr (_, _, e, _)
            | SynExpr.Do (e, _)
            | SynExpr.Assert (e, _)
            | SynExpr.Lazy (e, _)
            | SynExpr.YieldOrReturnFrom (_, e, _) -> walkExpr e
            | SynExpr.Lambda (_, _, pats, e, _) ->
                walkSimplePats pats
                walkExpr e
            | SynExpr.New (_, t, e, _)
            | SynExpr.TypeTest (e, t, _)
            | SynExpr.Upcast (e, t, _)
            | SynExpr.Downcast (e, t, _) -> walkExpr e; walkType t
            | SynExpr.Tuple (es, _, _)
            | Sequentials es
            | SynExpr.ArrayOrList (_, es, _) -> List.iter walkExpr es
            | SynExpr.App (_, _, e1, e2, _)
            | SynExpr.TryFinally (e1, e2, _, _, _)
            | SynExpr.While (_, e1, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.Record (_, _, fields, _) ->
                fields |> List.iter (fun ((ident, _), e, _) ->
                            addLongIdentWithDots ident
                            e |> Option.iter walkExpr)
            | SynExpr.Ident ident -> addIdent ident
            | SynExpr.ObjExpr(ty, argOpt, bindings, ifaces, _, _) ->
                argOpt |> Option.iter (fun (e, ident) ->
                    walkExpr e
                    ident |> Option.iter addIdent)
                walkType ty
                List.iter walkBinding bindings
                List.iter walkInterfaceImpl ifaces
            | SynExpr.LongIdent (_, ident, _, _) -> addLongIdentWithDots ident
            | SynExpr.For (_, ident, e1, _, e2, e3, _) ->
                addIdent ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.ForEach (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.MatchLambda (_, _, synMatchClauseList, _, _) ->
                List.iter walkClause synMatchClauseList
            | SynExpr.Match (_, e, synMatchClauseList, _, _) ->
                walkExpr e
                List.iter walkClause synMatchClauseList
            | SynExpr.TypeApp (e, _, tys, _, _, _, _) ->
                List.iter walkType tys; walkExpr e
            | SynExpr.LetOrUse (_, _, bindings, e, _) ->
                List.iter walkBinding bindings; walkExpr e
            | SynExpr.TryWith (e, _, clauses, _, _, _, _) ->
                List.iter walkClause clauses;  walkExpr e
            | SynExpr.IfThenElse (e1, e2, e3, _, _, _, _) ->
                List.iter walkExpr [e1; e2]
                e3 |> Option.iter walkExpr
            | SynExpr.LongIdentSet (ident, e, _)
            | SynExpr.DotGet (e, _, ident, _) ->
                addLongIdentWithDots ident
                walkExpr e
            | SynExpr.DotSet (e1, idents, e2, _) ->
                walkExpr e1
                addLongIdentWithDots idents
                walkExpr e2
            | SynExpr.DotIndexedGet (e, args, _, _) ->
                walkExpr e
                List.iter walkIndexerArg args
            | SynExpr.DotIndexedSet (e1, args, e2, _, _, _) ->
                walkExpr e1
                List.iter walkIndexerArg args
                walkExpr e2
            | SynExpr.NamedIndexedPropertySet (ident, e1, e2, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2]
            | SynExpr.DotNamedIndexedPropertySet (e1, ident, e2, e3, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.JoinIn (e1, _, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.LetOrUseBang (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.TraitCall (ts, sign, e, _) ->
                List.iter walkTypar ts
                walkMemberSig sign
                walkExpr e
            | SynExpr.Const (SynConst.Measure(_, m), _) -> walkMeasure m
            | _ -> ()

        and walkMeasure = function
            | SynMeasure.Product (m1, m2, _)
            | SynMeasure.Divide (m1, m2, _) -> walkMeasure m1; walkMeasure m2
            | SynMeasure.Named (longIdent, _) -> addLongIdent longIdent
            | SynMeasure.Seq (ms, _) -> List.iter walkMeasure ms
            | SynMeasure.Power (m, _, _) -> walkMeasure m
            | SynMeasure.Var (ty, _) -> walkTypar ty
            | SynMeasure.One
            | SynMeasure.Anon _ -> ()

        and walkSimplePat = function
            | SynSimplePat.Attrib (pat, attrs, _) ->
                walkSimplePat pat
                List.iter walkAttribute attrs
            | SynSimplePat.Typed(pat, t, _) ->
                walkSimplePat pat
                walkType t
            | _ -> ()

        and walkField (SynField.Field(attrs, _, _, t, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t

        and walkValSig (SynValSig.ValSpfn(attrs, _, _, t, SynValInfo(argInfos, argInfo), _, _, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t
            argInfo :: (argInfos |> List.concat)
            |> List.map (fun (SynArgInfo(attrs, _, _)) -> attrs)
            |> List.concat
            |> List.iter walkAttribute

        and walkMemberSig = function
            | SynMemberSig.Inherit (t, _)
            | SynMemberSig.Interface(t, _) -> walkType t
            | SynMemberSig.Member(vs, _, _) -> walkValSig vs
            | SynMemberSig.ValField(f, _) -> walkField f
            | SynMemberSig.NestedType(SynTypeDefnSig.TypeDefnSig (info, repr, memberSigs, _), _) ->
                let isTypeExtensionOrAlias =
                    match repr with
                    | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAbbrev, _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAugmentation, _, _) -> true
                    | _ -> false
                walkComponentInfo isTypeExtensionOrAlias info
                walkTypeDefnSigRepr repr
                List.iter walkMemberSig memberSigs

        and walkMember = function
            | SynMemberDefn.AbstractSlot (valSig, _, _) -> walkValSig valSig
            | SynMemberDefn.Member (binding, _) -> walkBinding binding
            | SynMemberDefn.ImplicitCtor (_, attrs, pats, _, _) ->
                List.iter walkAttribute attrs
                List.iter walkSimplePat pats
            | SynMemberDefn.ImplicitInherit (t, e, _, _) -> walkType t; walkExpr e
            | SynMemberDefn.LetBindings (bindings, _, _, _) -> List.iter walkBinding bindings
            | SynMemberDefn.Interface (t, members, _) ->
                walkType t
                members |> Option.iter (List.iter walkMember)
            | SynMemberDefn.Inherit (t, _, _) -> walkType t
            | SynMemberDefn.ValField (field, _) -> walkField field
            | SynMemberDefn.NestedType (tdef, _, _) -> walkTypeDefn tdef
            | SynMemberDefn.AutoProperty (attrs, _, _, t, _, _, _, _, e, _, _) ->
                List.iter walkAttribute attrs
                Option.iter walkType t
                walkExpr e
            | _ -> ()

        and walkEnumCase (EnumCase(attrs, _, _, _, _)) = List.iter walkAttribute attrs

        and walkUnionCaseType = function
            | SynUnionCaseType.UnionCaseFields fields -> List.iter walkField fields
            | SynUnionCaseType.UnionCaseFullType (t, _) -> walkType t

        and walkUnionCase (SynUnionCase.UnionCase (attrs, _, t, _, _, _)) =
            List.iter walkAttribute attrs
            walkUnionCaseType t

        and walkTypeDefnSimple = function
            | SynTypeDefnSimpleRepr.Enum (cases, _) -> List.iter walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union (_, cases, _) -> List.iter walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record (_, fields, _) -> List.iter walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev (_, t, _) -> walkType t
            | _ -> ()

        and walkComponentInfo isTypeExtensionOrAlias (ComponentInfo(attrs, typars, constraints, longIdent, _, _, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkTyparDecl typars
            List.iter walkTypeConstraint constraints
            if isTypeExtensionOrAlias then
                addLongIdent longIdent

        and walkTypeDefnRepr = function
            | SynTypeDefnRepr.ObjectModel (_, defns, _) -> List.iter walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception _ -> ()

        and walkTypeDefnSigRepr = function
            | SynTypeDefnSigRepr.ObjectModel (_, defns, _) -> List.iter walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception _ -> ()

        and walkTypeDefn (TypeDefn (info, repr, members, _)) =
            let isTypeExtensionOrAlias =
                match repr with
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAugmentation, _, _)
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAbbrev, _, _)
                | SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> true
                | _ -> false
            walkComponentInfo isTypeExtensionOrAlias info
            walkTypeDefnRepr repr
            List.iter walkMember members

        and walkSynModuleDecl (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace fragment
            | SynModuleDecl.NestedModule (info, _, modules, _, _) ->
                walkComponentInfo false info
                List.iter walkSynModuleDecl modules
            | SynModuleDecl.Let (_, bindings, _) -> List.iter walkBinding bindings
            | SynModuleDecl.DoExpr (_, expr, _) -> walkExpr expr
            | SynModuleDecl.Types (types, _) -> List.iter walkTypeDefn types
            | SynModuleDecl.Attributes (attrs, _) -> List.iter walkAttribute attrs
            | _ -> ()

        match input with
        | Some (ParsedInput.ImplFile input) ->
             walkImplFileInput input
        | _ -> ()
        //debug "%A" idents
        identsByEndPos :> _


    /// Get path to containing module/namespace of a given position
    let getModuleOrNamespacePath (pos: pos) (ast: ParsedInput) =
        let idents =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput(_, _, _, _, _, modules, _)) ->
                let rec walkModuleOrNamespace idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleDecl.NestedModule (componentInfo, _, nestedModuleDecls, _, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespace (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespace(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespace (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
            | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, modules)) ->
                let rec walkModuleOrNamespaceSig idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleSigDecl.NestedModule (componentInfo, _, nestedModuleDecls, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespaceSig (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespaceSig(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespaceSig (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
        idents
        |> List.rev
        |> Seq.concat
        |> Seq.map (fun ident -> ident.idText)
        |> String.concat "."


[<NoComparison>]
type internal SymbolUse =
    { SymbolUse: FSharpSymbolUse 
      IsUsed: bool
      FullNames: Idents[] }


[<AutoOpen>]
module internal TypedAstUtils =
    open System.Text.RegularExpressions

    let isAttribute<'T> (attribute: FSharpAttribute) =
        // CompiledName throws exception on DataContractAttribute generated by SQLProvider
        match Option.attempt (fun _ -> attribute.AttributeType.CompiledName) with
        | Some name when name = typeof<'T>.Name -> true
        | _ -> false

    let tryGetAttribute<'T> (attributes: seq<FSharpAttribute>) =
        attributes |> Seq.tryFind isAttribute<'T>

    let hasModuleSuffixAttribute (entity: FSharpEntity) = 
         entity.Attributes
         |> tryGetAttribute<CompilationRepresentationAttribute>
         |> Option.bind (fun a -> 
              Option.attempt (fun _ -> a.ConstructorArguments)
              |> Option.bind (fun args -> args |> Seq.tryPick (fun (_, arg) ->
                   let res =
                       match arg with
                       | :? int32 as arg when arg = int CompilationRepresentationFlags.ModuleSuffix -> 
                           Some() 
                       | :? CompilationRepresentationFlags as arg when arg = CompilationRepresentationFlags.ModuleSuffix -> 
                           Some() 
                       | _ -> 
                           None
                   res)))
         |> Option.isSome

    let isOperator (name: string) =
        name.StartsWith "( " && name.EndsWith " )" && name.Length > 4
            && name.Substring (2, name.Length - 4) 
               |> String.forall (fun c -> c <> ' ' && not (Char.IsLetter c))

    let private UnnamedUnionFieldRegex = Regex("^Item(\d+)?$", RegexOptions.Compiled)
    
    let isUnnamedUnionCaseField (field: FSharpField) = UnnamedUnionFieldRegex.IsMatch(field.Name)


module internal TypedAstPatterns =

    let (|AbbreviatedType|_|) (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then Some entity.AbbreviatedType
        else None

    let (|TypeWithDefinition|_|) (ty: FSharpType) =
        if ty.HasTypeDefinition then Some ty.TypeDefinition
        else None

    let rec getEntityAbbreviatedType (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then
            match entity.AbbreviatedType with
            | TypeWithDefinition def -> getEntityAbbreviatedType def
            | abbreviatedType -> entity, Some abbreviatedType
        else entity, None

    let rec getAbbreviatedType (fsharpType: FSharpType) =
        if fsharpType.IsAbbreviation then
            getAbbreviatedType fsharpType.AbbreviatedType
        else fsharpType

    let (|Attribute|_|) (entity: FSharpEntity) =
        let isAttribute (entity: FSharpEntity) =
            let getBaseType (entity: FSharpEntity) =
                try 
                    match entity.BaseType with
                    | Some (TypeWithDefinition def) -> Some def
                    | _ -> None
                with _ -> None

            let rec isAttributeType (ty: FSharpEntity option) =
                match ty with
                | None -> false
                | Some ty ->
                    match ty.TryGetFullName() with
                    | None -> false
                    | Some fullName ->
                        fullName = "System.Attribute" || isAttributeType (getBaseType ty)
            isAttributeType (Some entity)
        if isAttribute entity then Some() else None

    let (|ValueType|_|) (e: FSharpEntity) =
        if e.IsEnum || e.IsValueType || hasAttribute<MeasureAnnotatedAbbreviationAttribute> e.Attributes then Some()
        else None

    let (|Class|_|) (original: FSharpEntity, abbreviated: FSharpEntity, _) = 
        if abbreviated.IsClass 
           && (not abbreviated.IsStaticInstantiation || original.IsFSharpAbbreviation) then Some()
        else None 

    let (|Record|_|) (e: FSharpEntity) = if e.IsFSharpRecord then Some() else None
    let (|UnionType|_|) (e: FSharpEntity) = if e.IsFSharpUnion then Some() else None
    let (|Delegate|_|) (e: FSharpEntity) = if e.IsDelegate then Some() else None
    let (|FSharpException|_|) (e: FSharpEntity) = if e.IsFSharpExceptionDeclaration then Some() else None
    let (|Interface|_|) (e: FSharpEntity) = if e.IsInterface then Some() else None
    let (|AbstractClass|_|) (e: FSharpEntity) =
        if hasAttribute<AbstractClassAttribute> e.Attributes then Some() else None
            
    let (|FSharpType|_|) (e: FSharpEntity) = 
        if e.IsDelegate || e.IsFSharpExceptionDeclaration || e.IsFSharpRecord || e.IsFSharpUnion 
            || e.IsInterface || e.IsMeasure 
            || (e.IsFSharp && e.IsOpaque && not e.IsFSharpModule && not e.IsNamespace) then Some() 
        else None

    let (|ProvidedType|_|) (e: FSharpEntity) =
        if (e.IsProvided || e.IsProvidedAndErased || e.IsProvidedAndGenerated) && e.CompiledName = e.DisplayName then
            Some()
        else None

    let (|ByRef|_|) (e: FSharpEntity) = if e.IsByRef then Some() else None
    let (|Array|_|) (e: FSharpEntity) = if e.IsArrayType then Some() else None
    let (|FSharpModule|_|) (entity: FSharpEntity) = if entity.IsFSharpModule then Some() else None

    let (|Namespace|_|) (entity: FSharpEntity) = if entity.IsNamespace then Some() else None
    let (|ProvidedAndErasedType|_|) (entity: FSharpEntity) = if entity.IsProvidedAndErased then Some() else None
    let (|Enum|_|) (entity: FSharpEntity) = if entity.IsEnum then Some() else None

    let (|Tuple|_|) (ty: FSharpType option) = 
        ty |> Option.bind (fun ty -> if ty.IsTupleType then Some() else None)

    let (|RefCell|_|) (ty: FSharpType) = 
        match getAbbreviatedType ty with
        | TypeWithDefinition def when 
            def.IsFSharpRecord && def.FullName = "Microsoft.FSharp.Core.FSharpRef`1" -> Some() 
        | _ -> None

    let (|FunctionType|_|) (ty: FSharpType) = 
        if ty.IsFunctionType then Some() 
        else None

    let (|Pattern|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpUnionCase
        | :? FSharpActivePatternCase -> Some()
        | _ -> None

    /// Field (field, fieldAbbreviatedType)
    let (|Field|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpField as field -> Some (field, getAbbreviatedType field.FieldType)
        | _ -> None

    let (|MutableVar|_|) (symbol: FSharpSymbol) = 
        let isMutable = 
            match symbol with
            | :? FSharpField as field -> field.IsMutable && not field.IsLiteral
            | :? FSharpMemberOrFunctionOrValue as func -> func.IsMutable
            | _ -> false
        if isMutable then Some() else None

    /// Entity (originalEntity, abbreviatedEntity, abbreviatedType)
    let (|FSharpEntity|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> 
            let abbreviatedEntity, abbreviatedType = getEntityAbbreviatedType entity
            Some (entity, abbreviatedEntity, abbreviatedType)
        | _ -> None

    let (|Parameter|_|) (symbol: FSharpSymbol) = 
        match symbol with
        | :? FSharpParameter -> Some()
        | _ -> None

    let (|UnionCase|_|) (e: FSharpSymbol) = 
        match e with
        | :? FSharpUnionCase as uc -> Some uc
        | _ -> None

    let (|RecordField|_|) (e: FSharpSymbol) =
        match e with
        | :? FSharpField as field ->
            if field.DeclaringEntity.IsFSharpRecord then Some field else None
        | _ -> None

    let (|ActivePatternCase|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpActivePatternCase as case -> Some case
        | _ -> None

    /// Func (memberFunctionOrValue, fullType)
    let (|MemberFunctionOrValue|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> Some func
        | _ -> None

    /// Constructor (enclosingEntity)
    let (|Constructor|_|) (func: FSharpMemberOrFunctionOrValue) =
        match func.CompiledName with
        | ".ctor" | ".cctor" -> Some func.EnclosingEntity
        | _ -> None

    let (|Function|_|) excluded (func: FSharpMemberOrFunctionOrValue) =
        match func.FullTypeSafe |> Option.map getAbbreviatedType with
        | Some typ when typ.IsFunctionType
                       && not func.IsPropertyGetterMethod 
                       && not func.IsPropertySetterMethod
                       && not excluded
                       && not (isOperator func.DisplayName) -> Some()
        | _ -> None

    let (|ExtensionMember|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsExtensionMember then Some() else None

    let (|Event|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsEvent then Some () else None


module internal IdentifierUtils =

    open System
    open Microsoft.FSharp.Compiler.PrettyNaming
    open Microsoft.FSharp.Compiler.SourceCodeServices.PrettyNaming

    let DoubleBackTickDelimiter = "``"

    let isDoubleBacktickIdent (s: string) =
        let doubledDelimiter = 2 * DoubleBackTickDelimiter.Length
        if s.StartsWith(DoubleBackTickDelimiter) && s.EndsWith(DoubleBackTickDelimiter) && s.Length > doubledDelimiter then
            let inner = s.Substring(DoubleBackTickDelimiter.Length, s.Length - doubledDelimiter)
            not (inner.Contains(DoubleBackTickDelimiter))
        else false

    let isIdentifier (s: string) =
        if isDoubleBacktickIdent s then
            true
        else
            s |> Seq.mapi (fun i c -> i, c)
              |> Seq.forall (fun (i, c) -> 
                    if i = 0 then IsIdentifierFirstCharacter c else IsIdentifierPartCharacter c) 

    let isOperator (s: string) = 
        let allowedChars = Set.ofList ['!'; '%'; '&'; '*'; '+'; '-'; '.'; '/'; '<'; '='; '>'; '?'; '@'; '^'; '|'; '~']
        (IsPrefixOperator s || IsInfixOperator s || IsTernaryOperator s)
        && (s.ToCharArray() |> Array.forall (fun c -> Set.contains c allowedChars))

    /// Encapsulates identifiers for rename operations if needed
    let encapsulateIdentifier symbolKind newName =
        let isKeyWord = List.exists ((=) newName) KeywordNames
        let isAlreadyEncapsulated = newName.StartsWith DoubleBackTickDelimiter && newName.EndsWith DoubleBackTickDelimiter

        if isAlreadyEncapsulated then newName
        elif (symbolKind = LexerSymbolKind.Operator) || (symbolKind = LexerSymbolKind.GenericTypeParameter) || (symbolKind = LexerSymbolKind.StaticallyResolvedTypeParameter) then newName
        elif isKeyWord || not (isIdentifier newName) then DoubleBackTickDelimiter + newName + DoubleBackTickDelimiter
        else newName

    let isFixableIdentifier (s: string) = 
        not (String.IsNullOrEmpty s) && encapsulateIdentifier LexerSymbolKind.Ident s |> isIdentifier

    let private forbiddenChars = ["."; "+"; "$"; "&"; "["; "]"; "/"; "\\"; "*"; "\""]

    let isTypeNameIdent (s: string) =
        not (String.IsNullOrEmpty s) &&
        forbiddenChars |> Seq.forall (fun c -> not (s.Contains c)) &&
        isFixableIdentifier s 

    let isUnionCaseIdent (s: string) =
        isTypeNameIdent s &&    
        Char.IsUpper(s.Replace(DoubleBackTickDelimiter,"").[0])


[<AutoOpen>]
module internal Extensions =
    open System
    open System.IO
    open Microsoft.FSharp.Compiler.Ast
    open Microsoft.FSharp.Compiler.Lib
    open Microsoft.VisualStudio.FSharp.Editor.Logging
    open Microsoft.VisualStudio.FSharp.Editor

    type System.IServiceProvider with
        member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
        member x.GetService<'T, 'S>() = x.GetService(typeof<'S>) :?> 'T



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



        member self.TryParseAndCheckFileInProject (projectOptions, fileName, source) = async {
            let! (parseResults, checkAnswer) = self.ParseAndCheckFileInProject (fileName,0, source,projectOptions)
            match checkAnswer with
            | FSharpCheckFileAnswer.Aborted ->  return  None
            | FSharpCheckFileAnswer.Succeeded checkResults -> return Some (parseResults,checkResults)
        }

        member self.GetAllUsesOfAllSymbolsInSourceString (projectOptions, fileName, source: string, checkForUnusedOpens)  (*FSharpSymbolUse[] Async*) = async {
                  
            let! parseAndCheckResults = self.TryParseAndCheckFileInProject (projectOptions, fileName, source)
            match parseAndCheckResults with
            | None -> return [||]
            | Some(_parseResults,checkResults) ->
                let! fsharpSymbolsUses = checkResults.GetAllUsesOfAllSymbolsInFile()
                let allSymbolsUses =
                    fsharpSymbolsUses
                    |> Array.map (fun symbolUse -> 
                        let fullNames = 
                            match symbolUse.Symbol with
                            // Make sure that unsafe manipulation isn't executed if unused opens are disabled
                            | _ when not checkForUnusedOpens -> None
                            | TypedAstPatterns.MemberFunctionOrValue func when func.IsExtensionMember ->
                                if func.IsProperty then
                                    let fullNames =
                                        [|  if func.HasGetterMethod then
                                                yield func.GetterMethod.EnclosingEntity.TryGetFullName()
                                            if func.HasSetterMethod then
                                                yield func.SetterMethod.EnclosingEntity.TryGetFullName()
                                        |]
                                        |> Array.choose id
                                    match fullNames with
                                    | [||]  -> None 
                                    | _     -> Some fullNames
                                else 
                                    match func.EnclosingEntity with
                                    // C# extension method
                                    | TypedAstPatterns.FSharpEntity TypedAstPatterns.Class ->
                                        let fullName = symbolUse.Symbol.FullName.Split '.'
                                        if fullName.Length > 2 then
                                            (* For C# extension methods FCS returns full name including the class name, like:
                                                Namespace.StaticClass.ExtensionMethod
                                                So, in order to properly detect that "open Namespace" actually opens ExtensionMethod,
                                                we remove "StaticClass" part. This makes C# extension methods looks identically 
                                                with F# extension members.
                                            *)
                                            let fullNameWithoutClassName =
                                                Array.append fullName.[0..fullName.Length - 3] fullName.[fullName.Length - 1..]
                                            Some [|String.Join (".", fullNameWithoutClassName)|]
                                        else None
                                    | _ -> None
                            // Operators
                            | TypedAstPatterns.MemberFunctionOrValue func ->
                                match func with
                                | TypedAstPatterns.Constructor _ ->
                                    // full name of a constructor looks like "UnusedSymbolClassifierTests.PrivateClass.( .ctor )"
                                    // to make well formed full name parts we cut "( .ctor )" from the tail.
                                    let fullName = func.FullName
                                    let ctorSuffix = ".( .ctor )"
                                    let fullName =
                                        if fullName.EndsWith ctorSuffix then 
                                            fullName.[0..fullName.Length - ctorSuffix.Length - 1]
                                        else fullName
                                    Some [| fullName |]
                                | _ -> 
                                    Some [| yield func.FullName 
                                            match func.TryGetFullCompiledOperatorNameIdents() with
                                            | Some idents -> yield String.concat "." idents
                                            | None -> ()
                                        |]
                            | TypedAstPatterns.FSharpEntity e ->
                                match e with
                                | e, TypedAstPatterns.Attribute, _ ->
                                    e.TryGetFullName ()
                                    |> Option.map (fun fullName ->
                                        [| fullName; fullName.Substring(0, fullName.Length - "Attribute".Length) |])
                                | e, _, _ -> 
                                    e.TryGetFullName () |> Option.map (fun fullName -> [| fullName |])
                            | TypedAstPatterns.RecordField _
                            | TypedAstPatterns.UnionCase _ as symbol ->
                                Some [| let fullName = symbol.FullName
                                        yield fullName
                                        let idents = fullName.Split '.'
                                        // Union cases/Record fields can be accessible without mentioning the enclosing type. 
                                        // So we add a FullName without having the type part.
                                        if idents.Length > 1 then
                                            yield String.Join (".", Array.append idents.[0..idents.Length - 3] idents.[idents.Length - 1..])
                                    |]
                            |  _ -> None
                            |> Option.defaultValue [|symbolUse.Symbol.FullName|]
                            |> Array.map (fun fullName -> fullName.Split '.')
                  
                        {   SymbolUse = symbolUse
                            IsUsed = true
                            FullNames = fullNames 
                        })
                return allSymbolsUses 
            }


   

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