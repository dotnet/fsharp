// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ImplicitObjectConstructors =

    [<Theory; FileInlineData("E_AddExplicitWithImplicit.fs")>]
    let ``E_AddExplicitWithImplicit_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 762
