
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
.assembly StructUnion01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.StructUnion01
{
  // Offset: 0x00000000 Length: 0x0000088A
}
.mresource public FSharpOptimizationData.StructUnion01
{
  // Offset: 0x00000890 Length: 0x00000421
}
.module StructUnion01.dll
// MVID: {576332E3-D3E9-6B24-A745-0383E3326357}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x007C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed StructUnion01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit U
         extends [mscorlib]System.ValueType
         implements class [mscorlib]System.IEquatable`1<valuetype StructUnion01/U>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<valuetype StructUnion01/U>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly int32 item1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly int32 item2
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static valuetype StructUnion01/U 
            NewU(int32 item1,
                 int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void StructUnion01/U::.ctor(int32,
                                                                int32)
      IL_0007:  ret
    } // end of method U::NewU

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 item1,
                                 int32 item2) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 StructUnion01/U::item1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 StructUnion01/U::item2
      IL_000e:  ret
    } // end of method U::.ctor

    .method public hidebysig instance int32 
            get_Item1() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 StructUnion01/U::item1
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
      IL_0001:  ldfld      int32 StructUnion01/U::item2
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
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      StructUnion01/U
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>::Invoke(!0)
      IL_001a:  ret
    } // end of method U::__DebugDisplay

    .method public hidebysig virtual final 
            instance int32  CompareTo(valuetype StructUnion01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       77 (0x4d)
      .maxstack  4
      .locals init (int32 V_0,
               class [mscorlib]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0007:  stloc.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32 StructUnion01/U::item1
      IL_000e:  stloc.2
      IL_000f:  ldarg.1
      IL_0010:  ldfld      int32 StructUnion01/U::item1
      IL_0015:  stloc.3
      IL_0016:  ldloc.2
      IL_0017:  ldloc.3
      IL_0018:  bge.s      IL_001d

      IL_001a:  ldc.i4.m1
      IL_001b:  br.s       IL_0021

      IL_001d:  ldloc.2
      IL_001e:  ldloc.3
      IL_001f:  cgt
      IL_0021:  stloc.0
      IL_0022:  ldloc.0
      IL_0023:  ldc.i4.0
      IL_0024:  bge.s      IL_0028

      IL_0026:  ldloc.0
      IL_0027:  ret

      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.0
      IL_002a:  ble.s      IL_002e

      IL_002c:  ldloc.0
      IL_002d:  ret

      IL_002e:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0033:  stloc.1
      IL_0034:  ldarg.0
      IL_0035:  ldfld      int32 StructUnion01/U::item2
      IL_003a:  stloc.2
      IL_003b:  ldarg.1
      IL_003c:  ldfld      int32 StructUnion01/U::item2
      IL_0041:  stloc.3
      IL_0042:  ldloc.2
      IL_0043:  ldloc.3
      IL_0044:  bge.s      IL_0048

      IL_0046:  ldc.i4.m1
      IL_0047:  ret

      IL_0048:  ldloc.2
      IL_0049:  ldloc.3
      IL_004a:  cgt
      IL_004c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  StructUnion01/U
      IL_0007:  call       instance int32 StructUnion01/U::CompareTo(valuetype StructUnion01/U)
      IL_000c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       81 (0x51)
      .maxstack  4
      .locals init (valuetype StructUnion01/U V_0,
               valuetype StructUnion01/U& V_1,
               int32 V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  StructUnion01/U
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.0
      IL_000b:  pop
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 StructUnion01/U::item1
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      int32 StructUnion01/U::item1
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  bge.s      IL_0023

      IL_0020:  ldc.i4.m1
      IL_0021:  br.s       IL_0028

      IL_0023:  ldloc.3
      IL_0024:  ldloc.s    V_4
      IL_0026:  cgt
      IL_0028:  stloc.2
      IL_0029:  ldloc.2
      IL_002a:  ldc.i4.0
      IL_002b:  bge.s      IL_002f

      IL_002d:  ldloc.2
      IL_002e:  ret

      IL_002f:  ldloc.2
      IL_0030:  ldc.i4.0
      IL_0031:  ble.s      IL_0035

      IL_0033:  ldloc.2
      IL_0034:  ret

      IL_0035:  ldarg.0
      IL_0036:  ldfld      int32 StructUnion01/U::item2
      IL_003b:  stloc.3
      IL_003c:  ldloc.1
      IL_003d:  ldfld      int32 StructUnion01/U::item2
      IL_0042:  stloc.s    V_4
      IL_0044:  ldloc.3
      IL_0045:  ldloc.s    V_4
      IL_0047:  bge.s      IL_004b

      IL_0049:  ldc.i4.m1
      IL_004a:  ret

      IL_004b:  ldloc.3
      IL_004c:  ldloc.s    V_4
      IL_004e:  cgt
      IL_0050:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       50 (0x32)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  pop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.0
      IL_0006:  ldc.i4     0x9e3779b9
      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 StructUnion01/U::item2
      IL_0011:  ldloc.0
      IL_0012:  ldc.i4.6
      IL_0013:  shl
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.2
      IL_0016:  shr
      IL_0017:  add
      IL_0018:  add
      IL_0019:  add
      IL_001a:  stloc.0
      IL_001b:  ldc.i4     0x9e3779b9
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 StructUnion01/U::item1
      IL_0026:  ldloc.0
      IL_0027:  ldc.i4.6
      IL_0028:  shl
      IL_0029:  ldloc.0
      IL_002a:  ldc.i4.2
      IL_002b:  shr
      IL_002c:  add
      IL_002d:  add
      IL_002e:  add
      IL_002f:  stloc.0
      IL_0030:  ldloc.0
      IL_0031:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 StructUnion01/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       53 (0x35)
      .maxstack  4
      .locals init (valuetype StructUnion01/U V_0,
               valuetype StructUnion01/U& V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructUnion01/U>(object)
      IL_0006:  brtrue.s   IL_000a

      IL_0008:  ldc.i4.0
      IL_0009:  ret

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  StructUnion01/U
      IL_0010:  stloc.0
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  pop
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 StructUnion01/U::item1
      IL_001c:  ldloc.1
      IL_001d:  ldfld      int32 StructUnion01/U::item1
      IL_0022:  bne.un.s   IL_0033

      IL_0024:  ldarg.0
      IL_0025:  ldfld      int32 StructUnion01/U::item2
      IL_002a:  ldloc.1
      IL_002b:  ldfld      int32 StructUnion01/U::item2
      IL_0030:  ceq
      IL_0032:  ret

      IL_0033:  ldc.i4.0
      IL_0034:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype StructUnion01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       36 (0x24)
      .maxstack  4
      .locals init (valuetype StructUnion01/U& V_0)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  pop
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 StructUnion01/U::item1
      IL_000b:  ldloc.0
      IL_000c:  ldfld      int32 StructUnion01/U::item1
      IL_0011:  bne.un.s   IL_0022

      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 StructUnion01/U::item2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 StructUnion01/U::item2
      IL_001f:  ceq
      IL_0021:  ret

      IL_0022:  ldc.i4.0
      IL_0023:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructUnion01/U>(object)
      IL_0006:  brtrue.s   IL_000a

      IL_0008:  ldc.i4.0
      IL_0009:  ret

      IL_000a:  ldarg.0
      IL_000b:  ldarg.1
      IL_000c:  unbox.any  StructUnion01/U
      IL_0011:  call       instance bool StructUnion01/U::Equals(valuetype StructUnion01/U)
      IL_0016:  ret
    } // end of method U::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 StructUnion01/U::get_Tag()
    } // end of property U::Tag
    .property instance int32 Item1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 StructUnion01/U::get_Item1()
    } // end of property U::Item1
    .property instance int32 Item2()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 StructUnion01/U::get_Item2()
    } // end of property U::Item2
  } // end of class U

  .method public static int32  g1(valuetype StructUnion01/U _arg1) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarga.s   _arg1
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldarga.s   _arg1
    IL_0009:  ldfld      int32 StructUnion01/U::item2
    IL_000e:  add
    IL_000f:  ret
  } // end of method StructUnion01::g1

  .method public static int32  g2(valuetype StructUnion01/U u) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarga.s   u
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldarga.s   u
    IL_0009:  ldfld      int32 StructUnion01/U::item2
    IL_000e:  add
    IL_000f:  ret
  } // end of method StructUnion01::g2

  .method public static int32  g3(valuetype StructUnion01/U x) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     ( 
                          IL_0022)
    IL_0012:  ldarga.s   x
    IL_0014:  ldfld      int32 StructUnion01/U::item1
    IL_0019:  ldarga.s   x
    IL_001b:  ldfld      int32 StructUnion01/U::item2
    IL_0020:  add
    IL_0021:  ret

    IL_0022:  ldarga.s   x
    IL_0024:  ldfld      int32 StructUnion01/U::item2
    IL_0029:  ret
  } // end of method StructUnion01::g3

  .method public static int32  g4(valuetype StructUnion01/U x,
                                  valuetype StructUnion01/U y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       126 (0x7e)
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     ( 
                          IL_003a)
    IL_0012:  ldarga.s   y
    IL_0014:  ldfld      int32 StructUnion01/U::item2
    IL_0019:  stloc.0
    IL_001a:  ldarga.s   y
    IL_001c:  ldfld      int32 StructUnion01/U::item1
    IL_0021:  stloc.1
    IL_0022:  ldarga.s   x
    IL_0024:  ldfld      int32 StructUnion01/U::item2
    IL_0029:  stloc.2
    IL_002a:  ldarga.s   x
    IL_002c:  ldfld      int32 StructUnion01/U::item1
    IL_0031:  stloc.3
    IL_0032:  ldloc.3
    IL_0033:  ldloc.2
    IL_0034:  add
    IL_0035:  ldloc.1
    IL_0036:  add
    IL_0037:  ldloc.0
    IL_0038:  add
    IL_0039:  ret

    IL_003a:  ldarga.s   y
    IL_003c:  ldfld      int32 StructUnion01/U::item1
    IL_0041:  ldc.i4.5
    IL_0042:  sub
    IL_0043:  switch     ( 
                          IL_006e)
    IL_004c:  ldarga.s   y
    IL_004e:  ldfld      int32 StructUnion01/U::item2
    IL_0053:  ldarga.s   y
    IL_0055:  ldfld      int32 StructUnion01/U::item1
    IL_005a:  ldarga.s   x
    IL_005c:  ldfld      int32 StructUnion01/U::item2
    IL_0061:  ldarga.s   x
    IL_0063:  ldfld      int32 StructUnion01/U::item1
    IL_0068:  stloc.3
    IL_0069:  stloc.2
    IL_006a:  stloc.1
    IL_006b:  stloc.0
    IL_006c:  br.s       IL_0032

    IL_006e:  ldarga.s   x
    IL_0070:  ldfld      int32 StructUnion01/U::item2
    IL_0075:  ldarga.s   y
    IL_0077:  ldfld      int32 StructUnion01/U::item2
    IL_007c:  add
    IL_007d:  ret
  } // end of method StructUnion01::g4

  .method public static int32  f1(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (valuetype StructUnion01/U V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      StructUnion01/U
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldfld      int32 StructUnion01/U::item1
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldfld      int32 StructUnion01/U::item2
    IL_0015:  add
    IL_0016:  ret
  } // end of method StructUnion01::f1

  .method public static int32  f2(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (valuetype StructUnion01/U V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      StructUnion01/U
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldfld      int32 StructUnion01/U::item1
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldfld      int32 StructUnion01/U::item2
    IL_0015:  add
    IL_0016:  ret
  } // end of method StructUnion01::f2

  .method public static int32  f3(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  4
    .locals init (valuetype StructUnion01/U V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      StructUnion01/U
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldfld      int32 StructUnion01/U::item1
    IL_000e:  ldc.i4.3
    IL_000f:  sub
    IL_0010:  switch     ( 
                          IL_0029)
    IL_0019:  ldloca.s   V_0
    IL_001b:  ldfld      int32 StructUnion01/U::item1
    IL_0020:  ldloca.s   V_0
    IL_0022:  ldfld      int32 StructUnion01/U::item2
    IL_0027:  add
    IL_0028:  ret

    IL_0029:  ldloca.s   V_0
    IL_002b:  ldfld      int32 StructUnion01/U::item2
    IL_0030:  ret
  } // end of method StructUnion01::f3

  .method public static int32  f4(valuetype StructUnion01/U& x,
                                  valuetype StructUnion01/U& y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       146 (0x92)
    .maxstack  6
    .locals init (valuetype StructUnion01/U V_0,
             valuetype StructUnion01/U V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      StructUnion01/U
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldobj      StructUnion01/U
    IL_000d:  stloc.1
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldfld      int32 StructUnion01/U::item1
    IL_0015:  ldc.i4.3
    IL_0016:  sub
    IL_0017:  switch     ( 
                          IL_004c)
    IL_0020:  ldloca.s   V_1
    IL_0022:  ldfld      int32 StructUnion01/U::item2
    IL_0027:  stloc.2
    IL_0028:  ldloca.s   V_1
    IL_002a:  ldfld      int32 StructUnion01/U::item1
    IL_002f:  stloc.3
    IL_0030:  ldloca.s   V_0
    IL_0032:  ldfld      int32 StructUnion01/U::item2
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloca.s   V_0
    IL_003b:  ldfld      int32 StructUnion01/U::item1
    IL_0040:  stloc.s    V_5
    IL_0042:  ldloc.s    V_5
    IL_0044:  ldloc.s    V_4
    IL_0046:  add
    IL_0047:  ldloc.3
    IL_0048:  add
    IL_0049:  ldloc.2
    IL_004a:  add
    IL_004b:  ret

    IL_004c:  ldloca.s   V_1
    IL_004e:  ldfld      int32 StructUnion01/U::item1
    IL_0053:  ldc.i4.5
    IL_0054:  sub
    IL_0055:  switch     ( 
                          IL_0082)
    IL_005e:  ldloca.s   V_1
    IL_0060:  ldfld      int32 StructUnion01/U::item2
    IL_0065:  ldloca.s   V_1
    IL_0067:  ldfld      int32 StructUnion01/U::item1
    IL_006c:  ldloca.s   V_0
    IL_006e:  ldfld      int32 StructUnion01/U::item2
    IL_0073:  ldloca.s   V_0
    IL_0075:  ldfld      int32 StructUnion01/U::item1
    IL_007a:  stloc.s    V_5
    IL_007c:  stloc.s    V_4
    IL_007e:  stloc.3
    IL_007f:  stloc.2
    IL_0080:  br.s       IL_0042

    IL_0082:  ldloca.s   V_0
    IL_0084:  ldfld      int32 StructUnion01/U::item2
    IL_0089:  ldloca.s   V_1
    IL_008b:  ldfld      int32 StructUnion01/U::item2
    IL_0090:  add
    IL_0091:  ret
  } // end of method StructUnion01::f4

} // end of class StructUnion01

.class private abstract auto ansi sealed '<StartupCode$StructUnion01>'.$StructUnion01
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$StructUnion01>'.$StructUnion01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
