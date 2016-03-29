
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
.assembly Hash12
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash12
{
  // Offset: 0x00000000 Length: 0x00000A98
  // WARNING: managed resource file FSharpSignatureData.Hash12 created
}
.mresource public FSharpOptimizationData.Hash12
{
  // Offset: 0x00000AA0 Length: 0x00000589
  // WARNING: managed resource file FSharpOptimizationData.Hash12 created
}
.module Hash12.dll
// MVID: {56B7C2F3-9661-796E-A745-0383F3C2B756}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00C60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash12
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class Hash12/HashMicroPerfAndCodeGenerationTests/Key>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class Hash12/HashMicroPerfAndCodeGenerationTests/Key>,
                      [mscorlib]System.IComparable,
                      [mscorlib]System.Collections.IStructuralComparable
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly int32 item1
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly int32 item2
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class Hash12/HashMicroPerfAndCodeGenerationTests/Key 
              NewKey(int32 item1,
                     int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Hash12/HashMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
                                                                                                 int32)
        IL_0007:  ret
      } // end of method Key::NewKey

      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item1,
                                   int32 item2) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0014:  ret
      } // end of method Key::.ctor

      .method public hidebysig instance int32 
              get_Item1() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0006:  ret
      } // end of method Key::get_Item1

      .method public hidebysig instance int32 
              get_Item2() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0006:  ret
      } // end of method Key::get_Item2

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
      } // end of method Key::get_Tag

      .method assembly hidebysig specialname 
              instance object  __DebugDisplay() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::__DebugDisplay

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Hash12/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       120 (0x78)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 int32 V_2,
                 class [mscorlib]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_006e

        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_006c

        IL_000d:  ldarg.0
        IL_000e:  pop
        IL_000f:  ldarg.0
        IL_0010:  stloc.0
        IL_0011:  ldarg.1
        IL_0012:  stloc.1
        IL_0013:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0018:  stloc.3
        IL_0019:  ldloc.0
        IL_001a:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  stloc.s    V_4
        IL_0021:  ldloc.1
        IL_0022:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0027:  stloc.s    V_5
        IL_0029:  ldloc.s    V_4
        IL_002b:  ldloc.s    V_5
        IL_002d:  bge.s      IL_0033

        IL_002f:  ldc.i4.m1
        IL_0030:  nop
        IL_0031:  br.s       IL_003a

        IL_0033:  ldloc.s    V_4
        IL_0035:  ldloc.s    V_5
        IL_0037:  cgt
        IL_0039:  nop
        IL_003a:  stloc.2
        IL_003b:  ldloc.2
        IL_003c:  ldc.i4.0
        IL_003d:  bge.s      IL_0041

        IL_003f:  ldloc.2
        IL_0040:  ret

        IL_0041:  ldloc.2
        IL_0042:  ldc.i4.0
        IL_0043:  ble.s      IL_0047

        IL_0045:  ldloc.2
        IL_0046:  ret

        IL_0047:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_004c:  stloc.3
        IL_004d:  ldloc.0
        IL_004e:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0053:  stloc.s    V_4
        IL_0055:  ldloc.1
        IL_0056:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_005b:  stloc.s    V_5
        IL_005d:  ldloc.s    V_4
        IL_005f:  ldloc.s    V_5
        IL_0061:  bge.s      IL_0065

        IL_0063:  ldc.i4.m1
        IL_0064:  ret

        IL_0065:  ldloc.s    V_4
        IL_0067:  ldloc.s    V_5
        IL_0069:  cgt
        IL_006b:  ret

        IL_006c:  ldc.i4.1
        IL_006d:  ret

        IL_006e:  ldarg.1
        IL_006f:  ldnull
        IL_0070:  cgt.un
        IL_0072:  brfalse.s  IL_0076

        IL_0074:  ldc.i4.m1
        IL_0075:  ret

        IL_0076:  ldc.i4.0
        IL_0077:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 4,4 : 10,13
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldarg.1
        IL_0003:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_0008:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(class Hash12/HashMicroPerfAndCodeGenerationTests/Key)
        IL_000d:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       125 (0x7d)
        .maxstack  4
        .locals init ([0] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_2,
                 [3] int32 V_3,
                 [4] int32 V_4,
                 [5] int32 V_5)
        .line 4,4 : 10,13
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_0007:  stloc.0
        IL_0008:  ldarg.0
        IL_0009:  ldnull
        IL_000a:  cgt.un
        IL_000c:  brfalse.s  IL_006e

        IL_000e:  ldarg.1
        IL_000f:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_0014:  ldnull
        IL_0015:  cgt.un
        IL_0017:  brfalse.s  IL_006c

        IL_0019:  ldarg.0
        IL_001a:  pop
        IL_001b:  ldarg.0
        IL_001c:  stloc.1
        IL_001d:  ldloc.0
        IL_001e:  stloc.2
        IL_001f:  ldloc.1
        IL_0020:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0025:  stloc.s    V_4
        IL_0027:  ldloc.2
        IL_0028:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_002d:  stloc.s    V_5
        IL_002f:  ldloc.s    V_4
        IL_0031:  ldloc.s    V_5
        IL_0033:  bge.s      IL_0039

        IL_0035:  ldc.i4.m1
        IL_0036:  nop
        IL_0037:  br.s       IL_0040

        IL_0039:  ldloc.s    V_4
        IL_003b:  ldloc.s    V_5
        IL_003d:  cgt
        IL_003f:  nop
        IL_0040:  stloc.3
        IL_0041:  ldloc.3
        IL_0042:  ldc.i4.0
        IL_0043:  bge.s      IL_0047

        IL_0045:  ldloc.3
        IL_0046:  ret

        IL_0047:  ldloc.3
        IL_0048:  ldc.i4.0
        IL_0049:  ble.s      IL_004d

        IL_004b:  ldloc.3
        IL_004c:  ret

        IL_004d:  ldloc.1
        IL_004e:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0053:  stloc.s    V_4
        IL_0055:  ldloc.2
        IL_0056:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_005b:  stloc.s    V_5
        IL_005d:  ldloc.s    V_4
        IL_005f:  ldloc.s    V_5
        IL_0061:  bge.s      IL_0065

        IL_0063:  ldc.i4.m1
        IL_0064:  ret

        IL_0065:  ldloc.s    V_4
        IL_0067:  ldloc.s    V_5
        IL_0069:  cgt
        IL_006b:  ret

        IL_006c:  ldc.i4.1
        IL_006d:  ret

        IL_006e:  ldarg.1
        IL_006f:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_0074:  ldnull
        IL_0075:  cgt.un
        IL_0077:  brfalse.s  IL_007b

        IL_0079:  ldc.i4.m1
        IL_007a:  ret

        IL_007b:  ldc.i4.0
        IL_007c:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       61 (0x3d)
        .maxstack  7
        .locals init (int32 V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_003b

        IL_0007:  ldc.i4.0
        IL_0008:  stloc.0
        IL_0009:  ldarg.0
        IL_000a:  pop
        IL_000b:  ldarg.0
        IL_000c:  stloc.1
        IL_000d:  ldc.i4.0
        IL_000e:  stloc.0
        IL_000f:  ldc.i4     0x9e3779b9
        IL_0014:  ldloc.1
        IL_0015:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0029:  ldloc.1
        IL_002a:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_002f:  ldloc.0
        IL_0030:  ldc.i4.6
        IL_0031:  shl
        IL_0032:  ldloc.0
        IL_0033:  ldc.i4.2
        IL_0034:  shr
        IL_0035:  add
        IL_0036:  add
        IL_0037:  add
        IL_0038:  stloc.0
        IL_0039:  ldloc.0
        IL_003a:  ret

        IL_003b:  ldc.i4.0
        IL_003c:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        .line 4,4 : 10,13
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0007:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000c:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       64 (0x40)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_2)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0038

        IL_0007:  ldarg.1
        IL_0008:  isinst     Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_000d:  stloc.0
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_0036

        IL_0011:  ldarg.0
        IL_0012:  pop
        IL_0013:  ldarg.0
        IL_0014:  stloc.1
        IL_0015:  ldloc.0
        IL_0016:  stloc.2
        IL_0017:  ldloc.1
        IL_0018:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_001d:  ldloc.2
        IL_001e:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0023:  bne.un.s   IL_0034

        IL_0025:  ldloc.1
        IL_0026:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_002b:  ldloc.2
        IL_002c:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0031:  ceq
        IL_0033:  ret

        IL_0034:  ldc.i4.0
        IL_0035:  ret

        IL_0036:  ldc.i4.0
        IL_0037:  ret

        IL_0038:  ldarg.1
        IL_0039:  ldnull
        IL_003a:  cgt.un
        IL_003c:  ldc.i4.0
        IL_003d:  ceq
        IL_003f:  ret
      } // end of method Key::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Hash12/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       60 (0x3c)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0034

        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_0032

        IL_000d:  ldarg.0
        IL_000e:  pop
        IL_000f:  ldarg.0
        IL_0010:  stloc.0
        IL_0011:  ldarg.1
        IL_0012:  stloc.1
        IL_0013:  ldloc.0
        IL_0014:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0019:  ldloc.1
        IL_001a:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  bne.un.s   IL_0030

        IL_0021:  ldloc.0
        IL_0022:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0027:  ldloc.1
        IL_0028:  ldfld      int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_002d:  ceq
        IL_002f:  ret

        IL_0030:  ldc.i4.0
        IL_0031:  ret

        IL_0032:  ldc.i4.0
        IL_0033:  ret

        IL_0034:  ldarg.1
        IL_0035:  ldnull
        IL_0036:  cgt.un
        IL_0038:  ldc.i4.0
        IL_0039:  ceq
        IL_003b:  ret
      } // end of method Key::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  isinst     Hash12/HashMicroPerfAndCodeGenerationTests/Key
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  brfalse.s  IL_0013

        IL_000b:  ldarg.0
        IL_000c:  ldloc.0
        IL_000d:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/Key::Equals(class Hash12/HashMicroPerfAndCodeGenerationTests/Key)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } // end of method Key::Equals

      .property instance int32 Tag()
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } // end of property Key::Tag
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } // end of property Key::Item1
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } // end of property Key::Item2
    } // end of class Key

    .class auto autochar serializable sealed nested public beforefieldinit KeyWithInnerKeys
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [mscorlib]System.IComparable,
                      [mscorlib]System.Collections.IStructuralComparable
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly class Hash12/HashMicroPerfAndCodeGenerationTests/Key item1
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> item2
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys 
              NewKeyWithInnerKeys(class Hash12/HashMicroPerfAndCodeGenerationTests/Key item1,
                                  class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::.ctor(class Hash12/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                              class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>)
        IL_0007:  ret
      } // end of method KeyWithInnerKeys::NewKeyWithInnerKeys

      .method assembly specialname rtspecialname 
              instance void  .ctor(class Hash12/HashMicroPerfAndCodeGenerationTests/Key item1,
                                   class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0014:  ret
      } // end of method KeyWithInnerKeys::.ctor

      .method public hidebysig instance class Hash12/HashMicroPerfAndCodeGenerationTests/Key 
              get_Item1() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0006:  ret
      } // end of method KeyWithInnerKeys::get_Item1

      .method public hidebysig instance class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> 
              get_Item2() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0006:  ret
      } // end of method KeyWithInnerKeys::get_Item2

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
      } // end of method KeyWithInnerKeys::get_Tag

      .method assembly hidebysig specialname 
              instance object  __DebugDisplay() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyWithInnerKeys::__DebugDisplay

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       170 (0xaa)
        .maxstack  5
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 int32 V_2,
                 class [mscorlib]System.Collections.IComparer V_3,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_5,
                 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_6,
                 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_7,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_8,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_9,
                 int32 V_10)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse    IL_00a0

        IL_000a:  ldarg.1
        IL_000b:  ldnull
        IL_000c:  cgt.un
        IL_000e:  brfalse    IL_009e

        IL_0013:  ldarg.0
        IL_0014:  pop
        IL_0015:  ldarg.0
        IL_0016:  stloc.0
        IL_0017:  ldarg.1
        IL_0018:  stloc.1
        IL_0019:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_001e:  stloc.3
        IL_001f:  ldloc.0
        IL_0020:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0025:  stloc.s    V_4
        IL_0027:  ldloc.1
        IL_0028:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_002d:  stloc.s    V_5
        IL_002f:  ldloc.s    V_4
        IL_0031:  ldloc.s    V_5
        IL_0033:  ldloc.3
        IL_0034:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_0039:  stloc.2
        IL_003a:  ldloc.2
        IL_003b:  ldc.i4.0
        IL_003c:  bge.s      IL_0040

        IL_003e:  ldloc.2
        IL_003f:  ret

        IL_0040:  ldloc.2
        IL_0041:  ldc.i4.0
        IL_0042:  ble.s      IL_0046

        IL_0044:  ldloc.2
        IL_0045:  ret

        IL_0046:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_004b:  stloc.3
        IL_004c:  ldloc.0
        IL_004d:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0052:  stloc.s    V_6
        IL_0054:  ldloc.1
        IL_0055:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_005a:  stloc.s    V_7
        IL_005c:  ldloc.s    V_6
        IL_005e:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0063:  stloc.s    V_4
        IL_0065:  ldloc.s    V_6
        IL_0067:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_006c:  stloc.s    V_5
        IL_006e:  ldloc.s    V_7
        IL_0070:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0075:  stloc.s    V_8
        IL_0077:  ldloc.s    V_7
        IL_0079:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_007e:  stloc.s    V_9
        IL_0080:  ldloc.s    V_4
        IL_0082:  ldloc.s    V_8
        IL_0084:  ldloc.3
        IL_0085:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_008a:  stloc.s    V_10
        IL_008c:  ldloc.s    V_10
        IL_008e:  brfalse.s  IL_0093

        IL_0090:  ldloc.s    V_10
        IL_0092:  ret

        IL_0093:  ldloc.s    V_5
        IL_0095:  ldloc.s    V_9
        IL_0097:  ldloc.3
        IL_0098:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_009d:  ret

        IL_009e:  ldc.i4.1
        IL_009f:  ret

        IL_00a0:  ldarg.1
        IL_00a1:  ldnull
        IL_00a2:  cgt.un
        IL_00a4:  brfalse.s  IL_00a8

        IL_00a6:  ldc.i4.m1
        IL_00a7:  ret

        IL_00a8:  ldc.i4.0
        IL_00a9:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        .line 5,5 : 10,26
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldarg.1
        IL_0003:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0008:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::CompareTo(class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_000d:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       175 (0xaf)
        .maxstack  5
        .locals init ([0] class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 [1] class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 [2] class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
                 [3] int32 V_3,
                 [4] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 [5] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_5,
                 [6] class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_6,
                 [7] class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_7,
                 [8] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_8,
                 [9] class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_9,
                 [10] int32 V_10)
        .line 5,5 : 10,26
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0007:  stloc.0
        IL_0008:  ldarg.0
        IL_0009:  ldnull
        IL_000a:  cgt.un
        IL_000c:  brfalse    IL_00a0

        IL_0011:  ldarg.1
        IL_0012:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0017:  ldnull
        IL_0018:  cgt.un
        IL_001a:  brfalse    IL_009e

        IL_001f:  ldarg.0
        IL_0020:  pop
        IL_0021:  ldarg.0
        IL_0022:  stloc.1
        IL_0023:  ldloc.0
        IL_0024:  stloc.2
        IL_0025:  ldloc.1
        IL_0026:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_002b:  stloc.s    V_4
        IL_002d:  ldloc.2
        IL_002e:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0033:  stloc.s    V_5
        IL_0035:  ldloc.s    V_4
        IL_0037:  ldloc.s    V_5
        IL_0039:  ldarg.2
        IL_003a:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_003f:  stloc.3
        IL_0040:  ldloc.3
        IL_0041:  ldc.i4.0
        IL_0042:  bge.s      IL_0046

        IL_0044:  ldloc.3
        IL_0045:  ret

        IL_0046:  ldloc.3
        IL_0047:  ldc.i4.0
        IL_0048:  ble.s      IL_004c

        IL_004a:  ldloc.3
        IL_004b:  ret

        IL_004c:  ldloc.1
        IL_004d:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0052:  stloc.s    V_6
        IL_0054:  ldloc.2
        IL_0055:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_005a:  stloc.s    V_7
        IL_005c:  ldloc.s    V_6
        IL_005e:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0063:  stloc.s    V_4
        IL_0065:  ldloc.s    V_6
        IL_0067:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_006c:  stloc.s    V_5
        IL_006e:  ldloc.s    V_7
        IL_0070:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0075:  stloc.s    V_8
        IL_0077:  ldloc.s    V_7
        IL_0079:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_007e:  stloc.s    V_9
        IL_0080:  ldloc.s    V_4
        IL_0082:  ldloc.s    V_8
        IL_0084:  ldarg.2
        IL_0085:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_008a:  stloc.s    V_10
        IL_008c:  ldloc.s    V_10
        IL_008e:  brfalse.s  IL_0093

        IL_0090:  ldloc.s    V_10
        IL_0092:  ret

        IL_0093:  ldloc.s    V_5
        IL_0095:  ldloc.s    V_9
        IL_0097:  ldarg.2
        IL_0098:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [mscorlib]System.Collections.IComparer)
        IL_009d:  ret

        IL_009e:  ldc.i4.1
        IL_009f:  ret

        IL_00a0:  ldarg.1
        IL_00a1:  unbox.any  Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_00a6:  ldnull
        IL_00a7:  cgt.un
        IL_00a9:  brfalse.s  IL_00ad

        IL_00ab:  ldc.i4.m1
        IL_00ac:  ret

        IL_00ad:  ldc.i4.0
        IL_00ae:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       108 (0x6c)
        .maxstack  7
        .locals init (int32 V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_2,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_3,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 int32 V_5)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_006a

        IL_0007:  ldc.i4.0
        IL_0008:  stloc.0
        IL_0009:  ldarg.0
        IL_000a:  pop
        IL_000b:  ldarg.0
        IL_000c:  stloc.1
        IL_000d:  ldc.i4.0
        IL_000e:  stloc.0
        IL_000f:  ldc.i4     0x9e3779b9
        IL_0014:  ldloc.1
        IL_0015:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_001a:  stloc.2
        IL_001b:  ldloc.2
        IL_001c:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0021:  stloc.3
        IL_0022:  ldloc.2
        IL_0023:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0028:  stloc.s    V_4
        IL_002a:  ldloc.3
        IL_002b:  ldarg.1
        IL_002c:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_0031:  stloc.s    V_5
        IL_0033:  ldloc.s    V_5
        IL_0035:  ldc.i4.5
        IL_0036:  shl
        IL_0037:  ldloc.s    V_5
        IL_0039:  add
        IL_003a:  ldloc.s    V_4
        IL_003c:  ldarg.1
        IL_003d:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_0042:  xor
        IL_0043:  ldloc.0
        IL_0044:  ldc.i4.6
        IL_0045:  shl
        IL_0046:  ldloc.0
        IL_0047:  ldc.i4.2
        IL_0048:  shr
        IL_0049:  add
        IL_004a:  add
        IL_004b:  add
        IL_004c:  stloc.0
        IL_004d:  ldc.i4     0x9e3779b9
        IL_0052:  ldloc.1
        IL_0053:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0058:  ldarg.1
        IL_0059:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_005e:  ldloc.0
        IL_005f:  ldc.i4.6
        IL_0060:  shl
        IL_0061:  ldloc.0
        IL_0062:  ldc.i4.2
        IL_0063:  shr
        IL_0064:  add
        IL_0065:  add
        IL_0066:  add
        IL_0067:  stloc.0
        IL_0068:  ldloc.0
        IL_0069:  ret

        IL_006a:  ldc.i4.0
        IL_006b:  ret
      } // end of method KeyWithInnerKeys::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        .line 5,5 : 10,26
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0007:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000c:  ret
      } // end of method KeyWithInnerKeys::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       139 (0x8b)
        .maxstack  5
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_3,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_5,
                 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> V_6,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_7,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/Key V_8)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse    IL_0083

        IL_000a:  ldarg.1
        IL_000b:  isinst     Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0010:  stloc.0
        IL_0011:  ldloc.0
        IL_0012:  brfalse.s  IL_0081

        IL_0014:  ldarg.0
        IL_0015:  pop
        IL_0016:  ldarg.0
        IL_0017:  stloc.1
        IL_0018:  ldloc.0
        IL_0019:  stloc.2
        IL_001a:  ldloc.1
        IL_001b:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0020:  stloc.3
        IL_0021:  ldloc.2
        IL_0022:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0027:  stloc.s    V_4
        IL_0029:  ldloc.3
        IL_002a:  ldloc.s    V_4
        IL_002c:  ldarg.2
        IL_002d:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                  class [mscorlib]System.Collections.IEqualityComparer)
        IL_0032:  brfalse.s  IL_007f

        IL_0034:  ldloc.1
        IL_0035:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_003a:  stloc.s    V_5
        IL_003c:  ldloc.2
        IL_003d:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0042:  stloc.s    V_6
        IL_0044:  ldloc.s    V_5
        IL_0046:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_004b:  stloc.3
        IL_004c:  ldloc.s    V_5
        IL_004e:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0053:  stloc.s    V_4
        IL_0055:  ldloc.s    V_6
        IL_0057:  call       instance !0 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_005c:  stloc.s    V_7
        IL_005e:  ldloc.s    V_6
        IL_0060:  call       instance !1 class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0065:  stloc.s    V_8
        IL_0067:  ldloc.3
        IL_0068:  ldloc.s    V_7
        IL_006a:  ldarg.2
        IL_006b:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                  class [mscorlib]System.Collections.IEqualityComparer)
        IL_0070:  brfalse.s  IL_007d

        IL_0072:  ldloc.s    V_4
        IL_0074:  ldloc.s    V_8
        IL_0076:  ldarg.2
        IL_0077:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                  class [mscorlib]System.Collections.IEqualityComparer)
        IL_007c:  ret

        IL_007d:  ldc.i4.0
        IL_007e:  ret

        IL_007f:  ldc.i4.0
        IL_0080:  ret

        IL_0081:  ldc.i4.0
        IL_0082:  ret

        IL_0083:  ldarg.1
        IL_0084:  ldnull
        IL_0085:  cgt.un
        IL_0087:  ldc.i4.0
        IL_0088:  ceq
        IL_008a:  ret
      } // end of method KeyWithInnerKeys::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       70 (0x46)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_003e

        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_003c

        IL_000d:  ldarg.0
        IL_000e:  pop
        IL_000f:  ldarg.0
        IL_0010:  stloc.0
        IL_0011:  ldarg.1
        IL_0012:  stloc.1
        IL_0013:  ldloc.0
        IL_0014:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0019:  ldloc.1
        IL_001a:  ldfld      class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_001f:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/Key::Equals(class Hash12/HashMicroPerfAndCodeGenerationTests/Key)
        IL_0024:  brfalse.s  IL_003a

        IL_0026:  ldloc.0
        IL_0027:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_002c:  ldloc.1
        IL_002d:  ldfld      class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0032:  tail.
        IL_0034:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>>(!!0,
                                                                                                                                                                                                                                                                           !!0)
        IL_0039:  ret

        IL_003a:  ldc.i4.0
        IL_003b:  ret

        IL_003c:  ldc.i4.0
        IL_003d:  ret

        IL_003e:  ldarg.1
        IL_003f:  ldnull
        IL_0040:  cgt.un
        IL_0042:  ldc.i4.0
        IL_0043:  ceq
        IL_0045:  ret
      } // end of method KeyWithInnerKeys::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  4
        .locals init (class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0)
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  isinst     Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  brfalse.s  IL_0013

        IL_000b:  ldarg.0
        IL_000c:  ldloc.0
        IL_000d:  callvirt   instance bool Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::Equals(class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } // end of method KeyWithInnerKeys::Equals

      .property instance int32 Tag()
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Tag()
      } // end of property KeyWithInnerKeys::Tag
      .property instance class Hash12/HashMicroPerfAndCodeGenerationTests/Key
              Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item1()
      } // end of property KeyWithInnerKeys::Item1
      .property instance class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>
              Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key> Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item2()
      } // end of property KeyWithInnerKeys::Item2
    } // end of class KeyWithInnerKeys

    .method public static void  f9() cil managed
    {
      // Code size       62 (0x3e)
      .maxstack  6
      .locals init ([0] class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys key,
               [1] int32 i,
               [2] int32 V_2)
      .line 8,8 : 8,64
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  ldc.i4.2
      IL_0003:  call       class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_0008:  ldc.i4.1
      IL_0009:  ldc.i4.2
      IL_000a:  call       class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_000f:  ldc.i4.1
      IL_0010:  ldc.i4.2
      IL_0011:  call       class Hash12/HashMicroPerfAndCodeGenerationTests/Key Hash12/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_0016:  newobj     instance void class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>::.ctor(!0,
                                                                                                                                                                                          !1)
      IL_001b:  call       class Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::NewKeyWithInnerKeys(class Hash12/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                                                                                              class [mscorlib]System.Tuple`2<class Hash12/HashMicroPerfAndCodeGenerationTests/Key,class Hash12/HashMicroPerfAndCodeGenerationTests/Key>)
      IL_0020:  stloc.0
      .line 9,9 : 8,32
      IL_0021:  ldc.i4.0
      IL_0022:  stloc.1
      IL_0023:  br.s       IL_0035

      .line 10,10 : 12,30
      IL_0025:  ldloc.0
      IL_0026:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityERComparer()
      IL_002b:  callvirt   instance int32 Hash12/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_0030:  stloc.2
      IL_0031:  ldloc.1
      IL_0032:  ldc.i4.1
      IL_0033:  add
      IL_0034:  stloc.1
      .line 9,9 : 21,29
      IL_0035:  ldloc.1
      IL_0036:  ldc.i4     0x989681
      IL_003b:  blt.s      IL_0025

      IL_003d:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f9

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash12

.class private abstract auto ansi sealed '<StartupCode$Hash12>'.$Hash12$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash12>'.$Hash12$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
