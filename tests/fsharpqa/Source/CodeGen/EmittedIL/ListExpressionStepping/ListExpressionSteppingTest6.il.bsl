
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
  .ver 4:4:3:0
}
.assembly ListExpressionSteppingTest6
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest6
{
  // Offset: 0x00000000 Length: 0x0000029D
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest6
{
  // Offset: 0x000002A8 Length: 0x000000BC
}
.module ListExpressionSteppingTest6.exe
// MVID: {5B16FAD7-98A2-AB14-A745-0383D7FA165B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01150000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest6
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f7@7
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
              instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> enum0,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f7@7::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       304 (0x130)
        .maxstack  6
        .locals init ([0] int32 x,
                 [1] int32 V_1)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest6.fs'
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0025,
                              IL_0027,
                              IL_0029,
                              IL_002b)
        IL_0021:  br.s       IL_0045

        IL_0023:  br.s       IL_002d

        IL_0025:  br.s       IL_0030

        IL_0027:  br.s       IL_0033

        IL_0029:  br.s       IL_0039

        IL_002b:  br.s       IL_003f

        .line 100001,100001 : 0,0 ''
        IL_002d:  nop
        IL_002e:  br.s       IL_0099

        .line 100001,100001 : 0,0 ''
        IL_0030:  nop
        IL_0031:  br.s       IL_0096

        .line 100001,100001 : 0,0 ''
        IL_0033:  nop
        IL_0034:  br         IL_0106

        .line 100001,100001 : 0,0 ''
        IL_0039:  nop
        IL_003a:  br         IL_0103

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br         IL_0127

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        .line 7,9 : 11,21 ''
        IL_0046:  ldarg.0
        IL_0047:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
        IL_004c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_0051:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.1
        IL_0058:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 7,9 : 11,21 ''
        IL_005d:  ldarg.0
        IL_005e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        IL_0063:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0068:  brfalse.s  IL_0099

        IL_006a:  ldarg.0
        IL_006b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        IL_0070:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0075:  stloc.0
        .line 8,8 : 14,29 ''
        IL_0076:  ldstr      "hello"
        IL_007b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0080:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0085:  pop
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.2
        IL_0088:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 9,9 : 14,21 ''
        IL_008d:  ldarg.0
        IL_008e:  ldloc.0
        IL_008f:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
        IL_0094:  ldc.i4.1
        IL_0095:  ret

        .line 100001,100001 : 0,0 ''
        IL_0096:  nop
        IL_0097:  br.s       IL_005d

        IL_0099:  ldarg.0
        IL_009a:  ldc.i4.5
        IL_009b:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 7,9 : 11,21 ''
        IL_00a0:  ldarg.0
        IL_00a1:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        IL_00a6:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_00ab:  nop
        IL_00ac:  ldarg.0
        IL_00ad:  ldnull
        IL_00ae:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
        .line 10,12 : 11,21 ''
        IL_00b3:  ldarg.0
        IL_00b4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
        IL_00b9:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_00be:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_00c3:  ldarg.0
        IL_00c4:  ldc.i4.3
        IL_00c5:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 10,12 : 11,21 ''
        IL_00ca:  ldarg.0
        IL_00cb:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_00d0:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00d5:  brfalse.s  IL_0106

        IL_00d7:  ldarg.0
        IL_00d8:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_00dd:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_00e2:  stloc.1
        .line 11,11 : 14,31 ''
        IL_00e3:  ldstr      "goodbye"
        IL_00e8:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00ed:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00f2:  pop
        IL_00f3:  ldarg.0
        IL_00f4:  ldc.i4.4
        IL_00f5:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 12,12 : 14,21 ''
        IL_00fa:  ldarg.0
        IL_00fb:  ldloc.1
        IL_00fc:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
        IL_0101:  ldc.i4.1
        IL_0102:  ret

        .line 100001,100001 : 0,0 ''
        IL_0103:  nop
        IL_0104:  br.s       IL_00ca

        IL_0106:  ldarg.0
        IL_0107:  ldc.i4.5
        IL_0108:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        .line 10,12 : 11,21 ''
        IL_010d:  ldarg.0
        IL_010e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_0113:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0118:  nop
        IL_0119:  ldarg.0
        IL_011a:  ldnull
        IL_011b:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
        IL_0120:  ldarg.0
        IL_0121:  ldc.i4.5
        IL_0122:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        IL_0127:  ldarg.0
        IL_0128:  ldc.i4.0
        IL_0129:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
        IL_012e:  ldc.i4.0
        IL_012f:  ret
      } // end of method f7@7::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       191 (0xbf)
        .maxstack  6
        .locals init ([0] class [mscorlib]System.Exception V_0,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
                 [2] class [mscorlib]System.Exception e)
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldnull
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        IL_0008:  ldc.i4.5
        IL_0009:  sub
        IL_000a:  switch     ( 
                              IL_0015)
        IL_0013:  br.s       IL_001b

        .line 100001,100001 : 0,0 ''
        IL_0015:  nop
        IL_0016:  br         IL_00b2

        .line 100001,100001 : 0,0 ''
        IL_001b:  nop
        .try
        {
          IL_001c:  ldarg.0
          IL_001d:  ldfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
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

          .line 100001,100001 : 0,0 ''
          IL_004d:  nop
          IL_004e:  br.s       IL_008c

          .line 100001,100001 : 0,0 ''
          IL_0050:  nop
          IL_0051:  br.s       IL_0078

          .line 100001,100001 : 0,0 ''
          IL_0053:  nop
          IL_0054:  br.s       IL_0077

          .line 100001,100001 : 0,0 ''
          IL_0056:  nop
          IL_0057:  br.s       IL_0061

          .line 100001,100001 : 0,0 ''
          IL_0059:  nop
          IL_005a:  br.s       IL_0060

          .line 100001,100001 : 0,0 ''
          IL_005c:  nop
          IL_005d:  br.s       IL_008c

          .line 100001,100001 : 0,0 ''
          IL_005f:  nop
          .line 100001,100001 : 0,0 ''
          IL_0060:  nop
          IL_0061:  ldarg.0
          IL_0062:  ldc.i4.5
          IL_0063:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
          IL_0068:  ldarg.0
          IL_0069:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::enum0
          IL_006e:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_0073:  nop
          .line 100001,100001 : 0,0 ''
          IL_0074:  nop
          IL_0075:  br.s       IL_008c

          .line 100001,100001 : 0,0 ''
          IL_0077:  nop
          IL_0078:  ldarg.0
          IL_0079:  ldc.i4.5
          IL_007a:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
          IL_007f:  ldarg.0
          IL_0080:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::'enum'
          IL_0085:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_008a:  nop
          .line 100001,100001 : 0,0 ''
          IL_008b:  nop
          IL_008c:  ldarg.0
          IL_008d:  ldc.i4.5
          IL_008e:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
          IL_0093:  ldarg.0
          IL_0094:  ldc.i4.0
          IL_0095:  stfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
          IL_009a:  ldnull
          IL_009b:  stloc.1
          IL_009c:  leave.s    IL_00aa

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_009e:  castclass  [mscorlib]System.Exception
          IL_00a3:  stloc.2
          .line 7,9 : 11,21 ''
          IL_00a4:  ldloc.2
          IL_00a5:  stloc.0
          IL_00a6:  ldnull
          IL_00a7:  stloc.1
          IL_00a8:  leave.s    IL_00aa

          .line 100001,100001 : 0,0 ''
        }  // end handler
        IL_00aa:  ldloc.1
        IL_00ab:  pop
        .line 100001,100001 : 0,0 ''
        IL_00ac:  nop
        IL_00ad:  br         IL_0002

        IL_00b2:  ldloc.0
        IL_00b3:  ldnull
        IL_00b4:  cgt.un
        IL_00b6:  brfalse.s  IL_00ba

        IL_00b8:  br.s       IL_00bc

        IL_00ba:  br.s       IL_00be

        .line 100001,100001 : 0,0 ''
        IL_00bc:  ldloc.0
        IL_00bd:  throw

        .line 100001,100001 : 0,0 ''
        IL_00be:  ret
      } // end of method f7@7::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       78 (0x4e)
        .maxstack  5
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::pc
        IL_0006:  switch     ( 
                              IL_0025,
                              IL_0027,
                              IL_0029,
                              IL_002b,
                              IL_002d,
                              IL_002f)
        IL_0023:  br.s       IL_0043

        IL_0025:  br.s       IL_0031

        IL_0027:  br.s       IL_0034

        IL_0029:  br.s       IL_0037

        IL_002b:  br.s       IL_003a

        IL_002d:  br.s       IL_003d

        IL_002f:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0031:  nop
        IL_0032:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_004a

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0046

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0044

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        IL_0041:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0043:  nop
        IL_0044:  ldc.i4.1
        IL_0045:  ret

        IL_0046:  ldc.i4.1
        IL_0047:  ret

        IL_0048:  ldc.i4.1
        IL_0049:  ret

        IL_004a:  ldc.i4.1
        IL_004b:  ret

        IL_004c:  ldc.i4.0
        IL_004d:  ret
      } // end of method f7@7::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::current
        IL_0006:  ret
      } // end of method f7@7::get_LastGenerated

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
        IL_0004:  newobj     instance void ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                               int32,
                                                                                                               int32)
        IL_0009:  ret
      } // end of method f7@7::GetFreshEnumerator

    } // end of class f7@7

    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            get_es() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6::es@5
      IL_0005:  ret
    } // end of method ListExpressionSteppingTest6::get_es

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f7() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 7,12 : 9,23 ''
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void ListExpressionSteppingTest6/ListExpressionSteppingTest6/f7@7::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
      IL_0009:  tail.
      IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0010:  ret
    } // end of method ListExpressionSteppingTest6::f7

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
    } // end of property ListExpressionSteppingTest6::es
  } // end of class ListExpressionSteppingTest6

} // end of class ListExpressionSteppingTest6

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6
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
    // Code size       37 (0x25)
    .maxstack  6
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es)
    .line 5,5 : 5,21 ''
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  dup
    IL_0018:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6::es@5
    IL_001d:  stloc.0
    .line 14,14 : 13,17 ''
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::f7()
    IL_0023:  pop
    IL_0024:  ret
  } // end of method $ListExpressionSteppingTest6::main@

} // end of class '<StartupCode$ListExpressionSteppingTest6>'.$ListExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
