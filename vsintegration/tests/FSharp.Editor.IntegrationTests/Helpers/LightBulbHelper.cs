// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
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
        // PRODUCE phase budget: how long to wait for F# to publish a diagnostic (error or warning) to the Error
        // List. The unused-opens check can be slow, so keep this generous.
        private static readonly TimeSpan s_produceTimeout = TimeSpan.FromSeconds(100);
        // PRODUCE phase poll interval. We poll the Error List ONLY (no buffer edit, no lightbulb invoke/dismiss),
        // so F#'s idle-driven diagnostic crawler gets the uninterrupted window it needs to actually run.
        private static readonly TimeSpan s_producePoll = TimeSpan.FromSeconds(1.5);
        // If no diagnostic has appeared after this long, re-touch the buffer once (the first touch may have fired
        // before the project's checker was ready). Long enough not to cancel a slow in-flight analysis.
        private static readonly TimeSpan s_reTouchInterval = TimeSpan.FromSeconds(30);
        // READ phase budget: once a diagnostic is produced, keep (re-)invoking the lightbulb this long to read F#'s
        // code-fix set. F#'s code-fix source can lag the fast GitHub Copilot source by many seconds; when F# offers
        // the fix at all it usually does so within this window (its offering is non-deterministic headless).
        private static readonly TimeSpan s_readBudget = TimeSpan.FromSeconds(50);
        // Delay between read attempts in the READ phase.
        private static readonly TimeSpan s_readPoll = TimeSpan.FromSeconds(0.5);
        // Per read attempt: how long to wait for the session to populate / fire its update events.
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(8);
        // How long to wait for a session to become active after invoking ShowQuickFixes.
        private static readonly TimeSpan s_activeWait = TimeSpan.FromSeconds(3);

        // PopulateWithDataAsync returns Task<ImmutableArray<...>> and ActionSets is ImmutableArray;
        // System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime, so we invoke/read
        // these via reflection through the non-generic IEnumerable and never name ImmutableArray in compiled IL.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // Separates PRODUCE from READ - the keystone that keeps both the fast type-check fixes (AddNew) and the
        // flickering parse-error fix (AddMissingFun) reliable.
        //
        // PRODUCE phase: after one buffer touch, we wait for F# to produce the fix while keeping BOTH the buffer
        // and the lightbulb UI quiet. F#'s diagnostics flow through Roslyn's idle-driven solution crawler;
        // churning the buffer (re-touch) cancels the in-flight check, and repeatedly invoking/dismissing the
        // lightbulb starves the crawler of idle time - either one leaves nothing produced (the "0 diagnostics"
        // failure). We detect production via two low-churn signals (neither creates a lightbulb session): the
        // Error List count (GetErrorCountAsync counts errors AND warnings, so it sees the parse-error fix - which
        // broker.HasSuggestedActions is a false-negative for) OR broker.HasSuggestedActionsAsync (covers the case
        // where a code fix is offered but the Error List has not yet materialized it). Either one is enough.
        //
        // READ phase: once the fix exists the analysis has settled, so it is safe to aggressively (re-)invoke the
        // real ShowQuickFixes lightbulb and read its editor-owned session every ~0.5s. Frequent re-invocation is
        // what catches the ErrorFix session, which appears then dismisses within seconds headless; churn here can
        // no longer cancel the (already-completed) producer.
        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            ISuggestedActionCategorySet requestedCategories,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            Func<CancellationToken, Task<int>> getDiagnosticCountAsync,
            Func<CancellationToken, Task<bool>> hasSuggestedActionsAsync,
            Func<CancellationToken, Task> triggerReanalysisAsync,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;

            System.Diagnostics.Trace.TraceInformation(
                "[LightBulbHelper] GetCodeActionsAsync starting, produceTimeout={0}s, readBudget={1}s",
                s_produceTimeout.TotalSeconds, s_readBudget.TotalSeconds);

            // Kick analysis once (bump the doc version now the project is loaded), then leave the buffer and the
            // lightbulb quiet so the idle-driven crawler can complete and publish.
            await triggerReanalysisAsync(cancellationToken);
            var lastTouch = DateTime.UtcNow;
            var touches = 1;

            // PRODUCE phase: poll low-churn signals (no session create/dismiss, no re-touch spam) until the fix is produced.
            var produceDeadline = start + s_produceTimeout;
            var polls = 0;
            var lastDiagCount = -1;
            var lastHasFix = false;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    lastDiagCount = await getDiagnosticCountAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    lastDiagCount = -1;
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] diagnostic count threw {0}: {1}", ex.GetType().Name, ex.Message);
                }

                try
                {
                    lastHasFix = await hasSuggestedActionsAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    lastHasFix = false;
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] HasSuggestedActions threw {0}: {1}", ex.GetType().Name, ex.Message);
                }

                polls++;

                if (lastDiagCount > 0 || lastHasFix)
                {
                    System.Diagnostics.Trace.TraceInformation(
                        "[LightBulbHelper] Fix produced after {0:F1}s ({1} polls, {2} touches, diagCount={3}, hasFix={4}); entering READ phase",
                        (DateTime.UtcNow - start).TotalSeconds, polls, touches, lastDiagCount, lastHasFix);
                    break;
                }

                if (DateTime.UtcNow > produceDeadline)
                {
                    throw new InvalidOperationException(
                        $"No diagnostic/fix produced after {s_produceTimeout.TotalSeconds:F0}s " +
                        $"({polls} polls, {touches} touches, lastDiagCount={lastDiagCount}, hasFix={lastHasFix}).");
                }

                // Bounded fallback: if nothing appears, the first touch may have fired before the checker was
                // ready - re-touch once, then keep the buffer quiet again for another full interval.
                if (DateTime.UtcNow - lastTouch > s_reTouchInterval)
                {
                    System.Diagnostics.Trace.TraceInformation("[LightBulbHelper] No diagnostic after {0:F0}s; re-touching (touch #{1})", s_reTouchInterval.TotalSeconds, touches + 1);
                    await triggerReanalysisAsync(cancellationToken);
                    lastTouch = DateTime.UtcNow;
                    touches++;
                }

                await Task.Delay(s_producePoll, cancellationToken);
            }

            // READ phase: read F#'s code action from the editor's real lightbulb session. The critical detail:
            // we AWAIT PopulateWithDataAsync's full result (which reflects ALL suggested-action sources once they
            // finish) instead of returning on the first SuggestedActionsUpdated event. The GitHub Copilot source
            // reports FAST and a first-event read returns only its "GitHubCopilot" set, before F#'s slower code-fix
            // source has produced its set - that race is why AddNew was flaky and AddMissingFun always missed. We
            // then filter to F# fix/refactoring categories (dropping Copilot's).
            var readDeadline = DateTime.UtcNow + s_readBudget;
            var reads = 0;
            var lastDetail = "no read attempt completed";
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > readDeadline)
                {
                    throw new InvalidOperationException(
                        $"Diagnostic produced (diagCount={lastDiagCount}) but no F# code action could be read " +
                        $"within {s_readBudget.TotalSeconds:F0}s ({reads} reads; last read: {lastDetail}).");
                }

                reads++;
                var (sets, detail) = await TryGetFromRealSessionAsync(
                    broker, view, requestedCategories, joinableTaskFactory, showLightBulbAsync, cancellationToken);
                var fSharpSets = FilterToFSharpCategories(sets);
                lastDetail = $"{detail}; fSharp={fSharpSets.Count} [{string.Join(",", CategoryNames(sets))}]";

                System.Diagnostics.Trace.TraceInformation(
                    "[LightBulbHelper] Read #{0} ({1:F1}s, diagCount={2}): sets={3}, fSharp={4}; {5}",
                    reads, (DateTime.UtcNow - start).TotalSeconds, lastDiagCount, sets.Count, fSharpSets.Count, detail);

                if (fSharpSets.Count > 0)
                {
                    return fSharpSets;
                }

                await Task.Delay(s_readPoll, cancellationToken);
            }
        }

        // The suggested-action categories that correspond to F#'s own code fixes / refactorings. Everything else
        // (notably "GitHubCopilot", and IntelliCode-style suggestions) is noise for these tests and is dropped.
        private static readonly string[] s_fSharpCategories = { "CodeFix", "ErrorFix", "StyleFix", "Refactoring" };

        private static IReadOnlyList<SuggestedActionSet> FilterToFSharpCategories(IReadOnlyList<SuggestedActionSet> sets)
        {
            var result = new List<SuggestedActionSet>();
            foreach (var set in sets)
            {
                if (set?.CategoryName is string name
                    && Array.Exists(s_fSharpCategories, c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(set);
                }
            }

            return result;
        }

        private static IEnumerable<string> CategoryNames(IReadOnlyList<SuggestedActionSet> sets)
        {
            foreach (var set in sets)
            {
                yield return set?.CategoryName ?? "<null>";
            }
        }

        private static async Task<(IReadOnlyList<SuggestedActionSet> sets, string detail)> TryGetFromRealSessionAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            ISuggestedActionCategorySet requestedCategories,
            JoinableTaskFactory joinableTaskFactory,
            Func<Task> showLightBulbAsync,
            CancellationToken cancellationToken)
        {
            await joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // REUSE an already-active session instead of dismissing + re-showing it. Repeatedly dismissing/
            // re-invoking ShowQuickFixes churns the lightbulb, and that churn can CANCEL F#'s slow code-fix
            // computation before it completes (the "polling is not passive" failure) - so we only invoke the
            // command when no session is active, then keep populating the SAME session until F#'s set lands.
            if (!broker.IsLightBulbSessionActive(view))
            {
                await showLightBulbAsync();

                var activeDeadline = DateTime.UtcNow + s_activeWait;
                while (!broker.IsLightBulbSessionActive(view))
                {
                    if (DateTime.UtcNow > activeDeadline)
                    {
                        return (Array.Empty<SuggestedActionSet>(), "no active lightbulb session");
                    }

                    await Task.Delay(100, cancellationToken);
                }
            }

            if (broker.GetSession(view) is not IAsyncLightBulbSession session)
            {
                return (Array.Empty<SuggestedActionSet>(), "session active but GetSession not IAsyncLightBulbSession");
            }

            if (session.IsDismissed)
            {
                return (Array.Empty<SuggestedActionSet>(), "session already dismissed");
            }

            // Capture F#'s code-fix set from EITHER a SuggestedActionsUpdated event OR the PopulateWithDataAsync
            // result. F#'s code-fix source lags the fast GitHub Copilot source and shows up unpredictably in one or
            // the other (empirically: sometimes an update event carries it, sometimes only the final populate result
            // does), so we watch both and take whichever first yields an F#-category set.
            var fSharpTcs = new TaskCompletionSource<IReadOnlyList<SuggestedActionSet>>();

            void OnUpdated(object sender, SuggestedActionsUpdatedArgs e)
            {
                if (e.Status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    return;
                }

                var fsharp = FilterToFSharpCategories(ReadEnumerableProperty(e, "ActionSets"));
                if (fsharp.Count > 0)
                {
                    fSharpTcs.TrySetResult(fsharp);
                }
            }

            session.SuggestedActionsUpdated += OnUpdated;
            try
            {
                object? populateTaskObj;
                try
                {
                    populateTaskObj = s_populateWithDataAsync.Invoke(session, new object?[] { requestedCategories, null });
                }
                catch (Exception ex)
                {
                    return (Array.Empty<SuggestedActionSet>(), $"populate-invoke-failed {ex.GetType().Name}: {ex.Message}");
                }

                var populateTask = populateTaskObj as Task;

                using var perAttempt = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                perAttempt.CancelAfter(s_perAttemptTimeout);

                // Wait until the populate task finishes OR an update event yields an F# set, whichever first.
                try
                {
                    var waits = new List<Task> { fSharpTcs.Task };
                    if (populateTask is not null)
                    {
                        waits.Add(populateTask);
                    }

                    await Task.WhenAny(waits).WithCancellation(perAttempt.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // fall through to whatever we have
                }

                if (fSharpTcs.Task.Status == TaskStatus.RanToCompletion)
                {
                    var evSets = await fSharpTcs.Task;
                    return (evSets, $"from-update-event, fSharp={evSets.Count}");
                }

                if (populateTask is not null && populateTask.IsCompleted)
                {
                    var resultSets = ReadTaskResultSets(populateTask);
                    var fsharpResult = FilterToFSharpCategories(resultSets);
                    if (fsharpResult.Count > 0)
                    {
                        return (resultSets, $"from-populate-result, sets={resultSets.Count}, fSharp={fsharpResult.Count}");
                    }

                    // Give a late update event a brief chance after populate completes.
                    try
                    {
                        using var lateCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        lateCts.CancelAfter(TimeSpan.FromSeconds(1));
                        var late = await fSharpTcs.Task.WithCancellation(lateCts.Token);
                        return (late, $"from-late-update-event, fSharp={late.Count}");
                    }
                    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                    }

                    return (resultSets, $"populate-completed, sets={resultSets.Count}, fSharp=0");
                }

                return (Array.Empty<SuggestedActionSet>(), "no F# set from event or populate within attempt");
            }
            finally
            {
                session.SuggestedActionsUpdated -= OnUpdated;
            }
        }

        // Reads an ImmutableArray<SuggestedActionSet>-typed member (e.g. SuggestedActionsUpdatedArgs.ActionSets)
        // through the non-generic IEnumerable interface, never naming ImmutableArray in compiled IL.
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

            return CollectSets(value);
        }

        // Reads the Task<ImmutableArray<SuggestedActionSet>>.Result through the non-generic IEnumerable interface.
        private static IReadOnlyList<SuggestedActionSet> ReadTaskResultSets(Task task)
        {
            object? result;
            try
            {
                result = task.GetType().GetProperty("Result")?.GetValue(task);
            }
            catch
            {
                return Array.Empty<SuggestedActionSet>();
            }

            return CollectSets(result);
        }

        // Collects SuggestedActionSet items from a value that is an ImmutableArray<SuggestedActionSet> (or any
        // IEnumerable), via the non-generic IEnumerable so ImmutableArray is never named in compiled IL
        // (System.Collections.Immutable skews between the NuGet ref and the in-proc VS runtime).
        private static IReadOnlyList<SuggestedActionSet> CollectSets(object? value)
        {
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
    }
}
