// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open System.Collections.Generic

/// Resolves the references for a chosen or currently-executing framework, for
///   - script execution
///   - script editing
///   - script compilation
///   - out-of-project sources editing
///   - default references for fsc.exe
///   - default references for fsi.exe
type internal FxResolver =

    new:
        assumeDotNetFramework: bool *
        projectDir: string *
        useSdkRefs: bool *
        isInteractive: bool *
        rangeForErrors: Text.range *
        sdkDirOverride: string option ->
            FxResolver

    static member ClearStaticCaches: unit -> unit

    member GetDefaultReferences: useFsiAuxLib: bool -> string list * bool

    member GetFrameworkRefsPackDirectory: unit -> string option

    member GetSystemAssemblies: unit -> HashSet<string>

    /// Gets the selected target framework moniker, e.g netcore3.0, net472, and the running rid of the current machine
    member GetTfmAndRid: unit -> string * string

    member IsInReferenceAssemblyPackDirectory: fileName: string -> bool

    member TryGetDesiredDotNetSdkVersionForDirectory: unit -> Result<string, exn>

    member TryGetSdkDir: unit -> string option
