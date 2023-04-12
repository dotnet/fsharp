




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
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
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
  .class auto autochar serializable sealed nested public beforefieldinit U
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/U>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/U>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly initonly int32 item2
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static class assembly/U 
            NewU(int32 item1,
                 int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void assembly/U::.ctor(int32,
                                                                 int32)
      IL_0007:  ret
    } 

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 item1,
                                 int32 item2) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/U::item1
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/U::item2
      IL_0014:  ret
    } 

    .method public hidebysig instance int32 
            get_Item1() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/U::item1
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 
            get_Item2() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/U::item2
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } 

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(class assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/U V_0,
               class assembly/U V_1,
               int32 V_2,
               class [runtime]System.Collections.IComparer V_3,
               int32 V_4,
               int32 V_5,
               class [runtime]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0063

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0061

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0011:  stloc.3
      IL_0012:  ldloc.0
      IL_0013:  ldfld      int32 assembly/U::item1
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloc.1
      IL_001b:  ldfld      int32 assembly/U::item1
      IL_0020:  stloc.s    V_5
      IL_0022:  ldloc.s    V_4
      IL_0024:  ldloc.s    V_5
      IL_0026:  cgt
      IL_0028:  ldloc.s    V_4
      IL_002a:  ldloc.s    V_5
      IL_002c:  clt
      IL_002e:  sub
      IL_002f:  stloc.2
      IL_0030:  ldloc.2
      IL_0031:  ldc.i4.0
      IL_0032:  bge.s      IL_0036

      IL_0034:  ldloc.2
      IL_0035:  ret

      IL_0036:  ldloc.2
      IL_0037:  ldc.i4.0
      IL_0038:  ble.s      IL_003c

      IL_003a:  ldloc.2
      IL_003b:  ret

      IL_003c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0041:  stloc.s    V_6
      IL_0043:  ldloc.0
      IL_0044:  ldfld      int32 assembly/U::item2
      IL_0049:  stloc.s    V_7
      IL_004b:  ldloc.1
      IL_004c:  ldfld      int32 assembly/U::item2
      IL_0051:  stloc.s    V_8
      IL_0053:  ldloc.s    V_7
      IL_0055:  ldloc.s    V_8
      IL_0057:  cgt
      IL_0059:  ldloc.s    V_7
      IL_005b:  ldloc.s    V_8
      IL_005d:  clt
      IL_005f:  sub
      IL_0060:  ret

      IL_0061:  ldc.i4.1
      IL_0062:  ret

      IL_0063:  ldarg.1
      IL_0064:  brfalse.s  IL_0068

      IL_0066:  ldc.i4.m1
      IL_0067:  ret

      IL_0068:  ldc.i4.0
      IL_0069:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/U
      IL_0007:  callvirt   instance int32 assembly/U::CompareTo(class assembly/U)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/U V_0,
               class assembly/U V_1,
               class assembly/U V_2,
               int32 V_3,
               class [runtime]System.Collections.IComparer V_4,
               int32 V_5,
               int32 V_6,
               class [runtime]System.Collections.IComparer V_7,
               int32 V_8,
               int32 V_9)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0068

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/U
      IL_0010:  brfalse.s  IL_0066

      IL_0012:  ldarg.0
      IL_0013:  pop
      IL_0014:  ldarg.0
      IL_0015:  stloc.1
      IL_0016:  ldloc.0
      IL_0017:  stloc.2
      IL_0018:  ldarg.2
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.1
      IL_001c:  ldfld      int32 assembly/U::item1
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloc.2
      IL_0024:  ldfld      int32 assembly/U::item1
      IL_0029:  stloc.s    V_6
      IL_002b:  ldloc.s    V_5
      IL_002d:  ldloc.s    V_6
      IL_002f:  cgt
      IL_0031:  ldloc.s    V_5
      IL_0033:  ldloc.s    V_6
      IL_0035:  clt
      IL_0037:  sub
      IL_0038:  stloc.3
      IL_0039:  ldloc.3
      IL_003a:  ldc.i4.0
      IL_003b:  bge.s      IL_003f

      IL_003d:  ldloc.3
      IL_003e:  ret

      IL_003f:  ldloc.3
      IL_0040:  ldc.i4.0
      IL_0041:  ble.s      IL_0045

      IL_0043:  ldloc.3
      IL_0044:  ret

      IL_0045:  ldarg.2
      IL_0046:  stloc.s    V_7
      IL_0048:  ldloc.1
      IL_0049:  ldfld      int32 assembly/U::item2
      IL_004e:  stloc.s    V_8
      IL_0050:  ldloc.2
      IL_0051:  ldfld      int32 assembly/U::item2
      IL_0056:  stloc.s    V_9
      IL_0058:  ldloc.s    V_8
      IL_005a:  ldloc.s    V_9
      IL_005c:  cgt
      IL_005e:  ldloc.s    V_8
      IL_0060:  ldloc.s    V_9
      IL_0062:  clt
      IL_0064:  sub
      IL_0065:  ret

      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  ldarg.1
      IL_0069:  unbox.any  assembly/U
      IL_006e:  brfalse.s  IL_0072

      IL_0070:  ldc.i4.m1
      IL_0071:  ret

      IL_0072:  ldc.i4.0
      IL_0073:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class assembly/U V_1,
               class [runtime]System.Collections.IEqualityComparer V_2,
               class [runtime]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_003b

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
      IL_0013:  ldfld      int32 assembly/U::item2
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
      IL_0022:  ldc.i4     0x9e3779b9
      IL_0027:  ldarg.1
      IL_0028:  stloc.3
      IL_0029:  ldloc.1
      IL_002a:  ldfld      int32 assembly/U::item1
      IL_002f:  ldloc.0
      IL_0030:  ldc.i4.6
      IL_0031:  shl
      IL_0032:  ldloc.0
      IL_0033:  ldc.i4.2
      IL_0034:  shr
      IL_0035:  add
      IL_0036:  add
      IL_0037:  add
      IL_0038:  stloc.0
      IL_0039:  ldloc.0
      IL_003a:  ret

      IL_003b:  ldc.i4.0
      IL_003c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/U::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0,
               class assembly/U V_1,
               class assembly/U V_2,
               class assembly/U V_3,
               class [runtime]System.Collections.IEqualityComparer V_4,
               class [runtime]System.Collections.IEqualityComparer V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_003e

      IL_0003:  ldarg.1
      IL_0004:  isinst     assembly/U
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_003c

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.0
      IL_0010:  pop
      IL_0011:  ldarg.0
      IL_0012:  stloc.2
      IL_0013:  ldloc.1
      IL_0014:  stloc.3
      IL_0015:  ldarg.2
      IL_0016:  stloc.s    V_4
      IL_0018:  ldloc.2
      IL_0019:  ldfld      int32 assembly/U::item1
      IL_001e:  ldloc.3
      IL_001f:  ldfld      int32 assembly/U::item1
      IL_0024:  ceq
      IL_0026:  brfalse.s  IL_003a

      IL_0028:  ldarg.2
      IL_0029:  stloc.s    V_5
      IL_002b:  ldloc.2
      IL_002c:  ldfld      int32 assembly/U::item2
      IL_0031:  ldloc.3
      IL_0032:  ldfld      int32 assembly/U::item2
      IL_0037:  ceq
      IL_0039:  ret

      IL_003a:  ldc.i4.0
      IL_003b:  ret

      IL_003c:  ldc.i4.0
      IL_003d:  ret

      IL_003e:  ldarg.1
      IL_003f:  ldnull
      IL_0040:  cgt.un
      IL_0042:  ldc.i4.0
      IL_0043:  ceq
      IL_0045:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(class assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0,
               class assembly/U V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_002d

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_002b

      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  stloc.0
      IL_000a:  ldarg.1
      IL_000b:  stloc.1
      IL_000c:  ldloc.0
      IL_000d:  ldfld      int32 assembly/U::item1
      IL_0012:  ldloc.1
      IL_0013:  ldfld      int32 assembly/U::item1
      IL_0018:  bne.un.s   IL_0029

      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 assembly/U::item2
      IL_0020:  ldloc.1
      IL_0021:  ldfld      int32 assembly/U::item2
      IL_0026:  ceq
      IL_0028:  ret

      IL_0029:  ldc.i4.0
      IL_002a:  ret

      IL_002b:  ldc.i4.0
      IL_002c:  ret

      IL_002d:  ldarg.1
      IL_002e:  ldnull
      IL_002f:  cgt.un
      IL_0031:  ldc.i4.0
      IL_0032:  ceq
      IL_0034:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/U::Equals(class assembly/U)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/U::get_Tag()
    } 
    .property instance int32 Item1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 assembly/U::get_Item1()
    } 
    .property instance int32 Item2()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 assembly/U::get_Item2()
    } 
  } 

  .method public static class [runtime]System.Tuple`2<class assembly/U,class assembly/U> 
          assembly(int32 inp) cil managed
  {
    
    .maxstack  4
    .locals init (class assembly/U V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       class assembly/U assembly/U::NewU(int32,
                                                                       int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class [runtime]System.Tuple`2<class assembly/U,class assembly/U>::.ctor(!0,
                                                                                                                                  !1)
    IL_000f:  ret
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






