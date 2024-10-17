// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.DependencyManager.Nuget

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Security.Cryptography
open System.Text.RegularExpressions
open FSDependencyManager
open Internal.Utilities.FSharpEnvironment

[<AttributeUsage(AttributeTargets.Assembly ||| AttributeTargets.Class, AllowMultiple = false)>]
type DependencyManagerAttribute() =
    inherit Attribute()

// Resolved assembly information
type Resolution =
    {
        NugetPackageId: string
        NugetPackageVersion: string
        PackageRoot: string
        FullPath: string
        AssetType: string
        IsNotImplementationReference: string
        InitializeSourcePath: string
        NativePath: string
    }

/// The result of building the package resolution files.
type PackageBuildResolutionResult =
    {
        success: bool
        projectPath: string
        stdOut: string array
        stdErr: string array
        resolutionsFile: string option
        resolutions: Resolution[]
        references: string list
        loads: string list
        includes: string list
    }

module internal Utilities =

    let verifyFilesExist files =
        files |> List.tryFind (File.Exists >> not) |> Option.isNone

    let findLoadsFromResolutions (resolutions: Resolution[]) =
        resolutions
        |> Array.filter (fun r ->
            not (
                String.IsNullOrEmpty(r.NugetPackageId)
                || String.IsNullOrEmpty(r.InitializeSourcePath)
            )
            && File.Exists(r.InitializeSourcePath))
        |> Array.map (fun r -> r.InitializeSourcePath)
        |> Array.distinct

    let findReferencesFromResolutions (resolutions: Resolution array) =
        let equals (s1: string) (s2: string) =
            String.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) = 0

        resolutions
        |> Array.filter (fun r ->
            not (String.IsNullOrEmpty(r.NugetPackageId) || String.IsNullOrEmpty(r.FullPath))
            && not (equals r.IsNotImplementationReference "true")
            && equals r.AssetType "runtime")
        |> Array.map (fun r -> r.FullPath)
        |> Array.distinct

    let findIncludesFromResolutions (resolutions: Resolution[]) =
        let managedRoots =
            resolutions
            |> Array.filter (fun r ->
                not (String.IsNullOrEmpty(r.NugetPackageId) || String.IsNullOrEmpty(r.PackageRoot))
                && Directory.Exists(r.PackageRoot))
            |> Array.map (fun r -> r.PackageRoot)

        let nativeRoots =
            resolutions
            |> Array.filter (fun r -> not (String.IsNullOrEmpty(r.NugetPackageId) || String.IsNullOrEmpty(r.NativePath)))
            |> Array.map (fun r ->
                if Directory.Exists(r.NativePath) then
                    Some r.NativePath
                elif File.Exists(r.NativePath) then
                    Some(Path.GetDirectoryName(r.NativePath).Replace('\\', '/'))
                else
                    None)
            |> Array.filter (fun r -> r.IsSome)
            |> Array.map (fun r -> r.Value)

        Array.concat [| managedRoots; nativeRoots |] |> Array.distinct

    let getResolutionsFromFile resolutionsFile =
        let lines =
            try
                File
                    .ReadAllText(resolutionsFile)
                    .Split([| '\r'; '\n' |], StringSplitOptions.None)
                |> Array.filter (String.IsNullOrEmpty >> not)
            with _ ->
                [||]

        [|
            for line in lines do
                let fields = line.Split(',')

                if fields.Length < 8 then
                    raise (InvalidOperationException(sprintf "Internal error - Invalid resolutions file format '%s'" line))
                else
                    {
                        NugetPackageId = fields[0]
                        NugetPackageVersion = fields[1]
                        PackageRoot = fields[2]
                        FullPath = fields[3]
                        AssetType = fields[4]
                        IsNotImplementationReference = fields[5]
                        InitializeSourcePath = fields[6]
                        NativePath = fields[7]
                    }
        |]

    /// Return a string array delimited by commas
    /// Note that a quoted string is not going to be mangled into pieces.
    let trimChars = [| ' '; '\t'; '\''; '\"' |]

    let inline private isNotQuotedQuotation (text: string) n = n > 0 && text[n - 1] <> '\\'

    let getOptions text =
        let split (option: string) =
            let pos = option.IndexOf('=')

            let stringAsOpt text =
                if String.IsNullOrEmpty(text) then None else Some text

            let nameOpt =
                if pos <= 0 then
                    None
                else
                    stringAsOpt (option.Substring(0, pos).Trim(trimChars).ToLowerInvariant())

            let valueOpt =
                let valueText =
                    if pos < 0 then option
                    else if pos < option.Length then option.Substring(pos + 1)
                    else ""

                stringAsOpt (valueText.Trim(trimChars))

            nameOpt, valueOpt

        let last = String.length text - 1
        let result = ResizeArray()
        let mutable insideSQ = false
        let mutable start = 0
        let isSeparator c = c = ','

        for i = 0 to last do
            match text[i], insideSQ with
            | c, false when isSeparator c -> // split when seeing a separator
                result.Add(text.Substring(start, i - start))
                insideSQ <- false
                start <- i + 1
            | _, _ when i = last -> result.Add(text.Substring(start, i - start + 1))
            | c, true when isSeparator c -> // keep reading if a separator is inside quotation
                insideSQ <- true
            | '\'', _ when isNotQuotedQuotation text i -> // open or close quotation
                insideSQ <- not insideSQ // keep reading
            | _ -> ()

        result |> List.ofSeq |> List.map split

    let executeTool pathToExe arguments workingDir environment timeout =
        match pathToExe with
        | Some path ->
            let errorsList = ResizeArray()
            let outputList = ResizeArray()
            let mutable errorslock = obj
            let mutable outputlock = obj

            let outputDataReceived (message: string) =
                if not (isNull message) then
                    lock outputlock (fun () -> outputList.Add(message))

            let errorDataReceived (message: string) =
                if not (isNull message) then
                    lock errorslock (fun () -> errorsList.Add(message))

            let psi = ProcessStartInfo()
            psi.FileName <- path
            psi.WorkingDirectory <- workingDir
            psi.RedirectStandardOutput <- true
            psi.RedirectStandardError <- true
            psi.Arguments <- arguments
            psi.CreateNoWindow <- true
            psi.EnvironmentVariables.Remove("MSBuildSDKsPath") // Host can sometimes add this, and it can break things

            for varname, value in environment do
                psi.EnvironmentVariables[varname] <- value

            psi.UseShellExecute <- false

            use p = new Process()
            p.StartInfo <- psi

            p.OutputDataReceived.Add(fun a -> outputDataReceived a.Data)
            p.ErrorDataReceived.Add(fun a -> errorDataReceived a.Data)

            if p.Start() then
                p.BeginOutputReadLine()
                p.BeginErrorReadLine()

                if not (p.WaitForExit(timeout)) then
                    // Timed out resolving throw a diagnostic.
                    raise (TimeoutException(SR.timedoutResolvingPackages (psi.FileName, psi.Arguments)))
                else
                    p.WaitForExit()

            p.ExitCode = 0, outputList.ToArray(), errorsList.ToArray()

        | None -> false, Array.empty, Array.empty

    let buildProject projectPath binLogPath timeout =
        let binLoggingArguments =
            match binLogPath with
            | Some(path) ->
                let path =
                    match path with
                    | Some path -> path // specific file
                    | None -> Path.Combine(Path.GetDirectoryName(projectPath), "msbuild.binlog") // auto-generated file

                sprintf "/bl:\"%s\"" path
            | None -> ""

        let timeout =
            match timeout with
            | Some(timeout) -> timeout
            | None -> -1

        let arguments prefix =
            sprintf "%s -restore %s %c%s%c /nologo /t:InteractivePackageManagement" prefix binLoggingArguments '\"' projectPath '\"'

        let workingDir = Path.GetDirectoryName projectPath
        let dotnetHostPath = getDotnetHostPath ()
        let args = arguments "msbuild -v:quiet"
        let success, stdOut, stdErr = executeTool dotnetHostPath args workingDir [] timeout

#if DEBUG
        let diagnostics =
            [|
                $"workingDir:       {workingDir}"
                $"dotnetHostPath:   {dotnetHostPath}"
                $"arguments:        {args}"
            |]

        File.WriteAllLines(Path.Combine(workingDir, "build_CommandLine.txt"), diagnostics)
        File.WriteAllLines(Path.Combine(workingDir, "build_StandardOutput.txt"), stdOut)
        File.WriteAllLines(Path.Combine(workingDir, "build_StandardError.txt"), stdErr)
#endif

        let outputFile = projectPath + ".resolvedReferences.paths"

        let resolutionsFile, resolutions, references, loads, includes =
            if success && File.Exists(outputFile) then
                let resolutions = getResolutionsFromFile outputFile

                let references =
                    (findReferencesFromResolutions resolutions)
                    |> Array.filter (File.Exists)
                    |> Array.toList

                let loads = (findLoadsFromResolutions resolutions) |> Array.toList
                let includes = (findIncludesFromResolutions resolutions) |> Array.toList
                (Some outputFile), resolutions, references, loads, includes
            else
                None, [||], List.empty, List.empty, List.empty

        {
            success = success
            projectPath = projectPath
            stdOut = stdOut
            stdErr = stdErr
            resolutionsFile = resolutionsFile
            resolutions = resolutions
            references = references
            loads = loads
            includes = includes
        }

    let generateSourcesFromNugetConfigs scriptDirectory workingDir timeout =
        let dotnetHostPath = getDotnetHostPath ()
        let args = "nuget list source --format detailed"

        let success, stdOut, stdErr =
            executeTool dotnetHostPath args scriptDirectory [ "DOTNET_CLI_UI_LANGUAGE", "en-us" ] timeout
#if DEBUG
        let diagnostics =
            [|
                $"scriptDirectory:  {scriptDirectory}"
                $"dotnetHostPath:   {dotnetHostPath}"
                $"arguments:        {args}"
            |]

        File.WriteAllLines(Path.Combine(workingDir, "nuget_CommandLine.txt"), diagnostics)
        File.WriteAllLines(Path.Combine(workingDir, "nuget_StandardOutput.txt"), stdOut)
        File.WriteAllLines(Path.Combine(workingDir, "nuget_StandardError.txt"), stdErr)
#else
        ignore workingDir
        ignore stdErr
#endif
        if success then
            // String returned by `dotnet nuget list source --format detailed` is formatted similar to:
            // Registered Sources:
            //  1.  dotnet-eng [Enabled]
            //      https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-eng/nuget/v3/index.json
            //  2.  dotnet-tools [Enabled]
            //      https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json
            //  3.  dotnet5 [Enabled]
            //      https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet5/nuget/v3/index.json
            // Use enabled feeds only (see NuGet.Commands.ListSourceRunner.Run) and strip off the flags.
            let pattern =
                @"(\s*\d+\.+\s*)(?'name'\S*)(\s*)\[(?'enabled'Enabled|Disabled)\](\s*)(?'uri'[^\0\r\n]*)"

            let regex = new Regex(pattern, RegexOptions.ExplicitCapture)

            let sourcelist = String.concat Environment.NewLine stdOut

            String.concat
                Environment.NewLine
                [|
                    for m in regex.Matches(sourcelist) do
                        let name = m.Groups["name"].Value
                        let enabled = m.Groups["enabled"].Value
                        let uri = m.Groups["uri"].Value

                        if enabled.Length > 0 && enabled[0] = 'E' then
                            $"""    <add key="{name}" value="{uri}" />"""
                |]
        else
            ""

    let computeSha256HashOfBytes (bytes: byte[]) : byte[] = SHA256.Create().ComputeHash(bytes)
