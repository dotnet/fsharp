




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





.class public abstract auto ansi sealed For_loop_non_int
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<int64> inputSequence@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerator`1<int64> enumerator@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method assembly specialname static class [runtime]System.Collections.Generic.IEnumerable`1<int64> get_inputSequence@4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int64> For_loop_non_int::inputSequence@4
    IL_0005:  ret
  } 

  .method assembly specialname static class [runtime]System.Collections.Generic.IEnumerator`1<int64> get_enumerator@4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::enumerator@4
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$For_loop_non_int$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$For_loop_non_int$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  5
    .locals init (int64 V_0,
             class [runtime]System.IDisposable V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  ldc.i4.1
    IL_0003:  conv.i8
    IL_0004:  ldc.i4.s   100
    IL_0006:  conv.i8
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                     int64,
                                                                                                                                                                     int64)
    IL_000c:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int64> For_loop_non_int::inputSequence@4
    IL_0011:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> For_loop_non_int::get_inputSequence@4()
    IL_0016:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int64>::GetEnumerator()
    IL_001b:  stsfld     class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::enumerator@4
    .try
    {
      IL_0020:  br.s       IL_003d

      IL_0022:  call       class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::get_enumerator@4()
      IL_0027:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int64>::get_Current()
      IL_002c:  stloc.0
      IL_002d:  ldstr      "hello"
      IL_0032:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0037:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_003c:  pop
      IL_003d:  call       class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::get_enumerator@4()
      IL_0042:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0047:  brtrue.s   IL_0022

      IL_0049:  leave.s    IL_0061

    }  
    finally
    {
      IL_004b:  call       class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::get_enumerator@4()
      IL_0050:  isinst     [runtime]System.IDisposable
      IL_0055:  stloc.1
      IL_0056:  ldloc.1
      IL_0057:  brfalse.s  IL_0060

      IL_0059:  ldloc.1
      IL_005a:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_005f:  endfinally
      IL_0060:  endfinally
    }  
    IL_0061:  ret
  } 

  .property class [runtime]System.Collections.Generic.IEnumerable`1<int64>
          inputSequence@4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<int64> For_loop_non_int::get_inputSequence@4()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerator`1<int64>
          enumerator@4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerator`1<int64> For_loop_non_int::get_enumerator@4()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$For_loop_non_int$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void For_loop_non_int::staticInitialization@()
    IL_0005:  ret
  } 

} 






