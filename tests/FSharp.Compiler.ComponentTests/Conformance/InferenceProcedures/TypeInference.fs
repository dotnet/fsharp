// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeInference =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects status="error" span="(11,7-11,17)" id="FS0039">The type 'OverloadID' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"E_OnOverloadIDAttr01.fs"|])>]
    let ``TypeInference - E_OnOverloadIDAttr01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'OverloadID' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(19,22-19,23)" status="warning">.+'a.+'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"CheckWarningsWhenVariablesInstantiatedToInt.fs"|])>]
    let ``TypeInference - CheckWarningsWhenVariablesInstantiatedToInt.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches ".+'a.+'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(19,22-19,23)" status="warning">.+'a.+'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"CheckWarningsWhenVariablesInstantiatedToString.fs"|])>]
    let ``TypeInference - CheckWarningsWhenVariablesInstantiatedToString.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches ".+'a.+'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"AdHoc.fs"|])>]
    let ``TypeInference - AdHoc.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"RegressionTest01.fs"|])>]
    let ``TypeInference - RegressionTest01.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"RegressionTest02.fs"|])>]
    let ``TypeInference - RegressionTest02.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoDifferentTypeVariables01.fs"|])>]
    let ``TypeInference - TwoDifferentTypeVariables01.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoDifferentTypeVariables01rec.fs"|])>]
    let ``TypeInference - TwoDifferentTypeVariables01rec.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoDifferentTypeVariablesGen00.fs"|])>]
    let ``TypeInference - TwoDifferentTypeVariablesGen00.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoDifferentTypeVariablesGen00rec.fs"|])>]
    let ``TypeInference - TwoDifferentTypeVariablesGen00rec.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoEqualTypeVariables02.fs"|])>]
    let ``TypeInference - TwoEqualTypeVariables02.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"TwoEqualTypeVariables02rec.fs"|])>]
    let ``TypeInference - TwoEqualTypeVariables02rec.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"OneTypeVariable03.fs"|])>]
    let ``TypeInference - OneTypeVariable03.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"OneTypeVariable03rec.fs"|])>]
    let ``TypeInference - OneTypeVariable03rec.fs - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects spans="(67,17-67,42)" status="error" id="FS0043">A type parameter is missing a constraint 'when 'b :> C'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"E_TwoDifferentTypeVariablesGen01rec.fs"|])>]
    let ``TypeInference - E_TwoDifferentTypeVariablesGen01rec.fs - -a --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches " C'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(63,44-63,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_OneTypeVariable03.fs"|])>]
    let ``TypeInference - W_OneTypeVariable03.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(63,48-63,49)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_OneTypeVariable03rec.fs"|])>]
    let ``TypeInference - W_OneTypeVariable03rec.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(73,43-73,44)" status="warning">.+'b.+'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_TwoDifferentTypeVariables01.fs"|])>]
    let ``TypeInference - W_TwoDifferentTypeVariables01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches ".+'b.+'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(65,47-65,48)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_TwoDifferentTypeVariables01rec.fs"|])>]
    let ``TypeInference - W_TwoDifferentTypeVariables01rec.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(63,43-63,44)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_TwoEqualTypeVariables02.fs"|])>]
    let ``TypeInference - W_TwoEqualTypeVariables02.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0064" span="(62,44-62,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"W_TwoEqualTypeVariables02rec.fs"|])>]
    let ``TypeInference - W_TwoEqualTypeVariables02rec.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects id="FS0193" span="(5,9-5,10)" status="error">Type constraint mismatch. The type.+''a'.+is not compatible with type.+System\.IDisposable</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"E_PrettifyForall.fs"|])>]
    let ``TypeInference - E_PrettifyForall.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "Type constraint mismatch. The type.+''a'.+is not compatible with type.+System\.IDisposable"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/TypeInference)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/TypeInference", Includes=[|"IgnoreUnitParameters.fs"|])>]
    let ``TypeInference - IgnoreUnitParameters.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

