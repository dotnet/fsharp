
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
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly SteppingMatch03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch03
{
  // Offset: 0x00000000 Length: 0x0000025D
  // WARNING: managed resource file FSharpSignatureData.SteppingMatch03 created
}
.mresource public FSharpOptimizationData.SteppingMatch03
{
  // Offset: 0x00000268 Length: 0x0000007A
  // WARNING: managed resource file FSharpOptimizationData.SteppingMatch03 created
}
.module SteppingMatch03.exe
// MVID: {624CDD14-EAA6-E13B-A745-038314DD4C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000201B96B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch03
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  funcC<a,b,c>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> n) cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c> V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c> V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c> V_4)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_000a:  brtrue.s   IL_0026

    IL_000c:  ldloc.1
    IL_000d:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c>
    IL_0012:  brtrue.s   IL_0038

    IL_0014:  ldloc.0
    IL_0015:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c>
    IL_001a:  stloc.2
    IL_001b:  ldstr      "A"
    IL_0020:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0025:  ret

    IL_0026:  ldloc.0
    IL_0027:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_002c:  stloc.3
    IL_002d:  ldstr      "B"
    IL_0032:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0037:  ret

    IL_0038:  ldloc.0
    IL_0039:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c>
    IL_003e:  stloc.s    V_4
    IL_0040:  ldstr      "C"
    IL_0045:  call       void [System.Console]System.Console::WriteLine(string)
    IL_004a:  ret
  } // end of method SteppingMatch03::funcC

} // end of class SteppingMatch03

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch03>'.$SteppingMatch03
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SteppingMatch03::main@

} // end of class '<StartupCode$SteppingMatch03>'.$SteppingMatch03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\SteppingMatch\SteppingMatch03_fs\SteppingMatch03.res
