namespace FSharp.Compiler.ComponentTests.EmittedIL

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StaticLinking =

    let myRecordLibrary =
        FSharp """
module First
    type MyRecord =
        {
            A: string
            B: decimal
            C: int
            D: float
        }
    let getMyRecord () = { A = "Hello, World!"; B = 1.027m; C = 1028; D = 1.029 }
    """ |> withOptimize |> asLibrary

    let myRecordLibrary =
        FSharp """
module Second
    type Number = IntNumber of int | DoubleNumber of double

    let getMyIntNumber() = IntNumber 10

    let getMyDoubleNumber() = DoubleNumber 12.0
        """

    [<Fact>]
    let ``staticlinking_multiple_fs_libraries`` =
        let firstLibrary =
            FSharp """
module First
    type MyRecord =
        {
            A: string
            B: decimal
            C: int
            D: float
        }
    let getMyRecord () = { A = "Hello, World!"; B = 1.027m; C = 1028; D = 1.029 }
            """ |> withOptimize |> asLibrary

        let secondLibrary =
            FSharp """
module Second
    type Number = IntNumber of int | DoubleNumber of double

    let getMyIntNumber() = IntNumber 10

    let getMyDoubleNumber() = DoubleNumber 12.0
            """
            |> withOptimize
            |> asLibrary
        FSharp  """
                """
                |> asExe
                |> withOptions [ "--standalone" ]
                |> withStaticLink [ firstLibrary ]
                |> withStaticLink [ secondLibrary ]
                |> verifyCompileAndExecution
