




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
  .class auto ansi serializable sealed nested public R2
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/R2>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/R2>,
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
    .field assembly int32 C@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_A() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R2::A@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_B() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R2::B@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_C() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/R2::C@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(int32 a,
                                 int32 b,
                                 int32 c) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 20 54 79 70 65 5F 4E 6F 4F 76   
                                                                                                                                                     65 72 6C 61 70 5F 53 70 72 65 61 64 46 72 6F 6D   
                                                                                                                                                     41 6E 6F 6E 2B 52 32 00 00 )                      
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/R2::A@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/R2::B@
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      int32 assembly/R2::C@
      IL_001b:  ret
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
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0086

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_0084

      IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.1
      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R2::A@
      IL_0018:  stloc.2
      IL_0019:  ldarg.1
      IL_001a:  ldfld      int32 assembly/R2::A@
      IL_001f:  stloc.3
      IL_0020:  ldloc.2
      IL_0021:  ldloc.3
      IL_0022:  cgt
      IL_0024:  ldloc.2
      IL_0025:  ldloc.3
      IL_0026:  clt
      IL_0028:  sub
      IL_0029:  stloc.0
      IL_002a:  ldloc.0
      IL_002b:  ldc.i4.0
      IL_002c:  bge.s      IL_0030

      IL_002e:  ldloc.0
      IL_002f:  ret

      IL_0030:  ldloc.0
      IL_0031:  ldc.i4.0
      IL_0032:  ble.s      IL_0036

      IL_0034:  ldloc.0
      IL_0035:  ret

      IL_0036:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_003b:  stloc.1
      IL_003c:  ldarg.0
      IL_003d:  ldfld      int32 assembly/R2::B@
      IL_0042:  stloc.3
      IL_0043:  ldarg.1
      IL_0044:  ldfld      int32 assembly/R2::B@
      IL_0049:  stloc.s    V_4
      IL_004b:  ldloc.3
      IL_004c:  ldloc.s    V_4
      IL_004e:  cgt
      IL_0050:  ldloc.3
      IL_0051:  ldloc.s    V_4
      IL_0053:  clt
      IL_0055:  sub
      IL_0056:  stloc.2
      IL_0057:  ldloc.2
      IL_0058:  ldc.i4.0
      IL_0059:  bge.s      IL_005d

      IL_005b:  ldloc.2
      IL_005c:  ret

      IL_005d:  ldloc.2
      IL_005e:  ldc.i4.0
      IL_005f:  ble.s      IL_0063

      IL_0061:  ldloc.2
      IL_0062:  ret

      IL_0063:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0068:  stloc.1
      IL_0069:  ldarg.0
      IL_006a:  ldfld      int32 assembly/R2::C@
      IL_006f:  stloc.3
      IL_0070:  ldarg.1
      IL_0071:  ldfld      int32 assembly/R2::C@
      IL_0076:  stloc.s    V_4
      IL_0078:  ldloc.3
      IL_0079:  ldloc.s    V_4
      IL_007b:  cgt
      IL_007d:  ldloc.3
      IL_007e:  ldloc.s    V_4
      IL_0080:  clt
      IL_0082:  sub
      IL_0083:  ret

      IL_0084:  ldc.i4.1
      IL_0085:  ret

      IL_0086:  ldarg.1
      IL_0087:  brfalse.s  IL_008b

      IL_0089:  ldc.i4.m1
      IL_008a:  ret

      IL_008b:  ldc.i4.0
      IL_008c:  ret
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
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/R2
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_007a

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/R2
      IL_0010:  brfalse.s  IL_0078

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/R2::A@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/R2::A@
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
      IL_0037:  ldfld      int32 assembly/R2::B@
      IL_003c:  stloc.3
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/R2::B@
      IL_0043:  stloc.s    V_4
      IL_0045:  ldloc.3
      IL_0046:  ldloc.s    V_4
      IL_0048:  cgt
      IL_004a:  ldloc.3
      IL_004b:  ldloc.s    V_4
      IL_004d:  clt
      IL_004f:  sub
      IL_0050:  stloc.2
      IL_0051:  ldloc.2
      IL_0052:  ldc.i4.0
      IL_0053:  bge.s      IL_0057

      IL_0055:  ldloc.2
      IL_0056:  ret

      IL_0057:  ldloc.2
      IL_0058:  ldc.i4.0
      IL_0059:  ble.s      IL_005d

      IL_005b:  ldloc.2
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      int32 assembly/R2::C@
      IL_0063:  stloc.3
      IL_0064:  ldloc.0
      IL_0065:  ldfld      int32 assembly/R2::C@
      IL_006a:  stloc.s    V_4
      IL_006c:  ldloc.3
      IL_006d:  ldloc.s    V_4
      IL_006f:  cgt
      IL_0071:  ldloc.3
      IL_0072:  ldloc.s    V_4
      IL_0074:  clt
      IL_0076:  sub
      IL_0077:  ret

      IL_0078:  ldc.i4.1
      IL_0079:  ret

      IL_007a:  ldarg.1
      IL_007b:  unbox.any  assembly/R2
      IL_0080:  brfalse.s  IL_0084

      IL_0082:  ldc.i4.m1
      IL_0083:  ret

      IL_0084:  ldc.i4.0
      IL_0085:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0046

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 assembly/R2::C@
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
      IL_0020:  ldfld      int32 assembly/R2::B@
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
      IL_0035:  ldfld      int32 assembly/R2::A@
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
      IL_0044:  ldloc.0
      IL_0045:  ret

      IL_0046:  ldc.i4.0
      IL_0047:  ret
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
      IL_0001:  brfalse.s  IL_0037

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0035

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R2::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R2::A@
      IL_0012:  bne.un.s   IL_0033

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R2::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R2::B@
      IL_0020:  bne.un.s   IL_0031

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R2::C@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R2::C@
      IL_002e:  ceq
      IL_0030:  ret

      IL_0031:  ldc.i4.0
      IL_0032:  ret

      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldc.i4.0
      IL_0036:  ret

      IL_0037:  ldarg.1
      IL_0038:  ldnull
      IL_0039:  cgt.un
      IL_003b:  ldc.i4.0
      IL_003c:  ceq
      IL_003e:  ret
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
      IL_0001:  brfalse.s  IL_0037

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0035

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/R2::A@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/R2::A@
      IL_0012:  bne.un.s   IL_0033

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/R2::B@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/R2::B@
      IL_0020:  bne.un.s   IL_0031

      IL_0022:  ldarg.0
      IL_0023:  ldfld      int32 assembly/R2::C@
      IL_0028:  ldarg.1
      IL_0029:  ldfld      int32 assembly/R2::C@
      IL_002e:  ceq
      IL_0030:  ret

      IL_0031:  ldc.i4.0
      IL_0032:  ret

      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldc.i4.0
      IL_0036:  ret

      IL_0037:  ldarg.1
      IL_0038:  ldnull
      IL_0039:  cgt.un
      IL_003b:  ldc.i4.0
      IL_003c:  ceq
      IL_003e:  ret
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

    .property instance int32 A()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/R2::get_A()
    } 
    .property instance int32 B()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance int32 assembly/R2::get_B()
    } 
    .property instance int32 C()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 ) 
      .get instance int32 assembly/R2::get_C()
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






