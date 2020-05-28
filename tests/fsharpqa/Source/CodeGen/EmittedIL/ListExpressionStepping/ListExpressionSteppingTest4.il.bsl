
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
  .ver 4:5:0:0
}
.assembly ListExpressionSteppingTest4
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest4
{
  // Offset: 0x00000000 Length: 0x00000275
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest4
{
  // Offset: 0x00000280 Length: 0x000000AF
}
.module ListExpressionSteppingTest4.exe
// MVID: {5B9A68C1-3154-FA67-A745-0383C1689A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x018D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest4
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f3@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
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
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f3@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       190 (0xbe)
        .maxstack  6
        .locals init ([0] int32 z)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest4.fs'
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_002d

        IL_001b:  br.s       IL_0021

        IL_001d:  br.s       IL_0024

        IL_001f:  br.s       IL_0027

        .line 100001,100001 : 0,0 ''
        IL_0021:  nop
        IL_0022:  br.s       IL_0078

        .line 100001,100001 : 0,0 ''
        IL_0024:  nop
        IL_0025:  br.s       IL_00a0

        .line 100001,100001 : 0,0 ''
        IL_0027:  nop
        IL_0028:  br         IL_00b5

        .line 100001,100001 : 0,0 ''
        IL_002d:  nop
        .line 6,6 : 11,24 ''
        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.0
        IL_0030:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0035:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        .line 7,7 : 11,17 ''
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        IL_0040:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0045:  nop
        .line 8,8 : 11,24 ''
        IL_0046:  ldarg.0
        IL_0047:  ldc.i4.0
        IL_0048:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_004d:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::y
        .line 9,9 : 11,17 ''
        IL_0052:  ldarg.0
        IL_0053:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::y
        IL_0058:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_005d:  nop
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.1
        IL_0060:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        .line 10,10 : 11,19 ''
        IL_0065:  ldarg.0
        IL_0066:  ldarg.0
        IL_0067:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        IL_006c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0071:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::current
        IL_0076:  ldc.i4.1
        IL_0077:  ret

        .line 11,11 : 11,26 ''
        IL_0078:  ldarg.0
        IL_0079:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        IL_007e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0083:  ldarg.0
        IL_0084:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::y
        IL_0089:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_008e:  add
        IL_008f:  stloc.0
        IL_0090:  ldarg.0
        IL_0091:  ldc.i4.2
        IL_0092:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        .line 12,12 : 11,18 ''
        IL_0097:  ldarg.0
        IL_0098:  ldloc.0
        IL_0099:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::current
        IL_009e:  ldc.i4.1
        IL_009f:  ret

        .line 8,8 : 15,16 ''
        IL_00a0:  ldarg.0
        IL_00a1:  ldnull
        IL_00a2:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::y
        IL_00a7:  ldarg.0
        IL_00a8:  ldnull
        IL_00a9:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::x
        IL_00ae:  ldarg.0
        IL_00af:  ldc.i4.3
        IL_00b0:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        IL_00b5:  ldarg.0
        IL_00b6:  ldc.i4.0
        IL_00b7:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::current
        IL_00bc:  ldc.i4.0
        IL_00bd:  ret
      } // end of method f3@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  stfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        IL_0007:  ret
      } // end of method f3@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       56 (0x38)
        .maxstack  8
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::pc
        IL_0006:  switch     ( 
                              IL_001d,
                              IL_001f,
                              IL_0021,
                              IL_0023)
        IL_001b:  br.s       IL_0031

        IL_001d:  br.s       IL_0025

        IL_001f:  br.s       IL_0028

        IL_0021:  br.s       IL_002b

        IL_0023:  br.s       IL_002e

        .line 100001,100001 : 0,0 ''
        IL_0025:  nop
        IL_0026:  br.s       IL_0036

        .line 100001,100001 : 0,0 ''
        IL_0028:  nop
        IL_0029:  br.s       IL_0034

        .line 100001,100001 : 0,0 ''
        IL_002b:  nop
        IL_002c:  br.s       IL_0032

        .line 100001,100001 : 0,0 ''
        IL_002e:  nop
        IL_002f:  br.s       IL_0036

        .line 100001,100001 : 0,0 ''
        IL_0031:  nop
        IL_0032:  ldc.i4.0
        IL_0033:  ret

        IL_0034:  ldc.i4.0
        IL_0035:  ret

        IL_0036:  ldc.i4.0
        IL_0037:  ret
      } // end of method f3@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::current
        IL_0006:  ret
      } // end of method f3@6::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       10 (0xa)
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  ldnull
        IL_0002:  ldc.i4.0
        IL_0003:  ldc.i4.0
        IL_0004:  newobj     instance void ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                               int32,
                                                                                                               int32)
        IL_0009:  ret
      } // end of method f3@6::GetFreshEnumerator

    } // end of class f3@6

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f3() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 6,12 : 9,20 ''
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void ListExpressionSteppingTest4/ListExpressionSteppingTest4/f3@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
      IL_0009:  tail.
      IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0010:  ret
    } // end of method ListExpressionSteppingTest4::f3

  } // end of class ListExpressionSteppingTest4

} // end of class ListExpressionSteppingTest4

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest4>'.$ListExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       7 (0x7)
    .maxstack  8
    .line 14,14 : 13,17 ''
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4::f3()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $ListExpressionSteppingTest4::main@

} // end of class '<StartupCode$ListExpressionSteppingTest4>'.$ListExpressionSteppingTest4


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
