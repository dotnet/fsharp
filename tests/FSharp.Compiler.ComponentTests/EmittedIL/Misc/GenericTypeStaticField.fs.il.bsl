
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
.assembly GenericTypeStaticField
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenericTypeStaticField
{
  // Offset: 0x00000000 Length: 0x0000064A
  // WARNING: managed resource file FSharpSignatureData.GenericTypeStaticField created
}
.mresource public FSharpOptimizationData.GenericTypeStaticField
{
  // Offset: 0x00000650 Length: 0x00000219
  // WARNING: managed resource file FSharpOptimizationData.GenericTypeStaticField created
}
.module GenericTypeStaticField.exe
// MVID: {624F6ACD-3952-A38B-A745-0383CD6A4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00D60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenericTypeStaticField
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public Foo`1<a>
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly class GenericTypeStaticField/Foo`1<!a> theInstance
    .field static assembly int32 init@2
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } // end of method Foo`1::.ctor

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  newobj     instance void class GenericTypeStaticField/Foo`1<!a>::.ctor()
      IL_0005:  stsfld     class GenericTypeStaticField/Foo`1<!0> class GenericTypeStaticField/Foo`1<!a>::theInstance
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stsfld     int32 class GenericTypeStaticField/Foo`1<!a>::init@2
      IL_0012:  ret
    } // end of method Foo`1::.cctor

    .method public specialname static class GenericTypeStaticField/Foo`1<!a> 
            get_Instance() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 class GenericTypeStaticField/Foo`1<!a>::init@2
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     class GenericTypeStaticField/Foo`1<!0> class GenericTypeStaticField/Foo`1<!a>::theInstance
      IL_001a:  ret
    } // end of method Foo`1::get_Instance

    .property class GenericTypeStaticField/Foo`1<!a>
            Instance()
    {
      .get class GenericTypeStaticField/Foo`1<!a> GenericTypeStaticField/Foo`1::get_Instance()
    } // end of property Foo`1::Instance
  } // end of class Foo`1

  .class auto ansi serializable nested public Bar`2<a,b>
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly class GenericTypeStaticField/Bar`2<!a,!b> theInstance
    .field static assembly int32 'init@6-1'
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } // end of method Bar`2::.ctor

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  newobj     instance void class GenericTypeStaticField/Bar`2<!a,!b>::.ctor()
      IL_0005:  stsfld     class GenericTypeStaticField/Bar`2<!0,!1> class GenericTypeStaticField/Bar`2<!a,!b>::theInstance
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stsfld     int32 class GenericTypeStaticField/Bar`2<!a,!b>::'init@6-1'
      IL_0012:  ret
    } // end of method Bar`2::.cctor

    .method public specialname static class GenericTypeStaticField/Bar`2<!a,!b> 
            get_Instance() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 class GenericTypeStaticField/Bar`2<!a,!b>::'init@6-1'
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     class GenericTypeStaticField/Bar`2<!0,!1> class GenericTypeStaticField/Bar`2<!a,!b>::theInstance
      IL_001a:  ret
    } // end of method Bar`2::get_Instance

    .property class GenericTypeStaticField/Bar`2<!a,!b>
            Instance()
    {
      .get class GenericTypeStaticField/Bar`2<!a,!b> GenericTypeStaticField/Bar`2::get_Instance()
    } // end of property Bar`2::Instance
  } // end of class Bar`2

} // end of class GenericTypeStaticField

.class private abstract auto ansi sealed '<StartupCode$GenericTypeStaticField>'.$GenericTypeStaticField
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenericTypeStaticField::main@

} // end of class '<StartupCode$GenericTypeStaticField>'.$GenericTypeStaticField


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\Misc\GenericTypeStaticField_fs\GenericTypeStaticField.res
