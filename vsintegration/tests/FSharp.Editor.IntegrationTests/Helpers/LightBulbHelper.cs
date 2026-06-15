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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    internal static class LightBulbHelper
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan s_minRetriggerInterval = TimeSpan.FromMilliseconds(500);

        // The action SETS can only be obtained from the async lightbulb session: in modern Roslyn the
        // synchronous ISuggestedActionsSource.GetSuggestedActions returns null by design, and the real
        // producer is IAsyncSuggestedActionsSource.GetSuggestedActionsAsync, which the lightbulb session
        // drives via PopulateWithDataAsync. PopulateWithDataAsync returns Task<ImmutableArray<...>> and
        // SuggestedActionsUpdatedArgs.ActionSets is also ImmutableArray, both of which bind
        // System.Collections.Immutable - a version that skews between the test's NuGet reference and the
        // in-proc VS runtime - so we invoke/read them via reflection and never name ImmutableArray.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // Trigger ShowQuickFixes (creates the real, active, non-superseded session), then drive
        // PopulateWithDataAsync and read its aggregated result. Retry with a fresh session until items
        // appear (covers background analyzers such as unused-opens whose diagnostics arrive asynchronously)
        // or the overall timeout elapses, at which point we throw a per-attempt diagnostic dump (the CI log
        // is the only signal available).
        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            Action triggerCommand,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;
            var lastTrigger = DateTime.MinValue;
            var attempt = 0;
            var lastDetail = "no attempt completed";

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    throw new InvalidOperationException(
                        $"No code actions found after {s_timeout.TotalSeconds:F0}s ({attempt} attempts). " +
                        $"Last: {lastDetail}");
                }

                if (broker.GetSession(view) is not IAsyncLightBulbSession session)
                {
                    // (Re)trigger only when there is no live session, so we never dismiss an in-flight one.
                    if (DateTime.UtcNow - lastTrigger > s_minRetriggerInterval)
                    {
                        triggerCommand();
                        lastTrigger = DateTime.UtcNow;
                    }

                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                attempt++;
                var (sets, detail) = await TryPopulateAsync(session, cancellationToken);
                lastDetail = $"attempt {attempt}, elapsed {(DateTime.UtcNow - start).TotalSeconds:F1}s: {detail}";

                if (sets.Count > 0)
                {
                    return sets;
                }

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

        private static async Task<(IReadOnlyList<SuggestedActionSet> sets, string detail)> TryPopulateAsync(
            IAsyncLightBulbSession session,
            CancellationToken cancellationToken)
        {
            var eventTcs = new TaskCompletionSource<IReadOnlyList<SuggestedActionSet>>();

            void Handler(object sender, SuggestedActionsUpdatedArgs e)
            {
                if (e.Status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    return;
                }

                eventTcs.TrySetResult(ReadEnumerableProperty(e, "ActionSets"));
            }

            session.SuggestedActionsUpdated += Handler;
            try
            {
                Task populateTask;
                try
                {
                    populateTask = (Task)s_populateWithDataAsync.Invoke(session, new object?[] { null, null })!;
                }
                catch (TargetInvocationException tie) when (tie.InnerException is { } inner)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"populate-invoke-failed {inner.GetType().Name}: {inner.Message}");
                }
                catch (Exception ex)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"populate-invoke-failed {ex.GetType().Name}: {ex.Message}");
                }

                using var perAttempt = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                perAttempt.CancelAfter(s_perAttemptTimeout);

                string status;
                try
                {
                    await populateTask.WithCancellation(perAttempt.Token);
                    status = "populate-completed";
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    status = "populate-canceled";
                }
                catch (Exception ex)
                {
                    status = $"populate-faulted {ex.GetType().Name}: {ex.Message}";
                }

                // Prefer the authoritative aggregated task result; fall back to the last event payload.
                var fromTask = ReadTaskResult(populateTask);
                if (fromTask.Count > 0)
                {
                    return (fromTask, $"{status}, taskStatus={populateTask.Status}, result sets={fromTask.Count}");
                }

                if (eventTcs.Task.IsCompleted)
                {
                    var fromEvent = await eventTcs.Task;
                    return (fromEvent, $"{status}, taskStatus={populateTask.Status}, result sets=0, event sets={fromEvent.Count}");
                }

                return (Array.Empty<SuggestedActionSet>(), $"{status}, taskStatus={populateTask.Status}, result sets=0, no event");
            }
            finally
            {
                session.SuggestedActionsUpdated -= Handler;
            }
        }

        private static IReadOnlyList<SuggestedActionSet> ReadTaskResult(Task task)
        {
            if (task.Status != TaskStatus.RanToCompletion)
            {
                return Array.Empty<SuggestedActionSet>();
            }

            object? result;
            try
            {
                result = task.GetType().GetProperty("Result")?.GetValue(task);
            }
            catch
            {
                return Array.Empty<SuggestedActionSet>();
            }

            return ToActionSets(result);
        }

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

            return ToActionSets(value);
        }

        // Reads an ImmutableArray<SuggestedActionSet>-typed value through the non-generic IEnumerable
        // interface, never naming ImmutableArray (see class comment for why).
        private static IReadOnlyList<SuggestedActionSet> ToActionSets(object? boxed)
        {
            if (boxed is not IEnumerable sequence)
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
    }
}
