
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
.assembly Linq101Select01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Select01
{
  // Offset: 0x00000000 Length: 0x00000643
}
.mresource public FSharpOptimizationData.Linq101Select01
{
  // Offset: 0x00000648 Length: 0x00000204
}
.module Linq101Select01.exe
// MVID: {61FD32BA-6057-8F80-A745-0383BA32FD61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05A30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Select01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 11@12-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #1 input at line 11@12-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Select01.fs'
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 13,13 : 9,23 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@12-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #1 input at line 11@12-1' Linq101Select01/'Pipe #1 input at line 11@12-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::.cctor

  } // end of class 'Pipe #1 input at line 11@12-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@13'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 11@13'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       158 (0x9e)
      .maxstack  7
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_0074

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0071

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 13,13 : 9,23 ''
      IL_0025:  ldarg.0
      .line 12,12 : 9,12 ''
      IL_0026:  ldsfld     class Linq101Select01/'Pipe #1 input at line 11@12-1' Linq101Select01/'Pipe #1 input at line 11@12-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0030:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0074

      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stloc.0
      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.2
      IL_0061:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0066:  ldarg.0
      .line 13,13 : 17,22 ''
      IL_0067:  ldloc.0
      IL_0068:  ldc.i4.1
      IL_0069:  add
      IL_006a:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_006f:  ldc.i4.1
      IL_0070:  ret

      .line 100001,100001 : 0,0 ''
      IL_0071:  nop
      IL_0072:  br.s       IL_0046

      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.3
      IL_0076:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_007b:  ldarg.0
      IL_007c:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_0081:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0086:  nop
      IL_0087:  ldarg.0
      IL_0088:  ldnull
      IL_0089:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_008e:  ldarg.0
      IL_008f:  ldc.i4.3
      IL_0090:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.0
      IL_0097:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_009c:  ldc.i4.0
      IL_009d:  ret
    } // end of method 'Pipe #1 input at line 11@13'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
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
    } // end of method 'Pipe #1 input at line 11@13'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
    } // end of method 'Pipe #1 input at line 11@13'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@13'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@13'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              int32)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 11@13'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 11@13'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productNames@21-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101Select01/'productNames@21-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } // end of method 'productNames@21-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 22,22 : 9,31 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } // end of method 'productNames@21-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'productNames@21-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'productNames@21-1' Linq101Select01/'productNames@21-1'::@_instance
      IL_000a:  ret
    } // end of method 'productNames@21-1'::.cctor

  } // end of class 'productNames@21-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname productNames@22
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Select01/productNames@22::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method productNames@22::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       161 (0xa1)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_0077

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0074

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0098

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 22,22 : 9,31 ''
      IL_0025:  ldarg.0
      .line 21,21 : 9,12 ''
      IL_0026:  ldsfld     class Linq101Select01/'productNames@21-1' Linq101Select01/'productNames@21-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
      IL_0030:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0077

      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005e:  stloc.0
      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.2
      IL_0061:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_0066:  ldarg.0
      .line 22,22 : 17,30 ''
      IL_0067:  ldloc.0
      IL_0068:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_006d:  stfld      string Linq101Select01/productNames@22::current
      IL_0072:  ldc.i4.1
      IL_0073:  ret

      .line 100001,100001 : 0,0 ''
      IL_0074:  nop
      IL_0075:  br.s       IL_0046

      IL_0077:  ldarg.0
      IL_0078:  ldc.i4.3
      IL_0079:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_007e:  ldarg.0
      IL_007f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0084:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_0089:  nop
      IL_008a:  ldarg.0
      IL_008b:  ldnull
      IL_008c:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.3
      IL_0093:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_0098:  ldarg.0
      IL_0099:  ldnull
      IL_009a:  stfld      string Linq101Select01/productNames@22::current
      IL_009f:  ldc.i4.0
      IL_00a0:  ret
    } // end of method productNames@22::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/productNames@22::pc
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
        IL_0044:  stfld      int32 Linq101Select01/productNames@22::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/productNames@22::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Select01/productNames@22::current
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
    } // end of method productNames@22::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
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
    } // end of method productNames@22::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Select01/productNames@22::current
      IL_0006:  ret
    } // end of method productNames@22::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/productNames@22::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                string)
      IL_0008:  ret
    } // end of method productNames@22::GetFreshEnumerator

  } // end of class productNames@22

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 28@29-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #2 input at line 28@29-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 30,30 : 9,29 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@29-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #2 input at line 28@29-1' Linq101Select01/'Pipe #2 input at line 28@29-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::.cctor

  } // end of class 'Pipe #2 input at line 28@29-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 28@30'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
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
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #2 input at line 28@30'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       166 (0xa6)
      .maxstack  7
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_007c

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0079

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_009d

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 30,30 : 9,29 ''
      IL_0025:  ldarg.0
      .line 29,29 : 9,12 ''
      IL_0026:  ldsfld     class Linq101Select01/'Pipe #2 input at line 28@29-1' Linq101Select01/'Pipe #2 input at line 28@29-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0030:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_007c

      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stloc.0
      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.2
      IL_0061:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0066:  ldarg.0
      .line 30,30 : 17,28 ''
      IL_0067:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
      IL_006c:  ldloc.0
      IL_006d:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_0072:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_0077:  ldc.i4.1
      IL_0078:  ret

      .line 100001,100001 : 0,0 ''
      IL_0079:  nop
      IL_007a:  br.s       IL_0046

      IL_007c:  ldarg.0
      IL_007d:  ldc.i4.3
      IL_007e:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0083:  ldarg.0
      IL_0084:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0089:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_008e:  nop
      IL_008f:  ldarg.0
      IL_0090:  ldnull
      IL_0091:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0096:  ldarg.0
      IL_0097:  ldc.i4.3
      IL_0098:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_009d:  ldarg.0
      IL_009e:  ldnull
      IL_009f:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_00a4:  ldc.i4.0
      IL_00a5:  ret
    } // end of method 'Pipe #2 input at line 28@30'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
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
    } // end of method 'Pipe #2 input at line 28@30'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
    } // end of method 'Pipe #2 input at line 28@30'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 28@30'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@30'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              string)
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 28@30'::GetFreshEnumerator

  } // end of class 'Pipe #2 input at line 28@30'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 37@38-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #3 input at line 37@38-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
            Invoke(string _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] string w)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 39,39 : 8,41 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<string>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@38-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #3 input at line 37@38-1' Linq101Select01/'Pipe #3 input at line 37@38-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::.cctor

  } // end of class 'Pipe #3 input at line 37@38-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #3 input at line 37@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<string,string>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<string,string> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`2<string,string> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<string,string>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #3 input at line 37@39'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,string>>& next) cil managed
    {
      // Code size       175 (0xaf)
      .maxstack  7
      .locals init ([0] string w)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_0085

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0082

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_00a6

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      .line 39,39 : 8,41 ''
      IL_0028:  ldarg.0
      .line 38,38 : 8,11 ''
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #3 input at line 37@38-1' Linq101Select01/'Pipe #3 input at line 37@38-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_words()
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0085

      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_005c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0061:  stloc.0
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.2
      IL_0064:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_0069:  ldarg.0
      .line 39,39 : 16,40 ''
      IL_006a:  ldloc.0
      IL_006b:  callvirt   instance string [mscorlib]System.String::ToUpper()
      IL_0070:  ldloc.0
      IL_0071:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0076:  newobj     instance void class [mscorlib]System.Tuple`2<string,string>::.ctor(!0,
                                                                                              !1)
      IL_007b:  stfld      class [mscorlib]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0080:  ldc.i4.1
      IL_0081:  ret

      .line 100001,100001 : 0,0 ''
      IL_0082:  nop
      IL_0083:  br.s       IL_0049

      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0092:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_0097:  nop
      IL_0098:  ldarg.0
      IL_0099:  ldnull
      IL_009a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_009f:  ldarg.0
      IL_00a0:  ldc.i4.3
      IL_00a1:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_00a6:  ldarg.0
      IL_00a7:  ldnull
      IL_00a8:  stfld      class [mscorlib]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_00ad:  ldc.i4.0
      IL_00ae:  ret
    } // end of method 'Pipe #3 input at line 37@39'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [mscorlib]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
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
    } // end of method 'Pipe #3 input at line 37@39'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
    } // end of method 'Pipe #3 input at line 37@39'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,string> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 37@39'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,string>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@39'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              class [mscorlib]System.Tuple`2<string,string>)
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 37@39'::GetFreshEnumerator

  } // end of class 'Pipe #3 input at line 37@39'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 44@45-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #4 input at line 44@45-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 46,46 : 9,42 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@45-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #4 input at line 44@45-1' Linq101Select01/'Pipe #4 input at line 44@45-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::.cctor

  } // end of class 'Pipe #4 input at line 44@45-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #4 input at line 44@46'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<string,bool>>
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
    .field public class [mscorlib]System.Tuple`2<string,bool> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`2<string,bool> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<string,bool>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #4 input at line 44@46'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,bool>>& next) cil managed
    {
      // Code size       180 (0xb4)
      .maxstack  8
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_008a

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_00ab

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      .line 46,46 : 9,42 ''
      IL_0028:  ldarg.0
      .line 45,45 : 9,12 ''
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #4 input at line 44@45-1' Linq101Select01/'Pipe #4 input at line 44@45-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_008a

      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_005c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0061:  stloc.0
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.2
      IL_0064:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0069:  ldarg.0
      .line 46,46 : 17,41 ''
      IL_006a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
      IL_006f:  ldloc.0
      IL_0070:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_0075:  ldloc.0
      IL_0076:  ldc.i4.2
      IL_0077:  rem
      IL_0078:  ldc.i4.0
      IL_0079:  ceq
      IL_007b:  newobj     instance void class [mscorlib]System.Tuple`2<string,bool>::.ctor(!0,
                                                                                            !1)
      IL_0080:  stfld      class [mscorlib]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_0085:  ldc.i4.1
      IL_0086:  ret

      .line 100001,100001 : 0,0 ''
      IL_0087:  nop
      IL_0088:  br.s       IL_0049

      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0091:  ldarg.0
      IL_0092:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0097:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_009c:  nop
      IL_009d:  ldarg.0
      IL_009e:  ldnull
      IL_009f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_00a4:  ldarg.0
      IL_00a5:  ldc.i4.3
      IL_00a6:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [mscorlib]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_00b2:  ldc.i4.0
      IL_00b3:  ret
    } // end of method 'Pipe #4 input at line 44@46'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [mscorlib]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
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
    } // end of method 'Pipe #4 input at line 44@46'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
    } // end of method 'Pipe #4 input at line 44@46'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,bool> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 44@46'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<string,bool>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@46'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              class [mscorlib]System.Tuple`2<string,bool>)
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 44@46'::GetFreshEnumerator

  } // end of class 'Pipe #4 input at line 44@46'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 51@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #5 input at line 51@52-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 51@52-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 53,53 : 9,56 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 51@52-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@52-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #5 input at line 51@52-1' Linq101Select01/'Pipe #5 input at line 51@52-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 51@52-1'::.cctor

  } // end of class 'Pipe #5 input at line 51@52-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #5 input at line 51@53'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #5 input at line 51@53'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>>& next) cil managed
    {
      // Code size       181 (0xb5)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_008b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0088

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_00ac

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      .line 53,53 : 9,56 ''
      IL_0028:  ldarg.0
      .line 52,52 : 9,12 ''
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #5 input at line 51@52-1' Linq101Select01/'Pipe #5 input at line 51@52-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_008b

      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_005c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0061:  stloc.0
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.2
      IL_0064:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0069:  ldarg.0
      .line 53,53 : 17,55 ''
      IL_006a:  ldloc.0
      IL_006b:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0070:  ldloc.0
      IL_0071:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0076:  ldloc.0
      IL_0077:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_007c:  newobj     instance void class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                 !1,
                                                                                                                                 !2)
      IL_0081:  stfld      class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_0086:  ldc.i4.1
      IL_0087:  ret

      .line 100001,100001 : 0,0 ''
      IL_0088:  nop
      IL_0089:  br.s       IL_0049

      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.3
      IL_008d:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0092:  ldarg.0
      IL_0093:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0098:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_009d:  nop
      IL_009e:  ldarg.0
      IL_009f:  ldnull
      IL_00a0:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_00a5:  ldarg.0
      IL_00a6:  ldc.i4.3
      IL_00a7:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_00ac:  ldarg.0
      IL_00ad:  ldnull
      IL_00ae:  stfld      class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_00b3:  ldc.i4.0
      IL_00b4:  ret
    } // end of method 'Pipe #5 input at line 51@53'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
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
    } // end of method 'Pipe #5 input at line 51@53'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
    } // end of method 'Pipe #5 input at line 51@53'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 51@53'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@53'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                              int32,
                                                                                              class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>)
      IL_0008:  ret
    } // end of method 'Pipe #5 input at line 51@53'::GetFreshEnumerator

  } // end of class 'Pipe #5 input at line 51@53'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@60'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #6 input at line 59@60'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 59@60'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 61,61 : 9,22 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #6 input at line 59@60'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #6 input at line 59@60'::Invoke

  } // end of class 'Pipe #6 input at line 59@60'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@61-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #6 input at line 59@61-1' @_instance
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
    } // end of method 'Pipe #6 input at line 59@61-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .line 61,61 : 16,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  clt
      IL_0004:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@61-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #6 input at line 59@61-1' Linq101Select01/'Pipe #6 input at line 59@61-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::.cctor

  } // end of class 'Pipe #6 input at line 59@61-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@62-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #6 input at line 59@62-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::.ctor

    .method public strict virtual instance string 
            Invoke(int32 n) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      .line 62,62 : 16,26 ''
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_digits()
      IL_0005:  ldarg.1
      IL_0006:  tail.
      IL_0008:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@62-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #6 input at line 59@62-2' Linq101Select01/'Pipe #6 input at line 59@62-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::.cctor

  } // end of class 'Pipe #6 input at line 59@62-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@73-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 int32 a) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@73-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 Linq101Select01/'Pipe #7 input at line 71@73-1'::a
      IL_0014:  ret
    } // end of method 'Pipe #7 input at line 71@73-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,object> 
            Invoke(int32 _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] int32 b)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 74,74 : 9,22 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@73-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32 Linq101Select01/'Pipe #7 input at line 71@73-1'::a
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<int32,int32>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #7 input at line 71@73-1'::Invoke

  } // end of class 'Pipe #7 input at line 71@73-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@72'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #7 input at line 71@72'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      .locals init ([0] int32 a)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 73,73 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersB()
      IL_0013:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_001e:  ldloc.0
      IL_001f:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@73-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                int32)
      IL_0024:  tail.
      IL_0026:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002b:  ret
    } // end of method 'Pipe #7 input at line 71@72'::Invoke

  } // end of class 'Pipe #7 input at line 71@72'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@74-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<int32,int32>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #7 input at line 71@74-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<int32,int32>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  6
      .locals init ([0] int32 a,
               [1] int32 b)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      .line 74,74 : 16,21 ''
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  clt
      IL_0012:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@74-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #7 input at line 71@74-2' Linq101Select01/'Pipe #7 input at line 71@74-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::.cctor

  } // end of class 'Pipe #7 input at line 71@74-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@75-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Tuple`2<int32,int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #7 input at line 71@75-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Tuple`2<int32,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32> 
            Invoke(class [mscorlib]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  6
      .locals init ([0] int32 a,
               [1] int32 b)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      .line 75,75 : 17,20 ''
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0015:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@75-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #7 input at line 71@75-3' Linq101Select01/'Pipe #7 input at line 71@75-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::.cctor

  } // end of class 'Pipe #7 input at line 71@75-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@83-1'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@83-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #8 input at line 81@83-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #8 input at line 81@83-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 84,84 : 9,34 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@83-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #8 input at line 81@83-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #8 input at line 81@83-1'::Invoke

  } // end of class 'Pipe #8 input at line 81@83-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@82'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #8 input at line 81@82'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 83,83 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@83-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #8 input at line 81@82'::Invoke

  } // end of class 'Pipe #8 input at line 81@82'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@84-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #8 input at line 81@84-2' @_instance
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
    } // end of method 'Pipe #8 input at line 81@84-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  10
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 84,84 : 16,33 ''
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0xc350
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.2
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_LessThan(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } // end of method 'Pipe #8 input at line 81@84-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@84-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #8 input at line 81@84-2' Linq101Select01/'Pipe #8 input at line 81@84-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #8 input at line 81@84-2'::.cctor

  } // end of class 'Pipe #8 input at line 81@84-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@85-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #8 input at line 81@85-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal> 
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
      .line 85,85 : 17,49 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                !1,
                                                                                                                                !2)
      IL_0025:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@85-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #8 input at line 81@85-3' Linq101Select01/'Pipe #8 input at line 81@85-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::.cctor

  } // end of class 'Pipe #8 input at line 81@85-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@92-1'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@92-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #9 input at line 90@92-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #9 input at line 90@92-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 93,93 : 9,51 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@92-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #9 input at line 90@92-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #9 input at line 90@92-1'::Invoke

  } // end of class 'Pipe #9 input at line 90@92-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@91'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #9 input at line 90@91'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 92,92 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@92-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #9 input at line 90@91'::Invoke

  } // end of class 'Pipe #9 input at line 90@91'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@93-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #9 input at line 90@93-2' @_instance
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
    } // end of method 'Pipe #9 input at line 90@93-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       48 (0x30)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o,
               [2] valuetype [mscorlib]System.DateTime V_2,
               [3] valuetype [mscorlib]System.DateTime V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 93,93 : 16,50 ''
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  ldc.i4     0x7ce
      IL_001a:  ldc.i4.1
      IL_001b:  ldc.i4.1
      IL_001c:  newobj     instance void [mscorlib]System.DateTime::.ctor(int32,
                                                                          int32,
                                                                          int32)
      IL_0021:  stloc.3
      IL_0022:  ldloc.2
      IL_0023:  ldloc.3
      IL_0024:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0029:  ldc.i4.0
      IL_002a:  clt
      IL_002c:  ldc.i4.0
      IL_002d:  ceq
      IL_002f:  ret
    } // end of method 'Pipe #9 input at line 90@93-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@93-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #9 input at line 90@93-2' Linq101Select01/'Pipe #9 input at line 90@93-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #9 input at line 90@93-2'::.cctor

  } // end of class 'Pipe #9 input at line 90@93-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@94-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #9 input at line 90@94-3' @_instance
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
    } // end of method 'Pipe #9 input at line 90@94-3'::.ctor

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
      .line 94,94 : 17,53 ''
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
    } // end of method 'Pipe #9 input at line 90@94-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@94-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #9 input at line 90@94-3' Linq101Select01/'Pipe #9 input at line 90@94-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #9 input at line 90@94-3'::.cctor

  } // end of class 'Pipe #9 input at line 90@94-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@101-1'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders3@101-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'orders3@101-1'::c
      IL_0014:  ret
    } // end of method 'orders3@101-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 102,102 : 9,35 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders3@101-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'orders3@101-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'orders3@101-1'::Invoke

  } // end of class 'orders3@101-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit orders3@100
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_000d:  ret
    } // end of method orders3@100::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 101,101 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'orders3@101-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method orders3@100::Invoke

  } // end of class orders3@100

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@102-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders3@102-2' @_instance
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
    } // end of method 'orders3@102-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  10
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 102,102 : 16,34 ''
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0x4e20
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.1
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_GreaterThanOrEqual(valuetype [netstandard]System.Decimal,
                                                                                   valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } // end of method 'orders3@102-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders3@102-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders3@102-2' Linq101Select01/'orders3@102-2'::@_instance
      IL_000a:  ret
    } // end of method 'orders3@102-2'::.cctor

  } // end of class 'orders3@102-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@103-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>
  {
    .field static assembly initonly class Linq101Select01/'orders3@103-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'orders3@103-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal> 
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
      .line 103,103 : 17,49 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                !1,
                                                                                                                                !2)
      IL_0025:  ret
    } // end of method 'orders3@103-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders3@103-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders3@103-3' Linq101Select01/'orders3@103-3'::@_instance
      IL_000a:  ret
    } // end of method 'orders3@103-3'::.cctor

  } // end of class 'orders3@103-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit orders4@111
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders4@111::builder@
      IL_000d:  ret
    } // end of method orders4@111::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 112,112 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders4@111::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Customer,object>(!!0)
      IL_0010:  ret
    } // end of method orders4@111::Invoke

  } // end of class orders4@111

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@112-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders4@112-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'orders4@112-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 112,112 : 16,31 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0006:  ldstr      "WA"
      IL_000b:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0010:  ret
    } // end of method 'orders4@112-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@112-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@112-1' Linq101Select01/'orders4@112-1'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@112-1'::.cctor

  } // end of class 'orders4@112-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@113-3'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@113-3'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'orders4@113-3'::c
      IL_0014:  ret
    } // end of method 'orders4@113-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg3) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] class [Utils]Utils/Order o)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 114,114 : 9,42 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@113-3'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'orders4@113-3'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'orders4@113-3'::Invoke

  } // end of class 'orders4@113-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@111-2'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_000d:  ret
    } // end of method 'orders4@111-2'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg2) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 113,113 : 9,12 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'orders4@113-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'orders4@111-2'::Invoke

  } // end of class 'orders4@111-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@114-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders4@114-4' @_instance
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
    } // end of method 'orders4@114-4'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [Utils]Utils/Order o,
               [2] valuetype [mscorlib]System.DateTime V_2,
               [3] valuetype [mscorlib]System.DateTime V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      .line 114,114 : 16,41 ''
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  call       valuetype [mscorlib]System.DateTime Linq101Select01::get_cutOffDate()
      IL_001a:  stloc.3
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0022:  ldc.i4.0
      IL_0023:  clt
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  ret
    } // end of method 'orders4@114-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@114-4'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@114-4' Linq101Select01/'orders4@114-4'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@114-4'::.cctor

  } // end of class 'orders4@114-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@115-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`2<string,int32>>
  {
    .field static assembly initonly class Linq101Select01/'orders4@115-5' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'orders4@115-5'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,int32> 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       32 (0x20)
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
      .line 115,115 : 17,40 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  newobj     instance void class [mscorlib]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001f:  ret
    } // end of method 'orders4@115-5'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@115-5'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@115-5' Linq101Select01/'orders4@115-5'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@115-5'::.cctor

  } // end of class 'orders4@115-5'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbers@7
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numsPlusOne() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numsPlusOne@10
    IL_0005:  ret
  } // end of method Linq101Select01::get_numsPlusOne

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Select01>'.$Linq101Select01::products@17
    IL_0005:  ret
  } // end of method Linq101Select01::get_products

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          get_productNames() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::productNames@19
    IL_0005:  ret
  } // end of method Linq101Select01::get_productNames

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_strings() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::strings@26
    IL_0005:  ret
  } // end of method Linq101Select01::get_strings

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_textNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::textNums@27
    IL_0005:  ret
  } // end of method Linq101Select01::get_textNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::words@34
    IL_0005:  ret
  } // end of method Linq101Select01::get_words

  .method public specialname static class [mscorlib]System.Tuple`2<string,string>[] 
          get_upperLowerWords() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::upperLowerWords@36
    IL_0005:  ret
  } // end of method Linq101Select01::get_upperLowerWords

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> 
          get_digitOddEvens() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> '<StartupCode$Linq101Select01>'.$Linq101Select01::digitOddEvens@43
    IL_0005:  ret
  } // end of method Linq101Select01::get_digitOddEvens

  .method public specialname static class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] 
          get_productInfos() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::productInfos@50
    IL_0005:  ret
  } // end of method Linq101Select01::get_productInfos

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_digits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::digits@57
    IL_0005:  ret
  } // end of method Linq101Select01::get_digits

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_lowNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::lowNums@58
    IL_0005:  ret
  } // end of method Linq101Select01::get_lowNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbersA() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersA@67
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbersA

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbersB() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersB@68
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbersB

  .method public specialname static class [mscorlib]System.Tuple`2<int32,int32>[] 
          get_pairs() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<int32,int32>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::pairs@70
    IL_0005:  ret
  } // end of method Linq101Select01::get_pairs

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Select01>'.$Linq101Select01::customers@79
    IL_0005:  ret
  } // end of method Linq101Select01::get_customers

  .method public specialname static class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] 
          get_orders() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders@80
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders

  .method public specialname static class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] 
          get_orders2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders2@89
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders2

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> 
          get_orders3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders3@98
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders3

  .method public specialname static valuetype [mscorlib]System.DateTime 
          get_cutOffDate() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.DateTime '<StartupCode$Linq101Select01>'.$Linq101Select01::cutOffDate@107
    IL_0005:  ret
  } // end of method Linq101Select01::get_cutOffDate

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> 
          get_orders4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders4@109
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders4

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
  } // end of property Linq101Select01::numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numsPlusOne()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numsPlusOne()
  } // end of property Linq101Select01::numsPlusOne
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
  } // end of property Linq101Select01::products
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
          productNames()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<string> Linq101Select01::get_productNames()
  } // end of property Linq101Select01::productNames
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          strings()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
  } // end of property Linq101Select01::strings
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          textNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_textNums()
  } // end of property Linq101Select01::textNums
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_words()
  } // end of property Linq101Select01::words
  .property class [mscorlib]System.Tuple`2<string,string>[]
          upperLowerWords()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,string>[] Linq101Select01::get_upperLowerWords()
  } // end of property Linq101Select01::upperLowerWords
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>>
          digitOddEvens()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> Linq101Select01::get_digitOddEvens()
  } // end of property Linq101Select01::digitOddEvens
  .property class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[]
          productInfos()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] Linq101Select01::get_productInfos()
  } // end of property Linq101Select01::productInfos
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_digits()
  } // end of property Linq101Select01::digits
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          lowNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_lowNums()
  } // end of property Linq101Select01::lowNums
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersA()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersA()
  } // end of property Linq101Select01::numbersA
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersB()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersB()
  } // end of property Linq101Select01::numbersB
  .property class [mscorlib]System.Tuple`2<int32,int32>[]
          pairs()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<int32,int32>[] Linq101Select01::get_pairs()
  } // end of property Linq101Select01::pairs
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
  } // end of property Linq101Select01::customers
  .property class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[]
          orders()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] Linq101Select01::get_orders()
  } // end of property Linq101Select01::orders
  .property class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[]
          orders2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] Linq101Select01::get_orders2()
  } // end of property Linq101Select01::orders2
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>
          orders3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> Linq101Select01::get_orders3()
  } // end of property Linq101Select01::orders3
  .property valuetype [mscorlib]System.DateTime
          cutOffDate()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [mscorlib]System.DateTime Linq101Select01::get_cutOffDate()
  } // end of property Linq101Select01::cutOffDate
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>>
          orders4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> Linq101Select01::get_orders4()
  } // end of property Linq101Select01::orders4
} // end of class Linq101Select01

.class private abstract auto ansi sealed '<StartupCode$Linq101Select01>'.$Linq101Select01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numsPlusOne@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@17
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<string> productNames@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings@26
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> textNums@27
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@34
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,string>[] upperLowerWords@36
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> digitOddEvens@43
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] productInfos@50
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@57
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> lowNums@58
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersA@67
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersB@68
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<int32,int32>[] pairs@70
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@79
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] orders@80
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] orders2@89
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> orders3@98
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype [mscorlib]System.DateTime cutOffDate@107
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> orders4@109
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1161 (0x489)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numsPlusOne,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [3] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> productNames,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings,
             [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> textNums,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [7] class [mscorlib]System.Tuple`2<string,string>[] upperLowerWords,
             [8] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> digitOddEvens,
             [9] class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] productInfos,
             [10] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits,
             [11] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> lowNums,
             [12] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersA,
             [13] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersB,
             [14] class [mscorlib]System.Tuple`2<int32,int32>[] pairs,
             [15] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers,
             [16] class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] orders,
             [17] class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] orders2,
             [18] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> orders3,
             [19] valuetype [mscorlib]System.DateTime cutOffDate,
             [20] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> orders4,
             [21] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 'Pipe #1 input at line 11',
             [22] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_22,
             [23] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_23,
             [24] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 'Pipe #2 input at line 28',
             [25] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_25,
             [26] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,string>> 'Pipe #3 input at line 37',
             [27] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_27,
             [28] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,bool>> 'Pipe #4 input at line 44',
             [29] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_29,
             [30] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>> 'Pipe #5 input at line 51',
             [31] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             [32] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 'Pipe #6 input at line 59',
             [33] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_33,
             [34] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_34,
             [35] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_35,
             [36] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> 'Pipe #7 input at line 71',
             [37] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_37,
             [38] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> 'Pipe #8 input at line 81',
             [39] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_39,
             [40] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>> 'Pipe #9 input at line 90',
             [41] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             [42] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_42,
             [43] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43)
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
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbers@7
    IL_0048:  stloc.0
    .line 11,11 : 5,10 ''
    IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004e:  stloc.s    V_22
    IL_0050:  ldnull
    IL_0051:  ldc.i4.0
    IL_0052:  ldc.i4.0
    IL_0053:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@13'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            int32)
    IL_0058:  stloc.s    'Pipe #1 input at line 11'
    .line 14,14 : 10,20 ''
    IL_005a:  ldloc.s    'Pipe #1 input at line 11'
    IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0061:  dup
    IL_0062:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numsPlusOne@10
    IL_0067:  stloc.1
    .line 17,17 : 1,32 ''
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_006d:  dup
    IL_006e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Select01>'.$Linq101Select01::products@17
    IL_0073:  stloc.2
    .line 20,20 : 5,10 ''
    IL_0074:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0079:  stloc.s    V_23
    IL_007b:  ldnull
    IL_007c:  ldc.i4.0
    IL_007d:  ldnull
    IL_007e:  newobj     instance void Linq101Select01/productNames@22::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                              int32,
                                                                              string)
    IL_0083:  dup
    IL_0084:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::productNames@19
    IL_0089:  stloc.3
    .line 26,26 : 1,97 ''
    IL_008a:  ldstr      "zero"
    IL_008f:  ldstr      "one"
    IL_0094:  ldstr      "two"
    IL_0099:  ldstr      "three"
    IL_009e:  ldstr      "four"
    IL_00a3:  ldstr      "five"
    IL_00a8:  ldstr      "six"
    IL_00ad:  ldstr      "seven"
    IL_00b2:  ldstr      "eight"
    IL_00b7:  ldstr      "nine"
    IL_00bc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_00c1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00c6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00da:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00df:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ee:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f3:  dup
    IL_00f4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::strings@26
    IL_00f9:  stloc.s    strings
    .line 28,28 : 5,10 ''
    IL_00fb:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0100:  stloc.s    V_25
    IL_0102:  ldnull
    IL_0103:  ldc.i4.0
    IL_0104:  ldnull
    IL_0105:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@30'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            string)
    IL_010a:  stloc.s    'Pipe #2 input at line 28'
    .line 31,31 : 10,20 ''
    IL_010c:  ldloc.s    'Pipe #2 input at line 28'
    IL_010e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0113:  dup
    IL_0114:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::textNums@27
    IL_0119:  stloc.s    textNums
    .line 34,34 : 1,46 ''
    IL_011b:  ldstr      "aPPLE"
    IL_0120:  ldstr      "BlUeBeRrY"
    IL_0125:  ldstr      "cHeRry"
    IL_012a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_012f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0134:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0139:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013e:  dup
    IL_013f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::words@34
    IL_0144:  stloc.s    words
    .line 37,37 : 4,9 ''
    IL_0146:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_014b:  stloc.s    V_27
    IL_014d:  ldnull
    IL_014e:  ldc.i4.0
    IL_014f:  ldnull
    IL_0150:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@39'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                            int32,
                                                                                            class [mscorlib]System.Tuple`2<string,string>)
    IL_0155:  stloc.s    'Pipe #3 input at line 37'
    .line 40,40 : 9,20 ''
    IL_0157:  ldloc.s    'Pipe #3 input at line 37'
    IL_0159:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_015e:  dup
    IL_015f:  stsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::upperLowerWords@36
    IL_0164:  stloc.s    upperLowerWords
    .line 44,44 : 5,10 ''
    IL_0166:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_016b:  stloc.s    V_29
    IL_016d:  ldnull
    IL_016e:  ldc.i4.0
    IL_016f:  ldnull
    IL_0170:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@46'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            class [mscorlib]System.Tuple`2<string,bool>)
    IL_0175:  stloc.s    'Pipe #4 input at line 44'
    .line 47,47 : 10,20 ''
    IL_0177:  ldloc.s    'Pipe #4 input at line 44'
    IL_0179:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [mscorlib]System.Tuple`2<string,bool>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017e:  dup
    IL_017f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<string,bool>> '<StartupCode$Linq101Select01>'.$Linq101Select01::digitOddEvens@43
    IL_0184:  stloc.s    digitOddEvens
    .line 51,51 : 5,10 ''
    IL_0186:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_018b:  stloc.s    V_31
    IL_018d:  ldnull
    IL_018e:  ldc.i4.0
    IL_018f:  ldnull
    IL_0190:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@53'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                            int32,
                                                                                            class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>)
    IL_0195:  stloc.s    'Pipe #5 input at line 51'
    .line 54,54 : 10,21 ''
    IL_0197:  ldloc.s    'Pipe #5 input at line 51'
    IL_0199:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_019e:  dup
    IL_019f:  stsfld     class [mscorlib]System.Tuple`3<string,string,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::productInfos@50
    IL_01a4:  stloc.s    productInfos
    .line 57,57 : 1,21 ''
    IL_01a6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
    IL_01ab:  dup
    IL_01ac:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::digits@57
    IL_01b1:  stloc.s    digits
    .line 59,59 : 5,10 ''
    IL_01b3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01b8:  stloc.s    V_33
    IL_01ba:  ldloc.s    V_33
    IL_01bc:  ldloc.s    V_33
    .line 60,60 : 9,12 ''
    IL_01be:  ldloc.s    V_33
    IL_01c0:  ldloc.s    V_33
    IL_01c2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
    IL_01c7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01cc:  ldloc.s    V_33
    IL_01ce:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@60'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01d3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01d8:  ldsfld     class Linq101Select01/'Pipe #6 input at line 59@61-1' Linq101Select01/'Pipe #6 input at line 59@61-1'::@_instance
    IL_01dd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_01e2:  ldsfld     class Linq101Select01/'Pipe #6 input at line 59@62-2' Linq101Select01/'Pipe #6 input at line 59@62-2'::@_instance
    IL_01e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<int32,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01ec:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01f1:  stloc.s    'Pipe #6 input at line 59'
    .line 63,63 : 10,20 ''
    IL_01f3:  ldloc.s    'Pipe #6 input at line 59'
    IL_01f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01fa:  dup
    IL_01fb:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::lowNums@58
    IL_0200:  stloc.s    lowNums
    .line 64,64 : 1,59 ''
    IL_0202:  nop
    .line 100001,100001 : 0,0 ''
    IL_0203:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_lowNums()
    IL_0208:  stloc.s    V_34
    IL_020a:  ldstr      "four"
    IL_020f:  ldstr      "one"
    IL_0214:  ldstr      "three"
    IL_0219:  ldstr      "two"
    IL_021e:  ldstr      "zero"
    IL_0223:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0228:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_022d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0232:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0237:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_023c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0241:  stloc.s    V_35
    IL_0243:  ldloc.s    V_34
    IL_0245:  ldloc.s    V_35
    IL_0247:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_024c:  callvirt   instance bool class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Equals(object,
                                                                                                                    class [mscorlib]System.Collections.IEqualityComparer)
    IL_0251:  ldc.i4.0
    IL_0252:  ceq
    IL_0254:  brfalse.s  IL_0270

    .line 64,64 : 60,84 ''
    IL_0256:  ldstr      "lowNums failed"
    IL_025b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0260:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0265:  pop
    .line 64,64 : 86,92 ''
    IL_0266:  ldc.i4.1
    IL_0267:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_026c:  pop
    .line 100001,100001 : 0,0 ''
    IL_026d:  nop
    IL_026e:  br.s       IL_0271

    .line 100001,100001 : 0,0 ''
    IL_0270:  nop
    .line 67,67 : 1,37 ''
    IL_0271:  ldc.i4.0
    IL_0272:  ldc.i4.2
    IL_0273:  ldc.i4.4
    IL_0274:  ldc.i4.5
    IL_0275:  ldc.i4.6
    IL_0276:  ldc.i4.8
    IL_0277:  ldc.i4.s   9
    IL_0279:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_027e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0283:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0288:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_028d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0292:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0297:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_029c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02a1:  dup
    IL_02a2:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersA@67
    IL_02a7:  stloc.s    numbersA
    .line 68,68 : 1,31 ''
    IL_02a9:  ldc.i4.1
    IL_02aa:  ldc.i4.3
    IL_02ab:  ldc.i4.5
    IL_02ac:  ldc.i4.7
    IL_02ad:  ldc.i4.8
    IL_02ae:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_02b3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02b8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02bd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02c2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02c7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02cc:  dup
    IL_02cd:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersB@68
    IL_02d2:  stloc.s    numbersB
    .line 71,71 : 5,10 ''
    IL_02d4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02d9:  stloc.s    V_37
    IL_02db:  ldloc.s    V_37
    IL_02dd:  ldloc.s    V_37
    .line 72,72 : 9,12 ''
    IL_02df:  ldloc.s    V_37
    IL_02e1:  ldloc.s    V_37
    IL_02e3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersA()
    IL_02e8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02ed:  ldloc.s    V_37
    IL_02ef:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@72'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02f4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02f9:  ldsfld     class Linq101Select01/'Pipe #7 input at line 71@74-2' Linq101Select01/'Pipe #7 input at line 71@74-2'::@_instance
    IL_02fe:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0303:  ldsfld     class Linq101Select01/'Pipe #7 input at line 71@75-3' Linq101Select01/'Pipe #7 input at line 71@75-3'::@_instance
    IL_0308:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_030d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0312:  stloc.s    'Pipe #7 input at line 71'
    .line 76,76 : 10,21 ''
    IL_0314:  ldloc.s    'Pipe #7 input at line 71'
    IL_0316:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<int32,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_031b:  dup
    IL_031c:  stsfld     class [mscorlib]System.Tuple`2<int32,int32>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::pairs@70
    IL_0321:  stloc.s    pairs
    .line 79,79 : 1,34 ''
    IL_0323:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0328:  dup
    IL_0329:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Select01>'.$Linq101Select01::customers@79
    IL_032e:  stloc.s    customers
    .line 81,81 : 5,10 ''
    IL_0330:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0335:  stloc.s    V_39
    IL_0337:  ldloc.s    V_39
    IL_0339:  ldloc.s    V_39
    .line 82,82 : 9,12 ''
    IL_033b:  ldloc.s    V_39
    IL_033d:  ldloc.s    V_39
    IL_033f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_0344:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0349:  ldloc.s    V_39
    IL_034b:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0350:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0355:  ldsfld     class Linq101Select01/'Pipe #8 input at line 81@84-2' Linq101Select01/'Pipe #8 input at line 81@84-2'::@_instance
    IL_035a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_035f:  ldsfld     class Linq101Select01/'Pipe #8 input at line 81@85-3' Linq101Select01/'Pipe #8 input at line 81@85-3'::@_instance
    IL_0364:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0369:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_036e:  stloc.s    'Pipe #8 input at line 81'
    .line 86,86 : 10,21 ''
    IL_0370:  ldloc.s    'Pipe #8 input at line 81'
    IL_0372:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0377:  dup
    IL_0378:  stsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders@80
    IL_037d:  stloc.s    orders
    .line 90,90 : 5,10 ''
    IL_037f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0384:  stloc.s    V_41
    IL_0386:  ldloc.s    V_41
    IL_0388:  ldloc.s    V_41
    .line 91,91 : 9,12 ''
    IL_038a:  ldloc.s    V_41
    IL_038c:  ldloc.s    V_41
    IL_038e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_0393:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0398:  ldloc.s    V_41
    IL_039a:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_039f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03a4:  ldsfld     class Linq101Select01/'Pipe #9 input at line 90@93-2' Linq101Select01/'Pipe #9 input at line 90@93-2'::@_instance
    IL_03a9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_03ae:  ldsfld     class Linq101Select01/'Pipe #9 input at line 90@94-3' Linq101Select01/'Pipe #9 input at line 90@94-3'::@_instance
    IL_03b3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03b8:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_03bd:  stloc.s    'Pipe #9 input at line 90'
    .line 95,95 : 10,21 ''
    IL_03bf:  ldloc.s    'Pipe #9 input at line 90'
    IL_03c1:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03c6:  dup
    IL_03c7:  stsfld     class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.DateTime>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders2@89
    IL_03cc:  stloc.s    orders2
    .line 99,99 : 5,10 ''
    IL_03ce:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03d3:  stloc.s    V_42
    IL_03d5:  ldloc.s    V_42
    IL_03d7:  ldloc.s    V_42
    .line 100,100 : 9,12 ''
    IL_03d9:  ldloc.s    V_42
    IL_03db:  ldloc.s    V_42
    IL_03dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_03e2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03e7:  ldloc.s    V_42
    IL_03e9:  newobj     instance void Linq101Select01/orders3@100::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03ee:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03f3:  ldsfld     class Linq101Select01/'orders3@102-2' Linq101Select01/'orders3@102-2'::@_instance
    IL_03f8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_03fd:  ldsfld     class Linq101Select01/'orders3@103-3' Linq101Select01/'orders3@103-3'::@_instance
    IL_0402:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0407:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_040c:  dup
    IL_040d:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`3<string,int32,valuetype [mscorlib]System.Decimal>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders3@98
    IL_0412:  stloc.s    orders3
    .line 107,107 : 1,38 ''
    IL_0414:  ldc.i4     0x7cd
    IL_0419:  ldc.i4.1
    IL_041a:  ldc.i4.1
    IL_041b:  newobj     instance void [mscorlib]System.DateTime::.ctor(int32,
                                                                        int32,
                                                                        int32)
    IL_0420:  dup
    IL_0421:  stsfld     valuetype [mscorlib]System.DateTime '<StartupCode$Linq101Select01>'.$Linq101Select01::cutOffDate@107
    IL_0426:  stloc.s    cutOffDate
    .line 110,110 : 5,10 ''
    IL_0428:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_042d:  stloc.s    V_43
    IL_042f:  ldloc.s    V_43
    IL_0431:  ldloc.s    V_43
    IL_0433:  ldloc.s    V_43
    IL_0435:  ldloc.s    V_43
    .line 111,111 : 9,12 ''
    IL_0437:  ldloc.s    V_43
    IL_0439:  ldloc.s    V_43
    IL_043b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_0440:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0445:  ldloc.s    V_43
    IL_0447:  newobj     instance void Linq101Select01/orders4@111::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_044c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Customer,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0451:  ldsfld     class Linq101Select01/'orders4@112-1' Linq101Select01/'orders4@112-1'::@_instance
    IL_0456:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_045b:  ldloc.s    V_43
    IL_045d:  newobj     instance void Linq101Select01/'orders4@111-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0462:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0467:  ldsfld     class Linq101Select01/'orders4@114-4' Linq101Select01/'orders4@114-4'::@_instance
    IL_046c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0471:  ldsfld     class Linq101Select01/'orders4@115-5' Linq101Select01/'orders4@115-5'::@_instance
    IL_0476:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_047b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,int32>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0480:  dup
    IL_0481:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,int32>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders4@109
    IL_0486:  stloc.s    orders4
    IL_0488:  ret
  } // end of method $Linq101Select01::main@

} // end of class '<StartupCode$Linq101Select01>'.$Linq101Select01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
