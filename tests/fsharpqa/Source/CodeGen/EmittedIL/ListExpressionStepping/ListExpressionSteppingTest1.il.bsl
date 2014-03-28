
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 2:0:0:0
}
.assembly ListExpressionSteppingTest1
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest1
{
  // Offset: 0x00000000 Length: 0x0000024F
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest1
{
  // Offset: 0x00000258 Length: 0x000000AF
}
.module ListExpressionSteppingTest1.dll
// MVID: {4D94A5B6-6A43-AEBD-A745-0383B6A5944D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x001B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest1
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f0@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(int32 pc,
                                   int32 current) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  6
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::current
        IL_000e:  ldarg.0
        IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0014:  ret
      } // end of method f0@5::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       66 (0x42)
        .maxstack  6
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0017,
                              IL_0019)
        IL_0015:  br.s       IL_0021

        IL_0017:  br.s       IL_001b

        IL_0019:  br.s       IL_001e

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_001b:  nop
        IL_001c:  br.s       IL_0032

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_001e:  nop
        IL_001f:  br.s       IL_0039

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0021:  nop
        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.1
        IL_0024:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        .line 5,5 : 11,18 
        IL_0029:  ldarg.0
        IL_002a:  ldc.i4.1
        IL_002b:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::current
        IL_0030:  ldc.i4.1
        IL_0031:  ret

        IL_0032:  ldarg.0
        IL_0033:  ldc.i4.2
        IL_0034:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        IL_0039:  ldarg.0
        IL_003a:  ldc.i4.0
        IL_003b:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::current
        IL_0040:  ldc.i4.0
        IL_0041:  ret
      } // end of method f0@5::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       9 (0x9)
        .maxstack  6
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldc.i4.2
        IL_0003:  stfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        IL_0008:  ret
      } // end of method f0@5::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       46 (0x2e)
        .maxstack  5
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::pc
        IL_0007:  switch     ( 
                              IL_001a,
                              IL_001c,
                              IL_001e)
        IL_0018:  br.s       IL_0029

        IL_001a:  br.s       IL_0020

        IL_001c:  br.s       IL_0023

        IL_001e:  br.s       IL_0026

        IL_0020:  nop
        IL_0021:  br.s       IL_002c

        IL_0023:  nop
        IL_0024:  br.s       IL_002a

        IL_0026:  nop
        IL_0027:  br.s       IL_002c

        IL_0029:  nop
        IL_002a:  ldc.i4.0
        IL_002b:  ret

        IL_002c:  ldc.i4.0
        IL_002d:  ret
      } // end of method f0@5::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  5
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::current
        IL_0007:  ret
      } // end of method f0@5::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       9 (0x9)
        .maxstack  6
        IL_0000:  nop
        IL_0001:  ldc.i4.0
        IL_0002:  ldc.i4.0
        IL_0003:  newobj     instance void ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::.ctor(int32,
                                                                                                               int32)
        IL_0008:  ret
      } // end of method f0@5::GetFreshEnumerator

    } // end of class f0@5

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f0() cil managed
    {
      // Code size       16 (0x10)
      .maxstack  4
      .line 5,5 : 9,20 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void ListExpressionSteppingTest1/ListExpressionSteppingTest1/f0@5::.ctor(int32,
                                                                                                             int32)
      IL_0008:  tail.
      IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000f:  ret
    } // end of method ListExpressionSteppingTest1::f0

  } // end of class ListExpressionSteppingTest1

} // end of class ListExpressionSteppingTest1

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest1>'.$ListExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  3
    .line 6,6 : 13,17 
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest1/ListExpressionSteppingTest1::f0()
    IL_0006:  pop
    IL_0007:  ret
  } // end of method $ListExpressionSteppingTest1::.cctor

} // end of class '<StartupCode$ListExpressionSteppingTest1>'.$ListExpressionSteppingTest1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
