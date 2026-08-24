// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Compiler

module PreprocessorElif =

    [<Fact>]
    let ``elif basic branch selection - first branch taken`` () =
        Fsx
            """
#if DEFINED
let x = 1
#elif NOTDEFINED
let x = 2
#else
let x = 3
#endif
printfn "%d" x
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif basic branch selection - elif branch taken`` () =
        Fsx
            """
#if NOTDEFINED
let x = 1
#elif DEFINED
let x = 2
#else
let x = 3
#endif
printfn "%d" x
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif basic branch selection - else branch taken`` () =
        Fsx
            """
#if NOTDEFINED1
let x = 1
#elif NOTDEFINED2
let x = 2
#else
let x = 3
#endif
printfn "%d" x
            """
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif multiple elif chain`` () =
        Fsx
            """
#if A
let x = 1
#elif B
let x = 2
#elif C
let x = 3
#elif D
let x = 4
#else
let x = 5
#endif
printfn "%d" x
            """
        |> withDefines [ "C" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif without else`` () =
        Fsx
            """
#if NOTDEFINED
let x = 1
#elif DEFINED
let x = 2
#endif
printfn "%d" x
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif after else is an error`` () =
        Fsx
            """
#if DEFINED
let x = 1
#else
let x = 2
#elif NOTDEFINED
let x = 3
#endif
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldFail
        |> ignore

    [<Fact>]
    let ``elif with no matching if is an error`` () =
        Fsx
            """
#elif DEFINED
let x = 1
#endif
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldFail
        |> ignore

    [<Fact>]
    let ``elif requires langversion 11 or preview`` () =
        Fsx
            """
#if NOTDEFINED
let x = 1
#elif DEFINED
let x = 2
#endif
printfn "%d" x
            """
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "10"
        |> typecheck
        |> shouldFail
        |> ignore

    [<Fact>]
    let ``elif nested in if-elif-endif`` () =
        Fsx
            """
#if OUTER
  #if NOTDEFINED
  let x = 1
  #elif INNER
  let x = 2
  #endif
#else
  let x = 3
#endif
printfn "%d" x
            """
        |> withDefines [ "OUTER"; "INNER" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``elif first true branch wins`` () =
        Fsx
            """
#if A
let x = 1
#elif B
let x = 2
#elif C
let x = 3
#endif
printfn "%d" x
            """
        |> withDefines [ "A"; "B"; "C" ]
        |> withLangVersion "preview"
        |> typecheck
        |> shouldSucceed
        |> ignore
