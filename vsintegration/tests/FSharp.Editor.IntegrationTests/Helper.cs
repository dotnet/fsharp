﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FSharp.Editor.IntegrationTests;

internal static class Helper
{
    /// <summary>
    /// This method will retry the asynchronous action represented by <paramref name="action"/>,
    /// waiting for <paramref name="delay"/> time after each retry. If a given retry returns a value
    /// other than the default value of <typeparamref name="T"/>, this value is returned.
    /// </summary>
    /// <param name="action">the asynchronous action to retry</param>
    /// <param name="delay">the amount of time to wait between retries</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">type of return value</typeparam>
    /// <returns>the return value of <paramref name="action"/></returns>
    public static Task<T?> RetryAsync<T>(Func<CancellationToken, Task<T>> action, TimeSpan delay, CancellationToken cancellationToken)
    {
        return RetryCoreAsync(
            async cancellationToken =>
            {
                try
                {
                    return await action(cancellationToken);
                }
                catch (COMException)
                {
                    // Devenv can throw COMExceptions if it's busy when we make DTE calls.
                    return default;
                }
            },
            delay,
            cancellationToken);
    }

    private static async Task<T> RetryCoreAsync<T>(Func<CancellationToken, Task<T>> action, TimeSpan delay, CancellationToken cancellationToken)
    {
        while (true)
        {
            var retval = await action(cancellationToken);
            if (!Equals(default(T), retval))
            {
                return retval;
            }

            await Task.Delay(delay, cancellationToken);
        }
    }
}
