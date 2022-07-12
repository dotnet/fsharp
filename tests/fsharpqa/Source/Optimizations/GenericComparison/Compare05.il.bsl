
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
.assembly Compare05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare05
{
  // Offset: 0x00000000 Length: 0x000006DC
}
.mresource public FSharpOptimizationData.Compare05
{
  // Offset: 0x000006E0 Length: 0x000003BA
}
.module Compare05.dll
// MVID: {6220E4FA-051C-F88E-A745-0383FAE42062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06E50000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [mscorlib]System.Object
           implements class [mscorlib]System.IEquatable`1<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key>,
                      [mscorlib]System.Collections.IStructuralEquatable,
                      class [mscorlib]System.IComparable`1<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key>,
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
      .method public static class Compare05/CompareMicroPerfAndCodeGenerationTests/Key 
              NewKey(int32 item1,
                     int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void Compare05/CompareMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
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
        IL_0008:  stfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0001:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0001:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::__DebugDisplay

      .method public strict virtual instance string 
              ToString() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       22 (0x16)
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Compare05/CompareMicroPerfAndCodeGenerationTests/Key>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Compare05/CompareMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } // end of method Key::ToString

      .method public hidebysig virtual final 
              instance int32  CompareTo(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       110 (0x6e)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] int32 V_2,
                 [3] class [mscorlib]System.Collections.IComparer V_3,
                 [4] int32 V_4,
                 [5] int32 V_5)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 16707566,16707566 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Compare05.fsx'
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0067

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0065

        .line 16707566,16707566 : 0,0 ''
        IL_0006:  ldarg.0
        IL_0007:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.3
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0020:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_0022:  ldloc.s    V_4
        IL_0024:  ldloc.s    V_5
        IL_0026:  bge.s      IL_002c

        .line 16707566,16707566 : 0,0 ''
        IL_0028:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_0029:  nop
        IL_002a:  br.s       IL_0033

        .line 16707566,16707566 : 0,0 ''
        IL_002c:  ldloc.s    V_4
        IL_002e:  ldloc.s    V_5
        IL_0030:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_0032:  nop
        .line 16707566,16707566 : 0,0 ''
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
        IL_0047:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_004c:  stloc.s    V_4
        IL_004e:  ldloc.1
        IL_004f:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0054:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_0056:  ldloc.s    V_4
        IL_0058:  ldloc.s    V_5
        IL_005a:  bge.s      IL_005e

        .line 16707566,16707566 : 0,0 ''
        IL_005c:  ldc.i4.m1
        IL_005d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_005e:  ldloc.s    V_4
        IL_0060:  ldloc.s    V_5
        IL_0062:  cgt
        IL_0064:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0065:  ldc.i4.1
        IL_0066:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0067:  ldarg.1
        IL_0068:  brfalse.s  IL_006c

        .line 16707566,16707566 : 0,0 ''
        IL_006a:  ldc.i4.m1
        IL_006b:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006c:  ldc.i4.0
        IL_006d:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0007:  callvirt   instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key)
        IL_000c:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [mscorlib]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       115 (0x73)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_2,
                 [3] int32 V_3,
                 [4] int32 V_4,
                 [5] int32 V_5)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0067

        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldarg.1
        IL_000b:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0010:  brfalse.s  IL_0065

        .line 16707566,16707566 : 0,0 ''
        IL_0012:  ldarg.0
        IL_0013:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0026:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  bge.s      IL_0032

        .line 16707566,16707566 : 0,0 ''
        IL_002e:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_002f:  nop
        IL_0030:  br.s       IL_0039

        .line 16707566,16707566 : 0,0 ''
        IL_0032:  ldloc.s    V_4
        IL_0034:  ldloc.s    V_5
        IL_0036:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_0038:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0039:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_003a:  ldloc.3
        IL_003b:  ldc.i4.0
        IL_003c:  bge.s      IL_0040

        .line 16707566,16707566 : 0,0 ''
        IL_003e:  ldloc.3
        IL_003f:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0040:  ldloc.3
        IL_0041:  ldc.i4.0
        IL_0042:  ble.s      IL_0046

        .line 16707566,16707566 : 0,0 ''
        IL_0044:  ldloc.3
        IL_0045:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0046:  ldloc.1
        IL_0047:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_004c:  stloc.s    V_4
        IL_004e:  ldloc.2
        IL_004f:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0054:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_0056:  ldloc.s    V_4
        IL_0058:  ldloc.s    V_5
        IL_005a:  bge.s      IL_005e

        .line 16707566,16707566 : 0,0 ''
        IL_005c:  ldc.i4.m1
        IL_005d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_005e:  ldloc.s    V_4
        IL_0060:  ldloc.s    V_5
        IL_0062:  cgt
        IL_0064:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0065:  ldc.i4.1
        IL_0066:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0067:  ldarg.1
        IL_0068:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_006d:  brfalse.s  IL_0071

        .line 16707566,16707566 : 0,0 ''
        IL_006f:  ldc.i4.m1
        IL_0070:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0071:  ldc.i4.0
        IL_0072:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       57 (0x39)
        .maxstack  7
        .locals init ([0] int32 V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0005:  ldarg.0
        IL_0006:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0026:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
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

        .line 16707566,16707566 : 0,0 ''
        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       60 (0x3c)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_2)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0034

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  isinst     Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0009:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldloc.0
        IL_000b:  brfalse.s  IL_0032

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.0
        IL_000e:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_000f:  ldarg.0
        IL_0010:  stloc.1
        IL_0011:  ldloc.0
        IL_0012:  stloc.2
        .line 16707566,16707566 : 0,0 ''
        IL_0013:  ldloc.1
        IL_0014:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0019:  ldloc.2
        IL_001a:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  bne.un.s   IL_0030

        .line 16707566,16707566 : 0,0 ''
        IL_0021:  ldloc.1
        IL_0022:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0027:  ldloc.2
        IL_0028:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_002d:  ceq
        IL_002f:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0030:  ldc.i4.0
        IL_0031:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0032:  ldc.i4.0
        IL_0033:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0034:  ldarg.1
        IL_0035:  ldnull
        IL_0036:  cgt.un
        IL_0038:  ldc.i4.0
        IL_0039:  ceq
        IL_003b:  ret
      } // end of method Key::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       53 (0x35)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002d

        .line 16707566,16707566 : 0,0 ''
        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002b

        .line 16707566,16707566 : 0,0 ''
        IL_0006:  ldarg.0
        IL_0007:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        .line 16707566,16707566 : 0,0 ''
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        .line 16707566,16707566 : 0,0 ''
        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0026:  ceq
        IL_0028:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0029:  ldc.i4.0
        IL_002a:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_002b:  ldc.i4.0
        IL_002c:  ret

        .line 16707566,16707566 : 0,0 ''
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
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0)
        .line 16707566,16707566 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  isinst     Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        .line 16707566,16707566 : 0,0 ''
        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool Compare05/CompareMicroPerfAndCodeGenerationTests/Key::Equals(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key)
        IL_0011:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } // end of method Key::Equals

      .property instance int32 Tag()
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } // end of property Key::Tag
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } // end of property Key::Item1
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } // end of property Key::Item2
    } // end of class Key

    .method public static int32  f5() cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  4
      .locals init ([0] int32 x,
               [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key t1,
               [2] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key t2,
               [3] int32 i)
      .line 6,6 : 8,25 ''
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      .line 7,7 : 8,25 ''
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.2
      IL_0004:  call       class Compare05/CompareMicroPerfAndCodeGenerationTests/Key Compare05/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0009:  stloc.1
      .line 8,8 : 8,25 ''
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4.3
      IL_000c:  call       class Compare05/CompareMicroPerfAndCodeGenerationTests/Key Compare05/CompareMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                                   int32)
      IL_0011:  stloc.2
      .line 9,9 : 8,11 ''
      IL_0012:  ldc.i4.0
      IL_0013:  stloc.3
      IL_0014:  br.s       IL_0022

      .line 10,10 : 12,30 ''
      IL_0016:  ldloc.1
      IL_0017:  ldloc.2
      IL_0018:  callvirt   instance int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::CompareTo(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key)
      IL_001d:  stloc.0
      IL_001e:  ldloc.3
      IL_001f:  ldc.i4.1
      IL_0020:  add
      IL_0021:  stloc.3
      .line 9,9 : 18,20 ''
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4     0x989681
      IL_0028:  blt.s      IL_0016

      .line 11,11 : 8,9 ''
      IL_002a:  ldloc.0
      IL_002b:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f5

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare05

.class private abstract auto ansi sealed '<StartupCode$Compare05>'.$Compare05$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare05>'.$Compare05$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
