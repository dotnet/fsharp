module FSharpDiagServer.DesignTimeBuild

open System
open System.Diagnostics
open System.IO
open System.Text.Json

type DtbResult =
    { CompilerArgs: string array }

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

        // /t:Build runs BeforeBuild (generates buildproperties.fs via CompileBefore).
        // DesignTimeBuild=true skips dependency projects.
        // SkipCompilerExecution=true + ProvideCommandLineArgs=true populates FscCommandLineArgs without compiling.
        let psi =
            ProcessStartInfo(
                FileName = "dotnet",
                Arguments =
                    $"msbuild \"{fsprojPath}\" /t:Build /p:DesignTimeBuild=true /p:SkipCompilerExecution=true /p:ProvideCommandLineArgs=true /p:CopyBuildOutputToOutputDirectory=false /p:CopyOutputSymbolsToOutputDirectory=false /p:BUILDING_USING_DOTNET=true /p:Configuration={config.Configuration}{tfmArg} /nologo /v:q /getItem:FscCommandLineArgs",
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
                // MSBuild may emit warnings before the JSON; find the JSON start
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

                    return Ok { CompilerArgs = args }
            with ex ->
                return Error $"Failed to parse DTB output: {ex.Message}"
    }
