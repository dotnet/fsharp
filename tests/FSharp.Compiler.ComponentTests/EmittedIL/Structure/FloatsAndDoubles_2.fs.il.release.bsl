




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed floatsanddoubles
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public Float
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype floatsanddoubles/Float>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype floatsanddoubles/Float>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly float64 F@
    .method public hidebysig specialname instance float64  get_F() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0006:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype floatsanddoubles/Float obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype floatsanddoubles/Float& V_0,
               class [runtime]System.Collections.IComparer V_1,
               float64 V_2,
               float64 V_3)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      float64 floatsanddoubles/Float::F@
      IL_000f:  stloc.2
      IL_0010:  ldloc.0
      IL_0011:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  clt
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  ldc.i4.m1
      IL_001e:  ret

      IL_001f:  ldloc.2
      IL_0020:  ldloc.3
      IL_0021:  cgt
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  ldc.i4.1
      IL_0026:  ret

      IL_0027:  ldloc.2
      IL_0028:  ldloc.3
      IL_0029:  ceq
      IL_002b:  brfalse.s  IL_002f

      IL_002d:  ldc.i4.0
      IL_002e:  ret

      IL_002f:  ldloc.1
      IL_0030:  ldloc.2
      IL_0031:  ldloc.3
      IL_0032:  tail.
      IL_0034:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_0039:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  floatsanddoubles/Float
      IL_0007:  call       instance int32 floatsanddoubles/Float::CompareTo(valuetype floatsanddoubles/Float)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype floatsanddoubles/Float V_0,
               valuetype floatsanddoubles/Float& V_1,
               class [runtime]System.Collections.IComparer V_2,
               float64 V_3,
               float64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  floatsanddoubles/Float
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.2
      IL_000b:  stloc.2
      IL_000c:  ldarg.0
      IL_000d:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  clt
      IL_0020:  brfalse.s  IL_0024

      IL_0022:  ldc.i4.m1
      IL_0023:  ret

      IL_0024:  ldloc.3
      IL_0025:  ldloc.s    V_4
      IL_0027:  cgt
      IL_0029:  brfalse.s  IL_002d

      IL_002b:  ldc.i4.1
      IL_002c:  ret

      IL_002d:  ldloc.3
      IL_002e:  ldloc.s    V_4
      IL_0030:  ceq
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  ldc.i4.0
      IL_0035:  ret

      IL_0036:  ldloc.2
      IL_0037:  ldloc.3
      IL_0038:  ldloc.s    V_4
      IL_003a:  tail.
      IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_0041:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      float64 floatsanddoubles/Float::F@
      IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0)
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
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 floatsanddoubles/Float::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool 
            Equals(valuetype floatsanddoubles/Float obj,
                   class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype floatsanddoubles/Float& V_0,
               class [runtime]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.2
      IL_0004:  stloc.1
      IL_0005:  ldarg.0
      IL_0006:  ldfld      float64 floatsanddoubles/Float::F@
      IL_000b:  ldloc.0
      IL_000c:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0011:  ceq
      IL_0013:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (object V_0,
               valuetype floatsanddoubles/Float V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     floatsanddoubles/Float
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001d

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  floatsanddoubles/Float
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  ldarg.2
      IL_0017:  call       instance bool floatsanddoubles/Float::Equals(valuetype floatsanddoubles/Float,
                                                                        class [runtime]System.Collections.IEqualityComparer)
      IL_001c:  ret

      IL_001d:  ldc.i4.0
      IL_001e:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(float64 f) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      float64 floatsanddoubles/Float::F@
      IL_0007:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype floatsanddoubles/Float obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype floatsanddoubles/Float& V_0,
               float64 V_1,
               float64 V_2)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0009:  stloc.1
      IL_000a:  ldloc.0
      IL_000b:  ldfld      float64 floatsanddoubles/Float::F@
      IL_0010:  stloc.2
      IL_0011:  ldloc.1
      IL_0012:  ldloc.2
      IL_0013:  ceq
      IL_0015:  brfalse.s  IL_0019

      IL_0017:  ldc.i4.1
      IL_0018:  ret

      IL_0019:  ldloc.1
      IL_001a:  ldloc.1
      IL_001b:  beq.s      IL_0025

      IL_001d:  ldloc.2
      IL_001e:  ldloc.2
      IL_001f:  ceq
      IL_0021:  ldc.i4.0
      IL_0022:  ceq
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (object V_0,
               valuetype floatsanddoubles/Float V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     floatsanddoubles/Float
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001c

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  floatsanddoubles/Float
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  call       instance bool floatsanddoubles/Float::Equals(valuetype floatsanddoubles/Float)
      IL_001b:  ret

      IL_001c:  ldc.i4.0
      IL_001d:  ret
    } 

    .property instance float64 F()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance float64 floatsanddoubles/Float::get_F()
    } 
  } 

  .class sequential ansi serializable sealed nested public Double
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype floatsanddoubles/Double>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype floatsanddoubles/Double>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly float64 D@
    .method public hidebysig specialname instance float64  get_D() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0006:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype floatsanddoubles/Double obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype floatsanddoubles/Double& V_0,
               class [runtime]System.Collections.IComparer V_1,
               float64 V_2,
               float64 V_3)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      float64 floatsanddoubles/Double::D@
      IL_000f:  stloc.2
      IL_0010:  ldloc.0
      IL_0011:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  clt
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  ldc.i4.m1
      IL_001e:  ret

      IL_001f:  ldloc.2
      IL_0020:  ldloc.3
      IL_0021:  cgt
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  ldc.i4.1
      IL_0026:  ret

      IL_0027:  ldloc.2
      IL_0028:  ldloc.3
      IL_0029:  ceq
      IL_002b:  brfalse.s  IL_002f

      IL_002d:  ldc.i4.0
      IL_002e:  ret

      IL_002f:  ldloc.1
      IL_0030:  ldloc.2
      IL_0031:  ldloc.3
      IL_0032:  tail.
      IL_0034:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_0039:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  floatsanddoubles/Double
      IL_0007:  call       instance int32 floatsanddoubles/Double::CompareTo(valuetype floatsanddoubles/Double)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype floatsanddoubles/Double V_0,
               valuetype floatsanddoubles/Double& V_1,
               class [runtime]System.Collections.IComparer V_2,
               float64 V_3,
               float64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  floatsanddoubles/Double
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.2
      IL_000b:  stloc.2
      IL_000c:  ldarg.0
      IL_000d:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  clt
      IL_0020:  brfalse.s  IL_0024

      IL_0022:  ldc.i4.m1
      IL_0023:  ret

      IL_0024:  ldloc.3
      IL_0025:  ldloc.s    V_4
      IL_0027:  cgt
      IL_0029:  brfalse.s  IL_002d

      IL_002b:  ldc.i4.1
      IL_002c:  ret

      IL_002d:  ldloc.3
      IL_002e:  ldloc.s    V_4
      IL_0030:  ceq
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  ldc.i4.0
      IL_0035:  ret

      IL_0036:  ldloc.2
      IL_0037:  ldloc.3
      IL_0038:  ldloc.s    V_4
      IL_003a:  tail.
      IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_0041:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      float64 floatsanddoubles/Double::D@
      IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0)
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
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 floatsanddoubles/Double::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool 
            Equals(valuetype floatsanddoubles/Double obj,
                   class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype floatsanddoubles/Double& V_0,
               class [runtime]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.2
      IL_0004:  stloc.1
      IL_0005:  ldarg.0
      IL_0006:  ldfld      float64 floatsanddoubles/Double::D@
      IL_000b:  ldloc.0
      IL_000c:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0011:  ceq
      IL_0013:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (object V_0,
               valuetype floatsanddoubles/Double V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     floatsanddoubles/Double
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001d

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  floatsanddoubles/Double
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  ldarg.2
      IL_0017:  call       instance bool floatsanddoubles/Double::Equals(valuetype floatsanddoubles/Double,
                                                                         class [runtime]System.Collections.IEqualityComparer)
      IL_001c:  ret

      IL_001d:  ldc.i4.0
      IL_001e:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(float64 d) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      float64 floatsanddoubles/Double::D@
      IL_0007:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype floatsanddoubles/Double obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype floatsanddoubles/Double& V_0,
               float64 V_1,
               float64 V_2)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0009:  stloc.1
      IL_000a:  ldloc.0
      IL_000b:  ldfld      float64 floatsanddoubles/Double::D@
      IL_0010:  stloc.2
      IL_0011:  ldloc.1
      IL_0012:  ldloc.2
      IL_0013:  ceq
      IL_0015:  brfalse.s  IL_0019

      IL_0017:  ldc.i4.1
      IL_0018:  ret

      IL_0019:  ldloc.1
      IL_001a:  ldloc.1
      IL_001b:  beq.s      IL_0025

      IL_001d:  ldloc.2
      IL_001e:  ldloc.2
      IL_001f:  ceq
      IL_0021:  ldc.i4.0
      IL_0022:  ceq
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (object V_0,
               valuetype floatsanddoubles/Double V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     floatsanddoubles/Double
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001c

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  floatsanddoubles/Double
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  call       instance bool floatsanddoubles/Double::Equals(valuetype floatsanddoubles/Double)
      IL_001b:  ret

      IL_001c:  ldc.i4.0
      IL_001d:  ret
    } 

    .property instance float64 D()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance float64 floatsanddoubles/Double::get_D()
    } 
  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@31-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo5
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo5) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> floatsanddoubles/'main@31-4'::clo5
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(float64 arg50) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> floatsanddoubles/'main@31-4'::clo5
      IL_0006:  ldarg.1
      IL_0007:  tail.
      IL_0009:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@31-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo4
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo4) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> floatsanddoubles/'main@31-3'::clo4
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(float64 arg40) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> floatsanddoubles/'main@31-3'::clo4
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@31-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@31-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> clo3
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> clo3) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> floatsanddoubles/'main@31-2'::clo3
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> Invoke(bool arg30) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> floatsanddoubles/'main@31-2'::clo3
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@31-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@31-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> clo2
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> clo2) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> floatsanddoubles/'main@31-1'::clo2
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> Invoke(string arg20) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> floatsanddoubles/'main@31-1'::clo2
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@31-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit main@31
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> clo1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> clo1) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> floatsanddoubles/main@31::clo1
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> Invoke(string arg10) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> floatsanddoubles/main@31::clo1
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@31-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@36-9'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo5
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo5) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> floatsanddoubles/'main@36-9'::clo5
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(float64 arg50) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> floatsanddoubles/'main@36-9'::clo5
      IL_0006:  ldarg.1
      IL_0007:  tail.
      IL_0009:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@36-8'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo4
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo4) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> floatsanddoubles/'main@36-8'::clo4
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(float64 arg40) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> floatsanddoubles/'main@36-8'::clo4
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@36-9'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@36-7'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> clo3
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> clo3) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> floatsanddoubles/'main@36-7'::clo3
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> Invoke(bool arg30) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> floatsanddoubles/'main@36-7'::clo3
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@36-8'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@36-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> clo2
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> clo2) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> floatsanddoubles/'main@36-6'::clo2
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> Invoke(string arg20) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> floatsanddoubles/'main@36-6'::clo2
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@36-7'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>)
      IL_0013:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'main@36-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> clo1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> clo1) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> floatsanddoubles/'main@36-5'::clo1
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> Invoke(string arg10) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> floatsanddoubles/'main@36-5'::clo1
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void floatsanddoubles/'main@36-6'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>)
      IL_0013:  ret
    } 

  } 

  .field static assembly valuetype floatsanddoubles/Float[] floats@22
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype floatsanddoubles/Double[] doubles@23
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly string[] names@24
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static valuetype floatsanddoubles/Float[] get_floats() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype floatsanddoubles/Float[] floatsanddoubles::floats@22
    IL_0005:  ret
  } 

  .method public specialname static valuetype floatsanddoubles/Double[] get_doubles() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype floatsanddoubles/Double[] floatsanddoubles::doubles@23
    IL_0005:  ret
  } 

  .method public specialname static string[] get_names() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     string[] floatsanddoubles::names@24
    IL_0005:  ret
  } 

  .method public static int32  main(string[] argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  9
    .locals init (int32 V_0,
             int32 V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> V_2,
             int32 V_3,
             int32 V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>> V_5)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$floatsanddoubles::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$floatsanddoubles::init@
    IL_000b:  pop
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.0
    IL_000e:  br         IL_00b4

    IL_0013:  ldc.i4.0
    IL_0014:  stloc.1
    IL_0015:  br.s       IL_0093

    IL_0017:  ldstr      "Doubles:   %-17s = %-17s  is:  %-5b   Values %f = %f"
    IL_001c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`5<string,string,bool,float64,float64>>::.ctor(string)
    IL_0021:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  newobj     instance void floatsanddoubles/main@31::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>)
    IL_002d:  call       string[] floatsanddoubles::get_names()
    IL_0032:  ldloc.0
    IL_0033:  ldelem     [runtime]System.String
    IL_0038:  call       string[] floatsanddoubles::get_names()
    IL_003d:  ldloc.1
    IL_003e:  ldelem     [runtime]System.String
    IL_0043:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_0048:  ldloc.0
    IL_0049:  ldelema    floatsanddoubles/Double
    IL_004e:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_0053:  ldloc.1
    IL_0054:  ldelem     floatsanddoubles/Double
    IL_0059:  box        floatsanddoubles/Double
    IL_005e:  constrained. floatsanddoubles/Double
    IL_0064:  callvirt   instance bool [runtime]System.Object::Equals(object)
    IL_0069:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_006e:  ldloc.0
    IL_006f:  ldelema    floatsanddoubles/Double
    IL_0074:  ldfld      float64 floatsanddoubles/Double::D@
    IL_0079:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_007e:  ldloc.1
    IL_007f:  ldelema    floatsanddoubles/Double
    IL_0084:  ldfld      float64 floatsanddoubles/Double::D@
    IL_0089:  call       !!3 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::InvokeFast<bool,float64,float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>>>>>,
                                                                                                                                                                                  !0,
                                                                                                                                                                                  !1,
                                                                                                                                                                                  !!0,
                                                                                                                                                                                  !!1,
                                                                                                                                                                                  !!2)
    IL_008e:  pop
    IL_008f:  ldloc.1
    IL_0090:  ldc.i4.1
    IL_0091:  add
    IL_0092:  stloc.1
    IL_0093:  ldloc.1
    IL_0094:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_0099:  ldlen
    IL_009a:  conv.i4
    IL_009b:  blt        IL_0017

    IL_00a0:  ldstr      ""
    IL_00a5:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_00aa:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_00af:  pop
    IL_00b0:  ldloc.0
    IL_00b1:  ldc.i4.1
    IL_00b2:  add
    IL_00b3:  stloc.0
    IL_00b4:  ldloc.0
    IL_00b5:  call       valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
    IL_00ba:  ldlen
    IL_00bb:  conv.i4
    IL_00bc:  blt        IL_0013

    IL_00c1:  ldc.i4.0
    IL_00c2:  stloc.3
    IL_00c3:  br         IL_0175

    IL_00c8:  ldc.i4.0
    IL_00c9:  stloc.s    V_4
    IL_00cb:  br         IL_0153

    IL_00d0:  ldstr      "Floats:    %-17s = %-17s  is:  %-5b   Values %f = %f"
    IL_00d5:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`5<string,string,bool,float64,float64>>::.ctor(string)
    IL_00da:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_00df:  stloc.s    V_5
    IL_00e1:  ldloc.s    V_5
    IL_00e3:  newobj     instance void floatsanddoubles/'main@36-5'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>>>)
    IL_00e8:  call       string[] floatsanddoubles::get_names()
    IL_00ed:  ldloc.3
    IL_00ee:  ldelem     [runtime]System.String
    IL_00f3:  call       string[] floatsanddoubles::get_names()
    IL_00f8:  ldloc.s    V_4
    IL_00fa:  ldelem     [runtime]System.String
    IL_00ff:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_0104:  ldloc.3
    IL_0105:  ldelema    floatsanddoubles/Float
    IL_010a:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_010f:  ldloc.s    V_4
    IL_0111:  ldelem     floatsanddoubles/Float
    IL_0116:  box        floatsanddoubles/Float
    IL_011b:  constrained. floatsanddoubles/Float
    IL_0121:  callvirt   instance bool [runtime]System.Object::Equals(object)
    IL_0126:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_012b:  ldloc.3
    IL_012c:  ldelema    floatsanddoubles/Float
    IL_0131:  ldfld      float64 floatsanddoubles/Float::F@
    IL_0136:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_013b:  ldloc.s    V_4
    IL_013d:  ldelema    floatsanddoubles/Float
    IL_0142:  ldfld      float64 floatsanddoubles/Float::F@
    IL_0147:  call       !!3 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::InvokeFast<bool,float64,float64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>>>>>,
                                                                                                                                                                                  !0,
                                                                                                                                                                                  !1,
                                                                                                                                                                                  !!0,
                                                                                                                                                                                  !!1,
                                                                                                                                                                                  !!2)
    IL_014c:  pop
    IL_014d:  ldloc.s    V_4
    IL_014f:  ldc.i4.1
    IL_0150:  add
    IL_0151:  stloc.s    V_4
    IL_0153:  ldloc.s    V_4
    IL_0155:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_015a:  ldlen
    IL_015b:  conv.i4
    IL_015c:  blt        IL_00d0

    IL_0161:  ldstr      ""
    IL_0166:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_016b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0170:  pop
    IL_0171:  ldloc.3
    IL_0172:  ldc.i4.1
    IL_0173:  add
    IL_0174:  stloc.3
    IL_0175:  ldloc.3
    IL_0176:  call       valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
    IL_017b:  ldlen
    IL_017c:  conv.i4
    IL_017d:  blt        IL_00c8

    IL_0182:  ldc.i4.0
    IL_0183:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$floatsanddoubles::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$floatsanddoubles::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  6
    IL_0000:  ldc.i4.7
    IL_0001:  newarr     floatsanddoubles/Float
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     4.9406564584124654e-324
    IL_0011:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_0016:  stelem     floatsanddoubles/Float
    IL_001b:  dup
    IL_001c:  ldc.i4.1
    IL_001d:  ldc.r8     -1.7976931348623157e+308
    IL_0026:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_002b:  stelem     floatsanddoubles/Float
    IL_0030:  dup
    IL_0031:  ldc.i4.2
    IL_0032:  ldc.r8     1.7976931348623157e+308
    IL_003b:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_0040:  stelem     floatsanddoubles/Float
    IL_0045:  dup
    IL_0046:  ldc.i4.3
    IL_0047:  ldc.r8     (00 00 00 00 00 00 F0 FF)
    IL_0050:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_0055:  stelem     floatsanddoubles/Float
    IL_005a:  dup
    IL_005b:  ldc.i4.4
    IL_005c:  ldc.r8     (00 00 00 00 00 00 F0 7F)
    IL_0065:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_006a:  stelem     floatsanddoubles/Float
    IL_006f:  dup
    IL_0070:  ldc.i4.5
    IL_0071:  ldc.r8     (00 00 00 00 00 00 F8 FF)
    IL_007a:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_007f:  stelem     floatsanddoubles/Float
    IL_0084:  dup
    IL_0085:  ldc.i4.6
    IL_0086:  ldc.r8     7.0999999999999996
    IL_008f:  newobj     instance void floatsanddoubles/Float::.ctor(float64)
    IL_0094:  stelem     floatsanddoubles/Float
    IL_0099:  stsfld     valuetype floatsanddoubles/Float[] floatsanddoubles::floats@22
    IL_009e:  ldc.i4.7
    IL_009f:  newarr     floatsanddoubles/Double
    IL_00a4:  dup
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldc.r8     4.9406564584124654e-324
    IL_00af:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_00b4:  stelem     floatsanddoubles/Double
    IL_00b9:  dup
    IL_00ba:  ldc.i4.1
    IL_00bb:  ldc.r8     -1.7976931348623157e+308
    IL_00c4:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_00c9:  stelem     floatsanddoubles/Double
    IL_00ce:  dup
    IL_00cf:  ldc.i4.2
    IL_00d0:  ldc.r8     1.7976931348623157e+308
    IL_00d9:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_00de:  stelem     floatsanddoubles/Double
    IL_00e3:  dup
    IL_00e4:  ldc.i4.3
    IL_00e5:  ldc.r8     (00 00 00 00 00 00 F0 FF)
    IL_00ee:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_00f3:  stelem     floatsanddoubles/Double
    IL_00f8:  dup
    IL_00f9:  ldc.i4.4
    IL_00fa:  ldc.r8     (00 00 00 00 00 00 F0 7F)
    IL_0103:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_0108:  stelem     floatsanddoubles/Double
    IL_010d:  dup
    IL_010e:  ldc.i4.5
    IL_010f:  ldc.r8     (00 00 00 00 00 00 F8 FF)
    IL_0118:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_011d:  stelem     floatsanddoubles/Double
    IL_0122:  dup
    IL_0123:  ldc.i4.6
    IL_0124:  ldc.r8     8.0999999999999996
    IL_012d:  newobj     instance void floatsanddoubles/Double::.ctor(float64)
    IL_0132:  stelem     floatsanddoubles/Double
    IL_0137:  stsfld     valuetype floatsanddoubles/Double[] floatsanddoubles::doubles@23
    IL_013c:  ldc.i4.7
    IL_013d:  newarr     [runtime]System.String
    IL_0142:  dup
    IL_0143:  ldc.i4.0
    IL_0144:  ldstr      "Epsilon"
    IL_0149:  stelem     [runtime]System.String
    IL_014e:  dup
    IL_014f:  ldc.i4.1
    IL_0150:  ldstr      "MinValue"
    IL_0155:  stelem     [runtime]System.String
    IL_015a:  dup
    IL_015b:  ldc.i4.2
    IL_015c:  ldstr      "MaxValue"
    IL_0161:  stelem     [runtime]System.String
    IL_0166:  dup
    IL_0167:  ldc.i4.3
    IL_0168:  ldstr      "NegativeInfinity"
    IL_016d:  stelem     [runtime]System.String
    IL_0172:  dup
    IL_0173:  ldc.i4.4
    IL_0174:  ldstr      "PositiveInfinity"
    IL_0179:  stelem     [runtime]System.String
    IL_017e:  dup
    IL_017f:  ldc.i4.5
    IL_0180:  ldstr      "NaN"
    IL_0185:  stelem     [runtime]System.String
    IL_018a:  dup
    IL_018b:  ldc.i4.6
    IL_018c:  ldstr      "Number"
    IL_0191:  stelem     [runtime]System.String
    IL_0196:  stsfld     string[] floatsanddoubles::names@24
    IL_019b:  ret
  } 

  .property valuetype floatsanddoubles/Float[]
          floats()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype floatsanddoubles/Float[] floatsanddoubles::get_floats()
  } 
  .property valuetype floatsanddoubles/Double[]
          doubles()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype floatsanddoubles/Double[] floatsanddoubles::get_doubles()
  } 
  .property string[] names()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get string[] floatsanddoubles::get_names()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$floatsanddoubles
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void floatsanddoubles::staticInitialization@()
    IL_0005:  ret
  } 

} 






