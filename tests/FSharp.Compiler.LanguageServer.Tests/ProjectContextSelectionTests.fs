module LanguageServer.ProjectContextSelectionTests

open System
open Xunit
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.LanguageServer
open LanguageServer.ProtocolHelpers

#nowarn "57"

[<Fact>]
let ``makeProjectContextId and tryParseProjectContextId round-trip`` () =
    let identifier = FSharpProjectIdentifier(@"C:\repo\proj.fsproj", @"C:\repo\bin\net8.0\proj.dll")
    let id = Utils.makeProjectContextId identifier
    Assert.Equal(Some identifier, Utils.tryParseProjectContextId id)

[<Fact>]
let ``makeProjectContextId distinguishes TFMs that share the project file`` () =
    let net8 = FSharpProjectIdentifier(@"C:\repo\proj.fsproj", @"C:\repo\bin\net8.0\proj.dll")
    let net9 = FSharpProjectIdentifier(@"C:\repo\proj.fsproj", @"C:\repo\bin\net9.0\proj.dll")
    Assert.NotEqual<string>(Utils.makeProjectContextId net8, Utils.makeProjectContextId net9)

[<Theory>]
[<InlineData("")>]
[<InlineData(null)>]
[<InlineData("garbage")>]
[<InlineData("guid|onlytwo")>]
let ``tryParseProjectContextId returns None for malformed input`` (contextId: string) =
    Assert.Equal(None, Utils.tryParseProjectContextId contextId)

[<Fact>]
let ``tryGetProjectContext returns None for plain TextDocumentIdentifier`` () =
    let doc = TextDocumentIdentifier(Uri = Uri("file:///c:/foo.fs"))
    Assert.Equal(None, Utils.tryGetProjectContext doc)

[<Fact>]
let ``tryGetProjectContext returns None when VS ProjectContext is null`` () =
    let doc = VSTextDocumentIdentifier(Uri = Uri("file:///c:/foo.fs"))
    Assert.Equal(None, Utils.tryGetProjectContext doc)

[<Fact>]
let ``tryGetProjectContext extracts identifier from VS ProjectContext`` () =
    let identifier = FSharpProjectIdentifier(@"C:\repo\proj.fsproj", @"C:\repo\bin\net8.0\proj.dll")

    let doc =
        VSTextDocumentIdentifier(
            Uri = Uri("file:///c:/foo.fs"),
            ProjectContext = VSProjectContext(Id = Utils.makeProjectContextId identifier, Label = "proj", Kind = VSProjectKind.FSharp)
        )

    Assert.Equal(Some identifier, Utils.tryGetProjectContext doc)

[<Fact>]
let ``GetProjectContexts returns distinct ids for multi-target-like projects`` () =
    task {
        let! client = initializeLanguageServer None
        let content = "module M\nlet x = 1"
        let fileOnDisk, _, _ = setupMultiTargetLikeProject client content "multitarget.fsproj" "TFM_A" "TFM_B"
        do! openDocument client fileOnDisk content 1

        let! result = getProjectContexts client fileOnDisk
        Assert.Equal(2, result.ProjectContexts.Length)
        let ids = result.ProjectContexts |> Array.map (fun c -> c.Id) |> Array.distinct
        Assert.Equal(2, ids.Length)
    }

let private multiTargetSource =
    """module M
#if TFM_A
let a: int = "not an int"
#else
let b: string = 1
#endif
"""

let private diagnosticStartLines (report: RelatedFullDocumentDiagnosticReport) =
    report.Items |> Array.map (fun diagnostic -> diagnostic.Range.Start.Line)

[<Fact>]
let ``Diagnostics differ when client selects different project context`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk, pidA, pidB = setupMultiTargetLikeProject client multiTargetSource "multitarget.fsproj" "TFM_A" "TFM_B"
        do! openDocument client fileOnDisk multiTargetSource 1

        let ctxA = VSProjectContext(Id = Utils.makeProjectContextId pidA, Label = "multitarget", Kind = VSProjectKind.FSharp)
        let ctxB = VSProjectContext(Id = Utils.makeProjectContextId pidB, Label = "multitarget", Kind = VSProjectKind.FSharp)
        let! reportA = pullDiagnosticsInContext client fileOnDisk ctxA
        let! reportB = pullDiagnosticsInContext client fileOnDisk ctxB

        let linesA = diagnosticStartLines reportA
        let linesB = diagnosticStartLines reportB
        Assert.Contains(2, linesA)
        Assert.DoesNotContain(4, linesA)
        Assert.Contains(4, linesB)
        Assert.DoesNotContain(2, linesB)
    }

[<Fact>]
let ``Diagnostics with unknown project context fall back to first project`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk, _, _ = setupMultiTargetLikeProject client multiTargetSource "multitarget.fsproj" "TFM_A" "TFM_B"
        do! openDocument client fileOnDisk multiTargetSource 1

        let bogusContext =
            VSProjectContext(
                Id = Utils.makeProjectContextId (FSharpProjectIdentifier("ghost.fsproj", "ghost.dll")),
                Label = "ghost",
                Kind = VSProjectKind.FSharp
            )

        let! defaultReport = pullDiagnostics client fileOnDisk
        let! report = pullDiagnosticsInContext client fileOnDisk bogusContext
        Assert.NotEmpty(defaultReport.Items)
        Assert.Equal<int>(diagnosticStartLines defaultReport, diagnosticStartLines report)
    }
