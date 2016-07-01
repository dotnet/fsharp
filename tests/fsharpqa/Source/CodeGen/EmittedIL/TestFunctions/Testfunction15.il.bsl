
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
.assembly TestFunction15
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction15
{
  // Offset: 0x00000000 Length: 0x000001EA
}
.mresource public FSharpOptimizationData.TestFunction15
{
  // Offset: 0x000001F0 Length: 0x00000072
}
.module TestFunction15.exe
// MVID: {5775BAD5-A624-4662-A745-0383D5BA7557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00980000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction15
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit TestFunction15@6
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method TestFunction15@6::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 6,6 : 35,40 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction15.fs'
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  ret
    } // end of method TestFunction15@6::Invoke

  } // end of class TestFunction15@6

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          TestFunction15(int32 inp) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  7
    .locals init ([0] int32 x)
    .line 5,5 : 5,18
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  add
    IL_0004:  stloc.0
    .line 6,6 : 5,41
    IL_0005:  newobj     instance void TestFunction15/TestFunction15@6::.ctor()
    IL_000a:  ldc.i4.1
    IL_000b:  ldc.i4.2
    IL_000c:  ldc.i4.3
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0021:  tail.
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0028:  ret
  } // end of method TestFunction15::TestFunction15

} // end of class TestFunction15

.class private abstract auto ansi sealed '<StartupCode$TestFunction15>'.$TestFunction15
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction15::main@

} // end of class '<StartupCode$TestFunction15>'.$TestFunction15


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
