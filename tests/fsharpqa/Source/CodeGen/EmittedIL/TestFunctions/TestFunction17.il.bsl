
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
.assembly TestFunction17
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction17
{
  // Offset: 0x00000000 Length: 0x0000066E
}
.mresource public FSharpOptimizationData.TestFunction17
{
  // Offset: 0x00000678 Length: 0x000001CD
}
.module TestFunction17.exe
// MVID: {611C52B3-A624-45A8-A745-0383B3521C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06B30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction17
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public R
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class TestFunction17/R>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class TestFunction17/R>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 x@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 y@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_x() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::x@
      IL_0006:  ret
    } // end of method R::get_x

    .method public hidebysig specialname 
            instance int32  get_y() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 TestFunction17/R::y@
      IL_0006:  ret
    } // end of method R::get_y

    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 int32 y) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 TestFunction17/R::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 TestFunction17/R::y@
      IL_0014:  ret
    } // end of method R::.ctor

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class TestFunction17/R>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class TestFunction17/R,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method R::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class TestFunction17/R obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       109 (0x6d)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 4,4 : 6,7 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction17.fs'
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0063

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_0061

      .line 100001,100001 : 0,0 ''
      IL_000d:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0012:  stloc.1
      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 TestFunction17/R::x@
      IL_0019:  stloc.2
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction17/R::x@
      IL_0020:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_0021:  ldloc.2
      IL_0022:  ldloc.3
      IL_0023:  bge.s      IL_0029

      .line 100001,100001 : 0,0 ''
      IL_0025:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  ldloc.2
      IL_002a:  ldloc.3
      IL_002b:  cgt
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 100001,100001 : 0,0 ''
      IL_002e:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.0
      IL_0031:  bge.s      IL_0035

      .line 100001,100001 : 0,0 ''
      IL_0033:  ldloc.0
      IL_0034:  ret

      .line 100001,100001 : 0,0 ''
      IL_0035:  ldloc.0
      IL_0036:  ldc.i4.0
      IL_0037:  ble.s      IL_003b

      .line 100001,100001 : 0,0 ''
      IL_0039:  ldloc.0
      IL_003a:  ret

      .line 100001,100001 : 0,0 ''
      IL_003b:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0040:  stloc.s    V_4
      IL_0042:  ldarg.0
      IL_0043:  ldfld      int32 TestFunction17/R::y@
      IL_0048:  stloc.s    V_5
      IL_004a:  ldarg.1
      IL_004b:  ldfld      int32 TestFunction17/R::y@
      IL_0050:  stloc.s    V_6
      .line 100001,100001 : 0,0 ''
      IL_0052:  ldloc.s    V_5
      IL_0054:  ldloc.s    V_6
      IL_0056:  bge.s      IL_005a

      .line 100001,100001 : 0,0 ''
      IL_0058:  ldc.i4.m1
      IL_0059:  ret

      .line 100001,100001 : 0,0 ''
      IL_005a:  ldloc.s    V_5
      IL_005c:  ldloc.s    V_6
      IL_005e:  cgt
      IL_0060:  ret

      .line 100001,100001 : 0,0 ''
      IL_0061:  ldc.i4.1
      IL_0062:  ret

      .line 100001,100001 : 0,0 ''
      IL_0063:  ldarg.1
      IL_0064:  ldnull
      IL_0065:  cgt.un
      IL_0067:  brfalse.s  IL_006b

      .line 100001,100001 : 0,0 ''
      IL_0069:  ldc.i4.m1
      IL_006a:  ret

      .line 100001,100001 : 0,0 ''
      IL_006b:  ldc.i4.0
      IL_006c:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  TestFunction17/R
      IL_0007:  callvirt   instance int32 TestFunction17/R::CompareTo(class TestFunction17/R)
      IL_000c:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       125 (0x7d)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0,
               [1] class TestFunction17/R V_1,
               [2] int32 V_2,
               [3] class [mscorlib]System.Collections.IComparer V_3,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] class [mscorlib]System.Collections.IComparer V_6,
               [7] int32 V_7,
               [8] int32 V_8)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  TestFunction17/R
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_0009:  ldarg.0
      IL_000a:  ldnull
      IL_000b:  cgt.un
      IL_000d:  brfalse.s  IL_006e

      .line 100001,100001 : 0,0 ''
      IL_000f:  ldarg.1
      IL_0010:  unbox.any  TestFunction17/R
      IL_0015:  ldnull
      IL_0016:  cgt.un
      IL_0018:  brfalse.s  IL_006c

      .line 100001,100001 : 0,0 ''
      IL_001a:  ldarg.2
      IL_001b:  stloc.3
      IL_001c:  ldarg.0
      IL_001d:  ldfld      int32 TestFunction17/R::x@
      IL_0022:  stloc.s    V_4
      IL_0024:  ldloc.1
      IL_0025:  ldfld      int32 TestFunction17/R::x@
      IL_002a:  stloc.s    V_5
      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.s    V_4
      IL_002e:  ldloc.s    V_5
      IL_0030:  bge.s      IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0033:  nop
      IL_0034:  br.s       IL_003d

      .line 100001,100001 : 0,0 ''
      IL_0036:  ldloc.s    V_4
      IL_0038:  ldloc.s    V_5
      IL_003a:  cgt
      .line 100001,100001 : 0,0 ''
      IL_003c:  nop
      .line 100001,100001 : 0,0 ''
      IL_003d:  stloc.2
      .line 100001,100001 : 0,0 ''
      IL_003e:  ldloc.2
      IL_003f:  ldc.i4.0
      IL_0040:  bge.s      IL_0044

      .line 100001,100001 : 0,0 ''
      IL_0042:  ldloc.2
      IL_0043:  ret

      .line 100001,100001 : 0,0 ''
      IL_0044:  ldloc.2
      IL_0045:  ldc.i4.0
      IL_0046:  ble.s      IL_004a

      .line 100001,100001 : 0,0 ''
      IL_0048:  ldloc.2
      IL_0049:  ret

      .line 100001,100001 : 0,0 ''
      IL_004a:  ldarg.2
      IL_004b:  stloc.s    V_6
      IL_004d:  ldarg.0
      IL_004e:  ldfld      int32 TestFunction17/R::y@
      IL_0053:  stloc.s    V_7
      IL_0055:  ldloc.1
      IL_0056:  ldfld      int32 TestFunction17/R::y@
      IL_005b:  stloc.s    V_8
      .line 100001,100001 : 0,0 ''
      IL_005d:  ldloc.s    V_7
      IL_005f:  ldloc.s    V_8
      IL_0061:  bge.s      IL_0065

      .line 100001,100001 : 0,0 ''
      IL_0063:  ldc.i4.m1
      IL_0064:  ret

      .line 100001,100001 : 0,0 ''
      IL_0065:  ldloc.s    V_7
      IL_0067:  ldloc.s    V_8
      IL_0069:  cgt
      IL_006b:  ret

      .line 100001,100001 : 0,0 ''
      IL_006c:  ldc.i4.1
      IL_006d:  ret

      .line 100001,100001 : 0,0 ''
      IL_006e:  ldarg.1
      IL_006f:  unbox.any  TestFunction17/R
      IL_0074:  ldnull
      IL_0075:  cgt.un
      IL_0077:  brfalse.s  IL_007b

      .line 100001,100001 : 0,0 ''
      IL_0079:  ldc.i4.m1
      IL_007a:  ret

      .line 100001,100001 : 0,0 ''
      IL_007b:  ldc.i4.0
      IL_007c:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       59 (0x3b)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IEqualityComparer V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2)
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_0039

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldc.i4.0
      IL_0008:  stloc.0
      IL_0009:  ldc.i4     0x9e3779b9
      IL_000e:  ldarg.1
      IL_000f:  stloc.1
      IL_0010:  ldarg.0
      IL_0011:  ldfld      int32 TestFunction17/R::y@
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
      IL_0025:  ldarg.1
      IL_0026:  stloc.2
      IL_0027:  ldarg.0
      IL_0028:  ldfld      int32 TestFunction17/R::x@
      IL_002d:  ldloc.0
      IL_002e:  ldc.i4.6
      IL_002f:  shl
      IL_0030:  ldloc.0
      IL_0031:  ldc.i4.2
      IL_0032:  shr
      IL_0033:  add
      IL_0034:  add
      IL_0035:  add
      IL_0036:  stloc.0
      IL_0037:  ldloc.0
      IL_0038:  ret

      .line 100001,100001 : 0,0 ''
      IL_0039:  ldc.i4.0
      IL_003a:  ret
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 TestFunction17/R::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method R::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       66 (0x42)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0,
               [1] class TestFunction17/R V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_003a

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  isinst     TestFunction17/R
      IL_000d:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_000e:  ldloc.0
      IL_000f:  brfalse.s  IL_0038

      .line 100001,100001 : 0,0 ''
      IL_0011:  ldloc.0
      IL_0012:  stloc.1
      .line 100001,100001 : 0,0 ''
      IL_0013:  ldarg.2
      IL_0014:  stloc.2
      IL_0015:  ldarg.0
      IL_0016:  ldfld      int32 TestFunction17/R::x@
      IL_001b:  ldloc.1
      IL_001c:  ldfld      int32 TestFunction17/R::x@
      IL_0021:  ceq
      IL_0023:  brfalse.s  IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0025:  ldarg.2
      IL_0026:  stloc.3
      IL_0027:  ldarg.0
      IL_0028:  ldfld      int32 TestFunction17/R::y@
      IL_002d:  ldloc.1
      IL_002e:  ldfld      int32 TestFunction17/R::y@
      IL_0033:  ceq
      IL_0035:  ret

      .line 100001,100001 : 0,0 ''
      IL_0036:  ldc.i4.0
      IL_0037:  ret

      .line 100001,100001 : 0,0 ''
      IL_0038:  ldc.i4.0
      IL_0039:  ret

      .line 100001,100001 : 0,0 ''
      IL_003a:  ldarg.1
      IL_003b:  ldnull
      IL_003c:  cgt.un
      IL_003e:  ldc.i4.0
      IL_003f:  ceq
      IL_0041:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction17/R obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       54 (0x36)
      .maxstack  8
      .line 4,4 : 6,7 ''
      IL_0000:  nop
      .line 100001,100001 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  ldnull
      IL_0003:  cgt.un
      IL_0005:  brfalse.s  IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_002c

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 TestFunction17/R::x@
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 TestFunction17/R::x@
      IL_0019:  bne.un.s   IL_002a

      .line 100001,100001 : 0,0 ''
      IL_001b:  ldarg.0
      IL_001c:  ldfld      int32 TestFunction17/R::y@
      IL_0021:  ldarg.1
      IL_0022:  ldfld      int32 TestFunction17/R::y@
      IL_0027:  ceq
      IL_0029:  ret

      .line 100001,100001 : 0,0 ''
      IL_002a:  ldc.i4.0
      IL_002b:  ret

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldc.i4.0
      IL_002d:  ret

      .line 100001,100001 : 0,0 ''
      IL_002e:  ldarg.1
      IL_002f:  ldnull
      IL_0030:  cgt.un
      IL_0032:  ldc.i4.0
      IL_0033:  ceq
      IL_0035:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction17/R
      IL_0006:  stloc.0
      .line 100001,100001 : 0,0 ''
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool TestFunction17/R::Equals(class TestFunction17/R)
      IL_0011:  ret

      .line 100001,100001 : 0,0 ''
      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method R::Equals

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_x()
    } // end of property R::x
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 TestFunction17/R::get_y()
    } // end of property R::y
  } // end of class R

  .method public static class [mscorlib]System.Tuple`2<class TestFunction17/R,class TestFunction17/R> 
          TestFunction17(int32 inp) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init ([0] class TestFunction17/R x)
    .line 7,7 : 5,24 ''
    IL_0000:  ldc.i4.3
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void TestFunction17/R::.ctor(int32,
                                                               int32)
    IL_0007:  stloc.0
    .line 8,8 : 5,8 ''
    IL_0008:  ldloc.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class [mscorlib]System.Tuple`2<class TestFunction17/R,class TestFunction17/R>::.ctor(!0,
                                                                                                                            !1)
    IL_000f:  ret
  } // end of method TestFunction17::TestFunction17

} // end of class TestFunction17

.class private abstract auto ansi sealed '<StartupCode$TestFunction17>'.$TestFunction17
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction17::main@

} // end of class '<StartupCode$TestFunction17>'.$TestFunction17


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
