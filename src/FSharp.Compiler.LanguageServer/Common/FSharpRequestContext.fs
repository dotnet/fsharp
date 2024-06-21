namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open FSharp.Compiler.CodeAnalysis
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

