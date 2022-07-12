
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
.assembly TestFunction17
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction17
{
  // Offset: 0x00000000 Length: 0x000006B9
  // WARNING: managed resource file FSharpSignatureData.TestFunction17 created
}
.mresource public FSharpOptimizationData.TestFunction17
{
  // Offset: 0x000006C0 Length: 0x000001D3
  // WARNING: managed resource file FSharpOptimizationData.TestFunction17 created
}
.module TestFunction17.exe
// MVID: {628FBBC7-FD30-37B3-A745-0383C7BB8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002560FC20000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction17
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public R
         extends [System.Runtime]System.Object
         implements class [System.Runtime]System.IEquatable`1<class TestFunction17/R>,
                    [System.Runtime]System.Collections.IStructuralEquatable,
                    class [System.Runtime]System.IComparable`1<class TestFunction17/R>,
                    [System.Runtime]System.IComparable,
                    [System.Runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 x@
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 y@
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::x@
      IL_0006:  ret
    } // end of method R::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::y@
      IL_0006:  ret
    } // end of method R::get_y

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
      IL_0008:  stfld      int32 TestFunction17/R::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction17/R::y@
      IL_0014:  ret
    } // end of method R::.ctor

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction17/R>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method R::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction17/R obj) cil managed
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
      IL_000d:  ldfld      int32 TestFunction17/R::x@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction17/R::x@
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
      IL_0038:  ldfld      int32 TestFunction17/R::y@
      IL_003d:  stloc.s    V_5
      IL_003f:  ldarg.1
      IL_0040:  ldfld      int32 TestFunction17/R::y@
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
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction17/R
      IL_0007:  callvirt   instance int32 TestFunction17/R::CompareTo(class TestFunction17/R)
      IL_000c:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [System.Runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       111 (0x6f)
      .maxstack  5
      .locals init (class TestFunction17/R V_0,
               class TestFunction17/R V_1,
               int32 V_2,
               class [System.Runtime]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [System.Runtime]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction17/R
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  brfalse.s  IL_0063

      IL_000c:  ldarg.1
      IL_000d:  unbox.any  TestFunction17/R
      IL_0012:  brfalse.s  IL_0061

      IL_0014:  ldarg.2
      IL_0015:  stloc.3
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 TestFunction17/R::x@
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.1
      IL_001f:  ldfld      int32 TestFunction17/R::x@
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
      IL_0044:  ldfld      int32 TestFunction17/R::y@
      IL_0049:  stloc.s    V_7
      IL_004b:  ldloc.1
      IL_004c:  ldfld      int32 TestFunction17/R::y@
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
      IL_0064:  unbox.any  TestFunction17/R
      IL_0069:  brfalse.s  IL_006d

      IL_006b:  ldc.i4.m1
      IL_006c:  ret

      IL_006d:  ldc.i4.0
      IL_006e:  ret
    } // end of method R::CompareTo

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
      IL_000d:  ldfld      int32 TestFunction17/R::y@
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
      IL_0024:  ldfld      int32 TestFunction17/R::x@
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
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction17/R::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       62 (0x3e)
      .maxstack  4
      .locals init (class TestFunction17/R V_0,
               class TestFunction17/R V_1,
               class [System.Runtime]System.Collections.IEqualityComparer V_2,
               class [System.Runtime]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0036

      IL_0003:  ldarg.1
      IL_0004:  isinst     TestFunction17/R
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0034

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.2
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 TestFunction17/R::x@
      IL_0017:  ldloc.1
      IL_0018:  ldfld      int32 TestFunction17/R::x@
      IL_001d:  ceq
      IL_001f:  brfalse.s  IL_0032

      IL_0021:  ldarg.2
      IL_0022:  stloc.3
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 TestFunction17/R::y@
      IL_0029:  ldloc.1
      IL_002a:  ldfld      int32 TestFunction17/R::y@
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
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction17/R obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       47 (0x2f)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 TestFunction17/R::x@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 TestFunction17/R::x@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 TestFunction17/R::y@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction17/R::y@
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
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class TestFunction17/R V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction17/R
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction17/R::Equals(class TestFunction17/R)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method R::Equals

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_x()
    } // end of property R::x
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_y()
    } // end of property R::y
  } // end of class R

  .method public static class [System.Runtime]System.Tuple`2<class TestFunction17/R,class TestFunction17/R> 
          TestFunction17(int32 inp) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init (class TestFunction17/R V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void TestFunction17/R::.ctor(int32,
                                                               int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class [System.Runtime]System.Tuple`2<class TestFunction17/R,class TestFunction17/R>::.ctor(!0,
                                                                                                                                  !1)
    IL_000f:  ret
  } // end of method TestFunction17::TestFunction17

} // end of class TestFunction17

.class private abstract auto ansi sealed '<StartupCode$TestFunction17>'.$TestFunction17
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction17::main@

} // end of class '<StartupCode$TestFunction17>'.$TestFunction17


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\vzari\code\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net6.0\tests\EmittedIL\TestFunctions\TestFunction17_fs\TestFunction17.res
