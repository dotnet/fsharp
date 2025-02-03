




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





.class public abstract auto ansi sealed ABC
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Expr
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class ABC/Expr>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class ABC/Expr>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static class ABC/Expr NewNum(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void ABC/Expr::.ctor(int32)
      IL_0006:  ret
    } 

    .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 08 41 42 43 2B 45 78 70 72 00   
                                                                                                                               00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ABC/Expr::item
      IL_000d:  ret
    } 

    .method public hidebysig instance int32 get_Item() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ABC/Expr::item
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } 

    .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class ABC/Expr>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/Expr,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class ABC/Expr obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class ABC/Expr V_0,
               class ABC/Expr V_1,
               class [runtime]System.Collections.IComparer V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002f

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002d

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.2
      IL_0012:  ldloc.0
      IL_0013:  ldfld      int32 ABC/Expr::item
      IL_0018:  stloc.3
      IL_0019:  ldloc.1
      IL_001a:  ldfld      int32 ABC/Expr::item
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.3
      IL_0022:  ldloc.s    V_4
      IL_0024:  cgt
      IL_0026:  ldloc.3
      IL_0027:  ldloc.s    V_4
      IL_0029:  clt
      IL_002b:  sub
      IL_002c:  ret

      IL_002d:  ldc.i4.1
      IL_002e:  ret

      IL_002f:  ldarg.1
      IL_0030:  brfalse.s  IL_0034

      IL_0032:  ldc.i4.m1
      IL_0033:  ret

      IL_0034:  ldc.i4.0
      IL_0035:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  ABC/Expr
      IL_0007:  callvirt   instance int32 ABC/Expr::CompareTo(class ABC/Expr)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class ABC/Expr V_0,
               class ABC/Expr V_1,
               class ABC/Expr V_2,
               class [runtime]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  ABC/Expr
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_003a

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  ABC/Expr
      IL_0010:  brfalse.s  IL_0038

      IL_0012:  ldarg.0
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  stloc.1
      IL_0016:  ldloc.0
      IL_0017:  stloc.2
      IL_0018:  ldarg.2
      IL_0019:  stloc.3
      IL_001a:  ldloc.1
      IL_001b:  ldfld      int32 ABC/Expr::item
      IL_0020:  stloc.s    V_4
      IL_0022:  ldloc.2
      IL_0023:  ldfld      int32 ABC/Expr::item
      IL_0028:  stloc.s    V_5
      IL_002a:  ldloc.s    V_4
      IL_002c:  ldloc.s    V_5
      IL_002e:  cgt
      IL_0030:  ldloc.s    V_4
      IL_0032:  ldloc.s    V_5
      IL_0034:  clt
      IL_0036:  sub
      IL_0037:  ret

      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.1
      IL_003b:  unbox.any  ABC/Expr
      IL_0040:  brfalse.s  IL_0044

      IL_0042:  ldc.i4.m1
      IL_0043:  ret

      IL_0044:  ldc.i4.0
      IL_0045:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class ABC/Expr V_1,
               class [runtime]System.Collections.IEqualityComparer V_2)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0024

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  pop
      IL_0007:  ldarg.0
      IL_0008:  stloc.1
      IL_0009:  ldc.i4.0
      IL_000a:  stloc.0
      IL_000b:  ldc.i4     0x9e3779b9
      IL_0010:  ldarg.1
      IL_0011:  stloc.2
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 ABC/Expr::item
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.6
      IL_001a:  shl
      IL_001b:  ldloc.0
      IL_001c:  ldc.i4.2
      IL_001d:  shr
      IL_001e:  add
      IL_001f:  add
      IL_0020:  add
      IL_0021:  stloc.0
      IL_0022:  ldloc.0
      IL_0023:  ret

      IL_0024:  ldc.i4.0
      IL_0025:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 ABC/Expr::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class ABC/Expr obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class ABC/Expr V_0,
               class ABC/Expr V_1,
               class ABC/Expr V_2,
               class [runtime]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0021

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001f

      IL_0006:  ldarg.1
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  pop
      IL_000a:  ldarg.0
      IL_000b:  stloc.1
      IL_000c:  ldloc.0
      IL_000d:  stloc.2
      IL_000e:  ldarg.2
      IL_000f:  stloc.3
      IL_0010:  ldloc.1
      IL_0011:  ldfld      int32 ABC/Expr::item
      IL_0016:  ldloc.2
      IL_0017:  ldfld      int32 ABC/Expr::item
      IL_001c:  ceq
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret

      IL_0021:  ldarg.1
      IL_0022:  ldnull
      IL_0023:  cgt.un
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class ABC/Expr V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     ABC/Expr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool ABC/Expr::Equals(class ABC/Expr,
                                                          class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class ABC/Expr obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class ABC/Expr V_0,
               class ABC/Expr V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001d

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001b

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldloc.0
      IL_000d:  ldfld      int32 ABC/Expr::item
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 ABC/Expr::item
      IL_0018:  ceq
      IL_001a:  ret

      IL_001b:  ldc.i4.0
      IL_001c:  ret

      IL_001d:  ldarg.1
      IL_001e:  ldnull
      IL_001f:  cgt.un
      IL_0021:  ldc.i4.0
      IL_0022:  ceq
      IL_0024:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class ABC/Expr V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     ABC/Expr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool ABC/Expr::Equals(class ABC/Expr)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 ABC/Expr::get_Tag()
    } 
    .property instance int32 Item()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 ABC/Expr::get_Item()
    } 
  } 

  .class auto ansi serializable nested public beforefieldinit MyExn
         extends [runtime]System.Exception
         implements [runtime]System.Collections.IStructuralEquatable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .field assembly int32 Data0@
    .method public specialname rtspecialname instance void  .ctor(int32 data0) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ABC/MyExn::Data0@
      IL_000d:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ret
    } 

    .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                           valuetype [runtime]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } 

    .method public hidebysig specialname instance int32  get_Data0() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ABC/MyExn::Data0@
      IL_0006:  ret
    } 

    .method public strict virtual instance string get_Message() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/MyExn,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class ABC/MyExn>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/MyExn,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/MyExn,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual instance int32 GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class [runtime]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0023

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldc.i4     0x9e3779b9
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  castclass  ABC/MyExn
      IL_0012:  call       instance int32 ABC/MyExn::get_Data0()
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.6
      IL_0019:  shl
      IL_001a:  ldloc.0
      IL_001b:  ldc.i4.2
      IL_001c:  shr
      IL_001d:  add
      IL_001e:  add
      IL_001f:  add
      IL_0020:  stloc.0
      IL_0021:  ldloc.0
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret
    } 

    .method public hidebysig virtual instance int32 GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 ABC/MyExn::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class [runtime]System.Exception obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [runtime]System.Exception V_0,
               object V_1,
               class [runtime]System.Collections.IEqualityComparer V_2)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0034

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0032

      IL_0006:  ldarg.1
      IL_0007:  stloc.0
      IL_0008:  ldarg.1
      IL_0009:  stloc.1
      IL_000a:  ldloc.1
      IL_000b:  isinst     ABC/MyExn
      IL_0010:  ldnull
      IL_0011:  cgt.un
      IL_0013:  brfalse.s  IL_0030

      IL_0015:  ldarg.2
      IL_0016:  stloc.2
      IL_0017:  ldarg.0
      IL_0018:  castclass  ABC/MyExn
      IL_001d:  call       instance int32 ABC/MyExn::get_Data0()
      IL_0022:  ldloc.0
      IL_0023:  castclass  ABC/MyExn
      IL_0028:  call       instance int32 ABC/MyExn::get_Data0()
      IL_002d:  ceq
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldc.i4.0
      IL_0033:  ret

      IL_0034:  ldarg.1
      IL_0035:  ldnull
      IL_0036:  cgt.un
      IL_0038:  ldc.i4.0
      IL_0039:  ceq
      IL_003b:  ret
    } 

    .method public hidebysig virtual instance bool Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class [runtime]System.Exception V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     [runtime]System.Exception
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool ABC/MyExn::Equals(class [runtime]System.Exception,
                                                           class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig instance bool Equals(class [runtime]System.Exception obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0030

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002e

      IL_0006:  ldarg.1
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  isinst     ABC/MyExn
      IL_000e:  ldnull
      IL_000f:  cgt.un
      IL_0011:  brfalse.s  IL_002c

      IL_0013:  ldarg.0
      IL_0014:  castclass  ABC/MyExn
      IL_0019:  call       instance int32 ABC/MyExn::get_Data0()
      IL_001e:  ldarg.1
      IL_001f:  castclass  ABC/MyExn
      IL_0024:  call       instance int32 ABC/MyExn::get_Data0()
      IL_0029:  ceq
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret

      IL_0030:  ldarg.1
      IL_0031:  ldnull
      IL_0032:  cgt.un
      IL_0034:  ldc.i4.0
      IL_0035:  ceq
      IL_0037:  ret
    } 

    .method public hidebysig virtual instance bool Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [runtime]System.Exception V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     [runtime]System.Exception
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool ABC/MyExn::Equals(class [runtime]System.Exception)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Data0()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 ABC/MyExn::get_Data0()
    } 
  } 

  .class auto ansi serializable nested public A
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly string x
    .method public specialname rtspecialname instance void  .ctor(string x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      string ABC/A::x
      IL_000f:  ret
    } 

    .method public hidebysig specialname instance string  get_X() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ABC/A::x
      IL_0006:  ret
    } 

    .property instance string X()
    {
      .get instance string ABC/A::get_X()
    } 
  } 

  .class abstract auto ansi sealed nested public ABC
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Expr
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class ABC/ABC/Expr>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class ABC/ABC/Expr>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class ABC/ABC/Expr NewNum(int32 item) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  newobj     instance void ABC/ABC/Expr::.ctor(int32)
        IL_0006:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
      {
        .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0C 41 42 43 2B 41 42 43 2B 45   
                                                                                                                                 78 70 72 00 00 )                                  
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 ABC/ABC/Expr::item
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ABC/ABC/Expr::item
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Tag() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldc.i4.0
        IL_0003:  ret
      } 

      .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class ABC/ABC/Expr>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/Expr,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class ABC/ABC/Expr obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class ABC/ABC/Expr V_0,
                 class ABC/ABC/Expr V_1,
                 class [runtime]System.Collections.IComparer V_2,
                 int32 V_3,
                 int32 V_4)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002f

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002d

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.2
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 ABC/ABC/Expr::item
        IL_0018:  stloc.3
        IL_0019:  ldloc.1
        IL_001a:  ldfld      int32 ABC/ABC/Expr::item
        IL_001f:  stloc.s    V_4
        IL_0021:  ldloc.3
        IL_0022:  ldloc.s    V_4
        IL_0024:  cgt
        IL_0026:  ldloc.3
        IL_0027:  ldloc.s    V_4
        IL_0029:  clt
        IL_002b:  sub
        IL_002c:  ret

        IL_002d:  ldc.i4.1
        IL_002e:  ret

        IL_002f:  ldarg.1
        IL_0030:  brfalse.s  IL_0034

        IL_0032:  ldc.i4.m1
        IL_0033:  ret

        IL_0034:  ldc.i4.0
        IL_0035:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  ABC/ABC/Expr
        IL_0007:  callvirt   instance int32 ABC/ABC/Expr::CompareTo(class ABC/ABC/Expr)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class ABC/ABC/Expr V_0,
                 class ABC/ABC/Expr V_1,
                 class ABC/ABC/Expr V_2,
                 class [runtime]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  ABC/ABC/Expr
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_003a

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  ABC/ABC/Expr
        IL_0010:  brfalse.s  IL_0038

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldarg.2
        IL_0019:  stloc.3
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 ABC/ABC/Expr::item
        IL_0020:  stloc.s    V_4
        IL_0022:  ldloc.2
        IL_0023:  ldfld      int32 ABC/ABC/Expr::item
        IL_0028:  stloc.s    V_5
        IL_002a:  ldloc.s    V_4
        IL_002c:  ldloc.s    V_5
        IL_002e:  cgt
        IL_0030:  ldloc.s    V_4
        IL_0032:  ldloc.s    V_5
        IL_0034:  clt
        IL_0036:  sub
        IL_0037:  ret

        IL_0038:  ldc.i4.1
        IL_0039:  ret

        IL_003a:  ldarg.1
        IL_003b:  unbox.any  ABC/ABC/Expr
        IL_0040:  brfalse.s  IL_0044

        IL_0042:  ldc.i4.m1
        IL_0043:  ret

        IL_0044:  ldc.i4.0
        IL_0045:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class ABC/ABC/Expr V_1,
                 class [runtime]System.Collections.IEqualityComparer V_2)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0024

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldarg.1
        IL_0011:  stloc.2
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 ABC/ABC/Expr::item
        IL_0018:  ldloc.0
        IL_0019:  ldc.i4.6
        IL_001a:  shl
        IL_001b:  ldloc.0
        IL_001c:  ldc.i4.2
        IL_001d:  shr
        IL_001e:  add
        IL_001f:  add
        IL_0020:  add
        IL_0021:  stloc.0
        IL_0022:  ldloc.0
        IL_0023:  ret

        IL_0024:  ldc.i4.0
        IL_0025:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 ABC/ABC/Expr::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class ABC/ABC/Expr obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class ABC/ABC/Expr V_0,
                 class ABC/ABC/Expr V_1,
                 class ABC/ABC/Expr V_2,
                 class [runtime]System.Collections.IEqualityComparer V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0021

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_001f

        IL_0006:  ldarg.1
        IL_0007:  stloc.0
        IL_0008:  ldarg.0
        IL_0009:  pop
        IL_000a:  ldarg.0
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  stloc.2
        IL_000e:  ldarg.2
        IL_000f:  stloc.3
        IL_0010:  ldloc.1
        IL_0011:  ldfld      int32 ABC/ABC/Expr::item
        IL_0016:  ldloc.2
        IL_0017:  ldfld      int32 ABC/ABC/Expr::item
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret

        IL_0021:  ldarg.1
        IL_0022:  ldnull
        IL_0023:  cgt.un
        IL_0025:  ldc.i4.0
        IL_0026:  ceq
        IL_0028:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class ABC/ABC/Expr V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     ABC/ABC/Expr
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool ABC/ABC/Expr::Equals(class ABC/ABC/Expr,
                                                                class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class ABC/ABC/Expr obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class ABC/ABC/Expr V_0,
                 class ABC/ABC/Expr V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_001d

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_001b

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 ABC/ABC/Expr::item
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 ABC/ABC/Expr::item
        IL_0018:  ceq
        IL_001a:  ret

        IL_001b:  ldc.i4.0
        IL_001c:  ret

        IL_001d:  ldarg.1
        IL_001e:  ldnull
        IL_001f:  cgt.un
        IL_0021:  ldc.i4.0
        IL_0022:  ceq
        IL_0024:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class ABC/ABC/Expr V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     ABC/ABC/Expr
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool ABC/ABC/Expr::Equals(class ABC/ABC/Expr)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 ABC/ABC/Expr::get_Tag()
      } 
      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 ABC/ABC/Expr::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit MyExn
           extends [runtime]System.Exception
           implements [runtime]System.Collections.IStructuralEquatable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
      .field assembly int32 Data0@
      .method public specialname rtspecialname instance void  .ctor(int32 data0) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Exception::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 ABC/ABC/MyExn::Data0@
        IL_000d:  ret
      } 

      .method public specialname rtspecialname instance void  .ctor() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Exception::.ctor()
        IL_0006:  ret
      } 

      .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  ldarg.2
        IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                             valuetype [runtime]System.Runtime.Serialization.StreamingContext)
        IL_0008:  ret
      } 

      .method public hidebysig specialname instance int32  get_Data0() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ABC/ABC/MyExn::Data0@
        IL_0006:  ret
      } 

      .method public strict virtual instance string get_Message() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/MyExn,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class ABC/ABC/MyExn>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/MyExn,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class ABC/ABC/MyExn,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual instance int32 GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IEqualityComparer V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0023

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldc.i4     0x9e3779b9
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldarg.0
        IL_000d:  castclass  ABC/ABC/MyExn
        IL_0012:  call       instance int32 ABC/ABC/MyExn::get_Data0()
        IL_0017:  ldloc.0
        IL_0018:  ldc.i4.6
        IL_0019:  shl
        IL_001a:  ldloc.0
        IL_001b:  ldc.i4.2
        IL_001c:  shr
        IL_001d:  add
        IL_001e:  add
        IL_001f:  add
        IL_0020:  stloc.0
        IL_0021:  ldloc.0
        IL_0022:  ret

        IL_0023:  ldc.i4.0
        IL_0024:  ret
      } 

      .method public hidebysig virtual instance int32 GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 ABC/ABC/MyExn::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class [runtime]System.Exception obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class [runtime]System.Exception V_0,
                 object V_1,
                 class [runtime]System.Collections.IEqualityComparer V_2)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0034

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0032

        IL_0006:  ldarg.1
        IL_0007:  stloc.0
        IL_0008:  ldarg.1
        IL_0009:  stloc.1
        IL_000a:  ldloc.1
        IL_000b:  isinst     ABC/ABC/MyExn
        IL_0010:  ldnull
        IL_0011:  cgt.un
        IL_0013:  brfalse.s  IL_0030

        IL_0015:  ldarg.2
        IL_0016:  stloc.2
        IL_0017:  ldarg.0
        IL_0018:  castclass  ABC/ABC/MyExn
        IL_001d:  call       instance int32 ABC/ABC/MyExn::get_Data0()
        IL_0022:  ldloc.0
        IL_0023:  castclass  ABC/ABC/MyExn
        IL_0028:  call       instance int32 ABC/ABC/MyExn::get_Data0()
        IL_002d:  ceq
        IL_002f:  ret

        IL_0030:  ldc.i4.0
        IL_0031:  ret

        IL_0032:  ldc.i4.0
        IL_0033:  ret

        IL_0034:  ldarg.1
        IL_0035:  ldnull
        IL_0036:  cgt.un
        IL_0038:  ldc.i4.0
        IL_0039:  ceq
        IL_003b:  ret
      } 

      .method public hidebysig virtual instance bool Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class [runtime]System.Exception V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     [runtime]System.Exception
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool ABC/ABC/MyExn::Equals(class [runtime]System.Exception,
                                                                 class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig instance bool Equals(class [runtime]System.Exception obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (object V_0)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0030

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002e

        IL_0006:  ldarg.1
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  isinst     ABC/ABC/MyExn
        IL_000e:  ldnull
        IL_000f:  cgt.un
        IL_0011:  brfalse.s  IL_002c

        IL_0013:  ldarg.0
        IL_0014:  castclass  ABC/ABC/MyExn
        IL_0019:  call       instance int32 ABC/ABC/MyExn::get_Data0()
        IL_001e:  ldarg.1
        IL_001f:  castclass  ABC/ABC/MyExn
        IL_0024:  call       instance int32 ABC/ABC/MyExn::get_Data0()
        IL_0029:  ceq
        IL_002b:  ret

        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldc.i4.0
        IL_002f:  ret

        IL_0030:  ldarg.1
        IL_0031:  ldnull
        IL_0032:  cgt.un
        IL_0034:  ldc.i4.0
        IL_0035:  ceq
        IL_0037:  ret
      } 

      .method public hidebysig virtual instance bool Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class [runtime]System.Exception V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     [runtime]System.Exception
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool ABC/ABC/MyExn::Equals(class [runtime]System.Exception)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } 

      .property instance int32 Data0()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 ABC/ABC/MyExn::get_Data0()
      } 
    } 

    .class auto ansi serializable nested public A
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field assembly string x
      .method public specialname rtspecialname instance void  .ctor(string x) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  stfld      string ABC/ABC/A::x
        IL_000f:  ret
      } 

      .method public hidebysig specialname instance string  get_X() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      string ABC/ABC/A::x
        IL_0006:  ret
      } 

      .property instance string X()
      {
        .get instance string ABC/ABC/A::get_X()
      } 
    } 

    .method public static int32  'add'(int32 x,
                                       int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } 

    .method public specialname static string get_greeting() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "hello"
      IL_0005:  ret
    } 

    .property string greeting()
    {
      .get string ABC/ABC::get_greeting()
    } 
  } 

  .method public static int32  'add'(int32 x,
                                     int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } 

  .method public specialname static string get_greeting() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "hello"
    IL_0005:  ret
  } 

  .property string greeting()
  {
    .get string ABC::get_greeting()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ABC
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






