
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
.assembly Compare10
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare10
{
  // Offset: 0x00000000 Length: 0x00000AC1
  // WARNING: managed resource file FSharpSignatureData.Compare10 created
}
.mresource public FSharpOptimizationData.Compare10
{
  // Offset: 0x00000AC8 Length: 0x0000059A
  // WARNING: managed resource file FSharpOptimizationData.Compare10 created
}
.module Compare10.exe
// MVID: {628F4C90-2093-2BBE-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000206CE490000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare10
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [System.Runtime]System.Object
           implements class [System.Runtime]System.IEquatable`1<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>,
                      [System.Runtime]System.Collections.IStructuralEquatable,
                      class [System.Runtime]System.IComparable`1<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>,
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
      .method public static class Compare10/CompareMicroPerfAndCodeGenerationTests/Key 
              NewKey(int32 item1,
                     int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Compare10/CompareMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
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
        IL_0008:  stfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0001:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0001:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       105 (0x69)
        .maxstack  5
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_1,
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
        IL_0013:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0043:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.1
        IL_004b:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0002:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0007:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key)
        IL_000c:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [System.Runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       110 (0x6e)
        .maxstack  5
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0062

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0010:  brfalse.s  IL_0060

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0043:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.2
        IL_004b:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0063:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/Key
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
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_1)
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
        IL_0011:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0026:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0006:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       60 (0x3c)
        .maxstack  4
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_2)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0034

        IL_0003:  ldarg.1
        IL_0004:  isinst     Compare10/CompareMicroPerfAndCodeGenerationTests/Key
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
        IL_0014:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0019:  ldloc.2
        IL_001a:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  bne.un.s   IL_0030

        IL_0021:  ldloc.1
        IL_0022:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0027:  ldloc.2
        IL_0028:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
              instance bool  Equals(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       53 (0x35)
        .maxstack  4
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_1)
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
        IL_000d:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     Compare10/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/Key::Equals(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method Key::Equals

      .property instance int32 Tag()
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } // end of property Key::Tag
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } // end of property Key::Item1
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } // end of property Key::Item2
    } // end of class Key

    .class auto autochar serializable sealed nested public beforefieldinit KeyWithInnerKeys
           extends [System.Runtime]System.Object
           implements class [System.Runtime]System.IEquatable`1<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [System.Runtime]System.Collections.IStructuralEquatable,
                      class [System.Runtime]System.IComparable`1<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [System.Runtime]System.IComparable,
                      [System.Runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly class Compare10/CompareMicroPerfAndCodeGenerationTests/Key item1
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> item2
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys 
              NewKeyWithInnerKeys(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key item1,
                                  class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::.ctor(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,
                                                                                                                    class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>)
        IL_0007:  ret
      } // end of method KeyWithInnerKeys::NewKeyWithInnerKeys

      .method assembly specialname rtspecialname 
              instance void  .ctor(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key item1,
                                   class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0014:  ret
      } // end of method KeyWithInnerKeys::.ctor

      .method public hidebysig instance class Compare10/CompareMicroPerfAndCodeGenerationTests/Key 
              get_Item1() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0006:  ret
      } // end of method KeyWithInnerKeys::get_Item1

      .method public hidebysig instance class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> 
              get_Item2() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0006:  ret
      } // end of method KeyWithInnerKeys::get_Item2

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
      } // end of method KeyWithInnerKeys::get_Tag

      .method assembly hidebysig specialname 
              instance object  __DebugDisplay() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyWithInnerKeys::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method KeyWithInnerKeys::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       160 (0xa0)
        .maxstack  5
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 int32 V_2,
                 class [System.Runtime]System.Collections.IComparer V_3,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_4,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_5,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_6,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_7,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_8,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_9,
                 int32 V_10)
        IL_0000:  ldarg.0
        IL_0001:  brfalse    IL_0099

        IL_0006:  ldarg.1
        IL_0007:  brfalse    IL_0097

        IL_000c:  ldarg.0
        IL_000d:  pop
        IL_000e:  ldarg.0
        IL_000f:  stloc.0
        IL_0010:  ldarg.1
        IL_0011:  stloc.1
        IL_0012:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0017:  stloc.3
        IL_0018:  ldloc.0
        IL_0019:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.1
        IL_0021:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  ldloc.3
        IL_002d:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0032:  stloc.2
        IL_0033:  ldloc.2
        IL_0034:  ldc.i4.0
        IL_0035:  bge.s      IL_0039

        IL_0037:  ldloc.2
        IL_0038:  ret

        IL_0039:  ldloc.2
        IL_003a:  ldc.i4.0
        IL_003b:  ble.s      IL_003f

        IL_003d:  ldloc.2
        IL_003e:  ret

        IL_003f:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0044:  stloc.3
        IL_0045:  ldloc.0
        IL_0046:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_004b:  stloc.s    V_6
        IL_004d:  ldloc.1
        IL_004e:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0053:  stloc.s    V_7
        IL_0055:  ldloc.s    V_6
        IL_0057:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_005c:  stloc.s    V_4
        IL_005e:  ldloc.s    V_6
        IL_0060:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0065:  stloc.s    V_5
        IL_0067:  ldloc.s    V_7
        IL_0069:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_006e:  stloc.s    V_8
        IL_0070:  ldloc.s    V_7
        IL_0072:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0077:  stloc.s    V_9
        IL_0079:  ldloc.s    V_4
        IL_007b:  ldloc.s    V_8
        IL_007d:  ldloc.3
        IL_007e:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0083:  stloc.s    V_10
        IL_0085:  ldloc.s    V_10
        IL_0087:  brfalse.s  IL_008c

        IL_0089:  ldloc.s    V_10
        IL_008b:  ret

        IL_008c:  ldloc.s    V_5
        IL_008e:  ldloc.s    V_9
        IL_0090:  ldloc.3
        IL_0091:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0096:  ret

        IL_0097:  ldc.i4.1
        IL_0098:  ret

        IL_0099:  ldarg.1
        IL_009a:  brfalse.s  IL_009e

        IL_009c:  ldc.i4.m1
        IL_009d:  ret

        IL_009e:  ldc.i4.0
        IL_009f:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0007:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::CompareTo(class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_000c:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [System.Runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       165 (0xa5)
        .maxstack  5
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
                 int32 V_3,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_4,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_5,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_6,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_7,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_8,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_9,
                 int32 V_10)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse    IL_0099

        IL_000d:  ldarg.1
        IL_000e:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0013:  brfalse    IL_0097

        IL_0018:  ldarg.0
        IL_0019:  pop
        IL_001a:  ldarg.0
        IL_001b:  stloc.1
        IL_001c:  ldloc.0
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.2
        IL_0027:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_002c:  stloc.s    V_5
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  ldarg.2
        IL_0033:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0038:  stloc.3
        IL_0039:  ldloc.3
        IL_003a:  ldc.i4.0
        IL_003b:  bge.s      IL_003f

        IL_003d:  ldloc.3
        IL_003e:  ret

        IL_003f:  ldloc.3
        IL_0040:  ldc.i4.0
        IL_0041:  ble.s      IL_0045

        IL_0043:  ldloc.3
        IL_0044:  ret

        IL_0045:  ldloc.1
        IL_0046:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_004b:  stloc.s    V_6
        IL_004d:  ldloc.2
        IL_004e:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0053:  stloc.s    V_7
        IL_0055:  ldloc.s    V_6
        IL_0057:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_005c:  stloc.s    V_4
        IL_005e:  ldloc.s    V_6
        IL_0060:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0065:  stloc.s    V_5
        IL_0067:  ldloc.s    V_7
        IL_0069:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_006e:  stloc.s    V_8
        IL_0070:  ldloc.s    V_7
        IL_0072:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0077:  stloc.s    V_9
        IL_0079:  ldloc.s    V_4
        IL_007b:  ldloc.s    V_8
        IL_007d:  ldarg.2
        IL_007e:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0083:  stloc.s    V_10
        IL_0085:  ldloc.s    V_10
        IL_0087:  brfalse.s  IL_008c

        IL_0089:  ldloc.s    V_10
        IL_008b:  ret

        IL_008c:  ldloc.s    V_5
        IL_008e:  ldloc.s    V_9
        IL_0090:  ldarg.2
        IL_0091:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                            class [System.Runtime]System.Collections.IComparer)
        IL_0096:  ret

        IL_0097:  ldc.i4.1
        IL_0098:  ret

        IL_0099:  ldarg.1
        IL_009a:  unbox.any  Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_009f:  brfalse.s  IL_00a3

        IL_00a1:  ldc.i4.m1
        IL_00a2:  ret

        IL_00a3:  ldc.i4.0
        IL_00a4:  ret
      } // end of method KeyWithInnerKeys::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       104 (0x68)
        .maxstack  7
        .locals init (int32 V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_2,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_3,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0066

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
        IL_0011:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0016:  stloc.2
        IL_0017:  ldloc.2
        IL_0018:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_001d:  stloc.3
        IL_001e:  ldloc.2
        IL_001f:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.3
        IL_0027:  ldarg.1
        IL_0028:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_002d:  stloc.s    V_5
        IL_002f:  ldloc.s    V_5
        IL_0031:  ldc.i4.5
        IL_0032:  shl
        IL_0033:  ldloc.s    V_5
        IL_0035:  add
        IL_0036:  ldloc.s    V_4
        IL_0038:  ldarg.1
        IL_0039:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_003e:  xor
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
        IL_0049:  ldc.i4     0x9e3779b9
        IL_004e:  ldloc.1
        IL_004f:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0054:  ldarg.1
        IL_0055:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_005a:  ldloc.0
        IL_005b:  ldc.i4.6
        IL_005c:  shl
        IL_005d:  ldloc.0
        IL_005e:  ldc.i4.2
        IL_005f:  shr
        IL_0060:  add
        IL_0061:  add
        IL_0062:  add
        IL_0063:  stloc.0
        IL_0064:  ldloc.0
        IL_0065:  ret

        IL_0066:  ldc.i4.0
        IL_0067:  ret
      } // end of method KeyWithInnerKeys::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method KeyWithInnerKeys::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       135 (0x87)
        .maxstack  5
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_3,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_4,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_5,
                 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> V_6,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_7,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/Key V_8)
        IL_0000:  ldarg.0
        IL_0001:  brfalse    IL_007f

        IL_0006:  ldarg.1
        IL_0007:  isinst     Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_000c:  stloc.0
        IL_000d:  ldloc.0
        IL_000e:  brfalse.s  IL_007d

        IL_0010:  ldarg.0
        IL_0011:  pop
        IL_0012:  ldarg.0
        IL_0013:  stloc.1
        IL_0014:  ldloc.0
        IL_0015:  stloc.2
        IL_0016:  ldloc.1
        IL_0017:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_001c:  stloc.3
        IL_001d:  ldloc.2
        IL_001e:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0023:  stloc.s    V_4
        IL_0025:  ldloc.3
        IL_0026:  ldloc.s    V_4
        IL_0028:  ldarg.2
        IL_0029:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                        class [System.Runtime]System.Collections.IEqualityComparer)
        IL_002e:  brfalse.s  IL_007b

        IL_0030:  ldloc.1
        IL_0031:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0036:  stloc.s    V_5
        IL_0038:  ldloc.2
        IL_0039:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_003e:  stloc.s    V_6
        IL_0040:  ldloc.s    V_5
        IL_0042:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0047:  stloc.3
        IL_0048:  ldloc.s    V_5
        IL_004a:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_004f:  stloc.s    V_4
        IL_0051:  ldloc.s    V_6
        IL_0053:  call       instance !0 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_0058:  stloc.s    V_7
        IL_005a:  ldloc.s    V_6
        IL_005c:  call       instance !1 class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0061:  stloc.s    V_8
        IL_0063:  ldloc.3
        IL_0064:  ldloc.s    V_7
        IL_0066:  ldarg.2
        IL_0067:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                        class [System.Runtime]System.Collections.IEqualityComparer)
        IL_006c:  brfalse.s  IL_0079

        IL_006e:  ldloc.s    V_4
        IL_0070:  ldloc.s    V_8
        IL_0072:  ldarg.2
        IL_0073:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/Key::Equals(object,
                                                                                                        class [System.Runtime]System.Collections.IEqualityComparer)
        IL_0078:  ret

        IL_0079:  ldc.i4.0
        IL_007a:  ret

        IL_007b:  ldc.i4.0
        IL_007c:  ret

        IL_007d:  ldc.i4.0
        IL_007e:  ret

        IL_007f:  ldarg.1
        IL_0080:  ldnull
        IL_0081:  cgt.un
        IL_0083:  ldc.i4.0
        IL_0084:  ceq
        IL_0086:  ret
      } // end of method KeyWithInnerKeys::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       63 (0x3f)
        .maxstack  4
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0035

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0018:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/Key::Equals(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key)
        IL_001d:  brfalse.s  IL_0033

        IL_001f:  ldloc.0
        IL_0020:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0025:  ldloc.1
        IL_0026:  ldfld      class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_002b:  tail.
        IL_002d:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>>(!!0,
                                                                                                                                                                                                                                                                                             !!0)
        IL_0032:  ret

        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldc.i4.0
        IL_0036:  ret

        IL_0037:  ldarg.1
        IL_0038:  ldnull
        IL_0039:  cgt.un
        IL_003b:  ldc.i4.0
        IL_003c:  ceq
        IL_003e:  ret
      } // end of method KeyWithInnerKeys::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  4
        .locals init (class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0014

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  tail.
        IL_000e:  callvirt   instance bool Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::Equals(class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_0013:  ret

        IL_0014:  ldc.i4.0
        IL_0015:  ret
      } // end of method KeyWithInnerKeys::Equals

      .property instance int32 Tag()
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Tag()
      } // end of property KeyWithInnerKeys::Tag
      .property instance class Compare10/CompareMicroPerfAndCodeGenerationTests/Key
              Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item1()
      } // end of property KeyWithInnerKeys::Item1
      .property instance class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>
              Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key> Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item2()
      } // end of property KeyWithInnerKeys::Item2
    } // end of class KeyWithInnerKeys

    .method public static void  f9() cil managed
    {
      // Code size       91 (0x5b)
      .maxstack  6
      .locals init (int32 V_0,
               class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
               class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
               int32 V_3)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.2
      IL_0004:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0009:  ldc.i4.1
      IL_000a:  ldc.i4.2
      IL_000b:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0010:  ldc.i4.1
      IL_0011:  ldc.i4.2
      IL_0012:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0017:  newobj     instance void class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::.ctor(!0,
                                                                                                                                                                                                            !1)
      IL_001c:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::NewKeyWithInnerKeys(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,
                                                                                                                                                                                          class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>)
      IL_0021:  stloc.1
      IL_0022:  ldc.i4.1
      IL_0023:  ldc.i4.2
      IL_0024:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0029:  ldc.i4.1
      IL_002a:  ldc.i4.2
      IL_002b:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0030:  ldc.i4.1
      IL_0031:  ldc.i4.3
      IL_0032:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/Key Compare10/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0037:  newobj     instance void class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>::.ctor(!0,
                                                                                                                                                                                                            !1)
      IL_003c:  call       class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::NewKeyWithInnerKeys(class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,
                                                                                                                                                                                          class [System.Runtime]System.Tuple`2<class Compare10/CompareMicroPerfAndCodeGenerationTests/Key,class Compare10/CompareMicroPerfAndCodeGenerationTests/Key>)
      IL_0041:  stloc.2
      IL_0042:  ldc.i4.0
      IL_0043:  stloc.3
      IL_0044:  br.s       IL_0052

      IL_0046:  ldloc.1
      IL_0047:  ldloc.2
      IL_0048:  callvirt   instance int32 Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::CompareTo(class Compare10/CompareMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
      IL_004d:  stloc.0
      IL_004e:  ldloc.3
      IL_004f:  ldc.i4.1
      IL_0050:  add
      IL_0051:  stloc.3
      IL_0052:  ldloc.3
      IL_0053:  ldc.i4     0x989681
      IL_0058:  blt.s      IL_0046

      IL_005a:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f9

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare10

.class private abstract auto ansi sealed '<StartupCode$Compare10>'.$Compare10$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare10$fsx::main@

} // end of class '<StartupCode$Compare10>'.$Compare10$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Compare10_fsx\Compare10.res
