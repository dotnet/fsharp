




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
    IL_0001:  nop
    IL_0002:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0007:  stloc.0
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_000f:  ldc.i4     0x7d0
    IL_0014:  ble.s      IL_001a

    IL_0016:  ldc.i4.1
    IL_0017:  nop
    IL_0018:  br.s       IL_001c

    IL_001a:  ldc.i4.2
    IL_001b:  nop
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0023:  stloc.2
    IL_0024:  ldloca.s   V_2
    IL_0026:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_002b:  ldc.i4     0x7d0
    IL_0030:  ble.s      IL_0038

    IL_0032:  stloc.3
    IL_0033:  ldloc.3
    IL_0034:  ldc.i4.1
    IL_0035:  nop
    IL_0036:  br.s       IL_003e

    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  ldc.i4.2
    IL_003d:  nop
    IL_003e:  stloc.s    V_5
    IL_0040:  stloc.s    V_6
    IL_0042:  ldloc.s    V_6
    IL_0044:  ldloc.s    V_5
    IL_0046:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_004b:  stloc.s    V_7
    IL_004d:  ldloca.s   V_7
    IL_004f:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0054:  ldc.i4     0x7d0
    IL_0059:  bge.s      IL_0067

    IL_005b:  stloc.s    V_8
    IL_005d:  stloc.s    V_9
    IL_005f:  ldloc.s    V_9
    IL_0061:  ldloc.s    V_8
    IL_0063:  ldc.i4.1
    IL_0064:  nop
    IL_0065:  br.s       IL_0071

    IL_0067:  stloc.s    V_10
    IL_0069:  stloc.s    V_11
    IL_006b:  ldloc.s    V_11
    IL_006d:  ldloc.s    V_10
    IL_006f:  ldc.i4.2
    IL_0070:  nop
    IL_0071:  stloc.s    V_12
    IL_0073:  stloc.s    V_13
    IL_0075:  stloc.s    V_14
    IL_0077:  ldloc.s    V_14
    IL_0079:  ldloc.s    V_13
    IL_007b:  ldloc.s    V_12
    IL_007d:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0082:  stloc.s    V_15
    IL_0084:  ldloca.s   V_15
    IL_0086:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_008b:  ldc.i4     0x7d0
    IL_0090:  bge.s      IL_00a2

    IL_0092:  stloc.s    V_16
    IL_0094:  stloc.s    V_17
    IL_0096:  stloc.s    V_18
    IL_0098:  ldloc.s    V_18
    IL_009a:  ldloc.s    V_17
    IL_009c:  ldloc.s    V_16
    IL_009e:  ldc.i4.1
    IL_009f:  nop
    IL_00a0:  br.s       IL_00b0

    IL_00a2:  stloc.s    V_19
    IL_00a4:  stloc.s    V_20
    IL_00a6:  stloc.s    V_21
    IL_00a8:  ldloc.s    V_21
    IL_00aa:  ldloc.s    V_20
    IL_00ac:  ldloc.s    V_19
    IL_00ae:  ldc.i4.2
    IL_00af:  nop
    IL_00b0:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_00b5:  ret
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

  .method assembly specialname static class [runtime]System.Tuple`4<int32,int32,int32,int32> get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> '<StartupCode$assembly>'.$assembly::arg@1
    IL_0005:  ret
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
  .field static assembly class [runtime]System.Tuple`4<int32,int32,int32,int32> arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::F<int32>(!!0)
    IL_0006:  stsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> '<StartupCode$assembly>'.$assembly::arg@1
    IL_000b:  ret
  } 

} 





