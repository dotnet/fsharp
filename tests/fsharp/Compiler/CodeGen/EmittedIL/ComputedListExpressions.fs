// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open NUnit.Framework

[<TestFixture>]
module ``ComputedListExpressions`` =

    [<Test>]
    let ``ComputedListExpression01``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression01
let ListExpressionSteppingTest1 () = [ yield 1 ]
            """
            (fun verifier -> verifier.VerifyIL [
            """
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
        ListExpressionSteppingTest1() cil managed
{
  
  .maxstack  4
  .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0)
  IL_0000:  ldloca.s   V_0
  IL_0002:  ldc.i4.1
  IL_0003:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
  IL_0008:  nop
  IL_0009:  ldloca.s   V_0
  IL_000b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
  IL_0010:  ret
} 
            """
            ])

    [<Test>]
    let ``ComputedListExpression02``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression02
let ListExpressionSteppingTest2 () = 
    [ printfn "hello"
      yield 1
      printfn "goodbye"
      yield 2]
        """
            (fun verifier -> verifier.VerifyIL [
        """
      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
              ListExpressionSteppingTest2() cil managed
      {
        
        .maxstack  4
        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0)
        IL_0000:  nop
        IL_0001:  ldstr      "hello"
        IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0010:  pop
        IL_0011:  ldloca.s   V_0
        IL_0013:  ldc.i4.1
        IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_0019:  nop
        IL_001a:  ldstr      "goodbye"
        IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0029:  pop
        IL_002a:  ldloca.s   V_0
        IL_002c:  ldc.i4.2
        IL_002d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_0032:  nop
        IL_0033:  ldloca.s   V_0
        IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
        IL_003a:  ret
      } 
        """
            ])

    [<Test>]
    let ``ComputedListExpression03``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression03
let ListExpressionSteppingTest3 () = 
    let x = ref 0 
    [ while !x < 4 do 
            incr x
            printfn "hello"
            yield x ]
        """
            (fun verifier -> verifier.VerifyIL [
        """
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>> 
        ListExpressionSteppingTest3() cil managed
{
  
  .maxstack  4
  .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
           valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>> V_1)
  IL_0000:  ldc.i4.0
  IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
  IL_0006:  stloc.0
  IL_0007:  nop
  IL_0008:  ldloc.0
  IL_0009:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
  IL_000e:  ldc.i4.4
  IL_000f:  bge.s      IL_0034
    
  IL_0011:  ldloc.0
  IL_0012:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
  IL_0017:  nop
  IL_0018:  ldstr      "hello"
  IL_001d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
  IL_0022:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
  IL_0027:  pop
  IL_0028:  ldloca.s   V_1
  IL_002a:  ldloc.0
  IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>::Add(!0)
  IL_0030:  nop
  IL_0031:  nop
  IL_0032:  br.s       IL_0007
    
  IL_0034:  ldloca.s   V_1
  IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>::Close()
  IL_003b:  ret
} 
        """
            ])

    [<Test>]
    let ``ComputedListExpression04``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression04
let ListExpressionSteppingTest4 () = 
    [ let x = ref 0 
      try 
            let y = ref 0 
            incr y
            yield !x
            let z = !x + !y
            yield z 
      finally 
            incr x
            printfn "done" ]
        """
            (fun verifier -> verifier.VerifyIL [
        """
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
        ListExpressionSteppingTest4() cil managed
{
  
  .maxstack  4
  .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
           class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_3,
           int32 V_4)
  IL_0000:  nop
  IL_0001:  ldc.i4.0
  IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
  IL_0007:  stloc.1
  .try
  {
    IL_0008:  nop
    IL_0009:  ldc.i4.0
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
    IL_000f:  stloc.3
    IL_0010:  ldloc.3
    IL_0011:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
    IL_0016:  nop
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldloc.1
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
    IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloc.1
    IL_0026:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
    IL_002b:  ldloc.3
    IL_002c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
    IL_0031:  add
    IL_0032:  stloc.s    V_4
    IL_0034:  ldloca.s   V_0
    IL_0036:  ldloc.s    V_4
    IL_0038:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_003d:  nop
    IL_003e:  ldnull
    IL_003f:  stloc.2
    IL_0040:  leave.s    IL_005b
    
  }  
  finally
  {
    IL_0042:  nop
    IL_0043:  ldloc.1
    IL_0044:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
    IL_0049:  nop
    IL_004a:  ldstr      "done"
    IL_004f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0054:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0059:  pop
    IL_005a:  endfinally
  }  
  IL_005b:  ldloc.2
  IL_005c:  pop
  IL_005d:  ldloca.s   V_0
  IL_005f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
  IL_0064:  ret
} 
    
        """
            ])

    [<Test>]
    let ``ComputedListExpression05``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression05
let ListExpressionSteppingTest5 () = 
    [ for x in 1..4 do
            printfn "hello"
            yield x ]
        """
            (fun verifier -> verifier.VerifyIL [
        """
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
         ListExpressionSteppingTest5() cil managed
 {
   
   .maxstack  5
   .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
            class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
            class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
            int32 V_3,
            class [runtime]System.IDisposable V_4)
   IL_0000:  ldc.i4.1
   IL_0001:  ldc.i4.1
   IL_0002:  ldc.i4.4
   IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                          int32,
                                                                                                                                                                          int32)
   IL_0008:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
   IL_000d:  stloc.1
   .try
   {
     IL_000e:  ldloc.1
     IL_000f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
     IL_0014:  brfalse.s  IL_0039

     IL_0016:  ldloc.1
     IL_0017:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
     IL_001c:  stloc.3
     IL_001d:  ldstr      "hello"
     IL_0022:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
     IL_0027:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
     IL_002c:  pop
     IL_002d:  ldloca.s   V_0
     IL_002f:  ldloc.3
     IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
     IL_0035:  nop
     IL_0036:  nop
     IL_0037:  br.s       IL_000e

     IL_0039:  ldnull
     IL_003a:  stloc.2
     IL_003b:  leave.s    IL_0052

   }  
   finally
   {
     IL_003d:  ldloc.1
     IL_003e:  isinst     [runtime]System.IDisposable
     IL_0043:  stloc.s    V_4
     IL_0045:  ldloc.s    V_4
     IL_0047:  brfalse.s  IL_0051

     IL_0049:  ldloc.s    V_4
     IL_004b:  callvirt   instance void [runtime]System.IDisposable::Dispose()
     IL_0050:  endfinally
     IL_0051:  endfinally
   }  
   IL_0052:  ldloc.2
   IL_0053:  pop
   IL_0054:  ldloca.s   V_0
   IL_0056:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
   IL_005b:  ret
 } 

} 
        """
            ])

    [<Test>]
    let ``ComputedListExpression06``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"|]
            """
module ComputedListExpression06
let ListExpressionSteppingTest6 () = 
    [ for x in 1..4 do
        match x with 
        | 1 -> 
            printfn "hello"
            yield x 
        | 2 -> 
            printfn "hello"
            yield x 
        | _ -> 
            yield x 
        ]
        """
            (fun verifier -> verifier.VerifyIL [
        """
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
        ListExpressionSteppingTest6() cil managed
{
  
  .maxstack  5
  .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
           class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
           class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
           int32 V_3,
           class [runtime]System.IDisposable V_4)
  IL_0000:  ldc.i4.1
  IL_0001:  ldc.i4.1
  IL_0002:  ldc.i4.4
  IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                         int32,
                                                                                                                                                                         int32)
  IL_0008:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
  IL_000d:  stloc.1
  .try
  {
    IL_000e:  ldloc.1
    IL_000f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
    IL_0014:  brfalse.s  IL_0074
    
    IL_0016:  ldloc.1
    IL_0017:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
    IL_001c:  stloc.3
    IL_001d:  nop
    IL_001e:  ldloc.3
    IL_001f:  ldc.i4.1
    IL_0020:  sub
    IL_0021:  switch     ( 
                          IL_0030,
                          IL_004c)
    IL_002e:  br.s       IL_0068
    
    IL_0030:  ldstr      "hello"
    IL_0035:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_003a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_003f:  pop
    IL_0040:  ldloca.s   V_0
    IL_0042:  ldloc.3
    IL_0043:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0048:  nop
    IL_0049:  nop
    IL_004a:  br.s       IL_000e
    
    IL_004c:  ldstr      "hello"
    IL_0051:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0056:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_005b:  pop
    IL_005c:  ldloca.s   V_0
    IL_005e:  ldloc.3
    IL_005f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0064:  nop
    IL_0065:  nop
    IL_0066:  br.s       IL_000e
    
    IL_0068:  ldloca.s   V_0
    IL_006a:  ldloc.3
    IL_006b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  br.s       IL_000e
    
    IL_0074:  ldnull
    IL_0075:  stloc.2
    IL_0076:  leave.s    IL_008d
    
  }  
  finally
  {
    IL_0078:  ldloc.1
    IL_0079:  isinst     [runtime]System.IDisposable
    IL_007e:  stloc.s    V_4
    IL_0080:  ldloc.s    V_4
    IL_0082:  brfalse.s  IL_008c
    
    IL_0084:  ldloc.s    V_4
    IL_0086:  callvirt   instance void [runtime]System.IDisposable::Dispose()
    IL_008b:  endfinally
    IL_008c:  endfinally
  }  
  IL_008d:  ldloc.2
  IL_008e:  pop
  IL_008f:  ldloca.s   V_0
  IL_0091:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
  IL_0096:  ret
} 
        """
            ])
