// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Helpers

open FSharp.TestHelpers

module AssemblyResolver =
    
    /// Add VS assembly resolver using centralized discovery logic
    let addResolver () = addVSAssemblyResolver()
