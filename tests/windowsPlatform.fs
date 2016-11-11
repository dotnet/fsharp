[<RequireQualifiedAccess>]
module WindowsPlatform

open Microsoft.Win32
open System.IO
open System.Text.RegularExpressions
open FSharpTestSuiteTypes
open PlatformHelpers

let clrPaths envVars =

    let parseProcessorArchitecture (s : string) = 
        match s.ToUpper() with
        | "X86" -> X86
        | "IA64" -> IA64
        | "AMD64" -> AMD64
        | _ -> Unknown s

    let regQuery path value (baseKey: RegistryKey) =
        use regKey  = baseKey.OpenSubKey(path, false)
   
        if (regKey = null) then None
        else 
            match regKey.GetValue(value) with
            | null -> None
            | x -> Some x

    let regQueryREG_SOFTWARE path value =
        let hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
        match hklm32 |> regQuery path value with
        | Some (:? string as d) -> Some d
        | Some _ | None -> None


    /// current process architecture, using PROCESSOR_ARCHITECTURE environment variable
    let PROCESSOR_ARCHITECTURE = 
        match envVars |> Map.tryFind "PROCESSOR_ARCHITECTURE" |> Option.map parseProcessorArchitecture with
        | Some x -> x 
        | None -> failwithf "environment variable '%s' required " "PROCESSOR_ARCHITECTURE"

    let windir = 
        match envVars |> Map.tryFind "windir" with
        | Some x -> x 
        | None -> failwithf "environment variable '%s' required " "WINDIR"

    let mutable CORDIR =
        match Directory.EnumerateDirectories (windir/"Microsoft.NET"/"Framework", "v4.0.?????") |> List.ofSeq |> List.rev with
        | x :: _ -> x
        | [] -> failwith "couldn't determine CORDIR"

    // == Use the same runtime as our architecture
    // == ASSUMPTION: This could be a good or bad thing.
    match PROCESSOR_ARCHITECTURE with 
    | X86 -> () 
    | _ -> CORDIR <- CORDIR.Replace("Framework", "Framework64")

    let allSDK = 
         [ regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" "InstallationFolder"
           regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" "InstallationFolder"
           regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" "InstallationFolder"
           regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" "InstallationFolder"
           regQueryREG_SOFTWARE @"Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" "InstallationFolder" ]

    let mutable CORSDK = allSDK |> Seq.tryPick id |> function None -> failwith "couldn't find CORSDK" | Some d -> d

    /// Return real processor architecture (ignore WOW64)
    /// more info: http://blogs.msdn.com/b/david.wang/archive/2006/03/26/howto-detect-process-bitness.aspx
    /// use PROCESSOR_ARCHITEW6432 and PROCESSOR_ARCHITECTURE environment variables
    let OSARCH = 
        match envVars |> Map.tryFind "PROCESSOR_ARCHITEW6432" |> Option.map parseProcessorArchitecture with
        | Some arc -> arc
        | None -> PROCESSOR_ARCHITECTURE


    // == Fix up CORSDK for 64bit platforms...
    match PROCESSOR_ARCHITECTURE with
    | AMD64 -> CORSDK <- CORSDK/"x64"
    | IA64 -> CORSDK <- CORSDK/"IA64"
    | _ -> ()

    OSARCH, CORDIR, CORSDK
