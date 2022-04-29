
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
.assembly LetIfThenElse01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.LetIfThenElse01
{
  // Offset: 0x00000000 Length: 0x00000252
  // WARNING: managed resource file FSharpSignatureData.LetIfThenElse01 created
}
.mresource public FSharpOptimizationData.LetIfThenElse01
{
  // Offset: 0x00000258 Length: 0x00000076
  // WARNING: managed resource file FSharpOptimizationData.LetIfThenElse01 created
}
.module LetIfThenElse01.exe
// MVID: {624E1220-BE5A-D8FD-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02C10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed LetIfThenElse01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [mscorlib]System.Tuple`4<int32,int32,int32,int32> 
          F<a>(!!a y) cil managed
  {
    // Code size       128 (0x80)
    .maxstack  6
    .locals init (int32 V_0,
             valuetype [mscorlib]System.DateTime V_1,
             int32 V_2,
             valuetype [mscorlib]System.DateTime V_3,
             int32 V_4,
             valuetype [mscorlib]System.DateTime V_5,
             int32 V_6,
             valuetype [mscorlib]System.DateTime V_7)
    IL_0000:  nop
    IL_0001:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_1
    IL_0009:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_000e:  ldc.i4     0x7d0
    IL_0013:  ble.s      IL_0019

    IL_0015:  ldc.i4.1
    IL_0016:  nop
    IL_0017:  br.s       IL_001b

    IL_0019:  ldc.i4.2
    IL_001a:  nop
    IL_001b:  stloc.0
    IL_001c:  nop
    IL_001d:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0022:  stloc.3
    IL_0023:  ldloca.s   V_3
    IL_0025:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_002a:  ldc.i4     0x7d0
    IL_002f:  ble.s      IL_0035

    IL_0031:  ldc.i4.1
    IL_0032:  nop
    IL_0033:  br.s       IL_0037

    IL_0035:  ldc.i4.2
    IL_0036:  nop
    IL_0037:  stloc.2
    IL_0038:  nop
    IL_0039:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_003e:  stloc.s    V_5
    IL_0040:  ldloca.s   V_5
    IL_0042:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_0047:  ldc.i4     0x7d0
    IL_004c:  bge.s      IL_0052

    IL_004e:  ldc.i4.1
    IL_004f:  nop
    IL_0050:  br.s       IL_0054

    IL_0052:  ldc.i4.2
    IL_0053:  nop
    IL_0054:  stloc.s    V_4
    IL_0056:  nop
    IL_0057:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_005c:  stloc.s    V_7
    IL_005e:  ldloca.s   V_7
    IL_0060:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_0065:  ldc.i4     0x7d0
    IL_006a:  bge.s      IL_0070

    IL_006c:  ldc.i4.1
    IL_006d:  nop
    IL_006e:  br.s       IL_0072

    IL_0070:  ldc.i4.2
    IL_0071:  nop
    IL_0072:  stloc.s    V_6
    IL_0074:  ldloc.0
    IL_0075:  ldloc.2
    IL_0076:  ldloc.s    V_4
    IL_0078:  ldloc.s    V_6
    IL_007a:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_007f:  ret
  } // end of method LetIfThenElse01::F

} // end of class LetIfThenElse01

.class private abstract auto ansi sealed '<StartupCode$LetIfThenElse01>'.$LetIfThenElse01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       10 (0xa)
    .maxstack  3
    .locals init (class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_0,
             class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [mscorlib]System.Tuple`4<int32,int32,int32,int32> LetIfThenElse01::F<int32>(!!0)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ret
  } // end of method $LetIfThenElse01::main@

} // end of class '<StartupCode$LetIfThenElse01>'.$LetIfThenElse01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\LetIfThenElse01_fs\LetIfThenElse01.res
