module ParallelTypeCheckingTests.TestCompilation

open FSharp.Test
open FSharp.Test.Compiler
open NUnit.Framework
open ParallelTypeCheckingTests.TestUtils

type FProject =
    {
        Files: (string * string) list
        OutputType: CompileOutput
    }

    /// Used for naming test cases in the test explorer
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
module A1 =
module A1 =
    let x = 3
    type X = X of int
    let y = X 5
    let foo () = 5
    let inline bar () = 7
    module B2 =
        let a = "lalala"
"""
            "B.fs",
            """
module B
open A
let z = A.A1.x
let y = A.A1.y
let g = A.A1.x + 2
let h = A.A1.bar()
"""
        ]
        |> FProject.Make CompileOutput.Library

    let dependentSignatures =
        [
            "A.fsi",
            """
module A

type AType = class end
"""
            "A.fs",
            """
module A

type AType = class end
"""
            "B.fsi",
            """
module B

open A

val b: AType -> unit
"""
            "B.fs",
            """
module B

open A

let b (a:AType) = ()
"""
            "C.fsi",
            """
module C

type CType = class end
"""
            "C.fs",
            """
module C

type CType = class end
"""
            "D.fsi",
            """
module D

open A
open C

val d: CType -> unit 
"""
            "D.fs",
            """
module D

open A
open B
open C

let d (c: CType) =
    let a : AType = failwith "todo"
    b a
"""
        ]
        |> FProject.Make CompileOutput.Library

    let all =
        [
            encodeDecodeSimple
            diamondBroken1
            fsFsi
            emptyNamespace
            dependentSignatures
        ]

type Case =
    {
        Method: Method
        Project: FProject
    }

    override this.ToString() = $"{this.Method} - {this.Project}"

let methodOptions (method: Method) =
    match method with
    | Method.Sequential -> []
    | Method.ParallelCheckingOfBackedImplFiles -> [ "--test:ParallelCheckingWithSignatureFilesOn" ]
    | Method.Graph -> [ "--test:GraphBasedChecking" ]

let withMethod (method: Method) (cu: CompilationUnit) : CompilationUnit =
    match cu with
    | CompilationUnit.FS cs ->
        FS
            { cs with
                Options = cs.Options @ (methodOptions method) @ ["--optimize-"]
            }
    | cu -> cu

let compileAValidProject (x: Case) =
    use _ =
        global.FSharp.Compiler.Diagnostics.Activity.start "Compile codebase" [ "method", x.Method.ToString() ]

    printfn $"Method: {x.Method}"

    makeCompilationUnit x.Project.Files
    |> Compiler.withOutputType x.Project.OutputType
    |> withMethod x.Method
    |> Compiler.compile
    |> Compiler.Assertions.shouldSucceed
    |> ignore

let codebases = Codebases.all

[<TestCaseSource(nameof codebases)>]
let ``Compile a valid project using graph-based type-checking`` (project: FProject) =
    compileAValidProject
        {
            Method = Method.Graph
            Project = project
        }

/// <summary> Compile a project using the original fully sequential type-checking. <br/>
/// Useful as a sanity check </summary>
[<TestCaseSource(nameof codebases)>]
let ``Compile a valid project using sequential type-checking`` (project: FProject) =
    compileAValidProject
        {
            Method = Method.Sequential
            Project = project
        }
