open CompilerCompatLib
open System

[<EntryPoint>]
let main _argv =
    try
        // Test basic anonymous record functionality
        let record = Library.getAnonymousRecord()
        printfn "Basic record: X=%d, Y=%s" record.X record.Y
        
        // Verify expected values
        if record.X <> 42 || record.Y <> "hello" then
            printfn "ERROR: Basic record values don't match expected"
            1
        else
            printfn "✓ Basic record test passed"
            
            // Test complex anonymous record functionality
            let complex = Library.getComplexAnonymousRecord()
            printfn "Complex record: Simple.A=%d, Simple.B=%s" complex.Simple.A complex.Simple.B
            printfn "Complex record: List has %d items" complex.List.Length
            printfn "Complex record: Tuple=(%d, Value=%f)" (fst complex.Tuple) (snd complex.Tuple).Value
            
            // Test function that takes anonymous record
            let processed = Library.processAnonymousRecord({| X = 123; Y = "test" |})
            printfn "Processed result: %s" processed
            
            if processed = "Processed: X=123, Y=test" then
                printfn "✓ All compiler compatibility tests passed"
                0
            else
                printfn "ERROR: Processed result doesn't match expected"
                1
                
    with ex ->
        printfn "ERROR: Exception occurred: %s" ex.Message
        1