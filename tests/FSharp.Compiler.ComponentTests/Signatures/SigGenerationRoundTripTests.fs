module Signatures.SigGenerationRoundTripTests

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
        |> withLangVersion80
        |> withDefines ["TESTS_AS_APP";"COMPILED"]
        |> printSignatures

    Fsi generatedSignature
    |> withAdditionalSourceFile (FsSource implContents)
    |> withLangVersion80
    |> withDefines ["TESTS_AS_APP";"COMPILED"]
    |> ignoreWarnings
    |> withOptions [ "--warnaserror:64" ]
    |> asExe
    |> compile
    |> shouldSucceed
