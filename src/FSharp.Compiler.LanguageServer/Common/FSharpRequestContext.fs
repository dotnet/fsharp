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

        let FSharpTokenTypeToLSP (fst: SemanticClassificationType) =
            // XXX arbitrary
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

        workspace.GetSnapshotForFile file
        |> Option.map (fun snapshot ->
            async {
                let! _, checkFileAnswer = checker.ParseAndCheckFileInProject(file.LocalPath, snapshot, "LSP Get semantic classification")

                let! source =
                    snapshot.ProjectSnapshot.SourceFiles
                    |> Seq.find (fun f -> f.FileName = file.LocalPath)
                    |> _.GetSource()
                    |> Async.AwaitTask

                let mutable a' = []

                let tokenCallback =
                    fun (tok: FSharpToken) ->
                        let spanKind =
                            if tok.IsKeyword then
                                SemanticTokenTypes.Keyword
                            elif tok.IsNumericLiteral then
                                SemanticTokenTypes.Number
                            elif tok.IsCommentTrivia then
                                SemanticTokenTypes.Comment
                            elif tok.IsStringLiteral then
                                SemanticTokenTypes.String
                            else
                                SemanticTokenTypes.Function // XXX

                        a' <- (tok.Range, spanKind)::a'

                FSharpLexer.Tokenize(
                    source,
                    tokenCallback,
                    flags = (FSharpLexerFlags.Default &&& ~~~FSharpLexerFlags.Compiling &&& ~~~FSharpLexerFlags.UseLexFilter),
                    filePath = file.LocalPath
                )

                let a =
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Succeeded result -> result.GetSemanticClassification(None) // XXX not sure if range opt should be None
                    | FSharpCheckFileAnswer.Aborted -> [||] // XXX should be error maybe

                let b =
                    a
                    |> Array.map (fun y ->
                        let startLine = y.Range.StartLine - 1
                        let startCol = y.Range.StartColumn
                        let length = y.Range.EndColumn - y.Range.StartColumn // XXX Does not deal with multiline tokens?
                        let tokType = y.Type |> FSharpTokenTypeToLSP |> toIndex
                        let tokMods = 0
                        (startLine, startCol, length, tokType, tokMods)
                    )
                let b' =
                    a'
                    |> List.map (fun (r, t) ->
                        let startLine = r.StartLine - 1
                        let startCol = r.StartColumn
                        let length = r.EndColumn - r.StartColumn // XXX
                        let tokType = t |> toIndex
                        let tokMods = 0
                        (startLine, startCol, length, tokType, tokMods)
                    )
                let b'' =
                    Array.ofList b'
                    |> Array.append b
                    |> Array.sortWith (fun (l1, c1, _, _, _) (l2, c2, _, _, _) ->
                        let c = l1.CompareTo(l2)
                        if c <> 0 then c
                        else c1.CompareTo(c2))

                let c =
                    b''
                    |> Array.append [|(0,0,0,0,0)|]
                    |> Array.pairwise
                    |> Array.map (fun (prev, this) ->
                        let (prevSLine, prevSCol, _, _, _) = prev
                        let (thisSLine, thisSCol, length, tokType, tokMods) = this
                        (thisSLine - prevSLine, (if prevSLine = thisSLine then thisSCol - prevSCol else thisSCol), length, tokType, tokMods))

                return c |> Array.map (fun (v, w, x, y, z) -> [| v; w; x; y; z |]) |> Array.concat
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

