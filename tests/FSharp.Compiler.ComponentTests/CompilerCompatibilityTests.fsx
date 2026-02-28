#!/usr/bin/env dotnet fsi

(*
# F# Compiler Compatibility Test Suite

## What This Does

This test suite verifies **binary compatibility** of F# anonymous records across different F# compiler versions. It is meant as a place to grow by any other testing use case that wants to verify pickling handover, anon records are just the first pilot. For expanding this, just add more code to lib+app.
It ensures that libraries and applications compiled with different F# compilers can interoperate correctly, 
focusing on the binary serialization format (pickle format) of anonymous records.

The test suite exercises three critical compatibility scenarios:
1. **Baseline**: Both library and application built with the local (development) compiler
2. **Forward Compatibility**: Library built with SDK compiler, application with local compiler  
3. **Backward Compatibility**: Library built with local compiler, application with SDK compiler

## Why This Matters - Binary Compatibility of Pickle Format

F# uses a binary serialization format (pickle format) to encode type information and metadata for all signatures and also optimization related data.

**The Problem**: When the F# compiler changes, the pickle format can evolve. If not carefully managed, this can break binary compatibility:
- A library compiled with F# 9.0 might generate anonymous records that F# 8.0 can't read
- Breaking changes in the pickle format can cause compilation failures or incorrect behavior
- Even minor compiler changes can inadvertently alter binary serialization

**Why Anonymous Records**:  They just happen to be the fist use case:

This test suite acts as a **regression guard** to catch any changes that would break binary compatibility,
ensuring the F# ecosystem remains stable as the compiler evolves.

## How It Works

### 1. MSBuild Integration

The test controls which F# compiler is used through MSBuild properties:

**Local Compiler** (`LoadLocalFSharpBuild=True`):
- Uses the freshly-built compiler from `artifacts/bin/fsc`
- Configured via `UseLocalCompiler.Directory.Build.props` in repo root
- Allows testing bleeding-edge compiler changes

**SDK Compiler** (`LoadLocalFSharpBuild=False` or not set):
- Uses the F# compiler from the installed .NET SDK
- Represents what users have in production

### 2. Global.json Management

For testing specific .NET versions, the suite dynamically creates `global.json` files:

```json
{
  "sdk": {
    "version": "9.0.300",
    "rollForward": "latestMinor"
  }
}
```

This allows testing compatibility with specific SDK versions (like .NET 9) without requiring 
hardcoded installations. The `rollForward: latestMinor` policy provides flexibility across patch versions.

### 3. Build-Time Verification

Each project generates a `BuildInfo.fs` file at build time using MSBuild targets:

```xml
<Target Name="GenerateLibBuildInfo" BeforeTargets="BeforeCompile">
  <WriteLinesToFile File="LibBuildInfo.fs"
    Lines="module LibBuildInfo =
      let sdkVersion = &quot;$(NETCoreSdkVersion)&quot;
      let fsharpCompilerPath = &quot;$(FscToolPath)\$(FscToolExe)&quot;
      let dotnetFscCompilerPath = &quot;$(DotnetFscCompilerPath)&quot;
      let isLocalBuild = $(IsLocalBuildValue)" />
</Target>
```

This captures actual build-time information, allowing tests to verify which compiler was actually used.

### 4. Test Flow

For each scenario:
1. **Clean** previous builds to ensure isolation
2. **Pack** the library with specified compiler (creates NuGet package)
3. **Build** the application with specified compiler, referencing the packed library
4. **Run** the application and verify:
   - Anonymous records work correctly across compiler boundaries
   - Build info confirms correct compilers were used
   - No runtime errors or data corruption

### 5. Anonymous Record Testing

The library (`CompilerCompatLib`) exposes APIs using anonymous records:
- Simple anonymous records: `{| X = 42; Y = "hello" |}`
- Nested anonymous records: `{| Simple = {| A = 1 |}; List = [...] |}`
- Complex structures mixing anonymous records with other F# types

The application (`CompilerCompatApp`) consumes these APIs and validates that:
- Field access works correctly
- Nested structures are properly preserved
- Type information matches expectations

This ensures the binary pickle format remains compatible even when compilers change.

## Running the Tests

**Standalone script:**
```bash
dotnet fsi tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fsx
```

## Extending the Test Suite

To add more compatibility tests:
1. Add new functions to `CompilerCompatLib/Library.fs` 
2. Add corresponding validation in `CompilerCompatApp/Program.fs`
3. The existing test infrastructure will automatically verify compatibility

*)

// Standalone F# script to test compiler compatibility across different F# SDK versions
// Can be run with: dotnet fsi CompilerCompatibilityTests.fsx

open System
open System.IO
open System.Diagnostics

// Configuration
let compilerConfiguration = "Release"
let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../.."))
let projectsPath = Path.Combine(__SOURCE_DIRECTORY__, "../projects/CompilerCompat")
let libProjectPath = Path.Combine(projectsPath, "CompilerCompatLib")
let appProjectPath = Path.Combine(projectsPath, "CompilerCompatApp")

// Test scenarios: (libCompiler, appCompiler, description)
let testScenarios = [
    ("local", "local", "Baseline - Both library and app built with local compiler")
    ("latest", "local", "Forward compatibility - Library with SDK, app with local")
    ("local", "latest", "Backward compatibility - Library with local, app with SDK")
    ("latest", "latest", "SDK only - Both library and app built with latest SDK")
    ("net9", "local", "Net9 forward compatibility - Library with .NET 9 SDK, app with local")
    ("local", "net9", "Net9 backward compatibility - Library with local, app with .NET 9 SDK")
]

// Helper functions
let runCommand (command: string) (args: string) (workingDir: string) (envVars: (string * string) list) =
    let psi = ProcessStartInfo()
    psi.FileName <- command
    psi.Arguments <- args
    psi.WorkingDirectory <- workingDir
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardError <- true
    psi.UseShellExecute <- false
    psi.CreateNoWindow <- true
    
    // Set environment variables
    for (key, value) in envVars do
        psi.EnvironmentVariables.[key] <- value
    
    use p = new Process()
    p.StartInfo <- psi
    
    if not (p.Start()) then
        failwith $"Failed to start process: {command} {args}"
    
    let stdout = p.StandardOutput.ReadToEnd()
    let stderr = p.StandardError.ReadToEnd()
    p.WaitForExit()
    
    if p.ExitCode <> 0 then
        printfn "Command failed: %s %s" command args
        printfn "Working directory: %s" workingDir
        printfn "Exit code: %d" p.ExitCode
        printfn "Stdout: %s" stdout
        printfn "Stderr: %s" stderr
        failwith $"Command exited with code {p.ExitCode}"
    
    stdout

let cleanDirectory path =
    if Directory.Exists(path) then
        Directory.Delete(path, true)

let cleanBinObjDirectories projectPath =
    cleanDirectory (Path.Combine(projectPath, "bin"))
    cleanDirectory (Path.Combine(projectPath, "obj"))
    let libBuildInfo = Path.Combine(projectPath, "LibBuildInfo.fs")
    let appBuildInfo = Path.Combine(projectPath, "AppBuildInfo.fs")
    if File.Exists(libBuildInfo) then File.Delete(libBuildInfo)
    if File.Exists(appBuildInfo) then File.Delete(appBuildInfo)

let manageGlobalJson compilerVersion enable =
    let globalJsonPath = Path.Combine(projectsPath, "global.json")
    if compilerVersion = "net9" then
        if enable && not (File.Exists(globalJsonPath) && File.ReadAllText(globalJsonPath).Contains("9.0.0")) then
            printfn "  Enabling .NET 9 SDK via global.json..."
            let globalJsonContent = """{
  "sdk": {
    "version": "9.0.0",
    "rollForward": "latestMajor"
  },
  "msbuild-sdks": {
    "Microsoft.DotNet.Arcade.Sdk": "11.0.0-beta.25509.1"
  }
}"""
            File.WriteAllText(globalJsonPath, globalJsonContent)
        elif not enable && File.Exists(globalJsonPath) then
            printfn "  Removing global.json..."
            File.Delete(globalJsonPath)

let packProject projectPath compilerVersion outputDir =
    let useLocal = (compilerVersion = "local")
    // Use timestamp-based version to ensure fresh package each time
    let timestamp = DateTime.Now.ToString("HHmmss")
    let envVars = [
        ("LoadLocalFSharpBuild", if useLocal then "True" else "False")
        ("LocalFSharpCompilerConfiguration", compilerConfiguration)
        ("PackageVersion", $"1.0.{timestamp}")
    ]
    
    // Manage global.json for net9 compiler
    manageGlobalJson compilerVersion true
    
    printfn "  Packing library with %s compiler..." compilerVersion
    let projectFile = Path.Combine(projectPath, "CompilerCompatLib.fsproj")
    let output = runCommand "dotnet" $"pack \"{projectFile}\" -c {compilerConfiguration} -o \"{outputDir}\"" projectPath envVars
    
    // Clean up global.json after pack
    manageGlobalJson compilerVersion false
    
    output |> ignore

let buildApp projectPath compilerVersion =
    let useLocal = (compilerVersion = "local")
    let envVars = [
        ("LoadLocalFSharpBuild", if useLocal then "True" else "False")
        ("LocalFSharpCompilerConfiguration", compilerConfiguration)
    ]
    
    // Manage global.json for net9 compiler
    manageGlobalJson compilerVersion true
    
    printfn "  Building app with %s compiler..." compilerVersion
    let projectFile = Path.Combine(projectPath, "CompilerCompatApp.fsproj")
    
    // First restore with force to get fresh NuGet packages
    runCommand "dotnet" $"restore \"{projectFile}\" --force --no-cache" projectPath envVars |> ignore
    
    // Then build
    runCommand "dotnet" $"build \"{projectFile}\" -c {compilerConfiguration} --no-restore" projectPath envVars
    |> ignore
    
    // Clean up global.json after build
    manageGlobalJson compilerVersion false

let runApp() =
    let appDll = Path.Combine(appProjectPath, "bin", compilerConfiguration, "net8.0", "CompilerCompatApp.dll")
    printfn "  Running app..."
    // Use --roll-forward Major to allow running net8.0 app on net10.0 runtime
    let envVars = [
        ("DOTNET_ROLL_FORWARD", "Major")
    ]
    let output = runCommand "dotnet" $"\"{appDll}\"" appProjectPath envVars
    output

let extractValue (sectionHeader: string) (searchPattern: string) (lines: string array) =
    lines 
    |> Array.tryFindIndex (fun (l: string) -> l.StartsWith(sectionHeader))
    |> Option.bind (fun startIdx ->
        lines
        |> Array.skip (startIdx + 1)
        |> Array.take (min 10 (lines.Length - startIdx - 1))
        |> Array.tryFind (fun (l: string) -> l.Contains(searchPattern)))

let verifyOutput libCompilerVersion appCompilerVersion (output: string) =
    let lines = output.Split('\n') |> Array.map (fun (s: string) -> s.Trim())
    
    // Check for success message
    if not (Array.exists (fun (l: string) -> l.Contains("SUCCESS: All compiler compatibility tests passed")) lines) then
        failwith "App did not report success"
    
    // Extract build info
    let getBool section pattern = 
        extractValue section pattern lines 
        |> Option.map (fun l -> l.Contains("true"))
        |> Option.defaultValue false
    
    let libIsLocal = getBool "Library Build Info:" "Is Local Build:"
    let appIsLocal = getBool "Application Build Info:" "Is Local Build:"
    
    // Verify - both "latest" and "net9" should result in isLocalBuild=false
    let expectedLibIsLocal = (libCompilerVersion = "local")
    let expectedAppIsLocal = (appCompilerVersion = "local")
    
    if libIsLocal <> expectedLibIsLocal then
        failwith $"Library: expected isLocalBuild={expectedLibIsLocal} for '{libCompilerVersion}', but got {libIsLocal}"
    
    if appIsLocal <> expectedAppIsLocal then
        failwith $"App: expected isLocalBuild={expectedAppIsLocal} for '{appCompilerVersion}', but got {appIsLocal}"
    
    printfn "  ✓ Build info verification passed"

// Main test execution
let runTest (libCompiler, appCompiler, description) =
    printfn "\n=== Test: %s ===" description
    printfn "Library compiler: %s, App compiler: %s" libCompiler appCompiler
    
    try
        // Clean previous builds
        cleanBinObjDirectories libProjectPath
        cleanBinObjDirectories appProjectPath
        
        // Create local NuGet directory
        let localNuGetDir = Path.Combine(projectsPath, "local-nuget-packages")
        cleanDirectory localNuGetDir
        Directory.CreateDirectory(localNuGetDir) |> ignore
        
        // Create nuget.config for app
        let nugetConfig = Path.Combine(appProjectPath, "nuget.config")
        let nugetConfigContent = $"""<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-packages" value="{localNuGetDir}" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>"""
        File.WriteAllText(nugetConfig, nugetConfigContent)
        
        // Pack library
        packProject libProjectPath libCompiler localNuGetDir
        
        // Build and run app
        buildApp appProjectPath appCompiler
        let output = runApp()
        
        // Verify
        verifyOutput libCompiler appCompiler output
        
        printfn "✓ PASSED: %s" description
        true
    with ex ->
        printfn "✗ FAILED: %s" description
        printfn "Error: %s" ex.Message
        false

// Run all tests
printfn "F# Compiler Compatibility Test Suite"
printfn "======================================"

let results = testScenarios |> List.map runTest

let passed = results |> List.filter id |> List.length
let total = results |> List.length

printfn "\n======================================"
printfn "Results: %d/%d tests passed" passed total

if passed = total then
    printfn "All tests PASSED ✓"
    exit 0
else
    printfn "Some tests FAILED ✗"
    exit 1
