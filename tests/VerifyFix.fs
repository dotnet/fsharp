open System
open System.IO
open System.Reflection.PortableExecutable

let verify() =
    let fscPath = "artifacts/bin/fsc/Release/net10.0/fsc.dll"
    let keyPath = "tests/fsharp/keys/sha512full.snk"
    let sourceFile = "temp_test.fs"
    let outFile = "temp_test.dll"

    // 1. Create a dummy F# file
    File.WriteAllText(sourceFile, "module Temp\nlet x = 1")

    // 2. Compile using your newly built fsc.dll
    printfn "Compiling with %s..." fscPath
    let proc = System.Diagnostics.Process.Start("dotnet", sprintf "%s %s --keyfile:%s -o:%s" fscPath sourceFile keyPath outFile)
    proc.WaitForExit()

    if proc.ExitCode <> 0 then 
        failwith "Compilation failed! Check if fsc.dll path is correct."

    // 3. Read PE Headers to check Signature Size
    printfn "Checking Signature size in %s..." outFile
    using (new FileStream(outFile, FileMode.Open, FileAccess.Read)) (fun stream ->
        let reader = new PEReader(stream)
        let metadata = reader.PEHeaders.CorHeader.StrongNameSignatureDirectory
        let size = metadata.Size
        
        printfn "Found StrongNameSignature size: %d bytes" size
        
        if size = 544 then
            printfn "SUCCESS: Signature size is 544 bytes! The fix is working. ✅"
        else
            printfn "FAILURE: Signature size is %d. Expected 544. ❌" size
    )

verify()