namespace FSharp.VisualStudio.Extension.Tests;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Compiler.CodeAnalysis.Workspace;
using FSharp.Compiler.LanguageServer;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Telemetry;
using StreamJsonRpc;
using Xunit;

public sealed class TelemetryTests
{
    private static readonly string SessionId = Guid.NewGuid().ToString("D");
    private static Tuple<string, object>[] Request(string kind = "full") => [Tuple.Create("request_kind", (object)kind)];

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void AllTelemetryRequiresTheCheckboxOrAMicrosoftInternalUser(bool enabled, bool internalUser)
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var reporter = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => internalUser);
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        Assert.Equal(internalUser ? 1 : 0, events.Count);
        reporter.Configure(new(SessionId, 1, enabled));
        reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        using (reporter.ReportEventWithDuration(TelemetryEvents.GetDiagnostics, [])) { }
        reporter.ReportFault("lsp/context", "Refresh failed.", FSharpOption<Exception>.Some(new InvalidOperationException()));
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        Assert.Equal(enabled || internalUser ? 4 : 0, events.Count);
        Assert.All(events.Where(e => !e.Name.EndsWith("/initialized", StringComparison.Ordinal)),
            e => Assert.Equal(SessionId, e.Properties["vs.fsharp.rollout_session_id"]));
    }

    [Fact]
    public void MicrosoftInternalUsersDoNotWaitForConfigurationButClosedConnectionsStop()
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var reporter = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => true);
        reporter.ReportEvent(TelemetryEvents.DocumentOpened, []);
        using (reporter.ReportEventWithDuration(TelemetryEvents.GetDiagnostics, [])) { }
        reporter.ReportFault("lsp/context", "Refresh failed.", FSharpOption<Exception>.Some(new InvalidOperationException()));
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        Assert.Equal(4, events.Count);
        reporter.Disable();
        reporter.ReportEvent(TelemetryEvents.DocumentClosed, []);
        reporter.ReportLifecycle("lsp/initializationfailed", []);
        reporter.ReportFault("lsp/closed", "Closed.", FSharpOption<Exception>.Some(new InvalidOperationException()));
        Assert.Equal(4, events.Count);
    }

    [Fact]
    public void LifecycleWaitsForPolicyAndOptOutDiscardsPendingSummaries()
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var reporter = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => false);
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        Assert.Empty(events);
        reporter.Configure(new(SessionId, 1, false));
        reporter.Configure(new(SessionId, 2, true));
        Assert.Empty(events);
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        reporter.ReportLifecycle(TelemetryEvents.LspServerInitialized, []);
        Assert.Single(events);
    }

    [Fact]
    public void StalePoliciesAndInflightScopesCannotUndoOptOut()
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var reporter = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => false);
        reporter.Configure(new(SessionId, 2, false));
        Assert.Equal(2, reporter.Configure(new(SessionId, 1, true)));
        reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        reporter.Configure(new(SessionId, 3, true));
        var scope = reporter.ReportEventWithDuration(TelemetryEvents.GetSemanticTokens, Request());
        reporter.Configure(new(SessionId, 4, false));
        scope.Dispose();
        Assert.Empty(events);
        reporter.Configure(new(SessionId, 5, true));
        reporter.Disable();
        Assert.Equal(-1, reporter.Configure(new(SessionId, 6, true)));
        reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        Assert.Empty(events);
    }

    [Fact]
    public void ReconnectedServersReuseOnlyTheHostLifetimePolicy()
    {
        TelemetryConfiguration? policy = null;
        var events = new ConcurrentBag<TelemetryEvent>();
        var first = new VsTelemetryReporter(events.Add, configurationChanged: value => policy = value, isMicrosoftInternal: () => false);
        first.Configure(new(SessionId, 1, true));
        first.Disable();
        var reconnected = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => false);
        reconnected.Configure(Assert.IsType<TelemetryConfiguration>(policy));
        reconnected.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        Assert.Single(events);
        reconnected.Configure(new("invalid", 2, true));
        Assert.Equal(1, reconnected.Configure(new(SessionId, 0, true)));
        reconnected.ReportEvent(TelemetryEvents.DocumentOpened, []);
        Assert.Single(events);
    }

    [Fact]
    public void SamplesAreBoundedPerRequestKindUnderConcurrency()
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var clock = new Clock();
        var reporter = new VsTelemetryReporter(events.Add, clock, isMicrosoftInternal: () => false);
        reporter.Configure(new(SessionId, 1, true));
        Parallel.For(0, 100, _ => reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request()));
        reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request("range"));
        Assert.Equal(2, events.Count);
        clock.Advance();
        reporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        Assert.Equal(3, events.Count);
        Assert.All(events, e => Assert.Equal(1, e.Properties["vs.fsharp.schema_version"]));
    }

    [Fact]
    public void FaultsAreGatedDeduplicatedAndDoNotIncludeCancellation()
    {
        var events = new ConcurrentBag<TelemetryEvent>();
        var reporter = new VsTelemetryReporter(events.Add, isMicrosoftInternal: () => false);
        var error = FSharpOption<Exception>.Some(new InvalidOperationException());
        reporter.ReportFault("lsp/context", "Refresh failed.", error);
        Assert.Empty(events);
        reporter.Configure(new(SessionId, 1, true));
        reporter.ReportFault("lsp/context", "Refresh failed.", error);
        reporter.ReportFault("lsp/context", "Refresh failed.", error);
        reporter.ReportFault("lsp/context", "Cancelled.", FSharpOption<Exception>.Some(new OperationCanceledException()));
        reporter.ReportFault("lsp/context", "Disconnected.", FSharpOption<Exception>.Some(
            new ConnectionLostException("Cancelled", new OperationCanceledException())));
        Assert.IsType<FaultEvent>(Assert.Single(events));
        var failingReporter = new VsTelemetryReporter(_ => throw new InvalidOperationException(), isMicrosoftInternal: () => false);
        failingReporter.Configure(new(SessionId, 1, true));
        failingReporter.ReportEvent(TelemetryEvents.GetSemanticTokens, Request());
        failingReporter.ReportFault("lsp/context", "Refresh failed.", error);
    }

    [Fact]
    public async Task ConfigurationUsesTheRealJsonRpcAndServiceRegistration()
    {
        var reporter = new VsTelemetryReporter(_ => { }, isMicrosoftInternal: () => false);
        var connection = FSharpLanguageServer.Create(new FSharpWorkspace(), reporter.Register);
        using var serverRpc = connection.Item2.JsonRpc;
        using var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(
            connection.Item1.Item1, connection.Item1.Item2, Stuff.createConfiguredFormatter()));
        rpc.StartListening();
        await rpc.InvokeWithParameterObjectAsync<InitializeResult>(
            "initialize", new InitializeParams { Capabilities = new ClientCapabilities() }, cancellationToken: TestContext.Current.CancellationToken);
        var result = await rpc.InvokeWithParameterObjectAsync<TelemetryConfigurationResult>(
            "fsharp/telemetry/configure", new TelemetryConfiguration(SessionId, 7, false), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(7, result.Generation);
        Assert.True(result.SemanticTokensEnabled);
    }

    private sealed class Clock : TimeProvider
    {
        private long ticks;
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => Interlocked.Read(ref this.ticks);
        public void Advance() => Interlocked.Add(ref this.ticks, TimeSpan.FromSeconds(5).Ticks);
    }
}
