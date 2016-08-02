
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
.assembly CCtorDUWithMember01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CCtorDUWithMember01
{
  // Offset: 0x00000000 Length: 0x00000790
}
.mresource public FSharpOptimizationData.CCtorDUWithMember01
{
  // Offset: 0x00000798 Length: 0x00000227
}
.module CCtorDUWithMember01.exe
// MVID: {579FB63B-26F1-14EE-A745-03833BB69F57}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00490000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CCtorDUWithMember01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class CCtorDUWithMember01

.class private abstract auto ansi sealed '<StartupCode$CCtorDUWithMember01>'.$CCtorDUWithMember01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       62 (0x3e)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             [1] class CCtorDUWithMember01a/C V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2,
             [3] class CCtorDUWithMember01a/C V_3)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 2,2 : 1,46 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\CCtorDUWithMember\\CCtorDUWithMember01.fs'
    IL_0000:  ldstr      "File1.A = %A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class CCtorDUWithMember01a/C>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  nop
    IL_0011:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_A()
    IL_0016:  stloc.1
    IL_0017:  ldloc.0
    IL_0018:  ldloc.1
    IL_0019:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001e:  pop
    IL_001f:  ldstr      "Test.e2 = %A"
    IL_0024:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class CCtorDUWithMember01a/C>::.ctor(string)
    IL_0029:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002e:  stloc.2
    .line 3,3 : 1,47 ''
    IL_002f:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a::get_e2()
    IL_0034:  stloc.3
    IL_0035:  ldloc.2
    IL_0036:  ldloc.3
    IL_0037:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003c:  pop
    IL_003d:  ret
  } // end of method $CCtorDUWithMember01::main@

} // end of class '<StartupCode$CCtorDUWithMember01>'.$CCtorDUWithMember01

.class public abstract auto ansi sealed CCtorDUWithMember01a
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit C
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class CCtorDUWithMember01a/C>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class CCtorDUWithMember01a/C>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [mscorlib]System.Object
    {
      .field public static literal int32 A = int32(0x00000000)
      .field public static literal int32 B = int32(0x00000001)
    } // end of class Tags

    .field assembly initonly bool _tag
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class CCtorDUWithMember01a/C _unique_A
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class CCtorDUWithMember01a/C _unique_B
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void CCtorDUWithMember01a/C::.ctor(bool)
      IL_0006:  stsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_A
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void CCtorDUWithMember01a/C::.ctor(bool)
      IL_0011:  stsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_B
      IL_0016:  ret
    } // end of method C::.cctor

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
      IL_0008:  stfld      bool CCtorDUWithMember01a/C::_tag
      IL_000d:  ret
    } // end of method C::.ctor

    .method public static class CCtorDUWithMember01a/C 
            get_A() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_A
      IL_0005:  ret
    } // end of method C::get_A

    .method public hidebysig instance bool 
            get_IsA() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 CCtorDUWithMember01a/C::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method C::get_IsA

    .method public static class CCtorDUWithMember01a/C 
            get_B() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_B
      IL_0005:  ret
    } // end of method C::get_B

    .method public hidebysig instance bool 
            get_IsB() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 CCtorDUWithMember01a/C::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method C::get_IsB

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      bool CCtorDUWithMember01a/C::_tag
      IL_0006:  ret
    } // end of method C::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method C::__DebugDisplay

    .method public hidebysig virtual final 
            instance int32  CompareTo(class CCtorDUWithMember01a/C obj) cil managed
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
      IL_0016:  ldfld      bool CCtorDUWithMember01a/C::_tag
      IL_001b:  stloc.0
      IL_001c:  ldarg.1
      IL_001d:  ldfld      bool CCtorDUWithMember01a/C::_tag
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
    } // end of method C::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      .line 3,3 : 6,7 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\CCtorDUWithMember\\CCtorDUWithMember01a.fs'
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  unbox.any  CCtorDUWithMember01a/C
      IL_0008:  callvirt   instance int32 CCtorDUWithMember01a/C::CompareTo(class CCtorDUWithMember01a/C)
      IL_000d:  ret
    } // end of method C::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       82 (0x52)
      .maxstack  4
      .locals init ([0] class CCtorDUWithMember01a/C V_0,
               [1] int32 V_1,
               [2] int32 V_2)
      .line 3,3 : 6,7 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  CCtorDUWithMember01a/C
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  ldnull
      IL_000a:  cgt.un
      IL_000c:  brfalse.s  IL_0010

      IL_000e:  br.s       IL_0012

      IL_0010:  br.s       IL_003f

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldarg.1
      IL_0013:  unbox.any  CCtorDUWithMember01a/C
      IL_0018:  ldnull
      IL_0019:  cgt.un
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  br.s       IL_0021

      IL_001f:  br.s       IL_003d

      .line 100001,100001 : 0,0 ''
      IL_0021:  ldarg.0
      IL_0022:  ldfld      bool CCtorDUWithMember01a/C::_tag
      IL_0027:  stloc.1
      IL_0028:  ldloc.0
      IL_0029:  ldfld      bool CCtorDUWithMember01a/C::_tag
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
      IL_0040:  unbox.any  CCtorDUWithMember01a/C
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
    } // end of method C::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       40 (0x28)
      .maxstack  3
      .locals init (int32 V_0,
               class CCtorDUWithMember01a/C V_1,
               class CCtorDUWithMember01a/C V_2)
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
      IL_000e:  call       instance int32 CCtorDUWithMember01a/C::get_Tag()
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
    } // end of method C::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 3,3 : 6,7 ''
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0007:  callvirt   instance int32 CCtorDUWithMember01a/C::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000c:  ret
    } // end of method C::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       56 (0x38)
      .maxstack  4
      .locals init (class CCtorDUWithMember01a/C V_0,
               class CCtorDUWithMember01a/C V_1,
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
      IL_000c:  isinst     CCtorDUWithMember01a/C
      IL_0011:  stloc.0
      IL_0012:  ldloc.0
      IL_0013:  brfalse.s  IL_0017

      IL_0015:  br.s       IL_0019

      IL_0017:  br.s       IL_002e

      IL_0019:  ldloc.0
      IL_001a:  stloc.1
      IL_001b:  ldarg.0
      IL_001c:  ldfld      bool CCtorDUWithMember01a/C::_tag
      IL_0021:  stloc.2
      IL_0022:  ldloc.1
      IL_0023:  ldfld      bool CCtorDUWithMember01a/C::_tag
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
    } // end of method C::Equals

    .method public hidebysig specialname 
            instance int32  get_P() cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 6,6 : 18,19 ''
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  ret
    } // end of method C::get_P

    .method public hidebysig virtual final 
            instance bool  Equals(class CCtorDUWithMember01a/C obj) cil managed
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
      IL_0016:  ldfld      bool CCtorDUWithMember01a/C::_tag
      IL_001b:  stloc.0
      IL_001c:  ldarg.1
      IL_001d:  ldfld      bool CCtorDUWithMember01a/C::_tag
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
    } // end of method C::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       25 (0x19)
      .maxstack  4
      .locals init (class CCtorDUWithMember01a/C V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  isinst     CCtorDUWithMember01a/C
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  brfalse.s  IL_000d

      IL_000b:  br.s       IL_000f

      IL_000d:  br.s       IL_0017

      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  callvirt   instance bool CCtorDUWithMember01a/C::Equals(class CCtorDUWithMember01a/C)
      IL_0016:  ret

      IL_0017:  ldc.i4.0
      IL_0018:  ret
    } // end of method C::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 CCtorDUWithMember01a/C::get_Tag()
    } // end of property C::Tag
    .property class CCtorDUWithMember01a/C
            A()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_A()
    } // end of property C::A
    .property instance bool IsA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool CCtorDUWithMember01a/C::get_IsA()
    } // end of property C::IsA
    .property class CCtorDUWithMember01a/C
            B()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_B()
    } // end of property C::B
    .property instance bool IsB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool CCtorDUWithMember01a/C::get_IsB()
    } // end of property C::IsB
    .property instance int32 P()
    {
      .get instance int32 CCtorDUWithMember01a/C::get_P()
    } // end of property C::P
  } // end of class C

  .method public specialname static class CCtorDUWithMember01a/C 
          get_e2() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_A()
    IL_0006:  ret
  } // end of method CCtorDUWithMember01a::get_e2

  .property class CCtorDUWithMember01a/C e2()
  {
    .get class CCtorDUWithMember01a/C CCtorDUWithMember01a::get_e2()
  } // end of property CCtorDUWithMember01a::e2
} // end of class CCtorDUWithMember01a

.class private abstract auto ansi sealed '<StartupCode$CCtorDUWithMember01>'.$CCtorDUWithMember01a
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  3
    .locals init ([0] class CCtorDUWithMember01a/C e2)
    .line 8,8 : 1,13 ''
    IL_0000:  nop
    IL_0001:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a::get_e2()
    IL_0006:  stloc.0
    IL_0007:  ret
  } // end of method $CCtorDUWithMember01a::.cctor

} // end of class '<StartupCode$CCtorDUWithMember01>'.$CCtorDUWithMember01a


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
