




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
  .method public specialname static int32[] get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_w() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_0005:  ret
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
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32[] r@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  7
    .locals init (int32[] V_0,
             int32[] V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             uint64 V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_7,
             uint64 V_8,
             int32 V_9,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_10,
             int32 V_11,
             int32 V_12)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  dup
    IL_0008:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.5
    IL_000f:  ldc.i4.2
    IL_0010:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0015:  dup
    IL_0016:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_001b:  stloc.1
    IL_001c:  call       int32[] assembly::get_r()
    IL_0021:  ldlen
    IL_0022:  conv.i4
    IL_0023:  stloc.s    V_4
    IL_0025:  call       int32[] assembly::get_w()
    IL_002a:  ldlen
    IL_002b:  conv.i4
    IL_002c:  stloc.s    V_5
    IL_002e:  ldloc.s    V_4
    IL_0030:  ldloc.s    V_5
    IL_0032:  bge.s      IL_0039

    IL_0034:  ldloc.s    V_4
    IL_0036:  nop
    IL_0037:  br.s       IL_003c

    IL_0039:  ldloc.s    V_5
    IL_003b:  nop
    IL_003c:  ldc.i4.1
    IL_003d:  sub
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.0
    IL_0041:  bge.s      IL_0048

    IL_0043:  ldc.i4.0
    IL_0044:  conv.i8
    IL_0045:  nop
    IL_0046:  br.s       IL_0050

    IL_0048:  ldloc.3
    IL_0049:  ldc.i4.0
    IL_004a:  sub
    IL_004b:  conv.i8
    IL_004c:  ldc.i4.1
    IL_004d:  conv.i8
    IL_004e:  add
    IL_004f:  nop
    IL_0050:  stloc.s    V_6
    IL_0052:  ldc.i4.0
    IL_0053:  conv.i8
    IL_0054:  stloc.s    V_8
    IL_0056:  ldloc.3
    IL_0057:  stloc.s    V_9
    IL_0059:  br.s       IL_0072

    IL_005b:  ldloca.s   V_7
    IL_005d:  ldloc.s    V_9
    IL_005f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0064:  nop
    IL_0065:  ldloc.s    V_9
    IL_0067:  ldc.i4.m1
    IL_0068:  add
    IL_0069:  stloc.s    V_9
    IL_006b:  ldloc.s    V_8
    IL_006d:  ldc.i4.1
    IL_006e:  conv.i8
    IL_006f:  add
    IL_0070:  stloc.s    V_8
    IL_0072:  ldloc.s    V_8
    IL_0074:  ldloc.s    V_6
    IL_0076:  blt.un.s   IL_005b

    IL_0078:  ldloca.s   V_7
    IL_007a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_007f:  stloc.2
    IL_0080:  ldloc.2
    IL_0081:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0086:  stloc.s    V_10
    IL_0088:  br.s       IL_00c2

    IL_008a:  ldloc.2
    IL_008b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0090:  stloc.s    V_11
    IL_0092:  call       int32[] assembly::get_r()
    IL_0097:  ldloc.s    V_11
    IL_0099:  call       int32[] assembly::get_r()
    IL_009e:  ldloc.s    V_11
    IL_00a0:  ldelem     [runtime]System.Int32
    IL_00a5:  call       int32[] assembly::get_w()
    IL_00aa:  ldloc.s    V_11
    IL_00ac:  ldelem     [runtime]System.Int32
    IL_00b1:  add
    IL_00b2:  stelem     [runtime]System.Int32
    IL_00b7:  ldloc.s    V_10
    IL_00b9:  stloc.2
    IL_00ba:  ldloc.2
    IL_00bb:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00c0:  stloc.s    V_10
    IL_00c2:  ldloc.s    V_10
    IL_00c4:  brtrue.s   IL_008a

    IL_00c6:  nop
    IL_00c7:  nop
    IL_00c8:  call       int32[] assembly::get_r()
    IL_00cd:  ldc.i4.0
    IL_00ce:  ldelem     [runtime]System.Int32
    IL_00d3:  ldc.i4.3
    IL_00d4:  bne.un.s   IL_00da

    IL_00d6:  ldc.i4.0
    IL_00d7:  nop
    IL_00d8:  br.s       IL_00dc

    IL_00da:  ldc.i4.1
    IL_00db:  nop
    IL_00dc:  stloc.s    V_12
    IL_00de:  ldloc.s    V_12
    IL_00e0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00e5:  pop
    IL_00e6:  ret
  } 

} 






