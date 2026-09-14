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

// Both contain a triple-quoted string, which a triple-quoted literal cannot hold.
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
[<InlineData("""
namespace rec A.B

module C =
    let a = 1
""",
             """
module rec A.B.C
let a = 1
""")>]
[<InlineData("""
namespace A.B

module rec C =
    let a = 1
""",
             """
module rec A.B.C
let a = 1
""")>]
[<InlineData("""
namespace rec A.B

module rec C =
    let a = 1
""",
             """
module rec A.B.C
let a = 1
""")>]
[<InlineData("""
namespace A.B

// Helpers for B.
module C =
    let x = 1
""",
             """
// Helpers for B.
module A.B.C
let x = 1
""")>]
[<InlineData("""
namespace A.B

module C =
    let x = 1
#if DEBUG
    let y = 2
#endif
""",
             """
module A.B.C
let x = 1
#if DEBUG
let y = 2
#endif
""")>]
[<InlineData("namespace A.B\r\n\r\nmodule C =\r\n    let x = 1\r\n", "module A.B.C\r\nlet x = 1\r\n")>]
[<InlineData("""
namespace A.B

open System

module C =
    let x = 1
""",
             """
module A.B.C

open System

let x = 1
""")>]
[<InlineData("""
namespace A.B

open System
open System.Text

module C =
    let x = 1
""",
             """
module A.B.C

open System
open System.Text

let x = 1
""")>]
[<InlineData("""
namespace A.B

open System

/// Doc.
module C =
    let x = 1
""",
             """
/// Doc.
module A.B.C

open System

let x = 1
""")>]
let ``Nested module converts to a root module`` (before: string, after: string) =
    Assert.Equal(after, refactored before "namespace")

[<Theory>]
[<InlineData("""
module A.B.C

let x = 1
""",
             """
namespace A.B

module C =

    let x = 1
""")>]
[<InlineData("""
module rec A.B.C
let a = 1
""",
             """
namespace A.B

module rec C =
    let a = 1
""")>]
[<InlineData("""
module [<AutoOpen>] internal A.B.C
let x = 1
""",
             """
namespace A.B

module [<AutoOpen>] internal C =
    let x = 1
""")>]
[<InlineData("""
module ``A-B``.C // header
let x = 1
""",
             """
namespace ``A-B``

module C = // header
    let x = 1
""")>]
[<InlineData("module A.B.C\r\nlet x = 1\r\n", "namespace A.B\r\n\r\nmodule C =\r\n    let x = 1\r\n")>]
let ``Root module converts to a nested module`` (before: string, after: string) =
    Assert.Equal(after, refactored before "module")

[<Fact>]
let ``Converting to a nested module and back restores the root module`` () =
    let original =
        """
module A.B.C

let x = 1
"""

    Assert.Equal(original, refactored (refactored original "module") "namespace")

[<Theory>]
[<InlineData("""
namespace A.B

module C =
    let x = 1

module D =
    let y = 2
""",
             "namespace")>]
[<InlineData("""
namespace A.B

type T = int
""",
             "namespace")>]
[<InlineData("""
namespace global

module C =
    let x = 1
""",
             "namespace")>]
[<InlineData("""
module C

let x = 1
""",
             "module")>]
[<InlineData("""
namespace A.B

module C = begin
    let x = 1
end
""",
             "namespace")>]
[<InlineData("""
namespace A.B // B

module C =
    let x = 1
""",
             "namespace")>]
[<InlineData("""
namespace A.B

module C =
    let x = 1
""",
             "let")>]
[<InlineData("""
module A.B.C

let x = 1
""",
             "let")>]
[<InlineData("""
namespace A.B

module C =
""",
             "namespace")>]
[<InlineData("""
namespace A.B

module C =
    let x = 1

open System
""",
             "namespace")>]
[<InlineData("""
namespace A.B

open System

type T = int

module C =
    let x = 1
""",
             "namespace")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``No action when the file has a signature`` () =
    let code =
        """
namespace A.B

module C =
    let x = 1
"""

    let signature =
        """
namespace A.B

module C =
    val x: int
"""

    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature code |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt code "namespace", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertNamespaceModuleRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
