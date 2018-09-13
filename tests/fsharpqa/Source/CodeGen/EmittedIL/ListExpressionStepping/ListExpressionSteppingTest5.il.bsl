
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
.assembly ListExpressionSteppingTest5
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest5
{
  // Offset: 0x00000000 Length: 0x00000275
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest5
{
  // Offset: 0x00000280 Length: 0x000000AF
}
.module ListExpressionSteppingTest5.exe
// MVID: {5B9A6329-CBE3-BFEA-A745-038329639A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x027C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest5
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest5
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f4@6
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
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f4@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       232 (0xe8)
        .maxstack  6
        .locals init ([0] int32 z)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest5.fs'
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001f,
                              IL_0021,
                              IL_0023,
                              IL_0025)
        IL_001d:  br.s       IL_0039

        IL_001f:  br.s       IL_0027

        IL_0021:  br.s       IL_002d

        IL_0023:  br.s       IL_0030

        IL_0025:  br.s       IL_0033

        .line 100001,100001 : 0,0 ''
        IL_0027:  nop
        IL_0028:  br         IL_00ae

        .line 100001,100001 : 0,0 ''
        IL_002d:  nop
        IL_002e:  br.s       IL_007f

        .line 100001,100001 : 0,0 ''
        IL_0030:  nop
        IL_0031:  br.s       IL_00a7

        .line 100001,100001 : 0,0 ''
        IL_0033:  nop
        IL_0034:  br         IL_00df

        .line 100001,100001 : 0,0 ''
        IL_0039:  nop
        .line 6,6 : 11,24 ''
        IL_003a:  ldarg.0
        IL_003b:  ldc.i4.0
        IL_003c:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0041:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_0046:  ldarg.0
        IL_0047:  ldc.i4.1
        IL_0048:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        .line 8,8 : 15,28 ''
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.0
        IL_004f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0054:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::y
        .line 9,9 : 15,21 ''
        IL_0059:  ldarg.0
        IL_005a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::y
        IL_005f:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0064:  nop
        IL_0065:  ldarg.0
        IL_0066:  ldc.i4.2
        IL_0067:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        .line 10,10 : 15,23 ''
        IL_006c:  ldarg.0
        IL_006d:  ldarg.0
        IL_006e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_0073:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0078:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
        IL_007d:  ldc.i4.1
        IL_007e:  ret

        .line 11,11 : 15,30 ''
        IL_007f:  ldarg.0
        IL_0080:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_0085:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_008a:  ldarg.0
        IL_008b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::y
        IL_0090:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0095:  add
        IL_0096:  stloc.0
        IL_0097:  ldarg.0
        IL_0098:  ldc.i4.3
        IL_0099:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        .line 12,12 : 15,22 ''
        IL_009e:  ldarg.0
        IL_009f:  ldloc.0
        IL_00a0:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
        IL_00a5:  ldc.i4.1
        IL_00a6:  ret

        IL_00a7:  ldarg.0
        IL_00a8:  ldnull
        IL_00a9:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::y
        IL_00ae:  ldarg.0
        IL_00af:  ldc.i4.4
        IL_00b0:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        .line 14,14 : 14,20 ''
        IL_00b5:  ldarg.0
        IL_00b6:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_00bb:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_00c0:  nop
        .line 15,15 : 14,28 ''
        IL_00c1:  ldstr      "done"
        IL_00c6:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00cb:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00d0:  pop
        IL_00d1:  ldarg.0
        IL_00d2:  ldnull
        IL_00d3:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
        IL_00d8:  ldarg.0
        IL_00d9:  ldc.i4.4
        IL_00da:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        IL_00df:  ldarg.0
        IL_00e0:  ldc.i4.0
        IL_00e1:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
        IL_00e6:  ldc.i4.0
        IL_00e7:  ret
      } // end of method f4@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       176 (0xb0)
        .maxstack  6
        .locals init ([0] class [mscorlib]System.Exception V_0,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
                 [2] class [mscorlib]System.Exception e)
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.4
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0019

        .line 100001,100001 : 0,0 ''
        IL_0013:  nop
        IL_0014:  br         IL_00a3

        .line 100001,100001 : 0,0 ''
        IL_0019:  nop
        .try
        {
          IL_001a:  ldarg.0
          IL_001b:  ldfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
          IL_0020:  switch     ( 
                                IL_003b,
                                IL_003d,
                                IL_003f,
                                IL_0041,
                                IL_0043)
          IL_0039:  br.s       IL_0054

          IL_003b:  br.s       IL_0045

          IL_003d:  br.s       IL_0048

          IL_003f:  br.s       IL_004b

          IL_0041:  br.s       IL_004e

          IL_0043:  br.s       IL_0051

          .line 100001,100001 : 0,0 ''
          IL_0045:  nop
          IL_0046:  br.s       IL_007d

          .line 100001,100001 : 0,0 ''
          IL_0048:  nop
          IL_0049:  br.s       IL_0059

          .line 100001,100001 : 0,0 ''
          IL_004b:  nop
          IL_004c:  br.s       IL_0058

          .line 100001,100001 : 0,0 ''
          IL_004e:  nop
          IL_004f:  br.s       IL_0055

          .line 100001,100001 : 0,0 ''
          IL_0051:  nop
          IL_0052:  br.s       IL_007d

          .line 100001,100001 : 0,0 ''
          IL_0054:  nop
          .line 100001,100001 : 0,0 ''
          IL_0055:  nop
          IL_0056:  br.s       IL_0059

          .line 100001,100001 : 0,0 ''
          IL_0058:  nop
          IL_0059:  ldarg.0
          IL_005a:  ldc.i4.4
          IL_005b:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
          .line 14,14 : 14,20 ''
          IL_0060:  ldarg.0
          IL_0061:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::x
          IL_0066:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
          IL_006b:  nop
          .line 15,15 : 14,28 ''
          IL_006c:  ldstr      "done"
          IL_0071:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
          IL_0076:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
          IL_007b:  pop
          .line 100001,100001 : 0,0 ''
          IL_007c:  nop
          IL_007d:  ldarg.0
          IL_007e:  ldc.i4.4
          IL_007f:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
          IL_0084:  ldarg.0
          IL_0085:  ldc.i4.0
          IL_0086:  stfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
          IL_008b:  ldnull
          IL_008c:  stloc.1
          IL_008d:  leave.s    IL_009b

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_008f:  castclass  [mscorlib]System.Exception
          IL_0094:  stloc.2
          .line 6,6 : 15,16 ''
          IL_0095:  ldloc.2
          IL_0096:  stloc.0
          IL_0097:  ldnull
          IL_0098:  stloc.1
          IL_0099:  leave.s    IL_009b

          .line 100001,100001 : 0,0 ''
        }  // end handler
        IL_009b:  ldloc.1
        IL_009c:  pop
        .line 100001,100001 : 0,0 ''
        IL_009d:  nop
        IL_009e:  br         IL_0000

        IL_00a3:  ldloc.0
        IL_00a4:  ldnull
        IL_00a5:  cgt.un
        IL_00a7:  brfalse.s  IL_00ab

        IL_00a9:  br.s       IL_00ad

        IL_00ab:  br.s       IL_00af

        .line 100001,100001 : 0,0 ''
        IL_00ad:  ldloc.0
        IL_00ae:  throw

        .line 100001,100001 : 0,0 ''
        IL_00af:  ret
      } // end of method f4@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       67 (0x43)
        .maxstack  5
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::pc
        IL_0006:  switch     ( 
                              IL_0021,
                              IL_0023,
                              IL_0025,
                              IL_0027,
                              IL_0029)
        IL_001f:  br.s       IL_003a

        IL_0021:  br.s       IL_002b

        IL_0023:  br.s       IL_002e

        IL_0025:  br.s       IL_0031

        IL_0027:  br.s       IL_0034

        IL_0029:  br.s       IL_0037

        .line 100001,100001 : 0,0 ''
        IL_002b:  nop
        IL_002c:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_002e:  nop
        IL_002f:  br.s       IL_003f

        .line 100001,100001 : 0,0 ''
        IL_0031:  nop
        IL_0032:  br.s       IL_003d

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_003b

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  ldc.i4.1
        IL_003c:  ret

        IL_003d:  ldc.i4.1
        IL_003e:  ret

        IL_003f:  ldc.i4.1
        IL_0040:  ret

        IL_0041:  ldc.i4.0
        IL_0042:  ret
      } // end of method f4@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::current
        IL_0006:  ret
      } // end of method f4@6::get_LastGenerated

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
        IL_0004:  newobj     instance void ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                               int32,
                                                                                                               int32)
        IL_0009:  ret
      } // end of method f4@6::GetFreshEnumerator

    } // end of class f4@6

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f4() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 6,15 : 9,30 ''
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void ListExpressionSteppingTest5/ListExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
      IL_0009:  tail.
      IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0010:  ret
    } // end of method ListExpressionSteppingTest5::f4

  } // end of class ListExpressionSteppingTest5

} // end of class ListExpressionSteppingTest5

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest5>'.$ListExpressionSteppingTest5
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
    .line 17,17 : 13,17 ''
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5::f4()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $ListExpressionSteppingTest5::main@

} // end of class '<StartupCode$ListExpressionSteppingTest5>'.$ListExpressionSteppingTest5


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
