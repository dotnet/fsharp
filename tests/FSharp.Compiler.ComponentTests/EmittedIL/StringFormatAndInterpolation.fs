// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Utilities.Compiler

module ``StringFormatAndInterpolation`` =
    [<Fact>]
    let ``Interpolated string with no holes is reduced to a string or simple format when used in printf``() =
        FSharp """
module StringFormatAndInterpolation

let stringOnly () = $"no hole" 

let printed () = printf $"printed no hole"
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
IL_0000:  ldstr      "no hole"
IL_0005:  ret"""
                      """
IL_0000:  ldstr      "printed no hole"
IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormat<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
IL_000f:  pop
IL_0010:  ret"""]
