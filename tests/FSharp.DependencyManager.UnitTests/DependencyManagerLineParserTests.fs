// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests

open System.Linq
open FSharp.DependencyManager
open NUnit.Framework

[<TestFixture>]
type DependencyManagerLineParserTests() =

    let parseBinLoggingFlag text =
        let _, binLogging = FSharpDependencyManager.parsePackageReference [text]
        binLogging

    let parseSingleReference text =
        let packageReferences, _ = FSharpDependencyManager.parsePackageReference [text]
        packageReferences.Single()

    [<Test>]
    member __.``Binary logging defaults to false``() =
        let _, binLogging = FSharpDependencyManager.parsePackageReference []
        Assert.False(binLogging)

    [<Test>]
    member __.``Binary logging can be set to true``() =
        let binLogging = parseBinLoggingFlag "bl=true"
        Assert.True(binLogging)

    [<Test>]
    member __.``Binary logging can be set to false``() =
        let binLogging = parseBinLoggingFlag "bl=false"
        Assert.False(binLogging)

    [<Test>]
    member __.``Parse explicitly specified package name``() =
        let pr = parseSingleReference "Include=MyPackage"
        Assert.AreEqual("MyPackage", pr.Include)

    [<Test>]
    member __.``Parse implicitly specified package name``() =
        let pr = parseSingleReference "MyPackage"
        Assert.AreEqual("MyPackage", pr.Include)

    [<Test>]
    member __.``Parse version number``() =
        let pr = parseSingleReference "MyPackage, Version=1.2.3.4"
        Assert.AreEqual("1.2.3.4", pr.Version)

    [<Test>]
    member __.``Parse implicitly specified package name and implicitly specified version number``() =
        let pr = parseSingleReference "MyPackage, 1.2.3.4"
        Assert.AreEqual("MyPackage", pr.Include)
        Assert.AreEqual("1.2.3.4", pr.Version)

    [<Test>]
    member __.``Parse single restore source``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource"
        Assert.AreEqual("MyRestoreSource", pr.RestoreSources)

    [<Test>]
    member __.``Parse multiple restore sources``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource1, RestoreSources=MyRestoreSource2"
        Assert.AreEqual("MyRestoreSource1;MyRestoreSource2", pr.RestoreSources)

    [<Test>]
    member __.``Parse script``() =
        let pr = parseSingleReference "MyPackage, Script=SomeScript"
        Assert.AreEqual("SomeScript", pr.Script)
