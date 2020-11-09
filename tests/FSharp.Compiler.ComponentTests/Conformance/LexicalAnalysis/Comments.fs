// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Comments =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"star01.fs"|])>]
    let ``Comments - star01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="error" id="FS0010" span="(8,3-8,4)">Unexpected symbol '\*' in implementation file$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_star02.fs"|])>]
    let ``Comments - E_star02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\*' in implementation file$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"star03.fs"|])>]
    let ``Comments - star03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString001.fs"|])>]
    let ``Comments - embeddedString001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString002.fs"|])>]
    let ``Comments - embeddedString002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString003.fs"|])>]
    let ``Comments - embeddedString003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString004.fs"|])>]
    let ``Comments - embeddedString004.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="error" id="FS0517" span="(9,1-9,3)">End of file in string embedded in comment begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_embeddedString005.fs"|])>]
    let ``Comments - E_embeddedString005.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0517
        |> withDiagnosticMessageMatches "End of file in string embedded in comment begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects id="FS0516" status="error" span="(9,5)">End of file in comment begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment01.fs"|])>]
    let ``Comments - E_IncompleteComment01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects id="FS0516" status="error">End of file in comment begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment02.fs"|])>]
    let ``Comments - E_IncompleteComment02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments001.fs"|])>]
    let ``Comments - escapeCharsInComments001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments002.fs"|])>]
    let ``Comments - escapeCharsInComments002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle001.fs"|])>]
    let ``Comments - ocamlstyle001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle002.fs"|])>]
    let ``Comments - ocamlstyle002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested001.fs"|])>]
    let ``Comments - ocamlstyle_nested001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested002.fs"|])>]
    let ``Comments - ocamlstyle_nested002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested003.fs"|])>]
    let ``Comments - ocamlstyle_nested003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested004.fs"|])>]
    let ``Comments - ocamlstyle_nested004.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested005.fs"|])>]
    let ``Comments - ocamlstyle_nested005.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="error" span="(10,10)" id="FS0516">End of file in comment begun at or before here$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested006.fs"|])>]
    let ``Comments - E_ocamlstyle_nested006.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects id="FS0010" span="(23,1)" status="error">Incomplete structured construct at or before this point in implementation file$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested007.fs"|])>]
    let ``Comments - E_ocamlstyle_nested007.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in implementation file$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"XmlDocComments01.fs"|])>]
    let ``Comments - XmlDocComments01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

