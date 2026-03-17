// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ComponentTests.Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for NONTERM diagnostics - verifying error messages don't contain internal parser "NONTERM" tokens.
/// Migrated from tests/fsharpqa/Source/Diagnostics/NONTERM/
module NONTERM =

    let private resourcePath = __SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM"

    // tuplewithlazy01.fs - Unexpected keyword 'lazy' in pattern
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"tuplewithlazy01.fs"|])>]
    let ``tuplewithlazy01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // tuplewithlazy01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"tuplewithlazy01b.fs"|])>]
    let ``tuplewithlazy01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // attrUnionCaseDecl01.fs - Incomplete structured construct in union case
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"attrUnionCaseDecl01.fs"|])>]
    let ``attrUnionCaseDecl01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // attrUnionCaseDecl01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"attrUnionCaseDecl01b.fs"|])>]
    let ``attrUnionCaseDecl01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // fileModuleImpl04.fs - FSI mode, expect FS1159 error
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl04.fs"|])>]
    let ``fileModuleImpl04_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withErrorCode 1159
        |> ignore

    // fileModuleImpl03.fs - FSI mode, should succeed (warning about implicitly ignored result is OK)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl03.fs"|])>]
    let ``fileModuleImpl03_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // fileModuleImpl03b.fs - FSI mode, should succeed (warning about implicitly ignored result is OK)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl03b.fs"|])>]
    let ``fileModuleImpl03b_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // fileModuleImpl02.fs - Unexpected start of structured construct
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl02.fs"|])>]
    let ``fileModuleImpl02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // fileModuleImpl02b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl02b.fs"|])>]
    let ``fileModuleImpl02b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // typeConstraint01.fs - Unexpected end of input in type name
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typeConstraint01.fs"|])>]
    let ``typeConstraint01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // typeConstraint01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typeConstraint01b.fs"|])>]
    let ``typeConstraint01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // typ01.fs - Expecting type
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typ01.fs"|])>]
    let ``typ01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3565
        |> ignore

    // typ01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typ01b.fs"|])>]
    let ``typ01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3565
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // quoteExpr01.fs - FS0020 warning about implicitly ignored result
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"quoteExpr01.fs"|])>]
    let ``quoteExpr01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> ignore

    // quoteExpr01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"quoteExpr01b.fs"|])>]
    let ``quoteExpr01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // braceExpr01.fs - Unexpected symbol '<' in expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"braceExpr01.fs"|])>]
    let ``braceExpr01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // braceExpr01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"braceExpr01b.fs"|])>]
    let ``braceExpr01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // fileModuleImpl01.fs - Unexpected keyword 'val'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl01.fs"|])>]
    let ``fileModuleImpl01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // monadicExprNonEmptyInitial01.fs - Multiple parse errors
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicExprNonEmptyInitial01.fs"|])>]
    let ``monadicExprNonEmptyInitial01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> ignore

    // monadicPatternClauses01.fs - Unexpected token in match expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicPatternClauses01.fs"|])>]
    let ``monadicPatternClauses01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // typedSeqExprBlock01.fs - Expecting expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock01.fs"|])>]
    let ``typedSeqExprBlock01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3524
        |> ignore

    // typedSeqExprBlock02.fs - Expecting expression
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock02.fs"|])>]
    let ``typedSeqExprBlock02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3524
        |> ignore

    // interactiveExprOrDefinitionsTerminator01.fs - Unexpected symbol ';;'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator01.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // interactiveExprOrDefinitionsTerminator02.fs - Unexpected symbol ','
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator02.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // interactiveExprOrDefinitionsTerminator05.fs - Unexpected symbol ':'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator05.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator05_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // interactiveExprOrDefinitionsTerminator06.fs - Unexpected character
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator06.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator06_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // monadicExprNonEmptyInitial01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicExprNonEmptyInitial01b.fs"|])>]
    let ``monadicExprNonEmptyInitial01b_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // monadicPatternClauses01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicPatternClauses01b.fs"|])>]
    let ``monadicPatternClauses01b_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // typedSeqExprBlock01b.fs - Same test with --mlcompatibility, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock01b.fs"|])>]
    let ``typedSeqExprBlock01b_fs`` compilation =
        compilation
        |> withOptions ["--mlcompatibility"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3524
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // typedSeqExprBlock02b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock02b.fs"|])>]
    let ``typedSeqExprBlock02b_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3524
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // interactiveExprOrDefinitionsTerminator01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator01b.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator01b_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // interactiveExprOrDefinitionsTerminator02b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator02b.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator02b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // interactiveExprOrDefinitionsTerminator05b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator05b.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator05b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // interactiveExprOrDefinitionsTerminator06b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator06b.fs"|])>]
    let ``interactiveExprOrDefinitionsTerminator06b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore

    // memberDefinitionWithoutType01.fs - Unexpected keyword 'member'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"memberDefinitionWithoutType01.fs"|])>]
    let ``memberDefinitionWithoutType01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // memberDefinitionWithoutType01b.fs - Same test, verify NONTERM not in message
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"memberDefinitionWithoutType01b.fs"|])>]
    let ``memberDefinitionWithoutType01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageDoesntMatch "NONTERM"
        |> ignore
