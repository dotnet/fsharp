module Conformance.Spreads.Records

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Literal>]
let SupportedLangVersion = "preview"

let inlineLib =
    FsFromPath (Path.Combine (__SOURCE_DIRECTORY__, "SpreadInlineLib.fs"))
    |> withLangVersion SupportedLangVersion
    |> withName "SpreadInlineLib"
    |> asLibrary

let verifyCompileAndRun compilation =
    compilation
    |> asExe
    |> withLangVersion SupportedLangVersion
    |> compileAndRun

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordSpreads.fsx"|])>]
let ``RecordSpreads_fsx`` compilation =
    compilation
    |> withReferences [inlineLib]
    |> ignoreWarnings
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withDiagnostics [
        Information 3906, Line 13, Col 64, Line 13, Col 72, "Explicit field 'D' shadows a field with the same name from an earlier spread."
        Information 3906, Line 29, Col 58, Line 29, Col 63, "Explicit field 'Y' shadows a field with the same name from an earlier spread."
        Information 3906, Line 33, Col 50, Line 33, Col 55, "Explicit field 'A' shadows a field with the same name from an earlier spread."
        Information 3906, Line 35, Col 57, Line 35, Col 62, "Explicit field 'A' shadows a field with the same name from an earlier spread."
        Information 3906, Line 36, Col 55, Line 36, Col 60, "Explicit field 'A' shadows a field with the same name from an earlier spread."
        Information 3906, Line 58, Col 65, Line 58, Col 71, "Explicit field 'M' shadows a field with the same name from an earlier spread."
        Information 3906, Line 62, Col 60, Line 62, Col 65, "Explicit field 'A' shadows a field with the same name from an earlier spread."
        Information 3906, Line 70, Col 32, Line 70, Col 37, "Explicit field 'B' shadows a field with the same name from an earlier spread."
    ]
