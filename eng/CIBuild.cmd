@echo off
rem F# 15.9 servicing CI build entry point.
rem
rem Block 9a: minimal Arcade-engine build (restore + build of the product only).
rem This deliberately does NOT sign / pack / publish / insert yet — those layer in
rem subsequent Block 9 increments once the product build is proven green on the
rem 1ES VS image. The Arcade engine (eng\common\build.ps1) is the same one verified
rem to build the 15.9 product on the dev box (FSharp.Core, FSharp.Compiler.Private,
rem FSharp.Build, vsintegration) after the Block 5.x restore/build fixes.
rem
rem Localization is disabled (XliffTasks 0.2.0-beta-000081 is absent from modern
rem feeds; deferred per the Directory.Build.props S5 note). The build forwards any
rem extra args (e.g. -configuration Release) via %*.
set DisableLocalization=true
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0common\build.ps1" -ci -restore -build %*
exit /b %ERRORLEVEL%
