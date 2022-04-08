
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly Lock01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Lock01
{
  // Offset: 0x00000000 Length: 0x000001B7
  // WARNING: managed resource file FSharpSignatureData.Lock01 created
}
.mresource public FSharpOptimizationData.Lock01
{
  // Offset: 0x000001C0 Length: 0x00000064
  // WARNING: managed resource file FSharpOptimizationData.Lock01 created
}
.module Lock01.exe
// MVID: {624E2480-2BCA-B308-A745-038380244E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03610000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Lock01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static object 
          get_o() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     object '<StartupCode$Lock01>'.$Lock01::o@19
    IL_0005:  ret
  } // end of method Lock01::get_o

  .property object o()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get object Lock01::get_o()
  } // end of property Lock01::o
} // end of class Lock01

.class private abstract auto ansi sealed '<StartupCode$Lock01>'.$Lock01
       extends [mscorlib]System.Object
{
  .field static assembly object o@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init (object V_0,
             object V_1,
             bool V_2)
    IL_0000:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0005:  dup
    IL_0006:  stsfld     object '<StartupCode$Lock01>'.$Lock01::o@19
    IL_000b:  stloc.0
    IL_000c:  call       object Lock01::get_o()
    IL_0011:  stloc.1
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.2
    .try
    {
      IL_0014:  ldloc.1
      IL_0015:  ldloca.s   V_2
      IL_0017:  call       void [netstandard]System.Threading.Monitor::Enter(object,
                                                                             bool&)
      IL_001c:  leave.s    IL_0029

    }  // end .try
    finally
    {
      IL_001e:  ldloc.2
      IL_001f:  brfalse.s  IL_0028

      IL_0021:  ldloc.1
      IL_0022:  call       void [netstandard]System.Threading.Monitor::Exit(object)
      IL_0027:  endfinally
      IL_0028:  endfinally
    }  // end handler
    IL_0029:  ret
  } // end of method $Lock01::main@

} // end of class '<StartupCode$Lock01>'.$Lock01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Misc\Lock01_fs\Lock01.res
