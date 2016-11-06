// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Build nuget package for the fsharp compiler
//=========================================================================================

#load "scriptlib.fsx"

try

    //=========================================================================================
    // Command line arguments

    let usage = "usage: BuildNuGets.fsx --version:<build-version> -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let verbose     = getCmdLineArg    "--verbosity:"  "normal"
    let version     = getCmdLineArg    "--version:"    ""
    let nuspec      = getCmdLineArg    "--nuspec:"     ""
    let bindir      = getCmdLineArg    "--bindir:"     ""
    let nuspecTitle = getBasename nuspec
    let layouts     = getFullPath bindir ++ "layouts" ++ nuspecTitle
    let output      = getFullPath bindir ++ "nuget"
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Invoke nuget.exe to build nuget package

    let author =     "Microsoft"
    let licenseUrl = "https://github.com/Microsoft/visualfsharp/blob/master/License.txt"
    let projectUrl = "https://github.com/Microsoft/visualfsharp"
    let tags =       "Visual F# Compiler FSharp coreclr functional programming"

    let nugetArgs = sprintf "pack %s -BasePath \"%s\" -OutputDirectory \"%s\" -ExcludeEmptyDirectories -prop licenseUrl=\"%s\" -prop version=\"%s\" -prop authors=\"%s\" -prop projectURL=\"%s\" -prop tags=\"%s\" -Verbosity detailed"
                            nuspec
                            layouts
                            output
                            licenseUrl
                            version
                            author
                            projectUrl
                            tags

    let nugetExePath = getFullPath (__SOURCE_DIRECTORY__ ++ "../../.nuget/nuget.exe")
    makeDirectory output
    exit (executeProcess nugetExePath nugetArgs) 
with _ -> 
    exit 1
