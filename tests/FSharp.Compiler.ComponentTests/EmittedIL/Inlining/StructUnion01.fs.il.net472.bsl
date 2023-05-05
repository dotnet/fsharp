




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
    .method public static valuetype assembly/U
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
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 0F 53 74 72 75 63 74 55 6E 69
                                                                                                                               6F 6E 30 31 2B 55 00 00 )
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/U::item1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/U::item2
      IL_000e:  ret
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
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      assembly/U
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/U,string>::Invoke(!0)
      IL_001a:  ret
    }

    .method public strict virtual instance string
            ToString() cil managed
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

    .method public hidebysig virtual final
            instance int32  CompareTo(valuetype assembly/U obj) cil managed
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

    .method public hidebysig virtual final
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/U
      IL_0007:  call       instance int32 assembly/U::CompareTo(valuetype assembly/U)
      IL_000c:  ret
    }

    .method public hidebysig virtual final
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
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

    .method public hidebysig virtual final
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
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

    .method public hidebysig virtual final
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 assembly/U::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    }

    .method public hidebysig virtual final
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  4
      .locals init (valuetype assembly/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  brfalse.s  IL_0032

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/U
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  pop
      IL_0011:  ldarg.0
      IL_0012:  ldfld      int32 assembly/U::item1
      IL_0017:  ldloca.s   V_0
      IL_0019:  ldfld      int32 assembly/U::item1
      IL_001e:  bne.un.s   IL_0030

      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 assembly/U::item2
      IL_0026:  ldloca.s   V_0
      IL_0028:  ldfld      int32 assembly/U::item2
      IL_002d:  ceq
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret

      IL_0032:  ldc.i4.0
      IL_0033:  ret
    }

    .method public hidebysig virtual final
            instance bool  Equals(valuetype assembly/U obj) cil managed
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

    .method public hidebysig virtual final
            instance bool  Equals(object obj) cil managed
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
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      int32 assembly/U::item1
    IL_0008:  ldc.i4.3
    IL_0009:  sub
    IL_000a:  switch     (
                          IL_0015)
    IL_0013:  br.s       IL_001d

    IL_0015:  ldarga.s   x
    IL_0017:  ldfld      int32 assembly/U::item2
    IL_001c:  ret

    IL_001d:  ldarga.s   x
    IL_001f:  ldfld      int32 assembly/U::item1
    IL_0024:  ldarga.s   x
    IL_0026:  ldfld      int32 assembly/U::item2
    IL_002b:  add
    IL_002c:  ret
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
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      int32 assembly/U::item1
    IL_0008:  ldc.i4.3
    IL_0009:  sub
    IL_000a:  switch     (
                          IL_0015)
    IL_0013:  br.s       IL_0059

    IL_0015:  ldarga.s   y
    IL_0017:  ldfld      int32 assembly/U::item1
    IL_001c:  ldc.i4.5
    IL_001d:  sub
    IL_001e:  switch     (
                          IL_0049)
    IL_0027:  ldarga.s   y
    IL_0029:  ldfld      int32 assembly/U::item2
    IL_002e:  ldarga.s   y
    IL_0030:  ldfld      int32 assembly/U::item1
    IL_0035:  ldarga.s   x
    IL_0037:  ldfld      int32 assembly/U::item2
    IL_003c:  ldarga.s   x
    IL_003e:  ldfld      int32 assembly/U::item1
    IL_0043:  stloc.3
    IL_0044:  stloc.2
    IL_0045:  stloc.1
    IL_0046:  stloc.0
    IL_0047:  br.s       IL_0079

    IL_0049:  ldarga.s   x
    IL_004b:  ldfld      int32 assembly/U::item2
    IL_0050:  ldarga.s   y
    IL_0052:  ldfld      int32 assembly/U::item2
    IL_0057:  add
    IL_0058:  ret

    IL_0059:  ldarga.s   y
    IL_005b:  ldfld      int32 assembly/U::item2
    IL_0060:  stloc.0
    IL_0061:  ldarga.s   y
    IL_0063:  ldfld      int32 assembly/U::item1
    IL_0068:  stloc.1
    IL_0069:  ldarga.s   x
    IL_006b:  ldfld      int32 assembly/U::item2
    IL_0070:  stloc.2
    IL_0071:  ldarga.s   x
    IL_0073:  ldfld      int32 assembly/U::item1
    IL_0078:  stloc.3
    IL_0079:  ldloc.3
    IL_007a:  ldloc.2
    IL_007b:  add
    IL_007c:  ldloc.1
    IL_007d:  add
    IL_007e:  ldloc.0
    IL_007f:  add
    IL_0080:  ret
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
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 assembly/U::item1
    IL_0007:  ldc.i4.3
    IL_0008:  sub
    IL_0009:  switch     (
                          IL_0014)
    IL_0012:  br.s       IL_001b

    IL_0014:  ldarg.0
    IL_0015:  ldfld      int32 assembly/U::item2
    IL_001a:  ret

    IL_001b:  ldarg.0
    IL_001c:  ldfld      int32 assembly/U::item1
    IL_0021:  ldarg.0
    IL_0022:  ldfld      int32 assembly/U::item2
    IL_0027:  add
    IL_0028:  ret
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
    IL_000e:  nop
    IL_000f:  ldloca.s   V_0
    IL_0011:  ldfld      int32 assembly/U::item1
    IL_0016:  ldc.i4.3
    IL_0017:  sub
    IL_0018:  switch     (
                          IL_0023)
    IL_0021:  br.s       IL_0069

    IL_0023:  ldloca.s   V_1
    IL_0025:  ldfld      int32 assembly/U::item1
    IL_002a:  ldc.i4.5
    IL_002b:  sub
    IL_002c:  switch     (
                          IL_0059)
    IL_0035:  ldloca.s   V_1
    IL_0037:  ldfld      int32 assembly/U::item2
    IL_003c:  ldloca.s   V_1
    IL_003e:  ldfld      int32 assembly/U::item1
    IL_0043:  ldloca.s   V_0
    IL_0045:  ldfld      int32 assembly/U::item2
    IL_004a:  ldloca.s   V_0
    IL_004c:  ldfld      int32 assembly/U::item1
    IL_0051:  stloc.s    V_5
    IL_0053:  stloc.s    V_4
    IL_0055:  stloc.3
    IL_0056:  stloc.2
    IL_0057:  br.s       IL_008b

    IL_0059:  ldloca.s   V_0
    IL_005b:  ldfld      int32 assembly/U::item2
    IL_0060:  ldloca.s   V_1
    IL_0062:  ldfld      int32 assembly/U::item2
    IL_0067:  add
    IL_0068:  ret

    IL_0069:  ldloca.s   V_1
    IL_006b:  ldfld      int32 assembly/U::item2
    IL_0070:  stloc.2
    IL_0071:  ldloca.s   V_1
    IL_0073:  ldfld      int32 assembly/U::item1
    IL_0078:  stloc.3
    IL_0079:  ldloca.s   V_0
    IL_007b:  ldfld      int32 assembly/U::item2
    IL_0080:  stloc.s    V_4
    IL_0082:  ldloca.s   V_0
    IL_0084:  ldfld      int32 assembly/U::item1
    IL_0089:  stloc.s    V_5
    IL_008b:  ldloc.s    V_5
    IL_008d:  ldloc.s    V_4
    IL_008f:  add
    IL_0090:  ldloc.3
    IL_0091:  add
    IL_0092:  ldloc.2
    IL_0093:  add
    IL_0094:  ret
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

.class private auto ansi sealed System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
       extends [runtime]System.Enum
{
  .custom instance void [runtime]System.FlagsAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
  .field public specialname rtspecialname int32 value__ = int32(0x00000000)
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
  .field private valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
  .field private class [runtime]System.Type Type@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
  .method public specialname rtspecialname
          instance void  .ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType,
                               class [runtime]System.Type Type) cil managed
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

  .method public hidebysig specialname instance class [runtime]System.Type
          get_Type() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::Type@
    IL_0006:  ret
  }

  .method public hidebysig specialname instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
          get_MemberType() cil managed
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
