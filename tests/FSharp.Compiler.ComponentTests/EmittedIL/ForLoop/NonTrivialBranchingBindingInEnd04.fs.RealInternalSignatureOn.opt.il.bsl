




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
  .method public specialname static int32[] get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::r@6
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_w() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::w@7
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_current@9() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::current@9
    IL_0005:  ret
  } 

  .method assembly specialname static void set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::current@9
    IL_0006:  ret
  } 

  .method assembly specialname static int32 get_e1@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::e1@1
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_e2@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::e2@1
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_next@9() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::next@9
    IL_0005:  ret
  } 

  .method assembly specialname static void set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::next@9
    IL_0006:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
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
    IL_0007:  stsfld     int32[] assembly::r@6
    IL_000c:  ldc.i4.5
    IL_000d:  ldc.i4.2
    IL_000e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0013:  stsfld     int32[] assembly::w@7
    IL_0018:  call       int32[] assembly::get_r()
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  stsfld     int32 assembly::e1@1
    IL_0024:  call       int32[] assembly::get_w()
    IL_0029:  ldlen
    IL_002a:  conv.i4
    IL_002b:  stsfld     int32 assembly::e2@1
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
    IL_0054:  br.s       IL_005e

    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.0
    IL_0058:  sub
    IL_0059:  conv.i8
    IL_005a:  ldc.i4.1
    IL_005b:  conv.i8
    IL_005c:  add
    IL_005d:  nop
    IL_005e:  stloc.1
    IL_005f:  ldc.i4.0
    IL_0060:  conv.i8
    IL_0061:  stloc.3
    IL_0062:  ldloc.0
    IL_0063:  stloc.s    V_4
    IL_0065:  br.s       IL_007c

    IL_0067:  ldloca.s   V_2
    IL_0069:  ldloc.s    V_4
    IL_006b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0070:  nop
    IL_0071:  ldloc.s    V_4
    IL_0073:  ldc.i4.m1
    IL_0074:  add
    IL_0075:  stloc.s    V_4
    IL_0077:  ldloc.3
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add
    IL_007b:  stloc.3
    IL_007c:  ldloc.3
    IL_007d:  ldloc.1
    IL_007e:  blt.un.s   IL_0067

    IL_0080:  ldloca.s   V_2
    IL_0082:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0087:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::current@9
    IL_008c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_0091:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0096:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::next@9
    IL_009b:  br.s       IL_00e3

    IL_009d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00a2:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_00a7:  stloc.0
    IL_00a8:  call       int32[] assembly::get_r()
    IL_00ad:  ldloc.0
    IL_00ae:  call       int32[] assembly::get_r()
    IL_00b3:  ldloc.0
    IL_00b4:  ldelem     [runtime]System.Int32
    IL_00b9:  call       int32[] assembly::get_w()
    IL_00be:  ldloc.0
    IL_00bf:  ldelem     [runtime]System.Int32
    IL_00c4:  add
    IL_00c5:  stelem     [runtime]System.Int32
    IL_00ca:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00cf:  call       void assembly::set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00d4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00d9:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00de:  call       void assembly::set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00e3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00e8:  brtrue.s   IL_009d

    IL_00ea:  nop
    IL_00eb:  nop
    IL_00ec:  call       int32[] assembly::get_r()
    IL_00f1:  ldc.i4.0
    IL_00f2:  ldelem     [runtime]System.Int32
    IL_00f7:  ldc.i4.3
    IL_00f8:  bne.un.s   IL_00fe

    IL_00fa:  ldc.i4.0
    IL_00fb:  nop
    IL_00fc:  br.s       IL_0100

    IL_00fe:  ldc.i4.1
    IL_00ff:  nop
    IL_0100:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0105:  pop
    IL_0106:  ret
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
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






