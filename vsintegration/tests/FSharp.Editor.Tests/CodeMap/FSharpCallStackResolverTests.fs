// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeMap.FSharpCallStackResolverTests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

let private source =
    """
module Sample.Library

type Collision = { Value: int }

module Collision =
    let helper () = ()

type SampleClass(seed: int) =
    member _.InstanceMethod(x: int) = ()
    member _.Property with get () = seed

let (|Even|Odd|) n = if n % 2 = 0 then Even else Odd

let moduleFunction (a: int) (b: string) = ()

let pipelineLambda (xs: int list) =
    xs |> List.map (fun x -> x * 2)
"""

/// The resolver reports declaration ranges, so expectations are pinned to the source line a
/// construct is written on rather than to a hard-coded number.
let private lineOf (snippet: string) =
    let lines = source.Replace("\r\n", "\n").Split('\n')

    match
        lines
        |> Array.tryFindIndexV (fun line -> line.IndexOf(snippet, StringComparison.Ordinal) >= 0)
    with
    | ValueSome i -> i + 1
    | ValueNone -> failwith $"snippet not found in test source: %s{snippet}"

let private resolve (frameName: string) =
    let solution = RoslynTestHelpers.CreateSolution source
    let project = solution.Projects |> Seq.exactlyOne

    match FSharpStackFrameNameParser.parse frameName with
    | ValueNone -> failwith $"frame name did not parse: %s{frameName}"
    | ValueSome frame ->
        FSharpCallStackResolver.tryResolve solution.Workspace project.AssemblyName frame
        |> CancellableTask.runSynchronouslyWithoutCancellation

let frames: obj[] list =
    [
        [| "Sample.Library.moduleFunction"; "let moduleFunction" |]
        [| "Sample.Library.SampleClass.InstanceMethod"; "member _.InstanceMethod" |]
        [| "Sample.Library.SampleClass..ctor"; "type SampleClass" |]
        [| "Sample.Library.SampleClass.get_Property"; "member _.Property" |]
        [| "Sample.Library.|Even|Odd|"; "let (|Even|Odd|)" |]
        // the `Module` suffix the compiler adds when a module collides with a type must be undone
        [| "Sample.Library.CollisionModule.helper"; "let helper" |]
        // a lambda has no signature entity of its own, so it reports the binding containing it
        [| "Sample.Library.pipelineLambda@18.Invoke"; "let pipelineLambda" |]
    ]

[<Theory>]
[<MemberData(nameof frames)>]
let ``Frame resolves to the declaring source line`` (frameName: string) (snippet: string) =
    match resolve frameName with
    | ValueNone -> failwith $"expected %s{frameName} to resolve"
    | ValueSome resolved -> Assert.Equal(lineOf snippet, resolved.DeclarationRange.StartLine)

[<Theory>]
[<InlineData("Sample.Library.noSuchFunction")>]
[<InlineData("Some.Other.Assembly.thing")>]
let ``Unknown frames stay unresolved`` (frameName: string) = Assert.True((resolve frameName).IsNone)

[<Fact>]
let ``Frames from another assembly are not resolved`` () =
    let solution = RoslynTestHelpers.CreateSolution source

    let frame = (FSharpStackFrameNameParser.parse "Sample.Library.moduleFunction").Value

    let resolved =
        FSharpCallStackResolver.tryResolve solution.Workspace "SomeOtherAssembly" frame
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.True resolved.IsNone
