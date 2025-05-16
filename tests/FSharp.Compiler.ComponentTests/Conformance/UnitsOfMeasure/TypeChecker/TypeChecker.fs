// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeChecker =
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

    [<FileInlineData("Generalization01.fsx")>]
    [<FileInlineData("GenericSubType01.fsx")>]
    [<FileInlineData("Slash_InFunction01.fsx")>]
    [<FileInlineData("Slash_InMethod01.fsx")>]
    [<FileInlineData("TypeAbbreviation_decimal_01.fsx")>]
    [<FileInlineData("TypeAbbreviation_float32_01.fsx")>]
    [<FileInlineData("TypeAbbreviation_float_01.fsx")>]
    [<FileInlineData("TypeConstraint02.fsx")>]
    [<FileInlineData("ValueRestriction01.fsx")>]
    [<Theory>]
    let ``TypeChecker - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed


    [<Theory; FileInlineData("E_GenInterfaceWithDifferentGenInstantiations.fsx")>]
    let ``E_GenInterfaceWithDifferentGenInstantiations_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 3360, Line 13, Col 6, Line 13, Col 8, "'IB<'b>' cannot implement the interface 'IA<_>' with the two instantiations 'IA<kg>' and 'IA<'b>' because they may unify.")
        ]

    [<Theory; FileInlineData("E_typechecker01.fsx")>]
    let ``E_typechecker01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 687, Line 8, Col 1, Line 8, Col 2, "This value, type or method expects 2 type parameter(s) but was given 1")
        ]

    [<Theory; FileInlineData("W_LessGeneric01.fsx")>]
    let ``W_LessGeneric01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 64, Line 18, Col 17, Line 18, Col 19, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'u has been constrained to be measure '1'.")
        ]

    [<Theory; FileInlineData("W_LessGeneric02.fsx")>]
    let ``W_LessGeneric02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 464, Line 9, Col 18, Line 9, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 10, Col 18, Line 10, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 11, Col 18, Line 11, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 12, Col 18, Line 12, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 13, Col 18, Line 13, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 14, Col 18, Line 14, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 15, Col 18, Line 15, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 16, Col 18, Line 16, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 17, Col 18, Line 17, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 18, Col 18, Line 18, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 19, Col 18, Line 19, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 20, Col 18, Line 20, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 21, Col 18, Line 21, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 22, Col 18, Line 22, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 23, Col 18, Line 23, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 24, Col 18, Line 24, Col 25, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
            (Warning 464, Line 25, Col 21, Line 25, Col 28, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
        ]

    [<Theory; FileInlineData("W_TypeConstraint01.fsx")>]
    let ``W_TypeConstraint01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 64, Line 11, Col 7, Line 11, Col 14, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'u has been constrained to be measure 'kg'.")
            (Warning 64, Line 11, Col 19, Line 11, Col 26, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'v has been constrained to be measure 'kg'.")
        ]

    [<Theory; FileInlineData("W_TypeConstraint03.fsx")>]
    let ``W_TypeConstraint03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
#if NETFRAMEWORK
            (Warning 52, Line 10, Col 7, Line 10, Col 21, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
#endif
        ]
