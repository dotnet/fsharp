
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
.assembly TestFunction21
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction21
{
  // Offset: 0x00000000 Length: 0x00000675
}
.mresource public FSharpOptimizationData.TestFunction21
{
  // Offset: 0x00000680 Length: 0x000001CD
}
.module TestFunction21.exe
// MVID: {6220E156-A643-45E6-A745-038356E12062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06FC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction21
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit U
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class TestFunction21/U>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class TestFunction21/U>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly initonly int32 item2
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static class TestFunction21/U 
            NewU(int32 item1,
                 int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void TestFunction21/U::.ctor(int32,
                                                                 int32)
      IL_0007:  ret
    } // end of method U::NewU

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 item1,
                                 int32 item2) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 TestFunction21/U::item1
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction21/U::item2
      IL_0014:  ret
    } // end of method U::.ctor

    .method public hidebysig instance int32 
            get_Item1() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction21/U::item1
      IL_0006:  ret
    } // end of method U::get_Item1

    .method public hidebysig instance int32 
            get_Item2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction21/U::item2
      IL_0006:  ret
    } // end of method U::get_Item2

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } // end of method U::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction21/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       111 (0x6f)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1,
               [2] int32 V_2,
               [3] class [mscorlib]System.Collections.IComparer V_3,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] class [mscorlib]System.Collections.IComparer V_6,
               [7] int32 V_7,
               [8] int32 V_8)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction21.fs'
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0066

      .line 100001,100001 : 0,0 ''
      IL_0006:  ldarg.0
      IL_0007:  pop
      .line 100001,100001 : 0,0 ''
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.3
      IL_0012:  ldloc.0
      IL_0013:  ldfld      int32 TestFunction21/U::item1
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloc.1
      IL_001b:  ldfld      int32 TestFunction21/U::item1
      IL_0020:  stloc.s    V_5
      .line 100001,100001 : 0,0 ''
      IL_0022:  ldloc.s    V_4
      IL_0024:  ldloc.s    V_5
      IL_0026:  bge.s      IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0028:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  br.s       IL_0033

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.s    V_4
      IL_002e:  ldloc.s    V_5
      IL_0030:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0032:  nop
      .line 100001,100001 : 0,0 ''
      IL_0033:  stloc.2
      .line 100001,100001 : 0,0 ''
      IL_0034:  ldloc.2
      IL_0035:  ldc.i4.0
      IL_0036:  bge.s      IL_003a

      .line 100001,100001 : 0,0 ''
      IL_0038:  ldloc.2
      IL_0039:  ret

      .line 100001,100001 : 0,0 ''
      IL_003a:  ldloc.2
      IL_003b:  ldc.i4.0
      IL_003c:  ble.s      IL_0040

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldloc.2
      IL_003f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0040:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.0
      IL_0048:  ldfld      int32 TestFunction21/U::item2
      IL_004d:  stloc.s    V_7
      IL_004f:  ldloc.1
      IL_0050:  ldfld      int32 TestFunction21/U::item2
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
      IL_0069:  brfalse.s  IL_006d

      .line 100001,100001 : 0,0 ''
      IL_006b:  ldc.i4.m1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  ldc.i4.0
      IL_006e:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction21/U
      IL_0007:  callvirt   instance int32 TestFunction21/U::CompareTo(class TestFunction21/U)
      IL_000c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       121 (0x79)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1,
               [2] class TestFunction21/U V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6,
               [7] class [mscorlib]System.Collections.IComparer V_7,
               [8] int32 V_8,
               [9] int32 V_9)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction21/U
      IL_0006:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_006d

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  unbox.any  TestFunction21/U
      IL_0010:  brfalse.s  IL_006b

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldarg.0
      IL_0013:  pop
      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  stloc.1
      IL_0016:  ldloc.0
      IL_0017:  stloc.2
      IL_0018:  ldarg.2
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.1
      IL_001c:  ldfld      int32 TestFunction21/U::item1
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloc.2
      IL_0024:  ldfld      int32 TestFunction21/U::item1
      IL_0029:  stloc.s    V_6
      .line 100001,100001 : 0,0 ''
      IL_002b:  ldloc.s    V_5
      IL_002d:  ldloc.s    V_6
      IL_002f:  bge.s      IL_0035

      .line 100001,100001 : 0,0 ''
      IL_0031:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0032:  nop
      IL_0033:  br.s       IL_003c

      .line 100001,100001 : 0,0 ''
      IL_0035:  ldloc.s    V_5
      IL_0037:  ldloc.s    V_6
      IL_0039:  cgt
      .line 100001,100001 : 0,0 ''
      IL_003b:  nop
      .line 100001,100001 : 0,0 ''
      IL_003c:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_003d:  ldloc.3
      IL_003e:  ldc.i4.0
      IL_003f:  bge.s      IL_0043

      .line 100001,100001 : 0,0 ''
      IL_0041:  ldloc.3
      IL_0042:  ret

      .line 100001,100001 : 0,0 ''
      IL_0043:  ldloc.3
      IL_0044:  ldc.i4.0
      IL_0045:  ble.s      IL_0049

      .line 100001,100001 : 0,0 ''
      IL_0047:  ldloc.3
      IL_0048:  ret

      .line 100001,100001 : 0,0 ''
      IL_0049:  ldarg.2
      IL_004a:  stloc.s    V_7
      IL_004c:  ldloc.1
      IL_004d:  ldfld      int32 TestFunction21/U::item2
      IL_0052:  stloc.s    V_8
      IL_0054:  ldloc.2
      IL_0055:  ldfld      int32 TestFunction21/U::item2
      IL_005a:  stloc.s    V_9
      .line 100001,100001 : 0,0 ''
      IL_005c:  ldloc.s    V_8
      IL_005e:  ldloc.s    V_9
      IL_0060:  bge.s      IL_0064

      .line 100001,100001 : 0,0 ''
      IL_0062:  ldc.i4.m1
      IL_0063:  ret

      .line 100001,100001 : 0,0 ''
      IL_0064:  ldloc.s    V_8
      IL_0066:  ldloc.s    V_9
      IL_0068:  cgt
      IL_006a:  ret

      .line 100001,100001 : 0,0 ''
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  ldarg.1
      IL_006e:  unbox.any  TestFunction21/U
      IL_0073:  brfalse.s  IL_0077

      .line 100001,100001 : 0,0 ''
      IL_0075:  ldc.i4.m1
      IL_0076:  ret

      .line 100001,100001 : 0,0 ''
      IL_0077:  ldc.i4.0
      IL_0078:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       61 (0x3d)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class TestFunction21/U V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_003b

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0005:  ldarg.0
      IL_0006:  pop
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.0
      IL_0008:  stloc.1
      IL_0009:  ldc.i4.0
      IL_000a:  stloc.0
      IL_000b:  ldc.i4     0x9e3779b9
      IL_0010:  ldarg.1
      IL_0011:  stloc.2
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 TestFunction21/U::item2
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.6
      IL_001a:  shl
      IL_001b:  ldloc.0
      IL_001c:  ldc.i4.2
      IL_001d:  shr
      IL_001e:  add
      IL_001f:  add
      IL_0020:  add
      IL_0021:  stloc.0
      IL_0022:  ldc.i4     0x9e3779b9
      IL_0027:  ldarg.1
      IL_0028:  stloc.3
      IL_0029:  ldloc.1
      IL_002a:  ldfld      int32 TestFunction21/U::item1
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.6
      IL_0031:  shl
      IL_0032:  ldloc.0
      IL_0033:  ldc.i4.2
      IL_0034:  shr
      IL_0035:  add
      IL_0036:  add
      IL_0037:  add
      IL_0038:  stloc.0
      IL_0039:  ldloc.0
      IL_003a:  ret

      .line 100001,100001 : 0,0 ''
      IL_003b:  ldc.i4.0
      IL_003c:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction21/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       70 (0x46)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1,
               [2] class TestFunction21/U V_2,
               [3] class TestFunction21/U V_3,
               [4] class [mscorlib]System.Collections.IEqualityComparer V_4,
               [5] class [mscorlib]System.Collections.IEqualityComparer V_5)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_003e

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction21/U
      IL_0009:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_003c

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_000f:  ldarg.0
      IL_0010:  pop
      .line 100001,100001 : 0,0 ''
      IL_0011:  ldarg.0
      IL_0012:  stloc.2
      IL_0013:  ldloc.1
      IL_0014:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_0015:  ldarg.2
      IL_0016:  stloc.s    V_4
      IL_0018:  ldloc.2
      IL_0019:  ldfld      int32 TestFunction21/U::item1
      IL_001e:  ldloc.3
      IL_001f:  ldfld      int32 TestFunction21/U::item1
      IL_0024:  ceq
      IL_0026:  brfalse.s  IL_003a

      .line 100001,100001 : 0,0 ''
      IL_0028:  ldarg.2
      IL_0029:  stloc.s    V_5
      IL_002b:  ldloc.2
      IL_002c:  ldfld      int32 TestFunction21/U::item2
      IL_0031:  ldloc.3
      IL_0032:  ldfld      int32 TestFunction21/U::item2
      IL_0037:  ceq
      IL_0039:  ret

      .line 100001,100001 : 0,0 ''
      IL_003a:  ldc.i4.0
      IL_003b:  ret

      .line 100001,100001 : 0,0 ''
      IL_003c:  ldc.i4.0
      IL_003d:  ret

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldarg.1
      IL_003f:  ldnull
      IL_0040:  cgt.un
      IL_0042:  ldc.i4.0
      IL_0043:  ceq
      IL_0045:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       53 (0x35)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002d

      .line 100001,100001 : 0,0 ''
      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002b

      .line 100001,100001 : 0,0 ''
      IL_0006:  ldarg.0
      IL_0007:  pop
      .line 100001,100001 : 0,0 ''
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_000c:  ldloc.0
      IL_000d:  ldfld      int32 TestFunction21/U::item1
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 TestFunction21/U::item1
      IL_0018:  bne.un.s   IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 TestFunction21/U::item2
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 TestFunction21/U::item2
      IL_0026:  ceq
      IL_0028:  ret

      .line 100001,100001 : 0,0 ''
      IL_0029:  ldc.i4.0
      IL_002a:  ret

      .line 100001,100001 : 0,0 ''
      IL_002b:  ldc.i4.0
      IL_002c:  ret

      .line 100001,100001 : 0,0 ''
      IL_002d:  ldarg.1
      IL_002e:  ldnull
      IL_002f:  cgt.un
      IL_0031:  ldc.i4.0
      IL_0032:  ceq
      IL_0034:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction21/U
      IL_0006:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction21/U::Equals(class TestFunction21/U)
      IL_0011:  ret

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method U::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 TestFunction21/U::get_Tag()
    } // end of property U::Tag
    .property instance int32 Item1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 TestFunction21/U::get_Item1()
    } // end of property U::Item1
    .property instance int32 Item2()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 TestFunction21/U::get_Item2()
    } // end of property U::Item2
  } // end of class U

  .class auto ansi serializable sealed nested assembly beforefieldinit 'TestFunction21@7-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo2
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo2) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> TestFunction21/'TestFunction21@7-1'::clo2
      IL_000d:  ret
    } // end of method 'TestFunction21@7-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit 
            Invoke(int32 arg20) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> TestFunction21/'TestFunction21@7-1'::clo2
      IL_0006:  ldarg.1
      IL_0007:  tail.
      IL_0009:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000e:  ret
    } // end of method 'TestFunction21@7-1'::Invoke

  } // end of class 'TestFunction21@7-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit TestFunction21@7
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo1) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> TestFunction21/TestFunction21@7::clo1
      IL_000d:  ret
    } // end of method TestFunction21@7::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            Invoke(int32 arg10) cil managed
    {
      // Code size       20 (0x14)
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> TestFunction21/TestFunction21@7::clo1
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void TestFunction21/'TestFunction21@7-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  ret
    } // end of method TestFunction21@7::Invoke

  } // end of class TestFunction21@7

  .method public static void  TestFunction21(class TestFunction21/U _arg1) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  5
    .locals init ([0] class TestFunction21/U V_0,
             [1] int32 b,
             [2] int32 a,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> V_3)
    .line 100001,100001 : 0,0 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldfld      int32 TestFunction21/U::item2
    IL_0008:  stloc.1
    IL_0009:  ldloc.0
    IL_000a:  ldfld      int32 TestFunction21/U::item1
    IL_000f:  stloc.2
    .line 7,7 : 5,33 ''
    IL_0010:  ldstr      "a = %A, a = %A"
    IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Tuple`2<int32,int32>>::.ctor(string)
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001f:  stloc.3
    IL_0020:  ldloc.3
    IL_0021:  newobj     instance void TestFunction21/TestFunction21@7::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
    IL_0026:  ldloc.2
    IL_0027:  ldloc.1
    IL_0028:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                           !0,
                                                                                                                                                           !1)
    IL_002d:  pop
    IL_002e:  ret
  } // end of method TestFunction21::TestFunction21

} // end of class TestFunction21

.class private abstract auto ansi sealed '<StartupCode$TestFunction21>'.$TestFunction21
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction21::main@

} // end of class '<StartupCode$TestFunction21>'.$TestFunction21


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
