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
    internal static class LightBulbHelper
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(30);

        // We drive the producer-agnostic VS lightbulb broker session rather than querying a specific code-fix
        // source. The broker session aggregates ALL suggested-action sources (Roslyn today, the VS LSP client's
        // CodeActionSource once F# code actions move to LSP), so this test stays valid across that migration -
        // unlike calling Roslyn's IAsyncSuggestedActionsSource directly.
        //
        // Mechanics learned the hard way:
        //  * ShowQuickFixes only creates a session when a fix already exists at the caret, so a background/push
        //    analyzer (unused-opens) whose diagnostic isn't published yet never produces one. broker.CreateSession
        //    always gives us an owned session and still aggregates every source.
        //  * PopulateWithDataAsync returns a fast, EMPTY initial snapshot; the real aggregated sets arrive later
        //    via SuggestedActionsUpdated as each source completes. So we trigger populate but wait on the terminal
        //    event, not the populate task result.
        //  * PopulateWithDataAsync returns Task<ImmutableArray<...>> and SuggestedActionsUpdatedArgs.ActionSets is
        //    ImmutableArray; System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime,
        //    so we invoke/read these via reflection through the non-generic IEnumerable and never name
        //    ImmutableArray in compiled IL. (These are VS platform APIs, unaffected by the F# LSP move.)
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            ISuggestedActionCategorySet categories,
            JoinableTaskFactory joinableTaskFactory,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;
            var attempt = 0;
            var lastDetail = "no attempt completed";

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions found after {s_timeout.TotalSeconds:F0}s ({attempt} attempts). Last: {lastDetail}");
                }

                attempt++;
                var (sets, detail) = await TryPopulateViaSessionAsync(
                    broker, view, categories, joinableTaskFactory, cancellationToken);
                lastDetail = $"attempt {attempt}, elapsed {(DateTime.UtcNow - start).TotalSeconds:F1}s: {detail}";

                if (sets.Count > 0)
                {
                    return sets;
                }

                await Task.Delay(250, cancellationToken);
            }
        }

        private static async Task<(IReadOnlyList<SuggestedActionSet> sets, string detail)> TryPopulateViaSessionAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            ISuggestedActionCategorySet categories,
            JoinableTaskFactory joinableTaskFactory,
            CancellationToken cancellationToken)
        {
            await joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

#pragma warning disable CS0618 // ILightBulbBroker2.CreateSession's extra category-set param can't be validated for these tests locally
            var session = (IAsyncLightBulbSession)broker.CreateSession(categories, view);
#pragma warning restore CS0618

            // Result carries both the action sets and the terminal status (for diagnostics).
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
                // Expand the session so it survives the async computation. A collapsed/owned session
                // auto-dismisses the instant PopulateWithDataAsync reports its empty initial snapshot, which
                // loses the race for fixes that compute slower than that (type-check warnings, background
                // analyzers); an expanded session persists (the "computing..." lightbulb) and updates as the
                // sources complete - exactly like a real Ctrl+. invocation.
                string expandStatus;
                try
                {
                    session.Expand();
                    expandStatus = "expanded";
                }
                catch (Exception ex)
                {
                    expandStatus = $"expand-failed {ex.GetType().Name}: {ex.Message}";
                }

                // Trigger population (fires SuggestedActionsUpdated at least once with the latest data). We don't
                // rely on its empty early result - the terminal event delivers the aggregated sets.
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
                    return (sets, $"{expandStatus}, {populateStatus}, event status={status}, sets={sets.Count}");
                }
                catch (SessionDismissedException)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"{expandStatus}, {populateStatus}, session dismissed");
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"{expandStatus}, {populateStatus}, no terminal event within {s_perAttemptTimeout.TotalSeconds:F0}s");
                }
            }
            finally
            {
                session.SuggestedActionsUpdated -= OnUpdated;
                session.Dismissed -= OnDismissed;
                try
                {
                    broker.DismissSession(view);
                }
                catch
                {
                    // best-effort cleanup of the session we created
                }
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
