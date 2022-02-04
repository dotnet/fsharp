
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
.assembly GenIter03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter03
{
  // Offset: 0x00000000 Length: 0x000001F4
}
.mresource public FSharpOptimizationData.GenIter03
{
  // Offset: 0x000001F8 Length: 0x0000007B
}
.module GenIter03.exe
// MVID: {61FD4A6D-F77C-DC98-A745-03836D4AFD61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07420000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenC() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  5
    .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
             [3] int32 x,
             [4] class [mscorlib]System.IDisposable V_4)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 28,57 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\GeneratedIterators\\GenIter03.fs'
    IL_0000:  nop
    .line 4,4 : 30,33 ''
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .line 4,4 : 36,38 ''
    .try
    {
      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_002d

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.3
      .line 4,4 : 47,55 ''
      IL_001f:  ldloca.s   V_0
      IL_0021:  ldloc.3
      IL_0022:  ldloc.3
      IL_0023:  mul
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br.s       IL_0010

      IL_002d:  ldnull
      IL_002e:  stloc.2
      IL_002f:  leave.s    IL_0046

    }  // end .try
    finally
    {
      IL_0031:  ldloc.1
      IL_0032:  isinst     [mscorlib]System.IDisposable
      IL_0037:  stloc.s    V_4
      .line 100001,100001 : 0,0 ''
      IL_0039:  ldloc.s    V_4
      IL_003b:  brfalse.s  IL_0045

      .line 100001,100001 : 0,0 ''
      IL_003d:  ldloc.s    V_4
      IL_003f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0044:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0045:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0046:  ldloc.2
    IL_0047:  pop
    IL_0048:  ldloca.s   V_0
    IL_004a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004f:  ret
  } // end of method GenIter03::squaresOfOneToTenC

} // end of class GenIter03

.class private abstract auto ansi sealed '<StartupCode$GenIter03>'.$GenIter03
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenIter03::main@

} // end of class '<StartupCode$GenIter03>'.$GenIter03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
