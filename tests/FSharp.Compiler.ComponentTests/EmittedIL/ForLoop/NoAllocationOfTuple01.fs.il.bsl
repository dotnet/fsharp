
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
.assembly NoAllocationOfTuple01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NoAllocationOfTuple01
{
  // Offset: 0x00000000 Length: 0x0000022C
  // WARNING: managed resource file FSharpSignatureData.NoAllocationOfTuple01 created
}
.mresource public FSharpOptimizationData.NoAllocationOfTuple01
{
  // Offset: 0x00000230 Length: 0x00000085
  // WARNING: managed resource file FSharpOptimizationData.NoAllocationOfTuple01 created
}
.module NoAllocationOfTuple01.exe
// MVID: {624FA9CC-9F21-700D-A745-0383CCA94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E90000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed NoAllocationOfTuple01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32[]  loop(int32 n) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  5
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<int32>(int32)
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.m1
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.1
    IL_000a:  stloc.3
    IL_000b:  ldarg.0
    IL_000c:  stloc.2
    IL_000d:  ldloc.2
    IL_000e:  ldloc.3
    IL_000f:  blt.s      IL_0027

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.1
    IL_0015:  ldloc.0
    IL_0016:  ldloc.1
    IL_0017:  ldloc.3
    IL_0018:  stelem     [mscorlib]System.Int32
    IL_001d:  ldloc.3
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.3
    IL_0021:  ldloc.3
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  bne.un.s   IL_0011

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method NoAllocationOfTuple01::loop

} // end of class NoAllocationOfTuple01

.class private abstract auto ansi sealed '<StartupCode$NoAllocationOfTuple01>'.$NoAllocationOfTuple01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $NoAllocationOfTuple01::main@

} // end of class '<StartupCode$NoAllocationOfTuple01>'.$NoAllocationOfTuple01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\ForLoop\NoAllocationOfTuple01_fs\NoAllocationOfTuple01.res
