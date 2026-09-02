// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// What the provider decides to do with a frame, tested without a graph: the decision is a function
/// of the frame alone, and the workspace reaches it only as "which assembly owns this file".
module FSharp.Editor.Tests.CodeMap.FSharpCallStackPlanTests

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
