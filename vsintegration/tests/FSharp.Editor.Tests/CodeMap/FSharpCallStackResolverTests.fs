// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Runs the scenarios of `CallStackSample` and resolves the frames the runtime reports for them
/// against the source they were compiled from, so a change in how the compiler names anything
/// fails here rather than on a map.
module FSharp.Editor.Tests.CodeMap.FSharpCallStackResolverTests

open System.Reflection
open Microsoft.CodeAnalysis
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

let private source = CallStackSample.sourceText ()
let private lineOf = CallStackSample.lineIn source
let private solution = lazy RoslynTestHelpers.CreateSolution source

let private parse (spelling: string) =
    match FSharpStackFrameNameParser.parse spelling with
    | ValueSome frame -> frame
    | ValueNone -> failwith $"frame name did not parse: %s{spelling}"

let private resolveIn (solution: Solution) assemblyName (frame: ParsedFrame) =
    FSharpCallStackResolver.tryResolve solution.Workspace assemblyName frame
    |> CancellableTask.runSynchronouslyWithoutCancellation

/// Resolves a frame the way the provider does: its parsed name, and the line a debugger reports
/// for it when it reports one.
let private resolveAt (solution: Solution) line (frame: ParsedFrame) =
    let project = solution.Projects |> Seq.exactlyOne

    let position =
        if line > 0 then
            ValueSome
                {
                    File = (project.Documents |> Seq.exactlyOne).FilePath
                    Line = line
                }
        else
            ValueNone

    resolveIn solution project.AssemblyName { frame with SourcePosition = position }

/// Every spelling of a captured frame has to resolve, and all of them to the same declaration.
let private resolve (frame: CallStackSample.CapturedFrame) =
    match
        frame.Spellings
        |> List.map (parse >> resolveAt solution.Value frame.Line)
        |> List.distinct
    with
    | [ ValueSome resolved ] -> resolved
    | answers -> failwith $"%A{frame.Spellings} on line %d{frame.Line} resolved to %d{answers.Length} different answers"

/// The innermost frame of a scenario's stack that was executing the line `snippet` is written on.
let private frameOn (snippet: string) (scenario: string) =
    (CallStackSample.run scenario).Own
    |> Array.find (fun frame -> frame.Line = lineOf snippet)

let scenarios: obj array seq =
    CallStackSample.scenarios () |> Seq.map (fun scenario -> [| box scenario |])

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Every frame of a scenario resolves`` (scenario: string) =
    let stack = CallStackSample.run scenario
    Assert.NotEmpty stack.Own

    for frame in stack.Own do
        resolve frame |> ignore

[<Theory>]
[<InlineData(nameof CallStackSample.moduleAndTypeCollision, "sink \"module and type collision\"", "let helper ()")>]
[<InlineData(nameof CallStackSample.nestedModules, "sink \"nested modules\"", "let deeplyNested")>]
[<InlineData(nameof CallStackSample.genericTypeMember, "sink \"generic type member\"", "member _.Unwrap")>]
let ``A frame resolves to the line its declaration is written on`` (scenario: string) (snippet: string) (declaration: string) =
    Assert.Equal(lineOf declaration, (frameOn snippet scenario |> resolve).DeclarationRange.StartLine)

[<Theory>]
[<InlineData(nameof CallStackSample.moduleFunctions, "sink \"module functions\"")>]
[<InlineData(nameof CallStackSample.staticMember, "sink \"static member\"")>]
let ``A frame naming no member of its type stays unresolved`` (scenario: string) (snippet: string) =
    let frame =
        { parse (frameOn snippet scenario).Printed with
            Member = FrameMethod "noSuchMember"
        }

    Assert.True (resolveAt solution.Value 0 frame).IsNone

[<Fact>]
let ``Frames from another assembly are not resolved`` () =
    let frame =
        frameOn "sink \"module functions\"" (nameof CallStackSample.moduleFunctions)

    Assert.True (resolveIn solution.Value "SomeOtherAssembly" (parse frame.Printed)).IsNone

[<Theory>]
[<InlineData(nameof CallStackSample.propertyGetter, "sink \"property getter\"", "Computed")>]
[<InlineData(nameof CallStackSample.propertySetter, "sink \"property setter\"", "Tuned")>]
let ``An accessor resolves as its property`` (scenario: string) (snippet: string) (property: string) =
    let accessor = frameOn snippet scenario |> resolve

    Assert.Equal(ResolvedProperty, accessor.Kind)
    Assert.Equal(property, accessor.DisplayName)
    Assert.Equal(lineOf $"member _.%s{property}", accessor.DeclarationRange.StartLine)

let private fired = typeof<CallStackSample.Publisher>.GetEvent "Fired"

/// An accessor of a `[<CLIEvent>]` holds no user code, so no scenario can stop in one and its name
/// is read off the compiled event instead.
[<Fact>]
let ``An event accessor resolves as an event`` () =
    for accessor in [ fired.GetAddMethod(); fired.GetRemoveMethod() ] do
        match CallStackSample.reflectedName accessor |> parse |> resolveAt solution.Value 0 with
        | ValueNone -> failwith $"expected %s{accessor.Name} to resolve"
        | ValueSome resolved ->
            Assert.Equal(ResolvedEvent, resolved.Kind)
            Assert.Equal(lineOf "member _.Fired", resolved.DeclarationRange.StartLine)

[<Fact>]
let ``An event accessor is never a frame of its handler's stack`` () =
    let stack =
        (CallStackSample.run (nameof CallStackSample.eventHandler)).Own
        |> Array.map _.Method

    Assert.DoesNotContain(fired.GetAddMethod() :> MethodBase, stack)
    Assert.DoesNotContain(fired.GetRemoveMethod() :> MethodBase, stack)

[<Fact>]
let ``A method that raises an event stays a method`` () =
    let fire =
        frameOn "member _.Fire()" (nameof CallStackSample.eventHandler) |> resolve

    Assert.Equal(ResolvedMethod, fire.Kind)
    Assert.DoesNotContain(PropertyGet, fire.Traits)
    Assert.DoesNotContain(PropertySet, fire.Traits)

[<Theory>]
[<InlineData(nameof CallStackSample.nestedClosures, "let inner () = sink", "inner")>]
[<InlineData(nameof CallStackSample.taskBody, "sink \"task continuation\"", "outer")>]
let ``A closure lifted from a named binding is labelled with that name`` (scenario: string) (snippet: string) (label: string) =
    Assert.Equal(label, (frameOn snippet scenario |> resolve).DisplayName)

/// The compiler names these closures by a phrase of its own, and the function that opened an
/// `async` block is not on the stack to say whose body it is.
[<Theory>]
[<InlineData(nameof CallStackSample.asyncBody, "sink \"async body\"")>]
[<InlineData(nameof CallStackSample.pipelineLambdas, "sink \"pipeline lambdas\"")>]
let ``A computation expression body or a pipeline stage is labelled by its function`` (scenario: string) (snippet: string) =
    Assert.StartsWith($"%s{scenario}@", (frameOn snippet scenario |> resolve).DisplayName)

[<Fact>]
let ``Sibling closures stay distinct nodes`` () =
    let closures =
        (CallStackSample.run (nameof CallStackSample.nestedClosures)).Own
        |> Array.filter _.Method.DeclaringType.IsNested

    Assert.Equal(3, closures.Length)

    for closure in closures do
        Assert.Equal(ValueSome closure.Method.DeclaringType.Name, (resolve closure).MemberName)

[<Fact>]
let ``Resolved identity separates namespace from the type chain`` () =
    let resolved =
        frameOn "sink \"nested modules\"" (nameof CallStackSample.nestedModules)
        |> resolve

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
    let resolved =
        frameOn "sink \"generic type member\"" (nameof CallStackSample.genericTypeMember)
        |> resolve

    Assert.Equal<struct (string * int)>([| struct ("CallStackSample", 0); struct ("Box", 1) |], resolved.TypeChain)

/// A debugger prints the instantiation a frame runs in, whose type arguments carry dots of their own.
[<Fact>]
let ``A frame of a closed generic type resolves to the member of its definition`` () =
    let unwrap = typeof<CallStackSample.Box<string>>.GetMethod "Unwrap"

    match CallStackSample.debuggerName unwrap |> parse |> resolveAt solution.Value 0 with
    | ValueNone -> failwith "expected the closed instantiation to resolve"
    | ValueSome resolved -> Assert.Equal(lineOf "member _.Unwrap", resolved.DeclarationRange.StartLine)

/// The frame of the method that initializes the sample's file, as a debugger spells it. It names
/// the file rather than a construct, so the line reported with it is what places it.
let private startupFrame =
    lazy (CallStackSample.debuggerName CallStackSample.Startup.initializer |> parse)

let private resolveStartupOn (snippet: string) =
    resolveAt solution.Value (lineOf snippet) startupFrame.Value

[<Fact>]
let ``Module initialization resolves to the binding on the line`` () =
    match resolveStartupOn "let initialized" with
    | ValueNone -> failwith "expected the module initializer to resolve"
    | ValueSome resolved ->
        Assert.Equal("initialized", resolved.DisplayName)
        Assert.Equal(lineOf "let initialized", resolved.DeclarationRange.StartLine)

/// A private `static let` never reaches the assembly signature, so the enclosing type is the
/// closest name the signature can offer.
[<Fact>]
let ``A private static let resolves to its type's static constructor`` () =
    match resolveStartupOn "static let staticState" with
    | ValueNone -> failwith "expected the static initializer to resolve"
    | ValueSome resolved ->
        Assert.Equal("Initialized", resolved.DisplayName)
        Assert.Equal(ValueSome ".cctor", resolved.MemberName)
        Assert.Contains(Static, resolved.Traits)
        Assert.Contains(Constructor, resolved.Traits)

[<Theory>]
[<InlineData("sink \"generic type member\"", "Box")>]
[<InlineData("static let staticState", "Initialized")>]
let ``A line is answered with the type it is in, not the one before it`` (snippet: string) (expected: string) =
    match resolveStartupOn snippet with
    | ValueNone -> failwith $"expected the line of %s{snippet} to resolve"
    | ValueSome resolved -> Assert.Equal(expected, resolved.DisplayName)

[<Fact>]
let ``A startup frame with no position stays unresolved`` () =
    Assert.True (resolveAt solution.Value 0 startupFrame.Value).IsNone

/// Every initializer of a file runs in one method, so the line is all that tells two of them apart.
[<Fact>]
let ``Two initializers in one startup method resolve to different identities`` () =
    let moduleInitializer = (resolveStartupOn "let initialized").Value
    let staticInitializer = (resolveStartupOn "static let staticState").Value

    Assert.NotEqual<string>(moduleInitializer.DisplayName, staticInitializer.DisplayName)
    Assert.NotEqual(moduleInitializer.MemberName, staticInitializer.MemberName)
    Assert.NotEqual<struct (string * int)>(moduleInitializer.TypeChain, staticInitializer.TypeChain)
    Assert.DoesNotContain(Constructor, moduleInitializer.Traits)
    Assert.Contains(Constructor, staticInitializer.Traits)

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

/// FCS reports an entity's own line and its members' lines, so nothing claims the blank line, the
/// doc comment and the attribute between two declarations: they go to the declaration below them.
/// `linesBelow` reaches the blank line, which holds nothing to search for.
[<Theory>]
[<InlineData("member _.Unwrap", 0, "Unwrap")>]
[<InlineData("member _.Unwrap", 1, "Initialized")>]
[<InlineData("/// Doc comment belonging to Initialized.", 0, "Initialized")>]
[<InlineData("[<Sealed>]", 0, "Initialized")>]
[<InlineData("type Initialized", 0, "``.ctor``")>]
[<InlineData("static let staticState", 0, "Initialized")>]
[<InlineData("let state = Helpers.sink \"constructor\"", 0, "Initialized")>]
[<InlineData("static member StaticState", 0, "StaticState")>]
let ``A line is answered with the member on it, else with the type that owns it`` (snippet: string) (linesBelow: int) (expected: string) =
    let line = CallStackSample.lineIn TypesSeparatedByTrivia snippet + linesBelow

    match resolveAt (RoslynTestHelpers.CreateSolution TypesSeparatedByTrivia) line startupFrame.Value with
    | ValueNone -> failwith $"expected line %d{line} to resolve"
    | ValueSome resolved -> Assert.Equal(expected, resolved.DisplayName)
