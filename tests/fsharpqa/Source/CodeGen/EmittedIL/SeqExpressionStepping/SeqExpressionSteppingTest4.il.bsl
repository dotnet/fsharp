
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
.assembly SeqExpressionSteppingTest4
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest4
{
  // Offset: 0x00000000 Length: 0x00000249
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest4
{
  // Offset: 0x00000250 Length: 0x000000AD
}
.module SeqExpressionSteppingTest4.dll
// MVID: {4D94A7DA-241E-0874-A745-0383DAA7944D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x002D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest4
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f3@4
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .field public int32 z
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
                                   int32 z,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       44 (0x2c)
        .maxstack  6
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::z
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    pc
        IL_0018:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        IL_001d:  ldarg.0
        IL_001e:  ldarg.s    current
        IL_0020:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::current
        IL_0025:  ldarg.0
        IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_002b:  ret
      } // end of method f3@4::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       210 (0xd2)
        .maxstack  7
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_0030

        IL_001b:  br.s       IL_0021

        IL_001d:  br.s       IL_0024

        IL_001f:  br.s       IL_002a

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0021:  nop
        IL_0022:  br.s       IL_007b

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0024:  nop
        IL_0025:  br         IL_00ad

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_002a:  nop
        IL_002b:  br         IL_00c9

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0030:  nop
        .line 4,4 : 15,28 
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.0
        IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0038:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        .line 5,5 : 15,21 
        IL_003d:  ldarg.0
        IL_003e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        IL_0043:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0048:  nop
        .line 6,6 : 15,28 
        IL_0049:  ldarg.0
        IL_004a:  ldc.i4.0
        IL_004b:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0050:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::y
        .line 7,7 : 15,21 
        IL_0055:  ldarg.0
        IL_0056:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::y
        IL_005b:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.1
        IL_0063:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        .line 8,8 : 15,23 
        IL_0068:  ldarg.0
        IL_0069:  ldarg.0
        IL_006a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        IL_006f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0074:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::current
        IL_0079:  ldc.i4.1
        IL_007a:  ret

        .line 9,9 : 15,30 
        IL_007b:  ldarg.0
        IL_007c:  ldarg.0
        IL_007d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        IL_0082:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0087:  ldarg.0
        IL_0088:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::y
        IL_008d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0092:  add
        IL_0093:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::z
        IL_0098:  ldarg.0
        IL_0099:  ldc.i4.2
        IL_009a:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        .line 10,10 : 15,22 
        IL_009f:  ldarg.0
        IL_00a0:  ldarg.0
        IL_00a1:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::z
        IL_00a6:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::current
        IL_00ab:  ldc.i4.1
        IL_00ac:  ret

        .line 9,9 : 19,20 
        IL_00ad:  ldarg.0
        IL_00ae:  ldc.i4.0
        IL_00af:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::z
        .line 6,6 : 19,20 
        IL_00b4:  ldarg.0
        IL_00b5:  ldnull
        IL_00b6:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::y
        IL_00bb:  ldarg.0
        IL_00bc:  ldnull
        IL_00bd:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::x
        IL_00c2:  ldarg.0
        IL_00c3:  ldc.i4.3
        IL_00c4:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        IL_00c9:  ldarg.0
        IL_00ca:  ldc.i4.0
        IL_00cb:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::current
        IL_00d0:  ldc.i4.0
        IL_00d1:  ret
      } // end of method f3@4::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       9 (0x9)
        .maxstack  6
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldc.i4.3
        IL_0003:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        IL_0008:  ret
      } // end of method f3@4::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       57 (0x39)
        .maxstack  5
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::pc
        IL_0007:  switch     ( 
                              IL_001e,
                              IL_0020,
                              IL_0022,
                              IL_0024)
        IL_001c:  br.s       IL_0032

        IL_001e:  br.s       IL_0026

        IL_0020:  br.s       IL_0029

        IL_0022:  br.s       IL_002c

        IL_0024:  br.s       IL_002f

        IL_0026:  nop
        IL_0027:  br.s       IL_0037

        IL_0029:  nop
        IL_002a:  br.s       IL_0035

        IL_002c:  nop
        IL_002d:  br.s       IL_0033

        IL_002f:  nop
        IL_0030:  br.s       IL_0037

        IL_0032:  nop
        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldc.i4.0
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } // end of method f3@4::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  5
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::current
        IL_0007:  ret
      } // end of method f3@4::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  9
        IL_0000:  nop
        IL_0001:  ldnull
        IL_0002:  ldnull
        IL_0003:  ldc.i4.0
        IL_0004:  ldc.i4.0
        IL_0005:  ldc.i4.0
        IL_0006:  newobj     instance void SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32,
                                                                                                             int32)
        IL_000b:  ret
      } // end of method f3@4::GetFreshEnumerator

    } // end of class f3@4

    .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            f3() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  7
      .line 4,10 : 9,24 
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           int32,
                                                                                                           int32)
      IL_000b:  ret
    } // end of method SeqExpressionSteppingTest4::f3

  } // end of class SeqExpressionSteppingTest4

} // end of class SeqExpressionSteppingTest4

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest4>'.$SeqExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .line 12,12 : 13,30 
    IL_0000:  nop
    IL_0001:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4::f3()
    IL_0006:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  pop
    IL_000c:  ret
  } // end of method $SeqExpressionSteppingTest4::.cctor

} // end of class '<StartupCode$SeqExpressionSteppingTest4>'.$SeqExpressionSteppingTest4


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
