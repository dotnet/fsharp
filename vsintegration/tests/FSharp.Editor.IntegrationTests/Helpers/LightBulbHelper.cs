// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    // Reads code actions from the editor's OWN lightbulb session (triggered via the real ShowQuickFixes command),
    // exactly like the TypeScript-VS Apex tests and Roslyn's integration tests. We do NOT create a broker-owned
    // session: that gets superseded/dismissed by the editor's real lightbulb within ~20ms headless. The real
    // session is producer-agnostic (aggregates Roslyn today, the VS LSP CodeActionSource tomorrow).
    internal static class LightBulbHelper
    {
        // Overall budget for the whole "wait for fix, then read it" flow.
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(150);
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan s_activeWait = TimeSpan.FromSeconds(5);
        // Produce-gate polling: gently ask the broker whether a fix exists yet (no session churn).
        private static readonly TimeSpan s_produceGatePoll = TimeSpan.FromSeconds(1.5);
        // Even when the gate stays false, attempt a real lightbulb read this often. The broker's
        // HasSuggestedActions query is a false-negative for some F# fixes (e.g. parse-error "ErrorFix")
        // that ShowQuickFixes + the session DO surface. Spacing protects slow producers from churn.
        private static readonly TimeSpan s_fallbackReadInterval = TimeSpan.FromSeconds(15);
        // If no fix appears for this long, re-touch the buffer once (covers a touch that fired before
        // the project's checker was ready). Long enough not to cancel a slow in-flight analysis.
        private static readonly TimeSpan s_reTouchInterval = TimeSpan.FromSeconds(45);

        // PopulateWithDataAsync returns Task<ImmutableArray<...>> and ActionSets is ImmutableArray;
        // System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime, so we invoke/read
        // these via reflection through the non-generic IEnumerable and never name ImmutableArray in compiled IL.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // Separates PRODUCE from READ to avoid cancelling slow background analyzers. We poll a producer-agnostic
        // gate (broker.HasSuggestedActions) and read the real lightbulb when it reports a fix - this lets slow
        // analyzers (F# unused-opens needs the project's IncrementalBuilder + a full check) finish uninterrupted.
        // The gate is a false-negative for some fixes (F# parse-error "ErrorFix"), so we ALSO attempt a real read
        // on a slow cadence even when the gate is false; that spacing still protects slow producers from churn.
        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            Func<CancellationToken, Task<bool>> hasSuggestedActionsAsync,
            Func<CancellationToken, Task> triggerReanalysisAsync,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;

            System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] GetCodeActionsAsync starting, timeout={0}s", s_timeout.TotalSeconds);

            // Kick analysis once, then leave the buffer quiet so the (possibly slow) analyzer can complete.
            await triggerReanalysisAsync(cancellationToken);
            var lastTouch = DateTime.UtcNow;
            var lastFallbackRead = DateTime.UtcNow; // first fallback read after one full interval
            var touches = 1;
            var polls = 0;
            var reads = 0;
            var lastGate = false;
            var lastReadDetail = "no read attempted";

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions after {s_timeout.TotalSeconds:F0}s " +
                        $"(gate stayed {(lastGate ? "true" : "false")}; {polls} polls, {reads} reads, {touches} touches; last read: {lastReadDetail}).");
                }

                bool gate;
                try
                {
                    gate = await hasSuggestedActionsAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    gate = false;
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] HasSuggestedActions threw {0}: {1}", ex.GetType().Name, ex.Message);
                }

                polls++;
                lastGate = gate;

                // Read when the gate reports a fix (fast path for CodeFix), or periodically as a fallback for fixes
                // the gate can't see (ErrorFix). The fallback cadence keeps slow producers from being churned.
                if (gate || DateTime.UtcNow - lastFallbackRead >= s_fallbackReadInterval)
                {
                    reads++;
                    var (sets, detail) = await TryGetFromRealSessionAsync(
                        broker, view, joinableTaskFactory, showLightBulbAsync, cancellationToken);
                    lastFallbackRead = DateTime.UtcNow;
                    lastReadDetail = detail;

                    System.Diagnostics.Trace.TraceInformation(
                        "[LightBulbHelper] Read #{0} (gate={1}, {2:F1}s): sets={3}, detail={4}",
                        reads, gate, (DateTime.UtcNow - start).TotalSeconds, sets.Count, detail);

                    if (sets.Count > 0)
                    {
                        return sets;
                    }
                }

                // Bounded fallback: if nothing has appeared, the first touch may have fired before the checker was
                // ready - re-touch once, then keep the buffer quiet again for another full interval.
                if (DateTime.UtcNow - lastTouch > s_reTouchInterval)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Nothing yet; re-touching (touch #{0})", touches + 1);
                    await triggerReanalysisAsync(cancellationToken);
                    lastTouch = DateTime.UtcNow;
                    touches++;
                }

                await Task.Delay(s_produceGatePoll, cancellationToken);
            }
        }

        private static async Task<(IReadOnlyList<SuggestedActionSet> sets, string detail)> TryGetFromRealSessionAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            CancellationToken cancellationToken)
        {
            await joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] TryGetFromRealSession: isSessionActive={0}", broker.IsLightBulbSessionActive(view));

            if (broker.IsLightBulbSessionActive(view))
            {
                broker.DismissSession(view);
            }

            await showLightBulbAsync();

            var activeDeadline = DateTime.UtcNow + s_activeWait;
            while (!broker.IsLightBulbSessionActive(view))
            {
                if (DateTime.UtcNow > activeDeadline)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] TryGetFromRealSession: no session became active within {0}s", s_activeWait.TotalSeconds);
                    return (Array.Empty<SuggestedActionSet>(), "no active lightbulb session");
                }

                await Task.Delay(100, cancellationToken);
            }

            System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] TryGetFromRealSession: session is active");

            if (broker.GetSession(view) is not IAsyncLightBulbSession session)
            {
                System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] TryGetFromRealSession: GetSession returned non-async session");
                return (Array.Empty<SuggestedActionSet>(), "session active but GetSession not IAsyncLightBulbSession");
            }

            var eventTcs = new TaskCompletionSource<(IReadOnlyList<SuggestedActionSet> sets, QuerySuggestedActionCompletionStatus status)>();

            void OnUpdated(object sender, SuggestedActionsUpdatedArgs e)
            {
                if (e.Status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    return;
                }

                eventTcs.TrySetResult((ReadEnumerableProperty(e, "ActionSets"), e.Status));
            }

            void OnDismissed(object sender, EventArgs e)
                => eventTcs.TrySetException(new SessionDismissedException());

            session.SuggestedActionsUpdated += OnUpdated;
            session.Dismissed += OnDismissed;
            try
            {
                if (session.IsDismissed)
                {
                    return (Array.Empty<SuggestedActionSet>(), "session already dismissed");
                }

                // Ensure the session fires SuggestedActionsUpdated at least once with the latest computed data.
                string populateStatus;
                try
                {
                    var populateTask = (Task)s_populateWithDataAsync.Invoke(session, new object?[] { null, null })!;
                    populateTask.Forget();
                    populateStatus = "populate-invoked";
                }
                catch (Exception ex)
                {
                    populateStatus = $"populate-invoke-failed {ex.GetType().Name}: {ex.Message}";
                }

                using var perAttempt = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                perAttempt.CancelAfter(s_perAttemptTimeout);

                try
                {
                    var (sets, status) = await eventTcs.Task.WithCancellation(perAttempt.Token);
                    return (sets, $"{populateStatus}, event status={status}, sets={sets.Count}");
                }
                catch (SessionDismissedException)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"{populateStatus}, real session dismissed");
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"{populateStatus}, no terminal event within {s_perAttemptTimeout.TotalSeconds:F0}s");
                }
            }
            finally
            {
                session.SuggestedActionsUpdated -= OnUpdated;
                session.Dismissed -= OnDismissed;
            }
        }

        // Reads an ImmutableArray<SuggestedActionSet>-typed member through the non-generic IEnumerable
        // interface, never naming ImmutableArray (see class comment for why).
        private static IReadOnlyList<SuggestedActionSet> ReadEnumerableProperty(object source, string propertyName)
        {
            object? value;
            try
            {
                value = source.GetType().GetProperty(propertyName)?.GetValue(source);
            }
            catch
            {
                return Array.Empty<SuggestedActionSet>();
            }

            if (value is not IEnumerable sequence)
            {
                return Array.Empty<SuggestedActionSet>();
            }

            var list = new List<SuggestedActionSet>();
            try
            {
                foreach (var item in sequence)
                {
                    if (item is SuggestedActionSet set)
                    {
                        list.Add(set);
                    }
                }
            }
            catch
            {
                // A default(ImmutableArray) throws on enumeration; treat as no data.
                return Array.Empty<SuggestedActionSet>();
            }

            return list;
        }

        private sealed class SessionDismissedException : Exception
        {
        }
    }
}
