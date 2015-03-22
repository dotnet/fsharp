
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
.assembly ListExpressionSteppingTestNoExnFilters6
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTestNoExnFilters6
{
  // Offset: 0x00000000 Length: 0x000002BA
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTestNoExnFilters6
{
  // Offset: 0x000002C0 Length: 0x000000BC
}
.module ListExpressionSteppingTestNoExnFilters6.exe
// MVID: {4DAC0CD3-98A2-AB14-A745-0383D30CAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000580000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTestNoExnFilters6
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTestNoExnFilters6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f7@7
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 x
      .field public int32 x0
      .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> enum0
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
                                   int32 x0,
                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> enum0,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       52 (0x34)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x0
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    enum0
        IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_001d:  ldarg.0
        IL_001e:  ldarg.s    pc
        IL_0020:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        IL_0025:  ldarg.0
        IL_0026:  ldarg.s    current
        IL_0028:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
        IL_002d:  ldarg.0
        IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0033:  ret
      } // end of method f7@7::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       351 (0x15f)
        .maxstack  6
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0025,
                              IL_0027,
                              IL_0029,
                              IL_002b)
        IL_0021:  br.s       IL_0048

        IL_0023:  br.s       IL_002d

        IL_0025:  br.s       IL_0033

        IL_0027:  br.s       IL_0036

        IL_0029:  br.s       IL_003c

        IL_002b:  br.s       IL_0042

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_002d:  nop
        IL_002e:  br         IL_00b2

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0033:  nop
        IL_0034:  br.s       IL_00a8

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0036:  nop
        IL_0037:  br         IL_0135

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_003c:  nop
        IL_003d:  br         IL_012b

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0042:  nop
        IL_0043:  br         IL_0156

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0048:  nop
        .line 7,9 : 11,21 
        IL_0049:  ldarg.0
        IL_004a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6::get_es()
        IL_004f:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
        IL_0054:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_0059:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.1
        IL_0060:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 7,9 : 11,21 
        IL_0065:  ldarg.0
        IL_0066:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        IL_006b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0070:  brfalse.s  IL_00b2

        IL_0072:  ldarg.0
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        IL_0079:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_007e:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x
        .line 8,8 : 14,29 
        IL_0083:  ldstr      "hello"
        IL_0088:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_008d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0092:  pop
        IL_0093:  ldarg.0
        IL_0094:  ldc.i4.2
        IL_0095:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 9,9 : 14,21 
        IL_009a:  ldarg.0
        IL_009b:  ldarg.0
        IL_009c:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x
        IL_00a1:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
        IL_00a6:  ldc.i4.1
        IL_00a7:  ret

        .line 7,9 : 11,21 
        IL_00a8:  ldarg.0
        IL_00a9:  ldc.i4.0
        IL_00aa:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x
        .line 100001,100001 : 0,0 
        IL_00af:  nop
        IL_00b0:  br.s       IL_0065

        IL_00b2:  ldarg.0
        IL_00b3:  ldc.i4.5
        IL_00b4:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 7,9 : 11,21 
        IL_00b9:  ldarg.0
        IL_00ba:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        IL_00bf:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_00c4:  nop
        IL_00c5:  ldarg.0
        IL_00c6:  ldnull
        IL_00c7:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
        .line 10,12 : 11,21 
        IL_00cc:  ldarg.0
        IL_00cd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6::get_es()
        IL_00d2:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
        IL_00d7:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_00dc:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_00e1:  ldarg.0
        IL_00e2:  ldc.i4.3
        IL_00e3:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 10,12 : 11,21 
        IL_00e8:  ldarg.0
        IL_00e9:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_00ee:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00f3:  brfalse.s  IL_0135

        IL_00f5:  ldarg.0
        IL_00f6:  ldarg.0
        IL_00f7:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_00fc:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0101:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x0
        .line 11,11 : 14,31 
        IL_0106:  ldstr      "goodbye"
        IL_010b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0110:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0115:  pop
        IL_0116:  ldarg.0
        IL_0117:  ldc.i4.4
        IL_0118:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 12,12 : 14,21 
        IL_011d:  ldarg.0
        IL_011e:  ldarg.0
        IL_011f:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x0
        IL_0124:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
        IL_0129:  ldc.i4.1
        IL_012a:  ret

        .line 10,12 : 11,21 
        IL_012b:  ldarg.0
        IL_012c:  ldc.i4.0
        IL_012d:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::x0
        .line 100001,100001 : 0,0 
        IL_0132:  nop
        IL_0133:  br.s       IL_00e8

        IL_0135:  ldarg.0
        IL_0136:  ldc.i4.5
        IL_0137:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        .line 10,12 : 11,21 
        IL_013c:  ldarg.0
        IL_013d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_0142:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0147:  nop
        IL_0148:  ldarg.0
        IL_0149:  ldnull
        IL_014a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
        IL_014f:  ldarg.0
        IL_0150:  ldc.i4.5
        IL_0151:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        IL_0156:  ldarg.0
        IL_0157:  ldc.i4.0
        IL_0158:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
        IL_015d:  ldc.i4.0
        IL_015e:  ret
      } // end of method f7@7::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       191 (0xbf)
        .maxstack  6
        .locals init ([0] class [mscorlib]System.Exception V_0,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
                 [2] class [mscorlib]System.Exception e)
        .line 100001,100001 : 0,0 
        IL_0000:  ldnull
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        IL_0008:  ldc.i4.5
        IL_0009:  sub
        IL_000a:  switch     ( 
                              IL_0015)
        IL_0013:  br.s       IL_001b

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0015:  nop
        IL_0016:  br         IL_00b2

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_001b:  nop
        .try
        {
          IL_001c:  ldarg.0
          IL_001d:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
          IL_0022:  switch     ( 
                                IL_0041,
                                IL_0043,
                                IL_0045,
                                IL_0047,
                                IL_0049,
                                IL_004b)
          IL_003f:  br.s       IL_005f

          IL_0041:  br.s       IL_004d

          IL_0043:  br.s       IL_0050

          IL_0045:  br.s       IL_0053

          IL_0047:  br.s       IL_0056

          IL_0049:  br.s       IL_0059

          IL_004b:  br.s       IL_005c

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_004d:  nop
          IL_004e:  br.s       IL_008c

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_0050:  nop
          IL_0051:  br.s       IL_0078

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_0053:  nop
          IL_0054:  br.s       IL_0077

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_0056:  nop
          IL_0057:  br.s       IL_0061

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_0059:  nop
          IL_005a:  br.s       IL_0060

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_005c:  nop
          IL_005d:  br.s       IL_008c

          .line 100001,100001 : 0,0 
          .line 100001,100001 : 0,0 
          IL_005f:  nop
          .line 100001,100001 : 0,0 
          IL_0060:  nop
          IL_0061:  ldarg.0
          IL_0062:  ldc.i4.5
          IL_0063:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
          IL_0068:  ldarg.0
          IL_0069:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::enum0
          IL_006e:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_0073:  nop
          .line 100001,100001 : 0,0 
          IL_0074:  nop
          IL_0075:  br.s       IL_008c

          .line 100001,100001 : 0,0 
          IL_0077:  nop
          IL_0078:  ldarg.0
          IL_0079:  ldc.i4.5
          IL_007a:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
          IL_007f:  ldarg.0
          IL_0080:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::'enum'
          IL_0085:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_008a:  nop
          .line 100001,100001 : 0,0 
          IL_008b:  nop
          IL_008c:  ldarg.0
          IL_008d:  ldc.i4.5
          IL_008e:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
          IL_0093:  ldarg.0
          IL_0094:  ldc.i4.0
          IL_0095:  stfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
          IL_009a:  ldnull
          IL_009b:  stloc.1
          IL_009c:  leave.s    IL_00aa

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_009e:  castclass  [mscorlib]System.Exception
          IL_00a3:  stloc.2
          .line 7,9 : 11,21 
          IL_00a4:  ldloc.2
          IL_00a5:  stloc.0
          IL_00a6:  ldnull
          IL_00a7:  stloc.1
          IL_00a8:  leave.s    IL_00aa

          .line 100001,100001 : 0,0 
        }  // end handler
        IL_00aa:  ldloc.1
        IL_00ab:  pop
        .line 100001,100001 : 0,0 
        IL_00ac:  nop
        IL_00ad:  br         IL_0002

        IL_00b2:  ldloc.0
        IL_00b3:  ldnull
        IL_00b4:  cgt.un
        IL_00b6:  brfalse.s  IL_00ba

        IL_00b8:  br.s       IL_00bc

        IL_00ba:  br.s       IL_00be

        .line 100001,100001 : 0,0 
        IL_00bc:  ldloc.0
        IL_00bd:  throw

        .line 100001,100001 : 0,0 
        IL_00be:  ret
      } // end of method f7@7::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       79 (0x4f)
        .maxstack  5
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::pc
        IL_0007:  switch     ( 
                              IL_0026,
                              IL_0028,
                              IL_002a,
                              IL_002c,
                              IL_002e,
                              IL_0030)
        IL_0024:  br.s       IL_0044

        IL_0026:  br.s       IL_0032

        IL_0028:  br.s       IL_0035

        IL_002a:  br.s       IL_0038

        IL_002c:  br.s       IL_003b

        IL_002e:  br.s       IL_003e

        IL_0030:  br.s       IL_0041

        IL_0032:  nop
        IL_0033:  br.s       IL_004d

        IL_0035:  nop
        IL_0036:  br.s       IL_004b

        IL_0038:  nop
        IL_0039:  br.s       IL_0049

        IL_003b:  nop
        IL_003c:  br.s       IL_0047

        IL_003e:  nop
        IL_003f:  br.s       IL_0045

        IL_0041:  nop
        IL_0042:  br.s       IL_004d

        IL_0044:  nop
        IL_0045:  ldc.i4.1
        IL_0046:  ret

        IL_0047:  ldc.i4.1
        IL_0048:  ret

        IL_0049:  ldc.i4.1
        IL_004a:  ret

        IL_004b:  ldc.i4.1
        IL_004c:  ret

        IL_004d:  ldc.i4.0
        IL_004e:  ret
      } // end of method f7@7::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::current
        IL_0007:  ret
      } // end of method f7@7::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  10
        IL_0000:  nop
        IL_0001:  ldc.i4.0
        IL_0002:  ldc.i4.0
        IL_0003:  ldnull
        IL_0004:  ldnull
        IL_0005:  ldc.i4.0
        IL_0006:  ldc.i4.0
        IL_0007:  newobj     instance void ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::.ctor(int32,
                                                                                                               int32,
                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                               int32,
                                                                                                               int32)
        IL_000c:  ret
      } // end of method f7@7::GetFreshEnumerator

    } // end of class f7@7

    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            get_es() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTestNoExnFilters6>'.$ListExpressionSteppingTestNoExnFilters6::es@5
      IL_0005:  ret
    } // end of method ListExpressionSteppingTestNoExnFilters6::get_es

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f7() cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      .line 7,12 : 9,23 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldnull
      IL_0005:  ldc.i4.0
      IL_0006:  ldc.i4.0
      IL_0007:  newobj     instance void ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6/f7@7::.ctor(int32,
                                                                                                             int32,
                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
      IL_000c:  tail.
      IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0013:  ret
    } // end of method ListExpressionSteppingTestNoExnFilters6::f7

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6::get_es()
    } // end of property ListExpressionSteppingTestNoExnFilters6::es
  } // end of class ListExpressionSteppingTestNoExnFilters6

} // end of class ListExpressionSteppingTestNoExnFilters6

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTestNoExnFilters6>'.$ListExpressionSteppingTestNoExnFilters6
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es@5
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       38 (0x26)
    .maxstack  6
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es)
    .line 5,5 : 5,21 
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.2
    IL_0003:  ldc.i4.3
    IL_0004:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0018:  dup
    IL_0019:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTestNoExnFilters6>'.$ListExpressionSteppingTestNoExnFilters6::es@5
    IL_001e:  stloc.0
    .line 14,14 : 13,17 
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTestNoExnFilters6/ListExpressionSteppingTestNoExnFilters6::f7()
    IL_0024:  pop
    IL_0025:  ret
  } // end of method $ListExpressionSteppingTestNoExnFilters6::main@

} // end of class '<StartupCode$ListExpressionSteppingTestNoExnFilters6>'.$ListExpressionSteppingTestNoExnFilters6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
