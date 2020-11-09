// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeAbbreviations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    //<Expects id="FS0953" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"E_InfiniteAbbreviation02.fs"|])>]
    let ``TypeAbbreviations - E_InfiniteAbbreviation02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0953
        |> withDiagnosticMessageMatches "This type definition involves an immediate cyclic reference through an abbreviation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    //<Expects id="FS0010" span="(5,6-5,7)" status="error">Unexpected character '.+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"E_UnexpectedCharInTypeName01.fs"|])>]
    let ``TypeAbbreviations - E_UnexpectedCharInTypeName01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected character '.+'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    //<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"E_InheritTypeAbrev.fs"|])>]
    let ``TypeAbbreviations - E_InheritTypeAbrev.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0945
        |> withDiagnosticMessageMatches "Cannot inherit a sealed type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"TypeAbbreviationAfterForwardRef.fs"|])>]
    let ``TypeAbbreviations - TypeAbbreviationAfterForwardRef.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"PrivateTypeAbbreviation01.fs"|])>]
    let ``TypeAbbreviations - PrivateTypeAbbreviation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations)
    //<Expects id="FS1092" status="error" span="(15,10-15,13)">The type 'X' is not accessible from this code location</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations", Includes=[|"E_PrivateTypeAbbreviation02.fs"|])>]
    let ``TypeAbbreviations - E_PrivateTypeAbbreviation02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1092
        |> withDiagnosticMessageMatches "The type 'X' is not accessible from this code location"

