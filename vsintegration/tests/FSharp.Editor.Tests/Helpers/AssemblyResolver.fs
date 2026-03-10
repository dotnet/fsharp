// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Helpers

module AssemblyResolver =
    open FSharp.Test.VSAssemblyResolver

    /// Adds an assembly resolver that probes Visual Studio installation directories.
    /// This is a compatibility shim that delegates to the centralized implementation.
    let addResolver = addResolver
