
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
// MVID: {611C550D-051C-F88E-A745-03830D551C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x070E0000


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
        // Code size       120 (0x78)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] int32 V_2,
                 [3] class [mscorlib]System.Collections.IComparer V_3,
                 [4] int32 V_4,
                 [5] int32 V_5)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 4,4 : 10,13 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Compare05.fsx'
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_006e

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_006c

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
        IL_001a:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  stloc.s    V_4
        IL_0021:  ldloc.1
        IL_0022:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0027:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_0029:  ldloc.s    V_4
        IL_002b:  ldloc.s    V_5
        IL_002d:  bge.s      IL_0033

        .line 16707566,16707566 : 0,0 ''
        IL_002f:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_0030:  nop
        IL_0031:  br.s       IL_003a

        .line 16707566,16707566 : 0,0 ''
        IL_0033:  ldloc.s    V_4
        IL_0035:  ldloc.s    V_5
        IL_0037:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_0039:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_003a:  stloc.2
        .line 16707566,16707566 : 0,0 ''
        IL_003b:  ldloc.2
        IL_003c:  ldc.i4.0
        IL_003d:  bge.s      IL_0041

        .line 16707566,16707566 : 0,0 ''
        IL_003f:  ldloc.2
        IL_0040:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0041:  ldloc.2
        IL_0042:  ldc.i4.0
        IL_0043:  ble.s      IL_0047

        .line 16707566,16707566 : 0,0 ''
        IL_0045:  ldloc.2
        IL_0046:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0047:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_004c:  stloc.3
        IL_004d:  ldloc.0
        IL_004e:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0053:  stloc.s    V_4
        IL_0055:  ldloc.1
        IL_0056:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_005b:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_005d:  ldloc.s    V_4
        IL_005f:  ldloc.s    V_5
        IL_0061:  bge.s      IL_0065

        .line 16707566,16707566 : 0,0 ''
        IL_0063:  ldc.i4.m1
        IL_0064:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0065:  ldloc.s    V_4
        IL_0067:  ldloc.s    V_5
        IL_0069:  cgt
        IL_006b:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006c:  ldc.i4.1
        IL_006d:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006e:  ldarg.1
        IL_006f:  ldnull
        IL_0070:  cgt.un
        IL_0072:  brfalse.s  IL_0076

        .line 16707566,16707566 : 0,0 ''
        IL_0074:  ldc.i4.m1
        IL_0075:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0076:  ldc.i4.0
        IL_0077:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       13 (0xd)
        .maxstack  8
        .line 4,4 : 10,13 ''
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
        // Code size       124 (0x7c)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_2,
                 [3] int32 V_3,
                 [4] int32 V_4,
                 [5] int32 V_5)
        .line 4,4 : 10,13 ''
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.0
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_006d

        .line 16707566,16707566 : 0,0 ''
        IL_000d:  ldarg.1
        IL_000e:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0013:  ldnull
        IL_0014:  cgt.un
        IL_0016:  brfalse.s  IL_006b

        .line 16707566,16707566 : 0,0 ''
        IL_0018:  ldarg.0
        IL_0019:  pop
        .line 16707566,16707566 : 0,0 ''
        IL_001a:  ldarg.0
        IL_001b:  stloc.1
        IL_001c:  ldloc.0
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.2
        IL_0027:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_002c:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  bge.s      IL_0038

        .line 16707566,16707566 : 0,0 ''
        IL_0034:  ldc.i4.m1
        .line 16707566,16707566 : 0,0 ''
        IL_0035:  nop
        IL_0036:  br.s       IL_003f

        .line 16707566,16707566 : 0,0 ''
        IL_0038:  ldloc.s    V_4
        IL_003a:  ldloc.s    V_5
        IL_003c:  cgt
        .line 16707566,16707566 : 0,0 ''
        IL_003e:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_003f:  stloc.3
        .line 16707566,16707566 : 0,0 ''
        IL_0040:  ldloc.3
        IL_0041:  ldc.i4.0
        IL_0042:  bge.s      IL_0046

        .line 16707566,16707566 : 0,0 ''
        IL_0044:  ldloc.3
        IL_0045:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0046:  ldloc.3
        IL_0047:  ldc.i4.0
        IL_0048:  ble.s      IL_004c

        .line 16707566,16707566 : 0,0 ''
        IL_004a:  ldloc.3
        IL_004b:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_004c:  ldloc.1
        IL_004d:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0052:  stloc.s    V_4
        IL_0054:  ldloc.2
        IL_0055:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_005a:  stloc.s    V_5
        .line 16707566,16707566 : 0,0 ''
        IL_005c:  ldloc.s    V_4
        IL_005e:  ldloc.s    V_5
        IL_0060:  bge.s      IL_0064

        .line 16707566,16707566 : 0,0 ''
        IL_0062:  ldc.i4.m1
        IL_0063:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0064:  ldloc.s    V_4
        IL_0066:  ldloc.s    V_5
        IL_0068:  cgt
        IL_006a:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006b:  ldc.i4.1
        IL_006c:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_006d:  ldarg.1
        IL_006e:  unbox.any  Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_0073:  ldnull
        IL_0074:  cgt.un
        IL_0076:  brfalse.s  IL_007a

        .line 16707566,16707566 : 0,0 ''
        IL_0078:  ldc.i4.m1
        IL_0079:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_007a:  ldc.i4.0
        IL_007b:  ret
      } // end of method Key::CompareTo

      .method public hidebysig virtual final 
              instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       61 (0x3d)
        .maxstack  7
        .locals init ([0] int32 V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1)
        .line 4,4 : 10,13 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_003b

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
        IL_0015:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_002a:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
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

        .line 16707566,16707566 : 0,0 ''
        IL_003b:  ldc.i4.0
        IL_003c:  ret
      } // end of method Key::GetHashCode

      .method public hidebysig virtual final 
              instance int32  GetHashCode() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        .line 4,4 : 10,13 ''
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
        // Code size       64 (0x40)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1,
                 [2] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_2)
        .line 4,4 : 10,13 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0038

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  isinst     Compare05/CompareMicroPerfAndCodeGenerationTests/Key
        IL_000d:  stloc.0
        .line 16707566,16707566 : 0,0 ''
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_0036

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
        IL_0018:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001d:  ldloc.2
        IL_001e:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0023:  bne.un.s   IL_0034

        .line 16707566,16707566 : 0,0 ''
        IL_0025:  ldloc.1
        IL_0026:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_002b:  ldloc.2
        IL_002c:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0031:  ceq
        IL_0033:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0034:  ldc.i4.0
        IL_0035:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0036:  ldc.i4.0
        IL_0037:  ret

        .line 16707566,16707566 : 0,0 ''
        IL_0038:  ldarg.1
        IL_0039:  ldnull
        IL_003a:  cgt.un
        IL_003c:  ldc.i4.0
        IL_003d:  ceq
        IL_003f:  ret
      } // end of method Key::Equals

      .method public hidebysig virtual final 
              instance bool  Equals(class Compare05/CompareMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       60 (0x3c)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0,
                 [1] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_1)
        .line 4,4 : 10,13 ''
        IL_0000:  nop
        .line 16707566,16707566 : 0,0 ''
        IL_0001:  ldarg.0
        IL_0002:  ldnull
        IL_0003:  cgt.un
        IL_0005:  brfalse.s  IL_0034

        .line 16707566,16707566 : 0,0 ''
        IL_0007:  ldarg.1
        IL_0008:  ldnull
        IL_0009:  cgt.un
        IL_000b:  brfalse.s  IL_0032

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
        IL_0014:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_0019:  ldloc.1
        IL_001a:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item1
        IL_001f:  bne.un.s   IL_0030

        .line 16707566,16707566 : 0,0 ''
        IL_0021:  ldloc.0
        IL_0022:  ldfld      int32 Compare05/CompareMicroPerfAndCodeGenerationTests/Key::item2
        IL_0027:  ldloc.1
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
              instance bool  Equals(object obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       20 (0x14)
        .maxstack  4
        .locals init ([0] class Compare05/CompareMicroPerfAndCodeGenerationTests/Key V_0)
        .line 4,4 : 10,13 ''
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
      .line 9,9 : 8,32 ''
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
      .line 9,9 : 8,32 ''
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
