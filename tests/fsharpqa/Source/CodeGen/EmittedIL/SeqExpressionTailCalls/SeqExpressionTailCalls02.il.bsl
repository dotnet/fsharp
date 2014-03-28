
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
.assembly SeqExpressionTailCalls02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionTailCalls02
{
  // Offset: 0x00000000 Length: 0x00000238
}
.mresource public FSharpOptimizationData.SeqExpressionTailCalls02
{
  // Offset: 0x00000240 Length: 0x0000009E
}
.module SeqExpressionTailCalls02.dll
// MVID: {4D94AD8F-ED1E-676B-A745-03838FAD944D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x001A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionTailCalls02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname rwalk1@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
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
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::x
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method rwalk1@4::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       108 (0x6c)
      .maxstack  7
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002a

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0040

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_005c

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br.s       IL_0063

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.1
      IL_002d:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      .line 4,4 : 26,33 
      IL_0032:  ldarg.0
      IL_0033:  ldarg.0
      IL_0034:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::x
      IL_0039:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::current
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.2
      IL_0042:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      .line 4,4 : 42,54 
      IL_0047:  ldarg.1
      IL_0048:  ldarg.0
      IL_0049:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::x
      IL_004e:  ldc.i4.1
      IL_004f:  add
      IL_0050:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionTailCalls02::rwalk2(int32)
      IL_0055:  stobj      class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_005a:  ldc.i4.2
      IL_005b:  ret

      IL_005c:  ldarg.0
      IL_005d:  ldc.i4.3
      IL_005e:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      IL_0063:  ldarg.0
      IL_0064:  ldc.i4.0
      IL_0065:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::current
      IL_006a:  ldc.i4.0
      IL_006b:  ret
    } // end of method rwalk1@4::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  6
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.3
      IL_0003:  stfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
      IL_0008:  ret
    } // end of method rwalk1@4::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::pc
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
      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldc.i4.0
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method rwalk1@4::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::current
      IL_0007:  ret
    } // end of method rwalk1@4::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  7
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk1@4::x
      IL_0007:  ldc.i4.0
      IL_0008:  ldc.i4.0
      IL_0009:  newobj     instance void SeqExpressionTailCalls02/rwalk1@4::.ctor(int32,
                                                                                  int32,
                                                                                  int32)
      IL_000e:  ret
    } // end of method rwalk1@4::GetFreshEnumerator

  } // end of class rwalk1@4

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname rwalk2@5
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
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
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::x
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method rwalk2@5::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       108 (0x6c)
      .maxstack  7
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002a

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0040

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_005c

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br.s       IL_0063

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.1
      IL_002d:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      .line 5,5 : 26,33 
      IL_0032:  ldarg.0
      IL_0033:  ldarg.0
      IL_0034:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::x
      IL_0039:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::current
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.2
      IL_0042:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      .line 5,5 : 42,54 
      IL_0047:  ldarg.1
      IL_0048:  ldarg.0
      IL_0049:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::x
      IL_004e:  ldc.i4.1
      IL_004f:  add
      IL_0050:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionTailCalls02::rwalk1(int32)
      IL_0055:  stobj      class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_005a:  ldc.i4.2
      IL_005b:  ret

      IL_005c:  ldarg.0
      IL_005d:  ldc.i4.3
      IL_005e:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      IL_0063:  ldarg.0
      IL_0064:  ldc.i4.0
      IL_0065:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::current
      IL_006a:  ldc.i4.0
      IL_006b:  ret
    } // end of method rwalk2@5::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  6
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.3
      IL_0003:  stfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
      IL_0008:  ret
    } // end of method rwalk2@5::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::pc
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
      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldc.i4.0
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method rwalk2@5::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  5
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::current
      IL_0007:  ret
    } // end of method rwalk2@5::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  7
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 SeqExpressionTailCalls02/rwalk2@5::x
      IL_0007:  ldc.i4.0
      IL_0008:  ldc.i4.0
      IL_0009:  newobj     instance void SeqExpressionTailCalls02/rwalk2@5::.ctor(int32,
                                                                                  int32,
                                                                                  int32)
      IL_000e:  ret
    } // end of method rwalk2@5::GetFreshEnumerator

  } // end of class rwalk2@5

  .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          rwalk1(int32 x) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  5
    .line 4,4 : 20,56 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj     instance void SeqExpressionTailCalls02/rwalk1@4::.ctor(int32,
                                                                                int32,
                                                                                int32)
    IL_0009:  ret
  } // end of method SeqExpressionTailCalls02::rwalk1

  .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          rwalk2(int32 x) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  5
    .line 5,5 : 20,56 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj     instance void SeqExpressionTailCalls02/rwalk2@5::.ctor(int32,
                                                                                int32,
                                                                                int32)
    IL_0009:  ret
  } // end of method SeqExpressionTailCalls02::rwalk2

} // end of class SeqExpressionTailCalls02

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionTailCalls02>'.$SeqExpressionTailCalls02
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SeqExpressionTailCalls02>'.$SeqExpressionTailCalls02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
