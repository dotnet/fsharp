// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.VisualStudio.VSConstants;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class EditorInProcess
{

    public async Task GoToDefinitionAsync(CancellationToken cancellationToken)
    {
        await TestServices.Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, cancellationToken);
        await TestServices.Workspace.WaitForAsyncOperationsAsync(cancellationToken);
    }

    public async Task InvokeGoToDefinitionAsync(CancellationToken cancellationToken)
    {
        var commandGuid = typeof(VSStd97CmdID).GUID;
        var commandId = VSStd97CmdID.GotoDefn;
        await ExecuteCommandAsync(commandGuid, (uint)commandId, cancellationToken);
    }

    private async Task ExecuteCommandAsync(Guid commandGuid, uint commandId, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dispatcher = await GetRequiredGlobalServiceAsync<SUIHostCommandDispatcher, IOleCommandTarget>(cancellationToken);
        ErrorHandler.ThrowOnFailure(dispatcher.Exec(commandGuid, commandId, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero));
    }
}
