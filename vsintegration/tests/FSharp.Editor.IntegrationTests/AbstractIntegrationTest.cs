// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing.InProcess;
using Microsoft.VisualStudio.Extensibility.Testing;
using System.Threading;
using Xunit;

namespace Microsoft.CodeAnalysis.Testing
{
    [IdeSettings(MinVersion = VisualStudioVersion.VS2022)]
    public abstract class AbstractIntegrationTest : AbstractIdeIntegrationTest
    {
        protected CancellationToken TestToken => HangMitigatingCancellationToken;

        internal SolutionExplorerInProcess SolutionExplorer => TestServices.SolutionExplorer;
        internal EditorInProcess Editor => TestServices.Editor;
        internal ShellInProcess Shell => TestServices.Shell;
        internal WorkspaceInProcess Workspace => TestServices.Workspace;
        internal ErrorListInProcess ErrorList => TestServices.ErrorList;
        internal TelemetryInProcess Telemetry => TestServices.Telemetry;
    }
}
