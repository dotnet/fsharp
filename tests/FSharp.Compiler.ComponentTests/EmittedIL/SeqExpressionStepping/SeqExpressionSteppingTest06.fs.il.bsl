
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly SeqExpressionSteppingTest06
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest06
{
  // Offset: 0x00000000 Length: 0x00000306
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest06 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest06
{
  // Offset: 0x00000310 Length: 0x000000BB
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest06 created
}
.module SeqExpressionSteppingTest06.exe
// MVID: {624FDC53-C071-79A0-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000017ED0F80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest6
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest6
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f7@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> enum0
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 pc
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                   class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> enum0,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f7@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       292 (0x124)
        .maxstack  6
        .locals init (int32 V_0,
                 int32 V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0026,
                              IL_0029,
                              IL_002f,
                              IL_0035)
        IL_0021:  br.s       IL_003b

        IL_0023:  nop
        IL_0024:  br.s       IL_008e

        IL_0026:  nop
        IL_0027:  br.s       IL_0081

        IL_0029:  nop
        IL_002a:  br         IL_00fa

        IL_002f:  nop
        IL_0030:  br         IL_00ed

        IL_0035:  nop
        IL_0036:  br         IL_011b

        IL_003b:  nop
        IL_003c:  ldarg.0
        IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
        IL_0042:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_0047:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_004c:  ldarg.0
        IL_004d:  ldc.i4.1
        IL_004e:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0053:  br.s       IL_0081

        IL_0055:  ldarg.0
        IL_0056:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_005b:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0060:  stloc.0
        IL_0061:  ldstr      "hello"
        IL_0066:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_006b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0070:  pop
        IL_0071:  ldarg.0
        IL_0072:  ldc.i4.2
        IL_0073:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0078:  ldarg.0
        IL_0079:  ldloc.0
        IL_007a:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_007f:  ldc.i4.1
        IL_0080:  ret

        IL_0081:  ldarg.0
        IL_0082:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_0087:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
        IL_008c:  brtrue.s   IL_0055

        IL_008e:  ldarg.0
        IL_008f:  ldc.i4.5
        IL_0090:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0095:  ldarg.0
        IL_0096:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_009b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_00a0:  nop
        IL_00a1:  ldarg.0
        IL_00a2:  ldnull
        IL_00a3:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_00a8:  ldarg.0
        IL_00a9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
        IL_00ae:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_00b3:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00b8:  ldarg.0
        IL_00b9:  ldc.i4.3
        IL_00ba:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_00bf:  br.s       IL_00ed

        IL_00c1:  ldarg.0
        IL_00c2:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00c7:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_00cc:  stloc.1
        IL_00cd:  ldstr      "goodbye"
        IL_00d2:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00d7:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00dc:  pop
        IL_00dd:  ldarg.0
        IL_00de:  ldc.i4.4
        IL_00df:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_00e4:  ldarg.0
        IL_00e5:  ldloc.1
        IL_00e6:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_00eb:  ldc.i4.1
        IL_00ec:  ret

        IL_00ed:  ldarg.0
        IL_00ee:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00f3:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
        IL_00f8:  brtrue.s   IL_00c1

        IL_00fa:  ldarg.0
        IL_00fb:  ldc.i4.5
        IL_00fc:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0101:  ldarg.0
        IL_0102:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_0107:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_010c:  nop
        IL_010d:  ldarg.0
        IL_010e:  ldnull
        IL_010f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_0114:  ldarg.0
        IL_0115:  ldc.i4.5
        IL_0116:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_011b:  ldarg.0
        IL_011c:  ldc.i4.0
        IL_011d:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_0122:  ldc.i4.0
        IL_0123:  ret
      } // end of method f7@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       164 (0xa4)
        .maxstack  6
        .locals init (class [System.Runtime]System.Exception V_0,
                 class [System.Runtime]System.Exception V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  ldc.i4.5
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0019

        IL_0013:  nop
        IL_0014:  br         IL_009e

        IL_0019:  nop
        .try
        {
          IL_001a:  ldarg.0
          IL_001b:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0020:  switch     ( 
                                IL_003f,
                                IL_0042,
                                IL_0045,
                                IL_0048,
                                IL_004b,
                                IL_004e)
          IL_003d:  br.s       IL_0051

          IL_003f:  nop
          IL_0040:  br.s       IL_007e

          IL_0042:  nop
          IL_0043:  br.s       IL_006a

          IL_0045:  nop
          IL_0046:  br.s       IL_0069

          IL_0048:  nop
          IL_0049:  br.s       IL_0053

          IL_004b:  nop
          IL_004c:  br.s       IL_0052

          IL_004e:  nop
          IL_004f:  br.s       IL_007e

          IL_0051:  nop
          IL_0052:  nop
          IL_0053:  ldarg.0
          IL_0054:  ldc.i4.5
          IL_0055:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_005a:  ldarg.0
          IL_005b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
          IL_0060:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_0065:  nop
          IL_0066:  nop
          IL_0067:  br.s       IL_007e

          IL_0069:  nop
          IL_006a:  ldarg.0
          IL_006b:  ldc.i4.5
          IL_006c:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0071:  ldarg.0
          IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
          IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_007c:  nop
          IL_007d:  nop
          IL_007e:  ldarg.0
          IL_007f:  ldc.i4.5
          IL_0080:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0085:  ldarg.0
          IL_0086:  ldc.i4.0
          IL_0087:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
          IL_008c:  leave.s    IL_0098

        }  // end .try
        catch [System.Runtime]System.Object 
        {
          IL_008e:  castclass  [System.Runtime]System.Exception
          IL_0093:  stloc.1
          IL_0094:  ldloc.1
          IL_0095:  stloc.0
          IL_0096:  leave.s    IL_0098

        }  // end handler
        IL_0098:  nop
        IL_0099:  br         IL_0000

        IL_009e:  ldloc.0
        IL_009f:  brfalse.s  IL_00a3

        IL_00a1:  ldloc.0
        IL_00a2:  throw

        IL_00a3:  ret
      } // end of method f7@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       66 (0x42)
        .maxstack  5
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  switch     ( 
                              IL_0025,
                              IL_0028,
                              IL_002b,
                              IL_002e,
                              IL_0031,
                              IL_0034)
        IL_0023:  br.s       IL_0037

        IL_0025:  nop
        IL_0026:  br.s       IL_0040

        IL_0028:  nop
        IL_0029:  br.s       IL_003e

        IL_002b:  nop
        IL_002c:  br.s       IL_003c

        IL_002e:  nop
        IL_002f:  br.s       IL_003a

        IL_0031:  nop
        IL_0032:  br.s       IL_0038

        IL_0034:  nop
        IL_0035:  br.s       IL_0040

        IL_0037:  nop
        IL_0038:  ldc.i4.1
        IL_0039:  ret

        IL_003a:  ldc.i4.1
        IL_003b:  ret

        IL_003c:  ldc.i4.1
        IL_003d:  ret

        IL_003e:  ldc.i4.1
        IL_003f:  ret

        IL_0040:  ldc.i4.0
        IL_0041:  ret
      } // end of method f7@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_0006:  ret
      } // end of method f7@6::get_LastGenerated

      .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       10 (0xa)
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  ldnull
        IL_0002:  ldc.i4.0
        IL_0003:  ldc.i4.0
        IL_0004:  newobj     instance void SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_0009:  ret
      } // end of method f7@6::GetFreshEnumerator

    } // end of class f7@6

    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            get_es() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$SeqExpressionSteppingTest06>'.$SeqExpressionSteppingTest6::es@4
      IL_0005:  ret
    } // end of method SeqExpressionSteppingTest6::get_es

    .method public static class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> 
            f7() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                           class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_0009:  ret
    } // end of method SeqExpressionSteppingTest6::f7

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
    } // end of property SeqExpressionSteppingTest6::es
  } // end of class SeqExpressionSteppingTest6

} // end of class SeqExpressionSteppingTest6

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest06>'.$SeqExpressionSteppingTest6
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es@4
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       44 (0x2c)
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_1)
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
    IL_0018:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$SeqExpressionSteppingTest06>'.$SeqExpressionSteppingTest6::es@4
    IL_001d:  stloc.0
    IL_001e:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::f7()
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_002a:  pop
    IL_002b:  ret
  } // end of method $SeqExpressionSteppingTest6::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest06>'.$SeqExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest06_fs\SeqExpressionSteppingTest06.res
