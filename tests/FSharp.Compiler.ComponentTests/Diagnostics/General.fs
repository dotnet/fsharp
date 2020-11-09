// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Diagnostics

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module General =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(26,12-26,15)" id="FS0043">The type 'System\.Nullable<int>' does not support the operator '<>\?'\. Consider opening the module 'Microsoft\.FSharp\.Linq\.NullableOperators'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_NullableOperators01.fs"|])>]
    let ``General - E_NullableOperators01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "\?'\. Consider opening the module 'Microsoft\.FSharp\.Linq\.NullableOperators'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(5,9-5,13)" id="FS0741">Unable to parse format string 'Bad precision in format specifier'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringBadPrecision01.fs"|])>]
    let ``General - E_FormattingStringBadPrecision01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> withDiagnosticMessageMatches "Unable to parse format string 'Bad precision in format specifier'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string 'Bad format specifier: '/''$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringBadSpecifier01.fs"|])>]
    let ``General - E_FormattingStringBadSpecifier01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> withDiagnosticMessageMatches "Unable to parse format string 'Bad format specifier: '/''$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string ''c' format does not support precision'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringFlagSetTwice01.fs"|])>]
    let ``General - E_FormattingStringFlagSetTwice01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> withDiagnosticMessageMatches "Unable to parse format string ''c' format does not support precision'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string 'The # formatting modifier is invalid in F#'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringInvalid01.fs"|])>]
    let ``General - E_FormattingStringInvalid01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> withDiagnosticMessageMatches "Unable to parse format string 'The # formatting modifier is invalid in F#'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string ''c' format does not support precision'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringPrecision01.fs"|])>]
    let ``General - E_FormattingStringPrecision01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> withDiagnosticMessageMatches "Unable to parse format string ''c' format does not support precision'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(11,5-11,43)" id="FS0027">This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable m_cts = expression'.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"MutatingImmutable01.fs"|])>]
    let ``General - MutatingImmutable01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0027
        |> withDiagnosticMessageMatches "This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable m_cts = expression'."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(7,1-7,8)" id="FS0027">This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"MutatingImmutable02.fs"|])>]
    let ``General - MutatingImmutable02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0027
        |> withDiagnosticMessageMatches "This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(9,9-9,16)" id="FS0039">The value, namespace, type or module 'Big_int' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Big_int01.fs"|])>]
    let ``General - E_Big_int01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value, namespace, type or module 'Big_int' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" id="FS0001" span="(13,8-15,9)">This expression was expected to have type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Multiline01.fs"|])>]
    let ``General - E_Multiline01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" id="FS0020" span="(8,1-10,18)">The result of this expression has type 'int' and is implicitly ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_Multiline02.fs"|])>]
    let ``General - W_Multiline02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageMatches "The result of this expression has type 'int' and is implicitly ignored"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" id="FS0003" span="(8,1-9,4)">This value is not a function and cannot be applied.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Multiline03.fs"|])>]
    let ``General - E_Multiline03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003
        |> withDiagnosticMessageMatches "This value is not a function and cannot be applied.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_NoValueHasBeenCopiedWarning.fs"|])>]
    let ``General - W_NoValueHasBeenCopiedWarning.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(9,6-9,7)"   id="FS0599">Missing qualification after '\.'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_TryFinallyIncompleteStructuredConstruct.fs"|])>]
    let ``General - E_TryFinallyIncompleteStructuredConstruct.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0599
        |> withDiagnosticMessageMatches "Missing qualification after '\.'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects span="(4,5-4,6)" status="error" id="FS0201">Namespaces cannot contain values\. Consider using a module to hold your value declarations\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_SpanExtendsToNextToken01.fs"|])>]
    let ``General - E_SpanExtendsToNextToken01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0201
        |> withDiagnosticMessageMatches "Namespaces cannot contain values\. Consider using a module to hold your value declarations\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(19,33-19,45)" id="FS0001">.+'unit'.+'GrowingArray<int>'.+\(18,41\)-\(18,46\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"TypecheckSignature01.fs"|])>]
    let ``General - TypecheckSignature01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'.+\(18,41\)-\(18,46\)"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0064" span="(18,35-18,46)" status="warning">SealedType</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_HashOfSealedType01.fs"|])>]
    let ``General - W_HashOfSealedType01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "SealedType"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"Generic_Subtype01.fs"|])>]
    let ``General - Generic_Subtype01.fs - -a --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0564" span="(6,26-6,33)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_BaseInObjectExpression01.fs"|])>]
    let ``General - E_BaseInObjectExpression01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0564
        |> withDiagnosticMessageMatches "'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0564" span="(7,26-7,34)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AsBindingOnInheritDecl01.fs"|])>]
    let ``General - E_AsBindingOnInheritDecl01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0564
        |> withDiagnosticMessageMatches "'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0076" span="(6,1-6,22)" status="error">#r directives may only occur in F# script files \(extensions \.fsx or \.fsscript\)\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_NoPoundRDirectiveInFSFile01.fs"|])>]
    let ``General - E_NoPoundRDirectiveInFSFile01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0076
        |> withDiagnosticMessageMatches "#r directives may only occur in F# script files \(extensions \.fsx or \.fsscript\)\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS1125" span="(15,1-15,14)" status="warning">The instantiation of the generic type 'C2' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'C2<_>'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InstantiationOfGenericTypeMissing01.fs"|])>]
    let ``General - W_InstantiationOfGenericTypeMissing01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 1125
        |> withDiagnosticMessageMatches "'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InstantiationOfGenericTypeMissing02.fs"|])>]
    let ``General - W_InstantiationOfGenericTypeMissing02.fs - -a --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(11,20-11,21)" id="FS0086">The name '\(<\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_LessThan' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator01.fs"|])>]
    let ``General - W_redefineOperator01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(<\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_LessThan' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,21)" status="warning">The name '\(>\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_GreaterThan' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator02.fs"|])>]
    let ``General - W_redefineOperator02.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_GreaterThan' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(11,20-11,22)" id="FS0086">The name '\(<=\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_LessThanOrEqual' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator03.fs"|])>]
    let ``General - W_redefineOperator03.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(<=\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_LessThanOrEqual' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,22)" status="warning">The name '\(>=\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_GreaterThanOrEqual' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator04.fs"|])>]
    let ``General - W_redefineOperator04.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "=\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_GreaterThanOrEqual' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,22)" status="warning">The name '\(<>\)' should not be used as a member name\. To define equality semantics for a type, override the 'Object\.Equals' member\. If defining a static member for use from other CLI languages then use the name 'op_Inequality' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator05.fs"|])>]
    let ``General - W_redefineOperator05.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "\)' should not be used as a member name\. To define equality semantics for a type, override the 'Object\.Equals' member\. If defining a static member for use from other CLI languages then use the name 'op_Inequality' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,21)" status="warning">The name '\(=\)' should not be used as a member name\. To define equality semantics for a type, override the 'Object\.Equals' member\. If defining a static member for use from other CLI languages then use the name 'op_Equality' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator06.fs"|])>]
    let ``General - W_redefineOperator06.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(=\)' should not be used as a member name\. To define equality semantics for a type, override the 'Object\.Equals' member\. If defining a static member for use from other CLI languages then use the name 'op_Equality' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,21)" status="warning">The name '\(&\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_Amp' instead\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator07.fs"|])>]
    let ``General - W_redefineOperator07.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(&\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_Amp' instead\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,22)" status="warning">The name '\(&&\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_BooleanAnd' instead\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator08.fs"|])>]
    let ``General - W_redefineOperator08.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(&&\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_BooleanAnd' instead\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,22)" status="warning">The name '\(or\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'or' instead\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator09.fs"|])>]
    let ``General - W_redefineOperator09.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(or\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'or' instead\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0086" span="(11,20-11,22)" status="warning">The name '\(\|\|\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_BooleanOr' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator10.fs"|])>]
    let ``General - W_redefineOperator10.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> withDiagnosticMessageMatches "The name '\(\|\|\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_BooleanOr' instead\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0039" span="(7,35-7,41)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_class01.fs"|])>]
    let ``General - E_matrix_class01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0039" span="(8,49-8,55)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_interface01.fs"|])>]
    let ``General - E_matrix_interface01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0039" span="(6,18-6,24)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_LetBinding01.fs"|])>]
    let ``General - E_matrix_LetBinding01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0039" span="(6,29-6,35)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_struct01.fs"|])>]
    let ``General - E_matrix_struct01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0001" span="(15,22-15,26)" status="error">This expression was expected to have type.    'M\.M1\.Typ1'    .but here has type.    'M\.M2\.Typ1'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ExpressionHasType_FullPath01.fs"|])>]
    let ``General - E_ExpressionHasType_FullPath01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'M\.M1\.Typ1'    .but here has type.    'M\.M2\.Typ1'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS1125" span="(7,18-7,21)" status="warning">The instantiation of the generic type 'T' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'T<_>'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_GenericTypeProvideATypeInstantiation01.fs"|])>]
    let ``General - W_GenericTypeProvideATypeInstantiation01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 1125
        |> withDiagnosticMessageMatches "'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0420" span="(8,13-8,30)" status="error">Object constructors cannot directly use try/with and try/finally prior to the initialization of the object\. This includes constructs such as 'for x in \.\.\.' that may elaborate to uses of these constructs\. This is a limitation imposed by Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ObjectConstructorAndTry01.fs"|])>]
    let ``General - E_ObjectConstructorAndTry01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0420
        |> withDiagnosticMessageMatches "Object constructors cannot directly use try/with and try/finally prior to the initialization of the object\. This includes constructs such as 'for x in \.\.\.' that may elaborate to uses of these constructs\. This is a limitation imposed by Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0420" span="(9,13-9,30)" status="error">Object constructors cannot directly use try/with and try/finally prior to the initialization of the object\. This includes constructs such as 'for x in \.\.\.' that may elaborate to uses of these constructs\. This is a limitation imposed by Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ObjectConstructorAndTry02.fs"|])>]
    let ``General - E_ObjectConstructorAndTry02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0420
        |> withDiagnosticMessageMatches "Object constructors cannot directly use try/with and try/finally prior to the initialization of the object\. This includes constructs such as 'for x in \.\.\.' that may elaborate to uses of these constructs\. This is a limitation imposed by Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"X-DontWarnOnImplicitModule01.fsscript"|])>]
    let ``General - X-DontWarnOnImplicitModule01.fsscript - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(7,35-7,37)" id="FS0001">This expression was expected to have type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AreYouMissingAnArgumentToAFunction01.fs"|])>]
    let ``General - E_AreYouMissingAnArgumentToAFunction01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0001" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AreYouMissingAnArgumentToAFunction01b.fs"|])>]
    let ``General - E_AreYouMissingAnArgumentToAFunction01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0886" span="(14,7-14,13)" status="error">This is not a valid value for an enumeration literal</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_LiteralEnumerationMustHaveType01.fs"|])>]
    let ``General - E_LiteralEnumerationMustHaveType01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0886
        |> withDiagnosticMessageMatches "This is not a valid value for an enumeration literal"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0547" span="(7,6-7,8)" status="error": A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IndexedPropertySetter01.fs"|])>]
    let ``General - E_IndexedPropertySetter01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0547

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0807" span="(8,9-8,20)" status="error">Property 'X' is not readable$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_PropertyIsNotReadable01.fs"|])>]
    let ``General - E_PropertyIsNotReadable01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0807
        |> withDiagnosticMessageMatches "Property 'X' is not readable$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0077" span="(6,14-6,61)" status="warning">Member constraints with the name 'Pow' are given special status by the F# compiler as certain \.NET types are implicitly augmented with this member\. This may result in runtime failures if you attempt to invoke the member constraint from your own code\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_MemberConstraintsWithSpecialStatus01.fs"|])>]
    let ``General - E_MemberConstraintsWithSpecialStatus01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0077
        |> withDiagnosticMessageMatches "Member constraints with the name 'Pow' are given special status by the F# compiler as certain \.NET types are implicitly augmented with this member\. This may result in runtime failures if you attempt to invoke the member constraint from your own code\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0039" span="(7,10-7,16)" status="error">The type 'Matrix' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FoundInPowerPack_Matrix01.fs"|])>]
    let ``General - E_FoundInPowerPack_Matrix01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'Matrix' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0604" span="(14,9-14,10)" status="error">Unmatched '{'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedKeywordAs01.fs"|])>]
    let ``General - E_UnexpectedKeywordAs01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0604
        |> withDiagnosticMessageMatches "Unmatched '{'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects>\(18,22-18,30\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_Keyword_tailcall01.fs"|])>]
    let ``General - W_Keyword_tailcall01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0010" span="(6,21-6,22)" status="error">Unexpected quote symbol in field declaration$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_OCamlStylePolymorficRecordFields01.fs"|])>]
    let ``General - E_OCamlStylePolymorficRecordFields01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected quote symbol in field declaration$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0010" status="error" span="(8,1)">Incomplete structured construct at or before this point in member definition\. Expected 'with', '=' or other token\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IncompleteConstruct01.fs"|])>]
    let ``General - E_IncompleteConstruct01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in member definition\. Expected 'with', '=' or other token\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0010" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IncompleteConstruct01b.fs"|])>]
    let ``General - E_IncompleteConstruct01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0003" span="(7,26-7,29)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ThisValueIsNotAFunctionAndCannotBeApplied01.fs"|])>]
    let ``General - E_ThisValueIsNotAFunctionAndCannotBeApplied01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0010" status="error">'with'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedKeyworkWith01.fs"|])>]
    let ``General - E_UnexpectedKeyworkWith01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "'with'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0502" status="error" span="(22,1)">The member or object constructor 'Random' takes 0 type argument\(s\) but is here given 1\. The required signature is 'static member Variable\.Random : y:Variable<'a> -> Variable<'a>'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_MemberObjectctorTakeGiven.fs"|])>]
    let ``General - E_MemberObjectctorTakeGiven.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0502
        |> withDiagnosticMessageMatches "'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_StructMustHaveAtLeastOneField.fs"|])>]
    let ``General - E_StructMustHaveAtLeastOneField.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="notin">lambda</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedSymbol01.fs"|])>]
    let ``General - E_UnexpectedSymbol01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withDiagnosticMessageMatches "lambda"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0069" span="(18,23-18,24)" status="warning"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InterfaceImplementationInAugmentation01a.fs"|])>]
    let ``General - W_InterfaceImplementationInAugmentation01a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0069

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0069" span="(26,23-26,25)" status="warning">Interface implementations in augmentations are now deprecated\. Interface implementations should be given on the initial declaration of a type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InterfaceImplementationInAugmentation01b.fs"|])>]
    let ``General - W_InterfaceImplementationInAugmentation01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0069
        |> withDiagnosticMessageMatches "Interface implementations in augmentations are now deprecated\. Interface implementations should be given on the initial declaration of a type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0069" span="(27,23-27,25)" status="warning">Interface implementations in augmentations are now deprecated\. Interface implementations should be given on the initial declaration of a type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InterfaceImplementationInAugmentation02a.fs"|])>]
    let ``General - W_InterfaceImplementationInAugmentation02a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0069
        |> withDiagnosticMessageMatches "Interface implementations in augmentations are now deprecated\. Interface implementations should be given on the initial declaration of a type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0060" span="(13,22-13,25)" status="warning"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation01a.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation01a.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0060" span="(13,21-13,24)" status="warning"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation01b.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation01b.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0001" span="(13,28-13,31)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation02a.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation02a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0001" span="(13,27-13,30)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation02b.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation02b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0060" span="(20,21-20,24)" status="warning">Override implementations in augmentations are now deprecated\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation03a.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation03a.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> withDiagnosticMessageMatches "Override implementations in augmentations are now deprecated\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0060" span="(20,22-20,25)" status="warning">Override implementations in augmentations are now deprecated.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation03b.fs"|])>]
    let ``General - W_OverrideImplementationInAugmentation03b.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> withDiagnosticMessageMatches "Override implementations in augmentations are now deprecated."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects id="FS0071" span="(14,13-14,20)" status="error">Type constraint mismatch when applying the default type 'IA' for a type inference variable\. The type 'IA' is not compatible with the type 'IB' Consider adding further type constraints</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Quotation_UnresolvedGenericConstruct01.fs"|])>]
    let ``General - E_Quotation_UnresolvedGenericConstruct01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0071
        |> withDiagnosticMessageMatches "Type constraint mismatch when applying the default type 'IA' for a type inference variable\. The type 'IA' is not compatible with the type 'IB' Consider adding further type constraints"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" span="(6,11-6,14)" id="FS0039">The value or constructor 'lsl' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_OCamlCompatibility01.fs"|])>]
    let ``General - E_OCamlCompatibility01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'lsl' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="error" id="FS0035" span="(12,11-12,15)">This construct is deprecated: This form of object expression is not used in F#\. Use 'member this\.MemberName \.\.\. = \.\.\.' to define member implementations in object expressions\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_InvalidObjectExpression01.fs"|])>]
    let ``General - E_InvalidObjectExpression01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches "This construct is deprecated: This form of object expression is not used in F#\. Use 'member this\.MemberName \.\.\. = \.\.\.' to define member implementations in object expressions\."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(10,36-10,48)" id="FS0760">It is recommended that objects supporting the IDisposable interface are created using the syntax 'new Type\(args\)', rather than 'Type\(args\)' or 'Type' as a function value representing the constructor, to indicate that resources may be owned by the generated value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_CreateIDisposable.fs"|])>]
    let ``General - W_CreateIDisposable.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0760
        |> withDiagnosticMessageMatches "It is recommended that objects supporting the IDisposable interface are created using the syntax 'new Type\(args\)', rather than 'Type\(args\)' or 'Type' as a function value representing the constructor, to indicate that resources may be owned by the generated value"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(4,11-4,19)" id="FS3189">Redundant arguments are being ignored in function 'failwith'\. Expected 1 but got 2 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_FailwithRedundantArgs.fs"|])>]
    let ``General - W_FailwithRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'failwith'\. Expected 1 but got 2 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(4,11-4,20)" id="FS3189">Redundant arguments are being ignored in function 'failwithf'\. Expected 3 but got 4 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_FailwithfRedundantArgs.fs"|])>]
    let ``General - W_FailwithfRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'failwithf'\. Expected 3 but got 4 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(6,5-6,10)" id="FS3189">Redundant arguments are being ignored in function 'raise'\. Expected 1 but got 2 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_RaiseRedundantArgs.fs"|])>]
    let ``General - W_RaiseRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'raise'\. Expected 1 but got 2 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(6,5-6,15)" id="FS3189">Redundant arguments are being ignored in function 'invalidArg'\. Expected 2 but got 3 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InvalidArgRedundantArgs.fs"|])>]
    let ``General - W_InvalidArgRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'invalidArg'\. Expected 2 but got 3 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(6,17-6,24)" id="FS3189">Redundant arguments are being ignored in function 'nullArg'\. Expected 1 but got 2 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_NullArgRedundantArgs.fs"|])>]
    let ``General - W_NullArgRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'nullArg'\. Expected 1 but got 2 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(6,20-6,29)" id="FS3189">Redundant arguments are being ignored in function 'invalidOp'\. Expected 1 but got 2 arguments\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InvalidOpRedundantArgs.fs"|])>]
    let ``General - W_InvalidOpRedundantArgs.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> withDiagnosticMessageMatches "Redundant arguments are being ignored in function 'invalidOp'\. Expected 1 but got 2 arguments\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(10,5-10,14)" id="FS3190">Lowercase literal 'lowerCase' is being shadowed by a new pattern with the same name\. Only uppercase and module-prefixed literals can be used as named patterns\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_LowercaseLiteralIgnored.fs"|])>]
    let ``General - W_LowercaseLiteralIgnored.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3190
        |> withDiagnosticMessageMatches "Lowercase literal 'lowerCase' is being shadowed by a new pattern with the same name\. Only uppercase and module-prefixed literals can be used as named patterns\.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects status="warning" span="(13,7-13,8)" id="FS0026">This rule will never be matched$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_LowercaseLiteralNotIgnored.fs"|])>]
    let ``General - W_LowercaseLiteralNotIgnored.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched$"

