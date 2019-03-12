// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

#if NETCOREAPP
[<TestFixture>]
module SpanTests =

    [<Test>]
    let Script_SpanForInDo() =
        let script = 
            """
open System

let test () : unit =
    let span = Span([|1;2;3;4|])
    let result = ResizeArray()
    for item in span do
        result.Add(item)
    
    if result.[0] <> 1 || result.[1] <> 2 || result.[2] <> 3 || result.[3] <> 4 then
        failwith "SpanForInDo didn't work properly"

test ()
            """
        
        CompilerAssert.RunScript script
#endif