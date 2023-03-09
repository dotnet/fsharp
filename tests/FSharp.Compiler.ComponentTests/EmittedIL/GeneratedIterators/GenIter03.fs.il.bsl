




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
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenC() cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_0024

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldloca.s   V_0
      IL_001b:  ldloc.3
      IL_001c:  ldloc.3
      IL_001d:  mul
      IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0023:  nop
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002a:  brtrue.s   IL_0012

      IL_002c:  ldnull
      IL_002d:  stloc.2
      IL_002e:  leave.s    IL_0045

    }  
    finally
    {
      IL_0030:  ldloc.1
      IL_0031:  isinst     [runtime]System.IDisposable
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.s    V_4
      IL_003a:  brfalse.s  IL_0044

      IL_003c:  ldloc.s    V_4
      IL_003e:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0043:  endfinally
      IL_0044:  endfinally
    }  
    IL_0045:  ldloc.2
    IL_0046:  pop
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004e:  ret
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






