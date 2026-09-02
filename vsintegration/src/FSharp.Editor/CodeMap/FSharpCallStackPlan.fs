// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

/// What a frame node says about itself once the graph has been read: the name taken apart, the
/// assembly the debugger named as its module, and the position it reported for it. Nothing here
/// refers to the graph, so deciding what to do with a frame needs no Visual Studio at all.
[<Struct; NoEquality; NoComparison>]
type internal FrameFacts =
    {
        Frame: ParsedFrame
        /// The assembly named by the frame's module. Empty on the frames the debugger leaves without
        /// one - accessors and module initialization among them.
        ModuleAssembly: string voption
    }

/// What the provider does with a frame.
type internal FrameAction =
    /// Nothing names the assembly the frame belongs to, so it is left as the platform made it.
    | LeaveUnresolved
    /// FSharp.Core's own async, task and seq plumbing. It cannot be resolved - the assembly is no
    /// project of the workspace - and the pipeline folds it into External Code once told it is
    /// external, which is what keeps `MoveNext` and `Invoke` off the map.
    | FoldAsExternalCode
    | ResolveIn of assembly: string

[<RequireQualifiedAccess>]
module internal FSharpCallStackPlan =

    [<Literal>]
    let private FSharpCore = "FSharp.Core"

    /// The pipeline removes neither nodes nor links, so a map accumulates both across runs and one
    /// method node ends up claimed by frames from several of them, each reporting the line *its* run
    /// stopped on. A position is only worth trusting when every frame that claims the node agrees:
    /// picking one of them gave a static initializer the name of whichever type happened to sit above
    /// a stale line. An unresolved frame is honest; a confidently wrong one is not.
    let positionOf (claimed: SourcePosition seq) =
        match claimed |> Seq.distinct |> Seq.truncate 2 |> List.ofSeq with
        | [ agreed ] -> ValueSome agreed
        | _ -> ValueNone

    /// FSharp.Core ships with SourceLink, so the debugger has source for its frames and does not fold
    /// them into External Code the way it folds the BCL. They are recognised by the module the frame
    /// names, or - when it names none - by the file the debugger placed it in.
    let private isFSharpCore (facts: FrameFacts) =
        match facts.ModuleAssembly with
        | ValueSome assembly -> String.Equals(assembly, FSharpCore, StringComparison.OrdinalIgnoreCase)
        | ValueNone ->
            match facts.Frame.SourcePosition with
            | ValueSome position -> position.File.IndexOf(FSharpCore, StringComparison.OrdinalIgnoreCase) >= 0
            | ValueNone -> false

    /// `assemblyForFile` answers which project's assembly a source file belongs to, which is the only
    /// thing this decision needs from the workspace - and the only way to address a frame whose module
    /// the debugger left empty.
    let decide (assemblyForFile: string -> string voption) (facts: FrameFacts) =
        if isFSharpCore facts then
            FoldAsExternalCode
        else
            let assembly =
                match facts.ModuleAssembly with
                | ValueSome assembly -> ValueSome assembly
                | ValueNone ->
                    facts.Frame.SourcePosition
                    |> ValueOption.bind (fun position -> assemblyForFile position.File)

            match assembly with
            | ValueSome assembly -> ResolveIn assembly
            | ValueNone -> LeaveUnresolved
