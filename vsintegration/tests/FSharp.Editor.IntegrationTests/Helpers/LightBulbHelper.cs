// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    internal static class LightBulbHelper
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan s_minRetriggerInterval = TimeSpan.FromMilliseconds(500);

        // We trigger ShowQuickFixes and then *poll* the session via TryGetSuggestedActionSets instead of
        // subscribing to events / awaiting PopulateWithDataAsync. Two reasons:
        //  * PopulateWithDataAsync returns Task<ImmutableArray<SuggestedActionSet>>; ImmutableArray binds
        //    System.Collections.Immutable, whose version skews between the NuGet reference and the in-proc
        //    VS runtime, so calling it throws MissingMethodException at runtime. TryGetSuggestedActionSets
        //    only exposes IEnumerable in its signature, so it is version-tolerant.
        //  * Subscribing after the session exists is racy (missed terminal event -> hang, or Dismissed ->
        //    cancel). Polling the session state observes the terminal result regardless of timing.
        public static async Task<IEnumerable<SuggestedActionSet>> WaitForItemsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            Action triggerCommand,
            CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow + s_timeout;
            var lastTrigger = DateTime.MinValue;
            IEnumerable<SuggestedActionSet> lastSets = Array.Empty<SuggestedActionSet>();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    return lastSets;
                }

                var session = broker.GetSession(view);
                if (session is null)
                {
                    // (Re)trigger only when there is no live session, so we never dismiss an in-flight
                    // one - re-posting over an InProgress session restarts the computation from scratch.
                    if (DateTime.UtcNow - lastTrigger > s_minRetriggerInterval)
                    {
                        triggerCommand();
                        lastTrigger = DateTime.UtcNow;
                    }

                    await Task.Delay(100, cancellationToken);
                    continue;
                }

#pragma warning disable CS0618 // version-tolerant accessor; see class comment for why we avoid PopulateWithDataAsync
                var status = session.TryGetSuggestedActionSets(out var sets);
#pragma warning restore CS0618
                sets ??= Array.Empty<SuggestedActionSet>();

                // Return only once the lightbulb has actually produced actions. A slow background
                // analyzer (e.g. unused-opens) can report Completed/CompletedWithoutData with no data on
                // an early query; in that case we dismiss and re-trigger until the fix shows up.
                if (status == QuerySuggestedActionCompletionStatus.Completed && sets.Any())
                {
                    return sets;
                }

                if (status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                lastSets = sets;
                try
                {
                    broker.DismissSession(view);
                }
                catch
                {
                    // best-effort
                }

                await Task.Delay(250, cancellationToken);
            }
        }
    }
}
