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
/// finds the FS3888 diagnostic at index `diagIndex` on the .fs, invokes the
/// code-fix, captures the registered CodeAction, applies it and returns the
/// resulting .fsi text. Use `tryFixSig` for the common case (first diagnostic).
let private tryFixSigAt (diagIndex: int) (fsiCode: string) (fsCode: string) : string option =
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

    if diagIndex >= diagnostics.Length then None
    else
        let diagnostic = diagnostics[diagIndex]
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

let private tryFixSig fsiCode fsCode = tryFixSigAt 0 fsiCode fsCode

let private countDiags (fsiCode: string) (fsCode: string) : int =
    let documents = RoslynTestHelpers.GetFsiAndFsDocuments fsiCode fsCode |> Seq.toList
    let fsDoc = documents |> List.find (fun d -> not d.IsFSharpSignatureFile)
    let _, checkResults =
        fsDoc.GetFSharpParseAndCheckResultsAsync "test"
        |> Microsoft.VisualStudio.FSharp.Editor.CancellableTasks.CancellableTask.runSynchronouslyWithoutCancellation
    checkResults.Diagnostics
    |> Array.filter (fun d -> d.ErrorNumber = 3888)
    |> Array.length

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

// -------------------------------------------------------------------------
// Multi-attribute scenarios
// -------------------------------------------------------------------------

[<Fact>]
let ``Two enforced attributes stacked on separate lines produce two independent fixes`` () =
    let fsiCode = """
module M
val inline f: x: int -> int
"""
    let fsCode = """
module M
[<NoDynamicInvocationAttribute>]
[<RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""
    // Two FS3888 diagnostics fire (one per missing attribute).
    Assert.Equal(2, countDiags fsiCode fsCode)

    // First fix: NoDynamicInvocation.
    let firstFsi = tryFixSigAt 0 fsiCode fsCode
    Assert.Contains("[<NoDynamicInvocationAttribute>]", firstFsi |> Option.defaultValue "")

    // Second fix: RequiresExplicitTypeArguments.
    let secondFsi = tryFixSigAt 1 fsiCode fsCode
    Assert.Contains("[<RequiresExplicitTypeArguments>]", secondFsi |> Option.defaultValue "")

[<Fact>]
let ``Two enforced attributes on one line [<A; B>] produce two independent fixes`` () =
    let fsiCode = """
module M
val inline f: x: int -> int
"""
    let fsCode = """
module M
[<NoDynamicInvocationAttribute; RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""
    Assert.Equal(2, countDiags fsiCode fsCode)

    let firstFsi = tryFixSigAt 0 fsiCode fsCode |> Option.defaultValue ""
    // First diagnostic (NoDynamicInvocation): inserted as its OWN [< >] block
    // - not concatenated with the second attribute. The wrap is per-attribute
    // because SynAttribute.Range covers one attribute body, not the whole list.
    Assert.Contains("[<NoDynamicInvocationAttribute>]", firstFsi)
    // Should not have leaked the second attribute's text into the insertion.
    Assert.DoesNotContain("[<NoDynamicInvocationAttribute; ", firstFsi)

    let secondFsi = tryFixSigAt 1 fsiCode fsCode |> Option.defaultValue ""
    Assert.Contains("[<RequiresExplicitTypeArguments>]", secondFsi)
    Assert.DoesNotContain("[<RequiresExplicitTypeArguments; ", secondFsi)

[<Fact>]
let ``Mixed: enforced attr next to a non-enforced attr on same line - only the enforced one is copied`` () =
    let fsiCode = """
module M
val inline f: x: int -> int
"""
    let fsCode = """
module M
[<NoDynamicInvocationAttribute; System.Obsolete("old")>]
let inline f (x: int) = x + 1
"""
    Assert.Equal(1, countDiags fsiCode fsCode)
    let fsi = tryFixSig fsiCode fsCode |> Option.defaultValue ""
    Assert.Contains("[<NoDynamicInvocationAttribute>]", fsi)
    // System.Obsolete is not enforced - must not appear in the .fsi insertion.
    Assert.DoesNotContain("System.Obsolete", fsi)

[<Fact>]
let ``Non-enforced attribute already on .fsi declaration - new attribute is added above and the existing one is kept`` () =
    let fsiCode = """
module M
[<System.Obsolete("old")>]
val inline f: x: int -> int
"""
    let fsCode = """
module M
[<NoDynamicInvocationAttribute>]
[<System.Obsolete("old")>]
let inline f (x: int) = x + 1
"""
    let fsi = tryFixSig fsiCode fsCode |> Option.defaultValue ""
    // Both attributes should be present on the val in the .fsi.
    Assert.Contains("[<NoDynamicInvocationAttribute>]", fsi)
    Assert.Contains("""[<System.Obsolete("old")>]""", fsi)
