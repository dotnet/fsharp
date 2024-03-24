




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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed For_loop_custom_step
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  g() cil managed
  {
    
    .maxstack  5
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.IDisposable V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.s   15
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0010:  stloc.1
    .try
    {
      IL_0011:  br.s       IL_001e

      IL_0013:  ldloc.1
      IL_0014:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0019:  call       void [runtime]System.Console::WriteLine(int32)
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_0013

      IL_0026:  leave.s    IL_003a

    }  
    finally
    {
      IL_0028:  ldloc.1
      IL_0029:  isinst     [runtime]System.IDisposable
      IL_002e:  stloc.2
      IL_002f:  ldloc.2
      IL_0030:  brfalse.s  IL_0039

      IL_0032:  ldloc.2
      IL_0033:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0038:  endfinally
      IL_0039:  endfinally
    }  
    IL_003a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$For_loop_custom_step$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void For_loop_custom_step::g()
    IL_0005:  ret
  } 

} 






