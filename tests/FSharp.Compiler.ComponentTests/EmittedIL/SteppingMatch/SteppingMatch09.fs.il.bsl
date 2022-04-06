
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly SteppingMatch09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch09
{
  // Offset: 0x00000000 Length: 0x00000344
  // WARNING: managed resource file FSharpSignatureData.SteppingMatch09 created
}
.mresource public FSharpOptimizationData.SteppingMatch09
{
  // Offset: 0x00000348 Length: 0x000000EB
  // WARNING: managed resource file FSharpOptimizationData.SteppingMatch09 created
}
.module SteppingMatch09.exe
// MVID: {624CD660-7971-492C-A745-038360D64C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03130000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit GenericInner@15
         extends [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc
  {
    .field static assembly initonly class SteppingMatch09/GenericInner@15 @_instance
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

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void SteppingMatch09/GenericInner@15::.ctor()
      IL_0005:  stsfld     class SteppingMatch09/GenericInner@15 SteppingMatch09/GenericInner@15::@_instance
      IL_000a:  ret
    } // end of method GenericInner@15::.cctor

  } // end of class GenericInner@15

  .class auto ansi serializable sealed nested assembly beforefieldinit GenericInner@15T<T>
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
      // Code size       20 (0x14)
      .maxstack  6
      .locals init (class SteppingMatch09/GenericInner@15 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class SteppingMatch09/GenericInner@15 class SteppingMatch09/GenericInner@15T<!T>::self0@
      IL_0006:  stloc.0
      IL_0007:  nop
      IL_0008:  ldarg.1
      IL_0009:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!T>::get_TailOrNull()
      IL_000e:  brtrue.s   IL_0012

      IL_0010:  ldc.i4.1
      IL_0011:  ret

      IL_0012:  ldc.i4.2
      IL_0013:  ret
    } // end of method GenericInner@15T::Invoke

  } // end of class GenericInner@15T

  .class auto ansi serializable sealed nested assembly beforefieldinit NonGenericInner@25
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>
  {
    .field static assembly initonly class SteppingMatch09/NonGenericInner@25 @_instance
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
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  ldc.i4.1
      IL_000a:  ret

      IL_000b:  ldc.i4.2
      IL_000c:  ret
    } // end of method NonGenericInner@25::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void SteppingMatch09/NonGenericInner@25::.ctor()
      IL_0005:  stsfld     class SteppingMatch09/NonGenericInner@25 SteppingMatch09/NonGenericInner@25::@_instance
      IL_000a:  ret
    } // end of method NonGenericInner@25::.cctor

  } // end of class NonGenericInner@25

  .class auto ansi serializable sealed nested assembly beforefieldinit NonGenericInnerWithCapture@34
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
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  ldc.i4.1
      IL_000a:  ret

      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 SteppingMatch09/NonGenericInnerWithCapture@34::x
      IL_0011:  ret
    } // end of method NonGenericInnerWithCapture@34::Invoke

  } // end of class NonGenericInnerWithCapture@34

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32> 
          funcA(int32 n) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  sub
    IL_0004:  switch     ( 
                          IL_0013,
                          IL_001b)
    IL_0011:  br.s       IL_001d

    IL_0013:  ldc.i4.s   10
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_001a:  ret

    IL_001b:  ldnull
    IL_001c:  ret

    IL_001d:  ldc.i4.s   22
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_0024:  ret
  } // end of method SteppingMatch09::funcA

  .method public static int32  OuterWithGenericInner<a>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> list) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> V_1)
    IL_0000:  ldsfld     class SteppingMatch09/GenericInner@15 SteppingMatch09/GenericInner@15::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stloc.1
    IL_0009:  callvirt   instance object [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::Specialize<!!0>()
    IL_000e:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>,int32>
    IL_0013:  ldloc.1
    IL_0014:  tail.
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>,int32>::Invoke(!0)
    IL_001b:  ret
  } // end of method SteppingMatch09::OuterWithGenericInner

  .method public static int32  OuterWithNonGenericInner(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32> V_0)
    IL_0000:  ldsfld     class SteppingMatch09/NonGenericInner@25 SteppingMatch09/NonGenericInner@25::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  tail.
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::Invoke(!0)
    IL_000f:  ret
  } // end of method SteppingMatch09::OuterWithNonGenericInner

  .method public static int32  OuterWithNonGenericInnerWithCapture(int32 x,
                                                                   class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void SteppingMatch09/NonGenericInnerWithCapture@34::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>::Invoke(!0)
    IL_0010:  ret
  } // end of method SteppingMatch09::OuterWithNonGenericInnerWithCapture

} // end of class SteppingMatch09

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch09>'.$SteppingMatch09
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SteppingMatch09::main@

} // end of class '<StartupCode$SteppingMatch09>'.$SteppingMatch09


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SteppingMatch\SteppingMatch09_fs\SteppingMatch09.res
