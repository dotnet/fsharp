
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
.assembly CCtorDUWithMember01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

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
// MVID: {59B1923F-26F1-14EE-A745-03833F92B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00A70000


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
    // Code size       61 (0x3d)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             [1] class CCtorDUWithMember01a/C V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2,
             [3] class CCtorDUWithMember01a/C V_3)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 2,2 : 1,23 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\CCtorDUWithMember\\CCtorDUWithMember01.fs'
    IL_0000:  ldstr      "File1.A = %A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class CCtorDUWithMember01a/C>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_A()
    IL_0015:  stloc.1
    IL_0016:  ldloc.0
    IL_0017:  ldloc.1
    IL_0018:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001d:  pop
    .line 3,3 : 1,23 ''
    IL_001e:  ldstr      "Test.e2 = %A"
    IL_0023:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class CCtorDUWithMember01a/C>::.ctor(string)
    IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002d:  stloc.2
    IL_002e:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a::get_e2()
    IL_0033:  stloc.3
    IL_0034:  ldloc.2
    IL_0035:  ldloc.3
    IL_0036:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003b:  pop
    IL_003c:  ret
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

    .field assembly initonly int32 _tag
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
      IL_0001:  newobj     instance void CCtorDUWithMember01a/C::.ctor(int32)
      IL_0006:  stsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_A
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void CCtorDUWithMember01a/C::.ctor(int32)
      IL_0011:  stsfld     class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::_unique_B
      IL_0016:  ret
    } // end of method C::.cctor

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
      IL_0008:  stfld      int32 CCtorDUWithMember01a/C::_tag
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
      IL_0001:  ldfld      int32 CCtorDUWithMember01a/C::_tag
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

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class CCtorDUWithMember01a/C>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class CCtorDUWithMember01a/C,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method C::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class CCtorDUWithMember01a/C obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       64 (0x40)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] int32 V_1)
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\CCtorDUWithMember\\CCtorDUWithMember01a.fs'
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
      IL_0015:  ldfld      int32 CCtorDUWithMember01a/C::_tag
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  ldfld      int32 CCtorDUWithMember01a/C::_tag
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
    } // end of method C::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 3,3 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  CCtorDUWithMember01a/C
      IL_0007:  callvirt   instance int32 CCtorDUWithMember01a/C::CompareTo(class CCtorDUWithMember01a/C)
      IL_000c:  ret
    } // end of method C::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       81 (0x51)
      .maxstack  4
      .locals init ([0] class CCtorDUWithMember01a/C V_0,
               [1] int32 V_1,
               [2] int32 V_2)
      .line 3,3 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  CCtorDUWithMember01a/C
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  br.s       IL_0011

      IL_000f:  br.s       IL_003e

      .line 100001,100001 : 0,0 ''
      IL_0011:  ldarg.1
      IL_0012:  unbox.any  CCtorDUWithMember01a/C
      IL_0017:  ldnull
      IL_0018:  cgt.un
      IL_001a:  brfalse.s  IL_001e

      IL_001c:  br.s       IL_0020

      IL_001e:  br.s       IL_003c

      .line 100001,100001 : 0,0 ''
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 CCtorDUWithMember01a/C::_tag
      IL_0026:  stloc.1
      IL_0027:  ldloc.0
      IL_0028:  ldfld      int32 CCtorDUWithMember01a/C::_tag
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
      IL_003f:  unbox.any  CCtorDUWithMember01a/C
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
    } // end of method C::CompareTo

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
      IL_000d:  ldfld      int32 CCtorDUWithMember01a/C::_tag
      IL_0012:  ret

      .line 100001,100001 : 0,0 ''
      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method C::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 3,3 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 CCtorDUWithMember01a/C::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method C::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       55 (0x37)
      .maxstack  4
      .locals init ([0] class CCtorDUWithMember01a/C V_0,
               [1] class CCtorDUWithMember01a/C V_1,
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
      IL_000b:  isinst     CCtorDUWithMember01a/C
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_0018

      IL_0016:  br.s       IL_002d

      .line 100001,100001 : 0,0 ''
      IL_0018:  ldloc.0
      IL_0019:  stloc.1
      IL_001a:  ldarg.0
      IL_001b:  ldfld      int32 CCtorDUWithMember01a/C::_tag
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  ldfld      int32 CCtorDUWithMember01a/C::_tag
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
    } // end of method C::Equals

    .method public hidebysig specialname 
            instance int32  get_P() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 6,6 : 18,19 ''
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } // end of method C::get_P

    .method public hidebysig virtual final 
            instance bool  Equals(class CCtorDUWithMember01a/C obj) cil managed
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
      IL_0015:  ldfld      int32 CCtorDUWithMember01a/C::_tag
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  ldfld      int32 CCtorDUWithMember01a/C::_tag
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
    } // end of method C::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       24 (0x18)
      .maxstack  4
      .locals init ([0] class CCtorDUWithMember01a/C V_0)
      .line 3,3 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     CCtorDUWithMember01a/C
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance bool CCtorDUWithMember01a/C::Equals(class CCtorDUWithMember01a/C)
      IL_0015:  ret

      .line 100001,100001 : 0,0 ''
      IL_0016:  ldc.i4.0
      IL_0017:  ret
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
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a/C::get_A()
    IL_0005:  ret
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
    // Code size       7 (0x7)
    .maxstack  3
    .locals init ([0] class CCtorDUWithMember01a/C e2)
    .line 8,8 : 1,13 ''
    IL_0000:  call       class CCtorDUWithMember01a/C CCtorDUWithMember01a::get_e2()
    IL_0005:  stloc.0
    IL_0006:  ret
  } // end of method $CCtorDUWithMember01a::.cctor

} // end of class '<StartupCode$CCtorDUWithMember01>'.$CCtorDUWithMember01a


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
