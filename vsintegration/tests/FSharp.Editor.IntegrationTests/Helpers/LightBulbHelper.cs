// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    internal static class LightBulbHelper
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(60);

        // Query the suggested-action sources directly instead of driving the VS lightbulb UI session.
        //
        // Driving the lightbulb (PostExecCommand ShowQuickFixes -> GetSession -> PopulateWithDataAsync ->
        // SuggestedActionsUpdated) proved hopelessly racy in headless CI: session creation/dismissal,
        // command routing, event ordering and supersession produced different timing-dependent failures
        // every run. It also forced reflection gymnastics because PopulateWithDataAsync and
        // SuggestedActionsUpdatedArgs.ActionSets are typed as ImmutableArray<SuggestedActionSet>, which
        // binds System.Collections.Immutable - a version that skews between the test's NuGet reference and
        // the in-proc VS runtime and throws MissingMethodException.
        //
        // ILightBulbBroker.GetSuggestedActionsSources / ISuggestedActionsSource2.GetSuggestedActionCategoriesAsync
        // / ISuggestedActionsSource.GetSuggestedActions / SuggestedActionSet.Actions are all version-safe (no
        // ImmutableArray in their signatures), deterministic, and have no UI-session lifetime.
        // GetSuggestedActionCategoriesAsync drives the (lazy) F# code-fix computation - the Roslyn-backed
        // source throws NotImplementedException on HasSuggestedActionsAsync - and we retry until items appear
        // to cover background analyzers whose diagnostics (e.g. unused-opens) are published asynchronously.
        public static async Task<IReadOnlyList<SuggestedActionSet>> GetCodeActionsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var deadline = start + s_timeout;
            var attempt = 0;
            var log = new StringBuilder();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                attempt++;

                var caret = view.Caret.Position.BufferPosition;
                var line = caret.GetContainingLine();

                // Use the whole caret line as the query range: the lightbulb keyboard command behaves the
                // same way, and some F# fixes squiggle a sub-span of the line (e.g. the constructor
                // expression for FS0760), not the exact caret point.
                var span = line.Extent;

                var sources = broker.GetSuggestedActionsSources(view, view.TextBuffer)?.ToArray()
                    ?? Array.Empty<ISuggestedActionsSource>();

                var actionSets = new List<SuggestedActionSet>();

                log.Clear();
                log.AppendLine(
                    $"attempt {attempt}, elapsed {(DateTime.UtcNow - start).TotalSeconds:F1}s, " +
                    $"caret line {line.LineNumber} '{line.GetText()}', span [{span.Start.Position}..{span.End.Position}], " +
                    $"sources={sources.Length}");

                foreach (var source in sources)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var name = source.GetType().Name;
                    try
                    {
                        // The Roslyn-backed source (which surfaces the F# code fixes) throws on
                        // HasSuggestedActionsAsync ("We implement GetSuggestedActionCategoriesAsync").
                        // GetSuggestedActionCategoriesAsync is the async driver that runs the lazy fix
                        // computation; GetSuggestedActions then returns the computed sets.
                        ISuggestedActionCategorySet? categories = null;
                        if (source is ISuggestedActionsSource2 source2)
                        {
                            categories = await source2.GetSuggestedActionCategoriesAsync(
                                requestedActionCategories: null, range: span, cancellationToken);
                        }

                        var sets = source.GetSuggestedActions(
                            requestedActionCategories: categories, range: span, cancellationToken);

                        var count = 0;
                        if (sets is not null)
                        {
                            foreach (var set in sets)
                            {
                                if (set is not null)
                                {
                                    actionSets.Add(set);
                                    count++;
                                }
                            }
                        }

                        log.AppendLine($"  {name}: categories={(categories is null ? "null" : "set")}, sets={count}");
                    }
                    catch (NotImplementedException)
                    {
                        log.AppendLine($"  {name}: not-implemented (skipped)");
                    }
                    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                        log.AppendLine($"  {name}: canceled");
                    }
                    catch (Exception ex)
                    {
                        log.AppendLine($"  {name}: EXCEPTION {ex.GetType().Name}: {ex.Message}");
                    }
                }

                if (actionSets.Count > 0)
                {
                    return actionSets;
                }

                if (DateTime.UtcNow > deadline)
                {
                    // Throw rather than return empty so the (only) CI signal carries the diagnostic dump.
                    throw new InvalidOperationException(
                        $"No code actions found after {s_timeout.TotalSeconds:F0}s. Last attempt:{Environment.NewLine}{log}");
                }

                await Task.Delay(250, cancellationToken);
            }
        }
    }
}
