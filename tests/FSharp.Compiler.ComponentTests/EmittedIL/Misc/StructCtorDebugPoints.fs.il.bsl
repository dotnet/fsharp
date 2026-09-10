




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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public Flags
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype assembly/Flags>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype assembly/Flags>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int64 bits@
    .method public hidebysig specialname instance int64  get_bits() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int64 assembly/Flags::bits@
      IL_0006:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/Flags obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class [runtime]System.Collections.IComparer V_0,
               int64 V_1,
               int64 V_2)
      IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0005:  stloc.0
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int64 assembly/Flags::bits@
      IL_000c:  stloc.1
      IL_000d:  ldarga.s   obj
      IL_000f:  ldfld      int64 assembly/Flags::bits@
      IL_0014:  stloc.2
      IL_0015:  ldloc.1
      IL_0016:  ldloc.2
      IL_0017:  cgt
      IL_0019:  ldloc.1
      IL_001a:  ldloc.2
      IL_001b:  clt
      IL_001d:  sub
      IL_001e:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/Flags
      IL_0007:  call       instance int32 assembly/Flags::CompareTo(valuetype assembly/Flags)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/Flags V_0,
               int64 V_1,
               int64 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/Flags
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int64 assembly/Flags::bits@
      IL_000d:  stloc.1
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      int64 assembly/Flags::bits@
      IL_0015:  stloc.2
      IL_0016:  ldloc.1
      IL_0017:  ldloc.2
      IL_0018:  cgt
      IL_001a:  ldloc.1
      IL_001b:  ldloc.2
      IL_001c:  clt
      IL_001e:  sub
      IL_001f:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               int64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int64 assembly/Flags::bits@
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  conv.i4
      IL_0010:  ldloc.1
      IL_0011:  ldc.i4.s   32
      IL_0013:  shr
      IL_0014:  conv.i4
      IL_0015:  xor
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
      IL_0020:  ldloc.0
      IL_0021:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 assembly/Flags::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(valuetype assembly/Flags obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int64 assembly/Flags::bits@
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int64 assembly/Flags::bits@
      IL_000d:  ceq
      IL_000f:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype assembly/Flags V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Flags
      IL_0006:  brfalse.s  IL_001f

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/Flags
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldfld      int64 assembly/Flags::bits@
      IL_0015:  ldloca.s   V_0
      IL_0017:  ldfld      int64 assembly/Flags::bits@
      IL_001c:  ceq
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int64 bits) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int64 assembly/Flags::bits@
      IL_0007:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(bool a,
                                 bool b,
                                 bool c) cil managed
    {
      
      .maxstack  5
      .locals init (valuetype assembly/Flags& V_0,
               valuetype assembly/Flags& V_1,
               valuetype assembly/Flags& V_2,
               int64 V_3,
               valuetype assembly/Flags& V_4,
               int64 V_5,
               valuetype assembly/Flags& V_6,
               int64 V_7,
               valuetype assembly/Flags& V_8,
               int64 V_9,
               valuetype assembly/Flags& V_10,
               int64 V_11,
               valuetype assembly/Flags& V_12,
               int64 V_13,
               valuetype assembly/Flags& V_14)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_000d

      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  ldc.i4.1
      IL_0009:  conv.i8
      IL_000a:  nop
      IL_000b:  br.s       IL_0012

      IL_000d:  stloc.2
      IL_000e:  ldloc.2
      IL_000f:  ldc.i4.0
      IL_0010:  conv.i8
      IL_0011:  nop
      IL_0012:  stloc.3
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  ldarg.2
      IL_0019:  brfalse.s  IL_0028

      IL_001b:  stloc.s    V_5
      IL_001d:  stloc.s    V_6
      IL_001f:  ldloc.s    V_6
      IL_0021:  ldloc.s    V_5
      IL_0023:  ldc.i4.2
      IL_0024:  conv.i8
      IL_0025:  nop
      IL_0026:  br.s       IL_0033

      IL_0028:  stloc.s    V_7
      IL_002a:  stloc.s    V_8
      IL_002c:  ldloc.s    V_8
      IL_002e:  ldloc.s    V_7
      IL_0030:  ldc.i4.0
      IL_0031:  conv.i8
      IL_0032:  nop
      IL_0033:  or
      IL_0034:  stloc.s    V_9
      IL_0036:  stloc.s    V_10
      IL_0038:  ldloc.s    V_10
      IL_003a:  ldloc.s    V_9
      IL_003c:  ldarg.3
      IL_003d:  brfalse.s  IL_004c

      IL_003f:  stloc.s    V_11
      IL_0041:  stloc.s    V_12
      IL_0043:  ldloc.s    V_12
      IL_0045:  ldloc.s    V_11
      IL_0047:  ldc.i4.4
      IL_0048:  conv.i8
      IL_0049:  nop
      IL_004a:  br.s       IL_0057

      IL_004c:  stloc.s    V_13
      IL_004e:  stloc.s    V_14
      IL_0050:  ldloc.s    V_14
      IL_0052:  ldloc.s    V_13
      IL_0054:  ldc.i4.0
      IL_0055:  conv.i8
      IL_0056:  nop
      IL_0057:  or
      IL_0058:  call       instance void assembly/Flags::.ctor(int64)
      IL_005d:  ret
    } 

    .method public hidebysig specialname instance int64  get_Bits() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int64 assembly/Flags::bits@
      IL_0006:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/Flags obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int64 assembly/Flags::bits@
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int64 assembly/Flags::bits@
      IL_000d:  ceq
      IL_000f:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype assembly/Flags V_0,
               valuetype assembly/Flags V_1)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Flags
      IL_0006:  brfalse.s  IL_0021

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/Flags
      IL_000e:  stloc.0
      IL_000f:  ldloc.0
      IL_0010:  stloc.1
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int64 assembly/Flags::bits@
      IL_0017:  ldloca.s   V_1
      IL_0019:  ldfld      int64 assembly/Flags::bits@
      IL_001e:  ceq
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } 

    .property instance int64 bits()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int64 assembly/Flags::get_bits()
    } 
    .property instance int64 Bits()
    {
      .get instance int64 assembly/Flags::get_Bits()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 






