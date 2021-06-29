
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
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
  .ver 4:6:0:0
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
  // Offset: 0x00000000 Length: 0x000001CE
}
.mresource public FSharpOptimizationData.AnonRecd
{
  // Offset: 0x000001D8 Length: 0x0000006B
}
.module AnonRecd.exe
// MVID: {5CBDEF61-C42F-5208-A745-038361EFBD5C}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00D20000


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
    .line 4,4 : 5,22 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\AnonRecd.fs'
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
    // Code size       104 (0x68)
    .maxstack  5
    .locals init ([0] int32 V_0)
    .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\unknown'
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  cgt.un
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  br.s       IL_000a

    IL_0008:  br.s       IL_005a

    .line 100001,100001 : 0,0 ''
    IL_000a:  ldarg.1
    IL_000b:  ldnull
    IL_000c:  cgt.un
    IL_000e:  brfalse.s  IL_0012

    IL_0010:  br.s       IL_0014

    IL_0012:  br.s       IL_0058

    .line 100001,100001 : 0,0 ''
    IL_0014:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0019:  ldarg.0
    IL_001a:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0025:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  bge.s      IL_0031

    IL_002f:  br.s       IL_0033

    IL_0031:  br.s       IL_0035

    .line 100001,100001 : 0,0 ''
    IL_0033:  ldloc.0
    IL_0034:  ret

    .line 100001,100001 : 0,0 ''
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  ble.s      IL_003b

    IL_0039:  br.s       IL_003d

    IL_003b:  br.s       IL_003f

    .line 100001,100001 : 0,0 ''
    IL_003d:  ldloc.0
    IL_003e:  ret

    .line 100001,100001 : 0,0 ''
    IL_003f:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_004a:  ldarg.1
    IL_004b:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0050:  tail.
    IL_0052:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0057:  ret

    .line 100001,100001 : 0,0 ''
    IL_0058:  ldc.i4.1
    IL_0059:  ret

    .line 100001,100001 : 0,0 ''
    IL_005a:  ldarg.1
    IL_005b:  ldnull
    IL_005c:  cgt.un
    IL_005e:  brfalse.s  IL_0062

    IL_0060:  br.s       IL_0064

    IL_0062:  br.s       IL_0066

    .line 100001,100001 : 0,0 ''
    IL_0064:  ldc.i4.m1
    IL_0065:  ret

    .line 100001,100001 : 0,0 ''
    IL_0066:  ldc.i4.0
    IL_0067:  ret
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
    // Code size       115 (0x73)
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
    IL_0009:  ldarg.0
    IL_000a:  ldnull
    IL_000b:  cgt.un
    IL_000d:  brfalse.s  IL_0011

    IL_000f:  br.s       IL_0013

    IL_0011:  br.s       IL_0060

    .line 100001,100001 : 0,0 ''
    IL_0013:  ldarg.1
    IL_0014:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0019:  ldnull
    IL_001a:  cgt.un
    IL_001c:  brfalse.s  IL_0020

    IL_001e:  br.s       IL_0022

    IL_0020:  br.s       IL_005e

    .line 100001,100001 : 0,0 ''
    IL_0022:  ldarg.2
    IL_0023:  ldarg.0
    IL_0024:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0029:  ldloc.1
    IL_002a:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_002f:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  ldc.i4.0
    IL_0037:  bge.s      IL_003b

    IL_0039:  br.s       IL_003d

    IL_003b:  br.s       IL_003f

    .line 100001,100001 : 0,0 ''
    IL_003d:  ldloc.2
    IL_003e:  ret

    .line 100001,100001 : 0,0 ''
    IL_003f:  ldloc.2
    IL_0040:  ldc.i4.0
    IL_0041:  ble.s      IL_0045

    IL_0043:  br.s       IL_0047

    IL_0045:  br.s       IL_0049

    .line 100001,100001 : 0,0 ''
    IL_0047:  ldloc.2
    IL_0048:  ret

    .line 100001,100001 : 0,0 ''
    IL_0049:  ldarg.2
    IL_004a:  ldarg.0
    IL_004b:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0050:  ldloc.1
    IL_0051:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0056:  tail.
    IL_0058:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_005d:  ret

    .line 100001,100001 : 0,0 ''
    IL_005e:  ldc.i4.1
    IL_005f:  ret

    .line 100001,100001 : 0,0 ''
    IL_0060:  ldarg.1
    IL_0061:  unbox.any  class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0066:  ldnull
    IL_0067:  cgt.un
    IL_0069:  brfalse.s  IL_006d

    IL_006b:  br.s       IL_006f

    IL_006d:  br.s       IL_0071

    .line 100001,100001 : 0,0 ''
    IL_006f:  ldc.i4.m1
    IL_0070:  ret

    .line 100001,100001 : 0,0 ''
    IL_0071:  ldc.i4.0
    IL_0072:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::CompareTo

  .method public hidebysig virtual final 
          instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       70 (0x46)
    .maxstack  7
    .locals init ([0] int32 V_0)
    .line 100001,100001 : 0,0 ''
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  cgt.un
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  br.s       IL_000a

    IL_0008:  br.s       IL_0044

    .line 100001,100001 : 0,0 ''
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldc.i4     0x9e3779b9
    IL_0011:  ldarg.1
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0018:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
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
    IL_0027:  ldc.i4     0x9e3779b9
    IL_002c:  ldarg.1
    IL_002d:  ldarg.0
    IL_002e:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0033:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.6
    IL_003a:  shl
    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.2
    IL_003d:  shr
    IL_003e:  add
    IL_003f:  add
    IL_0040:  add
    IL_0041:  stloc.0
    IL_0042:  ldloc.0
    IL_0043:  ret

    .line 100001,100001 : 0,0 ''
    IL_0044:  ldc.i4.0
    IL_0045:  ret
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
    // Code size       83 (0x53)
    .maxstack  5
    .locals init ([0] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             [1] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1)
    .line 100001,100001 : 0,0 ''
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  cgt.un
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  br.s       IL_000a

    IL_0008:  br.s       IL_004b

    .line 100001,100001 : 0,0 ''
    IL_000a:  ldarg.1
    IL_000b:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  brfalse.s  IL_0016

    IL_0014:  br.s       IL_0018

    IL_0016:  br.s       IL_0049

    .line 100001,100001 : 0,0 ''
    IL_0018:  ldloc.0
    IL_0019:  stloc.1
    IL_001a:  ldarg.2
    IL_001b:  ldarg.0
    IL_001c:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0021:  ldloc.1
    IL_0022:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_002c:  brfalse.s  IL_0030

    IL_002e:  br.s       IL_0032

    IL_0030:  br.s       IL_0047

    .line 100001,100001 : 0,0 ''
    IL_0032:  ldarg.2
    IL_0033:  ldarg.0
    IL_0034:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0039:  ldloc.1
    IL_003a:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003f:  tail.
    IL_0041:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0046:  ret

    .line 100001,100001 : 0,0 ''
    IL_0047:  ldc.i4.0
    IL_0048:  ret

    .line 100001,100001 : 0,0 ''
    IL_0049:  ldc.i4.0
    IL_004a:  ret

    .line 100001,100001 : 0,0 ''
    IL_004b:  ldarg.1
    IL_004c:  ldnull
    IL_004d:  cgt.un
    IL_004f:  ldc.i4.0
    IL_0050:  ceq
    IL_0052:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  4
    .line 100001,100001 : 0,0 ''
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  cgt.un
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  br.s       IL_000a

    IL_0008:  br.s       IL_0043

    .line 100001,100001 : 0,0 ''
    IL_000a:  ldarg.1
    IL_000b:  ldnull
    IL_000c:  cgt.un
    IL_000e:  brfalse.s  IL_0012

    IL_0010:  br.s       IL_0014

    IL_0012:  br.s       IL_0041

    .line 100001,100001 : 0,0 ''
    IL_0014:  ldarg.0
    IL_0015:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001a:  ldarg.1
    IL_001b:  ldfld      !0 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0020:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0025:  brfalse.s  IL_0029

    IL_0027:  br.s       IL_002b

    IL_0029:  br.s       IL_003f

    .line 100001,100001 : 0,0 ''
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0031:  ldarg.1
    IL_0032:  ldfld      !1 class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0037:  tail.
    IL_0039:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_003e:  ret

    .line 100001,100001 : 0,0 ''
    IL_003f:  ldc.i4.0
    IL_0040:  ret

    .line 100001,100001 : 0,0 ''
    IL_0041:  ldc.i4.0
    IL_0042:  ret

    .line 100001,100001 : 0,0 ''
    IL_0043:  ldarg.1
    IL_0044:  ldnull
    IL_0045:  cgt.un
    IL_0047:  ldc.i4.0
    IL_0048:  ceq
    IL_004a:  ret
  } // end of method '<>f__AnonymousType1912756633`2'::Equals

  .method public hidebysig virtual final 
          instance bool  Equals(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       26 (0x1a)
    .maxstack  4
    .locals init ([0] class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    .line 1,1 : 1,1 ''
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  br.s       IL_000e

    IL_000c:  br.s       IL_0018

    .line 100001,100001 : 0,0 ''
    IL_000e:  ldarg.0
    IL_000f:  ldloc.0
    IL_0010:  tail.
    IL_0012:  callvirt   instance bool class '<>f__AnonymousType1912756633`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType1912756633`2'<!0,!1>)
    IL_0017:  ret

    .line 100001,100001 : 0,0 ''
    IL_0018:  ldc.i4.0
    IL_0019:  ret
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
