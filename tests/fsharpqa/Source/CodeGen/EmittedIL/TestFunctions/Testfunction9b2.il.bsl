
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
.assembly TestFunction9b2
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction9b2
{
  // Offset: 0x00000000 Length: 0x0000022C
}
.mresource public FSharpOptimizationData.TestFunction9b2
{
  // Offset: 0x00000230 Length: 0x00000083
}
.module TestFunction9b2.exe
// MVID: {4DAC30DF-9C0B-E35E-A745-0383DF30AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000160000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction9b2
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  TestFunction9b(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> x) cil managed
  {
    // Code size       412 (0x19c)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] int32 b,
             [4] int32 a,
             [5] int32 V_5,
             [6] int32 V_6,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_7,
             [8] int32 V_8,
             [9] int32 V_9,
             [10] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_10,
             [11] int32 V_11,
             [12] int32 V_12)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 5,17 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0009:  brfalse    IL_0196

    IL_000e:  ldloc.0
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0016:  ldc.i4.1
    IL_0017:  sub
    IL_0018:  switch     ( 
                          IL_0100)
    IL_0021:  ldloc.1
    IL_0022:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0027:  ldc.i4.3
    IL_0028:  sub
    IL_0029:  switch     ( 
                          IL_007b)
    IL_0032:  ldloc.1
    IL_0033:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0038:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003d:  brfalse    IL_0196

    IL_0042:  ldloc.1
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0048:  stloc.2
    IL_0049:  ldloc.2
    IL_004a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_004f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0054:  brtrue     IL_0196

    IL_0059:  ldloc.2
    IL_005a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_005f:  stloc.3
    IL_0060:  ldloc.1
    IL_0061:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0066:  stloc.s    a
    IL_0068:  ldloc.s    a
    IL_006a:  ldloc.3
    IL_006b:  add
    IL_006c:  ldc.i4.4
    IL_006d:  ceq
    IL_006f:  brfalse.s  IL_0076

    IL_0071:  br         IL_0180

    IL_0076:  br         IL_0196

    IL_007b:  ldloc.1
    IL_007c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0081:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0086:  brfalse    IL_0196

    IL_008b:  ldloc.1
    IL_008c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0091:  stloc.s    V_7
    IL_0093:  ldloc.s    V_7
    IL_0095:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_009a:  ldc.i4.4
    IL_009b:  sub
    IL_009c:  switch     ( 
                          IL_00ea)
    IL_00a5:  ldloc.s    V_7
    IL_00a7:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00ac:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00b1:  brtrue     IL_0196

    IL_00b6:  ldloc.s    V_7
    IL_00b8:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00bd:  stloc.s    V_8
    IL_00bf:  ldloc.1
    IL_00c0:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00c5:  stloc.s    V_9
    IL_00c7:  ldloc.s    V_9
    IL_00c9:  ldloc.s    V_8
    IL_00cb:  add
    IL_00cc:  ldc.i4.4
    IL_00cd:  ceq
    IL_00cf:  brfalse    IL_0196

    IL_00d4:  ldloc.s    V_7
    IL_00d6:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00db:  ldloc.1
    IL_00dc:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00e1:  stloc.s    V_6
    IL_00e3:  stloc.s    V_5
    IL_00e5:  br         IL_0190

    IL_00ea:  ldloc.s    V_7
    IL_00ec:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f1:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f6:  brtrue     IL_0196

    IL_00fb:  br         IL_017a

    IL_0100:  ldloc.1
    IL_0101:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0106:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_010b:  brfalse    IL_0196

    IL_0110:  ldloc.1
    IL_0111:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0116:  stloc.s    V_10
    IL_0118:  ldloc.s    V_10
    IL_011a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_011f:  ldc.i4.2
    IL_0120:  sub
    IL_0121:  switch     ( 
                          IL_0166)
    IL_012a:  ldloc.s    V_10
    IL_012c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0131:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0136:  brtrue.s   IL_0196

    IL_0138:  ldloc.s    V_10
    IL_013a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_013f:  stloc.s    V_11
    IL_0141:  ldloc.1
    IL_0142:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0147:  stloc.s    V_12
    IL_0149:  ldloc.s    V_12
    IL_014b:  ldloc.s    V_11
    IL_014d:  add
    IL_014e:  ldc.i4.4
    IL_014f:  ceq
    IL_0151:  brfalse.s  IL_0196

    IL_0153:  ldloc.s    V_10
    IL_0155:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_015a:  ldloc.1
    IL_015b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0160:  stloc.s    V_6
    IL_0162:  stloc.s    V_5
    IL_0164:  br.s       IL_0190

    IL_0166:  ldloc.s    V_10
    IL_0168:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_016d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0172:  brtrue.s   IL_0196

    .line 6,6 : 16,23 
    IL_0174:  ldstr      "three"
    IL_0179:  ret

    .line 7,7 : 16,23 
    IL_017a:  ldstr      "seven"
    IL_017f:  ret

    .line 5,5 : 5,17 
    IL_0180:  ldloc.2
    IL_0181:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0186:  stloc.s    V_5
    IL_0188:  ldloc.1
    IL_0189:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_018e:  stloc.s    V_6
    .line 8,8 : 29,35 
    IL_0190:  ldstr      "four"
    IL_0195:  ret

    .line 9,9 : 12,17 
    IL_0196:  ldstr      "big"
    IL_019b:  ret
  } // end of method TestFunction9b2::TestFunction9b

} // end of class TestFunction9b2

.class private abstract auto ansi sealed '<StartupCode$TestFunction9b2>'.$TestFunction9b2
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction9b2::main@

} // end of class '<StartupCode$TestFunction9b2>'.$TestFunction9b2


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
