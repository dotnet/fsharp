// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests


open System
open System.Linq
open FSharp.DependencyManager.Nuget

open Xunit

type DependencyManagerLineParserTests() =

    let parseBinLogPath text =
        let _, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" [text]
        binLogPath

    let parseSingleReference text =
        let packageReferences, _, _ = FSharpDependencyManager.parsePackageReference ".fsx" [text]
        packageReferences.Single()

    [<Fact>]
    member _.``Binary logging defaults to disabled``() =
        let _, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" []
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member _.``Binary logging can be set to default path``() =
        let binLogPath = parseBinLogPath "bl=true"
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member _.``Binary logging can be disabled``() =
        let binLogPath = parseBinLogPath "bl=false"
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member _.``Binary logging can be set to specific location``() =
        let binLogPath = parseBinLogPath "bl=path/to/file.binlog"
        Assert.Equal(Some(Some "path/to/file.binlog"), binLogPath)

    [<Fact>]
    member _.``Bare binary log argument isn't parsed as a package name: before``() =
        let packageReferences, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" ["bl, MyPackage"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member _.``Bare binary log argument isn't parsed as a package name: middle``() =
        let packageReferences, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" ["MyPackage, bl, 1.2.3.4"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal("1.2.3.4", packageReferences.Single().Version)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member _.``Bare binary log argument isn't parsed as a package name: after``() =
        let packageReferences, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" ["MyPackage, bl"]
        Assert.Equal("MyPackage", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member _.``Package named 'bl' can be explicitly referenced``() =
        let packageReferences, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" ["Include=bl"]
        Assert.Equal("bl", packageReferences.Single().Include)
        Assert.Equal(None, binLogPath)

    [<Fact>]
    member _.``Package named 'bl' can be explicitly referenced with binary logging``() =
        let packageReferences, binLogPath, _ = FSharpDependencyManager.parsePackageReference ".fsx" ["Include=bl,bl"]
        Assert.Equal("bl", packageReferences.Single().Include)
        Assert.Equal(Some (None: string option), binLogPath)

    [<Fact>]
    member _.``Parse explicitly specified package name``() =
        let pr = parseSingleReference "Include=MyPackage"
        Assert.Equal("MyPackage", pr.Include)

    [<Fact>]
    member _.``Parse implicitly specified package name``() =
        let pr = parseSingleReference "MyPackage"
        Assert.Equal("MyPackage", pr.Include)

    [<Fact>]
    member _.``Parse version number``() =
        let pr = parseSingleReference "MyPackage, Version=1.2.3.4"
        Assert.Equal("1.2.3.4", pr.Version)

    [<Fact>]
    member _.``Parse implicitly specified package name and implicitly specified version number``() =
        let pr = parseSingleReference "MyPackage, 1.2.3.4"
        Assert.Equal("MyPackage", pr.Include)
        Assert.Equal("1.2.3.4", pr.Version)

    [<Fact>]
    member _.``Parse single restore source``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource"
        Assert.Equal("MyRestoreSource", pr.RestoreSources)

    [<Fact>]
    member _.``Parse multiple restore sources``() =
        let pr = parseSingleReference "MyPackage, RestoreSources=MyRestoreSource1, RestoreSources=MyRestoreSource2"
        Assert.Equal("MyRestoreSource1;MyRestoreSource2", pr.RestoreSources)

    [<Fact>]
    member _.``Parse script``() =
        let pr = parseSingleReference "MyPackage, Script=SomeScript"
        Assert.Equal("SomeScript", pr.Script)

    [<Fact>]
    member _.``Include strings that look different but parse the same are reduced to a single item``() =
        let prs, _, _ =
            [ "MyPackage, Version=1.2.3.4"
              "Include=MyPackage, Version=1.2.3.4" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        let pr = prs.Single()
        Assert.Equal("MyPackage", pr.Include)

    [<Fact>]
    member _.``Timeout none is -1``() =
        let _, _, timeout =
            [ "timeout=none" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        Assert.Equal(timeout, Some -1)

    [<Fact>]
    member _.``Timeout 1000 is 1000``() =
        let _, _, timeout =
            [ "timeout=1000" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        Assert.Equal(timeout, Some 1000)

    [<Fact>]
    member _.``Timeout 0 is 0``() =
        let _, _, timeout =
            [ "timeout=0" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        Assert.Equal(timeout, Some 0)

    [<Fact>]
    member _.``Timeout for many values is the last specified ascending``() =
        let _, _, timeout =
            [ "timeout=none"
              "timeout=1"
              "timeout=10"
              "timeout=100"
             ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        Assert.Equal(timeout, Some 100)

    [<Fact>]
    member _.``Timeout for many values is the last specified descending``() =
        let _, _, timeout =
            [ "timeout=10000"
              "timeout=1000"
              "timeout=100"
              "timeout=10"
             ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" 
        Assert.Equal(timeout, Some 10)

    [<Fact>]
    member _.``Timeout invalid : timeout``() =
        try
            [ "timeout" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" |> ignore
            Assert.True(false, "ArgumentException expected")                    //Assert.Fail
        with
        | :? ArgumentException -> ()
        | _ -> Assert.True(false, "ArgumentException expected")                 //Assert.Fail

    [<Fact>]
    member _.``Timeout invalid timeout=``() =
        try
            [ "timeout=" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx" |> ignore 
            Assert.True(false, "ArgumentException expected")                    //Assert.Fail
        with
        | :? ArgumentException -> ()
        | _ -> Assert.True(false, "ArgumentException expected")                 //Assert.Fail

    [<Fact>]
    member _.``Timeout invalid timeout=nonesuch``() =
        try
            [ "timeout=nonesuch" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx"  |> ignore
            Assert.True(false, "ArgumentException expected")                    //Assert.Fail
        with
        | :? ArgumentException -> ()
        | _ -> Assert.True(false, "ArgumentException expected")                 //Assert.Fail


    [<Fact>]
    member _.``Timeout invalid timeout=-20``() =
        try
            [ "timeout=-20" ]
            |> FSharpDependencyManager.parsePackageReference ".fsx"  |> ignore
            Assert.True(false, "ArgumentException expected")                    //Assert.Fail
        with
        | :? ArgumentException -> ()
        | _ -> Assert.True(false, "ArgumentException expected")                 //Assert.Fail
