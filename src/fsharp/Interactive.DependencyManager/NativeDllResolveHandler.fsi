// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Interactive.DependencyManager

open System
open System.Collections.Generic

type NativeResolutionProbe = delegate of Unit -> IEnumerable<string>

/// Handle dll resolution
type NativeDllResolveHandler =

    /// Construct a new DependencyProvider
    new: nativeProbingRoots: NativeResolutionProbe -> NativeDllResolveHandler

    interface IDisposable
