
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction21
{
  // Offset: 0x00000000 Length: 0x000006AC
  // WARNING: managed resource file FSharpSignatureData.TestFunction21 created
}
.mresource public FSharpOptimizationData.TestFunction21
{
  // Offset: 0x000006B0 Length: 0x000001CD
  // WARNING: managed resource file FSharpOptimizationData.TestFunction21 created
}
.module TestFunction21.exe
// MVID: {624E339D-A643-45E6-A745-03839D334E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03000000


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
      // Code size       140 (0x8c)
      .maxstack  4
      .locals init (class TestFunction21/U V_0,
               class TestFunction21/U V_1,
               int32 V_2,
               class [mscorlib]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [mscorlib]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8,
               class [mscorlib]System.Collections.IComparer V_9,
               int32 V_10,
               int32 V_11,
               class [mscorlib]System.Collections.IComparer V_12,
               int32 V_13,
               int32 V_14)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0085

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_0083

      IL_000c:  ldarg.0
      IL_000d:  pop
      IL_000e:  ldarg.0
      IL_000f:  stloc.0
      IL_0010:  ldarg.1
      IL_0011:  stloc.1
      IL_0012:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0017:  stloc.3
      IL_0018:  ldloc.0
      IL_0019:  ldfld      int32 TestFunction21/U::item1
      IL_001e:  stloc.s    V_4
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 TestFunction21/U::item1
      IL_0026:  stloc.s    V_5
      IL_0028:  ldloc.3
      IL_0029:  stloc.s    V_6
      IL_002b:  ldloc.s    V_4
      IL_002d:  stloc.s    V_7
      IL_002f:  ldloc.s    V_5
      IL_0031:  stloc.s    V_8
      IL_0033:  ldloc.s    V_7
      IL_0035:  ldloc.s    V_8
      IL_0037:  bge.s      IL_003d

      IL_0039:  ldc.i4.m1
      IL_003a:  nop
      IL_003b:  br.s       IL_0044

      IL_003d:  ldloc.s    V_7
      IL_003f:  ldloc.s    V_8
      IL_0041:  cgt
      IL_0043:  nop
      IL_0044:  stloc.2
      IL_0045:  ldloc.2
      IL_0046:  ldc.i4.0
      IL_0047:  bge.s      IL_004b

      IL_0049:  ldloc.2
      IL_004a:  ret

      IL_004b:  ldloc.2
      IL_004c:  ldc.i4.0
      IL_004d:  ble.s      IL_0051

      IL_004f:  ldloc.2
      IL_0050:  ret

      IL_0051:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0056:  stloc.s    V_9
      IL_0058:  ldloc.0
      IL_0059:  ldfld      int32 TestFunction21/U::item2
      IL_005e:  stloc.s    V_10
      IL_0060:  ldloc.1
      IL_0061:  ldfld      int32 TestFunction21/U::item2
      IL_0066:  stloc.s    V_11
      IL_0068:  ldloc.s    V_9
      IL_006a:  stloc.s    V_12
      IL_006c:  ldloc.s    V_10
      IL_006e:  stloc.s    V_13
      IL_0070:  ldloc.s    V_11
      IL_0072:  stloc.s    V_14
      IL_0074:  ldloc.s    V_13
      IL_0076:  ldloc.s    V_14
      IL_0078:  bge.s      IL_007c

      IL_007a:  ldc.i4.m1
      IL_007b:  ret

      IL_007c:  ldloc.s    V_13
      IL_007e:  ldloc.s    V_14
      IL_0080:  cgt
      IL_0082:  ret

      IL_0083:  ldc.i4.1
      IL_0084:  ret

      IL_0085:  ldarg.1
      IL_0086:  brfalse.s  IL_008a

      IL_0088:  ldc.i4.m1
      IL_0089:  ret

      IL_008a:  ldc.i4.0
      IL_008b:  ret
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
      // Code size       151 (0x97)
      .maxstack  4
      .locals init (class TestFunction21/U V_0,
               class TestFunction21/U V_1,
               class TestFunction21/U V_2,
               int32 V_3,
               class [mscorlib]System.Collections.IComparer V_4,
               int32 V_5,
               int32 V_6,
               class [mscorlib]System.Collections.IComparer V_7,
               int32 V_8,
               int32 V_9,
               class [mscorlib]System.Collections.IComparer V_10,
               int32 V_11,
               int32 V_12,
               class [mscorlib]System.Collections.IComparer V_13,
               int32 V_14,
               int32 V_15)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction21/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse    IL_008b

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  TestFunction21/U
      IL_0013:  brfalse    IL_0089

      IL_0018:  ldarg.0
      IL_0019:  pop
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
      IL_0031:  ldloc.s    V_4
      IL_0033:  stloc.s    V_7
      IL_0035:  ldloc.s    V_5
      IL_0037:  stloc.s    V_8
      IL_0039:  ldloc.s    V_6
      IL_003b:  stloc.s    V_9
      IL_003d:  ldloc.s    V_8
      IL_003f:  ldloc.s    V_9
      IL_0041:  bge.s      IL_0047

      IL_0043:  ldc.i4.m1
      IL_0044:  nop
      IL_0045:  br.s       IL_004e

      IL_0047:  ldloc.s    V_8
      IL_0049:  ldloc.s    V_9
      IL_004b:  cgt
      IL_004d:  nop
      IL_004e:  stloc.3
      IL_004f:  ldloc.3
      IL_0050:  ldc.i4.0
      IL_0051:  bge.s      IL_0055

      IL_0053:  ldloc.3
      IL_0054:  ret

      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.0
      IL_0057:  ble.s      IL_005b

      IL_0059:  ldloc.3
      IL_005a:  ret

      IL_005b:  ldarg.2
      IL_005c:  stloc.s    V_10
      IL_005e:  ldloc.1
      IL_005f:  ldfld      int32 TestFunction21/U::item2
      IL_0064:  stloc.s    V_11
      IL_0066:  ldloc.2
      IL_0067:  ldfld      int32 TestFunction21/U::item2
      IL_006c:  stloc.s    V_12
      IL_006e:  ldloc.s    V_10
      IL_0070:  stloc.s    V_13
      IL_0072:  ldloc.s    V_11
      IL_0074:  stloc.s    V_14
      IL_0076:  ldloc.s    V_12
      IL_0078:  stloc.s    V_15
      IL_007a:  ldloc.s    V_14
      IL_007c:  ldloc.s    V_15
      IL_007e:  bge.s      IL_0082

      IL_0080:  ldc.i4.m1
      IL_0081:  ret

      IL_0082:  ldloc.s    V_14
      IL_0084:  ldloc.s    V_15
      IL_0086:  cgt
      IL_0088:  ret

      IL_0089:  ldc.i4.1
      IL_008a:  ret

      IL_008b:  ldarg.1
      IL_008c:  unbox.any  TestFunction21/U
      IL_0091:  brfalse.s  IL_0095

      IL_0093:  ldc.i4.m1
      IL_0094:  ret

      IL_0095:  ldc.i4.0
      IL_0096:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       75 (0x4b)
      .maxstack  7
      .locals init (int32 V_0,
               class TestFunction21/U V_1,
               class [mscorlib]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4,
               class [mscorlib]System.Collections.IEqualityComparer V_5,
               int32 V_6,
               class [mscorlib]System.Collections.IEqualityComparer V_7)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0049

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  pop
      IL_0007:  ldarg.0
      IL_0008:  stloc.1
      IL_0009:  ldc.i4.0
      IL_000a:  stloc.0
      IL_000b:  ldc.i4     0x9e3779b9
      IL_0010:  ldarg.1
      IL_0011:  stloc.2
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 TestFunction21/U::item2
      IL_0018:  stloc.3
      IL_0019:  ldloc.2
      IL_001a:  stloc.s    V_4
      IL_001c:  ldloc.3
      IL_001d:  ldloc.0
      IL_001e:  ldc.i4.6
      IL_001f:  shl
      IL_0020:  ldloc.0
      IL_0021:  ldc.i4.2
      IL_0022:  shr
      IL_0023:  add
      IL_0024:  add
      IL_0025:  add
      IL_0026:  stloc.0
      IL_0027:  ldc.i4     0x9e3779b9
      IL_002c:  ldarg.1
      IL_002d:  stloc.s    V_5
      IL_002f:  ldloc.1
      IL_0030:  ldfld      int32 TestFunction21/U::item1
      IL_0035:  stloc.s    V_6
      IL_0037:  ldloc.s    V_5
      IL_0039:  stloc.s    V_7
      IL_003b:  ldloc.s    V_6
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.6
      IL_003f:  shl
      IL_0040:  ldloc.0
      IL_0041:  ldc.i4.2
      IL_0042:  shr
      IL_0043:  add
      IL_0044:  add
      IL_0045:  add
      IL_0046:  stloc.0
      IL_0047:  ldloc.0
      IL_0048:  ret

      IL_0049:  ldc.i4.0
      IL_004a:  ret
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
      // Code size       94 (0x5e)
      .maxstack  4
      .locals init (class TestFunction21/U V_0,
               class TestFunction21/U V_1,
               class TestFunction21/U V_2,
               class TestFunction21/U V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4,
               int32 V_5,
               int32 V_6,
               class [mscorlib]System.Collections.IEqualityComparer V_7,
               class [mscorlib]System.Collections.IEqualityComparer V_8,
               int32 V_9,
               int32 V_10,
               class [mscorlib]System.Collections.IEqualityComparer V_11)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0056

      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction21/U
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0054

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.0
      IL_0010:  pop
      IL_0011:  ldarg.0
      IL_0012:  stloc.2
      IL_0013:  ldloc.1
      IL_0014:  stloc.3
      IL_0015:  ldarg.2
      IL_0016:  stloc.s    V_4
      IL_0018:  ldloc.2
      IL_0019:  ldfld      int32 TestFunction21/U::item1
      IL_001e:  stloc.s    V_5
      IL_0020:  ldloc.3
      IL_0021:  ldfld      int32 TestFunction21/U::item1
      IL_0026:  stloc.s    V_6
      IL_0028:  ldloc.s    V_4
      IL_002a:  stloc.s    V_7
      IL_002c:  ldloc.s    V_5
      IL_002e:  ldloc.s    V_6
      IL_0030:  ceq
      IL_0032:  brfalse.s  IL_0052

      IL_0034:  ldarg.2
      IL_0035:  stloc.s    V_8
      IL_0037:  ldloc.2
      IL_0038:  ldfld      int32 TestFunction21/U::item2
      IL_003d:  stloc.s    V_9
      IL_003f:  ldloc.3
      IL_0040:  ldfld      int32 TestFunction21/U::item2
      IL_0045:  stloc.s    V_10
      IL_0047:  ldloc.s    V_8
      IL_0049:  stloc.s    V_11
      IL_004b:  ldloc.s    V_9
      IL_004d:  ldloc.s    V_10
      IL_004f:  ceq
      IL_0051:  ret

      IL_0052:  ldc.i4.0
      IL_0053:  ret

      IL_0054:  ldc.i4.0
      IL_0055:  ret

      IL_0056:  ldarg.1
      IL_0057:  ldnull
      IL_0058:  cgt.un
      IL_005a:  ldc.i4.0
      IL_005b:  ceq
      IL_005d:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction21/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       53 (0x35)
      .maxstack  4
      .locals init (class TestFunction21/U V_0,
               class TestFunction21/U V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002d

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002b

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldloc.0
      IL_000d:  ldfld      int32 TestFunction21/U::item1
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 TestFunction21/U::item1
      IL_0018:  bne.un.s   IL_0029

      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 TestFunction21/U::item2
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 TestFunction21/U::item2
      IL_0026:  ceq
      IL_0028:  ret

      IL_0029:  ldc.i4.0
      IL_002a:  ret

      IL_002b:  ldc.i4.0
      IL_002c:  ret

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
      .locals init (class TestFunction21/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction21/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction21/U::Equals(class TestFunction21/U)
      IL_0011:  ret

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
    .locals init (class TestFunction21/U V_0,
             int32 V_1,
             int32 V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldfld      int32 TestFunction21/U::item2
    IL_0008:  stloc.1
    IL_0009:  ldloc.0
    IL_000a:  ldfld      int32 TestFunction21/U::item1
    IL_000f:  stloc.2
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
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\TestFunctions\TestFunction21_fs\TestFunction21.res
