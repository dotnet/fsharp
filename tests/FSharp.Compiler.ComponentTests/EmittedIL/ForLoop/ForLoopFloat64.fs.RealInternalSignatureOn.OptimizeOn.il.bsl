




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
  .class abstract auto ansi sealed nested public Down
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (int64 V_0,
               int64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.1
      IL_0004:  conv.i8
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int64)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  add
      IL_0012:  stloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  conv.i8
      IL_0016:  add
      IL_0017:  stloc.0
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.0
      IL_001a:  conv.i8
      IL_001b:  blt.un.s   IL_0008

      IL_001d:  ret
    } 

    .method public static void  constNonEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               int64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.s   10
      IL_0005:  conv.i8
      IL_0006:  stloc.1
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(int64)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.m1
      IL_0011:  conv.i8
      IL_0012:  add
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  conv.i8
      IL_0017:  add
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.s   10
      IL_001c:  conv.i8
      IL_001d:  blt.un.s   IL_0009

      IL_001f:  ret
    } 

    .method public static void  constFinish(int64 start) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               int64 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  conv.i8
      IL_0003:  bge.s      IL_000a

      IL_0005:  ldc.i4.0
      IL_0006:  conv.i8
      IL_0007:  nop
      IL_0008:  br.s       IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldc.i4.1
      IL_000c:  conv.i8
      IL_000d:  sub
      IL_000e:  ldc.i4.1
      IL_000f:  conv.i8
      IL_0010:  add.ovf.un
      IL_0011:  nop
      IL_0012:  stloc.0
      IL_0013:  ldc.i4.0
      IL_0014:  conv.i8
      IL_0015:  stloc.1
      IL_0016:  ldarg.0
      IL_0017:  stloc.2
      IL_0018:  br.s       IL_002a

      IL_001a:  ldloc.2
      IL_001b:  call       void assembly::set_c(int64)
      IL_0020:  ldloc.2
      IL_0021:  ldc.i4.m1
      IL_0022:  conv.i8
      IL_0023:  add
      IL_0024:  stloc.2
      IL_0025:  ldloc.1
      IL_0026:  ldc.i4.1
      IL_0027:  conv.i8
      IL_0028:  add
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  ldloc.0
      IL_002c:  blt.un.s   IL_001a

      IL_002e:  ret
    } 

    .method public static void  constStart(int64 finish) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               int64 V_2)
      IL_0000:  ldc.i4.s   10
      IL_0002:  conv.i8
      IL_0003:  ldarg.0
      IL_0004:  bge.s      IL_000b

      IL_0006:  ldc.i4.0
      IL_0007:  conv.i8
      IL_0008:  nop
      IL_0009:  br.s       IL_0014

      IL_000b:  ldc.i4.s   10
      IL_000d:  conv.i8
      IL_000e:  ldarg.0
      IL_000f:  sub
      IL_0010:  ldc.i4.1
      IL_0011:  conv.i8
      IL_0012:  add.ovf.un
      IL_0013:  nop
      IL_0014:  stloc.0
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.1
      IL_0018:  ldc.i4.s   10
      IL_001a:  conv.i8
      IL_001b:  stloc.2
      IL_001c:  br.s       IL_002e

      IL_001e:  ldloc.2
      IL_001f:  call       void assembly::set_c(int64)
      IL_0024:  ldloc.2
      IL_0025:  ldc.i4.m1
      IL_0026:  conv.i8
      IL_0027:  add
      IL_0028:  stloc.2
      IL_0029:  ldloc.1
      IL_002a:  ldc.i4.1
      IL_002b:  conv.i8
      IL_002c:  add
      IL_002d:  stloc.1
      IL_002e:  ldloc.1
      IL_002f:  ldloc.0
      IL_0030:  blt.un.s   IL_001e

      IL_0032:  ret
    } 

    .method public static void  annotatedStart(int64 start,
                                               int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.0
      IL_000a:  ldarg.1
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.m1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldarg.1
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.0
      IL_0040:  ldarg.1
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.m1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  annotatedFinish(int64 start,
                                                int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.0
      IL_000a:  ldarg.1
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.m1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldarg.1
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.0
      IL_0040:  ldarg.1
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.m1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  inferredStartAndFinish(int64 start,
                                                       int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.0
      IL_000a:  ldarg.1
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.m1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldarg.1
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.0
      IL_0040:  ldarg.1
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.m1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Up
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (int64 V_0,
               int64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.s   10
      IL_0005:  conv.i8
      IL_0006:  stloc.1
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(int64)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  conv.i8
      IL_0012:  add
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  conv.i8
      IL_0017:  add
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.0
      IL_001b:  conv.i8
      IL_001c:  blt.un.s   IL_0009

      IL_001e:  ret
    } 

    .method public static void  constNonEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               int64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.1
      IL_0004:  conv.i8
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int64)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  conv.i8
      IL_0011:  add
      IL_0012:  stloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  conv.i8
      IL_0016:  add
      IL_0017:  stloc.0
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.s   10
      IL_001b:  conv.i8
      IL_001c:  blt.un.s   IL_0008

      IL_001e:  ret
    } 

    .method public static void  constFinish(int64 start) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               int64 V_2)
      IL_0000:  ldc.i4.s   10
      IL_0002:  conv.i8
      IL_0003:  ldarg.0
      IL_0004:  bge.s      IL_000b

      IL_0006:  ldc.i4.0
      IL_0007:  conv.i8
      IL_0008:  nop
      IL_0009:  br.s       IL_0014

      IL_000b:  ldc.i4.s   10
      IL_000d:  conv.i8
      IL_000e:  ldarg.0
      IL_000f:  sub
      IL_0010:  ldc.i4.1
      IL_0011:  conv.i8
      IL_0012:  add.ovf.un
      IL_0013:  nop
      IL_0014:  stloc.0
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.1
      IL_0018:  ldarg.0
      IL_0019:  stloc.2
      IL_001a:  br.s       IL_002c

      IL_001c:  ldloc.2
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.2
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.2
      IL_0027:  ldloc.1
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.1
      IL_002c:  ldloc.1
      IL_002d:  ldloc.0
      IL_002e:  blt.un.s   IL_001c

      IL_0030:  ret
    } 

    .method public static void  constStart(int64 finish) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               int64 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  conv.i8
      IL_0003:  bge.s      IL_000a

      IL_0005:  ldc.i4.0
      IL_0006:  conv.i8
      IL_0007:  nop
      IL_0008:  br.s       IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldc.i4.1
      IL_000c:  conv.i8
      IL_000d:  sub
      IL_000e:  ldc.i4.1
      IL_000f:  conv.i8
      IL_0010:  add.ovf.un
      IL_0011:  nop
      IL_0012:  stloc.0
      IL_0013:  ldc.i4.0
      IL_0014:  conv.i8
      IL_0015:  stloc.1
      IL_0016:  ldc.i4.1
      IL_0017:  conv.i8
      IL_0018:  stloc.2
      IL_0019:  br.s       IL_002b

      IL_001b:  ldloc.2
      IL_001c:  call       void assembly::set_c(int64)
      IL_0021:  ldloc.2
      IL_0022:  ldc.i4.1
      IL_0023:  conv.i8
      IL_0024:  add
      IL_0025:  stloc.2
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  conv.i8
      IL_0029:  add
      IL_002a:  stloc.1
      IL_002b:  ldloc.1
      IL_002c:  ldloc.0
      IL_002d:  blt.un.s   IL_001b

      IL_002f:  ret
    } 

    .method public static void  annotatedStart(int64 start,
                                               int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  annotatedFinish(int64 start,
                                                int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  inferredStartAndFinish(int64 start,
                                                       int64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               int64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.s      IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(int64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.s      IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.s    V_4
      IL_004b:  ldarg.0
      IL_004c:  stloc.3
      IL_004d:  br.s       IL_0061

      IL_004f:  ldloc.3
      IL_0050:  call       void assembly::set_c(int64)
      IL_0055:  ldloc.3
      IL_0056:  ldc.i4.1
      IL_0057:  conv.i8
      IL_0058:  add
      IL_0059:  stloc.3
      IL_005a:  ldloc.s    V_4
      IL_005c:  ldc.i4.1
      IL_005d:  conv.i8
      IL_005e:  add
      IL_005f:  stloc.s    V_4
      IL_0061:  ldloc.s    V_4
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

  } 

  .field static assembly int64 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int64 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int64 assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int64 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int64 assembly::c@1
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
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stsfld     int64 assembly::c@1
    IL_0007:  ret
  } 

  .property int64 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(int64)
    .get int64 assembly::get_c()
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






