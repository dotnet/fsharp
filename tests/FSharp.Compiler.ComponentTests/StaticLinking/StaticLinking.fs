namespace EmittedIL

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
        """
        |> withOptimize
        |> asLibrary

    let myDiscriminatedUnionLibrary =
        FSharp """
module Second
    type Number = IntNumber of int | DoubleNumber of double

    let getMyIntDU() = IntNumber 10

    let getMyDoubleDU() = DoubleNumber 12.0
            """
            |> withOptimize
            |> asLibrary

    [<Fact>]
    let ``staticlinking_multiple_fs_libraries`` () =
        let tripleQuote = "\"\"\""
        let expectedRecord = """{ A = "Hello, World!";  B = 1.027M;  C = 1028;  D = 1.029 }""".Replace("\n", ";")
        let expectedIntDU = """IntNumber 10""".Replace("\n", ";")
        let expectedDoubleDU = """DoubleNumber 12.0""".Replace("\n", ";")

        FSharp ("""open System
open First
open Second

let expectedRecord = $(expectedRecord)
let actualRecord = (sprintf "%A" (getMyRecord())).Replace("\r\n", "\n").Replace("\n", ";")
if expectedRecord <> actualRecord then
    raise (new Exception $"Text failed:{Environment.NewLine}Expected: '{expectedRecord}'{Environment.NewLine}Actual: '{actualRecord}'{Environment.NewLine}")

let expectedIntDU = $(expectedIntDU)
let actualIntDU = (sprintf "%A" (getMyIntDU())).Replace("\r\n", "\n").Replace("\n", ";")
if expectedIntDU <> actualIntDU then
    raise (new Exception $"Text failed:{Environment.NewLine}Expected: '{expectedIntDU}'{Environment.NewLine}Actual: '{actualIntDU}'{Environment.NewLine}")

let expectedDoubleDU = $(expectedDoubleDU)
let actualDoubleDU = (sprintf "%A" (getMyDoubleDU())).Replace("\r\n", "\n").Replace("\n", ";")
if expectedDoubleDU <> actualDoubleDU then
    raise (new Exception $"Text failed:{Environment.NewLine}Expected: '{expectedDoubleDU}'{Environment.NewLine}Actual: '{actualDoubleDU}'{Environment.NewLine}")
        """.Replace("$(expectedRecord)",  tripleQuote + expectedRecord + tripleQuote)
           .Replace("$(expectedIntDU)",   tripleQuote + expectedIntDU + tripleQuote)
           .Replace("$(expectedDoubleDU)", tripleQuote + expectedDoubleDU + tripleQuote))
        |> asExe
        |> withOptimize
        |> withReferences [ myRecordLibrary.WithStaticLink(true) ]
        |> withReferences [ myDiscriminatedUnionLibrary.WithStaticLink(true) ]
        |> compileExeAndRun
        |> shouldSucceed
