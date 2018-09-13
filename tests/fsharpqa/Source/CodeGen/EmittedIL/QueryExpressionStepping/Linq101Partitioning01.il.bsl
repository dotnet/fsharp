
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
  .ver 4:5:0:0
}
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly Linq101Partitioning01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Partitioning01
{
  // Offset: 0x00000000 Length: 0x000003DE
}
.mresource public FSharpOptimizationData.Linq101Partitioning01
{
  // Offset: 0x000003E8 Length: 0x00000138
}
.module Linq101Partitioning01.exe
// MVID: {5B9A632A-B280-A6A2-A745-03832A639A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00AF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Partitioning01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname first3Numbers@12
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/first3Numbers@12::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method first3Numbers@12::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Partitioning01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/first3Numbers@12::pc
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

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 12,12 : 9,28 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      .line 12,12 : 9,28 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 12,12 : 9,28 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      .line 13,13 : 9,15 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      int32 Linq101Partitioning01/first3Numbers@12::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      .line 12,12 : 9,28 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.0
      IL_0093:  stfld      int32 Linq101Partitioning01/first3Numbers@12::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method first3Numbers@12::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/first3Numbers@12::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Partitioning01/first3Numbers@12::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/first3Numbers@12::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Partitioning01/first3Numbers@12::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 Linq101Partitioning01/first3Numbers@12::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 12,12 : 9,28 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method first3Numbers@12::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/first3Numbers@12::pc
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
    } // end of method first3Numbers@12::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/first3Numbers@12::current
      IL_0006:  ret
    } // end of method first3Numbers@12::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Partitioning01/first3Numbers@12::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                       int32,
                                                                                       int32)
      IL_0008:  ret
    } // end of method first3Numbers@12::GetFreshEnumerator

  } // end of class first3Numbers@12

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders@21-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'WAOrders@21-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Partitioning01/'WAOrders@21-1'::c
      IL_0014:  ret
    } // end of method 'WAOrders@21-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 21,21 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 22,22 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'WAOrders@21-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Partitioning01/'WAOrders@21-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'WAOrders@21-1'::Invoke

  } // end of class 'WAOrders@21-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit WAOrders@20
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders@20::builder@
      IL_000d:  ret
    } // end of method WAOrders@20::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 20,20 : 9,30 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 21,21 : 9,29 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders@20::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders@20::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders@20::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Partitioning01/'WAOrders@21-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                      class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method WAOrders@20::Invoke

  } // end of class WAOrders@20

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'WAOrders@22-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 22,22 : 16,31 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0014:  ldstr      "WA"
      IL_0019:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_001e:  ret
    } // end of method 'WAOrders@22-2'::Invoke

  } // end of class 'WAOrders@22-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders@23-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>::.ctor()
      IL_0006:  ret
    } // end of method 'WAOrders@23-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime> 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 23,23 : 17,53 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>::.ctor(!0,
                                                                                                                                 !1,
                                                                                                                                 !2)
      IL_0025:  ret
    } // end of method 'WAOrders@23-3'::Invoke

  } // end of class 'WAOrders@23-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname allButFirst4Numbers@29
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method allButFirst4Numbers@29::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
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

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 29,29 : 9,28 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      .line 29,29 : 9,28 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 29,29 : 9,28 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      .line 30,30 : 9,15 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      .line 29,29 : 9,28 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.0
      IL_0093:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method allButFirst4Numbers@29::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst4Numbers@29::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 29,29 : 9,28 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method allButFirst4Numbers@29::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::pc
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
    } // end of method allButFirst4Numbers@29::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst4Numbers@29::current
      IL_0006:  ret
    } // end of method allButFirst4Numbers@29::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Partitioning01/allButFirst4Numbers@29::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                             int32,
                                                                                             int32)
      IL_0008:  ret
    } // end of method allButFirst4Numbers@29::GetFreshEnumerator

  } // end of class allButFirst4Numbers@29

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders2@37-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'WAOrders2@37-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Partitioning01/'WAOrders2@37-1'::c
      IL_0014:  ret
    } // end of method 'WAOrders2@37-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 37,37 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 38,38 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'WAOrders2@37-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Partitioning01/'WAOrders2@37-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'WAOrders2@37-1'::Invoke

  } // end of class 'WAOrders2@37-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit WAOrders2@36
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders2@36::builder@
      IL_000d:  ret
    } // end of method WAOrders2@36::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 36,36 : 9,30 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 37,37 : 9,29 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders2@36::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders2@36::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/WAOrders2@36::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Partitioning01/'WAOrders2@37-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                       class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method WAOrders2@36::Invoke

  } // end of class WAOrders2@36

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders2@38-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'WAOrders2@38-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 38,38 : 16,31 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0014:  ldstr      "WA"
      IL_0019:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_001e:  ret
    } // end of method 'WAOrders2@38-2'::Invoke

  } // end of class 'WAOrders2@38-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'WAOrders2@39-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>::.ctor()
      IL_0006:  ret
    } // end of method 'WAOrders2@39-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime> 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 39,39 : 17,53 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>::.ctor(!0,
                                                                                                                                 !1,
                                                                                                                                 !2)
      IL_0025:  ret
    } // end of method 'WAOrders2@39-3'::Invoke

  } // end of class 'WAOrders2@39-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname firstNumbersLessThan6@45
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method firstNumbersLessThan6@45::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
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

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 45,45 : 9,28 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      .line 45,45 : 9,28 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 45,45 : 9,28 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      .line 46,46 : 9,26 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      .line 45,45 : 9,28 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.0
      IL_0093:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method firstNumbersLessThan6@45::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/firstNumbersLessThan6@45::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 45,45 : 9,28 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method firstNumbersLessThan6@45::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::pc
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
    } // end of method firstNumbersLessThan6@45::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/firstNumbersLessThan6@45::current
      IL_0006:  ret
    } // end of method firstNumbersLessThan6@45::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Partitioning01/firstNumbersLessThan6@45::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                               int32,
                                                                                               int32)
      IL_0008:  ret
    } // end of method firstNumbersLessThan6@45::GetFreshEnumerator

  } // end of class firstNumbersLessThan6@45

  .class auto ansi serializable sealed nested assembly beforefieldinit 'firstNumbersLessThan6@46-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'firstNumbersLessThan6@46-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .line 46,46 : 20,25 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.6
      IL_0002:  clt
      IL_0004:  ret
    } // end of method 'firstNumbersLessThan6@46-1'::Invoke

  } // end of class 'firstNumbersLessThan6@46-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname allButFirst3Numbers@52
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method allButFirst3Numbers@52::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
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

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 52,52 : 9,28 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      .line 52,52 : 9,28 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 52,52 : 9,28 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      .line 53,53 : 9,31 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      .line 52,52 : 9,28 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.0
      IL_0093:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method allButFirst3Numbers@52::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/allButFirst3Numbers@52::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 52,52 : 9,28 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method allButFirst3Numbers@52::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::pc
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
    } // end of method allButFirst3Numbers@52::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/allButFirst3Numbers@52::current
      IL_0006:  ret
    } // end of method allButFirst3Numbers@52::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Partitioning01/allButFirst3Numbers@52::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                             int32,
                                                                                             int32)
      IL_0008:  ret
    } // end of method allButFirst3Numbers@52::GetFreshEnumerator

  } // end of class allButFirst3Numbers@52

  .class auto ansi serializable sealed nested assembly beforefieldinit 'allButFirst3Numbers@53-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'allButFirst3Numbers@53-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 53,53 : 20,30 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.3
      IL_0002:  rem
      IL_0003:  ldc.i4.0
      IL_0004:  ceq
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method 'allButFirst3Numbers@53-1'::Invoke

  } // end of class 'allButFirst3Numbers@53-1'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::'numbers@7-5'
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_first3Numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::first3Numbers@10
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_first3Numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::'customers@17-2'
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_customers

  .method public specialname static class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] 
          get_WAOrders() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders@18
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_WAOrders

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_allButFirst4Numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst4Numbers@27
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_allButFirst4Numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> 
          get_WAOrders2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders2@34
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_WAOrders2

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_firstNumbersLessThan6() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::firstNumbersLessThan6@43
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_firstNumbersLessThan6

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_allButFirst3Numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst3Numbers@50
    IL_0005:  ret
  } // end of method Linq101Partitioning01::get_allButFirst3Numbers

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
  } // end of property Linq101Partitioning01::numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          first3Numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_first3Numbers()
  } // end of property Linq101Partitioning01::first3Numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Partitioning01::get_customers()
  } // end of property Linq101Partitioning01::customers
  .property class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[]
          WAOrders()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] Linq101Partitioning01::get_WAOrders()
  } // end of property Linq101Partitioning01::WAOrders
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          allButFirst4Numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_allButFirst4Numbers()
  } // end of property Linq101Partitioning01::allButFirst4Numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
          WAOrders2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> Linq101Partitioning01::get_WAOrders2()
  } // end of property Linq101Partitioning01::WAOrders2
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          firstNumbersLessThan6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_firstNumbersLessThan6()
  } // end of property Linq101Partitioning01::firstNumbersLessThan6
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          allButFirst3Numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_allButFirst3Numbers()
  } // end of property Linq101Partitioning01::allButFirst3Numbers
} // end of class Linq101Partitioning01

.class private abstract auto ansi sealed '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'numbers@7-5'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> first3Numbers@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 'customers@17-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] WAOrders@18
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst4Numbers@27
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> WAOrders2@34
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> firstNumbersLessThan6@43
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst3Numbers@50
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       432 (0x1b0)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> first3Numbers,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers,
             [3] class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] WAOrders,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst4Numbers,
             [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> WAOrders2,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> firstNumbersLessThan6,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst3Numbers,
             [8] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             [13] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_13)
    .line 7,7 : 1,47 ''
    IL_0000:  ldc.i4.5
    IL_0001:  ldc.i4.4
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.3
    IL_0004:  ldc.i4.s   9
    IL_0006:  ldc.i4.8
    IL_0007:  ldc.i4.6
    IL_0008:  ldc.i4.7
    IL_0009:  ldc.i4.2
    IL_000a:  ldc.i4.0
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0029:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0042:  dup
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::'numbers@7-5'
    IL_0048:  stloc.0
    .line 10,14 : 1,20 ''
    IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004e:  stloc.s    V_8
    IL_0050:  ldloc.s    V_8
    IL_0052:  ldnull
    IL_0053:  ldc.i4.0
    IL_0054:  ldc.i4.0
    IL_0055:  newobj     instance void Linq101Partitioning01/first3Numbers@12::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     int32)
    IL_005a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_005f:  ldc.i4.3
    IL_0060:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Take<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                              int32)
    IL_0065:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_006a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_006f:  dup
    IL_0070:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::first3Numbers@10
    IL_0075:  stloc.1
    .line 17,17 : 1,34 ''
    IL_0076:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_007b:  dup
    IL_007c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::'customers@17-2'
    IL_0081:  stloc.2
    .line 18,24 : 1,21 ''
    IL_0082:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0087:  stloc.s    V_9
    IL_0089:  ldloc.s    V_9
    IL_008b:  ldloc.s    V_9
    IL_008d:  ldloc.s    V_9
    IL_008f:  ldloc.s    V_9
    IL_0091:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Partitioning01::get_customers()
    IL_0096:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_009b:  ldloc.s    V_9
    IL_009d:  newobj     instance void Linq101Partitioning01/WAOrders@20::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00a2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00a7:  newobj     instance void Linq101Partitioning01/'WAOrders@22-2'::.ctor()
    IL_00ac:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00b1:  newobj     instance void Linq101Partitioning01/'WAOrders@23-3'::.ctor()
    IL_00b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00bb:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00c0:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00c5:  dup
    IL_00c6:  stsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders@18
    IL_00cb:  stloc.3
    .line 27,31 : 1,20 ''
    IL_00cc:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00d1:  stloc.s    V_10
    IL_00d3:  ldloc.s    V_10
    IL_00d5:  ldnull
    IL_00d6:  ldc.i4.0
    IL_00d7:  ldc.i4.0
    IL_00d8:  newobj     instance void Linq101Partitioning01/allButFirst4Numbers@29::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                           int32,
                                                                                           int32)
    IL_00dd:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00e2:  ldc.i4.4
    IL_00e3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Skip<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                              int32)
    IL_00e8:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00ed:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00f2:  dup
    IL_00f3:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst4Numbers@27
    IL_00f8:  stloc.s    allButFirst4Numbers
    .line 34,40 : 1,34 ''
    IL_00fa:  ldc.i4.2
    IL_00fb:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0100:  stloc.s    V_11
    IL_0102:  ldloc.s    V_11
    IL_0104:  ldloc.s    V_11
    IL_0106:  ldloc.s    V_11
    IL_0108:  ldloc.s    V_11
    IL_010a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Partitioning01::get_customers()
    IL_010f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0114:  ldloc.s    V_11
    IL_0116:  newobj     instance void Linq101Partitioning01/WAOrders2@36::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_011b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0120:  newobj     instance void Linq101Partitioning01/'WAOrders2@38-2'::.ctor()
    IL_0125:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_012a:  newobj     instance void Linq101Partitioning01/'WAOrders2@39-3'::.ctor()
    IL_012f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0134:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0139:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Skip<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(int32,
                                                                                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_013e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0143:  dup
    IL_0144:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders2@34
    IL_0149:  stloc.s    WAOrders2
    .line 43,47 : 1,20 ''
    IL_014b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0150:  stloc.s    V_12
    IL_0152:  ldloc.s    V_12
    IL_0154:  ldnull
    IL_0155:  ldc.i4.0
    IL_0156:  ldc.i4.0
    IL_0157:  newobj     instance void Linq101Partitioning01/firstNumbersLessThan6@45::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                             int32,
                                                                                             int32)
    IL_015c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0161:  newobj     instance void Linq101Partitioning01/'firstNumbersLessThan6@46-1'::.ctor()
    IL_0166:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::TakeWhile<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_016b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0170:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0175:  dup
    IL_0176:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::firstNumbersLessThan6@43
    IL_017b:  stloc.s    firstNumbersLessThan6
    .line 50,54 : 1,20 ''
    IL_017d:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0182:  stloc.s    V_13
    IL_0184:  ldloc.s    V_13
    IL_0186:  ldnull
    IL_0187:  ldc.i4.0
    IL_0188:  ldc.i4.0
    IL_0189:  newobj     instance void Linq101Partitioning01/allButFirst3Numbers@52::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                           int32,
                                                                                           int32)
    IL_018e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0193:  newobj     instance void Linq101Partitioning01/'allButFirst3Numbers@53-1'::.ctor()
    IL_0198:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SkipWhile<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_019d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01a2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01a7:  dup
    IL_01a8:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst3Numbers@50
    IL_01ad:  stloc.s    allButFirst3Numbers
    IL_01af:  ret
  } // end of method $Linq101Partitioning01::main@

} // end of class '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
