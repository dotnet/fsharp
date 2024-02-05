




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
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             int32 V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             int32 V_4)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.2
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.2
    IL_0004:  br.s       IL_0013

    IL_0006:  ldloca.s   V_1
    IL_0008:  ldloc.2
    IL_0009:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_000e:  nop
    IL_000f:  ldloc.2
    IL_0010:  ldc.i4.1
    IL_0011:  add
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldc.i4.3
    IL_0015:  bge.s      IL_0026

    IL_0017:  ldc.i4.1
    IL_0018:  ldloc.2
    IL_0019:  bge.s      IL_001f

    IL_001b:  ldc.i4.1
    IL_001c:  nop
    IL_001d:  br.s       IL_002b

    IL_001f:  ldc.i4.1
    IL_0020:  ldloc.2
    IL_0021:  ceq
    IL_0023:  nop
    IL_0024:  br.s       IL_002b

    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.3
    IL_0028:  ceq
    IL_002a:  nop
    IL_002b:  brtrue.s   IL_0006

    IL_002d:  ldloca.s   V_1
    IL_002f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003b:  stloc.3
    IL_003c:  br.s       IL_0066

    IL_003e:  ldloc.0
    IL_003f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0044:  stloc.s    V_4
    IL_0046:  ldstr      "%A"
    IL_004b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0050:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0055:  ldloc.s    V_4
    IL_0057:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_005c:  pop
    IL_005d:  ldloc.3
    IL_005e:  stloc.0
    IL_005f:  ldloc.0
    IL_0060:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0065:  stloc.3
    IL_0066:  ldloc.3
    IL_0067:  brtrue.s   IL_003e

    IL_0069:  ret
  } 

} 






