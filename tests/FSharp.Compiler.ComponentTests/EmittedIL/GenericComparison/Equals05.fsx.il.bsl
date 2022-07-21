
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly Equals05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals05
{
  // Offset: 0x00000000 Length: 0x000006F7
  // WARNING: managed resource file FSharpSignatureData.Equals05 created
}
.mresource public FSharpOptimizationData.Equals05
{
  // Offset: 0x00000700 Length: 0x000003C5
  // WARNING: managed resource file FSharpOptimizationData.Equals05 created
}
.module Equals05.exe
// MVID: {628F4C90-28FA-85D1-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001B974410000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals05
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public KeyR
           extends [System.Runtime]System.Object
           implements class [System.Runtime]System.IEquatable`1<class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR>,
                      [System.Runtime]System.Collections.IStructuralEquatable,
                      class [System.Runtime]System.IComparable`1<class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR>,
                      [System.Runtime]System.IComparable,
                      [System.Runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .field assembly int32 key1@
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field assembly int32 key2@
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public hidebysig specialname 
              instance int32  get_key1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0006:  ret
      } // end of method KeyR::get_key1

      .method public hidebysig specialname 
              instance int32  get_key2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0006:  ret
      } // end of method KeyR::get_key2

      .method public specialname rtspecialname 
              instance void  .ctor(int32 key1,
                                   int32 key2) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0014:  ret
      } // end of method KeyR::.ctor

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyR::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       87 (0x57)
        .maxstack  5
        .locals init (int32 V_0,
                 class [System.Runtime]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0050

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_004e

        IL_0006:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  stloc.2
        IL_0013:  ldarg.1
        IL_0014:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  stloc.3
        IL_001a:  ldloc.2
        IL_001b:  ldloc.3
        IL_001c:  cgt
        IL_001e:  ldloc.2
        IL_001f:  ldloc.3
        IL_0020:  clt
        IL_0022:  sub
        IL_0023:  stloc.0
        IL_0024:  ldloc.0
        IL_0025:  ldc.i4.0
        IL_0026:  bge.s      IL_002a

        IL_0028:  ldloc.0
        IL_0029:  ret

        IL_002a:  ldloc.0
        IL_002b:  ldc.i4.0
        IL_002c:  ble.s      IL_0030

        IL_002e:  ldloc.0
        IL_002f:  ret

        IL_0030:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0035:  stloc.1
        IL_0036:  ldarg.0
        IL_0037:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_003c:  stloc.2
        IL_003d:  ldarg.1
        IL_003e:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0043:  stloc.3
        IL_0044:  ldloc.2
        IL_0045:  ldloc.3
        IL_0046:  cgt
        IL_0048:  ldloc.2
        IL_0049:  ldloc.3
        IL_004a:  clt
        IL_004c:  sub
        IL_004d:  ret

        IL_004e:  ldc.i4.1
        IL_004f:  ret

        IL_0050:  ldarg.1
        IL_0051:  brfalse.s  IL_0055

        IL_0053:  ldc.i4.m1
        IL_0054:  ret

        IL_0055:  ldc.i4.0
        IL_0056:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0007:  callvirt   instance int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::CompareTo(class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR)
        IL_000c:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [System.Runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       92 (0x5c)
        .maxstack  5
        .locals init (class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0050

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0010:  brfalse.s  IL_004e

        IL_0012:  ldarg.0
        IL_0013:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0018:  stloc.2
        IL_0019:  ldloc.0
        IL_001a:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_001f:  stloc.3
        IL_0020:  ldloc.2
        IL_0021:  ldloc.3
        IL_0022:  cgt
        IL_0024:  ldloc.2
        IL_0025:  ldloc.3
        IL_0026:  clt
        IL_0028:  sub
        IL_0029:  stloc.1
        IL_002a:  ldloc.1
        IL_002b:  ldc.i4.0
        IL_002c:  bge.s      IL_0030

        IL_002e:  ldloc.1
        IL_002f:  ret

        IL_0030:  ldloc.1
        IL_0031:  ldc.i4.0
        IL_0032:  ble.s      IL_0036

        IL_0034:  ldloc.1
        IL_0035:  ret

        IL_0036:  ldarg.0
        IL_0037:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_003c:  stloc.2
        IL_003d:  ldloc.0
        IL_003e:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0043:  stloc.3
        IL_0044:  ldloc.2
        IL_0045:  ldloc.3
        IL_0046:  cgt
        IL_0048:  ldloc.2
        IL_0049:  ldloc.3
        IL_004a:  clt
        IL_004c:  sub
        IL_004d:  ret

        IL_004e:  ldc.i4.1
        IL_004f:  ret

        IL_0050:  ldarg.1
        IL_0051:  unbox.any  Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0056:  brfalse.s  IL_005a

        IL_0058:  ldc.i4.m1
        IL_0059:  ret

        IL_005a:  ldc.i4.0
        IL_005b:  ret
      } // end of method KeyR::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       51 (0x33)
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0031

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldc.i4     0x9e3779b9
        IL_000a:  ldarg.0
        IL_000b:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
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
        IL_0020:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
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

        IL_0031:  ldc.i4.0
        IL_0032:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method KeyR::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       54 (0x36)
        .maxstack  4
        .locals init (class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002e

        IL_0003:  ldarg.1
        IL_0004:  isinst     Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0009:  stloc.0
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_002c

        IL_000d:  ldarg.0
        IL_000e:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0013:  ldloc.0
        IL_0014:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0019:  bne.un.s   IL_002a

        IL_001b:  ldarg.0
        IL_001c:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0021:  ldloc.0
        IL_0022:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
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
              instance bool  Equals(class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       47 (0x2f)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0025

        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000c:  ldarg.1
        IL_000d:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  bne.un.s   IL_0023

        IL_0014:  ldarg.0
        IL_0015:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_001a:  ldarg.1
        IL_001b:  ldfld      int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0020:  ceq
        IL_0022:  ret

        IL_0023:  ldc.i4.0
        IL_0024:  ret

        IL_0025:  ldc.i4.0
        IL_0026:  ret

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
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init (class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::Equals(class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method KeyR::Equals

      .property instance int32 key1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::get_key1()
      } // end of property KeyR::key1
      .property instance int32 key2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
        .get instance int32 Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::get_key2()
      } // end of property KeyR::key2
    } // end of class KeyR

    .method public static void  f5c() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  5
      .locals init (bool V_0,
               class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR V_1,
               class Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.2
      IL_0004:  newobj     instance void Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::.ctor(int32,
                                                                                                    int32)
      IL_0009:  stloc.1
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4.3
      IL_000c:  newobj     instance void Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::.ctor(int32,
                                                                                                    int32)
      IL_0011:  stloc.2
      IL_0012:  ldc.i4.0
      IL_0013:  stloc.3
      IL_0014:  br.s       IL_0027

      IL_0016:  ldloc.1
      IL_0017:  ldloc.2
      IL_0018:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_001d:  callvirt   instance bool Equals05/EqualsMicroPerfAndCodeGenerationTests/KeyR::Equals(object,
                                                                                                     class [System.Runtime]System.Collections.IEqualityComparer)
      IL_0022:  stloc.0
      IL_0023:  ldloc.3
      IL_0024:  ldc.i4.1
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.3
      IL_0028:  ldc.i4     0x989681
      IL_002d:  blt.s      IL_0016

      IL_002f:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f5c

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals05

.class private abstract auto ansi sealed '<StartupCode$Equals05>'.$Equals05$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Equals05$fsx::main@

} // end of class '<StartupCode$Equals05>'.$Equals05$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Equals05_fsx\Equals05.res
