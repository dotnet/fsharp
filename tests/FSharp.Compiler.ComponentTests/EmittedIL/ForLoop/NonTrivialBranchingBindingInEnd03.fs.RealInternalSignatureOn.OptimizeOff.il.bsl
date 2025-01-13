




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
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             uint64 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_5,
             uint64 V_6,
             int32 V_7,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_8,
             int32 V_9,
             int32 V_10)
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
    IL_001f:  stloc.2
    IL_0020:  call       int32[] assembly::get_w()
    IL_0025:  ldlen
    IL_0026:  conv.i4
    IL_0027:  stloc.3
    IL_0028:  ldloc.2
    IL_0029:  ldloc.3
    IL_002a:  bge.s      IL_0030

    IL_002c:  ldloc.2
    IL_002d:  nop
    IL_002e:  br.s       IL_0032

    IL_0030:  ldloc.3
    IL_0031:  nop
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.0
    IL_0037:  bge.s      IL_003e

    IL_0039:  ldc.i4.0
    IL_003a:  conv.i8
    IL_003b:  nop
    IL_003c:  br.s       IL_0046

    IL_003e:  ldloc.1
    IL_003f:  ldc.i4.0
    IL_0040:  sub
    IL_0041:  conv.i8
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  nop
    IL_0046:  stloc.s    V_4
    IL_0048:  ldc.i4.0
    IL_0049:  conv.i8
    IL_004a:  stloc.s    V_6
    IL_004c:  ldc.i4.0
    IL_004d:  stloc.s    V_7
    IL_004f:  br.s       IL_0068

    IL_0051:  ldloca.s   V_5
    IL_0053:  ldloc.s    V_7
    IL_0055:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_005a:  nop
    IL_005b:  ldloc.s    V_7
    IL_005d:  ldc.i4.1
    IL_005e:  add
    IL_005f:  stloc.s    V_7
    IL_0061:  ldloc.s    V_6
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.s    V_6
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldloc.s    V_4
    IL_006c:  blt.un.s   IL_0051

    IL_006e:  ldloca.s   V_5
    IL_0070:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0075:  stloc.0
    IL_0076:  ldloc.0
    IL_0077:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_007c:  stloc.s    V_8
    IL_007e:  br.s       IL_00b8

    IL_0080:  ldloc.0
    IL_0081:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0086:  stloc.s    V_9
    IL_0088:  call       int32[] assembly::get_r()
    IL_008d:  ldloc.s    V_9
    IL_008f:  call       int32[] assembly::get_r()
    IL_0094:  ldloc.s    V_9
    IL_0096:  ldelem     [runtime]System.Int32
    IL_009b:  call       int32[] assembly::get_w()
    IL_00a0:  ldloc.s    V_9
    IL_00a2:  ldelem     [runtime]System.Int32
    IL_00a7:  add
    IL_00a8:  stelem     [runtime]System.Int32
    IL_00ad:  ldloc.s    V_8
    IL_00af:  stloc.0
    IL_00b0:  ldloc.0
    IL_00b1:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_00b6:  stloc.s    V_8
    IL_00b8:  ldloc.s    V_8
    IL_00ba:  brtrue.s   IL_0080

    IL_00bc:  nop
    IL_00bd:  nop
    IL_00be:  call       int32[] assembly::get_r()
    IL_00c3:  ldc.i4.0
    IL_00c4:  ldelem     [runtime]System.Int32
    IL_00c9:  ldc.i4.3
    IL_00ca:  bne.un.s   IL_00d0

    IL_00cc:  ldc.i4.0
    IL_00cd:  nop
    IL_00ce:  br.s       IL_00d2

    IL_00d0:  ldc.i4.1
    IL_00d1:  nop
    IL_00d2:  stloc.s    V_10
    IL_00d4:  ldloc.s    V_10
    IL_00d6:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00db:  pop
    IL_00dc:  ret
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






