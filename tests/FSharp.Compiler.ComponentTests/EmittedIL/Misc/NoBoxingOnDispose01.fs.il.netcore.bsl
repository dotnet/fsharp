




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
  .method public static void  f1<T>(class [runtime]System.Collections.Generic.List`1<!!T> x) cil managed
  {
    
    .maxstack  3
    .locals init (valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!!T> V_0,
             !!T V_1)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!0> class [runtime]System.Collections.Generic.List`1<!!T>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0012

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!!T>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  nop
      IL_0012:  ldloca.s   V_0
      IL_0014:  call       instance bool valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!!T>::MoveNext()
      IL_0019:  brtrue.s   IL_0009

      IL_001b:  leave.s    IL_002b

    }  
    finally
    {
      IL_001d:  ldloca.s   V_0
      IL_001f:  constrained. valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!!T>
      IL_0025:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_002a:  endfinally
    }  
    IL_002b:  ret
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





