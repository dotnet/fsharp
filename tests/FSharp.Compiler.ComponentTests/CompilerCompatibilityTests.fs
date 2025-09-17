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
    
    let runDotnetBuild projectPath useLocalCompiler =
        let args = 
            if useLocalCompiler then
                "build -c Release -p:LoadLocalFSharpBuild=True"
            else
                "build -c Release"
                
        let (exitCode, output, error) = Commands.executeProcess "dotnet" args projectPath
        
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

    [<Fact>]
    member _.``Baseline scenario - Both library and app built with local compiler``() =
        // Clean previous builds
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Build library with local compiler
        let libOutput = runDotnetBuild libProjectPath true
        Assert.Contains("CompilerCompatLib -> ", libOutput)
        
        // Build app with local compiler
        let appOutput = runDotnetBuild appProjectPath true
        Assert.Contains("CompilerCompatApp -> ", appOutput)
        
        // Run app and verify it works
        let appDllPath = Path.Combine(appProjectPath, "bin", "Release", "net8.0", "CompilerCompatApp.dll")
        Assert.True(File.Exists(appDllPath), $"App DLL not found at {appDllPath}")
        
        let (exitCode, output, _error) = runApp appDllPath
        Assert.Equal(0, exitCode)
        Assert.Contains("✓ All compiler compatibility tests passed", output)
        
    [<Fact>]
    member _.``Forward compatibility - Library built with SDK compiler, app with local compiler``() =
        // Clean previous builds
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Build library with SDK compiler
        let libOutput = runDotnetBuild libProjectPath false
        Assert.Contains("CompilerCompatLib -> ", libOutput)
        
        // Build app with local compiler (should use the library built with SDK compiler)
        let appOutput = runDotnetBuild appProjectPath true  
        Assert.Contains("CompilerCompatApp -> ", appOutput)
        
        // Run app and verify it works
        let appDllPath = Path.Combine(appProjectPath, "bin", "Release", "net8.0", "CompilerCompatApp.dll")
        Assert.True(File.Exists(appDllPath), $"App DLL not found at {appDllPath}")
        
        let (exitCode, output, _error) = runApp appDllPath
        Assert.Equal(0, exitCode)
        Assert.Contains("✓ All compiler compatibility tests passed", output)
        
    [<Fact>]  
    member _.``Backward compatibility - Library built with local compiler, app with SDK compiler``() =
        // Clean previous builds
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Build library with local compiler
        let libOutput = runDotnetBuild libProjectPath true
        Assert.Contains("CompilerCompatLib -> ", libOutput)
        
        // Build app with SDK compiler (should use the library built with local compiler)
        let appOutput = runDotnetBuild appProjectPath false
        Assert.Contains("CompilerCompatApp -> ", appOutput)
        
        // Run app and verify it works
        let appDllPath = Path.Combine(appProjectPath, "bin", "Release", "net8.0", "CompilerCompatApp.dll")
        Assert.True(File.Exists(appDllPath), $"App DLL not found at {appDllPath}")
        
        let (exitCode, output, _error) = runApp appDllPath
        Assert.Equal(0, exitCode)
        Assert.Contains("✓ All compiler compatibility tests passed", output)