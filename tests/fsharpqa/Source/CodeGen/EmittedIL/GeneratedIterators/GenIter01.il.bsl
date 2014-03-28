
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
.assembly GenIter01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter01
{
  // Offset: 0x00000000 Length: 0x000001D9
}
.mresource public FSharpOptimizationData.GenIter01
{
  // Offset: 0x000001E0 Length: 0x0000007A
}
.module GenIter01.exe
// MVID: {4D94B058-F836-DC98-A745-038358B0944D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00580000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar sealed nested assembly beforefieldinit specialname squaresOfOneToTen@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  6
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 GenIter01/squaresOfOneToTen@4::x
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      int32 GenIter01/squaresOfOneToTen@4::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_0023:  ret
    } // end of method squaresOfOneToTen@4::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       182 (0xb6)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter01/squaresOfOneToTen@4::pc
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

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_0082

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br         IL_00ad

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002d:  nop
      .line 4,5 : 7,23 
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.0
      IL_0030:  ldc.i4.1
      IL_0031:  ldc.i4.2
      IL_0032:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0037:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_0041:  ldarg.0
      IL_0042:  ldc.i4.1
      IL_0043:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
      .line 4,5 : 7,23 
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_004e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0053:  brfalse.s  IL_008c

      IL_0055:  ldarg.0
      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_005c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0061:  stfld      int32 GenIter01/squaresOfOneToTen@4::x
      IL_0066:  ldarg.0
      IL_0067:  ldc.i4.2
      IL_0068:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
      .line 5,5 : 18,23 
      IL_006d:  ldarg.0
      IL_006e:  ldarg.0
      IL_006f:  ldfld      int32 GenIter01/squaresOfOneToTen@4::x
      IL_0074:  ldarg.0
      IL_0075:  ldfld      int32 GenIter01/squaresOfOneToTen@4::x
      IL_007a:  mul
      IL_007b:  stfld      int32 GenIter01/squaresOfOneToTen@4::current
      IL_0080:  ldc.i4.1
      IL_0081:  ret

      .line 4,5 : 7,23 
      IL_0082:  ldarg.0
      IL_0083:  ldc.i4.0
      IL_0084:  stfld      int32 GenIter01/squaresOfOneToTen@4::x
      .line 100001,100001 : 0,0 
      IL_0089:  nop
      IL_008a:  br.s       IL_0048

      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.3
      IL_008e:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
      .line 4,5 : 7,23 
      IL_0093:  ldarg.0
      IL_0094:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_0099:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_009e:  nop
      IL_009f:  ldarg.0
      IL_00a0:  ldnull
      IL_00a1:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
      IL_00a6:  ldarg.0
      IL_00a7:  ldc.i4.3
      IL_00a8:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
      IL_00ad:  ldarg.0
      IL_00ae:  ldc.i4.0
      IL_00af:  stfld      int32 GenIter01/squaresOfOneToTen@4::current
      IL_00b4:  ldc.i4.0
      IL_00b5:  ret
    } // end of method squaresOfOneToTen@4::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 GenIter01/squaresOfOneToTen@4::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 GenIter01/squaresOfOneToTen@4::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004d:  nop
        .line 100001,100001 : 0,0 
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter01/squaresOfOneToTen@4::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 GenIter01/squaresOfOneToTen@4::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 GenIter01/squaresOfOneToTen@4::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 4,5 : 7,23 
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 
      IL_0095:  ret
    } // end of method squaresOfOneToTen@4::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 GenIter01/squaresOfOneToTen@4::pc
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
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method squaresOfOneToTen@4::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 GenIter01/squaresOfOneToTen@4::current
      IL_0007:  ret
    } // end of method squaresOfOneToTen@4::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void GenIter01/squaresOfOneToTen@4::.ctor(int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000a:  ret
    } // end of method squaresOfOneToTen@4::GetFreshEnumerator

  } // end of class squaresOfOneToTen@4

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTen() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  6
    .line 4,5 : 5,25 
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  ldnull
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  newobj     instance void GenIter01/squaresOfOneToTen@4::.ctor(int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_000a:  tail.
    IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0011:  ret
  } // end of method GenIter01::squaresOfOneToTen

} // end of class GenIter01

.class private abstract auto ansi sealed '<StartupCode$GenIter01>'.$GenIter01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  2
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $GenIter01::main@

} // end of class '<StartupCode$GenIter01>'.$GenIter01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
