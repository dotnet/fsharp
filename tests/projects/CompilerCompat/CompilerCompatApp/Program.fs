open CompilerCompatLib
open CompilerCompatApp
open System

[<EntryPoint>]
let main _argv =
    try
        // Helper to get the actual compiler path (prefer dotnetFscCompilerPath when using local build)
        let getActualCompilerPath (dotnetPath: string) (fallbackPath: string) =
            if dotnetPath <> "N/A" && dotnetPath <> "" then dotnetPath else fallbackPath
        
        // Print detailed build information to verify which compiler was used
        printfn "=== BUILD VERIFICATION ==="
        printfn "Library Build Info:"
        printfn "  SDK Version: %s" LibBuildInfo.sdkVersion
        printfn "  F# Compiler Path: %s" (getActualCompilerPath LibBuildInfo.dotnetFscCompilerPath LibBuildInfo.fsharpCompilerPath)
        printfn "  Is Local Build: %b" LibBuildInfo.isLocalBuild
        printfn "Application Build Info:"
        printfn "  SDK Version: %s" AppBuildInfo.sdkVersion
        printfn "  F# Compiler Path: %s" (getActualCompilerPath AppBuildInfo.dotnetFscCompilerPath AppBuildInfo.fsharpCompilerPath)
        printfn "  Is Local Build: %b" AppBuildInfo.isLocalBuild
        printfn "=========================="
        
        // Test basic anonymous record functionality
        let record = Library.getAnonymousRecord()
        printfn "Basic record: X=%d, Y=%s" record.X record.Y
        
        // Verify expected values
        if record.X <> 42 || record.Y <> "hello" then
            printfn "ERROR: Basic record values don't match expected"
            1
        else
            printfn "SUCCESS: Basic record test passed"
            
            // Test complex anonymous record functionality
            let complex = Library.getComplexAnonymousRecord()
            printfn "Complex record: Simple.A=%d, Simple.B=%s" complex.Simple.A complex.Simple.B
            printfn "Complex record: List has %d items" complex.List.Length
            printfn "Complex record: Tuple=(%d, Value=%f)" (fst complex.Tuple) (snd complex.Tuple).Value
            
            // Test function that takes anonymous record
            let processed = Library.processAnonymousRecord({| X = 123; Y = "test" |})
            printfn "Processed result: %s" processed
            
            if processed = "Processed: X=123, Y=test" then
                printfn "SUCCESS: All compiler compatibility tests passed"
                0
            else
                printfn "ERROR: Processed result doesn't match expected"
                1
                
    with ex ->
        printfn "ERROR: Exception occurred: %s" ex.Message
        1