




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
  .class auto ansi serializable sealed nested assembly beforefieldinit clo@1
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/clo@1 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 
            Invoke(int32 i) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/clo@1::.ctor()
      IL_0005:  stsfld     class assembly/clo@1 assembly/clo@1::@_instance
      IL_000a:  ret
    } 

  } 

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
    .locals init (int32 V_0)
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
    IL_004f:  bge.s      IL_0059

    IL_0051:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0056:  nop
    IL_0057:  br.s       IL_0069

    IL_0059:  ldloc.0
    IL_005a:  ldc.i4.0
    IL_005b:  sub
    IL_005c:  ldc.i4.1
    IL_005d:  add
    IL_005e:  ldsfld     class assembly/clo@1 assembly/clo@1::@_instance
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
    IL_0068:  nop
    IL_0069:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::current@9
    IL_006e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_0073:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0078:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::next@9
    IL_007d:  br.s       IL_00c5

    IL_007f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_0084:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0089:  stloc.0
    IL_008a:  call       int32[] assembly::get_r()
    IL_008f:  ldloc.0
    IL_0090:  call       int32[] assembly::get_r()
    IL_0095:  ldloc.0
    IL_0096:  ldelem     [runtime]System.Int32
    IL_009b:  call       int32[] assembly::get_w()
    IL_00a0:  ldloc.0
    IL_00a1:  ldelem     [runtime]System.Int32
    IL_00a6:  add
    IL_00a7:  stelem     [runtime]System.Int32
    IL_00ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00b1:  call       void assembly::set_current@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00b6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_current@9()
    IL_00bb:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00c0:  call       void assembly::set_next@9(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_00c5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_next@9()
    IL_00ca:  brtrue.s   IL_007f

    IL_00cc:  nop
    IL_00cd:  nop
    IL_00ce:  call       int32[] assembly::get_r()
    IL_00d3:  ldc.i4.0
    IL_00d4:  ldelem     [runtime]System.Int32
    IL_00d9:  ldc.i4.3
    IL_00da:  bne.un.s   IL_00e0

    IL_00dc:  ldc.i4.0
    IL_00dd:  nop
    IL_00de:  br.s       IL_00e2

    IL_00e0:  ldc.i4.1
    IL_00e1:  nop
    IL_00e2:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00e7:  pop
    IL_00e8:  ret
  } 

} 






