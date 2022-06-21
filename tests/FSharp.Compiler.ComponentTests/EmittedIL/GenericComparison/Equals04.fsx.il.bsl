
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
.assembly Equals04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals04
{
  // Offset: 0x00000000 Length: 0x00000704
  // WARNING: managed resource file FSharpSignatureData.Equals04 created
}
.mresource public FSharpOptimizationData.Equals04
{
  // Offset: 0x00000708 Length: 0x000003C3
  // WARNING: managed resource file FSharpOptimizationData.Equals04 created
}
.module Equals04.exe
// MVID: {628F4C90-417C-5CA8-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002876A530000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals04
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [System.Runtime]System.Object
           implements class [System.Runtime]System.IEquatable`1<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key>,
                      [System.Runtime]System.Collections.IStructuralEquatable,
                      class [System.Runtime]System.IComparable`1<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key>,
                      [System.Runtime]System.IComparable,
                      [System.Runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly int32 item1
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly int32 item2
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key 
              NewKey(int32 item1,
                     int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
                                                                                                     int32)
        IL_0007:  ret
      } // end of method Key::NewKey

      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item1,
                                   int32 item2) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0014:  ret
      } // end of method Key::.ctor

      .method public hidebysig instance int32 
              get_Item1() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0006:  ret
      } // end of method Key::get_Item1

      .method public hidebysig instance int32 
              get_Item2() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0006:  ret
      } // end of method Key::get_Item2

      .method public hidebysig instance int32 
              get_Tag() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       105 (0x69)
        .maxstack  5
        .locals init (class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_0,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1,
                 int32 V_2,
                 class [System.Runtime]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0062

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0060

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.3
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0020:  stloc.s    V_5
        IL_0022:  ldloc.s    V_4
        IL_0024:  ldloc.s    V_5
        IL_0026:  cgt
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  clt
        IL_002e:  sub
        IL_002f:  stloc.2
        IL_0030:  ldloc.2
        IL_0031:  ldc.i4.0
        IL_0032:  bge.s      IL_0036

        IL_0034:  ldloc.2
        IL_0035:  ret

        IL_0036:  ldloc.2
        IL_0037:  ldc.i4.0
        IL_0038:  ble.s      IL_003c

        IL_003a:  ldloc.2
        IL_003b:  ret

        IL_003c:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0041:  stloc.3
        IL_0042:  ldloc.0
        IL_0043:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.1
        IL_004b:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  brfalse.s  IL_0067

        IL_0065:  ldc.i4.m1
        IL_0066:  ret

        IL_0067:  ldc.i4.0
        IL_0068:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0007:  callvirt   instance int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::CompareTo(class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key)
        IL_000c:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [System.Runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       110 (0x6e)
        .maxstack  5
        .locals init (class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_0,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0062

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0010:  brfalse.s  IL_0060

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  cgt
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  clt
        IL_0034:  sub
        IL_0035:  stloc.3
        IL_0036:  ldloc.3
        IL_0037:  ldc.i4.0
        IL_0038:  bge.s      IL_003c

        IL_003a:  ldloc.3
        IL_003b:  ret

        IL_003c:  ldloc.3
        IL_003d:  ldc.i4.0
        IL_003e:  ble.s      IL_0042

        IL_0040:  ldloc.3
        IL_0041:  ret

        IL_0042:  ldloc.1
        IL_0043:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.2
        IL_004b:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  unbox.any  Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0068:  brfalse.s  IL_006c

        IL_006a:  ldc.i4.m1
        IL_006b:  ret

        IL_006c:  ldc.i4.0
        IL_006d:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       57 (0x39)
        .maxstack  7
        .locals init (int32 V_0,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0016:  ldloc.0
        IL_0017:  ldc.i4.6
        IL_0018:  shl
        IL_0019:  ldloc.0
        IL_001a:  ldc.i4.2
        IL_001b:  shr
        IL_001c:  add
        IL_001d:  add
        IL_001e:  add
        IL_001f:  stloc.0
        IL_0020:  ldc.i4     0x9e3779b9
        IL_0025:  ldloc.1
        IL_0026:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_002b:  ldloc.0
        IL_002c:  ldc.i4.6
        IL_002d:  shl
        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.2
        IL_0030:  shr
        IL_0031:  add
        IL_0032:  add
        IL_0033:  add
        IL_0034:  stloc.0
        IL_0035:  ldloc.0
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       60 (0x3c)
        .maxstack  4
        .locals init (class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_0,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_2)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0034

        IL_0003:  ldarg.1
        IL_0004:  isinst     Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0009:  stloc.0
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_0032

        IL_000d:  ldarg.0
        IL_000e:  pop
        IL_000f:  ldarg.0
        IL_0010:  stloc.1
        IL_0011:  ldloc.0
        IL_0012:  stloc.2
        IL_0013:  ldloc.1
        IL_0014:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0019:  ldloc.2
        IL_001a:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  bne.un.s   IL_0030

        IL_0021:  ldloc.1
        IL_0022:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0027:  ldloc.2
        IL_0028:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
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
              instance bool  Equals(class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       53 (0x35)
        .maxstack  4
        .locals init (class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_0,
                 class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002d

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002b

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::item2
        IL_0026:  ceq
        IL_0028:  ret

        IL_0029:  ldc.i4.0
        IL_002a:  ret

        IL_002b:  ldc.i4.0
        IL_002c:  ret

        IL_002d:  ldarg.1
        IL_002e:  ldnull
        IL_002f:  cgt.un
        IL_0031:  ldc.i4.0
        IL_0032:  ceq
        IL_0034:  ret
      } // end of method Key::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init (class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     Equals04/EqualsMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::Equals(class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method Key::Equals

      .property instance int32 Tag()
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } // end of property Key::Tag
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } // end of property Key::Item1
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } // end of property Key::Item2
    } // end of class Key

    .method public static bool  f5() cil managed
    {
      // Code size       49 (0x31)
      .maxstack  5
      .locals init (bool V_0,
               class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_1,
               class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.2
      IL_0004:  call       class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                               int32)
      IL_0009:  stloc.1
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4.3
      IL_000c:  call       class Equals04/EqualsMicroPerfAndCodeGenerationTests/Key Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                               int32)
      IL_0011:  stloc.2
      IL_0012:  ldc.i4.0
      IL_0013:  stloc.3
      IL_0014:  br.s       IL_0027

      IL_0016:  ldloc.1
      IL_0017:  ldloc.2
      IL_0018:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_001d:  callvirt   instance bool Equals04/EqualsMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                    class [System.Runtime]System.Collections.IEqualityComparer)
      IL_0022:  stloc.0
      IL_0023:  ldloc.3
      IL_0024:  ldc.i4.1
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.3
      IL_0028:  ldc.i4     0x989681
      IL_002d:  blt.s      IL_0016

      IL_002f:  ldloc.0
      IL_0030:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f5

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals04

.class private abstract auto ansi sealed '<StartupCode$Equals04>'.$Equals04$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Equals04$fsx::main@

} // end of class '<StartupCode$Equals04>'.$Equals04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Equals04_fsx\Equals04.res
