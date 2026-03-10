// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ComponentTests.Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for parsing errors at end of file (incomplete constructs).
/// Migrated from tests/fsharpqa/Source/Diagnostics/ParsingAtEOF/
module ParsingAtEOF =

    // while_cond01.fs - Missing 'do' in 'while' expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"while_cond01.fs"|])>]
    let ``while_cond01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3122
        |> ignore

    // du_with01.fs - Unexpected 'with' at end of DU definition
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"du_with01.fs"|])>]
    let ``du_with01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0058
        |> ignore

    // for_in01.fs - Missing 'do' in 'for' expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"for_in01.fs"|])>]
    let ``for_in01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3123
        |> ignore

    // for_in_range01.fs - Missing 'do' in 'for' expression with range
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"for_in_range01.fs"|])>]
    let ``for_in_range01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3123
        |> ignore

    // if01.fs - Incomplete conditional
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"if01.fs"|])>]
    let ``if01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0589
        |> ignore

    // match01.fs - Unexpected end of input in 'match' expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"match01.fs"|])>]
    let ``match01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3103
        |> ignore

    // try01.fs - Unexpected end of input in 'try' expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"try01.fs"|])>]
    let ``try01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3104
        |> ignore

    // type_id_equal01.fsx - Empty type definition
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_equal01.fsx"|])>]
    let ``type_id_equal01_fsx`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0058
        |> ignore

    // type_id_equal02.fsi + type_id_equal02.fs - Empty type definition in module (multi-file test)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_equal02.fsi"; "type_id_equal02.fs"|])>]
    let ``type_id_equal02`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0058
        |> ignore

    // type_id_equal_curly01.fs - Unmatched '{' in record type definition
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_equal_curly01.fs"|])>]
    let ``type_id_equal_curly01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0604
        |> ignore

    // type_id_parens01.fs - Empty type definition with parens
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_parens01.fs"|])>]
    let ``type_id_parens01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0058
        |> ignore
