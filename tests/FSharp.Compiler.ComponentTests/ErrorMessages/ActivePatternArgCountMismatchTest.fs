// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Active Pattern argument count mismatch test`` =

    [<Fact>]
    let ``test``() =
        FSharp """
let (|IsEven|_|) x = x % 2 = 0
match 1 with
| IsEven xxx -> ()
| _ -> printfn "Odd!"

let (|Ignore|) x = ()
match 1 with
| Ignore () () -> printfn "Ignored!"
| _ -> ()

let (|Equals|_|) z y x = x = y
match 1 with
| Equals "" 2 xxx -> ()
| _ -> printfn "Not Equal"

let (|A|) a b c d = d
match 1 with
| A -> ()
| _ -> ()
        """ |> withLangVersionPreview
            |> typecheck
            |> shouldSucceed
            |> withDiagnostics [
        (Error 3868, Line 4, Col 3, Line 4, Col 13, "This active pattern does not expect any arguments, i.e., it should be used like 'IsEven' instead of 'IsEven x'.")
        (Error 3868, Line 9, Col 3, Line 9, Col 15, "This active pattern expects exactly one pattern argument, e.g., 'Ignore pat'.")
        (Error 3868, Line 14, Col 3, Line 14, Col 18, "This active pattern expects 2 expression argument(s), e.g., 'Equals e1 e2'.")
        (Error 3868, Line 19, Col 3, Line 19, Col 4, "This active pattern expects 3 expression argument(s) and a pattern argument, e.g., 'A e1 e2 e3 pat'.")
    ]
