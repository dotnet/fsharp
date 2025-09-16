// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Simple test to verify VS installation discovery functionality
module Microsoft.VisualStudio.FSharp.TestHelpers.VSInstallDiscoveryTests

open System
open System.IO

/// Test basic discovery functionality
let testBasicDiscovery () =
    printfn "Testing VS installation discovery..."
    
    // Test the discovery process
    let result = VSInstallDiscovery.tryFindVSInstallation ()
    match result with
    | VSInstallDiscovery.Found (path, source) ->
        printfn $"✓ VS installation found at: {path}"
        printfn $"  Source: {source}"
        
        // Validate that the path looks reasonable
        if Directory.Exists(path) && Directory.Exists(Path.Combine(path, "IDE")) then
            printfn "  ✓ Path validation passed"
        else
            printfn "  ✗ Path validation failed - directory structure not as expected"
            
    | VSInstallDiscovery.NotFound reason ->
        printfn $"✗ VS installation not found: {reason}"
        printfn "  This is expected if VS is not installed or environment variables are not set"

    // Test the optional version
    let optPath = VSInstallDiscovery.tryGetVSInstallDir ()
    match optPath with
    | Some path -> printfn $"✓ tryGetVSInstallDir returned: {path}"
    | None -> printfn "✗ tryGetVSInstallDir returned None"

    // Test the logging version
    let logMessages = ResizeArray<string>()
    let logger msg = logMessages.Add(msg)
    let pathWithLogging = VSInstallDiscovery.getVSInstallDirWithLogging logger
    
    printfn "Logged messages:"
    for msg in logMessages do
        printfn $"  LOG: {msg}"
        
    printfn "✓ Basic discovery test completed"

/// Test environment variable scenarios  
let testEnvironmentVariables () =
    printfn "\nTesting environment variable scenarios..."
    
    // Save original values
    let originalVS170 = Environment.GetEnvironmentVariable("VS170COMNTOOLS")
    let originalVSAPP = Environment.GetEnvironmentVariable("VSAPPIDDIR")
    let originalFSHARP = Environment.GetEnvironmentVariable("FSHARP_VS_INSTALL_DIR")
    
    try
        // Test explicit override (highest priority)
        Environment.SetEnvironmentVariable("FSHARP_VS_INSTALL_DIR", @"C:\TestPath")
        Environment.SetEnvironmentVariable("VS170COMNTOOLS", @"C:\VS\Common7\Tools\")
        
        let result = VSInstallDiscovery.tryFindVSInstallation ()
        match result with
        | VSInstallDiscovery.Found (path, source) when source.Contains("FSHARP_VS_INSTALL_DIR") ->
            printfn "✓ FSHARP_VS_INSTALL_DIR override works correctly"
        | _ ->
            printfn "✗ FSHARP_VS_INSTALL_DIR override not working as expected"
        
        // Clear explicit override to test VS170COMNTOOLS
        Environment.SetEnvironmentVariable("FSHARP_VS_INSTALL_DIR", null)
        
        printfn "✓ Environment variable scenario tests completed"
        
    finally
        // Restore original values
        Environment.SetEnvironmentVariable("VS170COMNTOOLS", originalVS170)
        Environment.SetEnvironmentVariable("VSAPPIDDIR", originalVSAPP)
        Environment.SetEnvironmentVariable("FSHARP_VS_INSTALL_DIR", originalFSHARP)

/// Run all tests
let runTests () =
    printfn "=== VS Installation Discovery Tests ==="
    testBasicDiscovery ()
    testEnvironmentVariables ()
    printfn "=== Tests completed ==="