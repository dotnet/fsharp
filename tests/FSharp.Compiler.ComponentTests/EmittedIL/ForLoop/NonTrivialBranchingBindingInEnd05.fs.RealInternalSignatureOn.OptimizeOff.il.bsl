




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
  .field static assembly int32[] r@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
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
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8)
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
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.1
    IL_001a:  call       int32[] assembly::get_r()
    IL_001f:  ldlen
    IL_0020:  conv.i4
    IL_0021:  stloc.2
    IL_0022:  call       int32[] assembly::get_w()
    IL_0027:  ldlen
    IL_0028:  conv.i4
    IL_0029:  stloc.3
    IL_002a:  ldloc.2
    IL_002b:  ldloc.3
    IL_002c:  bge.s      IL_0032

    IL_002e:  ldloc.2
    IL_002f:  nop
    IL_0030:  br.s       IL_0034

    IL_0032:  ldloc.3
    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  sub
    IL_0036:  stloc.0
    IL_0037:  ldloc.0
    IL_0038:  ldloc.1
    IL_0039:  blt.s      IL_00a5

    IL_003b:  ldc.i4.0
    IL_003c:  stloc.s    V_5
    IL_003e:  call       int32[] assembly::get_r()
    IL_0043:  ldlen
    IL_0044:  conv.i4
    IL_0045:  stloc.s    V_6
    IL_0047:  call       int32[] assembly::get_w()
    IL_004c:  ldlen
    IL_004d:  conv.i4
    IL_004e:  stloc.s    V_7
    IL_0050:  ldloc.s    V_6
    IL_0052:  ldloc.s    V_7
    IL_0054:  bge.s      IL_005b

    IL_0056:  ldloc.s    V_6
    IL_0058:  nop
    IL_0059:  br.s       IL_005e

    IL_005b:  ldloc.s    V_7
    IL_005d:  nop
    IL_005e:  ldc.i4.1
    IL_005f:  sub
    IL_0060:  stloc.s    V_4
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldloc.s    V_5
    IL_0066:  blt.s      IL_009b

    IL_0068:  call       int32[] assembly::get_r()
    IL_006d:  ldloc.s    V_5
    IL_006f:  call       int32[] assembly::get_r()
    IL_0074:  ldloc.s    V_5
    IL_0076:  ldelem     [runtime]System.Int32
    IL_007b:  call       int32[] assembly::get_w()
    IL_0080:  ldloc.s    V_5
    IL_0082:  ldelem     [runtime]System.Int32
    IL_0087:  add
    IL_0088:  stelem     [runtime]System.Int32
    IL_008d:  ldloc.s    V_5
    IL_008f:  ldc.i4.1
    IL_0090:  add
    IL_0091:  stloc.s    V_5
    IL_0093:  ldloc.s    V_5
    IL_0095:  ldloc.s    V_4
    IL_0097:  ldc.i4.1
    IL_0098:  add
    IL_0099:  bne.un.s   IL_0068

    IL_009b:  ldloc.1
    IL_009c:  ldc.i4.1
    IL_009d:  add
    IL_009e:  stloc.1
    IL_009f:  ldloc.1
    IL_00a0:  ldloc.0
    IL_00a1:  ldc.i4.1
    IL_00a2:  add
    IL_00a3:  bne.un.s   IL_003b

    IL_00a5:  nop
    IL_00a6:  nop
    IL_00a7:  call       int32[] assembly::get_r()
    IL_00ac:  ldc.i4.0
    IL_00ad:  ldelem     [runtime]System.Int32
    IL_00b2:  ldc.i4.s   11
    IL_00b4:  bne.un.s   IL_00ba

    IL_00b6:  ldc.i4.0
    IL_00b7:  nop
    IL_00b8:  br.s       IL_00bc

    IL_00ba:  ldc.i4.1
    IL_00bb:  nop
    IL_00bc:  stloc.s    V_8
    IL_00be:  ldloc.s    V_8
    IL_00c0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00c5:  pop
    IL_00c6:  ret
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






