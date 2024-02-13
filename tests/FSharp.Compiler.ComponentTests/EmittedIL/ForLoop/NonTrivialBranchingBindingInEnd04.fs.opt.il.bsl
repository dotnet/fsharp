




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
  .method public specialname static int32[] 
          get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_0005:  ret
  } 

  .method public specialname static int32[] 
          get_w() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_current@9() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::current@9
    IL_0005:  ret
  } 

  .method assembly specialname static void 
          set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::current@9
    IL_0006:  ret
  } 

  .method assembly specialname static int32 
          get_e1@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::e1@1
    IL_0005:  ret
  } 

  .method assembly specialname static int32 
          get_e2@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::e2@1
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_next@9() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::next@9
    IL_0005:  ret
  } 

  .method assembly specialname static void 
          set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::next@9
    IL_0006:  ret
  } 

  .property int32[] r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_r()
  } 
  .property int32[] w()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_w()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          current@9()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
  } 
  .property int32 e1@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_e1@1()
  } 
  .property int32 e2@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_e2@1()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          next@9()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32[] r@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> current@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 e1@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 e2@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> next@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  7
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_000c:  ldc.i4.5
    IL_000d:  ldc.i4.2
    IL_000e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0013:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_0018:  call       int32[] assembly::get_r()
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  stsfld     int32 '<StartupCode$assembly>'.$assembly::e1@1
    IL_0024:  call       int32[] assembly::get_w()
    IL_0029:  ldlen
    IL_002a:  conv.i4
    IL_002b:  stsfld     int32 '<StartupCode$assembly>'.$assembly::e2@1
    IL_0030:  call       int32 assembly::get_e1@1()
    IL_0035:  call       int32 assembly::get_e2@1()
    IL_003a:  bge.s      IL_0044

    IL_003c:  call       int32 assembly::get_e1@1()
    IL_0041:  nop
    IL_0042:  br.s       IL_004a

    IL_0044:  call       int32 assembly::get_e2@1()
    IL_0049:  nop
    IL_004a:  ldc.i4.1
    IL_004b:  sub
    IL_004c:  stloc.0
    IL_004d:  ldloc.0
    IL_004e:  ldc.i4.0
    IL_004f:  bge.s      IL_0056

    IL_0051:  ldc.i4.0
    IL_0052:  conv.i8
    IL_0053:  nop
    IL_0054:  br.s       IL_005f

    IL_0056:  ldloc.0
    IL_0057:  conv.i8
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  sub
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  nop
    IL_005f:  stloc.1
    IL_0060:  ldloc.1
    IL_0061:  ldc.i4.1
    IL_0062:  bge.un.s   IL_006c

    IL_0064:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0069:  nop
    IL_006a:  br.s       IL_0095

    IL_006c:  ldc.i4.0
    IL_006d:  conv.i8
    IL_006e:  stloc.3
    IL_006f:  ldloc.0
    IL_0070:  stloc.s    V_4
    IL_0072:  br.s       IL_0089

    IL_0074:  ldloca.s   V_2
    IL_0076:  ldloc.s    V_4
    IL_0078:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_007d:  nop
    IL_007e:  ldloc.s    V_4
    IL_0080:  ldc.i4.m1
    IL_0081:  add
    IL_0082:  stloc.s    V_4
    IL_0084:  ldloc.3
    IL_0085:  ldc.i4.1
    IL_0086:  conv.i8
    IL_0087:  add
    IL_0088:  stloc.3
    IL_0089:  ldloc.3
    IL_008a:  ldloc.1
    IL_008b:  blt.un.s   IL_0074

    IL_008d:  ldloca.s   V_2
    IL_008f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0094:  nop
    IL_0095:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::current@9
    IL_009a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_009f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00a4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::next@9
    IL_00a9:  br.s       IL_00f1

    IL_00ab:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00b0:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00b5:  stloc.0
    IL_00b6:  call       int32[] assembly::get_r()
    IL_00bb:  ldloc.0
    IL_00bc:  call       int32[] assembly::get_r()
    IL_00c1:  ldloc.0
    IL_00c2:  ldelem     [runtime]System.Int32
    IL_00c7:  call       int32[] assembly::get_w()
    IL_00cc:  ldloc.0
    IL_00cd:  ldelem     [runtime]System.Int32
    IL_00d2:  add
    IL_00d3:  stelem     [runtime]System.Int32
    IL_00d8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00dd:  call       void assembly::set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00e2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00e7:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00ec:  call       void assembly::set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00f1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00f6:  brtrue.s   IL_00ab

    IL_00f8:  nop
    IL_00f9:  nop
    IL_00fa:  call       int32[] assembly::get_r()
    IL_00ff:  ldc.i4.0
    IL_0100:  ldelem     [runtime]System.Int32
    IL_0105:  ldc.i4.3
    IL_0106:  bne.un.s   IL_010c

    IL_0108:  ldc.i4.0
    IL_0109:  nop
    IL_010a:  br.s       IL_010e

    IL_010c:  ldc.i4.1
    IL_010d:  nop
    IL_010e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0113:  pop
    IL_0114:  ret
  } 

} 






