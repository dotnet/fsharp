




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





.class public abstract auto ansi sealed Program
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit F@6
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class Program/F@6 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit 
            Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Program/F@6::.ctor()
      IL_0005:  stsfld     class Program/F@6 Program/F@6::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static void  F<(class [FSharp.Core]Microsoft.FSharp.Control.IEvent`2<class [FSharp.Core]Microsoft.FSharp.Control.FSharpHandler`1<int32>,int32>) T>(!!T[0...,0...] x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  call       instance !!T !!T[0...,0...]::Get(int32,
                                                          int32)
    IL_0008:  box        !!T
    IL_000d:  unbox.any  class [runtime]System.IObservable`1<int32>
    IL_0012:  ldsfld     class Program/F@6 Program/F@6::@_instance
    IL_0017:  tail.
    IL_0019:  call       void [FSharp.Core]Microsoft.FSharp.Control.CommonExtensions::AddToObservable<int32>(class [runtime]System.IObservable`1<!!0>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001e:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Program
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






