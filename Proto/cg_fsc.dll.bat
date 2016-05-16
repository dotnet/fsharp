
pushd bin
call "%~dp0cg.bat" /in fsc.dll  /Trusted_Platform_Assemblies  .\System.Runtime.dll;.\FSharp.Core.dll;;.\FSharp.Compiler.dll;.\System.Console.dll;
popd
