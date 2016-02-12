// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests
open NUnit.Framework
open System
open System.IO
open System.Diagnostics
open UnitTests.TestLib.Utils
open Microsoft.BuildSettings

#if OPEN_BUILD
#else
[<TestFixture>]
type Script() = 
    let replaceIfNotNull (search:string) (replace:string) (s:string) =
        match s with
        | null -> s
        | s -> s.Replace(search,replace) 
    let replaceConstants(line:string) = 
        line
        |> replaceIfNotNull Version.OfFile "{VersionOfFile}"
        |> replaceIfNotNull Version.OfAssembly "{VersionOfAssembly}"
        |> replaceIfNotNull Version.ProductBuild "{VersionOfProductBuild}"
        |> replaceIfNotNull "4.0" "{DotNetMajorMinor}"
        |> replaceIfNotNull "4.5" "{DotNetMajorMinor}"
        |> replaceIfNotNull (sprintf "%s.%s" Version.Major Version.Minor) "{DotNetMajorMinor}"
        |> replaceIfNotNull "10.0" "{VsMajorMinor}"
        |> replaceIfNotNull "F# 3.0" "F# {FSharpCompilerVersion}"
        |> replaceIfNotNull (Environment.GetEnvironmentVariable("ProgramFiles")) "{ProgramFiles}"

    let runCheck(script:string,baseline:string) =
        let code,lines = Spawn.Batch script
        let combinedLines = String.Join("\r\n",lines).Trim([|'\r';'\n'|]) |> replaceConstants
        let baseline = baseline.Trim([|'\r';'\n'|])
        if baseline<>combinedLines then 
            for line in lines do
                printfn "%s" (replaceConstants line)
            Assert.AreEqual(baseline,combinedLines)

    [<Test>]
    member public __.NetModules_Bug915449() =
        let script = @"
@echo off
echo>a.cs public class A {}
echo>b.cs public class B {}
echo>r.fs let a = new A()
echo>>r.fs let b = new B()
echo>>r.fs System.Console.WriteLine(a.GetType())
echo>>r.fs System.Console.WriteLine(b.GetType())
csc /nologo /t:module a.cs
csc /nologo /t:module b.cs
al /nologo /out:c.dll a.netmodule b.netmodule
fsc /nologo /r:c.dll r.fs
"
        let baseline = @""
        runCheck(script,baseline)
#endif