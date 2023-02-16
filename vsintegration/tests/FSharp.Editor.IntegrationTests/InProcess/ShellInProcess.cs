// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FSharp.Editor.IntegrationTests;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Xunit;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class ShellInProcess
{
    public async Task ShowNavigateToDialogAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        await TestServices.Shell.ExecuteCommandAsync(VSConstants.VSStd12CmdID.NavigateTo, cancellationToken);

        await WaitForNavigateToFocusAsync(cancellationToken);

        async Task WaitForNavigateToFocusAsync(CancellationToken cancellationToken)
        {
            bool isSearchActive = false;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Take no direct action regarding activation, but assert the correct item already has focus
                await TestServices.JoinableTaskFactory.RunAsync(async () =>
                {
                    await TestServices.JoinableTaskFactory.SwitchToMainThreadAsync();
                    var searchBox = Assert.IsAssignableFrom<Control>(Keyboard.FocusedElement);
                    if ("PART_SearchBox" == searchBox.Name || "SearchBoxControl" == searchBox.Name)
                    {
                        isSearchActive = true;
                    }
                });

                if (isSearchActive)
                {
                    return;
                }

                // If the dialog has not been displayed, then wait some time for it to show. The
                // cancellation token passed in should be hang mitigating to avoid possible
                // infinite loop.
                await Task.Delay(100);
            }
        }
    }

    // This is based on WaitForQuiescenceAsync in the FileChangeService tests
    public async Task WaitForFileChangeNotificationsAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var fileChangeService = await GetRequiredGlobalServiceAsync<SVsFileChangeEx, IVsFileChangeEx>(cancellationToken);
        Assumes.Present(fileChangeService);

        var jobSynchronizer = fileChangeService.GetPropertyValue("JobSynchronizer");
        Assumes.Present(jobSynchronizer);

        var type = jobSynchronizer.GetType();
        var methodInfo = type.GetMethod("GetActiveSpawnedTasks", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assumes.Present(methodInfo);

        while (true)
        {
            var tasks = (Task[])methodInfo.Invoke(jobSynchronizer, null);
            if (!tasks.Any())
                return;

            await Task.WhenAll(tasks);
        }
    }

}