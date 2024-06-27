namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Tokenization
open System
open System.Threading
open System.Threading.Tasks

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
            flags = (FSharpLexerFlags.Default &&& ~~~FSharpLexerFlags.Compiling &&& ~~~FSharpLexerFlags.UseLexFilter),
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

    let toIndex (x: string) = SemanticTokenTypes.AllTypes |> Seq.findIndex (fun y -> y = x)

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace, checker: FSharpChecker) =
    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace
    member _.Checker = checker

    // TODO: split to parse and check diagnostics
    member _.GetDiagnosticsForFile(file: Uri) =

        workspace.GetSnapshotForFile file
        |> Option.map (fun snapshot ->
            async {
                let! parseResult, checkFileAnswer = checker.ParseAndCheckFileInProject(file.LocalPath, snapshot, "LSP Get diagnostics")

                return
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Succeeded result -> result.Diagnostics
                    | FSharpCheckFileAnswer.Aborted -> parseResult.Diagnostics
            })
        |> Option.defaultValue (async.Return [||])

    member _.GetSemanticTokensForFile(file: Uri) =

        workspace.GetSnapshotForFile file
        |> Option.map (fun snapshot ->
            async {
                let! _, checkFileAnswer = checker.ParseAndCheckFileInProject(file.LocalPath, snapshot, "LSP Get semantic classification")

                let semanticClassifications =
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Succeeded result -> result.GetSemanticClassification(None) // XXX not sure if range opt should be None
                    | FSharpCheckFileAnswer.Aborted -> [||] // XXX should be error maybe

                let! source =
                    snapshot.ProjectSnapshot.SourceFiles
                    |> Seq.find (fun f -> f.FileName = file.LocalPath)
                    |> _.GetSource()
                    |> Async.AwaitTask

                let syntacticClassifications = TokenTypes.GetSyntacticTokenTypes source file.LocalPath

                let lspFormatTokens =
                    semanticClassifications
                    |> Array.map (fun item -> (item.Range, item.Type |> TokenTypes.FSharpTokenTypeToLSP |> TokenTypes.toIndex))
                    |> Array.append (syntacticClassifications|> List.map (fun (r, t) -> (r, TokenTypes.toIndex t)) |> Array.ofList)
                    |> Array.map (fun (r, tokType) ->
                        let length = r.EndColumn - r.StartColumn // XXX Does not deal with multiline tokens?
                        {| startLine = r.StartLine - 1; startCol = r.StartColumn; length = length; tokType = tokType; tokMods = 0 |})
                        //(startLine, startCol, length, tokType, tokMods))
                    |> Array.sortWith (fun x1 x2 ->
                        let c = x1.startLine.CompareTo(x2.startLine)
                        if c <> 0 then c
                        else x1.startCol.CompareTo(x2.startCol))

                let tokensRelative =
                    lspFormatTokens
                    |> Array.append [| {| startLine = 0; startCol = 0; length = 0; tokType = 0; tokMods = 0 |} |]
                    |> Array.pairwise
                    |> Array.map (fun (prev, this) ->
                        {|
                            startLine = this.startLine - prev.startLine
                            startCol = (if prev.startLine = this.startLine then this.startCol - prev.startCol else this.startCol)
                            length = this.length
                            tokType = this.tokType
                            tokMods = this.tokMods
                        |})

                return tokensRelative
                    |> Array.map (fun tok ->
                        [| tok.startLine; tok.startCol; tok.length; tok.tokType; tok.tokMods |])
                    |> Array.concat
            })
        |> Option.defaultValue (async { return [||] })

type ContextHolder(intialWorkspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()

    // TODO: We need to get configuration for this somehow. Also make it replaceable when configuration changes.
    let checker =
        FSharpChecker.Create(
            keepAllBackgroundResolutions = true,
            keepAllBackgroundSymbolUses = true,
            enableBackgroundItemKeyStoreAndSemanticClassification = true,
            enablePartialTypeChecking = true,
            parallelReferenceResolution = true,
            captureIdentifiersWhenParsing = true,
            useSyntaxTreeCache = true,
            useTransparentCompiler = true
        )

    let mutable context =
        FSharpRequestContext(lspServices, logger, intialWorkspace, checker)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) =
        context <- FSharpRequestContext(lspServices, logger, f context.Workspace, checker)

type FShapRequestContextFactory(lspServices: ILspServices) =

    interface IRequestContextFactory<FSharpRequestContext> with

        member _.CreateRequestContextAsync<'TRequestParam>
            (
                queueItem: IQueueItem<FSharpRequestContext>,
                requestParam: 'TRequestParam,
                cancellationToken: CancellationToken
            ) =
            lspServices.GetRequiredService<ContextHolder>()
            |> _.GetContext()
            |> Task.FromResult

