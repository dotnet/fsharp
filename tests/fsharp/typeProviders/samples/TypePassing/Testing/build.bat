del script.exe
call msbuild 
call ..\..\..\..\..\..\Release\net40\bin\fsc.exe script.fsx 
call script.exe
