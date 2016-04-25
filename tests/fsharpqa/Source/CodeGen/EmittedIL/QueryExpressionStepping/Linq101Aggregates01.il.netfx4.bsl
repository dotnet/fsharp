
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
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Linq101Aggregates01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Aggregates01
{
  // Offset: 0x00000000 Length: 0x0000060A
}
.mresource public FSharpOptimizationData.Linq101Aggregates01
{
  // Offset: 0x00000610 Length: 0x00000211
}
.module Linq101Aggregates01.exe
// MVID: {56D01D45-D281-4783-A745-0383451DD056}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Aggregates01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname uniqueFactors@12
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 n
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
            instance void  .ctor(int32 _arg1,
                                 int32 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method uniqueFactors@12::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'c:\\KevinRansom\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Aggregates01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 12,12 : 9,33 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 12,12 : 9,33 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0063:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      .line 12,12 : 9,33 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      IL_006f:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 13,13 : 9,17 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      IL_0082:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.0
      IL_008b:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      .line 12,12 : 9,33 ''
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.0
      IL_0092:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 12,12 : 9,33 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method uniqueFactors@12::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 12,12 : 9,33 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method uniqueFactors@12::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
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
    } // end of method uniqueFactors@12::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_0007:  ret
    } // end of method uniqueFactors@12::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void Linq101Aggregates01/uniqueFactors@12::.ctor(int32,
                                                                                     int32,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     int32)
      IL_000b:  ret
    } // end of method uniqueFactors@12::GetFreshEnumerator

  } // end of class uniqueFactors@12

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname numSum@21
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 n
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
            instance void  .ctor(int32 _arg1,
                                 int32 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Linq101Aggregates01/numSum@21::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/numSum@21::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method numSum@21::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 21,21 : 9,28 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 21,21 : 9,28 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0063:  stfld      int32 Linq101Aggregates01/numSum@21::_arg1
      .line 21,21 : 9,28 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      int32 Linq101Aggregates01/numSum@21::_arg1
      IL_006f:  stfld      int32 Linq101Aggregates01/numSum@21::n
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 22,22 : 9,16 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      int32 Linq101Aggregates01/numSum@21::n
      IL_0082:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.0
      IL_008b:  stfld      int32 Linq101Aggregates01/numSum@21::n
      .line 21,21 : 9,28 ''
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.0
      IL_0092:  stfld      int32 Linq101Aggregates01/numSum@21::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 21,21 : 9,28 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method numSum@21::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/numSum@21::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/numSum@21::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101Aggregates01/numSum@21::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 21,21 : 9,28 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method numSum@21::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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
    } // end of method numSum@21::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0007:  ret
    } // end of method numSum@21::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000b:  ret
    } // end of method numSum@21::GetFreshEnumerator

  } // end of class numSum@21

  .class auto ansi serializable nested assembly beforefieldinit 'numSum@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'numSum@22-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 22,22 : 15,16 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'numSum@22-1'::Invoke

  } // end of class 'numSum@22-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname totalChars@30
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string w
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string w,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Aggregates01/totalChars@30::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Aggregates01/totalChars@30::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method totalChars@30::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 30,30 : 9,26 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 30,30 : 9,26 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Aggregates01/totalChars@30::_arg1
      .line 30,30 : 9,26 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Aggregates01/totalChars@30::_arg1
      IL_006f:  stfld      string Linq101Aggregates01/totalChars@30::w
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 31,31 : 9,25 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Aggregates01/totalChars@30::w
      IL_0082:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Aggregates01/totalChars@30::w
      .line 30,30 : 9,26 ''
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Aggregates01/totalChars@30::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 30,30 : 9,26 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method totalChars@30::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Aggregates01/totalChars@30::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 30,30 : 9,26 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method totalChars@30::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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
    } // end of method totalChars@30::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Aggregates01/totalChars@30::current
      IL_0007:  ret
    } // end of method totalChars@30::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_000b:  ret
    } // end of method totalChars@30::GetFreshEnumerator

  } // end of class totalChars@30

  .class auto ansi serializable nested assembly beforefieldinit 'totalChars@31-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'totalChars@31-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 31,31 : 16,24 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0007:  ret
    } // end of method 'totalChars@31-1'::Invoke

  } // end of class 'totalChars@31-1'

  .class auto ansi serializable nested assembly beforefieldinit categories@39
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories@39::builder@
      IL_000d:  ret
    } // end of method categories@39::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 39,39 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 40,40 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories@39::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories@39::Invoke

  } // end of class categories@39

  .class auto ansi serializable nested assembly beforefieldinit 'categories@40-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories@40-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 40,40 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories@40-1'::Invoke

  } // end of class 'categories@40-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories@40-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories@40-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 40,40 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories@40-2'::Invoke

  } // end of class 'categories@40-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sum@42
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method sum@42::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 42,42 : 13,26 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 42,42 : 13,26 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      .line 42,42 : 13,26 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 43,43 : 13,33 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      .line 42,42 : 13,26 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 42,42 : 13,26 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method sum@42::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/sum@42::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/sum@42::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/sum@42::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 42,42 : 13,26 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method sum@42::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/sum@42::pc
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
    } // end of method sum@42::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0007:  ret
    } // end of method sum@42::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method sum@42::GetFreshEnumerator

  } // end of class sum@42

  .class auto ansi serializable nested assembly beforefieldinit 'sum@43-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'sum@43-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 43,43 : 19,33 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0009:  ret
    } // end of method 'sum@43-1'::Invoke

  } // end of class 'sum@43-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories@40-3'::builder@
      IL_000d:  ret
    } // end of method 'categories@40-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       169 (0xa9)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] int32 sum,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               [4] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>> V_4,
               [5] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable> V_5,
               [6] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_6,
               [7] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_7,
               [8] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_8,
               [9] int32 V_9,
               [10] int32 V_10,
               [11] class [mscorlib]System.IDisposable V_11)
      .line 40,40 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  stloc.3
      IL_000b:  ldloc.0
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldnull
      IL_000f:  ldc.i4.0
      IL_0010:  ldnull
      IL_0011:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001b:  newobj     instance void Linq101Aggregates01/'sum@43-1'::.ctor()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                          !1)
      IL_0025:  stloc.s    V_4
      IL_0027:  ldloc.s    V_4
      IL_0029:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>>::get_Item1()
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_4
      IL_0032:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>>::get_Item2()
      IL_0037:  stloc.s    V_6
      IL_0039:  ldloc.s    V_5
      IL_003b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0040:  stloc.s    V_7
      IL_0042:  ldloc.s    V_7
      IL_0044:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0049:  stloc.s    V_8
      .try
      {
        IL_004b:  ldc.i4.0
        IL_004c:  stloc.s    V_10
        IL_004e:  ldloc.s    V_8
        IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0055:  brfalse.s  IL_006d

        .line 43,43 : 13,33 ''
        IL_0057:  ldloc.s    V_10
        IL_0059:  ldloc.s    V_6
        IL_005b:  ldloc.s    V_8
        IL_005d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0062:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
        IL_0067:  add.ovf
        IL_0068:  stloc.s    V_10
        .line 100001,100001 : 0,0 ''
        IL_006a:  nop
        IL_006b:  br.s       IL_004e

        IL_006d:  ldloc.s    V_10
        IL_006f:  stloc.s    V_9
        IL_0071:  leave.s    IL_0091

      }  // end .try
      finally
      {
        IL_0073:  ldloc.s    V_8
        IL_0075:  isinst     [mscorlib]System.IDisposable
        IL_007a:  stloc.s    V_11
        IL_007c:  ldloc.s    V_11
        IL_007e:  brfalse.s  IL_0082

        IL_0080:  br.s       IL_0084

        IL_0082:  br.s       IL_008e

        .line 100001,100001 : 0,0 ''
        IL_0084:  ldloc.s    V_11
        IL_0086:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_008b:  ldnull
        IL_008c:  pop
        IL_008d:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_008e:  ldnull
        IL_008f:  pop
        IL_0090:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0091:  ldloc.s    V_9
      IL_0093:  stloc.1
      .line 45,45 : 9,28 ''
      IL_0094:  ldarg.0
      IL_0095:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories@40-3'::builder@
      IL_009a:  ldloc.0
      IL_009b:  ldloc.1
      IL_009c:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                    !1)
      IL_00a1:  tail.
      IL_00a3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_00a8:  ret
    } // end of method 'categories@40-3'::Invoke

  } // end of class 'categories@40-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories@45-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Tuple`2<string,int32>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories@45-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,int32> 
            Invoke(class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32> tupledArg) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] int32 sum)
      .line 45,45 : 17,27 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0015:  ldloc.1
      IL_0016:  newobj     instance void class [mscorlib]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001b:  ret
    } // end of method 'categories@45-4'::Invoke

  } // end of class 'categories@45-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname minNum@49
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 n
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
            instance void  .ctor(int32 _arg1,
                                 int32 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Linq101Aggregates01/minNum@49::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/minNum@49::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method minNum@49::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 49,49 : 22,41 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 22,41 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0063:  stfld      int32 Linq101Aggregates01/minNum@49::_arg1
      .line 49,49 : 22,41 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      int32 Linq101Aggregates01/minNum@49::_arg1
      IL_006f:  stfld      int32 Linq101Aggregates01/minNum@49::n
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 42,49 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      int32 Linq101Aggregates01/minNum@49::n
      IL_0082:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.0
      IL_008b:  stfld      int32 Linq101Aggregates01/minNum@49::n
      .line 49,49 : 22,41 ''
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.0
      IL_0092:  stfld      int32 Linq101Aggregates01/minNum@49::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 22,41 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method minNum@49::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/minNum@49::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/minNum@49::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101Aggregates01/minNum@49::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 49,49 : 22,41 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method minNum@49::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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
    } // end of method minNum@49::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0007:  ret
    } // end of method minNum@49::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000b:  ret
    } // end of method minNum@49::GetFreshEnumerator

  } // end of class minNum@49

  .class auto ansi serializable nested assembly beforefieldinit 'minNum@49-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'minNum@49-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 49,49 : 48,49 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'minNum@49-1'::Invoke

  } // end of class 'minNum@49-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname shortestWord@52
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string w
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string w,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Aggregates01/shortestWord@52::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Aggregates01/shortestWord@52::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method shortestWord@52::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 52,52 : 28,45 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 28,45 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Aggregates01/shortestWord@52::_arg1
      .line 52,52 : 28,45 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Aggregates01/shortestWord@52::_arg1
      IL_006f:  stfld      string Linq101Aggregates01/shortestWord@52::w
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 46,60 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Aggregates01/shortestWord@52::w
      IL_0082:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Aggregates01/shortestWord@52::w
      .line 52,52 : 28,45 ''
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Aggregates01/shortestWord@52::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 28,45 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method shortestWord@52::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Aggregates01/shortestWord@52::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 52,52 : 28,45 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method shortestWord@52::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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
    } // end of method shortestWord@52::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0007:  ret
    } // end of method shortestWord@52::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(string,
                                                                                    string,
                                                                                    class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                    int32,
                                                                                    string)
      IL_000b:  ret
    } // end of method shortestWord@52::GetFreshEnumerator

  } // end of class shortestWord@52

  .class auto ansi serializable nested assembly beforefieldinit 'shortestWord@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'shortestWord@52-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 52,52 : 52,60 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0007:  ret
    } // end of method 'shortestWord@52-1'::Invoke

  } // end of class 'shortestWord@52-1'

  .class auto ansi serializable nested assembly beforefieldinit categories2@57
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories2@57::builder@
      IL_000d:  ret
    } // end of method categories2@57::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 57,57 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 58,58 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories2@57::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories2@57::Invoke

  } // end of class categories2@57

  .class auto ansi serializable nested assembly beforefieldinit 'categories2@58-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories2@58-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 58,58 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories2@58-1'::Invoke

  } // end of class 'categories2@58-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories2@58-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories2@58-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 58,58 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories2@58-2'::Invoke

  } // end of class 'categories2@58-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname min@59
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method min@59::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/min@59::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 59,59 : 27,40 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 27,40 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      .line 59,59 : 27,40 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 41,58 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      .line 59,59 : 27,40 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 27,40 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method min@59::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/min@59::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/min@59::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/min@59::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/min@59::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 59,59 : 27,40 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method min@59::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/min@59::pc
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
    } // end of method min@59::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0007:  ret
    } // end of method min@59::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method min@59::GetFreshEnumerator

  } // end of class min@59

  .class auto ansi serializable nested assembly beforefieldinit 'min@59-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'min@59-1'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 59,59 : 47,58 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'min@59-1'::Invoke

  } // end of class 'min@59-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories2@58-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories2@58-3'::builder@
      IL_000d:  ret
    } // end of method 'categories2@58-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       58 (0x3a)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 58,58 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldc.i4.0
      IL_000f:  ldnull
      IL_0010:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001a:  newobj     instance void Linq101Aggregates01/'min@59-1'::.ctor()
      IL_001f:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0024:  stloc.1
      .line 60,60 : 9,28 ''
      IL_0025:  ldarg.0
      IL_0026:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories2@58-3'::builder@
      IL_002b:  ldloc.0
      IL_002c:  ldloc.1
      IL_002d:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_0032:  tail.
      IL_0034:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_0039:  ret
    } // end of method 'categories2@58-3'::Invoke

  } // end of class 'categories2@58-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories2@60-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories2@60-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal> 
            Invoke(class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal> tupledArg) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min)
      .line 60,60 : 17,27 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0015:  ldloc.1
      IL_0016:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001b:  ret
    } // end of method 'categories2@60-4'::Invoke

  } // end of class 'categories2@60-4'

  .class auto ansi serializable nested assembly beforefieldinit categories3@66
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories3@66::builder@
      IL_000d:  ret
    } // end of method categories3@66::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 66,66 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 67,67 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories3@66::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories3@66::Invoke

  } // end of class categories3@66

  .class auto ansi serializable nested assembly beforefieldinit 'categories3@67-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories3@67-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 67,67 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories3@67-1'::Invoke

  } // end of class 'categories3@67-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories3@67-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories3@67-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 67,67 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories3@67-2'::Invoke

  } // end of class 'categories3@67-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'min@68-2'
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method 'min@68-2'::.ctor

    .method assembly hidebysig instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product arg) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 68,68 : 46,57 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'min@68-2'::Invoke

  } // end of class 'min@68-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname cheapestProducts@69
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method cheapestProducts@69::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 69,69 : 40,53 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 40,53 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      .line 69,69 : 40,53 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 54,79 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      .line 69,69 : 40,53 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 40,53 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method cheapestProducts@69::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 69,69 : 40,53 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method cheapestProducts@69::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
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
    } // end of method cheapestProducts@69::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0007:  ret
    } // end of method cheapestProducts@69::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method cheapestProducts@69::GetFreshEnumerator

  } // end of class cheapestProducts@69

  .class auto ansi serializable nested assembly beforefieldinit 'cheapestProducts@69-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [mscorlib]System.Decimal min
    .method assembly specialname rtspecialname 
            instance void  .ctor(valuetype [mscorlib]System.Decimal min) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'cheapestProducts@69-1'::min
      IL_000d:  ret
    } // end of method 'cheapestProducts@69-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 69,69 : 61,78 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0007:  ldarg.0
      IL_0008:  ldfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'cheapestProducts@69-1'::min
      IL_000d:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_0012:  ret
    } // end of method 'cheapestProducts@69-1'::Invoke

  } // end of class 'cheapestProducts@69-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories3@67-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories3@67-3'::builder@
      IL_000d:  ret
    } // end of method 'categories3@67-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       88 (0x58)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> cheapestProducts,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 67,67 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 68,68 : 9,58 ''
      IL_0003:  ldloc.0
      IL_0004:  newobj     instance void Linq101Aggregates01/'min@68-2'::.ctor()
      IL_0009:  ldftn      instance valuetype [mscorlib]System.Decimal Linq101Aggregates01/'min@68-2'::Invoke(class [Utils]Utils/Product)
      IL_000f:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                             native int)
      IL_0014:  call       valuetype [mscorlib]System.Decimal [System.Core]System.Linq.Enumerable::Min<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                   class [mscorlib]System.Func`2<!!0,valuetype [mscorlib]System.Decimal>)
      IL_0019:  stloc.1
      .line 69,69 : 13,29 ''
      IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_001f:  stloc.3
      IL_0020:  ldloc.3
      IL_0021:  ldloc.0
      IL_0022:  ldnull
      IL_0023:  ldnull
      IL_0024:  ldnull
      IL_0025:  ldc.i4.0
      IL_0026:  ldnull
      IL_0027:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_002c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0031:  ldloc.1
      IL_0032:  newobj     instance void Linq101Aggregates01/'cheapestProducts@69-1'::.ctor(valuetype [mscorlib]System.Decimal)
      IL_0037:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_003c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0041:  stloc.2
      .line 70,70 : 9,41 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories3@67-3'::builder@
      IL_0048:  ldloc.0
      IL_0049:  ldloc.1
      IL_004a:  ldloc.2
      IL_004b:  newobj     instance void class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_0050:  tail.
      IL_0052:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0057:  ret
    } // end of method 'categories3@67-3'::Invoke

  } // end of class 'categories3@67-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories3@70-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories3@70-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      // Code size       35 (0x23)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> cheapestProducts)
      .line 70,70 : 17,40 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  nop
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001c:  ldloc.2
      IL_001d:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0022:  ret
    } // end of method 'categories3@70-4'::Invoke

  } // end of class 'categories3@70-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxNum@74
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 n
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
            instance void  .ctor(int32 _arg1,
                                 int32 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/maxNum@74::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method maxNum@74::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 74,74 : 22,41 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 22,41 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0063:  stfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      .line 74,74 : 22,41 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      IL_006f:  stfld      int32 Linq101Aggregates01/maxNum@74::n
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 42,49 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      int32 Linq101Aggregates01/maxNum@74::n
      IL_0082:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.0
      IL_008b:  stfld      int32 Linq101Aggregates01/maxNum@74::n
      .line 74,74 : 22,41 ''
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.0
      IL_0092:  stfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 22,41 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.0
      IL_00bd:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method maxNum@74::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101Aggregates01/maxNum@74::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 74,74 : 22,41 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method maxNum@74::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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
    } // end of method maxNum@74::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0007:  ret
    } // end of method maxNum@74::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000b:  ret
    } // end of method maxNum@74::GetFreshEnumerator

  } // end of class maxNum@74

  .class auto ansi serializable nested assembly beforefieldinit 'maxNum@74-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'maxNum@74-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 74,74 : 48,49 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'maxNum@74-1'::Invoke

  } // end of class 'maxNum@74-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname longestLength@77
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string w
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string w,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Aggregates01/longestLength@77::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Aggregates01/longestLength@77::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method longestLength@77::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 77,77 : 29,46 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 29,46 ''
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Aggregates01/longestLength@77::_arg1
      .line 77,77 : 29,46 ''
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Aggregates01/longestLength@77::_arg1
      IL_006f:  stfld      string Linq101Aggregates01/longestLength@77::w
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 47,61 ''
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Aggregates01/longestLength@77::w
      IL_0082:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Aggregates01/longestLength@77::w
      .line 77,77 : 29,46 ''
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Aggregates01/longestLength@77::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 29,46 ''
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method longestLength@77::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Aggregates01/longestLength@77::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 77,77 : 29,46 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method longestLength@77::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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
    } // end of method longestLength@77::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Aggregates01/longestLength@77::current
      IL_0007:  ret
    } // end of method longestLength@77::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(string,
                                                                                     string,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                     int32,
                                                                                     string)
      IL_000b:  ret
    } // end of method longestLength@77::GetFreshEnumerator

  } // end of class longestLength@77

  .class auto ansi serializable nested assembly beforefieldinit 'longestLength@77-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'longestLength@77-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 77,77 : 53,61 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0007:  ret
    } // end of method 'longestLength@77-1'::Invoke

  } // end of class 'longestLength@77-1'

  .class auto ansi serializable nested assembly beforefieldinit categories4@82
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories4@82::builder@
      IL_000d:  ret
    } // end of method categories4@82::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 82,82 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 83,83 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories4@82::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories4@82::Invoke

  } // end of class categories4@82

  .class auto ansi serializable nested assembly beforefieldinit 'categories4@83-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories4@83-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 83,83 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories4@83-1'::Invoke

  } // end of class 'categories4@83-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories4@83-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories4@83-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 83,83 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories4@83-2'::Invoke

  } // end of class 'categories4@83-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensivePrice@84
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method mostExpensivePrice@84::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 84,84 : 42,55 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 42,55 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      .line 84,84 : 42,55 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 56,73 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      .line 84,84 : 42,55 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 42,55 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method mostExpensivePrice@84::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 84,84 : 42,55 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method mostExpensivePrice@84::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
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
    } // end of method mostExpensivePrice@84::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0007:  ret
    } // end of method mostExpensivePrice@84::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method mostExpensivePrice@84::GetFreshEnumerator

  } // end of class mostExpensivePrice@84

  .class auto ansi serializable nested assembly beforefieldinit 'mostExpensivePrice@84-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'mostExpensivePrice@84-1'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 84,84 : 62,73 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'mostExpensivePrice@84-1'::Invoke

  } // end of class 'mostExpensivePrice@84-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories4@83-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories4@83-3'::builder@
      IL_000d:  ret
    } // end of method 'categories4@83-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       58 (0x3a)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal mostExpensivePrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 83,83 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldc.i4.0
      IL_000f:  ldnull
      IL_0010:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001a:  newobj     instance void Linq101Aggregates01/'mostExpensivePrice@84-1'::.ctor()
      IL_001f:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0024:  stloc.1
      .line 85,85 : 9,43 ''
      IL_0025:  ldarg.0
      IL_0026:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories4@83-3'::builder@
      IL_002b:  ldloc.0
      IL_002c:  ldloc.1
      IL_002d:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_0032:  tail.
      IL_0034:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_0039:  ret
    } // end of method 'categories4@83-3'::Invoke

  } // end of class 'categories4@83-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories4@85-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories4@85-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal> 
            Invoke(class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal> tupledArg) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal mostExpensivePrice)
      .line 85,85 : 17,42 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0015:  ldloc.1
      IL_0016:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001b:  ret
    } // end of method 'categories4@85-4'::Invoke

  } // end of class 'categories4@85-4'

  .class auto ansi serializable nested assembly beforefieldinit categories5@91
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories5@91::builder@
      IL_000d:  ret
    } // end of method categories5@91::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 91,91 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 92,92 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories5@91::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories5@91::Invoke

  } // end of class categories5@91

  .class auto ansi serializable nested assembly beforefieldinit 'categories5@92-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories5@92-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 92,92 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories5@92-1'::Invoke

  } // end of class 'categories5@92-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories5@92-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories5@92-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 92,92 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories5@92-2'::Invoke

  } // end of class 'categories5@92-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxPrice@93
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method maxPrice@93::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 93,93 : 32,45 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 32,45 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      .line 93,93 : 32,45 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 46,63 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      .line 93,93 : 32,45 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 32,45 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method maxPrice@93::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 93,93 : 32,45 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method maxPrice@93::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
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
    } // end of method maxPrice@93::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0007:  ret
    } // end of method maxPrice@93::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [Utils]Utils/Product,
                                                                                class [Utils]Utils/Product,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method maxPrice@93::GetFreshEnumerator

  } // end of class maxPrice@93

  .class auto ansi serializable nested assembly beforefieldinit 'maxPrice@93-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'maxPrice@93-1'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 93,93 : 52,63 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'maxPrice@93-1'::Invoke

  } // end of class 'maxPrice@93-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensiveProducts@94
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg4
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg4,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method mostExpensiveProducts@94::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 94,94 : 45,58 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 45,58 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      .line 94,94 : 45,58 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 59,89 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      .line 94,94 : 45,58 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 45,58 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method mostExpensiveProducts@94::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 94,94 : 45,58 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method mostExpensiveProducts@94::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
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
    } // end of method mostExpensiveProducts@94::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0007:  ret
    } // end of method mostExpensiveProducts@94::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method mostExpensiveProducts@94::GetFreshEnumerator

  } // end of class mostExpensiveProducts@94

  .class auto ansi serializable nested assembly beforefieldinit 'mostExpensiveProducts@94-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [mscorlib]System.Decimal maxPrice
    .method assembly specialname rtspecialname 
            instance void  .ctor(valuetype [mscorlib]System.Decimal maxPrice) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'mostExpensiveProducts@94-1'::maxPrice
      IL_000d:  ret
    } // end of method 'mostExpensiveProducts@94-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 94,94 : 66,88 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0007:  ldarg.0
      IL_0008:  ldfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'mostExpensiveProducts@94-1'::maxPrice
      IL_000d:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_0012:  ret
    } // end of method 'mostExpensiveProducts@94-1'::Invoke

  } // end of class 'mostExpensiveProducts@94-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories5@92-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories5@92-3'::builder@
      IL_000d:  ret
    } // end of method 'categories5@92-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       101 (0x65)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal maxPrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> mostExpensiveProducts,
               [4] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_4)
      .line 92,92 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldc.i4.0
      IL_000f:  ldnull
      IL_0010:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [Utils]Utils/Product,
                                                                                class [Utils]Utils/Product,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001a:  newobj     instance void Linq101Aggregates01/'maxPrice@93-1'::.ctor()
      IL_001f:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0024:  stloc.1
      .line 94,94 : 13,34 ''
      IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_002a:  stloc.s    V_4
      IL_002c:  ldloc.s    V_4
      IL_002e:  ldloc.0
      IL_002f:  ldnull
      IL_0030:  ldnull
      IL_0031:  ldnull
      IL_0032:  ldc.i4.0
      IL_0033:  ldnull
      IL_0034:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0039:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_003e:  ldloc.1
      IL_003f:  newobj     instance void Linq101Aggregates01/'mostExpensiveProducts@94-1'::.ctor(valuetype [mscorlib]System.Decimal)
      IL_0044:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0049:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_004e:  stloc.3
      .line 95,95 : 9,46 ''
      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories5@92-3'::builder@
      IL_0055:  ldloc.0
      IL_0056:  ldloc.1
      IL_0057:  ldloc.3
      IL_0058:  newobj     instance void class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_005d:  tail.
      IL_005f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0064:  ret
    } // end of method 'categories5@92-3'::Invoke

  } // end of class 'categories5@92-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories5@95-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories5@95-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      // Code size       35 (0x23)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal maxPrice,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> mostExpensiveProducts)
      .line 95,95 : 17,45 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  nop
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001c:  ldloc.2
      IL_001d:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0022:  ret
    } // end of method 'categories5@95-4'::Invoke

  } // end of class 'categories5@95-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averageNum@100
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public float64 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public float64 n
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public float64 current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(float64 _arg1,
                                 float64 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> 'enum',
                                 int32 pc,
                                 float64 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      float64 Linq101Aggregates01/averageNum@100::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>::.ctor()
      IL_002b:  ret
    } // end of method averageNum@100::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>& next) cil managed
    {
      // Code size       223 (0xdf)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_00ad

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00ce

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 100,100 : 26,46 ''
      IL_0031:  ldarg.0
      IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
      IL_0037:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
      IL_003c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
      IL_0041:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.1
      IL_0048:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 26,46 ''
      IL_004d:  ldarg.0
      IL_004e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0053:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0058:  brfalse.s  IL_00ad

      IL_005a:  ldarg.0
      IL_005b:  ldarg.0
      IL_005c:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0061:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0066:  stfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      .line 100,100 : 26,46 ''
      IL_006b:  ldarg.0
      IL_006c:  ldarg.0
      IL_006d:  ldfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      IL_0072:  stfld      float64 Linq101Aggregates01/averageNum@100::n
      IL_0077:  ldarg.0
      IL_0078:  ldc.i4.2
      IL_0079:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 47,58 ''
      IL_007e:  ldarg.0
      IL_007f:  ldarg.0
      IL_0080:  ldfld      float64 Linq101Aggregates01/averageNum@100::n
      IL_0085:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_008a:  ldc.i4.1
      IL_008b:  ret

      IL_008c:  ldarg.0
      IL_008d:  ldc.r8     0.0
      IL_0096:  stfld      float64 Linq101Aggregates01/averageNum@100::n
      .line 100,100 : 26,46 ''
      IL_009b:  ldarg.0
      IL_009c:  ldc.r8     0.0
      IL_00a5:  stfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      .line 100001,100001 : 0,0 ''
      IL_00aa:  nop
      IL_00ab:  br.s       IL_004d

      IL_00ad:  ldarg.0
      IL_00ae:  ldc.i4.3
      IL_00af:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 26,46 ''
      IL_00b4:  ldarg.0
      IL_00b5:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_00ba:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
      IL_00bf:  nop
      IL_00c0:  ldarg.0
      IL_00c1:  ldnull
      IL_00c2:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_00c7:  ldarg.0
      IL_00c8:  ldc.i4.3
      IL_00c9:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_00ce:  ldarg.0
      IL_00cf:  ldc.r8     0.0
      IL_00d8:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_00dd:  ldc.i4.0
      IL_00de:  ret
    } // end of method averageNum@100::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       158 (0x9e)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0091

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.r8     0.0
        IL_0074:  stfld      float64 Linq101Aggregates01/averageNum@100::current
        IL_0079:  ldnull
        IL_007a:  stloc.1
        IL_007b:  leave.s    IL_0089

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_007d:  castclass  [mscorlib]System.Exception
        IL_0082:  stloc.2
        .line 100,100 : 26,46 ''
        IL_0083:  ldloc.2
        IL_0084:  stloc.0
        IL_0085:  ldnull
        IL_0086:  stloc.1
        IL_0087:  leave.s    IL_0089

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0089:  ldloc.1
      IL_008a:  pop
      .line 100001,100001 : 0,0 ''
      IL_008b:  nop
      IL_008c:  br         IL_0002

      IL_0091:  ldloc.0
      IL_0092:  ldnull
      IL_0093:  cgt.un
      IL_0095:  brfalse.s  IL_0099

      IL_0097:  br.s       IL_009b

      IL_0099:  br.s       IL_009d

      .line 100001,100001 : 0,0 ''
      IL_009b:  ldloc.0
      IL_009c:  throw

      .line 100001,100001 : 0,0 ''
      IL_009d:  ret
    } // end of method averageNum@100::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
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
    } // end of method averageNum@100::get_CheckClose

    .method public strict virtual instance float64 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0007:  ret
    } // end of method averageNum@100::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       36 (0x24)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.r8     0.0
      IL_000a:  ldc.r8     0.0
      IL_0013:  ldnull
      IL_0014:  ldc.i4.0
      IL_0015:  ldc.r8     0.0
      IL_001e:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(float64,
                                                                                   float64,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                   int32,
                                                                                   float64)
      IL_0023:  ret
    } // end of method averageNum@100::GetFreshEnumerator

  } // end of class averageNum@100

  .class auto ansi serializable nested assembly beforefieldinit 'averageNum@100-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'averageNum@100-1'::.ctor

    .method public strict virtual instance float64 
            Invoke(float64 n) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 100,100 : 57,58 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'averageNum@100-1'::Invoke

  } // end of class 'averageNum@100-1'

  .class auto ansi serializable nested assembly beforefieldinit averageLength@105
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/averageLength@105::builder@
      IL_000d:  ret
    } // end of method averageLength@105::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,object> 
            Invoke(string _arg1) cil managed
    {
      // Code size       32 (0x20)
      .maxstack  7
      .locals init ([0] string w,
               [1] float64 wl)
      .line 105,105 : 9,26 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 106,106 : 9,35 ''
      IL_0003:  ldloc.0
      IL_0004:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0009:  conv.r8
      IL_000a:  stloc.1
      .line 107,107 : 9,21 ''
      IL_000b:  ldarg.0
      IL_000c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/averageLength@105::builder@
      IL_0011:  ldloc.0
      IL_0012:  ldloc.1
      IL_0013:  newobj     instance void class [mscorlib]System.Tuple`2<string,float64>::.ctor(!0,
                                                                                               !1)
      IL_0018:  tail.
      IL_001a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<string,float64>,object>(!!0)
      IL_001f:  ret
    } // end of method averageLength@105::Invoke

  } // end of class averageLength@105

  .class auto ansi serializable nested assembly beforefieldinit 'averageLength@107-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'averageLength@107-1'::.ctor

    .method public strict virtual instance float64 
            Invoke(class [mscorlib]System.Tuple`2<string,float64> tupledArg) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  5
      .locals init ([0] string w,
               [1] float64 wl)
      .line 107,107 : 19,21 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<string,float64>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<string,float64>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.1
      IL_0010:  ret
    } // end of method 'averageLength@107-1'::Invoke

  } // end of class 'averageLength@107-1'

  .class auto ansi serializable nested assembly beforefieldinit categories6@113
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories6@113::builder@
      IL_000d:  ret
    } // end of method categories6@113::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 113,113 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 114,114 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories6@113::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method categories6@113::Invoke

  } // end of class categories6@113

  .class auto ansi serializable nested assembly beforefieldinit 'categories6@114-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'categories6@114-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 114,114 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'categories6@114-1'::Invoke

  } // end of class 'categories6@114-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories6@114-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'categories6@114-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 114,114 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'categories6@114-2'::Invoke

  } // end of class 'categories6@114-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averagePrice@115
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [Utils]Utils/Product _arg3
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product x
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [Utils]Utils/Product _arg3,
                                 class [Utils]Utils/Product x,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       52 (0x34)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    'enum'
      IL_0018:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    pc
      IL_0020:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0025:  ldarg.0
      IL_0026:  ldarg.s    current
      IL_0028:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_002d:  ldarg.0
      IL_002e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0033:  ret
    } // end of method averagePrice@115::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       200 (0xc8)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0030

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_009e

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_008d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00bf

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 115,115 : 36,49 ''
      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0038:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0042:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 36,49 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0054:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0059:  brfalse.s  IL_009e

      IL_005b:  ldarg.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0062:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0067:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      .line 115,115 : 36,49 ''
      IL_006c:  ldarg.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      IL_0073:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.2
      IL_007a:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 50,71 ''
      IL_007f:  ldarg.0
      IL_0080:  ldarg.0
      IL_0081:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      IL_0086:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      .line 115,115 : 36,49 ''
      IL_0094:  ldarg.0
      IL_0095:  ldnull
      IL_0096:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      .line 100001,100001 : 0,0 ''
      IL_009b:  nop
      IL_009c:  br.s       IL_004e

      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 36,49 ''
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_00ab:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00b0:  nop
      IL_00b1:  ldarg.0
      IL_00b2:  ldnull
      IL_00b3:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_00b8:  ldarg.0
      IL_00b9:  ldc.i4.3
      IL_00ba:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_00bf:  ldarg.0
      IL_00c0:  ldnull
      IL_00c1:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_00c6:  ldc.i4.0
      IL_00c7:  ret
    } // end of method averagePrice@115::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 115,115 : 36,49 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method averagePrice@115::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
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
    } // end of method averagePrice@115::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0007:  ret
    } // end of method averagePrice@115::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  10
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldnull
      IL_000a:  ldc.i4.0
      IL_000b:  ldnull
      IL_000c:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0011:  ret
    } // end of method averagePrice@115::GetFreshEnumerator

  } // end of class averagePrice@115

  .class auto ansi serializable nested assembly beforefieldinit 'averagePrice@115-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'averagePrice@115-1'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 115,115 : 60,71 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'averagePrice@115-1'::Invoke

  } // end of class 'averagePrice@115-1'

  .class auto ansi serializable nested assembly beforefieldinit 'categories6@114-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories6@114-3'::builder@
      IL_000d:  ret
    } // end of method 'categories6@114-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       270 (0x10e)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal averagePrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               [4] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>> V_4,
               [5] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable> V_5,
               [6] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal> V_6,
               [7] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_7,
               [8] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_8,
               [9] valuetype [mscorlib]System.Decimal V_9,
               [10] valuetype [mscorlib]System.Decimal V_10,
               [11] int32 V_11,
               [12] valuetype [mscorlib]System.Decimal V_12,
               [13] int32 V_13,
               [14] class [mscorlib]System.IDisposable V_14)
      .line 114,114 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  stloc.3
      IL_000b:  ldloc.0
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldnull
      IL_000f:  ldc.i4.0
      IL_0010:  ldnull
      IL_0011:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001b:  newobj     instance void Linq101Aggregates01/'averagePrice@115-1'::.ctor()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                       !1)
      IL_0025:  stloc.s    V_4
      IL_0027:  ldloc.s    V_4
      IL_0029:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>>::get_Item1()
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_4
      IL_0032:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>>::get_Item2()
      IL_0037:  stloc.s    V_6
      IL_0039:  ldloc.s    V_5
      IL_003b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0040:  stloc.s    V_7
      IL_0042:  ldloc.s    V_7
      IL_0044:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0049:  brfalse.s  IL_004d

      IL_004b:  br.s       IL_0060

      .line 100001,100001 : 0,0 ''
      IL_004d:  ldstr      "source"
      IL_0052:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
      IL_0057:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_005c:  pop
      .line 100001,100001 : 0,0 ''
      IL_005d:  nop
      IL_005e:  br.s       IL_0061

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0060:  nop
      IL_0061:  ldloc.s    V_7
      IL_0063:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0068:  stloc.s    V_8
      .try
      {
        IL_006a:  ldc.i4.0
        IL_006b:  ldc.i4.0
        IL_006c:  ldc.i4.0
        IL_006d:  ldc.i4.0
        IL_006e:  ldc.i4.0
        IL_006f:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                           int32,
                                                                           int32,
                                                                           bool,
                                                                           uint8)
        IL_0074:  stloc.s    V_10
        IL_0076:  ldc.i4.0
        IL_0077:  stloc.s    V_11
        IL_0079:  ldloc.s    V_8
        IL_007b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0080:  brfalse.s  IL_00a2

        IL_0082:  ldloc.s    V_10
        IL_0084:  ldloc.s    V_6
        IL_0086:  ldloc.s    V_8
        IL_0088:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_008d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::Invoke(!0)
        IL_0092:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                      valuetype [mscorlib]System.Decimal)
        IL_0097:  stloc.s    V_10
        .line 115,115 : 50,71 ''
        IL_0099:  ldloc.s    V_11
        IL_009b:  ldc.i4.1
        IL_009c:  add
        IL_009d:  stloc.s    V_11
        .line 100001,100001 : 0,0 ''
        IL_009f:  nop
        IL_00a0:  br.s       IL_0079

        IL_00a2:  ldloc.s    V_11
        IL_00a4:  brtrue.s   IL_00a8

        IL_00a6:  br.s       IL_00aa

        IL_00a8:  br.s       IL_00bd

        .line 100001,100001 : 0,0 ''
        IL_00aa:  ldstr      "source"
        IL_00af:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
        IL_00b4:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
        IL_00b9:  pop
        .line 100001,100001 : 0,0 ''
        IL_00ba:  nop
        IL_00bb:  br.s       IL_00be

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_00bd:  nop
        IL_00be:  ldloc.s    V_10
        IL_00c0:  stloc.s    V_12
        IL_00c2:  ldloc.s    V_11
        IL_00c4:  stloc.s    V_13
        IL_00c6:  ldloc.s    V_12
        IL_00c8:  ldloc.s    V_13
        IL_00ca:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Convert::ToDecimal(int32)
        IL_00cf:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Divide(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
        IL_00d4:  stloc.s    V_9
        IL_00d6:  leave.s    IL_00f6

      }  // end .try
      finally
      {
        IL_00d8:  ldloc.s    V_8
        IL_00da:  isinst     [mscorlib]System.IDisposable
        IL_00df:  stloc.s    V_14
        IL_00e1:  ldloc.s    V_14
        IL_00e3:  brfalse.s  IL_00e7

        IL_00e5:  br.s       IL_00e9

        IL_00e7:  br.s       IL_00f3

        .line 100001,100001 : 0,0 ''
        IL_00e9:  ldloc.s    V_14
        IL_00eb:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00f0:  ldnull
        IL_00f1:  pop
        IL_00f2:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_00f3:  ldnull
        IL_00f4:  pop
        IL_00f5:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_00f6:  ldloc.s    V_9
      IL_00f8:  stloc.1
      .line 116,116 : 9,37 ''
      IL_00f9:  ldarg.0
      IL_00fa:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories6@114-3'::builder@
      IL_00ff:  ldloc.0
      IL_0100:  ldloc.1
      IL_0101:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_0106:  tail.
      IL_0108:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_010d:  ret
    } // end of method 'categories6@114-3'::Invoke

  } // end of class 'categories6@114-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories6@116-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'categories6@116-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal> 
            Invoke(class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal> tupledArg) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal averagePrice)
      .line 116,116 : 17,36 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0015:  ldloc.1
      IL_0016:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001b:  ret
    } // end of method 'categories6@116-4'::Invoke

  } // end of class 'categories6@116-4'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_factorsOf300() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::factorsOf300@8
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_factorsOf300

  .method public specialname static int32 
          get_uniqueFactors() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::uniqueFactors@10
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_uniqueFactors

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers@17
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numbers

  .method public specialname static int32 
          get_numSum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numSum

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_words

  .method public specialname static int32 
          get_totalChars() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_totalChars

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_products

  .method public specialname static class [mscorlib]System.Tuple`2<string,int32>[] 
          get_categories() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories

  .method public specialname static int32 
          get_minNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_minNum

  .method public specialname static int32 
          get_shortestWord() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_shortestWord

  .method public specialname static class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] 
          get_categories2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories2

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] 
          get_categories3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories3

  .method public specialname static int32 
          get_maxNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_maxNum

  .method public specialname static int32 
          get_longestLength() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_longestLength

  .method public specialname static class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] 
          get_categories4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories4

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] 
          get_categories5() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories5

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> 
          get_numbers2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numbers2

  .method public specialname static float64 
          get_averageNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_averageNum

  .method public specialname static float64 
          get_averageLength() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_averageLength

  .method public specialname static class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] 
          get_categories6() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories6

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          factorsOf300()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
  } // end of property Linq101Aggregates01::factorsOf300
  .property int32 uniqueFactors()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_uniqueFactors()
  } // end of property Linq101Aggregates01::uniqueFactors
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
  } // end of property Linq101Aggregates01::numbers
  .property int32 numSum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_numSum()
  } // end of property Linq101Aggregates01::numSum
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
  } // end of property Linq101Aggregates01::words
  .property int32 totalChars()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_totalChars()
  } // end of property Linq101Aggregates01::totalChars
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
  } // end of property Linq101Aggregates01::products
  .property class [mscorlib]System.Tuple`2<string,int32>[]
          categories()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,int32>[] Linq101Aggregates01::get_categories()
  } // end of property Linq101Aggregates01::categories
  .property int32 minNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_minNum()
  } // end of property Linq101Aggregates01::minNum
  .property int32 shortestWord()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_shortestWord()
  } // end of property Linq101Aggregates01::shortestWord
  .property class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[]
          categories2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] Linq101Aggregates01::get_categories2()
  } // end of property Linq101Aggregates01::categories2
  .property class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] Linq101Aggregates01::get_categories3()
  } // end of property Linq101Aggregates01::categories3
  .property int32 maxNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_maxNum()
  } // end of property Linq101Aggregates01::maxNum
  .property int32 longestLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_longestLength()
  } // end of property Linq101Aggregates01::longestLength
  .property class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[]
          categories4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] Linq101Aggregates01::get_categories4()
  } // end of property Linq101Aggregates01::categories4
  .property class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories5()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] Linq101Aggregates01::get_categories5()
  } // end of property Linq101Aggregates01::categories5
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>
          numbers2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
  } // end of property Linq101Aggregates01::numbers2
  .property float64 averageNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 Linq101Aggregates01::get_averageNum()
  } // end of property Linq101Aggregates01::averageNum
  .property float64 averageLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 Linq101Aggregates01::get_averageLength()
  } // end of property Linq101Aggregates01::averageLength
  .property class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[]
          categories6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] Linq101Aggregates01::get_categories6()
  } // end of property Linq101Aggregates01::categories6
} // end of class Linq101Aggregates01

.class private abstract auto ansi sealed '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> factorsOf300@8
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 uniqueFactors@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@17
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 numSum@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@26
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 totalChars@28
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@35
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,int32>[] categories@37
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 minNum@49
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 shortestWord@52
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories2@55
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories3@64
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 maxNum@74
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 longestLength@77
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories4@80
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories5@89
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> numbers2@99
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageNum@100
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageLength@103
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories6@111
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1965 (0x7ad)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> factorsOf300,
             [1] int32 uniqueFactors,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [3] int32 numSum,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [5] int32 totalChars,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [7] class [mscorlib]System.Tuple`2<string,int32>[] categories,
             [8] int32 minNum,
             [9] int32 shortestWord,
             [10] class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories2,
             [11] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories3,
             [12] int32 maxNum,
             [13] int32 longestLength,
             [14] class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories4,
             [15] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories5,
             [16] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> numbers2,
             [17] float64 averageNum,
             [18] float64 averageLength,
             [19] class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] categories6,
             [20] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
             [21] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_21,
             [22] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_22,
             [23] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>> V_23,
             [24] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable> V_24,
             [25] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_25,
             [26] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_26,
             [27] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_27,
             [28] int32 V_28,
             [29] int32 V_29,
             [30] class [mscorlib]System.IDisposable V_30,
             [31] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             [32] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_32,
             [33] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>> V_33,
             [34] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable> V_34,
             [35] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_35,
             [36] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_36,
             [37] class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_37,
             [38] int32 V_38,
             [39] int32 V_39,
             [40] class [mscorlib]System.IDisposable V_40,
             [41] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             [42] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_42,
             [43] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43,
             [44] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_44,
             [45] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_45,
             [46] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_46,
             [47] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47,
             [48] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_48,
             [49] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_49,
             [50] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_50,
             [51] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_51,
             [52] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>> V_52,
             [53] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable> V_53,
             [54] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_54,
             [55] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_55,
             [56] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_56,
             [57] float64 V_57,
             [58] float64 V_58,
             [59] int32 V_59,
             [60] float64 V_60,
             [61] int32 V_61,
             [62] class [mscorlib]System.IDisposable V_62,
             [63] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_63,
             [64] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_64,
             [65] class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>> V_65,
             [66] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable> V_66,
             [67] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64> V_67,
             [68] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>> V_68,
             [69] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>> V_69,
             [70] float64 V_70,
             [71] float64 V_71,
             [72] int32 V_72,
             [73] float64 V_73,
             [74] int32 V_74,
             [75] class [mscorlib]System.IDisposable V_75,
             [76] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_76)
    .line 8,8 : 1,31 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.2
    IL_0003:  ldc.i4.3
    IL_0004:  ldc.i4.5
    IL_0005:  ldc.i4.5
    IL_0006:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  dup
    IL_0025:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::factorsOf300@8
    IL_002a:  stloc.0
    .line 10,14 : 1,20 ''
    IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0030:  stloc.s    builder@
    IL_0032:  ldloc.s    builder@
    IL_0034:  ldc.i4.0
    IL_0035:  ldc.i4.0
    IL_0036:  ldnull
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4.0
    IL_0039:  newobj     instance void Linq101Aggregates01/uniqueFactors@12::.ctor(int32,
                                                                                   int32,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                   int32,
                                                                                   int32)
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0043:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0048:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_004d:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0052:  dup
    IL_0053:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::uniqueFactors@10
    IL_0058:  stloc.1
    .line 17,17 : 1,47 ''
    IL_0059:  ldc.i4.5
    IL_005a:  ldc.i4.4
    IL_005b:  ldc.i4.1
    IL_005c:  ldc.i4.3
    IL_005d:  ldc.i4.s   9
    IL_005f:  ldc.i4.8
    IL_0060:  ldc.i4.6
    IL_0061:  ldc.i4.7
    IL_0062:  ldc.i4.2
    IL_0063:  ldc.i4.0
    IL_0064:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0069:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0073:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0078:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0082:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0087:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0091:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0096:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009b:  dup
    IL_009c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers@17
    IL_00a1:  stloc.2
    IL_00a2:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a7:  stloc.s    V_21
    IL_00a9:  ldloc.s    V_21
    IL_00ab:  stloc.s    V_22
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldc.i4.0
    IL_00af:  ldnull
    IL_00b0:  ldc.i4.0
    IL_00b1:  ldc.i4.0
    IL_00b2:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_00b7:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00bc:  newobj     instance void Linq101Aggregates01/'numSum@22-1'::.ctor()
    IL_00c1:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::.ctor(!0,
                                                                                                                                                                                                                                                              !1)
    IL_00c6:  stloc.s    V_23
    IL_00c8:  ldloc.s    V_23
    IL_00ca:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::get_Item1()
    IL_00cf:  stloc.s    V_24
    IL_00d1:  ldloc.s    V_23
    IL_00d3:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::get_Item2()
    IL_00d8:  stloc.s    V_25
    IL_00da:  ldloc.s    V_24
    IL_00dc:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00e1:  stloc.s    V_26
    IL_00e3:  ldloc.s    V_26
    IL_00e5:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_00ea:  stloc.s    V_27
    .try
    {
      IL_00ec:  ldc.i4.0
      IL_00ed:  stloc.s    V_29
      IL_00ef:  ldloc.s    V_27
      IL_00f1:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00f6:  brfalse.s  IL_010e

      .line 22,22 : 9,16 ''
      IL_00f8:  ldloc.s    V_29
      IL_00fa:  ldloc.s    V_25
      IL_00fc:  ldloc.s    V_27
      IL_00fe:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0103:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_0108:  add.ovf
      IL_0109:  stloc.s    V_29
      .line 100001,100001 : 0,0 ''
      IL_010b:  nop
      IL_010c:  br.s       IL_00ef

      IL_010e:  ldloc.s    V_29
      IL_0110:  stloc.s    V_28
      IL_0112:  leave.s    IL_0132

    }  // end .try
    finally
    {
      IL_0114:  ldloc.s    V_27
      IL_0116:  isinst     [mscorlib]System.IDisposable
      IL_011b:  stloc.s    V_30
      IL_011d:  ldloc.s    V_30
      IL_011f:  brfalse.s  IL_0123

      IL_0121:  br.s       IL_0125

      IL_0123:  br.s       IL_012f

      .line 100001,100001 : 0,0 ''
      IL_0125:  ldloc.s    V_30
      IL_0127:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_012c:  ldnull
      IL_012d:  pop
      IL_012e:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_012f:  ldnull
      IL_0130:  pop
      IL_0131:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0132:  ldloc.s    V_28
    IL_0134:  dup
    IL_0135:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_013a:  stloc.3
    .line 26,26 : 1,45 ''
    IL_013b:  ldstr      "cherry"
    IL_0140:  ldstr      "apple"
    IL_0145:  ldstr      "blueberry"
    IL_014a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_014f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0154:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0159:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_015e:  dup
    IL_015f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_0164:  stloc.s    words
    IL_0166:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_016b:  stloc.s    V_31
    IL_016d:  ldloc.s    V_31
    IL_016f:  stloc.s    V_32
    IL_0171:  ldnull
    IL_0172:  ldnull
    IL_0173:  ldnull
    IL_0174:  ldc.i4.0
    IL_0175:  ldnull
    IL_0176:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_017b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0180:  newobj     instance void Linq101Aggregates01/'totalChars@31-1'::.ctor()
    IL_0185:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>>::.ctor(!0,
                                                                                                                                                                                                                                                                !1)
    IL_018a:  stloc.s    V_33
    IL_018c:  ldloc.s    V_33
    IL_018e:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>>::get_Item1()
    IL_0193:  stloc.s    V_34
    IL_0195:  ldloc.s    V_33
    IL_0197:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>>::get_Item2()
    IL_019c:  stloc.s    V_35
    IL_019e:  ldloc.s    V_34
    IL_01a0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01a5:  stloc.s    V_36
    IL_01a7:  ldloc.s    V_36
    IL_01a9:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_01ae:  stloc.s    V_37
    .try
    {
      IL_01b0:  ldc.i4.0
      IL_01b1:  stloc.s    V_39
      IL_01b3:  ldloc.s    V_37
      IL_01b5:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_01ba:  brfalse.s  IL_01d2

      .line 31,31 : 9,25 ''
      IL_01bc:  ldloc.s    V_39
      IL_01be:  ldloc.s    V_35
      IL_01c0:  ldloc.s    V_37
      IL_01c2:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_01c7:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_01cc:  add.ovf
      IL_01cd:  stloc.s    V_39
      .line 100001,100001 : 0,0 ''
      IL_01cf:  nop
      IL_01d0:  br.s       IL_01b3

      IL_01d2:  ldloc.s    V_39
      IL_01d4:  stloc.s    V_38
      IL_01d6:  leave.s    IL_01f6

    }  // end .try
    finally
    {
      IL_01d8:  ldloc.s    V_37
      IL_01da:  isinst     [mscorlib]System.IDisposable
      IL_01df:  stloc.s    V_40
      IL_01e1:  ldloc.s    V_40
      IL_01e3:  brfalse.s  IL_01e7

      IL_01e5:  br.s       IL_01e9

      IL_01e7:  br.s       IL_01f3

      .line 100001,100001 : 0,0 ''
      IL_01e9:  ldloc.s    V_40
      IL_01eb:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_01f0:  ldnull
      IL_01f1:  pop
      IL_01f2:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_01f3:  ldnull
      IL_01f4:  pop
      IL_01f5:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_01f6:  ldloc.s    V_38
    IL_01f8:  dup
    IL_01f9:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_01fe:  stloc.s    totalChars
    .line 35,35 : 1,32 ''
    IL_0200:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0205:  dup
    IL_0206:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_020b:  stloc.s    products
    .line 37,46 : 1,21 ''
    IL_020d:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0212:  stloc.s    V_41
    IL_0214:  ldloc.s    V_41
    IL_0216:  ldloc.s    V_41
    IL_0218:  ldloc.s    V_41
    IL_021a:  ldloc.s    V_41
    IL_021c:  ldloc.s    V_41
    IL_021e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0223:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0228:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_022d:  ldloc.s    V_41
    IL_022f:  newobj     instance void Linq101Aggregates01/categories@39::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0234:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0239:  newobj     instance void Linq101Aggregates01/'categories@40-1'::.ctor()
    IL_023e:  newobj     instance void Linq101Aggregates01/'categories@40-2'::.ctor()
    IL_0243:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0248:  ldloc.s    V_41
    IL_024a:  newobj     instance void Linq101Aggregates01/'categories@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_024f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0254:  newobj     instance void Linq101Aggregates01/'categories@45-4'::.ctor()
    IL_0259:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_025e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,int32>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0263:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0268:  dup
    IL_0269:  stsfld     class [mscorlib]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_026e:  stloc.s    categories
    IL_0270:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0275:  stloc.s    V_42
    IL_0277:  ldloc.s    V_42
    IL_0279:  ldc.i4.0
    IL_027a:  ldc.i4.0
    IL_027b:  ldnull
    IL_027c:  ldc.i4.0
    IL_027d:  ldc.i4.0
    IL_027e:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0283:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0288:  newobj     instance void Linq101Aggregates01/'minNum@49-1'::.ctor()
    IL_028d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0292:  dup
    IL_0293:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_0298:  stloc.s    minNum
    IL_029a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_029f:  stloc.s    V_43
    IL_02a1:  ldloc.s    V_43
    IL_02a3:  ldnull
    IL_02a4:  ldnull
    IL_02a5:  ldnull
    IL_02a6:  ldc.i4.0
    IL_02a7:  ldnull
    IL_02a8:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_02ad:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_02b2:  newobj     instance void Linq101Aggregates01/'shortestWord@52-1'::.ctor()
    IL_02b7:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02bc:  dup
    IL_02bd:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_02c2:  stloc.s    shortestWord
    .line 55,61 : 1,21 ''
    IL_02c4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02c9:  stloc.s    V_44
    IL_02cb:  ldloc.s    V_44
    IL_02cd:  ldloc.s    V_44
    IL_02cf:  ldloc.s    V_44
    IL_02d1:  ldloc.s    V_44
    IL_02d3:  ldloc.s    V_44
    IL_02d5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_02da:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_02df:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02e4:  ldloc.s    V_44
    IL_02e6:  newobj     instance void Linq101Aggregates01/categories2@57::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02eb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02f0:  newobj     instance void Linq101Aggregates01/'categories2@58-1'::.ctor()
    IL_02f5:  newobj     instance void Linq101Aggregates01/'categories2@58-2'::.ctor()
    IL_02fa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02ff:  ldloc.s    V_44
    IL_0301:  newobj     instance void Linq101Aggregates01/'categories2@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0306:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_030b:  newobj     instance void Linq101Aggregates01/'categories2@60-4'::.ctor()
    IL_0310:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0315:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_031a:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_031f:  dup
    IL_0320:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_0325:  stloc.s    categories2
    .line 64,71 : 1,21 ''
    IL_0327:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_032c:  stloc.s    V_45
    IL_032e:  ldloc.s    V_45
    IL_0330:  ldloc.s    V_45
    IL_0332:  ldloc.s    V_45
    IL_0334:  ldloc.s    V_45
    IL_0336:  ldloc.s    V_45
    IL_0338:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_033d:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0342:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0347:  ldloc.s    V_45
    IL_0349:  newobj     instance void Linq101Aggregates01/categories3@66::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_034e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0353:  newobj     instance void Linq101Aggregates01/'categories3@67-1'::.ctor()
    IL_0358:  newobj     instance void Linq101Aggregates01/'categories3@67-2'::.ctor()
    IL_035d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0362:  ldloc.s    V_45
    IL_0364:  newobj     instance void Linq101Aggregates01/'categories3@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0369:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_036e:  newobj     instance void Linq101Aggregates01/'categories3@70-4'::.ctor()
    IL_0373:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0378:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_037d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0382:  dup
    IL_0383:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_0388:  stloc.s    categories3
    IL_038a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_038f:  stloc.s    V_46
    IL_0391:  ldloc.s    V_46
    IL_0393:  ldc.i4.0
    IL_0394:  ldc.i4.0
    IL_0395:  ldnull
    IL_0396:  ldc.i4.0
    IL_0397:  ldc.i4.0
    IL_0398:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_039d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_03a2:  newobj     instance void Linq101Aggregates01/'maxNum@74-1'::.ctor()
    IL_03a7:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03ac:  dup
    IL_03ad:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_03b2:  stloc.s    maxNum
    IL_03b4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03b9:  stloc.s    V_47
    IL_03bb:  ldloc.s    V_47
    IL_03bd:  ldnull
    IL_03be:  ldnull
    IL_03bf:  ldnull
    IL_03c0:  ldc.i4.0
    IL_03c1:  ldnull
    IL_03c2:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(string,
                                                                                   string,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_03c7:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_03cc:  newobj     instance void Linq101Aggregates01/'longestLength@77-1'::.ctor()
    IL_03d1:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03d6:  dup
    IL_03d7:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_03dc:  stloc.s    longestLength
    .line 80,86 : 1,21 ''
    IL_03de:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03e3:  stloc.s    V_48
    IL_03e5:  ldloc.s    V_48
    IL_03e7:  ldloc.s    V_48
    IL_03e9:  ldloc.s    V_48
    IL_03eb:  ldloc.s    V_48
    IL_03ed:  ldloc.s    V_48
    IL_03ef:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_03f4:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_03f9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03fe:  ldloc.s    V_48
    IL_0400:  newobj     instance void Linq101Aggregates01/categories4@82::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0405:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_040a:  newobj     instance void Linq101Aggregates01/'categories4@83-1'::.ctor()
    IL_040f:  newobj     instance void Linq101Aggregates01/'categories4@83-2'::.ctor()
    IL_0414:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0419:  ldloc.s    V_48
    IL_041b:  newobj     instance void Linq101Aggregates01/'categories4@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0420:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0425:  newobj     instance void Linq101Aggregates01/'categories4@85-4'::.ctor()
    IL_042a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_042f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0434:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0439:  dup
    IL_043a:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_043f:  stloc.s    categories4
    .line 89,96 : 1,21 ''
    IL_0441:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0446:  stloc.s    V_49
    IL_0448:  ldloc.s    V_49
    IL_044a:  ldloc.s    V_49
    IL_044c:  ldloc.s    V_49
    IL_044e:  ldloc.s    V_49
    IL_0450:  ldloc.s    V_49
    IL_0452:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0457:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_045c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0461:  ldloc.s    V_49
    IL_0463:  newobj     instance void Linq101Aggregates01/categories5@91::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0468:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_046d:  newobj     instance void Linq101Aggregates01/'categories5@92-1'::.ctor()
    IL_0472:  newobj     instance void Linq101Aggregates01/'categories5@92-2'::.ctor()
    IL_0477:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_047c:  ldloc.s    V_49
    IL_047e:  newobj     instance void Linq101Aggregates01/'categories5@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0483:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0488:  newobj     instance void Linq101Aggregates01/'categories5@95-4'::.ctor()
    IL_048d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0492:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0497:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_049c:  dup
    IL_049d:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_04a2:  stloc.s    categories5
    .line 99,99 : 1,66 ''
    IL_04a4:  ldc.r8     5.
    IL_04ad:  ldc.r8     4.
    IL_04b6:  ldc.r8     1.
    IL_04bf:  ldc.r8     3.
    IL_04c8:  ldc.r8     9.
    IL_04d1:  ldc.r8     8.
    IL_04da:  ldc.r8     6.
    IL_04e3:  ldc.r8     7.
    IL_04ec:  ldc.r8     2.
    IL_04f5:  ldc.r8     0.0
    IL_04fe:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_0503:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0508:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_050d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0512:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0517:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_051c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0521:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0526:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_052b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0530:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0535:  dup
    IL_0536:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_053b:  stloc.s    numbers2
    IL_053d:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0542:  stloc.s    V_50
    IL_0544:  ldloc.s    V_50
    IL_0546:  stloc.s    V_51
    IL_0548:  ldc.r8     0.0
    IL_0551:  ldc.r8     0.0
    IL_055a:  ldnull
    IL_055b:  ldc.i4.0
    IL_055c:  ldc.r8     0.0
    IL_0565:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(float64,
                                                                                 float64,
                                                                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_056a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_056f:  newobj     instance void Linq101Aggregates01/'averageNum@100-1'::.ctor()
    IL_0574:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>>::.ctor(!0,
                                                                                                                                                                                                                                                                    !1)
    IL_0579:  stloc.s    V_52
    IL_057b:  ldloc.s    V_52
    IL_057d:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>>::get_Item1()
    IL_0582:  stloc.s    V_53
    IL_0584:  ldloc.s    V_52
    IL_0586:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>>::get_Item2()
    IL_058b:  stloc.s    V_54
    IL_058d:  ldloc.s    V_53
    IL_058f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0594:  stloc.s    V_55
    IL_0596:  ldloc.s    V_55
    IL_0598:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_059d:  brfalse.s  IL_05a1

    IL_059f:  br.s       IL_05b4

    .line 100001,100001 : 0,0 ''
    IL_05a1:  ldstr      "source"
    IL_05a6:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_05ab:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_05b0:  pop
    .line 100001,100001 : 0,0 ''
    IL_05b1:  nop
    IL_05b2:  br.s       IL_05b5

    .line 100001,100001 : 0,0 ''
    .line 100001,100001 : 0,0 ''
    IL_05b4:  nop
    IL_05b5:  ldloc.s    V_55
    IL_05b7:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_05bc:  stloc.s    V_56
    .try
    {
      IL_05be:  ldc.r8     0.0
      IL_05c7:  stloc.s    V_58
      IL_05c9:  ldc.i4.0
      IL_05ca:  stloc.s    V_59
      IL_05cc:  ldloc.s    V_56
      IL_05ce:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_05d3:  brfalse.s  IL_05f1

      IL_05d5:  ldloc.s    V_58
      IL_05d7:  ldloc.s    V_54
      IL_05d9:  ldloc.s    V_56
      IL_05db:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_05e0:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_05e5:  add
      IL_05e6:  stloc.s    V_58
      .line 100,100 : 47,58 ''
      IL_05e8:  ldloc.s    V_59
      IL_05ea:  ldc.i4.1
      IL_05eb:  add
      IL_05ec:  stloc.s    V_59
      .line 100001,100001 : 0,0 ''
      IL_05ee:  nop
      IL_05ef:  br.s       IL_05cc

      IL_05f1:  ldloc.s    V_59
      IL_05f3:  brtrue.s   IL_05f7

      IL_05f5:  br.s       IL_05f9

      IL_05f7:  br.s       IL_060c

      .line 100001,100001 : 0,0 ''
      IL_05f9:  ldstr      "source"
      IL_05fe:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_0603:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_0608:  pop
      .line 100001,100001 : 0,0 ''
      IL_0609:  nop
      IL_060a:  br.s       IL_060d

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_060c:  nop
      IL_060d:  ldloc.s    V_58
      IL_060f:  stloc.s    V_60
      IL_0611:  ldloc.s    V_59
      IL_0613:  stloc.s    V_61
      IL_0615:  ldloc.s    V_60
      IL_0617:  ldloc.s    V_61
      IL_0619:  conv.r8
      IL_061a:  div
      IL_061b:  stloc.s    V_57
      IL_061d:  leave.s    IL_063d

    }  // end .try
    finally
    {
      IL_061f:  ldloc.s    V_56
      IL_0621:  isinst     [mscorlib]System.IDisposable
      IL_0626:  stloc.s    V_62
      IL_0628:  ldloc.s    V_62
      IL_062a:  brfalse.s  IL_062e

      IL_062c:  br.s       IL_0630

      IL_062e:  br.s       IL_063a

      .line 100001,100001 : 0,0 ''
      IL_0630:  ldloc.s    V_62
      IL_0632:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0637:  ldnull
      IL_0638:  pop
      IL_0639:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_063a:  ldnull
      IL_063b:  pop
      IL_063c:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_063d:  ldloc.s    V_57
    IL_063f:  dup
    IL_0640:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_0645:  stloc.s    averageNum
    IL_0647:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_064c:  stloc.s    V_63
    IL_064e:  ldloc.s    V_63
    IL_0650:  stloc.s    V_64
    IL_0652:  ldloc.s    V_63
    IL_0654:  ldloc.s    V_63
    IL_0656:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
    IL_065b:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
    IL_0660:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0665:  ldloc.s    V_63
    IL_0667:  newobj     instance void Linq101Aggregates01/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_066c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0671:  newobj     instance void Linq101Aggregates01/'averageLength@107-1'::.ctor()
    IL_0676:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                                  !1)
    IL_067b:  stloc.s    V_65
    IL_067d:  ldloc.s    V_65
    IL_067f:  call       instance !0 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>>::get_Item1()
    IL_0684:  stloc.s    V_66
    IL_0686:  ldloc.s    V_65
    IL_0688:  call       instance !1 class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>>::get_Item2()
    IL_068d:  stloc.s    V_67
    IL_068f:  ldloc.s    V_66
    IL_0691:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0696:  stloc.s    V_68
    IL_0698:  ldloc.s    V_68
    IL_069a:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>
    IL_069f:  brfalse.s  IL_06a3

    IL_06a1:  br.s       IL_06b6

    .line 100001,100001 : 0,0 ''
    IL_06a3:  ldstr      "source"
    IL_06a8:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_06ad:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_06b2:  pop
    .line 100001,100001 : 0,0 ''
    IL_06b3:  nop
    IL_06b4:  br.s       IL_06b7

    .line 100001,100001 : 0,0 ''
    .line 100001,100001 : 0,0 ''
    IL_06b6:  nop
    IL_06b7:  ldloc.s    V_68
    IL_06b9:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_06be:  stloc.s    V_69
    .try
    {
      IL_06c0:  ldc.r8     0.0
      IL_06c9:  stloc.s    V_71
      IL_06cb:  ldc.i4.0
      IL_06cc:  stloc.s    V_72
      IL_06ce:  ldloc.s    V_69
      IL_06d0:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_06d5:  brfalse.s  IL_06f3

      IL_06d7:  ldloc.s    V_71
      IL_06d9:  ldloc.s    V_67
      IL_06db:  ldloc.s    V_69
      IL_06dd:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>>::get_Current()
      IL_06e2:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_06e7:  add
      IL_06e8:  stloc.s    V_71
      .line 107,107 : 9,21 ''
      IL_06ea:  ldloc.s    V_72
      IL_06ec:  ldc.i4.1
      IL_06ed:  add
      IL_06ee:  stloc.s    V_72
      .line 100001,100001 : 0,0 ''
      IL_06f0:  nop
      IL_06f1:  br.s       IL_06ce

      IL_06f3:  ldloc.s    V_72
      IL_06f5:  brtrue.s   IL_06f9

      IL_06f7:  br.s       IL_06fb

      IL_06f9:  br.s       IL_070e

      .line 100001,100001 : 0,0 ''
      IL_06fb:  ldstr      "source"
      IL_0700:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_0705:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_070a:  pop
      .line 100001,100001 : 0,0 ''
      IL_070b:  nop
      IL_070c:  br.s       IL_070f

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_070e:  nop
      IL_070f:  ldloc.s    V_71
      IL_0711:  stloc.s    V_73
      IL_0713:  ldloc.s    V_72
      IL_0715:  stloc.s    V_74
      IL_0717:  ldloc.s    V_73
      IL_0719:  ldloc.s    V_74
      IL_071b:  conv.r8
      IL_071c:  div
      IL_071d:  stloc.s    V_70
      IL_071f:  leave.s    IL_073f

    }  // end .try
    finally
    {
      IL_0721:  ldloc.s    V_69
      IL_0723:  isinst     [mscorlib]System.IDisposable
      IL_0728:  stloc.s    V_75
      IL_072a:  ldloc.s    V_75
      IL_072c:  brfalse.s  IL_0730

      IL_072e:  br.s       IL_0732

      IL_0730:  br.s       IL_073c

      .line 100001,100001 : 0,0 ''
      IL_0732:  ldloc.s    V_75
      IL_0734:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0739:  ldnull
      IL_073a:  pop
      IL_073b:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_073c:  ldnull
      IL_073d:  pop
      IL_073e:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_073f:  ldloc.s    V_70
    IL_0741:  dup
    IL_0742:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_0747:  stloc.s    averageLength
    .line 111,117 : 1,21 ''
    IL_0749:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_074e:  stloc.s    V_76
    IL_0750:  ldloc.s    V_76
    IL_0752:  ldloc.s    V_76
    IL_0754:  ldloc.s    V_76
    IL_0756:  ldloc.s    V_76
    IL_0758:  ldloc.s    V_76
    IL_075a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_075f:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0764:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0769:  ldloc.s    V_76
    IL_076b:  newobj     instance void Linq101Aggregates01/categories6@113::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0770:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0775:  newobj     instance void Linq101Aggregates01/'categories6@114-1'::.ctor()
    IL_077a:  newobj     instance void Linq101Aggregates01/'categories6@114-2'::.ctor()
    IL_077f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0784:  ldloc.s    V_76
    IL_0786:  newobj     instance void Linq101Aggregates01/'categories6@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_078b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0790:  newobj     instance void Linq101Aggregates01/'categories6@116-4'::.ctor()
    IL_0795:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_079a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_079f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_07a4:  dup
    IL_07a5:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_07aa:  stloc.s    categories6
    IL_07ac:  ret
  } // end of method $Linq101Aggregates01::main@

} // end of class '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
