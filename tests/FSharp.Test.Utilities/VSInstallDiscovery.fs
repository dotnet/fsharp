// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

/// Test-only Visual Studio installation discovery infrastructure.
/// Provides a centralized, robust, and graceful discovery mechanism for Visual Studio installations 
/// used by integration/editor/unit tests under vsintegration/tests.
module VSInstallDiscovery =

    open System
    open System.IO
    open System.Diagnostics

    /// Result of VS installation discovery
    type VSInstallResult = 
        | Found of installPath: string * source: string
        | NotFound of reason: string

    /// Attempts to find a Visual Studio installation using multiple fallback strategies
    let tryFindVSInstallation () : VSInstallResult =
        
        /// Check if a path exists and looks like a valid VS installation
        let validateVSPath path =
            if String.IsNullOrEmpty(path) then false
            else
                try
                    let fullPath = Path.GetFullPath(path)
                    Directory.Exists(fullPath) &&
                    Directory.Exists(Path.Combine(fullPath, "IDE")) &&
                    (File.Exists(Path.Combine(fullPath, "IDE", "devenv.exe")) ||
                     File.Exists(Path.Combine(fullPath, "IDE", "VSIXInstaller.exe")))
                with
                | _ -> false

        /// Strategy 1: VSAPPIDDIR (derive parent of Common7/IDE)
        let tryVSAppIdDir () =
            let envVar = Environment.GetEnvironmentVariable("VSAPPIDDIR")
            if not (String.IsNullOrEmpty(envVar)) then
                try
                    let parentPath = Path.Combine(envVar, "..")
                    if validateVSPath parentPath then
                        Some (Found (Path.GetFullPath(parentPath), "VSAPPIDDIR environment variable"))
                    else None
                with
                | _ -> None
            else None

        /// Strategy 2: Highest version among VS*COMNTOOLS environment variables
        let tryVSCommonTools () =
            let vsVersions = [
                ("VS180COMNTOOLS", 18) // Visual Studio 2026
                ("VS170COMNTOOLS", 17) // Visual Studio 2022
                ("VS160COMNTOOLS", 16) // Visual Studio 2019  
                ("VS150COMNTOOLS", 15) // Visual Studio 2017
                ("VS140COMNTOOLS", 14) // Visual Studio 2015
                ("VS120COMNTOOLS", 12) // Visual Studio 2013
            ]
            
            vsVersions
            |> List.tryPick (fun (envName, version) ->
                let envVar = Environment.GetEnvironmentVariable(envName)
                if not (String.IsNullOrEmpty(envVar)) then
                    try
                        let parentPath = Path.Combine(envVar, "..")
                        if validateVSPath parentPath then
                            Some (Found (Path.GetFullPath(parentPath), $"{envName} environment variable (VS version {version})"))
                        else None
                    with
                    | _ -> None
                else None)

        /// Strategy 3: vswhere.exe (Visual Studio Installer)
        let tryVSWhere () =
            try
                let programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                let vswherePath = Path.Combine(programFiles, "Microsoft Visual Studio", "Installer", "vswhere.exe")
                
                if File.Exists(vswherePath) then
                    let startInfo = ProcessStartInfo(
                        FileName = vswherePath,
                        Arguments = "-latest -products * -requires Microsoft.Component.MSBuild -property installationPath",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    )
                    
                    use proc = Process.Start(startInfo)
                    proc.WaitForExit(5000) |> ignore // 5 second timeout
                    
                    if proc.ExitCode = 0 then
                        let output = proc.StandardOutput.ReadToEnd().Trim()
                        if validateVSPath output then
                            Some (Found (Path.GetFullPath(output), "vswhere.exe discovery"))
                        else None
                    else None
                else None
            with
            | _ -> None

        // Try each strategy in order of precedence
        match tryVSAppIdDir () with
        | Some result -> result
        | None ->
            match tryVSCommonTools () with
            | Some result -> result
            | None ->
                match tryVSWhere () with
                | Some result -> result
                | None -> NotFound "No Visual Studio installation found using any discovery method"

    /// Gets the VS installation directory, with graceful fallback behavior.
    /// Returns None if no VS installation can be found, allowing callers to handle gracefully.
    let tryGetVSInstallDir () : string option =
        match tryFindVSInstallation () with
        | Found (path, _) -> Some path
        | NotFound _ -> None

    /// Gets the VS installation directory with detailed logging.
    /// Useful for debugging installation discovery issues in tests.
    let getVSInstallDirWithLogging (logAction: string -> unit) : string option =
        match tryFindVSInstallation () with
        | Found (path, source) -> 
            logAction $"Visual Studio installation found at: {path} (via {source})"
            Some path
        | NotFound reason -> 
            logAction $"Visual Studio installation not found: {reason}"
            None
