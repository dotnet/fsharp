
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
.assembly StructUnion01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.StructUnion01
{
  // Offset: 0x00000000 Length: 0x000008BD
  // WARNING: managed resource file FSharpSignatureData.StructUnion01 created
}
.mresource public FSharpOptimizationData.StructUnion01
{
  // Offset: 0x000008C8 Length: 0x00000421
  // WARNING: managed resource file FSharpOptimizationData.StructUnion01 created
}
.module StructUnion01.exe
// MVID: {624E944C-488D-8510-A745-03834C944E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03BD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed StructUnion01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential autochar serializable sealed nested public beforefieldinit U
         extends [mscorlib]System.ValueType
         implements class [mscorlib]System.IEquatable`1<valuetype StructUnion01/U>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<valuetype StructUnion01/U>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .pack 0
    .size 1
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

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype StructUnion01/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      StructUnion01/U
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype StructUnion01/U,string>::Invoke(!0)
      IL_001a:  ret
    } // end of method U::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(valuetype StructUnion01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       81 (0x51)
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
      IL_000f:  ldarga.s   obj
      IL_0011:  ldfld      int32 StructUnion01/U::item1
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  bge.s      IL_001f

      IL_001b:  ldc.i4.m1
      IL_001c:  nop
      IL_001d:  br.s       IL_0024

      IL_001f:  ldloc.2
      IL_0020:  ldloc.3
      IL_0021:  cgt
      IL_0023:  nop
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  ldc.i4.0
      IL_0027:  bge.s      IL_002b

      IL_0029:  ldloc.0
      IL_002a:  ret

      IL_002b:  ldloc.0
      IL_002c:  ldc.i4.0
      IL_002d:  ble.s      IL_0031

      IL_002f:  ldloc.0
      IL_0030:  ret

      IL_0031:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0036:  stloc.1
      IL_0037:  ldarg.0
      IL_0038:  ldfld      int32 StructUnion01/U::item2
      IL_003d:  stloc.2
      IL_003e:  ldarga.s   obj
      IL_0040:  ldfld      int32 StructUnion01/U::item2
      IL_0045:  stloc.3
      IL_0046:  ldloc.2
      IL_0047:  ldloc.3
      IL_0048:  bge.s      IL_004c

      IL_004a:  ldc.i4.m1
      IL_004b:  ret

      IL_004c:  ldloc.2
      IL_004d:  ldloc.3
      IL_004e:  cgt
      IL_0050:  ret
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
      // Code size       76 (0x4c)
      .maxstack  4
      .locals init (valuetype StructUnion01/U V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  StructUnion01/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  pop
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 StructUnion01/U::item1
      IL_000f:  stloc.2
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldfld      int32 StructUnion01/U::item1
      IL_0017:  stloc.3
      IL_0018:  ldloc.2
      IL_0019:  ldloc.3
      IL_001a:  bge.s      IL_0020

      IL_001c:  ldc.i4.m1
      IL_001d:  nop
      IL_001e:  br.s       IL_0025

      IL_0020:  ldloc.2
      IL_0021:  ldloc.3
      IL_0022:  cgt
      IL_0024:  nop
      IL_0025:  stloc.1
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.0
      IL_0028:  bge.s      IL_002c

      IL_002a:  ldloc.1
      IL_002b:  ret

      IL_002c:  ldloc.1
      IL_002d:  ldc.i4.0
      IL_002e:  ble.s      IL_0032

      IL_0030:  ldloc.1
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldfld      int32 StructUnion01/U::item2
      IL_0038:  stloc.2
      IL_0039:  ldloca.s   V_0
      IL_003b:  ldfld      int32 StructUnion01/U::item2
      IL_0040:  stloc.3
      IL_0041:  ldloc.2
      IL_0042:  ldloc.3
      IL_0043:  bge.s      IL_0047

      IL_0045:  ldc.i4.m1
      IL_0046:  ret

      IL_0047:  ldloc.2
      IL_0048:  ldloc.3
      IL_0049:  cgt
      IL_004b:  ret
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
      // Code size       52 (0x34)
      .maxstack  4
      .locals init (valuetype StructUnion01/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     StructUnion01/U
      IL_0006:  brfalse.s  IL_0032

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  StructUnion01/U
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  pop
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 StructUnion01/U::item1
      IL_0017:  ldloca.s   V_0
      IL_0019:  ldfld      int32 StructUnion01/U::item1
      IL_001e:  bne.un.s   IL_0030

      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 StructUnion01/U::item2
      IL_0026:  ldloca.s   V_0
      IL_0028:  ldfld      int32 StructUnion01/U::item2
      IL_002d:  ceq
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldc.i4.0
      IL_0033:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype StructUnion01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       35 (0x23)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 StructUnion01/U::item1
      IL_0008:  ldarga.s   obj
      IL_000a:  ldfld      int32 StructUnion01/U::item1
      IL_000f:  bne.un.s   IL_0021

      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 StructUnion01/U::item2
      IL_0017:  ldarga.s   obj
      IL_0019:  ldfld      int32 StructUnion01/U::item2
      IL_001e:  ceq
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     StructUnion01/U
      IL_0006:  brfalse.s  IL_0015

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  unbox.any  StructUnion01/U
      IL_000f:  call       instance bool StructUnion01/U::Equals(valuetype StructUnion01/U)
      IL_0014:  ret

      IL_0015:  ldc.i4.0
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
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarga.s   u
    IL_0003:  ldfld      int32 StructUnion01/U::item1
    IL_0008:  ldarga.s   u
    IL_000a:  ldfld      int32 StructUnion01/U::item2
    IL_000f:  add
    IL_0010:  ret
  } // end of method StructUnion01::g2

  .method public static int32  g3(valuetype StructUnion01/U x) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      int32 StructUnion01/U::item1
    IL_0008:  ldc.i4.3
    IL_0009:  sub
    IL_000a:  switch     ( 
                          IL_0015)
    IL_0013:  br.s       IL_001d

    IL_0015:  ldarga.s   x
    IL_0017:  ldfld      int32 StructUnion01/U::item2
    IL_001c:  ret

    IL_001d:  ldarga.s   x
    IL_001f:  ldfld      int32 StructUnion01/U::item1
    IL_0024:  ldarga.s   x
    IL_0026:  ldfld      int32 StructUnion01/U::item2
    IL_002b:  add
    IL_002c:  ret
  } // end of method StructUnion01::g3

  .method public static int32  g4(valuetype StructUnion01/U x,
                                  valuetype StructUnion01/U y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       129 (0x81)
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      int32 StructUnion01/U::item1
    IL_0008:  ldc.i4.3
    IL_0009:  sub
    IL_000a:  switch     ( 
                          IL_0015)
    IL_0013:  br.s       IL_0059

    IL_0015:  ldarga.s   y
    IL_0017:  ldfld      int32 StructUnion01/U::item1
    IL_001c:  ldc.i4.5
    IL_001d:  sub
    IL_001e:  switch     ( 
                          IL_0049)
    IL_0027:  ldarga.s   y
    IL_0029:  ldfld      int32 StructUnion01/U::item2
    IL_002e:  ldarga.s   y
    IL_0030:  ldfld      int32 StructUnion01/U::item1
    IL_0035:  ldarga.s   x
    IL_0037:  ldfld      int32 StructUnion01/U::item2
    IL_003c:  ldarga.s   x
    IL_003e:  ldfld      int32 StructUnion01/U::item1
    IL_0043:  stloc.3
    IL_0044:  stloc.2
    IL_0045:  stloc.1
    IL_0046:  stloc.0
    IL_0047:  br.s       IL_0079

    IL_0049:  ldarga.s   x
    IL_004b:  ldfld      int32 StructUnion01/U::item2
    IL_0050:  ldarga.s   y
    IL_0052:  ldfld      int32 StructUnion01/U::item2
    IL_0057:  add
    IL_0058:  ret

    IL_0059:  ldarga.s   y
    IL_005b:  ldfld      int32 StructUnion01/U::item2
    IL_0060:  stloc.0
    IL_0061:  ldarga.s   y
    IL_0063:  ldfld      int32 StructUnion01/U::item1
    IL_0068:  stloc.1
    IL_0069:  ldarga.s   x
    IL_006b:  ldfld      int32 StructUnion01/U::item2
    IL_0070:  stloc.2
    IL_0071:  ldarga.s   x
    IL_0073:  ldfld      int32 StructUnion01/U::item1
    IL_0078:  stloc.3
    IL_0079:  ldloc.3
    IL_007a:  ldloc.2
    IL_007b:  add
    IL_007c:  ldloc.1
    IL_007d:  add
    IL_007e:  ldloc.0
    IL_007f:  add
    IL_0080:  ret
  } // end of method StructUnion01::g4

  .method public static int32  f1(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 StructUnion01/U::item2
    IL_000d:  add
    IL_000e:  ret
  } // end of method StructUnion01::f1

  .method public static int32  f2(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 StructUnion01/U::item2
    IL_000d:  add
    IL_000e:  ret
  } // end of method StructUnion01::f2

  .method public static int32  f3(valuetype StructUnion01/U& x) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 StructUnion01/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     ( 
                          IL_0014)
    IL_0012:  br.s       IL_001b

    IL_0014:  ldarg.0
    IL_0015:  ldfld      int32 StructUnion01/U::item2
    IL_001a:  ret

    IL_001b:  ldarg.0
    IL_001c:  ldfld      int32 StructUnion01/U::item1
    IL_0021:  ldarg.0
    IL_0022:  ldfld      int32 StructUnion01/U::item2
    IL_0027:  add
    IL_0028:  ret
  } // end of method StructUnion01::f3

  .method public static int32  f4(valuetype StructUnion01/U& x,
                                  valuetype StructUnion01/U& y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       149 (0x95)
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
    IL_000e:  nop
    IL_000f:  ldloca.s   V_0
    IL_0011:  ldfld      int32 StructUnion01/U::item1
    IL_0016:  ldc.i4.3
    IL_0017:  sub
    IL_0018:  switch     ( 
                          IL_0023)
    IL_0021:  br.s       IL_0069

    IL_0023:  ldloca.s   V_1
    IL_0025:  ldfld      int32 StructUnion01/U::item1
    IL_002a:  ldc.i4.5
    IL_002b:  sub
    IL_002c:  switch     ( 
                          IL_0059)
    IL_0035:  ldloca.s   V_1
    IL_0037:  ldfld      int32 StructUnion01/U::item2
    IL_003c:  ldloca.s   V_1
    IL_003e:  ldfld      int32 StructUnion01/U::item1
    IL_0043:  ldloca.s   V_0
    IL_0045:  ldfld      int32 StructUnion01/U::item2
    IL_004a:  ldloca.s   V_0
    IL_004c:  ldfld      int32 StructUnion01/U::item1
    IL_0051:  stloc.s    V_5
    IL_0053:  stloc.s    V_4
    IL_0055:  stloc.3
    IL_0056:  stloc.2
    IL_0057:  br.s       IL_008b

    IL_0059:  ldloca.s   V_0
    IL_005b:  ldfld      int32 StructUnion01/U::item2
    IL_0060:  ldloca.s   V_1
    IL_0062:  ldfld      int32 StructUnion01/U::item2
    IL_0067:  add
    IL_0068:  ret

    IL_0069:  ldloca.s   V_1
    IL_006b:  ldfld      int32 StructUnion01/U::item2
    IL_0070:  stloc.2
    IL_0071:  ldloca.s   V_1
    IL_0073:  ldfld      int32 StructUnion01/U::item1
    IL_0078:  stloc.3
    IL_0079:  ldloca.s   V_0
    IL_007b:  ldfld      int32 StructUnion01/U::item2
    IL_0080:  stloc.s    V_4
    IL_0082:  ldloca.s   V_0
    IL_0084:  ldfld      int32 StructUnion01/U::item1
    IL_0089:  stloc.s    V_5
    IL_008b:  ldloc.s    V_5
    IL_008d:  ldloc.s    V_4
    IL_008f:  add
    IL_0090:  ldloc.3
    IL_0091:  add
    IL_0092:  ldloc.2
    IL_0093:  add
    IL_0094:  ret
  } // end of method StructUnion01::f4

} // end of class StructUnion01

.class private abstract auto ansi sealed '<StartupCode$StructUnion01>'.$StructUnion01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $StructUnion01::main@

} // end of class '<StartupCode$StructUnion01>'.$StructUnion01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Inlining\StructUnion01_fs\StructUnion01.res
