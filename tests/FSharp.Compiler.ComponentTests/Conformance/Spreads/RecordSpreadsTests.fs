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
    |> verifyCompileAndRun
    |> shouldSucceed
