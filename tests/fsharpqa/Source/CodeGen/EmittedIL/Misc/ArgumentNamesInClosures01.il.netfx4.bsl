
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:4:1:0
}
.assembly ArgumentNamesInClosures01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ArgumentNamesInClosures01
{
  // Offset: 0x00000000 Length: 0x0000039F
}
.mresource public FSharpOptimizationData.ArgumentNamesInClosures01
{
  // Offset: 0x000003A8 Length: 0x0000010D
}
.module ArgumentNamesInClosures01.dll
// MVID: {5775B160-39CA-41B5-A745-038360B17557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x008E0000


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
      // Code size       10 (0xa)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 36,36 : 29,44 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Misc\\ArgumentNamesInClosures01.fs'
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  tail.
      IL_0004:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
      IL_0009:  ret
    } // end of method C::F

  } // end of class C

  .class auto ansi serializable nested public T
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 40,40 : 10,11
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  nop
      IL_0009:  ret
    } // end of method T::.ctor

    .method public hidebysig specialname 
            instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> 
            get_F() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      .line 41,41 : 22,23
      IL_0000:  nop
      IL_0001:  newobj     instance void M/get_F@41::.ctor()
      IL_0006:  ret
    } // end of method T::get_F

    .property instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
            F()
    {
      .get instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> M/T::get_F()
    } // end of property T::F
  } // end of class T

  .class auto ansi serializable nested assembly beforefieldinit get_F@41
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
  {
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
      // Code size       10 (0xa)
      .maxstack  8
      .line 41,41 : 22,23
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       int32 M::I(class M/C)
      IL_0009:  ret
    } // end of method get_F@41::Invoke

  } // end of class get_F@41

  .method public static int32  I(class M/C i_want_to_see_this_identifier) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    .line 38,38 : 47,80
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  tail.
    IL_0005:  callvirt   instance int32 M/C::F(object)
    IL_000a:  ret
  } // end of method M::I

} // end of class M

.class private abstract auto ansi sealed '<StartupCode$ArgumentNamesInClosures01>'.$M
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ArgumentNamesInClosures01>'.$M


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
