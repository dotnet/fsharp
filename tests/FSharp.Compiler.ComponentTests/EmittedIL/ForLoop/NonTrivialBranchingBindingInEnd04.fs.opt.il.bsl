




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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             int32 V_2)
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
    IL_004e:  stloc.2
    IL_004f:  ldloc.0
    IL_0050:  stloc.2
    IL_0051:  ldloc.2
    IL_0052:  ldc.i4.0
    IL_0053:  bge.s      IL_0058

    IL_0055:  nop
    IL_0056:  br.s       IL_0082

    IL_0058:  br.s       IL_0067

    IL_005a:  ldloca.s   V_1
    IL_005c:  ldloc.2
    IL_005d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0062:  nop
    IL_0063:  ldloc.2
    IL_0064:  ldc.i4.m1
    IL_0065:  add
    IL_0066:  stloc.2
    IL_0067:  ldc.i4.0
    IL_0068:  ldloc.2
    IL_0069:  bge.s      IL_007a

    IL_006b:  ldloc.2
    IL_006c:  ldloc.0
    IL_006d:  bge.s      IL_0073

    IL_006f:  ldc.i4.1
    IL_0070:  nop
    IL_0071:  br.s       IL_007f

    IL_0073:  ldloc.2
    IL_0074:  ldloc.0
    IL_0075:  ceq
    IL_0077:  nop
    IL_0078:  br.s       IL_007f

    IL_007a:  ldc.i4.0
    IL_007b:  ldloc.2
    IL_007c:  ceq
    IL_007e:  nop
    IL_007f:  brtrue.s   IL_005a

    IL_0081:  nop
    IL_0082:  ldloca.s   V_1
    IL_0084:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0089:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::current@9
    IL_008e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_0093:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0098:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::next@9
    IL_009d:  br.s       IL_00e5

    IL_009f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00a4:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00a9:  stloc.0
    IL_00aa:  call       int32[] assembly::get_r()
    IL_00af:  ldloc.0
    IL_00b0:  call       int32[] assembly::get_r()
    IL_00b5:  ldloc.0
    IL_00b6:  ldelem     [runtime]System.Int32
    IL_00bb:  call       int32[] assembly::get_w()
    IL_00c0:  ldloc.0
    IL_00c1:  ldelem     [runtime]System.Int32
    IL_00c6:  add
    IL_00c7:  stelem     [runtime]System.Int32
    IL_00cc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00d1:  call       void assembly::set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00d6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00db:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00e0:  call       void assembly::set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00e5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00ea:  brtrue.s   IL_009f

    IL_00ec:  nop
    IL_00ed:  nop
    IL_00ee:  call       int32[] assembly::get_r()
    IL_00f3:  ldc.i4.0
    IL_00f4:  ldelem     [runtime]System.Int32
    IL_00f9:  ldc.i4.3
    IL_00fa:  bne.un.s   IL_0100

    IL_00fc:  ldc.i4.0
    IL_00fd:  nop
    IL_00fe:  br.s       IL_0102

    IL_0100:  ldc.i4.1
    IL_0101:  nop
    IL_0102:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0107:  pop
    IL_0108:  ret
  } 

} 






