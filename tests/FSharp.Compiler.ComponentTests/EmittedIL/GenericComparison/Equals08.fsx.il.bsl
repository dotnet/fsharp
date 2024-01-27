




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
      
      .maxstack  6
      .locals init (bool V_0,
               int32[] V_1,
               int32[] V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   100
      IL_0004:  ldc.i4.0
      IL_0005:  sub
      IL_0006:  ldc.i4.1
      IL_0007:  add
      IL_0008:  ldc.i4.s   100
      IL_000a:  ldc.i4.0
      IL_000b:  sub
      IL_000c:  ldc.i4.1
      IL_000d:  add
      IL_000e:  ldc.i4.s   100
      IL_0010:  ldc.i4.0
      IL_0011:  sub
      IL_0012:  ldc.i4.1
      IL_0013:  add
      IL_0014:  ldc.i4.0
      IL_0015:  clt
      IL_0017:  neg
      IL_0018:  and
      IL_0019:  xor
      IL_001a:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t1@1::@_instance
      IL_001f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_0024:  stloc.1
      IL_0025:  ldc.i4.s   100
      IL_0027:  ldc.i4.0
      IL_0028:  sub
      IL_0029:  ldc.i4.1
      IL_002a:  add
      IL_002b:  ldc.i4.s   100
      IL_002d:  ldc.i4.0
      IL_002e:  sub
      IL_002f:  ldc.i4.1
      IL_0030:  add
      IL_0031:  ldc.i4.s   100
      IL_0033:  ldc.i4.0
      IL_0034:  sub
      IL_0035:  ldc.i4.1
      IL_0036:  add
      IL_0037:  ldc.i4.0
      IL_0038:  clt
      IL_003a:  neg
      IL_003b:  and
      IL_003c:  xor
      IL_003d:  ldsfld     class assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1 assembly/EqualsMicroPerfAndCodeGenerationTests/t2@1::@_instance
      IL_0042:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
      IL_0047:  stloc.2
      IL_0048:  ldc.i4.0
      IL_0049:  stloc.3
      IL_004a:  br.s       IL_0058

      IL_004c:  ldloc.1
      IL_004d:  ldloc.2
      IL_004e:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<int32[]>(!!0,
                                                                                                                                     !!0)
      IL_0053:  stloc.0
      IL_0054:  ldloc.3
      IL_0055:  ldc.i4.1
      IL_0056:  add
      IL_0057:  stloc.3
      IL_0058:  ldloc.3
      IL_0059:  ldc.i4     0x989681
      IL_005e:  blt.s      IL_004c

      IL_0060:  ldloc.0
      IL_0061:  ret
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






