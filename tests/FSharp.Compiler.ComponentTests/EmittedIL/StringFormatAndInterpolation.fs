// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``StringFormatAndInterpolation`` =
#if !DEBUG // sensitive to debug-level code coming across from debug FSharp.Core
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
IL_000a:  stloc.0
IL_000b:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
IL_0010:  ldloc.0
IL_0011:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [runtime]System.IO.TextWriter,
                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
IL_0016:  pop
IL_0017:  ret"""]

#endif

    [<Fact>]
    let ``Interpolated string with 2 parts consisting only of strings is lowered to concat`` () =
        FSharp $"""
module StringFormatAndInterpolation

let f (s: string) = $"ab{{s}}"
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
IL_0000:  ldstr      "ab"
IL_0005:  ldarg.0
IL_0006:  call       string [runtime]System.String::Concat(string,
                                                                  string)
IL_000b:  ret"""]

    [<Fact>]
    let ``Interpolated string with 3 parts consisting only of strings is lowered to concat`` () =
        //let str = "$\"\"\"ab{\"c\"}d\"\"\""
        FSharp $"""
module StringFormatAndInterpolation

let c = "c"
let str = $"ab{{c}}d"
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
IL_0000:  ldstr      "ab"
IL_0005:  ldstr      "c"
IL_000a:  ldstr      "d"
IL_000f:  call       string [runtime]System.String::Concat(string,
                                                                  string,
                                                                  string)"""]

    [<Fact>]
    let ``Interpolated string with 4 parts consisting only of strings is lowered to concat`` () =
        let str = "$\"\"\"a{\"b\"}{\"c\"}d\"\"\""
        FSharp $"""
module StringFormatAndInterpolation

let str () = {str}
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
IL_0000:  ldstr      "a"
IL_0005:  ldstr      "b"
IL_000a:  ldstr      "c"
IL_000f:  ldstr      "d"
IL_0014:  call       string [runtime]System.String::Concat(string,
                                                                  string,
                                                                  string,
                                                                  string)
IL_0019:  ret"""]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Interpolated string with concat converts to span implicitly`` () =
        let compilation = 
                FSharp $"""
        module InterpolatedStringByefLikes
        type Foo() =

                let sb = System.Text.StringBuilder()

                member _.Bar(s: System.ReadOnlySpan<char>) = sb.Append(s) |> ignore

        let [<EntryPoint>] main _ =
            let foo = Foo()
            let foos = "foo"
            foo.Bar($"{{foos}} is bar")
            0
                """

        compilation |> compile |> shouldSucceed |> ignore

        compilation |> asExe |> compileAndRun |> shouldSucceed |> ignore

        compilation |> compile |> shouldSucceed |> verifyIL ["""
.locals init (class InterpolatedStringByefLikes/Foo V_0,
        valuetype [runtime]System.ReadOnlySpan`1<char> V_1,
        class [runtime]System.Text.StringBuilder V_2)
IL_0000:  newobj     instance void InterpolatedStringByefLikes/Foo::.ctor()
IL_0005:  stloc.0
IL_0006:  ldstr      "foo"
IL_000b:  ldstr      " is bar"
IL_0010:  call       string [runtime]System.String::Concat(string,
                                                                string)
IL_0015:  call       valuetype [runtime]System.ReadOnlySpan`1<char> [runtime]System.String::op_Implicit(string)
IL_001a:  stloc.1
IL_001b:  ldloc.0
IL_001c:  ldfld      class [runtime]System.Text.StringBuilder InterpolatedStringByefLikes/Foo::sb
IL_0021:  ldloc.1
IL_0022:  callvirt   instance class [runtime]System.Text.StringBuilder [runtime]System.Text.StringBuilder::Append(valuetype [runtime]System.ReadOnlySpan`1<char>)
IL_0027:  stloc.2
IL_0028:  ldc.i4.0
IL_0029:  ret"""]