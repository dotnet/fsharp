REM == Find out OS architecture, no matter what cmd prompt
SET OSARCH=%PROCESSOR_ARCHITECTURE%
IF NOT "%PROCESSOR_ARCHITEW6432%"=="" SET OSARCH=%PROCESSOR_ARCHITEW6432%

REM == Find out path to native 'Program Files 32bit', no matter what
REM == architecture we are running on and no matter what command
REM == prompt we came from.
IF /I "%OSARCH%"=="x86"   set X86_PROGRAMFILES=%ProgramFiles%
IF /I "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

REM == Set path to FSharp.Data.TypeProviders.dll
REM == This binary is frozen at 4.3.0.0 version
set FSDATATYPEPROVIDERSPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers\FSharp.Data.TypeProviders.dll
IF EXIST "%FSCBinPath%\FSharp.Data.TypeProviders.dll" set FSDATATYPEPROVIDERSPATH=%FSCBinPath%\FSharp.Data.TypeProviders.dll

REM == Copy the FSharp.Data.TypeProvider.dll 
REM == Note: we need this because we are doing white box testing
IF EXIST "%FSDATATYPEPROVIDERSPATH%" copy /y "%FSDATATYPEPROVIDERSPATH%" .

REM == Copy in config files with needed binding redirects
xcopy /RY "%~dp0test.exe.config" "%cd%\test.exe.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\testX64.exe.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize.exe.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize-lib.dll.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\test--optimize-client-of-lib.exe.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\test--optminus--debug.exe.config*"
xcopy /RY "%~dp0test.exe.config" "%cd%\test--optplus--debug.exe.config*"

