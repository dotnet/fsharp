// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open FSharp.Test.Compiler

module DebuggerTestHelpers =

    let verifyMethodDebugPoints source methodName expectedSequencePoints =
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints(methodName, expectedSequencePoints) ]
