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

open Microsoft.DotNet.PlatformAbstractions
open Microsoft.Extensions.DependencyModel

#if !(NETSTANDARD || NETCOREAPP)
open Microsoft.Build.Evaluation
open Microsoft.Build.Framework
#endif

[<AttributeUsage(AttributeTargets.Assembly ||| AttributeTargets.Class , AllowMultiple = false)>]
type DependencyManagerAttribute() = inherit System.Attribute()

module Utilities =

    let private context =
        try
            match (Assembly.GetEntryAssembly()) with
            | null ->
                // No entry assembly therefore look for fsi.exe/fsc.exe alongside this assembly
                let loadAssembly path =
                    let asm = Assembly.LoadFrom(path)
                    DependencyContext.Load(asm)
                // If there is no entry assembly then we assume try fsi.exe, then fsc.exe`
                let location = typeof<DependencyManagerAttribute>.GetTypeInfo().Assembly.Location
                let pathToFsiExe = Path.Combine(Path.GetDirectoryName(location), "fsi.exe")
                if File.Exists(pathToFsiExe) then
                    loadAssembly pathToFsiExe |> Option.ofObj
                else
                    let pathToFscExe = Path.Combine(Path.GetDirectoryName(location), "fsc.exe")
                    if File.Exists(pathToFscExe) then
                        loadAssembly pathToFscExe |> Option.ofObj
                    else
                        None
            | asm -> DependencyContext.Load(asm) |> Option.ofObj
        with _ -> None

    // Latest documentation about nuget tfms is here:  https://docs.microsoft.com/en-us/nuget/schema/target-frameworks
    let netCoreAppTFM = [|
        2.2m,   ".NETCoreApp,Version=v2.2";
        2.2m,   ".NETStandard,Version=v2.2";
        2.1m,   ".NETCoreApp,Version=v2.1";
        2.0m,   ".NETStandard,Version=v2.0";
        2.0m,   ".NETCoreApp,Version=v2.0";
        1.0m,   ".NETStandard,Version=v1.6";
        1.0m,   ".NETStandard,Version=v1.5";
        1.0m,   ".NETStandard,Version=v1.4";
        1.0m,   ".NETStandard,Version=v1.3";
        1.0m,   ".NETStandard,Version=v1.2";
        1.0m,   ".NETStandard,Version=v1.1";
        1.0m,   ".NETStandard,Version=v1.0";
    |]

    let netDesktopTFM = [|
        0.472m, ".NETFramework,Version=v4.7.2";
        0.471m, ".NETFramework,Version=v4.7.1";
        0.47m,  ".NETFramework,Version=v4.7";
        0.47m,  ".NETStandard,Version=v2.0";
        0.462m, ".NETFramework,Version=v4.6.2";
        0.462m, ".NETStandard,Version=v1.6";
        0.461m, ".NETFramework,Version=v4.6.1";
        0.461m, ".NETStandard,Version=v1.5";
        0.46m,  ".NETFramework,Version=v4.6";
        0.46m,  ".NETStandard,Version=v1.4";
        0.452m, ".NETFramework,Version=v4.5.2";
        0.452m, ".NETStandard,Version=v1.3";
        0.451m, ".NETFramework,Version=v4.51";
        0.451m, ".NETStandard,Version=v1.2";
        0.45m,  ".NETFramework,Version=v4.5";
        0.45m,  ".NETStandard,Version=v1.1";
        0.45m,  ".NETStandard,Version=v1.0";
        0.40m,  ".NETFramework,Version=v4";
    |]

    let desktopProductVersionMonikers = [|
    // major, minor, build, revision, moniker
       4,     7,      3190,     0,    0.472m;
       4,     7,      2600,     0,    0.471m;
       4,     7,      2053,     0,    0.47m;
       4,     6,      1590,     0,    0.462m;
       4,     6,      1055,     0,    0.461m;
       4,     6 ,       81,     0,    0.46m;
       4,     0,     30319, 34209,    0.452m;
       4,     0,     30319, 18408,    0.451m;
       4,     0,     30319, 17929,    0.45m;
       4,     0,     30319,     1,    0.40m;
    |]

    let defaultMscorlibVersion = 4, 6, 1055, 0                      // Probably needs configuring
    let defaultFrameworkName = ".NETFramework,Version=v4.6.1"
    let netCoreAppPrefix = ".NETCoreApp,Version=v"
    let netStandardPrefix = ".NETStandard,Version=v"

    // Algorithm:
    // This returns a prioritized the list of supported tfms for the executing application
    // the first entry is the most appropriate.
    // On windows desktop the windows tfm that matches the installed dotnet framework version is the best match, followed by the highest supported netstandard dll
    // E.g.
    //     Priority search for net471 looks like:
    //        net471;netstandard2.0;net47;net462;netstandard1.6;net461;netstandard1.5;net46;netstandard1.4;net452;netstandard1.3;net451;netstandard1.2;net45;netstandard1.1;net40;netstandard1.0
    //     Priority for .NetCoreApp 2.1 looks like:
    //        netstandard2.0;netstandard1.6;netstandard1.5;netstandard1.4;netstandard1.3;netstandard1.2;netstandard1.1;netstandard1.0
    let executionTFMs =
        seq {
            let netStandardsFromVersion version =
                netCoreAppTFM |> Seq.filter(fun (ver,_) -> version >= ver) |> Seq.map(fun (_,moniker) -> moniker) |> Seq.toArray

            let netDesktopsFromVersion version =
                netDesktopTFM |> Seq.filter(fun (ver,_) -> version >= ver) |> Seq.map(fun (_,moniker) -> moniker) |> Seq.toArray

            match context with
            | Some ctxt ->
                let target = ctxt.Target.Framework.ToString()
                if target.StartsWith(netCoreAppPrefix) then
                    yield! (netStandardsFromVersion (Decimal.Parse(target.Substring(netCoreAppPrefix.Length))))
                elif target.StartsWith(netStandardPrefix) then
                    yield! (netStandardsFromVersion (Decimal.Parse(target.Substring(netStandardPrefix.Length))))
            | None ->
                let fileMajorPart, fileMinorPart, fileBuildPart, filePrivatePart =
                    try
                        let attrOpt = typeof<int>.GetTypeInfo().Assembly.GetCustomAttributes(typeof<AssemblyFileVersionAttribute>) |> Seq.tryHead
                        match attrOpt with
                        | Some attr ->
                            let fv = (downcast attr : AssemblyFileVersionAttribute).Version.Split([|'.'|]) |> Array.map(fun e ->  Int32.Parse(e))
                            fv.[0], fv.[1], fv.[2], fv.[3]
                        | _ -> defaultMscorlibVersion
                    with _ -> defaultMscorlibVersion

                // Get the ProductVersion of this framework compare with table yield compatible monikers
                for majorPart, minorPart, buildPart, privatePart, version in desktopProductVersionMonikers do
                    if   fileMajorPart < majorPart then ()
                    elif fileMajorPart > majorPart then
                        yield! (netDesktopsFromVersion version)
                    elif fileMinorPart < minorPart then ()
                    elif fileMinorPart > minorPart then
                        yield! (netDesktopsFromVersion version)
                    elif fileBuildPart < buildPart then ()
                    elif fileBuildPart = 30319 && filePrivatePart < privatePart then ()
                    elif fileBuildPart = 30319 && filePrivatePart > privatePart then
                        yield! (netDesktopsFromVersion version)
                    elif fileBuildPart < buildPart then ()
                    else
                        yield! (netDesktopsFromVersion version)
        } |> Seq.distinct |> Seq.toArray

    let frameworkIdentifier, frameworkVersion =
        let frameworkName =
            match executionTFMs |> Array.tryHead with
            | Some tfm -> new FrameworkName(tfm)
            | None -> new FrameworkName(defaultFrameworkName)
        frameworkName.Identifier, frameworkName.Version

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

        result |> Seq.map(fun option -> split option)

    // Path to the directory containing the fsharp compilers
    let fsharpCompilerPath = Path.GetDirectoryName(typeof<DependencyManagerAttribute>.GetTypeInfo().Assembly.Location)

    let isRunningOnCoreClr =
        // We are running on dotnet core if the executing application has .runtimeconfig.json
        let depsJsonPath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "deps.json")
        File.Exists(depsJsonPath)

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
        if isRunningOnCoreClr then
            match (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH")) with
            | value when not (String.IsNullOrEmpty(value)) -> Some value                           // Value set externally
            | _ ->
                let main = Process.GetCurrentProcess().MainModule
                if main.ModuleName.StartsWith("dotnet") then
                    Some main.FileName
                else
                    None
            else
                None
#endif
    let executeBuild pathToExe arguments =
        match pathToExe with
        | Some path ->
            let psi = ProcessStartInfo()
            psi.FileName <- path
            psi.RedirectStandardOutput <- false
            psi.RedirectStandardError <- false
            psi.Arguments <- arguments
            psi.CreateNoWindow <- true
            psi.UseShellExecute <- false

            use p = new Process()
            p.StartInfo <- psi
            p.Start() |> ignore
            p.WaitForExit()
            p.ExitCode = 0

        | None -> false

    let buildProject projectPath binLogging =

        let binLoggingArguments =
            match binLogging with
            | true -> "-bl"
            | _ -> ""

        let arguments prefix =
            sprintf "%s -restore %s %c%s%c /t:FSI-PackageManagement" prefix binLoggingArguments '\"' projectPath '\"'

        let succeeded =
#if !(NETSTANDARD || NETCOREAPP)
            // The Desktop build uses "msbuild" to build
            executeBuild msbuildExePath (arguments "")
#else
            // The coreclr uses "dotnet msbuild" to build
            executeBuild dotnetHostPath (arguments "msbuild")
#endif
        let outputFile = projectPath + ".fsx"
        let resultOutFile = if succeeded && File.Exists(outputFile) then Some outputFile else None
        succeeded, resultOutFile

    // Generate a project files for dependencymanager projects
    let generateLibrarySource = @"// Generated dependencymanager library
namespace lib"

    let generateProjectBody = @"
<Project Sdk='Microsoft.NET.Sdk'>
  <PropertyGroup>
    <TargetFrameworkIdentifier>$(TARGETFRAMEWORKIDENTIFIER)</TargetFrameworkIdentifier>
    <TargetFrameworkVersion>v$(TARGETFRAMEWORKVERSION)</TargetFrameworkVersion>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include='Library.fs' />
  </ItemGroup>
$(PACKAGEREFERENCES)

  <Target Name='CollectFSharpDesignTimeTools' BeforeTargets='BeforeCompile' DependsOnTargets='_GetFrameworkAssemblyReferences'>
    <ItemGroup>
      <PropertyNames Include = ""Pkg$([System.String]::Copy('%(PackageReference.FileName)').Replace('.','_'))"" Condition = "" '%(PackageReference.IsFSharpDesignTimeProvider)' == 'true' and '%(PackageReference.Extension)' == '' ""/>
      <PropertyNames Include = ""Pkg$([System.String]::Copy('%(PackageReference.FileName)%(PackageReference.Extension)').Replace('.','_'))"" Condition = "" '%(PackageReference.IsFSharpDesignTimeProvider)' == 'true' and '%(PackageReference.Extension)' != '' ""/>
      <FscCompilerTools Include = ""$(%(PropertyNames.Identity))"" />
    </ItemGroup>
  </Target>

  <Target Name=""PackageFSharpDesignTimeTools"" DependsOnTargets=""_GetFrameworkAssemblyReferences"">
    <PropertyGroup>
      <FSharpDesignTimeProtocol Condition = "" '$(FSharpDesignTimeProtocol)' == '' "">fsharp41</FSharpDesignTimeProtocol>
      <FSharpToolsDirectory Condition = "" '$(FSharpToolsDirectory)' == '' "">tools</FSharpToolsDirectory>
    </PropertyGroup>

    <Error Text=""'$(FSharpToolsDirectory)' is an invalid value for 'FSharpToolsDirectory' valid values are 'typeproviders' and 'tools'."" Condition=""'$(FSharpToolsDirectory)' != 'typeproviders' and '$(FSharpToolsDirectory)' != 'tools'"" />
    <Error Text=""The 'FSharpDesignTimeProtocol'  property can be only 'fsharp41'"" Condition=""'$(FSharpDesignTimeProtocol)' != 'fsharp41'"" />

    <ItemGroup>
      <_ResolvedOutputFiles
          Include=""%(_ResolvedProjectReferencePaths.RootDir)%(_ResolvedProjectReferencePaths.Directory)/**/*""
          Exclude=""%(_ResolvedProjectReferencePaths.RootDir)%(_ResolvedProjectReferencePaths.Directory)/**/FSharp.Core.dll;%(_ResolvedProjectReferencePaths.RootDir)%(_ResolvedProjectReferencePaths.Directory)/**/System.ValueTuple.dll""
          Condition=""'%(_ResolvedProjectReferencePaths.IsFSharpDesignTimeProvider)' == 'true'"">
        <NearestTargetFramework>%(_ResolvedProjectReferencePaths.NearestTargetFramework)</NearestTargetFramework>
      </_ResolvedOutputFiles>

      <_ResolvedOutputFiles
          Include=""@(BuiltProjectOutputGroupKeyOutput)""
          Condition=""'$(IsFSharpDesignTimeProvider)' == 'true' and '%(BuiltProjectOutputGroupKeyOutput->Filename)%(BuiltProjectOutputGroupKeyOutput->Extension)' != 'FSharp.Core.dll' and '%(BuiltProjectOutputGroupKeyOutput->Filename)%(BuiltProjectOutputGroupKeyOutput->Extension)' != 'System.ValueTuple.dll'"">
        <NearestTargetFramework>$(TargetFramework)</NearestTargetFramework>
      </_ResolvedOutputFiles>

      <TfmSpecificPackageFile Include=""@(_ResolvedOutputFiles)"">
         <PackagePath>$(FSharpToolsDirectory)/$(FSharpDesignTimeProtocol)/%(_ResolvedOutputFiles.NearestTargetFramework)/%(_ResolvedOutputFiles.FileName)%(_ResolvedOutputFiles.Extension)</PackagePath>
      </TfmSpecificPackageFile>

    </ItemGroup>
  </Target>

  <Target Name='ComputePackageRoots'
          BeforeTargets='CoreCompile;FSI-PackageManagement'
          DependsOnTargets='CollectPackageReferences'>
      <ItemGroup>
        <FsxResolvedFile Include='@(ResolvedCompileFileDefinitions)'>
           <PackageRootProperty>Pkg$([System.String]::Copy('%(ResolvedCompileFileDefinitions.NugetPackageId)').Replace('.','_'))</PackageRootProperty>
           <PackageRoot>$(%(FsxResolvedFile.PackageRootProperty))</PackageRoot>
           <InitializeSourcePath>$(%(FsxResolvedFile.PackageRootProperty))\content\%(ResolvedCompileFileDefinitions.FileName)%(ResolvedCompileFileDefinitions.Extension).fsx</InitializeSourcePath>
        </FsxResolvedFile>
      </ItemGroup>
  </Target>

  <Target Name='FSI-PackageManagement' DependsOnTargets='ResolvePackageAssets'>
    <ItemGroup>
      <ReferenceLines Remove='@(ReferenceLines)' />
      <ReferenceLines Include='// Generated from #r ""nuget:Package References""' />
      <ReferenceLines Include='// ============================================' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='// DOTNET_HOST_PATH:($(DOTNET_HOST_PATH))' />
      <ReferenceLines Include='// MSBuildSDKsPath:($(MSBuildSDKsPath))' />
      <ReferenceLines Include='// MSBuildExtensionsPath:($(MSBuildExtensionsPath))' />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='#r @""%(FsxResolvedFile.HintPath)""'                 Condition = ""%(FsxResolvedFile.NugetPackageId) != 'Microsoft.NETCore.App' and %(FsxResolvedFile.NugetPackageId) != 'FSharp.Core' and %(FsxResolvedFile.NugetPackageId) != 'System.ValueTuple' and Exists('%(FsxResolvedFile.HintPath)')"" />
      <ReferenceLines Include='//' />
      <ReferenceLines Include='#load @""%(FsxResolvedFile.InitializeSourcePath)""'  Condition = ""%(FsxResolvedFile.NugetPackageId) != 'Microsoft.NETCore.App' and %(FsxResolvedFile.NugetPackageId) != 'FSharp.Core' and %(FsxResolvedFile.NugetPackageId) != 'System.ValueTuple' and Exists('%(FsxResolvedFile.InitializeSourcePath)')"" />
    </ItemGroup>

    <WriteLinesToFile Lines='@(ReferenceLines)' File='$(MSBuildProjectFullPath).fsx' Overwrite='True' WriteOnlyWhenDifferent='True' />
    <ItemGroup>
      <FileWrites Include='$(MSBuildProjectFullPath).fsx' />
    </ItemGroup>
  </Target>

</Project>"
