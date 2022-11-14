module FSharp.Compiler.ComponentTests.Signatures.SigGenerationRoundTripTests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO

let testCasesDir = Path.Combine(__SOURCE_DIRECTORY__,"TestCasesForGenerationRoundTrip")
let allTestCases = 
    Directory.EnumerateFiles(testCasesDir) 
    |> Seq.toArray 
    |> Array.map Path.GetFileName
    |> Array.map (fun f -> [|f :> obj|])

[<Theory>]
[<MemberData(nameof(allTestCases))>]
let ``Generate and compile`` implFileName =    
    let implContents = File.ReadAllText (Path.Combine(testCasesDir,implFileName))

    let generatedSignature = 
        Fs implContents     
        |> withDefines ["TESTS_AS_APP"]
        |> printSignatures   

    Fsi generatedSignature    
    |> withAdditionalSourceFile (FsSource implContents)
    |> withDefines ["TESTS_AS_APP"]
    |> ignoreWarnings
    |> asExe
    |> compile
    |> shouldSucceed

