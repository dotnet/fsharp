// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

// System namespaces
open System
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq
open NUnit.Framework

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


[<SetUpFixture>]
type public AssemblyResolverTestFixture () =

    [<OneTimeSetUp>]
    member public __.Init () = AssemblyResolver.addResolver ()

[<TestFixture>][<Category "ProjectSystem">]
type Config() = 
    inherit TheTests()

    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore

    [<Test>]
    member this.TargetPlatform () =
        this.MakeProjectAndDoWithProjectFileAndConfigChangeNotifier(["foo.fs"], [], 
            this.MSBuildProjectMulitplatBoilerplate "Library",  
            (fun project ccn projFileName ->
                ccn((project :> IVsHierarchy), "Debug|x86")
                project.ComputeSourcesAndFlags()
                let flags = project.CompilationOptions |> List.ofArray
                Assert.IsTrue(List.exists (fun s -> s = "--platform:x86") flags)
                ()
        ))

    [<Test>]
    member this.``Configs.EnsureAtLeastOneConfiguration`` () =
        this.HelperEnsureAtLeastOne 
            @"<PropertyGroup Condition="" '$(Platform)' == 'x86' "" />" 
            [|"Debug"|]  // the goal of the test - when no configs, "Debug" should magically appear
            [|"x86"|]

    [<Test>]
    member this.``Configs.EnsureAtLeastOnePlatform`` () =
        this.HelperEnsureAtLeastOne 
            @"<PropertyGroup Condition="" '$(Configuration)' == 'Release' "" />"
            [|"Release"|]
            [|"Any CPU"|] // the goal of the test - when no platforms, "AnyCPU" should magically appear

    [<Test>]
    member this.``Configs.EnsureAtLeastOneConfigurationAndPlatform`` () =
        this.HelperEnsureAtLeastOne 
            ""
            [|"Debug"|] // first goal of the test - when no configs, "Debug" should magically appear
            [|"Any CPU"|] // second goal of the test - when no platforms, "AnyCPU" should magically appear

    [<Test>]
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

    [<Test>]
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
