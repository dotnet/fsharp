




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class sequential ansi serializable sealed nested public SomeRecord
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .field assembly int32 V@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public hidebysig specialname instance int32  get_V() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ret
      } 

      .method public specialname rtspecialname instance void  .ctor(int32 v) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 39 45 71 75 61 6C 73 32 31 2B   
                                                                                                                                                       45 71 75 61 6C 73 4D 69 63 72 6F 50 65 72 66 41   
                                                                                                                                                       6E 64 43 6F 64 65 47 65 6E 65 72 61 74 69 6F 6E   
                                                                                                                                                       54 65 73 74 73 2B 53 6F 6D 65 52 65 63 6F 72 64   
                                                                                                                                                       00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0007:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class [runtime]System.Collections.IComparer V_0,
                 int32 V_1,
                 int32 V_2)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.0
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000c:  stloc.1
        IL_000d:  ldarga.s   obj
        IL_000f:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
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
        IL_0002:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0007:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0,
                 int32 V_1,
                 int32 V_2)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  stloc.1
        IL_000e:  ldloca.s   V_0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
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
        .locals init (int32 V_0)
        IL_0000:  ldc.i4.0
        IL_0001:  stloc.0
        IL_0002:  ldc.i4     0x9e3779b9
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  ldloc.0
        IL_000e:  ldc.i4.6
        IL_000f:  shl
        IL_0010:  ldloc.0
        IL_0011:  ldc.i4.2
        IL_0012:  shr
        IL_0013:  add
        IL_0014:  add
        IL_0015:  add
        IL_0016:  stloc.0
        IL_0017:  ldloc.0
        IL_0018:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool 
              Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord obj,
                     class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  ceq
        IL_000f:  ret
      } 

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  brfalse.s  IL_001f

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0015:  ldloca.s   V_0
        IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  ceq
        IL_000f:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0,
                 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_1)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  brfalse.s  IL_0021

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_000e:  stloc.0
        IL_000f:  ldloc.0
        IL_0010:  stloc.1
        IL_0011:  ldarg.0
        IL_0012:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0017:  ldloca.s   V_1
        IL_0019:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_001e:  ceq
        IL_0020:  ret

        IL_0021:  ldc.i4.0
        IL_0022:  ret
      } 

      .property instance int32 V()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::get_V()
      } 
    } 

    .method public static bool  test(int32 x,
                                     int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0,
               valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_1)
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::.ctor(int32)
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::.ctor(int32)
      IL_000d:  stloc.1
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
      IL_0015:  ldloca.s   V_1
      IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
      IL_001c:  ceq
      IL_001e:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






