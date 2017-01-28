# To use .NET Core SDK

From Command Prompt:

```
.\scripts\use_dotnet_core_sdk.bat
```

From Powershell:

```
.\scripts\use_dotnet_core_sdk.ps1
```

After the script, if successful, the .NET Core SDK is avaiable in `PATH`
so `dotnet --version` will show the expected version

The expected version is configured in `DotnetCoreSdkVersion.txt` file

The .NET Core SDK is downloaded/installed only if not already avaiable, so the 
script can be rerun at will

# NOTE

If the expected version is already avaiable in `PATH` (maybe is already installed globally
in the machine), that installation is used (so no need to download/install)

Otherwise (if the expected version is not avaiable in `PATH`), the .NET Core SDK is downloaded 
and installed (using the `scripts/obtain/dotnet-install` from http://github.com/dotnet/cli repo)
in `.dotnetsdk` directory, and that directory is added to `PATH`
