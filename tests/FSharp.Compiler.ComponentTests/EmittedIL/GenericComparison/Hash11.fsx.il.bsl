




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
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit arr@1
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
    {
      .field static assembly initonly class assembly/HashMicroPerfAndCodeGenerationTests/arr@1 @_instance
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance int32 
              Invoke(int32 i) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldc.i4.0
        IL_0001:  ldarg.1
        IL_0002:  add
        IL_0003:  ret
      } 

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void assembly/HashMicroPerfAndCodeGenerationTests/arr@1::.ctor()
        IL_0005:  stsfld     class assembly/HashMicroPerfAndCodeGenerationTests/arr@1 assembly/HashMicroPerfAndCodeGenerationTests/arr@1::@_instance
        IL_000a:  ret
      } 

    } 

    .method public static void  f8() cil managed
    {
      
      .maxstack  6
      .locals init (int32[] V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  nop
      IL_0001:  ldc.i4.s   100
      IL_0003:  ldc.i4.0
      IL_0004:  sub
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  ldc.i4.s   100
      IL_0009:  ldc.i4.0
      IL_000a:  sub
      IL_000b:  ldc.i4.1
      IL_000c:  add
      IL_000d:  ldc.i4.s   100
      IL_000f:  ldc.i4.0
      IL_0010:  sub
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  ldc.i4.0
      IL_0014:  clt
      IL_0016:  neg
      IL_0017:  and
      IL_0018:  xor
      IL_0019:  ldsfld     class assembly/HashMicroPerfAndCodeGenerationTests/arr@1 assembly/HashMicroPerfAndCodeGenerationTests/arr@1::@_instance
      IL_001e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_0023:  stloc.0
      IL_0024:  ldc.i4.0
      IL_0025:  stloc.1
      IL_0026:  br.s       IL_0033

      IL_0028:  ldloc.0
      IL_0029:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<int32[]>(!!0)
      IL_002e:  stloc.2
      IL_002f:  ldloc.1
      IL_0030:  ldc.i4.1
      IL_0031:  add
      IL_0032:  stloc.1
      IL_0033:  ldloc.1
      IL_0034:  ldc.i4     0x989681
      IL_0039:  blt.s      IL_0028

      IL_003b:  ret
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






