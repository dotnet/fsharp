module FSharp.Compiler.ComponentTests.CompilerCompatibilityTests


open System.IO
open System.Diagnostics
open Xunit
open TestFramework

[<FSharp.Test.RunTestCasesInSequence>]
type CompilerCompatibilityTests() =

    let projectsPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../projects/CompilerCompat"))
    let libProjectPath = Path.Combine(projectsPath, "CompilerCompatLib")
    let appProjectPath = Path.Combine(projectsPath, "CompilerCompatApp")
    
    let createGlobalJson (directory: string) (sdkVersion: string) =
        let globalJsonPath = Path.Combine(directory, "global.json")
        let content = $"""{{
  "sdk": {{
    "version": "{sdkVersion}",
    "rollForward": "latestPatch"
  }}
}}"""
        File.WriteAllText(globalJsonPath, content)
        globalJsonPath
    
    let deleteGlobalJson (directory: string) =
        let globalJsonPath = Path.Combine(directory, "global.json")
        if File.Exists(globalJsonPath) then
            File.Delete(globalJsonPath)
    
    let executeDotnetProcess (command: string) (workingDir: string) (clearDotnetRoot: bool) =
        let psi = ProcessStartInfo()
        // For net9 scenarios, use full path to system dotnet to bypass repo's .dotnet
        // This ensures the spawned process uses system SDKs instead of repo's local SDK
        let dotnetExe =
            if clearDotnetRoot then
                if System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) then
                    @"C:\Program Files\dotnet\dotnet.exe"
                elif System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX) then
                    "/usr/local/share/dotnet/dotnet"
                else // Linux
                    "/usr/share/dotnet/dotnet"
            else
                "dotnet"
        
        psi.FileName <- dotnetExe
        psi.WorkingDirectory <- workingDir
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.Arguments <- command
        psi.CreateNoWindow <- true
        psi.EnvironmentVariables["DOTNET_ROLL_FORWARD"] <- "LatestMajor"
        psi.EnvironmentVariables["DOTNET_ROLL_FORWARD_TO_PRERELEASE"] <- "1"
        psi.EnvironmentVariables.Remove("MSBuildSDKsPath")
        
        // For net9 scenarios, remove DOTNET_ROOT so dotnet looks in its default locations
        if clearDotnetRoot then
            psi.EnvironmentVariables.Remove("DOTNET_ROOT")
        
        psi.UseShellExecute <- false

        use p = new Process()
        p.StartInfo <- psi

        if not (p.Start()) then failwith "new process did not start"

        let readOutput = backgroundTask { return! p.StandardOutput.ReadToEndAsync() }
        let readErrors = backgroundTask { return! p.StandardError.ReadToEndAsync() }

        p.WaitForExit()

        p.ExitCode, readOutput.Result, readErrors.Result
    
    let runDotnetCommand (command: string) (workingDir: string) (compilerVersion: string) =
        let clearDotnetRoot = (compilerVersion = "net9")
        executeDotnetProcess command workingDir clearDotnetRoot
        |> fun (exitCode, stdout, stderr) ->
            if exitCode <> 0 then
                failwith $"Command failed with exit code {exitCode}:\nCommand: dotnet {command}\nStdout:\n{stdout}\nStderr:\n{stderr}"
            stdout
    
    let buildProject (projectPath: string) (compilerVersion: string) =
        // Use the same configuration as the test project itself
#if DEBUG
        let configuration = "Debug"
#else
        let configuration = "Release"
#endif
        let projectDir = Path.GetDirectoryName(projectPath)
        
        // For net9, create a global.json to pin SDK version
        if compilerVersion = "net9" then
            createGlobalJson projectDir "9.0.306" |> ignore
        
        let buildArgs =
            if compilerVersion = "local" then
                $"build \"{projectPath}\" -c {configuration} --no-restore -p:LoadLocalFSharpBuild=true -p:LocalFSharpCompilerConfiguration={configuration}"
            else
                $"build \"{projectPath}\" -c {configuration} --no-restore"
        
        try
            runDotnetCommand buildArgs projectDir compilerVersion
        finally
            // Clean up global.json after build
            if compilerVersion = "net9" then
                deleteGlobalJson projectDir
    
    let packProject (projectPath: string) (compilerVersion: string) (outputDir: string) =
        // Use the same configuration as the test project itself
#if DEBUG
        let configuration = "Debug"
#else
        let configuration = "Release"
#endif
        let projectDir = Path.GetDirectoryName(projectPath)
        
        // For net9, create a global.json to pin SDK version
        if compilerVersion = "net9" then
            createGlobalJson projectDir "9.0.306" |> ignore
        
        let packArgs =
            if compilerVersion = "local" then
                $"pack \"{projectPath}\" -c {configuration} -p:LoadLocalFSharpBuild=true -p:LocalFSharpCompilerConfiguration={configuration} -o \"{outputDir}\""
            else
                $"pack \"{projectPath}\" -c {configuration} -o \"{outputDir}\""
        
        try
            runDotnetCommand packArgs projectDir compilerVersion
        finally
            // Clean up global.json after pack
            if compilerVersion = "net9" then
                deleteGlobalJson projectDir
    
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
        // The app is built to its local bin directory (not artifacts) due to isolated Directory.Build.props
        // Use the same configuration as the test project itself
#if DEBUG
        let configuration = "Debug"
#else
        let configuration = "Release"
#endif
        Path.Combine(appProjectPath, "bin", configuration, "net8.0", "CompilerCompatApp.dll")

    [<Theory>]
    [<InlineData("local", "local", "Baseline scenario - Both library and app built with local compiler")>]
    [<InlineData("latest", "local", "Forward compatibility - Library built with latest SDK, app with local compiler")>]
    [<InlineData("local", "latest", "Backward compatibility - Library built with local compiler, app with latest SDK")>]
    [<InlineData("net9", "local", "Cross-version compatibility - Library built with .NET 9, app with local compiler")>]
    [<InlineData("local", "net9", "Cross-version compatibility - Library built with local compiler, app with .NET 9")>]
    member _.``Compiler compatibility test``(libCompilerVersion: string, appCompilerVersion: string, scenarioDescription: string) =
        // Clean previous builds (no artifacts directory due to isolated Directory.Build.props)
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Create a local NuGet packages directory for this test
        let localNuGetDir = Path.Combine(projectsPath, "local-nuget-packages")
        if Directory.Exists(localNuGetDir) then Directory.Delete(localNuGetDir, true)
        Directory.CreateDirectory(localNuGetDir) |> ignore
        
        // Step 1: Pack the library with the specified compiler version (which will also build it)
        let libProjectFile = Path.Combine(libProjectPath, "CompilerCompatLib.fsproj")
        
        let packOutput = packProject libProjectFile libCompilerVersion localNuGetDir
        Assert.Contains("Successfully created package", packOutput)
        
        // Verify the nupkg file was created
        let nupkgFiles = Directory.GetFiles(localNuGetDir, "CompilerCompatLib.*.nupkg")
        Assert.True(nupkgFiles.Length > 0, $"No .nupkg file found in {localNuGetDir}")
        
        // Step 2: Configure app to use the local NuGet package
        // Create a temporary nuget.config that points to our local package directory
        let appNuGetConfig = Path.Combine(appProjectPath, "nuget.config")
        let nuGetConfigContent = $"""<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="{localNuGetDir}" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>"""
        File.WriteAllText(appNuGetConfig, nuGetConfigContent)
        
        // For net9, create global.json before restore
        if appCompilerVersion = "net9" then
            createGlobalJson appProjectPath "9.0.306" |> ignore
        
        try
            // Step 3: Clear global packages cache to ensure we get the fresh package, then restore the app  
            let appProjectFile = Path.Combine(appProjectPath, "CompilerCompatApp.fsproj")
            let _ = runDotnetCommand "nuget locals global-packages --clear" appProjectPath appCompilerVersion
            let restoreOutput = runDotnetCommand $"restore \"{appProjectFile}\" --force --no-cache" appProjectPath appCompilerVersion
            // Restore may say "Restore complete", "Restored", or "All projects are up-to-date" depending on state
            Assert.True(
                restoreOutput.Contains("Restore complete") || restoreOutput.Contains("Restored") || restoreOutput.Contains("up-to-date"),
                $"Restore did not complete successfully. Output:\n{restoreOutput}")
            
            // Step 4: Build the app with the specified compiler version
            // The app will use the NuGet package we just created and restored
            let appOutput = buildProject appProjectFile appCompilerVersion
            Assert.Contains("CompilerCompatApp -> ", appOutput)
        finally
            // Clean up global.json if we created it
            if appCompilerVersion = "net9" then
                deleteGlobalJson appProjectPath
        
        // Clean up the temporary nuget.config
        File.Delete(appNuGetConfig)
        
        // Step 5: Run the app and verify it works
        let appDllPath = getAppDllPath()
        Assert.True(File.Exists(appDllPath), $"App DLL not found at {appDllPath} for scenario: {scenarioDescription}")
        
        let (exitCode, output, error) = runApp appDllPath
        if exitCode <> 0 then
            printfn $"App failed with exit code {exitCode}"
            printfn $"Output:\n{output}"
            printfn $"Error:\n{error}"
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
        
        // Extract F# Compiler Paths to compare
        let extractCompilerPath (sectionHeader: string) =
            lines 
            |> Array.tryFindIndex (fun l -> l.StartsWith(sectionHeader))
            |> Option.bind (fun startIdx ->
                lines
                |> Array.skip (startIdx + 1)
                |> Array.tryFind (fun l -> l.Contains("F# Compiler Path:"))
                |> Option.map (fun l -> l.Trim()))
        
        let libCompilerPath = extractCompilerPath "Library Build Info:"
        let appCompilerPath = extractCompilerPath "Application Build Info:"
        
        // Validate that F# Compiler Path consistency matches compiler version consistency
        match libCompilerPath, appCompilerPath with
        | Some libPath, Some appPath ->
            if libCompilerVersion = appCompilerVersion then
                Assert.True((libPath = appPath), 
                    $"Same compiler version ('{libCompilerVersion}') should have same F# Compiler Path, but lib has '{libPath}' and app has '{appPath}'")
            else
                Assert.True((libPath <> appPath), 
                    $"Different compiler versions (lib='{libCompilerVersion}', app='{appCompilerVersion}') should have different F# Compiler Paths, but both have '{libPath}'")
        | _ -> Assert.True(false, "Could not extract F# Compiler Path from output")
        
        // Validate that local builds have artifacts path
        // Look for the section header, then check if any subsequent lines contain artifacts
        let hasArtifactsInSection (sectionHeader: string) =
            lines 
            |> Array.tryFindIndex (fun (l: string) -> l.StartsWith(sectionHeader))
            |> Option.map (fun startIdx ->
                lines
                |> Array.skip startIdx
                |> Array.take (min 10 (lines.Length - startIdx)) // Look in next 10 lines
                |> Array.exists (fun l -> l.Contains("artifacts")))
            |> Option.defaultValue false
        
        if expectedLibIsLocal then
            Assert.True(hasArtifactsInSection "Library Build Info:", 
                "Local library build should reference artifacts path")
        if expectedAppIsLocal then
            Assert.True(hasArtifactsInSection "Application Build Info:", 
                "Local app build should reference artifacts path")
        
        // Ensure build verification section is present
        Assert.True((output.Contains("=== BUILD VERIFICATION ===")), 
            "Build verification section should be present in output")
        Assert.True((output.Contains("==========================")), 
            "Build verification section should be properly formatted")