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
        private static readonly TimeSpan s_perAttemptTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_minRetriggerInterval = TimeSpan.FromMilliseconds(500);

        // PopulateWithDataAsync returns Task<ImmutableArray<SuggestedActionSet>> and SuggestedActionsUpdatedArgs
        // exposes ImmutableArray too; that type binds System.Collections.Immutable, whose version skews between
        // the NuGet reference and the in-proc VS runtime, so any compiled reference throws MissingMethodException.
        // We invoke/read these members via reflection so the skewed type never appears in our IL.
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // We trigger ShowQuickFixes (the command creates the real, active lightbulb session) and read items off
        // the SuggestedActionsUpdated event. Subscribing only after GetSession returns non-null avoids the NRE
        // race; we never subscribe Dismissed (its spurious cancel was the old TaskCanceled failure). A fresh
        // trigger is retried until actions appear, covering slow analyzers such as unused-opens.
        public static async Task<IEnumerable<SuggestedActionSet>> WaitForItemsAsync(
            ILightBulbBroker broker,
            IWpfTextView view,
            Action triggerCommand,
            CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow + s_timeout;
            var lastTrigger = DateTime.MinValue;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow > deadline)
                {
                    return Array.Empty<SuggestedActionSet>();
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

                var actionSets = await TryPopulateAsync(session, cancellationToken);
                if (actionSets.Count > 0)
                {
                    return actionSets;
                }

                // No items this attempt (slow analyzer not ready, or session superseded). Dismiss so the
                // next iteration re-triggers a fresh session instead of re-polling this terminal one.
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

        private static async Task<IReadOnlyList<SuggestedActionSet>> TryPopulateAsync(
            IAsyncLightBulbSession session,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<SuggestedActionSet>>();

            void Handler(object sender, SuggestedActionsUpdatedArgs e)
            {
                if (e.Status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    return;
                }

                tcs.TrySetResult(ReadActionSets(e, "ActionSets"));
            }

            session.SuggestedActionsUpdated += Handler;
            try
            {
                // Drive the computation (the command may already have populated, but this also re-fires the
                // event with the latest data). We swallow the populate task's own result/cancellation and rely
                // on the event so a superseded session retries instead of throwing.
                try
                {
                    var populateTask = (Task)s_populateWithDataAsync.Invoke(session, new object?[] { null, null })!;
                    populateTask.Forget();
                }
                catch
                {
                    // Reflection/invoke failure: fall through and let the event or timeout drive the retry.
                }

                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(s_perAttemptTimeout);

                try
                {
                    return await tcs.Task.WithCancellation(timeoutCts.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // This attempt did not produce items in time; the caller dismisses and retriggers.
                    return Array.Empty<SuggestedActionSet>();
                }
            }
            finally
            {
                session.SuggestedActionsUpdated -= Handler;
            }
        }

        // Reads an ImmutableArray<SuggestedActionSet>-typed member through the non-generic IEnumerable
        // interface, never naming ImmutableArray (see class comment for why).
        private static IReadOnlyList<SuggestedActionSet> ReadActionSets(object source, string propertyName)
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
    }
}
