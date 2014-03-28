
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
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
  .ver 4:0:0:0
}
.assembly TestFunction16
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction16
{
  // Offset: 0x00000000 Length: 0x0000069F
}
.mresource public FSharpOptimizationData.TestFunction16
{
  // Offset: 0x000006A8 Length: 0x000001CD
}
.module TestFunction16.exe
// MVID: {4BEB28EB-A624-45C5-A745-0383EB28EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x002E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction16
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable nested public beforefieldinit U
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class TestFunction16/U>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class TestFunction16/U>,
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
    .method public static class TestFunction16/U 
            NewU(int32 item1,
                 int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void TestFunction16/U::.ctor(int32,
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
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 TestFunction16/U::item1
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction16/U::item2
      IL_0014:  ret
    } // end of method U::.ctor

    .method public instance int32  get_Item1() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction16/U::item1
      IL_0006:  ret
    } // end of method U::get_Item1

    .method public instance int32  get_Item2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction16/U::item2
      IL_0006:  ret
    } // end of method U::get_Item2

    .method public instance int32  get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       4 (0x4)
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } // end of method U::get_Tag

    .method assembly specialname instance object 
            __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  4
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::__DebugDisplay

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction16/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       155 (0x9b)
      .maxstack  4
      .locals init (class TestFunction16/U V_0,
               class TestFunction16/U V_1,
               int32 V_2,
               class [mscorlib]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [mscorlib]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000e

      IL_0009:  br         IL_008d

      IL_000e:  ldarg.1
      IL_000f:  ldnull
      IL_0010:  cgt.un
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_001b

      IL_0016:  br         IL_008b

      IL_001b:  ldarg.0
      IL_001c:  pop
      IL_001d:  ldarg.0
      IL_001e:  stloc.0
      IL_001f:  ldarg.1
      IL_0020:  stloc.1
      IL_0021:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0026:  stloc.3
      IL_0027:  ldloc.0
      IL_0028:  ldfld      int32 TestFunction16/U::item1
      IL_002d:  stloc.s    V_4
      IL_002f:  ldloc.1
      IL_0030:  ldfld      int32 TestFunction16/U::item1
      IL_0035:  stloc.s    V_5
      IL_0037:  ldloc.s    V_4
      IL_0039:  ldloc.s    V_5
      IL_003b:  bge.s      IL_003f

      IL_003d:  br.s       IL_0041

      IL_003f:  br.s       IL_0045

      IL_0041:  ldc.i4.m1
      IL_0042:  nop
      IL_0043:  br.s       IL_004c

      IL_0045:  ldloc.s    V_4
      IL_0047:  ldloc.s    V_5
      IL_0049:  cgt
      IL_004b:  nop
      IL_004c:  stloc.2
      IL_004d:  ldloc.2
      IL_004e:  ldc.i4.0
      IL_004f:  bge.s      IL_0053

      IL_0051:  br.s       IL_0055

      IL_0053:  br.s       IL_0057

      IL_0055:  ldloc.2
      IL_0056:  ret

      IL_0057:  ldloc.2
      IL_0058:  ldc.i4.0
      IL_0059:  ble.s      IL_005d

      IL_005b:  br.s       IL_005f

      IL_005d:  br.s       IL_0061

      IL_005f:  ldloc.2
      IL_0060:  ret

      IL_0061:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0066:  stloc.s    V_6
      IL_0068:  ldloc.0
      IL_0069:  ldfld      int32 TestFunction16/U::item2
      IL_006e:  stloc.s    V_7
      IL_0070:  ldloc.1
      IL_0071:  ldfld      int32 TestFunction16/U::item2
      IL_0076:  stloc.s    V_8
      IL_0078:  ldloc.s    V_7
      IL_007a:  ldloc.s    V_8
      IL_007c:  bge.s      IL_0080

      IL_007e:  br.s       IL_0082

      IL_0080:  br.s       IL_0084

      IL_0082:  ldc.i4.m1
      IL_0083:  ret

      IL_0084:  ldloc.s    V_7
      IL_0086:  ldloc.s    V_8
      IL_0088:  cgt
      IL_008a:  ret

      IL_008b:  ldc.i4.1
      IL_008c:  ret

      IL_008d:  ldarg.1
      IL_008e:  ldnull
      IL_008f:  cgt.un
      IL_0091:  brfalse.s  IL_0095

      IL_0093:  br.s       IL_0097

      IL_0095:  br.s       IL_0099

      IL_0097:  ldc.i4.m1
      IL_0098:  ret

      IL_0099:  ldc.i4.0
      IL_009a:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  4
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 4,4 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  unbox.any  TestFunction16/U
      IL_0008:  call       instance int32 TestFunction16/U::CompareTo(class TestFunction16/U)
      IL_000d:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       165 (0xa5)
      .maxstack  4
      .locals init ([0] class TestFunction16/U V_0,
               [1] class TestFunction16/U V_1,
               [2] class TestFunction16/U V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6,
               [7] class [mscorlib]System.Collections.IComparer V_7,
               [8] int32 V_8,
               [9] int32 V_9)
      .line 4,4 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction16/U
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  ldnull
      IL_000a:  cgt.un
      IL_000c:  brfalse.s  IL_0010

      IL_000e:  br.s       IL_0015

      IL_0010:  br         IL_0092

      .line 100001,100001 : 0,0 
      IL_0015:  ldarg.1
      IL_0016:  unbox.any  TestFunction16/U
      IL_001b:  ldnull
      IL_001c:  cgt.un
      IL_001e:  brfalse.s  IL_0022

      IL_0020:  br.s       IL_0027

      IL_0022:  br         IL_0090

      .line 100001,100001 : 0,0 
      IL_0027:  ldarg.0
      IL_0028:  pop
      .line 100001,100001 : 0,0 
      IL_0029:  ldarg.0
      IL_002a:  stloc.1
      IL_002b:  ldloc.0
      IL_002c:  stloc.2
      IL_002d:  ldarg.2
      IL_002e:  stloc.s    V_4
      IL_0030:  ldloc.1
      IL_0031:  ldfld      int32 TestFunction16/U::item1
      IL_0036:  stloc.s    V_5
      IL_0038:  ldloc.2
      IL_0039:  ldfld      int32 TestFunction16/U::item1
      IL_003e:  stloc.s    V_6
      IL_0040:  ldloc.s    V_5
      IL_0042:  ldloc.s    V_6
      IL_0044:  bge.s      IL_0048

      IL_0046:  br.s       IL_004a

      IL_0048:  br.s       IL_004e

      .line 100001,100001 : 0,0 
      IL_004a:  ldc.i4.m1
      .line 100001,100001 : 0,0 
      IL_004b:  nop
      IL_004c:  br.s       IL_0055

      .line 100001,100001 : 0,0 
      IL_004e:  ldloc.s    V_5
      IL_0050:  ldloc.s    V_6
      IL_0052:  cgt
      .line 100001,100001 : 0,0 
      IL_0054:  nop
      .line 100001,100001 : 0,0 
      IL_0055:  stloc.3
      IL_0056:  ldloc.3
      IL_0057:  ldc.i4.0
      IL_0058:  bge.s      IL_005c

      IL_005a:  br.s       IL_005e

      IL_005c:  br.s       IL_0060

      .line 100001,100001 : 0,0 
      IL_005e:  ldloc.3
      IL_005f:  ret

      .line 100001,100001 : 0,0 
      IL_0060:  ldloc.3
      IL_0061:  ldc.i4.0
      IL_0062:  ble.s      IL_0066

      IL_0064:  br.s       IL_0068

      IL_0066:  br.s       IL_006a

      .line 100001,100001 : 0,0 
      IL_0068:  ldloc.3
      IL_0069:  ret

      .line 100001,100001 : 0,0 
      IL_006a:  ldarg.2
      IL_006b:  stloc.s    V_7
      IL_006d:  ldloc.1
      IL_006e:  ldfld      int32 TestFunction16/U::item2
      IL_0073:  stloc.s    V_8
      IL_0075:  ldloc.2
      IL_0076:  ldfld      int32 TestFunction16/U::item2
      IL_007b:  stloc.s    V_9
      IL_007d:  ldloc.s    V_8
      IL_007f:  ldloc.s    V_9
      IL_0081:  bge.s      IL_0085

      IL_0083:  br.s       IL_0087

      IL_0085:  br.s       IL_0089

      .line 100001,100001 : 0,0 
      IL_0087:  ldc.i4.m1
      IL_0088:  ret

      .line 100001,100001 : 0,0 
      IL_0089:  ldloc.s    V_8
      IL_008b:  ldloc.s    V_9
      IL_008d:  cgt
      IL_008f:  ret

      .line 100001,100001 : 0,0 
      IL_0090:  ldc.i4.1
      IL_0091:  ret

      .line 100001,100001 : 0,0 
      IL_0092:  ldarg.1
      IL_0093:  unbox.any  TestFunction16/U
      IL_0098:  ldnull
      IL_0099:  cgt.un
      IL_009b:  brfalse.s  IL_009f

      IL_009d:  br.s       IL_00a1

      IL_009f:  br.s       IL_00a3

      .line 100001,100001 : 0,0 
      IL_00a1:  ldc.i4.m1
      IL_00a2:  ret

      .line 100001,100001 : 0,0 
      IL_00a3:  ldc.i4.0
      IL_00a4:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       69 (0x45)
      .maxstack  7
      .locals init (int32 V_0,
               class TestFunction16/U V_1,
               class [mscorlib]System.Collections.IEqualityComparer V_2,
               class [mscorlib]System.Collections.IEqualityComparer V_3)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_0043

      IL_000b:  ldc.i4.0
      IL_000c:  stloc.0
      IL_000d:  ldarg.0
      IL_000e:  pop
      IL_000f:  ldarg.0
      IL_0010:  stloc.1
      IL_0011:  ldc.i4.0
      IL_0012:  stloc.0
      IL_0013:  ldc.i4     0x9e3779b9
      IL_0018:  ldarg.1
      IL_0019:  stloc.2
      IL_001a:  ldloc.1
      IL_001b:  ldfld      int32 TestFunction16/U::item2
      IL_0020:  ldloc.0
      IL_0021:  ldc.i4.6
      IL_0022:  shl
      IL_0023:  ldloc.0
      IL_0024:  ldc.i4.2
      IL_0025:  shr
      IL_0026:  add
      IL_0027:  add
      IL_0028:  add
      IL_0029:  stloc.0
      IL_002a:  ldc.i4     0x9e3779b9
      IL_002f:  ldarg.1
      IL_0030:  stloc.3
      IL_0031:  ldloc.1
      IL_0032:  ldfld      int32 TestFunction16/U::item1
      IL_0037:  ldloc.0
      IL_0038:  ldc.i4.6
      IL_0039:  shl
      IL_003a:  ldloc.0
      IL_003b:  ldc.i4.2
      IL_003c:  shr
      IL_003d:  add
      IL_003e:  add
      IL_003f:  add
      IL_0040:  stloc.0
      IL_0041:  ldloc.0
      IL_0042:  ret

      IL_0043:  ldc.i4.0
      IL_0044:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  4
      .line 4,4 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0007:  call       instance int32 TestFunction16/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000c:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       86 (0x56)
      .maxstack  4
      .locals init (class TestFunction16/U V_0,
               class TestFunction16/U V_1,
               class TestFunction16/U V_2,
               class TestFunction16/U V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4,
               class [mscorlib]System.Collections.IEqualityComparer V_5)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_004e

      IL_000b:  ldarg.1
      IL_000c:  isinst     TestFunction16/U
      IL_0011:  stloc.0
      IL_0012:  ldloc.0
      IL_0013:  brfalse.s  IL_0017

      IL_0015:  br.s       IL_0019

      IL_0017:  br.s       IL_004c

      IL_0019:  ldloc.0
      IL_001a:  stloc.1
      IL_001b:  ldarg.0
      IL_001c:  pop
      IL_001d:  ldarg.0
      IL_001e:  stloc.2
      IL_001f:  ldloc.1
      IL_0020:  stloc.3
      IL_0021:  ldarg.2
      IL_0022:  stloc.s    V_4
      IL_0024:  ldloc.2
      IL_0025:  ldfld      int32 TestFunction16/U::item1
      IL_002a:  ldloc.3
      IL_002b:  ldfld      int32 TestFunction16/U::item1
      IL_0030:  ceq
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  br.s       IL_0038

      IL_0036:  br.s       IL_004a

      IL_0038:  ldarg.2
      IL_0039:  stloc.s    V_5
      IL_003b:  ldloc.2
      IL_003c:  ldfld      int32 TestFunction16/U::item2
      IL_0041:  ldloc.3
      IL_0042:  ldfld      int32 TestFunction16/U::item2
      IL_0047:  ceq
      IL_0049:  ret

      IL_004a:  ldc.i4.0
      IL_004b:  ret

      IL_004c:  ldc.i4.0
      IL_004d:  ret

      IL_004e:  ldarg.1
      IL_004f:  ldnull
      IL_0050:  cgt.un
      IL_0052:  ldc.i4.0
      IL_0053:  ceq
      IL_0055:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction16/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       72 (0x48)
      .maxstack  4
      .locals init (class TestFunction16/U V_0,
               class TestFunction16/U V_1)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_0040

      IL_000b:  ldarg.1
      IL_000c:  ldnull
      IL_000d:  cgt.un
      IL_000f:  brfalse.s  IL_0013

      IL_0011:  br.s       IL_0015

      IL_0013:  br.s       IL_003e

      IL_0015:  ldarg.0
      IL_0016:  pop
      IL_0017:  ldarg.0
      IL_0018:  stloc.0
      IL_0019:  ldarg.1
      IL_001a:  stloc.1
      IL_001b:  ldloc.0
      IL_001c:  ldfld      int32 TestFunction16/U::item1
      IL_0021:  ldloc.1
      IL_0022:  ldfld      int32 TestFunction16/U::item1
      IL_0027:  bne.un.s   IL_002b

      IL_0029:  br.s       IL_002d

      IL_002b:  br.s       IL_003c

      IL_002d:  ldloc.0
      IL_002e:  ldfld      int32 TestFunction16/U::item2
      IL_0033:  ldloc.1
      IL_0034:  ldfld      int32 TestFunction16/U::item2
      IL_0039:  ceq
      IL_003b:  ret

      IL_003c:  ldc.i4.0
      IL_003d:  ret

      IL_003e:  ldc.i4.0
      IL_003f:  ret

      IL_0040:  ldarg.1
      IL_0041:  ldnull
      IL_0042:  cgt.un
      IL_0044:  ldc.i4.0
      IL_0045:  ceq
      IL_0047:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       25 (0x19)
      .maxstack  4
      .locals init (class TestFunction16/U V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  isinst     TestFunction16/U
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  brfalse.s  IL_000d

      IL_000b:  br.s       IL_000f

      IL_000d:  br.s       IL_0017

      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  call       instance bool TestFunction16/U::Equals(class TestFunction16/U)
      IL_0016:  ret

      IL_0017:  ldc.i4.0
      IL_0018:  ret
    } // end of method U::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 TestFunction16/U::get_Tag()
    } // end of property U::Tag
    .property instance int32 Item1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 TestFunction16/U::get_Item1()
    } // end of property U::Item1
    .property instance int32 Item2()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 TestFunction16/U::get_Item2()
    } // end of property U::Item2
  } // end of class U

  .method public static class [mscorlib]System.Tuple`2<class TestFunction16/U,class TestFunction16/U> 
          TestFunction16(int32 inp) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  4
    .locals init ([0] class TestFunction16/U x)
    .line 7,7 : 5,23 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  call       class TestFunction16/U TestFunction16/U::NewU(int32,
                                                                       int32)
    IL_0008:  stloc.0
    .line 8,8 : 5,8 
    IL_0009:  ldloc.0
    IL_000a:  ldloc.0
    IL_000b:  newobj     instance void class [mscorlib]System.Tuple`2<class TestFunction16/U,class TestFunction16/U>::.ctor(!0,
                                                                                                                            !1)
    IL_0010:  ret
  } // end of method TestFunction16::TestFunction16

} // end of class TestFunction16

.class private abstract auto ansi sealed '<StartupCode$TestFunction16>'.$TestFunction16
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  2
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction16::main@

} // end of class '<StartupCode$TestFunction16>'.$TestFunction16


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
