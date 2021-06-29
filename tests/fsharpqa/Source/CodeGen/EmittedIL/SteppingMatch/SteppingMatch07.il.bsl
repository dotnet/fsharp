
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
.assembly SteppingMatch07
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch07
{
  // Offset: 0x00000000 Length: 0x0000067D
}
.mresource public FSharpOptimizationData.SteppingMatch07
{
  // Offset: 0x00000688 Length: 0x000001D9
}
.module SteppingMatch07.dll
// MVID: {59B19213-D373-07F3-A745-03831392B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03330000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch07
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Discr
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class SteppingMatch07/Discr>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class SteppingMatch07/Discr>,
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

    .field assembly initonly int32 _tag
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch07/Discr _unique_CaseA
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch07/Discr _unique_CaseB
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void SteppingMatch07/Discr::.ctor(int32)
      IL_0006:  stsfld     class SteppingMatch07/Discr SteppingMatch07/Discr::_unique_CaseA
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void SteppingMatch07/Discr::.ctor(int32)
      IL_0011:  stsfld     class SteppingMatch07/Discr SteppingMatch07/Discr::_unique_CaseB
      IL_0016:  ret
    } // end of method Discr::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 SteppingMatch07/Discr::_tag
      IL_000d:  ret
    } // end of method Discr::.ctor

    .method public static class SteppingMatch07/Discr 
            get_CaseA() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch07/Discr SteppingMatch07/Discr::_unique_CaseA
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
      IL_0001:  call       instance int32 SteppingMatch07/Discr::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Discr::get_IsCaseA

    .method public static class SteppingMatch07/Discr 
            get_CaseB() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch07/Discr SteppingMatch07/Discr::_unique_CaseB
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
      IL_0001:  call       instance int32 SteppingMatch07/Discr::get_Tag()
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
      IL_0001:  ldfld      int32 SteppingMatch07/Discr::_tag
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
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Discr::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class SteppingMatch07/Discr>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch07/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Discr::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class SteppingMatch07/Discr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       64 (0x40)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] int32 V_1)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\SteppingMatch\\SteppingMatch07.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  ldnull
      IL_000c:  cgt.un
      IL_000e:  brfalse.s  IL_0012

      IL_0010:  br.s       IL_0014

      IL_0012:  br.s       IL_0030

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0021:  stloc.1
      IL_0022:  ldloc.0
      IL_0023:  ldloc.1
      IL_0024:  bne.un.s   IL_0028

      IL_0026:  br.s       IL_002a

      IL_0028:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_002a:  ldc.i4.0
      IL_002b:  ret

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.0
      IL_002d:  ldloc.1
      IL_002e:  sub
      IL_002f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0030:  ldc.i4.1
      IL_0031:  ret

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldarg.1
      IL_0033:  ldnull
      IL_0034:  cgt.un
      IL_0036:  brfalse.s  IL_003a

      IL_0038:  br.s       IL_003c

      IL_003a:  br.s       IL_003e

      .line 100001,100001 : 0,0 ''
      IL_003c:  ldc.i4.m1
      IL_003d:  ret

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldc.i4.0
      IL_003f:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  SteppingMatch07/Discr
      IL_0007:  callvirt   instance int32 SteppingMatch07/Discr::CompareTo(class SteppingMatch07/Discr)
      IL_000c:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       81 (0x51)
      .maxstack  4
      .locals init ([0] class SteppingMatch07/Discr V_0,
               [1] int32 V_1,
               [2] int32 V_2)
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  SteppingMatch07/Discr
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  br.s       IL_0011

      IL_000f:  br.s       IL_003e

      .line 100001,100001 : 0,0 ''
      IL_0011:  ldarg.1
      IL_0012:  unbox.any  SteppingMatch07/Discr
      IL_0017:  ldnull
      IL_0018:  cgt.un
      IL_001a:  brfalse.s  IL_001e

      IL_001c:  br.s       IL_0020

      IL_001e:  br.s       IL_003c

      .line 100001,100001 : 0,0 ''
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0026:  stloc.1
      IL_0027:  ldloc.0
      IL_0028:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_002d:  stloc.2
      IL_002e:  ldloc.1
      IL_002f:  ldloc.2
      IL_0030:  bne.un.s   IL_0034

      IL_0032:  br.s       IL_0036

      IL_0034:  br.s       IL_0038

      .line 100001,100001 : 0,0 ''
      IL_0036:  ldc.i4.0
      IL_0037:  ret

      .line 100001,100001 : 0,0 ''
      IL_0038:  ldloc.1
      IL_0039:  ldloc.2
      IL_003a:  sub
      IL_003b:  ret

      .line 100001,100001 : 0,0 ''
      IL_003c:  ldc.i4.1
      IL_003d:  ret

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldarg.1
      IL_003f:  unbox.any  SteppingMatch07/Discr
      IL_0044:  ldnull
      IL_0045:  cgt.un
      IL_0047:  brfalse.s  IL_004b

      IL_0049:  br.s       IL_004d

      IL_004b:  br.s       IL_004f

      .line 100001,100001 : 0,0 ''
      IL_004d:  ldc.i4.m1
      IL_004e:  ret

      .line 100001,100001 : 0,0 ''
      IL_004f:  ldc.i4.0
      IL_0050:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  3
      .locals init ([0] int32 V_0)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0013

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0012:  ret

      .line 100001,100001 : 0,0 ''
      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 SteppingMatch07/Discr::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       55 (0x37)
      .maxstack  4
      .locals init ([0] class SteppingMatch07/Discr V_0,
               [1] class SteppingMatch07/Discr V_1,
               [2] int32 V_2,
               [3] int32 V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_002f

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  isinst     SteppingMatch07/Discr
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_0018

      IL_0016:  br.s       IL_002d

      .line 100001,100001 : 0,0 ''
      IL_0018:  ldloc.0
      IL_0019:  stloc.1
      IL_001a:  ldarg.0
      IL_001b:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0027:  stloc.3
      IL_0028:  ldloc.2
      IL_0029:  ldloc.3
      IL_002a:  ceq
      IL_002c:  ret

      .line 100001,100001 : 0,0 ''
      IL_002d:  ldc.i4.0
      IL_002e:  ret

      .line 100001,100001 : 0,0 ''
      IL_002f:  ldarg.1
      IL_0030:  ldnull
      IL_0031:  cgt.un
      IL_0033:  ldc.i4.0
      IL_0034:  ceq
      IL_0036:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class SteppingMatch07/Discr obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       49 (0x31)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] int32 V_1)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  ldnull
      IL_000c:  cgt.un
      IL_000e:  brfalse.s  IL_0012

      IL_0010:  br.s       IL_0014

      IL_0012:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  ldfld      int32 SteppingMatch07/Discr::_tag
      IL_0021:  stloc.1
      IL_0022:  ldloc.0
      IL_0023:  ldloc.1
      IL_0024:  ceq
      IL_0026:  ret

      .line 100001,100001 : 0,0 ''
      IL_0027:  ldc.i4.0
      IL_0028:  ret

      .line 100001,100001 : 0,0 ''
      IL_0029:  ldarg.1
      IL_002a:  ldnull
      IL_002b:  cgt.un
      IL_002d:  ldc.i4.0
      IL_002e:  ceq
      IL_0030:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       24 (0x18)
      .maxstack  4
      .locals init ([0] class SteppingMatch07/Discr V_0)
      .line 4,4 : 6,11 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     SteppingMatch07/Discr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance bool SteppingMatch07/Discr::Equals(class SteppingMatch07/Discr)
      IL_0015:  ret

      .line 100001,100001 : 0,0 ''
      IL_0016:  ldc.i4.0
      IL_0017:  ret
    } // end of method Discr::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 SteppingMatch07/Discr::get_Tag()
    } // end of property Discr::Tag
    .property class SteppingMatch07/Discr
            CaseA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch07/Discr SteppingMatch07/Discr::get_CaseA()
    } // end of property Discr::CaseA
    .property instance bool IsCaseA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch07/Discr::get_IsCaseA()
    } // end of property Discr::IsCaseA
    .property class SteppingMatch07/Discr
            CaseB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch07/Discr SteppingMatch07/Discr::get_CaseB()
    } // end of property Discr::CaseB
    .property instance bool IsCaseB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch07/Discr::get_IsCaseB()
    } // end of property Discr::IsCaseB
  } // end of class Discr

  .method public static void  funcE(class SteppingMatch07/Discr n) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  8
    .line 6,6 : 9,21 ''
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 SteppingMatch07/Discr::get_Tag()
    IL_0006:  ldc.i4.1
    IL_0007:  bne.un.s   IL_000b

    IL_0009:  br.s       IL_0016

    .line 8,8 : 13,35 ''
    IL_000b:  ldstr      "A"
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ret

    .line 10,10 : 13,35 ''
    IL_0016:  ldstr      "B"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  ret
  } // end of method SteppingMatch07::funcE

} // end of class SteppingMatch07

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch07>'.$SteppingMatch07
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch07>'.$SteppingMatch07


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
