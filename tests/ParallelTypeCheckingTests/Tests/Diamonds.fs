module FSharp.Compiler.ComponentTests.TypeChecks.Diamonds

open NUnit.Framework
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Test>]
let ``Parallel type checking when signature files are available`` () =
    // File structure:
    //   Encode.fsi
    //   Encode.fs
    //   Decode.fsi
    //   Decode.fs
    //   Program.fs

    let encodeFsi =
        Fsi
            """
module Encode

val encode: obj -> string
"""
    let files =
        [
            "Encode.fs",
            """
module Encode

let encode (v: obj) : string = failwith "todo"
"""

            "Decode.fsi",
            """
module Decode

val decode: string -> obj
"""

            "Decode.fs",
            """
module Decode
let x : int = ""
let decode (v: string) : obj = failwith "todo"
"""

            "Program.fs", "printfn \"Hello from F#\""
        ]
        |> List.map (fun (name, code) -> SourceCodeFileKind.Create(name, code))

    encodeFsi
    |> withAdditionalSourceFiles files
    |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
    |> asExe
    |> compile
    |> shouldSucceed
