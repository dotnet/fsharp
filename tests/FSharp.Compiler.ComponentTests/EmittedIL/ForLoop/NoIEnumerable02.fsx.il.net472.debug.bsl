
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
.assembly NoIEnumerable02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NoIEnumerable02
{
  // Offset: 0x00000000 Length: 0x00000214
  // WARNING: managed resource file FSharpSignatureData.NoIEnumerable02 created
}
.mresource public FSharpOptimizationData.NoIEnumerable02
{
  // Offset: 0x00000218 Length: 0x0000006C
  // WARNING: managed resource file FSharpOptimizationData.NoIEnumerable02 created
}
.module NoIEnumerable02.exe
// MVID: {624FB32B-2028-CD92-A745-03832BB34F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed M
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  loop2(int32 N) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.s   100
    IL_0002:  stloc.1
    IL_0003:  ldarg.0
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  blt.s      IL_0023

    IL_0009:  ldstr      "aaa"
    IL_000e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0013:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0018:  pop
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  bne.un.s   IL_0009

    IL_0023:  ret
  } // end of method M::loop2

} // end of class M

.class private abstract auto ansi sealed '<StartupCode$NoIEnumerable02>'.$M$fsx
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $M$fsx::main@

} // end of class '<StartupCode$NoIEnumerable02>'.$M$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\ForLoop\NoIEnumerable02_fsx\NoIEnumerable02.res
