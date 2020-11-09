// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module FieldMembers =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/FieldMembers)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/FieldMembers", Includes=[|"StaticField01.fs"|])>]
    let ``FieldMembers - StaticField01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/FieldMembers)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/FieldMembers", Includes=[|"StaticField02.fs"|])>]
    let ``FieldMembers - StaticField02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/FieldMembers)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/FieldMembers", Includes=[|"StaticField03.fs"|])>]
    let ``FieldMembers - StaticField03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/FieldMembers)
    //<Expects status="error" span="(11,20)" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/FieldMembers", Includes=[|"E_StaticField01.fs"|])>]
    let ``FieldMembers - E_StaticField01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0881
        |> withDiagnosticMessageMatches "\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/FieldMembers)
    //<Expects span="(16,32)" status="error" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/FieldMembers", Includes=[|"E_StaticField02a.fs"|])>]
    let ``FieldMembers - E_StaticField02a.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0881
        |> withDiagnosticMessageMatches "\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$"

