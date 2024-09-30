




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
    .class sequential ansi serializable sealed nested assembly SomeStruct
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field assembly int32 v
      .field assembly int32 u
      .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.1
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_000c:  stloc.2
        IL_000d:  ldarga.s   obj
        IL_000f:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0014:  stloc.3
        IL_0015:  ldloc.2
        IL_0016:  ldloc.3
        IL_0017:  cgt
        IL_0019:  ldloc.2
        IL_001a:  ldloc.3
        IL_001b:  clt
        IL_001d:  sub
        IL_001e:  stloc.0
        IL_001f:  ldloc.0
        IL_0020:  ldc.i4.0
        IL_0021:  bge.s      IL_0025

        IL_0023:  ldloc.0
        IL_0024:  ret

        IL_0025:  ldloc.0
        IL_0026:  ldc.i4.0
        IL_0027:  ble.s      IL_002b

        IL_0029:  ldloc.0
        IL_002a:  ret

        IL_002b:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0030:  stloc.1
        IL_0031:  ldarg.0
        IL_0032:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_0037:  stloc.2
        IL_0038:  ldarga.s   obj
        IL_003a:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_003f:  stloc.3
        IL_0040:  ldloc.2
        IL_0041:  ldloc.3
        IL_0042:  cgt
        IL_0044:  ldloc.2
        IL_0045:  ldloc.3
        IL_0046:  clt
        IL_0048:  sub
        IL_0049:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_0007:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_000d:  stloc.2
        IL_000e:  ldloca.s   V_0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0015:  stloc.3
        IL_0016:  ldloc.2
        IL_0017:  ldloc.3
        IL_0018:  cgt
        IL_001a:  ldloc.2
        IL_001b:  ldloc.3
        IL_001c:  clt
        IL_001e:  sub
        IL_001f:  stloc.1
        IL_0020:  ldloc.1
        IL_0021:  ldc.i4.0
        IL_0022:  bge.s      IL_0026

        IL_0024:  ldloc.1
        IL_0025:  ret

        IL_0026:  ldloc.1
        IL_0027:  ldc.i4.0
        IL_0028:  ble.s      IL_002c

        IL_002a:  ldloc.1
        IL_002b:  ret

        IL_002c:  ldarg.0
        IL_002d:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_0032:  stloc.2
        IL_0033:  ldloca.s   V_0
        IL_0035:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_003a:  stloc.3
        IL_003b:  ldloc.2
        IL_003c:  ldloc.3
        IL_003d:  cgt
        IL_003f:  ldloc.2
        IL_0040:  ldloc.3
        IL_0041:  clt
        IL_0043:  sub
        IL_0044:  ret
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
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
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
        IL_0017:  ldc.i4     0x9e3779b9
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.6
        IL_0024:  shl
        IL_0025:  ldloc.0
        IL_0026:  ldc.i4.2
        IL_0027:  shr
        IL_0028:  add
        IL_0029:  add
        IL_002a:  add
        IL_002b:  stloc.0
        IL_002c:  ldloc.0
        IL_002d:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_000d:  bne.un.s   IL_001f

        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_0015:  ldarga.s   obj
        IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_0006:  brfalse.s  IL_0018

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldloc.0
        IL_0011:  ldarg.2
        IL_0012:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct,
                                                                                                             class [runtime]System.Collections.IEqualityComparer)
        IL_0017:  ret

        IL_0018:  ldc.i4.0
        IL_0019:  ret
      } 

      .method public specialname rtspecialname instance void  .ctor(int32 v, int32 u) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_000e:  ret
      } 

      .method public hidebysig specialname instance int32  get_V() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0006:  ret
      } 

      .method public hidebysig specialname instance int32  get_U() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_0006:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::v
        IL_000d:  bne.un.s   IL_001f

        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_0015:  ldarga.s   obj
        IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::u
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_0006:  brfalse.s  IL_0015

        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
        IL_000f:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .property instance int32 V()
      {
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::get_V()
      } 
      .property instance int32 U()
      {
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::get_U()
      } 
    } 

    .field static assembly bool arg@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct x@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct y@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method assembly specialname static bool get_arg@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct get_x@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct get_y@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ldc.i4.2
      IL_0002:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::.ctor(int32,
                                                                                                          int32)
      IL_0007:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_000c:  ldc.i4.2
      IL_000d:  ldc.i4.3
      IL_000e:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::.ctor(int32,
                                                                                                          int32)
      IL_0013:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0018:  ldsflda    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_001d:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
      IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0027:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct,
                                                                                                           class [runtime]System.Collections.IEqualityComparer)
      IL_002c:  stsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0031:  ret
    } 

    .property bool arg@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get bool assembly/EqualsMicroPerfAndCodeGenerationTests::get_arg@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
            x@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::get_x@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct
            y@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeStruct assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
    } 
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly/EqualsMicroPerfAndCodeGenerationTests::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






