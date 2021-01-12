// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.DotNet.DependencyManager

open System
open System.Reflection

/// Signature for Native library resolution probe callback
/// host implements this, it's job is to return a list of package roots to probe.
type NativeResolutionProbe = delegate of Unit -> seq<string>

type IRegisterResolvers =
    inherit IDisposable
    abstract RegisterAssemblyNativeResolvers: Assembly -> unit
    abstract RegisterPackageRoots: string seq -> unit

// Cut down AssemblyLoadContext, for loading native libraries
type NativeDllResolveHandler =

    /// Construct a new NativeDllResolveHandler
    new: nativeProbingRoots: NativeResolutionProbe -> NativeDllResolveHandler

    interface IRegisterResolvers
    interface IDisposable
