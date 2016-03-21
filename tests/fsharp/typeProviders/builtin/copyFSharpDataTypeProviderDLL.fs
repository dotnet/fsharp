module CopyFSharpDataTypeProviderDLL

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open PlatformHelpers
open NUnitConf

let copy (cfg: TestConfig) (dir: string) = attempt {
    let fileExists = Commands.fileExists dir >> Option.isSome
    let getfullpath = Commands.getfullpath dir

    let copy_y a = Commands.copy_y dir a >> checkResult

    // REM == Find out OS architecture, no matter what cmd prompt
    // SET OSARCH=%PROCESSOR_ARCHITECTURE%
    // IF NOT "%PROCESSOR_ARCHITEW6432%"=="" SET OSARCH=%PROCESSOR_ARCHITEW6432%
    let osArch = WindowsPlatform.osArch cfg.EnvironmentVariables

    // REM == Find out path to native 'Program Files 32bit', no matter what
    // REM == architecture we are running on and no matter what command
    // REM == prompt we came from.
    // IF /I "%OSARCH%"=="x86"   set X86_PROGRAMFILES=%ProgramFiles%
    // IF /I "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%
    let x86ProgramFiles = WindowsPlatform.x86ProgramFilesDirectory cfg.EnvironmentVariables osArch

    // REM == Set path to FSharp.Data.TypeProviders.dll
    // REM == This binary is frozen at 4.3.0.0 version
    // set FSDATATYPEPROVIDERSPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers\FSharp.Data.TypeProviders.dll
    // IF EXIST "%FSCBinPath%\FSharp.Data.TypeProviders.dll" set FSDATATYPEPROVIDERSPATH=%FSCBinPath%\FSharp.Data.TypeProviders.dll
    let FSDATATYPEPROVIDERSPATH =
        if fileExists (cfg.FSCBinPath/"FSharp.Data.TypeProviders.dll")
        then cfg.FSCBinPath/"FSharp.Data.TypeProviders.dll"
        else x86ProgramFiles/"Reference Assemblies"/"Microsoft"/"FSharp"/".NETFramework"/"v4.0"/"4.3.0.0"/"Type Providers"/"FSharp.Data.TypeProviders.dll"


    // REM == Copy the FSharp.Data.TypeProvider.dll 
    // REM == Note: we need this because we are doing white box testing
    // IF EXIST "%FSDATATYPEPROVIDERSPATH%" copy /y "%FSDATATYPEPROVIDERSPATH%" .
    do! if fileExists FSDATATYPEPROVIDERSPATH
        then copy_y FSDATATYPEPROVIDERSPATH ("."/"FSharp.Data.TypeProviders.dll")
        else Success ()

    // REM == Copy in config files with needed binding redirects
    let xcopy_ry a b =
        let removeReadonly p =
            let attr = File.GetAttributes(p)
            File.SetAttributes(p, attr &&& (~~~ FileAttributes.ReadOnly))

        if fileExists b then removeReadonly (getfullpath b)
        copy_y a b

    let ``test.exe.config`` = __SOURCE_DIRECTORY__/"test.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test.exe.config*"
    do! xcopy_ry ``test.exe.config`` "test.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\testX64.exe.config*"
    do! xcopy_ry ``test.exe.config`` "testX64.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize.exe.config*"
    do! xcopy_ry ``test.exe.config`` "test--optimize.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize-lib.dll.config*"
    do! xcopy_ry ``test.exe.config`` "test--optimize-lib.dll.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize-client-of-lib.exe.config*"
    do! xcopy_ry ``test.exe.config`` "test--optimize-client-of-lib.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test--optminus--debug.exe.config*"
    do! xcopy_ry ``test.exe.config`` "test--optminus--debug.exe.config"
    // xcopy /RY "%~dp0test.exe.config" "%cd%\test--optplus--debug.exe.config*"
    do! xcopy_ry ``test.exe.config`` "test--optplus--debug.exe.config"

    }
