// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Comments =

    // SOURCE: embeddedString001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString001.fs"|])>]
    let ``Comments - embeddedString001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: embeddedString002.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString002.fs"|])>]
    let ``Comments - embeddedString002_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: embeddedString003.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString003.fs"|])>]
    let ``Comments - embeddedString003_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: embeddedString004.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString004.fs"|])>]
    let ``Comments - embeddedString004_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_embeddedString005.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_embeddedString005.fs"|])>]
    let ``Comments - E_embeddedString005_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0517
        |> ignore

    // SOURCE: E_IncompleteComment01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment01.fs"|])>]
    let ``Comments - E_IncompleteComment01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: E_IncompleteComment02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment02.fs"|])>]
    let ``Comments - E_IncompleteComment02_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0516
        |> ignore

    // SOURCE: escapeCharsInComments001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments001.fs"|])>]
    let ``Comments - escapeCharsInComments001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: escapeCharsInComments002.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments002.fs"|])>]
    let ``Comments - escapeCharsInComments002_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle001.fs"|])>]
    let ``Comments - ocamlstyle001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle002.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle002.fs"|])>]
    let ``Comments - ocamlstyle002_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle_nested001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested001.fs"|])>]
    let ``Comments - ocamlstyle_nested001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle_nested002.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested002.fs"|])>]
    let ``Comments - ocamlstyle_nested002_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle_nested003.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested003.fs"|])>]
    let ``Comments - ocamlstyle_nested003_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle_nested004.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested004.fs"|])>]
    let ``Comments - ocamlstyle_nested004_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ocamlstyle_nested005.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested005.fs"|])>]
    let ``Comments - ocamlstyle_nested005_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_ocamlstyle_nested006.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested006.fs"|])>]
    let ``Comments - E_ocamlstyle_nested006_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0058
        |> ignore

    // SOURCE: E_ocamlstyle_nested007.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested007.fs"|])>]
    let ``Comments - E_ocamlstyle_nested007_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: XmlDocComments01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"XmlDocComments01.fs"|])>]
    let ``Comments - XmlDocComments01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: DontEscapeCommentFromString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"DontEscapeCommentFromString01.fs"|])>]
    let ``Comments - DontEscapeCommentFromString01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
