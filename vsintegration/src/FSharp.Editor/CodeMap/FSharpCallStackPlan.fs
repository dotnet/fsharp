// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

/// What a frame node says about itself once the graph has been read. Nothing here refers to the
/// graph, so what to do with a frame is decided, and tested, without Visual Studio.
[<Struct; NoEquality; NoComparison>]
type internal FrameFacts =
    {
        Frame: ParsedFrame
        /// The assembly named by the frame's module. Empty on the frames the debugger leaves without
        /// one - accessors and module initialization among them.
        ModuleAssembly: string voption
    }

type internal FrameAction =
    /// Nothing names the assembly the frame belongs to, so it is left as the platform made it.
    | LeaveUnresolved
    /// FSharp.Core's own async, task and seq plumbing. Its assembly is no project of the workspace,
    /// and the pipeline folds a frame into External Code once it reaches no method node.
    | FoldAsExternalCode
    | ResolveIn of assembly: string

[<RequireQualifiedAccess>]
module internal FSharpCallStackPlan =

    [<Literal>]
    let private FSharpCore = "FSharp.Core"

    /// A map accumulates nodes and links across runs, so one method node ends up claimed by frames
    /// of several runs, each reporting the line its own run stopped on. A position is trusted only
    /// when every frame that claims the node agrees on it.
    let positionOf (claimed: SourcePosition seq) =
        match claimed |> Seq.distinct |> Seq.truncate 2 |> List.ofSeq with
        | [ agreed ] -> ValueSome agreed
        | _ -> ValueNone

    /// FSharp.Core ships with SourceLink, so the debugger has source for its frames and does not fold
    /// them into External Code the way it folds the BCL. The module alone settles it, which lets a
    /// frame be folded even when its name does not parse.
    let isFSharpCoreModule (moduleAssembly: string voption) =
        moduleAssembly
        |> ValueOption.exists (fun assembly -> String.Equals(assembly, FSharpCore, StringComparison.OrdinalIgnoreCase))

    /// The debugger leaves some frames without a module, and SourceLink puts FSharp.Core's own source
    /// path on those instead.
    let private isFSharpCore (facts: FrameFacts) =
        match facts.ModuleAssembly, facts.Frame.SourcePosition with
        | ValueSome _, _ -> isFSharpCoreModule facts.ModuleAssembly
        | ValueNone, ValueSome position -> position.File.IndexOf(FSharpCore, StringComparison.OrdinalIgnoreCase) >= 0
        | ValueNone, ValueNone -> false

    /// `assemblyForFile` answers which project's assembly a source file belongs to: the only way to
    /// address a frame whose module the debugger left empty.
    let decide (assemblyForFile: string -> string voption) (facts: FrameFacts) =
        if isFSharpCore facts then
            FoldAsExternalCode
        else
            facts.ModuleAssembly
            |> ValueOption.orElseWith (fun () ->
                facts.Frame.SourcePosition
                |> ValueOption.bind (fun position -> assemblyForFile position.File))
            |> ValueOption.map ResolveIn
            |> ValueOption.defaultValue LeaveUnresolved
