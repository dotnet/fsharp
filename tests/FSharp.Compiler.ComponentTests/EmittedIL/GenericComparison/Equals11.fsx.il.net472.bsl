




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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class sequential autochar serializable sealed nested public beforefieldinit SomeUnion
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion>,
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
      .method public static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion NewSomeUnion(int32 item1, int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  3
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion V_0)
        IL_0000:  ldloca.s   V_0
        IL_0002:  initobj    assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0008:  ldloca.s   V_0
        IL_000a:  ldarg.0
        IL_000b:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_0010:  ldloca.s   V_0
        IL_0012:  ldarg.1
        IL_0013:  stfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_0018:  ldloc.0
        IL_0019:  ret
      } 

      .method public hidebysig instance int32 get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion obj) cil managed
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
        IL_0009:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_000e:  stloc.2
        IL_000f:  ldarga.s   obj
        IL_0011:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
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
        IL_0034:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_0039:  stloc.2
        IL_003a:  ldarga.s   obj
        IL_003c:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
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
        IL_0002:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0007:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  pop
        IL_0009:  ldarg.0
        IL_000a:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_000f:  stloc.2
        IL_0010:  ldloca.s   V_0
        IL_0012:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
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
        IL_002f:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_0034:  stloc.2
        IL_0035:  ldloca.s   V_0
        IL_0037:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
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
        IL_000c:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
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
        IL_0021:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
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
        IL_0006:  call       instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_0008:  ldarga.s   obj
        IL_000a:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_000f:  bne.un.s   IL_0021

        IL_0011:  ldarg.0
        IL_0012:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_0017:  ldarga.s   obj
        IL_0019:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_001e:  ceq
        IL_0020:  ret

        IL_0021:  ldc.i4.0
        IL_0022:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0006:  brfalse.s  IL_0018

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldloc.0
        IL_0011:  ldarg.2
        IL_0012:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,
                                                                                                            class [runtime]System.Collections.IEqualityComparer)
        IL_0017:  ret

        IL_0018:  ldc.i4.0
        IL_0019:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_0008:  ldarga.s   obj
        IL_000a:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item1
        IL_000f:  bne.un.s   IL_0021

        IL_0011:  ldarg.0
        IL_0012:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
        IL_0017:  ldarga.s   obj
        IL_0019:  ldfld      int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::item2
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
        IL_0001:  isinst     assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_0006:  brfalse.s  IL_0015

        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  unbox.any  assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
        IL_000f:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::get_Tag()
      } 
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::get_Item1()
      } 
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::get_Item2()
      } 
    } 

    .field static assembly bool arg@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion x@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion y@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method assembly specialname static bool get_arg@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion get_x@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion get_y@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ldc.i4.2
      IL_0002:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::NewSomeUnion(int32,
                                                                                                                                                                     int32)
      IL_0007:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_000c:  ldc.i4.2
      IL_000d:  ldc.i4.3
      IL_000e:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::NewSomeUnion(int32,
                                                                                                                                                                     int32)
      IL_0013:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0018:  ldsflda    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_001d:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
      IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0027:  call       instance bool assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion,
                                                                                                          class [runtime]System.Collections.IEqualityComparer)
      IL_002c:  stsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0031:  ret
    } 

    .property bool arg@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get bool assembly/EqualsMicroPerfAndCodeGenerationTests::get_arg@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
            x@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::get_x@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion
            y@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeUnion assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
    } 
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly/EqualsMicroPerfAndCodeGenerationTests::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






