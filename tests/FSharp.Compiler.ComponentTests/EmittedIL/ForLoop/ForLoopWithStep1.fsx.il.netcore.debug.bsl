
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
.assembly ForLoopWithStep1
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForLoopWithStep1
{
  // Offset: 0x00000000 Length: 0x00000256
  // WARNING: managed resource file FSharpSignatureData.ForLoopWithStep1 created
}
.mresource public FSharpOptimizationData.ForLoopWithStep1
{
  // Offset: 0x00000260 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.ForLoopWithStep1 created
}
.module ForLoopWithStep1.exe
// MVID: {62E35B4D-9D48-2410-A745-03834D5BE362}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000227076D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForLoopWithStep1
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  loop(int32 n,
                                   int32 step,
                                   int32 m) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    // Code size       84 (0x54)
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.2
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  brtrue.s   IL_0017

    IL_0007:  ldstr      "The step of a range cannot be zero."
    IL_000c:  ldstr      "step"
    IL_0011:  newobj     instance void [System.Runtime]System.ArgumentException::.ctor(string,
                                                                                       string)
    IL_0016:  throw

    IL_0017:  ldarg.2
    IL_0018:  stloc.0
    IL_0019:  br.s       IL_0043

    IL_001b:  ldstr      "%P()"
    IL_0020:  ldc.i4.1
    IL_0021:  newarr     [System.Runtime]System.Object
    IL_0026:  dup
    IL_0027:  ldc.i4.0
    IL_0028:  ldloc.2
    IL_0029:  box        [System.Runtime]System.Int32
    IL_002e:  stelem     [System.Runtime]System.Object
    IL_0033:  ldnull
    IL_0034:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string,
                                                                                                                                                                                                                                                                                                  object[],
                                                                                                                                                                                                                                                                                                  class [System.Runtime]System.Type[])
    IL_0039:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_003e:  pop
    IL_003f:  ldloc.2
    IL_0040:  ldloc.1
    IL_0041:  add
    IL_0042:  stloc.2
    IL_0043:  ldloc.2
    IL_0044:  ldloc.0
    IL_0045:  ble.s      IL_004b

    IL_0047:  ldloc.1
    IL_0048:  ldc.i4.0
    IL_0049:  bgt.s      IL_0053

    IL_004b:  ldloc.2
    IL_004c:  ldloc.0
    IL_004d:  bge.s      IL_001b

    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.0
    IL_0051:  bge.s      IL_001b

    IL_0053:  ret
  } // end of method ForLoopWithStep1::loop

} // end of class ForLoopWithStep1

.class private abstract auto ansi sealed '<StartupCode$ForLoopWithStep1>'.$ForLoopWithStep1$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForLoopWithStep1$fsx::main@

} // end of class '<StartupCode$ForLoopWithStep1>'.$ForLoopWithStep1$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\albert\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\ForLoop\ForLoopWithStep1_fsx\ForLoopWithStep1.res
