
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
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
.assembly SteppingMatch06
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch06
{
  // Offset: 0x00000000 Length: 0x0000067D
}
.mresource public FSharpOptimizationData.SteppingMatch06
{
  // Offset: 0x00000688 Length: 0x000001D9
}
.module SteppingMatch06.dll
// MVID: {579FB695-4FAE-FD21-A745-038395B69F57}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00B20000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch06
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Discr
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class SteppingMatch06/Discr>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class SteppingMatch06/Discr>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [mscorlib]System.Object
    {
      .field public static literal int32 CaseA = int32(0x00000000)
      .field public static literal int32 CaseB = int32(0x00000001)
    } // end of class Tags

    .field assembly initonly bool _tag
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch06/Discr _unique_CaseA
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch06/Discr _unique_CaseB
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void SteppingMatch06/Discr::.ctor(bool)
      IL_0006:  stsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseA
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void SteppingMatch06/Discr::.ctor(bool)
      IL_0011:  stsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseB
      IL_0016:  ret
    } // end of method Discr::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor(bool _tag) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      bool SteppingMatch06/Discr::_tag
      IL_000d:  ret
    } // end of method Discr::.ctor

    .method public static class SteppingMatch06/Discr 
            get_CaseA() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseA
      IL_0005:  ret
    } // end of method Discr::get_CaseA

    .method public hidebysig instance bool 
            get_IsCaseA() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SteppingMatch06/Discr::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Discr::get_IsCaseA

    .method public static class SteppingMatch06/Discr 
            get_CaseB() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseB
      IL_0005:  ret
    } // end of method Discr::get_CaseB

    .method public hidebysig instance bool 
            get_IsCaseB() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SteppingMatch06/Discr::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Discr::get_IsCaseB

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0006:  ret
    } // end of method Discr::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Discr::__DebugDisplay

    .method public hidebysig virtual final 
            instance int32  CompareTo(class SteppingMatch06/Discr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       65 (0x41)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_0033

      IL_000b:  ldarg.1
      IL_000c:  ldnull
      IL_000d:  cgt.un
      IL_000f:  brfalse.s  IL_0013

      IL_0011:  br.s       IL_0015

      IL_0013:  br.s       IL_0031

      IL_0015:  ldarg.0
      IL_0016:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_001b:  stloc.0
      IL_001c:  ldarg.1
      IL_001d:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0022:  stloc.1
      IL_0023:  ldloc.0
      IL_0024:  ldloc.1
      IL_0025:  bne.un.s   IL_0029

      IL_0027:  br.s       IL_002b

      IL_0029:  br.s       IL_002d

      IL_002b:  ldc.i4.0
      IL_002c:  ret

      IL_002d:  ldloc.0
      IL_002e:  ldloc.1
      IL_002f:  sub
      IL_0030:  ret

      IL_0031:  ldc.i4.1
      IL_0032:  ret

      IL_0033:  ldarg.1
      IL_0034:  ldnull
      IL_0035:  cgt.un
      IL_0037:  brfalse.s  IL_003b

      IL_0039:  br.s       IL_003d

      IL_003b:  br.s       IL_003f

      IL_003d:  ldc.i4.m1
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 4,4 : 6,11 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\SteppingMatch\\SteppingMatch06.fs'
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  unbox.any  SteppingMatch06/Discr
      IL_0008:  callvirt   instance int32 SteppingMatch06/Discr::CompareTo(class SteppingMatch06/Discr)
      IL_000d:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       82 (0x52)
      .maxstack  4
      .locals init ([0] class SteppingMatch06/Discr V_0,
               [1] int32 V_1,
               [2] int32 V_2)
      .line 4,4 : 6,11 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  SteppingMatch06/Discr
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  ldnull
      IL_000a:  cgt.un
      IL_000c:  brfalse.s  IL_0010

      IL_000e:  br.s       IL_0012

      IL_0010:  br.s       IL_003f

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldarg.1
      IL_0013:  unbox.any  SteppingMatch06/Discr
      IL_0018:  ldnull
      IL_0019:  cgt.un
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  br.s       IL_0021

      IL_001f:  br.s       IL_003d

      .line 100001,100001 : 0,0 ''
      IL_0021:  ldarg.0
      IL_0022:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0027:  stloc.1
      IL_0028:  ldloc.0
      IL_0029:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_002e:  stloc.2
      IL_002f:  ldloc.1
      IL_0030:  ldloc.2
      IL_0031:  bne.un.s   IL_0035

      IL_0033:  br.s       IL_0037

      IL_0035:  br.s       IL_0039

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldc.i4.0
      IL_0038:  ret

      .line 100001,100001 : 0,0 ''
      IL_0039:  ldloc.1
      IL_003a:  ldloc.2
      IL_003b:  sub
      IL_003c:  ret

      .line 100001,100001 : 0,0 ''
      IL_003d:  ldc.i4.1
      IL_003e:  ret

      .line 100001,100001 : 0,0 ''
      IL_003f:  ldarg.1
      IL_0040:  unbox.any  SteppingMatch06/Discr
      IL_0045:  ldnull
      IL_0046:  cgt.un
      IL_0048:  brfalse.s  IL_004c

      IL_004a:  br.s       IL_004e

      IL_004c:  br.s       IL_0050

      .line 100001,100001 : 0,0 ''
      IL_004e:  ldc.i4.m1
      IL_004f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0050:  ldc.i4.0
      IL_0051:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       40 (0x28)
      .maxstack  3
      .locals init (int32 V_0,
               class SteppingMatch06/Discr V_1,
               class SteppingMatch06/Discr V_2)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_0026

      IL_000b:  ldc.i4.0
      IL_000c:  stloc.0
      IL_000d:  ldarg.0
      IL_000e:  call       instance int32 SteppingMatch06/Discr::get_Tag()
      IL_0013:  ldc.i4.0
      IL_0014:  bne.un.s   IL_0018

      IL_0016:  br.s       IL_001a

      IL_0018:  br.s       IL_0020

      IL_001a:  ldarg.0
      IL_001b:  stloc.1
      IL_001c:  ldc.i4.0
      IL_001d:  stloc.0
      IL_001e:  ldloc.0
      IL_001f:  ret

      IL_0020:  ldarg.0
      IL_0021:  stloc.2
      IL_0022:  ldc.i4.1
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ret

      IL_0026:  ldc.i4.0
      IL_0027:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 4,4 : 6,11 ''
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0007:  callvirt   instance int32 SteppingMatch06/Discr::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000c:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       56 (0x38)
      .maxstack  4
      .locals init (class SteppingMatch06/Discr V_0,
               class SteppingMatch06/Discr V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_0030

      IL_000b:  ldarg.1
      IL_000c:  isinst     SteppingMatch06/Discr
      IL_0011:  stloc.0
      IL_0012:  ldloc.0
      IL_0013:  brfalse.s  IL_0017

      IL_0015:  br.s       IL_0019

      IL_0017:  br.s       IL_002e

      IL_0019:  ldloc.0
      IL_001a:  stloc.1
      IL_001b:  ldarg.0
      IL_001c:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0021:  stloc.2
      IL_0022:  ldloc.1
      IL_0023:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0028:  stloc.3
      IL_0029:  ldloc.2
      IL_002a:  ldloc.3
      IL_002b:  ceq
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret

      IL_0030:  ldarg.1
      IL_0031:  ldnull
      IL_0032:  cgt.un
      IL_0034:  ldc.i4.0
      IL_0035:  ceq
      IL_0037:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class SteppingMatch06/Discr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       50 (0x32)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000b

      IL_0009:  br.s       IL_002a

      IL_000b:  ldarg.1
      IL_000c:  ldnull
      IL_000d:  cgt.un
      IL_000f:  brfalse.s  IL_0013

      IL_0011:  br.s       IL_0015

      IL_0013:  br.s       IL_0028

      IL_0015:  ldarg.0
      IL_0016:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_001b:  stloc.0
      IL_001c:  ldarg.1
      IL_001d:  ldfld      bool SteppingMatch06/Discr::_tag
      IL_0022:  stloc.1
      IL_0023:  ldloc.0
      IL_0024:  ldloc.1
      IL_0025:  ceq
      IL_0027:  ret

      IL_0028:  ldc.i4.0
      IL_0029:  ret

      IL_002a:  ldarg.1
      IL_002b:  ldnull
      IL_002c:  cgt.un
      IL_002e:  ldc.i4.0
      IL_002f:  ceq
      IL_0031:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       25 (0x19)
      .maxstack  4
      .locals init (class SteppingMatch06/Discr V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  isinst     SteppingMatch06/Discr
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  brfalse.s  IL_000d

      IL_000b:  br.s       IL_000f

      IL_000d:  br.s       IL_0017

      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  callvirt   instance bool SteppingMatch06/Discr::Equals(class SteppingMatch06/Discr)
      IL_0016:  ret

      IL_0017:  ldc.i4.0
      IL_0018:  ret
    } // end of method Discr::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 SteppingMatch06/Discr::get_Tag()
    } // end of property Discr::Tag
    .property class SteppingMatch06/Discr
            CaseA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch06/Discr SteppingMatch06/Discr::get_CaseA()
    } // end of property Discr::CaseA
    .property instance bool IsCaseA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch06/Discr::get_IsCaseA()
    } // end of property Discr::IsCaseA
    .property class SteppingMatch06/Discr
            CaseB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch06/Discr SteppingMatch06/Discr::get_CaseB()
    } // end of property Discr::CaseB
    .property instance bool IsCaseB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch06/Discr::get_IsCaseB()
    } // end of property Discr::IsCaseB
  } // end of class Discr

  .method public static void  funcD(class SteppingMatch06/Discr n) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    .line 6,6 : 9,21 ''
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 SteppingMatch06/Discr::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0017

    .line 8,8 : 13,35 ''
    IL_000c:  ldstr      "B"
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  ret

    .line 10,10 : 13,35 ''
    IL_0017:  ldstr      "A"
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0021:  ret
  } // end of method SteppingMatch06::funcD

} // end of class SteppingMatch06

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch06>'.$SteppingMatch06
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch06>'.$SteppingMatch06


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
