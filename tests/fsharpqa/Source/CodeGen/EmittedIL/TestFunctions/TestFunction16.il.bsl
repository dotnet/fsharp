
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
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
  .ver 4:7:0:0
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
  // Offset: 0x00000000 Length: 0x00000683
}
.mresource public FSharpSignatureDataB.TestFunction16
{
  // Offset: 0x00000688 Length: 0x0000007F
}
.mresource public FSharpOptimizationData.TestFunction16
{
  // Offset: 0x00000710 Length: 0x000001CD
}
.mresource public FSharpOptimizationDataB.TestFunction16
{
  // Offset: 0x000008E8 Length: 0x0000002A
}
.module TestFunction16.exe
// MVID: {5E171A36-A624-45C5-A745-0383361A175E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07180000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction16
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit U
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
      .maxstack  8
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
      .maxstack  8
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

    .method public hidebysig instance int32 
            get_Item1() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction16/U::item1
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
      IL_0001:  ldfld      int32 TestFunction16/U::item2
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
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction16/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction16/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction16/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       177 (0xb1)
      .maxstack  4
      .locals init ([0] class TestFunction16/U V_0,
               [1] class TestFunction16/U V_1,
               [2] int32 V_2,
               [3] class [mscorlib]System.Collections.IComparer V_3,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] class [mscorlib]System.Collections.IComparer V_6,
               [7] int32 V_7,
               [8] int32 V_8,
               [9] class [mscorlib]System.Collections.IComparer V_9,
               [10] int32 V_10,
               [11] int32 V_11,
               [12] class [mscorlib]System.Collections.IComparer V_12,
               [13] int32 V_13,
               [14] int32 V_14)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction16.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000d

      IL_0008:  br         IL_00a3

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  ldnull
      IL_000f:  cgt.un
      IL_0011:  brfalse.s  IL_0015

      IL_0013:  br.s       IL_001a

      IL_0015:  br         IL_00a1

      .line 100001,100001 : 0,0 ''
      IL_001a:  ldarg.0
      IL_001b:  pop
      .line 100001,100001 : 0,0 ''
      IL_001c:  ldarg.0
      IL_001d:  stloc.0
      IL_001e:  ldarg.1
      IL_001f:  stloc.1
      IL_0020:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0025:  stloc.3
      IL_0026:  ldloc.0
      IL_0027:  ldfld      int32 TestFunction16/U::item1
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.1
      IL_002f:  ldfld      int32 TestFunction16/U::item1
      IL_0034:  stloc.s    V_5
      IL_0036:  ldloc.3
      IL_0037:  stloc.s    V_6
      IL_0039:  ldloc.s    V_4
      IL_003b:  stloc.s    V_7
      IL_003d:  ldloc.s    V_5
      IL_003f:  stloc.s    V_8
      IL_0041:  ldloc.s    V_7
      IL_0043:  ldloc.s    V_8
      IL_0045:  bge.s      IL_0049

      IL_0047:  br.s       IL_004b

      IL_0049:  br.s       IL_004f

      .line 100001,100001 : 0,0 ''
      IL_004b:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_004c:  nop
      IL_004d:  br.s       IL_0056

      .line 100001,100001 : 0,0 ''
      IL_004f:  ldloc.s    V_7
      IL_0051:  ldloc.s    V_8
      IL_0053:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0055:  nop
      .line 100001,100001 : 0,0 ''
      IL_0056:  stloc.2
      IL_0057:  ldloc.2
      IL_0058:  ldc.i4.0
      IL_0059:  bge.s      IL_005d

      IL_005b:  br.s       IL_005f

      IL_005d:  br.s       IL_0061

      .line 100001,100001 : 0,0 ''
      IL_005f:  ldloc.2
      IL_0060:  ret

      .line 100001,100001 : 0,0 ''
      IL_0061:  ldloc.2
      IL_0062:  ldc.i4.0
      IL_0063:  ble.s      IL_0067

      IL_0065:  br.s       IL_0069

      IL_0067:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_0069:  ldloc.2
      IL_006a:  ret

      .line 100001,100001 : 0,0 ''
      IL_006b:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0070:  stloc.s    V_9
      IL_0072:  ldloc.0
      IL_0073:  ldfld      int32 TestFunction16/U::item2
      IL_0078:  stloc.s    V_10
      IL_007a:  ldloc.1
      IL_007b:  ldfld      int32 TestFunction16/U::item2
      IL_0080:  stloc.s    V_11
      IL_0082:  ldloc.s    V_9
      IL_0084:  stloc.s    V_12
      IL_0086:  ldloc.s    V_10
      IL_0088:  stloc.s    V_13
      IL_008a:  ldloc.s    V_11
      IL_008c:  stloc.s    V_14
      IL_008e:  ldloc.s    V_13
      IL_0090:  ldloc.s    V_14
      IL_0092:  bge.s      IL_0096

      IL_0094:  br.s       IL_0098

      IL_0096:  br.s       IL_009a

      .line 100001,100001 : 0,0 ''
      IL_0098:  ldc.i4.m1
      IL_0099:  ret

      .line 100001,100001 : 0,0 ''
      IL_009a:  ldloc.s    V_13
      IL_009c:  ldloc.s    V_14
      IL_009e:  cgt
      IL_00a0:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a1:  ldc.i4.1
      IL_00a2:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a3:  ldarg.1
      IL_00a4:  ldnull
      IL_00a5:  cgt.un
      IL_00a7:  brfalse.s  IL_00ab

      IL_00a9:  br.s       IL_00ad

      IL_00ab:  br.s       IL_00af

      .line 100001,100001 : 0,0 ''
      IL_00ad:  ldc.i4.m1
      IL_00ae:  ret

      .line 100001,100001 : 0,0 ''
      IL_00af:  ldc.i4.0
      IL_00b0:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction16/U
      IL_0007:  callvirt   instance int32 TestFunction16/U::CompareTo(class TestFunction16/U)
      IL_000c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       188 (0xbc)
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
               [9] int32 V_9,
               [10] class [mscorlib]System.Collections.IComparer V_10,
               [11] int32 V_11,
               [12] int32 V_12,
               [13] class [mscorlib]System.Collections.IComparer V_13,
               [14] int32 V_14,
               [15] int32 V_15)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction16/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  br.s       IL_0014

      IL_000f:  br         IL_00a9

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.1
      IL_0015:  unbox.any  TestFunction16/U
      IL_001a:  ldnull
      IL_001b:  cgt.un
      IL_001d:  brfalse.s  IL_0021

      IL_001f:  br.s       IL_0026

      IL_0021:  br         IL_00a7

      .line 100001,100001 : 0,0 ''
      IL_0026:  ldarg.0
      IL_0027:  pop
      .line 100001,100001 : 0,0 ''
      IL_0028:  ldarg.0
      IL_0029:  stloc.1
      IL_002a:  ldloc.0
      IL_002b:  stloc.2
      IL_002c:  ldarg.2
      IL_002d:  stloc.s    V_4
      IL_002f:  ldloc.1
      IL_0030:  ldfld      int32 TestFunction16/U::item1
      IL_0035:  stloc.s    V_5
      IL_0037:  ldloc.2
      IL_0038:  ldfld      int32 TestFunction16/U::item1
      IL_003d:  stloc.s    V_6
      IL_003f:  ldloc.s    V_4
      IL_0041:  stloc.s    V_7
      IL_0043:  ldloc.s    V_5
      IL_0045:  stloc.s    V_8
      IL_0047:  ldloc.s    V_6
      IL_0049:  stloc.s    V_9
      IL_004b:  ldloc.s    V_8
      IL_004d:  ldloc.s    V_9
      IL_004f:  bge.s      IL_0053

      IL_0051:  br.s       IL_0055

      IL_0053:  br.s       IL_0059

      .line 100001,100001 : 0,0 ''
      IL_0055:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0056:  nop
      IL_0057:  br.s       IL_0060

      .line 100001,100001 : 0,0 ''
      IL_0059:  ldloc.s    V_8
      IL_005b:  ldloc.s    V_9
      IL_005d:  cgt
      .line 100001,100001 : 0,0 ''
      IL_005f:  nop
      .line 100001,100001 : 0,0 ''
      IL_0060:  stloc.3
      IL_0061:  ldloc.3
      IL_0062:  ldc.i4.0
      IL_0063:  bge.s      IL_0067

      IL_0065:  br.s       IL_0069

      IL_0067:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_0069:  ldloc.3
      IL_006a:  ret

      .line 100001,100001 : 0,0 ''
      IL_006b:  ldloc.3
      IL_006c:  ldc.i4.0
      IL_006d:  ble.s      IL_0071

      IL_006f:  br.s       IL_0073

      IL_0071:  br.s       IL_0075

      .line 100001,100001 : 0,0 ''
      IL_0073:  ldloc.3
      IL_0074:  ret

      .line 100001,100001 : 0,0 ''
      IL_0075:  ldarg.2
      IL_0076:  stloc.s    V_10
      IL_0078:  ldloc.1
      IL_0079:  ldfld      int32 TestFunction16/U::item2
      IL_007e:  stloc.s    V_11
      IL_0080:  ldloc.2
      IL_0081:  ldfld      int32 TestFunction16/U::item2
      IL_0086:  stloc.s    V_12
      IL_0088:  ldloc.s    V_10
      IL_008a:  stloc.s    V_13
      IL_008c:  ldloc.s    V_11
      IL_008e:  stloc.s    V_14
      IL_0090:  ldloc.s    V_12
      IL_0092:  stloc.s    V_15
      IL_0094:  ldloc.s    V_14
      IL_0096:  ldloc.s    V_15
      IL_0098:  bge.s      IL_009c

      IL_009a:  br.s       IL_009e

      IL_009c:  br.s       IL_00a0

      .line 100001,100001 : 0,0 ''
      IL_009e:  ldc.i4.m1
      IL_009f:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a0:  ldloc.s    V_14
      IL_00a2:  ldloc.s    V_15
      IL_00a4:  cgt
      IL_00a6:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a7:  ldc.i4.1
      IL_00a8:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a9:  ldarg.1
      IL_00aa:  unbox.any  TestFunction16/U
      IL_00af:  ldnull
      IL_00b0:  cgt.un
      IL_00b2:  brfalse.s  IL_00b6

      IL_00b4:  br.s       IL_00b8

      IL_00b6:  br.s       IL_00ba

      .line 100001,100001 : 0,0 ''
      IL_00b8:  ldc.i4.m1
      IL_00b9:  ret

      .line 100001,100001 : 0,0 ''
      IL_00ba:  ldc.i4.0
      IL_00bb:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       82 (0x52)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class TestFunction16/U V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IEqualityComparer V_4,
               [5] class [mscorlib]System.Collections.IEqualityComparer V_5,
               [6] int32 V_6,
               [7] class [mscorlib]System.Collections.IEqualityComparer V_7)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0050

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  pop
      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  stloc.1
      IL_0010:  ldc.i4.0
      IL_0011:  stloc.0
      IL_0012:  ldc.i4     0x9e3779b9
      IL_0017:  ldarg.1
      IL_0018:  stloc.2
      IL_0019:  ldloc.1
      IL_001a:  ldfld      int32 TestFunction16/U::item2
      IL_001f:  stloc.3
      IL_0020:  ldloc.2
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloc.3
      IL_0024:  ldloc.0
      IL_0025:  ldc.i4.6
      IL_0026:  shl
      IL_0027:  ldloc.0
      IL_0028:  ldc.i4.2
      IL_0029:  shr
      IL_002a:  add
      IL_002b:  add
      IL_002c:  add
      IL_002d:  stloc.0
      IL_002e:  ldc.i4     0x9e3779b9
      IL_0033:  ldarg.1
      IL_0034:  stloc.s    V_5
      IL_0036:  ldloc.1
      IL_0037:  ldfld      int32 TestFunction16/U::item1
      IL_003c:  stloc.s    V_6
      IL_003e:  ldloc.s    V_5
      IL_0040:  stloc.s    V_7
      IL_0042:  ldloc.s    V_6
      IL_0044:  ldloc.0
      IL_0045:  ldc.i4.6
      IL_0046:  shl
      IL_0047:  ldloc.0
      IL_0048:  ldc.i4.2
      IL_0049:  shr
      IL_004a:  add
      IL_004b:  add
      IL_004c:  add
      IL_004d:  stloc.0
      IL_004e:  ldloc.0
      IL_004f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0050:  ldc.i4.0
      IL_0051:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction16/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       109 (0x6d)
      .maxstack  4
      .locals init ([0] class TestFunction16/U V_0,
               [1] class TestFunction16/U V_1,
               [2] class TestFunction16/U V_2,
               [3] class TestFunction16/U V_3,
               [4] class [mscorlib]System.Collections.IEqualityComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6,
               [7] class [mscorlib]System.Collections.IEqualityComparer V_7,
               [8] class [mscorlib]System.Collections.IEqualityComparer V_8,
               [9] int32 V_9,
               [10] int32 V_10,
               [11] class [mscorlib]System.Collections.IEqualityComparer V_11)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0065

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  isinst     TestFunction16/U
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_0018

      IL_0016:  br.s       IL_0063

      .line 100001,100001 : 0,0 ''
      IL_0018:  ldloc.0
      IL_0019:  stloc.1
      IL_001a:  ldarg.0
      IL_001b:  pop
      .line 100001,100001 : 0,0 ''
      IL_001c:  ldarg.0
      IL_001d:  stloc.2
      IL_001e:  ldloc.1
      IL_001f:  stloc.3
      IL_0020:  ldarg.2
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloc.2
      IL_0024:  ldfld      int32 TestFunction16/U::item1
      IL_0029:  stloc.s    V_5
      IL_002b:  ldloc.3
      IL_002c:  ldfld      int32 TestFunction16/U::item1
      IL_0031:  stloc.s    V_6
      IL_0033:  ldloc.s    V_4
      IL_0035:  stloc.s    V_7
      IL_0037:  ldloc.s    V_5
      IL_0039:  ldloc.s    V_6
      IL_003b:  ceq
      IL_003d:  brfalse.s  IL_0041

      IL_003f:  br.s       IL_0043

      IL_0041:  br.s       IL_0061

      .line 100001,100001 : 0,0 ''
      IL_0043:  ldarg.2
      IL_0044:  stloc.s    V_8
      IL_0046:  ldloc.2
      IL_0047:  ldfld      int32 TestFunction16/U::item2
      IL_004c:  stloc.s    V_9
      IL_004e:  ldloc.3
      IL_004f:  ldfld      int32 TestFunction16/U::item2
      IL_0054:  stloc.s    V_10
      IL_0056:  ldloc.s    V_8
      IL_0058:  stloc.s    V_11
      IL_005a:  ldloc.s    V_9
      IL_005c:  ldloc.s    V_10
      IL_005e:  ceq
      IL_0060:  ret

      .line 100001,100001 : 0,0 ''
      IL_0061:  ldc.i4.0
      IL_0062:  ret

      .line 100001,100001 : 0,0 ''
      IL_0063:  ldc.i4.0
      IL_0064:  ret

      .line 100001,100001 : 0,0 ''
      IL_0065:  ldarg.1
      IL_0066:  ldnull
      IL_0067:  cgt.un
      IL_0069:  ldc.i4.0
      IL_006a:  ceq
      IL_006c:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction16/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       71 (0x47)
      .maxstack  4
      .locals init ([0] class TestFunction16/U V_0,
               [1] class TestFunction16/U V_1)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_003f

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  ldnull
      IL_000c:  cgt.un
      IL_000e:  brfalse.s  IL_0012

      IL_0010:  br.s       IL_0014

      IL_0012:  br.s       IL_003d

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  pop
      .line 100001,100001 : 0,0 ''
      IL_0016:  ldarg.0
      IL_0017:  stloc.0
      IL_0018:  ldarg.1
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 TestFunction16/U::item1
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 TestFunction16/U::item1
      IL_0026:  bne.un.s   IL_002a

      IL_0028:  br.s       IL_002c

      IL_002a:  br.s       IL_003b

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.0
      IL_002d:  ldfld      int32 TestFunction16/U::item2
      IL_0032:  ldloc.1
      IL_0033:  ldfld      int32 TestFunction16/U::item2
      IL_0038:  ceq
      IL_003a:  ret

      .line 100001,100001 : 0,0 ''
      IL_003b:  ldc.i4.0
      IL_003c:  ret

      .line 100001,100001 : 0,0 ''
      IL_003d:  ldc.i4.0
      IL_003e:  ret

      .line 100001,100001 : 0,0 ''
      IL_003f:  ldarg.1
      IL_0040:  ldnull
      IL_0041:  cgt.un
      IL_0043:  ldc.i4.0
      IL_0044:  ceq
      IL_0046:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       24 (0x18)
      .maxstack  4
      .locals init ([0] class TestFunction16/U V_0)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction16/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance bool TestFunction16/U::Equals(class TestFunction16/U)
      IL_0015:  ret

      .line 100001,100001 : 0,0 ''
      IL_0016:  ldc.i4.0
      IL_0017:  ret
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
    // Code size       16 (0x10)
    .maxstack  4
    .locals init ([0] class TestFunction16/U x)
    .line 7,7 : 5,23 ''
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       class TestFunction16/U TestFunction16/U::NewU(int32,
                                                                       int32)
    IL_0007:  stloc.0
    .line 8,8 : 5,8 ''
    IL_0008:  ldloc.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class [mscorlib]System.Tuple`2<class TestFunction16/U,class TestFunction16/U>::.ctor(!0,
                                                                                                                            !1)
    IL_000f:  ret
  } // end of method TestFunction16::TestFunction16

} // end of class TestFunction16

.class private abstract auto ansi sealed '<StartupCode$TestFunction16>'.$TestFunction16
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction16::main@

} // end of class '<StartupCode$TestFunction16>'.$TestFunction16


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
