namespace FSharp.Compiler.Service.Tests.HotReload

open System
open Xunit
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.HotReload

module RudeEditDiagnosticsTests =

    let private rude kind message =
        { Symbol = None
          Kind = kind
          Message = message }

    [<Fact>]
    let ``signature change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.SignatureChange "fallback")
        Assert.Equal("FSHRDL001", diag.Id)
        Assert.Contains("signature", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``inline change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.InlineChange "fallback")
        Assert.Equal("FSHRDL002", diag.Id)
        Assert.Contains("inline", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``type layout change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.TypeLayoutChange "fallback")
        Assert.Equal("FSHRDL003", diag.Id)
        Assert.Contains("representation", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``declaration added diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.DeclarationAdded "fallback")
        Assert.Equal("FSHRDL004", diag.Id)
        Assert.Contains("Adding", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``declaration removed diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.DeclarationRemoved "fallback")
        Assert.Equal("FSHRDL005", diag.Id)
        Assert.Contains("Removing", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``lambda shape change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.LambdaShapeChange "fallback")
        Assert.Equal("FSHRDL012", diag.Id)
        Assert.Contains("lambda", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``state machine shape change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.StateMachineShapeChange "fallback")
        Assert.Equal("FSHRDL013", diag.Id)
        Assert.Contains("state-machine", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``query expression shape change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.QueryExpressionShapeChange "fallback")
        Assert.Equal("FSHRDL014", diag.Id)
        Assert.Contains("query-expression", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``synthesized declaration change diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.SynthesizedDeclarationChange "fallback")
        Assert.Equal("FSHRDL015", diag.Id)
        Assert.Contains("synthesized", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``explicit interface insertion diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.InsertExplicitInterface "fallback")
        Assert.Equal("FSHRDL009", diag.Id)
        Assert.Contains("explicit interface", diag.Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``unsupported diagnostic id`` () =
        let diag = RudeEditDiagnostics.ofRudeEdit (rude RudeEditKind.Unsupported "custom")
        Assert.Equal("FSHRDL099", diag.Id)
        Assert.Equal("custom", diag.Message)
