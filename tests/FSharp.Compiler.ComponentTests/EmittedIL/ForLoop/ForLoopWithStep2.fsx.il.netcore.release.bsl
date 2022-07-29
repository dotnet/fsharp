
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly ForLoopWithStep2
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForLoopWithStep2
{
  // Offset: 0x00000000 Length: 0x00000249
  // WARNING: managed resource file FSharpSignatureData.ForLoopWithStep2 created
}
.mresource public FSharpOptimizationData.ForLoopWithStep2
{
  // Offset: 0x00000250 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.ForLoopWithStep2 created
}
.module ForLoopWithStep2.exe
// MVID: {62E352C7-65F8-C026-A745-0383C752E362}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002B014A70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForLoopWithStep2
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  loop(int32 n,
                                   int32 m) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0035

    IL_0006:  ldstr      "%P()"
    IL_000b:  ldc.i4.1
    IL_000c:  newarr     [System.Runtime]System.Object
    IL_0011:  dup
    IL_0012:  ldc.i4.0
    IL_0013:  ldloc.1
    IL_0014:  box        [System.Runtime]System.Int32
    IL_0019:  stelem     [System.Runtime]System.Object
    IL_001e:  ldnull
    IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string,
                                                                                                                                                                                                                                                                                                  object[],
                                                                                                                                                                                                                                                                                                  class [System.Runtime]System.Type[])
    IL_0024:  stloc.2
    IL_0025:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_002a:  ldloc.2
    IL_002b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [System.Runtime]System.IO.TextWriter,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0030:  pop
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.2
    IL_0033:  add
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldloc.0
    IL_0037:  ble.s      IL_0006

    IL_0039:  ret
  } // end of method ForLoopWithStep2::loop

} // end of class ForLoopWithStep2

.class private abstract auto ansi sealed '<StartupCode$ForLoopWithStep2>'.$ForLoopWithStep2$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForLoopWithStep2$fsx::main@

} // end of class '<StartupCode$ForLoopWithStep2>'.$ForLoopWithStep2$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\albert\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net6.0\tests\EmittedIL\ForLoop\ForLoopWithStep2_fsx\ForLoopWithStep2.res
