




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





.class public abstract auto ansi sealed M
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public beforefieldinit RecordLevelAttribute`1<T>
         extends [runtime]System.Attribute
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Attribute::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable nested public beforefieldinit FieldLevelAttribute`1<T>
         extends [runtime]System.Attribute
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Attribute::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested public Test
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class M/Test>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class M/Test>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void class M/RecordLevelAttribute`1<int32>::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field assembly string someField@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance string  get_someField() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string M/Test::someField@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(string someField) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 06 4D 2B 54 65 73 74 00 00 )    
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string M/Test::someField@
      IL_000d:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/Test,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class M/Test>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/Test,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/Test,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class M/Test obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [runtime]System.Collections.IComparer V_0,
               string V_1,
               string V_2,
               class [runtime]System.Collections.IComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0026

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0024

      IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000b:  stloc.0
      IL_000c:  ldarg.0
      IL_000d:  ldfld      string M/Test::someField@
      IL_0012:  stloc.1
      IL_0013:  ldarg.1
      IL_0014:  ldfld      string M/Test::someField@
      IL_0019:  stloc.2
      IL_001a:  ldloc.0
      IL_001b:  stloc.3
      IL_001c:  ldloc.1
      IL_001d:  ldloc.2
      IL_001e:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
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
      IL_0002:  unbox.any  M/Test
      IL_0007:  callvirt   instance int32 M/Test::CompareTo(class M/Test)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class M/Test V_0,
               class M/Test V_1,
               class [runtime]System.Collections.IComparer V_2,
               string V_3,
               string V_4,
               class [runtime]System.Collections.IComparer V_5)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  M/Test
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  brfalse.s  IL_0033

      IL_000c:  ldarg.1
      IL_000d:  unbox.any  M/Test
      IL_0012:  brfalse.s  IL_0031

      IL_0014:  ldarg.2
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldfld      string M/Test::someField@
      IL_001c:  stloc.3
      IL_001d:  ldloc.1
      IL_001e:  ldfld      string M/Test::someField@
      IL_0023:  stloc.s    V_4
      IL_0025:  ldloc.2
      IL_0026:  stloc.s    V_5
      IL_0028:  ldloc.3
      IL_0029:  ldloc.s    V_4
      IL_002b:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0030:  ret

      IL_0031:  ldc.i4.1
      IL_0032:  ret

      IL_0033:  ldarg.1
      IL_0034:  unbox.any  M/Test
      IL_0039:  brfalse.s  IL_003d

      IL_003b:  ldc.i4.m1
      IL_003c:  ret

      IL_003d:  ldc.i4.0
      IL_003e:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class [runtime]System.Collections.IEqualityComparer V_1,
               string V_2,
               class [runtime]System.Collections.IEqualityComparer V_3,
               string V_4,
               string V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0038

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      string M/Test::someField@
      IL_0012:  stloc.2
      IL_0013:  ldloc.1
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  stloc.s    V_4
      IL_0018:  ldloc.s    V_4
      IL_001a:  stloc.s    V_5
      IL_001c:  ldloc.s    V_5
      IL_001e:  brtrue.s   IL_0024

      IL_0020:  ldc.i4.0
      IL_0021:  nop
      IL_0022:  br.s       IL_002c

      IL_0024:  ldloc.s    V_5
      IL_0026:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
      IL_002b:  nop
      IL_002c:  ldloc.0
      IL_002d:  ldc.i4.6
      IL_002e:  shl
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.2
      IL_0031:  shr
      IL_0032:  add
      IL_0033:  add
      IL_0034:  add
      IL_0035:  stloc.0
      IL_0036:  ldloc.0
      IL_0037:  ret

      IL_0038:  ldc.i4.0
      IL_0039:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 M/Test::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool 
            Equals(class M/Test obj,
                   class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class M/Test V_0,
               class [runtime]System.Collections.IEqualityComparer V_1,
               string V_2,
               string V_3,
               class [runtime]System.Collections.IEqualityComparer V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0025

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0023

      IL_0006:  ldarg.1
      IL_0007:  stloc.0
      IL_0008:  ldarg.2
      IL_0009:  stloc.1
      IL_000a:  ldarg.0
      IL_000b:  ldfld      string M/Test::someField@
      IL_0010:  stloc.2
      IL_0011:  ldloc.0
      IL_0012:  ldfld      string M/Test::someField@
      IL_0017:  stloc.3
      IL_0018:  ldloc.1
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldarg.1
      IL_0026:  ldnull
      IL_0027:  cgt.un
      IL_0029:  ldc.i4.0
      IL_002a:  ceq
      IL_002c:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class M/Test V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     M/Test
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool M/Test::Equals(class M/Test,
                                                        class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class M/Test obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldfld      string M/Test::someField@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      string M/Test::someField@
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
      .locals init (class M/Test V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     M/Test
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool M/Test::Equals(class M/Test)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance string someField()
    {
      .custom instance void class M/FieldLevelAttribute`1<string>::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string M/Test::get_someField()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$M
       extends [runtime]System.Object
{
} 






