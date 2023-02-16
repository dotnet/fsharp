// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using WindowsInput;
using WindowsInput.Native;

namespace Microsoft.VisualStudio.Extensibility.Testing;

[TestService]
internal partial class InputInProcess
{
    internal async Task SendToNavigateToAsync(InputKey[] keys, CancellationToken cancellationToken)
    {
        // AbstractSendKeys runs synchronously, so switch to a background thread before the call
        await TaskScheduler.Default;

        // Take no direct action regarding activation, but assert the correct item already has focus
        await TestServices.JoinableTaskFactory.RunAsync(async () =>
        {
            await TestServices.JoinableTaskFactory.SwitchToMainThreadAsync();
        });

        var inputSimulator = new InputSimulator();
        foreach (var key in keys)
        {
            // If it is enter key, we need to wait for search item shows up in the search dialog.
            if (key.VirtualKeyCode == VirtualKeyCode.RETURN)
            {
                await WaitNavigationItemShowsUpAsync(cancellationToken);
            }

            key.Apply(inputSimulator);
        }

        await TestServices.JoinableTaskFactory.RunAsync(async () =>
        {
            await WaitForApplicationIdleAsync(cancellationToken);
        });
    }

    private async Task WaitNavigationItemShowsUpAsync(CancellationToken cancellationToken)
    {
        // Wait for the NavigateTo Features completes on Roslyn side.
        await TestServices.Workspace.WaitForAllAsyncOperationsAsync(new[] { "NavigateTo" }, cancellationToken);
        // Since the all-in-one search experience populates its results asychronously we need
        // to give it time to update the UI. Note: This is not a perfect solution.
        await Task.Delay(1000);
        await WaitForApplicationIdleAsync(cancellationToken);
    }
}
