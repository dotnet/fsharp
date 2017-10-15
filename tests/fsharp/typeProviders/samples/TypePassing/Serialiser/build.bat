del script.exe
call msbuild 
call ..\..\..\..\..\..\debug\net40\bin\fsc.exe script.fsx 
call script.exe
