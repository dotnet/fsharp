
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
.assembly AnonRecd
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 )

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 )

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AnonRecd
{
  // Offset: 0x00000000 Length: 0x00000234
  // WARNING: managed resource file FSharpSignatureData.AnonRecd created
}
.mresource public FSharpOptimizationData.AnonRecd
{
  // Offset: 0x00000238 Length: 0x0000006B
  // WARNING: managed resource file FSharpOptimizationData.AnonRecd created
}
.module AnonRecd.exe
// MVID: {624E1220-C42F-5208-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03330000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AnonRecd
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
  .method public static int32  main<a>(!!a argv) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init (int32 V_0,
             class '<>f__AnonymousType1912756633`2'<int32,int32> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.1
    IL_0004:  newobj     instance void class '<>f__AnonymousType1912756633`2'<int32,int32>::.ctor(!0,
                                                                                                  !1)
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ret
  } // end of method AnonRecd::main

} // end of class AnonRecd

.class private abstract auto ansi sealed '<StartupCode$AnonRecd>'.$AnonRecd
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $AnonRecd::main@

} // end of class '<StartupCode$AnonRecd>'.$AnonRecd

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType1912756633`2'<'<A>j__TPar','<B>j__TPar'>
       extends [mscorlib]System.Object
       implements [mscorlib]System.Collections.IStructuralComparable,
                  [mscorlib]System.IComparable,
                  class [mscorlib]System.IComparable`1<class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [mscorlib]System.Collections.IStructuralEquatable,
                  class [mscorlib]System.IEquatable`1<class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>>
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 )
  .field private !'<A>j__TPar' A@
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .field private !'<B>j__TPar' B@
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .method public specialname rtspecialname
          instance void  .ctor(!'<A>j__TPar' A,
                               !'<B>j__TPar' B) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::.ctor

  .method public hidebysig specialname instance !'<A>j__TPar'
          get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::get_A

  .method public hidebysig specialname instance !'<B>j__TPar'
          get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::get_B

  .method public strict virtual instance string
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::ToString

  .method public hidebysig virtual final
          instance int32  CompareTo(class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       75 (0x4b)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0044

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0042

    IL_0006:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  bge.s      IL_0023

    IL_0021:  ldloc.0
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ble.s      IL_0029

    IL_0027:  ldloc.0
    IL_0028:  ret

    IL_0029:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002e:  ldarg.0
    IL_002f:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0034:  ldarg.1
    IL_0035:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  tail.
    IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0041:  ret

    IL_0042:  ldc.i4.1
    IL_0043:  ret

    IL_0044:  ldarg.1
    IL_0045:  brfalse.s  IL_0049

    IL_0047:  ldc.i4.m1
    IL_0048:  ret

    IL_0049:  ldc.i4.0
    IL_004a:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final
          instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(class '<>f__AnonymousType1912756633`2'<!0,!1>)
    IL_000e:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final
          instance int32  CompareTo(object obj,
                                    class [mscorlib]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       86 (0x56)
    .maxstack  5
    .locals init (class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_004a

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0012:  brfalse.s  IL_0048

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0021:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.0
    IL_0029:  bge.s      IL_002d

    IL_002b:  ldloc.2
    IL_002c:  ret

    IL_002d:  ldloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  ble.s      IL_0033

    IL_0031:  ldloc.2
    IL_0032:  ret

    IL_0033:  ldarg.2
    IL_0034:  ldarg.0
    IL_0035:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  ldloc.1
    IL_003b:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0040:  tail.
    IL_0042:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0047:  ret

    IL_0048:  ldc.i4.1
    IL_0049:  ret

    IL_004a:  ldarg.1
    IL_004b:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0050:  brfalse.s  IL_0054

    IL_0052:  ldc.i4.m1
    IL_0053:  ret

    IL_0054:  ldc.i4.0
    IL_0055:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final
          instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       63 (0x3f)
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003d

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
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
    IL_0026:  ldarg.0
    IL_0027:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_002c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.6
    IL_0033:  shl
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.2
    IL_0036:  shr
    IL_0037:  add
    IL_0038:  add
    IL_0039:  add
    IL_003a:  stloc.0
    IL_003b:  ldloc.0
    IL_003c:  ret

    IL_003d:  ldc.i4.0
    IL_003e:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::GetHashCode

  .method public hidebysig virtual final
          instance int32  GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::GetHashCode

  .method public hidebysig virtual final
          instance bool  Equals(object obj,
                                class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       68 (0x44)
    .maxstack  5
    .locals init (class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003c

    IL_0003:  ldarg.1
    IL_0004:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  brfalse.s  IL_003a

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldarg.2
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0016:  ldloc.1
    IL_0017:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001c:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0021:  brfalse.s  IL_0038

    IL_0023:  ldarg.2
    IL_0024:  ldarg.0
    IL_0025:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_002a:  ldloc.1
    IL_002b:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0030:  tail.
    IL_0032:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0037:  ret

    IL_0038:  ldc.i4.0
    IL_0039:  ret

    IL_003a:  ldc.i4.0
    IL_003b:  ret

    IL_003c:  ldarg.1
    IL_003d:  ldnull
    IL_003e:  cgt.un
    IL_0040:  ldc.i4.0
    IL_0041:  ceq
    IL_0043:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final
          instance bool  Equals(class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       57 (0x39)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002f

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0017:  brfalse.s  IL_002d

    IL_0019:  ldarg.0
    IL_001a:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0025:  tail.
    IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_002c:  ret

    IL_002d:  ldc.i4.0
    IL_002e:  ret

    IL_002f:  ldc.i4.0
    IL_0030:  ret

    IL_0031:  ldarg.1
    IL_0032:  ldnull
    IL_0033:  cgt.un
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final
          instance bool  Equals(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       22 (0x16)
    .maxstack  4
    .locals init (class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType1912756633`2'<!0,!1>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 )
    .get instance !'<A>j__TPar' '<>f__AnonymousType1912756633`2'::get_A()
  } // end of property '<>f__AnonymousType1912756633`2'::A
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 )
    .get instance !'<B>j__TPar' '<>f__AnonymousType1912756633`2'::get_B()
  } // end of property '<>f__AnonymousType1912756633`2'::B
} // end of class '<>f__AnonymousType1912756633`2'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\AnonRecd_fs\AnonRecd.res
