




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
  .class auto ansi serializable nested public Foo`1<a>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly class assembly/Foo`1<!a> theInstance
    .field static assembly int32 init@2
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  newobj     instance void class assembly/Foo`1<!a>::.ctor()
      IL_0005:  stsfld     class assembly/Foo`1<!0> class assembly/Foo`1<!a>::theInstance
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stsfld     int32 class assembly/Foo`1<!a>::init@2
      IL_0012:  ret
    } 

    .method public specialname static class assembly/Foo`1<!a> 
            get_Instance() cil managed
    {
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 class assembly/Foo`1<!a>::init@2
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     class assembly/Foo`1<!0> class assembly/Foo`1<!a>::theInstance
      IL_001a:  ret
    } 

    .property class assembly/Foo`1<!a>
            Instance()
    {
      .get class assembly/Foo`1<!a> assembly/Foo`1::get_Instance()
    } 
  } 

  .class auto ansi serializable nested public Bar`2<a,b>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly class assembly/Bar`2<!a,!b> theInstance
    .field static assembly int32 'init@6-1'
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  newobj     instance void class assembly/Bar`2<!a,!b>::.ctor()
      IL_0005:  stsfld     class assembly/Bar`2<!0,!1> class assembly/Bar`2<!a,!b>::theInstance
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stsfld     int32 class assembly/Bar`2<!a,!b>::'init@6-1'
      IL_0012:  ret
    } 

    .method public specialname static class assembly/Bar`2<!a,!b> 
            get_Instance() cil managed
    {
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 class assembly/Bar`2<!a,!b>::'init@6-1'
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     class assembly/Bar`2<!0,!1> class assembly/Bar`2<!a,!b>::theInstance
      IL_001a:  ret
    } 

    .property class assembly/Bar`2<!a,!b>
            Instance()
    {
      .get class assembly/Bar`2<!a,!b> assembly/Bar`2::get_Instance()
    } 
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






