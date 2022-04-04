
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly GenIter04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter04
{
  // Offset: 0x00000000 Length: 0x00000211
  // WARNING: managed resource file FSharpSignatureData.GenIter04 created
}
.mresource public FSharpOptimizationData.GenIter04
{
  // Offset: 0x00000218 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.GenIter04 created
}
.module GenIter04.exe
// MVID: {62455CDD-F79D-DC98-A745-0383DD5C4562}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03940000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_squaresOfOneToTenD() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$GenIter04>'.$GenIter04::squaresOfOneToTenD@4
    IL_0005:  ret
  } // end of method GenIter04::get_squaresOfOneToTenD

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          squaresOfOneToTenD()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> GenIter04::get_squaresOfOneToTenD()
  } // end of property GenIter04::squaresOfOneToTenD
} // end of class GenIter04

.class private abstract auto ansi sealed '<StartupCode$GenIter04>'.$GenIter04
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> squaresOfOneToTenD@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       90 (0x5a)
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_2,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_3,
             int32 V_4,
             class [mscorlib]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.2
    .try
    {
      IL_0010:  ldloc.2
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_0030

      IL_0018:  ldloc.2
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.s    V_4
      IL_0020:  ldloca.s   V_1
      IL_0022:  ldloc.s    V_4
      IL_0024:  ldloc.s    V_4
      IL_0026:  mul
      IL_0027:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002c:  nop
      IL_002d:  nop
      IL_002e:  br.s       IL_0010

      IL_0030:  ldnull
      IL_0031:  stloc.3
      IL_0032:  leave.s    IL_0049

    }  // end .try
    finally
    {
      IL_0034:  ldloc.2
      IL_0035:  isinst     [mscorlib]System.IDisposable
      IL_003a:  stloc.s    V_5
      IL_003c:  ldloc.s    V_5
      IL_003e:  brfalse.s  IL_0048

      IL_0040:  ldloc.s    V_5
      IL_0042:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0047:  endfinally
      IL_0048:  endfinally
    }  // end handler
    IL_0049:  ldloc.3
    IL_004a:  pop
    IL_004b:  ldloca.s   V_1
    IL_004d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0052:  dup
    IL_0053:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$GenIter04>'.$GenIter04::squaresOfOneToTenD@4
    IL_0058:  stloc.0
    IL_0059:  ret
  } // end of method $GenIter04::main@

} // end of class '<StartupCode$GenIter04>'.$GenIter04


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GeneratedIterators\GenIter04_fs\GenIter04.res
