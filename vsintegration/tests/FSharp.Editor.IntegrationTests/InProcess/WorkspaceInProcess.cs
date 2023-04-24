// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
