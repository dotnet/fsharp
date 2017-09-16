
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
  .ver 4:4:1:0
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
  // Offset: 0x00000000 Length: 0x00000208
}
.mresource public FSharpOptimizationData.TestFunction9b3
{
  // Offset: 0x00000210 Length: 0x00000083
}
.module TestFunction9b3.exe
// MVID: {59B19208-C1A4-612A-A745-03830892B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00680000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction9b3
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  TestFunction9b(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> x) cil managed
  {
    // Code size       411 (0x19b)
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
    .line 5,5 : 5,17 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction9b3.fs'
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  brfalse    IL_0195

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0015:  ldc.i4.1
    IL_0016:  sub
    IL_0017:  switch     ( 
                          IL_00ff)
    IL_0020:  ldloc.1
    IL_0021:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0026:  ldc.i4.3
    IL_0027:  sub
    IL_0028:  switch     ( 
                          IL_007a)
    IL_0031:  ldloc.1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003c:  brfalse    IL_0195

    IL_0041:  ldloc.1
    IL_0042:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_004e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0053:  brtrue     IL_0195

    IL_0058:  ldloc.2
    IL_0059:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_005e:  stloc.3
    IL_005f:  ldloc.1
    IL_0060:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0065:  stloc.s    a
    IL_0067:  ldloc.s    a
    IL_0069:  ldloc.3
    IL_006a:  add
    IL_006b:  ldc.i4.4
    IL_006c:  ceq
    IL_006e:  brfalse.s  IL_0075

    IL_0070:  br         IL_017f

    IL_0075:  br         IL_0195

    IL_007a:  ldloc.1
    IL_007b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0080:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0085:  brfalse    IL_0195

    IL_008a:  ldloc.1
    IL_008b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0090:  stloc.s    V_7
    IL_0092:  ldloc.s    V_7
    IL_0094:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0099:  ldc.i4.4
    IL_009a:  sub
    IL_009b:  switch     ( 
                          IL_00e9)
    IL_00a4:  ldloc.s    V_7
    IL_00a6:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00ab:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00b0:  brtrue     IL_0195

    IL_00b5:  ldloc.s    V_7
    IL_00b7:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00bc:  stloc.s    V_8
    IL_00be:  ldloc.1
    IL_00bf:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00c4:  stloc.s    V_9
    IL_00c6:  ldloc.s    V_9
    IL_00c8:  ldloc.s    V_8
    IL_00ca:  add
    IL_00cb:  ldc.i4.4
    IL_00cc:  ceq
    IL_00ce:  brfalse    IL_0195

    IL_00d3:  ldloc.s    V_7
    IL_00d5:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00da:  ldloc.1
    IL_00db:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00e0:  stloc.s    V_6
    IL_00e2:  stloc.s    V_5
    IL_00e4:  br         IL_018f

    IL_00e9:  ldloc.s    V_7
    IL_00eb:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f0:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f5:  brtrue     IL_0195

    IL_00fa:  br         IL_0179

    IL_00ff:  ldloc.1
    IL_0100:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0105:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_010a:  brfalse    IL_0195

    IL_010f:  ldloc.1
    IL_0110:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0115:  stloc.s    V_10
    IL_0117:  ldloc.s    V_10
    IL_0119:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_011e:  ldc.i4.2
    IL_011f:  sub
    IL_0120:  switch     ( 
                          IL_0165)
    IL_0129:  ldloc.s    V_10
    IL_012b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0130:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0135:  brtrue.s   IL_0195

    IL_0137:  ldloc.s    V_10
    IL_0139:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_013e:  stloc.s    V_11
    IL_0140:  ldloc.1
    IL_0141:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0146:  stloc.s    V_12
    IL_0148:  ldloc.s    V_12
    IL_014a:  ldloc.s    V_11
    IL_014c:  add
    IL_014d:  ldc.i4.4
    IL_014e:  ceq
    IL_0150:  brfalse.s  IL_0195

    IL_0152:  ldloc.s    V_10
    IL_0154:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0159:  ldloc.1
    IL_015a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_015f:  stloc.s    V_6
    IL_0161:  stloc.s    V_5
    IL_0163:  br.s       IL_018f

    IL_0165:  ldloc.s    V_10
    IL_0167:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_016c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0171:  brtrue.s   IL_0195

    .line 6,6 : 16,23 ''
    IL_0173:  ldstr      "three"
    IL_0178:  ret

    .line 7,7 : 16,23 ''
    IL_0179:  ldstr      "seven"
    IL_017e:  ret

    .line 5,5 : 5,17 ''
    IL_017f:  ldloc.2
    IL_0180:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0185:  stloc.s    V_5
    IL_0187:  ldloc.1
    IL_0188:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_018d:  stloc.s    V_6
    .line 8,8 : 29,35 ''
    IL_018f:  ldstr      "four"
    IL_0194:  ret

    .line 9,9 : 12,17 ''
    IL_0195:  ldstr      "big"
    IL_019a:  ret
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
