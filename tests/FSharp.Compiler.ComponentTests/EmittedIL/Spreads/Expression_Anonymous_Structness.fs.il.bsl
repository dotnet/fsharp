




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
  .class auto ansi serializable sealed nested public RefNominalRecd
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/RefNominalRecd>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/RefNominalRecd>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 a) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 2E 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 41 6E 6F 6E 79 6D 6F 75 73 5F 53 74 72 75   
                                                                                                                                                     63 74 6E 65 73 73 2B 52 65 66 4E 6F 6D 69 6E 61   
                                                                                                                                                     6C 52 65 63 64 00 00 )                            
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/RefNominalRecd::A@
      IL_000d:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/RefNominalRecd,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/RefNominalRecd>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/RefNominalRecd,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/RefNominalRecd,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/RefNominalRecd obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class [runtime]System.Collections.IComparer V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0026

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0024

      IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  stloc.1
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0019:  stloc.2
      IL_001a:  ldloc.1
      IL_001b:  ldloc.2
      IL_001c:  cgt
      IL_001e:  ldloc.1
      IL_001f:  ldloc.2
      IL_0020:  clt
      IL_0022:  sub
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  brfalse.s  IL_002b

      IL_0029:  ldc.i4.m1
      IL_002a:  ret

      IL_002b:  ldc.i4.0
      IL_002c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/RefNominalRecd
      IL_0007:  callvirt   instance int32 assembly/RefNominalRecd::CompareTo(class assembly/RefNominalRecd)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/RefNominalRecd V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/RefNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_002c

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/RefNominalRecd
      IL_0010:  brfalse.s  IL_002a

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0018:  stloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldloc.2
      IL_0022:  cgt
      IL_0024:  ldloc.1
      IL_0025:  ldloc.2
      IL_0026:  clt
      IL_0028:  sub
      IL_0029:  ret

      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldarg.1
      IL_002d:  unbox.any  assembly/RefNominalRecd
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  ldc.i4.m1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001c

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 assembly/RefNominalRecd::A@
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

      IL_001c:  ldc.i4.0
      IL_001d:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/RefNominalRecd::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/RefNominalRecd obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0017

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0015

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  ceq
      IL_0014:  ret

      IL_0015:  ldc.i4.0
      IL_0016:  ret

      IL_0017:  ldarg.1
      IL_0018:  ldnull
      IL_0019:  cgt.un
      IL_001b:  ldc.i4.0
      IL_001c:  ceq
      IL_001e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/RefNominalRecd V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/RefNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/RefNominalRecd::Equals(class assembly/RefNominalRecd,
                                                                                                class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/RefNominalRecd obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0017

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0015

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  ceq
      IL_0014:  ret

      IL_0015:  ldc.i4.0
      IL_0016:  ret

      IL_0017:  ldarg.1
      IL_0018:  ldnull
      IL_0019:  cgt.un
      IL_001b:  ldc.i4.0
      IL_001c:  ceq
      IL_001e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/RefNominalRecd V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/RefNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/RefNominalRecd::Equals(class assembly/RefNominalRecd)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/RefNominalRecd::get_A()
    } 
  } 

  .class sequential ansi serializable sealed nested public StructNominalRecd
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype assembly/StructNominalRecd>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype assembly/StructNominalRecd>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 a) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 31 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 41 6E 6F 6E 79 6D 6F 75 73 5F 53 74 72 75   
                                                                                                                                                     63 74 6E 65 73 73 2B 53 74 72 75 63 74 4E 6F 6D   
                                                                                                                                                     69 6E 61 6C 52 65 63 64 00 00 )                   
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/StructNominalRecd::A@
      IL_0007:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/StructNominalRecd,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/StructNominalRecd>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/StructNominalRecd,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      assembly/StructNominalRecd
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/StructNominalRecd,string>::Invoke(!0)
      IL_001a:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/StructNominalRecd obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class [runtime]System.Collections.IComparer V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0005:  stloc.0
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000c:  stloc.1
      IL_000d:  ldarga.s   obj
      IL_000f:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_0002:  unbox.any  assembly/StructNominalRecd
      IL_0007:  call       instance int32 assembly/StructNominalRecd::CompareTo(valuetype assembly/StructNominalRecd)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/StructNominalRecd V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/StructNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000d:  stloc.1
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_0006:  call       instance int32 assembly/StructNominalRecd::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(valuetype assembly/StructNominalRecd obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000d:  ceq
      IL_000f:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype assembly/StructNominalRecd V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/StructNominalRecd
      IL_0006:  brfalse.s  IL_001f

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/StructNominalRecd
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0015:  ldloca.s   V_0
      IL_0017:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_001c:  ceq
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/StructNominalRecd obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000d:  ceq
      IL_000f:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype assembly/StructNominalRecd V_0,
               valuetype assembly/StructNominalRecd V_1)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/StructNominalRecd
      IL_0006:  brfalse.s  IL_0021

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/StructNominalRecd
      IL_000e:  stloc.0
      IL_000f:  ldloc.0
      IL_0010:  stloc.1
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0017:  ldloca.s   V_1
      IL_0019:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_001e:  ceq
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } 

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/StructNominalRecd::get_A()
    } 
  } 

  .class abstract auto ansi sealed nested public CopyAndUpdateAnonRecd
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'ref anon src, no explicit target, stays ref@25-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref anon src, explicit struct target, becomes struct@26-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref anon src, inferred struct target, becomes struct@27-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'struct anon src, no explicit target, stays struct@28-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@28-12'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@28-13'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct anon src, explicit struct target, stays struct@29-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@29-14'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@29-15'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct anon src, inferred struct target, stays struct@30-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@30-16'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@30-17'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'ref nominal src, no explicit target, stays ref@32-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref nominal src, explicit struct target, becomes struct@33-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref nominal src, inferred struct target, becomes struct@34-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'struct nominal src, no explicit target, stays struct@35-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@35-18'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@35-19'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct nominal src, explicit struct target, stays struct@36-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@36-20'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@36-21'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct nominal src, inferred struct target, stays struct@37-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@37-22'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@37-23'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_ref anon src, no explicit target, stays ref'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, no explicit target, stays ref@25-1'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref anon src, explicit struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, explicit struct target, becomes struct@26-1'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref anon src, inferred struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, inferred struct target, becomes struct@27-1'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_struct anon src, no explicit target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, no explicit target, stays struct@28-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@28-12'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@28-12'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@28-13'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@28-13'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct anon src, explicit struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, explicit struct target, stays struct@29-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@29-14'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@29-14'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@29-15'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@29-15'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct anon src, inferred struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, inferred struct target, stays struct@30-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@30-16'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@30-16'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@30-17'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@30-17'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_ref nominal src, no explicit target, stays ref'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, no explicit target, stays ref@32-1'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref nominal src, explicit struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, explicit struct target, becomes struct@33-1'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref nominal src, inferred struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, inferred struct target, becomes struct@34-1'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_struct nominal src, no explicit target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, no explicit target, stays struct@35-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@35-18'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@35-18'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@35-19'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@35-19'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct nominal src, explicit struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, explicit struct target, stays struct@36-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@36-20'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@36-20'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@36-21'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@36-21'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct nominal src, inferred struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, inferred struct target, stays struct@37-1'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@37-22'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@37-22'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@37-23'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@37-23'
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly static void  staticInitialization@() cil managed
    {
      
      .maxstack  4
      IL_0000:  nop
      IL_0001:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_0006:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_000b:  ldc.i4.2
      IL_000c:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_0011:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, no explicit target, stays ref@25-1'
      IL_0016:  nop
      IL_0017:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_001c:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_0021:  ldc.i4.2
      IL_0022:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_0027:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, explicit struct target, becomes struct@26-1'
      IL_002c:  nop
      IL_002d:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_0032:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_0037:  ldc.i4.2
      IL_0038:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_003d:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref anon src, inferred struct target, becomes struct@27-1'
      IL_0042:  nop
      IL_0043:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0048:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@28-12'
      IL_004d:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@28-12'()
      IL_0052:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@28-13'
      IL_0057:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@28-13'
      IL_005c:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0061:  ldc.i4.2
      IL_0062:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_0067:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, no explicit target, stays struct@28-1'
      IL_006c:  nop
      IL_006d:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0072:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@29-14'
      IL_0077:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@29-14'()
      IL_007c:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@29-15'
      IL_0081:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@29-15'
      IL_0086:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_008b:  ldc.i4.2
      IL_008c:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_0091:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, explicit struct target, stays struct@29-1'
      IL_0096:  nop
      IL_0097:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_009c:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@30-16'
      IL_00a1:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@30-16'()
      IL_00a6:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@30-17'
      IL_00ab:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@30-17'
      IL_00b0:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_00b5:  ldc.i4.2
      IL_00b6:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00bb:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct anon src, inferred struct target, stays struct@30-1'
      IL_00c0:  nop
      IL_00c1:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00c6:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00cb:  ldc.i4.2
      IL_00cc:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_00d1:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, no explicit target, stays ref@32-1'
      IL_00d6:  nop
      IL_00d7:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00dc:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00e1:  ldc.i4.2
      IL_00e2:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00e7:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, explicit struct target, becomes struct@33-1'
      IL_00ec:  nop
      IL_00ed:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00f2:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00f7:  ldc.i4.2
      IL_00f8:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00fd:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'ref nominal src, inferred struct target, becomes struct@34-1'
      IL_0102:  nop
      IL_0103:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0108:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@35-18'
      IL_010d:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@35-18'()
      IL_0112:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@35-19'
      IL_0117:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@35-19'
      IL_011c:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0121:  ldc.i4.2
      IL_0122:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_0127:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, no explicit target, stays struct@35-1'
      IL_012c:  nop
      IL_012d:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0132:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@36-20'
      IL_0137:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@36-20'()
      IL_013c:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@36-21'
      IL_0141:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@36-21'
      IL_0146:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_014b:  ldc.i4.2
      IL_014c:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_0151:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, explicit struct target, stays struct@36-1'
      IL_0156:  nop
      IL_0157:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_015c:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@37-22'
      IL_0161:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@37-22'()
      IL_0166:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@37-23'
      IL_016b:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'copyOfStruct@37-23'
      IL_0170:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0175:  ldc.i4.2
      IL_0176:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_017b:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'struct nominal src, inferred struct target, stays struct@37-1'
      IL_0180:  ret
    } 

    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'ref anon src, no explicit target, stays ref'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref anon src, no explicit target, stays ref'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref anon src, explicit struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref anon src, explicit struct target, becomes struct'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref anon src, inferred struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref anon src, inferred struct target, becomes struct'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'struct anon src, no explicit target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct anon src, no explicit target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@28-12'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@28-12'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@28-13'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@28-13'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct anon src, explicit struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct anon src, explicit struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@29-14'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@29-14'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@29-15'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@29-15'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct anon src, inferred struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct anon src, inferred struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@30-16'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@30-16'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@30-17'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@30-17'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'ref nominal src, no explicit target, stays ref'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref nominal src, no explicit target, stays ref'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref nominal src, explicit struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref nominal src, explicit struct target, becomes struct'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref nominal src, inferred struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_ref nominal src, inferred struct target, becomes struct'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'struct nominal src, no explicit target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct nominal src, no explicit target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@35-18'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@35-18'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@35-19'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@35-19'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct nominal src, explicit struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct nominal src, explicit struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@36-20'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@36-20'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@36-21'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@36-21'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct nominal src, inferred struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/CopyAndUpdateAnonRecd::'get_struct nominal src, inferred struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@37-22'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@37-22'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@37-23'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/CopyAndUpdateAnonRecd::'get_copyOfStruct@37-23'()
    } 
  } 

  .class abstract auto ansi sealed nested public NewAnonRecd
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'ref anon src, no explicit target, stays ref@10'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref anon src, explicit struct target, becomes struct@11'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref anon src, inferred struct target, becomes struct@12'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'struct anon src, no explicit target, stays struct@13'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> copyOfStruct@13
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@13-1'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct anon src, explicit struct target, stays struct@14'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@14-2'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@14-3'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct anon src, inferred struct target, stays struct@15'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@15-4'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@15-5'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'ref nominal src, no explicit target, stays ref@17'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref nominal src, explicit struct target, becomes struct@18'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'ref nominal src, inferred struct target, becomes struct@19'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly class '<>f__AnonymousType3357665219`2'<int32,int32> 'struct nominal src, no explicit target, stays struct@20'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@20-6'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@20-7'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct nominal src, explicit struct target, stays struct@21'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@21-8'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@21-9'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'struct nominal src, inferred struct target, stays struct@22'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@22-10'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> 'copyOfStruct@22-11'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_ref anon src, no explicit target, stays ref'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, no explicit target, stays ref@10'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref anon src, explicit struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, explicit struct target, becomes struct@11'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref anon src, inferred struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, inferred struct target, becomes struct@12'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_struct anon src, no explicit target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, no explicit target, stays struct@13'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> get_copyOfStruct@13() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::copyOfStruct@13
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@13-1'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@13-1'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct anon src, explicit struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, explicit struct target, stays struct@14'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@14-2'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@14-2'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@14-3'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@14-3'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct anon src, inferred struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, inferred struct target, stays struct@15'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@15-4'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@15-4'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@15-5'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@15-5'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_ref nominal src, no explicit target, stays ref'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, no explicit target, stays ref@17'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref nominal src, explicit struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, explicit struct target, becomes struct@18'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_ref nominal src, inferred struct target, becomes struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, inferred struct target, becomes struct@19'
      IL_0005:  ret
    } 

    .method public specialname static class '<>f__AnonymousType3357665219`2'<int32,int32> 'get_struct nominal src, no explicit target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, no explicit target, stays struct@20'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@20-6'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@20-6'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@20-7'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@20-7'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct nominal src, explicit struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, explicit struct target, stays struct@21'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@21-8'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@21-8'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@21-9'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@21-9'
      IL_0005:  ret
    } 

    .method public specialname static valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> 'get_struct nominal src, inferred struct target, stays struct'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, inferred struct target, stays struct@22'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@22-10'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@22-10'
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> 'get_copyOfStruct@22-11'() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@22-11'
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly static void  staticInitialization@() cil managed
    {
      
      .maxstack  4
      IL_0000:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_0005:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_000a:  ldc.i4.2
      IL_000b:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_0010:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, no explicit target, stays ref@10'
      IL_0015:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_001a:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_001f:  ldc.i4.2
      IL_0020:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_0025:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, explicit struct target, becomes struct@11'
      IL_002a:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_002f:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_0034:  ldc.i4.2
      IL_0035:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_003a:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref anon src, inferred struct target, becomes struct@12'
      IL_003f:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0044:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::copyOfStruct@13
      IL_0049:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::get_copyOfStruct@13()
      IL_004e:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@13-1'
      IL_0053:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@13-1'
      IL_0058:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_005d:  ldc.i4.2
      IL_005e:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_0063:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, no explicit target, stays struct@13'
      IL_0068:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_006d:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@14-2'
      IL_0072:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@14-2'()
      IL_0077:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@14-3'
      IL_007c:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@14-3'
      IL_0081:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0086:  ldc.i4.2
      IL_0087:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_008c:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, explicit struct target, stays struct@14'
      IL_0091:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0096:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@15-4'
      IL_009b:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@15-4'()
      IL_00a0:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@15-5'
      IL_00a5:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@15-5'
      IL_00aa:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_00af:  ldc.i4.2
      IL_00b0:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00b5:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct anon src, inferred struct target, stays struct@15'
      IL_00ba:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00bf:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00c4:  ldc.i4.2
      IL_00c5:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_00ca:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, no explicit target, stays ref@17'
      IL_00cf:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00d4:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00d9:  ldc.i4.2
      IL_00da:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00df:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, explicit struct target, becomes struct@18'
      IL_00e4:  call       class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
      IL_00e9:  call       instance !0 class '<>f__AnonymousType3348076434`1'<int32>::get_A()
      IL_00ee:  ldc.i4.2
      IL_00ef:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_00f4:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'ref nominal src, inferred struct target, becomes struct@19'
      IL_00f9:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_00fe:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@20-6'
      IL_0103:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@20-6'()
      IL_0108:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@20-7'
      IL_010d:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@20-7'
      IL_0112:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0117:  ldc.i4.2
      IL_0118:  newobj     instance void class '<>f__AnonymousType3357665219`2'<int32,int32>::.ctor(!0,
                                                                                                    !1)
      IL_011d:  stsfld     class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, no explicit target, stays struct@20'
      IL_0122:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0127:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@21-8'
      IL_012c:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@21-8'()
      IL_0131:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@21-9'
      IL_0136:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@21-9'
      IL_013b:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0140:  ldc.i4.2
      IL_0141:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_0146:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, explicit struct target, stays struct@21'
      IL_014b:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
      IL_0150:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@22-10'
      IL_0155:  call       valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@22-10'()
      IL_015a:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@22-11'
      IL_015f:  ldsflda    valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'copyOfStruct@22-11'
      IL_0164:  call       instance !0 valuetype '<>f__AnonymousType10002306269156`1'<int32>::get_A()
      IL_0169:  ldc.i4.2
      IL_016a:  newobj     instance void valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>::.ctor(!0,
                                                                                                            !1)
      IL_016f:  stsfld     valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'struct nominal src, inferred struct target, stays struct@22'
      IL_0174:  ret
    } 

    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'ref anon src, no explicit target, stays ref'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'get_ref anon src, no explicit target, stays ref'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref anon src, explicit struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_ref anon src, explicit struct target, becomes struct'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref anon src, inferred struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_ref anon src, inferred struct target, becomes struct'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'struct anon src, no explicit target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'get_struct anon src, no explicit target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            copyOfStruct@13()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::get_copyOfStruct@13()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@13-1'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@13-1'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct anon src, explicit struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_struct anon src, explicit struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@14-2'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@14-2'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@14-3'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@14-3'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct anon src, inferred struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_struct anon src, inferred struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@15-4'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@15-4'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@15-5'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@15-5'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'ref nominal src, no explicit target, stays ref'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'get_ref nominal src, no explicit target, stays ref'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref nominal src, explicit struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_ref nominal src, explicit struct target, becomes struct'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'ref nominal src, inferred struct target, becomes struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_ref nominal src, inferred struct target, becomes struct'()
    } 
    .property class '<>f__AnonymousType3357665219`2'<int32,int32>
            'struct nominal src, no explicit target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class '<>f__AnonymousType3357665219`2'<int32,int32> assembly/NewAnonRecd::'get_struct nominal src, no explicit target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@20-6'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@20-6'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@20-7'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@20-7'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct nominal src, explicit struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_struct nominal src, explicit struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@21-8'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@21-8'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@21-9'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@21-9'()
    } 
    .property valuetype '<>f__AnonymousType10001789011089`2'<int32,int32>
            'struct nominal src, inferred struct target, stays struct'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10001789011089`2'<int32,int32> assembly/NewAnonRecd::'get_struct nominal src, inferred struct target, stays struct'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@22-10'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@22-10'()
    } 
    .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
            'copyOfStruct@22-11'()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly/NewAnonRecd::'get_copyOfStruct@22-11'()
    } 
  } 

  .field static assembly class '<>f__AnonymousType3348076434`1'<int32> refAnonRecd@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType10002306269156`1'<int32> structAnonRecd@5
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd refNominalRecd@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd structNominalRecd@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class '<>f__AnonymousType3348076434`1'<int32> get_refAnonRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType3348076434`1'<int32> assembly::refAnonRecd@4
    IL_0005:  ret
  } 

  .method public specialname static valuetype '<>f__AnonymousType10002306269156`1'<int32> get_structAnonRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::structAnonRecd@5
    IL_0005:  ret
  } 

  .method public specialname static class assembly/RefNominalRecd get_refNominalRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/RefNominalRecd assembly::refNominalRecd@6
    IL_0005:  ret
  } 

  .method public specialname static valuetype assembly/StructNominalRecd get_structNominalRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::structNominalRecd@7
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  newobj     instance void class '<>f__AnonymousType3348076434`1'<int32>::.ctor(!0)
    IL_0006:  stsfld     class '<>f__AnonymousType3348076434`1'<int32> assembly::refAnonRecd@4
    IL_000b:  ldc.i4.1
    IL_000c:  newobj     instance void valuetype '<>f__AnonymousType10002306269156`1'<int32>::.ctor(!0)
    IL_0011:  stsfld     valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::structAnonRecd@5
    IL_0016:  ldc.i4.1
    IL_0017:  newobj     instance void assembly/RefNominalRecd::.ctor(int32)
    IL_001c:  stsfld     class assembly/RefNominalRecd assembly::refNominalRecd@6
    IL_0021:  ldc.i4.1
    IL_0022:  newobj     instance void assembly/StructNominalRecd::.ctor(int32)
    IL_0027:  stsfld     valuetype assembly/StructNominalRecd assembly::structNominalRecd@7
    IL_002c:  call       void assembly/NewAnonRecd::staticInitialization@()
    IL_0031:  call       void assembly/CopyAndUpdateAnonRecd::staticInitialization@()
    IL_0036:  ret
  } 

  .property class '<>f__AnonymousType3348076434`1'<int32>
          refAnonRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType3348076434`1'<int32> assembly::get_refAnonRecd()
  } 
  .property valuetype '<>f__AnonymousType10002306269156`1'<int32>
          structAnonRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType10002306269156`1'<int32> assembly::get_structAnonRecd()
  } 
  .property class assembly/RefNominalRecd
          refNominalRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/RefNominalRecd assembly::get_refNominalRecd()
  } 
  .property valuetype assembly/StructNominalRecd
          structNominalRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::get_structNominalRecd()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
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

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType3348076434`1'<'<A>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 33 33 34 38 30 37 36   
                                                                                                                                                   34 33 34 60 31 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_000d:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0021

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001f

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0017:  tail.
    IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001e:  ret

    IL_001f:  ldc.i4.1
    IL_0020:  ret

    IL_0021:  ldarg.1
    IL_0022:  brfalse.s  IL_0026

    IL_0024:  ldc.i4.m1
    IL_0025:  ret

    IL_0026:  ldc.i4.0
    IL_0027:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::CompareTo(class '<>f__AnonymousType3348076434`1'<!0>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> V_0,
             class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> V_1)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_002b

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0012:  brfalse.s  IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0021:  tail.
    IL_0023:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0028:  ret

    IL_0029:  ldc.i4.1
    IL_002a:  ret

    IL_002b:  ldarg.1
    IL_002c:  unbox.any  class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0031:  brfalse.s  IL_0035

    IL_0033:  ldc.i4.m1
    IL_0034:  ret

    IL_0035:  ldc.i4.0
    IL_0036:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0022

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
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

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001f

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001d

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0015:  tail.
    IL_0017:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_001c:  ret

    IL_001d:  ldc.i4.0
    IL_001e:  ret

    IL_001f:  ldarg.1
    IL_0020:  ldnull
    IL_0021:  cgt.un
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::Equals(class '<>f__AnonymousType3348076434`1'<!0>,
                                                                                                     class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001c

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001a

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::A@
    IL_0012:  tail.
    IL_0014:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret

    IL_001c:  ldarg.1
    IL_001d:  ldnull
    IL_001e:  cgt.un
    IL_0020:  ldc.i4.0
    IL_0021:  ceq
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType3348076434`1'<!'<A>j__TPar'>::Equals(class '<>f__AnonymousType3348076434`1'<!0>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType3348076434`1'::get_A()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType3357665219`2'<'<A>j__TPar','<B>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private !'<B>j__TPar' B@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A, !'<B>j__TPar' B) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 33 33 35 37 36 36 35   
                                                                                                                                                   32 31 39 60 32 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<B>j__TPar' get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0044

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0042

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  bge.s      IL_0023

    IL_0021:  ldloc.0
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ble.s      IL_0029

    IL_0027:  ldloc.0
    IL_0028:  ret

    IL_0029:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002e:  ldarg.0
    IL_002f:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0034:  ldarg.1
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  tail.
    IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0041:  ret

    IL_0042:  ldc.i4.1
    IL_0043:  ret

    IL_0044:  ldarg.1
    IL_0045:  brfalse.s  IL_0049

    IL_0047:  ldc.i4.m1
    IL_0048:  ret

    IL_0049:  ldc.i4.0
    IL_004a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(class '<>f__AnonymousType3357665219`2'<!0,!1>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_004a

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0012:  brfalse.s  IL_0048

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0021:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.0
    IL_0029:  bge.s      IL_002d

    IL_002b:  ldloc.2
    IL_002c:  ret

    IL_002d:  ldloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  ble.s      IL_0033

    IL_0031:  ldloc.2
    IL_0032:  ret

    IL_0033:  ldarg.2
    IL_0034:  ldarg.0
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  ldloc.1
    IL_003b:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0040:  tail.
    IL_0042:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0047:  ret

    IL_0048:  ldc.i4.1
    IL_0049:  ret

    IL_004a:  ldarg.1
    IL_004b:  unbox.any  class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0050:  brfalse.s  IL_0054

    IL_0052:  ldc.i4.m1
    IL_0053:  ret

    IL_0054:  ldc.i4.0
    IL_0055:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003d

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
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
    IL_0020:  ldc.i4     0x9e3779b9
    IL_0025:  ldarg.1
    IL_0026:  ldarg.0
    IL_0027:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_002c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.6
    IL_0033:  shl
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.2
    IL_0036:  shr
    IL_0037:  add
    IL_0038:  add
    IL_0039:  add
    IL_003a:  stloc.0
    IL_003b:  ldloc.0
    IL_003c:  ret

    IL_003d:  ldc.i4.0
    IL_003e:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0035

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0033

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0015:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_001a:  brfalse.s  IL_0031

    IL_001c:  ldarg.2
    IL_001d:  ldarg.0
    IL_001e:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0023:  ldloc.0
    IL_0024:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0029:  tail.
    IL_002b:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0030:  ret

    IL_0031:  ldc.i4.0
    IL_0032:  ret

    IL_0033:  ldc.i4.0
    IL_0034:  ret

    IL_0035:  ldarg.1
    IL_0036:  ldnull
    IL_0037:  cgt.un
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3357665219`2'<!0,!1>,
                                                                                                                   class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002f

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0017:  brfalse.s  IL_002d

    IL_0019:  ldarg.0
    IL_001a:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !1 class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0025:  tail.
    IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_002c:  ret

    IL_002d:  ldc.i4.0
    IL_002e:  ret

    IL_002f:  ldc.i4.0
    IL_0030:  ret

    IL_0031:  ldarg.1
    IL_0032:  ldnull
    IL_0033:  cgt.un
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType3357665219`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3357665219`2'<!0,!1>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType3357665219`2'::get_A()
  } 
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<B>j__TPar' '<>f__AnonymousType3357665219`2'::get_B()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType10002306269156`1'<'<A>j__TPar'>
       extends [runtime]System.ValueType
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 22 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 31 30 30 30 32 33 30   
                                                                                                                                                   36 32 36 39 31 35 36 60 31 00 00 )                
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0007:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  ldobj      valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>,string>::Invoke(!0)
    IL_001a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0008:  ldarg.0
    IL_0009:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_000e:  ldloc.0
    IL_000f:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0014:  tail.
    IL_0016:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001b:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>
    IL_0007:  call       instance int32 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::CompareTo(valuetype '<>f__AnonymousType10002306269156`1'<!0>)
    IL_000c:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> V_0,
             valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>& V_1)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  stloc.1
    IL_000a:  ldarg.2
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0011:  ldloc.1
    IL_0012:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0017:  tail.
    IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001e:  ret
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
    IL_0009:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
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
    IL_0006:  call       instance int32 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000b:  ret
  } 

  .method public hidebysig instance bool Equals(valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.2
    IL_0004:  ldarg.0
    IL_0005:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_000a:  ldloc.0
    IL_000b:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0010:  tail.
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0017:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_001a

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.2
    IL_0014:  call       instance bool valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::Equals(valuetype '<>f__AnonymousType10002306269156`1'<!0>,
                                                                                                             class [runtime]System.Collections.IEqualityComparer)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_0009:  ldloc.0
    IL_000a:  ldfld      !0 valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::A@
    IL_000f:  tail.
    IL_0011:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_0019

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  call       instance bool valuetype '<>f__AnonymousType10002306269156`1'<!'<A>j__TPar'>::Equals(valuetype '<>f__AnonymousType10002306269156`1'<!0>)
    IL_0018:  ret

    IL_0019:  ldc.i4.0
    IL_001a:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType10002306269156`1'::get_A()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType10001789011089`2'<'<A>j__TPar','<B>j__TPar'>
       extends [runtime]System.ValueType
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private !'<B>j__TPar' B@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A, !'<B>j__TPar' B) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 22 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 31 30 30 30 31 37 38   
                                                                                                                                                   39 30 31 31 30 38 39 60 32 00 00 )                
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  stfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_000e:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<B>j__TPar' get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  ldobj      valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_001a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0,
             int32 V_1)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0008:  ldarg.0
    IL_0009:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000e:  ldloc.0
    IL_000f:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0014:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.0
    IL_001c:  bge.s      IL_0020

    IL_001e:  ldloc.1
    IL_001f:  ret

    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.0
    IL_0022:  ble.s      IL_0026

    IL_0024:  ldloc.1
    IL_0025:  ret

    IL_0026:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0031:  ldloc.0
    IL_0032:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0037:  tail.
    IL_0039:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_003e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  call       instance int32 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(valuetype '<>f__AnonymousType10001789011089`2'<!0,!1>)
    IL_000c:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  stloc.1
    IL_000a:  ldarg.2
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldloc.1
    IL_0012:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.0
    IL_001f:  bge.s      IL_0023

    IL_0021:  ldloc.2
    IL_0022:  ret

    IL_0023:  ldloc.2
    IL_0024:  ldc.i4.0
    IL_0025:  ble.s      IL_0029

    IL_0027:  ldloc.2
    IL_0028:  ret

    IL_0029:  ldarg.2
    IL_002a:  ldarg.0
    IL_002b:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0030:  ldloc.1
    IL_0031:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0036:  tail.
    IL_0038:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_003d:  ret
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
    IL_0009:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
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
    IL_001d:  ldc.i4     0x9e3779b9
    IL_0022:  ldarg.1
    IL_0023:  ldarg.0
    IL_0024:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0029:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.6
    IL_0030:  shl
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.2
    IL_0033:  shr
    IL_0034:  add
    IL_0035:  add
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  call       instance int32 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000b:  ret
  } 

  .method public hidebysig instance bool Equals(valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.2
    IL_0004:  ldarg.0
    IL_0005:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000a:  ldloc.0
    IL_000b:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0010:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0015:  brfalse.s  IL_002c

    IL_0017:  ldarg.2
    IL_0018:  ldarg.0
    IL_0019:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001e:  ldloc.0
    IL_001f:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0024:  tail.
    IL_0026:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_002b:  ret

    IL_002c:  ldc.i4.0
    IL_002d:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_001a

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.2
    IL_0014:  call       instance bool valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(valuetype '<>f__AnonymousType10001789011089`2'<!0,!1>,
                                                                                                                           class [runtime]System.Collections.IEqualityComparer)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0009:  ldloc.0
    IL_000a:  ldfld      !0 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0014:  brfalse.s  IL_002a

    IL_0016:  ldarg.0
    IL_0017:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001c:  ldloc.0
    IL_001d:  ldfld      !1 valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0022:  tail.
    IL_0024:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0029:  ret

    IL_002a:  ldc.i4.0
    IL_002b:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_0019

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  call       instance bool valuetype '<>f__AnonymousType10001789011089`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(valuetype '<>f__AnonymousType10001789011089`2'<!0,!1>)
    IL_0018:  ret

    IL_0019:  ldc.i4.0
    IL_001a:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType10001789011089`2'::get_A()
  } 
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<B>j__TPar' '<>f__AnonymousType10001789011089`2'::get_B()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType97009502`0'
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType97009502`0'>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType97009502`0'>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor() cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1C 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 39 37 30 30 39 35 30   
                                                                                                                                                   32 60 30 00 00 )                                  
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType97009502`0',string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType97009502`0'>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType97009502`0',string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType97009502`0',string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType97009502`0' obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000a

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  ldc.i4.0
    IL_0007:  ret

    IL_0008:  ldc.i4.1
    IL_0009:  ret

    IL_000a:  ldarg.1
    IL_000b:  brfalse.s  IL_000f

    IL_000d:  ldc.i4.m1
    IL_000e:  ret

    IL_000f:  ldc.i4.0
    IL_0010:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  '<>f__AnonymousType97009502`0'
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 '<>f__AnonymousType97009502`0'::CompareTo(class '<>f__AnonymousType97009502`0')
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  3
    .locals init (class '<>f__AnonymousType97009502`0' V_0,
             class '<>f__AnonymousType97009502`0' V_1)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  '<>f__AnonymousType97009502`0'
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_0018

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  '<>f__AnonymousType97009502`0'
    IL_0012:  brfalse.s  IL_0016

    IL_0014:  ldc.i4.0
    IL_0015:  ret

    IL_0016:  ldc.i4.1
    IL_0017:  ret

    IL_0018:  ldarg.1
    IL_0019:  unbox.any  '<>f__AnonymousType97009502`0'
    IL_001e:  brfalse.s  IL_0022

    IL_0020:  ldc.i4.m1
    IL_0021:  ret

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0007

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ret

    IL_0007:  ldc.i4.0
    IL_0008:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 '<>f__AnonymousType97009502`0'::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType97009502`0' obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType97009502`0' V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000c

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_000a

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ret

    IL_000a:  ldc.i4.0
    IL_000b:  ret

    IL_000c:  ldarg.1
    IL_000d:  ldnull
    IL_000e:  cgt.un
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType97009502`0' V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     '<>f__AnonymousType97009502`0'
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool '<>f__AnonymousType97009502`0'::Equals(class '<>f__AnonymousType97009502`0',
                                                                              class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType97009502`0' obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000a

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0008

    IL_0006:  ldc.i4.1
    IL_0007:  ret

    IL_0008:  ldc.i4.0
    IL_0009:  ret

    IL_000a:  ldarg.1
    IL_000b:  ldnull
    IL_000c:  cgt.un
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType97009502`0' V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     '<>f__AnonymousType97009502`0'
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool '<>f__AnonymousType97009502`0'::Equals(class '<>f__AnonymousType97009502`0')
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

} 






