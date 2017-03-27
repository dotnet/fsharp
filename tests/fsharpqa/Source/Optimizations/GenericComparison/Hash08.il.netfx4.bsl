
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
.assembly Hash08
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash08
{
  // Offset: 0x00000000 Length: 0x000006D3
}
.mresource public FSharpOptimizationData.Hash08
{
  // Offset: 0x000006D8 Length: 0x000003B3
}
.module Hash08.dll
// MVID: {5772F679-9642-77BC-A745-038379F67257}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011B0000


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
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyR::ToString
      
      .method public hidebysig virtual final 
              instance int32  CompareTo(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       102 (0x66)
        .maxstack  4
        .locals init (int32 V_0,
                 class [mscorlib]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_005c

        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_005a

        IL_000d:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0012:  stloc.1
        IL_0013:  ldarg.0
        IL_0014:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  stloc.2
        IL_001a:  ldarg.1
        IL_001b:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0020:  stloc.3
        IL_0021:  ldloc.2
        IL_0022:  ldloc.3
        IL_0023:  bge.s      IL_0029

        IL_0025:  ldc.i4.m1
        IL_0026:  nop
        IL_0027:  br.s       IL_002e

        IL_0029:  ldloc.2
        IL_002a:  ldloc.3
        IL_002b:  cgt
        IL_002d:  nop
        IL_002e:  stloc.0
        IL_002f:  ldloc.0
        IL_0030:  ldc.i4.0
        IL_0031:  bge.s      IL_0035

        IL_0033:  ldloc.0
        IL_0034:  ret

        IL_0035:  ldloc.0
        IL_0036:  ldc.i4.0
        IL_0037:  ble.s      IL_003b

        IL_0039:  ldloc.0
        IL_003a:  ret

        IL_003b:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0040:  stloc.1
        IL_0041:  ldarg.0
        IL_0042:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0047:  stloc.2
        IL_0048:  ldarg.1
        IL_0049:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_004e:  stloc.3
        IL_004f:  ldloc.2
        IL_0050:  ldloc.3
        IL_0051:  bge.s      IL_0055

        IL_0053:  ldc.i4.m1
        IL_0054:  ret

        IL_0055:  ldloc.2
        IL_0056:  ldloc.3
        IL_0057:  cgt
        IL_0059:  ret

        IL_005a:  ldc.i4.1
        IL_005b:  ret

        IL_005c:  ldarg.1
        IL_005d:  ldnull
        IL_005e:  cgt.un
        IL_0060:  brfalse.s  IL_0064

        IL_0062:  ldc.i4.m1
        IL_0063:  ret

        IL_0064:  ldc.i4.0
        IL_0065:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 4,4 : 10,14 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\GenericComparison\\Hash08.fsx'
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldarg.1
        IL_0003:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0008:  callvirt   instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::CompareTo(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_000d:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       107 (0x6b)
        .maxstack  4
        .locals init ([0] class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0,
                 [1] int32 V_1,
                 [2] int32 V_2,
                 [3] int32 V_3)
        .line 4,4 : 10,14 ''
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0007:  stloc.0
        IL_0008:  ldarg.0
        IL_0009:  ldnull
        IL_000a:  cgt.un
        IL_000c:  brfalse.s  IL_005c

        IL_000e:  ldarg.1
        IL_000f:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0014:  ldnull
        IL_0015:  cgt.un
        IL_0017:  brfalse.s  IL_005a

        IL_0019:  ldarg.0
        IL_001a:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_001f:  stloc.2
        IL_0020:  ldloc.0
        IL_0021:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0026:  stloc.3
        IL_0027:  ldloc.2
        IL_0028:  ldloc.3
        IL_0029:  bge.s      IL_002f

        IL_002b:  ldc.i4.m1
        IL_002c:  nop
        IL_002d:  br.s       IL_0034

        IL_002f:  ldloc.2
        IL_0030:  ldloc.3
        IL_0031:  cgt
        IL_0033:  nop
        IL_0034:  stloc.1
        IL_0035:  ldloc.1
        IL_0036:  ldc.i4.0
        IL_0037:  bge.s      IL_003b

        IL_0039:  ldloc.1
        IL_003a:  ret

        IL_003b:  ldloc.1
        IL_003c:  ldc.i4.0
        IL_003d:  ble.s      IL_0041

        IL_003f:  ldloc.1
        IL_0040:  ret

        IL_0041:  ldarg.0
        IL_0042:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0047:  stloc.2
        IL_0048:  ldloc.0
        IL_0049:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_004e:  stloc.3
        IL_004f:  ldloc.2
        IL_0050:  ldloc.3
        IL_0051:  bge.s      IL_0055

        IL_0053:  ldc.i4.m1
        IL_0054:  ret

        IL_0055:  ldloc.2
        IL_0056:  ldloc.3
        IL_0057:  cgt
        IL_0059:  ret

        IL_005a:  ldc.i4.1
        IL_005b:  ret

        IL_005c:  ldarg.1
        IL_005d:  unbox.any  Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0062:  ldnull
        IL_0063:  cgt.un
        IL_0065:  brfalse.s  IL_0069

        IL_0067:  ldc.i4.m1
        IL_0068:  ret

        IL_0069:  ldc.i4.0
        IL_006a:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       55 (0x37)
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0035

        IL_0007:  ldc.i4.0
        IL_0008:  stloc.0
        IL_0009:  ldc.i4     0x9e3779b9
        IL_000e:  ldarg.0
        IL_000f:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0014:  ldloc.0
        IL_0015:  ldc.i4.6
        IL_0016:  shl
        IL_0017:  ldloc.0
        IL_0018:  ldc.i4.2
        IL_0019:  shr
        IL_001a:  add
        IL_001b:  add
        IL_001c:  add
        IL_001d:  stloc.0
        IL_001e:  ldc.i4     0x9e3779b9
        IL_0023:  ldarg.0
        IL_0024:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0029:  ldloc.0
        IL_002a:  ldc.i4.6
        IL_002b:  shl
        IL_002c:  ldloc.0
        IL_002d:  ldc.i4.2
        IL_002e:  shr
        IL_002f:  add
        IL_0030:  add
        IL_0031:  add
        IL_0032:  stloc.0
        IL_0033:  ldloc.0
        IL_0034:  ret

        IL_0035:  ldc.i4.0
        IL_0036:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        .line 4,4 : 10,14 ''
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0007:  callvirt   instance int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000c:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       58 (0x3a)
        .maxstack  4
        .locals init (class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0032

        IL_0007:  ldarg.1
        IL_0008:  isinst     Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_000d:  stloc.0
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_0030

        IL_0011:  ldarg.0
        IL_0012:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0017:  ldloc.0
        IL_0018:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_001d:  bne.un.s   IL_002e

        IL_001f:  ldarg.0
        IL_0020:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0025:  ldloc.0
        IL_0026:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_002b:  ceq
        IL_002d:  ret

        IL_002e:  ldc.i4.0
        IL_002f:  ret

        IL_0030:  ldc.i4.0
        IL_0031:  ret

        IL_0032:  ldarg.1
        IL_0033:  ldnull
        IL_0034:  cgt.un
        IL_0036:  ldc.i4.0
        IL_0037:  ceq
        IL_0039:  ret
      } // end of method KeyR::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       54 (0x36)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_002e

        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_002c

        IL_000d:  ldarg.0
        IL_000e:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0013:  ldarg.1
        IL_0014:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  bne.un.s   IL_002a

        IL_001b:  ldarg.0
        IL_001c:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0021:  ldarg.1
        IL_0022:  ldfld      int32 Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0027:  ceq
        IL_0029:  ret

        IL_002a:  ldc.i4.0
        IL_002b:  ret

        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldarg.1
        IL_002f:  ldnull
        IL_0030:  cgt.un
        IL_0032:  ldc.i4.0
        IL_0033:  ceq
        IL_0035:  ret
      } // end of method KeyR::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  4
        .locals init (class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  isinst     Hash08/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  brfalse.s  IL_0013

        IL_000b:  ldarg.0
        IL_000c:  ldloc.0
        IL_000d:  callvirt   instance bool Hash08/HashMicroPerfAndCodeGenerationTests/KeyR::Equals(class Hash08/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
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
      .line 7,7 : 8,32 ''
      IL_0000:  nop
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
      .line 7,7 : 8,32 ''
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
