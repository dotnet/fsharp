
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
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
  .ver 4:0:0:0
}
.assembly StructsAsArrayElements01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.StructsAsArrayElements01
{
  // Offset: 0x00000000 Length: 0x00000770
}
.mresource public FSharpOptimizationData.StructsAsArrayElements01
{
  // Offset: 0x00000778 Length: 0x0000022C
}
.module StructsAsArrayElements01.dll
// MVID: {4BEB2878-29F3-6E68-A745-03837828EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00250000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed StructsAsArrayElements01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public T
         extends [mscorlib]System.ValueType
         implements class [mscorlib]System.IEquatable`1<valuetype StructsAsArrayElements01/T>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<valuetype StructsAsArrayElements01/T>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field public int32 i
    .method public hidebysig virtual final 
            instance int32  CompareTo(valuetype StructsAsArrayElements01/T obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       39 (0x27)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T& V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarga.s   obj
      IL_0003:  stloc.0
      IL_0004:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0009:  stloc.1
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0010:  stloc.2
      IL_0011:  ldloc.0
      IL_0012:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0017:  stloc.3
      IL_0018:  ldloc.2
      IL_0019:  ldloc.3
      IL_001a:  bge.s      IL_001e

      IL_001c:  br.s       IL_0020

      IL_001e:  br.s       IL_0022

      .line 100001,100001 : 0,0 
      IL_0020:  ldc.i4.m1
      IL_0021:  ret

      .line 100001,100001 : 0,0 
      IL_0022:  ldloc.2
      IL_0023:  ldloc.3
      IL_0024:  cgt
      IL_0026:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  4
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  unbox.any  StructsAsArrayElements01/T
      IL_0008:  call       instance int32 StructsAsArrayElements01/T::CompareTo(valuetype StructsAsArrayElements01/T)
      IL_000d:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       45 (0x2d)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T V_0,
               [1] valuetype StructsAsArrayElements01/T& V_1,
               [2] class [mscorlib]System.Collections.IComparer V_2,
               [3] int32 V_3,
               [4] int32 V_4)
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  StructsAsArrayElements01/T
      IL_0007:  stloc.0
      IL_0008:  ldloca.s   V_0
      IL_000a:  stloc.1
      IL_000b:  ldarg.2
      IL_000c:  stloc.2
      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0013:  stloc.3
      IL_0014:  ldloc.1
      IL_0015:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_001a:  stloc.s    V_4
      IL_001c:  ldloc.3
      IL_001d:  ldloc.s    V_4
      IL_001f:  bge.s      IL_0023

      IL_0021:  br.s       IL_0025

      IL_0023:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      IL_0025:  ldc.i4.m1
      IL_0026:  ret

      .line 100001,100001 : 0,0 
      IL_0027:  ldloc.3
      IL_0028:  ldloc.s    V_4
      IL_002a:  cgt
      IL_002c:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IEqualityComparer V_1)
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  ldc.i4     0x9e3779b9
      IL_0008:  ldarg.1
      IL_0009:  stloc.1
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 StructsAsArrayElements01/T::i
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
      IL_001a:  ldloc.0
      IL_001b:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  4
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0007:  call       instance int32 StructsAsArrayElements01/T::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000c:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       40 (0x28)
      .maxstack  4
      .locals init (valuetype StructsAsArrayElements01/T V_0,
               valuetype StructsAsArrayElements01/T& V_1,
               class [mscorlib]System.Collections.IEqualityComparer V_2)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructsAsArrayElements01/T>(object)
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  br.s       IL_0026

      IL_000b:  ldarg.1
      IL_000c:  unbox.any  StructsAsArrayElements01/T
      IL_0011:  stloc.0
      IL_0012:  ldloca.s   V_0
      IL_0014:  stloc.1
      IL_0015:  ldarg.2
      IL_0016:  stloc.2
      IL_0017:  ldarg.0
      IL_0018:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_001d:  ldloc.1
      IL_001e:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0023:  ceq
      IL_0025:  ret

      IL_0026:  ldc.i4.0
      IL_0027:  ret
    } // end of method T::Equals

    .method public instance void  Set(int32 i) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  4
      .line 9,9 : 31,42 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 StructsAsArrayElements01/T::i
      IL_0008:  ret
    } // end of method T::Set

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype StructsAsArrayElements01/T obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       19 (0x13)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T& V_0)
      .line 7,7 : 6,7 
      IL_0000:  nop
      IL_0001:  ldarga.s   obj
      IL_0003:  stloc.0
      IL_0004:  ldarg.0
      IL_0005:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000a:  ldloc.0
      IL_000b:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0010:  ceq
      IL_0012:  ret
    } // end of method T::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       28 (0x1c)
      .maxstack  4
      .locals init (valuetype StructsAsArrayElements01/T V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructsAsArrayElements01/T>(object)
      IL_0007:  brtrue.s   IL_000b

      IL_0009:  br.s       IL_001a

      IL_000b:  ldarg.1
      IL_000c:  unbox.any  StructsAsArrayElements01/T
      IL_0011:  stloc.0
      IL_0012:  ldarg.0
      IL_0013:  ldloc.0
      IL_0014:  call       instance bool StructsAsArrayElements01/T::Equals(valuetype StructsAsArrayElements01/T)
      IL_0019:  ret

      IL_001a:  ldc.i4.0
      IL_001b:  ret
    } // end of method T::Equals

  } // end of class T

  .method public specialname static valuetype StructsAsArrayElements01/T[] 
          get_a() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  4
    IL_0000:  ldsfld     valuetype StructsAsArrayElements01/T[] '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01::a@11
    IL_0005:  ret
  } // end of method StructsAsArrayElements01::get_a

  .property valuetype StructsAsArrayElements01/T[]
          a()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype StructsAsArrayElements01/T[] StructsAsArrayElements01::get_a()
  } // end of property StructsAsArrayElements01::a
} // end of class StructsAsArrayElements01

.class private abstract auto ansi sealed '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01
       extends [mscorlib]System.Object
{
  .field static assembly initonly valuetype StructsAsArrayElements01/T[] a@11
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  4
    .locals init ([0] valuetype StructsAsArrayElements01/T[] a,
             [1] valuetype StructsAsArrayElements01/T V_1)
    .line 11,11 : 1,48 
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  ldloca.s   V_1
    IL_0005:  initobj    StructsAsArrayElements01/T
    IL_000b:  ldloc.1
    IL_000c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<valuetype StructsAsArrayElements01/T>(int32,
                                                                                                                                   !!0)
    IL_0011:  dup
    IL_0012:  stsfld     valuetype StructsAsArrayElements01/T[] '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01::a@11
    IL_0017:  stloc.0
    .line 12,12 : 1,13 
    IL_0018:  call       valuetype StructsAsArrayElements01/T[] StructsAsArrayElements01::get_a()
    IL_001d:  ldc.i4.0
    IL_001e:  ldelema    StructsAsArrayElements01/T
    IL_0023:  ldc.i4.s   27
    IL_0025:  call       instance void StructsAsArrayElements01/T::Set(int32)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method $StructsAsArrayElements01::.cctor

} // end of class '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
