// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SequenceExpressions =

    // SOURCE=version47/W_IfThenElse01.fs - success in 4.7+
    [<Theory; FileInlineData("W_IfThenElse01_v47.fs")>]
    let ``W_IfThenElse01_v47_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=version47/W_IfThenElse02.fs - success in 4.7+
    [<Theory; FileInlineData("W_IfThenElse02_v47.fs")>]
    let ``W_IfThenElse02_v47_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=version47/W_IfThenElse03.fs - success in 4.7+
    [<Theory; FileInlineData("W_IfThenElse03_v47.fs")>]
    let ``W_IfThenElse03_v47_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=IfThenElse04.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("IfThenElse04.fs")>]
    let ``IfThenElse04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=IfThenElse05.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("IfThenElse05.fs")>]
    let ``IfThenElse05_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=IfThenElse06.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("IfThenElse06.fs")>]
    let ``IfThenElse06_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=IfThenElse07.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("IfThenElse07.fs")>]
    let ``IfThenElse07_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=tailcalls01.fs
    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=tailcalls02.fs
    [<Theory; FileInlineData("tailcalls02.fs")>]
    let ``tailcalls02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CodeDisposalInMatch01.fs - uses deprecated ref cell operators
    [<Theory; FileInlineData("CodeDisposalInMatch01.fs")>]
    let ``CodeDisposalInMatch01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=final_yield_bang_keyword_01.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("final_yield_bang_keyword_01.fs")>]
    let ``final_yield_bang_keyword_01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=final_yield_dash_gt_01.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("final_yield_dash_gt_01.fs")>]
    let ``final_yield_dash_gt_01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=final_yield_keyword_01.fs SCFLAGS="--test:ErrorRanges --warnaserror"
    [<Theory; FileInlineData("final_yield_keyword_01.fs")>]
    let ``final_yield_keyword_01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=E_final_yield_dash_gt_01.fs SCFLAGS="--test:ErrorRanges" - error FS0596
    [<Theory; FileInlineData("E_final_yield_dash_gt_01.fs")>]
    let ``E_final_yield_dash_gt_01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 596
        |> withDiagnosticMessageMatches "'->' in sequence"

    // SOURCE=ReallyLongArray01.fs
    [<Theory; FileInlineData("ReallyLongArray01.fs")>]
    let ``ReallyLongArray01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=YieldInsideFlowControl.fs
    [<Theory; FileInlineData("YieldInsideFlowControl.fs")>]
    let ``YieldInsideFlowControl_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
