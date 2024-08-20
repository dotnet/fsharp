




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
    .class sequential ansi serializable sealed nested assembly SomeRecord
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 22 00 00 00 00 00 )                         
      .field assembly int32 V@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field assembly int32 U@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method assembly hidebysig specialname instance int32  get_V() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ret
      } 

      .method assembly hidebysig specialname instance int32  get_U() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_0006:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(int32 v, int32 u) cil managed
      {
        .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 39 45 71 75 61 6C 73 31 38 2B   
                                                                                                                                 45 71 75 61 6C 73 4D 69 63 72 6F 50 65 72 66 41   
                                                                                                                                 6E 64 43 6F 64 65 47 65 6E 65 72 61 74 69 6F 6E   
                                                                                                                                 54 65 73 74 73 2B 53 6F 6D 65 52 65 63 6F 72 64   
                                                                                                                                 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_000e:  ret
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
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.1
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000c:  stloc.2
        IL_000d:  ldarga.s   obj
        IL_000f:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
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
        IL_0032:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_0037:  stloc.2
        IL_0038:  ldarga.s   obj
        IL_003a:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
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
        IL_0002:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0007:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  stloc.2
        IL_000e:  ldloca.s   V_0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
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
        IL_002d:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_0032:  stloc.2
        IL_0033:  ldloca.s   V_0
        IL_0035:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
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
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
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
        IL_001d:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
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
        IL_0006:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  bne.un.s   IL_001f

        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_0015:  ldarga.s   obj
        IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  brfalse.s  IL_0018

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldloc.0
        IL_0011:  ldarg.2
        IL_0012:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord,
                                                                                                             class [runtime]System.Collections.IEqualityComparer)
        IL_0017:  ret

        IL_0018:  ldc.i4.0
        IL_0019:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::V@
        IL_000d:  bne.un.s   IL_001f

        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
        IL_0015:  ldarga.s   obj
        IL_0017:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::U@
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
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_0006:  brfalse.s  IL_0015

        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
        IL_000f:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .property instance int32 V()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::get_V()
      } 
      .property instance int32 U()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::get_U()
      } 
    } 

    .field static assembly bool arg@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord x@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord y@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method assembly specialname static bool get_arg@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord get_x@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord get_y@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
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
      IL_0002:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::.ctor(int32,
                                                                                                          int32)
      IL_0007:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_000c:  ldc.i4.2
      IL_000d:  ldc.i4.3
      IL_000e:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::.ctor(int32,
                                                                                                          int32)
      IL_0013:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0018:  ldsflda    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_001d:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
      IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0027:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord,
                                                                                                           class [runtime]System.Collections.IEqualityComparer)
      IL_002c:  stsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0031:  ret
    } 

    .property bool arg@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get bool assembly/EqualsMicroPerfAndCodeGenerationTests::get_arg@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
            x@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::get_x@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord
            y@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeRecord assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
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

.class private auto ansi serializable sealed System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
       extends [runtime]System.Enum
{
  .custom instance void [runtime]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes All = int32(0xFFFFFFFF)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes None = int32(0x00000000)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicParameterlessConstructor = int32(0x00000001)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicConstructors = int32(0x00000003)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicConstructors = int32(0x00000004)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicMethods = int32(0x00000008)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicMethods = int32(0x00000010)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicFields = int32(0x00000020)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicFields = int32(0x00000040)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicNestedTypes = int32(0x00000080)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicNestedTypes = int32(0x00000100)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicProperties = int32(0x00000200)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicProperties = int32(0x00000400)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicEvents = int32(0x00000800)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicEvents = int32(0x00001000)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes Interfaces = int32(0x00002000)
} 

.class private auto ansi beforefieldinit System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [runtime]System.Type Type@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType, class [runtime]System.Type Type) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::MemberType@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::Type@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance class [runtime]System.Type get_Type() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::Type@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes get_MemberType() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::MemberType@
    IL_0006:  ret
  } 

  .property instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
          MemberType()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::get_MemberType()
  } 
  .property instance class [runtime]System.Type
          Type()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::get_Type()
  } 
} 






