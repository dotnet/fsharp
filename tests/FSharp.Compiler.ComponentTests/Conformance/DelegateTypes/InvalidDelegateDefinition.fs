// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DelegateTypes

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module InvalidDelegateDefinition =
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"invalid_delegate_definition.fs"|])>]
    let ``invalid_delegate_definition.fs`` compilation =
        compilation
        |> asFsx
        |> runFsi
        |> shouldFail
