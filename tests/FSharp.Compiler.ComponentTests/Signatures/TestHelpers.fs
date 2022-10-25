module FSharp.Compiler.ComponentTests.Signatures.TestHelpers

open System
open FsUnit

let prependNewline v = String.Concat("\n", v)

let equal x =
    let x =
        match box x with
        | :? String as s -> s.Replace("\r\n", "\n") |> box
        | x -> x

    equal x