// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Threading;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests.Helpers
{
    // Deterministic, focus-independent synchronization via Roslyn's IAsynchronousOperationListener waiters - the
    // same mechanism Roslyn's own VS integration tests and the TypeScript-VS Apex tests use. Reached by reflection
    // because the provider is internal; Roslyn exposes a non-InternalsVisibleTo path (RoslynWaiterEnabled env var
    // + static Enable). Feature names are the stable literal strings from FeatureAttribute.
    internal static class AsyncOperationWaiter
    {
        public const string Workspace = "Workspace";
        public const string SolutionCrawlerLegacy = "SolutionCrawlerLegacy";
        public const string DiagnosticService = "DiagnosticService";
        public const string LightBulb = "LightBulb";

        private const string ProviderTypeName = "Microsoft.CodeAnalysis.Shared.TestHooks.AsynchronousOperationListenerProvider";
        private const string WorkspaceTypeName = "Microsoft.VisualStudio.LanguageServices.VisualStudioWorkspace";

        private static readonly object s_gate = new object();
        private static bool s_enableAttempted;

        // Enable the listener tracking so the waiters below actually wait. The env vars are the robust path (read
        // lazily by Roslyn, honored even for non-IVT teams); the static Enable covers an already-cached state.
        public static void EnableTracking()
        {
            lock (s_gate)
            {
                if (s_enableAttempted)
                {
                    return;
                }

                s_enableAttempted = true;

                Environment.SetEnvironmentVariable("RoslynWaiterEnabled", "1");
                Environment.SetEnvironmentVariable("RoslynWaiterDiagnosticTokenEnabled", "1");

                var enable = TryGetProviderType()?.GetMethod("Enable", new[] { typeof(bool), typeof(bool?) });
                enable?.Invoke(null, new object?[] { true, true });
            }
        }

        // Returns whether Roslyn currently reports listener tracking as enabled (s_enabled), for diagnostics: a
        // zero-duration drain on a disabled provider is a silent no-op, so we surface this in failure messages.
        public static bool IsTrackingEnabled()
        {
            var field = TryGetProviderType()?.GetField("s_enabled", BindingFlags.Public | BindingFlags.Static);
            return field?.GetValue(null) is bool enabled && enabled;
        }

        // Awaits completion of all queued async operations for the given Roslyn features. Focus-independent and
        // deterministic, unlike polling the lightbulb UI session.
        public static async Task WaitForFeaturesAsync(IComponentModel componentModel, string[] featureNames, CancellationToken cancellationToken)
        {
            System.Diagnostics.Trace.TraceInformation(
                "[AsyncOperationWaiter] WaitForFeaturesAsync: features=[{0}], trackingEnabled={1}",
                string.Join(", ", featureNames), IsTrackingEnabled());

            var providerType = TryGetProviderType();
            if (providerType is null)
            {
                System.Diagnostics.Trace.TraceInformation("[AsyncOperationWaiter] WaitForFeaturesAsync: providerType is null, returning");
                return;
            }

            var provider = TryGetService(componentModel, providerType);
            if (provider is null)
            {
                System.Diagnostics.Trace.TraceInformation("[AsyncOperationWaiter] WaitForFeaturesAsync: provider is null, returning");
                return;
            }

            var workspaceType = TryGetWorkspaceType();
            var workspace = workspaceType is null ? null : TryGetService(componentModel, workspaceType);

            System.Diagnostics.Trace.TraceInformation(
                "[AsyncOperationWaiter] WaitForFeaturesAsync: workspace={0}",
                workspace is null ? "null" : workspace.GetType().Name);

            var waitAll = providerType.GetMethod("WaitAllAsync");
            if (waitAll is null)
            {
                System.Diagnostics.Trace.TraceInformation("[AsyncOperationWaiter] WaitForFeaturesAsync: WaitAllAsync method not found, returning");
                return;
            }

            Task task;
            try
            {
                task = (Task)waitAll.Invoke(provider, new object?[] { workspace, featureNames, null, null })!;
            }
            catch (TargetInvocationException ex)
            {
                throw new InvalidOperationException(
                    $"Roslyn waiter invocation failed for [{string.Join(", ", featureNames)}] (trackingEnabled={IsTrackingEnabled()}).",
                    ex.InnerException ?? ex);
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await task.WithCancellation(cancellationToken);
                sw.Stop();
                System.Diagnostics.Trace.TraceInformation(
                    "[AsyncOperationWaiter] WaitForFeaturesAsync: features=[{0}] drained in {1:F1}ms",
                    string.Join(", ", featureNames), sw.Elapsed.TotalMilliseconds);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    $"Roslyn waiter timed out for [{string.Join(", ", featureNames)}] (trackingEnabled={IsTrackingEnabled()}).");
            }
        }

        private static object? TryGetService(IComponentModel componentModel, Type serviceType)
        {
            try
            {
                var getService = typeof(IComponentModel).GetMethod("GetService")!.MakeGenericMethod(serviceType);
                return getService.Invoke(componentModel, null);
            }
            catch
            {
                return null;
            }
        }

        private static Type? TryGetProviderType() => FindType("Microsoft.CodeAnalysis.Workspaces", ProviderTypeName);

        private static Type? TryGetWorkspaceType() => FindType("Microsoft.VisualStudio.LanguageServices", WorkspaceTypeName);

        private static Type? FindType(string assemblyName, string typeName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
            return assembly?.GetType(typeName);
        }
    }
}
