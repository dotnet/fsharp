// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace FSharp.VisualStudio.Extension;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Compiler.LanguageServer;
using FSharp.Compiler.LanguageServer.Common;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Telemetry;
using StreamJsonRpc;

internal sealed record TelemetryConfiguration(string RolloutSessionId, long Generation, bool Enabled);
internal sealed record TelemetryConfigurationResult(long Generation, bool SemanticTokensEnabled);

internal sealed class VsTelemetryReporter(
    Action<TelemetryEvent>? postEvent = null,
    TimeProvider? timeProvider = null,
    Action<TelemetryConfiguration>? configurationChanged = null,
    Func<bool>? isMicrosoftInternal = null) : ILspTelemetry,
    IRequestHandler<TelemetryConfiguration, TelemetryConfigurationResult, FSharpRequestContext>
{
    private const string EventPrefix = "vs/fsharp/";
    private const string PropertyPrefix = "vs.fsharp.";
    private readonly Action<TelemetryEvent> postEvent = postEvent ?? TelemetryService.DefaultSession.PostEvent;
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;
    private readonly Func<bool> isMicrosoftInternal = isMicrosoftInternal ?? (() => TelemetryService.DefaultSession.IsUserMicrosoftInternal);
    private readonly object gate = new();
    private readonly Dictionary<string, long> samples = new(StringComparer.Ordinal);
    private readonly HashSet<(string, Type?, string?)> faults = [];
    private readonly HashSet<string> lifecycleEvents = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Tuple<string, object>[]> pendingLifecycle = new(StringComparer.Ordinal);
    private TelemetryConfiguration? configuration;
    private bool closed;
    private bool CanReport => !this.closed && (this.isMicrosoftInternal() || this.configuration is { Enabled: true });

    public bool MutatesSolutionState => false;

    internal void Register(IServiceCollection services)
    {
        services.AddSingleton<ILspTelemetry>(this);
        services.AddSingleton<IMethodHandler>(this);
    }

    [LanguageServerEndpoint("fsharp/telemetry/configure", LanguageServerConstants.DefaultLanguageName)]
    public Task<TelemetryConfigurationResult> HandleRequestAsync(
        TelemetryConfiguration request, FSharpRequestContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var generation = Configure(request);
        var enabled = context.LspServices.GetRequiredService<FSharpLanguageServerConfig>().EnabledFeatures.SemanticTokens;
        return Task.FromResult(new TelemetryConfigurationResult(generation, enabled));
    }

    internal long Configure(TelemetryConfiguration? request)
    {
        lock (this.gate)
        {
            if (this.closed)
            {
                return -1;
            }

            if (request is null || !Guid.TryParse(request.RolloutSessionId, out _) || request.Generation < 0)
            {
                if (this.configuration is not null)
                {
                    this.configuration = this.configuration with { Enabled = false };
                    configurationChanged?.Invoke(this.configuration);
                }

                return -1;
            }

            if (this.configuration is not null && request.RolloutSessionId != this.configuration.RolloutSessionId)
            {
                return -1;
            }

            if (this.configuration is null ||
                (request.RolloutSessionId == this.configuration.RolloutSessionId &&
                 request.Generation > this.configuration.Generation))
            {
                this.configuration = request;
                configurationChanged?.Invoke(request);
            }

            if (this.CanReport)
            {
                foreach (var (name, properties) in this.pendingLifecycle)
                {
                    if (this.lifecycleEvents.Add(name))
                    {
                        this.Post(new TelemetryEvent(EventPrefix + name), properties);
                    }
                }
            }

            this.pendingLifecycle.Clear();
            return this.configuration.Generation;
        }
    }

    internal void Disable()
    {
        lock (this.gate)
        {
            this.configuration = null;
            this.pendingLifecycle.Clear();
            this.closed = true;
        }
    }

    internal void ReportLifecycle(string name, Tuple<string, object>[] properties)
    {
        lock (this.gate)
        {
            if (!this.closed && !this.lifecycleEvents.Contains(name))
            {
                if (this.CanReport)
                {
                    this.lifecycleEvents.Add(name);
                    this.Post(new TelemetryEvent(EventPrefix + name), properties);
                }
                else if (this.configuration is null)
                {
                    this.pendingLifecycle[name] = properties;
                }
            }
        }
    }

    public void ReportEvent(string name, Tuple<string, object>[] properties)
    {
        lock (this.gate)
        {
            if (!this.CanReport)
            {
                return;
            }

            if (name == TelemetryEvents.GetSemanticTokens)
            {
                var kind = Array.Find(properties, p => p.Item1 == "request_kind")?.Item2 as string;
                var key = name + "/" + kind;
                var now = this.timeProvider.GetTimestamp();
                if (this.samples.TryGetValue(key, out var previous) &&
                    this.timeProvider.GetElapsedTime(previous, now) < TimeSpan.FromSeconds(5))
                {
                    return;
                }

                this.samples[key] = now;
            }

            this.Post(new TelemetryEvent(EventPrefix + name, TelemetrySeverity.Normal), properties);
        }
    }

    public IDisposable ReportEventWithDuration(string name, Tuple<string, object>[] properties)
    {
        return new TelemetryScope(this, name, properties);
    }

    public void ReportFault(string name, string description, Microsoft.FSharp.Core.FSharpOption<Exception>? exn)
    {
        var error = exn?.Value;
        if (error is OperationCanceledException or ConnectionLostException { InnerException: OperationCanceledException })
        {
            return;
        }

        lock (this.gate)
        {
            if (this.CanReport &&
                this.faults.Count < 32 &&
                this.faults.Add((name, error?.GetType(), error?.StackTrace)))
            {
                this.Post(new FaultEvent(EventPrefix + name, description, error), []);
            }
        }
    }

    private void Post(TelemetryEvent telemetryEvent, Tuple<string, object>[] properties)
    {
        try
        {
            foreach (var prop in properties)
            {
                telemetryEvent.Properties[PropertyPrefix + prop.Item1] = prop.Item2;
            }

            telemetryEvent.Properties[PropertyPrefix + "schema_version"] = 1;
            telemetryEvent.Properties[PropertyPrefix + "provider"] = "lsp";
            if (this.configuration is not null)
            {
                telemetryEvent.Properties[PropertyPrefix + "rollout_session_id"] = this.configuration.RolloutSessionId;
            }

            this.postEvent(telemetryEvent);
        }
        catch (Exception)
        {
            Trace.TraceWarning("F# LSP telemetry could not be posted.");
        }
    }

    private sealed class TelemetryScope(
        VsTelemetryReporter reporter, string name, Tuple<string, object>[] properties) : IDisposable
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public void Dispose()
        {
            this.stopwatch.Stop();
            reporter.ReportEvent(name, [.. properties, Tuple.Create("duration_ms", (object)this.stopwatch.ElapsedMilliseconds)]);
        }
    }
}
