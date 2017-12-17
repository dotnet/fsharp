// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor
open System

/// Assert helpers
type internal Assert() = 
    /// Display a good exception for this error message and then rethrow.
    static member Exception(e:Exception) =  
        System.Diagnostics.Debug.Assert(false, "Unexpected exception seen in language service", e.ToString())
