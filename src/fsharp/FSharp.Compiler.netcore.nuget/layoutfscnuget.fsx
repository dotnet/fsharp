// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Layout the nuget package for the fsharp compiler
//=========================================================================================

#load "../../buildtools/scriptlib.fsx"

try
    //=========================================================================================
    // Command line arguments

    let usage = "usage: layoutfcsnuget.fsx -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let verbose     = getCmdLineArg    "--verbosity:"  "normal"
    let nuspec      = getCmdLineArg    "--nuspec:"     ""
    let bindir      = getCmdLineArg    "--bindir:"     ""
    let nuspecTitle = getBasename nuspec
    printfn ">>%s<<" bindir
    printfn ">>%s<<" nuspecTitle
    let layouts     = getFullPath bindir ++ "layouts" ++ nuspecTitle
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Layout nuget package

    let fsharpCompilerFiles =
        [  bindir ++ "fsc.exe"
           bindir ++ "FSharp.Compiler.dll"
           bindir ++ "default.win32manifest"
           bindir ++ "fsi.exe"
           bindir ++ "FSharp.Compiler.Interactive.Settings.dll"
           bindir ++ "FSharp.Build.dll"
           bindir ++ "Microsoft.FSharp.targets"
           bindir ++ "Microsoft.Portable.FSharp.targets" ]
    
    //Clean intermediate directoriy
    deleteDirectory layouts
    makeDirectory layouts
    for source in fsharpCompilerFiles do 
        copyFile source layouts
    exit 0

with e -> 
    printfn "Exception:  %s" e.Message
    printfn "Stacktrace: %s" e.StackTrace
    exit 1
