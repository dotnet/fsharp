// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace FSharp.VisualStudio.Extension;

using System;
using System.Diagnostics;
using FSharp.Compiler.LanguageServer;
using Microsoft.VisualStudio.Telemetry;

/// <summary>
/// VS telemetry implementation for the F# LSP server.
/// Routes telemetry events through <see cref="TelemetryService.DefaultSession"/>.
/// </summary>
internal sealed class VsTelemetryReporter : ILspTelemetry
{
    private const string EventPrefix = "vs/fsharp/";
    private const string PropertyPrefix = "vs.fsharp.";

    public void ReportEvent(string name, Tuple<string, object>[] properties)
    {
        var telemetryEvent = new TelemetryEvent(EventPrefix + name, TelemetrySeverity.Normal);

        foreach (var prop in properties)
        {
            telemetryEvent.Properties[PropertyPrefix + prop.Item1] = prop.Item2;
        }

        TelemetryService.DefaultSession.PostEvent(telemetryEvent);
    }

    public IDisposable ReportEventWithDuration(string name, Tuple<string, object>[] properties)
    {
        var stopwatch = Stopwatch.StartNew();
        return new TelemetryScope(name, properties, stopwatch);
    }

    public void ReportFault(string name, string description, Microsoft.FSharp.Core.FSharpOption<Exception>? exn)
    {
        var faultName = EventPrefix + name;

        if (exn is not null && Microsoft.FSharp.Core.FSharpOption<Exception>.get_IsSome(exn))
        {
            TelemetryService.DefaultSession.PostFault(faultName, description, exn.Value);
        }
        else
        {
            TelemetryService.DefaultSession.PostFault(faultName, description);
        }
    }

    private sealed class TelemetryScope(string name, Tuple<string, object>[] properties, Stopwatch stopwatch) : IDisposable
    {
        public void Dispose()
        {
            stopwatch.Stop();

            var telemetryEvent = new TelemetryEvent(EventPrefix + name, TelemetrySeverity.Normal);

            foreach (var prop in properties)
            {
                telemetryEvent.Properties[PropertyPrefix + prop.Item1] = prop.Item2;
            }

            telemetryEvent.Properties[PropertyPrefix + "duration_ms"] = stopwatch.ElapsedMilliseconds;

            TelemetryService.DefaultSession.PostEvent(telemetryEvent);
        }
    }
}
