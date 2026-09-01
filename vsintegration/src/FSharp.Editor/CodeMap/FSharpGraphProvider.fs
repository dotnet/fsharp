// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Threading

open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell

open Microsoft.VisualStudio.GraphModel
open Microsoft.VisualStudio.GraphModel.CodeSchema
open Microsoft.VisualStudio.GraphModel.Schemas
open Microsoft.VisualStudio.Progression
open Microsoft.VisualStudio.Progression.CodeSchema.Api

/// Resolves the F# frames of a debugger call stack shown on a Code Map. The debugger contributes
/// `CodeSchema_CallStackMethod` nodes carrying nothing but the module and the mangled frame name;
/// without a provider claiming them they stay unresolved and carry no source location.
[<LegacyProvider(typeof<IProvider>,
                 Name = "FSharpProvider",
                 Priority = 2.0,
                 IntellisenseType = "{BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B}",
                 ProjectCapability = "FSharp")>]
type internal FSharpGraphProvider() =

    let callStackMethodCategory = "CodeSchema_CallStackMethod"

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

    let resolvedNodeId (resolved: ResolvedFrame) =
        let assembly =
            match resolved.Project.OutputFilePath with
            | null -> GraphNodeId.Empty
            | path -> GraphNodeId.GetPartial(CodeGraphNodeIdName.Assembly, Uri(path))

        let ``namespace``, typeName =
            match resolved.EntityPath with
            | [] -> "", ""
            | path -> String.Join(".", path |> List.truncate (path.Length - 1)), List.last path

        [
            if assembly <> GraphNodeId.Empty then
                assembly
            if not (String.IsNullOrEmpty ``namespace``) then
                GraphNodeId.GetPartial(CodeGraphNodeIdName.Namespace, ``namespace``)
            if not (String.IsNullOrEmpty typeName) then
                GraphNodeId.GetPartial(CodeGraphNodeIdName.Type, typeName)
            match resolved.MemberName with
            | ValueSome name -> GraphNodeId.GetPartial(CodeGraphNodeIdName.Member, name)
            | ValueNone -> ()
        ]
        |> Array.ofList
        |> GraphNodeId.GetNested

    let sourceLocationOf (resolved: ResolvedFrame) =
        let range = resolved.DeclarationRange

        SourceLocation(Uri(range.FileName), Position(range.StartLine - 1, range.StartColumn), Position(range.EndLine - 1, range.EndColumn))

    let tryResolve (frameName: string) (assemblyName: string) =
        use cancellation = new CancellationTokenSource(resolutionTimeout)

        try
            FSharpCallStackResolver.tryResolveFrameName workspace.Value assemblyName frameName cancellation.Token
            |> Async.AwaitTask
            |> Async.RunSynchronously
        with _ ->
            ValueNone

    /// The synthesized parts of a frame name (`helperTwo@42.Invoke`) make ugly node labels even
    /// when full resolution fails; the parsed shape always yields a better one.
    let friendlyLabel (frame: ParsedFrame) =
        match frame.Member with
        | FrameClosureBody origin -> ValueSome origin.EnclosingName
        | FramePropertyGetter name
        | FramePropertySetter name -> ValueSome name
        | FrameStartupCode ->
            match frame.Path with
            | [] -> ValueNone
            | path -> ValueSome (List.last path).Name
        | FrameMethod _
        | FrameConstructor
        | FrameStaticConstructor
        | FrameActivePattern _ -> ValueNone

    let resolveNode (graph: Graph) (frameNode: GraphNode) =
        match frameOf frameNode with
        | ValueNone -> false
        | ValueSome(assemblyName, functionName) ->
            match FSharpStackFrameNameParser.parse functionName with
            | ValueNone -> false
            | ValueSome parsed ->
                friendlyLabel parsed
                |> ValueOption.iter (fun label -> frameNode.Label <- label)

                match tryResolve functionName assemblyName with
                | ValueNone -> false
                | ValueSome resolved ->
                    let label =
                        match resolved.MemberName with
                        | ValueSome name -> name
                        | ValueNone -> String.Join(".", resolved.EntityPath)

                    let methodNode =
                        graph.Nodes.GetOrCreate(resolvedNodeId resolved, label, CodeNodeCategories.Method)

                    methodNode.SetValue(CodeNodeProperties.SourceLocation, sourceLocationOf resolved)
                    |> ignore

                    CallStackMethodNode(frameNode).ReferencesMethodNode <- MethodNode methodNode
                    true

    let resolveCallStack (context: ActionContext) =
        let graph = context.Graph
        let mutable seen = 0
        let mutable resolvedCount = 0

        for frameNode in graph.Nodes.GetByCategory [| callStackMethodCategory |] |> Seq.toArray do
            try
                seen <- seen + 1

                if resolveNode graph frameNode then
                    resolvedCount <- resolvedCount + 1
                    context.AddHandled frameNode
            with e ->
                // An unresolvable frame is expected - it stays a grey unresolved node, and one bad
                // frame must not abandon the rest of the stack.
                Trace.WriteLine $"[FSharpCodeMap] frame failed: {e.Message}"

        Trace.WriteLine $"[FSharpCodeMap] ResolveCallStack: {resolvedCount}/{seen} F# frames resolved"

    interface IProvider with
        member _.Schema = schema

        member _.Initialize(_serviceProvider: IServiceProvider) =
            Trace.WriteLine "[FSharpCodeMap] FSharpGraphProvider.Initialize"
            Actions.ResolveCallStack.ActionHandlers.Add(ActionHandler(resolveCallStack))
