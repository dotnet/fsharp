// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Runs each scenario in `CallStackSample`, takes the frame names the runtime reports for it, and puts
/// them through the frame parser and the call stack resolver. The corpus is therefore whatever the
/// compiler actually mangled the sample into - a naming change surfaces here rather than on a map.
module FSharp.Editor.Tests.CodeMap.FSharpCallStackSampleTests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

let private solution =
    lazy RoslynTestHelpers.CreateSolution(CallStackSample.sourceText ())

/// Resolves one captured frame the way the provider does: the parsed name, plus the source position
/// the debugger would have supplied - which is the only anchor for a `task` body's closure, named
/// after line 1 rather than the line it was written on.
let private resolveFrame (struct (frameName: string, line: int)) =
    let solution = solution.Value
    let project = solution.Projects |> Seq.exactlyOne
    let document = project.Documents |> Seq.exactlyOne

    match FSharpStackFrameNameParser.parse frameName with
    | ValueNone -> failwith $"frame name did not parse: %s{frameName}"
    | ValueSome frame ->
        let positioned =
            { frame with
                SourcePosition =
                    if line > 0 then
                        ValueSome
                            {
                                File = document.FilePath
                                Line = line
                            }
                    else
                        ValueNone
            }

        positioned,
        (FSharpCallStackResolver.tryResolve solution.Workspace project.AssemblyName positioned
         |> CancellableTask.runSynchronouslyWithoutCancellation)

/// A scenario's whole stack has to come back resolved. Module initialization is the one frame that
/// cannot: its name points at the file rather than a construct, so it needs the source position the
/// debugger supplies, which `FSharpCallStackResolverTests` covers directly.
let private assertScenarioResolves (scenario: unit -> int) =
    scenario () |> ignore

    let frames = CallStackSample.frames ()
    Assert.NotEmpty frames

    let unresolved =
        frames
        |> Array.choose (fun captured ->
            let struct (frameName, line) = captured

            match resolveFrame captured with
            | { Member = FrameStartupCode }, _ -> None
            | _, ValueNone -> Some $"{frameName} (line {line})"
            | _, ValueSome _ -> None)

    if not (Array.isEmpty unresolved) then
        failwith $"""frames left unresolved:{Environment.NewLine}%s{String.Join(Environment.NewLine, unresolved)}"""

[<Fact>]
let ``Module functions resolve`` () =
    assertScenarioResolves CallStackSample.moduleFunctions

[<Fact>]
let ``Pipeline lambdas resolve`` () =
    assertScenarioResolves CallStackSample.pipelineLambdas

[<Fact>]
let ``Nested closures resolve`` () =
    assertScenarioResolves CallStackSample.nestedClosures

[<Fact>]
let ``Local functions resolve`` () =
    assertScenarioResolves (fun () -> CallStackSample.localFunctions 1)

[<Fact>]
let ``A generic function resolves`` () =
    assertScenarioResolves CallStackSample.genericFunctionScenario

[<Fact>]
let ``A custom operator resolves`` () =
    assertScenarioResolves CallStackSample.customOperator

[<Fact>]
let ``A CompiledName rename resolves`` () =
    assertScenarioResolves CallStackSample.originalSourceName

[<Fact>]
let ``An active pattern resolves`` () =
    assertScenarioResolves CallStackSample.activePattern

[<Fact>]
let ``A partial active pattern resolves`` () =
    assertScenarioResolves CallStackSample.partialActivePattern

[<Fact>]
let ``Recursion resolves`` () =
    assertScenarioResolves CallStackSample.recursion

[<Fact>]
let ``Mutual recursion resolves`` () =
    assertScenarioResolves CallStackSample.mutualRecursion

[<Fact>]
let ``A higher order function resolves`` () =
    assertScenarioResolves CallStackSample.higherOrder

[<Fact>]
let ``An async body resolves`` () =
    assertScenarioResolves CallStackSample.asyncBody

[<Fact>]
let ``A task body resolves`` () =
    assertScenarioResolves CallStackSample.taskBody

[<Fact>]
let ``A seq body resolves`` () =
    assertScenarioResolves CallStackSample.seqBody

[<Fact>]
let ``Nested modules resolve`` () =
    assertScenarioResolves CallStackSample.nestedModules

[<Fact>]
let ``Class members resolve`` () =
    assertScenarioResolves CallStackSample.instanceMember

[<Fact>]
let ``A static member resolves`` () =
    assertScenarioResolves CallStackSample.staticMember

[<Fact>]
let ``An interface implementation resolves`` () =
    assertScenarioResolves CallStackSample.interfaceImplementation

[<Fact>]
let ``A generic type member resolves`` () =
    assertScenarioResolves CallStackSample.genericTypeMember

[<Fact>]
let ``A constructor resolves`` () =
    assertScenarioResolves CallStackSample.constructors

[<Fact>]
let ``A union member resolves`` () =
    assertScenarioResolves CallStackSample.unionMember

[<Fact>]
let ``A record member resolves`` () =
    assertScenarioResolves CallStackSample.recordMember

/// The accessors are the pair that reached the map as bare `get` and `set` nodes.
[<Fact>]
let ``A property getter resolves as a property`` () =
    assertScenarioResolves CallStackSample.propertyGetter

    let accessor =
        CallStackSample.frames ()
        |> Array.pick (fun captured ->
            match resolveFrame captured with
            | _, ValueSome resolved when resolved.Kind = ResolvedProperty -> Some resolved
            | _ -> None)

    Assert.Equal("Computed", accessor.DisplayName)

[<Fact>]
let ``A property setter resolves as a property`` () =
    assertScenarioResolves CallStackSample.propertySetter

    let accessor =
        CallStackSample.frames ()
        |> Array.pick (fun captured ->
            match resolveFrame captured with
            | _, ValueSome resolved when resolved.Kind = ResolvedProperty -> Some resolved
            | _ -> None)

    Assert.Equal("Tuned", accessor.DisplayName)

/// The label a closure node carries. A named binding reads well on its own; a computation-expression
/// body or a pipeline stage is named by a phrase the compiler invented, so it takes the enclosing
/// declaration's name and the line - the `async` block's own function is not on the stack at all.
let private labelOf (contains: string) =
    CallStackSample.frames ()
    |> Array.pick (fun captured ->
        let struct (frameName, _) = captured

        if frameName.Contains contains then
            match resolveFrame captured with
            | _, ValueSome resolved -> Some resolved.DisplayName
            | _, ValueNone -> None
        else
            None)

[<Fact>]
let ``A closure lifted from a named binding is labelled with that name`` () =
    CallStackSample.nestedClosures () |> ignore
    Assert.Equal("inner", labelOf "inner@")

[<Fact>]
let ``An async body is labelled by the function that opened it`` () =
    CallStackSample.asyncBody () |> ignore

    Assert.StartsWith("asyncBody@", labelOf "Pipe #1 input at line")

[<Fact>]
let ``A pipeline stage is labelled by the function it runs in`` () =
    CallStackSample.pipelineLambdas () |> ignore

    Assert.StartsWith("pipelineLambdas@", labelOf "Pipe #1 stage")
