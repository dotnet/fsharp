
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
.assembly SeqExpressionSteppingTest3
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest3
{
  // Offset: 0x00000000 Length: 0x0000029C
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest3
{
  // Offset: 0x000002A0 Length: 0x000000AD
}
.module SeqExpressionSteppingTest3.exe
// MVID: {4DAC145E-2432-943F-A745-03835E14AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000580000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest3
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest3
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f2@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   int32 pc,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> current) cil managed
      {
        // Code size       28 (0x1c)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::current
        IL_0015:  ldarg.0
        IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>::.ctor()
        IL_001b:  ret
      } // end of method f2@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>& next) cil managed
      {
        // Code size       116 (0x74)
        .maxstack  6
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
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
        IL_001c:  br.s       IL_0061

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_001e:  nop
        IL_001f:  br.s       IL_006b

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0021:  nop
        .line 6,6 : 21,27 
        IL_0022:  ldarg.0
        IL_0023:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::x
        IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_002d:  ldc.i4.4
        IL_002e:  bge.s      IL_0064

        .line 7,7 : 18,24 
        IL_0030:  ldarg.0
        IL_0031:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::x
        IL_0036:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_003b:  nop
        .line 8,8 : 18,33 
        IL_003c:  ldstr      "hello"
        IL_0041:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0046:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_004b:  pop
        IL_004c:  ldarg.0
        IL_004d:  ldc.i4.1
        IL_004e:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
        .line 9,9 : 18,25 
        IL_0053:  ldarg.0
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::x
        IL_005a:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::current
        IL_005f:  ldc.i4.1
        IL_0060:  ret

        .line 100001,100001 : 0,0 
        IL_0061:  nop
        IL_0062:  br.s       IL_0022

        IL_0064:  ldarg.0
        IL_0065:  ldc.i4.2
        IL_0066:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
        IL_006b:  ldarg.0
        IL_006c:  ldnull
        IL_006d:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::current
        IL_0072:  ldc.i4.0
        IL_0073:  ret
      } // end of method f2@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       9 (0x9)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldc.i4.2
        IL_0003:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
        IL_0008:  ret
      } // end of method f2@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       46 (0x2e)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::pc
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
      } // end of method f2@6::get_CheckClose

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::current
        IL_0007:  ret
      } // end of method f2@6::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::x
        IL_0007:  ldc.i4.0
        IL_0008:  ldnull
        IL_0009:  newobj     instance void SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_000e:  ret
      } // end of method f2@6::GetFreshEnumerator

    } // end of class f2@6

    .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>> 
            f2() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  5
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x)
      .line 5,5 : 9,22 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
      IL_0007:  stloc.0
      .line 6,9 : 9,27 
      IL_0008:  ldloc.0
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0010:  ret
    } // end of method SeqExpressionSteppingTest3::f2

  } // end of class SeqExpressionSteppingTest3

} // end of class SeqExpressionSteppingTest3

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest3>'.$SeqExpressionSteppingTest3
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       13 (0xd)
    .maxstack  8
    .line 11,11 : 13,30 
    IL_0000:  nop
    IL_0001:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3::f2()
    IL_0006:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  pop
    IL_000c:  ret
  } // end of method $SeqExpressionSteppingTest3::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest3>'.$SeqExpressionSteppingTest3


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
