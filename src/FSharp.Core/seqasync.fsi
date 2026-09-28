// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

#if NET
open System
open System.Collections.Generic
open Microsoft.FSharp.Core

module RuntimeAsyncSequenceHelpers =
    val EnumerateTryWith:
        source: IAsyncEnumerable<'T> ->
        exceptionFilter: (exn -> IAsyncEnumerable<int>) ->
        exceptionHandler: (exn -> IAsyncEnumerable<'T>) ->
            IAsyncEnumerable<'T>
#endif
