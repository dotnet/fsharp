module FSharpDiagServer.DesignTimeBuild

open System
open System.Diagnostics
open System.IO
open System.Text.Json

type DtbResult =
    { CompilerArgs: string array
      IntermediateOutputPath: string }

type DtbConfig =
    { TargetFramework: string option
      Configuration: string }

let defaultConfig =
    { TargetFramework = Some "net10.0"
      Configuration = "Release" }

let run (fsprojPath: string) (config: DtbConfig) =
    async {
        let tfmArg =
            config.TargetFramework
            |> Option.map (fun tfm -> $" /p:TargetFramework={tfm}")
            |> Option.defaultValue ""

        let projDir = Path.GetDirectoryName(fsprojPath)
        let projName = Path.GetFileNameWithoutExtension(fsprojPath)

        // Query IntermediateOutputPath to find and delete the intermediate assembly,
        // defeating MSBuild's up-to-date check so CoreCompile actually runs.
        let iopPsi =
            ProcessStartInfo(
                FileName = "dotnet",
                Arguments =
                    $"msbuild \"{fsprojPath}\" /p:BUILDING_USING_DOTNET=true /p:Configuration={config.Configuration}{tfmArg} /nologo /v:q /getProperty:IntermediateOutputPath",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = projDir
            )
        use iopProc = Process.Start(iopPsi)
        let! iopOut = iopProc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask
        do! iopProc.WaitForExitAsync() |> Async.AwaitTask

        let iopOut = iopOut.Trim()
        // Handle both plain path and JSON output from /getProperty
        let intermediateDir =
            if iopOut.StartsWith("{") then
                try
                    let doc = JsonDocument.Parse(iopOut)
                    doc.RootElement.GetProperty("Properties").GetProperty("IntermediateOutputPath").GetString()
                with _ -> ""
            else iopOut
        let intermediateDir =
            if Path.IsPathRooted(intermediateDir) then intermediateDir
            elif intermediateDir.Length > 0 then Path.Combine(projDir, intermediateDir)
            else ""
        if intermediateDir.Length > 0 then
            let intermediateDll = Path.Combine(intermediateDir, projName + ".dll")
            if File.Exists(intermediateDll) then
                try File.Delete(intermediateDll) with _ -> ()

        // /t:CoreCompile + SkipCompilerExecution + ProvideCommandLineArgs populates FscCommandLineArgs.
        // BuildProjectReferences=false avoids rebuilding dependencies.
        let psi =
            ProcessStartInfo(
                FileName = "dotnet",
                Arguments =
                    $"msbuild \"{fsprojPath}\" /t:CoreCompile /p:SkipCompilerExecution=true /p:ProvideCommandLineArgs=true /p:CopyBuildOutputToOutputDirectory=false /p:CopyOutputSymbolsToOutputDirectory=false /p:BUILDING_USING_DOTNET=true /p:BuildProjectReferences=false /p:Configuration={config.Configuration}{tfmArg} /nologo /v:q \"/getItem:FscCommandLineArgs;ReferencePath\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = projDir
            )

        use proc = Process.Start(psi)
        let! stdout = proc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask
        let! stderr = proc.StandardError.ReadToEndAsync() |> Async.AwaitTask
        do! proc.WaitForExitAsync() |> Async.AwaitTask

        if proc.ExitCode <> 0 then
            return Error $"DTB failed (exit {proc.ExitCode}): {stderr}"
        else
            try
                let jsonStart = stdout.IndexOf('{')
                if jsonStart < 0 then
                    return Error $"No JSON in DTB output: {stdout.[..200]}"
                else
                    let doc = JsonDocument.Parse(stdout.Substring(jsonStart))
                    let items = doc.RootElement.GetProperty("Items")

                    let args =
                        items.GetProperty("FscCommandLineArgs").EnumerateArray()
                        |> Seq.map (fun e -> e.GetProperty("Identity").GetString())
                        |> Seq.toArray

                    let refs =
                        match items.TryGetProperty("ReferencePath") with
                        | true, refItems ->
                            refItems.EnumerateArray()
                            |> Seq.map (fun e ->
                                let path = e.GetProperty("Identity").GetString()
                                "-r:" + path)
                            |> Seq.toArray
                        | false, _ -> [||]

                    let combined = Array.append args refs

                    if args.Length = 0 then
                        return Error "DTB returned empty FscCommandLineArgs (CoreCompile was skipped)"
                    else
                        return Ok { CompilerArgs = combined; IntermediateOutputPath = intermediateDir }
            with ex ->
                return Error $"Failed to parse DTB output: {ex.Message}"
    }
