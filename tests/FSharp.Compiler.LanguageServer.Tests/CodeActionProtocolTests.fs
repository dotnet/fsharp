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
        let! client = initializeLanguageServer None
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
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags.Length > 0, "Expected diagnostics")
        Assert.Contains(actions, fun a -> a.Title = "Add 'new' keyword")
    }

[<Fact>]
let ``FS0043 - Replace == with =`` () =
    task {
        let content = "module M\nlet x = 1 == 2"
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0043"), "Expected FS0043")
        Assert.Contains(actions, fun a -> a.Title = "Use '=' for equality")
    }

[<Fact>]
let ``FS0043 - Replace != with <>`` () =
    task {
        let content = "module M\nlet x = 1 != 2"
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0043"), "Expected FS0043")
        Assert.Contains(actions, fun a -> a.Title = "Use '<>' for inequality")
    }

[<Fact>]
let ``FS0673 - Add instance member parameter`` () =
    task {
        let content = "module M\ntype T() =\n    member Foo() = ()"
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0673"), "Expected FS0673")
        Assert.Contains(actions, fun a -> a.Title = "Add missing instance member parameter")
    }

// ---------------------------------------------------------------
// Phase 2: Parse-tree fixes
// ---------------------------------------------------------------

[<Fact>]
let ``FS0010 - Add fun keyword`` () =
    task {
        let content = "module M\nlet f = x -> x + 1"
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client content
        let! diags, actions = openDiagnoseThenRequestActions client fileOnDisk content
        Assert.True(diags |> Array.exists (fun d -> d.Code.Value.Second = "FS0010"), "Expected FS0010")
        Assert.Contains(actions, fun a -> a.Title = "Add 'fun' keyword")
    }

[<Fact>]
let ``FS0039 - Make outer binding recursive`` () =
    task {
        let content = "module M\nlet f x = if x <= 0 then 1 else f (x - 1)"
        let! client = initializeLanguageServer None
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
        let! client = initializeLanguageServer None
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
        let! client = initializeLanguageServer None
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
        let! client = initializeLanguageServer None
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
        let! client1 = initializeLanguageServer None
        let file1 = setupSingleFileProject client1 content1
        let! _diags1, actions1 = openDiagnoseThenRequestActions client1 file1 content1
        Assert.Contains(actions1, fun a -> a.Title = "Use '=' for equality")

        // != fix
        let content2 = "module M\nlet x = 1 != 2"
        let! client2 = initializeLanguageServer None
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
        let config = { FSharpLanguageServerConfig.Default with EnabledFeatures = { Diagnostics = true; CodeActions = false } }
        let! client = createLanguageServer (FSharpWorkspace()) (Some config)
        Assert.False(client.Capabilities.CodeActionProvider.HasValue)
    }

[<Fact>]
let ``Server with code actions enabled advertises code action capabilities`` () =
    task {
        let! client = initializeLanguageServer None
        Assert.True(client.Capabilities.CodeActionProvider.HasValue)
    }
