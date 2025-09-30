




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1B 54 79 70 65 5F 53 70 72 65   
                                                                                                                               61 64 53 68 61 64 6F 77 73 53 70 72 65 61 64 2B   
                                                                                                                               52 31 00 00 )                                     
      
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
    .field assembly string A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance string  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/R2::A@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(string a) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1B 54 79 70 65 5F 53 70 72 65   
                                                                                                                               61 64 53 68 61 64 6F 77 73 53 70 72 65 61 64 2B   
                                                                                                                               52 32 00 00 )                                     
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string assembly/R2::A@
      IL_000d:  ret
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
      
      .maxstack  4
      .locals init (class [runtime]System.Collections.IComparer V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0020

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001e

      IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      string assembly/R2::A@
      IL_0012:  ldarg.1
      IL_0013:  ldfld      string assembly/R2::A@
      IL_0018:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_001d:  ret

      IL_001e:  ldc.i4.1
      IL_001f:  ret

      IL_0020:  ldarg.1
      IL_0021:  brfalse.s  IL_0025

      IL_0023:  ldc.i4.m1
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
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
      
      .maxstack  4
      .locals init (class assembly/R2 V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R2
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0026

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R2
      IL_0010:  brfalse.s  IL_0024

      IL_0012:  ldarg.0
      IL_0013:  ldfld      string assembly/R2::A@
      IL_0018:  ldloc.0
      IL_0019:  ldfld      string assembly/R2::A@
      IL_001e:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  unbox.any  assembly/R2
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
      .locals init (int32 V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002b

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      string assembly/R2::A@
      IL_0010:  stloc.1
      IL_0011:  ldloc.1
      IL_0012:  brtrue.s   IL_0018

      IL_0014:  ldc.i4.0
      IL_0015:  nop
      IL_0016:  br.s       IL_001f

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
      IL_001e:  nop
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.6
      IL_0021:  shl
      IL_0022:  ldloc.0
      IL_0023:  ldc.i4.2
      IL_0024:  shr
      IL_0025:  add
      IL_0026:  add
      IL_0027:  add
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret

      IL_002b:  ldc.i4.0
      IL_002c:  ret
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
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldfld      string assembly/R2::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      string assembly/R2::A@
      IL_0012:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0017:  ret

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldarg.1
      IL_001b:  ldnull
      IL_001c:  cgt.un
      IL_001e:  ldc.i4.0
      IL_001f:  ceq
      IL_0021:  ret
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
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldfld      string assembly/R2::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      string assembly/R2::A@
      IL_0012:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0017:  ret

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldarg.1
      IL_001b:  ldnull
      IL_001c:  cgt.un
      IL_001e:  ldc.i4.0
      IL_001f:  ceq
      IL_0021:  ret
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

    .property instance string A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string assembly/R2::get_A()
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
    .field assembly int32 B@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly string A@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R3::B@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance string  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/R3::A@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 b, string a) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1B 54 79 70 65 5F 53 70 72 65   
                                                                                                                               61 64 53 68 61 64 6F 77 73 53 70 72 65 61 64 2B   
                                                                                                                               52 33 00 00 )                                     
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R3::B@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      string assembly/R3::A@
      IL_0014:  ret
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
      
      .maxstack  5
      .locals init (int32 V_0,
               class [runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_004a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0048

      IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 assembly/R3::B@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/R3::B@
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
      IL_0037:  ldfld      string assembly/R3::A@
      IL_003c:  ldarg.1
      IL_003d:  ldfld      string assembly/R3::A@
      IL_0042:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0047:  ret

      IL_0048:  ldc.i4.1
      IL_0049:  ret

      IL_004a:  ldarg.1
      IL_004b:  brfalse.s  IL_004f

      IL_004d:  ldc.i4.m1
      IL_004e:  ret

      IL_004f:  ldc.i4.0
      IL_0050:  ret
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
      
      .maxstack  5
      .locals init (class assembly/R3 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R3
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_004a

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R3
      IL_0010:  brfalse.s  IL_0048

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R3::B@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/R3::B@
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
      IL_0037:  ldfld      string assembly/R3::A@
      IL_003c:  ldloc.0
      IL_003d:  ldfld      string assembly/R3::A@
      IL_0042:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0047:  ret

      IL_0048:  ldc.i4.1
      IL_0049:  ret

      IL_004a:  ldarg.1
      IL_004b:  unbox.any  assembly/R3
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
      .locals init (int32 V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0040

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      string assembly/R3::A@
      IL_0010:  stloc.1
      IL_0011:  ldloc.1
      IL_0012:  brtrue.s   IL_0018

      IL_0014:  ldc.i4.0
      IL_0015:  nop
      IL_0016:  br.s       IL_001f

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
      IL_001e:  nop
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.6
      IL_0021:  shl
      IL_0022:  ldloc.0
      IL_0023:  ldc.i4.2
      IL_0024:  shr
      IL_0025:  add
      IL_0026:  add
      IL_0027:  add
      IL_0028:  stloc.0
      IL_0029:  ldc.i4     0x9e3779b9
      IL_002e:  ldarg.0
      IL_002f:  ldfld      int32 assembly/R3::B@
      IL_0034:  ldloc.0
      IL_0035:  ldc.i4.6
      IL_0036:  shl
      IL_0037:  ldloc.0
      IL_0038:  ldc.i4.2
      IL_0039:  shr
      IL_003a:  add
      IL_003b:  add
      IL_003c:  add
      IL_003d:  stloc.0
      IL_003e:  ldloc.0
      IL_003f:  ret

      IL_0040:  ldc.i4.0
      IL_0041:  ret
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
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0028

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R3::B@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R3::B@
      IL_0012:  bne.un.s   IL_0026

      IL_0014:  ldarg.0
      IL_0015:  ldfld      string assembly/R3::A@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      string assembly/R3::A@
      IL_0020:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0025:  ret

      IL_0026:  ldc.i4.0
      IL_0027:  ret

      IL_0028:  ldc.i4.0
      IL_0029:  ret

      IL_002a:  ldarg.1
      IL_002b:  ldnull
      IL_002c:  cgt.un
      IL_002e:  ldc.i4.0
      IL_002f:  ceq
      IL_0031:  ret
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
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0028

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R3::B@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R3::B@
      IL_0012:  bne.un.s   IL_0026

      IL_0014:  ldarg.0
      IL_0015:  ldfld      string assembly/R3::A@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      string assembly/R3::A@
      IL_0020:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0025:  ret

      IL_0026:  ldc.i4.0
      IL_0027:  ret

      IL_0028:  ldc.i4.0
      IL_0029:  ret

      IL_002a:  ldarg.1
      IL_002b:  ldnull
      IL_002c:  cgt.un
      IL_002e:  ldc.i4.0
      IL_002f:  ceq
      IL_0031:  ret
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

    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R3::get_B()
    } 
    .property instance string A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance string assembly/R3::get_A()
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

    .method public specialname rtspecialname instance void  .ctor(int32 a, int32 b) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 1B 54 79 70 65 5F 53 70 72 65   
                                                                                                                               61 64 53 68 61 64 6F 77 73 53 70 72 65 61 64 2B   
                                                                                                                               52 34 00 00 )                                     
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R4::A@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R4::B@
      IL_0014:  ret
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
      IL_000d:  ldfld      int32 assembly/R4::A@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/R4::A@
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
      IL_0037:  ldfld      int32 assembly/R4::B@
      IL_003c:  stloc.2
      IL_003d:  ldarg.1
      IL_003e:  ldfld      int32 assembly/R4::B@
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
      IL_0002:  unbox.any  assembly/R4
      IL_0007:  callvirt   instance int32 assembly/R4::CompareTo(class assembly/R4)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/R4 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R4
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0050

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R4
      IL_0010:  brfalse.s  IL_004e

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R4::A@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/R4::A@
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
      IL_0037:  ldfld      int32 assembly/R4::B@
      IL_003c:  stloc.2
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/R4::B@
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
      IL_0051:  unbox.any  assembly/R4
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
      IL_0006:  callvirt   instance int32 assembly/R4::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/R4 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R4::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R4::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R4::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R4::B@
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
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R4::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R4::A@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R4::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R4::B@
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

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R4::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
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






