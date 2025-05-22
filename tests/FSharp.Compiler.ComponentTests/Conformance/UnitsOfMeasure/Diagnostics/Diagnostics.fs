// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Diagnostics =

    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    [<FileInlineData("IntTypes01.fsx")>]
    [<FileInlineData("RangeExpression01.fsx")>]
    [<FileInlineData("RangeOfDimensioned01.fsx")>]
    [<FileInlineData("RangeOfDimensioned02.fsx")>]
    [<Theory>]
    let ``Diagnostics - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_CantBeUsedAsPrefixArgToAType01.fsx")>]
    let ``E_CantBeUsedAsPrefixArgToAType01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 706, Line 9, Col 12, Line 9, Col 29, "Units-of-measure cannot be used as prefix arguments to a type. Rewrite as postfix arguments in angle brackets.")
        ]

    [<Theory; FileInlineData("E_CantBeUsedAsPrefixArgToAType02.fsx")>]
    let ``E_CantBeUsedAsPrefixArgToAType02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 706, Line 9, Col 12, Line 9, Col 24, "Units-of-measure cannot be used as prefix arguments to a type. Rewrite as postfix arguments in angle brackets.")
        ]

    [<Theory; FileInlineData("E_CantBeUsedAsPrefixArgToAType03.fsx")>]
    let ``E_CantBeUsedAsPrefixArgToAType03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 706, Line 9, Col 12, Line 9, Col 27, "Units-of-measure cannot be used as prefix arguments to a type. Rewrite as postfix arguments in angle brackets.")
        ]

    [<Theory; FileInlineData("E_CantBeUsedAsPrefixArgToAType04.fsx")>]
    let ``E_CantBeUsedAsPrefixArgToAType04_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 706, Line 9, Col 12, Line 9, Col 21, "Units-of-measure cannot be used as prefix arguments to a type. Rewrite as postfix arguments in angle brackets.")
        ]

    [<Theory; FileInlineData("E_CantBeUsedAsPrefixArgToAType05.fsx")>]
    let ``E_CantBeUsedAsPrefixArgToAType05_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 716, Line 10, Col 12, Line 10, Col 21, "Unexpected / in type")
            (Error 704, Line 10, Col 12, Line 10, Col 13, "Expected type, not unit-of-measure")
            (Error 706, Line 10, Col 14, Line 10, Col 21, "Units-of-measure cannot be used as prefix arguments to a type. Rewrite as postfix arguments in angle brackets.")
        ]

    [<Theory; FileInlineData("E_ExpectedTypeNotUnitOfMeasure01.fsx")>]
    let ``E_ExpectedTypeNotUnitOfMeasure01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
        (Error 33, Line 11, Col 10, Line 11, Col 19, "The non-generic type 'Microsoft.FSharp.Core.string' does not expect any type arguments, but here is given 1 type argument(s)")
        (Error 704, Line 12, Col 11, Line 12, Col 12, "Expected type, not unit-of-measure")
        (Error 704, Line 13, Col 17, Line 13, Col 18, "Expected type, not unit-of-measure")
        (Error 704, Line 14, Col 49, Line 14, Col 50, "Expected type, not unit-of-measure")
        ]

    [<Theory; FileInlineData("E_ExplicitUnitOfMeasureParameters01.fsx")>]
    let ``E_ExplicitUnitOfMeasureParameters01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 702, Line 6, Col 41, Line 6, Col 43, "Expected unit-of-measure parameter, not type parameter. Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.")
        ]

    [<Theory; FileInlineData("E_ExplicitUnitOfMeasureParameters02.fsx")>]
    let ``E_ExplicitUnitOfMeasureParameters02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 702, Line 6, Col 45, Line 6, Col 47, "Expected unit-of-measure parameter, not type parameter. Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.")
        ]

    [<Theory; FileInlineData("E_ExplicitUnitOfMeasureParameters03.fsx")>]
    let ``E_ExplicitUnitOfMeasureParameters03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 702, Line 6, Col 33, Line 6, Col 35, "Expected unit-of-measure parameter, not type parameter. Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.")
            (Warning 1178, Line 6, Col 6, Line 6, Col 12, "The struct, record or union type 'T_2920' is not structurally comparable because the type 'obj' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'T_2920' to clarify that the type is not comparable")
        ]

    [<Theory; FileInlineData("E_ExplicitUnitOfMeasureParameters04.fsx")>]
    let ``E_ExplicitUnitOfMeasureParameters04_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 702, Line 6, Col 27, Line 6, Col 29, "Expected unit-of-measure parameter, not type parameter. Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.")
            (Error 35, Line 6, Col 6, Line 6, Col 12, "This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated. Type abbreviations must use all declared type parameters in the type being abbreviated. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of ...'.")
        ]

    [<Theory; FileInlineData("E_NonGenVarInValueRestrictionWarning.fsx")>]
    let ``E_NonGenVarInValueRestrictionWarning_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 30, Line 6, Col 5, Line 6, Col 6, """Value restriction: The value 'x' has an inferred generic type
    val x: float<'_u> list ref
    However, values cannot have generic type variables like '_a in "let x: '_a". You can do one of the following:
    - Define it as a simple data term like an integer literal, a string literal or a union case like "let x = 1"
    - Add an explicit type annotation like "let x : int"
    - Use the value as a non-generic type in later code for type inference like "do x"
    or if you still want type-dependent results, you can define 'x' as a function instead by doing either:
    - Add a unit parameter like "let x()"
    - Write explicit type parameters like "let x<'a>".
    This error is because a let binding without parameters defines a value, not a function. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results.""")
            (Error 30, Line 7, Col 5, Line 7, Col 6, """Value restriction: The value 'y' has an inferred generic type
    val y: '_a list ref
    However, values cannot have generic type variables like '_a in "let x: '_a". You can do one of the following:
    - Define it as a simple data term like an integer literal, a string literal or a union case like "let x = 1"
    - Add an explicit type annotation like "let x : int"
    - Use the value as a non-generic type in later code for type inference like "do x"
    or if you still want type-dependent results, you can define 'y' as a function instead by doing either:
    - Add a unit parameter like "let x()"
    - Write explicit type parameters like "let x<'a>".
    This error is because a let binding without parameters defines a value, not a function. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results.""")
        ]

    [<Theory; FileInlineData("E_ParsingRationalExponents.fsx")>]
    let ``E_ParsingRationalExponents_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 620, Line 11, Col 19, Line 11, Col 20, "Unexpected integer literal in unit-of-measure expression")
            (Error 10, Line 12, Col 20, Line 12, Col 21, "Unexpected symbol ')' in binding. Expected integer literal or other token.")
            (Error 10, Line 13, Col 18, Line 13, Col 19, "Unexpected infix operator in binding. Expected integer literal, '-' or other token.")
        ]

    [<Theory; FileInlineData("E_RangeOfDecimals01.fsx")>]
    let ``E_RangeOfDecimals01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 16, Line 8, Col 23, "The type 'decimal<Kg>' does not match the type 'decimal'")
        ]

    [<Theory; FileInlineData("E_RangeOfDimensioned03.fsx")>]
    let ``E_RangeOfDimensioned03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 7, Col 11, Line 7, Col 18, "The type 'float<Kg>' does not match the type 'float'")
            (Error 1, Line 7, Col 22, Line 7, Col 28, """Type mismatch. Expecting a
'float<Kg>' 
but given a
'float<s>' 
The unit of measure 'Kg' does not match the unit of measure 's'""")
            (Error 1, Line 9, Col 22, Line 9, Col 28, "The unit of measure 's' does not match the unit of measure 'Kg'")
        ]

    [<Theory; FileInlineData("E_UnexpectedTypeParameter01.fsx")>]
    let ``E_UnexpectedTypeParameter01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 634, Line 7, Col 15, Line 7, Col 17, "Non-zero constants cannot have generic units. For generic zero, write 0.0<_>.")
            (Error 634, Line 8, Col 16, Line 8, Col 18, "Non-zero constants cannot have generic units. For generic zero, write 0.0<_>.")
            (Error 634, Line 9, Col 16, Line 9, Col 18, "Non-zero constants cannot have generic units. For generic zero, write 0.0<_>.")
        ]

    [<Theory; FileInlineData("E_UnsupportedType01.fsx")>]
    let ``E_UnsupportedType01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 636, Line 9, Col 9, Line 9, Col 15, "Units-of-measure are only supported on float, float32, decimal, and integer types.")
        ]

    [<Theory; FileInlineData("E_ZeroDenominator.fsx")>]
    let ``E_ZeroDenominator_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 625, Line 8, Col 19, Line 8, Col 20, "Denominator must not be 0 in unit-of-measure exponent")
            (Error 625, Line 9, Col 21, Line 9, Col 22, "Denominator must not be 0 in unit-of-measure exponent")
        ]

    [<Theory; FileInlineData("W_UnitOfMeasureCodeLessGeneric01.fsx")>]
    let ``W_UnitOfMeasureCodeLessGeneric01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 64, Line 5, Col 30, Line 5, Col 31, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'a has been constrained to be measure '1'.")
        ]


