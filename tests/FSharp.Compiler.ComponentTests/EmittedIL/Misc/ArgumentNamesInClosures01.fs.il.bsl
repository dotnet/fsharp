
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
.assembly ArgumentNamesInClosures01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ArgumentNamesInClosures01
{
  // Offset: 0x00000000 Length: 0x0000041E
  // WARNING: managed resource file FSharpSignatureData.ArgumentNamesInClosures01 created
}
.mresource public FSharpOptimizationData.ArgumentNamesInClosures01
{
  // Offset: 0x00000428 Length: 0x0000010D
  // WARNING: managed resource file FSharpOptimizationData.ArgumentNamesInClosures01 created
}
.module ArgumentNamesInClosures01.exe
// MVID: {624E1220-AE5C-5BA0-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05360000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed M
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig instance int32 
            F(object o) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  3
      .locals init (class M/C V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  tail.
      IL_0005:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
      IL_000a:  ret
    } // end of method C::F

  } // end of class C

  .class auto ansi serializable nested public T
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
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
    } // end of method T::.ctor

    .method public hidebysig specialname 
            instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> 
            get_F() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  3
      .locals init (class M/T V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldsfld     class M/get_F@41 M/get_F@41::@_instance
      IL_0007:  ret
    } // end of method T::get_F

    .property instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
            F()
    {
      .get instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> M/T::get_F()
    } // end of property T::F
  } // end of class T

  .class auto ansi serializable sealed nested assembly beforefieldinit get_F@41
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
  {
    .field static assembly initonly class M/get_F@41 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>::.ctor()
      IL_0006:  ret
    } // end of method get_F@41::.ctor

    .method public strict virtual instance int32 
            Invoke(class M/C i_want_to_see_this_identifier) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       int32 M::I(class M/C)
      IL_0008:  ret
    } // end of method get_F@41::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void M/get_F@41::.ctor()
      IL_0005:  stsfld     class M/get_F@41 M/get_F@41::@_instance
      IL_000a:  ret
    } // end of method get_F@41::.cctor

  } // end of class get_F@41

  .method public static int32  I(class M/C i_want_to_see_this_identifier) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  tail.
    IL_0004:  callvirt   instance int32 M/C::F(object)
    IL_0009:  ret
  } // end of method M::I

} // end of class M

.class private abstract auto ansi sealed '<StartupCode$ArgumentNamesInClosures01>'.$M
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $M::main@

} // end of class '<StartupCode$ArgumentNamesInClosures01>'.$M


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\ArgumentNamesInClosures01_fs\ArgumentNamesInClosures01.res
