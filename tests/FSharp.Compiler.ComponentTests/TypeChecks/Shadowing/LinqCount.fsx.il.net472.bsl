




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         
  .ver 4:0:0:0
}
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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname clo@4
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static bool  Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } 

  } 

  .method public specialname static class [runtime]System.Collections.Generic.List`1<int32> 
          get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.List`1<int32> '<StartupCode$assembly>'.$assembly$fsx::r@2
    IL_0005:  ret
  } 

  .property class [runtime]System.Collections.Generic.List`1<int32>
          r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.List`1<int32> assembly::get_r()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly initonly class [runtime]System.Collections.Generic.List`1<int32> r@2
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stsfld     class [runtime]System.Collections.Generic.List`1<int32> '<StartupCode$assembly>'.$assembly$fsx::r@2
    IL_000a:  call       class [runtime]System.Collections.Generic.List`1<int32> assembly::get_r()
    IL_000f:  callvirt   instance int32 class [runtime]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0014:  pop
    IL_0015:  call       class [runtime]System.Collections.Generic.List`1<int32> assembly::get_r()
    IL_001a:  ldnull
    IL_001b:  ldftn      bool assembly/clo@4::Invoke(int32)
    IL_0021:  newobj     instance void class [runtime]System.Func`2<int32,bool>::.ctor(object,
                                                                                        native int)
    IL_0026:  call       int32 [System.Core]System.Linq.Enumerable::Count<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                 class [runtime]System.Func`2<!!0,bool>)
    IL_002b:  pop
    IL_002c:  ret
  } 

} 






