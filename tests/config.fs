module TestConfig

open System
open System.IO
open System.Collections.Generic
open Microsoft.Win32

open PlatformHelpers
open FSharpTestSuiteTypes

let private fileExists = Commands.fileExists __SOURCE_DIRECTORY__ >> Option.isSome
let private directoryExists = Commands.directoryExists __SOURCE_DIRECTORY__ >> Option.isSome

let private regQuery = WindowsPlatform.regQuery

type private FSLibPaths = 
    { FSCOREDLLPATH : string
      FSCOREDLL20PATH : string
      FSCOREDLLPORTABLEPATH : string
      FSCOREDLLNETCOREPATH : string
      FSCOREDLLNETCORE78PATH : string
      FSCOREDLLNETCORE259PATH : string
      FSDATATPPATH : string
      FSCOREDLLVPREVPATH : string }

// REM ===
// REM === Find paths to shipped F# libraries referenced by clients
// REM ===
let private GetFSLibPaths env osArch fscBinPath =

    // REM == Find out path to native 'Program Files 32bit', no matter what
    // REM == architecture we are running on and no matter what command
    // REM == prompt we came from.
    // IF /I "%OSARCH%"=="x86"   set X86_PROGRAMFILES=%ProgramFiles%
    // IF /I "%OSARCH%"=="IA64"  set X86_PROGRAMFILES=%ProgramFiles(x86)%
    // IF /I "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%
    let X86_PROGRAMFILES = WindowsPlatform.x86ProgramFilesDirectory env osArch

    // REM == Default VS install locations
    // set FSCOREDLLPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.1.0
    let mutable FSCOREDLLPATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETFramework"/"v4.0"/"4.4.1.0"
    // set FSCOREDLL20PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v2.0\2.3.0.0
    let mutable FSCOREDLL20PATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETFramework"/"v2.0"/"2.3.0.0"
    // set FSCOREDLLPORTABLEPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.41.0
    let mutable FSCOREDLLPORTABLEPATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETPortable"/"3.47.41.0"
    // set FSCOREDLLNETCOREPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.41.0
    let mutable FSCOREDLLNETCOREPATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETCore"/"3.7.41.0"
    // set FSCOREDLLNETCORE78PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.41.0
    let mutable FSCOREDLLNETCORE78PATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETCore"/"3.78.41.0"
    // set FSCOREDLLNETCORE259PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.41.0
    let mutable FSCOREDLLNETCORE259PATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETCore"/"3.259.41.0"
    // set FSDATATPPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers
    let mutable FSDATATPPATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETFramework"/"v4.0"/"4.3.0.0"/"Type Providers"
    // set FSCOREDLLVPREVPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0
    let mutable FSCOREDLLVPREVPATH = X86_PROGRAMFILES/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETFramework"/"v4.0"/"4.4.0.0"

    // REM == Check if using open build instead

    // IF EXIST "%FSCBinPath%\FSharp.Core.dll" set FSCOREDLLPATH=%FSCBinPath%
    match fscBinPath with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLLPATH <- d
    | Some _ | None -> ()

    // IF EXIST "%FSCBinPath%\..\..\net20\bin\FSharp.Core.dll" set FSCOREDLL20PATH=%FSCBinPath%\..\..\net20\bin
    match fscBinPath |> Option.map (fun d -> d/".."/".."/"net20"/"bin") with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLL20PATH <- d
    | Some _ | None -> ()

    // IF EXIST "%FSCBinPath%\..\..\portable47\bin\FSharp.Core.dll" set FSCOREDLLPORTABLEPATH=%FSCBinPath%\..\..\portable47\bin
    match fscBinPath |> Option.map (fun d -> d/".."/".."/"portable47"/"bin") with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLLPORTABLEPATH <- d
    | Some _ | None -> ()

    // IF EXIST "%FSCBinPath%\..\..\portable7\bin\FSharp.Core.dll" set FSCOREDLLNETCOREPATH=%FSCBinPath%\..\..\portable7\bin
    match fscBinPath |> Option.map (fun d -> d/".."/".."/"portable7"/"bin") with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLLNETCOREPATH <- d
    | Some _ | None -> ()

    // IF EXIST "%FSCBinPath%\..\..\portable78\bin\FSharp.Core.dll" set FSCOREDLLNETCORE78PATH=%FSCBinPath%\..\..\portable78\bin
    match fscBinPath |> Option.map (fun d -> d/".."/".."/"portable78"/"bin") with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLLNETCORE78PATH <- d
    | Some _ | None -> ()

    // IF EXIST "%FSCBinPath%\..\..\portable259\bin\FSharp.Core.dll" set FSCOREDLLNETCORE259PATH=%FSCBinPath%\..\..\portable259\bin
    match fscBinPath |> Option.map (fun d -> d/".."/".."/"portable259"/"bin") with
    | Some d when fileExists (d/"FSharp.Core.dll") -> FSCOREDLLNETCORE259PATH <- d
    | Some _ | None -> ()

    // set FSCOREDLLPATH=%FSCOREDLLPATH%\FSharp.Core.dll
    FSCOREDLLPATH <- FSCOREDLLPATH/"FSharp.Core.dll"
    // set FSCOREDLL20PATH=%FSCOREDLL20PATH%\FSharp.Core.dll
    FSCOREDLL20PATH <- FSCOREDLL20PATH/"FSharp.Core.dll"
    // set FSCOREDLLPORTABLEPATH=%FSCOREDLLPORTABLEPATH%\FSharp.Core.dll
    FSCOREDLLPORTABLEPATH <- FSCOREDLLPORTABLEPATH/"FSharp.Core.dll"
    // set FSCOREDLLNETCOREPATH=%FSCOREDLLNETCOREPATH%\FSharp.Core.dll
    FSCOREDLLNETCOREPATH <- FSCOREDLLNETCOREPATH/"FSharp.Core.dll"
    // set FSCOREDLLNETCORE78PATH=%FSCOREDLLNETCORE78PATH%\FSharp.Core.dll
    FSCOREDLLNETCORE78PATH <- FSCOREDLLNETCORE78PATH/"FSharp.Core.dll"
    // set FSCOREDLLNETCORE259PATH=%FSCOREDLLNETCORE259PATH%\FSharp.Core.dll
    FSCOREDLLNETCORE259PATH <- FSCOREDLLNETCORE259PATH/"FSharp.Core.dll"
    // set FSCOREDLLVPREVPATH=%FSCOREDLLVPREVPATH%\FSharp.Core.dll
    FSCOREDLLVPREVPATH <- FSCOREDLLVPREVPATH/"FSharp.Core.dll"

    X86_PROGRAMFILES, {
        FSCOREDLLPATH = FSCOREDLLPATH;
        FSCOREDLL20PATH = FSCOREDLL20PATH;
        FSCOREDLLPORTABLEPATH = FSCOREDLLPORTABLEPATH;
        FSCOREDLLNETCOREPATH = FSCOREDLLNETCOREPATH;
        FSCOREDLLNETCORE78PATH = FSCOREDLLNETCORE78PATH;
        FSCOREDLLNETCORE259PATH = FSCOREDLLNETCORE259PATH;
        FSDATATPPATH = FSDATATPPATH;
        FSCOREDLLVPREVPATH = FSCOREDLLVPREVPATH }

// REM ===
// REM === Find path to FSC/FSI looking up the registry
// REM === Will set the FSCBinPath env variable.
// REM === This if for Dev11+/NDP4.5
// REM === Works on both XP and Vista and hopefully everything else
// REM === Works on 32bit and 64 bit, no matter what cmd prompt it is invoked from
// REM === 
let private SetFSCBinPath45 () =
    // FOR /F "tokens=1-2*" %%a IN ('reg query "%REG_SOFTWARE%\Microsoft\FSharp\4.1\Runtime\v4.0" /ve') DO set FSCBinPath=%%c
    // FOR /F "tokens=1-3*" %%a IN ('reg query "%REG_SOFTWARE%\Microsoft\FSharp\4.1\Runtime\v4.0" /ve') DO set FSCBinPath=%%d
    // IF EXIST "%FSCBinPath%" goto :EOF
    let hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
    match hklm32 |> regQuery @"SOFTWARE\Microsoft\FSharp\4.1\Runtime\v4.0" "" with
    | Some (:? string as d) when directoryExists d -> Some d
    | Some _ | None -> None

let private getMsbuildPath envVars x86_ProgramFiles corDir corDir40 =
        // rem first see if we have got msbuild installed
        let mutable MSBuildToolsPath = envVars |> Map.tryFind "MSBuildToolsPath"
        let VS150COMNTOOLS = match envVars |> Map.tryFind "VS150COMNTOOLS" with | Some x -> x | None -> ""

        // if exist "%X86_PROGRAMFILES%\MSBuild\15.0\Bin\MSBuild.exe" SET MSBuildToolsPath=%X86_PROGRAMFILES%\MSBuild\15.0\Bin\
        if VS150COMNTOOLS/".."/".."/"MSBuild"/"15.0"/"Bin"/"MSBuild.exe" |> fileExists then
            MSBuildToolsPath <- Some (VS150COMNTOOLS/".."/".."/"MSBuild"/"15.0"/"Bin" |> Commands.pathAddBackslash)
        if x86_ProgramFiles/"MSBuild"/"15.0"/"Bin"/"MSBuild.exe" |> fileExists then
            MSBuildToolsPath <- Some (x86_ProgramFiles/"MSBuild"/"15.0"/"Bin" |> Commands.pathAddBackslash)
        if x86_ProgramFiles/"MSBuild"/"14.0"/"Bin"/"MSBuild.exe" |> fileExists then
            MSBuildToolsPath <- Some (x86_ProgramFiles/"MSBuild"/"14.0"/"Bin" |> Commands.pathAddBackslash)

        // if not "%MSBuildToolsPath%" == "" goto done_MsBuildToolsPath
        match MSBuildToolsPath with
        | Some x -> Some x
        | None ->
            let mutable MSBuildToolsPath = None
            // IF NOT "%CORDIR%"=="" IF EXIST "%CORDIR%\msbuild.exe"         SET MSBuildToolsPath=%CORDIR%
            if not (corDir = "") then 
                if corDir/"msbuild.exe" |> fileExists 
                then MSBuildToolsPath <- Some corDir
            // IF     "%CORDIR40%"=="" IF NOT "%CORDIR%"=="" IF EXIST "%CORDIR%\..\V3.5\msbuild.exe" SET MSBuildToolsPath="%CORDIR%\..\V3.5\"
            if (corDir40 |> Option.isNone) then
                if (not (corDir = "")) then
                    if corDir/".."/"V3.5"/"msbuild.exe" |> fileExists
                    then MSBuildToolsPath <- Some (corDir/".."/"V3.5")

            // IF NOT "%CORDIR%"=="" FOR /f %%j IN ("%MSBuildToolsPath%") do SET MSBuildToolsPath=%%~fj
            if (not (corDir = "")) 
            then MSBuildToolsPath <- (MSBuildToolsPath |> Option.map Path.GetFullPath)
            MSBuildToolsPath
        // :done_MsBuildToolsPath



let config envVars =
    // set SCRIPT_ROOT=%_SCRIPT_DRIVE%%_SCRIPT_PATH%
    let SCRIPT_ROOT = __SOURCE_DIRECTORY__ |> Path.GetFullPath

    let env key = envVars |> Map.tryFind key
    let envOrDefault key def = defaultArg (env key) def
    let envOrFail key = env key |> function Some x -> x | None -> failwithf "environment variable '%s' required " key
    let where = Commands.where envVars

    let PROCESSOR_ARCHITECTURE = WindowsPlatform.processorArchitecture envVars

    // REM Do we know where fsc.exe is?
    // IF DEFINED FSCBinPath goto :FSCBinPathFound
    // FOR /F "delims=" %%i IN ('where fsc.exe') DO SET FSCBinPath=%%~dpi
    // :FSCBinPathFound
    let mutable FSCBinPath =
        match env "FSCBINPATH" with
        | Some p -> Some p
        | None -> where "fsc.exe" |> Option.map Path.GetDirectoryName

    // if not exist "%FSCBinPath%\fsc.exe" call :SetFSCBinPath45
    if not (FSCBinPath |> Option.map (fun dir -> dir/"fsc.exe") |> Option.exists fileExists)
    then FSCBinPath <- SetFSCBinPath45 ()

    // REM add %FSCBinPath% to path only if not already there. Otherwise, the path keeps growing.
    // echo %path%; | find /i "%FSCBinPath%;" > NUL
    // if ERRORLEVEL 1    set PATH=%PATH%;%FSCBinPath%
    //REVIEW add it? or better use only env var?

    // if "%FSDIFF%"=="" set FSDIFF=%SCRIPT_ROOT%fsharpqa\testenv\bin\diff.exe
    let FSDIFF = envOrDefault "FSDIFF" (SCRIPT_ROOT/"fsharpqa"/"testenv"/"bin"/"diff.exe")

    // rem check if we're already configured, if not use the configuration from the last line of the config file
    // if "%fsc%"=="" ( 
    //   set csc_flags=/nologo
    //   set fsiroot=fsi
    // )
    let mutable FSC = env "fsc"
    let csc_flags =
        match FSC with None -> "/nologo" | Some _ -> (envOrDefault "csc_flags" "/nologo")
    let mutable fsiroot =
        match FSC with None -> Some "fsi" | Some _ -> (env "fsiroot")

    // if not defined ALINK  set ALINK=al.exe
    let mutable ALINK = (envOrDefault "ALINK" "al.exe")
    let BUILD_CONFIG = envOrDefault "BUILD_CONFIG" "release"

    // REM SDK Dependencires.
    // if not defined ILDASM   set ILDASM=ildasm.exe
    let mutable ILDASM = envOrDefault "ILDASM" "ildasm.exe"
    let mutable SN = envOrDefault "SN" "sn.exe"
    // if not defined PEVERIFY set PEVERIFY=peverify.exe
    let mutable PEVERIFY = envOrDefault "PEVERIFY" "peverify.exe"
    // if not defined RESGEN   set RESGEN=resgen.exe
    let mutable RESGEN = envOrDefault "RESGEN" "resgen.exe"

    // if "%fsiroot%" == "" ( set fsiroot=fsi)
    if fsiroot |> Option.isNone then 
        fsiroot <- Some "fsi"

    // SET OSARCH=%PROCESSOR_ARCHITECTURE%
    // IF NOT "%PROCESSOR_ARCHITEW6432%"=="" SET OSARCH=%PROCESSOR_ARCHITEW6432%
    let OSARCH = WindowsPlatform.osArch envVars
         
    // IF "%fsiroot%"=="fsi" IF NOT "%OSARCH%"=="x86" (
    //   SET fsiroot=fsiAnyCPU
    //   set FSC_BASIC_64=FSC_BASIC_64
    // )
    let mutable FSC_BASIC_64 = env "FSC_BASIC_64"
    match fsiroot, OSARCH with
    | Some "fsi", X86 -> ()
    | Some "fsi", _ ->
        fsiroot <- Some "fsiAnyCPU"
        FSC_BASIC_64 <- Some "FSC_BASIC_64"
    | _ -> ()


    // set fsc_flags=%fsc_flags% 
    let mutable fsc_flags = env "fsc_flags"

    // set CORDIR=%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\
    let SystemRoot = envOrFail "SystemRoot"
    let mutable CORDIR = SystemRoot/"Microsoft.NET"/"Framework"/"v2.0.50727" |> Commands.pathAddBackslash
    // set CORDIR40=
    // FOR /D %%i IN (%windir%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR40=%%i
    let windir = envOrFail "windir"
    let CORDIR40 =
        match Directory.EnumerateDirectories (windir/"Microsoft.NET"/"Framework", "v4.0.?????") |> List.ofSeq |> List.rev with
        | x :: _ -> Some x
        | [] -> None
    // IF NOT "%CORDIR40%"=="" set CORDIR=%CORDIR40%
    match CORDIR40 with
    | None -> ()
    | Some d -> CORDIR <- d

    // REM == Use the same runtime as our architecture
    // REM == ASSUMPTION: This could be a good or bad thing.
    // IF /I NOT "%PROCESSOR_ARCHITECTURE%"=="x86" set CORDIR=%CORDIR:Framework=Framework64%
    match PROCESSOR_ARCHITECTURE with 
    | X86 -> () 
    | _ -> CORDIR <- CORDIR.Replace("Framework", "Framework64")

    let regQueryREG_SOFTWARE path value =
        let hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
        match hklm32 |> regQuery path value with
        | Some (:? string as d) -> Some d
        | Some _ | None -> None

    let allSDK = seq {
    // FOR /F "tokens=2* delims=	 "  %%A IN ('reg QUERY "%REG_SOFTWARE%\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET CORSDK=%%B
            yield regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" "InstallationFolder";
    // if "%CORSDK%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('reg QUERY "%REG_SOFTWARE%\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET CORSDK=%%B
            yield regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" "InstallationFolder";
    // if "%CORSDK%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('reg QUERY "%REG_SOFTWARE%\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET CORSDK=%%B
            yield regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" "InstallationFolder";
    // if "%CORSDK%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('reg QUERY "%REG_SOFTWARE%\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET CORSDK=%%B
            yield regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" "InstallationFolder";
    // if "%CORSDK%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('reg QUERY "%REG_SOFTWARE%\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET CORSDK=%%B
            yield regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" "InstallationFolder";
        }

    let mutable CORSDK = allSDK |> Seq.tryPick id

    // REM == Fix up CORSDK for 64bit platforms...
    // IF /I "%PROCESSOR_ARCHITECTURE%"=="AMD64" SET CORSDK=%CORSDK%\x64
    // IF /I "%PROCESSOR_ARCHITECTURE%"=="IA64"  SET CORSDK=%CORSDK%\IA64
    match PROCESSOR_ARCHITECTURE with
    | AMD64 -> CORSDK <- CORSDK |> Option.map (fun dir -> dir/"x64")
    | IA64 -> CORSDK <- CORSDK |> Option.map (fun dir -> dir/"IA64")
    | _ -> ()

    // REM add powerpack to flags only if not already there. Otherwise, the variable can keep growing.
    // echo %fsc_flags% | find /i "powerpack"
    // if ERRORLEVEL 1 set fsc_flags=%fsc_flags% -r:System.Core.dll --nowarn:20
    if fsc_flags |> Option.exists (fun flags -> flags.ToLower().Contains("powerpack")) then ()
    else fsc_flags <- Some (sprintf "%s -r:System.Core.dll --nowarn:20" (defaultArg fsc_flags ""))

    // if not defined fsi_flags set fsi_flags=%fsc_flags:--define:COMPILED=% --define:INTERACTIVE --maxerrors:1 --abortonerror
    let mutable fsi_flags = env "fsi_flags"
    if fsi_flags |> Option.isNone then (
        let fsc_flags_no_compiled = fsc_flags |> Option.fold (fun _ flags -> flags.Replace("--define:COMPILED", "")) ""
        fsi_flags <- Some (sprintf "%s --define:INTERACTIVE --maxerrors:1 --abortonerror" fsc_flags_no_compiled)
    )

    // echo %fsc_flags%; | find "--define:COMPILED" > NUL || (
    //     set fsc_flags=%fsc_flags% --define:COMPILED
    // )
    if not (fsc_flags |> Option.exists (fun flags -> flags.Contains("--define:COMPILED"))) then
        fsc_flags <- Some (sprintf "%s --define:COMPILED" (defaultArg fsc_flags ""))

    // if "%CORDIR%"=="unknown" set CORDIR=
    if CORDIR = "unknown" then CORDIR <- ""

    // REM use short names in the path so you don't have to deal with the space in things like "Program Files"
    // for /f "delims=" %%I in ("%CORSDK%") do set CORSDK=%%~dfsI%
    CORSDK <- CORSDK |> Option.map Commands.convertToShortPath

    // for /f "delims=" %%I in ("%CORDIR%") do set CORDIR=%%~dfsI%
    CORDIR <- Commands.convertToShortPath CORDIR


    // set NGEN=
    let mutable NGEN = None

    // IF NOT "%CORDIR%"=="" IF EXIST "%CORDIR%\csc.exe" SET CSC="%CORDIR%\csc.exe" %csc_flags%
    let mutable CSC = None
    if not (CORDIR = "") then
        if CORDIR/"csc.exe" |> fileExists 
        then CSC <- Some (CORDIR/"csc.exe")

    // IF NOT "%CORDIR%"=="" IF EXIST "%CORDIR%\ngen.exe"            SET NGEN=%CORDIR%\ngen.exe
    if not (CORDIR = "") then 
        if CORDIR/"ngen.exe" |> fileExists
        then NGEN <- Some (CORDIR/"ngen.exe")

    // IF NOT "%CORDIR%"=="" IF EXIST "%CORDIR%\al.exe"              SET ALINK=%CORDIR%\al.exe
    if not (CORDIR = "") then
        if CORDIR/"al.exe" |> fileExists 
        then ALINK <- CORDIR/"al.exe"

    // IF NOT "%CORSDK%"=="" IF EXIST "%CORSDK%\ildasm.exe"          SET ILDASM=%CORSDK%\ildasm.exe
    match CORSDK |> Option.map (fun d -> d/"ildasm.exe") with
    | Some p when fileExists p -> ILDASM <- p
    | Some _ | None -> ()
        
    match CORSDK |> Option.map (fun d -> d/"sn.exe") with
    | Some p when fileExists p -> SN <- p
    | Some _ | None -> ()
        
    // IF NOT "%CORSDK%"=="" IF EXIST "%CORSDK%\peverify.exe"        SET PEVERIFY=%CORSDK%\peverify.exe
    match CORSDK |> Option.map (fun d -> d/"peverify.exe") with
    | Some p when fileExists p -> PEVERIFY <- p
    | Some _ | None -> ()

    // IF NOT "%CORSDK%"=="" IF EXIST "%CORSDK%\resgen.exe"          SET RESGEN=%CORSDK%\resgen.exe
    // IF NOT "%CORSDK%"=="" IF NOT EXIST "%RESGEN%" IF EXIST "%CORSDK%\..\resgen.exe"       SET RESGEN=%CORSDK%\..\resgen.exe
    match CORSDK with
    | Some sdk ->
        if sdk/"resgen.exe" |> fileExists then RESGEN <- sdk/"resgen.exe"
        elif sdk/".."/"resgen.exe" |> fileExists then RESGEN <- sdk/".."/"resgen.exe"
    | None -> ()

    // IF NOT "%CORSDK%"=="" IF EXIST "%CORSDK%\al.exe"              SET ALINK=%CORSDK%\al.exe
    match CORSDK |> Option.map (fun d -> d/"al.exe") with
    | Some p when fileExists p -> ALINK <- p
    | Some _ | None -> ()

    // IF NOT DEFINED FSC SET FSC=fsc.exe
    let mutable FSC = envOrDefault "FSC" "fsc.exe"
    // IF NOT DEFINED FSI SET FSI=%fsiroot%.exe
    let mutable FSI = envOrDefault "FSI" (fsiroot |> Option.fold (+) ".exe")

    // IF DEFINED FSCBinPath IF EXIST "%FSCBinPath%\fsc.exe"   SET FSC=%FSCBinPath%\fsc.exe
    match FSCBinPath |> Option.map (fun d -> d/"fsc.exe") with
    | Some fscExe when fileExists fscExe -> FSC <- fscExe
    | Some _ | None -> ()

    // IF DEFINED FSCBinPath IF EXIST "%FSCBinPath%\%fsiroot%.exe"   SET FSI=%FSCBinPath%\%fsiroot%.exe
    match FSCBinPath, fsiroot with
    | Some dir, Some fsiExe when fileExists (dir/(fsiExe+".exe")) -> FSI <- dir/(fsiExe+".exe")
    | _ -> ()

    // REM == Located F# library DLLs in either open or Visual Studio contexts
    // call :GetFSLibPaths
    let X86_PROGRAMFILES, libs = GetFSLibPaths envVars OSARCH FSCBinPath

    let orBlank = Option.fold (fun _ x -> x) ""
    let msbuildToolsPath = getMsbuildPath envVars X86_PROGRAMFILES CORDIR CORDIR40

    {
      EnvironmentVariables = envVars
      ALINK = ALINK
      CORDIR = CORDIR |> Commands.pathAddBackslash
      CORSDK = CORSDK |> orBlank |> Commands.pathAddBackslash
      FSCBinPath = FSCBinPath |> orBlank |> Commands.pathAddBackslash
      FSCOREDLL20PATH = libs.FSCOREDLL20PATH
      FSCOREDLLPATH = libs.FSCOREDLLPATH
      FSCOREDLLPORTABLEPATH = libs.FSCOREDLLPORTABLEPATH
      FSCOREDLLNETCOREPATH = libs.FSCOREDLLNETCOREPATH
      FSCOREDLLNETCORE78PATH = libs.FSCOREDLLNETCORE78PATH
      FSCOREDLLNETCORE259PATH = libs.FSCOREDLLNETCORE259PATH
      FSDATATPPATH = libs.FSDATATPPATH
      FSCOREDLLVPREVPATH = libs.FSCOREDLLVPREVPATH
      FSDIFF = FSDIFF
      ILDASM = ILDASM
      SN = SN
      NGEN = NGEN |> orBlank
      PEVERIFY = PEVERIFY
      RESGEN = RESGEN
      CSC = CSC |> orBlank
      BUILD_CONFIG = BUILD_CONFIG
      FSC = FSC
      FSI = FSI
      csc_flags = csc_flags
      fsc_flags = fsc_flags |> orBlank
      fsi_flags = fsi_flags |> orBlank
      Directory=""
      MSBUILDTOOLSPATH = msbuildToolsPath |> Option.map (Commands.pathAddBackslash)
      MSBUILD = msbuildToolsPath |> Option.map (fun d -> d/"msbuild.exe") }
    


let logConfig (cfg: TestConfig) =
    log "---------------------------------------------------------------"
    log "Executables"
    log ""
    log "ALINK               =%s" cfg.ALINK
    log "CORDIR              =%s" cfg.CORDIR
    log "CORSDK              =%s" cfg.CORSDK
    log "CSC                 =%s" cfg.CSC
    log "BUILD_CONFIG        =%s" cfg.BUILD_CONFIG
    log "csc_flags           =%s" cfg.csc_flags
    log "FSC                 =%s" cfg.FSC
    log "fsc_flags           =%s" cfg.fsc_flags
    log "FSCBINPATH          =%s" cfg.FSCBinPath
    log "FSCOREDLL20PATH     =%s" cfg.FSCOREDLL20PATH
    log "FSCOREDLLPATH       =%s" cfg.FSCOREDLLPATH
    log "FSCOREDLLPORTABLEPATH =%s" cfg.FSCOREDLLPORTABLEPATH
    log "FSCOREDLLNETCOREPATH=%s" cfg.FSCOREDLLNETCOREPATH
    log "FSCOREDLLNETCORE78PATH=%s" cfg.FSCOREDLLNETCORE78PATH
    log "FSCOREDLLNETCORE259PATH=%s" cfg.FSCOREDLLNETCORE259PATH
    log "FSCOREDLLVPREVPATH=%s" cfg.FSCOREDLLVPREVPATH
    log "FSDATATPPATH        =%s" cfg.FSDATATPPATH
    log "FSDIFF              =%s" cfg.FSDIFF
    log "FSI                 =%s" cfg.FSI
    log "fsi_flags           =%s" cfg.fsi_flags
    log "ILDASM              =%s" cfg.ILDASM
    log "MSBUILDTOOLSPATH    =%A" cfg.MSBUILDTOOLSPATH
    log "MSBUILD             =%A" cfg.MSBUILD
    log "NGEN                =%s" cfg.NGEN
    log "PEVERIFY            =%s" cfg.PEVERIFY
    log "RESGEN              =%s" cfg.RESGEN
    log "---------------------------------------------------------------"
