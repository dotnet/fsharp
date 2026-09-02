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

/// The code schema was shaped for C# and VB, so it has nowhere to record that a frame came from a
/// module, a union, an active pattern or an inline function. A property carries its own schema, so
/// declaring them here is enough for them to reach the Properties pane and DGML styles - the schema
/// itself is never handed to a graph, the way Roslyn's `RoslynGraphProperties` did it.
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

    /// The frame came from a compiler-generated closure class, so the node names the binding the
    /// lambda or local function was lifted out of rather than anything the user can point at.
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

    /// The assembly the debugger named as the frame's module, which it leaves empty on accessors and
    /// on module initialization. The Uri is relative (`ClassLibrary.dll`) for workspace assemblies
    /// and absolute for external ones.
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

    let isFrameNode (node: GraphNode) =
        node.HasCategory ProgressionNodeCategories.CallStackFrame

    /// The links by which frames claim this method node - one per frame, and in an accumulated map
    /// that is more than one.
    let frameLinksOf (methodNode: GraphNode) =
        methodNode.IncomingLinks
        |> Seq.filter (fun link -> link.HasCategory ProgressionLinkCategories.References && isFrameNode link.Source)

    let framesOf (methodNode: GraphNode) =
        frameLinksOf methodNode |> Seq.map _.Source

    /// The source position the debugger resolved for a frame - the only usable anchor for a startup
    /// frame, whose name (`$Demo.$Demo`) points at the file rather than the construct.
    ///
    /// It cannot be read off the method node this action hands us: the pipeline stamps
    /// `CodeNodeProperties.SourceLocation` on those in its `AddSourceLocations` step, which runs
    /// after this handler. The frame nodes carry it already, and their `StartLine` is 1-based, the
    /// same base an FCS range uses.
    let frameSourcePosition (methodNode: GraphNode) =
        framesOf methodNode
        |> Seq.choose (fun node ->
            let frame = CallStackFrameNode node

            match frame.FileName with
            | null
            | "" -> None
            | file -> Some { File = file; Line = frame.StartLine })
        |> FSharpCallStackPlan.positionOf

    /// A startup frame carries no module, so its owning assembly is read from the project the source
    /// file belongs to instead.
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

    /// The modifiers the Properties pane shows and the map's styles can filter on. Only the flags
    /// that hold are written, so a node carries no misleading `false`s.
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

    let describeTraits (node: GraphNode) traits =
        for frameTrait in traits do
            for property in propertiesOf frameTrait do
                node.SetValue(property, true) |> ignore

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
        // The compiled name identifies the node, but the source name is what reads well: an operator
        // is `(>=>)`, not `op_GreaterEqualsGreater`, and a constructor is its type, not `.ctor`.
        let typeName () =
            match resolved.TypeChain with
            | [||] -> ValueNone
            | chain ->
                let struct (name, _) = Array.last chain
                ValueSome name

        let label =
            if resolved.Traits.Contains Constructor then
                typeName () |> ValueOption.defaultValue ".ctor"
            else
                resolved.DisplayName

        use transaction = new GraphTransactionScope()

        // A frame is always a method invocation, and the pipeline enforces that: `ReferencesMethodNode`
        // only follows a link whose target carries `Method`, and the next step declares every frame
        // without one unresolved - replacing the node and the label with `get`, `set` or `$Demo`.
        // What the member really is rides along as a category beside `Method`, plus the accessor
        // traits `describeTraits` writes, which is how C# frames render a property getter too.
        let methodNode =
            graph.Nodes.GetOrCreate(resolvedNodeId resolved, label, CodeNodeCategories.Method)

        match resolved.Kind with
        | ResolvedMethod -> ()
        | ResolvedProperty -> methodNode.AddCategory CodeNodeCategories.Property |> ignore
        | ResolvedEvent -> methodNode.AddCategory CodeNodeCategories.Event |> ignore

        // The declaration. Where the frame names a file, `AddSourceLocations` overwrites this with
        // the line that was executing - a call stack navigates to the call, not to the declaration -
        // and it writes once per node for the life of the map, guarded by the category it adds. The
        // C# resolver sets the declaration here and is overwritten the same way.
        //
        // A startup frame is the exception: the line the debugger reports for it is the source
        // range of a method the compiler wrote for the whole file, which lands on the blank line
        // above the type whose initializer is running. The declaration is the only place it can
        // navigate to, so the category is added here and the platform leaves it alone.
        let location = sourceLocationOf resolved
        methodNode.SetValue(CodeNodeProperties.SourceLocation, location) |> ignore

        if keepsDeclaration then
            methodNode.AddCategory CodeNodeCategories.SourceLocation |> ignore

        // `SourceLocation` alone lands navigation on the start of the line; the identifier location
        // is the one carrying the column, so the caret reaches the declaration itself.
        methodNode.SetValue(CodeNodeProperties.IdentifierSourceLocation, location)
        |> ignore

        describeTraits methodNode resolved.Traits

        // The built-in resolver dispatches on this, so a node that does not carry it reads as
        // language-less to anything walking the map later.
        methodNode.SetValue(CodeSchemaProperties.Language, FSharpConstants.FSharpLanguageName)
        |> ignore

        // Double-clicking a plain `CodeSchema_Method` runs that category's default action - follow
        // outgoing `Calls` links - and a call-stack map has none, so the node looks dead. Navigation
        // comes from `CallStackEntry`, which is what the resolved C# and VB nodes carry. It cannot be
        // copied off the frame: the platform adds it further down the pipeline, after this handler.
        callStackEntryCategory graph
        |> ValueOption.iter (fun category -> methodNode.AddCategory category |> ignore)

        CodeSchemaHelper.GetOrCreateLink<CallStackMethodReferencesMethodLink>(graph, frameNode.Id, methodNode.Id, String.Empty)
        |> ignore

        transaction.Complete()

    let resolveCallStack (context: ActionContext) =
        cancellableTask {
            let graph = context.Graph

            // Every input frame we can parse, with the source position the debugger resolved for it -
            // the only anchor a module-less startup frame has. `UnhandledInputNodes` already excludes
            // nodes another provider claimed.
            let parsedFrames =
                [|
                    for frameNode in context.UnhandledInputNodes() do
                        let frame = CallStackMethodNode frameNode

                        match FSharpStackFrameNameParser.parse frame.FunctionName with
                        | ValueNone ->
                            // A name we cannot take apart is still a name we can place, and
                            // FSharp.Core's belongs off the map whether or not it reads.
                            if FSharpCallStackPlan.isFSharpCoreModule (moduleAssemblyOf frame) then
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

            // What the debug engine handed us, before anything is resolved. A position here belongs to
            // the PDB of the running build, while the resolver reads the file the workspace has open:
            // when the two disagree the answer is a construct on the same line of a different version
            // of the source, and this line is the only place that shows it.
            for methodNode, facts, action in parsedFrames do
                let position =
                    match facts |> ValueOption.bind _.Frame.SourcePosition with
                    | ValueNone -> "no position"
                    | ValueSome position -> $"{Path.GetFileName position.File}:{position.Line}"

                Trace.WriteLine $"[FSharpCodeMap] {CallStackMethodNode(methodNode).FunctionName} at {position} -> {action}"

            // FSharp.Core's plumbing is external code that Just My Code did not filter out: it ships
            // with SourceLink, so the debugger counts it as ours and the pipeline's first step keeps
            // its frames.
            //
            // What folds a frame away is reaching no method node from it. `BuildResultLinks` walks
            // the chain of `Calls` links and draws one link per frame that reaches one, joining the
            // ends of a run of frames that do not by a single indirect link it labels "External
            // Code" - the same link it draws over everything else the debugger folded. So the whole
            // operation is cutting the frame loose from its method node: the chain of `Calls` links
            // keeps the exact shape the pipeline gave it, nothing here creates or re-points a link,
            // and the hop over FSharp.Core is drawn by the code that draws every other hop. What is
            // left behind - the method node and the grey one the pipeline will pair with it - is in
            // no `CallStack*Call` link and so is not in the result graph.
            //
            // The whole graph is walked for this, not only the nodes handed to this action: the
            // pipeline remembers every method node it has seen for the rest of the debug session and
            // hands a provider only the new ones, while the frame links are rebuilt on every stop.
            // Folding only what was handed over folds each FSharp.Core frame once per session and
            // leaves it standing every time after.
            let foldedAway =
                let byModule =
                    graph.Nodes.GetByCategory ProgressionNodeCategories.CallStackMethod
                    |> Seq.filter (fun methodNode ->
                        FSharpCallStackPlan.isFSharpCoreModule (moduleAssemblyOf (CallStackMethodNode methodNode)))

                let byDecision =
                    parsedFrames
                    |> Seq.choose (fun (methodNode, _, action) ->
                        if action = FoldAsExternalCode then
                            Some methodNode
                        else
                            None)

                Seq.append byModule byDecision
                |> Seq.distinct
                |> Seq.collect frameLinksOf
                |> Seq.toArray

            if foldedAway.Length > 0 then
                use folding = new GraphTransactionScope()

                for link in foldedAway do
                    graph.Links.Remove link |> ignore

                folding.Complete()

            let byAssembly =
                [|
                    for frameNode, facts, action in parsedFrames do
                        match action, facts with
                        | ResolveIn assembly, ValueSome facts -> yield assembly, frameNode, facts.Frame
                        | ResolveIn _, ValueNone
                        | FoldAsExternalCode, _
                        | LeaveUnresolved, _ -> ()
                |]
                |> Seq.groupBy (fun (assembly, _, _) -> assembly)
                |> Seq.map (fun (assembly, group) -> assembly, group |> Seq.map (fun (_, frameNode, frame) -> frameNode, frame))
                |> Seq.toArray

            let! resolutions =
                byAssembly
                |> Array.map (fun (assemblyName, group) -> struct (assemblyName, group |> Seq.map snd |> Seq.toArray))
                |> resolveAll context

            let mutable resolvedCount = 0

            for (_, group), results in Seq.zip byAssembly resolutions do
                for (frameNode, frame), resolved in Seq.zip group results do
                    match resolved with
                    | ValueNone -> ()
                    | ValueSome resolved ->
                        let keepsDeclaration =
                            match frame.Member with
                            | FrameStartupCode -> true
                            | _ -> false

                        try
                            attachResolvedMethod graph frameNode keepsDeclaration resolved
                            context.AddHandled frameNode
                            resolvedCount <- resolvedCount + 1
                        with e ->
                            // An unresolvable frame is expected - it stays a grey unresolved node, and
                            // one bad frame must not abandon the rest of the stack.
                            Trace.WriteLine $"[FSharpCodeMap] frame failed: {e.Message}"

            Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack: {resolvedCount}/{parsedFrames.Length} F# frames resolved"
        }

    interface IProvider with
        /// A provider contributes graph content, not a document schema; the built-in Roslyn one
        /// returns nothing here too, and handing back a graph that owns a schema corrupts the map.
        member _.Schema = null

        member _.Initialize(_serviceProvider: IServiceProvider) =
            Trace.WriteLine "[FSharpCodeMap] FSharpGraphProvider.Initialize"

            // The handler contract is synchronous: `ActionManager` runs it on a `JobQueue` worker
            // and continues into the next pipeline step the moment it returns, so the graph must be
            // complete by then. `JoinableTaskFactory.Run` is the same blocking bridge the built-in
            // C#/VB provider uses for this action.
            //
            // Nothing this provider does may take the map down with it: a stack it cannot resolve
            // has to degrade to unresolved nodes, which is exactly the state it started in.
            Actions.ResolveCallStack.ActionHandlers.Add(
                ActionHandler(fun context ->
                    try
                        ThreadHelper.JoinableTaskFactory.Run(fun () -> resolveCallStack context CancellationToken.None :> Task)
                    with e ->
                        Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack failed: {e}")
            )
