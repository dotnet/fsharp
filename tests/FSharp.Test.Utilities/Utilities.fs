// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System
open System.IO
open System.Reflection
open System.Collections.Immutable
open System.Diagnostics
open System.Threading
open System.Text
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open TestFramework
open Xunit
open System.Collections.Generic
open FSharp.Compiler.CodeAnalysis
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Xunit.Sdk


type TheoryForNETCOREAPPAttribute() =
    inherit TheoryAttribute()
    #if !NETCOREAPP
        do base.Skip <- "Only NETCOREAPP is supported runtime for this kind of test."
    #endif

type FactForNETCOREAPPAttribute() =
    inherit FactAttribute()
    #if !NETCOREAPP    
        do base.Skip <- "Only NETCOREAPP is supported runtime for this kind of test."
    #endif

type FactForDESKTOPAttribute() =
    inherit FactAttribute()
    #if NETCOREAPP
        do base.Skip <- "NETCOREAPP is not supported runtime for this kind of test, it is intended for DESKTOP only"
    #endif

// This file mimics how Roslyn handles their compilation references for compilation testing
module Utilities =

    type Async with
        static member RunImmediate (computation: Async<'T>, ?cancellationToken) =
            let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
            let ts = TaskCompletionSource<'T>()
            let task = ts.Task
            Async.StartWithContinuations(
                computation,
                (fun k -> ts.SetResult k),
                (fun exn -> ts.SetException exn),
                (fun _ -> ts.SetCanceled()),
                cancellationToken)
            task.Result

    [<RequireQualifiedAccess>]
    type TargetFramework =
        | NetStandard20
        | NetCoreApp31
        | Current

    let private getResourceStream name =
        let assembly = typeof<TargetFramework>.GetTypeInfo().Assembly

        let stream = assembly.GetManifestResourceStream(name);

        match stream with
        | null -> failwith (sprintf "Resource '%s' not found in %s." name assembly.FullName)
        | _ -> stream

    let private getResourceBlob name =
        use stream = getResourceStream name
        let (bytes: byte[]) = Array.zeroCreate (int stream.Length)
        use memoryStream = new MemoryStream (bytes)
        stream.CopyTo(memoryStream)
        bytes

    let inline getTestsDirectory src dir = src ++ dir

    module private TestReferences =
        let testDirectory = lazy ( 
            let path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
            Directory.CreateDirectory(path) |> ignore
            path)

        let private writeToTempDirectory name (image: byte array) =
            let path = Path.Combine(testDirectory.Force(), $"{name}.dll")
            File.WriteAllBytes(path, image)
            path

        [<RequireQualifiedAccess>]
        module NetStandard20 =
            let private System_Collections_Immutable = lazy getResourceBlob  "System.Collections.Immutable.dll"

            module Files =
                let netStandard = lazy writeToTempDirectory "netstandard" TestResources.NetFX.netstandard20.netstandard
                let mscorlib = lazy writeToTempDirectory "mscorlib" TestResources.NetFX.netstandard20.mscorlib
                let systemRuntime = lazy writeToTempDirectory "System.Runtime" TestResources.NetFX.netstandard20.System_Runtime
                let systemCore =  lazy writeToTempDirectory "System.Core" TestResources.NetFX.netstandard20.System_Core
                let systemDynamicRuntime = lazy writeToTempDirectory "System.Core" TestResources.NetFX.netstandard20.System_Dynamic_Runtime
                let systemCollectionsImmutable = lazy writeToTempDirectory "System.Collections.Immutable" (System_Collections_Immutable.Force())

            module References =
                let netStandardRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.netstandard).GetReference(display = "netstandard.dll (netstandard 2.0 ref)")
                let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.mscorlib).GetReference(display = "mscorlib.dll (netstandard 2.0 ref)")
                let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Runtime).GetReference(display = "System.Runtime.dll (netstandard 2.0 ref)")
                let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Core).GetReference(display = "System.Core.dll (netstandard 2.0 ref)")
                let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netstandard 2.0 ref)")
                let systemCollectionsImmutableRef = lazy AssemblyMetadata.CreateFromImage(System_Collections_Immutable.Force()).GetReference(display = "System.Collections.Immutable.dll (netstandard 2.0 ref)")

        [<RequireQualifiedAccess>]
        module NetCoreApp31 =
            let private mscorlib = lazy getResourceBlob "mscorlib.dll"
            let private netstandard = lazy getResourceBlob "netstandard.dll"
            let private System_Console = lazy getResourceBlob  "System.Console.dll"
            let private System_Core = lazy getResourceBlob "System.Core.dll"
            let private System_Dynamic_Runtime = lazy getResourceBlob "System.Dynamic.Runtime.dll"
            let private System_Runtime = lazy getResourceBlob  "System.Runtime.dll"

            module Files =
                let mscorlib = lazy writeToTempDirectory "mscorlib" (mscorlib.Force())
                let netStandard = lazy writeToTempDirectory "netstandard" (netstandard.Force())
                let systemConsole = lazy writeToTempDirectory "System.Console" (System_Console.Force())
                let systemCore =  lazy writeToTempDirectory "System.Core" (System_Core.Force())
                let systemDynamicRuntime = lazy writeToTempDirectory "System.Dynamic.Runtime" (System_Dynamic_Runtime.Force())
                let systemRuntime = lazy writeToTempDirectory "System.Runtime" (System_Runtime.Force())

            module References =
                let netStandardRef = lazy AssemblyMetadata.CreateFromImage(netstandard.Force()).GetReference(display = "netstandard.dll (netcoreapp 3.1 ref)")
                let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(mscorlib.Force()).GetReference(display = "mscorlib.dll (netcoreapp 3.1 ref)")
                let systemConsoleRef = lazy AssemblyMetadata.CreateFromImage(System_Console.Force()).GetReference(display = "System.Console.dll (netcoreapp 3.1 ref)")
                let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(System_Core.Force()).GetReference(display = "System.Core.dll (netcoreapp 3.1 ref)")
                let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(System_Dynamic_Runtime.Force()).GetReference(display = "System.Dynamic.Runtime.dll (netcoreapp 3.1 ref)")
                let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(System_Runtime.Force ()).GetReference(display = "System.Runtime.dll (netcoreapp 3.1 ref)")

    [<RequireQualifiedAccess>]
    module public TargetFrameworkUtil =

        let private config = initialConfig

        // Do a one time dotnet sdk build to compute the proper set of reference assemblies to pass to the compiler
        let private projectFile = """
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>$TARGETFRAMEWORK</TargetFramework>
        <UseFSharpPreview>true</UseFSharpPreview>
        <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
  </PropertyGroup>

  <ItemGroup><Compile Include="Program.fs" /></ItemGroup>
  <ItemGroup><Reference Include="$FSHARPCORELOCATION" /></ItemGroup>
  <ItemGroup Condition="'$(TARGETFRAMEWORK)'=='net472'">
        <Reference Include="System" />
        <Reference Include="System.Runtime" />
        <Reference Include="System.Core.dll" />
        <Reference Include="System.Xml.Linq.dll" />
        <Reference Include="System.Data.DataSetExtensions.dll" />
        <Reference Include="Microsoft.CSharp.dll" />
        <Reference Include="System.Data.dll" />
        <Reference Include="System.Deployment.dll" />
        <Reference Include="System.Drawing.dll" />
        <Reference Include="System.Net.Http.dll" />
        <Reference Include="System.Windows.Forms.dll" />
        <Reference Include="System.Xml.dll" />
  </ItemGroup>

  <Target Name="WriteFrameworkReferences" AfterTargets="AfterBuild">
        <WriteLinesToFile File="FrameworkReferences.txt" Lines="@(ReferencePath)" Overwrite="true" WriteOnlyWhenDifferent="true" />
  </Target>

</Project>"""

        let private directoryBuildProps = """
<Project>
</Project>
"""

        let private directoryBuildTargets = """
<Project>
</Project>
"""

        let private programFs = """
open System

[<EntryPoint>]
let main argv = 0"""

        let private getNetCoreAppReferences =
            let mutable output = [||]
            let mutable errors = [||]
            let mutable cleanUp = true
            let pathToArtifacts = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../.."))
            if Path.GetFileName(pathToArtifacts) <> "artifacts" then failwith "CompilerAssert did not find artifacts directory --- has the location changed????"
            let pathToTemp = Path.Combine(pathToArtifacts, "Temp")
            let projectDirectory = Path.Combine(pathToTemp,Guid.NewGuid().ToString() + ".tmp")
            let pathToFSharpCore = typeof<RequireQualifiedAccessAttribute>.Assembly.Location
            try
                try
                    Directory.CreateDirectory(projectDirectory) |> ignore
                    let projectFileName = Path.Combine(projectDirectory, "ProjectFile.fsproj")
                    let programFsFileName = Path.Combine(projectDirectory, "Program.fs")
                    let directoryBuildPropsFileName = Path.Combine(projectDirectory, "Directory.Build.props")
                    let directoryBuildTargetsFileName = Path.Combine(projectDirectory, "Directory.Build.targets")
                    let frameworkReferencesFileName = Path.Combine(projectDirectory, "FrameworkReferences.txt")
#if NETCOREAPP
                    File.WriteAllText(projectFileName, projectFile.Replace("$TARGETFRAMEWORK", "net9.0").Replace("$FSHARPCORELOCATION", pathToFSharpCore))
#else
                    File.WriteAllText(projectFileName, projectFile.Replace("$TARGETFRAMEWORK", "net472").Replace("$FSHARPCORELOCATION", pathToFSharpCore))
#endif
                    File.WriteAllText(programFsFileName, programFs)
                    File.WriteAllText(directoryBuildPropsFileName, directoryBuildProps)
                    File.WriteAllText(directoryBuildTargetsFileName, directoryBuildTargets)

                    let exitCode, dotnetoutput, dotneterrors = Commands.executeProcess config.DotNetExe "build" projectDirectory
                    
                    if exitCode <> 0 || errors.Length > 0 then
                        errors <- dotneterrors
                        output <- dotnetoutput
                        printfn "Output:\n=======\n"
                        output |> Seq.iter(fun line -> printfn "STDOUT:%s\n" line)
                        printfn "Errors:\n=======\n"
                        errors  |> Seq.iter(fun line -> printfn "STDERR:%s\n" line)
                        Assert.True(false, "Errors produced generating References")

                    File.ReadLines(frameworkReferencesFileName) |> Seq.toArray
                with | e ->
                    cleanUp <- false
                    let message =
                        let output = output |> String.concat "\nSTDOUT: "
                        let errors = errors |> String.concat "\nSTDERR: "
                        File.WriteAllText(Path.Combine(projectDirectory, "project.stdout"), output)
                        File.WriteAllText(Path.Combine(projectDirectory, "project.stderror"), errors)
                        $"""                        
Project directory: %s{projectDirectory}
STDOUT: %s{output}
STDERR: %s{errors}
An error occurred getting netcoreapp references (compare the output of `dotnet --list-sdks` and/or the contents of the local `/.dotnet` directory against what is in `global.json`): %A{e}
"""
                    raise (Exception (message, e))
            finally
                if cleanUp then
                    try Directory.Delete(projectDirectory, recursive=true) with | _ -> ()

        open TestReferences

        let private netStandard20Files =
            lazy ImmutableArray.Create(
                NetStandard20.Files.netStandard.Value,
                NetStandard20.Files.mscorlib.Value,
                NetStandard20.Files.systemRuntime.Value,
                NetStandard20.Files.systemCore.Value,
                NetStandard20.Files.systemDynamicRuntime.Value,
                NetStandard20.Files.systemCollectionsImmutable.Value)

        let private netStandard20References =
            lazy ImmutableArray.Create(
                NetStandard20.References.netStandardRef.Value, 
                NetStandard20.References.mscorlibRef.Value, 
                NetStandard20.References.systemRuntimeRef.Value, 
                NetStandard20.References.systemCoreRef.Value, 
                NetStandard20.References.systemDynamicRuntimeRef.Value,
                NetStandard20.References.systemCollectionsImmutableRef.Value)

        let private netCoreApp31References =
            lazy ImmutableArray.Create(
                NetCoreApp31.References.netStandardRef.Value, 
                NetCoreApp31.References.mscorlibRef.Value, 
                NetCoreApp31.References.systemRuntimeRef.Value, 
                NetCoreApp31.References.systemCoreRef.Value, 
                NetCoreApp31.References.systemDynamicRuntimeRef.Value, 
                NetCoreApp31.References.systemConsoleRef.Value)

        let currentReferences =
            getNetCoreAppReferences

        let currentReferencesAsPEs =
            getNetCoreAppReferences
            |> Seq.map (fun x -> PortableExecutableReference.CreateFromFile(x))
            |> ImmutableArray.CreateRange

        let getReferences tf =
            match tf with
                | TargetFramework.NetStandard20 -> netStandard20References.Value
                | TargetFramework.NetCoreApp31 -> netCoreApp31References.Value
                | TargetFramework.Current -> currentReferencesAsPEs

        let getFileReferences tf =
            match tf with
                | TargetFramework.NetStandard20 -> netStandard20Files.Value |> Seq.toArray
                | TargetFramework.NetCoreApp31 -> [||]                            //ToDo --- Perhaps NetCoreApp31Files 
                | TargetFramework.Current -> currentReferences


module internal FSharpProjectSnapshotSerialization =

    let serializeSnapshotToJson (snapshot: FSharpProjectSnapshot) =

        JsonConvert.SerializeObject(snapshot, Formatting.Indented, new JsonSerializerSettings(ReferenceLoopHandling = ReferenceLoopHandling.Ignore))