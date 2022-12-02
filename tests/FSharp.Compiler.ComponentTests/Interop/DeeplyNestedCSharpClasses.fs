// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System

module ``Deeply nested CSharpClasses`` =

//Expect error similar to:
//(8,29): error FS0039: The value, constructor, namespace or type 'somefunctoin' is not defined. Maybe you want one of the following:   somefunction

    [<Fact>]
    let ``Missing function generates good message and range`` () =

        let cslib = 
            CSharp """
using System;

namespace TorchSharp
{
    public class torch
    {

        public class nn
        {

            public class functional
            {
                public static void somefunction() { }
            }
        }
    }
}"""

        let fsharpSource =
            """
open TorchSharp

let loss2 = torch.nn.functional.somefunctoin()   //Note the miss-typed functionname we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compileAndRun
        |> shouldSucceed

