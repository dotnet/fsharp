
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
.assembly CCtorDUWithMember03a
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CCtorDUWithMember03a
{
  // Offset: 0x00000000 Length: 0x000002D3
  // WARNING: managed resource file FSharpSignatureData.CCtorDUWithMember03a created
}
.mresource public FSharpOptimizationData.CCtorDUWithMember03a
{
  // Offset: 0x000002D8 Length: 0x000000B3
  // WARNING: managed resource file FSharpOptimizationData.CCtorDUWithMember03a created
}
.module CCtorDUWithMember03a.exe
// MVID: {62441EFB-10BD-0BE8-A745-0383FB1E4462}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05470000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CCtorDUWithMember03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class CCtorDUWithMember03

.class private abstract auto ansi sealed '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $CCtorDUWithMember03::main@

} // end of class '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03

.class public abstract auto ansi sealed CCtorDUWithMember03a
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int32 
          get_x() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03a::x@3
    IL_0005:  ret
  } // end of method CCtorDUWithMember03a::get_x

  .method public specialname static void 
          set_x(int32 'value') cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03a::x@3
    IL_0006:  ret
  } // end of method CCtorDUWithMember03a::set_x

  .property int32 x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void CCtorDUWithMember03a::set_x(int32)
    .get int32 CCtorDUWithMember03a::get_x()
  } // end of property CCtorDUWithMember03a::x
} // end of class CCtorDUWithMember03a

.class private abstract auto ansi sealed '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03a
       extends [mscorlib]System.Object
{
  .field static assembly int32 x@3
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  stsfld     int32 '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03a::x@3
    IL_0006:  ret
  } // end of method $CCtorDUWithMember03a::.cctor

} // end of class '<StartupCode$CCtorDUWithMember03a>'.$CCtorDUWithMember03a


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\CCtorDUWithMember\CCtorDUWithMember03a_fs\CCtorDUWithMember03a.res
