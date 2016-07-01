
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
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
.assembly SteppingMatch09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch09
{
  // Offset: 0x00000000 Length: 0x00000318
}
.mresource public FSharpOptimizationData.SteppingMatch09
{
  // Offset: 0x00000320 Length: 0x000000EB
}
.module SteppingMatch09.dll
// MVID: {5775B195-4935-D6AC-A745-038395B17557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00900000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit GenericInner@15
         extends [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::.ctor()
      IL_0006:  ret
    } // end of method GenericInner@15::.ctor

    .method public strict virtual instance object 
            Specialize<T>() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void class SteppingMatch09/GenericInner@15T<!!T>::.ctor(class SteppingMatch09/GenericInner@15)
      IL_0006:  box        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!T>,int32>
      IL_000b:  ret
    } // end of method GenericInner@15::Specialize

  } // end of class GenericInner@15

  .class auto ansi serializable nested assembly beforefieldinit GenericInner@15T<T>
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!T>,int32>
  {
    .field public class SteppingMatch09/GenericInner@15 self0@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class SteppingMatch09/GenericInner@15 self0@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!T>,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class SteppingMatch09/GenericInner@15 class SteppingMatch09/GenericInner@15T<!T>::self0@
      IL_000d:  ret
    } // end of method GenericInner@15T::.ctor

    .method public strict virtual instance int32 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!T> list) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  6
      .locals init ([0] class SteppingMatch09/GenericInner@15 V_0)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 16,16 : 6,21 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\SteppingMatch\\SteppingMatch09.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class SteppingMatch09/GenericInner@15 class SteppingMatch09/GenericInner@15T<!T>::self0@
      IL_0006:  stloc.0
      IL_0007:  nop
      IL_0008:  ldarg.1
      IL_0009:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!T>::get_TailOrNull()
      IL_000e:  brtrue.s   IL_0012

      IL_0010:  br.s       IL_0014

      IL_0012:  br.s       IL_0016

      .line 17,17 : 14,15 ''
      IL_0014:  ldc.i4.1
      IL_0015:  ret

      .line 18,18 : 13,14 ''
      IL_0016:  ldc.i4.2
      IL_0017:  ret
    } // end of method GenericInner@15T::Invoke

  } // end of class GenericInner@15T

  .class auto ansi serializable nested assembly beforefieldinit NonGenericInner@25
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::.ctor()
      IL_0006:  ret
    } // end of method NonGenericInner@25::.ctor

    .method public strict virtual instance int32 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 25,25 : 6,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  br.s       IL_000d

      IL_000b:  br.s       IL_000f

      .line 26,26 : 14,15 ''
      IL_000d:  ldc.i4.1
      IL_000e:  ret

      .line 27,27 : 13,14 ''
      IL_000f:  ldc.i4.2
      IL_0010:  ret
    } // end of method NonGenericInner@25::Invoke

  } // end of class NonGenericInner@25

  .class auto ansi serializable nested assembly beforefieldinit NonGenericInnerWithCapture@34
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>
  {
    .field public int32 x
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 x) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 SteppingMatch09/NonGenericInnerWithCapture@34::x
      IL_000d:  ret
    } // end of method NonGenericInnerWithCapture@34::.ctor

    .method public strict virtual instance int32 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      .line 34,34 : 6,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  br.s       IL_000d

      IL_000b:  br.s       IL_000f

      .line 35,35 : 14,15 ''
      IL_000d:  ldc.i4.1
      IL_000e:  ret

      .line 36,36 : 13,14 ''
      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 SteppingMatch09/NonGenericInnerWithCapture@34::x
      IL_0015:  ret
    } // end of method NonGenericInnerWithCapture@34::Invoke

  } // end of class NonGenericInnerWithCapture@34

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32> 
          funcA(int32 n) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    .line 5,5 : 9,21 ''
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  sub
    IL_0004:  switch     ( 
                          IL_0013,
                          IL_0015)
    IL_0011:  br.s       IL_0021

    IL_0013:  br.s       IL_0017

    IL_0015:  br.s       IL_001f

    .line 7,7 : 13,21 ''
    IL_0017:  ldc.i4.s   10
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_001e:  ret

    .line 9,9 : 13,17 ''
    IL_001f:  ldnull
    IL_0020:  ret

    .line 11,11 : 20,34 ''
    IL_0021:  ldc.i4.s   22
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_0028:  ret
  } // end of method SteppingMatch09::funcA

  .method public static int32  OuterWithGenericInner<a>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> list) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc GenericInner,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> V_1)
    .line 20,20 : 3,20 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void SteppingMatch09/GenericInner@15::.ctor()
    IL_0006:  stloc.0
    .line 20,20 : 3,20 ''
    IL_0007:  ldloc.0
    IL_0008:  ldarg.0
    IL_0009:  stloc.1
    IL_000a:  callvirt   instance object [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::Specialize<!!0>()
    IL_000f:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>,int32>
    IL_0014:  ldloc.1
    IL_0015:  tail.
    IL_0017:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>,int32>::Invoke(!0)
    IL_001c:  ret
  } // end of method SteppingMatch09::OuterWithGenericInner

  .method public static int32  OuterWithNonGenericInner(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32> NonGenericInner)
    .line 29,29 : 3,23 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void SteppingMatch09/NonGenericInner@25::.ctor()
    IL_0006:  stloc.0
    .line 29,29 : 3,23 ''
    IL_0007:  ldloc.0
    IL_0008:  ldarg.0
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::Invoke(!0)
    IL_0010:  ret
  } // end of method SteppingMatch09::OuterWithNonGenericInner

  .method public static int32  OuterWithNonGenericInnerWithCapture(int32 x,
                                                                   class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32> NonGenericInnerWithCapture)
    .line 38,38 : 3,34 ''
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void SteppingMatch09/NonGenericInnerWithCapture@34::.ctor(int32)
    IL_0007:  stloc.0
    .line 38,38 : 3,34 ''
    IL_0008:  ldloc.0
    IL_0009:  ldarg.1
    IL_000a:  tail.
    IL_000c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::Invoke(!0)
    IL_0011:  ret
  } // end of method SteppingMatch09::OuterWithNonGenericInnerWithCapture

} // end of class SteppingMatch09

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch09>'.$SteppingMatch09
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch09>'.$SteppingMatch09


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
