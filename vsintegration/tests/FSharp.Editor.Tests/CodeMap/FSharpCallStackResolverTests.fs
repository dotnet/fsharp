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
let private lineIn (text: string) (snippet: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n')

    match
        lines
        |> Array.tryFindIndexV (fun line -> line.IndexOf(snippet, StringComparison.Ordinal) >= 0)
    with
    | ValueSome i -> i + 1
    | ValueNone -> failwith $"snippet not found in the sample: %s{snippet}"

let private lineOf = lineIn source

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
let private startupFrame () =
    match FSharpStackFrameNameParser.parse $"<StartupCode$Sample>.$%s{Sample}" with
    | ValueNone -> failwith "the startup frame did not parse"
    | ValueSome frame -> frame

let private resolveStartupAt (snippet: string) =
    let document =
        solution.Value.Projects |> Seq.exactlyOne |> _.Documents |> Seq.exactlyOne

    resolveParsed
        { startupFrame () with
            SourcePosition =
                ValueSome
                    {
                        File = document.FilePath
                        Line = lineOf snippet
                    }
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

/// The accessors the compiler writes for `[<CLIEvent>]` are the only `add_`/`remove_` frames F#
/// produces, and no scenario can stop inside one - they contain no user code. They are what makes a
/// node an event rather than a method.
[<Theory>]
[<InlineData("add_Fired")>]
[<InlineData("remove_Fired")>]
let ``An event accessor resolves as an event`` (accessor: string) =
    match resolve $"%s{Sample}.Publisher.%s{accessor}" with
    | ValueNone -> failwith $"expected %s{accessor} to resolve"
    | ValueSome resolved ->
        Assert.Equal(ResolvedEvent, resolved.Kind)
        Assert.Equal(lineOf "member _.Fired", resolved.DeclarationRange.StartLine)

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
        Assert.Equal("initialized", resolved.DisplayName)
        Assert.Equal(lineOf "let initialized", resolved.DeclarationRange.StartLine)

/// A private `static let` runs in its type's static constructor and never reaches the assembly
/// signature, so the enclosing type is the closest name the signature can offer.
[<Fact>]
let ``A private static let resolves to its type's static constructor`` () =
    match resolveStartupAt "static let staticState" with
    | ValueNone -> failwith "expected the static initializer to resolve"
    | ValueSome resolved ->
        Assert.Equal("Initialized", resolved.DisplayName)
        Assert.Equal(ValueSome ".cctor", resolved.MemberName)
        Assert.Contains(Static, resolved.Traits)
        Assert.Contains(Constructor, resolved.Traits)

/// `Box` is declared immediately before `Initialized`, so "the nearest declaration above the line"
/// is only the right answer while the line itself is right. A line read off a frame belonging to a
/// different run put a frame from `Initialized` several lines higher, and the nearest declaration
/// above *there* is `Box` - which is how a static initializer came to be labelled `Box` on a map.
/// Both sides of that boundary are pinned here.
[<Theory>]
[<InlineData("sink \"generic type member\"", "Box")>]
[<InlineData("static let staticState", "Initialized")>]
let ``A line is answered with the type it is in, not the one before it`` (snippet: string) (expected: string) =
    match resolveStartupAt snippet with
    | ValueNone -> failwith $"expected the line of %s{snippet} to resolve"
    | ValueSome resolved -> Assert.Equal(expected, resolved.DisplayName)

/// Module initialization is named after its file rather than after any construct, so a frame with
/// no line has nothing to resolve to. That is what frames disagreeing now leaves behind, and a grey
/// node is the honest answer where guessing produced `Box`.
[<Fact>]
let ``A startup frame with no position stays unresolved`` () =
    Assert.True (resolveParsed (startupFrame ())).IsNone

/// A generic type, then the blank line, doc comment and attribute belonging to the type after it.
/// Those lines are inside no entity: FCS reports an entity's own line and its members' lines, and
/// nothing claims the trivia between two declarations.
[<Literal>]
let private TypesSeparatedByTrivia =
    """
namespace Shipped

open System

module Helpers =
    let sink (scenario: string) = scenario.Length

type Box<'T>(value: 'T) =
    member _.Unwrap() = Helpers.sink "generic type member"

/// Doc comment belonging to Initialized.
[<Sealed>]
type Initialized(seed: int) =
    static let staticState = Helpers.sink "static constructor"
    let state = Helpers.sink "constructor" + seed

    member _.State = state
    static member StaticState = staticState
"""

let private resolveStartupIn (text: string) (snippet: string) =
    let solution = RoslynTestHelpers.CreateSolution text
    let project = solution.Projects |> Seq.exactlyOne
    let document = project.Documents |> Seq.exactlyOne

    let frame =
        { startupFrame () with
            SourcePosition =
                ValueSome
                    {
                        File = document.FilePath
                        Line = lineIn text snippet
                    }
        }

    FSharpCallStackResolver.tryResolve solution.Workspace project.AssemblyName frame
    |> CancellableTask.runSynchronouslyWithoutCancellation

/// The demo's static initializer reached the map labelled `Box` - the generic type declared above
/// `Initialized`, which has no static constructor at all. Every line from the one after `Box`'s last
/// member down to `Initialized`'s own is a line the answer must not be `Box` on.
[<Theory>]
[<InlineData("/// Doc comment belonging to Initialized.", "Initialized")>]
[<InlineData("[<Sealed>]", "Initialized")>]
[<InlineData("static let staticState", "Initialized")>]
[<InlineData("let state = Helpers.sink \"constructor\"", "Initialized")>]
let ``Trivia between two types is answered with the type below it`` (snippet: string) (expected: string) =
    match resolveStartupIn TypesSeparatedByTrivia snippet with
    | ValueNone -> failwith $"expected the line of %s{snippet} to resolve"
    | ValueSome resolved -> Assert.Equal(expected, resolved.DisplayName)

/// The blank line between `Box`'s last member and `Initialized`'s doc comment, addressed by number
/// because it holds nothing to search for.
[<Fact>]
let ``The blank line between two types is answered with the type below it`` () =
    let blank = lineIn TypesSeparatedByTrivia "member _.Unwrap" + 1

    let solution = RoslynTestHelpers.CreateSolution TypesSeparatedByTrivia
    let project = solution.Projects |> Seq.exactlyOne
    let document = project.Documents |> Seq.exactlyOne

    let frame =
        { startupFrame () with
            SourcePosition =
                ValueSome
                    {
                        File = document.FilePath
                        Line = blank
                    }
        }

    match
        FSharpCallStackResolver.tryResolve solution.Workspace project.AssemblyName frame
        |> CancellableTask.runSynchronouslyWithoutCancellation
    with
    | ValueNone -> failwith "expected the blank line to resolve"
    | ValueSome resolved -> Assert.Equal("Initialized", resolved.DisplayName)

/// A line that is a member's own is answered with that member, not with the type around it.
[<Theory>]
[<InlineData("member _.Unwrap", "Unwrap")>]
[<InlineData("type Initialized", "``.ctor``")>]
[<InlineData("static member StaticState", "StaticState")>]
let ``A member's own line is answered with the member`` (snippet: string) (expected: string) =
    match resolveStartupIn TypesSeparatedByTrivia snippet with
    | ValueNone -> failwith $"expected the line of %s{snippet} to resolve"
    | ValueSome resolved -> Assert.Equal(expected, resolved.DisplayName)
