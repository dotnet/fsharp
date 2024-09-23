// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

// System namespaces
open System
open System.Collections.Generic
open System.Globalization
open System.IO
open System.Text
open System.Text.RegularExpressions

// VS namespaces 
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp
open Microsoft.VisualStudio.FSharp.ProjectSystem

// Internal unittest namespaces
open Xunit
open Salsa
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem

type UpToDate() = 
    inherit TheTests()

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.ItemInputs () =
        this.MakeProjectAndDo(["file1.fs"], [], @"
                <ItemGroup>
                    <Content Include=""content.txt"" />
                    <Resource Include=""resource.txt"" />
                    <EmbeddedResource Include=""embedresource.txt"" />
                    <None Include=""App.config"" />
                    <None Include=""none.txt"" />
                </ItemGroup>
            ", (fun project ->
            let configNameDebug = ConfigCanonicalName("Debug", "x86")
            let config = project.ConfigProvider.GetProjectConfiguration(configNameDebug)
            let output = VsMocks.vsOutputWindowPane(ref [])
            let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)

            let sourcePath = Path.Combine(project.ProjectFolder, "file1.fs")
            let contentPath = Path.Combine(project.ProjectFolder, "content.txt")
            let resourcePath = Path.Combine(project.ProjectFolder, "resource.txt")
            let configPath = Path.Combine(project.ProjectFolder, "App.config")
            let nonePath = Path.Combine(project.ProjectFolder, "none.txt")
            let embedPath = Path.Combine(project.ProjectFolder, "embedresource.txt")

            let startTime = DateTime.UtcNow

            File.AppendAllText(sourcePath, "printfn \"hello\"")
            File.AppendAllText(contentPath, "some content")
            File.AppendAllText(resourcePath, "some resource")
            File.AppendAllText(configPath, """<?xml version="1.0" encoding="utf-8" ?><configuration></configuration>""")
            File.AppendAllText(nonePath, "none")
            File.AppendAllText(embedPath, "some embedded resource")

            Assert.False(config.IsUpToDate(logger, true))
            project.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(config.IsUpToDate(logger, true))

            // None items should not affect up-to-date (unless captured by well-known items, e.g. App.config)
            File.SetLastWriteTime(nonePath, DateTime.UtcNow.AddMinutes(5.))
            Assert.True(config.IsUpToDate(logger, true))

            for path in [sourcePath; contentPath; resourcePath; embedPath; configPath] do
                printfn "Testing path %s" path

                // touch file
                File.SetLastWriteTime(path, DateTime.UtcNow.AddMinutes(5.))
                Assert.False(config.IsUpToDate(logger, true))

                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))

                // delete file
                let originalContent = File.ReadAllText(path)
                File.Delete(path)
                Assert.False(config.IsUpToDate(logger, true))

                File.AppendAllText(path, originalContent)
                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))
            ))

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.PropertyInputs () =
        this.MakeProjectAndDo(["file1.fs"], [], @"
                <PropertyGroup>
                    <VersionFile>ver.txt</VersionFile>
                    <AssemblyOriginatorKeyFile>key.txt</AssemblyOriginatorKeyFile>
                </PropertyGroup>
            ", (fun project ->
            let configNameDebug = ConfigCanonicalName("Debug", "x86")
            let config = project.ConfigProvider.GetProjectConfiguration(configNameDebug)
            let output = VsMocks.vsOutputWindowPane(ref [])
            let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)

            let sourcePath = Path.Combine(project.ProjectFolder, "file1.fs")
            let verPath = Path.Combine(project.ProjectFolder, "ver.txt")
            let keyPath = Path.Combine(project.ProjectFolder, "key.txt")

            let startTime = DateTime.UtcNow

            File.AppendAllText(sourcePath, "printfn \"hello\"")
            File.AppendAllText(verPath, "1.2.3.4")
            File.AppendAllText(keyPath, "a key")

            project.SetConfiguration(config.ConfigCanonicalName);
            Assert.False(config.IsUpToDate(logger, true))
            project.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(config.IsUpToDate(logger, true))

            for path in [verPath; keyPath] do
                printfn "Testing path %s" path

                // touch file
                File.SetLastWriteTime(path, DateTime.UtcNow.AddMinutes(5.))
                Assert.False(config.IsUpToDate(logger, true))

                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))

                // delete file
                let originalContent = File.ReadAllText(path)
                File.Delete(path)
                Assert.False(config.IsUpToDate(logger, true))

                File.AppendAllText(path, originalContent)
                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))
            ))

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.ProjectFile () =
        this.MakeProjectAndDoWithProjectFile(["file1.fs"], [], "", (fun project projFileName ->
            let configNameDebug = ConfigCanonicalName("Debug", "x86")
            let config = project.ConfigProvider.GetProjectConfiguration(configNameDebug)
            let output = VsMocks.vsOutputWindowPane(ref [])
            let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)
            let absFilePath = Path.Combine(project.ProjectFolder, "file1.fs")
            let startTime = DateTime.UtcNow
            File.AppendAllText(absFilePath, "printfn \"hello\"")

            Assert.False(config.IsUpToDate(logger, true))
            project.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(config.IsUpToDate(logger, true))

            // touch proj file
            File.SetLastWriteTime(projFileName, DateTime.UtcNow.AddMinutes(5.))
            Assert.False(config.IsUpToDate(logger, true))

            File.SetLastWriteTime(projFileName, startTime)
            Assert.True(config.IsUpToDate(logger, true))
            ))

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.References () =
        let configNameDebug = ConfigCanonicalName("Debug", "x86")
        let output = VsMocks.vsOutputWindowPane(ref [])
        let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)

        DoWithTempFile "Proj1.fsproj" (fun proj1Path ->
            File.AppendAllText(proj1Path, TheTests.SimpleFsprojText(
                ["File1.fs"], // <Compile>
                [], // <Reference>
                "<PropertyGroup><TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion></PropertyGroup>"))  // other stuff
            use project1 = TheTests.CreateProject(proj1Path)
            let sourcePath1 = Path.Combine(project1.ProjectFolder, "File1.fs")
            File.AppendAllText(sourcePath1, "namespace Proj1\r\n")
            File.AppendAllText(sourcePath1, "module Test =\r\n")
            File.AppendAllText(sourcePath1, "    let X = 5\r\n")

            let config1 = project1.ConfigProvider.GetProjectConfiguration(configNameDebug)

            Assert.False(config1.IsUpToDate(logger, true))
            project1.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(config1.IsUpToDate(logger, true))

            let output1 = Path.Combine(project1.ProjectFolder, "bin\\debug", project1.OutputFileName)

            DoWithTempFile "Proj2.fsproj" (fun proj2Path ->
                File.AppendAllText(proj2Path, TheTests.SimpleFsprojText(
                    ["File2.fs"], // <Compile>
                    [output1], // <Reference>
                    "<PropertyGroup><TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion></PropertyGroup>"))  // other stuff
                use project2 = TheTests.CreateProject(proj2Path)
                let sourcePath2 = Path.Combine(project2.ProjectFolder, "File2.fs")
                File.AppendAllText(sourcePath2, "open Proj1\r\n")
                File.AppendAllText(sourcePath2, "let x = Test.X")

                let config2 = project2.ConfigProvider.GetProjectConfiguration(configNameDebug)
                let startTime = DateTime.UtcNow

                Assert.False(config2.IsUpToDate(logger, true))
                project2.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
                Assert.True(config2.IsUpToDate(logger, true))

                // reference is updated
                File.SetLastWriteTime(output1, DateTime.UtcNow.AddMinutes(5.))
                Assert.False(config2.IsUpToDate(logger, true))
                File.SetLastWriteTime(output1, startTime)
                Assert.True(config2.IsUpToDate(logger, true))

                // reference is missing
                File.Delete(output1)
                Assert.False(config2.IsUpToDate(logger, true))
            )
       )

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.OutputFiles () =
        this.MakeProjectAndDo(["file1.fs"], [], @"
               <ItemGroup>
                   <None Include=""App.config"" />
               </ItemGroup>
               <PropertyGroup>
                   <DocumentationFile>bin\Debug\Test.XML</DocumentationFile>
                   <DebugSymbols>true</DebugSymbols>
                   <DebugType>full</DebugType>
               </PropertyGroup>", (fun project ->
            let configNameDebug = ConfigCanonicalName("Debug", "x86")
            let config = project.ConfigProvider.GetProjectConfiguration(configNameDebug)
            let output = VsMocks.vsOutputWindowPane(ref [])
            let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)
            let sourcePath = Path.Combine(project.ProjectFolder, "file1.fs")
            let appConfigPath = Path.Combine(project.ProjectFolder, "App.config")

            let exeObjPath = Path.Combine(project.ProjectFolder, "obj\\x86\\debug", project.OutputFileName)
            let exeBinpath = Path.Combine(project.ProjectFolder, "bin\\debug\\", project.OutputFileName)
            let pdbObjPath = Regex.Replace(exeObjPath, "exe$", "pdb")
            let pdbBinPath = Regex.Replace(exeBinpath, "exe$", "pdb")
            let xmlDocPath = Regex.Replace(exeBinpath, "exe$", "xml")
            let exeConfigPath = Regex.Replace(exeBinpath, "exe$", "exe.config")

            File.AppendAllText(sourcePath, "printfn \"hello\"")
            File.AppendAllText(appConfigPath, """<?xml version="1.0" encoding="utf-8" ?><configuration></configuration>""")

            Assert.False(config.IsUpToDate(logger, true))
            project.Build(configNameDebug, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(config.IsUpToDate(logger, true))

            let startTime = DateTime.UtcNow

            for path in [exeObjPath; exeBinpath; pdbObjPath; pdbBinPath; xmlDocPath; exeConfigPath] do
                printfn "Testing output %s" path

                // touch file
                File.SetLastWriteTime(path, DateTime.UtcNow.AddMinutes(-5.))
                Assert.False(config.IsUpToDate(logger, true))

                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))

                // delete file
                let originalContent = File.ReadAllBytes(path)
                File.Delete(path)
                Assert.False(config.IsUpToDate(logger, true))

                File.WriteAllBytes(path, originalContent)
                File.SetLastWriteTime(path, startTime)
                Assert.True(config.IsUpToDate(logger, true))
            ))

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.ConfigChanges () =
        this.MakeProjectAndDo(["file1.fs"], [], "", (fun project ->
            let configNameDebugx86 = ConfigCanonicalName("Debug", "x86")
            let configNameReleasex86 = ConfigCanonicalName("Release", "x86")
            let configNameDebugAnyCPU = ConfigCanonicalName("Debug", "AnyCPU")
            let configNameReleaseAnyCPU = ConfigCanonicalName("Release", "AnyCPU")

            let debugConfigx86 = project.ConfigProvider.GetProjectConfiguration(configNameDebugx86)
            let releaseConfigx86 = project.ConfigProvider.GetProjectConfiguration(configNameReleasex86)
            let debugConfigAnyCPU = project.ConfigProvider.GetProjectConfiguration(configNameDebugAnyCPU)
            let releaseConfigAnyCPU = project.ConfigProvider.GetProjectConfiguration(configNameReleaseAnyCPU)

            let output = VsMocks.vsOutputWindowPane(ref [])
            let logger = OutputWindowLogger.CreateUpToDateCheckLogger(output)

            let sourcePath = Path.Combine(project.ProjectFolder, "file1.fs")
            File.AppendAllText(sourcePath, "printfn \"hello\"")

            Assert.False(debugConfigx86.IsUpToDate(logger, true))
            Assert.False(releaseConfigx86.IsUpToDate(logger, true))
            Assert.False(debugConfigAnyCPU.IsUpToDate(logger, true))
            Assert.False(releaseConfigAnyCPU.IsUpToDate(logger, true))

            project.Build(configNameDebugx86, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(debugConfigx86.IsUpToDate(logger, true))
            Assert.False(releaseConfigx86.IsUpToDate(logger, true))
            Assert.False(debugConfigAnyCPU.IsUpToDate(logger, true))
            Assert.False(releaseConfigAnyCPU.IsUpToDate(logger, true))

            project.Build(configNameReleasex86, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(debugConfigx86.IsUpToDate(logger, true))
            Assert.True(releaseConfigx86.IsUpToDate(logger, true))
            Assert.False(debugConfigAnyCPU.IsUpToDate(logger, true))
            Assert.False(releaseConfigAnyCPU.IsUpToDate(logger, true))

            project.Build(configNameDebugAnyCPU, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(debugConfigx86.IsUpToDate(logger, true))
            Assert.True(releaseConfigx86.IsUpToDate(logger, true))
            Assert.True(debugConfigAnyCPU.IsUpToDate(logger, true))
            Assert.False(releaseConfigAnyCPU.IsUpToDate(logger, true))

            project.Build(configNameReleaseAnyCPU, output, "Build", null) |> AssertBuildSuccessful
            Assert.True(debugConfigx86.IsUpToDate(logger, true))
            Assert.True(releaseConfigx86.IsUpToDate(logger, true))
            Assert.True(debugConfigAnyCPU.IsUpToDate(logger, true))
            Assert.True(releaseConfigAnyCPU.IsUpToDate(logger, true))
            ))

    [<Fact>]
    member public this.UTDCheckEnabled () =
        this.MakeProjectAndDo(["file1.fs"], [], @"
                <PropertyGroup>
                    <DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>
                </PropertyGroup>
            ", (fun project ->
            let configNameDebug = ConfigCanonicalName("Debug", "x86")
            let config = project.ConfigProvider.GetProjectConfiguration(configNameDebug)

            Assert.False(config.IsFastUpToDateCheckEnabled())
            ))

    [<Fact>]
    member public this.UTDOptionsFlags () =
        this.MakeProjectAndDo(["file1.fs"], [], "", (fun project ->
            let configNameDebugx86 = ConfigCanonicalName("Debug", "x86")
            let debugConfigx86 = project.ConfigProvider.GetProjectConfiguration(configNameDebugx86)            
            let buildableConfig = 
                match debugConfigx86.get_BuildableProjectCfg() with
                | 0, bc -> bc
                | _ -> failwith "get_BuildableProjectCfg failed"

            let testFlag flag expected =            
                let supported = Array.zeroCreate<int> 1
                let ready = Array.zeroCreate<int> 1
                buildableConfig.QueryStartUpToDateCheck(flag, supported, ready) |> ignore
                Assert.True(supported.[0] = expected)
                Assert.True(ready.[0] = expected)

            [ VSConstants.VSUTDCF_DTEEONLY, 1
              VSConstants.VSUTDCF_PACKAGE, 0
              VSConstants.VSUTDCF_PRIVATE, 1
              VSConstants.VSUTDCF_REBUILD, 1 ]
            |> List.iter (fun (flag, expected) -> testFlag flag expected)
          ))

    [<Fact>]
    member public this.UTDOptionsFlagsUTDDisabled () =
        this.MakeProjectAndDo(["file1.fs"], [],  @"
                <PropertyGroup>
                    <DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>
                </PropertyGroup>
            ", (fun project ->
            let configNameDebugx86 = ConfigCanonicalName("Debug", "x86")
            let debugConfigx86 = project.ConfigProvider.GetProjectConfiguration(configNameDebugx86)            
            let buildableConfig = 
                match debugConfigx86.get_BuildableProjectCfg() with
                | 0, bc -> bc
                | _ -> failwith "get_BuildableProjectCfg failed"

            let testFlag flag expected =            
                let supported = Array.zeroCreate<int> 1
                let ready = Array.zeroCreate<int> 1
                buildableConfig.QueryStartUpToDateCheck(flag, supported, ready) |> ignore
                Assert.Equal(supported.[0], expected)
                Assert.Equal(ready.[0], expected)

            [ VSConstants.VSUTDCF_DTEEONLY, 1
              VSConstants.VSUTDCF_PACKAGE, 0
              VSConstants.VSUTDCF_PRIVATE, 0
              VSConstants.VSUTDCF_REBUILD, 0 ]
            |> List.iter (fun (flag, expected) -> testFlag flag expected)
          ))

type ``UpToDate PreserveNewest`` () = 

    [<Fact>]
    member public this.IsUpToDatePreserveNewest () =

        let test (input, inputTimestamp) (output, outputTimestamp) =
            let logs = ref []
            let outputPanel = VsMocks.vsOutputWindowPane(logs)
            let logger = OutputWindowLogger((fun () -> true), outputPanel)
        
            let tryTimestamp (path: string) (_l: OutputWindowLogger) =
                let toN = function Some d -> Nullable<_>(d) | None -> Nullable<_>()
                match path with
                | x when x = input -> toN inputTimestamp
                | x when x = output -> toN outputTimestamp
                | _ -> failwithf "unexpected %s" path

            let u = ProjectConfig.IsUpToDatePreserveNewest(logger, (Func<_,_,_>(tryTimestamp)), input, output)
            u, !logs
            
        let now = System.DateTime.UtcNow
        let before = now.AddHours(-1.0)

        let ``no input -> not up-to-date and log`` =
            let u, logs = test ("readme.md", None) ("leggimi.md", None)
            Assert.False(u)
            logs
            |> List.exists (fun s -> s.Contains("readme.md") && s.Contains("can't find expected input"))
            |> Assert.True

        let ``no output -> not up-to-date and log`` =
            let u, logs = test ("from.txt", Some now) ("to.txt", None)
            Assert.False(u)
            logs
            |> List.exists (fun s -> s.Contains("to.txt") && s.Contains("can't find expected output"))
            |> Assert.True

        let ``a newer version of output file is ok`` =
            let u, logs = test ("before.doc", Some before) ("after.doc", Some now)
            Assert.True(u)
            logs |> AssertEqual []

        let ``stale output file -> not up-to-date and log`` =
            let u, logs = test ("logo.png", Some now) ("animatedlogo.gif", Some before)
            Assert.False(u)
            logs
            |> List.exists (fun s -> s.Contains("animatedlogo.gif") && s.Contains("stale"))
            |> Assert.True
            
        ()        
