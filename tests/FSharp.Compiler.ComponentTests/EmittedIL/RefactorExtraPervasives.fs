// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module RefactorExtraPervasives =

    let lazyPatternSource code = $"""
module LazyExtensionsModule
let printlazy v = match v with | {code} x -> x
let _ = printlazy (lazy "Hello, World")
"""

    [<InlineData("Lazy")>]
    [<InlineData("LazyExtensions.Lazy")>]
    [<Theory>]
    let ``LazyPattern - LazyExtensions`` (code) =
        FSharp (lazyPatternSource code)
        |> asExe
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method public static !!a  printlazy<a>(class [runtime]System.Lazy`1<!!a> v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [netstandard]System.Lazy`1<!!a>::get_Value()
    IL_0006:  ret
  } """
            """      .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class LazyExtensionsModule/clo@4 LazyExtensionsModule/clo@4::@_instance
    IL_0005:  call       class [runtime]System.Lazy`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::Create<string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_000a:  callvirt   instance !0 class [netstandard]System.Lazy`1<string>::get_Value()
    IL_000f:  pop
    IL_0010:  ret
  } """ ]
#else
            """
  .method public static !!a  printlazy<a>(class [runtime]System.Lazy`1<!!a> v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::LazyPattern<!!0>(class [runtime]System.Lazy`1<!!0>)
    IL_0008:  ret
  } """
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class LazyExtensionsModule/clo@4 LazyExtensionsModule/clo@4::@_instance
    IL_0005:  call       class [runtime]System.Lazy`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::Create<string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::LazyPattern<string>(class [runtime]System.Lazy`1<!!0>)
    IL_000f:  pop
    IL_0010:  ret
  } """ ]
#endif

    [<InlineData("ExtraTopLevelOperators.Lazy")>]
    [<Theory>]
    let ``LazyPattern - ExtraTopLevelOperators`` (code) =
        FSharp (lazyPatternSource code)
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method public static !!a  printlazy<a>(class [runtime]System.Lazy`1<!!a> v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [netstandard]System.Lazy`1<!!a>::get_Value()
    IL_0006:  ret
  } """
            """      .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class LazyExtensionsModule/clo@4 LazyExtensionsModule/clo@4::@_instance
    IL_0005:  call       class [runtime]System.Lazy`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::Create<string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_000a:  callvirt   instance !0 class [netstandard]System.Lazy`1<string>::get_Value()
    IL_000f:  pop
    IL_0010:  ret
  } """ ]
#else
            """
  .method public static !!a  printlazy<a>(class [runtime]System.Lazy`1<!!a> v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::LazyPattern<!!0>(class [runtime]System.Lazy`1<!!0>)
    IL_0008:  ret
  } """
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class LazyExtensionsModule/clo@4 LazyExtensionsModule/clo@4::@_instance
    IL_0005:  call       class [runtime]System.Lazy`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::Create<string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::LazyPattern<string>(class [runtime]System.Lazy`1<!!0>)
    IL_000f:  pop
    IL_0010:  ret
  } """ ]
#endif


    [<InlineData("let x = byte 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Operators.uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = ExtraTopLevelOperators.uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = byte 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = uint8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Operators.uint8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = ExtraTopLevelOperators.uint8 25 in y\nprintfn \"%d\" x")>]
    [<Theory>]
    let ``convert to byte unchecked``(code) =
        FSharp $"""
module OperatorsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  stsfld     uint8 OperatorsTests::x@3
    IL_0007:  ldstr      "%d"
    IL_000c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint8>::.ctor(string)
    IL_0011:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::format@1
    IL_0016:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_001b:  call       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::get_format@1()
    IL_0020:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0025:  call       uint8 OperatorsTests::get_x()
    IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_002f:  pop
    IL_0030:  ret
  } """ ]
#else
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  stsfld     uint8 OperatorsTests::x@3
    IL_0007:  ldstr      "%d"
    IL_000c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint8>::.ctor(string)
    IL_0011:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfExtensions::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0016:  call       uint8 OperatorsTests::get_x()
    IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0020:  pop
    IL_0021:  ret
  } """ ]
#endif

    [<InlineData("let x = Checked.byte 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Checked.uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Operators.Checked.uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = ExtraTopLevelOperators.Checked.uint8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Checked.byte 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Checked.uint8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Operators.Checked.uint8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = ExtraTopLevelOperators.Checked.uint8 25 in y\nprintfn \"%d\" x")>]
    [<Theory>]
    let ``convert to byte checked``(code) =
        FSharp $"""
module OperatorsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  conv.ovf.u1
    IL_0003:  stsfld     uint8 OperatorsTests::x@3
    IL_0008:  ldstr      "%d"
    IL_000d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint8>::.ctor(string)
    IL_0012:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::format@1
    IL_0017:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::get_format@1()
    IL_0021:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0026:  call       uint8 OperatorsTests::get_x()
    IL_002b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0030:  pop
    IL_0031:  ret
  } """ ]
#else
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  conv.ovf.u1
    IL_0003:  stsfld     uint8 OperatorsTests::x@3
    IL_0008:  ldstr      "%d"
    IL_000d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint8>::.ctor(string)
    IL_0012:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfExtensions::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0017:  call       uint8 OperatorsTests::get_x()
    IL_001c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0021:  pop
    IL_0022:  ret
  } 
""" ]
#endif

    [<InlineData("let x = sbyte 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Operators.int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = ExtraTopLevelOperators.int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = sbyte 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = int8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Operators.int8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = ExtraTopLevelOperators.int8 25 in y\nprintfn \"%d\" x")>]
    [<Theory>]
    let ``convert to sbyte unchecked``(code) =
        FSharp $"""
module OperatorsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  stsfld     int8 OperatorsTests::x@3
    IL_0007:  ldstr      "%d"
    IL_000c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int8>::.ctor(string)
    IL_0011:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::format@1
    IL_0016:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_001b:  call       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::get_format@1()
    IL_0020:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0025:  call       int8 OperatorsTests::get_x()
    IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_002f:  pop
    IL_0030:  ret
  } """ ]
#else
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  stsfld     int8 OperatorsTests::x@3
    IL_0007:  ldstr      "%d"
    IL_000c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int8>::.ctor(string)
    IL_0011:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfExtensions::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0016:  call       int8 OperatorsTests::get_x()
    IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0020:  pop
    IL_0021:  ret
  } """ ]
#endif

    [<InlineData("let x = Checked.sbyte 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Checked.int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = Operators.Checked.int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = ExtraTopLevelOperators.Checked.int8 25\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Checked.sbyte 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Checked.int8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = Operators.Checked.int8 25 in y\nprintfn \"%d\" x")>]
    [<InlineData("let x = let y = ExtraTopLevelOperators.Checked.int8 25 in y\nprintfn \"%d\" x")>]
    [<Theory>]
    let ``convert to sbyte checked``(code) =
        FSharp $"""
module OperatorsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  conv.ovf.i1
    IL_0003:  stsfld     int8 OperatorsTests::x@3
    IL_0008:  ldstr      "%d"
    IL_000d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int8>::.ctor(string)
    IL_0012:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::format@1
    IL_0017:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> OperatorsTests::get_format@1()
    IL_0021:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0026:  call       int8 OperatorsTests::get_x()
    IL_002b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0030:  pop
    IL_0031:  ret
  } """ ]
#else
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   25
    IL_0002:  conv.ovf.i1
    IL_0003:  stsfld     int8 OperatorsTests::x@3
    IL_0008:  ldstr      "%d"
    IL_000d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int8>::.ctor(string)
    IL_0012:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfExtensions::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0017:  call       int8 OperatorsTests::get_x()
    IL_001c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int8,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0021:  pop
    IL_0022:  ret
  } """ ]
#endif

    [<InlineData("let _x = async")>]
    [<InlineData("let _x = CommonExtensions.async")>]
    [<Theory>]
    let ``verifyDefaultAsyncImplementation - CommonExtensions``(code) =
        FSharp $"""
module AsyncTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Control.CommonExtensions::get_DefaultAsyncBuilder()
    IL_0005:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncTests::_x@3
    IL_000a:  ret
  } """]

    [<InlineData("let _x = ExtraTopLevelOperators.async")>]
    [<Theory>]
    let ``verifyDefaultAsyncImplementation - ExtraTopLevelOperators``(code) =
        FSharp $"""
module AsyncTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
    IL_0005:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncTests::_x@3
    IL_000a:  ret
  } """ ]

    [<InlineData("let _x = set [10; 20; 30; 40]")>]
    [<InlineData("let _x = CollectionExtensions.set [10; 20; 30; 40]")>]
    [<InlineData("let _x = ExtraTopLevelOperators.set [10; 20; 30; 40]")>]
    [<Theory>]
    let ``verifyDefaultsetImplementation - CollectionExtensions``(code) =
        let _expected =
#if Release
            "Collections"
#else
            if code = "let _x = ExtraTopLevelOperators.set [10; 20; 30; 40]" then
                "Core.ExtraTopLevelOperators::CreateSet<int32>"
            else
                "Collections.SetModule::OfSeq<int32>"

#endif

        FSharp $"""
module SetExtensionsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
#if Release
            """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.s   20
    IL_0004:  ldc.i4.s   30
    IL_0006:  ldc.i4.s   40
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0021:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0026:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32> SetExtensionsTests::_x@3
    IL_002b:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32>
          _x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32> SetExtensionsTests::get__x()
  }""" ]
#else
            $"""
  .method assembly specialname static void staticInitialization@() cil managed
  {{
    
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.s   20
    IL_0004:  ldc.i4.s   30
    IL_0006:  ldc.i4.s   40
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0021:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<!!0> [FSharp.Core]Microsoft.FSharp.{_expected}(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0026:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32> SetExtensionsTests::_x@3
    IL_002b:  ret
  }} 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32>
          _x()
  {{
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpSet`1<int32> SetExtensionsTests::get__x()
  }}""" ]
#endif

    [<InlineData("let _x =  array2D [ [ 10; 20 ]; [ 30; 40 ] ]")>]
    [<InlineData("let _x =  CollectionExtensions.array2D [ [ 10; 20 ]; [ 30; 40 ] ]")>]
    [<InlineData("let _x =  ExtraTopLevelOperators.array2D [ [ 10; 20 ]; [ 30; 40 ] ]")>]
    [<Theory>]
    let ``verifyDefaultarray2DImplementation - CollectionExtensions``(code) =
        let _expected =
#if Release
            "Collections.CollectionExtensions"
#else
            if code = "let _x =  ExtraTopLevelOperators.array2D [ [ 10; 20 ]; [ 30; 40 ] ]" then
                "Core.ExtraTopLevelOperators"
            else
                "Collections.CollectionExtensions"

#endif
        FSharp
            $"""
module array2DExtensionsTests
{code}"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            $"""
  .method assembly specialname static void staticInitialization@() cil managed
  {{
    
    .maxstack  6
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.s   20
    IL_0004:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  ldc.i4.s   30
    IL_0015:  ldc.i4.s   40
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0021:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0026:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::get_Empty()
    IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Cons(!0,
                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0030:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Cons(!0,
                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0035:  call       !!1[0...,0...] [FSharp.Core]Microsoft.FSharp.{_expected}::CreateArray2D<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_003a:  stsfld     int32[0...,0...] array2DExtensionsTests::_x@3
    IL_003f:  ret
  }}""" ]

