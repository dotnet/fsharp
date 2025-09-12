// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.TestHelpers

open System
open System.IO
open System.Reflection
open System.Diagnostics
open System.Text.RegularExpressions
open System.Globalization

[<AutoOpen>]
module VSInstallDiscovery =

    /// Try to locate Visual Studio root directory using various fallback strategies
    let tryLocateVSRoot () =
        // a. FSHARP_VS_INSTALL_DIR (explicit root override)
        let explicitOverride = Environment.GetEnvironmentVariable("FSHARP_VS_INSTALL_DIR")
        if not (String.IsNullOrEmpty explicitOverride) && Directory.Exists explicitOverride then
            Some explicitOverride
        else
            // b. VSAPPIDDIR (derive parent directory if points to IDE folder)
            let vsAppIdDir = Environment.GetEnvironmentVariable("VSAPPIDDIR")
            if not (String.IsNullOrEmpty vsAppIdDir) then
                let parentDir = Path.GetFullPath(Path.Combine(vsAppIdDir, ".."))
                if Directory.Exists parentDir then
                    Some parentDir
                else
                    None
            else
                // c. Highest version among any environment variables matching pattern VS*COMNTOOLS
                let envVars = Environment.GetEnvironmentVariables()
                let vsCommonToolsVars = 
                    [for key in envVars.Keys ->
                        let keyStr = string key
                        if keyStr.StartsWith("VS") && keyStr.EndsWith("COMNTOOLS") then
                            let versionMatch = Regex.Match(keyStr, @"VS(\d+)COMNTOOLS")
                            if versionMatch.Success then
                                let version = Int32.Parse(versionMatch.Groups.[1].Value)
                                let path = string envVars.[key]
                                if not (String.IsNullOrEmpty path) then
                                    Some (version, Path.GetFullPath(Path.Combine(path, "..")))
                                else
                                    None
                            else
                                None
                        else
                            None]
                    |> List.choose id
                    |> List.filter (fun (_, path) -> Directory.Exists path)
                    |> List.sortByDescending fst
                
                match vsCommonToolsVars with
                | (_, path) :: _ -> Some path
                | [] ->
                    // d. vswhere.exe invocation
                    try
                        // Try common locations for vswhere.exe first
                        let programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                        let vsWherePath = Path.Combine(programFiles, "Microsoft Visual Studio", "Installer", "vswhere.exe")
                        
                        let tryVsWhere (vsWhereExePath: string) =
                            if File.Exists vsWhereExePath then
                                try
                                    let psi = ProcessStartInfo(vsWhereExePath, "-latest -products * -requires Microsoft.Component.MSBuild -property installationPath")
                                    psi.UseShellExecute <- false
                                    psi.RedirectStandardOutput <- true
                                    psi.RedirectStandardError <- true
                                    psi.CreateNoWindow <- true
                                    use proc = Process.Start(psi)
                                    proc.WaitForExit(5000) |> ignore // 5 second timeout
                                    if proc.ExitCode = 0 then
                                        let output = proc.StandardOutput.ReadToEnd().Trim()
                                        if not (String.IsNullOrEmpty output) && Directory.Exists output then
                                            Some output
                                        else
                                            None
                                    else
                                        None
                                with
                                | _ -> None
                            else
                                None
                        
                        // Try explicit path first, then fall back to PATH
                        match tryVsWhere vsWherePath with
                        | Some result -> Some result
                        | None ->
                            // Try vswhere from PATH
                            tryVsWhere "vswhere"
                    with
                    | _ -> None

    /// Indicates whether Visual Studio installation was found
    let HasVisualStudio, VSRoot = 
        match tryLocateVSRoot() with 
        | Some root -> true, root 
        | None -> 
            printfn "[FSharp Tests] No Visual Studio installation found. Tests requiring VS editor assemblies will be skipped or run with reduced functionality."
            false, ""

    /// Get Visual Studio probing paths if VS installation is available
    let getVSProbingPaths () =
        if HasVisualStudio then
            [
                Path.Combine(VSRoot, @"IDE\CommonExtensions\Microsoft\Editor")
                Path.Combine(VSRoot, @"IDE\PublicAssemblies")
                Path.Combine(VSRoot, @"IDE\PrivateAssemblies")
                Path.Combine(VSRoot, @"IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices")
                Path.Combine(VSRoot, @"IDE\Extensions\Microsoft\CodeSense\Framework")
                Path.Combine(VSRoot, @"IDE")
            ]
            |> List.filter Directory.Exists
        else
            []

    /// Add VS assembly resolver and return IDisposable for cleanup
    let addVSAssemblyResolver () =
        let probingPaths = getVSProbingPaths()
        
        if probingPaths.IsEmpty then
            printfn "[FSharp Tests] No VS probing paths available. Assembly resolution will use default mechanisms only."
            { new IDisposable with member _.Dispose() = () }
        else
            printfn "[FSharp Tests] Registered VS assembly resolver with %d probing paths" probingPaths.Length
            
            let handler = ResolveEventHandler(fun _ args ->
                let found () =
                    probingPaths |> Seq.tryPick(fun p ->
                        try
                            let name = AssemblyName(args.Name)
                            let codebase = Path.GetFullPath(Path.Combine(p, name.Name) + ".dll")
                            if File.Exists(codebase) then
                                let loadedName = AssemblyName()
                                loadedName.CodeBase <- codebase
                                loadedName.CultureInfo <- Unchecked.defaultof<CultureInfo>
                                loadedName.Version <- Unchecked.defaultof<Version>
                                Some loadedName
                            else 
                                None
                        with 
                        | _ -> None)
                
                match found() with
                | None -> Unchecked.defaultof<Assembly>
                | Some name -> Assembly.Load(name))
            
            AppDomain.CurrentDomain.add_AssemblyResolve(handler)
            
            { new IDisposable with 
                member _.Dispose() = 
                    AppDomain.CurrentDomain.remove_AssemblyResolve(handler) }