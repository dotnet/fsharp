module LanguageServer.CodeActionProtocolTests

open System
open Xunit

open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Test.ProjectGeneration.WorkspaceHelpers
open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.LanguageServer

open LanguageServer.ProtocolHelpers

#nowarn "57"

let private initializeCodeActionServer () =
    let config =
        { FSharpLanguageServerConfig.Default with
            EnabledFeatures = { FSharpLanguageServerFeatures.Default with CodeActions = true } }
    createLanguageServer (FSharpWorkspace()) (Some config)

let private applyAction (source: string) (fileUri: Uri) (action: CodeAction) =
    let offset (position: Position) =
        let mutable start = 0
        for _ in 1 .. position.Line do
            start <- source.IndexOf('\n', start) + 1
        start + position.Character

    action.Edit.Changes[fileUri.AbsoluteUri]
    |> Array.sortByDescending (fun edit -> offset edit.Range.Start)
    |> Array.fold (fun (text: string) edit ->
        let start = offset edit.Range.Start
        text.Remove(start, offset edit.Range.End - start).Insert(start, edit.NewText)) source

/// Helper: open, pull diagnostics, then request code actions on ALL diagnostic lines.
/// Returns (diagnostics, codeActions) tuple for inspection.
let openDiagnoseThenRequestActions (client: TestRpcClient) (fileUri: Uri) (content: string) =
    task {
        do! openDocument client fileUri content 1
        let! report = pullDiagnostics client fileUri
        let diagnostics = report.Items
        // Request code actions covering all diagnostic lines
        let! allActions =
            task {
                if diagnostics.Length = 0 then
                    return [||]
                else
                    let minLine = diagnostics |> Array.map (fun d -> d.Range.Start.Line) |> Array.min
                    return! requestCodeActions client fileUri minLine
            }
        return diagnostics, allActions
    }

// ---------------------------------------------------------------
// Negative case
// ---------------------------------------------------------------

[<Fact>]
let ``Clean code produces no code actions`` () =
    task {
        let! client = initializeCodeActionServer ()
        let content = "module M\nlet x = 1"
        let fileOnDisk = setupSingleFileProject client content
        let! _diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.Empty(actions)
    }

// ---------------------------------------------------------------
// Phase 1: Simple text fixes
// ---------------------------------------------------------------

[<Fact>]
let ``FS0760 - Add new keyword for IDisposable constructor`` () =
    task {
        let content = "module M\nlet x = System.IO.FileStream(\"dummy.txt\", System.IO.FileMode.Create)"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags.Length > 0, "Expected diagnostics")
        Assert.Contains(actions, fun a -> a.Title = "Add 'new' keyword")
    }

[<Fact>]
let ``FS0043 - Replace == with =`` () =
    task {
        let content = "module M\nlet x = 1 == 2"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0043"), "Expected FS0043")
        Assert.Contains(actions, fun a -> a.Title = "Use '=' for equality")
    }

[<Fact>]
let ``FS0043 - Replace != with <>`` () =
    task {
        let content = "module M\nlet x = 1 != 2"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0043"), "Expected FS0043")
        Assert.Contains(actions, fun a -> a.Title = "Use '<>' for inequality")
    }

[<Fact>]
let ``FS0673 - Add instance member parameter`` () =
    task {
        let content = "module M\ntype T() =\n    member Foo() = ()"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0673"), "Expected FS0673")
        Assert.Contains(actions, fun a -> a.Title = "Add missing instance member parameter")
    }

// ---------------------------------------------------------------
// Phase 2: Parse-tree fixes
// ---------------------------------------------------------------

[<Theory>]
[<InlineData("x")>]
[<InlineData("``some value``")>]
[<InlineData("(x, y)")>]
let ``FS0010 - Add fun keyword before argument`` (argument: string) =
    task {
        let content = $"module M\nlet f = {argument} -> 1"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0010"), "Expected FS0010")
        let action = actions |> Array.find (fun a -> a.Title = "Add 'fun' keyword")
        Assert.Equal($"module M\nlet f = fun {argument} -> 1", applyAction content fileOnDisk action)
    }

[<Theory>]
[<InlineData("let mutable x = 1\nx = 2", "let mutable x = 1\nx <- 2")>]
[<InlineData("type C() =\n    member val P = 1 with get, set\nlet c = C()\nc.P = 2",
              "type C() =\n    member val P = 1 with get, set\nlet c = C()\nc.P <- 2")>]
let ``FS0020 - Mutation requires a writable symbol`` (body: string) (expected: string) =
    task {
        let content = "module M\n" + body
        let! client = initializeCodeActionServer ()
        let fileUri = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileUri content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0020"), "Expected FS0020")
        let action = actions |> Array.find (fun a -> a.Title = "Use '<-' for mutation")
        Assert.Equal("module M\n" + expected, applyAction content fileUri action)
    }

[<Theory>]
[<InlineData("let x = 1\nx = 2")>]
[<InlineData("let xs = [1]\nxs |> List.map (fun v -> v = 1)")>]
[<InlineData("let mutable xs = [1]\nxs |> List.map (fun v -> v = 1)")>]
let ``FS0020 - Does not mutate immutable bindings or nested equality`` (body: string) =
    task {
        let content = "module M\n" + body
        let! client = initializeCodeActionServer ()
        let fileUri = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileUri content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0020"), "Expected FS0020")
        Assert.DoesNotContain(actions, fun a -> a.Title = "Use '<-' for mutation")
    }

[<Theory>]
[<InlineData(false, "\n")>]
[<InlineData(true, "\n")>]
[<InlineData(true, "\r\n")>]
let ``FS0366 - Implement only missing interface members preserving newlines`` (partial: bool) (newline: string) =
    task {
        let header = "module M\ntype I =\n    abstract A: int\n    abstract B: int\ntype C() =\n    interface I"
        let content = (header + if partial then " with\n        member _.A = 1 // existing member" else "").Replace("\n", newline)
        let! client = initializeCodeActionServer ()
        let fileUri = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileUri content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0366"), "Expected FS0366")
        let action = actions |> Array.find (fun a -> a.Title = "Implement interface")
        let memberStub name = $"\n        member this.{name}: int = \n            raise (System.NotImplementedException())"
        let expected =
            header + " with"
            + (if partial then "\n        member _.A = 1 // existing member" else memberStub "A")
            + memberStub "B"
        Assert.Equal(expected.Replace("\n", newline), applyAction content fileUri action)
    }

[<Theory>]
[<InlineData("x", "y")>]
[<InlineData("``type``", "``old value``")>]
let ``FS3218 - Rename parameter and body uses to match signature`` (signatureName: string) (implementationName: string) =
    task {
        let! client = initializeCodeActionServer ()
        let basePath = System.IO.Path.Combine(Environment.CurrentDirectory, $"CodeActionSignature{Guid.NewGuid():N}")
        let signatureUri = Uri(basePath + ".fsi")
        let fileUri = Uri(basePath + ".fs")
        let signature = $"module M\nval f: {signatureName}: int -> int"
        let content = $"module M\nlet f {implementationName} = {implementationName} + 1"
        client.Workspace.Files.Open(signatureUri, signature)
        client.Workspace.Files.Open(fileUri, content)
        client.Workspace.Projects.AddOrUpdate(ProjectConfig.Create(), [signatureUri.LocalPath; fileUri.LocalPath]) |> ignore
        do! openDocument client signatureUri signature 1
        let! diags, actions = openDiagnoseThenRequestActions client fileUri content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS3218"), $"Expected FS3218: %A{diags}")
        let action = actions |> Array.find (fun a -> a.Title.StartsWith("Replace with '", StringComparison.Ordinal))
        Assert.Equal($"module M\nlet f {signatureName} = {signatureName} + 1", applyAction content fileUri action)
    }

[<Fact>]
let ``FS0039 - Make outer binding recursive`` () =
    task {
        let content = "module M\nlet f x = if x <= 0 then 1 else f (x - 1)"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0039"), "Expected FS0039")
        Assert.Contains(actions, fun a -> a.Title.Contains("recursive"))
    }

[<Fact>]
let ``FS0027 - Offers make mutable code action`` () =
    task {
        // FS0027: "This value is not mutable"
        let content = "module M\nlet x = 1\nx <- 2"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0027"), "Expected FS0027")
        Assert.Contains(actions, fun a -> a.Title.Contains("mutable"))
    }

// ---------------------------------------------------------------
// Code action edit content verification
// ---------------------------------------------------------------

[<Fact>]
let ``FS0043 code action edit replaces == with =`` () =
    task {
        let content = "module M\nlet x = 1 == 2"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! _diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        let action = actions |> Array.find (fun a -> a.Title = "Use '=' for equality")
        Assert.NotNull(action.Edit)
        let edits = action.Edit.Changes |> Seq.head |> fun kv -> kv.Value
        Assert.Contains(edits, fun e -> e.NewText = "=")
    }

[<Fact>]
let ``FS0760 code action edit inserts new keyword`` () =
    task {
        let content = "module M\nlet x = System.IO.FileStream(\"dummy.txt\", System.IO.FileMode.Create)"
        let! client = initializeCodeActionServer ()
        let fileOnDisk = setupSingleFileProject client content
        let! _diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        let action = actions |> Array.find (fun a -> a.Title = "Add 'new' keyword")
        Assert.NotNull(action.Edit)
        let edits = action.Edit.Changes |> Seq.head |> fun kv -> kv.Value
        Assert.Contains(edits, fun e -> e.NewText.StartsWith("new "))
    }

// ---------------------------------------------------------------
// Multiple code actions for same diagnostic code
// ---------------------------------------------------------------

[<Fact>]
let ``FS0043 produces both == and != fixes for respective operators`` () =
    task {
        // == fix
        let content1 = "module M\nlet x = 1 == 2"
        let! client1 = initializeCodeActionServer ()
        let file1 = setupSingleFileProject client1 content1
        let! _diags1, actions1 = openDiagnoseThenRequestActions client1 file1 content1
        Assert.Contains(actions1, fun a -> a.Title = "Use '=' for equality")

        // != fix
        let content2 = "module M\nlet x = 1 != 2"
        let! client2 = initializeCodeActionServer ()
        let file2 = setupSingleFileProject client2 content2
        let! _diags2, actions2 = openDiagnoseThenRequestActions client2 file2 content2
        Assert.Contains(actions2, fun a -> a.Title = "Use '<>' for inequality")
    }

// ---------------------------------------------------------------
// Code actions capability
// ---------------------------------------------------------------

[<Fact>]
let ``Server with code actions disabled does not advertise code action capabilities`` () =
    task {
        let! client = initializeLanguageServer None
        Assert.False(client.Capabilities.CodeActionProvider.HasValue)
    }

[<Fact>]
let ``Server with code actions enabled advertises code action capabilities`` () =
    task {
        let! client = initializeCodeActionServer ()
        Assert.True(client.Capabilities.CodeActionProvider.HasValue)
    }
