
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
.assembly EntryPoint01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.EntryPoint01
{
  // Offset: 0x00000000 Length: 0x000002C5
  // WARNING: managed resource file FSharpSignatureData.EntryPoint01 created
}
.mresource public FSharpOptimizationData.EntryPoint01
{
  // Offset: 0x000002D0 Length: 0x00000090
  // WARNING: managed resource file FSharpOptimizationData.EntryPoint01 created
}
.module EntryPoint01.exe
// MVID: {624E1220-9846-72C1-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03740000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed EntryPoint01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int32 
          get_static_initializer() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  ret
  } // end of method EntryPoint01::get_static_initializer

  .method public static int32  main(string[] argsz) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$EntryPoint01>'.$EntryPoint01::init@
    IL_0006:  ldsfld     int32 '<StartupCode$EntryPoint01>'.$EntryPoint01::init@
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  nop
    IL_000e:  call       int32 EntryPoint01::get_static_initializer()
    IL_0013:  ldc.i4.s   10
    IL_0015:  bne.un.s   IL_001b

    IL_0017:  ldc.i4.0
    IL_0018:  nop
    IL_0019:  br.s       IL_001d

    IL_001b:  ldc.i4.1
    IL_001c:  nop
    IL_001d:  tail.
    IL_001f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<int32>(int32)
    IL_0024:  ret
  } // end of method EntryPoint01::main

  .property int32 static_initializer()
  {
    .get int32 EntryPoint01::get_static_initializer()
  } // end of property EntryPoint01::static_initializer
} // end of class EntryPoint01

.class private abstract auto ansi sealed '<StartupCode$EntryPoint01>'.$EntryPoint01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  call       int32 EntryPoint01::get_static_initializer()
    IL_0005:  stloc.0
    IL_0006:  ret
  } // end of method $EntryPoint01::.cctor

} // end of class '<StartupCode$EntryPoint01>'.$EntryPoint01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\EntryPoint01_fs\EntryPoint01.res
