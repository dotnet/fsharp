
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly GenIter01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter01
{
  // Offset: 0x00000000 Length: 0x00000232
  // WARNING: managed resource file FSharpSignatureData.GenIter01 created
}
.mresource public FSharpOptimizationData.GenIter01
{
  // Offset: 0x00000238 Length: 0x0000007A
  // WARNING: managed resource file FSharpOptimizationData.GenIter01 created
}
.module GenIter01.exe
// MVID: {624FDC53-3C0A-FF42-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000020E5F660000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTen() cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [System.Runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0009:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000e:  stloc.1
    .try
    {
      IL_000f:  br.s       IL_0023

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0017:  stloc.3
      IL_0018:  ldloca.s   V_0
      IL_001a:  ldloc.3
      IL_001b:  ldloc.3
      IL_001c:  mul
      IL_001d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0022:  nop
      IL_0023:  ldloc.1
      IL_0024:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0029:  brtrue.s   IL_0011

      IL_002b:  ldnull
      IL_002c:  stloc.2
      IL_002d:  leave.s    IL_0044

    }  // end .try
    finally
    {
      IL_002f:  ldloc.1
      IL_0030:  isinst     [System.Runtime]System.IDisposable
      IL_0035:  stloc.s    V_4
      IL_0037:  ldloc.s    V_4
      IL_0039:  brfalse.s  IL_0043

      IL_003b:  ldloc.s    V_4
      IL_003d:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
      IL_0042:  endfinally
      IL_0043:  endfinally
    }  // end handler
    IL_0044:  ldloc.2
    IL_0045:  pop
    IL_0046:  ldloca.s   V_0
    IL_0048:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004d:  ret
  } // end of method GenIter01::squaresOfOneToTen

} // end of class GenIter01

.class private abstract auto ansi sealed '<StartupCode$GenIter01>'.$GenIter01
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenIter01::main@

} // end of class '<StartupCode$GenIter01>'.$GenIter01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GeneratedIterators\GenIter01_fs\GenIter01.res
