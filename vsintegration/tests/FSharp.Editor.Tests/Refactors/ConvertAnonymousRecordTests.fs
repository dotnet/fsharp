module FSharp.Editor.Tests.Refactors.ConvertAnonymousRecordTests

open System

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Compiler.Diagnostics

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private textOf (document: Document) =
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private errorsOf (document: Document) =
    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    checkResults.Diagnostics
    |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

let private refactorIn (context: TestContext) (code: string) (marker: string) =
    tryRefactor code (caretAt code marker) context (new FSharpConvertAnonymousRecordRefactoring())

let private refactored (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    let document = refactorIn context code marker
    Assert.Empty(errorsOf document)
    textOf document

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertAnonymousRecordRefactoring())

[<Theory>]
[<InlineData("""
module M

let show () = printfn "%A" {| A = 1 |}
""",
             "A = 1",
             """
module M

let show () = printfn "%A" struct {| A = 1 |}
""")>]
[<InlineData("""
module M

let items: list<{| A: int |}> = []
""",
             "A: int",
             """
module M

let items: list<struct {| A: int |}> = []
""")>]
[<InlineData("""
module M

let config =
    {|
        Name = "x"
        Size = 1
    |}
""",
             "Size",
             """
module M

let config =
    struct {|
        Name = "x"
        Size = 1
    |}
""")>]
let ``Anonymous record that flows nowhere else converts on its own`` (reference: string, marker: string, structs: string) =
    Assert.Equal(structs, refactored reference marker)
    Assert.Equal(reference, refactored structs marker)

[<Fact>]
let ``Value and the annotations it flows into convert together`` () =
    let reference =
        """
module M

let person = {| Name = "Ada"; Age = 36 |}
let copy: {| Name: string; Age: int |} = person
let age = person.Age
"""

    let structs =
        """
module M

let person = struct {| Name = "Ada"; Age = 36 |}
let copy: struct {| Name: string; Age: int |} = person
let age = person.Age
"""

    Assert.Equal(structs, refactored reference "Ada")
    Assert.Equal(reference, refactored structs "Ada")

[<Fact>]
let ``Parameter annotation converts its arguments`` () =
    let reference =
        """
module M

let greet (p: {| Name: string |}) = "Hi " + p.Name

let ada = {| Name = "Ada" |}
let greetings = [ greet ada; greet {| Name = "Bob" |} ]
"""

    let structs =
        """
module M

let greet (p: struct {| Name: string |}) = "Hi " + p.Name

let ada = struct {| Name = "Ada" |}
let greetings = [ greet ada; greet struct {| Name = "Bob" |} ]
"""

    Assert.Equal(structs, refactored reference "Name: string")
    Assert.Equal(reference, refactored structs "Name: string")

[<Fact>]
let ``Return type converts the result and the values it is bound to`` () =
    let reference =
        """
module M

let origin () : {| X: int; Y: int |} = {| X = 0; Y = 0 |}
let start: {| X: int; Y: int |} = origin ()
"""

    let structs =
        """
module M

let origin () : struct {| X: int; Y: int |} = struct {| X = 0; Y = 0 |}
let start: struct {| X: int; Y: int |} = origin ()
"""

    Assert.Equal(structs, refactored reference "X: int")
    Assert.Equal(reference, refactored structs "X: int")

[<Fact>]
let ``Record field converts its values and the annotations reading it`` () =
    let reference =
        """
module M

type Person = { Info: {| Age: int |}; Tags: {| Count: int |} }

let person = { Info = {| Age = 30 |}; Tags = {| Count = 0 |} }
let info: {| Age: int |} = person.Info
"""

    let structs =
        """
module M

type Person = { Info: struct {| Age: int |}; Tags: {| Count: int |} }

let person = { Info = struct {| Age = 30 |}; Tags = {| Count = 0 |} }
let info: struct {| Age: int |} = person.Info
"""

    Assert.Equal(structs, refactored reference "Age: int")
    Assert.Equal(reference, refactored structs "Age: int")

[<Theory>]
[<InlineData("""
module M

let point = {| X = 1 |}
let moved = {| point with Y = 2 |}
""",
             "X = 1",
             """
module M

let point = struct {| X = 1 |}
let moved = {| point with Y = 2 |}
""")>]
[<InlineData("""
module M

let point = {| X = 1 |}
let moved = {| point with Y = 2 |}
""",
             "with",
             """
module M

let point = {| X = 1 |}
let moved = struct {| point with Y = 2 |}
""")>]
let ``Copy-and-update converts independently of its source`` (reference: string, marker: string, structs: string) =
    Assert.Equal(structs, refactored reference marker)
    Assert.Equal(reference, refactored structs marker)

[<Fact>]
let ``Value declared in another file converts there`` () =
    let definition =
        """
module A

let person = {| Name = "Ada" |}
"""

    let code =
        """
module B

let name (p: {| Name: string |}) = p.Name
let text = name A.person
"""

    use context = TestContext.CreateWithCodeAndDependency code definition
    let document = refactorIn context code "Name: string"

    Assert.Equal(
        """
module B

let name (p: struct {| Name: string |}) = p.Name
let text = name A.person
""",
        textOf document
    )

    Assert.Equal(
        """
module A

let person = struct {| Name = "Ada" |}
""",
        (context.Solution.Projects |> Seq.head).Documents |> Seq.head |> textOf
    )

    // The checker reads the other file of a synthetic project from disk, so the result is checked as a new project.
    let definitionAfter =
        (context.Solution.Projects |> Seq.head).Documents |> Seq.head |> textOf

    use checkContext =
        TestContext.CreateWithCodeAndDependency (textOf document) definitionAfter

    Assert.Empty((checkContext.Solution.Projects |> Seq.head).Documents |> Seq.last |> errorsOf)

[<Fact>]
let ``Title names the target kind`` () =
    let reference =
        """
module M

let point = {| X = 1 |}
"""

    let structs =
        """
module M

let point = struct {| X = 1 |}
"""

    Assert.Equal("Convert to struct anonymous record", (actionsAt reference "X = 1" |> Seq.exactlyOne).Title)
    Assert.Equal("Convert to reference anonymous record", (actionsAt structs "X = 1" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("""
module M

let x = 1
""",
             "1")>]
[<InlineData("""
module M

type Point = { X: int }

let point = { X = 1 }
""",
             "X = 1")>]
[<InlineData("""
module M

let quoted = <@ {| A = 1 |} @>
""",
             "A = 1")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)
