
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
.assembly GenIter03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter03
{
  // Offset: 0x00000000 Length: 0x00000233
  // WARNING: managed resource file FSharpSignatureData.GenIter03 created
}
.mresource public FSharpOptimizationData.GenIter03
{
  // Offset: 0x00000238 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.GenIter03 created
}
.module GenIter03.exe
// MVID: {624FDC53-C611-6A3D-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001FC7B680000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter03
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenC() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [System.Runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_0024

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldloca.s   V_0
      IL_001b:  ldloc.3
      IL_001c:  ldloc.3
      IL_001d:  mul
      IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0023:  nop
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_002a:  brtrue.s   IL_0012

      IL_002c:  ldnull
      IL_002d:  stloc.2
      IL_002e:  leave.s    IL_0045

    }  // end .try
    finally
    {
      IL_0030:  ldloc.1
      IL_0031:  isinst     [System.Runtime]System.IDisposable
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.s    V_4
      IL_003a:  brfalse.s  IL_0044

      IL_003c:  ldloc.s    V_4
      IL_003e:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
      IL_0043:  endfinally
      IL_0044:  endfinally
    }  // end handler
    IL_0045:  ldloc.2
    IL_0046:  pop
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004e:  ret
  } // end of method GenIter03::squaresOfOneToTenC

} // end of class GenIter03

.class private abstract auto ansi sealed '<StartupCode$GenIter03>'.$GenIter03
       extends [System.Runtime]System.Object
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
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GeneratedIterators\GenIter03_fs\GenIter03.res
