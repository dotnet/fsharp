
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
  // Offset: 0x00000000 Length: 0x00000685
}
.mresource public FSharpOptimizationData.TestFunction21
{
  // Offset: 0x00000690 Length: 0x000001CD
}
.module TestFunction21.exe
// MVID: {59B19208-A643-45E6-A745-03830892B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00F80000


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
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
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
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction21/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       154 (0x9a)
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
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction21.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000d

      IL_0008:  br         IL_008c

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  ldnull
      IL_000f:  cgt.un
      IL_0011:  brfalse.s  IL_0015

      IL_0013:  br.s       IL_001a

      IL_0015:  br         IL_008a

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
      IL_0027:  ldfld      int32 TestFunction21/U::item1
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.1
      IL_002f:  ldfld      int32 TestFunction21/U::item1
      IL_0034:  stloc.s    V_5
      IL_0036:  ldloc.s    V_4
      IL_0038:  ldloc.s    V_5
      IL_003a:  bge.s      IL_003e

      IL_003c:  br.s       IL_0040

      IL_003e:  br.s       IL_0044

      .line 100001,100001 : 0,0 ''
      IL_0040:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0041:  nop
      IL_0042:  br.s       IL_004b

      .line 100001,100001 : 0,0 ''
      IL_0044:  ldloc.s    V_4
      IL_0046:  ldloc.s    V_5
      IL_0048:  cgt
      .line 100001,100001 : 0,0 ''
      IL_004a:  nop
      .line 100001,100001 : 0,0 ''
      IL_004b:  stloc.2
      IL_004c:  ldloc.2
      IL_004d:  ldc.i4.0
      IL_004e:  bge.s      IL_0052

      IL_0050:  br.s       IL_0054

      IL_0052:  br.s       IL_0056

      .line 100001,100001 : 0,0 ''
      IL_0054:  ldloc.2
      IL_0055:  ret

      .line 100001,100001 : 0,0 ''
      IL_0056:  ldloc.2
      IL_0057:  ldc.i4.0
      IL_0058:  ble.s      IL_005c

      IL_005a:  br.s       IL_005e

      IL_005c:  br.s       IL_0060

      .line 100001,100001 : 0,0 ''
      IL_005e:  ldloc.2
      IL_005f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0060:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0065:  stloc.s    V_6
      IL_0067:  ldloc.0
      IL_0068:  ldfld      int32 TestFunction21/U::item2
      IL_006d:  stloc.s    V_7
      IL_006f:  ldloc.1
      IL_0070:  ldfld      int32 TestFunction21/U::item2
      IL_0075:  stloc.s    V_8
      IL_0077:  ldloc.s    V_7
      IL_0079:  ldloc.s    V_8
      IL_007b:  bge.s      IL_007f

      IL_007d:  br.s       IL_0081

      IL_007f:  br.s       IL_0083

      .line 100001,100001 : 0,0 ''
      IL_0081:  ldc.i4.m1
      IL_0082:  ret

      .line 100001,100001 : 0,0 ''
      IL_0083:  ldloc.s    V_7
      IL_0085:  ldloc.s    V_8
      IL_0087:  cgt
      IL_0089:  ret

      .line 100001,100001 : 0,0 ''
      IL_008a:  ldc.i4.1
      IL_008b:  ret

      .line 100001,100001 : 0,0 ''
      IL_008c:  ldarg.1
      IL_008d:  ldnull
      IL_008e:  cgt.un
      IL_0090:  brfalse.s  IL_0094

      IL_0092:  br.s       IL_0096

      IL_0094:  br.s       IL_0098

      .line 100001,100001 : 0,0 ''
      IL_0096:  ldc.i4.m1
      IL_0097:  ret

      .line 100001,100001 : 0,0 ''
      IL_0098:  ldc.i4.0
      IL_0099:  ret
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
      IL_0002:  unbox.any  TestFunction21/U
      IL_0007:  callvirt   instance int32 TestFunction21/U::CompareTo(class TestFunction21/U)
      IL_000c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       164 (0xa4)
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
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction21/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  br.s       IL_0014

      IL_000f:  br         IL_0091

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.1
      IL_0015:  unbox.any  TestFunction21/U
      IL_001a:  ldnull
      IL_001b:  cgt.un
      IL_001d:  brfalse.s  IL_0021

      IL_001f:  br.s       IL_0026

      IL_0021:  br         IL_008f

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
      IL_0030:  ldfld      int32 TestFunction21/U::item1
      IL_0035:  stloc.s    V_5
      IL_0037:  ldloc.2
      IL_0038:  ldfld      int32 TestFunction21/U::item1
      IL_003d:  stloc.s    V_6
      IL_003f:  ldloc.s    V_5
      IL_0041:  ldloc.s    V_6
      IL_0043:  bge.s      IL_0047

      IL_0045:  br.s       IL_0049

      IL_0047:  br.s       IL_004d

      .line 100001,100001 : 0,0 ''
      IL_0049:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_004a:  nop
      IL_004b:  br.s       IL_0054

      .line 100001,100001 : 0,0 ''
      IL_004d:  ldloc.s    V_5
      IL_004f:  ldloc.s    V_6
      IL_0051:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0053:  nop
      .line 100001,100001 : 0,0 ''
      IL_0054:  stloc.3
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.0
      IL_0057:  bge.s      IL_005b

      IL_0059:  br.s       IL_005d

      IL_005b:  br.s       IL_005f

      .line 100001,100001 : 0,0 ''
      IL_005d:  ldloc.3
      IL_005e:  ret

      .line 100001,100001 : 0,0 ''
      IL_005f:  ldloc.3
      IL_0060:  ldc.i4.0
      IL_0061:  ble.s      IL_0065

      IL_0063:  br.s       IL_0067

      IL_0065:  br.s       IL_0069

      .line 100001,100001 : 0,0 ''
      IL_0067:  ldloc.3
      IL_0068:  ret

      .line 100001,100001 : 0,0 ''
      IL_0069:  ldarg.2
      IL_006a:  stloc.s    V_7
      IL_006c:  ldloc.1
      IL_006d:  ldfld      int32 TestFunction21/U::item2
      IL_0072:  stloc.s    V_8
      IL_0074:  ldloc.2
      IL_0075:  ldfld      int32 TestFunction21/U::item2
      IL_007a:  stloc.s    V_9
      IL_007c:  ldloc.s    V_8
      IL_007e:  ldloc.s    V_9
      IL_0080:  bge.s      IL_0084

      IL_0082:  br.s       IL_0086

      IL_0084:  br.s       IL_0088

      .line 100001,100001 : 0,0 ''
      IL_0086:  ldc.i4.m1
      IL_0087:  ret

      .line 100001,100001 : 0,0 ''
      IL_0088:  ldloc.s    V_8
      IL_008a:  ldloc.s    V_9
      IL_008c:  cgt
      IL_008e:  ret

      .line 100001,100001 : 0,0 ''
      IL_008f:  ldc.i4.1
      IL_0090:  ret

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldarg.1
      IL_0092:  unbox.any  TestFunction21/U
      IL_0097:  ldnull
      IL_0098:  cgt.un
      IL_009a:  brfalse.s  IL_009e

      IL_009c:  br.s       IL_00a0

      IL_009e:  br.s       IL_00a2

      .line 100001,100001 : 0,0 ''
      IL_00a0:  ldc.i4.m1
      IL_00a1:  ret

      .line 100001,100001 : 0,0 ''
      IL_00a2:  ldc.i4.0
      IL_00a3:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       68 (0x44)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class TestFunction21/U V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0042

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
      IL_001a:  ldfld      int32 TestFunction21/U::item2
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.6
      IL_0021:  shl
      IL_0022:  ldloc.0
      IL_0023:  ldc.i4.2
      IL_0024:  shr
      IL_0025:  add
      IL_0026:  add
      IL_0027:  add
      IL_0028:  stloc.0
      IL_0029:  ldc.i4     0x9e3779b9
      IL_002e:  ldarg.1
      IL_002f:  stloc.3
      IL_0030:  ldloc.1
      IL_0031:  ldfld      int32 TestFunction21/U::item1
      IL_0036:  ldloc.0
      IL_0037:  ldc.i4.6
      IL_0038:  shl
      IL_0039:  ldloc.0
      IL_003a:  ldc.i4.2
      IL_003b:  shr
      IL_003c:  add
      IL_003d:  add
      IL_003e:  add
      IL_003f:  stloc.0
      IL_0040:  ldloc.0
      IL_0041:  ret

      .line 100001,100001 : 0,0 ''
      IL_0042:  ldc.i4.0
      IL_0043:  ret
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
      IL_0006:  callvirt   instance int32 TestFunction21/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       85 (0x55)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1,
               [2] class TestFunction21/U V_2,
               [3] class TestFunction21/U V_3,
               [4] class [mscorlib]System.Collections.IEqualityComparer V_4,
               [5] class [mscorlib]System.Collections.IEqualityComparer V_5)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_004d

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  isinst     TestFunction21/U
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_0018

      IL_0016:  br.s       IL_004b

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
      IL_0024:  ldfld      int32 TestFunction21/U::item1
      IL_0029:  ldloc.3
      IL_002a:  ldfld      int32 TestFunction21/U::item1
      IL_002f:  ceq
      IL_0031:  brfalse.s  IL_0035

      IL_0033:  br.s       IL_0037

      IL_0035:  br.s       IL_0049

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldarg.2
      IL_0038:  stloc.s    V_5
      IL_003a:  ldloc.2
      IL_003b:  ldfld      int32 TestFunction21/U::item2
      IL_0040:  ldloc.3
      IL_0041:  ldfld      int32 TestFunction21/U::item2
      IL_0046:  ceq
      IL_0048:  ret

      .line 100001,100001 : 0,0 ''
      IL_0049:  ldc.i4.0
      IL_004a:  ret

      .line 100001,100001 : 0,0 ''
      IL_004b:  ldc.i4.0
      IL_004c:  ret

      .line 100001,100001 : 0,0 ''
      IL_004d:  ldarg.1
      IL_004e:  ldnull
      IL_004f:  cgt.un
      IL_0051:  ldc.i4.0
      IL_0052:  ceq
      IL_0054:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       71 (0x47)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1)
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
      IL_001b:  ldfld      int32 TestFunction21/U::item1
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 TestFunction21/U::item1
      IL_0026:  bne.un.s   IL_002a

      IL_0028:  br.s       IL_002c

      IL_002a:  br.s       IL_003b

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.0
      IL_002d:  ldfld      int32 TestFunction21/U::item2
      IL_0032:  ldloc.1
      IL_0033:  ldfld      int32 TestFunction21/U::item2
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
      .locals init ([0] class TestFunction21/U V_0)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction21/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance bool TestFunction21/U::Equals(class TestFunction21/U)
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
      .line 7,7 : 5,29 ''
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
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
      .line 7,7 : 5,29 ''
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
    .line 7,7 : 5,29 ''
    IL_0010:  ldstr      "a = %A, a = %A"
    IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Tuple`2<int32,int32>>::.ctor(string)
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
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
