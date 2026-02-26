




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
  .class auto ansi serializable sealed nested public Point
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/Point>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/Point>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .field public int32 x@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field public int32 y@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_x() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/Point::x@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance int32  get_y() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/Point::y@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance void  set_x(int32 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/Point::x@
      IL_0007:  ret
    } 

    .method public hidebysig specialname instance void  set_y(int32 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/Point::y@
      IL_0007:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32 x, int32 y) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 14 54 65 73 74 46 75 6E 63 74   
                                                                                                                                                     69 6F 6E 32 34 2B 50 6F 69 6E 74 00 00 )          
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/Point::x@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/Point::y@
      IL_0014:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Point,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/Point>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Point,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Point,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class assembly/Point obj) cil managed
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
      IL_000d:  ldfld      int32 assembly/Point::x@
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/Point::x@
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
      IL_0037:  ldfld      int32 assembly/Point::y@
      IL_003c:  stloc.2
      IL_003d:  ldarg.1
      IL_003e:  ldfld      int32 assembly/Point::y@
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
      IL_0002:  unbox.any  assembly/Point
      IL_0007:  callvirt   instance int32 assembly/Point::CompareTo(class assembly/Point)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/Point V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/Point
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0050

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/Point
      IL_0010:  brfalse.s  IL_004e

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/Point::x@
      IL_0018:  stloc.2
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/Point::x@
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
      IL_0037:  ldfld      int32 assembly/Point::y@
      IL_003c:  stloc.2
      IL_003d:  ldloc.0
      IL_003e:  ldfld      int32 assembly/Point::y@
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
      IL_0051:  unbox.any  assembly/Point
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
      IL_000b:  ldfld      int32 assembly/Point::y@
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
      IL_0020:  ldfld      int32 assembly/Point::x@
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
      IL_0006:  callvirt   instance int32 assembly/Point::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class assembly/Point obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/Point::x@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/Point::x@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/Point::y@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/Point::y@
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
      .locals init (class assembly/Point V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool assembly/Point::Equals(class assembly/Point,
                                                                      class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class assembly/Point obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/Point::x@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      int32 assembly/Point::x@
      IL_0012:  bne.un.s   IL_0023

      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32 assembly/Point::y@
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 assembly/Point::y@
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
      .locals init (class assembly/Point V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Point
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/Point::Equals(class assembly/Point)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .set instance void assembly/Point::set_x(int32)
      .get instance int32 assembly/Point::get_x()
    } 
    .property instance int32 y()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .set instance void assembly/Point::set_y(int32)
      .get instance int32 assembly/Point::get_y()
    } 
  } 

  .method public static int32  pinObject() cil managed
  {
    
    .maxstack  6
    .locals init (class assembly/Point V_0,
             native int V_1,
             int32& pinned V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void assembly/Point::.ctor(int32,
                                                                   int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldflda     int32 assembly/Point::x@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i
    IL_0015:  sizeof     [runtime]System.Int32
    IL_001b:  mul
    IL_001c:  add
    IL_001d:  ldobj      [runtime]System.Int32
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i
    IL_0025:  sizeof     [runtime]System.Int32
    IL_002b:  mul
    IL_002c:  add
    IL_002d:  ldobj      [runtime]System.Int32
    IL_0032:  add
    IL_0033:  ret
  } 

  .method public static int32  pinRef() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
             native int V_1,
             int32& pinned V_2)
    IL_0000:  ldc.i4.s   17
    IL_0002:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldflda     !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::contents@
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  conv.i
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldobj      [runtime]System.Int32
    IL_0018:  ldloc.1
    IL_0019:  ldobj      [runtime]System.Int32
    IL_001e:  add
    IL_001f:  ret
  } 

  .method public static float64  pinArray1() cil managed
  {
    
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64& pinned V_2)
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [runtime]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem.r8
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.r8     1.5
    IL_001d:  stelem.r8
    IL_001e:  dup
    IL_001f:  ldc.i4.2
    IL_0020:  ldc.r8     2.2999999999999998
    IL_0029:  stelem.r8
    IL_002a:  dup
    IL_002b:  ldc.i4.3
    IL_002c:  ldc.r8     3.3999999999999999
    IL_0035:  stelem.r8
    IL_0036:  dup
    IL_0037:  ldc.i4.4
    IL_0038:  ldc.r8     4.0999999999999996
    IL_0041:  stelem.r8
    IL_0042:  dup
    IL_0043:  ldc.i4.5
    IL_0044:  ldc.r8     5.9000000000000004
    IL_004d:  stelem.r8
    IL_004e:  stloc.0
    IL_004f:  nop
    IL_0050:  ldloc.0
    IL_0051:  brfalse.s  IL_006d

    IL_0053:  ldloc.0
    IL_0054:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Length<float64>(!!0[])
    IL_0059:  brfalse.s  IL_0068

    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.0
    IL_005d:  ldelema    [runtime]System.Double
    IL_0062:  stloc.2
    IL_0063:  ldloc.2
    IL_0064:  conv.i
    IL_0065:  nop
    IL_0066:  br.s       IL_0070

    IL_0068:  ldc.i4.0
    IL_0069:  conv.i
    IL_006a:  nop
    IL_006b:  br.s       IL_0070

    IL_006d:  ldc.i4.0
    IL_006e:  conv.i
    IL_006f:  nop
    IL_0070:  stloc.1
    IL_0071:  ldloc.1
    IL_0072:  ldc.i4.0
    IL_0073:  conv.i
    IL_0074:  sizeof     [runtime]System.Double
    IL_007a:  mul
    IL_007b:  add
    IL_007c:  ldobj      [runtime]System.Double
    IL_0081:  ldloc.1
    IL_0082:  ldc.i4.1
    IL_0083:  conv.i
    IL_0084:  sizeof     [runtime]System.Double
    IL_008a:  mul
    IL_008b:  add
    IL_008c:  ldobj      [runtime]System.Double
    IL_0091:  add
    IL_0092:  ret
  } 

  .method public static float64  pinArray2() cil managed
  {
    
    .maxstack  6
    .locals init (float64[] V_0,
             native int V_1,
             float64& pinned V_2)
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [runtime]System.Double
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r8     0.0
    IL_0011:  stelem.r8
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.r8     1.5
    IL_001d:  stelem.r8
    IL_001e:  dup
    IL_001f:  ldc.i4.2
    IL_0020:  ldc.r8     2.2999999999999998
    IL_0029:  stelem.r8
    IL_002a:  dup
    IL_002b:  ldc.i4.3
    IL_002c:  ldc.r8     3.3999999999999999
    IL_0035:  stelem.r8
    IL_0036:  dup
    IL_0037:  ldc.i4.4
    IL_0038:  ldc.r8     4.0999999999999996
    IL_0041:  stelem.r8
    IL_0042:  dup
    IL_0043:  ldc.i4.5
    IL_0044:  ldc.r8     5.9000000000000004
    IL_004d:  stelem.r8
    IL_004e:  stloc.0
    IL_004f:  ldloc.0
    IL_0050:  ldc.i4.0
    IL_0051:  ldelema    [runtime]System.Double
    IL_0056:  stloc.2
    IL_0057:  ldloc.2
    IL_0058:  conv.i
    IL_0059:  stloc.1
    IL_005a:  ldloc.1
    IL_005b:  ldc.i4.0
    IL_005c:  conv.i
    IL_005d:  sizeof     [runtime]System.Double
    IL_0063:  mul
    IL_0064:  add
    IL_0065:  ldobj      [runtime]System.Double
    IL_006a:  ldloc.1
    IL_006b:  ldc.i4.1
    IL_006c:  conv.i
    IL_006d:  sizeof     [runtime]System.Double
    IL_0073:  mul
    IL_0074:  add
    IL_0075:  ldobj      [runtime]System.Double
    IL_007a:  add
    IL_007b:  ret
  } 

  .method public static class [runtime]System.Tuple`2<char,char> pinString() cil managed
  {
    
    .maxstack  6
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "Hello World"
    IL_0006:  callvirt   instance char& modreq([runtime]System.Runtime.InteropServices.InAttribute) [runtime]System.String::GetPinnableReference()
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  conv.i
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  conv.i
    IL_0012:  sizeof     [runtime]System.Char
    IL_0018:  mul
    IL_0019:  add
    IL_001a:  ldobj      [runtime]System.Char
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i
    IL_0022:  sizeof     [runtime]System.Char
    IL_0028:  mul
    IL_0029:  add
    IL_002a:  ldobj      [runtime]System.Char
    IL_002f:  newobj     instance void class [runtime]System.Tuple`2<char,char>::.ctor(!0,
                                                                                              !1)
    IL_0034:  ret
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





