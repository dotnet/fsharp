namespace FSharp.Compiler.LanguageServer

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open Microsoft.VisualStudio.LanguageServer.Protocol

#nowarn "57"

module LspSemanticTokens =

    type TokenSource =
        | Semantic
        | Syntactic

    type NormalizedToken =
        {
            Line: int
            StartChar: int
            Length: int
            TokenType: string
            TokenModifiers: string list
            Source: TokenSource
        }

    let private classNameTokenType = "class name"
    let private structNameTokenType = "struct name"
    let private enumNameTokenType = "enum name"
    let private interfaceNameTokenType = "interface name"
    let private typeParameterNameTokenType = "type parameter name"
    let private delegateNameTokenType = "delegate name"
    let private fieldNameTokenType = "field name"
    let private propertyNameTokenType = "property name"
    let private eventNameTokenType = "event name"
    let private constantNameTokenType = "constant name"
    let private labelNameTokenType = "label name"
    let private extensionMethodNameTokenType = "extension method name"
    let private methodNameTokenType = "method name"
    let private namespaceNameTokenType = "namespace name"
    let private localNameTokenType = "local name"
    let private stringEscapeCharacterTokenType = "string - escape character"

    // F#-specific token type names. These match the MEF classification types registered in
    // vsintegration/src/FSharp.Editor/Classification/ClassificationDefinitions.fs and rely on the
    // VS LSP client forwarding them as identity entries (see RoslynSemanticTokenTypes / its F#
    // extender provided counterpart) so the existing ThemeColors / EditorFormatDefinition pipeline
    // applies the F# theme colors and honors user overrides set in Fonts and Colors.
    let private fsharpFunctionTokenType = "FSharp.Function"
    let private fsharpMutableVarTokenType = "FSharp.MutableVar"
    let private fsharpPrintfTokenType = "FSharp.Printf"
    let private fsharpDisposableTypeTokenType = "FSharp.DisposableType"
    let private fsharpDisposableLocalValueTokenType = "FSharp.DisposableLocalValue"

    let private fsharpDisposableTopLevelValueTokenType =
        "FSharp.DisposableTopLevelValue"

    let tokenTypes =
        [|
            SemanticTokenTypes.Variable
            SemanticTokenTypes.Keyword
            SemanticTokenTypes.Comment
            SemanticTokenTypes.String
            SemanticTokenTypes.Number
            SemanticTokenTypes.Operator
            classNameTokenType
            structNameTokenType
            enumNameTokenType
            interfaceNameTokenType
            typeParameterNameTokenType
            delegateNameTokenType
            fieldNameTokenType
            propertyNameTokenType
            eventNameTokenType
            constantNameTokenType
            labelNameTokenType
            extensionMethodNameTokenType
            methodNameTokenType
            namespaceNameTokenType
            localNameTokenType
            stringEscapeCharacterTokenType
            fsharpFunctionTokenType
            fsharpMutableVarTokenType
            fsharpPrintfTokenType
            fsharpDisposableTypeTokenType
            fsharpDisposableLocalValueTokenType
            fsharpDisposableTopLevelValueTokenType
        |]

    let tokenModifiers = [| SemanticTokenModifiers.Static |]

    let legend () =
        SemanticTokensLegend(TokenTypes = tokenTypes, TokenModifiers = tokenModifiers)

    let private tokenTypeIndexes =
        tokenTypes
        |> Array.mapi (fun index tokenType -> tokenType, index)
        |> Map.ofArray

    let private tokenModifierIndexes =
        tokenModifiers
        |> Array.mapi (fun index modifier -> modifier, index)
        |> Map.ofArray

    let private sourcePriority =
        function
        | TokenSource.Semantic -> 0
        | TokenSource.Syntactic -> 1

    // Mapping mirrors vsintegration/src/FSharp.Editor/Classification/ClassificationDefinitions.fs::getClassificationTypeName
    // so that VS receives the same classification names through the LSP client. Six F#-specific
    // classifications (Function, MutableVar, Printf, Disposable*) require a matching F# entry in the
    // VS LSP client's token-type-to-classification provider; if that mapping is not present they
    // fall back to "text" in the editor.
    let private mapSemanticClassification =
        function
        | SemanticClassificationType.ReferenceType
        | SemanticClassificationType.ConstructorForReferenceType
        | SemanticClassificationType.Type
        | SemanticClassificationType.TypeDef
        | SemanticClassificationType.Module
        | SemanticClassificationType.Exception -> Some classNameTokenType
        | SemanticClassificationType.ValueType
        | SemanticClassificationType.ConstructorForValueType -> Some structNameTokenType
        | SemanticClassificationType.UnionCase
        | SemanticClassificationType.Enumeration -> Some enumNameTokenType
        | SemanticClassificationType.Interface -> Some interfaceNameTokenType
        | SemanticClassificationType.TypeArgument -> Some typeParameterNameTokenType
        | SemanticClassificationType.Delegate -> Some delegateNameTokenType
        | SemanticClassificationType.UnionCaseField
        | SemanticClassificationType.Field -> Some fieldNameTokenType
        | SemanticClassificationType.Property
        | SemanticClassificationType.RecordField
        | SemanticClassificationType.RecordFieldAsFunction -> Some propertyNameTokenType
        | SemanticClassificationType.Event -> Some eventNameTokenType
        | SemanticClassificationType.NamedArgument -> Some labelNameTokenType
        | SemanticClassificationType.Literal -> Some constantNameTokenType
        | SemanticClassificationType.Method -> Some methodNameTokenType
        | SemanticClassificationType.ExtensionMethod -> Some extensionMethodNameTokenType
        | SemanticClassificationType.Namespace -> Some namespaceNameTokenType
        | SemanticClassificationType.LocalValue -> Some localNameTokenType
        | SemanticClassificationType.Value -> Some SemanticTokenTypes.Variable
        | SemanticClassificationType.ComputationExpression
        | SemanticClassificationType.IntrinsicFunction -> Some SemanticTokenTypes.Keyword
        | SemanticClassificationType.Operator -> Some SemanticTokenTypes.Operator
        | SemanticClassificationType.Function -> Some fsharpFunctionTokenType
        | SemanticClassificationType.MutableVar
        | SemanticClassificationType.MutableRecordField -> Some fsharpMutableVarTokenType
        | SemanticClassificationType.Printf -> Some fsharpPrintfTokenType
        | SemanticClassificationType.DisposableType -> Some fsharpDisposableTypeTokenType
        | SemanticClassificationType.DisposableLocalValue -> Some fsharpDisposableLocalValueTokenType
        | SemanticClassificationType.DisposableTopLevelValue -> Some fsharpDisposableTopLevelValueTokenType
        | SemanticClassificationType.Plaintext -> None
        | _ -> None

    let private mapSyntacticToken (token: FSharpTokenInfo) =
        match token.ColorClass with
        | FSharpTokenColorKind.Keyword
        | FSharpTokenColorKind.PreprocessorKeyword -> Some SemanticTokenTypes.Keyword
        | FSharpTokenColorKind.Comment -> Some SemanticTokenTypes.Comment
        | FSharpTokenColorKind.String -> Some SemanticTokenTypes.String
        | FSharpTokenColorKind.Number -> Some SemanticTokenTypes.Number
        | FSharpTokenColorKind.Operator -> Some SemanticTokenTypes.Operator
        | _ ->
            match token.CharClass with
            | FSharpTokenCharKind.Keyword -> Some SemanticTokenTypes.Keyword
            | FSharpTokenCharKind.LineComment
            | FSharpTokenCharKind.Comment -> Some SemanticTokenTypes.Comment
            | FSharpTokenCharKind.String -> Some SemanticTokenTypes.String
            | FSharpTokenCharKind.Literal -> Some SemanticTokenTypes.Number
            | FSharpTokenCharKind.Operator -> Some SemanticTokenTypes.Operator
            | _ -> None

    let private lineLength (source: ISourceText) line =
        if line >= 0 && line < source.GetLineCount() then
            (source.GetLineString line).Length
        else
            0

    let private clamp minValue maxValue value = value |> max minValue |> min maxValue

    // Mirrors vsintegration/src/FSharp.Editor/LanguageService/Tokenizer.fs::fixupSpan.
    // FCS reports classifications for qualified names at the range of the full long ident
    // (e.g. SemanticClassificationType.DisposableType at the range of "System.IO.FileStream").
    // Without trimming, the outer range overlaps every per-segment classification (Namespace
    // at "System", Namespace at "System.IO") which resolveOverlaps then drops; the result is
    // the entire qualified name being colored as if it were the rightmost identifier.
    // Trim the range down to the rightmost identifier so each segment gets its own classification.
    // The Printf classification covers an intentionally non-identifier span ("%s" etc.) and must
    // not be trimmed.
    let private fixupRange (source: ISourceText) (tokenType: string) (range: range) =
        if
            tokenType = stringEscapeCharacterTokenType
            || tokenType = fsharpPrintfTokenType
            || range.StartLine <> range.EndLine
        then
            range
        else
            let line = range.StartLine - 1

            if line < 0 || line >= source.GetLineCount() then
                range
            else
                let lineText = source.GetLineString line
                let startCol = clamp 0 lineText.Length range.StartColumn
                let endCol = clamp startCol lineText.Length range.EndColumn

                if endCol <= startCol then
                    range
                else
                    let text = lineText.Substring(startCol, endCol - startCol)

                    if text.EndsWith("``") then
                        let searchEnd = text.Length - 3

                        if searchEnd < 0 then
                            range
                        else
                            match text.LastIndexOf("``", searchEnd) with
                            | -1
                            | 0 -> range
                            | index -> Range.mkRange range.FileName (Position.mkPos range.StartLine (startCol + index)) range.End
                    elif PrettyNaming.IsOperatorDisplayName text then
                        range
                    else
                        match text.LastIndexOf '.' with
                        | -1
                        | 0 -> range
                        | index -> Range.mkRange range.FileName (Position.mkPos range.StartLine (startCol + index + 1)) range.End

    let private splitRange (source: ISourceText) (range: range) tokenType modifiers sourceKind =
        [|
            let startLine = range.StartLine - 1
            let endLine = range.EndLine - 1

            if startLine >= 0 && endLine >= startLine then
                for line in startLine..endLine do
                    let lengthOfLine = lineLength source line

                    let startChar =
                        if line = startLine then
                            clamp 0 lengthOfLine range.StartColumn
                        else
                            0

                    let endChar =
                        if line = endLine then
                            clamp 0 lengthOfLine range.EndColumn
                        else
                            lengthOfLine

                    let length = endChar - startChar

                    if length > 0 then
                        {
                            Line = line
                            StartChar = startChar
                            Length = length
                            TokenType = tokenType
                            TokenModifiers = modifiers
                            Source = sourceKind
                        }
        |]

    let fromSemanticClassifications (source: ISourceText) (items: SemanticClassificationItem array) =
        items
        |> Array.collect (fun item ->
            match mapSemanticClassification item.Type with
            | Some tokenType ->
                let fixedRange = fixupRange source tokenType item.Range
                splitRange source fixedRange tokenType [] TokenSource.Semantic
            | None -> [||])

    let fromTokenizer (parsingOptions: FSharpParsingOptions option) (fileName: string) (source: ISourceText) =
        let defines, langVersion, strictIndentation =
            match parsingOptions with
            | Some opts -> CompilerEnvironment.GetConditionalDefinesForEditing opts, Some opts.LangVersionText, opts.StrictIndentation
            | None -> [], None, None

        let tokenizer =
            FSharpSourceTokenizer(defines, Some fileName, langVersion, strictIndentation)

        let mutable state = FSharpTokenizerLexState.Initial

        [|
            for line in 0 .. source.GetLineCount() - 1 do
                let lineText = source.GetLineString line
                // Should we cache the results?
                let lineTokenizer = tokenizer.CreateLineTokenizer lineText
                let mutable keepScanning = true

                while keepScanning do
                    let token, nextState = lineTokenizer.ScanToken state
                    state <- nextState

                    match token with
                    | Some tokenInfo ->
                        match mapSyntacticToken tokenInfo with
                        | Some tokenType ->
                            let length = tokenInfo.RightColumn - tokenInfo.LeftColumn + 1

                            if length > 0 then
                                {
                                    Line = line
                                    StartChar = tokenInfo.LeftColumn
                                    Length = length
                                    TokenType = tokenType
                                    TokenModifiers = []
                                    Source = TokenSource.Syntactic
                                }
                        | None -> ()
                    | None -> keepScanning <- false
        |]

    let private overlaps existing candidate =
        existing.Line = candidate.Line
        && candidate.StartChar < existing.StartChar + existing.Length
        && existing.StartChar < candidate.StartChar + candidate.Length

    let private subtractOverlap existing candidate =
        if not (overlaps existing candidate) then
            [ candidate ]
        else
            let existingEnd = existing.StartChar + existing.Length
            let candidateEnd = candidate.StartChar + candidate.Length

            [
                if candidate.StartChar < existing.StartChar then
                    { candidate with
                        Length = existing.StartChar - candidate.StartChar
                    }

                if existingEnd < candidateEnd then
                    { candidate with
                        StartChar = existingEnd
                        Length = candidateEnd - existingEnd
                    }
            ]

    let resolveOverlaps (tokens: NormalizedToken array) =
        let sorted =
            tokens
            |> Array.sortBy (fun token -> sourcePriority token.Source, token.Line, token.StartChar, -token.Length)

        let accepted = ResizeArray()

        for token in sorted do
            match token.Source with
            | TokenSource.Semantic ->
                if not (accepted |> Seq.exists (fun existing -> overlaps existing token)) then
                    accepted.Add token
            | TokenSource.Syntactic ->
                let fragments =
                    ([ token ], accepted)
                    ||> Seq.fold (fun fragments existing -> fragments |> List.collect (subtractOverlap existing))

                for fragment in fragments do
                    if
                        fragment.Length > 0
                        && not (accepted |> Seq.exists (fun existing -> overlaps existing fragment))
                    then
                        accepted.Add fragment

        accepted.ToArray()

    // Assumes single-line tokens (NormalizedToken has no end-line field; splitRange already
    // breaks any multi-line semantic range into one token per line).
    let private intersectsRange (range: Range) token =
        let line = token.Line

        line >= range.Start.Line
        && line <= range.End.Line
        && (line <> range.Start.Line
            || token.StartChar + token.Length > range.Start.Character)
        && (line <> range.End.Line || token.StartChar < range.End.Character)

    let filterRange (range: Range option) (tokens: NormalizedToken array) =
        match range with
        | None -> tokens
        | Some range -> tokens |> Array.filter (intersectsRange range)

    let encode (tokens: NormalizedToken array) =
        let sorted = tokens |> Array.sortBy (fun token -> token.Line, token.StartChar)
        let data = ResizeArray(sorted.Length * 5)
        let mutable previousLine = 0
        let mutable previousStart = 0

        for token in sorted do
            match tokenTypeIndexes |> Map.tryFind token.TokenType with
            | None -> ()
            | Some tokenTypeIndex ->
                let deltaLine = token.Line - previousLine

                let deltaStart =
                    if deltaLine = 0 then
                        token.StartChar - previousStart
                    else
                        token.StartChar

                let modifierBits =
                    token.TokenModifiers
                    |> List.fold
                        (fun bits modifier ->
                            match tokenModifierIndexes |> Map.tryFind modifier with
                            | Some index -> bits ||| (1 <<< index)
                            | None -> bits)
                        0

                data.Add deltaLine
                data.Add deltaStart
                data.Add token.Length
                data.Add tokenTypeIndex
                data.Add modifierBits

                previousLine <- token.Line
                previousStart <- token.StartChar

        data.ToArray()

    let emptyResponse () = SemanticTokens(Data = [||])

    let responseFor source fileName semanticItems range parsingOptions =
        let tokens =
            Array.append (fromSemanticClassifications source semanticItems) (fromTokenizer parsingOptions fileName source)
            |> resolveOverlaps
            |> filterRange range

        SemanticTokens(Data = encode tokens)
