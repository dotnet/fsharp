




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
      IL_0002:  nop
      IL_0003:  ldc.i4.s   100
      IL_0005:  ldc.i4.0
      IL_0006:  bge.s      IL_0010

      IL_0008:  call       !!0[] [runtime]System.Array::Empty<int32>()
      IL_000d:  nop
      IL_000e:  br.s       IL_0021

      IL_0010:  ldc.i4.s   100
      IL_0012:  ldc.i4.0
      IL_0013:  sub
      IL_0014:  ldc.i4.1
      IL_0015:  add
      IL_0016:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1::@_instance
      IL_001b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_0020:  nop
      IL_0021:  stloc.1
      IL_0022:  nop
      IL_0023:  ldc.i4.s   100
      IL_0025:  ldc.i4.0
      IL_0026:  bge.s      IL_0030

      IL_0028:  call       !!0[] [runtime]System.Array::Empty<int32>()
      IL_002d:  nop
      IL_002e:  br.s       IL_0041

      IL_0030:  ldc.i4.s   100
      IL_0032:  ldc.i4.0
      IL_0033:  sub
      IL_0034:  ldc.i4.1
      IL_0035:  add
      IL_0036:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1::@_instance
      IL_003b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_0040:  nop
      IL_0041:  stloc.2
      IL_0042:  ldc.i4.0
      IL_0043:  stloc.3
      IL_0044:  br.s       IL_0052

      IL_0046:  ldloc.1
      IL_0047:  ldloc.2
      IL_0048:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<int32[]>(!!0,
                                                                                                                                     !!0)
      IL_004d:  stloc.0
      IL_004e:  ldloc.3
      IL_004f:  ldc.i4.1
      IL_0050:  add
      IL_0051:  stloc.3
      IL_0052:  ldloc.3
      IL_0053:  ldc.i4     0x989681
      IL_0058:  blt.s      IL_0046

      IL_005a:  ldloc.0
      IL_005b:  ret
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






               !!0)
      IL_005d:  stloc.0
      IL_005e:  ldloc.2
      IL_005f:  ldc.i4.1
      IL_0060:  add
      IL_0061:  stloc.2
      IL_0062:  ldloc.2
      IL_0063:  ldc.i4     0x989681
      IL_0068:  blt.s      IL_0056

      IL_006a:  ldloc.0
      IL_006b:  ret
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






