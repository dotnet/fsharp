namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
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

                let a =
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Succeeded result -> result.GetSemanticClassification(None) // XXX not sure if range opt should be None
                    | FSharpCheckFileAnswer.Aborted -> [||] // XXX should be error maybe

                // XXX Not sure if this is needed, if so TODO should be declared out of the scope of this function
                let compareRange (x: FSharp.Compiler.EditorServices.SemanticClassificationItem) (y: FSharp.Compiler.EditorServices.SemanticClassificationItem) =
                    let c = x.Range.StartLine.CompareTo(y.Range.StartLine)
                    if c <> 0 then c
                    else x.Range.StartColumn.CompareTo(y.Range.StartColumn)

                let b =
                    a
                    |> Array.sortWith compareRange
                    |> Array.map (fun y ->
                        let startLine = y.Range.StartLine - 1
                        let startCol = y.Range.StartColumn
                        let length = y.Range.EndColumn - y.Range.StartColumn // XXX
                        let tokType = y.Type |> FSharpTokenTypeToLSP |> toIndex
                        let tokMods = 0
                        (startLine, startCol, length, tokType, tokMods)
                    )
                let c =
                    b
                    |> Array.append [|(0,0,0,0,0)|]
                    |> Array.pairwise
                    |> Array.map (fun (prev, this) ->
                        let (prevSLine, prevSCol, _, _, _) = prev
                        let (thisSLine, thisSCol, length, tokType, tokMods) = this
                        (thisSLine - prevSLine, (if prevSLine = thisSLine then thisSCol - prevSCol else thisSCol), length, tokType, tokMods))

                let _TODO_DELETE_readable_names =
                    b |> Array.map( fun (_, _, _, t, _) -> SemanticTokenTypes.AllTypes.[t])

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

