




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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit t1@1
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
    {
      .field static assembly initonly class assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1 @_instance
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
        IL_0000:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1::.ctor()
        IL_0005:  stsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1::@_instance
        IL_000a:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit t2@1
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
    {
      .field static assembly initonly class assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1 @_instance
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
        IL_0000:  newobj     instance void assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1::.ctor()
        IL_0005:  stsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1::@_instance
        IL_000a:  ret
      } 

    } 

    .method public static bool  f8() cil managed
    {
      
      .maxstack  4
      .locals init (bool V_0,
               int32[] V_1,
               int32[] V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   101
      IL_0004:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1::@_instance
      IL_0009:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_000e:  stloc.1
      IL_000f:  ldc.i4.s   101
      IL_0011:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1::@_instance
      IL_0016:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_001b:  stloc.2
      IL_001c:  ldc.i4.0
      IL_001d:  stloc.3
      IL_001e:  br.s       IL_002c

      IL_0020:  ldloc.1
      IL_0021:  ldloc.2
      IL_0022:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<int32[]>(!!0,
                                                                                                                                     !!0)
      IL_0027:  stloc.0
      IL_0028:  ldloc.3
      IL_0029:  ldc.i4.1
      IL_002a:  add
      IL_002b:  stloc.3
      IL_002c:  ldloc.3
      IL_002d:  ldc.i4     0x989681
      IL_0032:  blt.s      IL_0020

      IL_0034:  ldloc.0
      IL_0035:  ret
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






