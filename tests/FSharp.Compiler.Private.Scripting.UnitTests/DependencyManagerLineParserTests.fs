// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests

open System.Linq
open FSharp.DependencyManager.Nuget

open Xunit

type DependencyManagerLineParserTests() =

    let parseBinLogPath text =
        let _, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" [text]
        binLogPath

    let parseSingleReference text =
        let packageReferences, _ = FSharpDependencyManager.parsePackageReference ".fsx" [text]
        packageReferences.Single()

    [<Fact>]
    member __.``Binary logging defaults to disabled``() =
        let _, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" []
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member __.``Binary logging can be set to default path``() =
        let binLogPath = parseBinLogPath "bl=true"
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member __.``Binary logging can be disabled``() =
        let binLogPath = parseBinLogPath "bl=false"
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member __.``Binary logging can be set to specific location``() =
        let binLogPath = parseBinLogPath "bl=path/to/file.binlog"
        Assert.Equal(Some(Some "path/to/file.binlog"), binLogPath)

    [<Fact>]
    member __.``Bare binary log argument isn't parsed as a package name: before``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" ["bl, MyPackage"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member __.``Bare binary log argument isn't parsed as a package name: middle``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" ["MyPackage, bl, 1.2.3.4"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal("1.2.3.4", packageReferences.Single().Version)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member __.``Bare binary log argument isn't parsed as a package name: after``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" ["MyPackage, bl"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member __.``Package named 'bl' can be explicitly referenced``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" ["Include=bl"]
        Assert.Equal("bl", packageReferences.Single().Include)
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member __.``Package named 'bl' can be explicitly referenced with binary logging``() =
        let packageReferences, binLogPath = FSharpDependencyManager.parsePackageReference ".fsx" ["Include=bl,bl"]
        Assert.Equal("bl", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member __.``Parse explicitly specified package name``() =
        let pr = parseSingleReference "Include=MyPackage"
        Assert.Equal("MyPackage", pr.Include)

    [<Fact>]
    member __.``Parse implicitly specified package name``() =
        let pr = parseSingleReference "MyPackage"
        Assert.Equal("MyPackage", pr.Include)

    [<Fact>]
    member __.``Parse version number``() =
        let pr = parseSingleReference "MyPackage, Version=1.2.3.4"
        Assert.Equal("1.2.3.4", pr.Version)

    [<Fact>]
    member __.``Parse implicitly specified package name and implicitly specified version number``() =
        let pr = parseSingleReference "MyPackage, 1.2.3.4"
        Assert.Equal("MyPackage", pr.Include)
        Assert.Equal("1.2.3.4", pr.Version)

    [<Fact>]
    member __.``Parse single restore source``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource"
        Assert.Equal("MyRestoreSource", pr.RestoreSources)

    [<Fact>]
    member __.``Parse multiple restore sources``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource1, RestoreSources=MyRestoreSource2"
        Assert.Equal("MyRestoreSource1;MyRestoreSource2", pr.RestoreSources)

    [<Fact>]
    member __.``Parse script``() =
        let pr = parseSingleReference "MyPackage, Script=SomeScript"
        Assert.Equal("SomeScript", pr.Script)

    [<Fact>]
    member __.``Include strings that look different but parse the same are reduced to a single item``() =
        let prs, _ =
            [ "MyPackage, Version=1.2.3.4"
              "Include=MyPackage, Version=1.2.3.4" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        let pr = prs.Single()
        Assert.Equal("MyPackage", pr.Include)
