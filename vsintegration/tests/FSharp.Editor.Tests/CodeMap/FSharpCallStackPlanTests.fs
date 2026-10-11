// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeMap.FSharpCallStackPlanTests

open Xunit
open Microsoft.VisualStudio.FSharp.Editor

[<Literal>]
let private ProjectFile = @"C:\Demo\ClassLibrary\Demo.fs"

let private assemblyForFile file =
    if file = ProjectFile then
        ValueSome "ClassLibrary"
    else
        ValueNone

/// The decision reads the module and the position only, so the frame it is made for needs no name.
let private decide moduleAssembly position =
    FSharpCallStackPlan.decide
        assemblyForFile
        {
            Frame =
                {
                    Path = [||]
                    Member = FrameStartupCode
                    MethodGenericArity = 0
                    SourcePosition = position
                }
            ModuleAssembly = moduleAssembly
        }

[<Fact>]
let ``A frame naming its module resolves in that assembly`` () =
    Assert.Equal(ResolveIn "ClassLibrary", decide (ValueSome "ClassLibrary") ValueNone)

[<Fact>]
let ``A frame without a module is addressed by the file it runs in`` () =
    Assert.Equal(ResolveIn "ClassLibrary", decide ValueNone (ValueSome { File = ProjectFile; Line = 144 }))

[<Fact>]
let ``A frame with neither a module nor a known file is left alone`` () =
    let elsewhere =
        {
            File = @"C:\Elsewhere\Other.fs"
            Line = 1
        }

    Assert.Equal(LeaveUnresolved, decide ValueNone (ValueSome elsewhere))

[<Fact>]
let ``A frame with no position and no module is left alone`` () =
    Assert.Equal(LeaveUnresolved, decide ValueNone ValueNone)

/// SourceLink puts FSharp.Core's own source path on frames the debugger reports without a module.
[<Fact>]
let ``An FSharp.Core frame is recognised by its source path when it names no module`` () =
    let position =
        {
            File = "/_/src/FSharp.Core/async.fs"
            Line = 1818
        }

    Assert.Equal(FoldAsExternalCode, decide ValueNone (ValueSome position))

[<Fact>]
let ``FSharp.Core is recognised whatever its casing`` () =
    Assert.Equal(FoldAsExternalCode, decide (ValueSome "fsharp.core") ValueNone)

/// What the plumbing of a pipeline, a computation expression or an event is made of is FSharp.Core's
/// business and changes with it, so the frames are the ones the scenario ran through.
[<Theory>]
[<InlineData(nameof CallStackSample.pipelineLambdas)>]
[<InlineData(nameof CallStackSample.asyncBody)>]
[<InlineData(nameof CallStackSample.seqBody)>]
[<InlineData(nameof CallStackSample.eventHandler)>]
let ``Every FSharp.Core frame of a scenario folds into External Code`` (scenario: string) =
    let plumbing = (CallStackSample.run scenario).Plumbing
    Assert.NotEmpty plumbing

    for frame in plumbing do
        for spelling in frame.Spellings do
            match FSharpStackFrameNameParser.parse spelling with
            | ValueNone -> failwith $"the parser rejects %s{spelling}, so nothing folds it"
            | ValueSome parsed ->
                let facts =
                    {
                        Frame = parsed
                        ModuleAssembly = ValueSome(frame.Method.DeclaringType.Assembly.GetName().Name)
                    }

                Assert.Equal(FoldAsExternalCode, FSharpCallStackPlan.decide assemblyForFile facts)

[<Theory>]
[<InlineData("FSharp.Core", true)>]
[<InlineData("fsharp.core", true)>]
[<InlineData("ClassLibrary", false)>]
[<InlineData("FSharp.Core.Extra", false)>]
let ``Only an FSharp.Core module folds whatever its frame name`` (assembly: string) (folds: bool) =
    Assert.Equal(folds, FSharpCallStackPlan.isFSharpCoreModule (ValueSome assembly))

[<Fact>]
let ``No module does not fold on its own`` () =
    Assert.False(FSharpCallStackPlan.isFSharpCoreModule ValueNone)

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
    let positions =
        [ { File = ProjectFile; Line = 168 }; { File = ProjectFile; Line = 164 } ]

    Assert.True (FSharpCallStackPlan.positionOf positions).IsNone

[<Fact>]
let ``A frame nothing claims yields no position`` () =
    Assert.True (FSharpCallStackPlan.positionOf []).IsNone
