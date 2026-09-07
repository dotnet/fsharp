// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// What the provider decides to do with a frame, tested without a graph: the decision is a function
/// of the frame alone, and the workspace reaches it only as "which assembly owns this file".
module FSharp.Editor.Tests.CodeMap.FSharpCallStackPlanTests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor

[<Literal>]
let private ProjectFile = @"C:\Demo\ClassLibrary\Demo.fs"

/// The one thing the decision asks of the workspace.
let private assemblyForFile file =
    if file = ProjectFile then
        ValueSome "ClassLibrary"
    else
        ValueNone

let private frameNamed name =
    match FSharpStackFrameNameParser.parse name with
    | ValueNone -> failwith $"frame name did not parse: %s{name}"
    | ValueSome frame -> frame

let private facts name moduleAssembly position =
    {
        Frame =
            { frameNamed name with
                SourcePosition = position
            }
        ModuleAssembly = moduleAssembly
    }

let private decide facts =
    FSharpCallStackPlan.decide assemblyForFile facts

[<Fact>]
let ``A frame naming its module resolves in that assembly`` () =
    facts "ClassLibrary.Demo.sink" (ValueSome "ClassLibrary") ValueNone
    |> decide
    |> fun action -> Assert.Equal(ResolveIn "ClassLibrary", action)

/// The debugger leaves the module empty on accessors and on module initialization, which is what
/// once kept them off the map entirely.
[<Fact>]
let ``A frame without a module is addressed by the file it runs in`` () =
    facts "ClassLibrary.Worker.Computed.get" ValueNone (ValueSome { File = ProjectFile; Line = 144 })
    |> decide
    |> fun action -> Assert.Equal(ResolveIn "ClassLibrary", action)

[<Fact>]
let ``A frame with neither a module nor a known file is left alone`` () =
    facts
        "Some.Other.Thing"
        ValueNone
        (ValueSome
            {
                File = @"C:\Elsewhere\Other.fs"
                Line = 1
            })
    |> decide
    |> fun action -> Assert.Equal(LeaveUnresolved, action)

[<Fact>]
let ``A frame with no position and no module is left alone`` () =
    facts "Some.Other.Thing" ValueNone ValueNone
    |> decide
    |> fun action -> Assert.Equal(LeaveUnresolved, action)

/// FSharp.Core's plumbing is folded into External Code rather than resolved - the assembly is no
/// project of the workspace, and a node of its own would be a bare `MoveNext` or `Invoke`.
[<Theory>]
[<InlineData("<StartupCode$FSharp-Core>.$Async.Sleep@1814-3.Invoke")>]
[<InlineData("<StartupCode$FSharp-Core>.$Tasks.resumptionInfo@159<int>.MoveNext")>]
[<InlineData("Microsoft.FSharp.Collections.ListModule.Map<int, int>")>]
let ``FSharp.Core frames fold into External Code`` (name: string) =
    facts name (ValueSome "FSharp.Core") ValueNone
    |> decide
    |> fun action -> Assert.Equal(FoldAsExternalCode, action)

/// SourceLink puts FSharp.Core's own source path on frames the debugger reports without a module.
[<Fact>]
let ``An FSharp.Core frame is recognised by its source path when it names no module`` () =
    let position =
        ValueSome
            {
                File = "/_/src/FSharp.Core/async.fs"
                Line = 1818
            }

    facts "Microsoft.FSharp.Control.AsyncPrimitives.CallThenInvoke" ValueNone position
    |> decide
    |> fun action -> Assert.Equal(FoldAsExternalCode, action)

/// Case matters nowhere in an assembly name.
[<Fact>]
let ``FSharp.Core is recognised whatever its casing`` () =
    facts "Whatever.Thing" (ValueSome "fsharp.core") ValueNone
    |> decide
    |> fun action -> Assert.Equal(FoldAsExternalCode, action)

/// The pipeline removes neither nodes nor links, so a map accumulates them and one method node ends
/// up claimed by frames from several runs. Guessing between them once gave a static initializer the
/// name of whichever type sat above a stale line.
[<Fact>]
let ``A position claimed by one frame is used`` () =
    let position = { File = ProjectFile; Line = 168 }
    Assert.Equal(ValueSome position, FSharpCallStackPlan.positionOf [ position ])

[<Fact>]
let ``Frames agreeing on a position are used`` () =
    let position = { File = ProjectFile; Line = 168 }

    Assert.Equal(ValueSome position, FSharpCallStackPlan.positionOf [ position; position ])

[<Fact>]
let ``Frames disagreeing about the line yield no position`` () =
    FSharpCallStackPlan.positionOf [ { File = ProjectFile; Line = 168 }; { File = ProjectFile; Line = 164 } ]
    |> fun position -> Assert.True position.IsNone

[<Fact>]
let ``A frame nothing claims yields no position`` () =
    Assert.True (FSharpCallStackPlan.positionOf []).IsNone

/// The FSharp.Core frames a scenario actually put on the stack, taken from the runtime rather than
/// written down here: what the plumbing of a pipeline or a computation expression is made of is
/// FSharp.Core's business and changes with it.
let private plumbingOf (scenario: unit -> int) =
    scenario () |> ignore
    CallStackSample.plumbingFrames ()

/// A frame the parser rejects is a frame the provider never builds facts for, so nothing folds it
/// and it stands on the map as a bare `Map`, `Invoke` or `MoveNext`. Both halves are asserted: the
/// name reads, and the decision is to fold.
let private assertEveryFrameFolds (frames: string array) =
    Assert.NotEmpty frames

    for name in frames do
        match FSharpStackFrameNameParser.parse name with
        | ValueNone -> failwith $"the parser rejects this FSharp.Core frame, so nothing folds it: %s{name}"
        | ValueSome frame ->
            {
                Frame = frame
                ModuleAssembly = ValueSome "FSharp.Core"
            }
            |> decide
            |> fun action -> Assert.Equal(FoldAsExternalCode, action)

/// One `List.map` is two frames - `ListModule.Map` and the `Primitives.Basics.List.map` it calls -
/// and both reached the map as their own nodes, `Map` above `map`. How many of them survive depends
/// on how FSharp.Core was optimized, so both are named here rather than counted; the debugger spells
/// them with the type arguments it resolved, the runtime without.
[<Theory>]
[<InlineData("Microsoft.FSharp.Collections.ListModule.Map")>]
[<InlineData("Microsoft.FSharp.Collections.ListModule.Map<int, int>")>]
[<InlineData("Microsoft.FSharp.Primitives.Basics.List.map")>]
[<InlineData("Microsoft.FSharp.Primitives.Basics.List.map<int, int>")>]
let ``Both halves of a List.map fold into External Code`` (name: string) =
    facts name (ValueSome "FSharp.Core") ValueNone
    |> decide
    |> fun action -> Assert.Equal(FoldAsExternalCode, action)

[<Fact>]
let ``Every FSharp.Core frame of a pipeline folds into External Code`` () =
    assertEveryFrameFolds (plumbingOf CallStackSample.pipelineLambdas)

/// The other shapes FSharp.Core reaches a stack in: a trampolined continuation and a closure the
/// compiler numbered from inside `<StartupCode$FSharp-Core>`; a generated state machine reached
/// through an explicitly implemented `IEnumerator.MoveNext`; a subscription reached through an
/// explicitly implemented `IObserver.OnNext`.
[<Fact>]
let ``Every FSharp.Core frame of an async body folds into External Code`` () =
    assertEveryFrameFolds (plumbingOf CallStackSample.asyncBody)

[<Fact>]
let ``Every FSharp.Core frame of a task body folds into External Code`` () =
    assertEveryFrameFolds (plumbingOf CallStackSample.taskBody)

[<Fact>]
let ``Every FSharp.Core frame of a seq body folds into External Code`` () =
    assertEveryFrameFolds (plumbingOf CallStackSample.seqBody)

[<Fact>]
let ``Every FSharp.Core frame of an event handler folds into External Code`` () =
    assertEveryFrameFolds (plumbingOf CallStackSample.eventHandler)

/// The module settles it on its own, so a name too mangled to read is still folded rather than left
/// standing. This is what the provider falls back to when the parser returns nothing.
[<Theory>]
[<InlineData("FSharp.Core")>]
[<InlineData("fsharp.core")>]
let ``An FSharp.Core module folds whatever its frame name`` (assembly: string) =
    Assert.True(FSharpCallStackPlan.isFSharpCoreModule (ValueSome assembly))

[<Theory>]
[<InlineData("ClassLibrary")>]
[<InlineData("FSharp.Core.Extra")>]
let ``Another module does not fold`` (assembly: string) =
    Assert.False(FSharpCallStackPlan.isFSharpCoreModule (ValueSome assembly))

[<Fact>]
let ``No module does not fold on its own`` () =
    Assert.False(FSharpCallStackPlan.isFSharpCoreModule ValueNone)
