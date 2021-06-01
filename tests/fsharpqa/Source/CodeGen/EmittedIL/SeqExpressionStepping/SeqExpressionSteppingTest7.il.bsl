
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 5:0:0:0
}
.assembly SeqExpressionSteppingTest7
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest7
{
  // Offset: 0x00000000 Length: 0x00000266
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest7
{
  // Offset: 0x00000270 Length: 0x00000098
}
.module SeqExpressionSteppingTest7.exe
// MVID: {60B68B80-2432-93C3-A745-0383808BB660}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06B60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname f@5<a>
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<!a>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public !a current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 pc,
                                 !a current) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      !0 class SeqExpressionSteppingTest7/f@5<!a>::current
      IL_000e:  ldarg.0
      IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<!a>::.ctor()
      IL_0014:  ret
    } // end of method f@5::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<!a>& next) cil managed
    {
      // Code size       105 (0x69)
      .maxstack  7
      .locals init ([0] string V_0,
               [1] !a V_1)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\SeqExpressionStepping\\SeqExpressionSteppingTest7.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_001a)
      IL_0015:  br.s       IL_001d

      .line 100001,100001 : 0,0 ''
      IL_0017:  nop
      IL_0018:  br.s       IL_0054

      .line 100001,100001 : 0,0 ''
      IL_001a:  nop
      IL_001b:  br.s       IL_0060

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      .line 5,5 : 14,36 ''
      IL_001e:  nop
      .line 5,5 : 18,24 ''
      IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
      IL_0024:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0029:  nop
      .line 5,5 : 26,30 ''
      IL_002a:  ldc.i4.1
      IL_002b:  brfalse.s  IL_0057

      .line 5,5 : 44,55 ''
      IL_002d:  ldstr      ""
      IL_0032:  stloc.0
      IL_0033:  ldarg.0
      IL_0034:  ldc.i4.1
      IL_0035:  stfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      .line 5,5 : 44,55 ''
      IL_003a:  ldarg.1
      IL_003b:  ldc.i4.0
      IL_003c:  brfalse.s  IL_0046

      IL_003e:  ldnull
      IL_003f:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<!a>
      IL_0044:  br.s       IL_004d

      IL_0046:  ldloc.0
      IL_0047:  call       class [mscorlib]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
      IL_004c:  throw

      IL_004d:  stobj      class [mscorlib]System.Collections.Generic.IEnumerable`1<!a>
      IL_0052:  ldc.i4.2
      IL_0053:  ret

      .line 100001,100001 : 0,0 ''
      IL_0054:  nop
      IL_0055:  br.s       IL_0059

      .line 5,5 : 14,36 ''
      IL_0057:  nop
      .line 100001,100001 : 0,0 ''
      IL_0058:  nop
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      !0 class SeqExpressionSteppingTest7/f@5<!a>::current
      IL_0067:  ldc.i4.0
      IL_0068:  ret
    } // end of method f@5::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.2
      IL_0002:  stfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      IL_0007:  ret
    } // end of method f@5::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       40 (0x28)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class SeqExpressionSteppingTest7/f@5<!a>::pc
      IL_0006:  switch     ( 
                            IL_0019,
                            IL_001c,
                            IL_001f)
      IL_0017:  br.s       IL_0022

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      IL_001a:  br.s       IL_0026

      .line 100001,100001 : 0,0 ''
      IL_001c:  nop
      IL_001d:  br.s       IL_0023

      .line 100001,100001 : 0,0 ''
      IL_001f:  nop
      IL_0020:  br.s       IL_0026

      .line 100001,100001 : 0,0 ''
      IL_0022:  nop
      IL_0023:  ldc.i4.0
      IL_0024:  ret

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  ldc.i4.0
      IL_0027:  ret
    } // end of method f@5::get_CheckClose

    .method public strict virtual instance !a 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class SeqExpressionSteppingTest7/f@5<!a>::current
      IL_0006:  ret
    } // end of method f@5::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!a> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  6
      .locals init (!a V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  ldloc.0
      IL_0002:  newobj     instance void class SeqExpressionSteppingTest7/f@5<!a>::.ctor(int32,
                                                                                         !0)
      IL_0007:  ret
    } // end of method f@5::GetFreshEnumerator

  } // end of class f@5

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> 
          get_r() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7::r@4
    IL_0005:  ret
  } // end of method SeqExpressionSteppingTest7::get_r

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 
          f<a>() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  4
    .locals init ([0] !!a V_0)
    .line 5,5 : 12,57 ''
    IL_0000:  ldc.i4.0
    IL_0001:  ldloc.0
    IL_0002:  newobj     instance void class SeqExpressionSteppingTest7/f@5<!!a>::.ctor(int32,
                                                                                        !0)
    IL_0007:  tail.
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<!!0>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000e:  ret
  } // end of method SeqExpressionSteppingTest7::f

  .property class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>
          r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
  } // end of property SeqExpressionSteppingTest7::r
} // end of class SeqExpressionSteppingTest7

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> r@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       103 (0x67)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> r,
             [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             [4] class [mscorlib]System.Exception V_4,
             [5] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_5)
    .line 4,4 : 1,14 ''
    IL_0000:  ldc.i4.0
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
    IL_0006:  dup
    IL_0007:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7::r@4
    IL_000c:  stloc.0
    .line 6,6 : 1,19 ''
    IL_000d:  ldstr      "res = %A"
    IL_0012:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_0017:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001c:  stloc.1
    .line 6,6 : 21,24 ''
    .try
    {
      IL_001d:  nop
      .line 6,6 : 25,29 ''
      IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> SeqExpressionSteppingTest7::f<int32>()
      IL_0023:  stloc.3
      IL_0024:  leave.s    IL_005c

      .line 6,6 : 30,34 ''
    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0026:  castclass  [mscorlib]System.Exception
      IL_002b:  stloc.s    V_4
      IL_002d:  ldloc.s    V_4
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_0034:  stloc.s    V_5
      IL_0036:  ldloc.s    V_5
      IL_0038:  brfalse.s  IL_0051

      .line 6,6 : 48,52 ''
      IL_003a:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
      IL_003f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
      IL_0044:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_004e:  stloc.3
      IL_004f:  leave.s    IL_005c

      .line 100001,100001 : 0,0 ''
      IL_0051:  rethrow
      IL_0053:  ldnull
      IL_0054:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
      IL_0059:  stloc.3
      IL_005a:  leave.s    IL_005c

      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_005c:  ldloc.3
    IL_005d:  stloc.2
    IL_005e:  ldloc.1
    IL_005f:  ldloc.2
    IL_0060:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0065:  pop
    IL_0066:  ret
  } // end of method $SeqExpressionSteppingTest7::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
