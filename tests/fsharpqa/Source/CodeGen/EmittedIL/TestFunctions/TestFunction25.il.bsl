
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
.assembly TestFunction25
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction25
{
  // Offset: 0x00000000 Length: 0x00000742
}
.mresource public FSharpOptimizationData.TestFunction25
{
  // Offset: 0x00000748 Length: 0x000003D2
}
.module TestFunction25.exe
// MVID: {60D4E82F-A643-4662-A745-03832FE8D460}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04E60000


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

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction25/Point,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction25/Point>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction25/Point,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction25/Point,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Point::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction25/Point obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       101 (0x65)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 16707566,16707566 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction25.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_005b

      .line 16707566,16707566 : 0,0 ''
      IL_0006:  ldarg.1
      IL_0007:  ldnull
      IL_0008:  cgt.un
      IL_000a:  brfalse.s  IL_0059

      .line 16707566,16707566 : 0,0 ''
      IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.1
      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 TestFunction25/Point::x@
      IL_0018:  stloc.2
      IL_0019:  ldarg.1
      IL_001a:  ldfld      int32 TestFunction25/Point::x@
      IL_001f:  stloc.3
      IL_0020:  ldloc.2
      IL_0021:  ldloc.3
      IL_0022:  bge.s      IL_0028

      .line 16707566,16707566 : 0,0 ''
      IL_0024:  ldc.i4.m1
      .line 16707566,16707566 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_002d

      .line 16707566,16707566 : 0,0 ''
      IL_0028:  ldloc.2
      IL_0029:  ldloc.3
      IL_002a:  cgt
      .line 16707566,16707566 : 0,0 ''
      IL_002c:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_002d:  stloc.0
      IL_002e:  ldloc.0
      IL_002f:  ldc.i4.0
      IL_0030:  bge.s      IL_0034

      .line 16707566,16707566 : 0,0 ''
      IL_0032:  ldloc.0
      IL_0033:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0034:  ldloc.0
      IL_0035:  ldc.i4.0
      IL_0036:  ble.s      IL_003a

      .line 16707566,16707566 : 0,0 ''
      IL_0038:  ldloc.0
      IL_0039:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_003a:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_003f:  stloc.1
      IL_0040:  ldarg.0
      IL_0041:  ldfld      int32 TestFunction25/Point::y@
      IL_0046:  stloc.2
      IL_0047:  ldarg.1
      IL_0048:  ldfld      int32 TestFunction25/Point::y@
      IL_004d:  stloc.3
      IL_004e:  ldloc.2
      IL_004f:  ldloc.3
      IL_0050:  bge.s      IL_0054

      .line 16707566,16707566 : 0,0 ''
      IL_0052:  ldc.i4.m1
      IL_0053:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0054:  ldloc.2
      IL_0055:  ldloc.3
      IL_0056:  cgt
      IL_0058:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0059:  ldc.i4.1
      IL_005a:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_005b:  ldarg.1
      IL_005c:  ldnull
      IL_005d:  cgt.un
      IL_005f:  brfalse.s  IL_0063

      .line 16707566,16707566 : 0,0 ''
      IL_0061:  ldc.i4.m1
      IL_0062:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0063:  ldc.i4.0
      IL_0064:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction25/Point
      IL_0007:  callvirt   instance int32 TestFunction25/Point::CompareTo(class TestFunction25/Point)
      IL_000c:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       106 (0x6a)
      .maxstack  4
      .locals init ([0] class TestFunction25/Point V_0,
               [1] int32 V_1,
               [2] int32 V_2,
               [3] int32 V_3)
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction25/Point
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_005b

      .line 16707566,16707566 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  unbox.any  TestFunction25/Point
      IL_0013:  ldnull
      IL_0014:  cgt.un
      IL_0016:  brfalse.s  IL_0059

      .line 16707566,16707566 : 0,0 ''
      IL_0018:  ldarg.0
      IL_0019:  ldfld      int32 TestFunction25/Point::x@
      IL_001e:  stloc.2
      IL_001f:  ldloc.0
      IL_0020:  ldfld      int32 TestFunction25/Point::x@
      IL_0025:  stloc.3
      IL_0026:  ldloc.2
      IL_0027:  ldloc.3
      IL_0028:  bge.s      IL_002e

      .line 16707566,16707566 : 0,0 ''
      IL_002a:  ldc.i4.m1
      .line 16707566,16707566 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0033

      .line 16707566,16707566 : 0,0 ''
      IL_002e:  ldloc.2
      IL_002f:  ldloc.3
      IL_0030:  cgt
      .line 16707566,16707566 : 0,0 ''
      IL_0032:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_0033:  stloc.1
      IL_0034:  ldloc.1
      IL_0035:  ldc.i4.0
      IL_0036:  bge.s      IL_003a

      .line 16707566,16707566 : 0,0 ''
      IL_0038:  ldloc.1
      IL_0039:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_003a:  ldloc.1
      IL_003b:  ldc.i4.0
      IL_003c:  ble.s      IL_0040

      .line 16707566,16707566 : 0,0 ''
      IL_003e:  ldloc.1
      IL_003f:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0040:  ldarg.0
      IL_0041:  ldfld      int32 TestFunction25/Point::y@
      IL_0046:  stloc.2
      IL_0047:  ldloc.0
      IL_0048:  ldfld      int32 TestFunction25/Point::y@
      IL_004d:  stloc.3
      IL_004e:  ldloc.2
      IL_004f:  ldloc.3
      IL_0050:  bge.s      IL_0054

      .line 16707566,16707566 : 0,0 ''
      IL_0052:  ldc.i4.m1
      IL_0053:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0054:  ldloc.2
      IL_0055:  ldloc.3
      IL_0056:  cgt
      IL_0058:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0059:  ldc.i4.1
      IL_005a:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_005b:  ldarg.1
      IL_005c:  unbox.any  TestFunction25/Point
      IL_0061:  ldnull
      IL_0062:  cgt.un
      IL_0064:  brfalse.s  IL_0068

      .line 16707566,16707566 : 0,0 ''
      IL_0066:  ldc.i4.m1
      IL_0067:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0068:  ldc.i4.0
      IL_0069:  ret
    } // end of method Point::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       54 (0x36)
      .maxstack  7
      .locals init ([0] int32 V_0)
      .line 16707566,16707566 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0034

      .line 16707566,16707566 : 0,0 ''
      IL_0006:  ldc.i4.0
      IL_0007:  stloc.0
      IL_0008:  ldc.i4     0x9e3779b9
      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 TestFunction25/Point::y@
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.6
      IL_0015:  shl
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.2
      IL_0018:  shr
      IL_0019:  add
      IL_001a:  add
      IL_001b:  add
      IL_001c:  stloc.0
      IL_001d:  ldc.i4     0x9e3779b9
      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 TestFunction25/Point::x@
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.6
      IL_002a:  shl
      IL_002b:  ldloc.0
      IL_002c:  ldc.i4.2
      IL_002d:  shr
      IL_002e:  add
      IL_002f:  add
      IL_0030:  add
      IL_0031:  stloc.0
      IL_0032:  ldloc.0
      IL_0033:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0034:  ldc.i4.0
      IL_0035:  ret
    } // end of method Point::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction25/Point::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Point::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       57 (0x39)
      .maxstack  4
      .locals init ([0] class TestFunction25/Point V_0)
      .line 16707566,16707566 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0031

      .line 16707566,16707566 : 0,0 ''
      IL_0006:  ldarg.1
      IL_0007:  isinst     TestFunction25/Point
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse.s  IL_002f

      .line 16707566,16707566 : 0,0 ''
      IL_0010:  ldarg.0
      IL_0011:  ldfld      int32 TestFunction25/Point::x@
      IL_0016:  ldloc.0
      IL_0017:  ldfld      int32 TestFunction25/Point::x@
      IL_001c:  bne.un.s   IL_002d

      .line 16707566,16707566 : 0,0 ''
      IL_001e:  ldarg.0
      IL_001f:  ldfld      int32 TestFunction25/Point::y@
      IL_0024:  ldloc.0
      IL_0025:  ldfld      int32 TestFunction25/Point::y@
      IL_002a:  ceq
      IL_002c:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_002d:  ldc.i4.0
      IL_002e:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_002f:  ldc.i4.0
      IL_0030:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0031:  ldarg.1
      IL_0032:  ldnull
      IL_0033:  cgt.un
      IL_0035:  ldc.i4.0
      IL_0036:  ceq
      IL_0038:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction25/Point obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       53 (0x35)
      .maxstack  8
      .line 16707566,16707566 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_002d

      .line 16707566,16707566 : 0,0 ''
      IL_0006:  ldarg.1
      IL_0007:  ldnull
      IL_0008:  cgt.un
      IL_000a:  brfalse.s  IL_002b

      .line 16707566,16707566 : 0,0 ''
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 TestFunction25/Point::x@
      IL_0012:  ldarg.1
      IL_0013:  ldfld      int32 TestFunction25/Point::x@
      IL_0018:  bne.un.s   IL_0029

      .line 16707566,16707566 : 0,0 ''
      IL_001a:  ldarg.0
      IL_001b:  ldfld      int32 TestFunction25/Point::y@
      IL_0020:  ldarg.1
      IL_0021:  ldfld      int32 TestFunction25/Point::y@
      IL_0026:  ceq
      IL_0028:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0029:  ldc.i4.0
      IL_002a:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_002b:  ldc.i4.0
      IL_002c:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_002d:  ldarg.1
      IL_002e:  ldnull
      IL_002f:  cgt.un
      IL_0031:  ldc.i4.0
      IL_0032:  ceq
      IL_0034:  ret
    } // end of method Point::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init ([0] class TestFunction25/Point V_0)
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction25/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      .line 16707566,16707566 : 0,0 ''
      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction25/Point::Equals(class TestFunction25/Point)
      IL_0011:  ret

      .line 16707566,16707566 : 0,0 ''
      IL_0012:  ldc.i4.0
      IL_0013:  ret
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
    // Code size       52 (0x34)
    .maxstack  6
    .locals init ([0] class TestFunction25/Point point,
             [1] native int p1,
             [2] int32& pinned V_2)
    .line 7,7 : 5,33 ''
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void TestFunction25/Point::.ctor(int32,
                                                                   int32)
    IL_0007:  stloc.0
    .line 8,8 : 5,28 ''
    IL_0008:  ldloc.0
    IL_0009:  ldflda     int32 TestFunction25/Point::x@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    .line 9,9 : 5,44 ''
    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i
    IL_0015:  sizeof     [mscorlib]System.Int32
    IL_001b:  mul
    IL_001c:  add
    IL_001d:  ldobj      [mscorlib]System.Int32
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i
    IL_0025:  sizeof     [mscorlib]System.Int32
    IL_002b:  mul
    IL_002c:  add
    IL_002d:  ldobj      [mscorlib]System.Int32
    IL_0032:  add
    IL_0033:  ret
  } // end of method TestFunction25::pinObject

  .method public static int32  pinRef() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> point,
             [1] native int p1,
             [2] int32& pinned V_2)
    .line 12,12 : 5,23 ''
    IL_0000:  ldc.i4.s   17
    IL_0002:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
    IL_0007:  stloc.0
    .line 13,13 : 5,35 ''
    IL_0008:  ldloc.0
    IL_0009:  ldflda     !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::contents@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    .line 14,14 : 5,42 ''
    IL_0012:  ldloc.1
    IL_0013:  ldobj      [mscorlib]System.Int32
    IL_0018:  ldloc.1
    IL_0019:  ldobj      [mscorlib]System.Int32
    IL_001e:  add
    IL_001f:  ret
  } // end of method TestFunction25::pinRef

  .method public static float64  pinArray1() cil managed
  {
    // Code size       170 (0xaa)
    .maxstack  6
    .locals init ([0] float64[] arr,
             [1] native int p1,
             [2] float64& pinned V_2)
    .line 17,17 : 5,49 ''
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [mscorlib]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem     [mscorlib]System.Double
    IL_0016:  dup
    IL_0017:  ldc.i4.1
    IL_0018:  ldc.r8     1.5
    IL_0021:  stelem     [mscorlib]System.Double
    IL_0026:  dup
    IL_0027:  ldc.i4.2
    IL_0028:  ldc.r8     2.2999999999999998
    IL_0031:  stelem     [mscorlib]System.Double
    IL_0036:  dup
    IL_0037:  ldc.i4.3
    IL_0038:  ldc.r8     3.3999999999999999
    IL_0041:  stelem     [mscorlib]System.Double
    IL_0046:  dup
    IL_0047:  ldc.i4.4
    IL_0048:  ldc.r8     4.
    IL_0051:  stelem     [mscorlib]System.Double
    IL_0056:  dup
    IL_0057:  ldc.i4.5
    IL_0058:  ldc.r8     5.9000000000000004
    IL_0061:  stelem     [mscorlib]System.Double
    IL_0066:  stloc.0
    .line 18,18 : 5,23 ''
    IL_0067:  ldloc.0
    IL_0068:  brfalse.s  IL_0084

    .line 16707566,16707566 : 0,0 ''
    IL_006a:  ldloc.0
    IL_006b:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Length<float64>(!!0[])
    IL_0070:  brfalse.s  IL_007f

    .line 16707566,16707566 : 0,0 ''
    IL_0072:  ldloc.0
    IL_0073:  ldc.i4.0
    IL_0074:  ldelema    [mscorlib]System.Double
    IL_0079:  stloc.2
    IL_007a:  ldloc.2
    IL_007b:  conv.i
    .line 16707566,16707566 : 0,0 ''
    IL_007c:  nop
    IL_007d:  br.s       IL_0087

    .line 16707566,16707566 : 0,0 ''
    IL_007f:  ldc.i4.0
    IL_0080:  conv.i
    .line 16707566,16707566 : 0,0 ''
    IL_0081:  nop
    IL_0082:  br.s       IL_0087

    .line 16707566,16707566 : 0,0 ''
    IL_0084:  ldc.i4.0
    IL_0085:  conv.i
    .line 16707566,16707566 : 0,0 ''
    IL_0086:  nop
    .line 16707566,16707566 : 0,0 ''
    IL_0087:  stloc.1
    .line 19,19 : 5,44 ''
    IL_0088:  ldloc.1
    IL_0089:  ldc.i4.0
    IL_008a:  conv.i
    IL_008b:  sizeof     [mscorlib]System.Double
    IL_0091:  mul
    IL_0092:  add
    IL_0093:  ldobj      [mscorlib]System.Double
    IL_0098:  ldloc.1
    IL_0099:  ldc.i4.1
    IL_009a:  conv.i
    IL_009b:  sizeof     [mscorlib]System.Double
    IL_00a1:  mul
    IL_00a2:  add
    IL_00a3:  ldobj      [mscorlib]System.Double
    IL_00a8:  add
    IL_00a9:  ret
  } // end of method TestFunction25::pinArray1

  .method public static float64  pinArray2() cil managed
  {
    // Code size       148 (0x94)
    .maxstack  6
    .locals init ([0] float64[] arr,
             [1] native int p,
             [2] float64& pinned V_2)
    .line 22,22 : 5,49 ''
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [mscorlib]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem     [mscorlib]System.Double
    IL_0016:  dup
    IL_0017:  ldc.i4.1
    IL_0018:  ldc.r8     1.5
    IL_0021:  stelem     [mscorlib]System.Double
    IL_0026:  dup
    IL_0027:  ldc.i4.2
    IL_0028:  ldc.r8     2.2999999999999998
    IL_0031:  stelem     [mscorlib]System.Double
    IL_0036:  dup
    IL_0037:  ldc.i4.3
    IL_0038:  ldc.r8     3.3999999999999999
    IL_0041:  stelem     [mscorlib]System.Double
    IL_0046:  dup
    IL_0047:  ldc.i4.4
    IL_0048:  ldc.r8     4.
    IL_0051:  stelem     [mscorlib]System.Double
    IL_0056:  dup
    IL_0057:  ldc.i4.5
    IL_0058:  ldc.r8     5.9000000000000004
    IL_0061:  stelem     [mscorlib]System.Double
    IL_0066:  stloc.0
    .line 24,24 : 5,27 ''
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.0
    IL_0069:  ldelema    [mscorlib]System.Double
    IL_006e:  stloc.2
    IL_006f:  ldloc.2
    IL_0070:  conv.i
    IL_0071:  stloc.1
    .line 25,25 : 5,42 ''
    IL_0072:  ldloc.1
    IL_0073:  ldc.i4.0
    IL_0074:  conv.i
    IL_0075:  sizeof     [mscorlib]System.Double
    IL_007b:  mul
    IL_007c:  add
    IL_007d:  ldobj      [mscorlib]System.Double
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.1
    IL_0084:  conv.i
    IL_0085:  sizeof     [mscorlib]System.Double
    IL_008b:  mul
    IL_008c:  add
    IL_008d:  ldobj      [mscorlib]System.Double
    IL_0092:  add
    IL_0093:  ret
  } // end of method TestFunction25::pinArray2

  .method public static class [mscorlib]System.Tuple`2<char,char> 
          pinString() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  6
    .locals init ([0] native int pChar,
             [1] string pinned V_1)
    .line 30,30 : 5,26 ''
    IL_0000:  ldstr      "Hello World"
    IL_0005:  stloc.1
    IL_0006:  ldstr      "Hello World"
    IL_000b:  conv.i
    IL_000c:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_0011:  add
    IL_0012:  stloc.0
    .line 31,31 : 5,50 ''
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i
    IL_0016:  sizeof     [mscorlib]System.Char
    IL_001c:  mul
    IL_001d:  add
    IL_001e:  ldobj      [mscorlib]System.Char
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i
    IL_0026:  sizeof     [mscorlib]System.Char
    IL_002c:  mul
    IL_002d:  add
    IL_002e:  ldobj      [mscorlib]System.Char
    IL_0033:  newobj     instance void class [mscorlib]System.Tuple`2<char,char>::.ctor(!0,
                                                                                        !1)
    IL_0038:  ret
  } // end of method TestFunction25::pinString

} // end of class TestFunction25

.class private abstract auto ansi sealed '<StartupCode$TestFunction25>'.$TestFunction25
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction25::main@

} // end of class '<StartupCode$TestFunction25>'.$TestFunction25


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
