




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern System.Collections
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         
  .ver 7:0:0:0
}
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
  .method public static void  f1<T>(class [System.Collections]System.Collections.Generic.List`1<!!T> x) cil managed
  {
    
    .maxstack  3
    .locals init (class [System.Collections]System.Collections.Generic.List`1<!!T> V_0,
             valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!!T> V_1,
             !!T V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<!!T>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0014

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!!T>::get_Current()
      IL_0012:  stloc.2
      IL_0013:  nop
      IL_0014:  ldloca.s   V_1
      IL_0016:  call       instance bool valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!!T>::MoveNext()
      IL_001b:  brtrue.s   IL_000b

      IL_001d:  leave.s    IL_002d

    }  
    finally
    {
      IL_001f:  ldloca.s   V_1
      IL_0021:  constrained. valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!!T>
      IL_0027:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_002c:  endfinally
    }  
    IL_002d:  ret
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






