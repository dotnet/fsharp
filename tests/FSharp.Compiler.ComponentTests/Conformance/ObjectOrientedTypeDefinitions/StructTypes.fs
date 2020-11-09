// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module StructTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="error" span="(8,17-8,23)" id="FS0438">Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'S3'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Overload_Equals.fs"|])>]
    let ``StructTypes - E_Overload_Equals.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'S3'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="error" span="(8,17-8,28)" id="FS0438">Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'S2'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Overload_GetHashCode.fs"|])>]
    let ``StructTypes - E_Overload_GetHashCode.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'S2'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"EqualAndBoxing01.fs"|])>]
    let ``StructTypes - EqualAndBoxing01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"Regressions01.fs"|])>]
    let ``StructTypes - Regressions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"Regressions02.fs"|])>]
    let ``StructTypes - Regressions02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="error" id="FS0037" span="(9,23-9,24)">Duplicate definition of field 'x'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Regressions02.fs"|])>]
    let ``StructTypes - E_Regressions02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of field 'x'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="error" span="(9,23-9,30)" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Regressions02b.fs"|])>]
    let ``StructTypes - E_Regressions02b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0881
        |> withDiagnosticMessageMatches "\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"NoClashMemberIFaceMember.fs"|])>]
    let ``StructTypes - NoClashMemberIFaceMember.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"Overload_Equals.fs"|])>]
    let ``StructTypes - Overload_Equals.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``StructTypes - Overload_GetHashCode.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"Overload_ToString.fs"|])>]
    let ``StructTypes - Overload_ToString.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects span="(7,9-7,13)" status="error" id="FS0688">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_ImplicitCTorUse01.fs"|])>]
    let ``StructTypes - E_ImplicitCTorUse01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0688
        |> withDiagnosticMessageMatches "The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects span="(7,18-7,22)" status="error" id="FS0688">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_ImplicitCTorUse02.fs"|])>]
    let ``StructTypes - E_ImplicitCTorUse02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0688
        |> withDiagnosticMessageMatches "The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructWithNoFields01.fs"|])>]
    let ``StructTypes - StructWithNoFields01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0081" span="(7,6-7,7)" status="error">Implicit object constructors for structs must take at least one argument</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_StructWithNoFields01.fs"|])>]
    let ``StructTypes - E_StructWithNoFields01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0081
        |> withDiagnosticMessageMatches "Implicit object constructors for structs must take at least one argument"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0947" status="error" span="(5,6-5,7)">Struct types cannot contain abstract members$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_NoAbstractMembers.fs"|])>]
    let ``StructTypes - E_NoAbstractMembers.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0947
        |> withDiagnosticMessageMatches "Struct types cannot contain abstract members$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects status="error" span="(7,5-7,14)" id="FS0901">Structs cannot contain value definitions because the default constructor for structs will not execute these bindings\. Consider adding additional arguments to the primary constructor for the type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_NoLetBindings.fs"|])>]
    let ``StructTypes - E_NoLetBindings.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0901
        |> withDiagnosticMessageMatches "Structs cannot contain value definitions because the default constructor for structs will not execute these bindings\. Consider adding additional arguments to the primary constructor for the type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"MutableFields.fs"|])>]
    let ``StructTypes - MutableFields.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"DoStaticLetDo.fs"|])>]
    let ``StructTypes - DoStaticLetDo.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"ExplicitCtor.fs"|])>]
    let ``StructTypes - ExplicitCtor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"IndexerProperties01.fs"|])>]
    let ``StructTypes - IndexerProperties01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructConstruction01.fs"|])>]
    let ``StructTypes - StructConstruction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructConstruction02.fs"|])>]
    let ``StructTypes - StructConstruction02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects span="(11,10-11,11)" status="error" id="FS0039">The value or constructor 'S' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_StructConstruction03.fs"|])>]
    let ``StructTypes - E_StructConstruction03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'S' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0035" status="error" span="(7,5-7,38)">This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_NoDefaultCtor.fs"|])>]
    let ``StructTypes - E_NoDefaultCtor.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches "This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0953" span="(31,10-31,25)" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_CyclicInheritance01.fs"|])>]
    let ``StructTypes - E_CyclicInheritance01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0953
        |> withDiagnosticMessageMatches "This type definition involves an immediate cyclic reference through an abbreviation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructInstantiation01.fs"|])>]
    let ``StructTypes - StructInstantiation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructAttribute01.fs"|])>]
    let ``StructTypes - StructAttribute01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0927" status="error">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"StructAttribute02.fs"|])>]
    let ``StructTypes - StructAttribute02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0927
        |> withDiagnosticMessageMatches "The kind of the type specified by its attributes does not match the kind implied by its definition"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_StructInheritance01.fs"|])>]
    let ``StructTypes - E_StructInheritance01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0945
        |> withDiagnosticMessageMatches "Cannot inherit a sealed type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0945" status="error" span="(17,5)">Cannot inherit a sealed type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_StructInheritance01b.fs"|])>]
    let ``StructTypes - E_StructInheritance01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0945
        |> withDiagnosticMessageMatches "Cannot inherit a sealed type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"GenericStruct01.fs"|])>]
    let ``StructTypes - GenericStruct01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0926" span="(8,6)" status="error">The attributes of this type specify multiple kinds for the type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_AbstractClassStruct.fs"|])>]
    let ``StructTypes - E_AbstractClassStruct.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0926
        |> withDiagnosticMessageMatches "The attributes of this type specify multiple kinds for the type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0688" span="(13,9-13,25)" status="error">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Nullness01.fs"|])>]
    let ``StructTypes - E_Nullness01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0688
        |> withDiagnosticMessageMatches "The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0688" span="(19,9-19,28)" status="error">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_Nullness02.fs"|])>]
    let ``StructTypes - E_Nullness02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0688
        |> withDiagnosticMessageMatches "The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/StructTypes)
    //<Expects id="FS0039" status="error">The value or constructor 'BadType4' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/StructTypes", Includes=[|"E_InvalidRecursiveGeneric01.fs"|])>]
    let ``StructTypes - E_InvalidRecursiveGeneric01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'BadType4' is not defined"

