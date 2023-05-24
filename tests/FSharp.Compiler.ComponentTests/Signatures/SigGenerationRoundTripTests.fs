module FSharp.Compiler.ComponentTests.Signatures.SigGenerationRoundTripTests

open Xunit
open FSharp.Test.Compiler
open System.IO

let testCasesDir = Path.Combine(__SOURCE_DIRECTORY__,"TestCasesForGenerationRoundTrip")
let commonTestCases = 
    Directory.EnumerateFiles(testCasesDir) 
    |> Seq.toArray 
    |> Array.map Path.GetFileName
    |> Array.map (fun f -> [|f :> obj|])

let private generateAndCompileAux testCasesDir implFileName =
    let implContents = File.ReadAllText (Path.Combine(testCasesDir,implFileName))

    let generatedSignature = 
        Fs implContents     
        |> withLangVersionPreview
        |> withDefines ["TESTS_AS_APP";"COMPILED"]
        |> printSignatures   

    Fsi generatedSignature    
    |> withAdditionalSourceFile (FsSource implContents)
    |> withLangVersionPreview
    |> withDefines ["TESTS_AS_APP";"COMPILED"]
    |> ignoreWarnings
    |> asExe
    |> compile
    |> shouldSucceed

[<Theory>]
[<MemberData(nameof(commonTestCases))>]
let ``Generate and compile`` implFileName =
    generateAndCompileAux testCasesDir implFileName
    
#if NETCOREAPP
let netcoreOnlyTestDir =  Path.Combine(__SOURCE_DIRECTORY__, "TestCasesForGenerationRoundTrip", "netcoreonly")
let netcoreOnlyTestCases = 
    Directory.EnumerateFiles(netcoreOnlyTestDir) 
    |> Seq.toArray 
    |> Array.map Path.GetFileName
    |> Array.map (fun f -> [|f :> obj|])

[<Theory>]
[<MemberData(nameof(netcoreOnlyTestCases))>]
let ``Generate and compile netcore`` implFileName =
    generateAndCompileAux netcoreOnlyTestDir implFileName
#endif


