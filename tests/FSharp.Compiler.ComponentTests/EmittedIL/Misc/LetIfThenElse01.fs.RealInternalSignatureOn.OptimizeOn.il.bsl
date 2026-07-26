




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
  .field static assembly class [runtime]System.Tuple`4<int32,int32,int32,int32> arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public static class [runtime]System.Tuple`4<int32,int32,int32,int32> F<a>(!!a y) cil managed
  {
    
    .maxstack  7
    .locals init (valuetype [runtime]System.DateTime V_0,
             int32 V_1,
             valuetype [runtime]System.DateTime V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             valuetype [runtime]System.DateTime V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10,
             int32 V_11,
             int32 V_12,
             int32 V_13,
             int32 V_14,
             valuetype [runtime]System.DateTime V_15,
             int32 V_16,
             int32 V_17,
             int32 V_18,
             int32 V_19,
             int32 V_20,
             int32 V_21)
    IL_0000:  nop
    IL_0001:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_000e:  ldc.i4     0x7d0
    IL_0013:  ble.s      IL_0019

    IL_0015:  ldc.i4.1
    IL_0016:  nop
    IL_0017:  br.s       IL_001b

    IL_0019:  ldc.i4.2
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0022:  stloc.2
    IL_0023:  ldloca.s   V_2
    IL_0025:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_002a:  ldc.i4     0x7d0
    IL_002f:  ble.s      IL_0037

    IL_0031:  stloc.3
    IL_0032:  ldloc.3
    IL_0033:  ldc.i4.1
    IL_0034:  nop
    IL_0035:  br.s       IL_003d

    IL_0037:  stloc.s    V_4
    IL_0039:  ldloc.s    V_4
    IL_003b:  ldc.i4.2
    IL_003c:  nop
    IL_003d:  stloc.s    V_5
    IL_003f:  stloc.s    V_6
    IL_0041:  ldloc.s    V_6
    IL_0043:  ldloc.s    V_5
    IL_0045:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_004a:  stloc.s    V_7
    IL_004c:  ldloca.s   V_7
    IL_004e:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0053:  ldc.i4     0x7d0
    IL_0058:  bge.s      IL_0066

    IL_005a:  stloc.s    V_8
    IL_005c:  stloc.s    V_9
    IL_005e:  ldloc.s    V_9
    IL_0060:  ldloc.s    V_8
    IL_0062:  ldc.i4.1
    IL_0063:  nop
    IL_0064:  br.s       IL_0070

    IL_0066:  stloc.s    V_10
    IL_0068:  stloc.s    V_11
    IL_006a:  ldloc.s    V_11
    IL_006c:  ldloc.s    V_10
    IL_006e:  ldc.i4.2
    IL_006f:  nop
    IL_0070:  stloc.s    V_12
    IL_0072:  stloc.s    V_13
    IL_0074:  stloc.s    V_14
    IL_0076:  ldloc.s    V_14
    IL_0078:  ldloc.s    V_13
    IL_007a:  ldloc.s    V_12
    IL_007c:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0081:  stloc.s    V_15
    IL_0083:  ldloca.s   V_15
    IL_0085:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_008a:  ldc.i4     0x7d0
    IL_008f:  bge.s      IL_00a1

    IL_0091:  stloc.s    V_16
    IL_0093:  stloc.s    V_17
    IL_0095:  stloc.s    V_18
    IL_0097:  ldloc.s    V_18
    IL_0099:  ldloc.s    V_17
    IL_009b:  ldloc.s    V_16
    IL_009d:  ldc.i4.1
    IL_009e:  nop
    IL_009f:  br.s       IL_00af

    IL_00a1:  stloc.s    V_19
    IL_00a3:  stloc.s    V_20
    IL_00a5:  stloc.s    V_21
    IL_00a7:  ldloc.s    V_21
    IL_00a9:  ldloc.s    V_20
    IL_00ab:  ldloc.s    V_19
    IL_00ad:  ldc.i4.2
    IL_00ae:  nop
    IL_00af:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_00b4:  ret
  } 

  .method assembly specialname static class [runtime]System.Tuple`4<int32,int32,int32,int32> get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::arg@1
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::F<int32>(!!0)
    IL_0006:  stsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::arg@1
    IL_000b:  ret
  } 

  .property class [runtime]System.Tuple`4<int32,int32,int32,int32>
          arg@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::get_arg@1()
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






