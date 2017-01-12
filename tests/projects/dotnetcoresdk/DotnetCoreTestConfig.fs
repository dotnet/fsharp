module DotnetCoreTestConfig

open System
open System.IO
open System.Xml.Linq

open Scripting
open TestFramework

let fsharpSuiteDirectory = __SOURCE_DIRECTORY__

let config configurationName envVars =

    let SCRIPT_ROOT = __SOURCE_DIRECTORY__ 
    let FSCBinPath = SCRIPT_ROOT ++ ".." ++ ".." ++ configurationName ++ "net40" ++ "bin"

    let csc_flags = "/nologo" 
    let fsc_flags = "-r:System.Core.dll --nowarn:20 --define:COMPILED" 
    let fsi_flags = "-r:System.Core.dll --nowarn:20  --define:INTERACTIVE --maxerrors:1 --abortonerror" 

    let CORDIR, CORSDK = WindowsPlatform.clrPaths envVars

    let Is64BitOperatingSystem = WindowsPlatform.Is64BitOperatingSystem envVars

    let fsiroot = if Is64BitOperatingSystem then "fsiAnyCpu" else "fsi"

    let CSC = (CORDIR ++ "csc.exe")
    let NGEN = (CORDIR ++ "ngen.exe")
    let ILDASM = (CORSDK ++ "ildasm.exe")
    let SN = (CORSDK ++ "sn.exe") 
    let PEVERIFY = (CORSDK ++ "peverify.exe")
    let FSC = (FSCBinPath ++ "fsc.exe")
    let FSI = (FSCBinPath ++ (fsiroot+".exe"))
    let FSCOREDLLPATH = (FSCBinPath ++ "FSharp.Core.dll") 

    let defaultPlatform = 
        match Is64BitOperatingSystem with 
//        | PlatformID.MacOSX, true -> "osx.10.10-x64"
//        | PlatformID.Unix,true -> "ubuntu.14.04-x64"
        | true -> "win7-x64"
        | false -> "win7-x86"

    let dotNetExe = 
        match resolveCmdFromPATH envVars "dotnet" with
        | Some p -> p
        | None -> failwith "dotnet not found in PATH"

    { EnvironmentVariables = envVars
      CORDIR = CORDIR |> Commands.pathAddBackslash
      CORSDK = CORSDK |> Commands.pathAddBackslash
      FSCBinPath = FSCBinPath |> Commands.pathAddBackslash
      FSCOREDLLPATH = FSCOREDLLPATH
      ILDASM = ILDASM
      SN = SN
      NGEN = NGEN 
      PEVERIFY = PEVERIFY
      CSC = CSC 
      BUILD_CONFIG = configurationName
      FSC = FSC
      FSI = FSI
      csc_flags = csc_flags
      fsc_flags = fsc_flags 
      fsi_flags = fsi_flags 
      Directory="" 
      DotNetExe = dotNetExe
      DefaultPlatform = defaultPlatform }

let initializeSuite () =

#if DEBUG
    let configurationName = "debug"
#else
    let configurationName = "release"
#endif
    let env = envVars ()

    let cfg =
        let c = config configurationName env
        let usedEnvVars = c.EnvironmentVariables  |> Map.add "FSC" c.FSC             
        { c with EnvironmentVariables = usedEnvVars }

    logConfig cfg

    cfg

let testConfig testDir =

    let suiteHelpers = lazy (initializeSuite ())
    let cfg = suiteHelpers.Value

    let dir = Path.GetFullPath(fsharpSuiteDirectory ++ testDir)
    log "------------------ %s ---------------" dir
    log "cd %s" dir
    { cfg with Directory =  dir }

let readFeedsFromNugetConfig (nugetConfig: string) =
    let xn name = XName.Get(name)
    XDocument
        .Load(nugetConfig)
        .Element(xn "configuration")
        .Element(xn "packageSources")
        .Elements(xn "add")
        .Attributes(xn "value")
    |> Seq.map (fun a -> a.Value)
    |> Seq.toList

let getArtifactsDir cfg =

    __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ (cfg.BUILD_CONFIG) ++ "artifacts"
