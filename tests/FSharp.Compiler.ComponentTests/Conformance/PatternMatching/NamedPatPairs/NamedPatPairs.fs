// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NamedPatPairs =
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs01.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs01.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 8, Col 18, Line 8, Col 19, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 18, Line 9, Col 19, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 25, Line 9, Col 26, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 19, Col 18, Line 19, Col 19, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 20, Col 18, Line 20, Col 19, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 21, Col 25, Line 21, Col 26, "Feature 'Allow comma as separator for pattern matching on multiple named discriminated unions fields' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]