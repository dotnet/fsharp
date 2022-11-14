﻿module FSharp.Compiler.ComponentTests.Signatures.SigGenerationRoundTripTests

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

