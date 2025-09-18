module FSharp.Compiler.ComponentTests.CompilerCompatibilityTests

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Assert
open TestFramework

type CompilerCompatibilityTests() =

    let projectsPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../projects/CompilerCompat"))
    let libProjectPath = Path.Combine(projectsPath, "CompilerCompatLib")
    let appProjectPath = Path.Combine(projectsPath, "CompilerCompatApp")
    
    let createGlobalJson version projectPath =
        let globalJsonContent = 
            match version with
            | "9" -> """{"sdk":{"version":"9.0.100"}}"""
            | _ -> failwith $"Unsupported version for global.json: {version}"
        
        let globalJsonPath = Path.Combine(projectPath, "global.json")
        File.WriteAllText(globalJsonPath, globalJsonContent)
        globalJsonPath
    
    let cleanupGlobalJson globalJsonPath =
        if File.Exists(globalJsonPath) then
            File.Delete(globalJsonPath)
    
    let runDotnetBuild projectPath compilerVersion =
        let globalJsonPath = 
            match compilerVersion with
            | "local" -> None
            | "latest" -> None  
            | version -> Some (createGlobalJson version projectPath)
        
        let args = 
            match compilerVersion with
            | "local" -> "build -c Release -p:LoadLocalFSharpBuild=True"
            | _ -> "build -c Release"
                
        let (exitCode, output, error) = Commands.executeProcess "dotnet" args projectPath
        
        // Cleanup global.json if created
        globalJsonPath |> Option.iter cleanupGlobalJson
        
        if exitCode <> 0 then
            let outputStr = String.concat "\n" output
            let errorStr = String.concat "\n" error
            failwith $"Build failed with exit code {exitCode}. Output: {outputStr}. Error: {errorStr}"
        
        String.concat "\n" output
    
    let runApp appBinaryPath =
        let (exitCode, output, error) = Commands.executeProcess "dotnet" appBinaryPath (Path.GetDirectoryName(appBinaryPath))
        (exitCode, String.concat "\n" output, String.concat "\n" error)
    
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