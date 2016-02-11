// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests.PowerPack

open NUnit.Framework
open System
open System.IO
open System.Diagnostics
open Microsoft.FSharp.Build
open Microsoft.Build.BuildEngine
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open UnitTests.TestLib.Utils.FilesystemHelpers

[<TestFixture>]
type FsLexTests() = 
    
    [<SetUp>]
    member this.Setup() = ()
        
    [<TearDown>]
    member this.TearDown() = ()
        
    [<Test>]
    member public this.TestCodePage() =
        let tool = new Microsoft.FSharp.Build.FsLex()
        tool.CodePage <- "65001"
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("--codepage 65001 ", cmd)

    [<Test>]
    member public this.TestOutputFile() =
        let tool = new Microsoft.FSharp.Build.FsLex()
        tool.OutputFile <- "result.fs"
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("-o result.fs ", cmd)

    [<Test>]
    member public this.TestUnicode() =
        let tool = new Microsoft.FSharp.Build.FsLex()
        tool.Unicode <- true
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("--unicode ", cmd)

    [<Test>]
    member public this.TestUnicodeNegCase() =
        let tool = new Microsoft.FSharp.Build.FsLex()
        tool.Unicode <- false
        let cmd = tool.InternalGenerateCommandLineCommands()
        // Verify Unicode flag not specified
        Assert.AreEqual("", cmd)

[<TestFixture>]
type FsYaccTests() = 
    
    [<SetUp>]
    member this.Setup() = ()
        
    [<TearDown>]
    member this.TearDown() = ()
        
    [<Test>]
    member public this.TestCodePage() =
        let tool = new Microsoft.FSharp.Build.FsYacc()
        tool.CodePage <- "65001"
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("--codepage 65001", cmd)

    [<Test>]
    member public this.TestOutputFile() =
        let tool = new Microsoft.FSharp.Build.FsYacc()
        tool.OutputFile <- "result.fs"
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("-o result.fs", cmd)

    [<Test>]
    member public this.TestMLCompatibility() =
        let tool = new Microsoft.FSharp.Build.FsYacc()
        tool.MLCompatibility <- true
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("--ml-compatibility", cmd)
        
    [<Test>]
    member public this.TestMLCompatibilityFalse() =
        let tool = new Microsoft.FSharp.Build.FsYacc()
        tool.MLCompatibility <- false
        let cmd = tool.InternalGenerateCommandLineCommands()
        Assert.AreEqual("", cmd)