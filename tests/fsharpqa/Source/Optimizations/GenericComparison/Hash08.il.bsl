
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
  .ver 6:0:0:0
}
.assembly Hash08
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash08
{
  // Offset: 0x00000000 Length: 0x000006C3
}
.mresource public FSharpOptimizationData.Hash08
{
  // Offset: 0x000006C8 Length: 0x000003B3
}
.module Hash08.dll
// MVID: {6220E4FA-9642-77BC-A745-0383FAE42062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x066F0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash08
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public KeyR
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR>,
                      [mscorlib]System.IComparable,
                      [mscorlib]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .field assembly int32 key1@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field assembly int32 key2@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public hidebysig specialname 
              instance int32  get_key1() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0006:  ret
      } // end of method KeyR::get_key1

      .method public hidebysig specialname 
              instance int32  get_key2() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0006:  ret
      } // end of method KeyR::get_key2

      .method public specialname rtspecialname 
              instance void  .ctor(int32 key1,
                                   int32 key2) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0014:  ret
      } // end of method KeyR::.ctor

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyR::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       92 (0x5c)
        .maxstack  4
        .locals init ([0] int32 V_0,
                 [1] class [mscorlib]System.Collections.IComparer V_1,
                 [2] int32 V_2,
                 [3] int32 V_3)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 16707566,16707566 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Hash08.fsx'
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0055

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0053

        .line 16707566,16707566 : 0,0 ''
        IL_0006:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  stloc.2
        IL_0013:  ldarg.1
        IL_0014:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_001a:  ldloc.2
        IL_001b:  ldloc.3
        IL_001c:  bge.s      IL_0022

        .line 16707566,16707566 : 0,0 ''
        IL_001e:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_001f:  nop
        IL_0020:  br.s       IL_0027

        .line 16707566,16707566 : 0,0 ''
        IL_0022:  ldloc.2
        IL_0023:  ldloc.3
        IL_0024:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_0026:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0027:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.0
        IL_002a:  bge.s      IL_002e

        .line 16707566,16707566 : 0,0 ''
        IL_002c:  ldloc.0
        IL_002d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.0
        IL_0030:  ble.s      IL_0034

        .line 16707566,16707566 : 0,0 ''
        IL_0032:  ldloc.0
        IL_0033:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0034:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0039:  stloc.1
        IL_003a:  ldarg.0
        IL_003b:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0040:  stloc.2
        IL_0041:  ldarg.1
        IL_0042:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0047:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_0048:  ldloc.2
        IL_0049:  ldloc.3
        IL_004a:  bge.s      IL_004e

        .line 16707566,16707566 : 0,0 ''
        IL_004c:  ldc.i4.m1
        IL_004d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004e:  ldloc.2
        IL_004f:  ldloc.3
        IL_0050:  cgt
        IL_0052:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0053:  ldc.i4.1
        IL_0054:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0055:  ldarg.1
        IL_0056:  brfalse.s  IL_005a

        .line 16707566,16707566 : 0,0 ''
        IL_0058:  ldc.i4.m1
        IL_0059:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_005a:  ldc.i4.0
        IL_005b:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0007:  callvirt   instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::CompareTo(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_000c:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       97 (0x61)
        .maxstack  4
        .locals init ([0] class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0,
                 [1] int32 V_1,
                 [2] int32 V_2,
                 [3] int32 V_3)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0055

        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldarg.1
        IL_000b:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0010:  brfalse.s  IL_0053

        .line 16707566,16707566 : 0,0 ''
        IL_0012:  ldarg.0
        IL_0013:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0018:  stloc.2
        IL_0019:  ldloc.0
        IL_001a:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_001f:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_0020:  ldloc.2
        IL_0021:  ldloc.3
        IL_0022:  bge.s      IL_0028

        .line 16707566,16707566 : 0,0 ''
        IL_0024:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_0025:  nop
        IL_0026:  br.s       IL_002d

        .line 16707566,16707566 : 0,0 ''
        IL_0028:  ldloc.2
        IL_0029:  ldloc.3
        IL_002a:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_002c:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_002d:  stloc.1
        .line 16707566,16707566 : 0,0 ''
        IL_002e:  ldloc.1
        IL_002f:  ldc.i4.0
        IL_0030:  bge.s      IL_0034

        .line 16707566,16707566 : 0,0 ''
        IL_0032:  ldloc.1
        IL_0033:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0034:  ldloc.1
        IL_0035:  ldc.i4.0
        IL_0036:  ble.s      IL_003a

        .line 16707566,16707566 : 0,0 ''
        IL_0038:  ldloc.1
        IL_0039:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_003a:  ldarg.0
        IL_003b:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0040:  stloc.2
        IL_0041:  ldloc.0
        IL_0042:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0047:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_0048:  ldloc.2
        IL_0049:  ldloc.3
        IL_004a:  bge.s      IL_004e

        .line 16707566,16707566 : 0,0 ''
        IL_004c:  ldc.i4.m1
        IL_004d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004e:  ldloc.2
        IL_004f:  ldloc.3
        IL_0050:  cgt
        IL_0052:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0053:  ldc.i4.1
        IL_0054:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0055:  ldarg.1
        IL_0056:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_005b:  brfalse.s  IL_005f

        .line 16707566,16707566 : 0,0 ''
        IL_005d:  ldc.i4.m1
        IL_005e:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_005f:  ldc.i4.0
        IL_0060:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       51 (0x33)
        .maxstack  7
        .locals init ([0] int32 V_0)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0031

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldc.i4     0x9e3779b9
        IL_000a:  ldarg.0
        IL_000b:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0010:  ldloc.0
        IL_0011:  ldc.i4.6
        IL_0012:  shl
        IL_0013:  ldloc.0
        IL_0014:  ldc.i4.2
        IL_0015:  shr
        IL_0016:  add
        IL_0017:  add
        IL_0018:  add
        IL_0019:  stloc.0
        IL_001a:  ldc.i4     0x9e3779b9
        IL_001f:  ldarg.0
        IL_0020:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0025:  ldloc.0
        IL_0026:  ldc.i4.6
        IL_0027:  shl
        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.2
        IL_002a:  shr
        IL_002b:  add
        IL_002c:  add
        IL_002d:  add
        IL_002e:  stloc.0
        IL_002f:  ldloc.0
        IL_0030:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0031:  ldc.i4.0
        IL_0032:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       54 (0x36)
        .maxstack  4
        .locals init ([0] class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002e

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  isinst     Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0009:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_002c

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.0
        IL_000e:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0013:  ldloc.0
        IL_0014:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  bne.un.s   IL_002a

        .line 16707566,16707566 : 0,0 ''
        IL_001b:  ldarg.0
        IL_001c:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0021:  ldloc.0
        IL_0022:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0027:  ceq
        IL_0029:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_002a:  ldc.i4.0
        IL_002b:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_002c:  ldc.i4.0
        IL_002d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_002e:  ldarg.1
        IL_002f:  ldnull
        IL_0030:  cgt.un
        IL_0032:  ldc.i4.0
        IL_0033:  ceq
        IL_0035:  ret
      } // end of method KeyR::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       47 (0x2f)
        .maxstack  8
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0025

        .line 16707566,16707566 : 0,0 ''
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000c:  ldarg.1
        IL_000d:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  bne.un.s   IL_0023

        .line 16707566,16707566 : 0,0 ''
        IL_0014:  ldarg.0
        IL_0015:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_001a:  ldarg.1
        IL_001b:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0020:  ceq
        IL_0022:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0023:  ldc.i4.0
        IL_0024:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0025:  ldc.i4.0
        IL_0026:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0027:  ldarg.1
        IL_0028:  ldnull
        IL_0029:  cgt.un
        IL_002b:  ldc.i4.0
        IL_002c:  ceq
        IL_002e:  ret
      } // end of method KeyR::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init ([0] class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  isinst     Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::Equals(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_0011:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method KeyR::Equals

      .property instance int32 key1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::get_key1()
      } // end of property KeyR::key1
      .property instance int32 key2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
        .get instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::get_key2()
      } // end of property KeyR::key2
    } // end of class KeyR

    .method public static void  f5c() cil managed
    {
      // Code size       36 (0x24)
      .maxstack  4
      .locals init ([0] int32 i,
               [1] int32 V_1)
      .line 6,6 : 8,25 ''
      IL_0000:  nop
      .line 7,7 : 8,11 ''
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_001b

      .line 8,8 : 12,49 ''
      IL_0005:  ldc.i4.1
      IL_0006:  ldc.i4.2
      IL_0007:  newobj     instance void Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::.ctor(int32,
                                                                                                int32)
      IL_000c:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityERComparer()
      IL_0011:  callvirt   instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_0016:  stloc.1
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.0
      .line 7,7 : 18,20 ''
      IL_001b:  ldloc.0
      IL_001c:  ldc.i4     0x989681
      IL_0021:  blt.s      IL_0005

      IL_0023:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f5c

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash08

.class private abstract auto ansi sealed '<StartupCode$Hash08>'.$Hash08$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash08>'.$Hash08$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
