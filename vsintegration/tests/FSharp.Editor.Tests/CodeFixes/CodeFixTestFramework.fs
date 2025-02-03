// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework

open System
open System.Collections.Immutable
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Diagnostics
open FSharp.Editor.Tests.Helpers

type TestCodeFix = { Message: string; FixedCode: string }

module TestCodeFix =
    /// Creates a test code fix from the given Roslyn source text and F# code fix.
    let ofFSharpCodeFix (sourceText: SourceText) (codeFix: FSharpCodeFix) =
        {
            Message = codeFix.Message
            FixedCode = string (sourceText.WithChanges codeFix.Changes)
        }

type Mode =
    | Auto
    | WithOption of CustomProjectOption: string
    | WithSignature of FsiCode: string
    | Manual of Squiggly: string * Diagnostic: string
    | WithSettings of CodeFixesOptions

module ValueOption =
    let inline either f y o =
        match o with
        | ValueSome v -> f v
        | ValueNone -> y

module Document =
    /// Creates a code analysis document from the
    /// given code according to the given mode.
    let create mode code =
        match mode with
        | Auto -> RoslynTestHelpers.GetFsDocument code
        | WithOption option -> RoslynTestHelpers.GetFsDocument(code, option)
        | WithSignature fsiCode -> RoslynTestHelpers.GetFsiAndFsDocuments fsiCode code |> Seq.last
        | Manual _ -> RoslynTestHelpers.GetFsDocument code
        | WithSettings settings -> RoslynTestHelpers.GetFsDocument(code, customEditorOptions = settings)

module Diagnostic =
    let ofFSharpDiagnostic sourceText filePath fsharpDiagnostic =
        RoslynHelpers.ConvertError(fsharpDiagnostic, RoslynHelpers.RangeToLocation(fsharpDiagnostic.Range, sourceText, filePath))

module FSharpDiagnostics =
    /// Generates F# diagnostics using the given document according to the given mode.
    let generate mode (document: Document) =
        match mode with
        | Manual(squiggly, diagnostic) ->
            cancellableTask {
                let! sourceText = document.GetTextAsync()
                let spanStart = sourceText.ToString().IndexOf squiggly
                let span = TextSpan(spanStart, squiggly.Length)
                let range = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)

                let number, prefix =
                    let regex = Regex "([A-Z]+)(\d+)"
                    let matchGroups = regex.Match(diagnostic).Groups
                    let prefix = matchGroups[1].Value
                    let number = int matchGroups[2].Value
                    number, prefix

                return
                    [|
                        FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "test", number, range, prefix)
                    |]
            }

        | Auto
        | WithOption _
        | WithSignature _
        | WithSettings _ ->
            document.GetFSharpParseAndCheckResultsAsync "test"
            |> CancellableTask.map (fun (_, checkResults) -> checkResults.Diagnostics)

module CodeFixContext =
    let private registerCodeFix =
        Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

    let tryCreate (fixable: Diagnostic -> bool) (document: Document) (diagnostics: ImmutableArray<Diagnostic>) =
        diagnostics
        |> Seq.filter fixable
        |> Seq.tryHead
        |> ValueOption.ofOption
        |> ValueOption.map (fun diagnostic ->
            CodeFixContext(
                document,
                diagnostic.Location.SourceSpan,
                ImmutableArray.Create diagnostic,
                registerCodeFix,
                System.Threading.CancellationToken.None
            ))

type CodeFixProvider with

    member this.CanFix(diagnostic: Diagnostic) =
        this.FixableDiagnosticIds.Contains diagnostic.Id

let tryFix (code: string) mode (fixProvider: 'T when 'T :> IFSharpCodeFixProvider and 'T :> CodeFixProvider) =
    cancellableTask {
        let document = Document.create mode code
        let sourceText = SourceText.From code
        let! fsharpDiagnostics = FSharpDiagnostics.generate mode document

        let canFix =
            match mode with
            | Manual _ -> fun _ -> true
            | _ -> fixProvider.CanFix

        let context =
            fsharpDiagnostics
            |> Seq.map (Diagnostic.ofFSharpDiagnostic sourceText document.FilePath)
            |> Seq.toImmutableArray
            |> CodeFixContext.tryCreate canFix document

        return!
            context
            |> ValueOption.either fixProvider.GetCodeFixIfAppliesAsync (CancellableTask.singleton ValueNone)
            |> CancellableTask.map (ValueOption.map (TestCodeFix.ofFSharpCodeFix sourceText) >> ValueOption.toOption)
    }
    |> CancellableTask.runSynchronouslyWithoutCancellation

let multiFix (code: string) mode (fixProvider: 'T when 'T :> IFSharpMultiCodeFixProvider and 'T :> CodeFixProvider) =
    cancellableTask {
        let document = Document.create mode code
        let sourceText = SourceText.From code
        let! fsharpDiagnostics = FSharpDiagnostics.generate mode document

        let canFix =
            match mode with
            | Manual _ -> fun _ -> true
            | _ -> fixProvider.CanFix

        let context =
            fsharpDiagnostics
            |> Seq.map (Diagnostic.ofFSharpDiagnostic sourceText document.FilePath)
            |> Seq.toImmutableArray
            |> CodeFixContext.tryCreate canFix document

        return!
            context
            |> ValueOption.either fixProvider.GetCodeFixesAsync (CancellableTask.singleton Seq.empty)
            |> CancellableTask.map (Seq.map (TestCodeFix.ofFSharpCodeFix sourceText))
    }
    |> CancellableTask.runSynchronouslyWithoutCancellation

/// Contains types and functions for conveniently making code fix assertions using xUnit.
[<AutoOpen>]
module Xunit =
    open System.Threading.Tasks
    open Xunit

    /// Indicates that a code fix was expected but was not generated.
    exception MissingCodeFixException of message: string * exn: Xunit.Sdk.XunitException

    /// Indicates that a code fix was not expected but was generated nonetheless.
    exception UnexpectedCodeFixException of message: string * exn: Xunit.Sdk.XunitException

    /// Indicates that the offered code fix was incorrect.
    exception WrongCodeFixException of message: string * exn: Xunit.Sdk.XunitException

    /// <summary>
    /// Asserts that the actual code equals the expected code.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="expected"/> or <paramref name="actual"/> are null.
    /// </exception>
    /// <exception cref="T:Xunit.Sdk.EqualException">
    /// Thrown if any line in the actual code differs from the corresponding line in the expected code.
    /// </exception>
    let shouldEqual expected actual =
        if isNull expected then
            nullArg (nameof expected)

        if isNull actual then
            nullArg (nameof actual)

        let split (s: string) =
            s.Split([| Environment.NewLine |], StringSplitOptions.RemoveEmptyEntries)

        let expectedLines = split expected
        let actualLines = split actual

        Assert.Equal<string>(expectedLines, actualLines)

    /// <summary>
    /// Expects no code fix to be applied to the given code.
    /// </summary>
    /// <param name="code">The code to try to fix.</param>
    /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework.Xunit.UnexpectedCodeFixException">
    /// Thrown if a code fix is applied.
    /// </exception>
    let expectNoFix (tryFix: string -> CancellableTask<TestCodeFix option>) code =
        cancellableTask {
            match! tryFix code with
            | None -> ()
            | Some actual ->
                let e = Assert.ThrowsAny(fun () -> shouldEqual code actual.FixedCode)
                raise (UnexpectedCodeFixException("Did not expect a code fix but got one anyway.", e))
        }
        |> CancellableTask.startWithoutCancellation

    /// <summary>
    /// Expects the given code to be fixed as specified, or,
    /// if <paramref name="code"/> = <paramref name="fixedCode"/>, for the code not to be fixed.
    /// </summary>
    /// <param name="tryFix">A function to apply to the given code to generate a code fix.</param>
    /// <param name="code">The code to try to fix.</param>
    /// <param name="fixedCode">The code with the expected fix applied.</param>
    /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework.Xunit.MissingCodeFixException">
    /// Thrown if a code fix is not applied when expected.
    /// </exception>
    /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework.Xunit.UnexpectedCodeFixException">
    /// Thrown if a code fix is applied when not expected.
    /// </exception>
    /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework.Xunit.WrongCodeFixException">
    /// Thrown if the generated fix does not match the expected fixed code.
    /// </exception>
    let expectFix (tryFix: string -> CancellableTask<TestCodeFix option>) code fixedCode =
        if code = fixedCode then
            expectNoFix tryFix code
        else
            cancellableTask {
                match! tryFix code with
                | None ->
                    let e = Assert.ThrowsAny(fun () -> shouldEqual fixedCode code)
                    return raise (MissingCodeFixException("Expected a code fix but did not get one.", e))

                | Some actual ->
                    try
                        shouldEqual fixedCode actual.FixedCode
                    with :? Xunit.Sdk.XunitException as e ->
                        return raise (WrongCodeFixException("The applied code fix did not match the expected fix.", e))
            }
            |> CancellableTask.startWithoutCancellation

    [<Sealed>]
    type MemberDataBuilder private () =
        static member val Instance = MemberDataBuilder()
        member _.Combine(xs, ys) = Seq.append xs ys
        member _.Delay f = f ()
        member _.Zero() = Seq.empty
        member _.Yield(x, y) = Seq.singleton [| box x; box y |]

        member _.YieldFrom pairs =
            seq { for x, y in pairs -> [| box x; box y |] }

        member _.For(xs, f) = xs |> Seq.collect f
        member _.Run objArrays = objArrays

    /// <summary>
    /// Given a sequence of pairs, builds an <c>obj array seq</c> for use with the <see cref="T:Xunit.MemberDataAttribute"/>.
    ///
    /// memberData {
    ///     originalCode, fixedCode
    ///     …
    /// }
    /// </summary>
    let memberData = MemberDataBuilder.Instance
