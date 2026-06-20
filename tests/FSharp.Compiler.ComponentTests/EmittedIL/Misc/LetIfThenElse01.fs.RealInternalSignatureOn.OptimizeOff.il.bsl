




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
  .method public static class [runtime]System.Tuple`4<int32,int32,int32,int32> F<a>(!!a y) cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0,
             valuetype [runtime]System.DateTime V_1,
             int32 V_2,
             valuetype [runtime]System.DateTime V_3,
             int32 V_4,
             valuetype [runtime]System.DateTime V_5,
             int32 V_6,
             valuetype [runtime]System.DateTime V_7)
    IL_0000:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_1
    IL_0008:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_000d:  ldc.i4     0x7d0
    IL_0012:  ble.s      IL_0018

    IL_0014:  ldc.i4.1
    IL_0015:  nop
    IL_0016:  br.s       IL_001a

    IL_0018:  ldc.i4.2
    IL_0019:  nop
    IL_001a:  stloc.0
    IL_001b:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0020:  stloc.3
    IL_0021:  ldloca.s   V_3
    IL_0023:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0028:  ldc.i4     0x7d0
    IL_002d:  ble.s      IL_0033

    IL_002f:  ldc.i4.1
    IL_0030:  nop
    IL_0031:  br.s       IL_0035

    IL_0033:  ldc.i4.2
    IL_0034:  nop
    IL_0035:  stloc.2
    IL_0036:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_003b:  stloc.s    V_5
    IL_003d:  ldloca.s   V_5
    IL_003f:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0044:  ldc.i4     0x7d0
    IL_0049:  bge.s      IL_004f

    IL_004b:  ldc.i4.1
    IL_004c:  nop
    IL_004d:  br.s       IL_0051

    IL_004f:  ldc.i4.2
    IL_0050:  nop
    IL_0051:  stloc.s    V_4
    IL_0053:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0058:  stloc.s    V_7
    IL_005a:  ldloca.s   V_7
    IL_005c:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0061:  ldc.i4     0x7d0
    IL_0066:  bge.s      IL_006c

    IL_0068:  ldc.i4.1
    IL_0069:  nop
    IL_006a:  br.s       IL_006e

    IL_006c:  ldc.i4.2
    IL_006d:  nop
    IL_006e:  stloc.s    V_6
    IL_0070:  ldloc.0
    IL_0071:  ldloc.2
    IL_0072:  ldloc.s    V_4
    IL_0074:  ldloc.s    V_6
    IL_0076:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_007b:  ret
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Tuple`4<int32,int32,int32,int32> V_0,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::F<int32>(!!0)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ret
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





