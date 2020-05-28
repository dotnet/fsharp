The MSBuild.12.0 dependencies have been converted to a NuGet package.

To create an updated version of the package:

1. Copy the appropriate `Microsoft.Build.*.dll` files to this directory.
2. Update the `<version>` element of `MSBuild.v12.0.nuspec`.
3. Run `msbuild MSBuild.v12.0.csproj /t:Pack`
4. Upload `<repo-root>\artifacts\bin\fcs\FSharp.Compiler.Service.MSBuild.v12.0.*.nupkg` to the MyGet feed at
   `https://dotnet.myget.org/F/fsharp/api/v3/index.json`
