




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
    .method public static class assembly/U NewU(int32 item1, int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void assembly/U::.ctor(int32,
                                                                 int32)
      IL_0007:  ret
    } 

    .method assembly specialname rtspecialname instance void  .ctor(int32 item1, int32 item2) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 10 54 65 73 74 46 75 6E 63 74   
                                                                                                                               69 6F 6E 31 36 2B 55 00 00 )                      
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

    .method public hidebysig instance int32 get_Item1() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/U::item1
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 get_Item2() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/U::item2
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
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
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

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/U obj) cil managed
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
               int32 V_8,
               class [runtime]System.Collections.IComparer V_9,
               int32 V_10,
               int32 V_11,
               class [runtime]System.Collections.IComparer V_12,
               int32 V_13,
               int32 V_14)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_007d

      IL_0006:  ldarg.1
      IL_0007:  brfalse.s  IL_007b

      IL_0009:  ldarg.0
      IL_000a:  pop
      IL_000b:  ldarg.0
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  stloc.1
      IL_000f:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0014:  stloc.3
      IL_0015:  ldloc.0
      IL_0016:  ldfld      int32 assembly/U::item1
      IL_001b:  stloc.s    V_4
      IL_001d:  ldloc.1
      IL_001e:  ldfld      int32 assembly/U::item1
      IL_0023:  stloc.s    V_5
      IL_0025:  ldloc.3
      IL_0026:  stloc.s    V_6
      IL_0028:  ldloc.s    V_4
      IL_002a:  stloc.s    V_7
      IL_002c:  ldloc.s    V_5
      IL_002e:  stloc.s    V_8
      IL_0030:  ldloc.s    V_7
      IL_0032:  ldloc.s    V_8
      IL_0034:  cgt
      IL_0036:  ldloc.s    V_7
      IL_0038:  ldloc.s    V_8
      IL_003a:  clt
      IL_003c:  sub
      IL_003d:  stloc.2
      IL_003e:  ldloc.2
      IL_003f:  ldc.i4.0
      IL_0040:  bge.s      IL_0044

      IL_0042:  ldloc.2
      IL_0043:  ret

      IL_0044:  ldloc.2
      IL_0045:  ldc.i4.0
      IL_0046:  ble.s      IL_004a

      IL_0048:  ldloc.2
      IL_0049:  ret

      IL_004a:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_004f:  stloc.s    V_9
      IL_0051:  ldloc.0
      IL_0052:  ldfld      int32 assembly/U::item2
      IL_0057:  stloc.s    V_10
      IL_0059:  ldloc.1
      IL_005a:  ldfld      int32 assembly/U::item2
      IL_005f:  stloc.s    V_11
      IL_0061:  ldloc.s    V_9
      IL_0063:  stloc.s    V_12
      IL_0065:  ldloc.s    V_10
      IL_0067:  stloc.s    V_13
      IL_0069:  ldloc.s    V_11
      IL_006b:  stloc.s    V_14
      IL_006d:  ldloc.s    V_13
      IL_006f:  ldloc.s    V_14
      IL_0071:  cgt
      IL_0073:  ldloc.s    V_13
      IL_0075:  ldloc.s    V_14
      IL_0077:  clt
      IL_0079:  sub
      IL_007a:  ret

      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  ldarg.1
      IL_007e:  brfalse.s  IL_0082

      IL_0080:  ldc.i4.m1
      IL_0081:  ret

      IL_0082:  ldc.i4.0
      IL_0083:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/U
      IL_0007:  callvirt   instance int32 assembly/U::CompareTo(class assembly/U)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
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
               int32 V_9,
               class [runtime]System.Collections.IComparer V_10,
               int32 V_11,
               int32 V_12,
               class [runtime]System.Collections.IComparer V_13,
               int32 V_14,
               int32 V_15)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0080

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/U
      IL_0010:  brfalse.s  IL_007e

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
      IL_002b:  ldloc.s    V_4
      IL_002d:  stloc.s    V_7
      IL_002f:  ldloc.s    V_5
      IL_0031:  stloc.s    V_8
      IL_0033:  ldloc.s    V_6
      IL_0035:  stloc.s    V_9
      IL_0037:  ldloc.s    V_8
      IL_0039:  ldloc.s    V_9
      IL_003b:  cgt
      IL_003d:  ldloc.s    V_8
      IL_003f:  ldloc.s    V_9
      IL_0041:  clt
      IL_0043:  sub
      IL_0044:  stloc.3
      IL_0045:  ldloc.3
      IL_0046:  ldc.i4.0
      IL_0047:  bge.s      IL_004b

      IL_0049:  ldloc.3
      IL_004a:  ret

      IL_004b:  ldloc.3
      IL_004c:  ldc.i4.0
      IL_004d:  ble.s      IL_0051

      IL_004f:  ldloc.3
      IL_0050:  ret

      IL_0051:  ldarg.2
      IL_0052:  stloc.s    V_10
      IL_0054:  ldloc.1
      IL_0055:  ldfld      int32 assembly/U::item2
      IL_005a:  stloc.s    V_11
      IL_005c:  ldloc.2
      IL_005d:  ldfld      int32 assembly/U::item2
      IL_0062:  stloc.s    V_12
      IL_0064:  ldloc.s    V_10
      IL_0066:  stloc.s    V_13
      IL_0068:  ldloc.s    V_11
      IL_006a:  stloc.s    V_14
      IL_006c:  ldloc.s    V_12
      IL_006e:  stloc.s    V_15
      IL_0070:  ldloc.s    V_14
      IL_0072:  ldloc.s    V_15
      IL_0074:  cgt
      IL_0076:  ldloc.s    V_14
      IL_0078:  ldloc.s    V_15
      IL_007a:  clt
      IL_007c:  sub
      IL_007d:  ret

      IL_007e:  ldc.i4.1
      IL_007f:  ret

      IL_0080:  ldarg.1
      IL_0081:  unbox.any  assembly/U
      IL_0086:  brfalse.s  IL_008a

      IL_0088:  ldc.i4.m1
      IL_0089:  ret

      IL_008a:  ldc.i4.0
      IL_008b:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class assembly/U V_1,
               class [runtime]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               class [runtime]System.Collections.IEqualityComparer V_4,
               class [runtime]System.Collections.IEqualityComparer V_5,
               int32 V_6,
               class [runtime]System.Collections.IEqualityComparer V_7)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0049

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
      IL_0018:  stloc.3
      IL_0019:  ldloc.2
      IL_001a:  stloc.s    V_4
      IL_001c:  ldloc.3
      IL_001d:  ldloc.0
      IL_001e:  ldc.i4.6
      IL_001f:  shl
      IL_0020:  ldloc.0
      IL_0021:  ldc.i4.2
      IL_0022:  shr
      IL_0023:  add
      IL_0024:  add
      IL_0025:  add
      IL_0026:  stloc.0
      IL_0027:  ldc.i4     0x9e3779b9
      IL_002c:  ldarg.1
      IL_002d:  stloc.s    V_5
      IL_002f:  ldloc.1
      IL_0030:  ldfld      int32 assembly/U::item1
      IL_0035:  stloc.s    V_6
      IL_0037:  ldloc.s    V_5
      IL_0039:  stloc.s    V_7
      IL_003b:  ldloc.s    V_6
      IL_003d:  ldloc.0
      IL_003e:  ldc.i4.6
      IL_003f:  shl
      IL_0040:  ldloc.0
      IL_0041:  ldc.i4.2
      IL_0042:  shr
      IL_0043:  add
      IL_0044:  add
      IL_0045:  add
      IL_0046:  stloc.0
      IL_0047:  ldloc.0
      IL_0048:  ret

      IL_0049:  ldc.i4.0
      IL_004a:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/U::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/U obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0,
               class assembly/U V_1,
               class assembly/U V_2,
               class [runtime]System.Collections.IEqualityComparer V_3,
               int32 V_4,
               int32 V_5,
               class [runtime]System.Collections.IEqualityComparer V_6,
               class [runtime]System.Collections.IEqualityComparer V_7,
               int32 V_8,
               int32 V_9,
               class [runtime]System.Collections.IEqualityComparer V_10)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_004d

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_004b

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
      IL_0011:  ldfld      int32 assembly/U::item1
      IL_0016:  stloc.s    V_4
      IL_0018:  ldloc.2
      IL_0019:  ldfld      int32 assembly/U::item1
      IL_001e:  stloc.s    V_5
      IL_0020:  ldloc.3
      IL_0021:  stloc.s    V_6
      IL_0023:  ldloc.s    V_4
      IL_0025:  ldloc.s    V_5
      IL_0027:  ceq
      IL_0029:  brfalse.s  IL_0049

      IL_002b:  ldarg.2
      IL_002c:  stloc.s    V_7
      IL_002e:  ldloc.1
      IL_002f:  ldfld      int32 assembly/U::item2
      IL_0034:  stloc.s    V_8
      IL_0036:  ldloc.2
      IL_0037:  ldfld      int32 assembly/U::item2
      IL_003c:  stloc.s    V_9
      IL_003e:  ldloc.s    V_7
      IL_0040:  stloc.s    V_10
      IL_0042:  ldloc.s    V_8
      IL_0044:  ldloc.s    V_9
      IL_0046:  ceq
      IL_0048:  ret

      IL_0049:  ldc.i4.0
      IL_004a:  ret

      IL_004b:  ldc.i4.0
      IL_004c:  ret

      IL_004d:  ldarg.1
      IL_004e:  ldnull
      IL_004f:  cgt.un
      IL_0051:  ldc.i4.0
      IL_0052:  ceq
      IL_0054:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/U::Equals(class assembly/U,
                                                                  class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/U obj) cil managed
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

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
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

  .method public static class [runtime]System.Tuple`2<class assembly/U,class assembly/U> assembly(int32 inp) cil managed
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






