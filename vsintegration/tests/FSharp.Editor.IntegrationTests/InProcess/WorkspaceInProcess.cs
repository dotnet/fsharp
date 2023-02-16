// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.Editor.Shared.Utilities;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.NavigateTo;
using Microsoft.CodeAnalysis.Shared.TestHooks;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Threading;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class WorkspaceInProcess
{
    public async Task WaitForAllAsyncOperationsAsync(string[] featureNames, CancellationToken cancellationToken)
    {
        if (featureNames.Contains("Workspace"))
        {
            await WaitForProjectSystemAsync(cancellationToken);
            await TestServices.Shell.WaitForFileChangeNotificationsAsync(cancellationToken);
            await TestServices.Editor.WaitForEditorOperationsAsync(cancellationToken);
        }

        var listenerProvider = await GetComponentModelServiceAsync<AsynchronousOperationListenerProvider>(cancellationToken);
        var workspace = await GetComponentModelServiceAsync<VisualStudioWorkspace>(cancellationToken);

        if (featureNames.Contains("NavigateTo"))
        {
            var statusService = workspace.Services.GetRequiredService<IWorkspaceStatusService>();

            // Make sure the "priming" operation has started for Nav To
            var threadingContext = await GetComponentModelServiceAsync<IThreadingContext>(cancellationToken);
            var asyncListener = listenerProvider.GetListener(FeatureAttribute.NavigateTo);
            var searchHost = new DefaultNavigateToSearchHost(workspace.CurrentSolution, asyncListener, threadingContext.DisposalToken);

            // Calling DefaultNavigateToSearchHost.IsFullyLoadedAsync starts the fire-and-forget asynchronous
            // operation to populate the remote host. The call to WaitAllAsync below will wait for that operation to
            // complete.
            await searchHost.IsFullyLoadedAsync(cancellationToken);
        }

        await listenerProvider.WaitAllAsync(workspace, featureNames).WithCancellation(cancellationToken);
    }
}
