module FSharp.Editor.Tests.Refactors.ConvertNamespaceModuleTests

open System
open System.Threading

open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor

open Xunit

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private refactored (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code

    let document =
        tryRefactor code (caretAt code marker) context (new FSharpConvertNamespaceModuleRefactoring())

    (document.GetTextAsync() |> GetTaskResult).ToString()

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertNamespaceModuleRefactoring())

let private nestedHelpers =
    "namespace My.Company\n\n/// Utilities.\n[<AutoOpen; RequireQualifiedAccess>]\nmodule private Helpers =\n    let inline twice x = x + x\n    let banner = \"\"\"\n  not\n    touched\"\"\"\n"

let private rootHelpers =
    "/// Utilities.\n[<AutoOpen; RequireQualifiedAccess>]\nmodule private My.Company.Helpers\nlet inline twice x = x + x\nlet banner = \"\"\"\n  not\n    touched\"\"\"\n"

[<Theory>]
[<InlineData("namespace")>]
[<InlineData("module")>]
let ``Namespace with a single nested module converts to a root module`` (marker: string) =
    Assert.Equal(rootHelpers, refactored nestedHelpers marker)

[<Fact>]
let ``Root module converts to a namespace with a nested module`` () =
    Assert.Equal(nestedHelpers, refactored rootHelpers "module")

[<Theory>]
[<InlineData("namespace rec A.B\n\nmodule C =\n    let a = 1\n", "module rec A.B.C\nlet a = 1\n")>]
[<InlineData("namespace A.B\n\nmodule rec C =\n    let a = 1\n", "module rec A.B.C\nlet a = 1\n")>]
[<InlineData("namespace rec A.B\n\nmodule rec C =\n    let a = 1\n", "module rec A.B.C\nlet a = 1\n")>]
[<InlineData("namespace A.B\n\n// Helpers for B.\nmodule C =\n    let x = 1\n", "// Helpers for B.\nmodule A.B.C\nlet x = 1\n")>]
[<InlineData("namespace A.B\n\nmodule C =\n    let x = 1\n#if DEBUG\n    let y = 2\n#endif\n",
             "module A.B.C\nlet x = 1\n#if DEBUG\nlet y = 2\n#endif\n")>]
[<InlineData("namespace A.B\r\n\r\nmodule C =\r\n    let x = 1\r\n", "module A.B.C\r\nlet x = 1\r\n")>]
[<InlineData("namespace A.B\n\nopen System\n\nmodule C =\n    let x = 1\n", "module A.B.C\n\nopen System\n\nlet x = 1\n")>]
[<InlineData("namespace A.B\n\nopen System\nopen System.Text\n\nmodule C =\n    let x = 1\n",
             "module A.B.C\n\nopen System\nopen System.Text\n\nlet x = 1\n")>]
[<InlineData("namespace A.B\n\nopen System\n\n/// Doc.\nmodule C =\n    let x = 1\n", "/// Doc.\nmodule A.B.C\n\nopen System\n\nlet x = 1\n")>]
let ``Nested module converts to a root module`` (before: string, after: string) =
    Assert.Equal(after, refactored before "namespace")

[<Theory>]
[<InlineData("module A.B.C\n\nlet x = 1\n", "namespace A.B\n\nmodule C =\n\n    let x = 1\n")>]
[<InlineData("module rec A.B.C\nlet a = 1\n", "namespace A.B\n\nmodule rec C =\n    let a = 1\n")>]
[<InlineData("module [<AutoOpen>] internal A.B.C\nlet x = 1\n", "namespace A.B\n\nmodule [<AutoOpen>] internal C =\n    let x = 1\n")>]
[<InlineData("module ``A-B``.C // header\nlet x = 1\n", "namespace ``A-B``\n\nmodule C = // header\n    let x = 1\n")>]
[<InlineData("module A.B.C\r\nlet x = 1\r\n", "namespace A.B\r\n\r\nmodule C =\r\n    let x = 1\r\n")>]
let ``Root module converts to a nested module`` (before: string, after: string) =
    Assert.Equal(after, refactored before "module")

[<Fact>]
let ``Converting to a nested module and back restores the root module`` () =
    let original = "module A.B.C\n\nlet x = 1\n"
    Assert.Equal(original, refactored (refactored original "module") "namespace")

[<Theory>]
[<InlineData("namespace A.B\n\nmodule C =\n    let x = 1\n\nmodule D =\n    let y = 2\n", "namespace")>]
[<InlineData("namespace A.B\n\ntype T = int\n", "namespace")>]
[<InlineData("namespace global\n\nmodule C =\n    let x = 1\n", "namespace")>]
[<InlineData("module C\n\nlet x = 1\n", "module")>]
[<InlineData("namespace A.B\n\nmodule C = begin\n    let x = 1\nend\n", "namespace")>]
[<InlineData("namespace A.B // B\n\nmodule C =\n    let x = 1\n", "namespace")>]
[<InlineData("namespace A.B\n\nmodule C =\n    let x = 1\n", "let")>]
[<InlineData("module A.B.C\n\nlet x = 1\n", "let")>]
[<InlineData("namespace A.B\n\nmodule C =\n", "namespace")>]
[<InlineData("namespace A.B\n\nmodule C =\n    let x = 1\n\nopen System\n", "namespace")>]
[<InlineData("namespace A.B\n\nopen System\n\ntype T = int\n\nmodule C =\n    let x = 1\n", "namespace")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``No action when the file has a signature`` () =
    let code = "namespace A.B\n\nmodule C =\n    let x = 1\n"
    let signature = "namespace A.B\n\nmodule C =\n    val x: int\n"
    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature code |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(0, 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertNamespaceModuleRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
