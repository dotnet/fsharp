// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate ReferenceLoading.PaketHandler into F# codebase
module internal Microsoft.FSharp.Compiler.ReferenceLoading.PaketHandler

// NOTE: this contains mostly members whose intents are :
// * to keep ReferenceLoading.PaketHandler usable outside of F# (so it can be used in scriptcs & others)
// * to minimize footprint of integration in fsi/CompileOps

/// hardcoded to load the "Main" group (implicit in paket)
let scriptName = "main.group.fsx"

/// hardcoded to net461 as we don't have fsi on netcore
let targetFramework = "net461"

/// used to alter package management tool command depending runtime context (if running Mono, prefix with "mono ").
let AlterPackageManagementToolCommand command =
    if Microsoft.FSharp.Compiler.AbstractIL.IL.runningOnMono 
    then "mono " + command
    else command

/// Resolves absolute load script location: something like
/// baseDir/.paket/load/scriptName
/// or
/// baseDir/.paket/load/frameworkDir/scriptName 
let GetPaketLoadScriptLocation baseDir optionalFrameworkDir =
  ReferenceLoading.PaketHandler.GetPaketLoadScriptLocation
    baseDir
    optionalFrameworkDir
    "main.group.fsx"

let GetCommandForTargetFramework targetFramework =
    ReferenceLoading.PaketHandler.MakePackageManagerCommand "fsx" targetFramework