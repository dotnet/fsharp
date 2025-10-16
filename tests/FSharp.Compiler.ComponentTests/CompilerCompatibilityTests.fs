module FSharp.Compiler.ComponentTests.CompilerCompatibilityTests


open System.IO
open Xunit
open TestFramework

[<FSharp.Test.RunTestCasesInSequence>]
type CompilerCompatibilityTests() =

    let projectsPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../projects/CompilerCompat"))
    let libProjectPath = Path.Combine(projectsPath, "CompilerCompatLib")
    let appProjectPath = Path.Combine(projectsPath, "CompilerCompatApp")
    
    let runDotnetBuild projectPath compilerVersion =
        let args = 
            match compilerVersion with
            | "local" -> "build -c Release -p:LoadLocalFSharpBuild=True"
            | _ -> "build -c Release"
                
        let (exitCode, output, error) = Commands.executeProcess "dotnet" args projectPath
        
        if exitCode <> 0 then
            failwith $"Build failed with exit code {exitCode}. Output: {output}. Error: {error}"
        
        output
    
    let runApp appBinaryPath =
        Commands.executeProcess "dotnet" appBinaryPath (Path.GetDirectoryName(appBinaryPath))

    let cleanBinObjDirectories projectPath =
        let binPath = Path.Combine(projectPath, "bin")
        let objPath = Path.Combine(projectPath, "obj")
        
        if Directory.Exists(binPath) then
            Directory.Delete(binPath, true)
        if Directory.Exists(objPath) then
            Directory.Delete(objPath, true)

    let getAppDllPath () =
        // The app is built to artifacts directory due to Directory.Build.props
        Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "artifacts", "bin", "CompilerCompatApp", "Release", "net8.0", "CompilerCompatApp.dll")

    [<Theory>]
    [<InlineData("local", "local", "Baseline scenario - Both library and app built with local compiler")>]
    [<InlineData("latest", "local", "Forward compatibility - Library built with latest SDK, app with local compiler")>]
    [<InlineData("local", "latest", "Backward compatibility - Library built with local compiler, app with latest SDK")>]
    member _.``Compiler compatibility test``(libCompilerVersion: string, appCompilerVersion: string, scenarioDescription: string) =
        // Clean previous builds
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Build library with specified compiler version
        let libOutput = runDotnetBuild libProjectPath libCompilerVersion
        Assert.Contains("CompilerCompatLib -> ", libOutput)
        
        // Build app with specified compiler version
        let appOutput = runDotnetBuild appProjectPath appCompilerVersion
        Assert.Contains("CompilerCompatApp -> ", appOutput)
        
        // Run app and verify it works
        let appDllPath = getAppDllPath()
        Assert.True(File.Exists(appDllPath), $"App DLL not found at {appDllPath} for scenario: {scenarioDescription}")
        
        let (exitCode, output, _error) = runApp appDllPath
        Assert.Equal(0, exitCode)
        Assert.Contains("SUCCESS: All compiler compatibility tests passed", output)
        
        // Parse build info from output to validate compiler usage consistency
        let lines = output.Split('\n') |> Array.map (fun s -> s.Trim())
        
        // Extract isLocalBuild values from the output
        let parseIsLocalBuild (prefix: string) =
            lines 
            |> Array.tryFindIndex (fun l -> l.StartsWith(prefix))
            |> Option.bind (fun startIdx ->
                lines 
                |> Array.skip (startIdx + 1)
                |> Array.tryFind (fun l -> l.Contains("Is Local Build: "))
                |> Option.map (fun l -> l.Contains("Is Local Build: true")))
            |> function Some x -> x | None -> false
        
        let libIsLocalBuild = parseIsLocalBuild "Library Build Info:"
        let appIsLocalBuild = parseIsLocalBuild "Application Build Info:"
        
        // Validate that build info matches expected compiler versions
        let expectedLibIsLocal = libCompilerVersion = "local"
        let expectedAppIsLocal = appCompilerVersion = "local"
        
        Assert.True((libIsLocalBuild = expectedLibIsLocal), 
            $"Library build info mismatch: expected isLocalBuild={expectedLibIsLocal} for version '{libCompilerVersion}', but got {libIsLocalBuild}")
        Assert.True((appIsLocalBuild = expectedAppIsLocal), 
            $"Application build info mismatch: expected isLocalBuild={expectedAppIsLocal} for version '{appCompilerVersion}', but got {appIsLocalBuild}")
        
        // Validate consistency: same compiler versions should have same build info
        if libCompilerVersion = appCompilerVersion then
            Assert.True((libIsLocalBuild = appIsLocalBuild), 
                $"Inconsistent build info: both lib and app use '{libCompilerVersion}' but have different isLocalBuild values (lib={libIsLocalBuild}, app={appIsLocalBuild})")
        else
            Assert.True((libIsLocalBuild <> appIsLocalBuild), 
                $"Expected different build info for different compiler versions (lib='{libCompilerVersion}', app='{appCompilerVersion}'), but both have isLocalBuild={libIsLocalBuild}")
        
        // Additional validation: check that we have actual build-time values
        Assert.True((lines |> Array.exists (fun l -> l.Contains("SDK Version:") && not (l.Contains("Unknown")))), 
            "SDK Version should be captured from build-time, not show 'Unknown'")
        Assert.True((lines |> Array.exists (fun l -> l.Contains("F# Compiler Path:") && not (l.Contains("Unknown")))), 
            "F# Compiler Path should be captured from build-time, not show 'Unknown'")
        
        // Validate that local builds have artifacts path and non-local builds don't
        if expectedLibIsLocal then
            Assert.True((lines |> Array.exists (fun l -> l.Contains("Library") && l.Contains("artifacts"))), 
                "Local library build should reference artifacts path")
        if expectedAppIsLocal then
            Assert.True((lines |> Array.exists (fun l -> l.Contains("Application") && l.Contains("artifacts"))), 
                "Local app build should reference artifacts path")
        
        // Ensure build verification section is present
        Assert.True((output.Contains("=== BUILD VERIFICATION ===")), 
            "Build verification section should be present in output")
        Assert.True((output.Contains("==========================")), 
            "Build verification section should be properly formatted")