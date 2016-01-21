@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.

call %~d0%~p0..\..\..\config.bat

if EXIST provided.dll del provided.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
if errorlevel 1 goto :Error

if EXIST providedJ.dll del providedJ.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
if errorlevel 1 goto :Error

if EXIST providedK.dll del providedK.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
if errorlevel 1 goto :Error

if EXIST provider.dll del provider.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:provider.dll -a  provider.fsx
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:provider_providerAttributeErrorConsume.dll -a  providerAttributeError.fsx
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:provider_ProviderAttribute_EmptyConsume.dll -a  providerAttribute_Empty.fsx
if ERRORLEVEL 1 goto :Error

if EXIST helloWorldProvider.dll del helloWorldProvider.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:helloWorldProvider.dll -a  ..\helloWorld\provider.fsx
if ERRORLEVEL 1 goto :Error

if EXIST MostBasicProvider.dll del MostBasicProvider.dll
if ERRORLEVEL 1 goto :Error

"%FSC%" --out:MostBasicProvider.dll -a  MostBasicProvider.fsx
if ERRORLEVEL 1 goto :Error

set FAILURES=

set TESTS_SIMPLE=neg2h neg4 neg1 neg1_a neg2 neg2c neg2e neg2g neg6
REM neg7 - excluded 
set TESTS_SIMPLE=%TESTS_SIMPLE% InvalidInvokerExpression providerAttributeErrorConsume ProviderAttribute_EmptyConsume

set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNestedNamespaces_Exception
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_NamespaceName_Exception
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_NamespaceName_Empty
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetTypes_Exception
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ResolveTypeName_Exception
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNamespaces_Exception
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParameters_Exception 
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParametersForMethod_Exception 
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetInvokerExpression_Exception 
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetTypes_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ResolveTypeName_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetNamespaces_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParameters_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetStaticParametersForMethod_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_GetInvokerExpression_Null
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_DoesNotHaveConstructor
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ConstructorThrows
set TESTS_WITH_DEFINE=%TESTS_WITH_DEFINE% EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments

REM for running one at a time easily:
REM set TESTS_SIMPLE=neg7
REM set TESTS_WITH_DEFINE=

if "%1"=="" goto :RunAllTests

if "%1"=="--withDefine" goto :RunSpecificWithDefine

call :RunTest %1
goto :ReportResults

:RunSpecificWithDefine
call :RunTestWithDefine %2
goto :ReportResults

:RunAllTests

for %%T in (%TESTS_SIMPLE%) do call :RunTest %%T
for %%T in (%TESTS_WITH_DEFINE%) do call :RunTestWithDefine %%T

:ReportResults
if "%FAILURES%"=="" goto :Ok

echo fsharp %~f0 - Build failed
echo Failures: %FAILURES%
exit /b 1

:Ok
echo Built fsharp %~f0 ok.
endlocal
exit /b 0


:RunTestWithDefine
"%FSC%" --define:%1 --out:provider_%1.dll -a  provider.fsx
if ERRORLEVEL 1 goto :Error

:RunTest
if EXIST %1.bslpp   call :Preprocess "%1" ""
if EXIST %1.vsbslpp call :Preprocess "%1" "vs"

:DoRunTest
call ..\..\single-neg-test.bat %1
if ERRORLEVEL 1 goto Error
GOTO :EOF

:Error
set FAILURES=%FAILURES% %1
GOTO :EOF

:Preprocess
"%FSI%" --exec sed.fsx "<ASSEMBLY>" "%~d0%~p0provider_%1.dll" < %~1.%~2bslpp | fsi --exec sed.fsx "<URIPATH>" "file:///%CD%\\" > %~1.%~2bsl

goto :EOF