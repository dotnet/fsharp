// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddMissingAttributeToSignatureTests

open System.Collections.Immutable
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework
open Xunit

let private codeFix = AddMissingAttributeToSignatureCodeFixProvider()

/// Cross-document harness: builds an .fsi + .fs pair, runs the F# checker,
/// finds the first FS3888 diagnostic on the .fs, invokes the code-fix, captures
/// the registered CodeAction, applies it and returns the resulting .fsi text.
let private tryFixSig (fsiCode: string) (fsCode: string) : string option =
    let documents = RoslynTestHelpers.GetFsiAndFsDocuments fsiCode fsCode |> Seq.toList
    let fsiDoc = documents |> List.find (fun d -> d.IsFSharpSignatureFile)
    let fsDoc  = documents |> List.find (fun d -> not d.IsFSharpSignatureFile)

    let sourceText = fsDoc.GetTextAsync().Result
    let _, checkResults =
        fsDoc.GetFSharpParseAndCheckResultsAsync "test"
        |> Microsoft.VisualStudio.FSharp.Editor.CancellableTasks.CancellableTask.runSynchronouslyWithoutCancellation

    let diagnostics =
        checkResults.Diagnostics
        |> Array.filter (fun d -> d.ErrorNumber = 3888)
        |> Array.map (Diagnostic.ofFSharpDiagnostic sourceText fsDoc.FilePath)

    if diagnostics.Length = 0 then None
    else
        let diagnostic = diagnostics[0]
        let mutable captured : CodeAction option = None
        let register =
            System.Action<CodeAction, ImmutableArray<Diagnostic>>(fun action _ -> captured <- Some action)
        let ctx =
            CodeFixContext(
                fsDoc,
                diagnostic.Location.SourceSpan,
                ImmutableArray.Create diagnostic,
                register,
                CancellationToken.None
            )
        codeFix.RegisterCodeFixesAsync(ctx).Wait()
        match captured with
        | None -> None
        | Some action ->
            let operations = action.GetOperationsAsync(CancellationToken.None).Result
            let applyOp =
                operations
                |> Seq.pick (function
                    | :? ApplyChangesOperation as op -> Some op
                    | _ -> None)
            let newSolution = applyOp.ChangedSolution
            let newFsi = newSolution.GetDocument(fsiDoc.Id)
            Some ((newFsi.GetTextAsync().Result).ToString())

[<Fact>]
let ``Module-level: AutoOpen on nested module is inserted into .fsi`` () =
    let fsiCode = """
module M
module Inner =
    val x: int
"""
    let fsCode = """
module M
[<AutoOpen>]
module Inner =
    let x = 42
"""
    let expectedFsi = """
module M
[<AutoOpen>]
module Inner =
    val x: int
"""
    let actual = tryFixSig fsiCode fsCode
    Assert.Equal<string>(expectedFsi, actual |> Option.defaultValue "")

[<Fact>]
let ``Type-level: RequireQualifiedAccess on union is inserted into .fsi`` () =
    let fsiCode = """
module M
type U = A | B
"""
    let fsCode = """
module M
[<RequireQualifiedAccess>]
type U = A | B
"""
    let expectedFsi = """
module M
[<RequireQualifiedAccess>]
type U = A | B
"""
    let actual = tryFixSig fsiCode fsCode
    Assert.Equal<string>(expectedFsi, actual |> Option.defaultValue "")

[<Fact>]
let ``Function-level: NoDynamicInvocation on val is inserted into .fsi`` () =
    let fsiCode = """
module M
val inline f: x: int -> int
"""
    let fsCode = """
module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""
    let expectedFsi = """
module M
[<NoDynamicInvocationAttribute>]
val inline f: x: int -> int
"""
    let actual = tryFixSig fsiCode fsCode
    Assert.Equal<string>(expectedFsi, actual |> Option.defaultValue "")

[<Fact>]
let ``Attribute with argument: AllowNullLiteral(false) is copied verbatim with args`` () =
    let fsiCode = """
module M
type C =
    new: unit -> C
"""
    let fsCode = """
module M
[<AllowNullLiteral(false)>]
type C() = class end
"""
    let expectedFsi = """
module M
[<AllowNullLiteral(false)>]
type C =
    new: unit -> C
"""
    let actual = tryFixSig fsiCode fsCode
    Assert.Equal<string>(expectedFsi, actual |> Option.defaultValue "")
