// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class WorkspaceInProcess
{
    public async Task WaitForAsyncOperationsAsync(CancellationToken cancellationToken)
    {
        await WaitForProjectSystemAsync(cancellationToken);
    }
}
