
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
  .ver 5:0:0:0
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
// MVID: {611C52B3-A643-45E6-A745-0383B3521C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06DD0000


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
      // Code size       121 (0x79)
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
      .line 4,4 : 6,7 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction21.fs'
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_006f

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_006d

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.0
      IL_000e:  pop
      .line 100001,100001 : 0,0 ''
      IL_000f:  ldarg.0
      IL_0010:  stloc.0
      IL_0011:  ldarg.1
      IL_0012:  stloc.1
      IL_0013:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0018:  stloc.3
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 TestFunction21/U::item1
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.1
      IL_0022:  ldfld      int32 TestFunction21/U::item1
      IL_0027:  stloc.s    V_5
      .line 100001,100001 : 0,0 ''
      IL_0029:  ldloc.s    V_4
      IL_002b:  ldloc.s    V_5
      IL_002d:  bge.s      IL_0033

      .line 100001,100001 : 0,0 ''
      IL_002f:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0030:  nop
      IL_0031:  br.s       IL_003a

      .line 100001,100001 : 0,0 ''
      IL_0033:  ldloc.s    V_4
      IL_0035:  ldloc.s    V_5
      IL_0037:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0039:  nop
      .line 100001,100001 : 0,0 ''
      IL_003a:  stloc.2
      .line 100001,100001 : 0,0 ''
      IL_003b:  ldloc.2
      IL_003c:  ldc.i4.0
      IL_003d:  bge.s      IL_0041

      .line 100001,100001 : 0,0 ''
      IL_003f:  ldloc.2
      IL_0040:  ret

      .line 100001,100001 : 0,0 ''
      IL_0041:  ldloc.2
      IL_0042:  ldc.i4.0
      IL_0043:  ble.s      IL_0047

      .line 100001,100001 : 0,0 ''
      IL_0045:  ldloc.2
      IL_0046:  ret

      .line 100001,100001 : 0,0 ''
      IL_0047:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_004c:  stloc.s    V_6
      IL_004e:  ldloc.0
      IL_004f:  ldfld      int32 TestFunction21/U::item2
      IL_0054:  stloc.s    V_7
      IL_0056:  ldloc.1
      IL_0057:  ldfld      int32 TestFunction21/U::item2
      IL_005c:  stloc.s    V_8
      .line 100001,100001 : 0,0 ''
      IL_005e:  ldloc.s    V_7
      IL_0060:  ldloc.s    V_8
      IL_0062:  bge.s      IL_0066

      .line 100001,100001 : 0,0 ''
      IL_0064:  ldc.i4.m1
      IL_0065:  ret

      .line 100001,100001 : 0,0 ''
      IL_0066:  ldloc.s    V_7
      IL_0068:  ldloc.s    V_8
      IL_006a:  cgt
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  ldc.i4.1
      IL_006e:  ret

      .line 100001,100001 : 0,0 ''
      IL_006f:  ldarg.1
      IL_0070:  ldnull
      IL_0071:  cgt.un
      IL_0073:  brfalse.s  IL_0077

      .line 100001,100001 : 0,0 ''
      IL_0075:  ldc.i4.m1
      IL_0076:  ret

      .line 100001,100001 : 0,0 ''
      IL_0077:  ldc.i4.0
      IL_0078:  ret
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
      // Code size       130 (0x82)
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
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_0073

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  unbox.any  TestFunction21/U
      IL_0013:  ldnull
      IL_0014:  cgt.un
      IL_0016:  brfalse.s  IL_0071

      .line 100001,100001 : 0,0 ''
      IL_0018:  ldarg.0
      IL_0019:  pop
      .line 100001,100001 : 0,0 ''
      IL_001a:  ldarg.0
      IL_001b:  stloc.1
      IL_001c:  ldloc.0
      IL_001d:  stloc.2
      IL_001e:  ldarg.2
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.1
      IL_0022:  ldfld      int32 TestFunction21/U::item1
      IL_0027:  stloc.s    V_5
      IL_0029:  ldloc.2
      IL_002a:  ldfld      int32 TestFunction21/U::item1
      IL_002f:  stloc.s    V_6
      .line 100001,100001 : 0,0 ''
      IL_0031:  ldloc.s    V_5
      IL_0033:  ldloc.s    V_6
      IL_0035:  bge.s      IL_003b

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0038:  nop
      IL_0039:  br.s       IL_0042

      .line 100001,100001 : 0,0 ''
      IL_003b:  ldloc.s    V_5
      IL_003d:  ldloc.s    V_6
      IL_003f:  cgt
      .line 100001,100001 : 0,0 ''
      IL_0041:  nop
      .line 100001,100001 : 0,0 ''
      IL_0042:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_0043:  ldloc.3
      IL_0044:  ldc.i4.0
      IL_0045:  bge.s      IL_0049

      .line 100001,100001 : 0,0 ''
      IL_0047:  ldloc.3
      IL_0048:  ret

      .line 100001,100001 : 0,0 ''
      IL_0049:  ldloc.3
      IL_004a:  ldc.i4.0
      IL_004b:  ble.s      IL_004f

      .line 100001,100001 : 0,0 ''
      IL_004d:  ldloc.3
      IL_004e:  ret

      .line 100001,100001 : 0,0 ''
      IL_004f:  ldarg.2
      IL_0050:  stloc.s    V_7
      IL_0052:  ldloc.1
      IL_0053:  ldfld      int32 TestFunction21/U::item2
      IL_0058:  stloc.s    V_8
      IL_005a:  ldloc.2
      IL_005b:  ldfld      int32 TestFunction21/U::item2
      IL_0060:  stloc.s    V_9
      .line 100001,100001 : 0,0 ''
      IL_0062:  ldloc.s    V_8
      IL_0064:  ldloc.s    V_9
      IL_0066:  bge.s      IL_006a

      .line 100001,100001 : 0,0 ''
      IL_0068:  ldc.i4.m1
      IL_0069:  ret

      .line 100001,100001 : 0,0 ''
      IL_006a:  ldloc.s    V_8
      IL_006c:  ldloc.s    V_9
      IL_006e:  cgt
      IL_0070:  ret

      .line 100001,100001 : 0,0 ''
      IL_0071:  ldc.i4.1
      IL_0072:  ret

      .line 100001,100001 : 0,0 ''
      IL_0073:  ldarg.1
      IL_0074:  unbox.any  TestFunction21/U
      IL_0079:  ldnull
      IL_007a:  cgt.un
      IL_007c:  brfalse.s  IL_0080

      .line 100001,100001 : 0,0 ''
      IL_007e:  ldc.i4.m1
      IL_007f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0080:  ldc.i4.0
      IL_0081:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       65 (0x41)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class TestFunction21/U V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_003f

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldc.i4.0
      IL_0008:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0009:  ldarg.0
      IL_000a:  pop
      .line 100001,100001 : 0,0 ''
      IL_000b:  ldarg.0
      IL_000c:  stloc.1
      IL_000d:  ldc.i4.0
      IL_000e:  stloc.0
      IL_000f:  ldc.i4     0x9e3779b9
      IL_0014:  ldarg.1
      IL_0015:  stloc.2
      IL_0016:  ldloc.1
      IL_0017:  ldfld      int32 TestFunction21/U::item2
      IL_001c:  ldloc.0
      IL_001d:  ldc.i4.6
      IL_001e:  shl
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.2
      IL_0021:  shr
      IL_0022:  add
      IL_0023:  add
      IL_0024:  add
      IL_0025:  stloc.0
      IL_0026:  ldc.i4     0x9e3779b9
      IL_002b:  ldarg.1
      IL_002c:  stloc.3
      IL_002d:  ldloc.1
      IL_002e:  ldfld      int32 TestFunction21/U::item1
      IL_0033:  ldloc.0
      IL_0034:  ldc.i4.6
      IL_0035:  shl
      IL_0036:  ldloc.0
      IL_0037:  ldc.i4.2
      IL_0038:  shr
      IL_0039:  add
      IL_003a:  add
      IL_003b:  add
      IL_003c:  stloc.0
      IL_003d:  ldloc.0
      IL_003e:  ret

      .line 100001,100001 : 0,0 ''
      IL_003f:  ldc.i4.0
      IL_0040:  ret
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
      // Code size       74 (0x4a)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1,
               [2] class TestFunction21/U V_2,
               [3] class TestFunction21/U V_3,
               [4] class [mscorlib]System.Collections.IEqualityComparer V_4,
               [5] class [mscorlib]System.Collections.IEqualityComparer V_5)
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0042

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  isinst     TestFunction21/U
      IL_000d:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_000e:  ldloc.0
      IL_000f:  brfalse.s  IL_0040

      .line 100001,100001 : 0,0 ''
      IL_0011:  ldloc.0
      IL_0012:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_0013:  ldarg.0
      IL_0014:  pop
      .line 100001,100001 : 0,0 ''
      IL_0015:  ldarg.0
      IL_0016:  stloc.2
      IL_0017:  ldloc.1
      IL_0018:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_0019:  ldarg.2
      IL_001a:  stloc.s    V_4
      IL_001c:  ldloc.2
      IL_001d:  ldfld      int32 TestFunction21/U::item1
      IL_0022:  ldloc.3
      IL_0023:  ldfld      int32 TestFunction21/U::item1
      IL_0028:  ceq
      IL_002a:  brfalse.s  IL_003e

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldarg.2
      IL_002d:  stloc.s    V_5
      IL_002f:  ldloc.2
      IL_0030:  ldfld      int32 TestFunction21/U::item2
      IL_0035:  ldloc.3
      IL_0036:  ldfld      int32 TestFunction21/U::item2
      IL_003b:  ceq
      IL_003d:  ret

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldc.i4.0
      IL_003f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0040:  ldc.i4.0
      IL_0041:  ret

      .line 100001,100001 : 0,0 ''
      IL_0042:  ldarg.1
      IL_0043:  ldnull
      IL_0044:  cgt.un
      IL_0046:  ldc.i4.0
      IL_0047:  ceq
      IL_0049:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       60 (0x3c)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0,
               [1] class TestFunction21/U V_1)
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0034

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_0032

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.0
      IL_000e:  pop
      .line 100001,100001 : 0,0 ''
      IL_000f:  ldarg.0
      IL_0010:  stloc.0
      IL_0011:  ldarg.1
      IL_0012:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_0013:  ldloc.0
      IL_0014:  ldfld      int32 TestFunction21/U::item1
      IL_0019:  ldloc.1
      IL_001a:  ldfld      int32 TestFunction21/U::item1
      IL_001f:  bne.un.s   IL_0030

      .line 100001,100001 : 0,0 ''
      IL_0021:  ldloc.0
      IL_0022:  ldfld      int32 TestFunction21/U::item2
      IL_0027:  ldloc.1
      IL_0028:  ldfld      int32 TestFunction21/U::item2
      IL_002d:  ceq
      IL_002f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0030:  ldc.i4.0
      IL_0031:  ret

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldc.i4.0
      IL_0033:  ret

      .line 100001,100001 : 0,0 ''
      IL_0034:  ldarg.1
      IL_0035:  ldnull
      IL_0036:  cgt.un
      IL_0038:  ldc.i4.0
      IL_0039:  ceq
      IL_003b:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init ([0] class TestFunction21/U V_0)
      .line 4,4 : 6,7 ''
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
