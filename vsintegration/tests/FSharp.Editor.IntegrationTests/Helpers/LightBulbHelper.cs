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
        // Once a fix is known to be offered, how long we keep (re-)invoking the lightbulb to read it.
        private static readonly TimeSpan s_readBudget = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan s_activeWait = TimeSpan.FromSeconds(5);
        // Produce-gate polling: gently ask the broker whether a fix exists yet (no session churn).
        private static readonly TimeSpan s_produceGatePoll = TimeSpan.FromSeconds(1.5);
        // If the gate stays false this long, re-touch the buffer once (covers a touch that fired before
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

        // Separates PRODUCE from READ. First wait (deterministically, without churning the lightbulb) until the
        // broker reports a fix is offered at the caret - this gives slow background analyzers (F# unused-opens needs
        // the project's IncrementalBuilder + a full check) an uninterrupted window. Only THEN invoke the real
        // lightbulb to read the action sets; repeatedly dismissing/re-posting a session cancels the slow query, so
        // we never do that until the fix is known to exist.
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
            var touches = 1;

            // PRODUCE-GATE: poll the broker (no session create/dismiss, no re-touch) until a fix is offered.
            var pollCount = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool offered;
                try
                {
                    offered = await hasSuggestedActionsAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    offered = false;
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] HasSuggestedActions threw {0}: {1}", ex.GetType().Name, ex.Message);
                }

                pollCount++;
                if (offered)
                {
                    System.Diagnostics.Trace.TraceInformation(
                        "[LightBulbHelper] Fix offered after {0:F1}s ({1} polls, {2} touches); invoking lightbulb",
                        (DateTime.UtcNow - start).TotalSeconds, pollCount, touches);
                    break;
                }

                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions offered after {s_timeout.TotalSeconds:F0}s " +
                        $"(HasSuggestedActions stayed false; {pollCount} polls, {touches} touches).");
                }

                // Bounded fallback: if the gate is stuck, the first touch may have fired before the checker was
                // ready - re-touch once, then keep the buffer quiet again for another full interval.
                if (DateTime.UtcNow - lastTouch > s_reTouchInterval)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Gate stuck; re-touching (touch #{0})", touches + 1);
                    await triggerReanalysisAsync(cancellationToken);
                    lastTouch = DateTime.UtcNow;
                    touches++;
                }

                await Task.Delay(s_produceGatePoll, cancellationToken);
            }

            // READ: the fix is confirmed offered, so the slow analysis is done - now it's safe to (re-)invoke the
            // real lightbulb and read its session. Any session churn here can no longer cancel the producer.
            var readDeadline = DateTime.UtcNow + s_readBudget;
            var attempt = 0;
            var lastDetail = "no read attempt completed";
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > readDeadline)
                {
                    throw new InvalidOperationException(
                        $"A fix was offered at the caret but the lightbulb session could not be read within " +
                        $"{s_readBudget.TotalSeconds:F0}s ({attempt} attempts). Last: {lastDetail}");
                }

                attempt++;
                var (sets, detail) = await TryGetFromRealSessionAsync(
                    broker, view, joinableTaskFactory, showLightBulbAsync, cancellationToken);
                lastDetail = $"read attempt {attempt}: {detail}";

                System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Read attempt {0}: sets={1}, detail={2}", attempt, sets.Count, detail);

                if (sets.Count > 0)
                {
                    return sets;
                }

                await Task.Delay(250, cancellationToken);
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
