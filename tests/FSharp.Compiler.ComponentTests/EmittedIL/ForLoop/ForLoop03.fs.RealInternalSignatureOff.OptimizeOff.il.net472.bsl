




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





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static class [runtime]System.Collections.Generic.List`1<int32> get_ra() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.List`1<int32> '<StartupCode$assembly>'.$assembly::ra@5
    IL_0005:  ret
  } 

  .method public static void  test1() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class [runtime]System.Collections.Generic.List`1<int32> V_2,
             valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32> V_3,
             int32 V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_003f

    IL_0006:  call       class [runtime]System.Collections.Generic.List`1<int32> assembly::get_ra()
    IL_000b:  stloc.2
    IL_000c:  ldloc.2
    IL_000d:  callvirt   instance valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!0> class [runtime]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0012:  stloc.3
    .try
    {
      IL_0013:  br.s       IL_0022

      IL_0015:  ldloca.s   V_3
      IL_0017:  call       instance !0 valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.0
      IL_001f:  ldc.i4.1
      IL_0020:  add
      IL_0021:  stloc.0
      IL_0022:  ldloca.s   V_3
      IL_0024:  call       instance bool valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_0029:  brtrue.s   IL_0015

      IL_002b:  leave.s    IL_003b

    }  
    finally
    {
      IL_002d:  ldloca.s   V_3
      IL_002f:  constrained. valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0035:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.1
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.1
    IL_0041:  ldc.i4     0x989680
    IL_0046:  add
    IL_0047:  blt.s      IL_0006

    IL_0049:  ldstr      "z = %d"
    IL_004e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0053:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0058:  ldloc.0
    IL_0059:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_005e:  pop
    IL_005f:  ret
  } 

  .property class [runtime]System.Collections.Generic.List`1<int32>
          ra()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.List`1<int32> assembly::get_ra()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly class [runtime]System.Collections.Generic.List`1<int32> ra@5
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  5
    .locals init (class [runtime]System.Collections.Generic.List`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldc.i4.s   100
    IL_0002:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_0007:  dup
    IL_0008:  stsfld     class [runtime]System.Collections.Generic.List`1<int32> '<StartupCode$assembly>'.$assembly::ra@5
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.1
    IL_0010:  br.s       IL_0021

    IL_0012:  call       class [runtime]System.Collections.Generic.List`1<int32> assembly::get_ra()
    IL_0017:  ldloc.1
    IL_0018:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.1
    IL_0023:  ldc.i4.s   100
    IL_0025:  add
    IL_0026:  blt.s      IL_0012

    IL_0028:  ret
  } 

} 






