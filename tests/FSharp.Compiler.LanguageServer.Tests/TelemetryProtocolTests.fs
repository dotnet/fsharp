module LanguageServer.TelemetryProtocolTests

open System
open System.Collections.Concurrent
open System.IO
open System.Threading.Tasks

open Microsoft.Extensions.DependencyInjection
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Handlers
open StreamJsonRpc
open Xunit

open LanguageServer.ProtocolHelpers

#nowarn "57"

type private RecordingTelemetry(?throwOnSemanticEvent: bool) =
    let events = ConcurrentQueue<string * (string * obj) array>()
    let faults = ConcurrentQueue<string>()
    member _.Events = events.ToArray()
    member _.Faults = faults.ToArray()

    interface ILspTelemetry with
        member _.ReportEvent(name, props) =
            if name = TelemetryEvents.GetSemanticTokens && defaultArg throwOnSemanticEvent false then
                failwith "Telemetry publisher failed"

            events.Enqueue(name, props)

        member _.ReportEventWithDuration(name, props) =
            NullTelemetry.Instance.ReportEventWithDuration(name, props)

        member _.ReportFault(name, _, _) = faults.Enqueue name

let private createClient (telemetry: RecordingTelemetry) =
    createLanguageServerWithServices (FSharpWorkspace()) None ignore (fun services ->
        services.AddSingleton<ILspTelemetry>(telemetry) |> ignore)

let private measurements (telemetry: RecordingTelemetry) =
    telemetry.Events
    |> Array.choose (fun (name, props) ->
        if name = TelemetryEvents.GetSemanticTokens then Some(Map.ofArray props) else None)

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Semantic request telemetry distinguishes semantic data from lexical only results`` rangeRequest inProject =
    task {
        let telemetry = RecordingTelemetry()
        let! client = createClient telemetry
        let content = "module M\nlet value = 1"
        let uri =
            if inProject then
                setupSingleFileProject client content
            else
                Uri(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".fs"))

        do! openDocument client uri content 1
        let! response =
            if rangeRequest then
                pullSemanticTokensRange client uri (Range(Start = Position(Line = 0), End = Position(Line = 1, Character = 13)))
            else
                pullSemanticTokensFull client uri

        Assert.NotEmpty(response.Data)
        let props = Assert.Single(measurements telemetry)
        Assert.Equal(box "completed", props["outcome"])
        Assert.Equal(box inProject, props["semantic_data_available"])
        Assert.Equal(box true, props["isOpenDocument"])
        Assert.Equal(box content.Length, props["document_length"])
        Assert.Equal(box (response.Data.Length / 5), props["token_count"])
        Assert.Equal(box (if rangeRequest then "range" else "full"), props["request_kind"])
        Assert.True(unbox<int64> props["provider_duration_ms"] >= unbox<int64> props["duration_ms"])
        Assert.False(props.ContainsKey "cacheHit")
    }

[<Fact>]
let ``No source and empty responses have distinct outcomes`` () =
    task {
        let telemetry = RecordingTelemetry()
        let! client = createClient telemetry
        let missing = Uri(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".fs"))
        let! noSource = pullSemanticTokensFull client missing
        Assert.Empty(noSource.Data)
        let empty = setupSingleFileProject client ""
        do! openDocument client empty "" 1
        let! emptyResponse = pullSemanticTokensFull client empty
        Assert.Empty(emptyResponse.Data)
        let events = measurements telemetry
        Assert.Equal(box "no_source", events[0]["outcome"])
        Assert.Equal(box "completed", events[1]["outcome"])
    }

[<Fact>]
let ``Open state follows document notifications and close`` () =
    task {
        let telemetry = RecordingTelemetry()
        let! client = createClient telemetry
        let uri = setupSingleFileProject client "module M\nlet x = 1"
        do! openDocument client uri "module M\nlet x = 1" 1
        let! _ = pullSemanticTokensFull client uri
        do! changeDocument client uri "module M\nlet x = 2" 2
        let! _ = pullSemanticTokensFull client uri
        do! closeDocument client uri
        let! _ = pullSemanticTokensFull client uri
        Assert.Equal<bool>([| true; true; false |], measurements telemetry |> Array.map (fun p -> unbox p["isOpenDocument"]))
    }

[<Fact>]
let ``A telemetry failure does not change semantic responses`` () =
    task {
        let! client = createClient (RecordingTelemetry(throwOnSemanticEvent = true))
        let content = "module M\nlet value = 1"
        let uri = setupSingleFileProject client content
        do! openDocument client uri content 1
        let! response = pullSemanticTokensFull client uri
        Assert.NotEmpty(response.Data)
    }

[<Fact>]
let ``Failed semantic requests still record their terminal outcome`` () =
    task {
        let telemetry = RecordingTelemetry()
        let! client = createClient telemetry
        let uri = setupSingleFileProject client "module M\nlet value = 1"
        File.Delete(uri.LocalPath)
        let! _ = Assert.ThrowsAsync<RemoteInvocationException>(fun () -> pullSemanticTokensFull client uri :> Task)
        Assert.Equal(box "faulted", (Assert.Single(measurements telemetry))["outcome"])
    }

[<Fact>]
let ``Handled project context refresh failures produce server fault telemetry`` () =
    task {
        let telemetry = RecordingTelemetry()
        let! client =
            createLanguageServerWithServices
                (FSharpWorkspace())
                None
                (fun rpc -> rpc.AddLocalRpcMethod("workspace/semanticTokens/refresh", Action(fun () -> failwith "Refresh failed")))
                (fun services -> services.AddSingleton<ILspTelemetry>(telemetry) |> ignore)

        let uri, first, _ = setupMultiTargetLikeProject client "module M\nlet value = 1" "multi.fsproj" "NET8" "NET9"
        let parameters: ActiveProjectContextParams =
            { Uri = uri; ProjectFileName = first.ProjectFileName; OutputFileName = first.OutputFileName }
        let! accepted = client.JsonRpc.InvokeWithParameterObjectAsync<bool>("textDocument/fsharp/setActiveProjectContext", parameters)
        Assert.False(accepted)
        Assert.Equal(TelemetryEvents.SetActiveProjectContext, Assert.Single(telemetry.Faults))
        let _, props = telemetry.Events |> Array.find (fun (name, _) -> name = TelemetryEvents.SetActiveProjectContext)
        Assert.Equal(box "refresh_failed", (Map.ofArray props)["outcome"])
    }
