REM ==
REM == Make sure you are using a 2.0 or 3.5 C# compiler
REM == The intent here is to build a 2.0 assembly!
REM == 

IF NOT EXIST x86 mkdir x86
csc /o+ /debug- /platform:x86 /out:x86\ExecAssembly20.exe ExecAssembly.cs

IF NOT EXIST AMD64 mkdir AMD64
csc /o+ /debug- /platform:x64 /out:AMD64\ExecAssembly20.exe ExecAssembly.cs

IF NOT EXIST IA64 mkdir IA64
csc /o+ /debug- /platform:Itanium /out:IA64\ExecAssembly20.exe ExecAssembly.cs

@ECHO Now you should checkout the binaries under testenv\bin\<arch> and
@ECHO replace them with these one you just built!

