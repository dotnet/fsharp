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

            // ---- RFC FS-1043 breaking change compat tests ----

            // T4a: inline addOne via concrete wrapper
            let addOneResult = Library.addOneConcrete 41
            if addOneResult <> 42 then
                printfn "ERROR: addOneConcrete 41 = %d, expected 42" addOneResult
                1
            else
                printfn "SUCCESS: addOneConcrete test passed"

                // T4b: inline negate via concrete wrapper
                let negateResult = Library.negateConcrete 7
                if negateResult <> -7 then
                    printfn "ERROR: negateConcrete 7 = %d, expected -7" negateResult
                    1
                else
                    printfn "SUCCESS: negateConcrete test passed"

                    // T4c: pass concrete wrapper as function value
                    let applyResult = Library.applyToInt Library.addOneConcrete 41
                    if applyResult <> 42 then
                        printfn "ERROR: applyToInt addOneConcrete 41 = %d, expected 42" applyResult
                        1
                    else
                        printfn "SUCCESS: applyToInt test passed"

                        // T5: custom type operator via concrete wrapper
                        let n1 = { Library.V = 3 }
                        let n2 = { Library.V = 4 }
                        let numResult = Library.addNumsConcrete n1 n2
                        if numResult.V <> 7 then
                            printfn "ERROR: addNumsConcrete {V=3} {V=4} = {V=%d}, expected {V=7}" numResult.V
                            1
                        else
                            printfn "SUCCESS: custom type operator compat test passed"
                            printfn "SUCCESS: All compiler compatibility tests passed"
                            0
                
    with ex ->
        printfn "ERROR: Exception occurred: %s" ex.Message
        1