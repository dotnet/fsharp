
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly TestFunction24
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction24
{
  // Offset: 0x00000000 Length: 0x0000076F
  // WARNING: managed resource file FSharpSignatureData.TestFunction24 created
}
.mresource public FSharpOptimizationData.TestFunction24
{
  // Offset: 0x00000778 Length: 0x0000022E
  // WARNING: managed resource file FSharpOptimizationData.TestFunction24 created
}
.module TestFunction24.exe
// MVID: {628F4C90-2576-C1EA-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000018C43700000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction24
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public Point
         extends [System.Runtime]System.Object
         implements class [System.Runtime]System.IEquatable`1<class TestFunction24/Point>,
                    [System.Runtime]System.Collections.IStructuralEquatable,
                    class [System.Runtime]System.IComparable`1<class TestFunction24/Point>,
                    [System.Runtime]System.IComparable,
                    [System.Runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field public int32 x@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field public int32 y@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction24/Point::x@
      IL_0006:  ret
    } // end of method Point::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction24/Point::y@
      IL_0006:  ret
    } // end of method Point::get_y

    .method public hidebysig specialname 
            instance void  set_x(int32 'value') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 TestFunction24/Point::x@
      IL_0007:  ret
    } // end of method Point::set_x

    .method public hidebysig specialname 
            instance void  set_y(int32 'value') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 TestFunction24/Point::y@
      IL_0007:  ret
    } // end of method Point::set_y

    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 int32 y) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 TestFunction24/Point::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction24/Point::y@
      IL_0014:  ret
    } // end of method Point::.ctor

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction24/Point,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction24/Point>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction24/Point,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction24/Point,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Point::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction24/Point obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       119 (0x77)
      .maxstack  5
      .locals init (int32 V_0,
               class [System.Runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3,
               class [System.Runtime]System.Collections.IComparer V_4,
               int32 V_5,
               int32 V_6,
               class [System.Runtime]System.Collections.IComparer V_7,
               int32 V_8,
               int32 V_9,
               class [System.Runtime]System.Collections.IComparer V_10,
               int32 V_11,
               int32 V_12)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0070

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_006e

      IL_0006:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction24/Point::x@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction24/Point::x@
      IL_0019:  stloc.3
      IL_001a:  ldloc.1
      IL_001b:  stloc.s    V_4
      IL_001d:  ldloc.2
      IL_001e:  stloc.s    V_5
      IL_0020:  ldloc.3
      IL_0021:  stloc.s    V_6
      IL_0023:  ldloc.s    V_5
      IL_0025:  ldloc.s    V_6
      IL_0027:  cgt
      IL_0029:  ldloc.s    V_5
      IL_002b:  ldloc.s    V_6
      IL_002d:  clt
      IL_002f:  sub
      IL_0030:  stloc.0
      IL_0031:  ldloc.0
      IL_0032:  ldc.i4.0
      IL_0033:  bge.s      IL_0037

      IL_0035:  ldloc.0
      IL_0036:  ret

      IL_0037:  ldloc.0
      IL_0038:  ldc.i4.0
      IL_0039:  ble.s      IL_003d

      IL_003b:  ldloc.0
      IL_003c:  ret

      IL_003d:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0042:  stloc.s    V_7
      IL_0044:  ldarg.0
      IL_0045:  ldfld      int32 TestFunction24/Point::y@
      IL_004a:  stloc.s    V_8
      IL_004c:  ldarg.1
      IL_004d:  ldfld      int32 TestFunction24/Point::y@
      IL_0052:  stloc.s    V_9
      IL_0054:  ldloc.s    V_7
      IL_0056:  stloc.s    V_10
      IL_0058:  ldloc.s    V_8
      IL_005a:  stloc.s    V_11
      IL_005c:  ldloc.s    V_9
      IL_005e:  stloc.s    V_12
      IL_0060:  ldloc.s    V_11
      IL_0062:  ldloc.s    V_12
      IL_0064:  cgt
      IL_0066:  ldloc.s    V_11
      IL_0068:  ldloc.s    V_12
      IL_006a:  clt
      IL_006c:  sub
      IL_006d:  ret

      IL_006e:  ldc.i4.1
      IL_006f:  ret

      IL_0070:  ldarg.1
      IL_0071:  brfalse.s  IL_0075

      IL_0073:  ldc.i4.m1
      IL_0074:  ret

      IL_0075:  ldc.i4.0
      IL_0076:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction24/Point
      IL_0007:  callvirt   instance int32 TestFunction24/Point::CompareTo(class TestFunction24/Point)
      IL_000c:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [System.Runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       134 (0x86)
      .maxstack  5
      .locals init (class TestFunction24/Point V_0,
               class TestFunction24/Point V_1,
               int32 V_2,
               class [System.Runtime]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [System.Runtime]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8,
               class [System.Runtime]System.Collections.IComparer V_9,
               int32 V_10,
               int32 V_11,
               class [System.Runtime]System.Collections.IComparer V_12,
               int32 V_13,
               int32 V_14)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction24/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  brfalse.s  IL_007a

      IL_000c:  ldarg.1
      IL_000d:  unbox.any  TestFunction24/Point
      IL_0012:  brfalse.s  IL_0078

      IL_0014:  ldarg.2
      IL_0015:  stloc.3
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 TestFunction24/Point::x@
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.1
      IL_001f:  ldfld      int32 TestFunction24/Point::x@
      IL_0024:  stloc.s    V_5
      IL_0026:  ldloc.3
      IL_0027:  stloc.s    V_6
      IL_0029:  ldloc.s    V_4
      IL_002b:  stloc.s    V_7
      IL_002d:  ldloc.s    V_5
      IL_002f:  stloc.s    V_8
      IL_0031:  ldloc.s    V_7
      IL_0033:  ldloc.s    V_8
      IL_0035:  cgt
      IL_0037:  ldloc.s    V_7
      IL_0039:  ldloc.s    V_8
      IL_003b:  clt
      IL_003d:  sub
      IL_003e:  stloc.2
      IL_003f:  ldloc.2
      IL_0040:  ldc.i4.0
      IL_0041:  bge.s      IL_0045

      IL_0043:  ldloc.2
      IL_0044:  ret

      IL_0045:  ldloc.2
      IL_0046:  ldc.i4.0
      IL_0047:  ble.s      IL_004b

      IL_0049:  ldloc.2
      IL_004a:  ret

      IL_004b:  ldarg.2
      IL_004c:  stloc.s    V_9
      IL_004e:  ldarg.0
      IL_004f:  ldfld      int32 TestFunction24/Point::y@
      IL_0054:  stloc.s    V_10
      IL_0056:  ldloc.1
      IL_0057:  ldfld      int32 TestFunction24/Point::y@
      IL_005c:  stloc.s    V_11
      IL_005e:  ldloc.s    V_9
      IL_0060:  stloc.s    V_12
      IL_0062:  ldloc.s    V_10
      IL_0064:  stloc.s    V_13
      IL_0066:  ldloc.s    V_11
      IL_0068:  stloc.s    V_14
      IL_006a:  ldloc.s    V_13
      IL_006c:  ldloc.s    V_14
      IL_006e:  cgt
      IL_0070:  ldloc.s    V_13
      IL_0072:  ldloc.s    V_14
      IL_0074:  clt
      IL_0076:  sub
      IL_0077:  ret

      IL_0078:  ldc.i4.1
      IL_0079:  ret

      IL_007a:  ldarg.1
      IL_007b:  unbox.any  TestFunction24/Point
      IL_0080:  brfalse.s  IL_0084

      IL_0082:  ldc.i4.m1
      IL_0083:  ret

      IL_0084:  ldc.i4.0
      IL_0085:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       68 (0x44)
      .maxstack  7
      .locals init (int32 V_0,
               class [System.Runtime]System.Collections.IEqualityComparer V_1,
               int32 V_2,
               class [System.Runtime]System.Collections.IEqualityComparer V_3,
               class [System.Runtime]System.Collections.IEqualityComparer V_4,
               int32 V_5,
               class [System.Runtime]System.Collections.IEqualityComparer V_6)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0042

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction24/Point::y@
      IL_0012:  stloc.2
      IL_0013:  ldloc.1
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.6
      IL_0018:  shl
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.2
      IL_001b:  shr
      IL_001c:  add
      IL_001d:  add
      IL_001e:  add
      IL_001f:  stloc.0
      IL_0020:  ldc.i4     0x9e3779b9
      IL_0025:  ldarg.1
      IL_0026:  stloc.s    V_4
      IL_0028:  ldarg.0
      IL_0029:  ldfld      int32 TestFunction24/Point::x@
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_4
      IL_0032:  stloc.s    V_6
      IL_0034:  ldloc.s    V_5
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

      IL_0042:  ldc.i4.0
      IL_0043:  ret
    } // end of method Point::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction24/Point::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Point::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       84 (0x54)
      .maxstack  4
      .locals init (class TestFunction24/Point V_0,
               class TestFunction24/Point V_1,
               class [System.Runtime]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               int32 V_4,
               class [System.Runtime]System.Collections.IEqualityComparer V_5,
               class [System.Runtime]System.Collections.IEqualityComparer V_6,
               int32 V_7,
               int32 V_8,
               class [System.Runtime]System.Collections.IEqualityComparer V_9)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_004c

      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction24/Point
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_004a

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.2
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 TestFunction24/Point::x@
      IL_0017:  stloc.3
      IL_0018:  ldloc.1
      IL_0019:  ldfld      int32 TestFunction24/Point::x@
      IL_001e:  stloc.s    V_4
      IL_0020:  ldloc.2
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloc.3
      IL_0024:  ldloc.s    V_4
      IL_0026:  ceq
      IL_0028:  brfalse.s  IL_0048

      IL_002a:  ldarg.2
      IL_002b:  stloc.s    V_6
      IL_002d:  ldarg.0
      IL_002e:  ldfld      int32 TestFunction24/Point::y@
      IL_0033:  stloc.s    V_7
      IL_0035:  ldloc.1
      IL_0036:  ldfld      int32 TestFunction24/Point::y@
      IL_003b:  stloc.s    V_8
      IL_003d:  ldloc.s    V_6
      IL_003f:  stloc.s    V_9
      IL_0041:  ldloc.s    V_7
      IL_0043:  ldloc.s    V_8
      IL_0045:  ceq
      IL_0047:  ret

      IL_0048:  ldc.i4.0
      IL_0049:  ret

      IL_004a:  ldc.i4.0
      IL_004b:  ret

      IL_004c:  ldarg.1
      IL_004d:  ldnull
      IL_004e:  cgt.un
      IL_0050:  ldc.i4.0
      IL_0051:  ceq
      IL_0053:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction24/Point obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       47 (0x2f)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 TestFunction24/Point::x@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 TestFunction24/Point::x@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 TestFunction24/Point::y@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction24/Point::y@
      IL_0020:  ceq
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret

      IL_0027:  ldarg.1
      IL_0028:  ldnull
      IL_0029:  cgt.un
      IL_002b:  ldc.i4.0
      IL_002c:  ceq
      IL_002e:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class TestFunction24/Point V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction24/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction24/Point::Equals(class TestFunction24/Point)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Point::Equals

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .set instance void TestFunction24/Point::set_x(int32)
      .get instance int32 TestFunction24/Point::get_x()
    } // end of property Point::x
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .set instance void TestFunction24/Point::set_y(int32)
      .get instance int32 TestFunction24/Point::get_y()
    } // end of property Point::y
  } // end of class Point

  .method public static int32  pinObject() cil managed
  {
    // Code size       82 (0x52)
    .maxstack  6
    .locals init (class TestFunction24/Point V_0,
             native int V_1,
             int32& pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6,
             native int V_7,
             int32 V_8,
             native int V_9,
             int32 V_10)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void TestFunction24/Point::.ctor(int32,
                                                                   int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldflda     int32 TestFunction24/Point::x@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  stloc.3
    IL_0014:  ldc.i4.0
    IL_0015:  stloc.s    V_4
    IL_0017:  ldloc.3
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_4
    IL_001c:  stloc.s    V_6
    IL_001e:  ldloc.s    V_5
    IL_0020:  ldloc.s    V_6
    IL_0022:  conv.i
    IL_0023:  sizeof     [System.Runtime]System.Int32
    IL_0029:  mul
    IL_002a:  add
    IL_002b:  ldobj      [System.Runtime]System.Int32
    IL_0030:  ldloc.1
    IL_0031:  stloc.s    V_7
    IL_0033:  ldc.i4.1
    IL_0034:  stloc.s    V_8
    IL_0036:  ldloc.s    V_7
    IL_0038:  stloc.s    V_9
    IL_003a:  ldloc.s    V_8
    IL_003c:  stloc.s    V_10
    IL_003e:  ldloc.s    V_9
    IL_0040:  ldloc.s    V_10
    IL_0042:  conv.i
    IL_0043:  sizeof     [System.Runtime]System.Int32
    IL_0049:  mul
    IL_004a:  add
    IL_004b:  ldobj      [System.Runtime]System.Int32
    IL_0050:  add
    IL_0051:  ret
  } // end of method TestFunction24::pinObject

  .method public static int32  pinRef() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
             native int V_1,
             int32& pinned V_2)
    IL_0000:  ldc.i4.s   17
    IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldflda     !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::contents@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldobj      [System.Runtime]System.Int32
    IL_0018:  ldloc.1
    IL_0019:  ldobj      [System.Runtime]System.Int32
    IL_001e:  add
    IL_001f:  ret
  } // end of method TestFunction24::pinRef

  .method public static float64  pinArray1() cil managed
  {
    // Code size       204 (0xcc)
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64[] V_2,
             float64& pinned V_3,
             native int V_4,
             int32 V_5,
             native int V_6,
             int32 V_7,
             native int V_8,
             int32 V_9,
             native int V_10,
             int32 V_11)
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [System.Runtime]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem     [System.Runtime]System.Double
    IL_0016:  dup
    IL_0017:  ldc.i4.1
    IL_0018:  ldc.r8     1.5
    IL_0021:  stelem     [System.Runtime]System.Double
    IL_0026:  dup
    IL_0027:  ldc.i4.2
    IL_0028:  ldc.r8     2.2999999999999998
    IL_0031:  stelem     [System.Runtime]System.Double
    IL_0036:  dup
    IL_0037:  ldc.i4.3
    IL_0038:  ldc.r8     3.3999999999999999
    IL_0041:  stelem     [System.Runtime]System.Double
    IL_0046:  dup
    IL_0047:  ldc.i4.4
    IL_0048:  ldc.r8     4.0999999999999996
    IL_0051:  stelem     [System.Runtime]System.Double
    IL_0056:  dup
    IL_0057:  ldc.i4.5
    IL_0058:  ldc.r8     5.9000000000000004
    IL_0061:  stelem     [System.Runtime]System.Double
    IL_0066:  stloc.0
    IL_0067:  ldloc.0
    IL_0068:  stloc.2
    IL_0069:  ldloc.2
    IL_006a:  brfalse.s  IL_0086

    IL_006c:  ldloc.2
    IL_006d:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Length<float64>(!!0[])
    IL_0072:  brfalse.s  IL_0081

    IL_0074:  ldloc.2
    IL_0075:  ldc.i4.0
    IL_0076:  ldelema    [System.Runtime]System.Double
    IL_007b:  stloc.3
    IL_007c:  ldloc.3
    IL_007d:  conv.i
    IL_007e:  nop
    IL_007f:  br.s       IL_0089

    IL_0081:  ldc.i4.0
    IL_0082:  conv.i
    IL_0083:  nop
    IL_0084:  br.s       IL_0089

    IL_0086:  ldc.i4.0
    IL_0087:  conv.i
    IL_0088:  nop
    IL_0089:  stloc.1
    IL_008a:  ldloc.1
    IL_008b:  stloc.s    V_4
    IL_008d:  ldc.i4.0
    IL_008e:  stloc.s    V_5
    IL_0090:  ldloc.s    V_4
    IL_0092:  stloc.s    V_6
    IL_0094:  ldloc.s    V_5
    IL_0096:  stloc.s    V_7
    IL_0098:  ldloc.s    V_6
    IL_009a:  ldloc.s    V_7
    IL_009c:  conv.i
    IL_009d:  sizeof     [System.Runtime]System.Double
    IL_00a3:  mul
    IL_00a4:  add
    IL_00a5:  ldobj      [System.Runtime]System.Double
    IL_00aa:  ldloc.1
    IL_00ab:  stloc.s    V_8
    IL_00ad:  ldc.i4.1
    IL_00ae:  stloc.s    V_9
    IL_00b0:  ldloc.s    V_8
    IL_00b2:  stloc.s    V_10
    IL_00b4:  ldloc.s    V_9
    IL_00b6:  stloc.s    V_11
    IL_00b8:  ldloc.s    V_10
    IL_00ba:  ldloc.s    V_11
    IL_00bc:  conv.i
    IL_00bd:  sizeof     [System.Runtime]System.Double
    IL_00c3:  mul
    IL_00c4:  add
    IL_00c5:  ldobj      [System.Runtime]System.Double
    IL_00ca:  add
    IL_00cb:  ret
  } // end of method TestFunction24::pinArray1

  .method public static float64  pinArray2() cil managed
  {
    // Code size       178 (0xb2)
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64& pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6,
             native int V_7,
             int32 V_8,
             native int V_9,
             int32 V_10)
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [System.Runtime]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem     [System.Runtime]System.Double
    IL_0016:  dup
    IL_0017:  ldc.i4.1
    IL_0018:  ldc.r8     1.5
    IL_0021:  stelem     [System.Runtime]System.Double
    IL_0026:  dup
    IL_0027:  ldc.i4.2
    IL_0028:  ldc.r8     2.2999999999999998
    IL_0031:  stelem     [System.Runtime]System.Double
    IL_0036:  dup
    IL_0037:  ldc.i4.3
    IL_0038:  ldc.r8     3.3999999999999999
    IL_0041:  stelem     [System.Runtime]System.Double
    IL_0046:  dup
    IL_0047:  ldc.i4.4
    IL_0048:  ldc.r8     4.0999999999999996
    IL_0051:  stelem     [System.Runtime]System.Double
    IL_0056:  dup
    IL_0057:  ldc.i4.5
    IL_0058:  ldc.r8     5.9000000000000004
    IL_0061:  stelem     [System.Runtime]System.Double
    IL_0066:  stloc.0
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.0
    IL_0069:  ldelema    [System.Runtime]System.Double
    IL_006e:  stloc.2
    IL_006f:  ldloc.2
    IL_0070:  conv.i
    IL_0071:  stloc.1
    IL_0072:  ldloc.1
    IL_0073:  stloc.3
    IL_0074:  ldc.i4.0
    IL_0075:  stloc.s    V_4
    IL_0077:  ldloc.3
    IL_0078:  stloc.s    V_5
    IL_007a:  ldloc.s    V_4
    IL_007c:  stloc.s    V_6
    IL_007e:  ldloc.s    V_5
    IL_0080:  ldloc.s    V_6
    IL_0082:  conv.i
    IL_0083:  sizeof     [System.Runtime]System.Double
    IL_0089:  mul
    IL_008a:  add
    IL_008b:  ldobj      [System.Runtime]System.Double
    IL_0090:  ldloc.1
    IL_0091:  stloc.s    V_7
    IL_0093:  ldc.i4.1
    IL_0094:  stloc.s    V_8
    IL_0096:  ldloc.s    V_7
    IL_0098:  stloc.s    V_9
    IL_009a:  ldloc.s    V_8
    IL_009c:  stloc.s    V_10
    IL_009e:  ldloc.s    V_9
    IL_00a0:  ldloc.s    V_10
    IL_00a2:  conv.i
    IL_00a3:  sizeof     [System.Runtime]System.Double
    IL_00a9:  mul
    IL_00aa:  add
    IL_00ab:  ldobj      [System.Runtime]System.Double
    IL_00b0:  add
    IL_00b1:  ret
  } // end of method TestFunction24::pinArray2

  .method public static class [System.Runtime]System.Tuple`2<char,char> 
          pinString() cil managed
  {
    // Code size       93 (0x5d)
    .maxstack  6
    .locals init (string V_0,
             native int V_1,
             string pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6,
             native int V_7,
             int32 V_8,
             native int V_9,
             int32 V_10)
    IL_0000:  ldstr      "Hello World"
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.2
    IL_0008:  ldloc.2
    IL_0009:  brfalse.s  IL_0016

    IL_000b:  ldloc.2
    IL_000c:  conv.i
    IL_000d:  call       int32 [System.Runtime]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_0012:  add
    IL_0013:  nop
    IL_0014:  br.s       IL_0018

    IL_0016:  ldloc.2
    IL_0017:  nop
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  stloc.3
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stloc.s    V_5
    IL_0021:  ldloc.s    V_4
    IL_0023:  stloc.s    V_6
    IL_0025:  ldloc.s    V_5
    IL_0027:  ldloc.s    V_6
    IL_0029:  conv.i
    IL_002a:  sizeof     [System.Runtime]System.Char
    IL_0030:  mul
    IL_0031:  add
    IL_0032:  ldobj      [System.Runtime]System.Char
    IL_0037:  ldloc.1
    IL_0038:  stloc.s    V_7
    IL_003a:  ldc.i4.1
    IL_003b:  stloc.s    V_8
    IL_003d:  ldloc.s    V_7
    IL_003f:  stloc.s    V_9
    IL_0041:  ldloc.s    V_8
    IL_0043:  stloc.s    V_10
    IL_0045:  ldloc.s    V_9
    IL_0047:  ldloc.s    V_10
    IL_0049:  conv.i
    IL_004a:  sizeof     [System.Runtime]System.Char
    IL_0050:  mul
    IL_0051:  add
    IL_0052:  ldobj      [System.Runtime]System.Char
    IL_0057:  newobj     instance void class [System.Runtime]System.Tuple`2<char,char>::.ctor(!0,
                                                                                              !1)
    IL_005c:  ret
  } // end of method TestFunction24::pinString

} // end of class TestFunction24

.class private abstract auto ansi sealed '<StartupCode$TestFunction24>'.$TestFunction24
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction24::main@

} // end of class '<StartupCode$TestFunction24>'.$TestFunction24


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\TestFunctions\TestFunction24_fs\TestFunction24.res
