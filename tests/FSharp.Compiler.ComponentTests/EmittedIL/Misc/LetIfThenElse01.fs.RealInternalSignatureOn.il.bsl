




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
    IL_0000:  nop
    IL_0001:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_1
    IL_0009:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_000e:  ldc.i4     0x7d0
    IL_0013:  ble.s      IL_0019

    IL_0015:  ldc.i4.1
    IL_0016:  nop
    IL_0017:  br.s       IL_001b

    IL_0019:  ldc.i4.2
    IL_001a:  nop
    IL_001b:  stloc.0
    IL_001c:  nop
    IL_001d:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0022:  stloc.3
    IL_0023:  ldloca.s   V_3
    IL_0025:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_002a:  ldc.i4     0x7d0
    IL_002f:  ble.s      IL_0035

    IL_0031:  ldc.i4.1
    IL_0032:  nop
    IL_0033:  br.s       IL_0037

    IL_0035:  ldc.i4.2
    IL_0036:  nop
    IL_0037:  stloc.2
    IL_0038:  nop
    IL_0039:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_003e:  stloc.s    V_5
    IL_0040:  ldloca.s   V_5
    IL_0042:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0047:  ldc.i4     0x7d0
    IL_004c:  bge.s      IL_0052

    IL_004e:  ldc.i4.1
    IL_004f:  nop
    IL_0050:  br.s       IL_0054

    IL_0052:  ldc.i4.2
    IL_0053:  nop
    IL_0054:  stloc.s    V_4
    IL_0056:  nop
    IL_0057:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_005c:  stloc.s    V_7
    IL_005e:  ldloca.s   V_7
    IL_0060:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0065:  ldc.i4     0x7d0
    IL_006a:  bge.s      IL_0070

    IL_006c:  ldc.i4.1
    IL_006d:  nop
    IL_006e:  br.s       IL_0072

    IL_0070:  ldc.i4.2
    IL_0071:  nop
    IL_0072:  stloc.s    V_6
    IL_0074:  ldloc.0
    IL_0075:  ldloc.2
    IL_0076:  ldloc.s    V_4
    IL_0078:  ldloc.s    V_6
    IL_007a:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_007f:  ret
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






