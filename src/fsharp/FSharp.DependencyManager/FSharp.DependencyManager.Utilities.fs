// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.DependencyManager

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.Versioning

open Internal.Utilities.FSharpEnvironment

#if !(NETSTANDARD || NETCOREAPP)
open Microsoft.Build.Evaluation
open Microsoft.Build.Framework
#endif

[<AttributeUsage(AttributeTargets.Assembly ||| AttributeTargets.Class , AllowMultiple = false)>]
type DependencyManagerAttribute() = inherit System.Attribute()

module Utilities =

    /// Return a string array delimited by commas
    /// Note that a quoted string is not going to be mangled into pieces. 
    let trimChars = [| ' '; '\t'; '\''; '\"' |]

    let inline private isNotQuotedQuotation (text: string) n = n > 0 && text.[n-1] <> '\\'

    let getOptions text =
        let split (option:string) =
            let pos = option.IndexOf('=')
            let stringAsOpt text =
                if String.IsNullOrEmpty(text) then None
                else Some text
            let nameOpt =
                if pos <= 0 then None
                else stringAsOpt (option.Substring(0, pos).Trim(trimChars).ToLowerInvariant())
            let valueOpt =
                let valueText =
                    if pos < 0 then option
                    else if pos < option.Length then
                        option.Substring(pos + 1)
                    else ""
                stringAsOpt (valueText.Trim(trimChars))
            nameOpt,valueOpt

        let last = String.length text - 1
        let result = ResizeArray()
        let mutable insideSQ = false
        let mutable start = 0
        let isSeperator c = c = ','
        for i = 0 to last do
            match text.[i], insideSQ with
            | c, false when isSeperator c ->                        // split when seeing a separator
                result.Add(text.Substring(start, i - start))
                insideSQ <- false
                start <- i + 1
            | _, _ when i = last ->
                result.Add(text.Substring(start, i - start + 1))
            | c, true when isSeperator c ->                         // keep reading if a separator is inside quotation
                insideSQ <- true
            | '\'', _ when isNotQuotedQuotation text i ->           // open or close quotation
                insideSQ <- not insideSQ                            // keep reading
            | _ -> ()

        result
        |> List.ofSeq
        |> List.map (fun option -> split option)

    // Path to the directory containing the fsharp compilers
    let fsharpCompilerPath = Path.GetDirectoryName(typeof<DependencyManagerAttribute>.GetTypeInfo().Assembly.Location)

    // We are running on dotnet core if the executing mscorlib is System.Private.CoreLib
    let isRunningOnCoreClr = (typeof<obj>.Assembly).FullName.StartsWith("System.Private.CoreLib", StringComparison.InvariantCultureIgnoreCase)

    let isWindows = 
        match Environment.OSVersion.Platform with
        | PlatformID.Unix -> false
        | PlatformID.MacOSX -> false
        | _ -> true

    let dotnet =
        if isWindows then "dotnet.exe" else "dotnet"

    let sdks = "Sdks"

#if !(NETSTANDARD || NETCOREAPP)
    let msbuildExePath =
        // Find msbuild.exe when invoked from desktop compiler.
        // 1. Look relative to F# compiler location                 Normal retail build
        // 2. Use VSAPPDIR                                          Nightly when started from VS, or F5
        // 3. Use VSINSTALLDIR                                   -- When app is run outside of VS, and
        //                                                          is not copied relative to a vs install.
        let vsRootFromVSAPPIDDIR =
            let vsappiddir = Environment.GetEnvironmentVariable("VSAPPIDDIR")
            if not (String.IsNullOrEmpty(vsappiddir)) then
                Path.GetFullPath(Path.Combine(vsappiddir, "../.."))
            else
                null

        let roots = [|
            Path.GetFullPath(Path.Combine(fsharpCompilerPath, "../../../../.."))
            vsRootFromVSAPPIDDIR
            Environment.GetEnvironmentVariable("VSINSTALLDIR")
            |]

        let msbuildPath root = Path.GetFullPath(Path.Combine(root, "MSBuild/Current/Bin/MSBuild.exe"))

        let msbuildPathExists root =
            if String.IsNullOrEmpty(root) then
                false
            else
                File.Exists(msbuildPath root)

        let msbuildOption rootOpt =
            match rootOpt with
            | Some root -> Some (msbuildPath root)
            | _ -> None

        roots |> Array.tryFind(fun root -> msbuildPathExists root) |> msbuildOption
#else
    let dotnetHostPath =
        // How to find dotnet.exe --- woe is me; probing rules make me sad.
        // Algorithm:
        // 1. Look for DOTNET_HOST_PATH environment variable
        //    this is the main user programable override .. provided by user to find a specific dotnet.exe
        // 2. Probe for are we part of an .NetSDK install
        //    In an sdk install we are always installed in:   sdk\3.0.100-rc2-014234\FSharp
        //    dotnet or dotnet.exe will be found in the directory that contains the sdk directory
        // 3. We are loaded in-process to some other application ... Eg. try .net
        //    See if the host is dotnet.exe ... from netcoreapp3.0 on this is fairly unlikely
        // 4. If it's none of the above we are going to have to rely on the path containing the way to find dotnet.exe
        //
        if isRunningOnCoreClr then
            match (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH")) with
            | value when not (String.IsNullOrEmpty(value)) ->
                Some value                           // Value set externally
            | _ ->
                // Probe for netsdk install
                let dotnetLocation =
                    let dotnetApp =
                        let platform = Environment.OSVersion.Platform
                        if platform = PlatformID.Unix then "dotnet" else "dotnet.exe"
                    let assemblyLocation = typeof<DependencyManagerAttribute>.GetTypeInfo().Assembly.Location
                    Path.Combine(assemblyLocation, "../../..", dotnetApp)

                if File.Exists(dotnetLocation) then
                    Some dotnetLocation
                else
                    let main = Process.GetCurrentProcess().MainModule
                    if main.ModuleName ="dotnet" then
                        Some main.FileName
                    else
                        Some dotnet
        else
            None
#endif

    let drainStreamToFile (stream: StreamReader) filename =
        use file = File.OpenWrite(filename)
        use writer = new StreamWriter(file)
        let rec copyLines () =
            match stream.ReadLine() with
            | null -> ()
            | line ->
                writer.WriteLine(line)
                copyLines ()
        copyLines ()

    let executeBuild pathToExe arguments workingDir =
        match pathToExe with
        | Some path ->

            let psi = ProcessStartInfo()
            psi.FileName <- path
            psi.WorkingDirectory <- workingDir
            psi.RedirectStandardOutput <- true
            psi.RedirectStandardError <- true
            psi.Arguments <- arguments
            psi.CreateNoWindow <- true
            psi.UseShellExecute <- false

            use p = new Process()
            p.StartInfo <- psi
            p.Start() |> ignore

            drainStreamToFile p.StandardOutput (Path.Combine(workingDir, "StandardOutput.txt"))
            drainStreamToFile p.StandardError (Path.Combine(workingDir, "StandardError.txt"))

            p.WaitForExit()
            p.ExitCode = 0

        | None -> false

    let buildProject projectPath binLogPath =
        let binLoggingArguments =
            match binLogPath with
            | Some(path) ->
                let path = match path with
                           | Some path -> path // specific file
                           | None -> Path.Combine(Path.GetDirectoryName(projectPath), "msbuild.binlog") // auto-generated file
                sprintf "/bl:\"%s\"" path
            | None -> ""

        let arguments prefix =
            sprintf "%s -restore %s %c%s%c /t:InteractivePackageManagement" prefix binLoggingArguments '\"' projectPath '\"'

        let workingDir = Path.GetDirectoryName projectPath

        let succeeded =
#if !(NETSTANDARD || NETCOREAPP)
            // The Desktop build uses "msbuild" to build
            executeBuild msbuildExePath (arguments "") workingDir
#else
            // The coreclr uses "dotnet msbuild" to build
            executeBuild dotnetHostPath (arguments "msbuild") workingDir
#endif
        let outputFile = projectPath + ".fsx"
        let resultOutFile = if succeeded && File.Exists(outputFile) then Some outputFile else None
        succeeded, resultOutFile

    let generateProjectBody = @"
<Project Sdk='Microsoft.NET.Sdk'>

  <PropertyGroup>
    <TargetFramework>$(TARGETFRAMEWORK)</TargetFramework>
    <IsPackable>false</IsPackable>

    <!-- Temporary fix some sdks, shipped internally with broken parameterization -->
    <FSharpCoreImplicitPackageVersion Condition=""'$(FSharpCoreImplicitPackageVersion)' == '{{FSharpCoreShippedPackageVersion}}'"">4.7.0</FSharpCoreImplicitPackageVersion>
    <FSharpCoreImplicitPackageVersion Condition=""'$(FSharpCoreImplicitPackageVersion)' == '{{FSharpCorePreviewPackageVersion}}'"">4.7.1-*</FSharpCoreImplicitPackageVersion>
  </PropertyGroup>

$(PACKAGEREFERENCES)

  <Target Name='ComputePackageRootsForInteractivePackageManagement'
          BeforeTargets='CoreCompile'
          DependsOnTargets='CollectPackageReferences'>
      <ItemGroup>
        <InteractiveResolvedFile Remove='@(InteractiveResolvedFile)' />
        <InteractiveResolvedFile Include='@(ResolvedCompileFileDefinitions->ClearMetadata())' KeepDuplicates='false'>
            <NormalizedIdentity Condition=""'%(Identity)'!=''"">$([System.String]::Copy('%(Identity)').Replace('\', '/'))</NormalizedIdentity>
            <NormalizedPathInPackage Condition=""'%(ResolvedCompileFileDefinitions.PathInPackage)'!=''"">$([System.String]::Copy('%(ResolvedCompileFileDefinitions.PathInPackage)').Replace('\', '/'))</NormalizedPathInPackage>
            <PositionPathInPackage Condition=""'%(InteractiveResolvedFile.NormalizedPathInPackage)'!=''"">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').IndexOf('%(InteractiveResolvedFile.NormalizedPathInPackage)'))</PositionPathInPackage>
            <PackageRoot Condition=""'%(InteractiveResolvedFile.NormalizedPathInPackage)'!='' and '%(InteractiveResolvedFile.PositionPathInPackage)'!='-1'"">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').Substring(0, %(InteractiveResolvedFile.PositionPathInPackage)))</PackageRoot>
            <InitializeSourcePath>%(InteractiveResolvedFile.PackageRoot)content\%(ResolvedCompileFileDefinitions.FileName)%(ResolvedCompileFileDefinitions.Extension).fsx</InitializeSourcePath>
            <IsNotImplementationReference>$([System.String]::Copy('%(ResolvedCompileFileDefinitions.PathInPackage)').StartsWith('ref/'))</IsNotImplementationReference>
            <NuGetPackageId>%(ResolvedCompileFileDefinitions.NuGetPackageId)</NuGetPackageId>
            <NuGetPackageVersion>%(ResolvedCompileFileDefinitions.NuGetPackageVersion)</NuGetPackageVersion>
        </InteractiveResolvedFile>
        <InteractiveResolvedFile Include='@(RuntimeCopyLocalItems->ClearMetadata())' KeepDuplicates='false' >
            <NormalizedIdentity Condition=""'%(Identity)'!=''"">$([System.String]::Copy('%(Identity)').Replace('\', '/'))</NormalizedIdentity>
            <NormalizedPathInPackage Condition=""'%(RuntimeCopyLocalItems.PathInPackage)'!=''"">$([System.String]::Copy('%(RuntimeCopyLocalItems.PathInPackage)').Replace('\', '/'))</NormalizedPathInPackage>
            <PositionPathInPackage Condition=""'%(InteractiveResolvedFile.NormalizedPathInPackage)'!=''"">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').IndexOf('%(InteractiveResolvedFile.NormalizedPathInPackage)'))</PositionPathInPackage>
            <PackageRoot Condition=""'%(InteractiveResolvedFile.NormalizedPathInPackage)'!='' and '%(InteractiveResolvedFile.PositionPathInPackage)'!='-1'"">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').Substring(0, %(InteractiveResolvedFile.PositionPathInPackage)))</PackageRoot>
            <InitializeSourcePath>%(InteractiveResolvedFile.PackageRoot)content\%(RuntimeCopyLocalItems.FileName)%(RuntimeCopyLocalItems.Extension).fsx</InitializeSourcePath>
            <IsNotImplementationReference>$([System.String]::Copy('%(RuntimeCopyLocalItems.PathInPackage)').StartsWith('ref/'))</IsNotImplementationReference>
            <NuGetPackageId>%(RuntimeCopyLocalItems.NuGetPackageId)</NuGetPackageId>
            <NuGetPackageVersion>%(RuntimeCopyLocalItems.NuGetPackageVersion)</NuGetPackageVersion>
        </InteractiveResolvedFile>
        <NativeIncludeRoots
            Include='@(RuntimeTargetsCopyLocalItems)'
            Condition=""'%(RuntimeTargetsCopyLocalItems.AssetType)' == 'native'"">
            <Path>$([MSBuild]::EnsureTrailingSlash('$([System.String]::Copy('%(FullPath)').Substring(0, $([System.String]::Copy('%(FullPath)').LastIndexOf('runtimes'))))'))</Path>
        </NativeIncludeRoots>
      </ItemGroup>
  </Target>

  <Target Name='InteractivePackageManagement' DependsOnTargets='ResolvePackageAssets;ComputePackageRootsForInteractivePackageManagement'>
    <ItemGroup>
      <ReferenceLines Remove='@(ReferenceLines)' />
      <ReferenceLines Include='// Generated from #r ""nuget:Package References""' />
      <ReferenceLines Include='// ============================================' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='// DOTNET_HOST_PATH:($(DOTNET_HOST_PATH))' />
      <ReferenceLines Include='// MSBuildSDKsPath:($(MSBuildSDKsPath))' />
      <ReferenceLines Include='// MSBuildExtensionsPath:($(MSBuildExtensionsPath))' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='// References' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='#r @""%(InteractiveResolvedFile.FullPath)""' Condition = ""'%(InteractiveResolvedFile.NugetPackageId)' != '' and '%(InteractiveResolvedFile.NugetPackageId)' != 'Microsoft.NETCore.App' and '%(InteractiveResolvedFile.NugetPackageId)' != 'FSharp.Core' and '%(InteractiveResolvedFile.NugetPackageId)' != 'System.ValueTuple' and '%(InteractiveResolvedFile.IsNotImplementationReference)' != 'true' and Exists('%(InteractiveResolvedFile.FullPath)')"" KeepDuplicates='false' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='// Includes' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='#I @""%(InteractiveResolvedFile.PackageRoot)""' Condition= ""'%(InteractiveResolvedFile.NugetPackageId)' != '' and '%(InteractiveResolvedFile.NugetPackageId)' != 'Microsoft.NETCore.App' and '%(InteractiveResolvedFile.NugetPackageId)' != 'FSharp.Core' and '%(InteractiveResolvedFile.NugetPackageId)' != 'System.ValueTuple' and $([System.String]::Copy('%(InteractiveResolvedFile.PackageRoot)').EndsWith('/')) and Exists(%(InteractiveResolvedFile.PackageRoot))"" KeepDuplicates='false' />
      <ReferenceLines Include='#I @""%(NativeIncludeRoots.Path)""' Condition= ""'%(InteractiveResolvedFile.NugetPackageId)' != '' and '%(InteractiveResolvedFile.NugetPackageId)' != 'Microsoft.NETCore.App' and '%(InteractiveResolvedFile.NugetPackageId)' != 'FSharp.Core' and '%(InteractiveResolvedFile.NugetPackageId)' != 'System.ValueTuple' and '%(NativeIncludeRoots.Path)' != ''"" KeepDuplicates='false' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='// Load Sources' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='#load @""%(InteractiveResolvedFile.InitializeSourcePath)""'  Condition = ""'%(InteractiveResolvedFile.InitializeSourcePath)' != '' and '%(InteractiveResolvedFile.NugetPackageId)' != 'Microsoft.NETCore.App' and '%(InteractiveResolvedFile.NugetPackageId)' != 'FSharp.Core' and '%(InteractiveResolvedFile.NugetPackageId)' != 'System.ValueTuple' and Exists('%(InteractiveResolvedFile.InitializeSourcePath)')"" KeepDuplicates='false' />
    </ItemGroup>

    <WriteLinesToFile Lines='@(ReferenceLines)' File='$(MSBuildProjectFullPath).fsx' Overwrite='True' WriteOnlyWhenDifferent='True' />
    <ItemGroup>
      <FileWrites Include='$(MSBuildProjectFullPath).fsx' />
    </ItemGroup>
  </Target>

</Project>"
