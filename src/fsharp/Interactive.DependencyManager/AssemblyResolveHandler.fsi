// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Interactive.DependencyManager

open System
open System.Collections.Generic

type AssemblyResolutionProbe = delegate of Unit -> IEnumerable<string>

/// Handle dll resolution
type AssemblyResolveHandler =

    /// Construct a new DependencyProvider
    new: assemblyProbingPaths: AssemblyResolutionProbe -> AssemblyResolveHandler

    interface IDisposable
