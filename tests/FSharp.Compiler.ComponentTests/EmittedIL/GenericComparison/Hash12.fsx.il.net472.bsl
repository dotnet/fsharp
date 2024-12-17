




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
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class assembly/HashMicroPerfAndCodeGenerationTests/Key>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class assembly/HashMicroPerfAndCodeGenerationTests/Key>,
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
      .method public static class assembly/HashMicroPerfAndCodeGenerationTests/Key NewKey(int32 item1, int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void assembly/HashMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
                                                                                                 int32)
        IL_0007:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(int32 item1, int32 item2) cil managed
      {
        .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 2E 48 61 73 68 31 32 2B 48 61   
                                                                                                                                 73 68 4D 69 63 72 6F 50 65 72 66 41 6E 64 43 6F   
                                                                                                                                 64 65 47 65 6E 65 72 61 74 69 6F 6E 54 65 73 74   
                                                                                                                                 73 2B 4B 65 79 00 00 )                            
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0014:  ret
      } 

      .method public hidebysig instance int32 get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 int32 V_2,
                 class [runtime]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0062

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0060

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.3
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
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
        IL_0041:  stloc.3
        IL_0042:  ldloc.0
        IL_0043:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.1
        IL_004b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  brfalse.s  IL_0067

        IL_0065:  ldc.i4.m1
        IL_0066:  ret

        IL_0067:  ldc.i4.0
        IL_0068:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0007:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/Key)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0062

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0010:  brfalse.s  IL_0060

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  cgt
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  clt
        IL_0034:  sub
        IL_0035:  stloc.3
        IL_0036:  ldloc.3
        IL_0037:  ldc.i4.0
        IL_0038:  bge.s      IL_003c

        IL_003a:  ldloc.3
        IL_003b:  ret

        IL_003c:  ldloc.3
        IL_003d:  ldc.i4.0
        IL_003e:  ble.s      IL_0042

        IL_0040:  ldloc.3
        IL_0041:  ret

        IL_0042:  ldloc.1
        IL_0043:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.2
        IL_004b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0068:  brfalse.s  IL_006c

        IL_006a:  ldc.i4.m1
        IL_006b:  ret

        IL_006c:  ldc.i4.0
        IL_006d:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0016:  ldloc.0
        IL_0017:  ldc.i4.6
        IL_0018:  shl
        IL_0019:  ldloc.0
        IL_001a:  ldc.i4.2
        IL_001b:  shr
        IL_001c:  add
        IL_001d:  add
        IL_001e:  add
        IL_001f:  stloc.0
        IL_0020:  ldc.i4     0x9e3779b9
        IL_0025:  ldloc.1
        IL_0026:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_002b:  ldloc.0
        IL_002c:  ldc.i4.6
        IL_002d:  shl
        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.2
        IL_0030:  shr
        IL_0031:  add
        IL_0032:  add
        IL_0033:  add
        IL_0034:  stloc.0
        IL_0035:  ldloc.0
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
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
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
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

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                  class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
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
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
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
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } 
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } 
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } 
    } 

    .class auto autochar serializable sealed nested public beforefieldinit KeyWithInnerKeys
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly class assembly/HashMicroPerfAndCodeGenerationTests/Key item1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> item2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys NewKeyWithInnerKeys(class assembly/HashMicroPerfAndCodeGenerationTests/Key item1, class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::.ctor(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                              class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>)
        IL_0007:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(class assembly/HashMicroPerfAndCodeGenerationTests/Key item1, class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> item2) cil managed
      {
        .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 3B 48 61 73 68 31 32 2B 48 61   
                                                                                                                                 73 68 4D 69 63 72 6F 50 65 72 66 41 6E 64 43 6F   
                                                                                                                                 64 65 47 65 6E 65 72 61 74 69 6F 6E 54 65 73 74   
                                                                                                                                 73 2B 4B 65 79 57 69 74 68 49 6E 6E 65 72 4B 65   
                                                                                                                                 79 73 00 00 )                                     
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0014:  ret
      } 

      .method public hidebysig instance class assembly/HashMicroPerfAndCodeGenerationTests/Key get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 int32 V_2,
                 class [runtime]System.Collections.IComparer V_3,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_5,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_6,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_7,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_8,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_9,
                 int32 V_10)
        IL_0000:  ldarg.0
        IL_0001:  brfalse    IL_0099

        IL_0006:  ldarg.1
        IL_0007:  brfalse    IL_0097

        IL_000c:  ldarg.0
        IL_000d:  pop
        IL_000e:  ldarg.0
        IL_000f:  stloc.0
        IL_0010:  ldarg.1
        IL_0011:  stloc.1
        IL_0012:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0017:  stloc.3
        IL_0018:  ldloc.0
        IL_0019:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.1
        IL_0021:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  ldloc.3
        IL_002d:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
        IL_0032:  stloc.2
        IL_0033:  ldloc.2
        IL_0034:  ldc.i4.0
        IL_0035:  bge.s      IL_0039

        IL_0037:  ldloc.2
        IL_0038:  ret

        IL_0039:  ldloc.2
        IL_003a:  ldc.i4.0
        IL_003b:  ble.s      IL_003f

        IL_003d:  ldloc.2
        IL_003e:  ret

        IL_003f:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0044:  stloc.3
        IL_0045:  ldloc.0
        IL_0046:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_004b:  stloc.s    V_6
        IL_004d:  ldloc.1
        IL_004e:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0053:  stloc.s    V_7
        IL_0055:  ldloc.s    V_6
        IL_0057:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_005c:  stloc.s    V_4
        IL_005e:  ldloc.s    V_6
        IL_0060:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0065:  stloc.s    V_5
        IL_0067:  ldloc.s    V_7
        IL_0069:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_006e:  stloc.s    V_8
        IL_0070:  ldloc.s    V_7
        IL_0072:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0077:  stloc.s    V_9
        IL_0079:  ldloc.s    V_4
        IL_007b:  ldloc.s    V_8
        IL_007d:  ldloc.3
        IL_007e:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
        IL_0083:  stloc.s    V_10
        IL_0085:  ldloc.s    V_10
        IL_0087:  brfalse.s  IL_008c

        IL_0089:  ldloc.s    V_10
        IL_008b:  ret

        IL_008c:  ldloc.s    V_5
        IL_008e:  ldloc.s    V_9
        IL_0090:  ldloc.3
        IL_0091:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
        IL_0096:  ret

        IL_0097:  ldc.i4.1
        IL_0098:  ret

        IL_0099:  ldarg.1
        IL_009a:  brfalse.s  IL_009e

        IL_009c:  ldc.i4.m1
        IL_009d:  ret

        IL_009e:  ldc.i4.0
        IL_009f:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0007:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_2,
                 int32 V_3,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_5,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_6,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_7,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_8,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_9,
                 int32 V_10)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse    IL_0099

        IL_000d:  ldarg.1
        IL_000e:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0013:  brfalse    IL_0097

        IL_0018:  ldarg.0
        IL_0019:  pop
        IL_001a:  ldarg.0
        IL_001b:  stloc.1
        IL_001c:  ldloc.0
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.2
        IL_0027:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_002c:  stloc.s    V_5
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  ldarg.2
        IL_0033:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
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

        IL_0045:  ldloc.1
        IL_0046:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_004b:  stloc.s    V_6
        IL_004d:  ldloc.2
        IL_004e:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0053:  stloc.s    V_7
        IL_0055:  ldloc.s    V_6
        IL_0057:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_005c:  stloc.s    V_4
        IL_005e:  ldloc.s    V_6
        IL_0060:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0065:  stloc.s    V_5
        IL_0067:  ldloc.s    V_7
        IL_0069:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_006e:  stloc.s    V_8
        IL_0070:  ldloc.s    V_7
        IL_0072:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0077:  stloc.s    V_9
        IL_0079:  ldloc.s    V_4
        IL_007b:  ldloc.s    V_8
        IL_007d:  ldarg.2
        IL_007e:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
        IL_0083:  stloc.s    V_10
        IL_0085:  ldloc.s    V_10
        IL_0087:  brfalse.s  IL_008c

        IL_0089:  ldloc.s    V_10
        IL_008b:  ret

        IL_008c:  ldloc.s    V_5
        IL_008e:  ldloc.s    V_9
        IL_0090:  ldarg.2
        IL_0091:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(object,
                                                                                                      class [runtime]System.Collections.IComparer)
        IL_0096:  ret

        IL_0097:  ldc.i4.1
        IL_0098:  ret

        IL_0099:  ldarg.1
        IL_009a:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_009f:  brfalse.s  IL_00a3

        IL_00a1:  ldc.i4.m1
        IL_00a2:  ret

        IL_00a3:  ldc.i4.0
        IL_00a4:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_2,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_3,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0066

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0016:  stloc.2
        IL_0017:  ldloc.2
        IL_0018:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_001d:  stloc.3
        IL_001e:  ldloc.2
        IL_001f:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0024:  stloc.s    V_4
        IL_0026:  ldloc.3
        IL_0027:  ldarg.1
        IL_0028:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_002d:  stloc.s    V_5
        IL_002f:  ldloc.s    V_5
        IL_0031:  ldc.i4.5
        IL_0032:  shl
        IL_0033:  ldloc.s    V_5
        IL_0035:  add
        IL_0036:  ldloc.s    V_4
        IL_0038:  ldarg.1
        IL_0039:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_003e:  xor
        IL_003f:  ldloc.0
        IL_0040:  ldc.i4.6
        IL_0041:  shl
        IL_0042:  ldloc.0
        IL_0043:  ldc.i4.2
        IL_0044:  shr
        IL_0045:  add
        IL_0046:  add
        IL_0047:  add
        IL_0048:  stloc.0
        IL_0049:  ldc.i4     0x9e3779b9
        IL_004e:  ldloc.1
        IL_004f:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0054:  ldarg.1
        IL_0055:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_005a:  ldloc.0
        IL_005b:  ldc.i4.6
        IL_005c:  shl
        IL_005d:  ldloc.0
        IL_005e:  ldc.i4.2
        IL_005f:  shr
        IL_0060:  add
        IL_0061:  add
        IL_0062:  add
        IL_0063:  stloc.0
        IL_0064:  ldloc.0
        IL_0065:  ret

        IL_0066:  ldc.i4.0
        IL_0067:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_2,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_3,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_4,
                 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> V_5,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_6,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_7)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0071

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_006f

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0012:  stloc.2
        IL_0013:  ldloc.1
        IL_0014:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0019:  stloc.3
        IL_001a:  ldloc.2
        IL_001b:  ldloc.3
        IL_001c:  ldarg.2
        IL_001d:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                  class [runtime]System.Collections.IEqualityComparer)
        IL_0022:  brfalse.s  IL_006d

        IL_0024:  ldloc.0
        IL_0025:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_002a:  stloc.s    V_4
        IL_002c:  ldloc.1
        IL_002d:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0032:  stloc.s    V_5
        IL_0034:  ldloc.s    V_4
        IL_0036:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_003b:  stloc.2
        IL_003c:  ldloc.s    V_4
        IL_003e:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0043:  stloc.3
        IL_0044:  ldloc.s    V_5
        IL_0046:  call       instance !0 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item1()
        IL_004b:  stloc.s    V_6
        IL_004d:  ldloc.s    V_5
        IL_004f:  call       instance !1 class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::get_Item2()
        IL_0054:  stloc.s    V_7
        IL_0056:  ldloc.2
        IL_0057:  ldloc.s    V_6
        IL_0059:  ldarg.2
        IL_005a:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                  class [runtime]System.Collections.IEqualityComparer)
        IL_005f:  brfalse.s  IL_006b

        IL_0061:  ldloc.3
        IL_0062:  ldloc.s    V_7
        IL_0064:  ldarg.2
        IL_0065:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                  class [runtime]System.Collections.IEqualityComparer)
        IL_006a:  ret

        IL_006b:  ldc.i4.0
        IL_006c:  ret

        IL_006d:  ldc.i4.0
        IL_006e:  ret

        IL_006f:  ldc.i4.0
        IL_0070:  ret

        IL_0071:  ldarg.1
        IL_0072:  ldnull
        IL_0073:  cgt.un
        IL_0075:  ldc.i4.0
        IL_0076:  ceq
        IL_0078:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys,
                                                                                                               class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0035

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item1
        IL_0018:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key)
        IL_001d:  brfalse.s  IL_0033

        IL_001f:  ldloc.0
        IL_0020:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_0025:  ldloc.1
        IL_0026:  ldfld      class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::item2
        IL_002b:  tail.
        IL_002d:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>>(!!0,
                                                                                                                                                                                                                                                                           !!0)
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
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0014

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  tail.
        IL_000e:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys)
        IL_0013:  ret

        IL_0014:  ldc.i4.0
        IL_0015:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Tag()
      } 
      .property instance class assembly/HashMicroPerfAndCodeGenerationTests/Key
              Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item1()
      } 
      .property instance class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>
              Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key> assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::get_Item2()
      } 
    } 

    .method public static void  f9() cil managed
    {
      
      .maxstack  6
      .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  ldc.i4.2
      IL_0003:  call       class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_0008:  ldc.i4.1
      IL_0009:  ldc.i4.2
      IL_000a:  call       class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_000f:  ldc.i4.1
      IL_0010:  ldc.i4.2
      IL_0011:  call       class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_0016:  newobj     instance void class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::.ctor(!0,
                                                                                                                                                                                          !1)
      IL_001b:  call       class assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::NewKeyWithInnerKeys(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                                                                                              class [runtime]System.Tuple`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,class assembly/HashMicroPerfAndCodeGenerationTests/Key>)
      IL_0020:  stloc.0
      IL_0021:  ldc.i4.0
      IL_0022:  stloc.1
      IL_0023:  br.s       IL_0035

      IL_0025:  ldloc.0
      IL_0026:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityERComparer()
      IL_002b:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyWithInnerKeys::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_0030:  stloc.2
      IL_0031:  ldloc.1
      IL_0032:  ldc.i4.1
      IL_0033:  add
      IL_0034:  stloc.1
      IL_0035:  ldloc.1
      IL_0036:  ldc.i4     0x989681
      IL_003b:  blt.s      IL_0025

      IL_003d:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
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






