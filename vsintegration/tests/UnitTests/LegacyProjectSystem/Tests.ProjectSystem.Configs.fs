// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

// System namespaces
open System
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq
open Xunit

// VS namespaces 
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp
open Microsoft.VisualStudio.FSharp.ProjectSystem

// Internal unittest namespaces
open Salsa
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem


type Config() = 
    inherit TheTests()

    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore

    [<Fact>]
    member this.TargetPlatform () =
        this.MakeProjectAndDoWithProjectFileAndConfigChangeNotifier(["foo.fs"], [], 
            this.MSBuildProjectMultiPlatformBoilerplate "Library",  
            (fun project ccn projFileName ->
                ccn((project :> IVsHierarchy), "Debug|x86")
                project.ComputeSourcesAndFlags()
                let flags = project.CompilationOptions |> List.ofArray
                Assert.True(List.exists (fun s -> s = "--platform:x86") flags)
                ()
        ))

    [<Fact>]
    member this.``Configs.EnsureAtLeastOneConfiguration`` () =
        this.HelperEnsureAtLeastOne 
            @"<PropertyGroup Condition="" '$(Platform)' == 'x86' "" />" 
            [|"Debug"|]  // the goal of the test - when no configs, "Debug" should magically appear
            [|"x86"|]

    [<Fact>]
    member this.``Configs.EnsureAtLeastOnePlatform`` () =
        this.HelperEnsureAtLeastOne 
            @"<PropertyGroup Condition="" '$(Configuration)' == 'Release' "" />"
            [|"Release"|]
            [|"Any CPU"|] // the goal of the test - when no platforms, "AnyCPU" should magically appear

    [<Fact>]
    member this.``Configs.EnsureAtLeastOneConfigurationAndPlatform`` () =
        this.HelperEnsureAtLeastOne 
            ""
            [|"Debug"|] // first goal of the test - when no configs, "Debug" should magically appear
            [|"Any CPU"|] // second goal of the test - when no platforms, "AnyCPU" should magically appear

    [<Fact>]
    member this.``Configs.EnsureAtLeastOneConfiguration.Imported`` () =
        // Take advantage of the fact that we always create projects one directory below TempPath
        let tmpTargets = Path.Combine(Path.GetTempPath(), "foo.targets")
        try
            File.AppendAllText(tmpTargets,
                @"<Project ToolsVersion='4.0' DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                    <PropertyGroup Condition="" '$(Platform)' == 'x86' "" />
                  </Project>")

            this.HelperEnsureAtLeastOne 
                @"<Import Project=""..\foo.targets"" />"
                [|"Debug"|]  // the goal of the test - when no configs, "Debug" should magically appear
                [|"x86"|]
        finally
            File.Delete tmpTargets

    [<Fact>]
    member this.``Configs.EnsureAtLeastOnePlatform.Imported`` () =
        // Take advantage of the fact that we always create projects one directory below TempPath
        // The unit test failed due to the previous test use the same target name "foo.targets". 
        // The vs record the platform config "x86", so the next test failed for the unexpected platform.
        let tmpTargets = Path.Combine(Path.GetTempPath(), "foo2.targets")
        try
            File.AppendAllText(tmpTargets,
                @"<Project ToolsVersion='4.0' DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                    <PropertyGroup Condition="" '$(Configuration)' == 'Release' "" />
                  </Project>")
            this.HelperEnsureAtLeastOne 
                @"<Import Project=""..\foo2.targets"" />"
                [|"Release"|]
                [|"Any CPU"|] // the goal of the test - when no platforms, "AnyCPU" should magically appear
        finally
            File.Delete tmpTargets
    
    [<Fact>]
    member this.``Configs.Renaming`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiConfigBoilerplate ["Debug",""; "Release",""],
            (fun project projFileName ->
                this.CheckConfigNames(project, [|"Debug"; "Release"|])
                project.ConfigProvider.RenameCfgsOfCfgName("Debug", "Buggy") |> AssertEqual VSConstants.S_OK
                this.CheckConfigNames(project, [|"Buggy";"Release"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiConfigBoilerplate ["Buggy",""; "Release",""]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                
            ))
            
    [<Fact>]
    member this.``Configs.Deleting`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiConfigBoilerplate ["Debug",""; "Release",""],
            (fun project projFileName ->
                this.CheckConfigNames(project, [|"Debug";"Release"|])
                project.ConfigProvider.DeleteCfgsOfCfgName("Debug") |> AssertEqual VSConstants.S_OK
                this.CheckConfigNames(project, [|"Release"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiConfigBoilerplate ["Release",""]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                
            ))
    [<Fact>]
    member this.``Configs.Adding`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiConfigBoilerplate ["Debug","<Foo/>"; "Release",""],
            (fun project projFileName ->    
                this.CheckConfigNames(project, [|"Debug";"Release"|])
                project.ConfigProvider.AddCfgsOfCfgName("Buzz", "Debug", 0) |> AssertEqual VSConstants.S_OK
                this.CheckConfigNames(project, [|"Debug";"Release";"Buzz"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiConfigBoilerplate ["Debug","<Foo/>"; "Release",""; "Buzz","<Foo/>"]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                
            ))
    [<Fact>]
    member this.``Configs.AddingBaseless`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiConfigBoilerplate ["Debug","<Foo/>"; "Release",""],
            (fun project projFileName ->
                this.CheckConfigNames(project, [|"Debug";"Release"|])
                project.ConfigProvider.AddCfgsOfCfgName("Buzz", null, 0) |> AssertEqual VSConstants.S_OK
                this.CheckConfigNames(project, [|"Debug";"Release";"Buzz"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiConfigBoilerplate ["Debug","<Foo/>"; "Release",""; "Buzz",""]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                
            ))

    [<Fact>]
    member this.``Configs.Platforms.Deleting`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiPlatform ["Any CPU",""; "x86",""],
            (fun project projFileName ->
                this.CheckPlatformNames(project, [|"Any CPU";"x86"|])
                project.ConfigProvider.DeleteCfgsOfPlatformName("Any CPU") |> AssertEqual VSConstants.S_OK
                this.CheckPlatformNames(project, [|"x86"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiPlatform ["x86",""]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                

        ))
        
    [<Fact>]
    member this.``Configs.Platforms.Adding`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiPlatform ["Any CPU",""; "x86","<Custom/>"],
            (fun project projFileName ->
                this.CheckPlatformNames(project, [|"Any CPU";"x86"|])
                project.ConfigProvider.AddCfgsOfPlatformName("x64", "x86") |> AssertEqual VSConstants.S_OK
                this.CheckPlatformNames(project, [|"Any CPU";"x86";"x64"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiPlatform ["Any CPU",""; "x86","<Custom/>"; "x64","<Custom/>"]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                

        ))

    [<Fact>]
    member this.``Configs.Platforms.AddingBaseless`` () =
        this.MakeProjectAndDoWithProjectFile(["foo.fs"], [],
            this.MSBuildProjectMultiPlatform ["Any CPU",""; "x86",""],
            (fun project projFileName ->
                this.CheckPlatformNames(project, [|"Any CPU";"x86"|])
                project.ConfigProvider.AddCfgsOfPlatformName("x64", null) |> AssertEqual VSConstants.S_OK
                this.CheckPlatformNames(project, [|"Any CPU";"x86";"x64"|])
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(projFileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let expected = this.MSBuildProjectMultiPlatform ["Any CPU",""; "x86",""; "x64",""]
                let expectedXDoc = XDocument.Load(new StringReader(TheTests.SimpleFsprojText(["foo.fs"],[],expected)))
                TheTests.AssertSimilarXml(expectedXDoc.Root, xDoc.Root)                

        ))
