
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
  // Offset: 0x00000000 Length: 0x0000067E
}
.mresource public FSharpOptimizationData.TestFunction17
{
  // Offset: 0x00000688 Length: 0x000001CD
}
.module TestFunction17.exe
// MVID: {59B199CC-A624-45A8-A745-0383CC99B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x027C0000


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
      // Code size       142 (0x8e)
      .maxstack  4
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction17.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000d

      IL_0008:  br         IL_0080

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  ldnull
      IL_000f:  cgt.un
      IL_0011:  brfalse.s  IL_0015

      IL_0013:  br.s       IL_001a

      IL_0015:  br         IL_007e

      .line 100001,100001 : 0,0 ''
      IL_001a:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_001f:  stloc.1
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 TestFunction17/R::x@
      IL_0026:  stloc.2
      IL_0027:  ldarg.1
      IL_0028:  ldfld      int32 TestFunction17/R::x@
      IL_002d:  stloc.3
      IL_002e:  ldloc.2
      IL_002f:  ldloc.3
      IL_0030:  bge.s      IL_0034

      IL_0032:  br.s       IL_0036

      IL_0034:  br.s       IL_003a

      .line 100001,100001 : 0,0 ''
      IL_0036:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0037:  nop
      IL_0038:  br.s       IL_003f

      .line 100001,100001 : 0,0 ''
      IL_003a:  ldloc.2
      IL_003b:  ldloc.3
      IL_003c:  cgt
      .line 100001,100001 : 0,0 ''
      IL_003e:  nop
      .line 100001,100001 : 0,0 ''
      IL_003f:  stloc.0
      IL_0040:  ldloc.0
      IL_0041:  ldc.i4.0
      IL_0042:  bge.s      IL_0046

      IL_0044:  br.s       IL_0048

      IL_0046:  br.s       IL_004a

      .line 100001,100001 : 0,0 ''
      IL_0048:  ldloc.0
      IL_0049:  ret

      .line 100001,100001 : 0,0 ''
      IL_004a:  ldloc.0
      IL_004b:  ldc.i4.0
      IL_004c:  ble.s      IL_0050

      IL_004e:  br.s       IL_0052

      IL_0050:  br.s       IL_0054

      .line 100001,100001 : 0,0 ''
      IL_0052:  ldloc.0
      IL_0053:  ret

      .line 100001,100001 : 0,0 ''
      IL_0054:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0059:  stloc.s    V_4
      IL_005b:  ldarg.0
      IL_005c:  ldfld      int32 TestFunction17/R::y@
      IL_0061:  stloc.s    V_5
      IL_0063:  ldarg.1
      IL_0064:  ldfld      int32 TestFunction17/R::y@
      IL_0069:  stloc.s    V_6
      IL_006b:  ldloc.s    V_5
      IL_006d:  ldloc.s    V_6
      IL_006f:  bge.s      IL_0073

      IL_0071:  br.s       IL_0075

      IL_0073:  br.s       IL_0077

      .line 100001,100001 : 0,0 ''
      IL_0075:  ldc.i4.m1
      IL_0076:  ret

      .line 100001,100001 : 0,0 ''
      IL_0077:  ldloc.s    V_5
      IL_0079:  ldloc.s    V_6
      IL_007b:  cgt
      IL_007d:  ret

      .line 100001,100001 : 0,0 ''
      IL_007e:  ldc.i4.1
      IL_007f:  ret

      .line 100001,100001 : 0,0 ''
      IL_0080:  ldarg.1
      IL_0081:  ldnull
      IL_0082:  cgt.un
      IL_0084:  brfalse.s  IL_0088

      IL_0086:  br.s       IL_008a

      IL_0088:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_008a:  ldc.i4.m1
      IL_008b:  ret

      .line 100001,100001 : 0,0 ''
      IL_008c:  ldc.i4.0
      IL_008d:  ret
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
      // Code size       159 (0x9f)
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
      IL_0009:  ldarg.0
      IL_000a:  ldnull
      IL_000b:  cgt.un
      IL_000d:  brfalse.s  IL_0011

      IL_000f:  br.s       IL_0016

      IL_0011:  br         IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0016:  ldarg.1
      IL_0017:  unbox.any  TestFunction17/R
      IL_001c:  ldnull
      IL_001d:  cgt.un
      IL_001f:  brfalse.s  IL_0023

      IL_0021:  br.s       IL_0028

      IL_0023:  br         IL_008a

      .line 100001,100001 : 0,0 ''
      IL_0028:  ldarg.2
      IL_0029:  stloc.3
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 TestFunction17/R::x@
      IL_0030:  stloc.s    V_4
      IL_0032:  ldloc.1
      IL_0033:  ldfld      int32 TestFunction17/R::x@
      IL_0038:  stloc.s    V_5
      IL_003a:  ldloc.s    V_4
      IL_003c:  ldloc.s    V_5
      IL_003e:  bge.s      IL_0042

      IL_0040:  br.s       IL_0044

      IL_0042:  br.s       IL_0048

      .line 100001,100001 : 0,0 ''
      IL_0044:  ldc.i4.m1
      .line 100001,100001 : 0,0 ''
      IL_0045:  nop
      IL_0046:  br.s       IL_004f

      .line 100001,100001 : 0,0 ''
      IL_0048:  ldloc.s    V_4
      IL_004a:  ldloc.s    V_5
      IL_004c:  cgt
      .line 100001,100001 : 0,0 ''
      IL_004e:  nop
      .line 100001,100001 : 0,0 ''
      IL_004f:  stloc.2
      IL_0050:  ldloc.2
      IL_0051:  ldc.i4.0
      IL_0052:  bge.s      IL_0056

      IL_0054:  br.s       IL_0058

      IL_0056:  br.s       IL_005a

      .line 100001,100001 : 0,0 ''
      IL_0058:  ldloc.2
      IL_0059:  ret

      .line 100001,100001 : 0,0 ''
      IL_005a:  ldloc.2
      IL_005b:  ldc.i4.0
      IL_005c:  ble.s      IL_0060

      IL_005e:  br.s       IL_0062

      IL_0060:  br.s       IL_0064

      .line 100001,100001 : 0,0 ''
      IL_0062:  ldloc.2
      IL_0063:  ret

      .line 100001,100001 : 0,0 ''
      IL_0064:  ldarg.2
      IL_0065:  stloc.s    V_6
      IL_0067:  ldarg.0
      IL_0068:  ldfld      int32 TestFunction17/R::y@
      IL_006d:  stloc.s    V_7
      IL_006f:  ldloc.1
      IL_0070:  ldfld      int32 TestFunction17/R::y@
      IL_0075:  stloc.s    V_8
      IL_0077:  ldloc.s    V_7
      IL_0079:  ldloc.s    V_8
      IL_007b:  bge.s      IL_007f

      IL_007d:  br.s       IL_0081

      IL_007f:  br.s       IL_0083

      .line 100001,100001 : 0,0 ''
      IL_0081:  ldc.i4.m1
      IL_0082:  ret

      .line 100001,100001 : 0,0 ''
      IL_0083:  ldloc.s    V_7
      IL_0085:  ldloc.s    V_8
      IL_0087:  cgt
      IL_0089:  ret

      .line 100001,100001 : 0,0 ''
      IL_008a:  ldc.i4.1
      IL_008b:  ret

      .line 100001,100001 : 0,0 ''
      IL_008c:  ldarg.1
      IL_008d:  unbox.any  TestFunction17/R
      IL_0092:  ldnull
      IL_0093:  cgt.un
      IL_0095:  brfalse.s  IL_0099

      IL_0097:  br.s       IL_009b

      IL_0099:  br.s       IL_009d

      .line 100001,100001 : 0,0 ''
      IL_009b:  ldc.i4.m1
      IL_009c:  ret

      .line 100001,100001 : 0,0 ''
      IL_009d:  ldc.i4.0
      IL_009e:  ret
    } // end of method R::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       62 (0x3e)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IEqualityComparer V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_003c

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.0
      IL_000c:  ldc.i4     0x9e3779b9
      IL_0011:  ldarg.1
      IL_0012:  stloc.1
      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 TestFunction17/R::y@
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.6
      IL_001b:  shl
      IL_001c:  ldloc.0
      IL_001d:  ldc.i4.2
      IL_001e:  shr
      IL_001f:  add
      IL_0020:  add
      IL_0021:  add
      IL_0022:  stloc.0
      IL_0023:  ldc.i4     0x9e3779b9
      IL_0028:  ldarg.1
      IL_0029:  stloc.2
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 TestFunction17/R::x@
      IL_0030:  ldloc.0
      IL_0031:  ldc.i4.6
      IL_0032:  shl
      IL_0033:  ldloc.0
      IL_0034:  ldc.i4.2
      IL_0035:  shr
      IL_0036:  add
      IL_0037:  add
      IL_0038:  add
      IL_0039:  stloc.0
      IL_003a:  ldloc.0
      IL_003b:  ret

      .line 100001,100001 : 0,0 ''
      IL_003c:  ldc.i4.0
      IL_003d:  ret
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
      // Code size       77 (0x4d)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0,
               [1] class TestFunction17/R V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0045

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  isinst     TestFunction17/R
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  br.s       IL_0018

      IL_0016:  br.s       IL_0043

      .line 100001,100001 : 0,0 ''
      IL_0018:  ldloc.0
      IL_0019:  stloc.1
      IL_001a:  ldarg.2
      IL_001b:  stloc.2
      IL_001c:  ldarg.0
      IL_001d:  ldfld      int32 TestFunction17/R::x@
      IL_0022:  ldloc.1
      IL_0023:  ldfld      int32 TestFunction17/R::x@
      IL_0028:  ceq
      IL_002a:  brfalse.s  IL_002e

      IL_002c:  br.s       IL_0030

      IL_002e:  br.s       IL_0041

      .line 100001,100001 : 0,0 ''
      IL_0030:  ldarg.2
      IL_0031:  stloc.3
      IL_0032:  ldarg.0
      IL_0033:  ldfld      int32 TestFunction17/R::y@
      IL_0038:  ldloc.1
      IL_0039:  ldfld      int32 TestFunction17/R::y@
      IL_003e:  ceq
      IL_0040:  ret

      .line 100001,100001 : 0,0 ''
      IL_0041:  ldc.i4.0
      IL_0042:  ret

      .line 100001,100001 : 0,0 ''
      IL_0043:  ldc.i4.0
      IL_0044:  ret

      .line 100001,100001 : 0,0 ''
      IL_0045:  ldarg.1
      IL_0046:  ldnull
      IL_0047:  cgt.un
      IL_0049:  ldc.i4.0
      IL_004a:  ceq
      IL_004c:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class TestFunction17/R obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       65 (0x41)
      .maxstack  4
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  br.s       IL_000a

      IL_0008:  br.s       IL_0039

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  ldnull
      IL_000c:  cgt.un
      IL_000e:  brfalse.s  IL_0012

      IL_0010:  br.s       IL_0014

      IL_0012:  br.s       IL_0037

      .line 100001,100001 : 0,0 ''
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 TestFunction17/R::x@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 TestFunction17/R::x@
      IL_0020:  bne.un.s   IL_0024

      IL_0022:  br.s       IL_0026

      IL_0024:  br.s       IL_0035

      .line 100001,100001 : 0,0 ''
      IL_0026:  ldarg.0
      IL_0027:  ldfld      int32 TestFunction17/R::y@
      IL_002c:  ldarg.1
      IL_002d:  ldfld      int32 TestFunction17/R::y@
      IL_0032:  ceq
      IL_0034:  ret

      .line 100001,100001 : 0,0 ''
      IL_0035:  ldc.i4.0
      IL_0036:  ret

      .line 100001,100001 : 0,0 ''
      IL_0037:  ldc.i4.0
      IL_0038:  ret

      .line 100001,100001 : 0,0 ''
      IL_0039:  ldarg.1
      IL_003a:  ldnull
      IL_003b:  cgt.un
      IL_003d:  ldc.i4.0
      IL_003e:  ceq
      IL_0040:  ret
    } // end of method R::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       24 (0x18)
      .maxstack  4
      .locals init ([0] class TestFunction17/R V_0)
      .line 4,4 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  isinst     TestFunction17/R
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_000e:  ldarg.0
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance bool TestFunction17/R::Equals(class TestFunction17/R)
      IL_0015:  ret

      .line 100001,100001 : 0,0 ''
      IL_0016:  ldc.i4.0
      IL_0017:  ret
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
