
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
.assembly AnonRecd
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AnonRecd
{
  // Offset: 0x00000000 Length: 0x000001C2
}
.mresource public FSharpOptimizationData.AnonRecd
{
  // Offset: 0x000001C8 Length: 0x0000006B
}
.module AnonRecd.exe
// MVID: {611C4D7C-C42F-5208-A745-03837C4D1C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07240000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AnonRecd
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  main<a>(!!a argv) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init ([0] int32 x,
             [1] class '<>f__AnonymousType1912756633`2'<int32,int32> a)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 5,22 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\AnonRecd.fs'
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    .line 6,6 : 5,31 ''
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.1
    IL_0004:  newobj     instance void class '<>f__AnonymousType1912756633`2'<int32,int32>::.ctor(!0,
                                                                                                  !1)
    IL_0009:  stloc.1
    .line 8,8 : 5,15 ''
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    .line 10,10 : 5,6 ''
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
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private !'<B>j__TPar' B@
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::get_A

  .method public hidebysig specialname instance !'<B>j__TPar' 
          get_B() cil managed
  {
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
    // Code size       85 (0x55)
    .maxstack  5
    .locals init ([0] int32 V_0)
    .line 1,1 : 1,1 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\unknown'
    IL_0000:  nop
    .line 100001,100001 : 0,0 ''
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  brfalse.s  IL_004b

    .line 100001,100001 : 0,0 ''
    IL_0007:  ldarg.1
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s  IL_0049

    .line 100001,100001 : 0,0 ''
    IL_000d:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0018:  ldarg.1
    IL_0019:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0023:  stloc.0
    .line 100001,100001 : 0,0 ''
    IL_0024:  ldloc.0
    IL_0025:  ldc.i4.0
    IL_0026:  bge.s      IL_002a

    .line 100001,100001 : 0,0 ''
    IL_0028:  ldloc.0
    IL_0029:  ret

    .line 100001,100001 : 0,0 ''
    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.0
    IL_002c:  ble.s      IL_0030

    .line 100001,100001 : 0,0 ''
    IL_002e:  ldloc.0
    IL_002f:  ret

    .line 100001,100001 : 0,0 ''
    IL_0030:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0035:  ldarg.0
    IL_0036:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003b:  ldarg.1
    IL_003c:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0041:  tail.
    IL_0043:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0048:  ret

    .line 100001,100001 : 0,0 ''
    IL_0049:  ldc.i4.1
    IL_004a:  ret

    .line 100001,100001 : 0,0 ''
    IL_004b:  ldarg.1
    IL_004c:  ldnull
    IL_004d:  cgt.un
    IL_004f:  brfalse.s  IL_0053

    .line 100001,100001 : 0,0 ''
    IL_0051:  ldc.i4.m1
    IL_0052:  ret

    .line 100001,100001 : 0,0 ''
    IL_0053:  ldc.i4.0
    IL_0054:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final 
          instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  8
    .line 1,1 : 1,1 ''
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
    // Code size       95 (0x5f)
    .maxstack  5
    .locals init ([0] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             [1] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1,
             [2] int32 V_2)
    .line 1,1 : 1,1 ''
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    .line 100001,100001 : 0,0 ''
    IL_0009:  ldarg.0
    IL_000a:  ldnull
    IL_000b:  cgt.un
    IL_000d:  brfalse.s  IL_0050

    .line 100001,100001 : 0,0 ''
    IL_000f:  ldarg.1
    IL_0010:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0015:  ldnull
    IL_0016:  cgt.un
    IL_0018:  brfalse.s  IL_004e

    .line 100001,100001 : 0,0 ''
    IL_001a:  ldarg.2
    IL_001b:  ldarg.0
    IL_001c:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0021:  ldloc.1
    IL_0022:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0027:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_002c:  stloc.2
    .line 100001,100001 : 0,0 ''
    IL_002d:  ldloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  bge.s      IL_0033

    .line 100001,100001 : 0,0 ''
    IL_0031:  ldloc.2
    IL_0032:  ret

    .line 100001,100001 : 0,0 ''
    IL_0033:  ldloc.2
    IL_0034:  ldc.i4.0
    IL_0035:  ble.s      IL_0039

    .line 100001,100001 : 0,0 ''
    IL_0037:  ldloc.2
    IL_0038:  ret

    .line 100001,100001 : 0,0 ''
    IL_0039:  ldarg.2
    IL_003a:  ldarg.0
    IL_003b:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0040:  ldloc.1
    IL_0041:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0046:  tail.
    IL_0048:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_004d:  ret

    .line 100001,100001 : 0,0 ''
    IL_004e:  ldc.i4.1
    IL_004f:  ret

    .line 100001,100001 : 0,0 ''
    IL_0050:  ldarg.1
    IL_0051:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0056:  ldnull
    IL_0057:  cgt.un
    IL_0059:  brfalse.s  IL_005d

    .line 100001,100001 : 0,0 ''
    IL_005b:  ldc.i4.m1
    IL_005c:  ret

    .line 100001,100001 : 0,0 ''
    IL_005d:  ldc.i4.0
    IL_005e:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final 
          instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       67 (0x43)
    .maxstack  7
    .locals init ([0] int32 V_0)
    .line 1,1 : 1,1 ''
    IL_0000:  nop
    .line 100001,100001 : 0,0 ''
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  brfalse.s  IL_0041

    .line 100001,100001 : 0,0 ''
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.0
    IL_0009:  ldc.i4     0x9e3779b9
    IL_000e:  ldarg.1
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0015:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.6
    IL_001c:  shl
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.2
    IL_001f:  shr
    IL_0020:  add
    IL_0021:  add
    IL_0022:  add
    IL_0023:  stloc.0
    IL_0024:  ldc.i4     0x9e3779b9
    IL_0029:  ldarg.1
    IL_002a:  ldarg.0
    IL_002b:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0030:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.6
    IL_0037:  shl
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.2
    IL_003a:  shr
    IL_003b:  add
    IL_003c:  add
    IL_003d:  add
    IL_003e:  stloc.0
    IL_003f:  ldloc.0
    IL_0040:  ret

    .line 100001,100001 : 0,0 ''
    IL_0041:  ldc.i4.0
    IL_0042:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::GetHashCode

  .method public hidebysig virtual final 
          instance int32  GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       14 (0xe)
    .maxstack  8
    .line 1,1 : 1,1 ''
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
    // Code size       72 (0x48)
    .maxstack  5
    .locals init ([0] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             [1] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1)
    .line 1,1 : 1,1 ''
    IL_0000:  nop
    .line 100001,100001 : 0,0 ''
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  brfalse.s  IL_0040

    .line 100001,100001 : 0,0 ''
    IL_0007:  ldarg.1
    IL_0008:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_000d:  stloc.0
    .line 100001,100001 : 0,0 ''
    IL_000e:  ldloc.0
    IL_000f:  brfalse.s  IL_003e

    .line 100001,100001 : 0,0 ''
    IL_0011:  ldloc.0
    IL_0012:  stloc.1
    .line 100001,100001 : 0,0 ''
    IL_0013:  ldarg.2
    IL_0014:  ldarg.0
    IL_0015:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001a:  ldloc.1
    IL_001b:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0020:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0025:  brfalse.s  IL_003c

    .line 100001,100001 : 0,0 ''
    IL_0027:  ldarg.2
    IL_0028:  ldarg.0
    IL_0029:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_002e:  ldloc.1
    IL_002f:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0034:  tail.
    IL_0036:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_003b:  ret

    .line 100001,100001 : 0,0 ''
    IL_003c:  ldc.i4.0
    IL_003d:  ret

    .line 100001,100001 : 0,0 ''
    IL_003e:  ldc.i4.0
    IL_003f:  ret

    .line 100001,100001 : 0,0 ''
    IL_0040:  ldarg.1
    IL_0041:  ldnull
    IL_0042:  cgt.un
    IL_0044:  ldc.i4.0
    IL_0045:  ceq
    IL_0047:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       64 (0x40)
    .maxstack  4
    .line 1,1 : 1,1 ''
    IL_0000:  nop
    .line 100001,100001 : 0,0 ''
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  brfalse.s  IL_0038

    .line 100001,100001 : 0,0 ''
    IL_0007:  ldarg.1
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s  IL_0036

    .line 100001,100001 : 0,0 ''
    IL_000d:  ldarg.0
    IL_000e:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0013:  ldarg.1
    IL_0014:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0019:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_001e:  brfalse.s  IL_0034

    .line 100001,100001 : 0,0 ''
    IL_0020:  ldarg.0
    IL_0021:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0026:  ldarg.1
    IL_0027:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_002c:  tail.
    IL_002e:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0033:  ret

    .line 100001,100001 : 0,0 ''
    IL_0034:  ldc.i4.0
    IL_0035:  ret

    .line 100001,100001 : 0,0 ''
    IL_0036:  ldc.i4.0
    IL_0037:  ret

    .line 100001,100001 : 0,0 ''
    IL_0038:  ldarg.1
    IL_0039:  ldnull
    IL_003a:  cgt.un
    IL_003c:  ldc.i4.0
    IL_003d:  ceq
    IL_003f:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  4
    .locals init ([0] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    .line 1,1 : 1,1 ''
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    .line 100001,100001 : 0,0 ''
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    .line 100001,100001 : 0,0 ''
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType1912756633`2'<!0,!1>)
    IL_0013:  ret

    .line 100001,100001 : 0,0 ''
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
