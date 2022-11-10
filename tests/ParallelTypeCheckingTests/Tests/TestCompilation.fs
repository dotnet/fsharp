﻿module ParallelTypeCheckingTests.TestCompilation

open FSharp.Test
open NUnit.Framework
open ParallelTypeCheckingTests.TestUtils

type OutputType =
    | Exe
    | Library

type FProject =
    {
        Files: (string * string) list
        OutputType: CompileOutput
    }

    override this.ToString() =
        let names =
            this.Files
            |> List.map (fun (name, _) -> name)
            |> fun names -> System.String.Join(", ", names)

        $"{this.OutputType} - {names}"

    static member Make outputType files =
        {
            Files = files
            OutputType = outputType
        }

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
        |> FProject.Make CompileOutput.Exe

    let diamondBroken1 =
        [
            "A.fs",
            """
module A
let a = 1
"""
            "B.fsi",
            """
module B
open A
val b : int
"""
            "B.fs",
            """
module B
let b = 1 + A.a
"""
            "C.fs",
            """
namespace N.M.K
module Y2 = let y = 4
"""
            "D.fs",
            """
namespace N.M.K
module Y3 = let y = 5
"""
            "E.fs",
            """
namespace N.M.K
module Y4 =
    let y = 6
"""
        ]
        |> FProject.Make CompileOutput.Library

    let fsFsi =
        [
            "B.fsi",
            """
module B
val b : int
"""
            "B.fs",
            """
module B
let b = 1
"""
        ]
        |> FProject.Make CompileOutput.Library

    let emptyNamespace =
        [
            "A.fs",
            """
namespace A
"""
            "B.fs",
            """
module B
open A
"""
        ]
        |> FProject.Make CompileOutput.Library

    let all = [ encodeDecodeSimple; diamondBroken1; fsFsi; emptyNamespace ]

type Case =
    {
        Method: Method
        Project: FProject
    }

    override this.ToString() = $"{this.Method} - {this.Project}"

let compile (x: Case) =
    use _ =
        FSharp.Compiler.Diagnostics.Activity.start "Compile codebase" [ "method", x.Method.ToString() ]

    setupCompilationMethod x.Method

    makeCompilationUnit x.Project.Files
    |> Compiler.withOutputType x.Project.OutputType
    |> Compiler.compile
    |> Compiler.Assertions.shouldSucceed
    |> ignore

let codebases = Codebases.all

[<TestCaseSource(nameof (codebases))>]
let ``Compile graph-based`` (project: FProject) =
    compile
        {
            Method = Method.Graph
            Project = project
        }

/// <summary> Compile a project using the original fully sequential type-checking. <br/>
/// Useful as a sanity check </summary>
[<TestCaseSource(nameof (codebases))>]
let ``Compile sequential`` (project: FProject) =
    compile
        {
            Method = Method.Sequential
            Project = project
        }
