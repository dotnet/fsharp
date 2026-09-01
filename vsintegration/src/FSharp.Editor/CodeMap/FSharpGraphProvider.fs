// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler.Syntax

open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell

open Microsoft.VisualStudio.GraphModel
open Microsoft.VisualStudio.GraphModel.CodeSchema
open Microsoft.VisualStudio.GraphModel.Schemas
open Microsoft.VisualStudio.Progression
open Microsoft.VisualStudio.Progression.CodeSchema.Api

open CancellableTasks

/// Resolves the F# frames of a debugger call stack shown on a Code Map. The debugger contributes
/// `CodeSchema_CallStackMethod` nodes carrying nothing but the module and the mangled frame name;
/// without a provider claiming them they stay unresolved and carry no source location.
[<LegacyProvider(typeof<IProvider>,
                 Name = "FSharpProvider",
                 Priority = 2.0,
                 IntellisenseType = "{BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B}",
                 ProjectCapability = "FSharp")>]
type internal FSharpGraphProvider() =

    /// The frame resolution runs on a Progression worker, so a project that is slow to check must
    /// leave the node unresolved rather than stall the whole map.
    let resolutionTimeout = TimeSpan.FromSeconds 30.0

    let schema = Graph()

    let workspace =
        lazy
            let componentModel =
                Package.GetGlobalService(typeof<SComponentModel>) :?> IComponentModel

            componentModel.GetService<VisualStudioWorkspace>()

    /// The debugger encodes the frame's module and mangled name into the node id; `CallStackMethodNode`
    /// is the schema's own reader for it. The module Uri is relative (`ClassLibrary.dll`) for
    /// workspace assemblies and absolute for external ones.
    let frameOf (node: GraphNode) =
        let frame = CallStackMethodNode node

        match frame.Module, frame.FunctionName with
        | null, _ -> ValueNone
        | _, (null | "") -> ValueNone
        | moduleUri, functionName ->
            let modulePath =
                if moduleUri.IsAbsoluteUri then
                    moduleUri.LocalPath
                else
                    moduleUri.OriginalString

            ValueSome(Path.GetFileNameWithoutExtension modulePath, functionName)

    /// Builds the same node identity Roslyn's `GraphNodeIdCreation.GetIdForMemberAsync` produced,
    /// so a frame resolved here fuses with the node the metadata provider creates for the same
    /// method: a nested type becomes `Type=(Name=… ParentType=…)`, a generic one carries
    /// `GenericParameterCount`, and only a top-level non-generic type collapses to a plain string.
    let rec private typePartial (chain: struct (string * int) array) upTo (nodeName: GraphNodeIdName) =
        let struct (name, arity) = chain.[upTo]

        if upTo = 0 && arity = 0 then
            GraphNodeId.GetPartial(nodeName, name)
        else
            let partials =
                [|
                    GraphNodeId.GetPartial(Microsoft.VisualStudio.Progression.CodeSchema.CodeQualifiedName.Name, name)
                    if arity > 0 then
                        GraphNodeId.GetPartial(CodeGraphNodeIdName.GenericParameterCountIdentifier, string arity)
                    if upTo > 0 then
                        typePartial chain (upTo - 1) CodeGraphNodeIdName.ParentType
                |]

            let value: obj =
                if partials.Length > 1 then
                    GraphNodeIdCollection(false, partials)
                else
                    GraphNodeId.GetNested partials

            GraphNodeId.GetPartial(nodeName, value)

    let resolvedNodeId (resolved: ResolvedFrame) =
        [|
            match resolved.Project.OutputFilePath with
            | null -> ()
            | path -> GraphNodeId.GetPartial(CodeGraphNodeIdName.Assembly, Uri(path))
            match resolved.Namespace with
            | ValueSome ns -> GraphNodeId.GetPartial(CodeGraphNodeIdName.Namespace, ns)
            | ValueNone -> ()
            if resolved.TypeChain.Length > 0 then
                typePartial resolved.TypeChain (resolved.TypeChain.Length - 1) CodeGraphNodeIdName.Type
            match resolved.MemberName with
            | ValueSome name -> GraphNodeId.GetPartial(CodeGraphNodeIdName.Member, name)
            | ValueNone -> ()
        |]
        |> GraphNodeId.GetNested

    let sourceLocationOf (resolved: ResolvedFrame) =
        let range = resolved.DeclarationRange

        SourceLocation(Uri(range.FileName), Position(range.StartLine - 1, range.StartColumn), Position(range.EndLine - 1, range.EndColumn))

    /// Checks every reported assembly concurrently, each one exactly once for the whole stack.
    /// `ActionManager` runs the action on a `JobQueue` worker, so blocking here never reaches the
    /// UI - but the handler returns void and the platform continues into the next pipeline step the
    /// moment it does, so the graph has to be complete before we return.
    let resolveAll (context: ActionContext) (framesByAssembly: struct (string * ParsedFrame array) array) =
        cancellableTask {
            let! ambient = CancellableTask.getCancellationToken ()
            use cancellation = CancellationTokenSource.CreateLinkedTokenSource ambient
            cancellation.CancelAfter resolutionTimeout
            use _ = context.Cancelled.Subscribe(fun _ -> cancellation.Cancel())

            try
                let resolveEverything =
                    framesByAssembly
                    |> Seq.map (fun struct (assemblyName, frames) ->
                        FSharpCallStackResolver.tryResolveMany workspace.Value assemblyName frames)
                    |> CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount)

                return! resolveEverything cancellation.Token
            with e when not (e :? OperationCanceledException) ->
                Trace.WriteLine $"[FSharpCodeMap] resolution failed: {e.Message}"

                return
                    framesByAssembly
                    |> Array.map (fun struct (_, frames) -> frames |> Array.map (fun _ -> ValueNone))
        }

    /// The synthesized parts of a frame name (`helperTwo@42.Invoke`, `op_PlusBangPlus`) make ugly
    /// node labels even when full resolution fails; the parsed shape always yields a better one.
    let friendlyLabel (frame: ParsedFrame) =
        match frame.Member with
        | FrameClosureBody origin -> ValueSome origin.EnclosingName
        | FramePropertyGetter name
        | FramePropertySetter name -> ValueSome name
        | FrameMethod name when PrettyNaming.IsLogicalOpName name ->
            ValueSome $"({PrettyNaming.ConvertValLogicalNameToDisplayNameCore name})"
        | FrameStartupCode ->
            match frame.Path with
            | [||] -> ValueNone
            | path -> ValueSome (Array.last path).Name
        | FrameMethod _
        | FrameConstructor
        | FrameStaticConstructor
        | FrameActivePattern _ -> ValueNone

    /// Mirrors what the built-in C#/VB resolver does for a frame it recognises: create the method
    /// node, give it a source location, and link the frame to it - all inside a graph transaction.
    let attachResolvedMethod (graph: Graph) (frameNode: GraphNode) (resolved: ResolvedFrame) =
        let label =
            match resolved.MemberName with
            | ValueSome name -> name
            | ValueNone -> String.Join(".", resolved.EntityPath)

        use transaction = new GraphTransactionScope()

        let methodNode =
            graph.Nodes.GetOrCreate(resolvedNodeId resolved, label, CodeNodeCategories.Method)

        methodNode.SetValue(CodeNodeProperties.SourceLocation, sourceLocationOf resolved)
        |> ignore

        CodeSchemaHelper.GetOrCreateLink<CallStackMethodReferencesMethodLink>(graph, frameNode.Id, methodNode.Id, String.Empty)
        |> ignore

        transaction.Complete()

    let resolveCallStack (context: ActionContext) =
        cancellableTask {
            let graph = context.Graph

            // The platform hands the frame nodes in as the action's input; a node another provider
            // already claimed is excluded. Snapshot before resolution starts mutating the graph.
            let candidates =
                [|
                    for frameNode in context.UnhandledInputNodes() do
                        match frameOf frameNode with
                        | ValueNone -> ()
                        | ValueSome(assemblyName, functionName) ->
                            match FSharpStackFrameNameParser.parse functionName with
                            | ValueNone -> ()
                            | ValueSome parsed -> yield assemblyName, frameNode, parsed
                |]

            if candidates.Length > 0 then
                use labelling = new GraphTransactionScope()

                for _, frameNode, parsed in candidates do
                    friendlyLabel parsed |> ValueOption.iter (fun label -> frameNode.Label <- label)

                labelling.Complete()

            let byAssembly =
                candidates
                |> Array.groupBy (fun (assemblyName, _, _) -> assemblyName)
                |> Array.map (fun (assemblyName, group) ->
                    assemblyName, group |> Array.map (fun (_, frameNode, parsed) -> frameNode, parsed))

            let! resolutions =
                byAssembly
                |> Array.map (fun (assemblyName, group) -> struct (assemblyName, group |> Array.map snd))
                |> resolveAll context

            let mutable resolvedCount = 0

            for (_, group), results in Array.zip byAssembly resolutions do
                for (frameNode, _), resolved in Array.zip group results do
                    match resolved with
                    | ValueNone -> ()
                    | ValueSome resolved ->
                        try
                            attachResolvedMethod graph frameNode resolved
                            context.AddHandled frameNode
                            resolvedCount <- resolvedCount + 1
                        with e ->
                            // An unresolvable frame is expected - it stays a grey unresolved node, and
                            // one bad frame must not abandon the rest of the stack.
                            Trace.WriteLine $"[FSharpCodeMap] frame failed: {e.Message}"

            Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack: {resolvedCount}/{candidates.Length} F# frames resolved"
        }

    interface IProvider with
        member _.Schema = schema

        member _.Initialize(_serviceProvider: IServiceProvider) =
            Trace.WriteLine "[FSharpCodeMap] FSharpGraphProvider.Initialize"

            // The handler contract is synchronous: `ActionManager` runs it on a `JobQueue` worker
            // and continues into the next pipeline step the moment it returns, so the graph must be
            // complete by then. `JoinableTaskFactory.Run` is the same blocking bridge the built-in
            // C#/VB provider uses for this action.
            Actions.ResolveCallStack.ActionHandlers.Add(
                ActionHandler(fun context ->
                    ThreadHelper.JoinableTaskFactory.Run(fun () -> resolveCallStack context CancellationToken.None :> Task))
            )
