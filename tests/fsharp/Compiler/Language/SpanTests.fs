// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

[<TestFixture>]
module SpanTests =

    [<Test>]
    let Script_SpanForInDo() =
        let references = 
            ILChecker.getPackageDlls "System.Memory" "4.5.0" "netstandard2.0" [ "System.Memory.dll" ]
            |> List.fold (fun references dll -> references + "#r " + "\"\"\"" + dll + "\"\"\"\n") String.Empty
        let script = 
            references +
            """
open System

let test () =
    let span = Span([|1;2;3;4|])
    let result = ResizeArray()
    for item in span do
        result.Add(item)
    result.ToArray()

//test ()
            """
        
        CompilerAssert.RunScript script (Some [|1;2;3;4|])