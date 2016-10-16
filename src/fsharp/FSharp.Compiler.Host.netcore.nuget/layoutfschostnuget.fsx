// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Layout the nuget host package for the fsharp compiler
//=========================================================================================
open System.IO

#load "../../buildtools/scriptlib.fsx"

try
    //=========================================================================================
    // Command line arguments

    let usage = "usage: layoutfcsnhostnuget.fsx -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let verbose     = getCmdLineArg    "--verbosity:"  "normal"
    let nuspec      = getCmdLineArg    "--nuspec:"     ""
    let bindir      = getCmdLineArg    "--bindir:"     ""
    let nuspecTitle = getBasename nuspec
    let layouts     = getFullPath bindir ++ "layouts" ++ nuspecTitle
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Layout nuget package

    //Clean intermediate directory
    deleteDirectory layouts
    makeDirectory layouts
    exit 0

with e -> 
    printfn "Exception:  %s" e.Message
    printfn "Stacktrace: %s" e.StackTrace
    exit 1
