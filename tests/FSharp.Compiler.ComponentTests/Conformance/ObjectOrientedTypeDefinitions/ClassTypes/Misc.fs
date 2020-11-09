// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Misc =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"GenericClass01.fs"|])>]
    let ``Misc - GenericClass01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects status="error" span="(10,12-10,28)" id="FS0696">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_CyclicInheritance.fs"|])>]
    let ``Misc - E_CyclicInheritance.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0696
        |> withDiagnosticMessageMatches "This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0365" status="error" span="(17,6)">No implementation was given for 'abstract member Foo\.f : int -> int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_AbstractClass01.fs"|])>]
    let ``Misc - E_AbstractClass01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0365
        |> withDiagnosticMessageMatches " int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0759" status="error" span="(18,9)">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_AbstractClass02.fs"|])>]
    let ``Misc - E_AbstractClass02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0759
        |> withDiagnosticMessageMatches "Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0759" status="error" span="(10,12)">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_AbstractClass03.fs"|])>]
    let ``Misc - E_AbstractClass03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0759
        |> withDiagnosticMessageMatches "Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_SealedClass01.fs"|])>]
    let ``Misc - E_SealedClass01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0945
        |> withDiagnosticMessageMatches "Cannot inherit a sealed type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by its type annotations.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"W_SealedClass02.fs"|])>]
    let ``Misc - W_SealedClass02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by its type annotations."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"W_SealedClass03.fs"|])>]
    let ``Misc - W_SealedClass03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects status="error" span="(9,5-9,8)" id="FS0010">Unexpected keyword 'end' in implementation file$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_NoNestedTypes.fs"|])>]
    let ``Misc - E_NoNestedTypes.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'end' in implementation file$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"AbstractClassAttribute01.fs"|])>]
    let ``Misc - AbstractClassAttribute01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0939" status="error">Only classes may be given the 'AbstractClass' attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"AbstractClassAttribute02.fs"|])>]
    let ``Misc - AbstractClassAttribute02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0939
        |> withDiagnosticMessageMatches "Only classes may be given the 'AbstractClass' attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0696" span="(9,60-9,61)" status="error">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_ExplicitConstructor.fs"|])>]
    let ``Misc - E_ExplicitConstructor.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0696
        |> withDiagnosticMessageMatches "This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0001" span="(12,9-12,10)" status="error">A generic construct requires that the type 'A' be non-abstract</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_AbstractClassAttribute01.fs"|])>]
    let ``Misc - E_AbstractClassAttribute01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "A generic construct requires that the type 'A' be non-abstract"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0771" span="(30,17)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_RestrictedSuperTypes.fs"|])>]
    let ``Misc - E_RestrictedSuperTypes.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0771
        |> withDiagnosticMessageMatches "The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"Decondensation.fs"|])>]
    let ``Misc - Decondensation.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    //<Expects id="FS0010" status="error" span="(13,16-13,18)">Unexpected infix operator in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"E_ZeroArity.fs"|])>]
    let ``Misc - E_ZeroArity.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected infix operator in expression$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"ValueRestrictionCtor.fs"|])>]
    let ``Misc - ValueRestrictionCtor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/Misc", Includes=[|"AbstractClassMultipleConstructors01.fs"|])>]
    let ``Misc - AbstractClassMultipleConstructors01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

