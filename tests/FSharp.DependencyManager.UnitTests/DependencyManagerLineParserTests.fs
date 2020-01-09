// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests

open System.Linq
open FSharp.DependencyManager
open NUnit.Framework

[<TestFixture>]
type DependencyManagerLineParserTests() =

    let parseBinLogPath text =
        let _, binLogPath = FSharpDependencyManager.parsePackageReference [text]
        binLogPath

    let parseSingleReference text =
        let packageReferences, _ = FSharpDependencyManager.parsePackageReference [text]
        packageReferences.Single()

    [<Test>]
    member __.``Binary logging defaults to disabled``() =
        let _, binLogPath = FSharpDependencyManager.parsePackageReference []
        Assert.AreEqual(None, binLogPath)

    [<Test>]
    member __.``Binary logging can be set to default path``() =
        let binLogPath = parseBinLogPath "bl=true"
        Assert.AreEqual(Some (None: string option), binLogPath)

    [<Test>]
    member __.``Binary logging can be disabled``() =
        let binLogPath = parseBinLogPath "bl=false"
        Assert.AreEqual(None, binLogPath)

    [<Test>]
    member __.``Binary logging can be set to specific location``() =
        let binLogPath = parseBinLogPath "bl=path/to/file.binlog"
        Assert.AreEqual(Some(Some "path/to/file.binlog"), binLogPath)

    [<Test>]
    member __.``Bare binary log argument isn't parsed as a package name: before``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ["bl, MyPackage"]
        Assert.AreEqual("MyPackage", packageReferences.Single().Include)
        Assert.AreEqual(Some (None: string option), binLogPath)

    [<Test>]
    member __.``Bare binary log argument isn't parsed as a package name: middle``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ["MyPackage, bl, 1.2.3.4"]
        Assert.AreEqual("MyPackage", packageReferences.Single().Include)
        Assert.AreEqual("1.2.3.4", packageReferences.Single().Version)
        Assert.AreEqual(Some (None: string option), binLogPath)

    [<Test>]
    member __.``Bare binary log argument isn't parsed as a package name: after``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ["MyPackage, bl"]
        Assert.AreEqual("MyPackage", packageReferences.Single().Include)
        Assert.AreEqual(Some (None: string option), binLogPath)

    [<Test>]
    member __.``Package named 'bl' can be explicitly referenced``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ["Include=bl"]
        Assert.AreEqual("bl", packageReferences.Single().Include)
        Assert.AreEqual(None, binLogPath)

    [<Test>]
    member __.``Package named 'bl' can be explicitly referenced with binary logging``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ["Include=bl,bl"]
        Assert.AreEqual("bl", packageReferences.Single().Include)
        Assert.AreEqual(Some (None: string option), binLogPath)

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

    [<Test>]
    member __.``Include strings that look different but parse the same are reduced to a single item``() =
        let prs, _ =
            [ "MyPackage, Version=1.2.3.4"
              "Include=MyPackage, Version=1.2.3.4" ]
            |> FSharpDependencyManager.parsePackageReference
        let pr = prs.Single()
        Assert.AreEqual("MyPackage", pr.Include)
