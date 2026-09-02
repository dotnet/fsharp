// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The frames a running scenario cannot produce, resolved against the same source `CallStackSample`
/// is compiled from: the spellings only a debug engine uses, the ones that need a source position
/// rather than a name, and the identity a resolved frame has to carry.
///
/// Everything a scenario *can* produce is covered end to end by `FSharpCallStackSampleTests`.
module FSharp.Editor.Tests.CodeMap.FSharpCallStackResolverTests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

[<Literal>]
let private Sample = "FSharp.Editor.Tests.CodeMap.CallStackSample"

let private source = CallStackSample.sourceText ()

/// The resolver reports declaration ranges, so expectations are pinned to the source line a
/// construct is written on rather than to a hard-coded number.
let private lineOf (snippet: string) =
    let lines = source.Replace("\r\n", "\n").Split('\n')

    match
        lines
        |> Array.tryFindIndexV (fun line -> line.IndexOf(snippet, StringComparison.Ordinal) >= 0)
    with
    | ValueSome i -> i + 1
    | ValueNone -> failwith $"snippet not found in the sample: %s{snippet}"

let private solution = lazy RoslynTestHelpers.CreateSolution source

let private resolveParsed (frame: ParsedFrame) =
    let solution = solution.Value
    let project = solution.Projects |> Seq.exactlyOne

    FSharpCallStackResolver.tryResolve solution.Workspace project.AssemblyName frame
    |> CancellableTask.runSynchronouslyWithoutCancellation

let private resolve (frameName: string) =
    match FSharpStackFrameNameParser.parse frameName with
    | ValueNone -> failwith $"frame name did not parse: %s{frameName}"
    | ValueSome frame -> resolveParsed frame

/// A `<StartupCode$…>` frame names the file rather than the construct, so the line the debugger
/// reports is the only anchor. The provider reads it off the frame node and fills it in here.
let private resolveStartupAt (snippet: string) =
    let solution = solution.Value
    let document = solution.Projects |> Seq.exactlyOne |> _.Documents |> Seq.exactlyOne

    match FSharpStackFrameNameParser.parse $"<StartupCode$Sample>.$%s{Sample}" with
    | ValueNone -> failwith "the startup frame did not parse"
    | ValueSome frame ->
        resolveParsed
            { frame with
                SourcePosition = ValueSome(struct (document.FilePath, lineOf snippet))
            }

/// Frame names whose path the resolver has to take apart, rather than constructs a scenario would
/// exercise anyway: a module renamed by a collision, modules nested three deep, a generic type.
let frames: obj[] list =
    [
        [| $"%s{Sample}.CollisionModule.helper"; "let helper ()" |]
        [| $"%s{Sample}.Outer.Middle.Inner.deeplyNested"; "let deeplyNested" |]
        [| $"%s{Sample}.Box`1.Unwrap"; "member _.Unwrap" |]
    ]

[<Theory>]
[<MemberData(nameof frames)>]
let ``Frame resolves to the declaring source line`` (frameName: string) (snippet: string) =
    match resolve frameName with
    | ValueNone -> failwith $"expected %s{frameName} to resolve"
    | ValueSome resolved -> Assert.Equal(lineOf snippet, resolved.DeclarationRange.StartLine)

[<Theory>]
[<InlineData("noSuchFunction")>]
[<InlineData("Worker.NoSuchMember")>]
let ``Unknown frames stay unresolved`` (member': string) =
    Assert.True((resolve $"%s{Sample}.%s{member'}").IsNone)

[<Fact>]
let ``Frames from another assembly are not resolved`` () =
    let frame = (FSharpStackFrameNameParser.parse $"%s{Sample}.moduleFunctions").Value

    let resolved =
        FSharpCallStackResolver.tryResolve solution.Value.Workspace "SomeOtherAssembly" frame
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.True resolved.IsNone

/// A debug engine spells an accessor with a trailing segment, where metadata - and so the frame a
/// running scenario produces - spells it `get_Computed`.
[<Fact>]
let ``The debug engine's spelling of a getter resolves as a property`` () =
    match resolve $"%s{Sample}.Worker.Computed.get" with
    | ValueNone -> failwith "expected the getter to resolve"
    | ValueSome resolved ->
        Assert.Equal(ResolvedProperty, resolved.Kind)
        Assert.Equal(lineOf "member _.Computed", resolved.DeclarationRange.StartLine)

[<Fact>]
let ``The debug engine's spelling of a setter resolves as a property`` () =
    match resolve $"%s{Sample}.Worker.Tuned.set" with
    | ValueNone -> failwith "expected the setter to resolve"
    | ValueSome resolved ->
        Assert.Equal(ResolvedProperty, resolved.Kind)
        Assert.Equal(lineOf "member _.Tuned", resolved.DeclarationRange.StartLine)

[<Fact>]
let ``Sibling closures stay distinct nodes`` () =
    let line = lineOf "let inner () = sink"

    let identityOf frameName =
        match resolve frameName with
        | ValueNone -> failwith $"expected %s{frameName} to resolve"
        | ValueSome resolved -> resolved.MemberName

    let first = identityOf $"%s{Sample}.inner@%d{line}.Invoke"
    let second = identityOf $"%s{Sample}.inner@%d{line}-1.Invoke"

    Assert.NotEqual(first, second)
    Assert.Equal(ValueSome $"inner@%d{line}", first)

[<Fact>]
let ``Resolved identity separates namespace from the type chain`` () =
    match resolve $"%s{Sample}.Outer.Middle.Inner.deeplyNested" with
    | ValueNone -> failwith "expected the nested-module frame to resolve"
    | ValueSome resolved ->
        Assert.Equal(ValueSome "FSharp.Editor.Tests.CodeMap", resolved.Namespace)

        Assert.Equal<struct (string * int)>(
            [|
                struct ("CallStackSample", 0)
                struct ("Outer", 0)
                struct ("Middle", 0)
                struct ("Inner", 0)
            |],
            resolved.TypeChain
        )

[<Fact>]
let ``Generic type carries its arity in the type chain`` () =
    match resolve $"%s{Sample}.Box`1.Unwrap" with
    | ValueNone -> failwith "expected the generic-type frame to resolve"
    | ValueSome resolved -> Assert.Equal<struct (string * int)>([| struct ("CallStackSample", 0); struct ("Box", 1) |], resolved.TypeChain)

[<Fact>]
let ``Module initialization resolves to the binding on the line`` () =
    match resolveStartupAt "let initialized" with
    | ValueNone -> failwith "expected the module initializer to resolve"
    | ValueSome resolved ->
        Assert.Equal(ValueSome "initialized", resolved.DisplayName)
        Assert.Equal(lineOf "let initialized", resolved.DeclarationRange.StartLine)

/// A private `static let` runs in its type's static constructor and never reaches the assembly
/// signature, so the enclosing type is the closest name the signature can offer.
[<Fact>]
let ``A private static let resolves to its type's static constructor`` () =
    match resolveStartupAt "static let staticState" with
    | ValueNone -> failwith "expected the static initializer to resolve"
    | ValueSome resolved ->
        Assert.Equal(ValueSome "Initialized", resolved.DisplayName)
        Assert.Equal(ValueSome ".cctor", resolved.MemberName)
        Assert.True resolved.Modifiers.IsStatic
        Assert.True resolved.Modifiers.IsConstructor
