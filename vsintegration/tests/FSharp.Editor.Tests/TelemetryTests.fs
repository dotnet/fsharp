namespace FSharp.Editor.Tests

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.Telemetry
open Microsoft.VisualStudio.Threading
open StreamJsonRpc
open Xunit

type TelemetryTests() =
    [<Theory>]
    [<InlineData("old", "enabled")>]
    [<InlineData("lsp", "fallback_only")>]
    member _.``Legacy provider state describes its classification scope``(provider, expectedState) =
        use options = new EditorOptions()
        let events = ConcurrentQueue<TelemetryEvent>()

        let telemetry =
            SemanticHighlightingTelemetry(events.Enqueue, isMicrosoftInternal = (fun () -> true))

        FSharpClassificationService(provider, options, telemetry) |> ignore
        let state = Assert.Single(events)
        Assert.Equal(box "old", state.Properties["dotnet.fsharp.provider"])
        Assert.Equal(box provider, state.Properties["dotnet.fsharp.resolved_provider"])
        Assert.Equal(box expectedState, state.Properties["dotnet.fsharp.state"])

    [<Fact>]
    member _.``Closing the last view does not stop consent forwarding to a pending request``() =
        task {
            use context = new JoinableTaskContext()
            use options = new EditorOptions()
            let policies = ConcurrentQueue<HighlightingTelemetryConfiguration>()
            let published = ConcurrentQueue<TelemetryEvent>()

            let telemetry =
                SemanticHighlightingTelemetry(ignore, isMicrosoftInternal = (fun () -> false))

            let receiver =
                SemanticHighlightingTelemetry(published.Enqueue, isMicrosoftInternal = (fun () -> false))

            use forwarder =
                new LspTelemetryConsentForwarder(
                    telemetry,
                    options,
                    context,
                    (fun parameters _ ->
                        policies.Enqueue parameters

                        Task.FromResult(
                            {
                                Generation = parameters.Generation
                                SemanticTokensEnabled = true
                            }
                        ))
                )

            let closed = Event<EventHandler, EventArgs>()
            forwarder.RegisterView("lsp", "lsp", closed.Publish)
            do! forwarder.WaitForPendingRequestsAsync()
            Assert.True((policies.ToArray() |> Array.last).Enabled)

            let finish =
                TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

            let pending =
                task {
                    do! finish.Task
                    receiver.ReportDetails((fun () -> (policies.ToArray() |> Array.last).Enabled), [||])
                }

            closed.Trigger(null, EventArgs.Empty)

            options.With(
                { options.Advanced with
                    SendAdditionalTelemetry = false
                }
            )

            do! forwarder.WaitForPendingRequestsAsync()
            finish.SetResult()
            do! pending
            Assert.Empty(published)
            Assert.False((policies.ToArray() |> Array.last).Enabled)
        }

    [<Fact>]
    member _.``Disposing the consent service detaches settings and cancels its work``() =
        task {
            use context = new JoinableTaskContext()
            use options = new EditorOptions()

            let telemetry =
                SemanticHighlightingTelemetry(ignore, isMicrosoftInternal = (fun () -> false))

            let mutable calls = 0
            let mutable token = CancellationToken.None

            use forwarder =
                new LspTelemetryConsentForwarder(
                    telemetry,
                    options,
                    context,
                    (fun parameters cancellationToken ->
                        token <- cancellationToken
                        calls <- calls + 1

                        Task.FromResult(
                            {
                                Generation = parameters.Generation
                                SemanticTokensEnabled = true
                            }
                        ))
                )

            let closed = Event<EventHandler, EventArgs>()
            forwarder.RegisterView("lsp", "lsp", closed.Publish)
            do! forwarder.WaitForPendingRequestsAsync()
            (forwarder :> IDisposable).Dispose()

            options.With(
                { options.Advanced with
                    SendAdditionalTelemetry = false
                }
            )

            do! forwarder.WaitForPendingRequestsAsync()
            Assert.Equal(1, calls)
            Assert.True(token.IsCancellationRequested)
        }

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(true, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, true)>]
    member _.``All new telemetry requires the checkbox or a Microsoft internal user``(checkbox, internalUser) =
        let events = ConcurrentQueue<TelemetryEvent>()
        use options = new EditorOptions()

        options.With(
            { options.Advanced with
                SendAdditionalTelemetry = checkbox
                UseTransparentCompiler = true
            }
        )

        let enabled () =
            options.Advanced.SendAdditionalTelemetry

        let telemetry =
            SemanticHighlightingTelemetry(events.Enqueue, isMicrosoftInternal = (fun () -> internalUser))

        telemetry.ReportExposure("lsp", "lsp", enabled)
        telemetry.ReportExposure("lsp", "lsp", enabled)
        telemetry.ReportState("lsp", "ready", [||], enabled)
        telemetry.ReportState("lsp", "ready", [||], enabled)
        telemetry.ReportDetails(enabled, [||])
        Assert.Equal(checkbox || internalUser, (telemetry.Configure options).Enabled)
        Assert.Equal((if checkbox || internalUser then 3 else 0), events.Count)

    [<Fact>]
    member _.``Suppressed exposure and state do not consume their one time guards``() =
        let events = ConcurrentQueue<TelemetryEvent>()

        let telemetry =
            SemanticHighlightingTelemetry(events.Enqueue, isMicrosoftInternal = (fun () -> false))

        let mutable checkbox = false
        let enabled () = checkbox
        telemetry.ReportExposure("old", "old", enabled)
        telemetry.ReportState("old", "enabled", [||], enabled)
        Assert.Empty(events)
        checkbox <- true

        for _ in 1..2 do
            telemetry.ReportExposure("old", "old", enabled)
            telemetry.ReportState("old", "enabled", [||], enabled)

        Assert.Equal(2, events.Count)
        checkbox <- false
        telemetry.ReportState("lsp", "ready", [||], enabled)
        telemetry.ReportDetails(enabled, [||])
        Assert.Equal(2, events.Count)

    [<Fact>]
    member _.``Failed LSP startup stays in the rollout denominator``() =
        let events = ConcurrentQueue<TelemetryEvent>()

        for provider, ready in [ "old", false; "lsp", true; "lsp", false ] do
            let telemetry =
                SemanticHighlightingTelemetry(events.Enqueue, isMicrosoftInternal = (fun () -> false))

            telemetry.ReportExposure(provider, provider, (fun () -> true))
            telemetry.ReportExposure(provider, provider, (fun () -> true))

            if ready then
                telemetry.ReportState("lsp", "ready", [||], (fun () -> true))

        let exposures =
            events
            |> Seq.filter (fun e -> e.Name.EndsWith("/exposure", StringComparison.Ordinal))
            |> Seq.toArray

        Assert.Equal(3, exposures.Length)

        Assert.Equal(
            2,
            exposures
            |> Array.filter (fun e -> e.Properties["dotnet.fsharp.resolved_provider"] = box "lsp")
            |> Array.length
        )

        Assert.Single(
            events
            |> Seq.filter (fun e -> e.Name.EndsWith("/providerstate", StringComparison.Ordinal))
        )
        |> ignore

    [<Theory>]
    [<InlineData("old", "old", "old")>]
    [<InlineData("lsp", "lsp", "lsp")>]
    [<InlineData("unset", "unset", "old")>]
    [<InlineData("unknown", "unrecognized", "old")>]
    member _.``Exposure counts a session once regardless of views or details``(setting, expectedSetting, expectedProvider) =
        let events = ConcurrentQueue<TelemetryEvent>()

        let telemetry =
            SemanticHighlightingTelemetry(events.Enqueue, isMicrosoftInternal = (fun () -> false))

        let struct (settingValue, provider) =
            HighlightingTelemetry.normalizeProvider setting

        Parallel.For(0, 100, fun _ -> telemetry.ReportExposure(settingValue, provider, (fun () -> true)))
        |> ignore

        telemetry.ReportDetails((fun () -> false), [||])
        let exposure = Assert.Single(events)
        Assert.Equal(box expectedSetting, exposure.Properties["dotnet.fsharp.setting_value"])
        Assert.Equal(box expectedProvider, exposure.Properties["dotnet.fsharp.resolved_provider"])

        Assert.True(
            Guid.TryParse(string exposure.Properties["dotnet.fsharp.rollout_session_id"])
            |> fst
        )

    [<Fact>]
    member _.``Highlighting details honor opt out sampling and publisher failures``() =
        let events = ConcurrentQueue<TelemetryEvent>()
        let mutable now = DateTime.UtcNow

        let telemetry =
            SemanticHighlightingTelemetry(events.Enqueue, (fun () -> now), isMicrosoftInternal = (fun () -> false))

        telemetry.ReportDetails((fun () -> false), [||])
        telemetry.ReportDetails((fun () -> true), [||])
        telemetry.ReportDetails((fun () -> true), [||])
        Assert.Single(events) |> ignore
        now <- now.AddSeconds(5.)
        telemetry.ReportDetails((fun () -> true), [||])
        Assert.Equal(2, events.Count)

        let failing =
            SemanticHighlightingTelemetry((fun _ -> failwith "Publisher failed"), isMicrosoftInternal = (fun () -> false))

        failing.ReportExposure("lsp", "lsp", (fun () -> true))
        failing.ReportDetails((fun () -> true), [||])
        failing.ReportDetails((fun () -> failwith "Settings unavailable"), [||])

    [<Fact>]
    member _.``Settings updates change the policy without compiler overrides``() =
        use options = new EditorOptions()

        let telemetry =
            SemanticHighlightingTelemetry(ignore, isMicrosoftInternal = (fun () -> false))

        let mutable changed = false
        use _subscription = options.SettingsChanged.Subscribe(fun () -> changed <- true)

        options.With(
            { options.Advanced with
                SendAdditionalTelemetry = false
                UseTransparentCompiler = true
            }
        )

        Assert.True(changed)
        Assert.False((telemetry.Configure options).Enabled)

        options.With(
            { options.Advanced with
                SendAdditionalTelemetry = true
            }
        )

        Assert.True((telemetry.Configure options).Enabled)

    [<Theory>]
    [<InlineData("null")>]
    [<InlineData("timeout")>]
    [<InlineData("disconnect")>]
    [<InlineData("disposed")>]
    [<InlineData("cancelled")>]
    [<InlineData("throw")>]
    member _.``Startup retries read the latest consent until the server is ready``(firstResponse: string) =
        task {
            let events = ConcurrentQueue<TelemetryEvent>()

            let telemetry =
                SemanticHighlightingTelemetry(
                    events.Enqueue,
                    delay = (fun _ -> Task.CompletedTask),
                    isMicrosoftInternal = (fun () -> false)
                )

            use options = new EditorOptions()
            let mutable calls = 0

            do!
                telemetry.ConfigureServer(
                    options,
                    (fun parameters ->
                        calls <- calls + 1

                        if calls = 1 then
                            options.With(
                                { options.Advanced with
                                    SendAdditionalTelemetry = false
                                }
                            )

                            match firstResponse with
                            | "null" -> Task.FromResult(Unchecked.defaultof<_>)
                            | "timeout" -> Task.FromException<_>(TimeoutException())
                            | "disconnect" -> Task.FromException<_>(ConnectionLostException())
                            | "disposed" -> Task.FromException<_>(ObjectDisposedException("rpc"))
                            | "cancelled" -> Task.FromException<_>(OperationCanceledException())
                            | "throw" -> raise (TimeoutException())
                            | _ -> failwith "Unknown startup response"
                        else
                            Assert.False(parameters.Enabled)

                            Task.FromResult(
                                {
                                    Generation = parameters.Generation
                                    SemanticTokensEnabled = true
                                }
                            )),
                    true,
                    CancellationToken.None
                )

            Assert.Equal(2, calls)
            Assert.Empty(events)
        }

    [<Theory>]
    [<InlineData(true, true, 7)>]
    [<InlineData(false, true, 1)>]
    [<InlineData(true, false, 1)>]
    member _.``Startup failures respect the retry budget and permanent failures stop``(retry, transient, expectedCalls) =
        task {
            let events = ConcurrentQueue<TelemetryEvent>()

            let telemetry =
                SemanticHighlightingTelemetry(
                    events.Enqueue,
                    delay = (fun _ -> Task.CompletedTask),
                    isMicrosoftInternal = (fun () -> false)
                )

            let mutable calls = 0

            do!
                telemetry.ConfigureServer(
                    new EditorOptions(),
                    (fun _ ->
                        calls <- calls + 1

                        let error: exn =
                            if transient then
                                TimeoutException()
                            else
                                InvalidOperationException()

                        Task.FromException<_>(error)),
                    retry,
                    CancellationToken.None
                )

            Assert.Equal(expectedCalls, calls)
            Assert.Equal(box "unavailable", (Assert.Single(events)).Properties["dotnet.fsharp.state"])
        }

    [<Fact>]
    member _.``Consent forwarding needs no selected provider and cancelled startup does not retry``() =
        task {
            use cancellation = new CancellationTokenSource()
            let events = ConcurrentQueue<TelemetryEvent>()

            let telemetry =
                SemanticHighlightingTelemetry(
                    events.Enqueue,
                    delay =
                        (fun _ ->
                            cancellation.Cancel()
                            Task.FromCanceled(cancellation.Token)),
                    isMicrosoftInternal = (fun () -> false)
                )

            use options = new EditorOptions()
            let mutable sent = false

            do!
                telemetry.ConfigureServer(
                    options,
                    (fun _ ->
                        sent <- true
                        Task.FromResult(Unchecked.defaultof<_>)),
                    true,
                    cancellation.Token
                )

            Assert.False(sent)
            Assert.Empty(events)

            options.With(
                { options.Advanced with
                    SendAdditionalTelemetry = false
                }
            )

            let direct =
                SemanticHighlightingTelemetry(ignore, isMicrosoftInternal = (fun () -> false))

            do!
                direct.ConfigureServer(
                    options,
                    (fun parameters ->
                        sent <- true
                        Assert.False(parameters.Enabled)

                        Task.FromResult(
                            {
                                Generation = parameters.Generation
                                SemanticTokensEnabled = true
                            }
                        )),
                    false,
                    CancellationToken.None
                )

            Assert.True(sent)
        }
