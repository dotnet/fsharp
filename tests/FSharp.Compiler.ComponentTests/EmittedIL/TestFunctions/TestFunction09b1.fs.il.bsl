
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly TestFunction09b1
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction09b1
{
  // Offset: 0x00000000 Length: 0x00000240
  // WARNING: managed resource file FSharpSignatureData.TestFunction09b1 created
}
.mresource public FSharpOptimizationData.TestFunction09b1
{
  // Offset: 0x00000248 Length: 0x00000085
  // WARNING: managed resource file FSharpOptimizationData.TestFunction09b1 created
}
.module TestFunction09b1.exe
// MVID: {624E2CBA-CE7E-E1C6-A745-0383BA2C4E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x035A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction09b1
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  TestFunction9b(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> x) cil managed
  {
    // Code size       409 (0x199)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_7,
             int32 V_8,
             int32 V_9,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_10,
             int32 V_11,
             int32 V_12)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  brfalse    IL_0193

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0015:  ldc.i4.1
    IL_0016:  sub
    IL_0017:  switch     ( 
                          IL_00fd)
    IL_0020:  ldloc.1
    IL_0021:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0026:  ldc.i4.3
    IL_0027:  sub
    IL_0028:  switch     ( 
                          IL_0078)
    IL_0031:  ldloc.1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003c:  brfalse    IL_0193

    IL_0041:  ldloc.1
    IL_0042:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_004e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0053:  brtrue     IL_0193

    IL_0058:  ldloc.2
    IL_0059:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_005e:  stloc.3
    IL_005f:  ldloc.1
    IL_0060:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0065:  stloc.s    V_4
    IL_0067:  ldloc.s    V_4
    IL_0069:  ldloc.3
    IL_006a:  add
    IL_006b:  ldc.i4.4
    IL_006c:  ceq
    IL_006e:  brfalse    IL_0193

    IL_0073:  br         IL_017d

    IL_0078:  ldloc.1
    IL_0079:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_007e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0083:  brfalse    IL_0193

    IL_0088:  ldloc.1
    IL_0089:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_008e:  stloc.s    V_7
    IL_0090:  ldloc.s    V_7
    IL_0092:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0097:  ldc.i4.4
    IL_0098:  sub
    IL_0099:  switch     ( 
                          IL_00e7)
    IL_00a2:  ldloc.s    V_7
    IL_00a4:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00a9:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00ae:  brtrue     IL_0193

    IL_00b3:  ldloc.s    V_7
    IL_00b5:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00ba:  stloc.s    V_8
    IL_00bc:  ldloc.1
    IL_00bd:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00c2:  stloc.s    V_9
    IL_00c4:  ldloc.s    V_9
    IL_00c6:  ldloc.s    V_8
    IL_00c8:  add
    IL_00c9:  ldc.i4.4
    IL_00ca:  ceq
    IL_00cc:  brfalse    IL_0193

    IL_00d1:  ldloc.s    V_7
    IL_00d3:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00d8:  ldloc.1
    IL_00d9:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00de:  stloc.s    V_6
    IL_00e0:  stloc.s    V_5
    IL_00e2:  br         IL_018d

    IL_00e7:  ldloc.s    V_7
    IL_00e9:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00ee:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00f3:  brtrue     IL_0193

    IL_00f8:  br         IL_0177

    IL_00fd:  ldloc.1
    IL_00fe:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0103:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0108:  brfalse    IL_0193

    IL_010d:  ldloc.1
    IL_010e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0113:  stloc.s    V_10
    IL_0115:  ldloc.s    V_10
    IL_0117:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_011c:  ldc.i4.2
    IL_011d:  sub
    IL_011e:  switch     ( 
                          IL_0163)
    IL_0127:  ldloc.s    V_10
    IL_0129:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_012e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0133:  brtrue.s   IL_0193

    IL_0135:  ldloc.s    V_10
    IL_0137:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_013c:  stloc.s    V_11
    IL_013e:  ldloc.1
    IL_013f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0144:  stloc.s    V_12
    IL_0146:  ldloc.s    V_12
    IL_0148:  ldloc.s    V_11
    IL_014a:  add
    IL_014b:  ldc.i4.4
    IL_014c:  ceq
    IL_014e:  brfalse.s  IL_0193

    IL_0150:  ldloc.s    V_10
    IL_0152:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0157:  ldloc.1
    IL_0158:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_015d:  stloc.s    V_6
    IL_015f:  stloc.s    V_5
    IL_0161:  br.s       IL_018d

    IL_0163:  ldloc.s    V_10
    IL_0165:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_016a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_016f:  brtrue.s   IL_0193

    IL_0171:  ldstr      "three"
    IL_0176:  ret

    IL_0177:  ldstr      "seven"
    IL_017c:  ret

    IL_017d:  ldloc.2
    IL_017e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0183:  stloc.s    V_5
    IL_0185:  ldloc.1
    IL_0186:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_018b:  stloc.s    V_6
    IL_018d:  ldstr      "four"
    IL_0192:  ret

    IL_0193:  ldstr      "big"
    IL_0198:  ret
  } // end of method TestFunction09b1::TestFunction9b

} // end of class TestFunction09b1

.class private abstract auto ansi sealed '<StartupCode$TestFunction09b1>'.$TestFunction09b1
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction09b1::main@

} // end of class '<StartupCode$TestFunction09b1>'.$TestFunction09b1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\TestFunctions\TestFunction09b1_fs\TestFunction09b1.res
