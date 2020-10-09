namespace Internal.Utilities

open System.Runtime.InteropServices

module internal RidHelpers =

    // Computer valid dotnet-rids for this environment:
    //      https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
    //
    // Where rid is: win, win-x64, win-x86, osx-x64, linux-x64 etc ...
    let probingRids, baseRid, platformRid =
        let processArchitecture = RuntimeInformation.ProcessArchitecture
        let baseRid =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then "win"
            elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then "osx"
            else "linux"
        let platformRid =
            match processArchitecture with
            | Architecture.X64 ->  baseRid + "-x64"
            | Architecture.X86 -> baseRid + "-x86"
            | Architecture.Arm64 -> baseRid + "-arm64"
            | _ -> baseRid + "-arm"
        [| "any"; baseRid; platformRid |], baseRid, platformRid
