@echo on

set

@echo Check Dev 12
dir "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0"

@echo Check Dev 14
dir "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0"

@echo check msbuild 12.0
dir "%ProgramFiles(x86)%\MSBuild\12.0\Bin\"

@echo check msbuild 14.0
dir "%ProgramFiles(x86)%\MSBuild\14.0\Bin\"
