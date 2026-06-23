// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FSharp.Editor.IntegrationTests.Helpers;
using Microsoft.CodeAnalysis.Testing.InProcess;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Microsoft.CodeAnalysis.Testing
{
    // RoslynWaiter* env vars enable Roslyn's async-operation listener tracking in devenv from launch, so the
    // deterministic AsyncOperationWaiter drains actually wait (otherwise they no-op). See AsyncOperationWaiter.
    [IdeSettings(MinVersion = VisualStudioVersion.VS18, MaxVersion = VisualStudioVersion.VS18,
        EnvironmentVariables = new[] { "RoslynWaiterEnabled=1", "RoslynWaiterDiagnosticTokenEnabled=1" })]
    public abstract class AbstractIntegrationTest : AbstractIdeIntegrationTest
    {
        private static int s_traceListenerAdded;

        protected AbstractIntegrationTest()
        {
            AsyncOperationWaiter.EnableTracking();

            // Ensure Trace output is forwarded to the console (captured by xunit runner in the test log).
            if (System.Threading.Interlocked.Exchange(ref s_traceListenerAdded, 1) == 0)
            {
                Trace.Listeners.Add(new ConsoleTraceListener());
            }
        }

        protected CancellationToken TestToken => HangMitigatingCancellationToken;

        internal SolutionExplorerInProcess SolutionExplorer => TestServices.SolutionExplorer;
        internal EditorInProcess Editor => TestServices.Editor;
        internal ShellInProcess Shell => TestServices.Shell;
        internal WorkspaceInProcess Workspace => TestServices.Workspace;
        internal ErrorListInProcess ErrorList => TestServices.ErrorList;
        internal TelemetryInProcess Telemetry => TestServices.Telemetry;
    }
}
