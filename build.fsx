#r "packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NGenHelper

let msBuild configs projects = MSBuild "" "" configs projects |> ignore

let protoBinDir = "./proto/net40/bin/"
let buildLogsDir = "./logs/build"

let testLogsDir = "./logs/test"
let build_verbosity = "detailed"
let hasVS =  if hasBuildParam "vs" then "VS" else ""
let buildConfig = if hasBuildParam "debug" then hasVS + "Debug" else hasVS + "Release"

let getBinDir targetPlatform = buildConfig @@ targetPlatform @@ "bin/"
let net40BinDir = getBinDir "net40"
let testToolDir = "./tests/fsharpqa/testenv/bin"

let projectFiles = [yield "./src/fsharp-library-build.proj"
                    if hasBuildParam "unittests" then yield "./src/fsharp-library-unittests-build.proj"]
let vsProjectFiles = [yield "./vsintegration/fsharp-vsintegration-build.proj"
                      if hasBuildParam "unittests" then yield "./vsintegration/fsharp-vsintegration-unittests-build.proj"]

let NGEN32WithTimeout _ =
    { ToolPath = NGen32
      TimeOut = TimeSpan.FromMinutes 30.
      WorkingDir = currentDirectory }
let NGEN64WithTimeout _ =
    { ToolPath = NGen64
      TimeOut = TimeSpan.FromMinutes 30.
      WorkingDir = currentDirectory }

RestorePackages()

Target "Clean" (fun _ ->
    CleanDirs [ buildLogsDir; testLogsDir; protoBinDir; "./" @@ buildConfig ]
)


Target "SetEnvironment" (fun _ ->
    // Set some environment variables which can be useful for debugging purposes;
    // they either influence the build process or activate internal switches within the F# compiler / libraries.
    #if DEBUG
    setProcessEnvironVar "FSHARP_verboseOptimizationInfo" Boolean.TrueString
    setProcessEnvironVar "FSHARP_verboseOptimizations" Boolean.TrueString
    #else
    clearProcessEnvironVar "FSHARP_verboseOptimizationInfo"
    clearProcessEnvironVar "FSHARP_verboseOptimizations"
    #endif
)


Target "BuildProtoCompiler" (fun _ ->
    // This uses the lkg compiler to build
    GACHelper.GAC id "/i \"lkg/FSharp-2.0.50726.900/bin/FSharp.Core.dll\""
    ["./src/fsharp-proto-build.proj"]
    |> msBuild []
)


Target "BuildNet40" (fun _ ->    
    projectFiles |> Seq.append ["./src/fsharp-compiler-build.proj"
                                "./src/fsharp-typeproviders-build.proj"] 
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "net40"]
)


Target "BuildNet20" (fun _ ->
    ["./src/fsharp-library-build.proj" ]
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "net20"]
)


Target "BuildPortable7" (fun _ ->
    projectFiles
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "portable7"]
)


Target "BuildPortable47" (fun _ ->
    projectFiles
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "portable47"]
)


Target "BuildPortable78" (fun _ ->
    projectFiles
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "portable78"]
)


Target "BuildPortable259" (fun _ ->
    projectFiles
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "portable259"]
)

Target "BuildPLibs" DoNothing

Target "RunCoreUnitTests" (fun _ ->
    let runTests targetPlatform =
        [ getBinDir targetPlatform @@ "FSharp.Core.Unittests.dll"]
        |> NUnit (fun p ->
            { p with
                DisableShadowCopy = true
                TimeOut = TimeSpan.FromMinutes 20.
                Domain = NUnitDomainModel.MultipleDomainModel
                ErrorOutputFile = testLogsDir @@ "coreunit_" + targetPlatform + "_error.log"
                Out = testLogsDir @@ "coreunit_" + targetPlatform + "_Xml.xml" 
                OutputFile = testLogsDir @@ "coreunit_" + targetPlatform + "_output.log"
                WorkingDir = currentDirectory })

    runTests "net40"
    runTests "portable7"
    runTests "portable47"
    runTests "portable78"
    runTests "portable259"
)


Target "BuildVSAddins" (fun _ -> 
    vsProjectFiles
    |> msBuild ["Configuration", buildConfig; "TargetFramework", "net40"]
)


Target "RunVSUnitTests" (fun _ ->
    [net40BinDir @@ "Unittests.dll"]
    |> NUnit (fun p ->
        { p with
            DisableShadowCopy = false
            TimeOut = TimeSpan.FromMinutes 360.            
            Domain = NUnitDomainModel.SingleDomainModel
            ErrorOutputFile = testLogsDir @@ "ideunit_error.log"
            Out = testLogsDir @@ "ideunit_xml.xml" 
            OutputFile = testLogsDir @@ "ideunit_output.log"
            WorkingDir = currentDirectory })
)


Target "DisableStrongNameVerification" (fun _ ->
    // disable strong-name validation for f# binaries built from open source that are signed with the microsoft key    
    let microsoftPublicKeyToken = "b03f5f7f11d50a3a"

    let fsharpAssemblyNames =
        [| "FSharp.Build"
           "FSharp.Core"
           "FSharp.Compiler"
           "FSharp.Compiler.Interactive.Settings"
           "FSharp.Compiler.Hosted"
           "FSharp.Compiler.Server.Shared"
           "FSharp.Editor"
           "FSharp.LanguageService"
           "FSharp.LanguageService.Base"
           "FSharp.LanguageService.Compiler"
           "FSharp.ProjectSystem.Base"
           "FSharp.ProjectSystem.FSharp"
           "FSharp.ProjectSystem.PropertyPages"
           "FSharp.VS.FSI" |]
    
    for assembly in fsharpAssemblyNames do
        StrongNamingHelper.DisableVerification assembly microsoftPublicKeyToken
)


Target "GAC_Fsharp_Core_Dll" (fun _ ->   
    GACHelper.GAC id (sprintf "/if \"%sFSharp.Core.dll\"" net40BinDir)
)


Target "NGen" (fun _ -> 
    [net40BinDir @@ "fsc.exe"; net40BinDir @@ "fsi.exe"; net40BinDir @@ "FSharp.Build.dll"]
    |> NGenHelper.Install NGEN32WithTimeout
  
    if Environment.Is64BitOperatingSystem then
        [net40BinDir @@ "fsiAnyCpu.exe"; net40BinDir @@ "FSharp.Build.dll"]
        |> NGenHelper.Install NGEN64WithTimeout
)


Target "BuildTestTool" (fun _ ->   
    !! "./tests/**/ILComparer.fsproj"
    ++ "./tests/**/HostedCompilerServer.fsproj"
    |> MSBuild testToolDir "Build" ["Configuration", buildConfig; "TargetFramework", "net40"]
    |> Log "Test-Build: "

    [net40BinDir @@ "FSharp.Core.sigdata"
     net40BinDir @@ "FSharp.Core.optdata"]
    |> Copy testToolDir
)


Target "Help" (fun _ ->
  printfn "\nVisualfsharp O/S Repo build tool\nCopyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.\nLicensed under the Apache License, Version 2.0.  See License.txt in the project root for license information."
  printfn "\nfake build.fsx  [debug] [vs]  [unittests]  [use_ngen]"
  printfn "\n    release         --- Build project files for core compiler"
  printfn "    vs              --- Include vsintegration files and tests use delay signed framework key"
  printfn "    unittests       --- build and run unit tests"
  printfn "    use_ngen        --- ngen built assemblies, takes a long time"
  printfn "    help            --- this message\n"
  printfn "Example:\n"
  printfn "    fake build.fsx release vs unittests\n"
)

Target "Default" DoNothing

"Clean"
  ==> "SetEnvironment"
  ==> "BuildProtoCompiler"
  ==> "BuildNet40"
  ==> "BuildNet20"
  ==> "BuildPortable7"
  ==> "BuildPortable47"
  ==> "BuildPortable78"
  ==> "BuildPortable259"
  ==> "BuildPLibs"
  ==> "DisableStrongNameVerification"
  ==> "GAC_Fsharp_Core_Dll"
  =?> ("NGen",hasBuildParam "use_ngen")
  ==> "BuildTestTool"
  =?> ("RunCoreUnitTests",hasBuildParam "unittests")
  =?> ("BuildVSAddins",hasBuildParam "vs")
  =?> ("RunVSUnitTests",hasBuildParam "vs" && hasBuildParam "unittests")
  ==> "Default"




RunTargetOrDefault "Default"  
