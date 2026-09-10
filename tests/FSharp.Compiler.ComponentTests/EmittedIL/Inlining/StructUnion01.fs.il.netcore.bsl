




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
  .class sequential autochar serializable sealed nested public beforefieldinit U
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype assembly/U>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype assembly/U>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly int32 item1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly int32 item2
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static valuetype assembly/U NewU(int32 item1, int32 item2) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype assembly/U V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    assembly/U
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldarg.0
      IL_000b:  stfld      int32 assembly/U::item1
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.1
      IL_0013:  stfld      int32 assembly/U::item2
      IL_0018:  ldloc.0
      IL_0019:  ret
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
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      assembly/U
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>::Invoke(!0)
      IL_001a:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      assembly/U
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>::Invoke(!0)
      IL_001a:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (int32 V_0,
               class [runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0007:  stloc.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32 assembly/U::item1
      IL_000e:  stloc.2
      IL_000f:  ldarga.s   obj
      IL_0011:  ldfld      int32 assembly/U::item1
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  cgt
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  clt
      IL_001f:  sub
      IL_0020:  stloc.0
      IL_0021:  ldloc.0
      IL_0022:  ldc.i4.0
      IL_0023:  bge.s      IL_0027

      IL_0025:  ldloc.0
      IL_0026:  ret

      IL_0027:  ldloc.0
      IL_0028:  ldc.i4.0
      IL_0029:  ble.s      IL_002d

      IL_002b:  ldloc.0
      IL_002c:  ret

      IL_002d:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0032:  stloc.1
      IL_0033:  ldarg.0
      IL_0034:  ldfld      int32 assembly/U::item2
      IL_0039:  stloc.2
      IL_003a:  ldarga.s   obj
      IL_003c:  ldfld      int32 assembly/U::item2
      IL_0041:  stloc.3
      IL_0042:  ldloc.2
      IL_0043:  ldloc.3
      IL_0044:  cgt
      IL_0046:  ldloc.2
      IL_0047:  ldloc.3
      IL_0048:  clt
      IL_004a:  sub
      IL_004b:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/U
      IL_0007:  call       instance int32 assembly/U::CompareTo(valuetype assembly/U)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/U V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  pop
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 assembly/U::item1
      IL_000f:  stloc.2
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldfld      int32 assembly/U::item1
      IL_0017:  stloc.3
      IL_0018:  ldloc.2
      IL_0019:  ldloc.3
      IL_001a:  cgt
      IL_001c:  ldloc.2
      IL_001d:  ldloc.3
      IL_001e:  clt
      IL_0020:  sub
      IL_0021:  stloc.1
      IL_0022:  ldloc.1
      IL_0023:  ldc.i4.0
      IL_0024:  bge.s      IL_0028

      IL_0026:  ldloc.1
      IL_0027:  ret

      IL_0028:  ldloc.1
      IL_0029:  ldc.i4.0
      IL_002a:  ble.s      IL_002e

      IL_002c:  ldloc.1
      IL_002d:  ret

      IL_002e:  ldarg.0
      IL_002f:  ldfld      int32 assembly/U::item2
      IL_0034:  stloc.2
      IL_0035:  ldloca.s   V_0
      IL_0037:  ldfld      int32 assembly/U::item2
      IL_003c:  stloc.3
      IL_003d:  ldloc.2
      IL_003e:  ldloc.3
      IL_003f:  cgt
      IL_0041:  ldloc.2
      IL_0042:  ldloc.3
      IL_0043:  clt
      IL_0045:  sub
      IL_0046:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  pop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.0
      IL_0006:  ldc.i4     0x9e3779b9
      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 assembly/U::item2
      IL_0011:  ldloc.0
      IL_0012:  ldc.i4.6
      IL_0013:  shl
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.2
      IL_0016:  shr
      IL_0017:  add
      IL_0018:  add
      IL_0019:  add
      IL_001a:  stloc.0
      IL_001b:  ldc.i4     0x9e3779b9
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 assembly/U::item1
      IL_0026:  ldloc.0
      IL_0027:  ldc.i4.6
      IL_0028:  shl
      IL_0029:  ldloc.0
      IL_002a:  ldc.i4.2
      IL_002b:  shr
      IL_002c:  add
      IL_002d:  add
      IL_002e:  add
      IL_002f:  stloc.0
      IL_0030:  ldloc.0
      IL_0031:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 assembly/U::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(valuetype assembly/U obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 assembly/U::item1
      IL_0008:  ldarga.s   obj
      IL_000a:  ldfld      int32 assembly/U::item1
      IL_000f:  bne.un.s   IL_0021

      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 assembly/U::item2
      IL_0017:  ldarga.s   obj
      IL_0019:  ldfld      int32 assembly/U::item2
      IL_001e:  ceq
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  brfalse.s  IL_0018

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/U
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  ldarg.2
      IL_0012:  call       instance bool assembly/U::Equals(valuetype assembly/U,
                                                                 class [runtime]System.Collections.IEqualityComparer)
      IL_0017:  ret

      IL_0018:  ldc.i4.0
      IL_0019:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 assembly/U::item1
      IL_0008:  ldarga.s   obj
      IL_000a:  ldfld      int32 assembly/U::item1
      IL_000f:  bne.un.s   IL_0021

      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 assembly/U::item2
      IL_0017:  ldarga.s   obj
      IL_0019:  ldfld      int32 assembly/U::item2
      IL_001e:  ceq
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  brfalse.s  IL_0015

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  unbox.any  assembly/U
      IL_000f:  call       instance bool assembly/U::Equals(valuetype assembly/U)
      IL_0014:  ret

      IL_0015:  ldc.i4.0
      IL_0016:  ret
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

  .method public static int32  g1(valuetype assembly/U _arg1) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarga.s   _arg1
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldarga.s   _arg1
    IL_0009:  ldfld      int32 assembly/U::item2
    IL_000e:  add
    IL_000f:  ret
  } 

  .method public static int32  g2(valuetype assembly/U u) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarga.s   u
    IL_0003:  ldfld      int32 assembly/U::item1
    IL_0008:  ldarga.s   u
    IL_000a:  ldfld      int32 assembly/U::item2
    IL_000f:  add
    IL_0010:  ret
  } 

  .method public static int32  g3(valuetype assembly/U x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     ( 
                          IL_0014)
    IL_0012:  br.s       IL_001c

    IL_0014:  ldarga.s   x
    IL_0016:  ldfld      int32 assembly/U::item2
    IL_001b:  ret

    IL_001c:  ldarga.s   x
    IL_001e:  ldfld      int32 assembly/U::item1
    IL_0023:  ldarga.s   x
    IL_0025:  ldfld      int32 assembly/U::item2
    IL_002a:  add
    IL_002b:  ret
  } 

  .method public static int32  g4(valuetype assembly/U x,
                                  valuetype assembly/U y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     ( 
                          IL_0014)
    IL_0012:  br.s       IL_0058

    IL_0014:  ldarga.s   y
    IL_0016:  ldfld      int32 assembly/U::item1
    IL_001b:  ldc.i4.5
    IL_001c:  sub
    IL_001d:  switch     ( 
                          IL_0048)
    IL_0026:  ldarga.s   y
    IL_0028:  ldfld      int32 assembly/U::item2
    IL_002d:  ldarga.s   y
    IL_002f:  ldfld      int32 assembly/U::item1
    IL_0034:  ldarga.s   x
    IL_0036:  ldfld      int32 assembly/U::item2
    IL_003b:  ldarga.s   x
    IL_003d:  ldfld      int32 assembly/U::item1
    IL_0042:  stloc.3
    IL_0043:  stloc.2
    IL_0044:  stloc.1
    IL_0045:  stloc.0
    IL_0046:  br.s       IL_0078

    IL_0048:  ldarga.s   x
    IL_004a:  ldfld      int32 assembly/U::item2
    IL_004f:  ldarga.s   y
    IL_0051:  ldfld      int32 assembly/U::item2
    IL_0056:  add
    IL_0057:  ret

    IL_0058:  ldarga.s   y
    IL_005a:  ldfld      int32 assembly/U::item2
    IL_005f:  stloc.0
    IL_0060:  ldarga.s   y
    IL_0062:  ldfld      int32 assembly/U::item1
    IL_0067:  stloc.1
    IL_0068:  ldarga.s   x
    IL_006a:  ldfld      int32 assembly/U::item2
    IL_006f:  stloc.2
    IL_0070:  ldarga.s   x
    IL_0072:  ldfld      int32 assembly/U::item1
    IL_0077:  stloc.3
    IL_0078:  ldloc.3
    IL_0079:  ldloc.2
    IL_007a:  add
    IL_007b:  ldloc.1
    IL_007c:  add
    IL_007d:  ldloc.0
    IL_007e:  add
    IL_007f:  ret
  } 

  .method public static int32  f1(valuetype assembly/U& x) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 assembly/U::item2
    IL_000d:  add
    IL_000e:  ret
  } 

  .method public static int32  f2(valuetype assembly/U& x) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 assembly/U::item2
    IL_000d:  add
    IL_000e:  ret
  } 

  .method public static int32  f3(valuetype assembly/U& x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 assembly/U::item1
    IL_0006:  ldc.i4.3
    IL_0007:  sub
    IL_0008:  switch     ( 
                          IL_0013)
    IL_0011:  br.s       IL_001a

    IL_0013:  ldarg.0
    IL_0014:  ldfld      int32 assembly/U::item2
    IL_0019:  ret

    IL_001a:  ldarg.0
    IL_001b:  ldfld      int32 assembly/U::item1
    IL_0020:  ldarg.0
    IL_0021:  ldfld      int32 assembly/U::item2
    IL_0026:  add
    IL_0027:  ret
  } 

  .method public static int32  f4(valuetype assembly/U& x,
                                  valuetype assembly/U& y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (valuetype assembly/U V_0,
             valuetype assembly/U V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      assembly/U
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldobj      assembly/U
    IL_000d:  stloc.1
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldfld      int32 assembly/U::item1
    IL_0015:  ldc.i4.3
    IL_0016:  sub
    IL_0017:  switch     ( 
                          IL_0022)
    IL_0020:  br.s       IL_0068

    IL_0022:  ldloca.s   V_1
    IL_0024:  ldfld      int32 assembly/U::item1
    IL_0029:  ldc.i4.5
    IL_002a:  sub
    IL_002b:  switch     ( 
                          IL_0058)
    IL_0034:  ldloca.s   V_1
    IL_0036:  ldfld      int32 assembly/U::item2
    IL_003b:  ldloca.s   V_1
    IL_003d:  ldfld      int32 assembly/U::item1
    IL_0042:  ldloca.s   V_0
    IL_0044:  ldfld      int32 assembly/U::item2
    IL_0049:  ldloca.s   V_0
    IL_004b:  ldfld      int32 assembly/U::item1
    IL_0050:  stloc.s    V_5
    IL_0052:  stloc.s    V_4
    IL_0054:  stloc.3
    IL_0055:  stloc.2
    IL_0056:  br.s       IL_008a

    IL_0058:  ldloca.s   V_0
    IL_005a:  ldfld      int32 assembly/U::item2
    IL_005f:  ldloca.s   V_1
    IL_0061:  ldfld      int32 assembly/U::item2
    IL_0066:  add
    IL_0067:  ret

    IL_0068:  ldloca.s   V_1
    IL_006a:  ldfld      int32 assembly/U::item2
    IL_006f:  stloc.2
    IL_0070:  ldloca.s   V_1
    IL_0072:  ldfld      int32 assembly/U::item1
    IL_0077:  stloc.3
    IL_0078:  ldloca.s   V_0
    IL_007a:  ldfld      int32 assembly/U::item2
    IL_007f:  stloc.s    V_4
    IL_0081:  ldloca.s   V_0
    IL_0083:  ldfld      int32 assembly/U::item1
    IL_0088:  stloc.s    V_5
    IL_008a:  ldloc.s    V_5
    IL_008c:  ldloc.s    V_4
    IL_008e:  add
    IL_008f:  ldloc.3
    IL_0090:  add
    IL_0091:  ldloc.2
    IL_0092:  add
    IL_0093:  ret
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





