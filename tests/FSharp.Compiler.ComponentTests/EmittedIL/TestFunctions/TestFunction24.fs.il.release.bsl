
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
  // Offset: 0x00000000 Length: 0x0000078D
  // WARNING: managed resource file FSharpSignatureData.TestFunction24 created
}
.mresource public FSharpOptimizationData.TestFunction24
{
  // Offset: 0x00000798 Length: 0x0000022E
  // WARNING: managed resource file FSharpOptimizationData.TestFunction24 created
}
.module TestFunction24.exe
// MVID: {628FBBC7-30D0-AD8E-A745-0383C7BB8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000013F6FC80000


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
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field public int32 y@
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction24/Point::x@
      IL_0006:  ret
    } // end of method Point::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction24/Point::y@
      IL_0006:  ret
    } // end of method Point::get_y

    .method public hidebysig specialname 
            instance void  set_x(int32 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
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
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
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
      // Code size       94 (0x5e)
      .maxstack  5
      .locals init (int32 V_0,
               class [System.Runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3,
               class [System.Runtime]System.Collections.IComparer V_4,
               int32 V_5,
               int32 V_6)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0057

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0055

      IL_0006:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction24/Point::x@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction24/Point::x@
      IL_0019:  stloc.3
      IL_001a:  ldloc.2
      IL_001b:  ldloc.3
      IL_001c:  cgt
      IL_001e:  ldloc.2
      IL_001f:  ldloc.3
      IL_0020:  clt
      IL_0022:  sub
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldc.i4.0
      IL_0026:  bge.s      IL_002a

      IL_0028:  ldloc.0
      IL_0029:  ret

      IL_002a:  ldloc.0
      IL_002b:  ldc.i4.0
      IL_002c:  ble.s      IL_0030

      IL_002e:  ldloc.0
      IL_002f:  ret

      IL_0030:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0035:  stloc.s    V_4
      IL_0037:  ldarg.0
      IL_0038:  ldfld      int32 TestFunction24/Point::y@
      IL_003d:  stloc.s    V_5
      IL_003f:  ldarg.1
      IL_0040:  ldfld      int32 TestFunction24/Point::y@
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.s    V_5
      IL_0049:  ldloc.s    V_6
      IL_004b:  cgt
      IL_004d:  ldloc.s    V_5
      IL_004f:  ldloc.s    V_6
      IL_0051:  clt
      IL_0053:  sub
      IL_0054:  ret

      IL_0055:  ldc.i4.1
      IL_0056:  ret

      IL_0057:  ldarg.1
      IL_0058:  brfalse.s  IL_005c

      IL_005a:  ldc.i4.m1
      IL_005b:  ret

      IL_005c:  ldc.i4.0
      IL_005d:  ret
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
      // Code size       111 (0x6f)
      .maxstack  5
      .locals init (class TestFunction24/Point V_0,
               class TestFunction24/Point V_1,
               int32 V_2,
               class [System.Runtime]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [System.Runtime]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction24/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  brfalse.s  IL_0063

      IL_000c:  ldarg.1
      IL_000d:  unbox.any  TestFunction24/Point
      IL_0012:  brfalse.s  IL_0061

      IL_0014:  ldarg.2
      IL_0015:  stloc.3
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 TestFunction24/Point::x@
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.1
      IL_001f:  ldfld      int32 TestFunction24/Point::x@
      IL_0024:  stloc.s    V_5
      IL_0026:  ldloc.s    V_4
      IL_0028:  ldloc.s    V_5
      IL_002a:  cgt
      IL_002c:  ldloc.s    V_4
      IL_002e:  ldloc.s    V_5
      IL_0030:  clt
      IL_0032:  sub
      IL_0033:  stloc.2
      IL_0034:  ldloc.2
      IL_0035:  ldc.i4.0
      IL_0036:  bge.s      IL_003a

      IL_0038:  ldloc.2
      IL_0039:  ret

      IL_003a:  ldloc.2
      IL_003b:  ldc.i4.0
      IL_003c:  ble.s      IL_0040

      IL_003e:  ldloc.2
      IL_003f:  ret

      IL_0040:  ldarg.2
      IL_0041:  stloc.s    V_6
      IL_0043:  ldarg.0
      IL_0044:  ldfld      int32 TestFunction24/Point::y@
      IL_0049:  stloc.s    V_7
      IL_004b:  ldloc.1
      IL_004c:  ldfld      int32 TestFunction24/Point::y@
      IL_0051:  stloc.s    V_8
      IL_0053:  ldloc.s    V_7
      IL_0055:  ldloc.s    V_8
      IL_0057:  cgt
      IL_0059:  ldloc.s    V_7
      IL_005b:  ldloc.s    V_8
      IL_005d:  clt
      IL_005f:  sub
      IL_0060:  ret

      IL_0061:  ldc.i4.1
      IL_0062:  ret

      IL_0063:  ldarg.1
      IL_0064:  unbox.any  TestFunction24/Point
      IL_0069:  brfalse.s  IL_006d

      IL_006b:  ldc.i4.m1
      IL_006c:  ret

      IL_006d:  ldc.i4.0
      IL_006e:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       55 (0x37)
      .maxstack  7
      .locals init (int32 V_0,
               class [System.Runtime]System.Collections.IEqualityComparer V_1,
               class [System.Runtime]System.Collections.IEqualityComparer V_2)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0035

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction24/Point::y@
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
      IL_0024:  ldfld      int32 TestFunction24/Point::x@
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

      IL_0035:  ldc.i4.0
      IL_0036:  ret
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
      // Code size       62 (0x3e)
      .maxstack  4
      .locals init (class TestFunction24/Point V_0,
               class TestFunction24/Point V_1,
               class [System.Runtime]System.Collections.IEqualityComparer V_2,
               class [System.Runtime]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0036

      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction24/Point
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0034

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.2
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 TestFunction24/Point::x@
      IL_0017:  ldloc.1
      IL_0018:  ldfld      int32 TestFunction24/Point::x@
      IL_001d:  ceq
      IL_001f:  brfalse.s  IL_0032

      IL_0021:  ldarg.2
      IL_0022:  stloc.3
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 TestFunction24/Point::y@
      IL_0029:  ldloc.1
      IL_002a:  ldfld      int32 TestFunction24/Point::y@
      IL_002f:  ceq
      IL_0031:  ret

      IL_0032:  ldc.i4.0
      IL_0033:  ret

      IL_0034:  ldc.i4.0
      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldnull
      IL_0038:  cgt.un
      IL_003a:  ldc.i4.0
      IL_003b:  ceq
      IL_003d:  ret
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
    // Code size       66 (0x42)
    .maxstack  6
    .locals init (class TestFunction24/Point V_0,
             native int V_1,
             int32& pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6)
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
    IL_0018:  ldloc.s    V_4
    IL_001a:  conv.i
    IL_001b:  sizeof     [System.Runtime]System.Int32
    IL_0021:  mul
    IL_0022:  add
    IL_0023:  ldobj      [System.Runtime]System.Int32
    IL_0028:  ldloc.1
    IL_0029:  stloc.s    V_5
    IL_002b:  ldc.i4.1
    IL_002c:  stloc.s    V_6
    IL_002e:  ldloc.s    V_5
    IL_0030:  ldloc.s    V_6
    IL_0032:  conv.i
    IL_0033:  sizeof     [System.Runtime]System.Int32
    IL_0039:  mul
    IL_003a:  add
    IL_003b:  ldobj      [System.Runtime]System.Int32
    IL_0040:  add
    IL_0041:  ret
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
    // Code size       188 (0xbc)
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64[] V_2,
             float64& pinned V_3,
             native int V_4,
             int32 V_5,
             native int V_6,
             int32 V_7)
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
    IL_0092:  ldloc.s    V_5
    IL_0094:  conv.i
    IL_0095:  sizeof     [System.Runtime]System.Double
    IL_009b:  mul
    IL_009c:  add
    IL_009d:  ldobj      [System.Runtime]System.Double
    IL_00a2:  ldloc.1
    IL_00a3:  stloc.s    V_6
    IL_00a5:  ldc.i4.1
    IL_00a6:  stloc.s    V_7
    IL_00a8:  ldloc.s    V_6
    IL_00aa:  ldloc.s    V_7
    IL_00ac:  conv.i
    IL_00ad:  sizeof     [System.Runtime]System.Double
    IL_00b3:  mul
    IL_00b4:  add
    IL_00b5:  ldobj      [System.Runtime]System.Double
    IL_00ba:  add
    IL_00bb:  ret
  } // end of method TestFunction24::pinArray1

  .method public static float64  pinArray2() cil managed
  {
    // Code size       162 (0xa2)
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64& pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6)
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
    IL_0078:  ldloc.s    V_4
    IL_007a:  conv.i
    IL_007b:  sizeof     [System.Runtime]System.Double
    IL_0081:  mul
    IL_0082:  add
    IL_0083:  ldobj      [System.Runtime]System.Double
    IL_0088:  ldloc.1
    IL_0089:  stloc.s    V_5
    IL_008b:  ldc.i4.1
    IL_008c:  stloc.s    V_6
    IL_008e:  ldloc.s    V_5
    IL_0090:  ldloc.s    V_6
    IL_0092:  conv.i
    IL_0093:  sizeof     [System.Runtime]System.Double
    IL_0099:  mul
    IL_009a:  add
    IL_009b:  ldobj      [System.Runtime]System.Double
    IL_00a0:  add
    IL_00a1:  ret
  } // end of method TestFunction24::pinArray2

  .method public static class [System.Runtime]System.Tuple`2<char,char> 
          pinString() cil managed
  {
    // Code size       77 (0x4d)
    .maxstack  6
    .locals init (string V_0,
             native int V_1,
             string pinned V_2,
             native int V_3,
             int32 V_4,
             native int V_5,
             int32 V_6)
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
    IL_001f:  ldloc.s    V_4
    IL_0021:  conv.i
    IL_0022:  sizeof     [System.Runtime]System.Char
    IL_0028:  mul
    IL_0029:  add
    IL_002a:  ldobj      [System.Runtime]System.Char
    IL_002f:  ldloc.1
    IL_0030:  stloc.s    V_5
    IL_0032:  ldc.i4.1
    IL_0033:  stloc.s    V_6
    IL_0035:  ldloc.s    V_5
    IL_0037:  ldloc.s    V_6
    IL_0039:  conv.i
    IL_003a:  sizeof     [System.Runtime]System.Char
    IL_0040:  mul
    IL_0041:  add
    IL_0042:  ldobj      [System.Runtime]System.Char
    IL_0047:  newobj     instance void class [System.Runtime]System.Tuple`2<char,char>::.ctor(!0,
                                                                                              !1)
    IL_004c:  ret
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
// WARNING: Created Win32 resource file C:\Users\vzari\code\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net6.0\tests\EmittedIL\TestFunctions\TestFunction24_fs\TestFunction24.res
