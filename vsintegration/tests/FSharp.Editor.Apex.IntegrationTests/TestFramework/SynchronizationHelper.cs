//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Test.Apex.Services;
using Microsoft.Test.Apex.VisualStudio;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Synchronization helper for the F# Apex integration tests.
    /// This is a compact, self-contained equivalent of the TypeScript-VS SynchronizationHelper:
    /// where that type delegated to the Roslyn "WaitForFeatures" async-operation waiter, this one
    /// polls via the Apex <see cref="ISynchronizationService"/> so the harness has no dependency on
    /// the JavaScript/TypeScript BrowserTools layer.
    /// </summary>
    public sealed class SynchronizationHelper
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(1);

        private readonly VisualStudioHost visualStudio;
        private readonly ISynchronizationService synchronizationService;

        public SynchronizationHelper(VisualStudioHost visualStudio, ISynchronizationService synchronizationService)
        {
            this.visualStudio = visualStudio;
            this.synchronizationService = synchronizationService;
        }

        /// <summary>
        /// Waits for the solution to be fully loaded and for background analysis to settle.
        /// </summary>
        public void WaitForSolutionCrawler()
        {
            this.visualStudio.ObjectModel.Solution.WaitForFullyLoaded();
            this.Settle();
        }

        /// <summary>
        /// Waits for the light bulb / code-action pipeline to settle before the light bulb is queried.
        /// </summary>
        public void WaitForLightBulb() => this.Settle();

        /// <summary>
        /// Blocks until <paramref name="condition"/> returns true or the timeout elapses.
        /// </summary>
        public void WaitFor(Func<bool> condition, TimeSpan timeout)
            => this.TryWaitForCondition(condition, timeout);

        /// <summary>
        /// Polls <paramref name="condition"/> until it returns true or the timeout elapses.
        /// </summary>
        /// <returns>True if the condition became true within the timeout; otherwise false.</returns>
        public bool TryWaitForCondition(Func<bool> condition, TimeSpan? timeout = null)
        {
            return this.synchronizationService.TryWaitFor(timeout ?? DefaultTimeout, () => condition());
        }

        private void Settle()
            => this.synchronizationService.TryWaitFor(DefaultInterval, () => false);
    }
}
