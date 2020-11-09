// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module IdentifiersAndKeywords =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"ValidIdentifier01.fs"|])>]
    let ``IdentifiersAndKeywords - ValidIdentifier01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"ValidIdentifier02.fs"|])>]
    let ``IdentifiersAndKeywords - ValidIdentifier02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS1156" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_InvalidIdentifier01.fs"|])>]
    let ``IdentifiersAndKeywords - E_InvalidIdentifier01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal. Valid numeric literals include"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0037" status="error">Duplicate definition of value 'x'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_NameCollision01.fs"|])>]
    let ``IdentifiersAndKeywords - E_NameCollision01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of value 'x'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"W_ReservedWord01.fs"|])>]
    let ``IdentifiersAndKeywords - W_ReservedWord01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0010" status="error">Unexpected keyword 'class' in pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_KeywordIdent01.fs"|])>]
    let ``IdentifiersAndKeywords - E_KeywordIdent01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'class' in pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects status="error" span="(10,11-10,18)" id="FS0883">Invalid namespace, module, type or union case name$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ValidIdentifier03.fs"|])>]
    let ``IdentifiersAndKeywords - E_ValidIdentifier03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0883
        |> withDiagnosticMessageMatches "Invalid namespace, module, type or union case name$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0010" span="(15,6)" status="error">Unexpected character '.+' in expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ValidIdentifier04.fs"|])>]
    let ``IdentifiersAndKeywords - E_ValidIdentifier04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected character '.+' in expression"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0035" status="error">This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"StructNotAllowDoKeyword.fs"|])>]
    let ``IdentifiersAndKeywords - StructNotAllowDoKeyword.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches "This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0599" span="(8,2-8,3)" status="error">Missing qualification after '\.'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_MissingQualification.fs"|])>]
    let ``IdentifiersAndKeywords - E_MissingQualification.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0599
        |> withDiagnosticMessageMatches "Missing qualification after '\.'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS1104" status="warning">Identifiers containing '@' are reserved for use in F# code generation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"W_IdentContainsAtSign.fs"|])>]
    let ``IdentifiersAndKeywords - W_IdentContainsAtSign.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 1104
        |> withDiagnosticMessageMatches "Identifiers containing '@' are reserved for use in F# code generation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects span="(30,5-30,12)" status="warning" id="FS0046">The identifier 'virtual' is reserved for future use by F#</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ReservedIdentKeywords.fs"|])>]
    let ``IdentifiersAndKeywords - E_ReservedIdentKeywords.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0046
        |> withDiagnosticMessageMatches "The identifier 'virtual' is reserved for future use by F#"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifiersAndKeywords)
    //<Expects id="FS0883" span="(59,8-59,26)" status="error">Invalid namespace, module, type or union case name</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_QuotedTypeModuleNames01.fs"|])>]
    let ``IdentifiersAndKeywords - E_QuotedTypeModuleNames01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0883
        |> withDiagnosticMessageMatches "Invalid namespace, module, type or union case name"

