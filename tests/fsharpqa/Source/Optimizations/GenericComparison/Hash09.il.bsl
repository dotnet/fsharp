
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
.assembly Hash09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash09
{
  // Offset: 0x00000000 Length: 0x0000087E
}
.mresource public FSharpOptimizationData.Hash09
{
  // Offset: 0x00000888 Length: 0x00000680
}
.module Hash09.dll
// MVID: {611C550D-9642-77DB-A745-03830D551C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07230000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit GenericKey`1<a>
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>,
                      [mscorlib]System.IComparable,
                      [mscorlib]System.Collections.IStructuralComparable
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly !a item1
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly !a item2
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> 
              NewGenericKey(!a item1,
                            !a item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::.ctor(!0,
                                                                                                                    !0)
        IL_0007:  ret
      } // end of method GenericKey`1::NewGenericKey

      .method assembly specialname rtspecialname 
              instance void  .ctor(!a item1,
                                   !a item2) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0014:  ret
      } // end of method GenericKey`1::.ctor

      .method public hidebysig instance !a 
              get_Item1() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0006:  ret
      } // end of method GenericKey`1::get_Item1

      .method public hidebysig instance !a 
              get_Item2() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0006:  ret
      } // end of method GenericKey`1::get_Item2

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
      } // end of method GenericKey`1::get_Tag

      .method assembly hidebysig specialname 
              instance object  __DebugDisplay() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method GenericKey`1::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method GenericKey`1::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       111 (0x6f)
        .maxstack  5
        .locals init ([0] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 [1] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 [2] int32 V_2,
                 [3] class [mscorlib]System.Collections.IComparer V_3,
                 [4] !a V_4,
                 [5] !a V_5)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 4,4 : 10,20 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Hash09.fsx'
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0065

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_0063

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.0
        IL_000e:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_000f:  ldarg.0
        IL_0010:  stloc.0
        IL_0011:  ldarg.1
        IL_0012:  stloc.1
        IL_0013:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0018:  stloc.3
        IL_0019:  ldloc.0
        IL_001a:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_001f:  stloc.s    V_4
        IL_0021:  ldloc.1
        IL_0022:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0027:  stloc.s    V_5
        IL_0029:  ldloc.3
        IL_002a:  ldloc.s    V_4
        IL_002c:  ldloc.s    V_5
        IL_002e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0033:  stloc.2
        .line 16707566,16707566 : 0,0 ''
        IL_0034:  ldloc.2
        IL_0035:  ldc.i4.0
        IL_0036:  bge.s      IL_003a

        .line 16707566,16707566 : 0,0 ''
        IL_0038:  ldloc.2
        IL_0039:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_003a:  ldloc.2
        IL_003b:  ldc.i4.0
        IL_003c:  ble.s      IL_0040

        .line 16707566,16707566 : 0,0 ''
        IL_003e:  ldloc.2
        IL_003f:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0040:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0045:  stloc.3
        IL_0046:  ldloc.0
        IL_0047:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_004c:  stloc.s    V_4
        IL_004e:  ldloc.1
        IL_004f:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0054:  stloc.s    V_5
        IL_0056:  ldloc.3
        IL_0057:  ldloc.s    V_4
        IL_0059:  ldloc.s    V_5
        IL_005b:  tail.
        IL_005d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0062:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0063:  ldc.i4.1
        IL_0064:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0065:  ldarg.1
        IL_0066:  ldnull
        IL_0067:  cgt.un
        IL_0069:  brfalse.s  IL_006d

        .line 16707566,16707566 : 0,0 ''
        IL_006b:  ldc.i4.m1
        IL_006c:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006d:  ldc.i4.0
        IL_006e:  ret
      } // end of method GenericKey`1::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        .line 4,4 : 10,20 ''
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0007:  tail.
        IL_0009:  callvirt   instance int32 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::CompareTo(class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!0>)
        IL_000e:  ret
      } // end of method GenericKey`1::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       115 (0x73)
        .maxstack  5
        .locals init ([0] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 [1] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 [2] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_2,
                 [3] int32 V_3,
                 [4] !a V_4,
                 [5] !a V_5)
        .line 4,4 : 10,20 ''
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.0
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_0064

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.1
        IL_000e:  unbox.any  class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0013:  ldnull
        IL_0014:  cgt.un
        IL_0016:  brfalse.s  IL_0062

        .line 16707566,16707566 : 0,0 ''
        IL_0018:  ldarg.0
        IL_0019:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_001a:  ldarg.0
        IL_001b:  stloc.1
        IL_001c:  ldloc.0
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.2
        IL_0027:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_002c:  stloc.s    V_5
        IL_002e:  ldarg.2
        IL_002f:  ldloc.s    V_4
        IL_0031:  ldloc.s    V_5
        IL_0033:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0038:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_0039:  ldloc.3
        IL_003a:  ldc.i4.0
        IL_003b:  bge.s      IL_003f

        .line 16707566,16707566 : 0,0 ''
        IL_003d:  ldloc.3
        IL_003e:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_003f:  ldloc.3
        IL_0040:  ldc.i4.0
        IL_0041:  ble.s      IL_0045

        .line 16707566,16707566 : 0,0 ''
        IL_0043:  ldloc.3
        IL_0044:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0045:  ldloc.1
        IL_0046:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_004b:  stloc.s    V_4
        IL_004d:  ldloc.2
        IL_004e:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0053:  stloc.s    V_5
        IL_0055:  ldarg.2
        IL_0056:  ldloc.s    V_4
        IL_0058:  ldloc.s    V_5
        IL_005a:  tail.
        IL_005c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0061:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0062:  ldc.i4.1
        IL_0063:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0064:  ldarg.1
        IL_0065:  unbox.any  class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_006a:  ldnull
        IL_006b:  cgt.un
        IL_006d:  brfalse.s  IL_0071

        .line 16707566,16707566 : 0,0 ''
        IL_006f:  ldc.i4.m1
        IL_0070:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0071:  ldc.i4.0
        IL_0072:  ret
      } // end of method GenericKey`1::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       77 (0x4d)
        .maxstack  7
        .locals init ([0] int32 V_0,
                 [1] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 [2] !a V_2)
        .line 4,4 : 10,20 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_004b

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldc.i4.0
        IL_0008:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0009:  ldarg.0
        IL_000a:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_000b:  ldarg.0
        IL_000c:  stloc.1
        IL_000d:  ldc.i4.0
        IL_000e:  stloc.0
        IL_000f:  ldc.i4     0x9e3779b9
        IL_0014:  ldloc.1
        IL_0015:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_001a:  stloc.2
        IL_001b:  ldarg.1
        IL_001c:  ldloc.2
        IL_001d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.6
        IL_0024:  shl
        IL_0025:  ldloc.0
        IL_0026:  ldc.i4.2
        IL_0027:  shr
        IL_0028:  add
        IL_0029:  add
        IL_002a:  add
        IL_002b:  stloc.0
        IL_002c:  ldc.i4     0x9e3779b9
        IL_0031:  ldloc.1
        IL_0032:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0037:  stloc.2
        IL_0038:  ldarg.1
        IL_0039:  ldloc.2
        IL_003a:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_003f:  ldloc.0
        IL_0040:  ldc.i4.6
        IL_0041:  shl
        IL_0042:  ldloc.0
        IL_0043:  ldc.i4.2
        IL_0044:  shr
        IL_0045:  add
        IL_0046:  add
        IL_0047:  add
        IL_0048:  stloc.0
        IL_0049:  ldloc.0
        IL_004a:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004b:  ldc.i4.0
        IL_004c:  ret
      } // end of method GenericKey`1::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        .line 4,4 : 10,20 ''
        IL_0000:  ldarg.0
        IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method GenericKey`1::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       88 (0x58)
        .maxstack  5
        .locals init ([0] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 [1] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 [2] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_2,
                 [3] !a V_3,
                 [4] !a V_4)
        .line 4,4 : 10,20 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0050

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  isinst     class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_000d:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_004e

        .line 16707566,16707566 : 0,0 ''
        IL_0011:  ldarg.0
        IL_0012:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_0013:  ldarg.0
        IL_0014:  stloc.1
        IL_0015:  ldloc.0
        IL_0016:  stloc.2
        .line 16707566,16707566 : 0,0 ''
        IL_0017:  ldloc.1
        IL_0018:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_001d:  stloc.3
        IL_001e:  ldloc.2
        IL_001f:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0024:  stloc.s    V_4
        IL_0026:  ldarg.2
        IL_0027:  ldloc.3
        IL_0028:  ldloc.s    V_4
        IL_002a:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_002f:  brfalse.s  IL_004c

        .line 16707566,16707566 : 0,0 ''
        IL_0031:  ldloc.1
        IL_0032:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0037:  stloc.3
        IL_0038:  ldloc.2
        IL_0039:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_003e:  stloc.s    V_4
        IL_0040:  ldarg.2
        IL_0041:  ldloc.3
        IL_0042:  ldloc.s    V_4
        IL_0044:  tail.
        IL_0046:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!a>(class [mscorlib]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_004b:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004c:  ldc.i4.0
        IL_004d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004e:  ldc.i4.0
        IL_004f:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0050:  ldarg.1
        IL_0051:  ldnull
        IL_0052:  cgt.un
        IL_0054:  ldc.i4.0
        IL_0055:  ceq
        IL_0057:  ret
      } // end of method GenericKey`1::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       78 (0x4e)
        .maxstack  4
        .locals init ([0] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 [1] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 [2] !a V_2,
                 [3] !a V_3)
        .line 4,4 : 10,20 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0046

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_0044

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.0
        IL_000e:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_000f:  ldarg.0
        IL_0010:  stloc.0
        IL_0011:  ldarg.1
        IL_0012:  stloc.1
        .line 16707566,16707566 : 0,0 ''
        IL_0013:  ldloc.0
        IL_0014:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0019:  stloc.2
        IL_001a:  ldloc.1
        IL_001b:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0020:  stloc.3
        IL_0021:  ldloc.2
        IL_0022:  ldloc.3
        IL_0023:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!a>(!!0,
                                                                                                                                    !!0)
        IL_0028:  brfalse.s  IL_0042

        .line 16707566,16707566 : 0,0 ''
        IL_002a:  ldloc.0
        IL_002b:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0030:  stloc.2
        IL_0031:  ldloc.1
        IL_0032:  ldfld      !0 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0037:  stloc.3
        IL_0038:  ldloc.2
        IL_0039:  ldloc.3
        IL_003a:  tail.
        IL_003c:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!a>(!!0,
                                                                                                                                    !!0)
        IL_0041:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0042:  ldc.i4.0
        IL_0043:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0044:  ldc.i4.0
        IL_0045:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0046:  ldarg.1
        IL_0047:  ldnull
        IL_0048:  cgt.un
        IL_004a:  ldc.i4.0
        IL_004b:  ceq
        IL_004d:  ret
      } // end of method GenericKey`1::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  4
        .locals init ([0] class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0)
        .line 4,4 : 10,20 ''
        IL_0000:  ldarg.1
        IL_0001:  isinst     class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0014

        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  tail.
        IL_000e:  callvirt   instance bool class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::Equals(class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!0>)
        IL_0013:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0014:  ldc.i4.0
        IL_0015:  ret
      } // end of method GenericKey`1::Equals

      .property instance int32 Tag()
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1::get_Tag()
      } // end of property GenericKey`1::Tag
      .property instance !a Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance !a Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1::get_Item1()
      } // end of property GenericKey`1::Item1
      .property instance !a Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance !a Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1::get_Item2()
      } // end of property GenericKey`1::Item2
    } // end of class GenericKey`1

    .method public static void  f6() cil managed
    {
      // Code size       35 (0x23)
      .maxstack  4
      .locals init ([0] int32 i,
               [1] int32 V_1)
      .line 7,7 : 8,32 ''
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_001a

      .line 8,8 : 12,44 ''
      IL_0004:  ldc.i4.1
      IL_0005:  ldc.i4.2
      IL_0006:  call       class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<!0> class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<int32>::NewGenericKey(!0,
                                                                                                                                                                                 !0)
      IL_000b:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityERComparer()
      IL_0010:  callvirt   instance int32 class Hash09/HashMicroPerfAndCodeGenerationTests/GenericKey`1<int32>::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_0015:  stloc.1
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.1
      IL_0018:  add
      IL_0019:  stloc.0
      .line 7,7 : 8,32 ''
      IL_001a:  ldloc.0
      IL_001b:  ldc.i4     0x989681
      IL_0020:  blt.s      IL_0004

      IL_0022:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f6

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash09

.class private abstract auto ansi sealed '<StartupCode$Hash09>'.$Hash09$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash09>'.$Hash09$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
