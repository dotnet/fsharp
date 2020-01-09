
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
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
  .ver 4:7:0:0
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
  // Offset: 0x00000000 Length: 0x00000754
}
.mresource public FSharpSignatureDataB.StructsAsArrayElements01
{
  // Offset: 0x00000758 Length: 0x0000009B
}
.mresource public FSharpOptimizationData.StructsAsArrayElements01
{
  // Offset: 0x000007F8 Length: 0x0000022C
}
.mresource public FSharpOptimizationDataB.StructsAsArrayElements01
{
  // Offset: 0x00000A28 Length: 0x00000038
}
.module StructsAsArrayElements01.dll
// MVID: {5E171A36-29F3-6E68-A745-0383361A175E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06E70000


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
      // Code size       51 (0x33)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T& V_0,
               [1] class [mscorlib]System.Collections.IComparer V_1,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] class [mscorlib]System.Collections.IComparer V_4,
               [5] int32 V_5,
               [6] int32 V_6)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 7,7 : 6,7 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\StructsAsArrayElements01.fs'
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  stloc.2
      IL_0010:  ldloc.0
      IL_0011:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0016:  stloc.3
      IL_0017:  ldloc.1
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloc.2
      IL_001b:  stloc.s    V_5
      IL_001d:  ldloc.3
      IL_001e:  stloc.s    V_6
      IL_0020:  ldloc.s    V_5
      IL_0022:  ldloc.s    V_6
      IL_0024:  bge.s      IL_0028

      IL_0026:  br.s       IL_002a

      IL_0028:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_002a:  ldc.i4.m1
      IL_002b:  ret

      .line 100001,100001 : 0,0 ''
      IL_002c:  ldloc.s    V_5
      IL_002e:  ldloc.s    V_6
      IL_0030:  cgt
      IL_0032:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      .line 7,7 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  StructsAsArrayElements01/T
      IL_0007:  call       instance int32 StructsAsArrayElements01/T::CompareTo(valuetype StructsAsArrayElements01/T)
      IL_000c:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       56 (0x38)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T V_0,
               [1] valuetype StructsAsArrayElements01/T& V_1,
               [2] class [mscorlib]System.Collections.IComparer V_2,
               [3] int32 V_3,
               [4] int32 V_4,
               [5] class [mscorlib]System.Collections.IComparer V_5,
               [6] int32 V_6,
               [7] int32 V_7)
      .line 7,7 : 6,7 ''
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  StructsAsArrayElements01/T
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.2
      IL_000b:  stloc.2
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.2
      IL_001c:  stloc.s    V_5
      IL_001e:  ldloc.3
      IL_001f:  stloc.s    V_6
      IL_0021:  ldloc.s    V_4
      IL_0023:  stloc.s    V_7
      IL_0025:  ldloc.s    V_6
      IL_0027:  ldloc.s    V_7
      IL_0029:  bge.s      IL_002d

      IL_002b:  br.s       IL_002f

      IL_002d:  br.s       IL_0031

      .line 100001,100001 : 0,0 ''
      IL_002f:  ldc.i4.m1
      IL_0030:  ret

      .line 100001,100001 : 0,0 ''
      IL_0031:  ldloc.s    V_6
      IL_0033:  ldloc.s    V_7
      IL_0035:  cgt
      IL_0037:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       31 (0x1f)
      .maxstack  7
      .locals init ([0] int32 V_0,
               [1] class [mscorlib]System.Collections.IEqualityComparer V_1,
               [2] int32 V_2,
               [3] class [mscorlib]System.Collections.IEqualityComparer V_3)
      .line 7,7 : 6,7 ''
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  stloc.2
      IL_0010:  ldloc.1
      IL_0011:  stloc.3
      IL_0012:  ldloc.2
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.6
      IL_0015:  shl
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.2
      IL_0018:  shr
      IL_0019:  add
      IL_001a:  add
      IL_001b:  add
      IL_001c:  stloc.0
      IL_001d:  ldloc.0
      IL_001e:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      .line 7,7 : 6,7 ''
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 StructsAsArrayElements01/T::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       48 (0x30)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T V_0,
               [1] valuetype StructsAsArrayElements01/T& V_1,
               [2] class [mscorlib]System.Collections.IEqualityComparer V_2,
               [3] int32 V_3,
               [4] int32 V_4,
               [5] class [mscorlib]System.Collections.IEqualityComparer V_5)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructsAsArrayElements01/T>(object)
      IL_0006:  brtrue.s   IL_000a

      IL_0008:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  unbox.any  StructsAsArrayElements01/T
      IL_0010:  stloc.0
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.1
      IL_0014:  ldarg.2
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_001c:  stloc.3
      IL_001d:  ldloc.1
      IL_001e:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0023:  stloc.s    V_4
      IL_0025:  ldloc.2
      IL_0026:  stloc.s    V_5
      IL_0028:  ldloc.3
      IL_0029:  ldloc.s    V_4
      IL_002b:  ceq
      IL_002d:  ret

      .line 100001,100001 : 0,0 ''
      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method T::Equals

    .method public hidebysig instance void 
            Set(int32 i) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 9,9 : 31,42 ''
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 StructsAsArrayElements01/T::i
      IL_0007:  ret
    } // end of method T::Set

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype StructsAsArrayElements01/T obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       18 (0x12)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T& V_0)
      .line 7,7 : 6,7 ''
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0009:  ldloc.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  ceq
      IL_0011:  ret
    } // end of method T::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       27 (0x1b)
      .maxstack  4
      .locals init ([0] valuetype StructsAsArrayElements01/T V_0)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype StructsAsArrayElements01/T>(object)
      IL_0006:  brtrue.s   IL_000a

      IL_0008:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_000a:  ldarg.1
      IL_000b:  unbox.any  StructsAsArrayElements01/T
      IL_0010:  stloc.0
      IL_0011:  ldarg.0
      IL_0012:  ldloc.0
      IL_0013:  call       instance bool StructsAsArrayElements01/T::Equals(valuetype StructsAsArrayElements01/T)
      IL_0018:  ret

      .line 100001,100001 : 0,0 ''
      IL_0019:  ldc.i4.0
      IL_001a:  ret
    } // end of method T::Equals

  } // end of class T

  .method public specialname static valuetype StructsAsArrayElements01/T[] 
          get_a() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
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
    // Code size       35 (0x23)
    .maxstack  4
    .locals init ([0] valuetype StructsAsArrayElements01/T[] a,
             [1] valuetype StructsAsArrayElements01/T V_1)
    .line 11,11 : 1,48 ''
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldloc.1
    IL_0003:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<valuetype StructsAsArrayElements01/T>(int32,
                                                                                                                                   !!0)
    IL_0008:  dup
    IL_0009:  stsfld     valuetype StructsAsArrayElements01/T[] '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01::a@11
    IL_000e:  stloc.0
    .line 12,12 : 1,13 ''
    IL_000f:  call       valuetype StructsAsArrayElements01/T[] StructsAsArrayElements01::get_a()
    IL_0014:  ldc.i4.0
    IL_0015:  ldelema    StructsAsArrayElements01/T
    IL_001a:  ldc.i4.s   27
    IL_001c:  call       instance void StructsAsArrayElements01/T::Set(int32)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method $StructsAsArrayElements01::.cctor

} // end of class '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
