
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly ListExpressionSteppingTest6
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest6
{
  // Offset: 0x00000000 Length: 0x00000291
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest6
{
  // Offset: 0x00000298 Length: 0x000000BC
}
.module ListExpressionSteppingTest6.exe
// MVID: {61FD4A6D-98A2-AB14-A745-03836D4AFD61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06960000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest6
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            get_es() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6::es@5
      IL_0005:  ret
    } // end of method ListExpressionSteppingTest6::get_es

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f7() cil managed
    {
      // Code size       181 (0xb5)
      .maxstack  4
      .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
               [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
               [3] int32 x,
               [4] class [mscorlib]System.IDisposable V_4,
               [5] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_5,
               [6] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_6,
               [7] int32 V_7,
               [8] class [mscorlib]System.IDisposable V_8)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 7,12 : 9,23 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest6.fs'
      IL_0000:  nop
      .line 7,7 : 11,14 ''
      IL_0001:  nop
      .line 7,7 : 20,22 ''
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
      IL_0007:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_000c:  stloc.1
      .line 7,7 : 17,19 ''
      .try
      {
        IL_000d:  ldloc.1
        IL_000e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0013:  brfalse.s  IL_0038

        IL_0015:  ldloc.1
        IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_001b:  stloc.3
        .line 8,8 : 14,29 ''
        IL_001c:  ldstr      "hello"
        IL_0021:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0026:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_002b:  pop
        .line 9,9 : 14,21 ''
        IL_002c:  ldloca.s   V_0
        IL_002e:  ldloc.3
        IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_0034:  nop
        .line 100001,100001 : 0,0 ''
        IL_0035:  nop
        IL_0036:  br.s       IL_000d

        IL_0038:  ldnull
        IL_0039:  stloc.2
        IL_003a:  leave.s    IL_0051

      }  // end .try
      finally
      {
        IL_003c:  ldloc.1
        IL_003d:  isinst     [mscorlib]System.IDisposable
        IL_0042:  stloc.s    V_4
        .line 100001,100001 : 0,0 ''
        IL_0044:  ldloc.s    V_4
        IL_0046:  brfalse.s  IL_0050

        .line 100001,100001 : 0,0 ''
        IL_0048:  ldloc.s    V_4
        IL_004a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_004f:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_0050:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0051:  ldloc.2
      IL_0052:  pop
      .line 10,10 : 11,14 ''
      IL_0053:  nop
      .line 10,10 : 20,22 ''
      IL_0054:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
      IL_0059:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_005e:  stloc.s    V_5
      .line 10,10 : 17,19 ''
      .try
      {
        IL_0060:  ldloc.s    V_5
        IL_0062:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0067:  brfalse.s  IL_008f

        IL_0069:  ldloc.s    V_5
        IL_006b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0070:  stloc.s    V_7
        .line 11,11 : 14,31 ''
        IL_0072:  ldstr      "goodbye"
        IL_0077:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_007c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0081:  pop
        .line 12,12 : 14,21 ''
        IL_0082:  ldloca.s   V_0
        IL_0084:  ldloc.s    V_7
        IL_0086:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_008b:  nop
        .line 100001,100001 : 0,0 ''
        IL_008c:  nop
        IL_008d:  br.s       IL_0060

        IL_008f:  ldnull
        IL_0090:  stloc.s    V_6
        IL_0092:  leave.s    IL_00aa

      }  // end .try
      finally
      {
        IL_0094:  ldloc.s    V_5
        IL_0096:  isinst     [mscorlib]System.IDisposable
        IL_009b:  stloc.s    V_8
        .line 100001,100001 : 0,0 ''
        IL_009d:  ldloc.s    V_8
        IL_009f:  brfalse.s  IL_00a9

        .line 100001,100001 : 0,0 ''
        IL_00a1:  ldloc.s    V_8
        IL_00a3:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00a8:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_00a9:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_00aa:  ldloc.s    V_6
      IL_00ac:  pop
      IL_00ad:  ldloca.s   V_0
      IL_00af:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_00b4:  ret
    } // end of method ListExpressionSteppingTest6::f7

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
    } // end of property ListExpressionSteppingTest6::es
  } // end of class ListExpressionSteppingTest6

} // end of class ListExpressionSteppingTest6

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es@5
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       37 (0x25)
    .maxstack  6
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es)
    .line 5,5 : 5,21 ''
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  dup
    IL_0018:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6::es@5
    IL_001d:  stloc.0
    .line 14,14 : 13,17 ''
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::f7()
    IL_0023:  pop
    IL_0024:  ret
  } // end of method $ListExpressionSteppingTest6::main@

} // end of class '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
