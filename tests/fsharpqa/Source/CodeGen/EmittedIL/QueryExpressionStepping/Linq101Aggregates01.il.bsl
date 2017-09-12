
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
  // Offset: 0x00000000 Length: 0x00000610
}
.mresource public FSharpOptimizationData.Linq101Aggregates01
{
  // Offset: 0x00000618 Length: 0x00000211
}
.module Linq101Aggregates01.exe
// MVID: {59B770B7-D281-4783-A745-0383B770B759}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x013F0000


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
      // Code size       191 (0xbf)
      .maxstack  6
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\src\\manofstick\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Aggregates01.fs'
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 12,12 : 9,33 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 12,12 : 9,33 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      .line 12,12 : 9,33 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      IL_006a:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 13,13 : 9,17 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      IL_007d:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::n
      .line 12,12 : 9,33 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      .line 12,12 : 9,33 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/uniqueFactors@12::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method uniqueFactors@12::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/uniqueFactors@12::current
      IL_0006:  ret
    } // end of method uniqueFactors@12::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void Linq101Aggregates01/uniqueFactors@12::.ctor(int32,
                                                                                     int32,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     int32)
      IL_000a:  ret
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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 21,21 : 9,28 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 21,21 : 9,28 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101Aggregates01/numSum@21::_arg1
      .line 21,21 : 9,28 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101Aggregates01/numSum@21::_arg1
      IL_006a:  stfld      int32 Linq101Aggregates01/numSum@21::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 22,22 : 9,16 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101Aggregates01/numSum@21::n
      IL_007d:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101Aggregates01/numSum@21::n
      .line 21,21 : 9,28 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/numSum@21::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      .line 21,21 : 9,28 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method numSum@21::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0006:  ret
    } // end of method numSum@21::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000a:  ret
    } // end of method numSum@21::GetFreshEnumerator

  } // end of class numSum@21

  .class auto ansi serializable sealed nested assembly beforefieldinit 'numSum@22-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 22,22 : 15,16 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'numSum@22-1'::Invoke

  } // end of class 'numSum@22-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'numSum@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> projection) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> Linq101Aggregates01/'numSum@22-3'::projection
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4.0
      IL_0009:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>::.ctor(!1)
      IL_000e:  ldarg.0
      IL_000f:  pop
      IL_0010:  ret
    } // end of method 'numSum@22-3'::.ctor

    .method public hidebysig virtual instance bool 
            ProcessNext(int32 'value') cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>::get_Result()
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> Linq101Aggregates01/'numSum@22-3'::projection
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_0013:  add.ovf
      IL_0014:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>::set_Result(!1)
      IL_0019:  nop
      .line 22,22 : 9,16 ''
      IL_001a:  ldc.i4.0
      IL_001b:  ret
    } // end of method 'numSum@22-3'::ProcessNext

  } // end of class 'numSum@22-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'numSum@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> projection) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> Linq101Aggregates01/'numSum@22-2'::projection
      IL_000d:  ret
    } // end of method 'numSum@22-2'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<int32,int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      .line 22,22 : 9,16 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> Linq101Aggregates01/'numSum@22-2'::projection
      IL_0006:  newobj     instance void Linq101Aggregates01/'numSum@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>)
      IL_000b:  ret
    } // end of method 'numSum@22-2'::Invoke

  } // end of class 'numSum@22-2'

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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 30,30 : 9,26 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 30,30 : 9,26 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005e:  stfld      string Linq101Aggregates01/totalChars@30::_arg1
      .line 30,30 : 9,26 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      string Linq101Aggregates01/totalChars@30::_arg1
      IL_006a:  stfld      string Linq101Aggregates01/totalChars@30::w
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 31,31 : 9,25 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      string Linq101Aggregates01/totalChars@30::w
      IL_007d:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      string Linq101Aggregates01/totalChars@30::w
      .line 30,30 : 9,26 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/totalChars@30::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      .line 30,30 : 9,26 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method totalChars@30::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/totalChars@30::current
      IL_0006:  ret
    } // end of method totalChars@30::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_000a:  ret
    } // end of method totalChars@30::GetFreshEnumerator

  } // end of class totalChars@30

  .class auto ansi serializable sealed nested assembly beforefieldinit 'totalChars@31-1'
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
      // Code size       7 (0x7)
      .maxstack  8
      .line 31,31 : 16,24 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'totalChars@31-1'::Invoke

  } // end of class 'totalChars@31-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'totalChars@31-3'
         extends class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> projection) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> Linq101Aggregates01/'totalChars@31-3'::projection
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4.0
      IL_0009:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>::.ctor(!1)
      IL_000e:  ldarg.0
      IL_000f:  pop
      IL_0010:  ret
    } // end of method 'totalChars@31-3'::.ctor

    .method public hidebysig virtual instance bool 
            ProcessNext(string 'value') cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>::get_Result()
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> Linq101Aggregates01/'totalChars@31-3'::projection
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_0013:  add.ovf
      IL_0014:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>::set_Result(!1)
      IL_0019:  nop
      .line 31,31 : 9,25 ''
      IL_001a:  ldc.i4.0
      IL_001b:  ret
    } // end of method 'totalChars@31-3'::ProcessNext

  } // end of class 'totalChars@31-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'totalChars@31-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> projection) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> Linq101Aggregates01/'totalChars@31-2'::projection
      IL_000d:  ret
    } // end of method 'totalChars@31-2'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<string,int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      .line 31,31 : 9,25 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> Linq101Aggregates01/'totalChars@31-2'::projection
      IL_0006:  newobj     instance void Linq101Aggregates01/'totalChars@31-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>)
      IL_000b:  ret
    } // end of method 'totalChars@31-2'::Invoke

  } // end of class 'totalChars@31-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories@39
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 39,39 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 40,40 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories@39::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories@39::Invoke

  } // end of class categories@39

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories@40-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 40,40 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories@40-1'::Invoke

  } // end of class 'categories@40-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories@40-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 40,40 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 42,42 : 13,26 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 42,42 : 13,26 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      .line 42,42 : 13,26 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 43,43 : 13,33 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::x
      .line 42,42 : 13,26 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/sum@42::pc
      .line 42,42 : 13,26 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method sum@42::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0006:  ret
    } // end of method sum@42::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method sum@42::GetFreshEnumerator

  } // end of class sum@42

  .class auto ansi serializable sealed nested assembly beforefieldinit 'sum@43-1'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 43,43 : 19,33 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0008:  ret
    } // end of method 'sum@43-1'::Invoke

  } // end of class 'sum@43-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'sum@43-3'
         extends class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> projection) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> Linq101Aggregates01/'sum@43-3'::projection
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4.0
      IL_0009:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>::.ctor(!1)
      IL_000e:  ldarg.0
      IL_000f:  pop
      IL_0010:  ret
    } // end of method 'sum@43-3'::.ctor

    .method public hidebysig virtual instance bool 
            ProcessNext(class [Utils]Utils/Product 'value') cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>::get_Result()
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> Linq101Aggregates01/'sum@43-3'::projection
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
      IL_0013:  add.ovf
      IL_0014:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>::set_Result(!1)
      IL_0019:  nop
      .line 43,43 : 13,33 ''
      IL_001a:  ldc.i4.0
      IL_001b:  ret
    } // end of method 'sum@43-3'::ProcessNext

  } // end of class 'sum@43-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'sum@43-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> projection
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> projection) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> Linq101Aggregates01/'sum@43-2'::projection
      IL_000d:  ret
    } // end of method 'sum@43-2'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<class [Utils]Utils/Product,int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      .line 43,43 : 13,33 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> Linq101Aggregates01/'sum@43-2'::projection
      IL_0006:  newobj     instance void Linq101Aggregates01/'sum@43-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>)
      IL_000b:  ret
    } // end of method 'sum@43-2'::Invoke

  } // end of class 'sum@43-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories@40-3'
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
      // Code size       108 (0x6c)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] int32 sum,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               [4] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable> V_4,
               [5] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_5,
               [6] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6)
      .line 40,40 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldc.i4.0
      IL_000f:  ldnull
      IL_0010:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001a:  stloc.s    V_4
      IL_001c:  newobj     instance void Linq101Aggregates01/'sum@43-1'::.ctor()
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloc.s    V_4
      IL_0025:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_002a:  stloc.s    V_6
      IL_002c:  ldloc.s    V_6
      IL_002e:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0033:  brfalse.s  IL_0037

      IL_0035:  br.s       IL_0042

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldstr      "source"
      IL_003c:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
      IL_0041:  throw

      .line 100001,100001 : 0,0 ''
      IL_0042:  nop
      IL_0043:  ldloc.s    V_6
      IL_0045:  call       class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ISeqModule::OfSeq<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_004a:  ldloc.s    V_5
      IL_004c:  newobj     instance void Linq101Aggregates01/'sum@43-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>)
      IL_0051:  callvirt   instance !!0 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<class [Utils]Utils/Product>::Fold<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<!0,!!0>>)
      IL_0056:  stloc.1
      .line 45,45 : 9,28 ''
      IL_0057:  ldarg.0
      IL_0058:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories@40-3'::builder@
      IL_005d:  ldloc.0
      IL_005e:  ldloc.1
      IL_005f:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                    !1)
      IL_0064:  tail.
      IL_0066:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_006b:  ret
    } // end of method 'categories@40-3'::Invoke

  } // end of class 'categories@40-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories@45-4'
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
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] int32 sum)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item2()
      IL_000d:  stloc.1
      .line 45,45 : 17,27 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001a:  ret
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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 49,49 : 22,41 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 22,41 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101Aggregates01/minNum@49::_arg1
      .line 49,49 : 22,41 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101Aggregates01/minNum@49::_arg1
      IL_006a:  stfld      int32 Linq101Aggregates01/minNum@49::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 42,49 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101Aggregates01/minNum@49::n
      IL_007d:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101Aggregates01/minNum@49::n
      .line 49,49 : 22,41 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/minNum@49::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      .line 49,49 : 22,41 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method minNum@49::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0006:  ret
    } // end of method minNum@49::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000a:  ret
    } // end of method minNum@49::GetFreshEnumerator

  } // end of class minNum@49

  .class auto ansi serializable sealed nested assembly beforefieldinit 'minNum@49-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 49,49 : 48,49 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 52,52 : 28,45 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 28,45 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005e:  stfld      string Linq101Aggregates01/shortestWord@52::_arg1
      .line 52,52 : 28,45 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      string Linq101Aggregates01/shortestWord@52::_arg1
      IL_006a:  stfld      string Linq101Aggregates01/shortestWord@52::w
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 46,60 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      string Linq101Aggregates01/shortestWord@52::w
      IL_007d:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      string Linq101Aggregates01/shortestWord@52::w
      .line 52,52 : 28,45 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/shortestWord@52::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      .line 52,52 : 28,45 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method shortestWord@52::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0006:  ret
    } // end of method shortestWord@52::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(string,
                                                                                    string,
                                                                                    class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                    int32,
                                                                                    string)
      IL_000a:  ret
    } // end of method shortestWord@52::GetFreshEnumerator

  } // end of class shortestWord@52

  .class auto ansi serializable sealed nested assembly beforefieldinit 'shortestWord@52-1'
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
      // Code size       7 (0x7)
      .maxstack  8
      .line 52,52 : 52,60 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'shortestWord@52-1'::Invoke

  } // end of class 'shortestWord@52-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories2@57
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 57,57 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 58,58 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories2@57::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories2@57::Invoke

  } // end of class categories2@57

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories2@58-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 58,58 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories2@58-1'::Invoke

  } // end of class 'categories2@58-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories2@58-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 58,58 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 59,59 : 27,40 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 27,40 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      .line 59,59 : 27,40 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 41,58 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::x
      .line 59,59 : 27,40 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/min@59::pc
      .line 59,59 : 27,40 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/min@59::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method min@59::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0006:  ret
    } // end of method min@59::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method min@59::GetFreshEnumerator

  } // end of class min@59

  .class auto ansi serializable sealed nested assembly beforefieldinit 'min@59-1'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 59,59 : 47,58 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'min@59-1'::Invoke

  } // end of class 'min@59-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories2@58-3'
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
      // Code size       57 (0x39)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 58,58 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  ldloc.0
      IL_000a:  ldnull
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldc.i4.0
      IL_000e:  ldnull
      IL_000f:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [Utils]Utils/Product,
                                                                           class [Utils]Utils/Product,
                                                                           class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0014:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0019:  newobj     instance void Linq101Aggregates01/'min@59-1'::.ctor()
      IL_001e:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0023:  stloc.1
      .line 60,60 : 9,28 ''
      IL_0024:  ldarg.0
      IL_0025:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories2@58-3'::builder@
      IL_002a:  ldloc.0
      IL_002b:  ldloc.1
      IL_002c:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_0031:  tail.
      IL_0033:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_0038:  ret
    } // end of method 'categories2@58-3'::Invoke

  } // end of class 'categories2@58-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories2@60-4'
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
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      .line 60,60 : 17,27 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
    } // end of method 'categories2@60-4'::Invoke

  } // end of class 'categories2@60-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories3@66
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 66,66 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 67,67 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories3@66::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories3@66::Invoke

  } // end of class categories3@66

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories3@67-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 67,67 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories3@67-1'::Invoke

  } // end of class 'categories3@67-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories3@67-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 67,67 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 68,68 : 46,57 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 69,69 : 40,53 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 40,53 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      .line 69,69 : 40,53 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 54,79 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::x
      .line 69,69 : 40,53 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      .line 69,69 : 40,53 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method cheapestProducts@69::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0006:  ret
    } // end of method cheapestProducts@69::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method cheapestProducts@69::GetFreshEnumerator

  } // end of class cheapestProducts@69

  .class auto ansi serializable sealed nested assembly beforefieldinit 'cheapestProducts@69-1'
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
      // Code size       18 (0x12)
      .maxstack  8
      .line 69,69 : 61,78 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'cheapestProducts@69-1'::min
      IL_000c:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_0011:  ret
    } // end of method 'cheapestProducts@69-1'::Invoke

  } // end of class 'cheapestProducts@69-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories3@67-3'
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
      // Code size       87 (0x57)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> cheapestProducts,
               [3] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 67,67 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 68,68 : 9,58 ''
      IL_0002:  ldloc.0
      IL_0003:  newobj     instance void Linq101Aggregates01/'min@68-2'::.ctor()
      IL_0008:  ldftn      instance valuetype [mscorlib]System.Decimal Linq101Aggregates01/'min@68-2'::Invoke(class [Utils]Utils/Product)
      IL_000e:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                             native int)
      IL_0013:  call       valuetype [mscorlib]System.Decimal [System.Core]System.Linq.Enumerable::Min<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                   class [mscorlib]System.Func`2<!!0,valuetype [mscorlib]System.Decimal>)
      IL_0018:  stloc.1
      .line 70,70 : 9,41 ''
      IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_001e:  stloc.3
      IL_001f:  ldloc.3
      IL_0020:  ldloc.0
      IL_0021:  ldnull
      IL_0022:  ldnull
      IL_0023:  ldnull
      IL_0024:  ldc.i4.0
      IL_0025:  ldnull
      IL_0026:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_002b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0030:  ldloc.1
      IL_0031:  newobj     instance void Linq101Aggregates01/'cheapestProducts@69-1'::.ctor(valuetype [mscorlib]System.Decimal)
      IL_0036:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_003b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0040:  stloc.2
      .line 70,70 : 9,41 ''
      IL_0041:  ldarg.0
      IL_0042:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories3@67-3'::builder@
      IL_0047:  ldloc.0
      IL_0048:  ldloc.1
      IL_0049:  ldloc.2
      IL_004a:  newobj     instance void class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_004f:  tail.
      IL_0051:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0056:  ret
    } // end of method 'categories3@67-3'::Invoke

  } // end of class 'categories3@67-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories3@70-4'
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
      // Code size       34 (0x22)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal min,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> cheapestProducts)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      .line 70,70 : 17,40 ''
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0021:  ret
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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 74,74 : 22,41 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 22,41 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      .line 74,74 : 22,41 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      IL_006a:  stfld      int32 Linq101Aggregates01/maxNum@74::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 42,49 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101Aggregates01/maxNum@74::n
      IL_007d:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101Aggregates01/maxNum@74::n
      .line 74,74 : 22,41 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/maxNum@74::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      .line 74,74 : 22,41 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method maxNum@74::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0006:  ret
    } // end of method maxNum@74::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_000a:  ret
    } // end of method maxNum@74::GetFreshEnumerator

  } // end of class maxNum@74

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxNum@74-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 74,74 : 48,49 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
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
      // Code size       191 (0xbf)
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
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 77,77 : 29,46 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 29,46 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005e:  stfld      string Linq101Aggregates01/longestLength@77::_arg1
      .line 77,77 : 29,46 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      string Linq101Aggregates01/longestLength@77::_arg1
      IL_006a:  stfld      string Linq101Aggregates01/longestLength@77::w
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 47,61 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      string Linq101Aggregates01/longestLength@77::w
      IL_007d:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      string Linq101Aggregates01/longestLength@77::w
      .line 77,77 : 29,46 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/longestLength@77::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      .line 77,77 : 29,46 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method longestLength@77::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/longestLength@77::current
      IL_0006:  ret
    } // end of method longestLength@77::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(string,
                                                                                     string,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                     int32,
                                                                                     string)
      IL_000a:  ret
    } // end of method longestLength@77::GetFreshEnumerator

  } // end of class longestLength@77

  .class auto ansi serializable sealed nested assembly beforefieldinit 'longestLength@77-1'
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
      // Code size       7 (0x7)
      .maxstack  8
      .line 77,77 : 53,61 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'longestLength@77-1'::Invoke

  } // end of class 'longestLength@77-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories4@82
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 82,82 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 83,83 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories4@82::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories4@82::Invoke

  } // end of class categories4@82

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories4@83-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 83,83 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories4@83-1'::Invoke

  } // end of class 'categories4@83-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories4@83-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 83,83 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 84,84 : 42,55 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 42,55 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      .line 84,84 : 42,55 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 56,73 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::x
      .line 84,84 : 42,55 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      .line 84,84 : 42,55 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method mostExpensivePrice@84::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0006:  ret
    } // end of method mostExpensivePrice@84::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method mostExpensivePrice@84::GetFreshEnumerator

  } // end of class mostExpensivePrice@84

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensivePrice@84-1'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 84,84 : 62,73 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'mostExpensivePrice@84-1'::Invoke

  } // end of class 'mostExpensivePrice@84-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories4@83-3'
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
      // Code size       57 (0x39)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal mostExpensivePrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 83,83 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  ldloc.0
      IL_000a:  ldnull
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldc.i4.0
      IL_000e:  ldnull
      IL_000f:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [Utils]Utils/Product,
                                                                                          class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0014:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0019:  newobj     instance void Linq101Aggregates01/'mostExpensivePrice@84-1'::.ctor()
      IL_001e:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0023:  stloc.1
      .line 85,85 : 9,43 ''
      IL_0024:  ldarg.0
      IL_0025:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories4@83-3'::builder@
      IL_002a:  ldloc.0
      IL_002b:  ldloc.1
      IL_002c:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_0031:  tail.
      IL_0033:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_0038:  ret
    } // end of method 'categories4@83-3'::Invoke

  } // end of class 'categories4@83-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories4@85-4'
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
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal mostExpensivePrice)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      .line 85,85 : 17,42 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
    } // end of method 'categories4@85-4'::Invoke

  } // end of class 'categories4@85-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories5@91
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 91,91 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 92,92 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories5@91::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories5@91::Invoke

  } // end of class categories5@91

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories5@92-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 92,92 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories5@92-1'::Invoke

  } // end of class 'categories5@92-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories5@92-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 92,92 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 93,93 : 32,45 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 32,45 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      .line 93,93 : 32,45 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 46,63 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::x
      .line 93,93 : 32,45 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      .line 93,93 : 32,45 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method maxPrice@93::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0006:  ret
    } // end of method maxPrice@93::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [Utils]Utils/Product,
                                                                                class [Utils]Utils/Product,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method maxPrice@93::GetFreshEnumerator

  } // end of class maxPrice@93

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxPrice@93-1'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 93,93 : 52,63 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 94,94 : 45,58 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 45,58 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      .line 94,94 : 45,58 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 59,89 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::x
      .line 94,94 : 45,58 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::_arg4
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      .line 94,94 : 45,58 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method mostExpensiveProducts@94::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0006:  ret
    } // end of method mostExpensiveProducts@94::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method mostExpensiveProducts@94::GetFreshEnumerator

  } // end of class mostExpensiveProducts@94

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensiveProducts@94-1'
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
      // Code size       18 (0x12)
      .maxstack  8
      .line 94,94 : 66,88 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [mscorlib]System.Decimal Linq101Aggregates01/'mostExpensiveProducts@94-1'::maxPrice
      IL_000c:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
      IL_0011:  ret
    } // end of method 'mostExpensiveProducts@94-1'::Invoke

  } // end of class 'mostExpensiveProducts@94-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories5@92-3'
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
      // Code size       100 (0x64)
      .maxstack  11
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal maxPrice,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
               [3] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> mostExpensiveProducts,
               [4] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_4)
      .line 92,92 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  ldloc.0
      IL_000a:  ldnull
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldc.i4.0
      IL_000e:  ldnull
      IL_000f:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [Utils]Utils/Product,
                                                                                class [Utils]Utils/Product,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0014:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0019:  newobj     instance void Linq101Aggregates01/'maxPrice@93-1'::.ctor()
      IL_001e:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0023:  stloc.1
      .line 95,95 : 9,46 ''
      IL_0024:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0029:  stloc.s    V_4
      IL_002b:  ldloc.s    V_4
      IL_002d:  ldloc.0
      IL_002e:  ldnull
      IL_002f:  ldnull
      IL_0030:  ldnull
      IL_0031:  ldc.i4.0
      IL_0032:  ldnull
      IL_0033:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [Utils]Utils/Product,
                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0038:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_003d:  ldloc.1
      IL_003e:  newobj     instance void Linq101Aggregates01/'mostExpensiveProducts@94-1'::.ctor(valuetype [mscorlib]System.Decimal)
      IL_0043:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0048:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_004d:  stloc.3
      .line 95,95 : 9,46 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories5@92-3'::builder@
      IL_0054:  ldloc.0
      IL_0055:  ldloc.1
      IL_0056:  ldloc.3
      IL_0057:  newobj     instance void class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_005c:  tail.
      IL_005e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0063:  ret
    } // end of method 'categories5@92-3'::Invoke

  } // end of class 'categories5@92-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories5@95-4'
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
      // Code size       34 (0x22)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal maxPrice,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> mostExpensiveProducts)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      .line 95,95 : 17,45 ''
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0021:  ret
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
      // Code size       218 (0xda)
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
      IL_0021:  nop
      IL_0022:  br         IL_00a8

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0087

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br         IL_00c9

      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      .line 100,100 : 26,46 ''
      IL_0031:  ldarg.0
      IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
      IL_0037:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
      IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0041:  ldarg.0
      IL_0042:  ldc.i4.1
      IL_0043:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 26,46 ''
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_004e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0053:  brfalse.s  IL_00a8

      IL_0055:  ldarg.0
      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_005c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0061:  stfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      .line 100,100 : 26,46 ''
      IL_0066:  ldarg.0
      IL_0067:  ldarg.0
      IL_0068:  ldfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      IL_006d:  stfld      float64 Linq101Aggregates01/averageNum@100::n
      IL_0072:  ldarg.0
      IL_0073:  ldc.i4.2
      IL_0074:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 47,58 ''
      IL_0079:  ldarg.0
      IL_007a:  ldarg.0
      IL_007b:  ldfld      float64 Linq101Aggregates01/averageNum@100::n
      IL_0080:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0085:  ldc.i4.1
      IL_0086:  ret

      IL_0087:  ldarg.0
      IL_0088:  ldc.r8     0.0
      IL_0091:  stfld      float64 Linq101Aggregates01/averageNum@100::n
      .line 100,100 : 26,46 ''
      IL_0096:  ldarg.0
      IL_0097:  ldc.r8     0.0
      IL_00a0:  stfld      float64 Linq101Aggregates01/averageNum@100::_arg1
      .line 100001,100001 : 0,0 ''
      IL_00a5:  nop
      IL_00a6:  br.s       IL_0048

      IL_00a8:  ldarg.0
      IL_00a9:  ldc.i4.3
      IL_00aa:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      .line 100,100 : 26,46 ''
      IL_00af:  ldarg.0
      IL_00b0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_00b5:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
      IL_00ba:  nop
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_00c2:  ldarg.0
      IL_00c3:  ldc.i4.3
      IL_00c4:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_00c9:  ldarg.0
      IL_00ca:  ldc.r8     0.0
      IL_00d3:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_00d8:  ldc.i4.0
      IL_00d9:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0091

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method averageNum@100::get_CheckClose

    .method public strict virtual instance float64 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0006:  ret
    } // end of method averageNum@100::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       35 (0x23)
      .maxstack  9
      IL_0000:  ldc.r8     0.0
      IL_0009:  ldc.r8     0.0
      IL_0012:  ldnull
      IL_0013:  ldc.i4.0
      IL_0014:  ldc.r8     0.0
      IL_001d:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(float64,
                                                                                   float64,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                   int32,
                                                                                   float64)
      IL_0022:  ret
    } // end of method averageNum@100::GetFreshEnumerator

  } // end of class averageNum@100

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageNum@100-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 100,100 : 57,58 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'averageNum@100-1'::Invoke

  } // end of class 'averageNum@100-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit averageLength@105
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
      // Code size       31 (0x1f)
      .maxstack  7
      .locals init ([0] string w,
               [1] float64 wl)
      .line 105,105 : 9,26 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 106,106 : 9,35 ''
      IL_0002:  ldloc.0
      IL_0003:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0008:  conv.r8
      IL_0009:  stloc.1
      .line 107,107 : 9,21 ''
      IL_000a:  ldarg.0
      IL_000b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/averageLength@105::builder@
      IL_0010:  ldloc.0
      IL_0011:  ldloc.1
      IL_0012:  newobj     instance void class [mscorlib]System.Tuple`2<string,float64>::.ctor(!0,
                                                                                               !1)
      IL_0017:  tail.
      IL_0019:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<string,float64>,object>(!!0)
      IL_001e:  ret
    } // end of method averageLength@105::Invoke

  } // end of class averageLength@105

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageLength@107-1'
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
      // Code size       16 (0x10)
      .maxstack  5
      .locals init ([0] string w,
               [1] float64 wl)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<string,float64>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<string,float64>::get_Item2()
      IL_000d:  stloc.1
      .line 107,107 : 19,21 ''
      IL_000e:  ldloc.1
      IL_000f:  ret
    } // end of method 'averageLength@107-1'::Invoke

  } // end of class 'averageLength@107-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit categories6@113
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
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 113,113 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 114,114 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/categories6@113::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method categories6@113::Invoke

  } // end of class categories6@113

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories6@114-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 114,114 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'categories6@114-1'::Invoke

  } // end of class 'categories6@114-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories6@114-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 114,114 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
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
      // Code size       192 (0xc0)
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
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0096

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b7

      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 115,115 : 36,49 ''
      IL_002e:  ldarg.0
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 36,49 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0096

      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_005a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      .line 115,115 : 36,49 ''
      IL_0064:  ldarg.0
      IL_0065:  ldarg.0
      IL_0066:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      IL_006b:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.2
      IL_0072:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 50,71 ''
      IL_0077:  ldarg.0
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      IL_007e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.0
      IL_0086:  ldnull
      IL_0087:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::x
      .line 115,115 : 36,49 ''
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::_arg3
      .line 100001,100001 : 0,0 ''
      IL_0093:  nop
      IL_0094:  br.s       IL_0046

      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      .line 115,115 : 36,49 ''
      IL_009d:  ldarg.0
      IL_009e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_00a3:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a8:  nop
      IL_00a9:  ldarg.0
      IL_00aa:  ldnull
      IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_00b0:  ldarg.0
      IL_00b1:  ldc.i4.3
      IL_00b2:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_00b7:  ldarg.0
      IL_00b8:  ldnull
      IL_00b9:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_00be:  ldc.i4.0
      IL_00bf:  ret
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
      IL_0015:  nop
      IL_0016:  br         IL_0089

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
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

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
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method averagePrice@115::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0006:  ret
    } // end of method averagePrice@115::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  10
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0006:  ldnull
      IL_0007:  ldnull
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0010:  ret
    } // end of method averagePrice@115::GetFreshEnumerator

  } // end of class averagePrice@115

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averagePrice@115-1'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 115,115 : 60,71 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'averagePrice@115-1'::Invoke

  } // end of class 'averagePrice@115-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories6@114-3'
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
      // Code size       232 (0xe8)
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
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldnull
      IL_000d:  ldnull
      IL_000e:  ldc.i4.0
      IL_000f:  ldnull
      IL_0010:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001a:  stloc.s    V_4
      IL_001c:  newobj     instance void Linq101Aggregates01/'averagePrice@115-1'::.ctor()
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloc.s    V_4
      IL_0025:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_002a:  stloc.s    V_6
      IL_002c:  ldloc.s    V_6
      IL_002e:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0033:  brfalse.s  IL_0037

      IL_0035:  br.s       IL_0042

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldstr      "source"
      IL_003c:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
      IL_0041:  throw

      .line 100001,100001 : 0,0 ''
      IL_0042:  nop
      IL_0043:  ldloc.s    V_6
      IL_0045:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_004a:  stloc.s    V_7
      .try
      {
        IL_004c:  ldc.i4.0
        IL_004d:  ldc.i4.0
        IL_004e:  ldc.i4.0
        IL_004f:  ldc.i4.0
        IL_0050:  ldc.i4.0
        IL_0051:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                           int32,
                                                                           int32,
                                                                           bool,
                                                                           uint8)
        IL_0056:  stloc.s    V_9
        IL_0058:  ldc.i4.0
        IL_0059:  stloc.s    V_10
        IL_005b:  ldloc.s    V_7
        IL_005d:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0062:  brfalse.s  IL_0084

        IL_0064:  ldloc.s    V_9
        IL_0066:  ldloc.s    V_5
        IL_0068:  ldloc.s    V_7
        IL_006a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_006f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::Invoke(!0)
        IL_0074:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                      valuetype [mscorlib]System.Decimal)
        IL_0079:  stloc.s    V_9
        .line 115,115 : 50,71 ''
        IL_007b:  ldloc.s    V_10
        IL_007d:  ldc.i4.1
        IL_007e:  add
        IL_007f:  stloc.s    V_10
        .line 100001,100001 : 0,0 ''
        IL_0081:  nop
        IL_0082:  br.s       IL_005b

        IL_0084:  ldloc.s    V_10
        IL_0086:  brtrue.s   IL_008a

        IL_0088:  br.s       IL_008c

        IL_008a:  br.s       IL_0097

        .line 100001,100001 : 0,0 ''
        IL_008c:  ldstr      "source"
        IL_0091:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
        IL_0096:  throw

        .line 100001,100001 : 0,0 ''
        IL_0097:  nop
        IL_0098:  ldloc.s    V_9
        IL_009a:  stloc.s    V_11
        IL_009c:  ldloc.s    V_10
        IL_009e:  stloc.s    V_12
        IL_00a0:  ldloc.s    V_11
        IL_00a2:  ldloc.s    V_12
        IL_00a4:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Convert::ToDecimal(int32)
        IL_00a9:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Divide(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
        IL_00ae:  stloc.s    V_8
        IL_00b0:  leave.s    IL_00d0

      }  // end .try
      finally
      {
        IL_00b2:  ldloc.s    V_7
        IL_00b4:  isinst     [mscorlib]System.IDisposable
        IL_00b9:  stloc.s    V_13
        IL_00bb:  ldloc.s    V_13
        IL_00bd:  brfalse.s  IL_00c1

        IL_00bf:  br.s       IL_00c3

        IL_00c1:  br.s       IL_00cd

        .line 100001,100001 : 0,0 ''
        IL_00c3:  ldloc.s    V_13
        IL_00c5:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00ca:  ldnull
        IL_00cb:  pop
        IL_00cc:  endfinally
        .line 100001,100001 : 0,0 ''
        IL_00cd:  ldnull
        IL_00ce:  pop
        IL_00cf:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_00d0:  ldloc.s    V_8
      IL_00d2:  stloc.1
      .line 116,116 : 9,37 ''
      IL_00d3:  ldarg.0
      IL_00d4:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'categories6@114-3'::builder@
      IL_00d9:  ldloc.0
      IL_00da:  ldloc.1
      IL_00db:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_00e0:  tail.
      IL_00e2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(!!0)
      IL_00e7:  ret
    } // end of method 'categories6@114-3'::Invoke

  } // end of class 'categories6@114-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'categories6@116-4'
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
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
               [1] valuetype [mscorlib]System.Decimal averagePrice)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      .line 116,116 : 17,36 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
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
    // Code size       1735 (0x6c7)
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
             [26] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_26,
             [27] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_27,
             [28] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable> V_28,
             [29] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_29,
             [30] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_30,
             [31] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             [32] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_32,
             [33] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_33,
             [34] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_34,
             [35] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_35,
             [36] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_36,
             [37] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_37,
             [38] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_38,
             [39] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_39,
             [40] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_40,
             [41] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             [42] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable> V_42,
             [43] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_43,
             [44] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_44,
             [45] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_45,
             [46] float64 V_46,
             [47] float64 V_47,
             [48] int32 V_48,
             [49] float64 V_49,
             [50] int32 V_50,
             [51] class [mscorlib]System.IDisposable V_51,
             [52] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_52,
             [53] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_53,
             [54] class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable> V_54,
             [55] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64> V_55,
             [56] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>> V_56,
             [57] class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>> V_57,
             [58] float64 V_58,
             [59] float64 V_59,
             [60] int32 V_60,
             [61] float64 V_61,
             [62] int32 V_62,
             [63] class [mscorlib]System.IDisposable V_63,
             [64] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_64)
    .line 8,8 : 1,31 ''
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  ldc.i4.5
    IL_0004:  ldc.i4.5
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  dup
    IL_0024:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::factorsOf300@8
    IL_0029:  stloc.0
    .line 10,14 : 1,20 ''
    IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_002f:  stloc.s    builder@
    IL_0031:  ldloc.s    builder@
    IL_0033:  ldc.i4.0
    IL_0034:  ldc.i4.0
    IL_0035:  ldnull
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4.0
    IL_0038:  newobj     instance void Linq101Aggregates01/uniqueFactors@12::.ctor(int32,
                                                                                   int32,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                   int32,
                                                                                   int32)
    IL_003d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0042:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0047:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_004c:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0051:  dup
    IL_0052:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::uniqueFactors@10
    IL_0057:  stloc.1
    .line 17,17 : 1,47 ''
    IL_0058:  ldc.i4.5
    IL_0059:  ldc.i4.4
    IL_005a:  ldc.i4.1
    IL_005b:  ldc.i4.3
    IL_005c:  ldc.i4.s   9
    IL_005e:  ldc.i4.8
    IL_005f:  ldc.i4.6
    IL_0060:  ldc.i4.7
    IL_0061:  ldc.i4.2
    IL_0062:  ldc.i4.0
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0072:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0077:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0081:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0086:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0090:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0095:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009a:  dup
    IL_009b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers@17
    IL_00a0:  stloc.2
    IL_00a1:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a6:  stloc.s    V_21
    IL_00a8:  ldloc.s    V_21
    IL_00aa:  stloc.s    V_22
    IL_00ac:  ldc.i4.0
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldnull
    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.0
    IL_00b1:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_00b6:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00bb:  stloc.s    V_23
    IL_00bd:  newobj     instance void Linq101Aggregates01/'numSum@22-1'::.ctor()
    IL_00c2:  stloc.s    V_24
    IL_00c4:  ldloc.s    V_23
    IL_00c6:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00cb:  stloc.s    V_25
    IL_00cd:  ldloc.s    V_25
    IL_00cf:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00d4:  brfalse.s  IL_00d8

    IL_00d6:  br.s       IL_00e3

    .line 100001,100001 : 0,0 ''
    IL_00d8:  ldstr      "source"
    IL_00dd:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_00e2:  throw

    .line 100001,100001 : 0,0 ''
    IL_00e3:  nop
    IL_00e4:  ldloc.s    V_25
    IL_00e6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ISeqModule::OfSeq<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00eb:  ldloc.s    V_24
    IL_00ed:  newobj     instance void Linq101Aggregates01/'numSum@22-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>)
    IL_00f2:  callvirt   instance !!0 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<int32>::Fold<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<!0,!!0>>)
    IL_00f7:  dup
    IL_00f8:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_00fd:  stloc.3
    .line 26,26 : 1,45 ''
    IL_00fe:  ldstr      "cherry"
    IL_0103:  ldstr      "apple"
    IL_0108:  ldstr      "blueberry"
    IL_010d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0112:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0117:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_011c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0121:  dup
    IL_0122:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_0127:  stloc.s    words
    IL_0129:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_012e:  stloc.s    V_26
    IL_0130:  ldloc.s    V_26
    IL_0132:  stloc.s    V_27
    IL_0134:  ldnull
    IL_0135:  ldnull
    IL_0136:  ldnull
    IL_0137:  ldc.i4.0
    IL_0138:  ldnull
    IL_0139:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_013e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0143:  stloc.s    V_28
    IL_0145:  newobj     instance void Linq101Aggregates01/'totalChars@31-1'::.ctor()
    IL_014a:  stloc.s    V_29
    IL_014c:  ldloc.s    V_28
    IL_014e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0153:  stloc.s    V_30
    IL_0155:  ldloc.s    V_30
    IL_0157:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
    IL_015c:  brfalse.s  IL_0160

    IL_015e:  br.s       IL_016b

    .line 100001,100001 : 0,0 ''
    IL_0160:  ldstr      "source"
    IL_0165:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_016a:  throw

    .line 100001,100001 : 0,0 ''
    IL_016b:  nop
    IL_016c:  ldloc.s    V_30
    IL_016e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ISeqModule::OfSeq<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0173:  ldloc.s    V_29
    IL_0175:  newobj     instance void Linq101Aggregates01/'totalChars@31-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>)
    IL_017a:  callvirt   instance !!0 class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.ISeq`1<string>::Fold<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.SeqComposition.Folder`2<!0,!!0>>)
    IL_017f:  dup
    IL_0180:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_0185:  stloc.s    totalChars
    .line 35,35 : 1,32 ''
    IL_0187:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_018c:  dup
    IL_018d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_0192:  stloc.s    products
    .line 37,46 : 1,21 ''
    IL_0194:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0199:  stloc.s    V_31
    IL_019b:  ldloc.s    V_31
    IL_019d:  ldloc.s    V_31
    IL_019f:  ldloc.s    V_31
    IL_01a1:  ldloc.s    V_31
    IL_01a3:  ldloc.s    V_31
    IL_01a5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_01aa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01af:  ldloc.s    V_31
    IL_01b1:  newobj     instance void Linq101Aggregates01/categories@39::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01bb:  newobj     instance void Linq101Aggregates01/'categories@40-1'::.ctor()
    IL_01c0:  newobj     instance void Linq101Aggregates01/'categories@40-2'::.ctor()
    IL_01c5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_01ca:  ldloc.s    V_31
    IL_01cc:  newobj     instance void Linq101Aggregates01/'categories@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01d1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01d6:  newobj     instance void Linq101Aggregates01/'categories@45-4'::.ctor()
    IL_01db:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01e0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,int32>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01e5:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01ea:  dup
    IL_01eb:  stsfld     class [mscorlib]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_01f0:  stloc.s    categories
    IL_01f2:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01f7:  stloc.s    V_32
    IL_01f9:  ldloc.s    V_32
    IL_01fb:  ldc.i4.0
    IL_01fc:  ldc.i4.0
    IL_01fd:  ldnull
    IL_01fe:  ldc.i4.0
    IL_01ff:  ldc.i4.0
    IL_0200:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0205:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_020a:  newobj     instance void Linq101Aggregates01/'minNum@49-1'::.ctor()
    IL_020f:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0214:  dup
    IL_0215:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_021a:  stloc.s    minNum
    IL_021c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0221:  stloc.s    V_33
    IL_0223:  ldloc.s    V_33
    IL_0225:  ldnull
    IL_0226:  ldnull
    IL_0227:  ldnull
    IL_0228:  ldc.i4.0
    IL_0229:  ldnull
    IL_022a:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_022f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0234:  newobj     instance void Linq101Aggregates01/'shortestWord@52-1'::.ctor()
    IL_0239:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_023e:  dup
    IL_023f:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0244:  stloc.s    shortestWord
    .line 55,61 : 1,21 ''
    IL_0246:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_024b:  stloc.s    V_34
    IL_024d:  ldloc.s    V_34
    IL_024f:  ldloc.s    V_34
    IL_0251:  ldloc.s    V_34
    IL_0253:  ldloc.s    V_34
    IL_0255:  ldloc.s    V_34
    IL_0257:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_025c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0261:  ldloc.s    V_34
    IL_0263:  newobj     instance void Linq101Aggregates01/categories2@57::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0268:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_026d:  newobj     instance void Linq101Aggregates01/'categories2@58-1'::.ctor()
    IL_0272:  newobj     instance void Linq101Aggregates01/'categories2@58-2'::.ctor()
    IL_0277:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_027c:  ldloc.s    V_34
    IL_027e:  newobj     instance void Linq101Aggregates01/'categories2@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0283:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0288:  newobj     instance void Linq101Aggregates01/'categories2@60-4'::.ctor()
    IL_028d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0292:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0297:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_029c:  dup
    IL_029d:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_02a2:  stloc.s    categories2
    .line 64,71 : 1,21 ''
    IL_02a4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02a9:  stloc.s    V_35
    IL_02ab:  ldloc.s    V_35
    IL_02ad:  ldloc.s    V_35
    IL_02af:  ldloc.s    V_35
    IL_02b1:  ldloc.s    V_35
    IL_02b3:  ldloc.s    V_35
    IL_02b5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_02ba:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02bf:  ldloc.s    V_35
    IL_02c1:  newobj     instance void Linq101Aggregates01/categories3@66::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02c6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02cb:  newobj     instance void Linq101Aggregates01/'categories3@67-1'::.ctor()
    IL_02d0:  newobj     instance void Linq101Aggregates01/'categories3@67-2'::.ctor()
    IL_02d5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02da:  ldloc.s    V_35
    IL_02dc:  newobj     instance void Linq101Aggregates01/'categories3@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02e1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02e6:  newobj     instance void Linq101Aggregates01/'categories3@70-4'::.ctor()
    IL_02eb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02f0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_02f5:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02fa:  dup
    IL_02fb:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_0300:  stloc.s    categories3
    IL_0302:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0307:  stloc.s    V_36
    IL_0309:  ldloc.s    V_36
    IL_030b:  ldc.i4.0
    IL_030c:  ldc.i4.0
    IL_030d:  ldnull
    IL_030e:  ldc.i4.0
    IL_030f:  ldc.i4.0
    IL_0310:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(int32,
                                                                            int32,
                                                                            class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0315:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_031a:  newobj     instance void Linq101Aggregates01/'maxNum@74-1'::.ctor()
    IL_031f:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0324:  dup
    IL_0325:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_032a:  stloc.s    maxNum
    IL_032c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0331:  stloc.s    V_37
    IL_0333:  ldloc.s    V_37
    IL_0335:  ldnull
    IL_0336:  ldnull
    IL_0337:  ldnull
    IL_0338:  ldc.i4.0
    IL_0339:  ldnull
    IL_033a:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(string,
                                                                                   string,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_033f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0344:  newobj     instance void Linq101Aggregates01/'longestLength@77-1'::.ctor()
    IL_0349:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_034e:  dup
    IL_034f:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_0354:  stloc.s    longestLength
    .line 80,86 : 1,21 ''
    IL_0356:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_035b:  stloc.s    V_38
    IL_035d:  ldloc.s    V_38
    IL_035f:  ldloc.s    V_38
    IL_0361:  ldloc.s    V_38
    IL_0363:  ldloc.s    V_38
    IL_0365:  ldloc.s    V_38
    IL_0367:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_036c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0371:  ldloc.s    V_38
    IL_0373:  newobj     instance void Linq101Aggregates01/categories4@82::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0378:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_037d:  newobj     instance void Linq101Aggregates01/'categories4@83-1'::.ctor()
    IL_0382:  newobj     instance void Linq101Aggregates01/'categories4@83-2'::.ctor()
    IL_0387:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_038c:  ldloc.s    V_38
    IL_038e:  newobj     instance void Linq101Aggregates01/'categories4@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0393:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0398:  newobj     instance void Linq101Aggregates01/'categories4@85-4'::.ctor()
    IL_039d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03a2:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_03a7:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03ac:  dup
    IL_03ad:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_03b2:  stloc.s    categories4
    .line 89,96 : 1,21 ''
    IL_03b4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03b9:  stloc.s    V_39
    IL_03bb:  ldloc.s    V_39
    IL_03bd:  ldloc.s    V_39
    IL_03bf:  ldloc.s    V_39
    IL_03c1:  ldloc.s    V_39
    IL_03c3:  ldloc.s    V_39
    IL_03c5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_03ca:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03cf:  ldloc.s    V_39
    IL_03d1:  newobj     instance void Linq101Aggregates01/categories5@91::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03d6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03db:  newobj     instance void Linq101Aggregates01/'categories5@92-1'::.ctor()
    IL_03e0:  newobj     instance void Linq101Aggregates01/'categories5@92-2'::.ctor()
    IL_03e5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_03ea:  ldloc.s    V_39
    IL_03ec:  newobj     instance void Linq101Aggregates01/'categories5@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03f1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03f6:  newobj     instance void Linq101Aggregates01/'categories5@95-4'::.ctor()
    IL_03fb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0400:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0405:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_040a:  dup
    IL_040b:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0410:  stloc.s    categories5
    .line 99,99 : 1,66 ''
    IL_0412:  ldc.r8     5.
    IL_041b:  ldc.r8     4.
    IL_0424:  ldc.r8     1.
    IL_042d:  ldc.r8     3.
    IL_0436:  ldc.r8     9.
    IL_043f:  ldc.r8     8.
    IL_0448:  ldc.r8     6.
    IL_0451:  ldc.r8     7.
    IL_045a:  ldc.r8     2.
    IL_0463:  ldc.r8     0.0
    IL_046c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_0471:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0476:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_047b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0480:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0485:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_048a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_048f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0494:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0499:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_049e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04a3:  dup
    IL_04a4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_04a9:  stloc.s    numbers2
    IL_04ab:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_04b0:  stloc.s    V_40
    IL_04b2:  ldloc.s    V_40
    IL_04b4:  stloc.s    V_41
    IL_04b6:  ldc.r8     0.0
    IL_04bf:  ldc.r8     0.0
    IL_04c8:  ldnull
    IL_04c9:  ldc.i4.0
    IL_04ca:  ldc.r8     0.0
    IL_04d3:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(float64,
                                                                                 float64,
                                                                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_04d8:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_04dd:  stloc.s    V_42
    IL_04df:  newobj     instance void Linq101Aggregates01/'averageNum@100-1'::.ctor()
    IL_04e4:  stloc.s    V_43
    IL_04e6:  ldloc.s    V_42
    IL_04e8:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_04ed:  stloc.s    V_44
    IL_04ef:  ldloc.s    V_44
    IL_04f1:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_04f6:  brfalse.s  IL_04fa

    IL_04f8:  br.s       IL_0505

    .line 100001,100001 : 0,0 ''
    IL_04fa:  ldstr      "source"
    IL_04ff:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_0504:  throw

    .line 100001,100001 : 0,0 ''
    IL_0505:  nop
    IL_0506:  ldloc.s    V_44
    IL_0508:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_050d:  stloc.s    V_45
    .try
    {
      IL_050f:  ldc.r8     0.0
      IL_0518:  stloc.s    V_47
      IL_051a:  ldc.i4.0
      IL_051b:  stloc.s    V_48
      IL_051d:  ldloc.s    V_45
      IL_051f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0524:  brfalse.s  IL_0542

      IL_0526:  ldloc.s    V_47
      IL_0528:  ldloc.s    V_43
      IL_052a:  ldloc.s    V_45
      IL_052c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0531:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_0536:  add
      IL_0537:  stloc.s    V_47
      .line 100,100 : 47,58 ''
      IL_0539:  ldloc.s    V_48
      IL_053b:  ldc.i4.1
      IL_053c:  add
      IL_053d:  stloc.s    V_48
      .line 100001,100001 : 0,0 ''
      IL_053f:  nop
      IL_0540:  br.s       IL_051d

      IL_0542:  ldloc.s    V_48
      IL_0544:  brtrue.s   IL_0548

      IL_0546:  br.s       IL_054a

      IL_0548:  br.s       IL_0555

      .line 100001,100001 : 0,0 ''
      IL_054a:  ldstr      "source"
      IL_054f:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_0554:  throw

      .line 100001,100001 : 0,0 ''
      IL_0555:  nop
      IL_0556:  ldloc.s    V_47
      IL_0558:  stloc.s    V_49
      IL_055a:  ldloc.s    V_48
      IL_055c:  stloc.s    V_50
      IL_055e:  ldloc.s    V_49
      IL_0560:  ldloc.s    V_50
      IL_0562:  conv.r8
      IL_0563:  div
      IL_0564:  stloc.s    V_46
      IL_0566:  leave.s    IL_0586

    }  // end .try
    finally
    {
      IL_0568:  ldloc.s    V_45
      IL_056a:  isinst     [mscorlib]System.IDisposable
      IL_056f:  stloc.s    V_51
      IL_0571:  ldloc.s    V_51
      IL_0573:  brfalse.s  IL_0577

      IL_0575:  br.s       IL_0579

      IL_0577:  br.s       IL_0583

      .line 100001,100001 : 0,0 ''
      IL_0579:  ldloc.s    V_51
      IL_057b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0580:  ldnull
      IL_0581:  pop
      IL_0582:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0583:  ldnull
      IL_0584:  pop
      IL_0585:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0586:  ldloc.s    V_46
    IL_0588:  dup
    IL_0589:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_058e:  stloc.s    averageNum
    IL_0590:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0595:  stloc.s    V_52
    IL_0597:  ldloc.s    V_52
    IL_0599:  stloc.s    V_53
    IL_059b:  ldloc.s    V_52
    IL_059d:  ldloc.s    V_52
    IL_059f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
    IL_05a4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_05a9:  ldloc.s    V_52
    IL_05ab:  newobj     instance void Linq101Aggregates01/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_05b0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_05b5:  stloc.s    V_54
    IL_05b7:  newobj     instance void Linq101Aggregates01/'averageLength@107-1'::.ctor()
    IL_05bc:  stloc.s    V_55
    IL_05be:  ldloc.s    V_54
    IL_05c0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,float64>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_05c5:  stloc.s    V_56
    IL_05c7:  ldloc.s    V_56
    IL_05c9:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>
    IL_05ce:  brfalse.s  IL_05d2

    IL_05d0:  br.s       IL_05dd

    .line 100001,100001 : 0,0 ''
    IL_05d2:  ldstr      "source"
    IL_05d7:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_05dc:  throw

    .line 100001,100001 : 0,0 ''
    IL_05dd:  nop
    IL_05de:  ldloc.s    V_56
    IL_05e0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_05e5:  stloc.s    V_57
    .try
    {
      IL_05e7:  ldc.r8     0.0
      IL_05f0:  stloc.s    V_59
      IL_05f2:  ldc.i4.0
      IL_05f3:  stloc.s    V_60
      IL_05f5:  ldloc.s    V_57
      IL_05f7:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_05fc:  brfalse.s  IL_061a

      IL_05fe:  ldloc.s    V_59
      IL_0600:  ldloc.s    V_55
      IL_0602:  ldloc.s    V_57
      IL_0604:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,float64>>::get_Current()
      IL_0609:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_060e:  add
      IL_060f:  stloc.s    V_59
      .line 107,107 : 9,21 ''
      IL_0611:  ldloc.s    V_60
      IL_0613:  ldc.i4.1
      IL_0614:  add
      IL_0615:  stloc.s    V_60
      .line 100001,100001 : 0,0 ''
      IL_0617:  nop
      IL_0618:  br.s       IL_05f5

      IL_061a:  ldloc.s    V_60
      IL_061c:  brtrue.s   IL_0620

      IL_061e:  br.s       IL_0622

      IL_0620:  br.s       IL_062d

      .line 100001,100001 : 0,0 ''
      IL_0622:  ldstr      "source"
      IL_0627:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor(string)
      IL_062c:  throw

      .line 100001,100001 : 0,0 ''
      IL_062d:  nop
      IL_062e:  ldloc.s    V_59
      IL_0630:  stloc.s    V_61
      IL_0632:  ldloc.s    V_60
      IL_0634:  stloc.s    V_62
      IL_0636:  ldloc.s    V_61
      IL_0638:  ldloc.s    V_62
      IL_063a:  conv.r8
      IL_063b:  div
      IL_063c:  stloc.s    V_58
      IL_063e:  leave.s    IL_065e

    }  // end .try
    finally
    {
      IL_0640:  ldloc.s    V_57
      IL_0642:  isinst     [mscorlib]System.IDisposable
      IL_0647:  stloc.s    V_63
      IL_0649:  ldloc.s    V_63
      IL_064b:  brfalse.s  IL_064f

      IL_064d:  br.s       IL_0651

      IL_064f:  br.s       IL_065b

      .line 100001,100001 : 0,0 ''
      IL_0651:  ldloc.s    V_63
      IL_0653:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0658:  ldnull
      IL_0659:  pop
      IL_065a:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_065b:  ldnull
      IL_065c:  pop
      IL_065d:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_065e:  ldloc.s    V_58
    IL_0660:  dup
    IL_0661:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_0666:  stloc.s    averageLength
    .line 111,117 : 1,21 ''
    IL_0668:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_066d:  stloc.s    V_64
    IL_066f:  ldloc.s    V_64
    IL_0671:  ldloc.s    V_64
    IL_0673:  ldloc.s    V_64
    IL_0675:  ldloc.s    V_64
    IL_0677:  ldloc.s    V_64
    IL_0679:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_067e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0683:  ldloc.s    V_64
    IL_0685:  newobj     instance void Linq101Aggregates01/categories6@113::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_068a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_068f:  newobj     instance void Linq101Aggregates01/'categories6@114-1'::.ctor()
    IL_0694:  newobj     instance void Linq101Aggregates01/'categories6@114-2'::.ctor()
    IL_0699:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_069e:  ldloc.s    V_64
    IL_06a0:  newobj     instance void Linq101Aggregates01/'categories6@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_06a5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_06aa:  newobj     instance void Linq101Aggregates01/'categories6@116-4'::.ctor()
    IL_06af:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_06b4:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_06b9:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_06be:  dup
    IL_06bf:  stsfld     class [mscorlib]System.Tuple`2<string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_06c4:  stloc.s    categories6
    IL_06c6:  ret
  } // end of method $Linq101Aggregates01::main@

} // end of class '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
