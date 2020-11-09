// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Diagnostics

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module NONTERM =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(13,9-13,10)" id="FS0583">Unmatched '\('$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"tuplewithlazy01.fs"|])>]
    let ``NONTERM - tuplewithlazy01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0583
        |> withDiagnosticMessageMatches "Unmatched '\('$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(8,25-8,29)" id="FS0010">Unexpected keyword 'lazy' in type definition$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"tuplewithlazy01b.fs"|])>]
    let ``NONTERM - tuplewithlazy01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'lazy' in type definition$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(10,5-10,9)" status="error">Incomplete structured construct at or before this point in union case\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"attrUnionCaseDecl01.fs"|])>]
    let ``NONTERM - attrUnionCaseDecl01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in union case\. Expected identifier, '\(', '\(\*\)' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error" span="(12,5-12,9)">Incomplete structured construct at or before this point in union case\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"attrUnionCaseDecl01b.fs"|])>]
    let ``NONTERM - attrUnionCaseDecl01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in union case\. Expected identifier, '\(', '\(\*\)' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(6,8-6,20)" status="error">Unexpected start of structured construct in definition\. Expected identifier, 'global' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl02.fs"|])>]
    let ``NONTERM - fileModuleImpl02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected start of structured construct in definition\. Expected identifier, 'global' or other token"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected start of structured construct in definition\. Expected identifier, 'global' or other token\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl02b.fs"|])>]
    let ``NONTERM - fileModuleImpl02b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected start of structured construct in definition\. Expected identifier, 'global' or other token\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(9,1-9,1)" status="error">Unexpected end of input in type name\. Expected '>' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typeConstraint01.fs"|])>]
    let ``NONTERM - typeConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "' or other token"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typeConstraint01b.fs"|])>]
    let ``NONTERM - typeConstraint01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(9,115-9,117)" status="error">Unexpected symbol '->' in type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typ01.fs"|])>]
    let ``NONTERM - typ01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "' in type"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected symbol '->' in type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typ01b.fs"|])>]
    let ``NONTERM - typ01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "' in type"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0020" span="(5,1-5,24)" status="warning">The result of this expression has type 'seq<Quotations.Var>' and is implicitly ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"quoteExpr01.fs"|])>]
    let ``NONTERM - quoteExpr01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageMatches "' and is implicitly ignored"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0020" status="warning"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"quoteExpr01b.fs"|])>]
    let ``NONTERM - quoteExpr01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0604" span="(8,1-8,2)" status="error">Unmatched '{'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"braceExpr01.fs"|])>]
    let ``NONTERM - braceExpr01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0604
        |> withDiagnosticMessageMatches "Unmatched '{'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0604" status="error" span="(8,1-8,2)"></Expects>: error : Unmatched '{'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"braceExpr01b.fs"|])>]
    let ``NONTERM - braceExpr01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0604
        |> withDiagnosticMessageMatches ": error : Unmatched '{'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(8,1-8,4)" status="error">'val'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl01.fs"|])>]
    let ``NONTERM - fileModuleImpl01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'val'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(16,1-16,4)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicExprNonEmptyInitial01.fs"|])>]
    let ``NONTERM - monadicExprNonEmptyInitial01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3118
        |> withDiagnosticMessageMatches "Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(22,25-22,32)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicPatternClauses01.fs"|])>]
    let ``NONTERM - monadicPatternClauses01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(8,1-8,1)" id="FS0010">Incomplete structured construct at or before this point in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock01.fs"|])>]
    let ``NONTERM - typedSeqExprBlock01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in expression$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(9,1-9,1)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock02.fs"|])>]
    let ``NONTERM - typedSeqExprBlock02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(7,9-7,11)" status="error">'if'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"declExpr01.fs"|])>]
    let ``NONTERM - declExpr01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'if'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0598" span="(6,1-6,2)" status="error">'\['</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator01.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0598
        |> withDiagnosticMessageMatches "'\['"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(5,6-5,7)" status="error">','.+'->'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator02.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(6,3-6,9)" status="error">'member'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator03.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'member'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(6,3-6,9)" status="error">'member'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator04.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'member'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(5,16-5,17)" status="error">':'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator05.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "':'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(5,1-5,2)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator06.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(9,4-9,6)" id="FS0010">Unexpected symbol '\[<' in definition$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileNamespaceImpl01.fs"|])>]
    let ``NONTERM - fileNamespaceImpl01.fs - --test:ErrorRanges --nowarn:62`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--nowarn:62"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\[<' in definition$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects span="(7,1)" status="error" id="FS0010">Unexpected keyword 'val' in definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileModuleImpl01b.fs"|])>]
    let ``NONTERM - fileModuleImpl01b.fs - --nowarn:62`` compilation =
        compilation
        |> withOptions ["--nowarn:62"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'val' in definition"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects status="error" span="(15,1)" id="FS0528">Unexpected end of input$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicExprNonEmptyInitial01b.fs"|])>]
    let ``NONTERM - monadicExprNonEmptyInitial01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0528
        |> withDiagnosticMessageMatches "Unexpected end of input$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"monadicPatternClauses01b.fs"|])>]
    let ``NONTERM - monadicPatternClauses01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock01b.fs"|])>]
    let ``NONTERM - typedSeqExprBlock01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"typedSeqExprBlock02b.fs"|])>]
    let ``NONTERM - typedSeqExprBlock02b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"declExpr01b.fs"|])>]
    let ``NONTERM - declExpr01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0598" span="(6,1)" status="error">Unmatched '\['$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator01b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0598
        |> withDiagnosticMessageMatches "Unmatched '\['$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected symbol ',' in lambda expression.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator02b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator02b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol ',' in lambda expression."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected keyword 'member' in type definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator03b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator03b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'member' in type definition"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected keyword 'member' in type definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator04b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator04b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'member' in type definition"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected symbol '.' in implementation file</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator05b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator05b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '.' in implementation file"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(6,1-6,2)" status="error">Unexpected character '.' in implementation file</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"interactiveExprOrDefinitionsTerminator06b.fs"|])>]
    let ``NONTERM - interactiveExprOrDefinitionsTerminator06b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected character '.' in implementation file"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" status="error">Unexpected symbol '\[<' in definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"fileNamespaceImpl01b.fs"|])>]
    let ``NONTERM - fileNamespaceImpl01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\[<' in definition"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/NONTERM)
    //<Expects id="FS0010" span="(5,1-5,7)" status="error">Unexpected keyword 'member' in implementation file</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/NONTERM", Includes=[|"memberDefinitionWithoutType01.fs"|])>]
    let ``NONTERM - memberDefinitionWithoutType01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'member' in implementation file"

