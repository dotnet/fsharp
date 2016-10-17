// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Layout the nuget package for the fsharp compiler
//=========================================================================================

#load "../../buildtools/scriptlib.fsx"

try

    //=========================================================================================
    // Command line arguments

    let usage = "usage: layoutfscorenuget.fsx -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let verbose     = getCmdLineArg    "--verbosity:"  "normal"
    let nuspec      = getCmdLineArg    "--nuspec:"     ""
    let bindir      = getCmdLineArg    "--bindir:"     ""
    let nuspecTitle = getBasename nuspec
    let layouts     = getFullPath bindir ++ "layouts" ++ nuspecTitle
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Layout nuget package

    let fsharpCoreFiles =
        [ bindir ++ "FSharp.Core.xml"
          bindir ++ "FSharp.Core.dll"
          bindir ++ "FSharp.Core.sigdata"
          bindir ++ "FSharp.Core.optdata"
          __SOURCE_DIRECTORY__ ++ "FSharp.Core.runtimeconfig.json" ]

    //Clean intermediate directory
    deleteDirectory layouts
    makeDirectory layouts

    for source in fsharpCoreFiles do 
        copyFile source layouts

    exit 0

with e -> 
    printfn "Exception:  %s" e.Message
    printfn "Stacktrace: %s" e.StackTrace
    exit 1
