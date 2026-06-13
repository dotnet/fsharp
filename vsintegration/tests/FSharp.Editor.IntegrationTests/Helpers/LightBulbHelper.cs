// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    internal static class LightBulbHelper
    {
        // Create and own the session (ILightBulbBroker has no session-created event) instead of posting
        // ShowQuickFixes then racing to find it: posting races the F# analyzer (NRE / TaskCanceled on CI).
        public static async Task<IEnumerable<SuggestedActionSet>> WaitForItemsAsync(
            ILightBulbBroker broker,
            ISuggestedActionCategorySet categories,
            IWpfTextView view,
            CancellationToken cancellationToken)
        {
            // Keep the obsolete simple overload: ILightBulbBroker2.CreateSession's extra category-set
            // changes expanded-vs-available actions and can't be validated against these tests locally.
#pragma warning disable CS0618
            var asyncSession = (IAsyncLightBulbSession)broker.CreateSession(categories, view);
#pragma warning restore CS0618
            var tcs = new TaskCompletionSource<IEnumerable<SuggestedActionSet>>();

            void Handler(object s, SuggestedActionsUpdatedArgs e)
            {
                // Ignore intermediate updates; we only care once the lightbulb items are all computed.
                if (e.Status == QuerySuggestedActionCompletionStatus.InProgress)
                {
                    return;
                }

                if (e.Status == QuerySuggestedActionCompletionStatus.Completed ||
                    e.Status == QuerySuggestedActionCompletionStatus.CompletedWithoutData)
                {
                    tcs.TrySetResult(e.ActionSets ?? Array.Empty<SuggestedActionSet>());
                }
                else
                {
                    tcs.TrySetException(new InvalidOperationException($"Light bulb transitioned to non-complete state: {e.Status}"));
                }

                asyncSession.SuggestedActionsUpdated -= Handler;
            }

            asyncSession.SuggestedActionsUpdated += Handler;

            try
            {
                // Fast path: the computation may already be done; read it synchronously.
                if (TryGetCompletedActionSets(asyncSession, out var existing))
                {
                    return existing;
                }

                // Drives the F# code-fix sources and awaits them, so slow analyzers (e.g. unused-opens)
                // are handled here, and guarantees SuggestedActionsUpdated fires with the latest data.
                await asyncSession.PopulateWithDataAsync(overrideRequestedActionCategories: null, operationContext: null);

                // Prefer the event result; fall back to a synchronous read if it fired before we resumed.
                if (tcs.Task.IsCompleted)
                {
                    return await tcs.Task.WithCancellation(cancellationToken);
                }

                if (TryGetCompletedActionSets(asyncSession, out existing))
                {
                    return existing;
                }

                return await tcs.Task.WithCancellation(cancellationToken);
            }
            finally
            {
                asyncSession.SuggestedActionsUpdated -= Handler;
                try
                {
                    asyncSession.Dismiss();
                }
                catch
                {
                    // Best-effort cleanup of the session we created; ignore if already gone.
                }
            }
        }

        private static bool TryGetCompletedActionSets(IAsyncLightBulbSession session, out IEnumerable<SuggestedActionSet> actionSets)
        {
            // Defensive fallback read; no non-obsolete synchronous accessor returns the computed sets.
#pragma warning disable CS0618
            var status = session.TryGetSuggestedActionSets(out var sets);
#pragma warning restore CS0618
            if (status == QuerySuggestedActionCompletionStatus.Completed ||
                status == QuerySuggestedActionCompletionStatus.CompletedWithoutData)
            {
                actionSets = sets ?? Array.Empty<SuggestedActionSet>();
                return true;
            }

            actionSets = Array.Empty<SuggestedActionSet>();
            return false;
        }
    }
}
