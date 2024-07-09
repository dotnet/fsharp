




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





.class public abstract auto ansi sealed M
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig instance int32 
            F(object o) cil managed
    {
      
      .maxstack  3
      .locals init (class M/C V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  tail.
      IL_0005:  callvirt   instance int32 [runtime]System.Object::GetHashCode()
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable nested public T
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
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

    .method public hidebysig specialname 
            instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> 
            get_F() cil managed
    {
      
      .maxstack  3
      .locals init (class M/T V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldsfld     class M/get_F@41 M/get_F@41::@_instance
      IL_0007:  ret
    } 

    .property instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
            F()
    {
      .get instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32> M/T::get_F()
    } 
  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit get_F@41
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>
  {
    .field static assembly initonly class M/get_F@41 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class M/C,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 
            Invoke(class M/C i_want_to_see_this_identifier) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       int32 M::I(class M/C)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void M/get_F@41::.ctor()
      IL_0005:  stsfld     class M/get_F@41 M/get_F@41::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static int32  I(class M/C i_want_to_see_this_identifier) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  tail.
    IL_0004:  callvirt   instance int32 M/C::F(object)
    IL_0009:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$M
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






