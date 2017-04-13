
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
  // Offset: 0x00000000 Length: 0x00000606
}
.mresource public FSharpOptimizationData.Linq101Aggregates01
{
  // Offset: 0x00000610 Length: 0x00000211
}
.module Linq101Aggregates01.exe
// MVID: {58EED771-D281-4783-A745-038371D7EE58}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01660000


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
      .line 100001,100001 : 0,0 'c:\\microsoft\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Aggregates01.fs'
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      // Code size       148 (0x94)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] int32 sum,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               [4] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable> V_4,
               [5] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_5,
               [6] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               [7] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_7,
               [8] int32 V_8,
               [9] int32 V_9,
               [10] class [mscorlib]System.IDisposable V_10)
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
      IL_001b:  stloc.s    V_4
      IL_001d:  newobj     instance void Linq101Aggregates01/'sum@43-1'::.ctor()
      IL_0022:  stloc.s    V_5
      IL_0024:  ldloc.s    V_4
      IL_0026:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_002b:  stloc.s    V_6
      IL_002d:  ldloc.s    V_6
      IL_002f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0034:  stloc.s    V_7
      .try
      {
        IL_0036:  ldc.i4.0
        IL_0037:  stloc.s    V_9
        IL_0039:  ldloc.s    V_7
        IL_003b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0040:  brfalse.s  IL_0058

        .line 43,43 : 13,33 ''
        IL_0042:  ldloc.s    V_9
        IL_0044:  ldloc.s    V_5
        IL_0046:  ldloc.s    V_7
        IL_0048:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_004d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
        IL_0052:  add.ovf
        IL_0053:  stloc.s    V_9
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  br.s       IL_0039

        IL_0058:  ldloc.s    V_9
        IL_005a:  stloc.s    V_8
        IL_005c:  leave.s    IL_007c

      }  // end .try
      finally
      {
        IL_005e:  ldloc.s    V_7
        IL_0060:  isinst     [mscorlib]System.IDisposable
        IL_0065:  stloc.s    V_10
        IL_0067:  ldloc.s    V_10
        IL_0069:  brfalse.s  IL_006d

        IL_006b:  br.s       IL_006f

        IL_006d:  br.s       IL_0079

        .line 100001,100001 : 0,0 ''
        IL_006f:  ldloc.s    V_10
        IL_0071:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0076:  ldnull
        IL_0077:  pop
        IL_0078:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_0079:  ldnull
        IL_007a:  pop
        IL_007b:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007c:  ldloc.s    V_8
      IL_007e:  stloc.1
      .line 45,45 : 9,28 ''
      IL_007f:  ldarg.0
      IL_0080:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories@40-3'::builder@
      IL_0085:  ldloc.0
      IL_0086:  ldloc.1
      IL_0087:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                    !1)
      IL_008c:  tail.
      IL_008e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_0093:  ret
    } // end of method 'categories@40-3'::Invoke

  } // end of class 'categories@40-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories@45-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Tuple`2<string,int32>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .line 70,70 : 9,41 ''
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .line 95,95 : 9,46 ''
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      // Code size       249 (0xf9)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal averagePrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               [4] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable> V_4,
               [5] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal> V_5,
               [6] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               [7] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_7,
               [8] valuetype [mscorlib]System.Decimal V_8,
               [9] valuetype [mscorlib]System.Decimal V_9,
               [10] int32 V_10,
               [11] valuetype [mscorlib]System.Decimal V_11,
               [12] int32 V_12,
               [13] class [mscorlib]System.IDisposable V_13)
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
      IL_001b:  stloc.s    V_4
      IL_001d:  newobj     instance void Linq101Aggregates01/'averagePrice@115-1'::.ctor()
      IL_0022:  stloc.s    V_5
      IL_0024:  ldloc.s    V_4
      IL_0026:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_002b:  stloc.s    V_6
      IL_002d:  ldloc.s    V_6
      IL_002f:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0034:  brfalse.s  IL_0038

      IL_0036:  br.s       IL_004b

      .line 100001,100001 : 0,0 ''
      IL_0038:  ldstr      "source"
      IL_003d:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
      IL_0042:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_0047:  pop
      .line 100001,100001 : 0,0 ''
      IL_0048:  nop
      IL_0049:  br.s       IL_004c

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_004b:  nop
      IL_004c:  ldloc.s    V_6
      IL_004e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0053:  stloc.s    V_7
      .try
      {
        IL_0055:  ldc.i4.0
        IL_0056:  ldc.i4.0
        IL_0057:  ldc.i4.0
        IL_0058:  ldc.i4.0
        IL_0059:  ldc.i4.0
        IL_005a:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                           int32,
                                                                           int32,
                                                                           bool,
                                                                           uint8)
        IL_005f:  stloc.s    V_9
        IL_0061:  ldc.i4.0
        IL_0062:  stloc.s    V_10
        IL_0064:  ldloc.s    V_7
        IL_0066:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_006b:  brfalse.s  IL_008d

        IL_006d:  ldloc.s    V_9
        IL_006f:  ldloc.s    V_5
        IL_0071:  ldloc.s    V_7
        IL_0073:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0078:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::Invoke(!0)
        IL_007d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                      valuetype [mscorlib]System.Decimal)
        IL_0082:  stloc.s    V_9
        .line 115,115 : 50,71 ''
        IL_0084:  ldloc.s    V_10
        IL_0086:  ldc.i4.1
        IL_0087:  add
        IL_0088:  stloc.s    V_10
        .line 100001,100001 : 0,0 ''
        IL_008a:  nop
        IL_008b:  br.s       IL_0064

        IL_008d:  ldloc.s    V_10
        IL_008f:  brtrue.s   IL_0093

        IL_0091:  br.s       IL_0095

        IL_0093:  br.s       IL_00a8

        .line 100001,100001 : 0,0 ''
        IL_0095:  ldstr      "source"
        IL_009a:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
        IL_009f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
        IL_00a4:  pop
        .line 100001,100001 : 0,0 ''
        IL_00a5:  nop
        IL_00a6:  br.s       IL_00a9

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_00a8:  nop
        IL_00a9:  ldloc.s    V_9
        IL_00ab:  stloc.s    V_11
        IL_00ad:  ldloc.s    V_10
        IL_00af:  stloc.s    V_12
        IL_00b1:  ldloc.s    V_11
        IL_00b3:  ldloc.s    V_12
        IL_00b5:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Convert::ToDecimal(int32)
        IL_00ba:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Divide(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
        IL_00bf:  stloc.s    V_8
        IL_00c1:  leave.s    IL_00e1

      }  // end .try
      finally
      {
        IL_00c3:  ldloc.s    V_7
        IL_00c5:  isinst     [mscorlib]System.IDisposable
        IL_00ca:  stloc.s    V_13
        IL_00cc:  ldloc.s    V_13
        IL_00ce:  brfalse.s  IL_00d2

        IL_00d0:  br.s       IL_00d4

        IL_00d2:  br.s       IL_00de

        .line 100001,100001 : 0,0 ''
        IL_00d4:  ldloc.s    V_13
        IL_00d6:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00db:  ldnull
        IL_00dc:  pop
        IL_00dd:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_00de:  ldnull
        IL_00df:  pop
        IL_00e0:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_00e1:  ldloc.s    V_8
      IL_00e3:  stloc.1
      .line 116,116 : 9,37 ''
      IL_00e4:  ldarg.0
      IL_00e5:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories6@114-3'::builder@
      IL_00ea:  ldloc.0
      IL_00eb:  ldloc.1
      IL_00ec:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_00f1:  tail.
      IL_00f3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_00f8:  ret
    } // end of method 'categories6@114-3'::Invoke

  } // end of class 'categories6@114-3'

  .class auto ansi serializable nested assembly beforefieldinit 'categories6@116-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
    // Code size       1881 (0x759)
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
             [23] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable> V_23,
             [24] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_24,
             [25] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_25,
             [26] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_26,
             [27] int32 V_27,
             [28] int32 V_28,
             [29] class [mscorlib]System.IDisposable V_29,
             [30] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_30,
             [31] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             [32] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable> V_32,
             [33] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_33,
             [34] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_34,
             [35] class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_35,
             [36] int32 V_36,
             [37] int32 V_37,
             [38] class [mscorlib]System.IDisposable V_38,
             [39] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_39,
             [40] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_40,
             [41] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             [42] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_42,
             [43] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43,
             [44] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_44,
             [45] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_45,
             [46] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_46,
             [47] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47,
             [48] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_48,
             [49] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_49,
             [50] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable> V_50,
             [51] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_51,
             [52] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_52,
             [53] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_53,
             [54] float64 V_54,
             [55] float64 V_55,
             [56] int32 V_56,
             [57] float64 V_57,
             [58] int32 V_58,
             [59] class [mscorlib]System.IDisposable V_59,
             [60] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_60,
             [61] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_61,
             [62] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable> V_62,
             [63] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64> V_63,
             [64] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>> V_64,
             [65] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>> V_65,
             [66] float64 V_66,
             [67] float64 V_67,
             [68] int32 V_68,
             [69] float64 V_69,
             [70] int32 V_70,
             [71] class [mscorlib]System.IDisposable V_71,
             [72] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_72)
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
    IL_00bc:  stloc.s    V_23
    IL_00be:  newobj     instance void Linq101Aggregates01/'numSum@22-1'::.ctor()
    IL_00c3:  stloc.s    V_24
    IL_00c5:  ldloc.s    V_23
    IL_00c7:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00cc:  stloc.s    V_25
    IL_00ce:  ldloc.s    V_25
    IL_00d0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_00d5:  stloc.s    V_26
    .try
    {
      IL_00d7:  ldc.i4.0
      IL_00d8:  stloc.s    V_28
      IL_00da:  ldloc.s    V_26
      IL_00dc:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00e1:  brfalse.s  IL_00f9

      .line 22,22 : 9,16 ''
      IL_00e3:  ldloc.s    V_28
      IL_00e5:  ldloc.s    V_24
      IL_00e7:  ldloc.s    V_26
      IL_00e9:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_00ee:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_00f3:  add.ovf
      IL_00f4:  stloc.s    V_28
      .line 100001,100001 : 0,0 ''
      IL_00f6:  nop
      IL_00f7:  br.s       IL_00da

      IL_00f9:  ldloc.s    V_28
      IL_00fb:  stloc.s    V_27
      IL_00fd:  leave.s    IL_011d

    }  // end .try
    finally
    {
      IL_00ff:  ldloc.s    V_26
      IL_0101:  isinst     [mscorlib]System.IDisposable
      IL_0106:  stloc.s    V_29
      IL_0108:  ldloc.s    V_29
      IL_010a:  brfalse.s  IL_010e

      IL_010c:  br.s       IL_0110

      IL_010e:  br.s       IL_011a

      .line 100001,100001 : 0,0 ''
      IL_0110:  ldloc.s    V_29
      IL_0112:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0117:  ldnull
      IL_0118:  pop
      IL_0119:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_011a:  ldnull
      IL_011b:  pop
      IL_011c:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_011d:  ldloc.s    V_27
    IL_011f:  dup
    IL_0120:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_0125:  stloc.3
    .line 26,26 : 1,45 ''
    IL_0126:  ldstr      "cherry"
    IL_012b:  ldstr      "apple"
    IL_0130:  ldstr      "blueberry"
    IL_0135:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_013a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0144:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0149:  dup
    IL_014a:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_014f:  stloc.s    words
    IL_0151:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0156:  stloc.s    V_30
    IL_0158:  ldloc.s    V_30
    IL_015a:  stloc.s    V_31
    IL_015c:  ldnull
    IL_015d:  ldnull
    IL_015e:  ldnull
    IL_015f:  ldc.i4.0
    IL_0160:  ldnull
    IL_0161:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_0166:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_016b:  stloc.s    V_32
    IL_016d:  newobj     instance void Linq101Aggregates01/'totalChars@31-1'::.ctor()
    IL_0172:  stloc.s    V_33
    IL_0174:  ldloc.s    V_32
    IL_0176:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_017b:  stloc.s    V_34
    IL_017d:  ldloc.s    V_34
    IL_017f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0184:  stloc.s    V_35
    .try
    {
      IL_0186:  ldc.i4.0
      IL_0187:  stloc.s    V_37
      IL_0189:  ldloc.s    V_35
      IL_018b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0190:  brfalse.s  IL_01a8

      .line 31,31 : 9,25 ''
      IL_0192:  ldloc.s    V_37
      IL_0194:  ldloc.s    V_33
      IL_0196:  ldloc.s    V_35
      IL_0198:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_019d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_01a2:  add.ovf
      IL_01a3:  stloc.s    V_37
      .line 100001,100001 : 0,0 ''
      IL_01a5:  nop
      IL_01a6:  br.s       IL_0189

      IL_01a8:  ldloc.s    V_37
      IL_01aa:  stloc.s    V_36
      IL_01ac:  leave.s    IL_01cc

    }  // end .try
    finally
    {
      IL_01ae:  ldloc.s    V_35
      IL_01b0:  isinst     [mscorlib]System.IDisposable
      IL_01b5:  stloc.s    V_38
      IL_01b7:  ldloc.s    V_38
      IL_01b9:  brfalse.s  IL_01bd

      IL_01bb:  br.s       IL_01bf

      IL_01bd:  br.s       IL_01c9

      .line 100001,100001 : 0,0 ''
      IL_01bf:  ldloc.s    V_38
      IL_01c1:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_01c6:  ldnull
      IL_01c7:  pop
      IL_01c8:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_01c9:  ldnull
      IL_01ca:  pop
      IL_01cb:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_01cc:  ldloc.s    V_36
    IL_01ce:  dup
    IL_01cf:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_01d4:  stloc.s    totalChars
    .line 35,35 : 1,32 ''
    IL_01d6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01db:  dup
    IL_01dc:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_01e1:  stloc.s    products
    .line 37,46 : 1,21 ''
    IL_01e3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01e8:  stloc.s    V_39
    IL_01ea:  ldloc.s    V_39
    IL_01ec:  ldloc.s    V_39
    IL_01ee:  ldloc.s    V_39
    IL_01f0:  ldloc.s    V_39
    IL_01f2:  ldloc.s    V_39
    IL_01f4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_01f9:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_01fe:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0203:  ldloc.s    V_39
    IL_0205:  newobj     instance void Linq101Aggregates01/categories@39::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_020a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_020f:  newobj     instance void Linq101Aggregates01/'categories@40-1'::.ctor()
    IL_0214:  newobj     instance void Linq101Aggregates01/'categories@40-2'::.ctor()
    IL_0219:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_021e:  ldloc.s    V_39
    IL_0220:  newobj     instance void Linq101Aggregates01/'categories@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0225:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_022a:  newobj     instance void Linq101Aggregates01/'categories@45-4'::.ctor()
    IL_022f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0234:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,int32>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0239:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_023e:  dup
    IL_023f:  stsfld     class [mscorlib]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_0244:  stloc.s    categories
    IL_0246:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_024b:  stloc.s    V_40
    IL_024d:  ldloc.s    V_40
    IL_024f:  ldc.i4.0
    IL_0250:  ldc.i4.0
    IL_0251:  ldnull
    IL_0252:  ldc.i4.0
    IL_0253:  ldc.i4.0
    IL_0254:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0259:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_025e:  newobj     instance void Linq101Aggregates01/'minNum@49-1'::.ctor()
    IL_0263:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0268:  dup
    IL_0269:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_026e:  stloc.s    minNum
    IL_0270:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0275:  stloc.s    V_41
    IL_0277:  ldloc.s    V_41
    IL_0279:  ldnull
    IL_027a:  ldnull
    IL_027b:  ldnull
    IL_027c:  ldc.i4.0
    IL_027d:  ldnull
    IL_027e:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_0283:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0288:  newobj     instance void Linq101Aggregates01/'shortestWord@52-1'::.ctor()
    IL_028d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0292:  dup
    IL_0293:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0298:  stloc.s    shortestWord
    .line 55,61 : 1,21 ''
    IL_029a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_029f:  stloc.s    V_42
    IL_02a1:  ldloc.s    V_42
    IL_02a3:  ldloc.s    V_42
    IL_02a5:  ldloc.s    V_42
    IL_02a7:  ldloc.s    V_42
    IL_02a9:  ldloc.s    V_42
    IL_02ab:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_02b0:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_02b5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02ba:  ldloc.s    V_42
    IL_02bc:  newobj     instance void Linq101Aggregates01/categories2@57::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02c1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02c6:  newobj     instance void Linq101Aggregates01/'categories2@58-1'::.ctor()
    IL_02cb:  newobj     instance void Linq101Aggregates01/'categories2@58-2'::.ctor()
    IL_02d0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02d5:  ldloc.s    V_42
    IL_02d7:  newobj     instance void Linq101Aggregates01/'categories2@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02dc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02e1:  newobj     instance void Linq101Aggregates01/'categories2@60-4'::.ctor()
    IL_02e6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02eb:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_02f0:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02f5:  dup
    IL_02f6:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_02fb:  stloc.s    categories2
    .line 64,71 : 1,21 ''
    IL_02fd:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0302:  stloc.s    V_43
    IL_0304:  ldloc.s    V_43
    IL_0306:  ldloc.s    V_43
    IL_0308:  ldloc.s    V_43
    IL_030a:  ldloc.s    V_43
    IL_030c:  ldloc.s    V_43
    IL_030e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0313:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0318:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_031d:  ldloc.s    V_43
    IL_031f:  newobj     instance void Linq101Aggregates01/categories3@66::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0324:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0329:  newobj     instance void Linq101Aggregates01/'categories3@67-1'::.ctor()
    IL_032e:  newobj     instance void Linq101Aggregates01/'categories3@67-2'::.ctor()
    IL_0333:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0338:  ldloc.s    V_43
    IL_033a:  newobj     instance void Linq101Aggregates01/'categories3@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_033f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0344:  newobj     instance void Linq101Aggregates01/'categories3@70-4'::.ctor()
    IL_0349:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_034e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0353:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0358:  dup
    IL_0359:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_035e:  stloc.s    categories3
    IL_0360:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0365:  stloc.s    V_44
    IL_0367:  ldloc.s    V_44
    IL_0369:  ldc.i4.0
    IL_036a:  ldc.i4.0
    IL_036b:  ldnull
    IL_036c:  ldc.i4.0
    IL_036d:  ldc.i4.0
    IL_036e:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0373:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0378:  newobj     instance void Linq101Aggregates01/'maxNum@74-1'::.ctor()
    IL_037d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0382:  dup
    IL_0383:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_0388:  stloc.s    maxNum
    IL_038a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_038f:  stloc.s    V_45
    IL_0391:  ldloc.s    V_45
    IL_0393:  ldnull
    IL_0394:  ldnull
    IL_0395:  ldnull
    IL_0396:  ldc.i4.0
    IL_0397:  ldnull
    IL_0398:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(string,
                                                                                   string,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_039d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_03a2:  newobj     instance void Linq101Aggregates01/'longestLength@77-1'::.ctor()
    IL_03a7:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03ac:  dup
    IL_03ad:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_03b2:  stloc.s    longestLength
    .line 80,86 : 1,21 ''
    IL_03b4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03b9:  stloc.s    V_46
    IL_03bb:  ldloc.s    V_46
    IL_03bd:  ldloc.s    V_46
    IL_03bf:  ldloc.s    V_46
    IL_03c1:  ldloc.s    V_46
    IL_03c3:  ldloc.s    V_46
    IL_03c5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_03ca:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_03cf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03d4:  ldloc.s    V_46
    IL_03d6:  newobj     instance void Linq101Aggregates01/categories4@82::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03db:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03e0:  newobj     instance void Linq101Aggregates01/'categories4@83-1'::.ctor()
    IL_03e5:  newobj     instance void Linq101Aggregates01/'categories4@83-2'::.ctor()
    IL_03ea:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_03ef:  ldloc.s    V_46
    IL_03f1:  newobj     instance void Linq101Aggregates01/'categories4@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03f6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03fb:  newobj     instance void Linq101Aggregates01/'categories4@85-4'::.ctor()
    IL_0400:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0405:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_040a:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_040f:  dup
    IL_0410:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_0415:  stloc.s    categories4
    .line 89,96 : 1,21 ''
    IL_0417:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_041c:  stloc.s    V_47
    IL_041e:  ldloc.s    V_47
    IL_0420:  ldloc.s    V_47
    IL_0422:  ldloc.s    V_47
    IL_0424:  ldloc.s    V_47
    IL_0426:  ldloc.s    V_47
    IL_0428:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_042d:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0432:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0437:  ldloc.s    V_47
    IL_0439:  newobj     instance void Linq101Aggregates01/categories5@91::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_043e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0443:  newobj     instance void Linq101Aggregates01/'categories5@92-1'::.ctor()
    IL_0448:  newobj     instance void Linq101Aggregates01/'categories5@92-2'::.ctor()
    IL_044d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0452:  ldloc.s    V_47
    IL_0454:  newobj     instance void Linq101Aggregates01/'categories5@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0459:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_045e:  newobj     instance void Linq101Aggregates01/'categories5@95-4'::.ctor()
    IL_0463:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0468:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_046d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0472:  dup
    IL_0473:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0478:  stloc.s    categories5
    .line 99,99 : 1,66 ''
    IL_047a:  ldc.r8     5.
    IL_0483:  ldc.r8     4.
    IL_048c:  ldc.r8     1.
    IL_0495:  ldc.r8     3.
    IL_049e:  ldc.r8     9.
    IL_04a7:  ldc.r8     8.
    IL_04b0:  ldc.r8     6.
    IL_04b9:  ldc.r8     7.
    IL_04c2:  ldc.r8     2.
    IL_04cb:  ldc.r8     0.0
    IL_04d4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_04d9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04de:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04e3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04e8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04ed:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04f2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04f7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04fc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0501:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0506:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_050b:  dup
    IL_050c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_0511:  stloc.s    numbers2
    IL_0513:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0518:  stloc.s    V_48
    IL_051a:  ldloc.s    V_48
    IL_051c:  stloc.s    V_49
    IL_051e:  ldc.r8     0.0
    IL_0527:  ldc.r8     0.0
    IL_0530:  ldnull
    IL_0531:  ldc.i4.0
    IL_0532:  ldc.r8     0.0
    IL_053b:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(float64,
                                                                                 float64,
                                                                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_0540:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0545:  stloc.s    V_50
    IL_0547:  newobj     instance void Linq101Aggregates01/'averageNum@100-1'::.ctor()
    IL_054c:  stloc.s    V_51
    IL_054e:  ldloc.s    V_50
    IL_0550:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0555:  stloc.s    V_52
    IL_0557:  ldloc.s    V_52
    IL_0559:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_055e:  brfalse.s  IL_0562

    IL_0560:  br.s       IL_0575

    .line 100001,100001 : 0,0 ''
    IL_0562:  ldstr      "source"
    IL_0567:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_056c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_0571:  pop
    .line 100001,100001 : 0,0 ''
    IL_0572:  nop
    IL_0573:  br.s       IL_0576

    .line 100001,100001 : 0,0 ''
    .line 100001,100001 : 0,0 ''
    IL_0575:  nop
    IL_0576:  ldloc.s    V_52
    IL_0578:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_057d:  stloc.s    V_53
    .try
    {
      IL_057f:  ldc.r8     0.0
      IL_0588:  stloc.s    V_55
      IL_058a:  ldc.i4.0
      IL_058b:  stloc.s    V_56
      IL_058d:  ldloc.s    V_53
      IL_058f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0594:  brfalse.s  IL_05b2

      IL_0596:  ldloc.s    V_55
      IL_0598:  ldloc.s    V_51
      IL_059a:  ldloc.s    V_53
      IL_059c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_05a1:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_05a6:  add
      IL_05a7:  stloc.s    V_55
      .line 100,100 : 47,58 ''
      IL_05a9:  ldloc.s    V_56
      IL_05ab:  ldc.i4.1
      IL_05ac:  add
      IL_05ad:  stloc.s    V_56
      .line 100001,100001 : 0,0 ''
      IL_05af:  nop
      IL_05b0:  br.s       IL_058d

      IL_05b2:  ldloc.s    V_56
      IL_05b4:  brtrue.s   IL_05b8

      IL_05b6:  br.s       IL_05ba

      IL_05b8:  br.s       IL_05cd

      .line 100001,100001 : 0,0 ''
      IL_05ba:  ldstr      "source"
      IL_05bf:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_05c4:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_05c9:  pop
      .line 100001,100001 : 0,0 ''
      IL_05ca:  nop
      IL_05cb:  br.s       IL_05ce

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_05cd:  nop
      IL_05ce:  ldloc.s    V_55
      IL_05d0:  stloc.s    V_57
      IL_05d2:  ldloc.s    V_56
      IL_05d4:  stloc.s    V_58
      IL_05d6:  ldloc.s    V_57
      IL_05d8:  ldloc.s    V_58
      IL_05da:  conv.r8
      IL_05db:  div
      IL_05dc:  stloc.s    V_54
      IL_05de:  leave.s    IL_05fe

    }  // end .try
    finally
    {
      IL_05e0:  ldloc.s    V_53
      IL_05e2:  isinst     [mscorlib]System.IDisposable
      IL_05e7:  stloc.s    V_59
      IL_05e9:  ldloc.s    V_59
      IL_05eb:  brfalse.s  IL_05ef

      IL_05ed:  br.s       IL_05f1

      IL_05ef:  br.s       IL_05fb

      .line 100001,100001 : 0,0 ''
      IL_05f1:  ldloc.s    V_59
      IL_05f3:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_05f8:  ldnull
      IL_05f9:  pop
      IL_05fa:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_05fb:  ldnull
      IL_05fc:  pop
      IL_05fd:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_05fe:  ldloc.s    V_54
    IL_0600:  dup
    IL_0601:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_0606:  stloc.s    averageNum
    IL_0608:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_060d:  stloc.s    V_60
    IL_060f:  ldloc.s    V_60
    IL_0611:  stloc.s    V_61
    IL_0613:  ldloc.s    V_60
    IL_0615:  ldloc.s    V_60
    IL_0617:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
    IL_061c:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
    IL_0621:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0626:  ldloc.s    V_60
    IL_0628:  newobj     instance void Linq101Aggregates01/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_062d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0632:  stloc.s    V_62
    IL_0634:  newobj     instance void Linq101Aggregates01/'averageLength@107-1'::.ctor()
    IL_0639:  stloc.s    V_63
    IL_063b:  ldloc.s    V_62
    IL_063d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0642:  stloc.s    V_64
    IL_0644:  ldloc.s    V_64
    IL_0646:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>
    IL_064b:  brfalse.s  IL_064f

    IL_064d:  br.s       IL_0662

    .line 100001,100001 : 0,0 ''
    IL_064f:  ldstr      "source"
    IL_0654:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_0659:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_065e:  pop
    .line 100001,100001 : 0,0 ''
    IL_065f:  nop
    IL_0660:  br.s       IL_0663

    .line 100001,100001 : 0,0 ''
    .line 100001,100001 : 0,0 ''
    IL_0662:  nop
    IL_0663:  ldloc.s    V_64
    IL_0665:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_066a:  stloc.s    V_65
    .try
    {
      IL_066c:  ldc.r8     0.0
      IL_0675:  stloc.s    V_67
      IL_0677:  ldc.i4.0
      IL_0678:  stloc.s    V_68
      IL_067a:  ldloc.s    V_65
      IL_067c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0681:  brfalse.s  IL_069f

      IL_0683:  ldloc.s    V_67
      IL_0685:  ldloc.s    V_63
      IL_0687:  ldloc.s    V_65
      IL_0689:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>>::get_Current()
      IL_068e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_0693:  add
      IL_0694:  stloc.s    V_67
      .line 107,107 : 9,21 ''
      IL_0696:  ldloc.s    V_68
      IL_0698:  ldc.i4.1
      IL_0699:  add
      IL_069a:  stloc.s    V_68
      .line 100001,100001 : 0,0 ''
      IL_069c:  nop
      IL_069d:  br.s       IL_067a

      IL_069f:  ldloc.s    V_68
      IL_06a1:  brtrue.s   IL_06a5

      IL_06a3:  br.s       IL_06a7

      IL_06a5:  br.s       IL_06ba

      .line 100001,100001 : 0,0 ''
      IL_06a7:  ldstr      "source"
      IL_06ac:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_06b1:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_06b6:  pop
      .line 100001,100001 : 0,0 ''
      IL_06b7:  nop
      IL_06b8:  br.s       IL_06bb

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_06ba:  nop
      IL_06bb:  ldloc.s    V_67
      IL_06bd:  stloc.s    V_69
      IL_06bf:  ldloc.s    V_68
      IL_06c1:  stloc.s    V_70
      IL_06c3:  ldloc.s    V_69
      IL_06c5:  ldloc.s    V_70
      IL_06c7:  conv.r8
      IL_06c8:  div
      IL_06c9:  stloc.s    V_66
      IL_06cb:  leave.s    IL_06eb

    }  // end .try
    finally
    {
      IL_06cd:  ldloc.s    V_65
      IL_06cf:  isinst     [mscorlib]System.IDisposable
      IL_06d4:  stloc.s    V_71
      IL_06d6:  ldloc.s    V_71
      IL_06d8:  brfalse.s  IL_06dc

      IL_06da:  br.s       IL_06de

      IL_06dc:  br.s       IL_06e8

      .line 100001,100001 : 0,0 ''
      IL_06de:  ldloc.s    V_71
      IL_06e0:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_06e5:  ldnull
      IL_06e6:  pop
      IL_06e7:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_06e8:  ldnull
      IL_06e9:  pop
      IL_06ea:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_06eb:  ldloc.s    V_66
    IL_06ed:  dup
    IL_06ee:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_06f3:  stloc.s    averageLength
    .line 111,117 : 1,21 ''
    IL_06f5:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_06fa:  stloc.s    V_72
    IL_06fc:  ldloc.s    V_72
    IL_06fe:  ldloc.s    V_72
    IL_0700:  ldloc.s    V_72
    IL_0702:  ldloc.s    V_72
    IL_0704:  ldloc.s    V_72
    IL_0706:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_070b:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_0710:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0715:  ldloc.s    V_72
    IL_0717:  newobj     instance void Linq101Aggregates01/categories6@113::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_071c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0721:  newobj     instance void Linq101Aggregates01/'categories6@114-1'::.ctor()
    IL_0726:  newobj     instance void Linq101Aggregates01/'categories6@114-2'::.ctor()
    IL_072b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0730:  ldloc.s    V_72
    IL_0732:  newobj     instance void Linq101Aggregates01/'categories6@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0737:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_073c:  newobj     instance void Linq101Aggregates01/'categories6@116-4'::.ctor()
    IL_0741:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0746:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_074b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0750:  dup
    IL_0751:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_0756:  stloc.s    categories6
    IL_0758:  ret
  } // end of method $Linq101Aggregates01::main@

} // end of class '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
