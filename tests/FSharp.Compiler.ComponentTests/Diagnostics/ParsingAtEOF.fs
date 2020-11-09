// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Diagnostics

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ParsingAtEOF =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(4,1-4,6)" id="FS3122">Missing 'do' in 'while' expression\. Expected 'while <expr> do <expr>'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"while_cond01.fs"|])>]
    let ``ParsingAtEOF - while_cond01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3122
        |> withDiagnosticMessageMatches "'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="warning" span="(9,7-9,7)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(6:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"du_with01.fs"|])>]
    let ``ParsingAtEOF - du_with01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0058
        |> withDiagnosticMessageMatches "Possible incorrect indentation: this token is offside of context started at position \(6:1\)\. Try indenting this token further or using standard formatting conventions\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(5,7-5,9)" id="FS3100">Expected an expression after this point$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"for_in01.fs"|])>]
    let ``ParsingAtEOF - for_in01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3100
        |> withDiagnosticMessageMatches "Expected an expression after this point$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(4,1-4,4)" id="FS3123">Missing 'do' in 'for' expression\. Expected 'for <pat> in <expr> do <expr>'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"for_in_range01.fs"|])>]
    let ``ParsingAtEOF - for_in_range01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3123
        |> withDiagnosticMessageMatches "'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(4,1-4,3)" id="FS0589">Incomplete conditional\. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"if01.fs"|])>]
    let ``ParsingAtEOF - if01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0589
        |> withDiagnosticMessageMatches "'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(4,1-4,6)" id="FS3103">Unexpected end of input in 'match' expression\. Expected 'match <expr> with | <pat> -> <expr> | <pat> -> <expr> \.\.\.'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"match01.fs"|])>]
    let ``ParsingAtEOF - match01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3103
        |> withDiagnosticMessageMatches " \.\.\.'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(4,1-4,4)" id="FS3104">Unexpected end of input in 'try' expression\. Expected 'try <expr> with <rules>' or 'try <expr> finally <expr>'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"try01.fs"|])>]
    let ``ParsingAtEOF - try01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3104
        |> withDiagnosticMessageMatches "'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(6,1-6,1)" id="FS0010">Incomplete structured construct at or before this point in type definition\. Expected '}' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_equal_curly01.fs"|])>]
    let ``ParsingAtEOF - type_id_equal_curly01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in type definition\. Expected '}' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/ParsingAtEOF)
    //<Expects status="error" span="(7,6-7,8)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/ParsingAtEOF", Includes=[|"type_id_parens01.fs"|])>]
    let ``ParsingAtEOF - type_id_parens01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0547
        |> withDiagnosticMessageMatches "A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$"

