
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16774
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
  .ver 4:0:0:0
}
.assembly TestFunction14
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction14
{
  // Offset: 0x00000000 Length: 0x0000020E
}
.mresource public FSharpOptimizationData.TestFunction14
{
  // Offset: 0x00000218 Length: 0x00000072
}
.module TestFunction14.exe
// MVID: {4DAC30EA-A624-4587-A745-0383EA30AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000470000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction14
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit TestFunction14@5
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,int32>::.ctor()
      IL_0006:  ret
    } // end of method TestFunction14@5::.ctor

    .method public strict virtual instance int32 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> f) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 24,27 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.2
      IL_0003:  tail.
      IL_0005:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_000a:  ret
    } // end of method TestFunction14@5::Invoke

  } // end of class TestFunction14@5

  .class auto ansi serializable nested assembly beforefieldinit 'TestFunction14@5-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'TestFunction14@5-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .line 5,5 : 40,45 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  ret
    } // end of method 'TestFunction14@5-1'::Invoke

  } // end of class 'TestFunction14@5-1'

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          TestFunction14() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    .line 5,5 : 5,47 
    IL_0000:  nop
    IL_0001:  newobj     instance void TestFunction14/TestFunction14@5::.ctor()
    IL_0006:  newobj     instance void TestFunction14/'TestFunction14@5-1'::.ctor()
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::get_Empty()
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0015:  tail.
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_001c:  ret
  } // end of method TestFunction14::TestFunction14

} // end of class TestFunction14

.class private abstract auto ansi sealed '<StartupCode$TestFunction14>'.$TestFunction14
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction14::main@

} // end of class '<StartupCode$TestFunction14>'.$TestFunction14


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
