module FSharp.Compiler.ComponentTests.Signatures.TestHelpers

open System
open FsUnit
open FSharp.Test.Compiler

let prependNewline v = String.Concat("\n", v)

let equal x =
    let x =
        match box x with
        | :? String as s -> s.Replace("\r\n", "\n") |> box
        | x -> x

    equal x

let assertSingleSignatureBinding implementation signature =
    FSharp $"module A\n\n{implementation}"
    |> printSignatures
    |> should equal $"\nmodule A\n\n{signature}"
