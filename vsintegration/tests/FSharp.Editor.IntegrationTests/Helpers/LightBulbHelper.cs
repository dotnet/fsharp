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
        // Overall budget for the read loop.
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(120);
        // Once a session becomes active, how long to wait for its terminal SuggestedActionsUpdated event.
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(15);
        // How long to wait for a session to become active after invoking ShowQuickFixes. Kept short so a
        // no-session attempt returns quickly and we can re-invoke - the broken-syntax (ErrorFix) lightbulb
        // session is unstable and dismisses fast, so frequent re-invocation is how we catch it.
        private static readonly TimeSpan s_activeWait = TimeSpan.FromSeconds(3);
        // Delay between aggressive read attempts.
        private static readonly TimeSpan s_readPoll = TimeSpan.FromSeconds(0.5);
        // While no diagnostic has been produced yet, re-touch the buffer this often to nudge production.
        private static readonly TimeSpan s_reTouchInterval = TimeSpan.FromSeconds(30);

        // PopulateWithDataAsync returns Task<ImmutableArray<...>> and ActionSets is ImmutableArray;
        // System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime, so we invoke/read
        // these via reflection through the non-generic IEnumerable and never name ImmutableArray in compiled IL.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // Aggressively invokes the real editor lightbulb (ShowQuickFixes) and reads its session every ~0.5s. The
        // F# fixes we test are on-demand compiler diagnostics, and the parse-error (ErrorFix) lightbulb session is
        // unstable headless - it appears then dismisses within seconds - so frequent re-invocation is what catches
        // it (a sparse cadence misses the flickering session, the failure mode seen on CI). broker.HasSuggestedActions
        // is a proven false-negative for the ErrorFix, so we don't gate on it; instead we use the Error List (the
        // F# diagnostic is error/warning severity and reliably appears there) as a produce signal for re-touch and
        // diagnostics. The periodic re-touch nudges production but can disrupt the slow unused-opens check, so
        // that test remains flaky.
        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            Func<CancellationToken, Task<int>> getDiagnosticCountAsync,
            Func<CancellationToken, Task> triggerReanalysisAsync,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;

            System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] GetCodeActionsAsync starting, timeout={0}s", s_timeout.TotalSeconds);

            // Kick analysis once up front.
            await triggerReanalysisAsync(cancellationToken);
            var lastTouch = DateTime.UtcNow;
            var touches = 1;
            var reads = 0;
            var lastDetail = "no read attempted";
            var lastDiagCount = -1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions after {s_timeout.TotalSeconds:F0}s " +
                        $"({reads} reads, {touches} touches, lastDiagCount={lastDiagCount}; last read: {lastDetail}).");
                }

                // Error List produce signal (errors + warnings). Deterministic for the F# compiler-diagnostic fixes.
                try
                {
                    lastDiagCount = await getDiagnosticCountAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    lastDiagCount = -1;
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] diagnostic count threw {0}: {1}", ex.GetType().Name, ex.Message);
                }

                // Aggressive read: invoke the real lightbulb and try to read its session.
                reads++;
                var (sets, detail) = await TryGetFromRealSessionAsync(
                    broker, view, joinableTaskFactory, showLightBulbAsync, cancellationToken);
                lastDetail = detail;

                System.Diagnostics.Trace.TraceInformation(
                    "[LightBulbHelper] Read #{0} ({1:F1}s, diagCount={2}): sets={3}, detail={4}",
                    reads, (DateTime.UtcNow - start).TotalSeconds, lastDiagCount, sets.Count, detail);

                if (sets.Count > 0)
                {
                    return sets;
                }

                // Re-touch only while no diagnostic has been produced yet (helps the "diagnostic never computed"
                // case); once diagnostics exist, leave the buffer alone so reads aren't disrupted.
                if (lastDiagCount <= 0 && DateTime.UtcNow - lastTouch > s_reTouchInterval)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] No diagnostic yet; re-touching (touch #{0})", touches + 1);
                    await triggerReanalysisAsync(cancellationToken);
                    lastTouch = DateTime.UtcNow;
                    touches++;
                }

                await Task.Delay(s_readPoll, cancellationToken);
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
