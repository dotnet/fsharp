
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
.assembly TestFunction25
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction25
{
  // Offset: 0x00000000 Length: 0x0000075B
  // WARNING: managed resource file FSharpSignatureData.TestFunction25 created
}
.mresource public FSharpOptimizationData.TestFunction25
{
  // Offset: 0x00000760 Length: 0x000003D2
  // WARNING: managed resource file FSharpOptimizationData.TestFunction25 created
}
.module TestFunction25.exe
// MVID: {5769538F-A643-4662-A745-03838F536957}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01400000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction25
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public Point
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class TestFunction25/Point>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class TestFunction25/Point>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field public int32 x@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field public int32 y@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction25/Point::x@
      IL_0006:  ret
    } // end of method Point::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction25/Point::y@
      IL_0006:  ret
    } // end of method Point::get_y

    .method public hidebysig specialname 
            instance void  set_x(int32 'value') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 TestFunction25/Point::x@
      IL_0007:  ret
    } // end of method Point::set_x

    .method public hidebysig specialname 
            instance void  set_y(int32 'value') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 TestFunction25/Point::y@
      IL_0007:  ret
    } // end of method Point::set_y

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
      IL_0008:  stfld      int32 TestFunction25/Point::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction25/Point::y@
      IL_0014:  ret
    } // end of method Point::.ctor

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction25/Point obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       102 (0x66)
      .maxstack  4
      .locals init (int32 V_0,
               class [mscorlib]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_005c

      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_005a

      IL_000d:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0012:  stloc.1
      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 TestFunction25/Point::x@
      IL_0019:  stloc.2
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction25/Point::x@
      IL_0020:  stloc.3
      IL_0021:  ldloc.2
      IL_0022:  ldloc.3
      IL_0023:  bge.s      IL_0029

      IL_0025:  ldc.i4.m1
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  ldloc.2
      IL_002a:  ldloc.3
      IL_002b:  cgt
      IL_002d:  nop
      IL_002e:  stloc.0
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.0
      IL_0031:  bge.s      IL_0035

      IL_0033:  ldloc.0
      IL_0034:  ret

      IL_0035:  ldloc.0
      IL_0036:  ldc.i4.0
      IL_0037:  ble.s      IL_003b

      IL_0039:  ldloc.0
      IL_003a:  ret

      IL_003b:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0040:  stloc.1
      IL_0041:  ldarg.0
      IL_0042:  ldfld      int32 TestFunction25/Point::y@
      IL_0047:  stloc.2
      IL_0048:  ldarg.1
      IL_0049:  ldfld      int32 TestFunction25/Point::y@
      IL_004e:  stloc.3
      IL_004f:  ldloc.2
      IL_0050:  ldloc.3
      IL_0051:  bge.s      IL_0055

      IL_0053:  ldc.i4.m1
      IL_0054:  ret

      IL_0055:  ldloc.2
      IL_0056:  ldloc.3
      IL_0057:  cgt
      IL_0059:  ret

      IL_005a:  ldc.i4.1
      IL_005b:  ret

      IL_005c:  ldarg.1
      IL_005d:  ldnull
      IL_005e:  cgt.un
      IL_0060:  brfalse.s  IL_0064

      IL_0062:  ldc.i4.m1
      IL_0063:  ret

      IL_0064:  ldc.i4.0
      IL_0065:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  unbox.any  TestFunction25/Point
      IL_0008:  callvirt   instance int32 TestFunction25/Point::CompareTo(class TestFunction25/Point)
      IL_000d:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       107 (0x6b)
      .maxstack  4
      .locals init ([0] class TestFunction25/Point V_0,
               [1] int32 V_1,
               [2] int32 V_2,
               [3] int32 V_3)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction25/Point
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  ldnull
      IL_000a:  cgt.un
      IL_000c:  brfalse.s  IL_005c

      IL_000e:  ldarg.1
      IL_000f:  unbox.any  TestFunction25/Point
      IL_0014:  ldnull
      IL_0015:  cgt.un
      IL_0017:  brfalse.s  IL_005a

      IL_0019:  ldarg.0
      IL_001a:  ldfld      int32 TestFunction25/Point::x@
      IL_001f:  stloc.2
      IL_0020:  ldloc.0
      IL_0021:  ldfld      int32 TestFunction25/Point::x@
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldloc.3
      IL_0029:  bge.s      IL_002f

      IL_002b:  ldc.i4.m1
      IL_002c:  nop
      IL_002d:  br.s       IL_0034

      IL_002f:  ldloc.2
      IL_0030:  ldloc.3
      IL_0031:  cgt
      IL_0033:  nop
      IL_0034:  stloc.1
      IL_0035:  ldloc.1
      IL_0036:  ldc.i4.0
      IL_0037:  bge.s      IL_003b

      IL_0039:  ldloc.1
      IL_003a:  ret

      IL_003b:  ldloc.1
      IL_003c:  ldc.i4.0
      IL_003d:  ble.s      IL_0041

      IL_003f:  ldloc.1
      IL_0040:  ret

      IL_0041:  ldarg.0
      IL_0042:  ldfld      int32 TestFunction25/Point::y@
      IL_0047:  stloc.2
      IL_0048:  ldloc.0
      IL_0049:  ldfld      int32 TestFunction25/Point::y@
      IL_004e:  stloc.3
      IL_004f:  ldloc.2
      IL_0050:  ldloc.3
      IL_0051:  bge.s      IL_0055

      IL_0053:  ldc.i4.m1
      IL_0054:  ret

      IL_0055:  ldloc.2
      IL_0056:  ldloc.3
      IL_0057:  cgt
      IL_0059:  ret

      IL_005a:  ldc.i4.1
      IL_005b:  ret

      IL_005c:  ldarg.1
      IL_005d:  unbox.any  TestFunction25/Point
      IL_0062:  ldnull
      IL_0063:  cgt.un
      IL_0065:  brfalse.s  IL_0069

      IL_0067:  ldc.i4.m1
      IL_0068:  ret

      IL_0069:  ldc.i4.0
      IL_006a:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       55 (0x37)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0035

      IL_0007:  ldc.i4.0
      IL_0008:  stloc.0
      IL_0009:  ldc.i4     0x9e3779b9
      IL_000e:  ldarg.0
      IL_000f:  ldfld      int32 TestFunction25/Point::y@
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.6
      IL_0016:  shl
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.2
      IL_0019:  shr
      IL_001a:  add
      IL_001b:  add
      IL_001c:  add
      IL_001d:  stloc.0
      IL_001e:  ldc.i4     0x9e3779b9
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 TestFunction25/Point::x@
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
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0007:  callvirt   instance int32 TestFunction25/Point::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000c:  ret
    } // end of method Point::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       58 (0x3a)
      .maxstack  4
      .locals init (class TestFunction25/Point V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0032

      IL_0007:  ldarg.1
      IL_0008:  isinst     TestFunction25/Point
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  brfalse.s  IL_0030

      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 TestFunction25/Point::x@
      IL_0017:  ldloc.0
      IL_0018:  ldfld      int32 TestFunction25/Point::x@
      IL_001d:  bne.un.s   IL_002e

      IL_001f:  ldarg.0
      IL_0020:  ldfld      int32 TestFunction25/Point::y@
      IL_0025:  ldloc.0
      IL_0026:  ldfld      int32 TestFunction25/Point::y@
      IL_002b:  ceq
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldarg.1
      IL_0033:  ldnull
      IL_0034:  cgt.un
      IL_0036:  ldc.i4.0
      IL_0037:  ceq
      IL_0039:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction25/Point obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       54 (0x36)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_002e

      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_002c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 TestFunction25/Point::x@
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction25/Point::x@
      IL_0019:  bne.un.s   IL_002a

      IL_001b:  ldarg.0
      IL_001c:  ldfld      int32 TestFunction25/Point::y@
      IL_0021:  ldarg.1
      IL_0022:  ldfld      int32 TestFunction25/Point::y@
      IL_0027:  ceq
      IL_0029:  ret

      IL_002a:  ldc.i4.0
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldarg.1
      IL_002f:  ldnull
      IL_0030:  cgt.un
      IL_0032:  ldc.i4.0
      IL_0033:  ceq
      IL_0035:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  4
      .locals init (class TestFunction25/Point V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  isinst     TestFunction25/Point
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  brfalse.s  IL_0013

      IL_000b:  ldarg.0
      IL_000c:  ldloc.0
      IL_000d:  callvirt   instance bool TestFunction25/Point::Equals(class TestFunction25/Point)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method Point::Equals

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .set instance void TestFunction25/Point::set_x(int32)
      .get instance int32 TestFunction25/Point::get_x()
    } // end of property Point::x
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .set instance void TestFunction25/Point::set_y(int32)
      .get instance int32 TestFunction25/Point::get_y()
    } // end of property Point::y
  } // end of class Point

  .method public static int32  pinObject() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  6
    .locals init ([0] class TestFunction25/Point point,
             [1] native int p1,
             [2] int32& pinned V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.2
    IL_0003:  newobj     instance void TestFunction25/Point::.ctor(int32,
                                                                   int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  ldflda     int32 TestFunction25/Point::x@
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  conv.i
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i
    IL_0016:  sizeof     [mscorlib]System.Int32
    IL_001c:  mul
    IL_001d:  add
    IL_001e:  ldobj      [mscorlib]System.Int32
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i
    IL_0026:  sizeof     [mscorlib]System.Int32
    IL_002c:  mul
    IL_002d:  add
    IL_002e:  ldobj      [mscorlib]System.Int32
    IL_0033:  add
    IL_0034:  ret
  } // end of method TestFunction25::pinObject

  .method public static int32  pinRef() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> point,
             [1] native int p1,
             [2] int32& pinned V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   17
    IL_0003:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  ldflda     !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::contents@
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  conv.i
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldobj      [mscorlib]System.Int32
    IL_0019:  ldloc.1
    IL_001a:  ldobj      [mscorlib]System.Int32
    IL_001f:  add
    IL_0020:  ret
  } // end of method TestFunction25::pinRef

  .method public static float64  pinArray1() cil managed
  {
    // Code size       168 (0xa8)
    .maxstack  6
    .locals init ([0] float64[] arr,
             [1] native int p1,
             [2] float64& pinned V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.6
    IL_0002:  newarr     [mscorlib]System.Double
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.r8     0.0
    IL_0012:  stelem     [mscorlib]System.Double
    IL_0017:  dup
    IL_0018:  ldc.i4.1
    IL_0019:  ldc.r8     1.5
    IL_0022:  stelem     [mscorlib]System.Double
    IL_0027:  dup
    IL_0028:  ldc.i4.2
    IL_0029:  ldc.r8     2.2999999999999998
    IL_0032:  stelem     [mscorlib]System.Double
    IL_0037:  dup
    IL_0038:  ldc.i4.3
    IL_0039:  ldc.r8     3.3999999999999999
    IL_0042:  stelem     [mscorlib]System.Double
    IL_0047:  dup
    IL_0048:  ldc.i4.4
    IL_0049:  ldc.r8     4.
    IL_0052:  stelem     [mscorlib]System.Double
    IL_0057:  dup
    IL_0058:  ldc.i4.5
    IL_0059:  ldc.r8     5.9000000000000004
    IL_0062:  stelem     [mscorlib]System.Double
    IL_0067:  stloc.0
    IL_0068:  ldloc.0
    IL_0069:  brfalse.s  IL_0082

    IL_006b:  ldloc.0
    IL_006c:  ldlen
    IL_006d:  conv.i4
    IL_006e:  brfalse.s  IL_007d

    IL_0070:  ldloc.0
    IL_0071:  ldc.i4.0
    IL_0072:  ldelema    [mscorlib]System.Double
    IL_0077:  stloc.2
    IL_0078:  ldloc.2
    IL_0079:  conv.i
    IL_007a:  nop
    IL_007b:  br.s       IL_0085

    IL_007d:  ldc.i4.0
    IL_007e:  conv.i
    IL_007f:  nop
    IL_0080:  br.s       IL_0085

    IL_0082:  ldc.i4.0
    IL_0083:  conv.i
    IL_0084:  nop
    IL_0085:  stloc.1
    IL_0086:  ldloc.1
    IL_0087:  ldc.i4.0
    IL_0088:  conv.i
    IL_0089:  sizeof     [mscorlib]System.Double
    IL_008f:  mul
    IL_0090:  add
    IL_0091:  ldobj      [mscorlib]System.Double
    IL_0096:  ldloc.1
    IL_0097:  ldc.i4.1
    IL_0098:  conv.i
    IL_0099:  sizeof     [mscorlib]System.Double
    IL_009f:  mul
    IL_00a0:  add
    IL_00a1:  ldobj      [mscorlib]System.Double
    IL_00a6:  add
    IL_00a7:  ret
  } // end of method TestFunction25::pinArray1

  .method public static float64  pinArray2() cil managed
  {
    // Code size       149 (0x95)
    .maxstack  6
    .locals init ([0] float64[] arr,
             [1] native int p,
             [2] float64& pinned V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.6
    IL_0002:  newarr     [mscorlib]System.Double
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.r8     0.0
    IL_0012:  stelem     [mscorlib]System.Double
    IL_0017:  dup
    IL_0018:  ldc.i4.1
    IL_0019:  ldc.r8     1.5
    IL_0022:  stelem     [mscorlib]System.Double
    IL_0027:  dup
    IL_0028:  ldc.i4.2
    IL_0029:  ldc.r8     2.2999999999999998
    IL_0032:  stelem     [mscorlib]System.Double
    IL_0037:  dup
    IL_0038:  ldc.i4.3
    IL_0039:  ldc.r8     3.3999999999999999
    IL_0042:  stelem     [mscorlib]System.Double
    IL_0047:  dup
    IL_0048:  ldc.i4.4
    IL_0049:  ldc.r8     4.
    IL_0052:  stelem     [mscorlib]System.Double
    IL_0057:  dup
    IL_0058:  ldc.i4.5
    IL_0059:  ldc.r8     5.9000000000000004
    IL_0062:  stelem     [mscorlib]System.Double
    IL_0067:  stloc.0
    IL_0068:  ldloc.0
    IL_0069:  ldc.i4.0
    IL_006a:  ldelema    [mscorlib]System.Double
    IL_006f:  stloc.2
    IL_0070:  ldloc.2
    IL_0071:  conv.i
    IL_0072:  stloc.1
    IL_0073:  ldloc.1
    IL_0074:  ldc.i4.0
    IL_0075:  conv.i
    IL_0076:  sizeof     [mscorlib]System.Double
    IL_007c:  mul
    IL_007d:  add
    IL_007e:  ldobj      [mscorlib]System.Double
    IL_0083:  ldloc.1
    IL_0084:  ldc.i4.1
    IL_0085:  conv.i
    IL_0086:  sizeof     [mscorlib]System.Double
    IL_008c:  mul
    IL_008d:  add
    IL_008e:  ldobj      [mscorlib]System.Double
    IL_0093:  add
    IL_0094:  ret
  } // end of method TestFunction25::pinArray2

  .method public static class [mscorlib]System.Tuple`2<char,char> 
          pinString() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  6
    .locals init ([0] native int pChar,
             [1] string pinned V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "Hello World"
    IL_0006:  stloc.1
    IL_0007:  ldstr      "Hello World"
    IL_000c:  conv.i
    IL_000d:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i
    IL_0017:  sizeof     [mscorlib]System.Char
    IL_001d:  mul
    IL_001e:  add
    IL_001f:  ldobj      [mscorlib]System.Char
    IL_0024:  ldloc.0
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i
    IL_0027:  sizeof     [mscorlib]System.Char
    IL_002d:  mul
    IL_002e:  add
    IL_002f:  ldobj      [mscorlib]System.Char
    IL_0034:  newobj     instance void class [mscorlib]System.Tuple`2<char,char>::.ctor(!0,
                                                                                        !1)
    IL_0039:  ret
  } // end of method TestFunction25::pinString

} // end of class TestFunction25

.class private abstract auto ansi sealed '<StartupCode$TestFunction25>'.$TestFunction25
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction25::main@

} // end of class '<StartupCode$TestFunction25>'.$TestFunction25


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file TestFunction25.il.netfx4.res
