




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
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
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public KeyR
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class assembly/HashMicroPerfAndCodeGenerationTests/KeyR>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class assembly/HashMicroPerfAndCodeGenerationTests/KeyR>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .field assembly int32 key1@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field assembly int32 key2@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public hidebysig specialname instance int32  get_key1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0006:  ret
      } 

      .method public hidebysig specialname instance int32  get_key2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0006:  ret
      } 

      .method public specialname rtspecialname 
              instance void  .ctor(int32 key1,
                                   int32 key2) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 2F 48 61 73 68 30 38 2B 48 61   
                                                                                                                                                       73 68 4D 69 63 72 6F 50 65 72 66 41 6E 64 43 6F   
                                                                                                                                                       64 65 47 65 6E 65 72 61 74 69 6F 6E 54 65 73 74   
                                                                                                                                                       73 2B 4B 65 79 52 00 00 )                         
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_0014:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyR,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/HashMicroPerfAndCodeGenerationTests/KeyR>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyR,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/KeyR,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
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
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  stloc.2
        IL_0013:  ldarg.1
        IL_0014:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
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
        IL_0037:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_003c:  stloc.2
        IL_003d:  ldarg.1
        IL_003e:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
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
        IL_0002:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0007:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyR V_0,
                 int32 V_1,
                 int32 V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0050

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0010:  brfalse.s  IL_004e

        IL_0012:  ldarg.0
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0018:  stloc.2
        IL_0019:  ldloc.0
        IL_001a:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
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
        IL_0037:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_003c:  stloc.2
        IL_003d:  ldloc.0
        IL_003e:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
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
        IL_0051:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/KeyR
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
        IL_000b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
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
        IL_0020:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
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
        IL_0006:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool 
              Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR obj,
                     class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0025

        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000c:  ldarg.1
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  bne.un.s   IL_0023

        IL_0014:  ldarg.0
        IL_0015:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_001a:  ldarg.1
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
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

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/KeyR::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR,
                                                                                                   class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0025

        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_000c:  ldarg.1
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key1@
        IL_0012:  bne.un.s   IL_0023

        IL_0014:  ldarg.0
        IL_0015:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
        IL_001a:  ldarg.1
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::key2@
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
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/KeyR V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/KeyR
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/KeyR::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/KeyR)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } 

      .property instance int32 key1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::get_key1()
      } 
      .property instance int32 key2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::get_key2()
      } 
    } 

    .method public static void  f5c() cil managed
    {
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_001b

      IL_0005:  ldc.i4.1
      IL_0006:  ldc.i4.2
      IL_0007:  newobj     instance void assembly/HashMicroPerfAndCodeGenerationTests/KeyR::.ctor(int32,
                                                                                                int32)
      IL_000c:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityERComparer()
      IL_0011:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/KeyR::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_0016:  stloc.1
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.0
      IL_001b:  ldloc.0
      IL_001c:  ldc.i4     0x989681
      IL_0021:  blt.s      IL_0005

      IL_0023:  ret
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






