




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
  .class auto ansi serializable sealed nested public R1
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/R1>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/R1>,
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
      IL_0001:  ldfld      int32 assembly/R1::A@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R1::B@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 a, int32 b) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1F 54 79 70 65 5F 4E 6F 4F 76   
                                                                                                                                                     65 72 6C 61 70 5F 53 70 72 65 61 64 5F 53 70 72   
                                                                                                                                                     65 61 64 2B 52 31 00 00 )                         
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R1::A@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R1::B@
      IL_0014:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/R1>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R1,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/R1 obj) cil managed
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
      IL_000d:  ldfld      int32 assembly/R1::A@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/R1::A@
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
      IL_0037:  ldfld      int32 assembly/R1::B@
      IL_003c:  stloc.2
      IL_003d:  ldarg.1
      IL_003e:  ldfld      int32 assembly/R1::B@
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
      IL_0002:  unbox.any  assembly/R1
      IL_0007:  callvirt   instance int32 assembly/R1::CompareTo(class assembly/R1)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/R1 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R1
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0050

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R1
      IL_0010:  brfalse.s  IL_004e

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R1::A@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/R1::A@
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
      IL_0037:  ldfld      int32 assembly/R1::B@
      IL_003c:  stloc.2
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/R1::B@
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
      IL_0051:  unbox.any  assembly/R1
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
      IL_000b:  ldfld      int32 assembly/R1::B@
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
      IL_0020:  ldfld      int32 assembly/R1::A@
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
      IL_0006:  callvirt   instance int32 assembly/R1::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/R1 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R1::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R1::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R1::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R1::B@
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
      .locals init (class assembly/R1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/R1::Equals(class assembly/R1,
                                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/R1 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R1::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R1::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R1::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R1::B@
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
      .locals init (class assembly/R1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/R1::Equals(class assembly/R1)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R1::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/R1::get_B()
    } 
  } 

  .class auto ansi serializable sealed nested public R2
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/R2>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/R2>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly int32 C@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 D@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_C() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R2::C@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_D() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R2::D@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 c, int32 d) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1F 54 79 70 65 5F 4E 6F 4F 76   
                                                                                                                                                     65 72 6C 61 70 5F 53 70 72 65 61 64 5F 53 70 72   
                                                                                                                                                     65 61 64 2B 52 32 00 00 )                         
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R2::C@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R2::D@
      IL_0014:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R2,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/R2>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R2,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R2,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/R2 obj) cil managed
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
      IL_000d:  ldfld      int32 assembly/R2::C@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/R2::C@
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
      IL_0037:  ldfld      int32 assembly/R2::D@
      IL_003c:  stloc.2
      IL_003d:  ldarg.1
      IL_003e:  ldfld      int32 assembly/R2::D@
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
      IL_0002:  unbox.any  assembly/R2
      IL_0007:  callvirt   instance int32 assembly/R2::CompareTo(class assembly/R2)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/R2 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R2
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0050

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R2
      IL_0010:  brfalse.s  IL_004e

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R2::C@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/R2::C@
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
      IL_0037:  ldfld      int32 assembly/R2::D@
      IL_003c:  stloc.2
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/R2::D@
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
      IL_0051:  unbox.any  assembly/R2
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
      IL_000b:  ldfld      int32 assembly/R2::D@
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
      IL_0020:  ldfld      int32 assembly/R2::C@
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
      IL_0006:  callvirt   instance int32 assembly/R2::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/R2 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R2::C@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R2::C@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R2::D@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R2::D@
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
      .locals init (class assembly/R2 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R2
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/R2::Equals(class assembly/R2,
                                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/R2 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R2::C@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R2::C@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R2::D@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R2::D@
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
      .locals init (class assembly/R2 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R2
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/R2::Equals(class assembly/R2)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 C()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R2::get_C()
    } 
    .property instance int32 D()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/R2::get_D()
    } 
  } 

  .class auto ansi serializable sealed nested public R3
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/R3>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/R3>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit clo@3
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class assembly/R3 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class assembly/R3 obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class assembly/R3 this, class assembly/R3 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/R3 assembly/R3/clo@3::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class assembly/R3 assembly/R3/clo@3::obj
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.1
        IL_0006:  ldarg.0
        IL_0007:  ldfld      class assembly/R3 assembly/R3/clo@3::this
        IL_000c:  ldfld      int32 assembly/R3::A@
        IL_0011:  stloc.2
        IL_0012:  ldarg.0
        IL_0013:  ldfld      class assembly/R3 assembly/R3/clo@3::obj
        IL_0018:  ldfld      int32 assembly/R3::A@
        IL_001d:  stloc.3
        IL_001e:  ldloc.2
        IL_001f:  ldloc.3
        IL_0020:  cgt
        IL_0022:  ldloc.2
        IL_0023:  ldloc.3
        IL_0024:  clt
        IL_0026:  sub
        IL_0027:  stloc.0
        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.0
        IL_002a:  bge.s      IL_002e

        IL_002c:  ldloc.0
        IL_002d:  ret

        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.0
        IL_0030:  ble.s      IL_0034

        IL_0032:  ldloc.0
        IL_0033:  ret

        IL_0034:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0039:  stloc.1
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class assembly/R3 assembly/R3/clo@3::this
        IL_0040:  ldfld      int32 assembly/R3::B@
        IL_0045:  stloc.3
        IL_0046:  ldarg.0
        IL_0047:  ldfld      class assembly/R3 assembly/R3/clo@3::obj
        IL_004c:  ldfld      int32 assembly/R3::B@
        IL_0051:  stloc.s    V_4
        IL_0053:  ldloc.3
        IL_0054:  ldloc.s    V_4
        IL_0056:  cgt
        IL_0058:  ldloc.3
        IL_0059:  ldloc.s    V_4
        IL_005b:  clt
        IL_005d:  sub
        IL_005e:  stloc.2
        IL_005f:  ldloc.2
        IL_0060:  ldc.i4.0
        IL_0061:  bge.s      IL_0065

        IL_0063:  ldloc.2
        IL_0064:  ret

        IL_0065:  ldloc.2
        IL_0066:  ldc.i4.0
        IL_0067:  ble.s      IL_006b

        IL_0069:  ldloc.2
        IL_006a:  ret

        IL_006b:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0070:  stloc.1
        IL_0071:  ldarg.0
        IL_0072:  ldfld      class assembly/R3 assembly/R3/clo@3::this
        IL_0077:  ldfld      int32 assembly/R3::C@
        IL_007c:  stloc.s    V_4
        IL_007e:  ldarg.0
        IL_007f:  ldfld      class assembly/R3 assembly/R3/clo@3::obj
        IL_0084:  ldfld      int32 assembly/R3::C@
        IL_0089:  stloc.s    V_5
        IL_008b:  ldloc.s    V_4
        IL_008d:  ldloc.s    V_5
        IL_008f:  cgt
        IL_0091:  ldloc.s    V_4
        IL_0093:  ldloc.s    V_5
        IL_0095:  clt
        IL_0097:  sub
        IL_0098:  stloc.3
        IL_0099:  ldloc.3
        IL_009a:  ldc.i4.0
        IL_009b:  bge.s      IL_009f

        IL_009d:  ldloc.3
        IL_009e:  ret

        IL_009f:  ldloc.3
        IL_00a0:  ldc.i4.0
        IL_00a1:  ble.s      IL_00a5

        IL_00a3:  ldloc.3
        IL_00a4:  ret

        IL_00a5:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_00aa:  stloc.1
        IL_00ab:  ldarg.0
        IL_00ac:  ldfld      class assembly/R3 assembly/R3/clo@3::this
        IL_00b1:  ldfld      int32 assembly/R3::D@
        IL_00b6:  stloc.s    V_4
        IL_00b8:  ldarg.0
        IL_00b9:  ldfld      class assembly/R3 assembly/R3/clo@3::obj
        IL_00be:  ldfld      int32 assembly/R3::D@
        IL_00c3:  stloc.s    V_5
        IL_00c5:  ldloc.s    V_4
        IL_00c7:  ldloc.s    V_5
        IL_00c9:  cgt
        IL_00cb:  ldloc.s    V_4
        IL_00cd:  ldloc.s    V_5
        IL_00cf:  clt
        IL_00d1:  sub
        IL_00d2:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'clo@3-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class assembly/R3 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class assembly/R3 objTemp
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class assembly/R3 this, class assembly/R3 objTemp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/R3 assembly/R3/'clo@3-1'::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class assembly/R3 assembly/R3/'clo@3-1'::objTemp
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3,
                 int32 V_4)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::this
        IL_0006:  ldfld      int32 assembly/R3::A@
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::objTemp
        IL_0012:  ldfld      int32 assembly/R3::A@
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldloc.2
        IL_001a:  cgt
        IL_001c:  ldloc.1
        IL_001d:  ldloc.2
        IL_001e:  clt
        IL_0020:  sub
        IL_0021:  stloc.0
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.0
        IL_0024:  bge.s      IL_0028

        IL_0026:  ldloc.0
        IL_0027:  ret

        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.0
        IL_002a:  ble.s      IL_002e

        IL_002c:  ldloc.0
        IL_002d:  ret

        IL_002e:  ldarg.0
        IL_002f:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::this
        IL_0034:  ldfld      int32 assembly/R3::B@
        IL_0039:  stloc.2
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::objTemp
        IL_0040:  ldfld      int32 assembly/R3::B@
        IL_0045:  stloc.3
        IL_0046:  ldloc.2
        IL_0047:  ldloc.3
        IL_0048:  cgt
        IL_004a:  ldloc.2
        IL_004b:  ldloc.3
        IL_004c:  clt
        IL_004e:  sub
        IL_004f:  stloc.1
        IL_0050:  ldloc.1
        IL_0051:  ldc.i4.0
        IL_0052:  bge.s      IL_0056

        IL_0054:  ldloc.1
        IL_0055:  ret

        IL_0056:  ldloc.1
        IL_0057:  ldc.i4.0
        IL_0058:  ble.s      IL_005c

        IL_005a:  ldloc.1
        IL_005b:  ret

        IL_005c:  ldarg.0
        IL_005d:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::this
        IL_0062:  ldfld      int32 assembly/R3::C@
        IL_0067:  stloc.3
        IL_0068:  ldarg.0
        IL_0069:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::objTemp
        IL_006e:  ldfld      int32 assembly/R3::C@
        IL_0073:  stloc.s    V_4
        IL_0075:  ldloc.3
        IL_0076:  ldloc.s    V_4
        IL_0078:  cgt
        IL_007a:  ldloc.3
        IL_007b:  ldloc.s    V_4
        IL_007d:  clt
        IL_007f:  sub
        IL_0080:  stloc.2
        IL_0081:  ldloc.2
        IL_0082:  ldc.i4.0
        IL_0083:  bge.s      IL_0087

        IL_0085:  ldloc.2
        IL_0086:  ret

        IL_0087:  ldloc.2
        IL_0088:  ldc.i4.0
        IL_0089:  ble.s      IL_008d

        IL_008b:  ldloc.2
        IL_008c:  ret

        IL_008d:  ldarg.0
        IL_008e:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::this
        IL_0093:  ldfld      int32 assembly/R3::D@
        IL_0098:  stloc.3
        IL_0099:  ldarg.0
        IL_009a:  ldfld      class assembly/R3 assembly/R3/'clo@3-1'::objTemp
        IL_009f:  ldfld      int32 assembly/R3::D@
        IL_00a4:  stloc.s    V_4
        IL_00a6:  ldloc.3
        IL_00a7:  ldloc.s    V_4
        IL_00a9:  cgt
        IL_00ab:  ldloc.3
        IL_00ac:  ldloc.s    V_4
        IL_00ae:  clt
        IL_00b0:  sub
        IL_00b1:  ret
      } 

    } 

    .field assembly int32 A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 B@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 C@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 D@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R3::A@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R3::B@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_C() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R3::C@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_D() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R3::D@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(int32 a,
                                 int32 b,
                                 int32 c,
                                 int32 d) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1F 54 79 70 65 5F 4E 6F 4F 76   
                                                                                                                                                     65 72 6C 61 70 5F 53 70 72 65 61 64 5F 53 70 72   
                                                                                                                                                     65 61 64 2B 52 33 00 00 )                         
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R3::A@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R3::B@
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      int32 assembly/R3::C@
      IL_001b:  ldarg.0
      IL_001c:  ldarg.s    d
      IL_001e:  stfld      int32 assembly/R3::D@
      IL_0023:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R3,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/R3>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R3,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R3,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/R3 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void assembly/R3/clo@3::.ctor(class assembly/R3,
                                                                                      class assembly/R3)
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldnull
      IL_0010:  tail.
      IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0017:  ret

      IL_0018:  ldc.i4.1
      IL_0019:  ret

      IL_001a:  ldarg.1
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  ldc.i4.m1
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/R3
      IL_0007:  callvirt   instance int32 assembly/R3::CompareTo(class assembly/R3)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/R3 V_0,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_1)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R3
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0026

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R3
      IL_0010:  brfalse.s  IL_0024

      IL_0012:  ldarg.0
      IL_0013:  ldloc.0
      IL_0014:  newobj     instance void assembly/R3/'clo@3-1'::.ctor(class assembly/R3,
                                                                                          class assembly/R3)
      IL_0019:  stloc.1
      IL_001a:  ldloc.1
      IL_001b:  ldnull
      IL_001c:  tail.
      IL_001e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  unbox.any  assembly/R3
      IL_002c:  brfalse.s  IL_0030

      IL_002e:  ldc.i4.m1
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_005b

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 assembly/R3::D@
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
      IL_0020:  ldfld      int32 assembly/R3::C@
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
      IL_002f:  ldc.i4     0x9e3779b9
      IL_0034:  ldarg.0
      IL_0035:  ldfld      int32 assembly/R3::B@
      IL_003a:  ldloc.0
      IL_003b:  ldc.i4.6
      IL_003c:  shl
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.2
      IL_003f:  shr
      IL_0040:  add
      IL_0041:  add
      IL_0042:  add
      IL_0043:  stloc.0
      IL_0044:  ldc.i4     0x9e3779b9
      IL_0049:  ldarg.0
      IL_004a:  ldfld      int32 assembly/R3::A@
      IL_004f:  ldloc.0
      IL_0050:  ldc.i4.6
      IL_0051:  shl
      IL_0052:  ldloc.0
      IL_0053:  ldc.i4.2
      IL_0054:  shr
      IL_0055:  add
      IL_0056:  add
      IL_0057:  add
      IL_0058:  stloc.0
      IL_0059:  ldloc.0
      IL_005a:  ret

      IL_005b:  ldc.i4.0
      IL_005c:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/R3::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/R3 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0047

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0045

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R3::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R3::A@
      IL_0012:  bne.un.s   IL_0043

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R3::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R3::B@
      IL_0020:  bne.un.s   IL_0041

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R3::C@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R3::C@
      IL_002e:  bne.un.s   IL_003f

      IL_0030:  ldarg.0
      IL_0031:  ldfld      int32 assembly/R3::D@
      IL_0036:  ldarg.1
      IL_0037:  ldfld      int32 assembly/R3::D@
      IL_003c:  ceq
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret

      IL_0041:  ldc.i4.0
      IL_0042:  ret

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret

      IL_0047:  ldarg.1
      IL_0048:  ldnull
      IL_0049:  cgt.un
      IL_004b:  ldc.i4.0
      IL_004c:  ceq
      IL_004e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/R3 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R3
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/R3::Equals(class assembly/R3,
                                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/R3 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0047

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0045

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R3::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R3::A@
      IL_0012:  bne.un.s   IL_0043

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R3::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R3::B@
      IL_0020:  bne.un.s   IL_0041

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R3::C@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R3::C@
      IL_002e:  bne.un.s   IL_003f

      IL_0030:  ldarg.0
      IL_0031:  ldfld      int32 assembly/R3::D@
      IL_0036:  ldarg.1
      IL_0037:  ldfld      int32 assembly/R3::D@
      IL_003c:  ceq
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret

      IL_0041:  ldc.i4.0
      IL_0042:  ret

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret

      IL_0047:  ldarg.1
      IL_0048:  ldnull
      IL_0049:  cgt.un
      IL_004b:  ldc.i4.0
      IL_004c:  ceq
      IL_004e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/R3 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R3
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/R3::Equals(class assembly/R3)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R3::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/R3::get_B()
    } 
    .property instance int32 C()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 ) 
      .get instance int32 assembly/R3::get_C()
    } 
    .property instance int32 D()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 ) 
      .get instance int32 assembly/R3::get_D()
    } 
  } 

  .class auto ansi serializable sealed nested public R4
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/R4>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/R4>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit 'clo@4-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class assembly/R4 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class assembly/R4 obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class assembly/R4 this, class assembly/R4 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/R4 assembly/R4/'clo@4-2'::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class assembly/R4 assembly/R4/'clo@4-2'::obj
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 int32 V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.1
        IL_0006:  ldarg.0
        IL_0007:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::this
        IL_000c:  ldfld      int32 assembly/R4::C@
        IL_0011:  stloc.2
        IL_0012:  ldarg.0
        IL_0013:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::obj
        IL_0018:  ldfld      int32 assembly/R4::C@
        IL_001d:  stloc.3
        IL_001e:  ldloc.2
        IL_001f:  ldloc.3
        IL_0020:  cgt
        IL_0022:  ldloc.2
        IL_0023:  ldloc.3
        IL_0024:  clt
        IL_0026:  sub
        IL_0027:  stloc.0
        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.0
        IL_002a:  bge.s      IL_002e

        IL_002c:  ldloc.0
        IL_002d:  ret

        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.0
        IL_0030:  ble.s      IL_0034

        IL_0032:  ldloc.0
        IL_0033:  ret

        IL_0034:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0039:  stloc.1
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::this
        IL_0040:  ldfld      int32 assembly/R4::D@
        IL_0045:  stloc.3
        IL_0046:  ldarg.0
        IL_0047:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::obj
        IL_004c:  ldfld      int32 assembly/R4::D@
        IL_0051:  stloc.s    V_4
        IL_0053:  ldloc.3
        IL_0054:  ldloc.s    V_4
        IL_0056:  cgt
        IL_0058:  ldloc.3
        IL_0059:  ldloc.s    V_4
        IL_005b:  clt
        IL_005d:  sub
        IL_005e:  stloc.2
        IL_005f:  ldloc.2
        IL_0060:  ldc.i4.0
        IL_0061:  bge.s      IL_0065

        IL_0063:  ldloc.2
        IL_0064:  ret

        IL_0065:  ldloc.2
        IL_0066:  ldc.i4.0
        IL_0067:  ble.s      IL_006b

        IL_0069:  ldloc.2
        IL_006a:  ret

        IL_006b:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0070:  stloc.1
        IL_0071:  ldarg.0
        IL_0072:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::this
        IL_0077:  ldfld      int32 assembly/R4::A@
        IL_007c:  stloc.s    V_4
        IL_007e:  ldarg.0
        IL_007f:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::obj
        IL_0084:  ldfld      int32 assembly/R4::A@
        IL_0089:  stloc.s    V_5
        IL_008b:  ldloc.s    V_4
        IL_008d:  ldloc.s    V_5
        IL_008f:  cgt
        IL_0091:  ldloc.s    V_4
        IL_0093:  ldloc.s    V_5
        IL_0095:  clt
        IL_0097:  sub
        IL_0098:  stloc.3
        IL_0099:  ldloc.3
        IL_009a:  ldc.i4.0
        IL_009b:  bge.s      IL_009f

        IL_009d:  ldloc.3
        IL_009e:  ret

        IL_009f:  ldloc.3
        IL_00a0:  ldc.i4.0
        IL_00a1:  ble.s      IL_00a5

        IL_00a3:  ldloc.3
        IL_00a4:  ret

        IL_00a5:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_00aa:  stloc.1
        IL_00ab:  ldarg.0
        IL_00ac:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::this
        IL_00b1:  ldfld      int32 assembly/R4::B@
        IL_00b6:  stloc.s    V_4
        IL_00b8:  ldarg.0
        IL_00b9:  ldfld      class assembly/R4 assembly/R4/'clo@4-2'::obj
        IL_00be:  ldfld      int32 assembly/R4::B@
        IL_00c3:  stloc.s    V_5
        IL_00c5:  ldloc.s    V_4
        IL_00c7:  ldloc.s    V_5
        IL_00c9:  cgt
        IL_00cb:  ldloc.s    V_4
        IL_00cd:  ldloc.s    V_5
        IL_00cf:  clt
        IL_00d1:  sub
        IL_00d2:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'clo@4-3'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class assembly/R4 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class assembly/R4 objTemp
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class assembly/R4 this, class assembly/R4 objTemp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/R4 assembly/R4/'clo@4-3'::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class assembly/R4 assembly/R4/'clo@4-3'::objTemp
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3,
                 int32 V_4)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::this
        IL_0006:  ldfld      int32 assembly/R4::C@
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::objTemp
        IL_0012:  ldfld      int32 assembly/R4::C@
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldloc.2
        IL_001a:  cgt
        IL_001c:  ldloc.1
        IL_001d:  ldloc.2
        IL_001e:  clt
        IL_0020:  sub
        IL_0021:  stloc.0
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.0
        IL_0024:  bge.s      IL_0028

        IL_0026:  ldloc.0
        IL_0027:  ret

        IL_0028:  ldloc.0
        IL_0029:  ldc.i4.0
        IL_002a:  ble.s      IL_002e

        IL_002c:  ldloc.0
        IL_002d:  ret

        IL_002e:  ldarg.0
        IL_002f:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::this
        IL_0034:  ldfld      int32 assembly/R4::D@
        IL_0039:  stloc.2
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::objTemp
        IL_0040:  ldfld      int32 assembly/R4::D@
        IL_0045:  stloc.3
        IL_0046:  ldloc.2
        IL_0047:  ldloc.3
        IL_0048:  cgt
        IL_004a:  ldloc.2
        IL_004b:  ldloc.3
        IL_004c:  clt
        IL_004e:  sub
        IL_004f:  stloc.1
        IL_0050:  ldloc.1
        IL_0051:  ldc.i4.0
        IL_0052:  bge.s      IL_0056

        IL_0054:  ldloc.1
        IL_0055:  ret

        IL_0056:  ldloc.1
        IL_0057:  ldc.i4.0
        IL_0058:  ble.s      IL_005c

        IL_005a:  ldloc.1
        IL_005b:  ret

        IL_005c:  ldarg.0
        IL_005d:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::this
        IL_0062:  ldfld      int32 assembly/R4::A@
        IL_0067:  stloc.3
        IL_0068:  ldarg.0
        IL_0069:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::objTemp
        IL_006e:  ldfld      int32 assembly/R4::A@
        IL_0073:  stloc.s    V_4
        IL_0075:  ldloc.3
        IL_0076:  ldloc.s    V_4
        IL_0078:  cgt
        IL_007a:  ldloc.3
        IL_007b:  ldloc.s    V_4
        IL_007d:  clt
        IL_007f:  sub
        IL_0080:  stloc.2
        IL_0081:  ldloc.2
        IL_0082:  ldc.i4.0
        IL_0083:  bge.s      IL_0087

        IL_0085:  ldloc.2
        IL_0086:  ret

        IL_0087:  ldloc.2
        IL_0088:  ldc.i4.0
        IL_0089:  ble.s      IL_008d

        IL_008b:  ldloc.2
        IL_008c:  ret

        IL_008d:  ldarg.0
        IL_008e:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::this
        IL_0093:  ldfld      int32 assembly/R4::B@
        IL_0098:  stloc.3
        IL_0099:  ldarg.0
        IL_009a:  ldfld      class assembly/R4 assembly/R4/'clo@4-3'::objTemp
        IL_009f:  ldfld      int32 assembly/R4::B@
        IL_00a4:  stloc.s    V_4
        IL_00a6:  ldloc.3
        IL_00a7:  ldloc.s    V_4
        IL_00a9:  cgt
        IL_00ab:  ldloc.3
        IL_00ac:  ldloc.s    V_4
        IL_00ae:  clt
        IL_00b0:  sub
        IL_00b1:  ret
      } 

    } 

    .field assembly int32 C@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 D@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly int32 B@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_C() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R4::C@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_D() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R4::D@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R4::A@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R4::B@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(int32 c,
                                 int32 d,
                                 int32 a,
                                 int32 b) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1F 54 79 70 65 5F 4E 6F 4F 76   
                                                                                                                                                     65 72 6C 61 70 5F 53 70 72 65 61 64 5F 53 70 72   
                                                                                                                                                     65 61 64 2B 52 34 00 00 )                         
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R4::C@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R4::D@
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      int32 assembly/R4::A@
      IL_001b:  ldarg.0
      IL_001c:  ldarg.s    b
      IL_001e:  stfld      int32 assembly/R4::B@
      IL_0023:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R4,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/R4>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R4,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/R4,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/R4 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void assembly/R4/'clo@4-2'::.ctor(class assembly/R4,
                                                                                          class assembly/R4)
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldnull
      IL_0010:  tail.
      IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0017:  ret

      IL_0018:  ldc.i4.1
      IL_0019:  ret

      IL_001a:  ldarg.1
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  ldc.i4.m1
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/R4
      IL_0007:  callvirt   instance int32 assembly/R4::CompareTo(class assembly/R4)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/R4 V_0,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_1)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R4
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0026

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R4
      IL_0010:  brfalse.s  IL_0024

      IL_0012:  ldarg.0
      IL_0013:  ldloc.0
      IL_0014:  newobj     instance void assembly/R4/'clo@4-3'::.ctor(class assembly/R4,
                                                                                          class assembly/R4)
      IL_0019:  stloc.1
      IL_001a:  ldloc.1
      IL_001b:  ldnull
      IL_001c:  tail.
      IL_001e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  unbox.any  assembly/R4
      IL_002c:  brfalse.s  IL_0030

      IL_002e:  ldc.i4.m1
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_005b

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 assembly/R4::B@
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
      IL_0020:  ldfld      int32 assembly/R4::A@
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
      IL_002f:  ldc.i4     0x9e3779b9
      IL_0034:  ldarg.0
      IL_0035:  ldfld      int32 assembly/R4::D@
      IL_003a:  ldloc.0
      IL_003b:  ldc.i4.6
      IL_003c:  shl
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.2
      IL_003f:  shr
      IL_0040:  add
      IL_0041:  add
      IL_0042:  add
      IL_0043:  stloc.0
      IL_0044:  ldc.i4     0x9e3779b9
      IL_0049:  ldarg.0
      IL_004a:  ldfld      int32 assembly/R4::C@
      IL_004f:  ldloc.0
      IL_0050:  ldc.i4.6
      IL_0051:  shl
      IL_0052:  ldloc.0
      IL_0053:  ldc.i4.2
      IL_0054:  shr
      IL_0055:  add
      IL_0056:  add
      IL_0057:  add
      IL_0058:  stloc.0
      IL_0059:  ldloc.0
      IL_005a:  ret

      IL_005b:  ldc.i4.0
      IL_005c:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/R4::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/R4 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0047

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0045

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R4::C@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R4::C@
      IL_0012:  bne.un.s   IL_0043

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R4::D@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R4::D@
      IL_0020:  bne.un.s   IL_0041

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R4::A@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R4::A@
      IL_002e:  bne.un.s   IL_003f

      IL_0030:  ldarg.0
      IL_0031:  ldfld      int32 assembly/R4::B@
      IL_0036:  ldarg.1
      IL_0037:  ldfld      int32 assembly/R4::B@
      IL_003c:  ceq
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret

      IL_0041:  ldc.i4.0
      IL_0042:  ret

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret

      IL_0047:  ldarg.1
      IL_0048:  ldnull
      IL_0049:  cgt.un
      IL_004b:  ldc.i4.0
      IL_004c:  ceq
      IL_004e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/R4 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R4
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/R4::Equals(class assembly/R4,
                                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/R4 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0047

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0045

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R4::C@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R4::C@
      IL_0012:  bne.un.s   IL_0043

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R4::D@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R4::D@
      IL_0020:  bne.un.s   IL_0041

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R4::A@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R4::A@
      IL_002e:  bne.un.s   IL_003f

      IL_0030:  ldarg.0
      IL_0031:  ldfld      int32 assembly/R4::B@
      IL_0036:  ldarg.1
      IL_0037:  ldfld      int32 assembly/R4::B@
      IL_003c:  ceq
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret

      IL_0041:  ldc.i4.0
      IL_0042:  ret

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret

      IL_0047:  ldarg.1
      IL_0048:  ldnull
      IL_0049:  cgt.un
      IL_004b:  ldc.i4.0
      IL_004c:  ceq
      IL_004e:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/R4 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/R4
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/R4::Equals(class assembly/R4)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 C()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R4::get_C()
    } 
    .property instance int32 D()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/R4::get_D()
    } 
    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 ) 
      .get instance int32 assembly/R4::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 ) 
      .get instance int32 assembly/R4::get_B()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






