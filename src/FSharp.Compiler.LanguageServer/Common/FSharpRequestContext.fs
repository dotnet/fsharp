namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System.Threading
open System.Threading.Tasks

open System
open System.Collections.Generic
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Tokenization

open System.Threading
open FSharp.Compiler.CodeAnalysis.Workspace

#nowarn "57"

module TokenTypes =

    [<return: Struct>]
    let (|LexicalClassification|_|) (tok: FSharpToken) =
        if tok.IsKeyword then
            ValueSome SemanticTokenTypes.Keyword
        elif tok.IsNumericLiteral then
            ValueSome SemanticTokenTypes.Number
        elif tok.IsCommentTrivia then
            ValueSome SemanticTokenTypes.Comment
        elif tok.IsStringLiteral then
            ValueSome SemanticTokenTypes.String
        else
            ValueNone

    // Tokenizes the source code and returns a list of token ranges and their SemanticTokenTypes
    let GetSyntacticTokenTypes (source: FSharp.Compiler.Text.ISourceText) (fileName: string) =
        let mutable tokRangesAndTypes = []

        let tokenCallback =
            fun (tok: FSharpToken) ->
                match tok with
                | LexicalClassification tokType -> tokRangesAndTypes <- (tok.Range, tokType) :: tokRangesAndTypes
                | _ -> ()

        FSharpLexer.Tokenize(
            source,
            tokenCallback,
            flags =
                (FSharpLexerFlags.Default
                 &&& ~~~FSharpLexerFlags.Compiling
                 &&& ~~~FSharpLexerFlags.UseLexFilter),
            filePath = fileName
        )

        tokRangesAndTypes

    let FSharpTokenTypeToLSP (fst: SemanticClassificationType) =
        // XXX kinda arbitrary mapping
        match fst with
        | SemanticClassificationType.ReferenceType -> SemanticTokenTypes.Class
        | SemanticClassificationType.ValueType -> SemanticTokenTypes.Struct
        | SemanticClassificationType.UnionCase -> SemanticTokenTypes.Enum
        | SemanticClassificationType.UnionCaseField -> SemanticTokenTypes.EnumMember
        | SemanticClassificationType.Function -> SemanticTokenTypes.Function
        | SemanticClassificationType.Property -> SemanticTokenTypes.Property
        | SemanticClassificationType.Module -> SemanticTokenTypes.Type
        | SemanticClassificationType.Namespace -> SemanticTokenTypes.Namespace
        | SemanticClassificationType.Interface -> SemanticTokenTypes.Interface
        | SemanticClassificationType.TypeArgument -> SemanticTokenTypes.TypeParameter
        | SemanticClassificationType.Operator -> SemanticTokenTypes.Operator
        | SemanticClassificationType.Method -> SemanticTokenTypes.Method
        | SemanticClassificationType.ExtensionMethod -> SemanticTokenTypes.Method
        | SemanticClassificationType.Field -> SemanticTokenTypes.Property
        | SemanticClassificationType.Event -> SemanticTokenTypes.Event
        | SemanticClassificationType.Delegate -> SemanticTokenTypes.Function
        | SemanticClassificationType.NamedArgument -> SemanticTokenTypes.Parameter
        | SemanticClassificationType.LocalValue -> SemanticTokenTypes.Variable
        | SemanticClassificationType.Plaintext -> SemanticTokenTypes.String
        | SemanticClassificationType.Type -> SemanticTokenTypes.Type
        | SemanticClassificationType.Printf -> SemanticTokenTypes.Keyword
        | _ -> SemanticTokenTypes.Comment

    let toIndex (x: string) =
        SemanticTokenTypes.AllTypes |> Seq.findIndex (fun y -> y = x)

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace) =
    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace

    member this.GetSemanticTokensForFile(file) =
        task {
            let! scv = this.Workspace.Query.GetSemanticClassification file
            let! source = this.Workspace.Query.GetSource file

            match scv, source with
            | Some view, Some source ->

                let tokens = ResizeArray()

                view.ForEach(fun item ->
                    let range = item.Range
                    let tokenType = item.Type |> TokenTypes.FSharpTokenTypeToLSP |> TokenTypes.toIndex
                    tokens.Add(range, tokenType))

                let syntacticClassifications =
                    TokenTypes.GetSyntacticTokenTypes source file.LocalPath
                    |> Seq.map (fun (r, t) -> (r, TokenTypes.toIndex t))

                let allTokens = Seq.append tokens syntacticClassifications

                let lspFormatTokens =
                    allTokens
                    |> Seq.map (fun (r, tokType) ->
                        let length = r.EndColumn - r.StartColumn // XXX Does not deal with multiline tokens?

                        {|
                            startLine = r.StartLine - 1
                            startCol = r.StartColumn
                            length = length
                            tokType = tokType
                            tokMods = 0
                        |})
                    //(startLine, startCol, length, tokType, tokMods))
                    |> Seq.sortWith (fun x1 x2 ->
                        let c = x1.startLine.CompareTo(x2.startLine)
                        if c <> 0 then c else x1.startCol.CompareTo(x2.startCol))

                let tokensRelative =
                    lspFormatTokens
                    |> Seq.append
                        [|
                            {|
                                startLine = 0
                                startCol = 0
                                length = 0
                                tokType = 0
                                tokMods = 0
                            |}
                        |]
                    |> Seq.pairwise
                    |> Seq.map (fun (prev, this) ->
                        {|
                            startLine = this.startLine - prev.startLine
                            startCol =
                                (if prev.startLine = this.startLine then
                                     this.startCol - prev.startCol
                                 else
                                     this.startCol)
                            length = this.length
                            tokType = this.tokType
                            tokMods = this.tokMods
                        |})

                return
                    tokensRelative
                    |> Seq.map (fun tok -> [| tok.startLine; tok.startCol; tok.length; tok.tokType; tok.tokMods |])
                    |> Seq.concat
                    |> Seq.toArray

            | _ -> return [||]
        }

type ContextHolder(workspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()

    let context = FSharpRequestContext(lspServices, logger, workspace)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) = f context.Workspace

type FShapRequestContextFactory(lspServices: ILspServices) =

    inherit AbstractRequestContextFactory<FSharpRequestContext>()

    override _.CreateRequestContextAsync<'TRequestParam>
        (
            _queueItem: IQueueItem<FSharpRequestContext>,
            _methodHandler: IMethodHandler,
            _requestParam: 'TRequestParam,
            _cancellationToken: CancellationToken
        ) =
        lspServices.GetRequiredService<ContextHolder>()
        |> _.GetContext()
        |> Task.FromResult
