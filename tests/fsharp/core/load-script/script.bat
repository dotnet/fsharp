@echo off
del 3.exe 2>nul 1>nul
type 1.fsx
type 2.fsx
type 3.fsx
echo Test 1=================================================
"%FSC%" 3.fsx --nologo
3.exe
del 3.exe
echo Test 2=================================================
"%FSI%" 3.fsx
echo Test 3=================================================
"%FSI%" --nologo < pipescr
echo.
echo Test 4=================================================
"%FSI%" usesfsi.fsx
echo Test 5=================================================
"%FSC%" usesfsi.fsx --nologo
echo Test 6=================================================
"%FSC%" usesfsi.fsx --nologo -r FSharp.Compiler.Interactive.Settings
echo Test 7=================================================
"%FSI%" 1.fsx 2.fsx 3.fsx
echo Test 8=================================================
"%FSI%" 3.fsx 2.fsx 1.fsx
echo Test 9=================================================
"%FSI%" multiple-load-1.fsx
echo Test 10=================================================
"%FSI%" multiple-load-2.fsx
echo Test 11=================================================
"%FSC%" FlagCheck.fs --nologo
FlagCheck.exe
del FlagCheck.exe
echo Test 12=================================================
"%FSC%" FlagCheck.fsx  --nologo
FlagCheck.exe
del FlagCheck.exe
echo Test 13=================================================
"%FSI%" load-FlagCheckFs.fsx
echo Test 14=================================================
"%FSI%" FlagCheck.fsx
echo Test 15=================================================
"%FSI%" ProjectDriver.fsx
echo Test 16=================================================
"%FSC%" ProjectDriver.fsx --nologo
ProjectDriver.exe
del ProjectDriver.exe
echo Test 17=================================================
"%FSI%" load-IncludeNoWarn211.fsx
echo Done ==================================================
