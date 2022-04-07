
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
.assembly TestFunction17
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction17
{
  // Offset: 0x00000000 Length: 0x0000066E
}
.mresource public FSharpOptimizationData.TestFunction17
{
  // Offset: 0x00000678 Length: 0x000001CD
}
.module TestFunction17.exe
// MVID: {6220E156-A624-45A8-A745-038356E12062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x051E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction17
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public R
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class TestFunction17/R>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class TestFunction17/R>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 x@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 y@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::x@
      IL_0006:  ret
    } // end of method R::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::y@
      IL_0006:  ret
    } // end of method R::get_y

    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 int32 y) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 TestFunction17/R::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction17/R::y@
      IL_0014:  ret
    } // end of method R::.ctor

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction17/R>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method R::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction17/R obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       99 (0x63)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction17.fs'
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_005c

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_005a

      .line 100001,100001 : 0,0 ''
      IL_0006:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction17/R::x@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction17/R::x@
      IL_0019:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_001a:  ldloc.2
      IL_001b:  ldloc.3
      IL_001c:  bge.s      IL_0022

      .line 100001,100001 : 0,0 ''
      IL_001e:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_001f:  nop
      IL_0020:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0022:  ldloc.2
      IL_0023:  ldloc.3
      IL_0024:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      .line 100001,100001 : 0,0 ''
      IL_0027:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.0
      IL_002a:  bge.s      IL_002e

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.0
      IL_002d:  ret

      .line 100001,100001 : 0,0 ''
      IL_002e:  ldloc.0
      IL_002f:  ldc.i4.0
      IL_0030:  ble.s      IL_0034

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldloc.0
      IL_0033:  ret

      .line 100001,100001 : 0,0 ''
      IL_0034:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0039:  stloc.s    V_4
      IL_003b:  ldarg.0
      IL_003c:  ldfld      int32 TestFunction17/R::y@
      IL_0041:  stloc.s    V_5
      IL_0043:  ldarg.1
      IL_0044:  ldfld      int32 TestFunction17/R::y@
      IL_0049:  stloc.s    V_6
      .line 100001,100001 : 0,0 ''
      IL_004b:  ldloc.s    V_5
      IL_004d:  ldloc.s    V_6
      IL_004f:  bge.s      IL_0053

      .line 100001,100001 : 0,0 ''
      IL_0051:  ldc.i4.m1
      IL_0052:  ret

      .line 100001,100001 : 0,0 ''
      IL_0053:  ldloc.s    V_5
      IL_0055:  ldloc.s    V_6
      IL_0057:  cgt
      IL_0059:  ret

      .line 100001,100001 : 0,0 ''
      IL_005a:  ldc.i4.1
      IL_005b:  ret

      .line 100001,100001 : 0,0 ''
      IL_005c:  ldarg.1
      IL_005d:  brfalse.s  IL_0061

      .line 100001,100001 : 0,0 ''
      IL_005f:  ldc.i4.m1
      IL_0060:  ret

      .line 100001,100001 : 0,0 ''
      IL_0061:  ldc.i4.0
      IL_0062:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction17/R
      IL_0007:  callvirt   instance int32 TestFunction17/R::CompareTo(class TestFunction17/R)
      IL_000c:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       116 (0x74)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0,
               [1] class TestFunction17/R V_1,
               [2] int32 V_2,
               [3] class [mscorlib]System.Collections.IComparer V_3,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] class [mscorlib]System.Collections.IComparer V_6,
               [7] int32 V_7,
               [8] int32 V_8)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction17/R
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_0009:  ldarg.0
      IL_000a:  brfalse.s  IL_0068

      .line 100001,100001 : 0,0 ''
      IL_000c:  ldarg.1
      IL_000d:  unbox.any  TestFunction17/R
      IL_0012:  brfalse.s  IL_0066

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.2
      IL_0015:  stloc.3
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 TestFunction17/R::x@
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.1
      IL_001f:  ldfld      int32 TestFunction17/R::x@
      IL_0024:  stloc.s    V_5
      .line 100001,100001 : 0,0 ''
      IL_0026:  ldloc.s    V_4
      IL_0028:  ldloc.s    V_5
      IL_002a:  bge.s      IL_0030

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      IL_002e:  br.s       IL_0037

      .line 100001,100001 : 0,0 ''
      IL_0030:  ldloc.s    V_4
      IL_0032:  ldloc.s    V_5
      IL_0034:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0036:  nop
      .line 100001,100001 : 0,0 ''
      IL_0037:  stloc.2
      .line 100001,100001 : 0,0 ''
      IL_0038:  ldloc.2
      IL_0039:  ldc.i4.0
      IL_003a:  bge.s      IL_003e

      .line 100001,100001 : 0,0 ''
      IL_003c:  ldloc.2
      IL_003d:  ret

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldloc.2
      IL_003f:  ldc.i4.0
      IL_0040:  ble.s      IL_0044

      .line 100001,100001 : 0,0 ''
      IL_0042:  ldloc.2
      IL_0043:  ret

      .line 100001,100001 : 0,0 ''
      IL_0044:  ldarg.2
      IL_0045:  stloc.s    V_6
      IL_0047:  ldarg.0
      IL_0048:  ldfld      int32 TestFunction17/R::y@
      IL_004d:  stloc.s    V_7
      IL_004f:  ldloc.1
      IL_0050:  ldfld      int32 TestFunction17/R::y@
      IL_0055:  stloc.s    V_8
      .line 100001,100001 : 0,0 ''
      IL_0057:  ldloc.s    V_7
      IL_0059:  ldloc.s    V_8
      IL_005b:  bge.s      IL_005f

      .line 100001,100001 : 0,0 ''
      IL_005d:  ldc.i4.m1
      IL_005e:  ret

      .line 100001,100001 : 0,0 ''
      IL_005f:  ldloc.s    V_7
      IL_0061:  ldloc.s    V_8
      IL_0063:  cgt
      IL_0065:  ret

      .line 100001,100001 : 0,0 ''
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  ldarg.1
      IL_0069:  unbox.any  TestFunction17/R
      IL_006e:  brfalse.s  IL_0072

      .line 100001,100001 : 0,0 ''
      IL_0070:  ldc.i4.m1
      IL_0071:  ret

      .line 100001,100001 : 0,0 ''
      IL_0072:  ldc.i4.0
      IL_0073:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       55 (0x37)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IEqualityComparer V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0035

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction17/R::y@
      IL_0012:  ldloc.0
      IL_0013:  ldc.i4.6
      IL_0014:  shl
      IL_0015:  ldloc.0
      IL_0016:  ldc.i4.2
      IL_0017:  shr
      IL_0018:  add
      IL_0019:  add
      IL_001a:  add
      IL_001b:  stloc.0
      IL_001c:  ldc.i4     0x9e3779b9
      IL_0021:  ldarg.1
      IL_0022:  stloc.2
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 TestFunction17/R::x@
      IL_0029:  ldloc.0
      IL_002a:  ldc.i4.6
      IL_002b:  shl
      IL_002c:  ldloc.0
      IL_002d:  ldc.i4.2
      IL_002e:  shr
      IL_002f:  add
      IL_0030:  add
      IL_0031:  add
      IL_0032:  stloc.0
      IL_0033:  ldloc.0
      IL_0034:  ret

      .line 100001,100001 : 0,0 ''
      IL_0035:  ldc.i4.0
      IL_0036:  ret
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction17/R::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       62 (0x3e)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0,
               [1] class TestFunction17/R V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction17/R
      IL_0009:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0034

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_000f:  ldarg.2
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 TestFunction17/R::x@
      IL_0017:  ldloc.1
      IL_0018:  ldfld      int32 TestFunction17/R::x@
      IL_001d:  ceq
      IL_001f:  brfalse.s  IL_0032

      .line 100001,100001 : 0,0 ''
      IL_0021:  ldarg.2
      IL_0022:  stloc.3
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 TestFunction17/R::y@
      IL_0029:  ldloc.1
      IL_002a:  ldfld      int32 TestFunction17/R::y@
      IL_002f:  ceq
      IL_0031:  ret

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldc.i4.0
      IL_0033:  ret

      .line 100001,100001 : 0,0 ''
      IL_0034:  ldc.i4.0
      IL_0035:  ret

      .line 100001,100001 : 0,0 ''
      IL_0036:  ldarg.1
      IL_0037:  ldnull
      IL_0038:  cgt.un
      IL_003a:  ldc.i4.0
      IL_003b:  ceq
      IL_003d:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction17/R obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       47 (0x2f)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      .line 100001,100001 : 0,0 ''
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 TestFunction17/R::x@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 TestFunction17/R::x@
      IL_0012:  bne.un.s   IL_0023

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 TestFunction17/R::y@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction17/R::y@
      IL_0020:  ceq
      IL_0022:  ret

      .line 100001,100001 : 0,0 ''
      IL_0023:  ldc.i4.0
      IL_0024:  ret

      .line 100001,100001 : 0,0 ''
      IL_0025:  ldc.i4.0
      IL_0026:  ret

      .line 100001,100001 : 0,0 ''
      IL_0027:  ldarg.1
      IL_0028:  ldnull
      IL_0029:  cgt.un
      IL_002b:  ldc.i4.0
      IL_002c:  ceq
      IL_002e:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction17/R
      IL_0006:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction17/R::Equals(class TestFunction17/R)
      IL_0011:  ret

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method R::Equals

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_x()
    } // end of property R::x
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_y()
    } // end of property R::y
  } // end of class R

  .method public static class [mscorlib]System.Tuple`2<class TestFunction17/R,class TestFunction17/R> 
          TestFunction17(int32 inp) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init ([0] class TestFunction17/R x)
    .line 7,7 : 5,24 ''
    IL_0000:  ldc.i4.3
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void TestFunction17/R::.ctor(int32,
                                                               int32)
    IL_0007:  stloc.0
    .line 8,8 : 5,8 ''
    IL_0008:  ldloc.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class [mscorlib]System.Tuple`2<class TestFunction17/R,class TestFunction17/R>::.ctor(!0,
                                                                                                                            !1)
    IL_000f:  ret
  } // end of method TestFunction17::TestFunction17

} // end of class TestFunction17

.class private abstract auto ansi sealed '<StartupCode$TestFunction17>'.$TestFunction17
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction17::main@

} // end of class '<StartupCode$TestFunction17>'.$TestFunction17


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
