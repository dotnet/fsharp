// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell

open Microsoft.VisualStudio.GraphModel
open Microsoft.VisualStudio.GraphModel.CodeSchema
open Microsoft.VisualStudio.GraphModel.Schemas
open Microsoft.VisualStudio.Progression
open Microsoft.VisualStudio.Progression.CodeSchema.Api

open CancellableTasks

/// `Microsoft.VisualStudio.GraphModel.CodeSchema` is already open and the two namespaces share type
/// names, so the Progression half is reached through aliases rather than a second `open`.
type private CodeSchemaProperties = Microsoft.VisualStudio.Progression.CodeSchema.Properties
type private CodeQualifiedName = Microsoft.VisualStudio.Progression.CodeSchema.CodeQualifiedName
type private ProgressionNodeCategories = Microsoft.VisualStudio.Progression.CodeSchema.NodeCategories
type private ProgressionLinkCategories = Microsoft.VisualStudio.Progression.LinkCategories

/// The code schema was shaped for C# and VB and has nowhere to record that a frame came from a
/// module, a union, an active pattern or an inline function. A property carries its own schema, so
/// declaring them here is enough for them to reach the Properties pane and DGML styles; the schema
/// itself is never handed to a graph, as with Roslyn's `RoslynGraphProperties`.
[<RequireQualifiedAccess>]
module internal FSharpGraphSchema =

    let private schema = GraphSchema "FSharp"

    let private declare name =
        schema.Properties.AddNewProperty(name, typeof<bool>)

    let IsModule = declare "FSharpProperty_IsModule"
    let IsUnion = declare "FSharpProperty_IsUnion"
    let IsRecord = declare "FSharpProperty_IsRecord"
    let IsException = declare "FSharpProperty_IsException"
    let IsMeasure = declare "FSharpProperty_IsMeasure"
    let IsActivePattern = declare "FSharpProperty_IsActivePattern"
    let IsUnionCaseTester = declare "FSharpProperty_IsUnionCaseTester"
    let IsFunction = declare "FSharpProperty_IsFunction"
    let IsInline = declare "FSharpProperty_IsInline"
    let IsMutable = declare "FSharpProperty_IsMutable"
    let IsLifted = declare "FSharpProperty_IsLifted"

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

    let workspace =
        lazy
            let componentModel =
                Package.GetGlobalService(typeof<SComponentModel>) :?> IComponentModel

            componentModel.GetService<VisualStudioWorkspace>()

    /// The debugger leaves the module empty on accessors and on module initialization. The Uri is
    /// relative (`ClassLibrary.dll`) for workspace assemblies and absolute for external ones.
    let moduleAssemblyOf (frame: CallStackMethodNode) =
        match frame.Module with
        | null -> ValueNone
        | moduleUri ->
            let modulePath =
                if moduleUri.IsAbsoluteUri then
                    moduleUri.LocalPath
                else
                    moduleUri.OriginalString

            ValueSome(Path.GetFileNameWithoutExtension modulePath)

    let isFSharpCoreMethod (methodNode: GraphNode) =
        FSharpCallStackPlan.isFSharpCoreModule (moduleAssemblyOf (CallStackMethodNode methodNode))

    /// The links by which frames claim a method node: in an accumulated map, more than one.
    let frameLinksOf (methodNode: GraphNode) =
        methodNode.IncomingLinks
        |> Seq.filter (fun link ->
            link.HasCategory ProgressionLinkCategories.References
            && link.Source.HasCategory ProgressionNodeCategories.CallStackFrame)

    /// The position the debugger resolved for a frame. It is read off the frame nodes: the pipeline
    /// stamps `CodeNodeProperties.SourceLocation` on a method node only in `AddSourceLocations`,
    /// which runs after this handler. A frame's `StartLine` is 1-based, as an FCS range is.
    let frameSourcePosition (methodNode: GraphNode) =
        frameLinksOf methodNode
        |> Seq.chooseV (fun link ->
            let frame = CallStackFrameNode link.Source

            match frame.FileName with
            | null
            | "" -> ValueNone
            | file -> ValueSome { File = file; Line = frame.StartLine })
        |> FSharpCallStackPlan.positionOf

    let assemblyNameForFile (file: string) =
        workspace.Value.CurrentSolution.GetDocumentIdsWithFilePath file
        |> Seq.tryHeadV
        |> ValueOption.bind (fun documentId ->
            match workspace.Value.CurrentSolution.GetProject documentId.ProjectId with
            | null -> ValueNone
            | project -> ValueSome project.AssemblyName)

    /// Builds the same node identity Roslyn's `GraphNodeIdCreation.GetIdForMemberAsync` produced,
    /// so a frame resolved here fuses with the node the metadata provider creates for the same
    /// method: a nested type becomes `Type=(Name=… ParentType=…)`, a generic one carries
    /// `GenericParameterCount`, and only a top-level non-generic type collapses to a plain string.
    let rec typePartial (chain: struct (string * int) array) upTo (nodeName: GraphNodeIdName) =
        let struct (name, arity) = chain.[upTo]

        if upTo = 0 && arity = 0 then
            GraphNodeId.GetPartial(nodeName, name)
        else
            let partials =
                [|
                    GraphNodeId.GetPartial(CodeQualifiedName.Name, name)
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

    /// Checks every reported assembly concurrently, each one exactly once for the whole stack. The
    /// handler returns void and the platform continues into the next pipeline step the moment it
    /// does, so the graph has to be complete before this returns.
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
                    |> Array.map (fun struct (_, frames) -> Array.create frames.Length ValueNone)
        }

    /// The graph properties a trait sets. A lifted frame sets two: `IsAnonymous` is how the built-in
    /// provider marks a C# lambda, and the map's styles key off it.
    let propertiesOf =
        function
        | Public -> [ CodeSchemaProperties.IsPublic ]
        | Internal -> [ CodeSchemaProperties.IsInternal ]
        | Private -> [ CodeSchemaProperties.IsPrivate ]
        | Static -> [ CodeSchemaProperties.IsStatic ]
        | Generic -> [ CodeSchemaProperties.IsGeneric ]
        | Constructor -> [ CodeSchemaProperties.IsConstructor ]
        | Operator -> [ CodeSchemaProperties.IsOperator ]
        | Extension -> [ CodeSchemaProperties.IsExtension ]
        | Abstract -> [ CodeSchemaProperties.IsAbstract ]
        | CompilerGenerated -> [ CodeSchemaProperties.IsCompilerGenerated ]
        | PropertyGet -> [ CodeSchemaProperties.IsPropertyGet ]
        | PropertySet -> [ CodeSchemaProperties.IsPropertySet ]
        | Lifted -> [ CodeSchemaProperties.IsAnonymous; FSharpGraphSchema.IsLifted ]
        | Module -> [ FSharpGraphSchema.IsModule ]
        | Union -> [ FSharpGraphSchema.IsUnion ]
        | Record -> [ FSharpGraphSchema.IsRecord ]
        | Exception -> [ FSharpGraphSchema.IsException ]
        | Measure -> [ FSharpGraphSchema.IsMeasure ]
        | ActivePattern -> [ FSharpGraphSchema.IsActivePattern ]
        | UnionCaseTester -> [ FSharpGraphSchema.IsUnionCaseTester ]
        | Function -> [ FSharpGraphSchema.IsFunction ]
        | Inline -> [ FSharpGraphSchema.IsInline ]
        | Mutable -> [ FSharpGraphSchema.IsMutable ]

    /// `CallStackEntry` belongs to the Architecture Explorer, not to a schema this provider can
    /// reference, so it is looked up by id in whichever schema the map document already carries.
    let callStackEntryCategory (graph: Graph) =
        graph.AllSchemas
        |> Seq.tryPickV (fun schema ->
            match schema.FindCategory "CallStackEntry" with
            | null -> ValueNone
            | category -> ValueSome category)

    /// Mirrors what the built-in C#/VB resolver does for a frame it recognises: create the method
    /// node, give it a source location, and link the frame to it - all inside a graph transaction.
    let attachResolvedMethod (graph: Graph) (frameNode: GraphNode) keepsDeclaration (resolved: ResolvedFrame) =
        // The compiled name identifies the node; the label is the source name, and for a
        // constructor its type.
        let label =
            match Array.tryLast resolved.TypeChain with
            | Some(struct (typeName, _)) when resolved.Traits.Contains Constructor -> typeName
            | None when resolved.Traits.Contains Constructor -> ".ctor"
            | _ -> resolved.DisplayName

        use transaction = new GraphTransactionScope()

        // The pipeline follows a frame's link only to a node that carries `Method`, and its next
        // step declares every frame without one unresolved. What the member really is rides along
        // as a second category, which is how a C# frame renders a property getter too.
        let methodNode =
            graph.Nodes.GetOrCreate(resolvedNodeId resolved, label, CodeNodeCategories.Method)

        match resolved.Kind with
        | ResolvedMethod -> ()
        | ResolvedProperty -> methodNode.AddCategory CodeNodeCategories.Property |> ignore
        | ResolvedEvent -> methodNode.AddCategory CodeNodeCategories.Event |> ignore

        // The declaration. `AddSourceLocations` overwrites it with the line that was executing,
        // once per node and guarded by the `SourceLocation` category, as it does for C#. For a
        // startup frame that line belongs to a method the compiler wrote for the whole file, so the
        // category is added here and the platform leaves the declaration alone.
        let location = sourceLocationOf resolved
        methodNode.SetValue(CodeNodeProperties.SourceLocation, location) |> ignore

        if keepsDeclaration then
            methodNode.AddCategory CodeNodeCategories.SourceLocation |> ignore

        // `SourceLocation` alone lands navigation on the start of the line; the identifier location
        // is the one carrying the column.
        methodNode.SetValue(CodeNodeProperties.IdentifierSourceLocation, location)
        |> ignore

        for frameTrait in resolved.Traits do
            for property in propertiesOf frameTrait do
                methodNode.SetValue(property, true) |> ignore

        // The built-in resolver dispatches on the language of a node.
        methodNode.SetValue(CodeSchemaProperties.Language, FSharpConstants.FSharpLanguageName)
        |> ignore

        // A plain `CodeSchema_Method` follows outgoing `Calls` links on double-click, and a
        // call-stack map has none. Navigation comes from `CallStackEntry`, which the platform adds
        // to the frame only after this handler, so it cannot be copied off it.
        callStackEntryCategory graph
        |> ValueOption.iter (fun category -> methodNode.AddCategory category |> ignore)

        CodeSchemaHelper.GetOrCreateLink<CallStackMethodReferencesMethodLink>(graph, frameNode.Id, methodNode.Id, String.Empty)
        |> ignore

        transaction.Complete()

    let resolveCallStack (context: ActionContext) =
        cancellableTask {
            let graph = context.Graph

            // `UnhandledInputNodes` already excludes nodes another provider claimed.
            let parsedFrames =
                [|
                    for frameNode in context.UnhandledInputNodes() do
                        let frame = CallStackMethodNode frameNode

                        match FSharpStackFrameNameParser.parse frame.FunctionName with
                        | ValueNone ->
                            if isFSharpCoreMethod frameNode then
                                yield frameNode, ValueNone, FoldAsExternalCode
                        | ValueSome parsed ->
                            let facts =
                                {
                                    Frame =
                                        { parsed with
                                            SourcePosition = frameSourcePosition frameNode
                                        }
                                    ModuleAssembly = moduleAssemblyOf frame
                                }

                            yield frameNode, ValueSome facts, FSharpCallStackPlan.decide assemblyNameForFile facts
                |]

            // A position belongs to the PDB of the running build, while the resolver reads the file
            // the workspace has open. This trace is the only place a disagreement between them shows.
            for methodNode, facts, action in parsedFrames do
                let position =
                    match facts |> ValueOption.bind _.Frame.SourcePosition with
                    | ValueNone -> "no position"
                    | ValueSome position -> $"{Path.GetFileName position.File}:{position.Line}"

                Trace.WriteLine $"[FSharpCodeMap] {CallStackMethodNode(methodNode).FunctionName} at {position} -> {action}"

            // What folds a frame away is reaching no method node: `BuildResultLinks` joins the ends of
            // a run of such frames by the one indirect link it labels "External Code". So folding is
            // cutting the frame loose from its method node, and nothing here creates or re-points a link.
            //
            // The whole graph is walked, not only the nodes handed to this action: the pipeline hands
            // a provider only the method nodes new to the debug session, while it rebuilds the frame
            // links on every stop.
            let foldedAway =
                seq {
                    yield!
                        graph.Nodes.GetByCategory ProgressionNodeCategories.CallStackMethod
                        |> Seq.filter isFSharpCoreMethod

                    for methodNode, _, action in parsedFrames do
                        if action.IsFoldAsExternalCode then
                            methodNode
                }
                |> Seq.distinct
                |> Seq.collect frameLinksOf
                |> Seq.toArray

            if foldedAway.Length > 0 then
                use folding = new GraphTransactionScope()

                for link in foldedAway do
                    graph.Links.Remove link |> ignore

                folding.Complete()

            let byAssembly =
                parsedFrames
                |> Array.chooseV (fun (frameNode, facts, action) ->
                    match action, facts with
                    | ResolveIn assembly, ValueSome facts -> ValueSome struct (assembly, frameNode, facts.Frame)
                    | _ -> ValueNone)
                |> Array.groupBy (fun struct (assembly, _, _) -> assembly)

            let! resolutions =
                byAssembly
                |> Array.map (fun (assembly, group) -> struct (assembly, group |> Array.map (fun struct (_, _, frame) -> frame)))
                |> resolveAll context

            let mutable resolvedCount = 0

            for (_, group), results in Seq.zip byAssembly resolutions do
                for struct (_, frameNode, frame), resolved in Seq.zip group results do
                    match resolved with
                    | ValueNone -> ()
                    | ValueSome resolved ->
                        try
                            attachResolvedMethod graph frameNode frame.Member.IsFrameStartupCode resolved
                            context.AddHandled frameNode
                            resolvedCount <- resolvedCount + 1
                        with e ->
                            // One bad frame must not abandon the rest of the stack: it stays a grey
                            // unresolved node.
                            Trace.WriteLine $"[FSharpCodeMap] frame failed: {e.Message}"

            Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack: {resolvedCount}/{parsedFrames.Length} F# frames resolved"
        }

    interface IProvider with
        /// A provider contributes graph content, not a document schema; the built-in Roslyn one
        /// returns nothing here too, and handing back a graph that owns a schema corrupts the map.
        member _.Schema = null

        member _.Initialize(_serviceProvider: IServiceProvider) =
            Trace.WriteLine "[FSharpCodeMap] FSharpGraphProvider.Initialize"

            // `ActionManager` runs the handler on a `JobQueue` worker and continues into the next
            // pipeline step the moment it returns, so it blocks until the graph is complete - the
            // same bridge the built-in C#/VB provider uses. A stack it cannot resolve has to degrade
            // to unresolved nodes, the state it started in, rather than take the map down.
            Actions.ResolveCallStack.ActionHandlers.Add(
                ActionHandler(fun context ->
                    try
                        ThreadHelper.JoinableTaskFactory.Run(fun () -> resolveCallStack context CancellationToken.None :> Task)
                    with e ->
                        Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack failed: {e}")
            )
