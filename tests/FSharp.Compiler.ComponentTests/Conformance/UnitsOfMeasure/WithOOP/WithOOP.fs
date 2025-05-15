// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module WithOOP =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("GenericUOM01.fsx")>]
    [<FileInlineData("InInterface01.fsx")>]
    [<FileInlineData("Polymorphism02.fsx")>]
    [<FileInlineData("StaticsOnMeasure01.fsx")>]
    [<Theory>]
    let ``TypeChecker - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_GenericUOM01.fsx")>]
    let ``E_GenericUOM01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Warning 842, Line 8, Col 36, Line 8, Col 51, "This attribute is not valid for use on this language element")
        ]

    [<Theory; FileInlineData("E_NoConstructorOnMeasure01.fsx")>]
    let ``E_NoConstructorOnMeasure01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 904, Line 8, Col 5, Line 8, Col 18, "Measure declarations may have only static members: constructors are not available")
        ]

    [<Theory; FileInlineData("E_NoInstanceOnMeasure01.fsx")>]
    let ``E_NoInstanceOnMeasure01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 897, Line 9, Col 5, Line 9, Col 26, "Measure declarations may have only static members")
        ]


    [<Theory; FileInlineData("E_OverloadsDifferByUOMAttr.fsx")>]
    let ``E_OverloadsDifferByUOMAttr_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 438, Line 10, Col 5, Line 10, Col 8, "Duplicate method. The method '.ctor' has the same name and signature as another method in type 'Foo<'a>' once tuples, functions, units of measure and/or provided types are erased.")
            (Error 438, Line 9, Col 5, Line 9, Col 8, "Duplicate method. The method '.ctor' has the same name and signature as another method in type 'Foo<'a>' once tuples, functions, units of measure and/or provided types are erased.")
        ]

    [<Theory; FileInlineData("E_Polymorphism01.fsx")>]
    let ``E_Polymorphism01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 366, Line 12, Col 13, Line 12, Col 25, "No implementation was given for 'abstract Unit.Factor: unit -> float'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]
