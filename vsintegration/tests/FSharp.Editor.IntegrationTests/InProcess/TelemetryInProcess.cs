// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Threading;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Microsoft.VisualStudio.Extensibility.Testing;

[TestService]
internal partial class TelemetryInProcess
{
    internal async Task<TelemetryChannel> EnableTestTelemetryChannelAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        TelemetryService.DetachTestChannel(LoggerTestChannel.Instance);
        LoggerTestChannel.Instance.Clear();
        TelemetryService.AttachTestChannel(LoggerTestChannel.Instance);
        return new TelemetryChannel(TestServices);
    }

    internal async Task DisableTestTelemetryChannelAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        TelemetryService.DetachTestChannel(LoggerTestChannel.Instance);
        LoggerTestChannel.Instance.Clear();
    }

    public async Task<TelemetryEvent> TryWaitForTelemetryEventsAsync(string eventName, CancellationToken cancellationToken)
        => await LoggerTestChannel.Instance.TryWaitForEventsAsync(eventName, cancellationToken);

    public class TelemetryChannel : IAsyncDisposable
    {
        internal TestServices _testServices;

        public TelemetryChannel(TestServices testServices)
        {
            _testServices = testServices;
        }

        public async ValueTask DisposeAsync()
            => await _testServices.Telemetry.DisableTestTelemetryChannelAsync(CancellationToken.None);

        public async Task<TelemetryEvent> GetEventAsync(string eventName, CancellationToken cancellationToken) 
            => await _testServices.Telemetry.TryWaitForTelemetryEventsAsync(eventName, cancellationToken);
    }

    private sealed class LoggerTestChannel : ITelemetryTestChannel
    {
        public static readonly LoggerTestChannel Instance = new();

        private AsyncQueue<TelemetryEvent> _eventsQueue = new();

        public async Task<TelemetryEvent> TryWaitForEventsAsync(string eventName, CancellationToken cancellationToken)
        {
            while (true)
            {
                var result = await _eventsQueue.DequeueAsync(cancellationToken);
                if (result.Name == eventName)
                {
                    return result;
                }
            }
        }

        public void Clear()
        {
            _eventsQueue.Complete();
            _eventsQueue = new AsyncQueue<TelemetryEvent>();
        }

        void ITelemetryTestChannel.OnPostEvent(object sender, TelemetryTestChannelEventArgs e)
        {
            _eventsQueue.Enqueue(e.Event);
        }
    }
}
