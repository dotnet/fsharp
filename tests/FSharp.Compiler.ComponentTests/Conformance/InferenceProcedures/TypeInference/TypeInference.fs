// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeInference =

    // SOURCE=E_OnOverloadIDAttr01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_OnOverloadIDAttr01.fs")>]
    let ``E_OnOverloadIDAttr01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore

    // SOURCE=CheckWarningsWhenVariablesInstantiatedToInt.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64 - type variable constrained
    [<Theory; FileInlineData("CheckWarningsWhenVariablesInstantiatedToInt.fs")>]
    let ``CheckWarningsWhenVariablesInstantiatedToInt_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=CheckWarningsWhenVariablesInstantiatedToString.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64 - type variable constrained
    [<Theory; FileInlineData("CheckWarningsWhenVariablesInstantiatedToString.fs")>]
    let ``CheckWarningsWhenVariablesInstantiatedToString_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=AdHoc.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("AdHoc.fs")>]
    let ``AdHoc_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=RegressionTest01.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("RegressionTest01.fs")>]
    let ``RegressionTest01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=RegressionTest02.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("RegressionTest02.fs")>]
    let ``RegressionTest02_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoDifferentTypeVariables01.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoDifferentTypeVariables01.fs")>]
    let ``TwoDifferentTypeVariables01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoDifferentTypeVariables01rec.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoDifferentTypeVariables01rec.fs")>]
    let ``TwoDifferentTypeVariables01rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoDifferentTypeVariablesGen00.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoDifferentTypeVariablesGen00.fs")>]
    let ``TwoDifferentTypeVariablesGen00_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoDifferentTypeVariablesGen00rec.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoDifferentTypeVariablesGen00rec.fs")>]
    let ``TwoDifferentTypeVariablesGen00rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoEqualTypeVariables02.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoEqualTypeVariables02.fs")>]
    let ``TwoEqualTypeVariables02_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=TwoEqualTypeVariables02rec.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("TwoEqualTypeVariables02rec.fs")>]
    let ``TwoEqualTypeVariables02rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=OneTypeVariable03.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("OneTypeVariable03.fs")>]
    let ``OneTypeVariable03_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=OneTypeVariable03rec.fs SCFLAGS="-a --test:ErrorRanges --warnaserror+"
    [<Theory; FileInlineData("OneTypeVariable03rec.fs")>]
    let ``OneTypeVariable03rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_TwoDifferentTypeVariablesGen01rec.fs SCFLAGS="-a --test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_TwoDifferentTypeVariablesGen01rec.fs")>]
    let ``E_TwoDifferentTypeVariablesGen01rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    // SOURCE=W_OneTypeVariable03.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_OneTypeVariable03.fs")>]
    let ``W_OneTypeVariable03_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=W_OneTypeVariable03rec.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_OneTypeVariable03rec.fs")>]
    let ``W_OneTypeVariable03rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=W_TwoDifferentTypeVariables01.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_TwoDifferentTypeVariables01.fs")>]
    let ``W_TwoDifferentTypeVariables01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=W_TwoDifferentTypeVariables01rec.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_TwoDifferentTypeVariables01rec.fs")>]
    let ``W_TwoDifferentTypeVariables01rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=W_TwoEqualTypeVariables02.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_TwoEqualTypeVariables02.fs")>]
    let ``W_TwoEqualTypeVariables02_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=W_TwoEqualTypeVariables02rec.fs SCFLAGS="-a --test:ErrorRanges"
    // Test expects warning 64
    [<Theory; FileInlineData("W_TwoEqualTypeVariables02rec.fs")>]
    let ``W_TwoEqualTypeVariables02rec_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withWarningCode 64

    // SOURCE=E_PrettifyForall.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_PrettifyForall.fs")>]
    let ``E_PrettifyForall_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 193
        |> ignore

    // SOURCE=IgnoreUnitParameters.fs
    [<Theory; FileInlineData("IgnoreUnitParameters.fs")>]
    let ``IgnoreUnitParameters_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=IgnoreUnitParameters.fs SCFLAGS="--optimize- -g"
    [<Theory; FileInlineData("IgnoreUnitParameters.fs")>]
    let ``IgnoreUnitParameters2_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--optimize-"; "-g"]
        |> typecheck
        |> shouldSucceed
