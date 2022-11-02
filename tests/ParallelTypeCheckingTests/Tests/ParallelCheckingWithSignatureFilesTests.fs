module FSharp.Compiler.ComponentTests.TypeChecks.ParallelCheckingWithSignatureFilesTests

open NUnit.Framework
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open ParallelTypeCheckingTests.Tests.Utils


let methods =
    [
        Method.Sequential
        Method.ParallelFs
        Method.Nojaf
        Method.Graph
    ]

[<TestCaseSource(nameof(methods))>]
let ``Parallel type checking when signature files are available`` (method : Method) =
    let files =
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
    
    makeCompilationUnit files
    |> asExe
    |> setupCompilationMethod method
    |> compile
    |> shouldSucceed
    ()
