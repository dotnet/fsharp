




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
  .method public specialname static native int get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(native int 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.i
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.i
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f1() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f2(native int start) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldarg.0
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.i
    IL_0024:  ldarg.0
    IL_0025:  sub
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  sizeof     [runtime]System.IntPtr
    IL_002e:  ldc.i4.4
    IL_002f:  bne.un.s   IL_0041

    IL_0031:  ldloc.0
    IL_0032:  ldc.i8     0xffffffff
    IL_003b:  conv.u
    IL_003c:  ceq
    IL_003e:  nop
    IL_003f:  br.s       IL_004f

    IL_0041:  ldloc.0
    IL_0042:  ldc.i8     0xffffffffffffffff
    IL_004b:  conv.u
    IL_004c:  ceq
    IL_004e:  nop
    IL_004f:  brfalse.s  IL_00ae

    IL_0051:  ldc.i8     0x0
    IL_005a:  conv.i
    IL_005b:  stloc.1
    IL_005c:  ldarg.0
    IL_005d:  stloc.2
    IL_005e:  ldloc.2
    IL_005f:  call       void assembly::set_c(native int)
    IL_0064:  ldloc.2
    IL_0065:  ldc.i8     0x1
    IL_006e:  conv.i
    IL_006f:  add
    IL_0070:  stloc.2
    IL_0071:  ldloc.1
    IL_0072:  ldc.i8     0x1
    IL_007b:  conv.i
    IL_007c:  add
    IL_007d:  stloc.1
    IL_007e:  br.s       IL_00a0

    IL_0080:  ldloc.2
    IL_0081:  call       void assembly::set_c(native int)
    IL_0086:  ldloc.2
    IL_0087:  ldc.i8     0x1
    IL_0090:  conv.i
    IL_0091:  add
    IL_0092:  stloc.2
    IL_0093:  ldloc.1
    IL_0094:  ldc.i8     0x1
    IL_009d:  conv.i
    IL_009e:  add
    IL_009f:  stloc.1
    IL_00a0:  ldloc.1
    IL_00a1:  ldc.i8     0x0
    IL_00aa:  conv.i
    IL_00ab:  bgt.un.s   IL_0080

    IL_00ad:  ret

    IL_00ae:  ldc.i8     0xa
    IL_00b7:  conv.i
    IL_00b8:  ldarg.0
    IL_00b9:  bge.s      IL_00c8

    IL_00bb:  ldc.i8     0x0
    IL_00c4:  conv.i
    IL_00c5:  nop
    IL_00c6:  br.s       IL_00e0

    IL_00c8:  ldc.i8     0xa
    IL_00d1:  conv.i
    IL_00d2:  ldarg.0
    IL_00d3:  sub
    IL_00d4:  ldc.i8     0x1
    IL_00dd:  conv.i
    IL_00de:  add.ovf.un
    IL_00df:  nop
    IL_00e0:  stloc.1
    IL_00e1:  ldc.i8     0x0
    IL_00ea:  conv.i
    IL_00eb:  stloc.2
    IL_00ec:  ldarg.0
    IL_00ed:  stloc.3
    IL_00ee:  br.s       IL_0110

    IL_00f0:  ldloc.3
    IL_00f1:  call       void assembly::set_c(native int)
    IL_00f6:  ldloc.3
    IL_00f7:  ldc.i8     0x1
    IL_0100:  conv.i
    IL_0101:  add
    IL_0102:  stloc.3
    IL_0103:  ldloc.2
    IL_0104:  ldc.i8     0x1
    IL_010d:  conv.i
    IL_010e:  add
    IL_010f:  stloc.2
    IL_0110:  ldloc.2
    IL_0111:  ldloc.1
    IL_0112:  blt.un.s   IL_00f0

    IL_0114:  ret
  } 

  .method public static void  f3(native int finish) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.i
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.i
    IL_0025:  sub
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  sizeof     [runtime]System.IntPtr
    IL_002e:  ldc.i4.4
    IL_002f:  bne.un.s   IL_0041

    IL_0031:  ldloc.0
    IL_0032:  ldc.i8     0xffffffff
    IL_003b:  conv.u
    IL_003c:  ceq
    IL_003e:  nop
    IL_003f:  br.s       IL_004f

    IL_0041:  ldloc.0
    IL_0042:  ldc.i8     0xffffffffffffffff
    IL_004b:  conv.u
    IL_004c:  ceq
    IL_004e:  nop
    IL_004f:  brfalse.s  IL_00b7

    IL_0051:  ldc.i8     0x0
    IL_005a:  conv.i
    IL_005b:  stloc.1
    IL_005c:  ldc.i8     0x1
    IL_0065:  conv.i
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  call       void assembly::set_c(native int)
    IL_006d:  ldloc.2
    IL_006e:  ldc.i8     0x1
    IL_0077:  conv.i
    IL_0078:  add
    IL_0079:  stloc.2
    IL_007a:  ldloc.1
    IL_007b:  ldc.i8     0x1
    IL_0084:  conv.i
    IL_0085:  add
    IL_0086:  stloc.1
    IL_0087:  br.s       IL_00a9

    IL_0089:  ldloc.2
    IL_008a:  call       void assembly::set_c(native int)
    IL_008f:  ldloc.2
    IL_0090:  ldc.i8     0x1
    IL_0099:  conv.i
    IL_009a:  add
    IL_009b:  stloc.2
    IL_009c:  ldloc.1
    IL_009d:  ldc.i8     0x1
    IL_00a6:  conv.i
    IL_00a7:  add
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.1
    IL_00aa:  ldc.i8     0x0
    IL_00b3:  conv.i
    IL_00b4:  bgt.un.s   IL_0089

    IL_00b6:  ret

    IL_00b7:  ldarg.0
    IL_00b8:  ldc.i8     0x1
    IL_00c1:  conv.i
    IL_00c2:  bge.s      IL_00d1

    IL_00c4:  ldc.i8     0x0
    IL_00cd:  conv.i
    IL_00ce:  nop
    IL_00cf:  br.s       IL_00e9

    IL_00d1:  ldarg.0
    IL_00d2:  ldc.i8     0x1
    IL_00db:  conv.i
    IL_00dc:  sub
    IL_00dd:  ldc.i8     0x1
    IL_00e6:  conv.i
    IL_00e7:  add.ovf.un
    IL_00e8:  nop
    IL_00e9:  stloc.1
    IL_00ea:  ldc.i8     0x0
    IL_00f3:  conv.i
    IL_00f4:  stloc.2
    IL_00f5:  ldc.i8     0x1
    IL_00fe:  conv.i
    IL_00ff:  stloc.3
    IL_0100:  br.s       IL_0122

    IL_0102:  ldloc.3
    IL_0103:  call       void assembly::set_c(native int)
    IL_0108:  ldloc.3
    IL_0109:  ldc.i8     0x1
    IL_0112:  conv.i
    IL_0113:  add
    IL_0114:  stloc.3
    IL_0115:  ldloc.2
    IL_0116:  ldc.i8     0x1
    IL_011f:  conv.i
    IL_0120:  add
    IL_0121:  stloc.2
    IL_0122:  ldloc.2
    IL_0123:  ldloc.1
    IL_0124:  blt.un.s   IL_0102

    IL_0126:  ret
  } 

  .method public static void  f4(native int start,
                                 native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0011

    IL_0004:  ldc.i8     0x0
    IL_000d:  conv.i
    IL_000e:  nop
    IL_000f:  br.s       IL_0015

    IL_0011:  ldarg.1
    IL_0012:  ldarg.0
    IL_0013:  sub
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  sizeof     [runtime]System.IntPtr
    IL_001c:  ldc.i4.4
    IL_001d:  bne.un.s   IL_002f

    IL_001f:  ldloc.0
    IL_0020:  ldc.i8     0xffffffff
    IL_0029:  conv.u
    IL_002a:  ceq
    IL_002c:  nop
    IL_002d:  br.s       IL_003d

    IL_002f:  ldloc.0
    IL_0030:  ldc.i8     0xffffffffffffffff
    IL_0039:  conv.u
    IL_003a:  ceq
    IL_003c:  nop
    IL_003d:  brfalse.s  IL_009c

    IL_003f:  ldc.i8     0x0
    IL_0048:  conv.i
    IL_0049:  stloc.1
    IL_004a:  ldarg.0
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  call       void assembly::set_c(native int)
    IL_0052:  ldloc.2
    IL_0053:  ldc.i8     0x1
    IL_005c:  conv.i
    IL_005d:  add
    IL_005e:  stloc.2
    IL_005f:  ldloc.1
    IL_0060:  ldc.i8     0x1
    IL_0069:  conv.i
    IL_006a:  add
    IL_006b:  stloc.1
    IL_006c:  br.s       IL_008e

    IL_006e:  ldloc.2
    IL_006f:  call       void assembly::set_c(native int)
    IL_0074:  ldloc.2
    IL_0075:  ldc.i8     0x1
    IL_007e:  conv.i
    IL_007f:  add
    IL_0080:  stloc.2
    IL_0081:  ldloc.1
    IL_0082:  ldc.i8     0x1
    IL_008b:  conv.i
    IL_008c:  add
    IL_008d:  stloc.1
    IL_008e:  ldloc.1
    IL_008f:  ldc.i8     0x0
    IL_0098:  conv.i
    IL_0099:  bgt.un.s   IL_006e

    IL_009b:  ret

    IL_009c:  ldarg.1
    IL_009d:  ldarg.0
    IL_009e:  bge.s      IL_00ad

    IL_00a0:  ldc.i8     0x0
    IL_00a9:  conv.i
    IL_00aa:  nop
    IL_00ab:  br.s       IL_00bc

    IL_00ad:  ldarg.1
    IL_00ae:  ldarg.0
    IL_00af:  sub
    IL_00b0:  ldc.i8     0x1
    IL_00b9:  conv.i
    IL_00ba:  add.ovf.un
    IL_00bb:  nop
    IL_00bc:  stloc.1
    IL_00bd:  ldc.i8     0x0
    IL_00c6:  conv.i
    IL_00c7:  stloc.2
    IL_00c8:  ldarg.0
    IL_00c9:  stloc.3
    IL_00ca:  br.s       IL_00ec

    IL_00cc:  ldloc.3
    IL_00cd:  call       void assembly::set_c(native int)
    IL_00d2:  ldloc.3
    IL_00d3:  ldc.i8     0x1
    IL_00dc:  conv.i
    IL_00dd:  add
    IL_00de:  stloc.3
    IL_00df:  ldloc.2
    IL_00e0:  ldc.i8     0x1
    IL_00e9:  conv.i
    IL_00ea:  add
    IL_00eb:  stloc.2
    IL_00ec:  ldloc.2
    IL_00ed:  ldloc.1
    IL_00ee:  blt.un.s   IL_00cc

    IL_00f0:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f6() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x2
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x5
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f7(native int start) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldarg.0
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.i
    IL_0024:  ldarg.0
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.i
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.i
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  stloc.1
    IL_0049:  ldarg.0
    IL_004a:  stloc.2
    IL_004b:  br.s       IL_006d

    IL_004d:  ldloc.2
    IL_004e:  call       void assembly::set_c(native int)
    IL_0053:  ldloc.2
    IL_0054:  ldc.i8     0x2
    IL_005d:  conv.i
    IL_005e:  add
    IL_005f:  stloc.2
    IL_0060:  ldloc.1
    IL_0061:  ldc.i8     0x1
    IL_006a:  conv.i
    IL_006b:  add
    IL_006c:  stloc.1
    IL_006d:  ldloc.1
    IL_006e:  ldloc.0
    IL_006f:  blt.un.s   IL_004d

    IL_0071:  ret
  } 

  .method public static void  f8(native int step) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.i
    IL_000b:  bne.un.s   IL_002b

    IL_000d:  ldc.i8     0x1
    IL_0016:  conv.i
    IL_0017:  ldarg.0
    IL_0018:  ldc.i8     0xa
    IL_0021:  conv.i
    IL_0022:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0027:  pop
    IL_0028:  nop
    IL_0029:  br.s       IL_002c

    IL_002b:  nop
    IL_002c:  ldc.i8     0x0
    IL_0035:  conv.i
    IL_0036:  ldarg.0
    IL_0037:  bge.s      IL_0076

    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.i
    IL_0043:  ldc.i8     0x1
    IL_004c:  conv.i
    IL_004d:  bge.s      IL_005c

    IL_004f:  ldc.i8     0x0
    IL_0058:  conv.i
    IL_0059:  nop
    IL_005a:  br.s       IL_00bd

    IL_005c:  ldc.i8     0xa
    IL_0065:  conv.i
    IL_0066:  ldc.i8     0x1
    IL_006f:  conv.i
    IL_0070:  sub
    IL_0071:  ldarg.0
    IL_0072:  div.un
    IL_0073:  nop
    IL_0074:  br.s       IL_00bd

    IL_0076:  ldc.i8     0x1
    IL_007f:  conv.i
    IL_0080:  ldc.i8     0xa
    IL_0089:  conv.i
    IL_008a:  bge.s      IL_0099

    IL_008c:  ldc.i8     0x0
    IL_0095:  conv.i
    IL_0096:  nop
    IL_0097:  br.s       IL_00bd

    IL_0099:  ldc.i8     0x1
    IL_00a2:  conv.i
    IL_00a3:  ldc.i8     0xa
    IL_00ac:  conv.i
    IL_00ad:  sub
    IL_00ae:  ldarg.0
    IL_00af:  not
    IL_00b0:  ldc.i8     0x1
    IL_00b9:  conv.i
    IL_00ba:  add
    IL_00bb:  div.un
    IL_00bc:  nop
    IL_00bd:  stloc.0
    IL_00be:  sizeof     [runtime]System.IntPtr
    IL_00c4:  ldc.i4.4
    IL_00c5:  bne.un.s   IL_00d7

    IL_00c7:  ldloc.0
    IL_00c8:  ldc.i8     0xffffffff
    IL_00d1:  conv.u
    IL_00d2:  ceq
    IL_00d4:  nop
    IL_00d5:  br.s       IL_00e5

    IL_00d7:  ldloc.0
    IL_00d8:  ldc.i8     0xffffffffffffffff
    IL_00e1:  conv.u
    IL_00e2:  ceq
    IL_00e4:  nop
    IL_00e5:  brfalse.s  IL_013b

    IL_00e7:  ldc.i8     0x0
    IL_00f0:  conv.i
    IL_00f1:  stloc.1
    IL_00f2:  ldc.i8     0x1
    IL_00fb:  conv.i
    IL_00fc:  stloc.2
    IL_00fd:  ldloc.2
    IL_00fe:  call       void assembly::set_c(native int)
    IL_0103:  ldloc.2
    IL_0104:  ldarg.0
    IL_0105:  add
    IL_0106:  stloc.2
    IL_0107:  ldloc.1
    IL_0108:  ldc.i8     0x1
    IL_0111:  conv.i
    IL_0112:  add
    IL_0113:  stloc.1
    IL_0114:  br.s       IL_012d

    IL_0116:  ldloc.2
    IL_0117:  call       void assembly::set_c(native int)
    IL_011c:  ldloc.2
    IL_011d:  ldarg.0
    IL_011e:  add
    IL_011f:  stloc.2
    IL_0120:  ldloc.1
    IL_0121:  ldc.i8     0x1
    IL_012a:  conv.i
    IL_012b:  add
    IL_012c:  stloc.1
    IL_012d:  ldloc.1
    IL_012e:  ldc.i8     0x0
    IL_0137:  conv.i
    IL_0138:  bgt.un.s   IL_0116

    IL_013a:  ret

    IL_013b:  ldarg.0
    IL_013c:  ldc.i8     0x0
    IL_0145:  conv.i
    IL_0146:  bne.un.s   IL_0166

    IL_0148:  ldc.i8     0x1
    IL_0151:  conv.i
    IL_0152:  ldarg.0
    IL_0153:  ldc.i8     0xa
    IL_015c:  conv.i
    IL_015d:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0162:  pop
    IL_0163:  nop
    IL_0164:  br.s       IL_0167

    IL_0166:  nop
    IL_0167:  ldc.i8     0x0
    IL_0170:  conv.i
    IL_0171:  ldarg.0
    IL_0172:  bge.s      IL_01bf

    IL_0174:  ldc.i8     0xa
    IL_017d:  conv.i
    IL_017e:  ldc.i8     0x1
    IL_0187:  conv.i
    IL_0188:  bge.s      IL_019a

    IL_018a:  ldc.i8     0x0
    IL_0193:  conv.i
    IL_0194:  nop
    IL_0195:  br         IL_0211

    IL_019a:  ldc.i8     0xa
    IL_01a3:  conv.i
    IL_01a4:  ldc.i8     0x1
    IL_01ad:  conv.i
    IL_01ae:  sub
    IL_01af:  ldarg.0
    IL_01b0:  div.un
    IL_01b1:  ldc.i8     0x1
    IL_01ba:  conv.i
    IL_01bb:  add.ovf.un
    IL_01bc:  nop
    IL_01bd:  br.s       IL_0211

    IL_01bf:  ldc.i8     0x1
    IL_01c8:  conv.i
    IL_01c9:  ldc.i8     0xa
    IL_01d2:  conv.i
    IL_01d3:  bge.s      IL_01e2

    IL_01d5:  ldc.i8     0x0
    IL_01de:  conv.i
    IL_01df:  nop
    IL_01e0:  br.s       IL_0211

    IL_01e2:  ldc.i8     0x1
    IL_01eb:  conv.i
    IL_01ec:  ldc.i8     0xa
    IL_01f5:  conv.i
    IL_01f6:  sub
    IL_01f7:  ldarg.0
    IL_01f8:  not
    IL_01f9:  ldc.i8     0x1
    IL_0202:  conv.i
    IL_0203:  add
    IL_0204:  div.un
    IL_0205:  ldc.i8     0x1
    IL_020e:  conv.i
    IL_020f:  add.ovf.un
    IL_0210:  nop
    IL_0211:  stloc.1
    IL_0212:  ldc.i8     0x0
    IL_021b:  conv.i
    IL_021c:  stloc.2
    IL_021d:  ldc.i8     0x1
    IL_0226:  conv.i
    IL_0227:  stloc.3
    IL_0228:  br.s       IL_0241

    IL_022a:  ldloc.3
    IL_022b:  call       void assembly::set_c(native int)
    IL_0230:  ldloc.3
    IL_0231:  ldarg.0
    IL_0232:  add
    IL_0233:  stloc.3
    IL_0234:  ldloc.2
    IL_0235:  ldc.i8     0x1
    IL_023e:  conv.i
    IL_023f:  add
    IL_0240:  stloc.2
    IL_0241:  ldloc.2
    IL_0242:  ldloc.1
    IL_0243:  blt.un.s   IL_022a

    IL_0245:  ret
  } 

  .method public static void  f9(native int finish) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.i
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.i
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.i
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.i
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  stloc.1
    IL_0049:  ldc.i8     0x1
    IL_0052:  conv.i
    IL_0053:  stloc.2
    IL_0054:  br.s       IL_0076

    IL_0056:  ldloc.2
    IL_0057:  call       void assembly::set_c(native int)
    IL_005c:  ldloc.2
    IL_005d:  ldc.i8     0x2
    IL_0066:  conv.i
    IL_0067:  add
    IL_0068:  stloc.2
    IL_0069:  ldloc.1
    IL_006a:  ldc.i8     0x1
    IL_0073:  conv.i
    IL_0074:  add
    IL_0075:  stloc.1
    IL_0076:  ldloc.1
    IL_0077:  ldloc.0
    IL_0078:  blt.un.s   IL_0056

    IL_007a:  ret
  } 

  .method public static void  f10(native int start,
                                  native int step,
                                  native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.i
    IL_000b:  bne.un.s   IL_0019

    IL_000d:  ldarg.2
    IL_000e:  ldarg.1
    IL_000f:  ldarg.2
    IL_0010:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0015:  pop
    IL_0016:  nop
    IL_0017:  br.s       IL_001a

    IL_0019:  nop
    IL_001a:  ldc.i8     0x0
    IL_0023:  conv.i
    IL_0024:  ldarg.1
    IL_0025:  bge.s      IL_0040

    IL_0027:  ldarg.2
    IL_0028:  ldarg.2
    IL_0029:  bge.s      IL_0038

    IL_002b:  ldc.i8     0x0
    IL_0034:  conv.i
    IL_0035:  nop
    IL_0036:  br.s       IL_0063

    IL_0038:  ldarg.2
    IL_0039:  ldarg.2
    IL_003a:  sub
    IL_003b:  ldarg.1
    IL_003c:  div.un
    IL_003d:  nop
    IL_003e:  br.s       IL_0063

    IL_0040:  ldarg.2
    IL_0041:  ldarg.2
    IL_0042:  bge.s      IL_0051

    IL_0044:  ldc.i8     0x0
    IL_004d:  conv.i
    IL_004e:  nop
    IL_004f:  br.s       IL_0063

    IL_0051:  ldarg.2
    IL_0052:  ldarg.2
    IL_0053:  sub
    IL_0054:  ldarg.1
    IL_0055:  not
    IL_0056:  ldc.i8     0x1
    IL_005f:  conv.i
    IL_0060:  add
    IL_0061:  div.un
    IL_0062:  nop
    IL_0063:  stloc.0
    IL_0064:  sizeof     [runtime]System.IntPtr
    IL_006a:  ldc.i4.4
    IL_006b:  bne.un.s   IL_007d

    IL_006d:  ldloc.0
    IL_006e:  ldc.i8     0xffffffff
    IL_0077:  conv.u
    IL_0078:  ceq
    IL_007a:  nop
    IL_007b:  br.s       IL_008b

    IL_007d:  ldloc.0
    IL_007e:  ldc.i8     0xffffffffffffffff
    IL_0087:  conv.u
    IL_0088:  ceq
    IL_008a:  nop
    IL_008b:  brfalse.s  IL_00d8

    IL_008d:  ldc.i8     0x0
    IL_0096:  conv.i
    IL_0097:  stloc.1
    IL_0098:  ldarg.2
    IL_0099:  stloc.2
    IL_009a:  ldloc.2
    IL_009b:  call       void assembly::set_c(native int)
    IL_00a0:  ldloc.2
    IL_00a1:  ldarg.1
    IL_00a2:  add
    IL_00a3:  stloc.2
    IL_00a4:  ldloc.1
    IL_00a5:  ldc.i8     0x1
    IL_00ae:  conv.i
    IL_00af:  add
    IL_00b0:  stloc.1
    IL_00b1:  br.s       IL_00ca

    IL_00b3:  ldloc.2
    IL_00b4:  call       void assembly::set_c(native int)
    IL_00b9:  ldloc.2
    IL_00ba:  ldarg.1
    IL_00bb:  add
    IL_00bc:  stloc.2
    IL_00bd:  ldloc.1
    IL_00be:  ldc.i8     0x1
    IL_00c7:  conv.i
    IL_00c8:  add
    IL_00c9:  stloc.1
    IL_00ca:  ldloc.1
    IL_00cb:  ldc.i8     0x0
    IL_00d4:  conv.i
    IL_00d5:  bgt.un.s   IL_00b3

    IL_00d7:  ret

    IL_00d8:  ldarg.1
    IL_00d9:  ldc.i8     0x0
    IL_00e2:  conv.i
    IL_00e3:  bne.un.s   IL_00f1

    IL_00e5:  ldarg.2
    IL_00e6:  ldarg.1
    IL_00e7:  ldarg.2
    IL_00e8:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_00ed:  pop
    IL_00ee:  nop
    IL_00ef:  br.s       IL_00f2

    IL_00f1:  nop
    IL_00f2:  ldc.i8     0x0
    IL_00fb:  conv.i
    IL_00fc:  ldarg.1
    IL_00fd:  bge.s      IL_0123

    IL_00ff:  ldarg.2
    IL_0100:  ldarg.2
    IL_0101:  bge.s      IL_0110

    IL_0103:  ldc.i8     0x0
    IL_010c:  conv.i
    IL_010d:  nop
    IL_010e:  br.s       IL_0151

    IL_0110:  ldarg.2
    IL_0111:  ldarg.2
    IL_0112:  sub
    IL_0113:  ldarg.1
    IL_0114:  div.un
    IL_0115:  ldc.i8     0x1
    IL_011e:  conv.i
    IL_011f:  add.ovf.un
    IL_0120:  nop
    IL_0121:  br.s       IL_0151

    IL_0123:  ldarg.2
    IL_0124:  ldarg.2
    IL_0125:  bge.s      IL_0134

    IL_0127:  ldc.i8     0x0
    IL_0130:  conv.i
    IL_0131:  nop
    IL_0132:  br.s       IL_0151

    IL_0134:  ldarg.2
    IL_0135:  ldarg.2
    IL_0136:  sub
    IL_0137:  ldarg.1
    IL_0138:  not
    IL_0139:  ldc.i8     0x1
    IL_0142:  conv.i
    IL_0143:  add
    IL_0144:  div.un
    IL_0145:  ldc.i8     0x1
    IL_014e:  conv.i
    IL_014f:  add.ovf.un
    IL_0150:  nop
    IL_0151:  stloc.1
    IL_0152:  ldc.i8     0x0
    IL_015b:  conv.i
    IL_015c:  stloc.2
    IL_015d:  ldarg.2
    IL_015e:  stloc.3
    IL_015f:  br.s       IL_0178

    IL_0161:  ldloc.3
    IL_0162:  call       void assembly::set_c(native int)
    IL_0167:  ldloc.3
    IL_0168:  ldarg.1
    IL_0169:  add
    IL_016a:  stloc.3
    IL_016b:  ldloc.2
    IL_016c:  ldc.i8     0x1
    IL_0175:  conv.i
    IL_0176:  add
    IL_0177:  stloc.2
    IL_0178:  ldloc.2
    IL_0179:  ldloc.1
    IL_017a:  blt.un.s   IL_0161

    IL_017c:  ret
  } 

  .method public static void  f11(native int start,
                                  native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             native int V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_001d

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(native int)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldarg.0
    IL_001f:  ldc.i8     0x0
    IL_0028:  conv.i
    IL_0029:  ldarg.1
    IL_002a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_002f:  pop
    IL_0030:  ldc.i4.m1
    IL_0031:  blt.un.s   IL_0006

    IL_0033:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0,
             native int V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i8     0x1
    IL_000b:  conv.i
    IL_000c:  stloc.1
    IL_000d:  br.s       IL_0026

    IL_000f:  ldloc.1
    IL_0010:  call       void assembly::set_c(native int)
    IL_0015:  ldloc.1
    IL_0016:  ldc.i8     0x0
    IL_001f:  conv.i
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.0
    IL_0027:  ldc.i8     0x1
    IL_0030:  conv.i
    IL_0031:  ldc.i8     0x0
    IL_003a:  conv.i
    IL_003b:  ldc.i8     0xa
    IL_0044:  conv.i
    IL_0045:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_004a:  pop
    IL_004b:  ldc.i4.m1
    IL_004c:  blt.un.s   IL_000f

    IL_004e:  ret
  } 

  .method public static void  f13() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldc.i8     0x1
    IL_0013:  conv.i
    IL_0014:  bge.s      IL_0023

    IL_0016:  ldc.i8     0x0
    IL_001f:  conv.i
    IL_0020:  nop
    IL_0021:  br.s       IL_0039

    IL_0023:  ldc.i8     0xa
    IL_002c:  conv.i
    IL_002d:  ldc.i8     0x1
    IL_0036:  conv.i
    IL_0037:  sub
    IL_0038:  nop
    IL_0039:  stloc.0
    IL_003a:  sizeof     [runtime]System.IntPtr
    IL_0040:  ldc.i4.4
    IL_0041:  bne.un.s   IL_0053

    IL_0043:  ldloc.0
    IL_0044:  ldc.i8     0xffffffff
    IL_004d:  conv.u
    IL_004e:  ceq
    IL_0050:  nop
    IL_0051:  br.s       IL_0061

    IL_0053:  ldloc.0
    IL_0054:  ldc.i8     0xffffffffffffffff
    IL_005d:  conv.u
    IL_005e:  ceq
    IL_0060:  nop
    IL_0061:  brfalse.s  IL_00c9

    IL_0063:  ldc.i8     0x0
    IL_006c:  conv.i
    IL_006d:  stloc.1
    IL_006e:  ldc.i8     0xa
    IL_0077:  conv.i
    IL_0078:  stloc.2
    IL_0079:  ldloc.2
    IL_007a:  call       void assembly::set_c(native int)
    IL_007f:  ldloc.2
    IL_0080:  ldc.i8     0xffffffffffffffff
    IL_0089:  conv.i
    IL_008a:  add
    IL_008b:  stloc.2
    IL_008c:  ldloc.1
    IL_008d:  ldc.i8     0x1
    IL_0096:  conv.i
    IL_0097:  add
    IL_0098:  stloc.1
    IL_0099:  br.s       IL_00bb

    IL_009b:  ldloc.2
    IL_009c:  call       void assembly::set_c(native int)
    IL_00a1:  ldloc.2
    IL_00a2:  ldc.i8     0xffffffffffffffff
    IL_00ab:  conv.i
    IL_00ac:  add
    IL_00ad:  stloc.2
    IL_00ae:  ldloc.1
    IL_00af:  ldc.i8     0x1
    IL_00b8:  conv.i
    IL_00b9:  add
    IL_00ba:  stloc.1
    IL_00bb:  ldloc.1
    IL_00bc:  ldc.i8     0x0
    IL_00c5:  conv.i
    IL_00c6:  bgt.un.s   IL_009b

    IL_00c8:  ret

    IL_00c9:  ldc.i8     0xa
    IL_00d2:  conv.i
    IL_00d3:  ldc.i8     0x1
    IL_00dc:  conv.i
    IL_00dd:  bge.s      IL_00ec

    IL_00df:  ldc.i8     0x0
    IL_00e8:  conv.i
    IL_00e9:  nop
    IL_00ea:  br.s       IL_010d

    IL_00ec:  ldc.i8     0xa
    IL_00f5:  conv.i
    IL_00f6:  ldc.i8     0x1
    IL_00ff:  conv.i
    IL_0100:  sub
    IL_0101:  ldc.i8     0x1
    IL_010a:  conv.i
    IL_010b:  add.ovf.un
    IL_010c:  nop
    IL_010d:  stloc.1
    IL_010e:  ldc.i8     0x0
    IL_0117:  conv.i
    IL_0118:  stloc.2
    IL_0119:  ldc.i8     0xa
    IL_0122:  conv.i
    IL_0123:  stloc.3
    IL_0124:  br.s       IL_0146

    IL_0126:  ldloc.3
    IL_0127:  call       void assembly::set_c(native int)
    IL_012c:  ldloc.3
    IL_012d:  ldc.i8     0xffffffffffffffff
    IL_0136:  conv.i
    IL_0137:  add
    IL_0138:  stloc.3
    IL_0139:  ldloc.2
    IL_013a:  ldc.i8     0x1
    IL_0143:  conv.i
    IL_0144:  add
    IL_0145:  stloc.2
    IL_0146:  ldloc.2
    IL_0147:  ldloc.1
    IL_0148:  blt.un.s   IL_0126

    IL_014a:  ret
  } 

  .method public static void  f14() cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             native int V_1,
             native int V_2,
             native int V_3)
    IL_0000:  ldc.i8     0xfffffffffffffffe
    IL_0009:  conv.i
    IL_000a:  ldc.i8     0x0
    IL_0013:  conv.i
    IL_0014:  bne.un.s   IL_003d

    IL_0016:  ldc.i8     0xa
    IL_001f:  conv.i
    IL_0020:  ldc.i8     0xfffffffffffffffe
    IL_0029:  conv.i
    IL_002a:  ldc.i8     0x1
    IL_0033:  conv.i
    IL_0034:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0039:  pop
    IL_003a:  nop
    IL_003b:  br.s       IL_003e

    IL_003d:  nop
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  ldc.i8     0xfffffffffffffffe
    IL_0051:  conv.i
    IL_0052:  bge.s      IL_009a

    IL_0054:  ldc.i8     0x1
    IL_005d:  conv.i
    IL_005e:  ldc.i8     0xa
    IL_0067:  conv.i
    IL_0068:  bge.s      IL_0077

    IL_006a:  ldc.i8     0x0
    IL_0073:  conv.i
    IL_0074:  nop
    IL_0075:  br.s       IL_00ea

    IL_0077:  ldc.i8     0x1
    IL_0080:  conv.i
    IL_0081:  ldc.i8     0xa
    IL_008a:  conv.i
    IL_008b:  sub
    IL_008c:  ldc.i8     0xfffffffffffffffe
    IL_0095:  conv.i
    IL_0096:  div.un
    IL_0097:  nop
    IL_0098:  br.s       IL_00ea

    IL_009a:  ldc.i8     0xa
    IL_00a3:  conv.i
    IL_00a4:  ldc.i8     0x1
    IL_00ad:  conv.i
    IL_00ae:  bge.s      IL_00bd

    IL_00b0:  ldc.i8     0x0
    IL_00b9:  conv.i
    IL_00ba:  nop
    IL_00bb:  br.s       IL_00ea

    IL_00bd:  ldc.i8     0xa
    IL_00c6:  conv.i
    IL_00c7:  ldc.i8     0x1
    IL_00d0:  conv.i
    IL_00d1:  sub
    IL_00d2:  ldc.i8     0xfffffffffffffffe
    IL_00db:  conv.i
    IL_00dc:  not
    IL_00dd:  ldc.i8     0x1
    IL_00e6:  conv.i
    IL_00e7:  add
    IL_00e8:  div.un
    IL_00e9:  nop
    IL_00ea:  stloc.0
    IL_00eb:  sizeof     [runtime]System.IntPtr
    IL_00f1:  ldc.i4.4
    IL_00f2:  bne.un.s   IL_0104

    IL_00f4:  ldloc.0
    IL_00f5:  ldc.i8     0xffffffff
    IL_00fe:  conv.u
    IL_00ff:  ceq
    IL_0101:  nop
    IL_0102:  br.s       IL_0112

    IL_0104:  ldloc.0
    IL_0105:  ldc.i8     0xffffffffffffffff
    IL_010e:  conv.u
    IL_010f:  ceq
    IL_0111:  nop
    IL_0112:  brfalse.s  IL_017a

    IL_0114:  ldc.i8     0x0
    IL_011d:  conv.i
    IL_011e:  stloc.1
    IL_011f:  ldc.i8     0xa
    IL_0128:  conv.i
    IL_0129:  stloc.2
    IL_012a:  ldloc.2
    IL_012b:  call       void assembly::set_c(native int)
    IL_0130:  ldloc.2
    IL_0131:  ldc.i8     0xfffffffffffffffe
    IL_013a:  conv.i
    IL_013b:  add
    IL_013c:  stloc.2
    IL_013d:  ldloc.1
    IL_013e:  ldc.i8     0x1
    IL_0147:  conv.i
    IL_0148:  add
    IL_0149:  stloc.1
    IL_014a:  br.s       IL_016c

    IL_014c:  ldloc.2
    IL_014d:  call       void assembly::set_c(native int)
    IL_0152:  ldloc.2
    IL_0153:  ldc.i8     0xfffffffffffffffe
    IL_015c:  conv.i
    IL_015d:  add
    IL_015e:  stloc.2
    IL_015f:  ldloc.1
    IL_0160:  ldc.i8     0x1
    IL_0169:  conv.i
    IL_016a:  add
    IL_016b:  stloc.1
    IL_016c:  ldloc.1
    IL_016d:  ldc.i8     0x0
    IL_0176:  conv.i
    IL_0177:  bgt.un.s   IL_014c

    IL_0179:  ret

    IL_017a:  ldc.i8     0xfffffffffffffffe
    IL_0183:  conv.i
    IL_0184:  ldc.i8     0x0
    IL_018d:  conv.i
    IL_018e:  bne.un.s   IL_01b7

    IL_0190:  ldc.i8     0xa
    IL_0199:  conv.i
    IL_019a:  ldc.i8     0xfffffffffffffffe
    IL_01a3:  conv.i
    IL_01a4:  ldc.i8     0x1
    IL_01ad:  conv.i
    IL_01ae:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_01b3:  pop
    IL_01b4:  nop
    IL_01b5:  br.s       IL_01b8

    IL_01b7:  nop
    IL_01b8:  ldc.i8     0x0
    IL_01c1:  conv.i
    IL_01c2:  ldc.i8     0xfffffffffffffffe
    IL_01cb:  conv.i
    IL_01cc:  bge.s      IL_0222

    IL_01ce:  ldc.i8     0x1
    IL_01d7:  conv.i
    IL_01d8:  ldc.i8     0xa
    IL_01e1:  conv.i
    IL_01e2:  bge.s      IL_01f4

    IL_01e4:  ldc.i8     0x0
    IL_01ed:  conv.i
    IL_01ee:  nop
    IL_01ef:  br         IL_027d

    IL_01f4:  ldc.i8     0x1
    IL_01fd:  conv.i
    IL_01fe:  ldc.i8     0xa
    IL_0207:  conv.i
    IL_0208:  sub
    IL_0209:  ldc.i8     0xfffffffffffffffe
    IL_0212:  conv.i
    IL_0213:  div.un
    IL_0214:  ldc.i8     0x1
    IL_021d:  conv.i
    IL_021e:  add.ovf.un
    IL_021f:  nop
    IL_0220:  br.s       IL_027d

    IL_0222:  ldc.i8     0xa
    IL_022b:  conv.i
    IL_022c:  ldc.i8     0x1
    IL_0235:  conv.i
    IL_0236:  bge.s      IL_0245

    IL_0238:  ldc.i8     0x0
    IL_0241:  conv.i
    IL_0242:  nop
    IL_0243:  br.s       IL_027d

    IL_0245:  ldc.i8     0xa
    IL_024e:  conv.i
    IL_024f:  ldc.i8     0x1
    IL_0258:  conv.i
    IL_0259:  sub
    IL_025a:  ldc.i8     0xfffffffffffffffe
    IL_0263:  conv.i
    IL_0264:  not
    IL_0265:  ldc.i8     0x1
    IL_026e:  conv.i
    IL_026f:  add
    IL_0270:  div.un
    IL_0271:  ldc.i8     0x1
    IL_027a:  conv.i
    IL_027b:  add.ovf.un
    IL_027c:  nop
    IL_027d:  stloc.1
    IL_027e:  ldc.i8     0x0
    IL_0287:  conv.i
    IL_0288:  stloc.2
    IL_0289:  ldc.i8     0xa
    IL_0292:  conv.i
    IL_0293:  stloc.3
    IL_0294:  br.s       IL_02b6

    IL_0296:  ldloc.3
    IL_0297:  call       void assembly::set_c(native int)
    IL_029c:  ldloc.3
    IL_029d:  ldc.i8     0xfffffffffffffffe
    IL_02a6:  conv.i
    IL_02a7:  add
    IL_02a8:  stloc.3
    IL_02a9:  ldloc.2
    IL_02aa:  ldc.i8     0x1
    IL_02b3:  conv.i
    IL_02b4:  add
    IL_02b5:  stloc.2
    IL_02b6:  ldloc.2
    IL_02b7:  ldloc.1
    IL_02b8:  blt.un.s   IL_0296

    IL_02ba:  ret
  } 

  .property native int c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(native int)
    .get native int assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly native int c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_000f:  ret
  } 

} 






