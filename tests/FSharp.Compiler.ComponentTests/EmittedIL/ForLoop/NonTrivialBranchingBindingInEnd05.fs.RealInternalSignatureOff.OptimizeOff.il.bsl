




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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10)
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
    IL_001c:  ldc.i4.0
    IL_001d:  stloc.3
    IL_001e:  call       int32[] assembly::get_r()
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  stloc.s    V_4
    IL_0027:  call       int32[] assembly::get_w()
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  stloc.s    V_5
    IL_0030:  ldloc.s    V_4
    IL_0032:  ldloc.s    V_5
    IL_0034:  bge.s      IL_003b

    IL_0036:  ldloc.s    V_4
    IL_0038:  nop
    IL_0039:  br.s       IL_003e

    IL_003b:  ldloc.s    V_5
    IL_003d:  nop
    IL_003e:  ldc.i4.1
    IL_003f:  sub
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  ldloc.3
    IL_0043:  blt.s      IL_00af

    IL_0045:  ldc.i4.0
    IL_0046:  stloc.s    V_7
    IL_0048:  call       int32[] assembly::get_r()
    IL_004d:  ldlen
    IL_004e:  conv.i4
    IL_004f:  stloc.s    V_8
    IL_0051:  call       int32[] assembly::get_w()
    IL_0056:  ldlen
    IL_0057:  conv.i4
    IL_0058:  stloc.s    V_9
    IL_005a:  ldloc.s    V_8
    IL_005c:  ldloc.s    V_9
    IL_005e:  bge.s      IL_0065

    IL_0060:  ldloc.s    V_8
    IL_0062:  nop
    IL_0063:  br.s       IL_0068

    IL_0065:  ldloc.s    V_9
    IL_0067:  nop
    IL_0068:  ldc.i4.1
    IL_0069:  sub
    IL_006a:  stloc.s    V_6
    IL_006c:  ldloc.s    V_6
    IL_006e:  ldloc.s    V_7
    IL_0070:  blt.s      IL_00a5

    IL_0072:  call       int32[] assembly::get_r()
    IL_0077:  ldloc.s    V_7
    IL_0079:  call       int32[] assembly::get_r()
    IL_007e:  ldloc.s    V_7
    IL_0080:  ldelem     [runtime]System.Int32
    IL_0085:  call       int32[] assembly::get_w()
    IL_008a:  ldloc.s    V_7
    IL_008c:  ldelem     [runtime]System.Int32
    IL_0091:  add
    IL_0092:  stelem     [runtime]System.Int32
    IL_0097:  ldloc.s    V_7
    IL_0099:  ldc.i4.1
    IL_009a:  add
    IL_009b:  stloc.s    V_7
    IL_009d:  ldloc.s    V_7
    IL_009f:  ldloc.s    V_6
    IL_00a1:  ldc.i4.1
    IL_00a2:  add
    IL_00a3:  bne.un.s   IL_0072

    IL_00a5:  ldloc.3
    IL_00a6:  ldc.i4.1
    IL_00a7:  add
    IL_00a8:  stloc.3
    IL_00a9:  ldloc.3
    IL_00aa:  ldloc.2
    IL_00ab:  ldc.i4.1
    IL_00ac:  add
    IL_00ad:  bne.un.s   IL_0045

    IL_00af:  nop
    IL_00b0:  nop
    IL_00b1:  call       int32[] assembly::get_r()
    IL_00b6:  ldc.i4.0
    IL_00b7:  ldelem     [runtime]System.Int32
    IL_00bc:  ldc.i4.s   11
    IL_00be:  bne.un.s   IL_00c4

    IL_00c0:  ldc.i4.0
    IL_00c1:  nop
    IL_00c2:  br.s       IL_00c6

    IL_00c4:  ldc.i4.1
    IL_00c5:  nop
    IL_00c6:  stloc.s    V_10
    IL_00c8:  ldloc.s    V_10
    IL_00ca:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00cf:  pop
    IL_00d0:  ret
  } 

} 






