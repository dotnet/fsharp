// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

[<TestFixture>]
module SpanTests =

    let x = 1

#if NET472
#else
    [<Test>]
    let Script_SpanForInDo() =
        let script = 
            """
open System

let test () =
    let span = Span([|1;2;3;4|])
    let result = ResizeArray()
    for item in span do
        result.Add(item)
    result.ToArray()

test ()
            """
        
        CompilerAssert.RunScript script ""
#endif