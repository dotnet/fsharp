




.assembly extern runtime { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:0:0:0
}
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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Experiment.Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public Repro
         extends [netstandard]System.ValueType
         implements class [netstandard]System.IEquatable`1<valuetype Experiment.Test/Repro>,
                    [netstandard]System.Collections.IStructuralEquatable,
                    class [netstandard]System.IComparable`1<valuetype Experiment.Test/Repro>,
                    [netstandard]System.IComparable,
                    [netstandard]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 hash@
    .method public hidebysig specialname instance int32  get_hash() cil managed
    {
      .custom instance void System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [netstandard]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0006:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype Experiment.Test/Repro obj) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype Experiment.Test/Repro& V_0,
               class [netstandard]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  call       class [netstandard]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_000f:  stloc.2
      IL_0010:  ldloc.0
      IL_0011:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  cgt
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  clt
      IL_001f:  sub
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  Experiment.Test/Repro
      IL_0007:  call       instance int32 Experiment.Test/Repro::CompareTo(valuetype Experiment.Test/Repro)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [netstandard]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype Experiment.Test/Repro V_0,
               valuetype Experiment.Test/Repro& V_1,
               class [netstandard]System.Collections.IComparer V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  Experiment.Test/Repro
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.2
      IL_000b:  stloc.2
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  cgt
      IL_0020:  ldloc.3
      IL_0021:  ldloc.s    V_4
      IL_0023:  clt
      IL_0025:  sub
      IL_0026:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [netstandard]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class [netstandard]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_000f:  ldloc.0
      IL_0010:  ldc.i4.6
      IL_0011:  shl
      IL_0012:  ldloc.0
      IL_0013:  ldc.i4.2
      IL_0014:  shr
      IL_0015:  add
      IL_0016:  add
      IL_0017:  add
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [netstandard]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 Experiment.Test/Repro::GetHashCode(class [netstandard]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool 
            Equals(valuetype Experiment.Test/Repro obj,
                   class [netstandard]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype Experiment.Test/Repro& V_0,
               class [netstandard]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.2
      IL_0004:  stloc.1
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_000b:  ldloc.0
      IL_000c:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0011:  ceq
      IL_0013:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [netstandard]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (object V_0,
               valuetype Experiment.Test/Repro V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     Experiment.Test/Repro
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001d

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  Experiment.Test/Repro
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  ldarg.2
      IL_0017:  call       instance bool Experiment.Test/Repro::Equals(valuetype Experiment.Test/Repro,
                                                                       class [netstandard]System.Collections.IEqualityComparer)
      IL_001c:  ret

      IL_001d:  ldc.i4.0
      IL_001e:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 length) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               valuetype Experiment.Test/Repro& V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  stloc.1
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.3
      IL_0006:  ldarg.1
      IL_0007:  ldc.i4.1
      IL_0008:  sub
      IL_0009:  stloc.2
      IL_000a:  ldloc.2
      IL_000b:  ldloc.3
      IL_000c:  blt.s      IL_001d

      IL_000e:  ldc.i4.s   26
      IL_0010:  ldloc.0
      IL_0011:  mul
      IL_0012:  stloc.0
      IL_0013:  ldloc.3
      IL_0014:  ldc.i4.1
      IL_0015:  add
      IL_0016:  stloc.3
      IL_0017:  ldloc.3
      IL_0018:  ldloc.2
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  bne.un.s   IL_000e

      IL_001d:  ldloc.1
      IL_001e:  ldloc.0
      IL_001f:  stfld      int32 Experiment.Test/Repro::hash@
      IL_0024:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype Experiment.Test/Repro obj) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype Experiment.Test/Repro& V_0)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_0009:  ldloc.0
      IL_000a:  ldfld      int32 Experiment.Test/Repro::hash@
      IL_000f:  ceq
      IL_0011:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (object V_0,
               valuetype Experiment.Test/Repro V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     Experiment.Test/Repro
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001c

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  Experiment.Test/Repro
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  call       instance bool Experiment.Test/Repro::Equals(valuetype Experiment.Test/Repro)
      IL_001b:  ret

      IL_001c:  ldc.i4.0
      IL_001d:  ret
    } 

    .property instance int32 hash()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 Experiment.Test/Repro::get_hash()
    } 
  } 

  .method public static int32  test() cil managed
  {
    
    .maxstack  3
    .locals init (valuetype Experiment.Test/Repro V_0)
    IL_0000:  ldc.i4.s   42
    IL_0002:  newobj     instance void Experiment.Test/Repro::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldfld      int32 Experiment.Test/Repro::hash@
    IL_000f:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>.$Experiment'.Test
       extends [runtime]System.Object
{
} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.IsReadOnlyAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor() cil managed
  {
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [netstandard]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ret
  } 

} 






