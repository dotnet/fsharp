// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS1232" span="(8,3-8,6)" status="error">End of file in triple-quote string begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_Pathological01.fs"|])>]
    let ``Basic - E_Pathological01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1232
        |> withDiagnosticMessageMatches "End of file in triple-quote string begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"Pathological02.fs"|])>]
    let ``Basic - Pathological02.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0514" span="(9,21-9,22)" status="error">End of file in string begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_Pathological03.fs"|])>]
    let ``Basic - E_Pathological03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0514
        |> withDiagnosticMessageMatches "End of file in string begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"Pathological04.fs"|])>]
    let ``Basic - Pathological04.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0515" span="(8,33-8,35)" status="error">End of file in verbatim string begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_Pathological05.fs"|])>]
    let ``Basic - E_Pathological05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0515
        |> withDiagnosticMessageMatches "End of file in verbatim string begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS1232" span="(8,20-8,23)" status="error">End of file in triple-quote string begun at or before here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_Pathological06.fs"|])>]
    let ``Basic - E_Pathological06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1232
        |> withDiagnosticMessageMatches "End of file in triple-quote string begun at or before here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"ManyLetBindings.fs"|])>]
    let ``Basic - ManyLetBindings.fs - --debug:full --optimize-`` compilation =
        compilation
        |> withOptions ["--debug:full"; "--optimize-"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"ManyLetBindings.fs"|])>]
    let ``Basic - ManyLetBindings.fs - --debug:portable --optimize-`` compilation =
        compilation
        |> withOptions ["--debug:portable"; "--optimize-"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"SanityCheck.fs"|])>]
    let ``Basic - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"nestedLetBindings.fs"|])>]
    let ``Basic - nestedLetBindings.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"Literals01.fs"|])>]
    let ``Basic - Literals01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="warning" span="(26,13)" id="FS3178">This is not valid literal expression. The \[<Literal>\] attribute will be ignored\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_Literals04.fs"|])>]
    let ``Basic - E_Literals04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 3178
        |> withDiagnosticMessageMatches "\] attribute will be ignored\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"AsPat01.fs"|])>]
    let ``Basic - AsPat01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"AsPat02.fs"|])>]
    let ``Basic - AsPat02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="error" id="FS0001">Type mismatch\. Expecting a+</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_AsPat01.fs"|])>]
    let ``Basic - E_AsPat01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "Type mismatch\. Expecting a+"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0020" status="warning">The result of this expression has type 'int' and is implicitly ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"W_DoBindingsNotUnit01.fs"|])>]
    let ``Basic - W_DoBindingsNotUnit01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageMatches "The result of this expression has type 'int' and is implicitly ignored"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0832" span="(11,12-11,16)" status="error">Only functions may be marked 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_ErrorsForInlineValue.fs"|])>]
    let ``Basic - E_ErrorsForInlineValue.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0832
        |> withDiagnosticMessageMatches "Only functions may be marked 'inline'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0010" span="(7,13-7,14)" status="error">Unexpected reserved keyword in pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_GenericTypeAnnotations01.fs"|])>]
    let ``Basic - E_GenericTypeAnnotations01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected reserved keyword in pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects id="FS0842" span="(14,44-14,59)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_AttributesOnLet01.fs"|])>]
    let ``Basic - E_AttributesOnLet01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="error" span="(10,16-10,17)" id="FS0010">Unexpected symbol '\)' in implementation file$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_ErrorsforIncompleteTryWith.fs"|])>]
    let ``Basic - E_ErrorsforIncompleteTryWith.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\)' in implementation file$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="error" id="FS0037" span="(10,13-10,24)">Duplicate definition of value 'foo'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_InvalidInnerRecursiveBinding.fs"|])>]
    let ``Basic - E_InvalidInnerRecursiveBinding.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of value 'foo'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/Basic)
    //<Expects status="error" id="FS0037" span="(8,5-8,8)">Duplicate definition of value 'foo'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/Basic", Includes=[|"E_InvalidInnerRecursiveBinding2.fs"|])>]
    let ``Basic - E_InvalidInnerRecursiveBinding2.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of value 'foo'"

