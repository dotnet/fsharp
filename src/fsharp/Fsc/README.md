## How to build fsc

How to build and run the F# compiler

```
dotnet restore Fsc.netcore.fsproj
dotnet build Fsc.netcore.fsproj -c Release
dotnet bin\Release\netcoreapp1.1\fsc.dll
``` 
