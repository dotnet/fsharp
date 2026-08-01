




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
    .field assembly int32 B@
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

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 a, int32 b) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 2C 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 4E 6F 6D 69 6E 61 6C 5F 53 74 72 75 63 74   
                                                                                                                                                     6E 65 73 73 2B 52 65 66 4E 6F 6D 69 6E 61 6C 52   
                                                                                                                                                     65 63 64 00 00 )                                  
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/RefNominalRecd::A@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/RefNominalRecd::B@
      IL_0014:  ret
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
      .locals init (int32 V_0,
               class [runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0050

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_004e

      IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0019:  stloc.3
      IL_001a:  ldloc.2
      IL_001b:  ldloc.3
      IL_001c:  cgt
      IL_001e:  ldloc.2
      IL_001f:  ldloc.3
      IL_0020:  clt
      IL_0022:  sub
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldc.i4.0
      IL_0026:  bge.s      IL_002a

      IL_0028:  ldloc.0
      IL_0029:  ret

      IL_002a:  ldloc.0
      IL_002b:  ldc.i4.0
      IL_002c:  ble.s      IL_0030

      IL_002e:  ldloc.0
      IL_002f:  ret

      IL_0030:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0035:  stloc.1
      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_003c:  stloc.2
      IL_003d:  ldarg.1
      IL_003e:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_0043:  stloc.3
      IL_0044:  ldloc.2
      IL_0045:  ldloc.3
      IL_0046:  cgt
      IL_0048:  ldloc.2
      IL_0049:  ldloc.3
      IL_004a:  clt
      IL_004c:  sub
      IL_004d:  ret

      IL_004e:  ldc.i4.1
      IL_004f:  ret

      IL_0050:  ldarg.1
      IL_0051:  brfalse.s  IL_0055

      IL_0053:  ldc.i4.m1
      IL_0054:  ret

      IL_0055:  ldc.i4.0
      IL_0056:  ret
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
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/RefNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0050

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/RefNominalRecd
      IL_0010:  brfalse.s  IL_004e

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_001f:  stloc.3
      IL_0020:  ldloc.2
      IL_0021:  ldloc.3
      IL_0022:  cgt
      IL_0024:  ldloc.2
      IL_0025:  ldloc.3
      IL_0026:  clt
      IL_0028:  sub
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  ldc.i4.0
      IL_002c:  bge.s      IL_0030

      IL_002e:  ldloc.1
      IL_002f:  ret

      IL_0030:  ldloc.1
      IL_0031:  ldc.i4.0
      IL_0032:  ble.s      IL_0036

      IL_0034:  ldloc.1
      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_003c:  stloc.2
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_0043:  stloc.3
      IL_0044:  ldloc.2
      IL_0045:  ldloc.3
      IL_0046:  cgt
      IL_0048:  ldloc.2
      IL_0049:  ldloc.3
      IL_004a:  clt
      IL_004c:  sub
      IL_004d:  ret

      IL_004e:  ldc.i4.1
      IL_004f:  ret

      IL_0050:  ldarg.1
      IL_0051:  unbox.any  assembly/RefNominalRecd
      IL_0056:  brfalse.s  IL_005a

      IL_0058:  ldc.i4.m1
      IL_0059:  ret

      IL_005a:  ldc.i4.0
      IL_005b:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0031

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 assembly/RefNominalRecd::B@
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
      IL_001a:  ldc.i4     0x9e3779b9
      IL_001f:  ldarg.0
      IL_0020:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0025:  ldloc.0
      IL_0026:  ldc.i4.6
      IL_0027:  shl
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.2
      IL_002a:  shr
      IL_002b:  add
      IL_002c:  add
      IL_002d:  add
      IL_002e:  stloc.0
      IL_002f:  ldloc.0
      IL_0030:  ret

      IL_0031:  ldc.i4.0
      IL_0032:  ret
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
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_0020:  ceq
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret

      IL_0027:  ldarg.1
      IL_0028:  ldnull
      IL_0029:  cgt.un
      IL_002b:  ldc.i4.0
      IL_002c:  ceq
      IL_002e:  ret
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
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/RefNominalRecd::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/RefNominalRecd::B@
      IL_0020:  ceq
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret

      IL_0027:  ldarg.1
      IL_0028:  ldnull
      IL_0029:  cgt.un
      IL_002b:  ldc.i4.0
      IL_002c:  ceq
      IL_002e:  ret
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
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/RefNominalRecd::get_B()
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
    .field assembly int32 B@
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

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 a, int32 b) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 2F 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 4E 6F 6D 69 6E 61 6C 5F 53 74 72 75 63 74   
                                                                                                                                                     6E 65 73 73 2B 53 74 72 75 63 74 4E 6F 6D 69 6E   
                                                                                                                                                     61 6C 52 65 63 64 00 00 )                         
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/StructNominalRecd::A@
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/StructNominalRecd::B@
      IL_000e:  ret
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
      .locals init (int32 V_0,
               class [runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0005:  stloc.1
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000c:  stloc.2
      IL_000d:  ldarga.s   obj
      IL_000f:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_0032:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_0037:  stloc.2
      IL_0038:  ldarga.s   obj
      IL_003a:  ldfld      int32 assembly/StructNominalRecd::B@
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
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/StructNominalRecd
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000d:  stloc.2
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_002d:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_0032:  stloc.2
      IL_0033:  ldloca.s   V_0
      IL_0035:  ldfld      int32 assembly/StructNominalRecd::B@
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
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::B@
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
      IL_001d:  ldfld      int32 assembly/StructNominalRecd::A@
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
      IL_000d:  bne.un.s   IL_001f

      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_0015:  ldarga.s   obj
      IL_0017:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_001c:  ceq
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/StructNominalRecd V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/StructNominalRecd
      IL_0006:  brfalse.s  IL_0018

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/StructNominalRecd
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  ldarg.2
      IL_0012:  call       instance bool assembly/StructNominalRecd::Equals(valuetype assembly/StructNominalRecd,
                                                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0017:  ret

      IL_0018:  ldc.i4.0
      IL_0019:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/StructNominalRecd obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int32 assembly/StructNominalRecd::A@
      IL_000d:  bne.un.s   IL_001f

      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 assembly/StructNominalRecd::B@
      IL_0015:  ldarga.s   obj
      IL_0017:  ldfld      int32 assembly/StructNominalRecd::B@
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
      IL_0001:  isinst     assembly/StructNominalRecd
      IL_0006:  brfalse.s  IL_0015

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  unbox.any  assembly/StructNominalRecd
      IL_000f:  call       instance bool assembly/StructNominalRecd::Equals(valuetype assembly/StructNominalRecd)
      IL_0014:  ret

      IL_0015:  ldc.i4.0
      IL_0016:  ret
    } 

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/StructNominalRecd::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/StructNominalRecd::get_B()
    } 
  } 

  .field static assembly class '<>f__AnonymousType3545307392`2'<int32,int32> refAnonRecd@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> structAnonRecd@5
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd refNominalRecd@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd structNominalRecd@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd 'ref nominal src, ref nominal dst@9'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'ref nominal src, struct nominal dst@10'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd 'struct nominal src, ref nominal dst@11'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd copyOfStruct@11
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'copyOfStruct@11-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'struct nominal src, struct nominal dst@12'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'copyOfStruct@12-2'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'copyOfStruct@12-3'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd 'ref anon src, ref nominal dst@13'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'ref anon src, struct nominal dst@14'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/RefNominalRecd 'struct anon src, ref nominal dst@15'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'copyOfStruct@15-4'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'copyOfStruct@15-5'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype assembly/StructNominalRecd 'struct anon src, struct nominal dst@16'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'copyOfStruct@16-6'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'copyOfStruct@16-7'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class '<>f__AnonymousType3545307392`2'<int32,int32> get_refAnonRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType3545307392`2'<int32,int32> assembly::refAnonRecd@4
    IL_0005:  ret
  } 

  .method public specialname static valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> get_structAnonRecd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::structAnonRecd@5
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

  .method public specialname static class assembly/RefNominalRecd 'get_ref nominal src, ref nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/RefNominalRecd assembly::'ref nominal src, ref nominal dst@9'
    IL_0005:  ret
  } 

  .method public specialname static valuetype assembly/StructNominalRecd 'get_ref nominal src, struct nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'ref nominal src, struct nominal dst@10'
    IL_0005:  ret
  } 

  .method public specialname static class assembly/RefNominalRecd 'get_struct nominal src, ref nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/RefNominalRecd assembly::'struct nominal src, ref nominal dst@11'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype assembly/StructNominalRecd get_copyOfStruct@11() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::copyOfStruct@11
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype assembly/StructNominalRecd 'get_copyOfStruct@11-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@11-1'
    IL_0005:  ret
  } 

  .method public specialname static valuetype assembly/StructNominalRecd 'get_struct nominal src, struct nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'struct nominal src, struct nominal dst@12'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype assembly/StructNominalRecd 'get_copyOfStruct@12-2'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@12-2'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype assembly/StructNominalRecd 'get_copyOfStruct@12-3'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@12-3'
    IL_0005:  ret
  } 

  .method public specialname static class assembly/RefNominalRecd 'get_ref anon src, ref nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/RefNominalRecd assembly::'ref anon src, ref nominal dst@13'
    IL_0005:  ret
  } 

  .method public specialname static valuetype assembly/StructNominalRecd 'get_ref anon src, struct nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'ref anon src, struct nominal dst@14'
    IL_0005:  ret
  } 

  .method public specialname static class assembly/RefNominalRecd 'get_struct anon src, ref nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/RefNominalRecd assembly::'struct anon src, ref nominal dst@15'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'get_copyOfStruct@15-4'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@15-4'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'get_copyOfStruct@15-5'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@15-5'
    IL_0005:  ret
  } 

  .method public specialname static valuetype assembly/StructNominalRecd 'get_struct anon src, struct nominal dst'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype assembly/StructNominalRecd assembly::'struct anon src, struct nominal dst@16'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'get_copyOfStruct@16-6'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@16-6'
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> 'get_copyOfStruct@16-7'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@16-7'
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
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>& V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void class '<>f__AnonymousType3545307392`2'<int32,int32>::.ctor(!0,
                                                                                                  !1)
    IL_0007:  stsfld     class '<>f__AnonymousType3545307392`2'<int32,int32> assembly::refAnonRecd@4
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.2
    IL_000e:  newobj     instance void valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>::.ctor(!0,
                                                                                                         !1)
    IL_0013:  stsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::structAnonRecd@5
    IL_0018:  ldc.i4.1
    IL_0019:  ldc.i4.2
    IL_001a:  newobj     instance void assembly/RefNominalRecd::.ctor(int32,
                                                                                           int32)
    IL_001f:  stsfld     class assembly/RefNominalRecd assembly::refNominalRecd@6
    IL_0024:  ldc.i4.1
    IL_0025:  ldc.i4.2
    IL_0026:  newobj     instance void assembly/StructNominalRecd::.ctor(int32,
                                                                                              int32)
    IL_002b:  stsfld     valuetype assembly/StructNominalRecd assembly::structNominalRecd@7
    IL_0030:  call       class assembly/RefNominalRecd assembly::get_refNominalRecd()
    IL_0035:  ldfld      int32 assembly/RefNominalRecd::A@
    IL_003a:  ldc.i4.3
    IL_003b:  newobj     instance void assembly/RefNominalRecd::.ctor(int32,
                                                                                           int32)
    IL_0040:  stsfld     class assembly/RefNominalRecd assembly::'ref nominal src, ref nominal dst@9'
    IL_0045:  call       class assembly/RefNominalRecd assembly::get_refNominalRecd()
    IL_004a:  ldfld      int32 assembly/RefNominalRecd::A@
    IL_004f:  ldc.i4.3
    IL_0050:  newobj     instance void assembly/StructNominalRecd::.ctor(int32,
                                                                                              int32)
    IL_0055:  stsfld     valuetype assembly/StructNominalRecd assembly::'ref nominal src, struct nominal dst@10'
    IL_005a:  call       valuetype assembly/StructNominalRecd assembly::get_structNominalRecd()
    IL_005f:  stsfld     valuetype assembly/StructNominalRecd assembly::copyOfStruct@11
    IL_0064:  call       valuetype assembly/StructNominalRecd assembly::get_copyOfStruct@11()
    IL_0069:  stsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@11-1'
    IL_006e:  ldsflda    valuetype assembly/StructNominalRecd assembly::'copyOfStruct@11-1'
    IL_0073:  ldfld      int32 assembly/StructNominalRecd::A@
    IL_0078:  ldc.i4.3
    IL_0079:  newobj     instance void assembly/RefNominalRecd::.ctor(int32,
                                                                                           int32)
    IL_007e:  stsfld     class assembly/RefNominalRecd assembly::'struct nominal src, ref nominal dst@11'
    IL_0083:  call       valuetype assembly/StructNominalRecd assembly::get_structNominalRecd()
    IL_0088:  stsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@12-2'
    IL_008d:  call       valuetype assembly/StructNominalRecd assembly::'get_copyOfStruct@12-2'()
    IL_0092:  stsfld     valuetype assembly/StructNominalRecd assembly::'copyOfStruct@12-3'
    IL_0097:  ldsflda    valuetype assembly/StructNominalRecd assembly::'copyOfStruct@12-3'
    IL_009c:  ldfld      int32 assembly/StructNominalRecd::A@
    IL_00a1:  ldc.i4.3
    IL_00a2:  newobj     instance void assembly/StructNominalRecd::.ctor(int32,
                                                                                              int32)
    IL_00a7:  stsfld     valuetype assembly/StructNominalRecd assembly::'struct nominal src, struct nominal dst@12'
    IL_00ac:  call       class '<>f__AnonymousType3545307392`2'<int32,int32> assembly::get_refAnonRecd()
    IL_00b1:  call       instance !0 class '<>f__AnonymousType3545307392`2'<int32,int32>::get_A()
    IL_00b6:  ldc.i4.3
    IL_00b7:  newobj     instance void assembly/RefNominalRecd::.ctor(int32,
                                                                                           int32)
    IL_00bc:  stsfld     class assembly/RefNominalRecd assembly::'ref anon src, ref nominal dst@13'
    IL_00c1:  call       class '<>f__AnonymousType3545307392`2'<int32,int32> assembly::get_refAnonRecd()
    IL_00c6:  call       instance !0 class '<>f__AnonymousType3545307392`2'<int32,int32>::get_A()
    IL_00cb:  ldc.i4.3
    IL_00cc:  newobj     instance void assembly/StructNominalRecd::.ctor(int32,
                                                                                              int32)
    IL_00d1:  stsfld     valuetype assembly/StructNominalRecd assembly::'ref anon src, struct nominal dst@14'
    IL_00d6:  call       valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::get_structAnonRecd()
    IL_00db:  stsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@15-4'
    IL_00e0:  call       valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@15-4'()
    IL_00e5:  stsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@15-5'
    IL_00ea:  ldsflda    valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@15-5'
    IL_00ef:  stloc.0
    IL_00f0:  ldloca.s   V_0
    IL_00f2:  call       instance !0 valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>::get_A()
    IL_00f7:  ldc.i4.3
    IL_00f8:  newobj     instance void assembly/RefNominalRecd::.ctor(int32,
                                                                                           int32)
    IL_00fd:  stsfld     class assembly/RefNominalRecd assembly::'struct anon src, ref nominal dst@15'
    IL_0102:  call       valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::get_structAnonRecd()
    IL_0107:  stsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@16-6'
    IL_010c:  call       valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@16-6'()
    IL_0111:  stsfld     valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@16-7'
    IL_0116:  ldsflda    valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'copyOfStruct@16-7'
    IL_011b:  stloc.0
    IL_011c:  ldloca.s   V_0
    IL_011e:  call       instance !0 valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>::get_A()
    IL_0123:  ldc.i4.3
    IL_0124:  newobj     instance void assembly/StructNominalRecd::.ctor(int32,
                                                                                              int32)
    IL_0129:  stsfld     valuetype assembly/StructNominalRecd assembly::'struct anon src, struct nominal dst@16'
    IL_012e:  ret
  } 

  .property class '<>f__AnonymousType3545307392`2'<int32,int32>
          refAnonRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType3545307392`2'<int32,int32> assembly::get_refAnonRecd()
  } 
  .property valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>
          structAnonRecd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::get_structAnonRecd()
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
  .property class assembly/RefNominalRecd
          'ref nominal src, ref nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/RefNominalRecd assembly::'get_ref nominal src, ref nominal dst'()
  } 
  .property valuetype assembly/StructNominalRecd
          'ref nominal src, struct nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_ref nominal src, struct nominal dst'()
  } 
  .property class assembly/RefNominalRecd
          'struct nominal src, ref nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/RefNominalRecd assembly::'get_struct nominal src, ref nominal dst'()
  } 
  .property valuetype assembly/StructNominalRecd
          copyOfStruct@11()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::get_copyOfStruct@11()
  } 
  .property valuetype assembly/StructNominalRecd
          'copyOfStruct@11-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_copyOfStruct@11-1'()
  } 
  .property valuetype assembly/StructNominalRecd
          'struct nominal src, struct nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_struct nominal src, struct nominal dst'()
  } 
  .property valuetype assembly/StructNominalRecd
          'copyOfStruct@12-2'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_copyOfStruct@12-2'()
  } 
  .property valuetype assembly/StructNominalRecd
          'copyOfStruct@12-3'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_copyOfStruct@12-3'()
  } 
  .property class assembly/RefNominalRecd
          'ref anon src, ref nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/RefNominalRecd assembly::'get_ref anon src, ref nominal dst'()
  } 
  .property valuetype assembly/StructNominalRecd
          'ref anon src, struct nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_ref anon src, struct nominal dst'()
  } 
  .property class assembly/RefNominalRecd
          'struct anon src, ref nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/RefNominalRecd assembly::'get_struct anon src, ref nominal dst'()
  } 
  .property valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>
          'copyOfStruct@15-4'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@15-4'()
  } 
  .property valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>
          'copyOfStruct@15-5'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@15-5'()
  } 
  .property valuetype assembly/StructNominalRecd
          'struct anon src, struct nominal dst'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype assembly/StructNominalRecd assembly::'get_struct anon src, struct nominal dst'()
  } 
  .property valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>
          'copyOfStruct@16-6'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@16-6'()
  } 
  .property valuetype '<>f__AnonymousType1000930219981`2'<int32,int32>
          'copyOfStruct@16-7'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype '<>f__AnonymousType1000930219981`2'<int32,int32> assembly::'get_copyOfStruct@16-7'()
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

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType1000930219981`2'<'<A>j__TPar','<B>j__TPar'>
       extends [runtime]System.ValueType
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>
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
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 21 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 31 30 30 30 39 33 30   
                                                                                                                                                   32 31 39 39 38 31 60 32 00 00 )                   
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  stfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_000e:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<B>j__TPar' get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  ldobj      valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_001a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0,
             int32 V_1)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0008:  ldarg.0
    IL_0009:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000e:  ldloc.0
    IL_000f:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_002c:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0031:  ldloc.0
    IL_0032:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    IL_0002:  unbox.any  valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  call       instance int32 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(valuetype '<>f__AnonymousType1000930219981`2'<!0,!1>)
    IL_000c:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  stloc.1
    IL_000a:  ldarg.2
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldloc.1
    IL_0012:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_002b:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0030:  ldloc.1
    IL_0031:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    IL_0009:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    IL_0024:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_0006:  call       instance int32 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000b:  ret
  } 

  .method public hidebysig instance bool Equals(valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.2
    IL_0004:  ldarg.0
    IL_0005:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000a:  ldloc.0
    IL_000b:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0010:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0015:  brfalse.s  IL_002c

    IL_0017:  ldarg.2
    IL_0018:  ldarg.0
    IL_0019:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001e:  ldloc.0
    IL_001f:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_001a

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.2
    IL_0014:  call       instance bool valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(valuetype '<>f__AnonymousType1000930219981`2'<!0,!1>,
                                                                                                                          class [runtime]System.Collections.IEqualityComparer)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>& V_0)
    IL_0000:  ldarga.s   obj
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0009:  ldloc.0
    IL_000a:  ldfld      !0 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0014:  brfalse.s  IL_002a

    IL_0016:  ldarg.0
    IL_0017:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001c:  ldloc.0
    IL_001d:  ldfld      !1 valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    .locals init (valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_0019

    IL_000a:  ldarg.1
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>>(object)
    IL_0010:  stloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldloc.0
    IL_0013:  call       instance bool valuetype '<>f__AnonymousType1000930219981`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(valuetype '<>f__AnonymousType1000930219981`2'<!0,!1>)
    IL_0018:  ret

    IL_0019:  ldc.i4.0
    IL_001a:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType1000930219981`2'::get_A()
  } 
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<B>j__TPar' '<>f__AnonymousType1000930219981`2'::get_B()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType3545307392`2'<'<A>j__TPar','<B>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>>
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
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 33 35 34 35 33 30 37   
                                                                                                                                                   33 39 32 60 32 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<B>j__TPar' get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
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
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_002f:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0034:  ldarg.1
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    IL_0002:  unbox.any  class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(class '<>f__AnonymousType3545307392`2'<!0,!1>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_004a

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0012:  brfalse.s  IL_0048

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  ldloc.1
    IL_003b:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0040:  tail.
    IL_0042:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0047:  ret

    IL_0048:  ldc.i4.1
    IL_0049:  ret

    IL_004a:  ldarg.1
    IL_004b:  unbox.any  class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
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
    IL_000c:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    IL_0027:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
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
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0035

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0033

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0015:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_001a:  brfalse.s  IL_0031

    IL_001c:  ldarg.2
    IL_001d:  ldarg.0
    IL_001e:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0023:  ldloc.0
    IL_0024:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    .locals init (class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3545307392`2'<!0,!1>,
                                                                                                                   class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002f

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0017:  brfalse.s  IL_002d

    IL_0019:  ldarg.0
    IL_001a:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !1 class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
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
    .locals init (class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType3545307392`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3545307392`2'<!0,!1>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType3545307392`2'::get_A()
  } 
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<B>j__TPar' '<>f__AnonymousType3545307392`2'::get_B()
  } 
} 






