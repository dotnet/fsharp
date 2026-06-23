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
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan s_activeWait = TimeSpan.FromSeconds(5);

        // PopulateWithDataAsync returns Task<ImmutableArray<...>> and ActionSets is ImmutableArray;
        // System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime, so we invoke/read
        // these via reflection through the non-generic IEnumerable and never name ImmutableArray in compiled IL.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            Func<CancellationToken, Task> drainLightBulbOperationsAsync,
            Func<CancellationToken, Task> triggerReanalysisAsync,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;
            var attempt = 0;
            var lastDetail = "no attempt completed";

            System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] GetCodeActionsAsync starting, timeout={0}s", s_timeout.TotalSeconds);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions found after {s_timeout.TotalSeconds:F0}s ({attempt} attempts). Last: {lastDetail}");
                }

                attempt++;

                if (attempt % 2 == 1)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Attempt {0}: triggering reanalysis", attempt);
                    await triggerReanalysisAsync(cancellationToken);
                }

                var (sets, detail) = await TryGetFromRealSessionAsync(
                    broker, view, joinableTaskFactory, showLightBulbAsync, drainLightBulbOperationsAsync, cancellationToken);
                lastDetail = $"attempt {attempt}, elapsed {(DateTime.UtcNow - start).TotalSeconds:F1}s: {detail}";

                System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Attempt {0}: sets={1}, detail={2}", attempt, sets.Count, detail);

                if (sets.Count > 0)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] Success on attempt {0} with {1} action sets", attempt, sets.Count);
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
            Func<CancellationToken, Task> drainLightBulbOperationsAsync,
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

                // Best-effort deterministic drain (no-op if F# work isn't tracked by Roslyn's LightBulb listener).
                try
                {
                    await drainLightBulbOperationsAsync(cancellationToken);
                }
                catch
                {
                    // ignore - the terminal event below is the source of truth
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
