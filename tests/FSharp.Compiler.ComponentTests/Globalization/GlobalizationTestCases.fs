// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Globalization

open Xunit
open FSharp.Test.Compiler
open System.IO

module ``Globalization Test Cases`` =

    [<Fact>]
    let ``Globalization - Arabic.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Arabic.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - gb18030.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "gb18030.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - HexCharEncode.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "HexCharEncode.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Hindi.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Hindi.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Mongolian.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Mongolian.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]


    [<Fact>]
    let ``Globalization - RightToLeft.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "RightToLeft.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Surrogates.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Surrogates.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Tamil.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Tamil.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Tibetan.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Tibetan.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - TurkishI.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "TurkishI.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Uighur.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Uighur.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - utf16.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "utf16.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - utf8.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "utf8.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Fact>]
    let ``Globalization - Yi.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Yi.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]
