// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveExtraAttributeFromImplementationTests

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

let private codeFix = RemoveExtraAttributeFromImplementationCodeFixProvider()

/// Same-document harness: builds an .fsi + .fs pair, runs the F# checker,
/// finds the FS3888 diagnostic at index `diagIndex` on the .fs, invokes the
/// reverse code-fix, and returns the resulting .fs text.
let private tryFixFsAt (diagIndex: int) (fsiCode: string) (fsCode: string) : string option =
    let documents = RoslynTestHelpers.GetFsiAndFsDocuments fsiCode fsCode |> Seq.toList
    let fsDoc = documents |> List.find (fun d -> not d.IsFSharpSignatureFile)

    let sourceText = fsDoc.GetTextAsync().Result

    let _, checkResults =
        fsDoc.GetFSharpParseAndCheckResultsAsync "test"
        |> Microsoft.VisualStudio.FSharp.Editor.CancellableTasks.CancellableTask.runSynchronouslyWithoutCancellation

    let diagnostics =
        checkResults.Diagnostics
        |> Array.filter (fun d -> d.ErrorNumber = 3888)
        |> Array.map (Diagnostic.ofFSharpDiagnostic sourceText fsDoc.FilePath)

    if diagIndex >= diagnostics.Length then
        None
    else
        let diagnostic = diagnostics[diagIndex]
        let mutable captured: CodeAction option = None

        let register =
            System.Action<CodeAction, ImmutableArray<Diagnostic>>(fun action _ -> captured <- Some action)

        let ctx =
            CodeFixContext(fsDoc, diagnostic.Location.SourceSpan, ImmutableArray.Create diagnostic, register, CancellationToken.None)

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
            let newFsDoc = newSolution.GetDocument(fsDoc.Id)
            Some((newFsDoc.GetTextAsync().Result).ToString())

let private tryFixFs fsiCode fsCode = tryFixFsAt 0 fsiCode fsCode

[<Fact>]
let ``Lone attribute on its own line is removed cleanly`` () =
    let fsi =
        """module M
val inline f: x: int -> int
"""

    let fs =
        """module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""

    let expected =
        """module M
let inline f (x: int) = x + 1
"""

    Assert.Equal(Some expected, tryFixFs fsi fs)

[<Fact>]
let ``First sibling in [<A; B>] list is removed, second sibling preserved`` () =
    let fsi =
        """module M
val inline f: x: int -> int
"""

    let fs =
        """module M
[<NoDynamicInvocationAttribute; RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""
    // Reverse-fix the FIRST diagnostic (NoDynamicInvocation). The other
    // FS3888 (RequiresExplicitTypeArguments) is still pending but its
    // separate code-action would remove the second sibling.
    let expected =
        """module M
[<RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""

    Assert.Equal(Some expected, tryFixFsAt 0 fsi fs)

[<Fact>]
let ``Second sibling in [<A; B>] list is removed, first sibling preserved`` () =
    let fsi =
        """module M
val inline f: x: int -> int
"""

    let fs =
        """module M
[<NoDynamicInvocationAttribute; RequiresExplicitTypeArguments>]
let inline f (x: int) = x + 1
"""

    let expected =
        """module M
[<NoDynamicInvocationAttribute>]
let inline f (x: int) = x + 1
"""

    Assert.Equal(Some expected, tryFixFsAt 1 fsi fs)

[<Fact>]
let ``Attribute on type with body is removed without breaking the type`` () =
    let fsi =
        """module M
type C =
    new: unit -> C
"""

    let fs =
        """module M
[<AllowNullLiteral>]
type C() = class end
"""

    let expected =
        """module M
type C() = class end
"""

    Assert.Equal(Some expected, tryFixFs fsi fs)

[<Fact>]
let ``Attribute with arguments is removed cleanly`` () =
    let fsi =
        """module M
type C =
    new: unit -> C
"""

    let fs =
        """module M
[<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type C() = class end
"""

    let expected =
        """module M
type C() = class end
"""

    Assert.Equal(Some expected, tryFixFs fsi fs)
