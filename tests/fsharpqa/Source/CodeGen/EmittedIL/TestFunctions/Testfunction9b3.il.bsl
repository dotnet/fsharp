
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
  .ver 6:0:0:0
}
.assembly TestFunction9b3
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction9b3
{
  // Offset: 0x00000000 Length: 0x00000204
}
.mresource public FSharpOptimizationData.TestFunction9b3
{
  // Offset: 0x00000208 Length: 0x00000083
}
.module TestFunction9b3.exe
// MVID: {61E07031-C1A4-612A-A745-03833170E061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07320000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction9b3
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
    .line 5,5 : 5,17 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction9b3.fs'
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    .line 100001,100001 : 0,0 ''
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  brfalse    IL_0196

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    .line 100001,100001 : 0,0 ''
    IL_000f:  ldloc.1
    IL_0010:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0015:  ldc.i4.1
    IL_0016:  sub
    IL_0017:  switch     ( 
                          IL_00ff)
    .line 100001,100001 : 0,0 ''
    IL_0020:  ldloc.1
    IL_0021:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0026:  ldc.i4.3
    IL_0027:  sub
    IL_0028:  switch     ( 
                          IL_0079)
    .line 100001,100001 : 0,0 ''
    IL_0031:  ldloc.1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003c:  brfalse    IL_0196

    IL_0041:  ldloc.1
    IL_0042:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0047:  stloc.2
    .line 100001,100001 : 0,0 ''
    IL_0048:  ldloc.2
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_004e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0053:  brtrue     IL_0196

    .line 8,8 : 18,25 ''
    IL_0058:  nop
    .line 100001,100001 : 0,0 ''
    IL_0059:  ldloc.2
    IL_005a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_005f:  stloc.3
    IL_0060:  ldloc.1
    IL_0061:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0066:  stloc.s    a
    .line 8,8 : 18,25 ''
    IL_0068:  ldloc.s    a
    IL_006a:  ldloc.3
    IL_006b:  add
    IL_006c:  ldc.i4.4
    IL_006d:  ceq
    IL_006f:  brfalse    IL_0196

    IL_0074:  br         IL_0180

    .line 100001,100001 : 0,0 ''
    IL_0079:  ldloc.1
    IL_007a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_007f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0084:  brfalse    IL_0196

    IL_0089:  ldloc.1
    IL_008a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_008f:  stloc.s    V_7
    .line 100001,100001 : 0,0 ''
    IL_0091:  ldloc.s    V_7
    IL_0093:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0098:  ldc.i4.4
    IL_0099:  sub
    IL_009a:  switch     ( 
                          IL_00e9)
    .line 100001,100001 : 0,0 ''
    IL_00a3:  ldloc.s    V_7
    IL_00a5:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00aa:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00af:  brtrue     IL_0196

    .line 8,8 : 18,25 ''
    IL_00b4:  nop
    .line 100001,100001 : 0,0 ''
    IL_00b5:  ldloc.s    V_7
    IL_00b7:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00bc:  stloc.s    V_8
    IL_00be:  ldloc.1
    IL_00bf:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00c4:  stloc.s    V_9
    .line 8,8 : 18,25 ''
    IL_00c6:  ldloc.s    V_9
    IL_00c8:  ldloc.s    V_8
    IL_00ca:  add
    IL_00cb:  ldc.i4.4
    IL_00cc:  ceq
    IL_00ce:  brfalse    IL_0196

    .line 100001,100001 : 0,0 ''
    IL_00d3:  ldloc.s    V_7
    IL_00d5:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00da:  ldloc.1
    IL_00db:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00e0:  stloc.s    V_6
    IL_00e2:  stloc.s    V_5
    IL_00e4:  br         IL_0190

    .line 100001,100001 : 0,0 ''
    IL_00e9:  ldloc.s    V_7
    IL_00eb:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f0:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f5:  brtrue     IL_0196

    IL_00fa:  br         IL_017a

    .line 100001,100001 : 0,0 ''
    IL_00ff:  ldloc.1
    IL_0100:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0105:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_010a:  brfalse    IL_0196

    IL_010f:  ldloc.1
    IL_0110:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0115:  stloc.s    V_10
    .line 100001,100001 : 0,0 ''
    IL_0117:  ldloc.s    V_10
    IL_0119:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_011e:  ldc.i4.2
    IL_011f:  sub
    IL_0120:  switch     ( 
                          IL_0166)
    .line 100001,100001 : 0,0 ''
    IL_0129:  ldloc.s    V_10
    IL_012b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0130:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0135:  brtrue.s   IL_0196

    .line 8,8 : 18,25 ''
    IL_0137:  nop
    .line 100001,100001 : 0,0 ''
    IL_0138:  ldloc.s    V_10
    IL_013a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_013f:  stloc.s    V_11
    IL_0141:  ldloc.1
    IL_0142:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0147:  stloc.s    V_12
    .line 8,8 : 18,25 ''
    IL_0149:  ldloc.s    V_12
    IL_014b:  ldloc.s    V_11
    IL_014d:  add
    IL_014e:  ldc.i4.4
    IL_014f:  ceq
    IL_0151:  brfalse.s  IL_0196

    .line 100001,100001 : 0,0 ''
    IL_0153:  ldloc.s    V_10
    IL_0155:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_015a:  ldloc.1
    IL_015b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0160:  stloc.s    V_6
    IL_0162:  stloc.s    V_5
    IL_0164:  br.s       IL_0190

    .line 100001,100001 : 0,0 ''
    IL_0166:  ldloc.s    V_10
    IL_0168:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_016d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0172:  brtrue.s   IL_0196

    .line 6,6 : 16,23 ''
    IL_0174:  ldstr      "three"
    IL_0179:  ret

    .line 7,7 : 16,23 ''
    IL_017a:  ldstr      "seven"
    IL_017f:  ret

    .line 100001,100001 : 0,0 ''
    IL_0180:  ldloc.2
    IL_0181:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0186:  stloc.s    V_5
    IL_0188:  ldloc.1
    IL_0189:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_018e:  stloc.s    V_6
    .line 8,8 : 29,35 ''
    IL_0190:  ldstr      "four"
    IL_0195:  ret

    .line 9,9 : 12,17 ''
    IL_0196:  ldstr      "big"
    IL_019b:  ret
  } // end of method TestFunction9b3::TestFunction9b

} // end of class TestFunction9b3

.class private abstract auto ansi sealed '<StartupCode$TestFunction9b3>'.$TestFunction9b3
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction9b3::main@

} // end of class '<StartupCode$TestFunction9b3>'.$TestFunction9b3


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
