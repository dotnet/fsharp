
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
.assembly StaticInit_Class01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.StaticInit_Class01
{
  // Offset: 0x00000000 Length: 0x0000036D
  // WARNING: managed resource file FSharpSignatureData.StaticInit_Class01 created
}
.mresource public FSharpOptimizationData.StaticInit_Class01
{
  // Offset: 0x00000378 Length: 0x000000AD
  // WARNING: managed resource file FSharpOptimizationData.StaticInit_Class01 created
}
.module StaticInit_Class01.exe
// MVID: {624CC9CC-EC48-71DA-A745-0383CCC94C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x034B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed StaticInit_ClassS01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly int32 x
    .field static assembly int32 init@4
    .method public specialname rtspecialname 
            instance void  .ctor(valuetype [mscorlib]System.DateTime s) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } // end of method C::.ctor

    .method assembly static int32  f() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 StaticInit_ClassS01/C::init@4
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     int32 StaticInit_ClassS01/C::x
      IL_001a:  ldstr      "2"
      IL_001f:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0024:  add
      IL_0025:  ret
    } // end of method C::f

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$StaticInit_Class01>'.$StaticInit_ClassS01::init@
      IL_0006:  ldsfld     int32 '<StartupCode$StaticInit_Class01>'.$StaticInit_ClassS01::init@
      IL_000b:  pop
      IL_000c:  ret
    } // end of method C::.cctor

  } // end of class C

} // end of class StaticInit_ClassS01

.class private abstract auto ansi sealed '<StartupCode$StaticInit_Class01>'.$StaticInit_ClassS01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldstr      "1"
    IL_0005:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000a:  stsfld     int32 StaticInit_ClassS01/C::x
    IL_000f:  ldc.i4.1
    IL_0010:  volatile.
    IL_0012:  stsfld     int32 StaticInit_ClassS01/C::init@4
    IL_0017:  ret
  } // end of method $StaticInit_ClassS01::main@

} // end of class '<StartupCode$StaticInit_Class01>'.$StaticInit_ClassS01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\StaticInit\StaticInit_Class01_fs\StaticInit_Class01.res
