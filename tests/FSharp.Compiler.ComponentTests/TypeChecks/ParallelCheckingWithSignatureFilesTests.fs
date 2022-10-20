namespace FSharp.Compiler.ComponentTests.TypeChecks
module ParallelCheckingWithSignatureFilesTests = 
    
    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler

    [<Fact>]
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

        let encodeFs =
            SourceCodeFileKind.Create(
                "Encode.fs",
                """
    module Encode

    let encode (v: obj) : string = failwith "todo"
    """
            )

        let decodeFsi =
            SourceCodeFileKind.Create(
                "Decode.fsi",
                """
    module Decode

    val decode: string -> obj
    """
            )

        let decodeFs =
            SourceCodeFileKind.Create(
                "Decode.fs",
                """
    module Decode

    let decode (v: string) : obj = failwith "todo"
    """
            )

        let programFs = SourceCodeFileKind.Create("Program.fs", "printfn \"Hello from F#\"")

        encodeFsi
        |> withAdditionalSourceFiles [ encodeFs; decodeFsi; decodeFs; programFs ]
        |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
        |> asExe
        |> compile
        |> shouldSucceed
