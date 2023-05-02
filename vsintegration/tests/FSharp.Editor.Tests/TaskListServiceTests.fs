module FSharp.Editor.Tests.TaskListServiceTests

open System
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.IO
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.Text

let createDocument (fileContents: string) =
    RoslynTestHelpers.CreateSolution(fileContents)
    |> RoslynTestHelpers.GetSingleDocument

let private service =
    new Microsoft.VisualStudio.FSharp.Editor.FSharpTaskListService()

let private ct = CancellationToken.None

let private descriptors =
    [| "TODO"; "HACK" |] |> Array.map (fun s -> s, Unchecked.defaultof<_>)

let assertTasks expectedTasks fileContents =
    let doc = createDocument fileContents
    let sourceText = doc.GetTextAsync().Result

    let t =
        service.GetTaskListItems(doc, sourceText, [], (Some "preview"), descriptors, ct)

    let tasks = t |> Seq.map (fun t -> t.Message) |> List.ofSeq
    Assert.Equal<string list>(expectedTasks |> List.sort, tasks |> List.sort)

[<Fact>]
let ``End of line comment is a task`` () =
    assertTasks [ "TODO improve" ] "let x = 1 // TODO improve"

[<Fact>]
let ``Inline comments are tasks`` () =
    assertTasks [ "TODO first "; "HACK second " ] "let x = 1 (* TODO first *) + 2  (* HACK second *)"

[<Fact>]
let ``ifdef code is not a task`` () =
    """
let x = 1
#if UNDEFINED_VAR
// TODO not here
#endif
"""
    |> assertTasks []

[<Fact>]
let ``Multiline comment can have more tasks`` () =
    """
(* TODO first
TODO second
TODO third *)
"""
    |> assertTasks [ "TODO first"; "TODO second"; "TODO third " ]

[<Fact>]
let ``Lowercase todo is still a task`` () =
    assertTasks [ "todo improve" ] "let x = 1 // todo improve"

[<Fact>]
let ``Descriptor followed by letter is NOT a task`` () =
    assertTasks [] "let x = 1 // hackathon solution"

[<Fact>]
let ``Descriptor followed by non-letter is OK`` () =
    assertTasks [ "HACK2: using 1" ] "let x = 1 // HACK2: using 1"
