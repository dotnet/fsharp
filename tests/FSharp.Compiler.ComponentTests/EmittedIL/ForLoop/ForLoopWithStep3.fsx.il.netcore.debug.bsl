
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
.assembly ForLoopWithStep3
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForLoopWithStep3
{
  // Offset: 0x00000000 Length: 0x00000264
  // WARNING: managed resource file FSharpSignatureData.ForLoopWithStep3 created
}
.mresource public FSharpOptimizationData.ForLoopWithStep3
{
  // Offset: 0x00000268 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.ForLoopWithStep3 created
}
.module ForLoopWithStep3.exe
// MVID: {62E35B4D-8E9F-8EFA-A745-03834D5BE362}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002B1E0100000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForLoopWithStep3
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  loop<a,b>(!!a n,
                                        !!b m) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       53 (0x35)
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.s   10
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.s   -2
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0030

    IL_0008:  ldstr      "%P()"
    IL_000d:  ldc.i4.1
    IL_000e:  newarr     [System.Runtime]System.Object
    IL_0013:  dup
    IL_0014:  ldc.i4.0
    IL_0015:  ldloc.1
    IL_0016:  box        [System.Runtime]System.Int32
    IL_001b:  stelem     [System.Runtime]System.Object
    IL_0020:  ldnull
    IL_0021:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string,
                                                                                                                                                                                                                                                                                                  object[],
                                                                                                                                                                                                                                                                                                  class [System.Runtime]System.Type[])
    IL_0026:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002b:  pop
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.3
    IL_002e:  sub
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  bge.s      IL_0008

    IL_0034:  ret
  } // end of method ForLoopWithStep3::loop

} // end of class ForLoopWithStep3

.class private abstract auto ansi sealed '<StartupCode$ForLoopWithStep3>'.$ForLoopWithStep3$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForLoopWithStep3$fsx::main@

} // end of class '<StartupCode$ForLoopWithStep3>'.$ForLoopWithStep3$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\albert\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\ForLoop\ForLoopWithStep3_fsx\ForLoopWithStep3.res
