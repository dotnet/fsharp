
module UpdateCmd

open System.IO
open NUnit.Framework
open Microsoft.Win32

open PlatformHelpers
open FSharpTestSuiteTypes

type Configuration = 
    | DEBUG
    | RELEASE
    override this.ToString() = 
        match this with
        | DEBUG -> "Debug"
        | RELEASE -> "Release"

type updateCmdArgs = 
    { Configuration : Configuration
      Ngen : bool }

let private regQuery = WindowsPlatform.regQuery

let private checkResult result =
    match result with
    | CmdResult.ErrorLevel (msg, err) -> Failure (RunError.ProcessExecError (msg, err, sprintf "ERRORLEVEL %d" err))
    | CmdResult.Success -> Success ()

let updateCmd envVars args = attempt {
    // @echo off
    // setlocal
    ignore "useless"

    // if /i "%1" == "debug" goto :ok
    // if /i "%1" == "release" goto :ok
    ignore "already validated input"

    // echo adding required strong name verification skipping, and NGening built binaries
    // echo Usage:
    // echo    update.cmd debug [-ngen]
    // echo    update.cmd release [-ngen]
    // exit /b 1
    ignore "useless help"

    //:ok
    let env k () = match envVars |> Map.tryFind k with None -> Failure (sprintf "environment variable '%s' not found" k) | Some x -> Success x
    let ``~dp0`` = __SOURCE_DIRECTORY__
    let exec exe args = 
        log "%s %s" exe args
        use toLog = redirectToLog ()
        Process.exec { RedirectError = Some toLog.Post; RedirectOutput = Some toLog.Post; RedirectInput = None } ``~dp0`` envVars exe args

    // set BINDIR=%~dp0..\%1\net40\bin
    let! binDir = env "FSCBINPATH"

    // if /i "%PROCESSOR_ARCHITECTURE%"=="x86" set X86_PROGRAMFILES=%ProgramFiles%
    // if /I "%PROCESSOR_ARCHITECTURE%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%
    let processorArchitecture = WindowsPlatform.processorArchitecture envVars
    let x86_ProgramFiles = WindowsPlatform.x86ProgramFilesDirectory envVars processorArchitecture

    let! windir = env "windir"

    let REGEXE32BIT path value =
        let hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
        match hklm32 |> regQuery path value with
        | Some (:? string as d) -> Some d
        | Some _ | None -> None

    let allWINSDKNETFXTOOLS = seq {
    //                             FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
        yield REGEXE32BIT @"Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" "InstallationFolder"
    // if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
        yield REGEXE32BIT @"Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" "InstallationFolder"
    // if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
        yield REGEXE32BIT @"Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" "InstallationFolder"
    // if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
        yield REGEXE32BIT @"Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" "InstallationFolder"
    // if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
        yield REGEXE32BIT @"Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" "InstallationFolder"
        }

    let WINSDKNETFXTOOLS = match allWINSDKNETFXTOOLS |> Seq.tryPick id with Some sdk -> sdk | None -> ""

    // set SN32="%WINSDKNETFXTOOLS%sn.exe"
    let SN32 = WINSDKNETFXTOOLS/"sn.exe"
    // set SN64="%WINSDKNETFXTOOLS%x64\sn.exe"
    let SN64 = WINSDKNETFXTOOLS/"x64"/"sn.exe"
    // set NGEN32=%windir%\Microsoft.NET\Framework\v4.0.30319\ngen.exe
    let NGEN32 = windir/"Microsoft.NET"/"Framework"/"v4.0.30319"/"ngen.exe"
    // set NGEN64=%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe
    let NGEN64 = windir/"Microsoft.NET"/"Framework64"/"v4.0.30319"/"ngen.exe"

    let checkResult = function CmdResult.ErrorLevel (msg, err) -> Failure (sprintf "%s. ERRORLEVEL %d" msg err) | CmdResult.Success -> Success ()

    let ngen32 = Commands.ngen exec NGEN32 >> checkResult
    let ngen64 = Commands.ngen exec NGEN64 >> checkResult
    let sn32 = exec SN32 >> checkResult
    let sn64 = exec SN64 >> checkResult

    // rem Disable strong-name validation for F# binaries built from open source that are signed with the microsoft key
    // %SN32% -Vr FSharp.Core,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Build,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Compiler.Hosted,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Compiler,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.Editor,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.LanguageService,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
    // %SN32% -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
    // %SN32% -Vr Unittests,b03f5f7f11d50a3a
    // %SN32% -Vr Salsa,b03f5f7f11d50a3a

    let strongName (snExe: string -> Result<_,_>) = attempt {
        let all = 
            [ "FSharp.Core";
            "FSharp.Build";
            "FSharp.Compiler.Interactive.Settings";"FSharp.Compiler.Hosted";
            "FSharp.Compiler";"FSharp.Compiler.Server.Shared";
            "FSharp.Editor";
            "FSharp.LanguageService";"FSharp.LanguageService.Base";"FSharp.LanguageService.Compiler";
            "FSharp.ProjectSystem.Base";"FSharp.ProjectSystem.FSharp";"FSharp.ProjectSystem.PropertyPages";
            "FSharp.VS.FSI";
            "VisualFSharp.Unittests";
            "VisualFSharp.Salsa" ]
        for a in all do
            snExe (sprintf " -Vr %s,b03f5f7f11d50a3a" a)   |> ignore // ignore result - SN is not needed for tests to pass, and this fails without admin rights

        }

    do! strongName sn32
        
    //if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    do! if processorArchitecture = AMD64 then
            //  %SN64% -Vr FSharp.Core,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Build,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Compiler.Hosted,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Compiler,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.Editor,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.LanguageService,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
            //  %SN64% -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
            //  %SN64% -Vr Unittests,b03f5f7f11d50a3a
            //  %SN64% -Vr Salsa,b03f5f7f11d50a3a
            strongName sn64
        else 
            (fun () -> Success ())
    //)

    // rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
    // if /i not "%2"=="-ngen" goto :donengen

    if args.Ngen then
        // "%NGEN32%" install "%BINDIR%\fsc.exe" /queue:1
        // "%NGEN32%" install "%BINDIR%\fsi.exe" /queue:1
        // "%NGEN32%" install "%BINDIR%\FSharp.Build.dll" /queue:1
        // "%NGEN32%" executeQueuedItems 1
        ngen32 [binDir/"fsc.exe"; binDir/"fsi.exe"; binDir/"FSharp.Build.dll"] |> ignore // Ignore because may fail without admin rights


        // if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
        if processorArchitecture = AMD64 then
            // "%NGEN64%" install "%BINDIR%\fsiAnyCpu.exe" /queue:1
            // "%NGEN64%" install "%BINDIR%\FSharp.Build.dll" /queue:1
            // "%NGEN64%" executeQueuedItems 1
            ngen64 [binDir/"fsiAnyCpu.exe"; binDir/"FSharp.Build.dll"] |> ignore // Ignore because may fail without admin rights
        // )
    //:donengen
    
    }
