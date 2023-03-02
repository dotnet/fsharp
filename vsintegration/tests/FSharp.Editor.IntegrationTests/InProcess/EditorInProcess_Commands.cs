// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

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
}
