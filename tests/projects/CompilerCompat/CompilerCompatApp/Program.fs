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
            
            // Test well-known attribute types
            let sealedObj = CompilerCompatLib.Library.SealedType()
            printfn "Sealed: %d" sealedObj.Value

            let sr = { CompilerCompatLib.Library.StructRecord.X = 1; CompilerCompatLib.Library.StructRecord.Y = 2.0 }
            printfn "Struct: %d, %f" sr.X sr.Y

            let u = CompilerCompatLib.Library.NoHelpersUnion.Case1
            printfn "Union: %A" u

            let q = CompilerCompatLib.Library.QualifiedEnum.A
            printfn "Enum: %A" q

            printfn "Literal: %d" CompilerCompatLib.Library.LiteralValue
            printfn "Reflected: %d" (CompilerCompatLib.Library.reflectedFunction 1)

            // Test literal attribute arg cross-compiler compatibility
            let attrObj = CompilerCompatLib.Library.TypeWithLiteralAttrArg()
            printfn "LiteralAttr: %s" (attrObj.GetValue())
            if attrObj.GetValue() <> CompilerCompatLib.Library.LiteralAttrArg then
                printfn "ERROR: Literal attr arg value mismatch"
                1
            elif processed <> "Processed: X=123, Y=test" then
                printfn "ERROR: Processed result doesn't match expected"
                1
            else
                // FS-1073 record-constructor cross-compiler checks.
                // 'makeRecordCtorPoint' is an inline function in the library; if the library was built with
                // the local compiler it was authored with the new positional-ctor syntax, and this app
                // (possibly an older compiler) must be able to inline its pickled body correctly.
                let viaInline = Library.makeRecordCtorPoint 7 9
                // Direct construction of a packaged record. When this app is built with the local compiler
                // it uses the new positional constructor on a record defined in another assembly.
#if RECORD_CTOR_FEATURE
                let viaCtor = Library.RecordCtorPoint(3, 4)
#else
                let viaCtor = { Library.RecordCtorPoint.A = 3; Library.RecordCtorPoint.B = 4 }
#endif
                if viaInline.A <> 7 || viaInline.B <> 9 then
                    printfn "ERROR: inline record constructor result mismatch"
                    1
                elif viaCtor.A <> 3 || viaCtor.B <> 4 then
                    printfn "ERROR: record constructor result mismatch"
                    1
                else
                    printfn "RecordCtor: inline=(%d,%d) direct=(%d,%d)" viaInline.A viaInline.B viaCtor.A viaCtor.B
                    printfn "SUCCESS: All compiler compatibility tests passed"
                    0
                
    with ex ->
        printfn "ERROR: Exception occurred: %s" ex.Message
        1