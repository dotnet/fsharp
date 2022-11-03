module ParallelTypeCheckingTests.CompilationTests

open FSharp.Test
open NUnit.Framework
open FSharp.Test.Compiler
open ParallelTypeCheckingTests.Utils

type OutputType =
    | Exe
    | Library

module Codebases =
    let encodeDecodeSimple =
        [
            "Encode.fsi",
            """
module Encode

val encode: obj -> string
"""

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

let decode (v: string) : obj = failwith "todo"
"""

            "Program.fs", "printfn \"Hello from F#\""
        ]
    
    let diamondBroken1 =
        [
            "A.fs", """
module A
let a = 1
"""
            "B.fsi", """
module B
open A
val b : int
"""
            "B.fs", """
module B
let b = 1 + A.a
"""
            "C.fs", """
namespace N.M.K
module Y2 = let y = 4
"""
            "D.fs", """
namespace N.M.K
module Y3 = let y = 5
"""
            "E.fs", """
namespace N.M.K
module Y4 =
    let y = 6
"""
        ]
    
    let all =
        [
            encodeDecodeSimple, CompileOutput.Exe
            diamondBroken1, CompileOutput.Library
        ]

type Case =
    {
        Files : (string * string) list
        OutputType : CompileOutput
        Method : Method
    }

let cases : Case list =
    methods
    |> List.allPairs Codebases.all
    |> List.map (fun ((a, t), b) -> {Files = a; OutputType = t; Method = b})

[<TestCaseSource(nameof(cases))>]
let ``Compile all codebase examples with all methods`` (x : Case) =
    makeCompilationUnit x.Files
    |> withOutputType x.OutputType
    |> setupCompilationMethod x.Method
    |> compile
    |> shouldSucceed
    |> ignore
