
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 6:0:0:0
}
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
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
  // Offset: 0x00000000 Length: 0x000003D2
}
.mresource public FSharpOptimizationData.Linq101Partitioning01
{
  // Offset: 0x000003D8 Length: 0x00000138
}
.module Linq101Partitioning01.exe
// MVID: {61E07032-B280-A6A2-A745-03833270E061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05270000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Partitioning01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@12'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 11@12'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Partitioning01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 12,12 : 9,12 ''
      IL_0025:  nop
      .line 12,12 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 13,13 : 9,15 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #1 input at line 11@12'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #1 input at line 11@12'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #1 input at line 11@12'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #1 input at line 11@12'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #1 input at line 11@12'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@12'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Partitioning01/'Pipe #1 input at line 11@12'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                    int32,
                                                                                                    int32)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 11@12'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 11@12'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 19@21-1'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@21-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Partitioning01/'Pipe #2 input at line 19@21-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #2 input at line 19@21-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 22,22 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@21-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Partitioning01/'Pipe #2 input at line 19@21-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #2 input at line 19@21-1'::Invoke

  } // end of class 'Pipe #2 input at line 19@21-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 19@20'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@20'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 19@20'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 21,21 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@20'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@20'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #2 input at line 19@20'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Partitioning01/'Pipe #2 input at line 19@21-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                      class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #2 input at line 19@20'::Invoke

  } // end of class 'Pipe #2 input at line 19@20'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 19@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #2 input at line 19@22-2' @_instance
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
    } // end of method 'Pipe #2 input at line 19@22-2'::.ctor

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
      IL_0019:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_001e:  ret
    } // end of method 'Pipe #2 input at line 19@22-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #2 input at line 19@22-2'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #2 input at line 19@22-2' Linq101Partitioning01/'Pipe #2 input at line 19@22-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 19@22-2'::.cctor

  } // end of class 'Pipe #2 input at line 19@22-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 19@23-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #2 input at line 19@23-3' @_instance
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
    } // end of method 'Pipe #2 input at line 19@23-3'::.ctor

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
    } // end of method 'Pipe #2 input at line 19@23-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #2 input at line 19@23-3'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #2 input at line 19@23-3' Linq101Partitioning01/'Pipe #2 input at line 19@23-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 19@23-3'::.cctor

  } // end of class 'Pipe #2 input at line 19@23-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #3 input at line 28@29'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #3 input at line 28@29'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 29,29 : 9,12 ''
      IL_0025:  nop
      .line 29,29 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 30,30 : 9,15 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #3 input at line 28@29'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #3 input at line 28@29'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #3 input at line 28@29'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #3 input at line 28@29'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #3 input at line 28@29'::current
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 28@29'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Partitioning01/'Pipe #3 input at line 28@29'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                    int32,
                                                                                                    int32)
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 28@29'::GetFreshEnumerator

  } // end of class 'Pipe #3 input at line 28@29'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 35@37-1'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@37-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Partitioning01/'Pipe #4 input at line 35@37-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #4 input at line 35@37-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 38,38 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@37-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Partitioning01/'Pipe #4 input at line 35@37-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #4 input at line 35@37-1'::Invoke

  } // end of class 'Pipe #4 input at line 35@37-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 35@36'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@36'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #4 input at line 35@36'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 37,37 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@36'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@36'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Partitioning01/'Pipe #4 input at line 35@36'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Partitioning01/'Pipe #4 input at line 35@37-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                      class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #4 input at line 35@36'::Invoke

  } // end of class 'Pipe #4 input at line 35@36'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 35@38-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #4 input at line 35@38-2' @_instance
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
    } // end of method 'Pipe #4 input at line 35@38-2'::.ctor

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
      IL_0019:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_001e:  ret
    } // end of method 'Pipe #4 input at line 35@38-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #4 input at line 35@38-2'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #4 input at line 35@38-2' Linq101Partitioning01/'Pipe #4 input at line 35@38-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 35@38-2'::.cctor

  } // end of class 'Pipe #4 input at line 35@38-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 35@39-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #4 input at line 35@39-3' @_instance
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
    } // end of method 'Pipe #4 input at line 35@39-3'::.ctor

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
    } // end of method 'Pipe #4 input at line 35@39-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #4 input at line 35@39-3'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #4 input at line 35@39-3' Linq101Partitioning01/'Pipe #4 input at line 35@39-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 35@39-3'::.cctor

  } // end of class 'Pipe #4 input at line 35@39-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #5 input at line 44@45'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #5 input at line 44@45'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 45,45 : 9,12 ''
      IL_0025:  nop
      .line 45,45 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 46,46 : 9,26 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #5 input at line 44@45'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #5 input at line 44@45'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #5 input at line 44@45'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #5 input at line 44@45'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #5 input at line 44@45'::current
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 44@45'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Partitioning01/'Pipe #5 input at line 44@45'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                    int32,
                                                                                                    int32)
      IL_0008:  ret
    } // end of method 'Pipe #5 input at line 44@45'::GetFreshEnumerator

  } // end of class 'Pipe #5 input at line 44@45'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 44@46-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #5 input at line 44@46-1' @_instance
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
    } // end of method 'Pipe #5 input at line 44@46-1'::.ctor

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
    } // end of method 'Pipe #5 input at line 44@46-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #5 input at line 44@46-1'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #5 input at line 44@46-1' Linq101Partitioning01/'Pipe #5 input at line 44@46-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 44@46-1'::.cctor

  } // end of class 'Pipe #5 input at line 44@46-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #6 input at line 51@52'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #6 input at line 51@52'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 52,52 : 9,12 ''
      IL_0025:  nop
      .line 52,52 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Partitioning01::get_numbers()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 53,53 : 9,31 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #6 input at line 51@52'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Partitioning01/'Pipe #6 input at line 51@52'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #6 input at line 51@52'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #6 input at line 51@52'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Partitioning01/'Pipe #6 input at line 51@52'::current
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 51@52'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Partitioning01/'Pipe #6 input at line 51@52'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                    int32,
                                                                                                    int32)
      IL_0008:  ret
    } // end of method 'Pipe #6 input at line 51@52'::GetFreshEnumerator

  } // end of class 'Pipe #6 input at line 51@52'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 51@53-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Linq101Partitioning01/'Pipe #6 input at line 51@53-1' @_instance
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
    } // end of method 'Pipe #6 input at line 51@53-1'::.ctor

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
    } // end of method 'Pipe #6 input at line 51@53-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Partitioning01/'Pipe #6 input at line 51@53-1'::.ctor()
      IL_0005:  stsfld     class Linq101Partitioning01/'Pipe #6 input at line 51@53-1' Linq101Partitioning01/'Pipe #6 input at line 51@53-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 51@53-1'::.cctor

  } // end of class 'Pipe #6 input at line 51@53-1'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::numbers@7
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::customers@17
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
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> first3Numbers@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@17
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
    // Code size       466 (0x1d2)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> first3Numbers,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers,
             [3] class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] WAOrders,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst4Numbers,
             [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> WAOrders2,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> firstNumbersLessThan6,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> allButFirst3Numbers,
             [8] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 'Pipe #1 input at line 11',
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> 'Pipe #2 input at line 19',
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 'Pipe #3 input at line 28',
             [13] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_13,
             [14] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> 'Pipe #4 input at line 35',
             [15] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_15,
             [16] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> 'Pipe #4 stage #1 at line 40',
             [17] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 'Pipe #5 input at line 44',
             [18] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_18,
             [19] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 'Pipe #6 input at line 51',
             [20] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_20)
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
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::numbers@7
    IL_0048:  stloc.0
    .line 10,14 : 1,20 ''
    IL_0049:  nop
    .line 11,11 : 5,10 ''
    IL_004a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004f:  stloc.s    V_9
    .line 11,11 : 5,10 ''
    IL_0051:  ldloc.s    V_9
    IL_0053:  ldnull
    IL_0054:  ldc.i4.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Linq101Partitioning01/'Pipe #1 input at line 11@12'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
    IL_005b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0060:  ldc.i4.3
    IL_0061:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Take<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                              int32)
    IL_0066:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_006b:  stloc.s    'Pipe #1 input at line 11'
    .line 14,14 : 10,20 ''
    IL_006d:  ldloc.s    'Pipe #1 input at line 11'
    IL_006f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0074:  dup
    IL_0075:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::first3Numbers@10
    IL_007a:  stloc.1
    .line 17,17 : 1,34 ''
    IL_007b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0080:  dup
    IL_0081:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::customers@17
    IL_0086:  stloc.2
    .line 18,24 : 1,21 ''
    IL_0087:  nop
    .line 19,19 : 5,10 ''
    IL_0088:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_008d:  stloc.s    V_11
    .line 19,19 : 5,10 ''
    IL_008f:  ldloc.s    V_11
    IL_0091:  ldloc.s    V_11
    .line 20,20 : 9,12 ''
    IL_0093:  ldloc.s    V_11
    IL_0095:  ldloc.s    V_11
    IL_0097:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Partitioning01::get_customers()
    IL_009c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00a1:  ldloc.s    V_11
    IL_00a3:  newobj     instance void Linq101Partitioning01/'Pipe #2 input at line 19@20'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00a8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00ad:  ldsfld     class Linq101Partitioning01/'Pipe #2 input at line 19@22-2' Linq101Partitioning01/'Pipe #2 input at line 19@22-2'::@_instance
    IL_00b2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00b7:  ldsfld     class Linq101Partitioning01/'Pipe #2 input at line 19@23-3' Linq101Partitioning01/'Pipe #2 input at line 19@23-3'::@_instance
    IL_00bc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00c1:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00c6:  stloc.s    'Pipe #2 input at line 19'
    .line 24,24 : 10,21 ''
    IL_00c8:  ldloc.s    'Pipe #2 input at line 19'
    IL_00ca:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00cf:  dup
    IL_00d0:  stsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders@18
    IL_00d5:  stloc.3
    .line 27,31 : 1,20 ''
    IL_00d6:  nop
    .line 28,28 : 5,10 ''
    IL_00d7:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00dc:  stloc.s    V_13
    .line 28,28 : 5,10 ''
    IL_00de:  ldloc.s    V_13
    IL_00e0:  ldnull
    IL_00e1:  ldc.i4.0
    IL_00e2:  ldc.i4.0
    IL_00e3:  newobj     instance void Linq101Partitioning01/'Pipe #3 input at line 28@29'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
    IL_00e8:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00ed:  ldc.i4.4
    IL_00ee:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Skip<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                              int32)
    IL_00f3:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00f8:  stloc.s    'Pipe #3 input at line 28'
    .line 31,31 : 10,20 ''
    IL_00fa:  ldloc.s    'Pipe #3 input at line 28'
    IL_00fc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0101:  dup
    IL_0102:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst4Numbers@27
    IL_0107:  stloc.s    allButFirst4Numbers
    .line 34,40 : 1,34 ''
    IL_0109:  nop
    .line 35,35 : 5,10 ''
    IL_010a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_010f:  stloc.s    V_15
    .line 35,35 : 5,10 ''
    IL_0111:  ldloc.s    V_15
    IL_0113:  ldloc.s    V_15
    .line 36,36 : 9,12 ''
    IL_0115:  ldloc.s    V_15
    IL_0117:  ldloc.s    V_15
    IL_0119:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Partitioning01::get_customers()
    IL_011e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0123:  ldloc.s    V_15
    IL_0125:  newobj     instance void Linq101Partitioning01/'Pipe #4 input at line 35@36'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_012a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_012f:  ldsfld     class Linq101Partitioning01/'Pipe #4 input at line 35@38-2' Linq101Partitioning01/'Pipe #4 input at line 35@38-2'::@_instance
    IL_0134:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0139:  ldsfld     class Linq101Partitioning01/'Pipe #4 input at line 35@39-3' Linq101Partitioning01/'Pipe #4 input at line 35@39-3'::@_instance
    IL_013e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0143:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0148:  stloc.s    'Pipe #4 input at line 35'
    .line 40,40 : 10,20 ''
    IL_014a:  ldc.i4.2
    IL_014b:  ldloc.s    'Pipe #4 input at line 35'
    IL_014d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Skip<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(int32,
                                                                                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0152:  stloc.s    'Pipe #4 stage #1 at line 40'
    .line 40,40 : 24,34 ''
    IL_0154:  ldloc.s    'Pipe #4 stage #1 at line 40'
    IL_0156:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_015b:  dup
    IL_015c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::WAOrders2@34
    IL_0161:  stloc.s    WAOrders2
    .line 43,47 : 1,20 ''
    IL_0163:  nop
    .line 44,44 : 5,10 ''
    IL_0164:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0169:  stloc.s    V_18
    .line 44,44 : 5,10 ''
    IL_016b:  ldloc.s    V_18
    IL_016d:  ldnull
    IL_016e:  ldc.i4.0
    IL_016f:  ldc.i4.0
    IL_0170:  newobj     instance void Linq101Partitioning01/'Pipe #5 input at line 44@45'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
    IL_0175:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_017a:  ldsfld     class Linq101Partitioning01/'Pipe #5 input at line 44@46-1' Linq101Partitioning01/'Pipe #5 input at line 44@46-1'::@_instance
    IL_017f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::TakeWhile<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0184:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0189:  stloc.s    'Pipe #5 input at line 44'
    .line 47,47 : 10,20 ''
    IL_018b:  ldloc.s    'Pipe #5 input at line 44'
    IL_018d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0192:  dup
    IL_0193:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::firstNumbersLessThan6@43
    IL_0198:  stloc.s    firstNumbersLessThan6
    .line 50,54 : 1,20 ''
    IL_019a:  nop
    .line 51,51 : 5,10 ''
    IL_019b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01a0:  stloc.s    V_20
    .line 51,51 : 5,10 ''
    IL_01a2:  ldloc.s    V_20
    IL_01a4:  ldnull
    IL_01a5:  ldc.i4.0
    IL_01a6:  ldc.i4.0
    IL_01a7:  newobj     instance void Linq101Partitioning01/'Pipe #6 input at line 51@52'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
    IL_01ac:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_01b1:  ldsfld     class Linq101Partitioning01/'Pipe #6 input at line 51@53-1' Linq101Partitioning01/'Pipe #6 input at line 51@53-1'::@_instance
    IL_01b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SkipWhile<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_01bb:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01c0:  stloc.s    'Pipe #6 input at line 51'
    .line 54,54 : 10,20 ''
    IL_01c2:  ldloc.s    'Pipe #6 input at line 51'
    IL_01c4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01c9:  dup
    IL_01ca:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01::allButFirst3Numbers@50
    IL_01cf:  stloc.s    allButFirst3Numbers
    IL_01d1:  ret
  } // end of method $Linq101Partitioning01::main@

} // end of class '<StartupCode$Linq101Partitioning01>'.$Linq101Partitioning01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
