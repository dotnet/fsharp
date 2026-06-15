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

        // IAsyncLightBulbSession.PopulateWithDataAsync returns Task<ImmutableArray<SuggestedActionSet>>.
        // ImmutableArray binds System.Collections.Immutable, whose version skews between the NuGet
        // reference (10.0.0.8) and the in-proc VS runtime (10.0.0.3), so a direct compiled call throws
        // MissingMethodException. We invoke it via reflection and await it as the non-generic base Task,
        // so that skewed type never appears in our IL. The same skew makes SuggestedActionsUpdatedArgs
        // and TryGetSuggestedActionSets fragile, so results are read out of the task via the non-generic
        // IEnumerable interface only (see ExtractActionSets).
        private static readonly MethodInfo s_populateWithDataAsync =
            typeof(IAsyncLightBulbSession).GetMethod(
                "PopulateWithDataAsync",
                new[] { typeof(ISuggestedActionCategorySet), typeof(IUIThreadOperationContext) })
            ?? throw new InvalidOperationException("IAsyncLightBulbSession.PopulateWithDataAsync not found.");

        // We create and own the session (instead of posting ShowQuickFixes and racing to find it) so the
        // session is never dismissed out from under us. PopulateWithDataAsync drives the F# code-fix
        // sources and awaits them; a slow background analyzer (e.g. unused-opens) may still yield no data
        // on an early query, so we retry with a fresh session until actions appear or we time out.
        public static async Task<IEnumerable<SuggestedActionSet>> WaitForItemsAsync(
            ILightBulbBroker broker,
            ISuggestedActionCategorySet categories,
            IWpfTextView view,
            CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow + s_timeout;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

#pragma warning disable CS0618 // ILightBulbBroker2.CreateSession's extra category-set param can't be validated for these tests locally
                var session = (IAsyncLightBulbSession)broker.CreateSession(categories, view);
#pragma warning restore CS0618

                try
                {
                    var populateTask = (Task)s_populateWithDataAsync.Invoke(session, new object?[] { null, null })!;
                    await populateTask.WithCancellation(cancellationToken);

                    var actionSets = ExtractActionSets(populateTask);
                    if (actionSets.Count > 0 || DateTime.UtcNow > deadline)
                    {
                        return actionSets;
                    }
                }
                finally
                {
                    try
                    {
                        session.Dismiss();
                    }
                    catch
                    {
                        // best-effort cleanup of the session we created
                    }
                }

                await Task.Delay(250, cancellationToken);
            }
        }

        // Reads the populated SuggestedActionSets out of the completed Task<ImmutableArray<...>> through the
        // non-generic IEnumerable interface, never naming ImmutableArray (see class comment for why).
        private static IReadOnlyList<SuggestedActionSet> ExtractActionSets(Task populateTask)
        {
            object? result;
            try
            {
                result = populateTask.GetType().GetProperty("Result")?.GetValue(populateTask);
            }
            catch
            {
                return Array.Empty<SuggestedActionSet>();
            }

            if (result is not IEnumerable sequence)
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
