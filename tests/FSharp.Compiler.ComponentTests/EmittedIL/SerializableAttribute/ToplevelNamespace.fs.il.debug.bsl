
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
.assembly ToplevelNamespace
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ToplevelNamespace
{
  // Offset: 0x00000000 Length: 0x0000187A
  // WARNING: managed resource file FSharpSignatureData.ToplevelNamespace created
}
.mresource public FSharpOptimizationData.ToplevelNamespace
{
  // Offset: 0x00001880 Length: 0x0000055C
  // WARNING: managed resource file FSharpOptimizationData.ToplevelNamespace created
}
.module ToplevelNamespace.exe
// MVID: {624CBE98-962F-9DFD-A745-038398BE4C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03330000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto autochar serializable sealed beforefieldinit XYZ.Expr
       extends [mscorlib]System.Object
       implements class [mscorlib]System.IEquatable`1<class XYZ.Expr>,
                  [mscorlib]System.Collections.IStructuralEquatable,
                  class [mscorlib]System.IComparable`1<class XYZ.Expr>,
                  [mscorlib]System.IComparable,
                  [mscorlib]System.Collections.IStructuralComparable
{
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                 61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
  .field assembly initonly int32 item
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static class XYZ.Expr  NewNum(int32 item) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void XYZ.Expr::.ctor(int32)
    IL_0006:  ret
  } // end of method Expr::NewNum

  .method assembly specialname rtspecialname 
          instance void  .ctor(int32 item) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 XYZ.Expr::item
    IL_000d:  ret
  } // end of method Expr::.ctor

  .method public hidebysig instance int32 
          get_Item() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 XYZ.Expr::item
    IL_0006:  ret
  } // end of method Expr::get_Item

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
  } // end of method Expr::get_Tag

  .method assembly hidebysig specialname 
          instance object  __DebugDisplay() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldstr      "%+0.8A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>::Invoke(!0)
    IL_0015:  ret
  } // end of method Expr::__DebugDisplay

  .method public strict virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class XYZ.Expr>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.Expr,string>::Invoke(!0)
    IL_0015:  ret
  } // end of method Expr::ToString

  .method public hidebysig virtual final 
          instance int32  CompareTo(class XYZ.Expr obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       67 (0x43)
    .maxstack  4
    .locals init (class XYZ.Expr V_0,
             class XYZ.Expr V_1,
             class [mscorlib]System.Collections.IComparer V_2,
             int32 V_3,
             int32 V_4,
             class [mscorlib]System.Collections.IComparer V_5,
             int32 V_6,
             int32 V_7)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003c

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_003a

    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  stloc.0
    IL_000a:  ldarg.1
    IL_000b:  stloc.1
    IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  ldfld      int32 XYZ.Expr::item
    IL_0018:  stloc.3
    IL_0019:  ldloc.1
    IL_001a:  ldfld      int32 XYZ.Expr::item
    IL_001f:  stloc.s    V_4
    IL_0021:  ldloc.2
    IL_0022:  stloc.s    V_5
    IL_0024:  ldloc.3
    IL_0025:  stloc.s    V_6
    IL_0027:  ldloc.s    V_4
    IL_0029:  stloc.s    V_7
    IL_002b:  ldloc.s    V_6
    IL_002d:  ldloc.s    V_7
    IL_002f:  bge.s      IL_0033

    IL_0031:  ldc.i4.m1
    IL_0032:  ret

    IL_0033:  ldloc.s    V_6
    IL_0035:  ldloc.s    V_7
    IL_0037:  cgt
    IL_0039:  ret

    IL_003a:  ldc.i4.1
    IL_003b:  ret

    IL_003c:  ldarg.1
    IL_003d:  brfalse.s  IL_0041

    IL_003f:  ldc.i4.m1
    IL_0040:  ret

    IL_0041:  ldc.i4.0
    IL_0042:  ret
  } // end of method Expr::CompareTo

  .method public hidebysig virtual final 
          instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  XYZ.Expr
    IL_0007:  callvirt   instance int32 XYZ.Expr::CompareTo(class XYZ.Expr)
    IL_000c:  ret
  } // end of method Expr::CompareTo

  .method public hidebysig virtual final 
          instance int32  CompareTo(object obj,
                                    class [mscorlib]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       82 (0x52)
    .maxstack  4
    .locals init (class XYZ.Expr V_0,
             class XYZ.Expr V_1,
             class XYZ.Expr V_2,
             class [mscorlib]System.Collections.IComparer V_3,
             int32 V_4,
             int32 V_5,
             class [mscorlib]System.Collections.IComparer V_6,
             int32 V_7,
             int32 V_8)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  XYZ.Expr
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  brfalse.s  IL_0046

    IL_000a:  ldarg.1
    IL_000b:  unbox.any  XYZ.Expr
    IL_0010:  brfalse.s  IL_0044

    IL_0012:  ldarg.0
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.0
    IL_0017:  stloc.2
    IL_0018:  ldarg.2
    IL_0019:  stloc.3
    IL_001a:  ldloc.1
    IL_001b:  ldfld      int32 XYZ.Expr::item
    IL_0020:  stloc.s    V_4
    IL_0022:  ldloc.2
    IL_0023:  ldfld      int32 XYZ.Expr::item
    IL_0028:  stloc.s    V_5
    IL_002a:  ldloc.3
    IL_002b:  stloc.s    V_6
    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_7
    IL_0031:  ldloc.s    V_5
    IL_0033:  stloc.s    V_8
    IL_0035:  ldloc.s    V_7
    IL_0037:  ldloc.s    V_8
    IL_0039:  bge.s      IL_003d

    IL_003b:  ldc.i4.m1
    IL_003c:  ret

    IL_003d:  ldloc.s    V_7
    IL_003f:  ldloc.s    V_8
    IL_0041:  cgt
    IL_0043:  ret

    IL_0044:  ldc.i4.1
    IL_0045:  ret

    IL_0046:  ldarg.1
    IL_0047:  unbox.any  XYZ.Expr
    IL_004c:  brfalse.s  IL_0050

    IL_004e:  ldc.i4.m1
    IL_004f:  ret

    IL_0050:  ldc.i4.0
    IL_0051:  ret
  } // end of method Expr::CompareTo

  .method public hidebysig virtual final 
          instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       43 (0x2b)
    .maxstack  7
    .locals init (int32 V_0,
             class XYZ.Expr V_1,
             class [mscorlib]System.Collections.IEqualityComparer V_2,
             int32 V_3,
             class [mscorlib]System.Collections.IEqualityComparer V_4)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0029

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
    IL_0013:  ldfld      int32 XYZ.Expr::item
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
    IL_0027:  ldloc.0
    IL_0028:  ret

    IL_0029:  ldc.i4.0
    IL_002a:  ret
  } // end of method Expr::GetHashCode

  .method public hidebysig virtual final 
          instance int32  GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  callvirt   instance int32 XYZ.Expr::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
    IL_000b:  ret
  } // end of method Expr::GetHashCode

  .method public hidebysig virtual final 
          instance bool  Equals(object obj,
                                class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       61 (0x3d)
    .maxstack  4
    .locals init (class XYZ.Expr V_0,
             class XYZ.Expr V_1,
             class XYZ.Expr V_2,
             class XYZ.Expr V_3,
             class [mscorlib]System.Collections.IEqualityComparer V_4,
             int32 V_5,
             int32 V_6,
             class [mscorlib]System.Collections.IEqualityComparer V_7)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0035

    IL_0003:  ldarg.1
    IL_0004:  isinst     XYZ.Expr
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  brfalse.s  IL_0033

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
    IL_0019:  ldfld      int32 XYZ.Expr::item
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.3
    IL_0021:  ldfld      int32 XYZ.Expr::item
    IL_0026:  stloc.s    V_6
    IL_0028:  ldloc.s    V_4
    IL_002a:  stloc.s    V_7
    IL_002c:  ldloc.s    V_5
    IL_002e:  ldloc.s    V_6
    IL_0030:  ceq
    IL_0032:  ret

    IL_0033:  ldc.i4.0
    IL_0034:  ret

    IL_0035:  ldarg.1
    IL_0036:  ldnull
    IL_0037:  cgt.un
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  ret
  } // end of method Expr::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(class XYZ.Expr obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (class XYZ.Expr V_0,
             class XYZ.Expr V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001d

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001b

    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  stloc.0
    IL_000a:  ldarg.1
    IL_000b:  stloc.1
    IL_000c:  ldloc.0
    IL_000d:  ldfld      int32 XYZ.Expr::item
    IL_0012:  ldloc.1
    IL_0013:  ldfld      int32 XYZ.Expr::item
    IL_0018:  ceq
    IL_001a:  ret

    IL_001b:  ldc.i4.0
    IL_001c:  ret

    IL_001d:  ldarg.1
    IL_001e:  ldnull
    IL_001f:  cgt.un
    IL_0021:  ldc.i4.0
    IL_0022:  ceq
    IL_0024:  ret
  } // end of method Expr::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       20 (0x14)
    .maxstack  4
    .locals init (class XYZ.Expr V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     XYZ.Expr
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt   instance bool XYZ.Expr::Equals(class XYZ.Expr)
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } // end of method Expr::Equals

  .property instance int32 Tag()
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .get instance int32 XYZ.Expr::get_Tag()
  } // end of property Expr::Tag
  .property instance int32 Item()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance int32 XYZ.Expr::get_Item()
  } // end of property Expr::Item
} // end of class XYZ.Expr

.class public auto ansi serializable beforefieldinit XYZ.MyExn
       extends [mscorlib]System.Exception
       implements [mscorlib]System.Collections.IStructuralEquatable
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
  .field assembly int32 Data0@
  .method public specialname rtspecialname 
          instance void  .ctor(int32 data0) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 XYZ.MyExn::Data0@
    IL_000d:  ret
  } // end of method MyExn::.ctor

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
    IL_0006:  ret
  } // end of method MyExn::.ctor

  .method family specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo info,
                               valuetype [mscorlib]System.Runtime.Serialization.StreamingContext context) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  call       instance void [mscorlib]System.Exception::.ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo,
                                                                         valuetype [mscorlib]System.Runtime.Serialization.StreamingContext)
    IL_0008:  ret
  } // end of method MyExn::.ctor

  .method public hidebysig specialname instance int32 
          get_Data0() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 XYZ.MyExn::Data0@
    IL_0006:  ret
  } // end of method MyExn::get_Data0

  .method public hidebysig virtual instance int32 
          GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  7
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.IEqualityComparer V_1,
             int32 V_2,
             class [mscorlib]System.Collections.IEqualityComparer V_3)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0027

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  stloc.1
    IL_000c:  ldarg.0
    IL_000d:  castclass  XYZ.MyExn
    IL_0012:  call       instance int32 XYZ.MyExn::get_Data0()
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  stloc.3
    IL_001a:  ldloc.2
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.6
    IL_001d:  shl
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.2
    IL_0020:  shr
    IL_0021:  add
    IL_0022:  add
    IL_0023:  add
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  ret

    IL_0027:  ldc.i4.0
    IL_0028:  ret
  } // end of method MyExn::GetHashCode

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  callvirt   instance int32 XYZ.MyExn::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
    IL_000b:  ret
  } // end of method MyExn::GetHashCode

  .method public hidebysig virtual instance bool 
          Equals(object obj,
                 class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       78 (0x4e)
    .maxstack  4
    .locals init (class [mscorlib]System.Exception V_0,
             class [mscorlib]System.Exception V_1,
             object V_2,
             class [mscorlib]System.Collections.IEqualityComparer V_3,
             int32 V_4,
             int32 V_5,
             class [mscorlib]System.Collections.IEqualityComparer V_6)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0046

    IL_0003:  ldarg.1
    IL_0004:  isinst     [mscorlib]System.Exception
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  brfalse.s  IL_0044

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldloc.0
    IL_0010:  stloc.2
    IL_0011:  ldloc.2
    IL_0012:  isinst     XYZ.MyExn
    IL_0017:  ldnull
    IL_0018:  cgt.un
    IL_001a:  brfalse.s  IL_0042

    IL_001c:  ldarg.2
    IL_001d:  stloc.3
    IL_001e:  ldarg.0
    IL_001f:  castclass  XYZ.MyExn
    IL_0024:  call       instance int32 XYZ.MyExn::get_Data0()
    IL_0029:  stloc.s    V_4
    IL_002b:  ldloc.1
    IL_002c:  castclass  XYZ.MyExn
    IL_0031:  call       instance int32 XYZ.MyExn::get_Data0()
    IL_0036:  stloc.s    V_5
    IL_0038:  ldloc.3
    IL_0039:  stloc.s    V_6
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldloc.s    V_5
    IL_003f:  ceq
    IL_0041:  ret

    IL_0042:  ldc.i4.0
    IL_0043:  ret

    IL_0044:  ldc.i4.0
    IL_0045:  ret

    IL_0046:  ldarg.1
    IL_0047:  ldnull
    IL_0048:  cgt.un
    IL_004a:  ldc.i4.0
    IL_004b:  ceq
    IL_004d:  ret
  } // end of method MyExn::Equals

  .method public hidebysig instance bool 
          Equals(class [mscorlib]System.Exception obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       56 (0x38)
    .maxstack  4
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0030

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002e

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  isinst     XYZ.MyExn
    IL_000e:  ldnull
    IL_000f:  cgt.un
    IL_0011:  brfalse.s  IL_002c

    IL_0013:  ldarg.0
    IL_0014:  castclass  XYZ.MyExn
    IL_0019:  call       instance int32 XYZ.MyExn::get_Data0()
    IL_001e:  ldarg.1
    IL_001f:  castclass  XYZ.MyExn
    IL_0024:  call       instance int32 XYZ.MyExn::get_Data0()
    IL_0029:  ceq
    IL_002b:  ret

    IL_002c:  ldc.i4.0
    IL_002d:  ret

    IL_002e:  ldc.i4.0
    IL_002f:  ret

    IL_0030:  ldarg.1
    IL_0031:  ldnull
    IL_0032:  cgt.un
    IL_0034:  ldc.i4.0
    IL_0035:  ceq
    IL_0037:  ret
  } // end of method MyExn::Equals

  .method public hidebysig virtual instance bool 
          Equals(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       20 (0x14)
    .maxstack  4
    .locals init (class [mscorlib]System.Exception V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     [mscorlib]System.Exception
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt   instance bool XYZ.MyExn::Equals(class [mscorlib]System.Exception)
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } // end of method MyExn::Equals

  .property instance int32 Data0()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance int32 XYZ.MyExn::get_Data0()
  } // end of property MyExn::Data0
} // end of class XYZ.MyExn

.class public auto ansi serializable XYZ.A
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field assembly string x
  .method public specialname rtspecialname 
          instance void  .ctor(string x) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  stfld      string XYZ.A::x
    IL_000f:  ret
  } // end of method A::.ctor

  .method public hidebysig specialname instance string 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string XYZ.A::x
    IL_0006:  ret
  } // end of method A::get_X

  .property instance string X()
  {
    .get instance string XYZ.A::get_X()
  } // end of property A::X
} // end of class XYZ.A

.class public abstract auto ansi sealed XYZ.ABC
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Expr
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class XYZ.ABC/Expr>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class XYZ.ABC/Expr>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static class XYZ.ABC/Expr 
            NewNum(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void XYZ.ABC/Expr::.ctor(int32)
      IL_0006:  ret
    } // end of method Expr::NewNum

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 item) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 XYZ.ABC/Expr::item
      IL_000d:  ret
    } // end of method Expr::.ctor

    .method public hidebysig instance int32 
            get_Item() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0006:  ret
    } // end of method Expr::get_Item

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
    } // end of method Expr::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Expr::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class XYZ.ABC/Expr>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/Expr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Expr::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class XYZ.ABC/Expr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       67 (0x43)
      .maxstack  4
      .locals init (class XYZ.ABC/Expr V_0,
               class XYZ.ABC/Expr V_1,
               class [mscorlib]System.Collections.IComparer V_2,
               int32 V_3,
               int32 V_4,
               class [mscorlib]System.Collections.IComparer V_5,
               int32 V_6,
               int32 V_7)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_003c

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_003a

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.2
      IL_0012:  ldloc.0
      IL_0013:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0018:  stloc.3
      IL_0019:  ldloc.1
      IL_001a:  ldfld      int32 XYZ.ABC/Expr::item
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.2
      IL_0022:  stloc.s    V_5
      IL_0024:  ldloc.3
      IL_0025:  stloc.s    V_6
      IL_0027:  ldloc.s    V_4
      IL_0029:  stloc.s    V_7
      IL_002b:  ldloc.s    V_6
      IL_002d:  ldloc.s    V_7
      IL_002f:  bge.s      IL_0033

      IL_0031:  ldc.i4.m1
      IL_0032:  ret

      IL_0033:  ldloc.s    V_6
      IL_0035:  ldloc.s    V_7
      IL_0037:  cgt
      IL_0039:  ret

      IL_003a:  ldc.i4.1
      IL_003b:  ret

      IL_003c:  ldarg.1
      IL_003d:  brfalse.s  IL_0041

      IL_003f:  ldc.i4.m1
      IL_0040:  ret

      IL_0041:  ldc.i4.0
      IL_0042:  ret
    } // end of method Expr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  XYZ.ABC/Expr
      IL_0007:  callvirt   instance int32 XYZ.ABC/Expr::CompareTo(class XYZ.ABC/Expr)
      IL_000c:  ret
    } // end of method Expr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       82 (0x52)
      .maxstack  4
      .locals init (class XYZ.ABC/Expr V_0,
               class XYZ.ABC/Expr V_1,
               class XYZ.ABC/Expr V_2,
               class [mscorlib]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [mscorlib]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  XYZ.ABC/Expr
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0046

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  XYZ.ABC/Expr
      IL_0010:  brfalse.s  IL_0044

      IL_0012:  ldarg.0
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  stloc.1
      IL_0016:  ldloc.0
      IL_0017:  stloc.2
      IL_0018:  ldarg.2
      IL_0019:  stloc.3
      IL_001a:  ldloc.1
      IL_001b:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0020:  stloc.s    V_4
      IL_0022:  ldloc.2
      IL_0023:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0028:  stloc.s    V_5
      IL_002a:  ldloc.3
      IL_002b:  stloc.s    V_6
      IL_002d:  ldloc.s    V_4
      IL_002f:  stloc.s    V_7
      IL_0031:  ldloc.s    V_5
      IL_0033:  stloc.s    V_8
      IL_0035:  ldloc.s    V_7
      IL_0037:  ldloc.s    V_8
      IL_0039:  bge.s      IL_003d

      IL_003b:  ldc.i4.m1
      IL_003c:  ret

      IL_003d:  ldloc.s    V_7
      IL_003f:  ldloc.s    V_8
      IL_0041:  cgt
      IL_0043:  ret

      IL_0044:  ldc.i4.1
      IL_0045:  ret

      IL_0046:  ldarg.1
      IL_0047:  unbox.any  XYZ.ABC/Expr
      IL_004c:  brfalse.s  IL_0050

      IL_004e:  ldc.i4.m1
      IL_004f:  ret

      IL_0050:  ldc.i4.0
      IL_0051:  ret
    } // end of method Expr::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       43 (0x2b)
      .maxstack  7
      .locals init (int32 V_0,
               class XYZ.ABC/Expr V_1,
               class [mscorlib]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0029

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
      IL_0013:  ldfld      int32 XYZ.ABC/Expr::item
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
      IL_0027:  ldloc.0
      IL_0028:  ret

      IL_0029:  ldc.i4.0
      IL_002a:  ret
    } // end of method Expr::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 XYZ.ABC/Expr::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Expr::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       61 (0x3d)
      .maxstack  4
      .locals init (class XYZ.ABC/Expr V_0,
               class XYZ.ABC/Expr V_1,
               class XYZ.ABC/Expr V_2,
               class XYZ.ABC/Expr V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4,
               int32 V_5,
               int32 V_6,
               class [mscorlib]System.Collections.IEqualityComparer V_7)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0035

      IL_0003:  ldarg.1
      IL_0004:  isinst     XYZ.ABC/Expr
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0033

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
      IL_0019:  ldfld      int32 XYZ.ABC/Expr::item
      IL_001e:  stloc.s    V_5
      IL_0020:  ldloc.3
      IL_0021:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0026:  stloc.s    V_6
      IL_0028:  ldloc.s    V_4
      IL_002a:  stloc.s    V_7
      IL_002c:  ldloc.s    V_5
      IL_002e:  ldloc.s    V_6
      IL_0030:  ceq
      IL_0032:  ret

      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldarg.1
      IL_0036:  ldnull
      IL_0037:  cgt.un
      IL_0039:  ldc.i4.0
      IL_003a:  ceq
      IL_003c:  ret
    } // end of method Expr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class XYZ.ABC/Expr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       37 (0x25)
      .maxstack  4
      .locals init (class XYZ.ABC/Expr V_0,
               class XYZ.ABC/Expr V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001d

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001b

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldloc.0
      IL_000d:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 XYZ.ABC/Expr::item
      IL_0018:  ceq
      IL_001a:  ret

      IL_001b:  ldc.i4.0
      IL_001c:  ret

      IL_001d:  ldarg.1
      IL_001e:  ldnull
      IL_001f:  cgt.un
      IL_0021:  ldc.i4.0
      IL_0022:  ceq
      IL_0024:  ret
    } // end of method Expr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class XYZ.ABC/Expr V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     XYZ.ABC/Expr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool XYZ.ABC/Expr::Equals(class XYZ.ABC/Expr)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Expr::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 XYZ.ABC/Expr::get_Tag()
    } // end of property Expr::Tag
    .property instance int32 Item()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 XYZ.ABC/Expr::get_Item()
    } // end of property Expr::Item
  } // end of class Expr

  .class auto ansi serializable nested public beforefieldinit MyExn
         extends [mscorlib]System.Exception
         implements [mscorlib]System.Collections.IStructuralEquatable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .field assembly int32 Data0@
    .method public specialname rtspecialname 
            instance void  .ctor(int32 data0) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 XYZ.ABC/MyExn::Data0@
      IL_000d:  ret
    } // end of method MyExn::.ctor

    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
      IL_0006:  ret
    } // end of method MyExn::.ctor

    .method family specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo info,
                                 valuetype [mscorlib]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [mscorlib]System.Exception::.ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo,
                                                                           valuetype [mscorlib]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } // end of method MyExn::.ctor

    .method public hidebysig specialname 
            instance int32  get_Data0() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 XYZ.ABC/MyExn::Data0@
      IL_0006:  ret
    } // end of method MyExn::get_Data0

    .method public hidebysig virtual instance int32 
            GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       41 (0x29)
      .maxstack  7
      .locals init (int32 V_0,
               class [mscorlib]System.Collections.IEqualityComparer V_1,
               int32 V_2,
               class [mscorlib]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  castclass  XYZ.ABC/MyExn
      IL_0012:  call       instance int32 XYZ.ABC/MyExn::get_Data0()
      IL_0017:  stloc.2
      IL_0018:  ldloc.1
      IL_0019:  stloc.3
      IL_001a:  ldloc.2
      IL_001b:  ldloc.0
      IL_001c:  ldc.i4.6
      IL_001d:  shl
      IL_001e:  ldloc.0
      IL_001f:  ldc.i4.2
      IL_0020:  shr
      IL_0021:  add
      IL_0022:  add
      IL_0023:  add
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  ret

      IL_0027:  ldc.i4.0
      IL_0028:  ret
    } // end of method MyExn::GetHashCode

    .method public hidebysig virtual instance int32 
            GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 XYZ.ABC/MyExn::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method MyExn::GetHashCode

    .method public hidebysig virtual instance bool 
            Equals(object obj,
                   class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       78 (0x4e)
      .maxstack  4
      .locals init (class [mscorlib]System.Exception V_0,
               class [mscorlib]System.Exception V_1,
               object V_2,
               class [mscorlib]System.Collections.IEqualityComparer V_3,
               int32 V_4,
               int32 V_5,
               class [mscorlib]System.Collections.IEqualityComparer V_6)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0046

      IL_0003:  ldarg.1
      IL_0004:  isinst     [mscorlib]System.Exception
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0044

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldloc.0
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  isinst     XYZ.ABC/MyExn
      IL_0017:  ldnull
      IL_0018:  cgt.un
      IL_001a:  brfalse.s  IL_0042

      IL_001c:  ldarg.2
      IL_001d:  stloc.3
      IL_001e:  ldarg.0
      IL_001f:  castclass  XYZ.ABC/MyExn
      IL_0024:  call       instance int32 XYZ.ABC/MyExn::get_Data0()
      IL_0029:  stloc.s    V_4
      IL_002b:  ldloc.1
      IL_002c:  castclass  XYZ.ABC/MyExn
      IL_0031:  call       instance int32 XYZ.ABC/MyExn::get_Data0()
      IL_0036:  stloc.s    V_5
      IL_0038:  ldloc.3
      IL_0039:  stloc.s    V_6
      IL_003b:  ldloc.s    V_4
      IL_003d:  ldloc.s    V_5
      IL_003f:  ceq
      IL_0041:  ret

      IL_0042:  ldc.i4.0
      IL_0043:  ret

      IL_0044:  ldc.i4.0
      IL_0045:  ret

      IL_0046:  ldarg.1
      IL_0047:  ldnull
      IL_0048:  cgt.un
      IL_004a:  ldc.i4.0
      IL_004b:  ceq
      IL_004d:  ret
    } // end of method MyExn::Equals

    .method public hidebysig instance bool 
            Equals(class [mscorlib]System.Exception obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       56 (0x38)
      .maxstack  4
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0030

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002e

      IL_0006:  ldarg.1
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  isinst     XYZ.ABC/MyExn
      IL_000e:  ldnull
      IL_000f:  cgt.un
      IL_0011:  brfalse.s  IL_002c

      IL_0013:  ldarg.0
      IL_0014:  castclass  XYZ.ABC/MyExn
      IL_0019:  call       instance int32 XYZ.ABC/MyExn::get_Data0()
      IL_001e:  ldarg.1
      IL_001f:  castclass  XYZ.ABC/MyExn
      IL_0024:  call       instance int32 XYZ.ABC/MyExn::get_Data0()
      IL_0029:  ceq
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret

      IL_0030:  ldarg.1
      IL_0031:  ldnull
      IL_0032:  cgt.un
      IL_0034:  ldc.i4.0
      IL_0035:  ceq
      IL_0037:  ret
    } // end of method MyExn::Equals

    .method public hidebysig virtual instance bool 
            Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class [mscorlib]System.Exception V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     [mscorlib]System.Exception
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool XYZ.ABC/MyExn::Equals(class [mscorlib]System.Exception)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method MyExn::Equals

    .property instance int32 Data0()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 XYZ.ABC/MyExn::get_Data0()
    } // end of property MyExn::Data0
  } // end of class MyExn

  .class auto ansi serializable nested public A
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly string x
    .method public specialname rtspecialname 
            instance void  .ctor(string x) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      string XYZ.ABC/A::x
      IL_000f:  ret
    } // end of method A::.ctor

    .method public hidebysig specialname 
            instance string  get_X() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string XYZ.ABC/A::x
      IL_0006:  ret
    } // end of method A::get_X

    .property instance string X()
    {
      .get instance string XYZ.ABC/A::get_X()
    } // end of property A::X
  } // end of class A

  .class abstract auto ansi sealed nested public ABC
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Expr
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class XYZ.ABC/ABC/Expr>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class XYZ.ABC/ABC/Expr>,
                      [mscorlib]System.IComparable,
                      [mscorlib]System.Collections.IStructuralComparable
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly int32 item
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class XYZ.ABC/ABC/Expr 
              NewNum(int32 item) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  newobj     instance void XYZ.ABC/ABC/Expr::.ctor(int32)
        IL_0006:  ret
      } // end of method Expr::NewNum

      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 XYZ.ABC/ABC/Expr::item
        IL_000d:  ret
      } // end of method Expr::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0006:  ret
      } // end of method Expr::get_Item

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
      } // end of method Expr::get_Tag

      .method assembly hidebysig specialname 
              instance object  __DebugDisplay() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Expr::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class XYZ.ABC/ABC/Expr>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class XYZ.ABC/ABC/Expr,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Expr::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class XYZ.ABC/ABC/Expr obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       67 (0x43)
        .maxstack  4
        .locals init (class XYZ.ABC/ABC/Expr V_0,
                 class XYZ.ABC/ABC/Expr V_1,
                 class [mscorlib]System.Collections.IComparer V_2,
                 int32 V_3,
                 int32 V_4,
                 class [mscorlib]System.Collections.IComparer V_5,
                 int32 V_6,
                 int32 V_7)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_003c

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_003a

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.2
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0018:  stloc.3
        IL_0019:  ldloc.1
        IL_001a:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_001f:  stloc.s    V_4
        IL_0021:  ldloc.2
        IL_0022:  stloc.s    V_5
        IL_0024:  ldloc.3
        IL_0025:  stloc.s    V_6
        IL_0027:  ldloc.s    V_4
        IL_0029:  stloc.s    V_7
        IL_002b:  ldloc.s    V_6
        IL_002d:  ldloc.s    V_7
        IL_002f:  bge.s      IL_0033

        IL_0031:  ldc.i4.m1
        IL_0032:  ret

        IL_0033:  ldloc.s    V_6
        IL_0035:  ldloc.s    V_7
        IL_0037:  cgt
        IL_0039:  ret

        IL_003a:  ldc.i4.1
        IL_003b:  ret

        IL_003c:  ldarg.1
        IL_003d:  brfalse.s  IL_0041

        IL_003f:  ldc.i4.m1
        IL_0040:  ret

        IL_0041:  ldc.i4.0
        IL_0042:  ret
      } // end of method Expr::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  XYZ.ABC/ABC/Expr
        IL_0007:  callvirt   instance int32 XYZ.ABC/ABC/Expr::CompareTo(class XYZ.ABC/ABC/Expr)
        IL_000c:  ret
      } // end of method Expr::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       82 (0x52)
        .maxstack  4
        .locals init (class XYZ.ABC/ABC/Expr V_0,
                 class XYZ.ABC/ABC/Expr V_1,
                 class XYZ.ABC/ABC/Expr V_2,
                 class [mscorlib]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5,
                 class [mscorlib]System.Collections.IComparer V_6,
                 int32 V_7,
                 int32 V_8)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  XYZ.ABC/ABC/Expr
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0046

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  XYZ.ABC/ABC/Expr
        IL_0010:  brfalse.s  IL_0044

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldarg.2
        IL_0019:  stloc.3
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0020:  stloc.s    V_4
        IL_0022:  ldloc.2
        IL_0023:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0028:  stloc.s    V_5
        IL_002a:  ldloc.3
        IL_002b:  stloc.s    V_6
        IL_002d:  ldloc.s    V_4
        IL_002f:  stloc.s    V_7
        IL_0031:  ldloc.s    V_5
        IL_0033:  stloc.s    V_8
        IL_0035:  ldloc.s    V_7
        IL_0037:  ldloc.s    V_8
        IL_0039:  bge.s      IL_003d

        IL_003b:  ldc.i4.m1
        IL_003c:  ret

        IL_003d:  ldloc.s    V_7
        IL_003f:  ldloc.s    V_8
        IL_0041:  cgt
        IL_0043:  ret

        IL_0044:  ldc.i4.1
        IL_0045:  ret

        IL_0046:  ldarg.1
        IL_0047:  unbox.any  XYZ.ABC/ABC/Expr
        IL_004c:  brfalse.s  IL_0050

        IL_004e:  ldc.i4.m1
        IL_004f:  ret

        IL_0050:  ldc.i4.0
        IL_0051:  ret
      } // end of method Expr::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       43 (0x2b)
        .maxstack  7
        .locals init (int32 V_0,
                 class XYZ.ABC/ABC/Expr V_1,
                 class [mscorlib]System.Collections.IEqualityComparer V_2,
                 int32 V_3,
                 class [mscorlib]System.Collections.IEqualityComparer V_4)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0029

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
        IL_0013:  ldfld      int32 XYZ.ABC/ABC/Expr::item
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
        IL_0027:  ldloc.0
        IL_0028:  ret

        IL_0029:  ldc.i4.0
        IL_002a:  ret
      } // end of method Expr::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 XYZ.ABC/ABC/Expr::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method Expr::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       61 (0x3d)
        .maxstack  4
        .locals init (class XYZ.ABC/ABC/Expr V_0,
                 class XYZ.ABC/ABC/Expr V_1,
                 class XYZ.ABC/ABC/Expr V_2,
                 class XYZ.ABC/ABC/Expr V_3,
                 class [mscorlib]System.Collections.IEqualityComparer V_4,
                 int32 V_5,
                 int32 V_6,
                 class [mscorlib]System.Collections.IEqualityComparer V_7)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0035

        IL_0003:  ldarg.1
        IL_0004:  isinst     XYZ.ABC/ABC/Expr
        IL_0009:  stloc.0
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_0033

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
        IL_0019:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_001e:  stloc.s    V_5
        IL_0020:  ldloc.3
        IL_0021:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0026:  stloc.s    V_6
        IL_0028:  ldloc.s    V_4
        IL_002a:  stloc.s    V_7
        IL_002c:  ldloc.s    V_5
        IL_002e:  ldloc.s    V_6
        IL_0030:  ceq
        IL_0032:  ret

        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldarg.1
        IL_0036:  ldnull
        IL_0037:  cgt.un
        IL_0039:  ldc.i4.0
        IL_003a:  ceq
        IL_003c:  ret
      } // end of method Expr::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class XYZ.ABC/ABC/Expr obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       37 (0x25)
        .maxstack  4
        .locals init (class XYZ.ABC/ABC/Expr V_0,
                 class XYZ.ABC/ABC/Expr V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_001d

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_001b

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 XYZ.ABC/ABC/Expr::item
        IL_0018:  ceq
        IL_001a:  ret

        IL_001b:  ldc.i4.0
        IL_001c:  ret

        IL_001d:  ldarg.1
        IL_001e:  ldnull
        IL_001f:  cgt.un
        IL_0021:  ldc.i4.0
        IL_0022:  ceq
        IL_0024:  ret
      } // end of method Expr::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init (class XYZ.ABC/ABC/Expr V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     XYZ.ABC/ABC/Expr
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool XYZ.ABC/ABC/Expr::Equals(class XYZ.ABC/ABC/Expr)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method Expr::Equals

      .property instance int32 Tag()
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 XYZ.ABC/ABC/Expr::get_Tag()
      } // end of property Expr::Tag
      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 XYZ.ABC/ABC/Expr::get_Item()
      } // end of property Expr::Item
    } // end of class Expr

    .class auto ansi serializable nested public beforefieldinit MyExn
           extends [mscorlib]System.Exception
           implements [mscorlib]System.Collections.IStructuralEquatable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
      .field assembly int32 Data0@
      .method public specialname rtspecialname 
              instance void  .ctor(int32 data0) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 XYZ.ABC/ABC/MyExn::Data0@
        IL_000d:  ret
      } // end of method MyExn::.ctor

      .method public specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
        IL_0006:  ret
      } // end of method MyExn::.ctor

      .method family specialname rtspecialname 
              instance void  .ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo info,
                                   valuetype [mscorlib]System.Runtime.Serialization.StreamingContext context) cil managed
      {
        // Code size       9 (0x9)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  ldarg.2
        IL_0003:  call       instance void [mscorlib]System.Exception::.ctor(class [mscorlib]System.Runtime.Serialization.SerializationInfo,
                                                                             valuetype [mscorlib]System.Runtime.Serialization.StreamingContext)
        IL_0008:  ret
      } // end of method MyExn::.ctor

      .method public hidebysig specialname 
              instance int32  get_Data0() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 XYZ.ABC/ABC/MyExn::Data0@
        IL_0006:  ret
      } // end of method MyExn::get_Data0

      .method public hidebysig virtual instance int32 
              GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       41 (0x29)
        .maxstack  7
        .locals init (int32 V_0,
                 class [mscorlib]System.Collections.IEqualityComparer V_1,
                 int32 V_2,
                 class [mscorlib]System.Collections.IEqualityComparer V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldc.i4     0x9e3779b9
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  castclass  XYZ.ABC/ABC/MyExn
        IL_0012:  call       instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  stloc.3
        IL_001a:  ldloc.2
        IL_001b:  ldloc.0
        IL_001c:  ldc.i4.6
        IL_001d:  shl
        IL_001e:  ldloc.0
        IL_001f:  ldc.i4.2
        IL_0020:  shr
        IL_0021:  add
        IL_0022:  add
        IL_0023:  add
        IL_0024:  stloc.0
        IL_0025:  ldloc.0
        IL_0026:  ret

        IL_0027:  ldc.i4.0
        IL_0028:  ret
      } // end of method MyExn::GetHashCode

      .method public hidebysig virtual instance int32 
              GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 XYZ.ABC/ABC/MyExn::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method MyExn::GetHashCode

      .method public hidebysig virtual instance bool 
              Equals(object obj,
                     class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       78 (0x4e)
        .maxstack  4
        .locals init (class [mscorlib]System.Exception V_0,
                 class [mscorlib]System.Exception V_1,
                 object V_2,
                 class [mscorlib]System.Collections.IEqualityComparer V_3,
                 int32 V_4,
                 int32 V_5,
                 class [mscorlib]System.Collections.IEqualityComparer V_6)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0046

        IL_0003:  ldarg.1
        IL_0004:  isinst     [mscorlib]System.Exception
        IL_0009:  stloc.0
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_0044

        IL_000d:  ldloc.0
        IL_000e:  stloc.1
        IL_000f:  ldloc.0
        IL_0010:  stloc.2
        IL_0011:  ldloc.2
        IL_0012:  isinst     XYZ.ABC/ABC/MyExn
        IL_0017:  ldnull
        IL_0018:  cgt.un
        IL_001a:  brfalse.s  IL_0042

        IL_001c:  ldarg.2
        IL_001d:  stloc.3
        IL_001e:  ldarg.0
        IL_001f:  castclass  XYZ.ABC/ABC/MyExn
        IL_0024:  call       instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
        IL_0029:  stloc.s    V_4
        IL_002b:  ldloc.1
        IL_002c:  castclass  XYZ.ABC/ABC/MyExn
        IL_0031:  call       instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
        IL_0036:  stloc.s    V_5
        IL_0038:  ldloc.3
        IL_0039:  stloc.s    V_6
        IL_003b:  ldloc.s    V_4
        IL_003d:  ldloc.s    V_5
        IL_003f:  ceq
        IL_0041:  ret

        IL_0042:  ldc.i4.0
        IL_0043:  ret

        IL_0044:  ldc.i4.0
        IL_0045:  ret

        IL_0046:  ldarg.1
        IL_0047:  ldnull
        IL_0048:  cgt.un
        IL_004a:  ldc.i4.0
        IL_004b:  ceq
        IL_004d:  ret
      } // end of method MyExn::Equals

      .method public hidebysig instance bool 
              Equals(class [mscorlib]System.Exception obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       56 (0x38)
        .maxstack  4
        .locals init (object V_0)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0030

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002e

        IL_0006:  ldarg.1
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  isinst     XYZ.ABC/ABC/MyExn
        IL_000e:  ldnull
        IL_000f:  cgt.un
        IL_0011:  brfalse.s  IL_002c

        IL_0013:  ldarg.0
        IL_0014:  castclass  XYZ.ABC/ABC/MyExn
        IL_0019:  call       instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
        IL_001e:  ldarg.1
        IL_001f:  castclass  XYZ.ABC/ABC/MyExn
        IL_0024:  call       instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
        IL_0029:  ceq
        IL_002b:  ret

        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldc.i4.0
        IL_002f:  ret

        IL_0030:  ldarg.1
        IL_0031:  ldnull
        IL_0032:  cgt.un
        IL_0034:  ldc.i4.0
        IL_0035:  ceq
        IL_0037:  ret
      } // end of method MyExn::Equals

      .method public hidebysig virtual instance bool 
              Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init (class [mscorlib]System.Exception V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     [mscorlib]System.Exception
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool XYZ.ABC/ABC/MyExn::Equals(class [mscorlib]System.Exception)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method MyExn::Equals

      .property instance int32 Data0()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 XYZ.ABC/ABC/MyExn::get_Data0()
      } // end of property MyExn::Data0
    } // end of class MyExn

    .class auto ansi serializable nested public A
           extends [mscorlib]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field assembly string x
      .method public specialname rtspecialname 
              instance void  .ctor(string x) cil managed
      {
        // Code size       16 (0x10)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  stfld      string XYZ.ABC/ABC/A::x
        IL_000f:  ret
      } // end of method A::.ctor

      .method public hidebysig specialname 
              instance string  get_X() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      string XYZ.ABC/ABC/A::x
        IL_0006:  ret
      } // end of method A::get_X

      .property instance string X()
      {
        .get instance string XYZ.ABC/ABC/A::get_X()
      } // end of property A::X
    } // end of class A

    .method public static int32  'add'(int32 x,
                                       int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } // end of method ABC::'add'

    .method public specialname static string 
            get_greeting() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldstr      "hello"
      IL_0005:  ret
    } // end of method ABC::get_greeting

    .property string greeting()
    {
      .get string XYZ.ABC/ABC::get_greeting()
    } // end of property ABC::greeting
  } // end of class ABC

  .method public static int32  'add'(int32 x,
                                     int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } // end of method ABC::'add'

  .method public specialname static string 
          get_greeting() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "hello"
    IL_0005:  ret
  } // end of method ABC::get_greeting

  .property string greeting()
  {
    .get string XYZ.ABC::get_greeting()
  } // end of property ABC::greeting
} // end of class XYZ.ABC

.class private abstract auto ansi sealed '<StartupCode$ToplevelNamespace>'.$ToplevelNamespace
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (string V_0,
             string V_1)
    IL_0000:  call       string XYZ.ABC::get_greeting()
    IL_0005:  stloc.0
    IL_0006:  call       string XYZ.ABC/ABC::get_greeting()
    IL_000b:  stloc.1
    IL_000c:  ret
  } // end of method $ToplevelNamespace::main@

} // end of class '<StartupCode$ToplevelNamespace>'.$ToplevelNamespace


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SerializableAttribute\ToplevelNamespace_fs\ToplevelNamespace.res
