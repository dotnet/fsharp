// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.SpecialAttributesAndTypes.Imported.System

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ThreadStatic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/SpecialAttributesAndTypes/Imported/System.ThreadStatic)
    //<Expects status="errors" span="(8,9-8,10)" id="FS0056">Thread static and context static 'let' bindings are deprecated\. Instead use a declaration of the form 'static val mutable <ident> : <type>' in a class\. Add the 'DefaultValue' attribute to this declaration to indicate that the value is initialized to the default value on each new thread\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/SpecialAttributesAndTypes/Imported/System.ThreadStatic", Includes=[|"W_Deprecated01.fs"|])>]
    let ``ThreadStatic - W_Deprecated01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck

