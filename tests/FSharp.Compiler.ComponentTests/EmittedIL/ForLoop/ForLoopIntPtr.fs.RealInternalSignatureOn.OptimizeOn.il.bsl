




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
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
      IL_0000:  ldc.i8     0x1
      IL_0009:  conv.i
      IL_000a:  ldc.i8     0xa
      IL_0013:  conv.i
      IL_0014:  bge.s      IL_0023

      IL_0016:  ldc.i8     0x0
      IL_001f:  conv.i
      IL_0020:  nop
      IL_0021:  br.s       IL_0039

      IL_0023:  ldc.i8     0x1
      IL_002c:  conv.i
      IL_002d:  ldc.i8     0xa
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
      IL_0061:  brfalse.s  IL_00af

      IL_0063:  ldc.i4.1
      IL_0064:  stloc.1
      IL_0065:  ldc.i8     0x0
      IL_006e:  conv.i
      IL_006f:  stloc.2
      IL_0070:  ldc.i8     0x1
      IL_0079:  conv.i
      IL_007a:  stloc.3
      IL_007b:  br.s       IL_00ab

      IL_007d:  ldloc.3
      IL_007e:  call       void assembly::set_c(native int)
      IL_0083:  ldloc.3
      IL_0084:  ldc.i8     0xffffffffffffffff
      IL_008d:  conv.i
      IL_008e:  add
      IL_008f:  stloc.3
      IL_0090:  ldloc.2
      IL_0091:  ldc.i8     0x1
      IL_009a:  conv.i
      IL_009b:  add
      IL_009c:  stloc.2
      IL_009d:  ldloc.2
      IL_009e:  ldc.i8     0x0
      IL_00a7:  conv.i
      IL_00a8:  cgt.un
      IL_00aa:  stloc.1
      IL_00ab:  ldloc.1
      IL_00ac:  brtrue.s   IL_007d

      IL_00ae:  ret

      IL_00af:  ldc.i8     0x1
      IL_00b8:  conv.i
      IL_00b9:  ldc.i8     0xa
      IL_00c2:  conv.i
      IL_00c3:  bge.s      IL_00d2

      IL_00c5:  ldc.i8     0x0
      IL_00ce:  conv.i
      IL_00cf:  nop
      IL_00d0:  br.s       IL_00f3

      IL_00d2:  ldc.i8     0x1
      IL_00db:  conv.i
      IL_00dc:  ldc.i8     0xa
      IL_00e5:  conv.i
      IL_00e6:  sub
      IL_00e7:  ldc.i8     0x1
      IL_00f0:  conv.i
      IL_00f1:  add.ovf.un
      IL_00f2:  nop
      IL_00f3:  stloc.2
      IL_00f4:  ldc.i8     0x0
      IL_00fd:  conv.i
      IL_00fe:  stloc.3
      IL_00ff:  ldc.i8     0x1
      IL_0108:  conv.i
      IL_0109:  stloc.s    V_4
      IL_010b:  br.s       IL_0130

      IL_010d:  ldloc.s    V_4
      IL_010f:  call       void assembly::set_c(native int)
      IL_0114:  ldloc.s    V_4
      IL_0116:  ldc.i8     0xffffffffffffffff
      IL_011f:  conv.i
      IL_0120:  add
      IL_0121:  stloc.s    V_4
      IL_0123:  ldloc.3
      IL_0124:  ldc.i8     0x1
      IL_012d:  conv.i
      IL_012e:  add
      IL_012f:  stloc.3
      IL_0130:  ldloc.3
      IL_0131:  ldloc.2
      IL_0132:  blt.un.s   IL_010d

      IL_0134:  ret
    } 

    .method public static void  constNonEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_0061:  brfalse.s  IL_00af

      IL_0063:  ldc.i4.1
      IL_0064:  stloc.1
      IL_0065:  ldc.i8     0x0
      IL_006e:  conv.i
      IL_006f:  stloc.2
      IL_0070:  ldc.i8     0xa
      IL_0079:  conv.i
      IL_007a:  stloc.3
      IL_007b:  br.s       IL_00ab

      IL_007d:  ldloc.3
      IL_007e:  call       void assembly::set_c(native int)
      IL_0083:  ldloc.3
      IL_0084:  ldc.i8     0xffffffffffffffff
      IL_008d:  conv.i
      IL_008e:  add
      IL_008f:  stloc.3
      IL_0090:  ldloc.2
      IL_0091:  ldc.i8     0x1
      IL_009a:  conv.i
      IL_009b:  add
      IL_009c:  stloc.2
      IL_009d:  ldloc.2
      IL_009e:  ldc.i8     0x0
      IL_00a7:  conv.i
      IL_00a8:  cgt.un
      IL_00aa:  stloc.1
      IL_00ab:  ldloc.1
      IL_00ac:  brtrue.s   IL_007d

      IL_00ae:  ret

      IL_00af:  ldc.i8     0xa
      IL_00b8:  conv.i
      IL_00b9:  ldc.i8     0x1
      IL_00c2:  conv.i
      IL_00c3:  bge.s      IL_00d2

      IL_00c5:  ldc.i8     0x0
      IL_00ce:  conv.i
      IL_00cf:  nop
      IL_00d0:  br.s       IL_00f3

      IL_00d2:  ldc.i8     0xa
      IL_00db:  conv.i
      IL_00dc:  ldc.i8     0x1
      IL_00e5:  conv.i
      IL_00e6:  sub
      IL_00e7:  ldc.i8     0x1
      IL_00f0:  conv.i
      IL_00f1:  add.ovf.un
      IL_00f2:  nop
      IL_00f3:  stloc.2
      IL_00f4:  ldc.i8     0x0
      IL_00fd:  conv.i
      IL_00fe:  stloc.3
      IL_00ff:  ldc.i8     0xa
      IL_0108:  conv.i
      IL_0109:  stloc.s    V_4
      IL_010b:  br.s       IL_0130

      IL_010d:  ldloc.s    V_4
      IL_010f:  call       void assembly::set_c(native int)
      IL_0114:  ldloc.s    V_4
      IL_0116:  ldc.i8     0xffffffffffffffff
      IL_011f:  conv.i
      IL_0120:  add
      IL_0121:  stloc.s    V_4
      IL_0123:  ldloc.3
      IL_0124:  ldc.i8     0x1
      IL_012d:  conv.i
      IL_012e:  add
      IL_012f:  stloc.3
      IL_0130:  ldloc.3
      IL_0131:  ldloc.2
      IL_0132:  blt.un.s   IL_010d

      IL_0134:  ret
    } 

    .method public static void  constFinish(native int start) cil managed
    {
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_004f:  brfalse.s  IL_0094

      IL_0051:  ldc.i4.1
      IL_0052:  stloc.1
      IL_0053:  ldc.i8     0x0
      IL_005c:  conv.i
      IL_005d:  stloc.2
      IL_005e:  ldarg.0
      IL_005f:  stloc.3
      IL_0060:  br.s       IL_0090

      IL_0062:  ldloc.3
      IL_0063:  call       void assembly::set_c(native int)
      IL_0068:  ldloc.3
      IL_0069:  ldc.i8     0xffffffffffffffff
      IL_0072:  conv.i
      IL_0073:  add
      IL_0074:  stloc.3
      IL_0075:  ldloc.2
      IL_0076:  ldc.i8     0x1
      IL_007f:  conv.i
      IL_0080:  add
      IL_0081:  stloc.2
      IL_0082:  ldloc.2
      IL_0083:  ldc.i8     0x0
      IL_008c:  conv.i
      IL_008d:  cgt.un
      IL_008f:  stloc.1
      IL_0090:  ldloc.1
      IL_0091:  brtrue.s   IL_0062

      IL_0093:  ret

      IL_0094:  ldarg.0
      IL_0095:  ldc.i8     0x1
      IL_009e:  conv.i
      IL_009f:  bge.s      IL_00ae

      IL_00a1:  ldc.i8     0x0
      IL_00aa:  conv.i
      IL_00ab:  nop
      IL_00ac:  br.s       IL_00c6

      IL_00ae:  ldarg.0
      IL_00af:  ldc.i8     0x1
      IL_00b8:  conv.i
      IL_00b9:  sub
      IL_00ba:  ldc.i8     0x1
      IL_00c3:  conv.i
      IL_00c4:  add.ovf.un
      IL_00c5:  nop
      IL_00c6:  stloc.2
      IL_00c7:  ldc.i8     0x0
      IL_00d0:  conv.i
      IL_00d1:  stloc.3
      IL_00d2:  ldarg.0
      IL_00d3:  stloc.s    V_4
      IL_00d5:  br.s       IL_00fa

      IL_00d7:  ldloc.s    V_4
      IL_00d9:  call       void assembly::set_c(native int)
      IL_00de:  ldloc.s    V_4
      IL_00e0:  ldc.i8     0xffffffffffffffff
      IL_00e9:  conv.i
      IL_00ea:  add
      IL_00eb:  stloc.s    V_4
      IL_00ed:  ldloc.3
      IL_00ee:  ldc.i8     0x1
      IL_00f7:  conv.i
      IL_00f8:  add
      IL_00f9:  stloc.3
      IL_00fa:  ldloc.3
      IL_00fb:  ldloc.2
      IL_00fc:  blt.un.s   IL_00d7

      IL_00fe:  ret
    } 

    .method public static void  constStart(native int finish) cil managed
    {
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_004f:  brfalse.s  IL_009d

      IL_0051:  ldc.i4.1
      IL_0052:  stloc.1
      IL_0053:  ldc.i8     0x0
      IL_005c:  conv.i
      IL_005d:  stloc.2
      IL_005e:  ldc.i8     0xa
      IL_0067:  conv.i
      IL_0068:  stloc.3
      IL_0069:  br.s       IL_0099

      IL_006b:  ldloc.3
      IL_006c:  call       void assembly::set_c(native int)
      IL_0071:  ldloc.3
      IL_0072:  ldc.i8     0xffffffffffffffff
      IL_007b:  conv.i
      IL_007c:  add
      IL_007d:  stloc.3
      IL_007e:  ldloc.2
      IL_007f:  ldc.i8     0x1
      IL_0088:  conv.i
      IL_0089:  add
      IL_008a:  stloc.2
      IL_008b:  ldloc.2
      IL_008c:  ldc.i8     0x0
      IL_0095:  conv.i
      IL_0096:  cgt.un
      IL_0098:  stloc.1
      IL_0099:  ldloc.1
      IL_009a:  brtrue.s   IL_006b

      IL_009c:  ret

      IL_009d:  ldc.i8     0xa
      IL_00a6:  conv.i
      IL_00a7:  ldarg.0
      IL_00a8:  bge.s      IL_00b7

      IL_00aa:  ldc.i8     0x0
      IL_00b3:  conv.i
      IL_00b4:  nop
      IL_00b5:  br.s       IL_00cf

      IL_00b7:  ldc.i8     0xa
      IL_00c0:  conv.i
      IL_00c1:  ldarg.0
      IL_00c2:  sub
      IL_00c3:  ldc.i8     0x1
      IL_00cc:  conv.i
      IL_00cd:  add.ovf.un
      IL_00ce:  nop
      IL_00cf:  stloc.2
      IL_00d0:  ldc.i8     0x0
      IL_00d9:  conv.i
      IL_00da:  stloc.3
      IL_00db:  ldc.i8     0xa
      IL_00e4:  conv.i
      IL_00e5:  stloc.s    V_4
      IL_00e7:  br.s       IL_010c

      IL_00e9:  ldloc.s    V_4
      IL_00eb:  call       void assembly::set_c(native int)
      IL_00f0:  ldloc.s    V_4
      IL_00f2:  ldc.i8     0xffffffffffffffff
      IL_00fb:  conv.i
      IL_00fc:  add
      IL_00fd:  stloc.s    V_4
      IL_00ff:  ldloc.3
      IL_0100:  ldc.i8     0x1
      IL_0109:  conv.i
      IL_010a:  add
      IL_010b:  stloc.3
      IL_010c:  ldloc.3
      IL_010d:  ldloc.2
      IL_010e:  blt.un.s   IL_00e9

      IL_0110:  ret
    } 

    .method public static void  annotatedStart(native int start,
                                               native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0011

      IL_0004:  ldc.i8     0x0
      IL_000d:  conv.i
      IL_000e:  nop
      IL_000f:  br.s       IL_0015

      IL_0011:  ldarg.0
      IL_0012:  ldarg.1
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0xffffffffffffffff
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.0
      IL_0083:  ldarg.1
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.0
      IL_0094:  ldarg.1
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0xffffffffffffffff
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

    .method public static void  annotatedFinish(native int start,
                                                native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0011

      IL_0004:  ldc.i8     0x0
      IL_000d:  conv.i
      IL_000e:  nop
      IL_000f:  br.s       IL_0015

      IL_0011:  ldarg.0
      IL_0012:  ldarg.1
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0xffffffffffffffff
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.0
      IL_0083:  ldarg.1
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.0
      IL_0094:  ldarg.1
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0xffffffffffffffff
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

    .method public static void  inferredStartAndFinish(native int start,
                                                       native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  bge.s      IL_0011

      IL_0004:  ldc.i8     0x0
      IL_000d:  conv.i
      IL_000e:  nop
      IL_000f:  br.s       IL_0015

      IL_0011:  ldarg.0
      IL_0012:  ldarg.1
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0xffffffffffffffff
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.0
      IL_0083:  ldarg.1
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.0
      IL_0094:  ldarg.1
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0xffffffffffffffff
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Up
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
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

    .method public static void  constNonEmpty() cil managed
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

    .method public static void  constFinish(native int start) cil managed
    {
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_004f:  brfalse.s  IL_0094

      IL_0051:  ldc.i4.1
      IL_0052:  stloc.1
      IL_0053:  ldc.i8     0x0
      IL_005c:  conv.i
      IL_005d:  stloc.2
      IL_005e:  ldarg.0
      IL_005f:  stloc.3
      IL_0060:  br.s       IL_0090

      IL_0062:  ldloc.3
      IL_0063:  call       void assembly::set_c(native int)
      IL_0068:  ldloc.3
      IL_0069:  ldc.i8     0x1
      IL_0072:  conv.i
      IL_0073:  add
      IL_0074:  stloc.3
      IL_0075:  ldloc.2
      IL_0076:  ldc.i8     0x1
      IL_007f:  conv.i
      IL_0080:  add
      IL_0081:  stloc.2
      IL_0082:  ldloc.2
      IL_0083:  ldc.i8     0x0
      IL_008c:  conv.i
      IL_008d:  cgt.un
      IL_008f:  stloc.1
      IL_0090:  ldloc.1
      IL_0091:  brtrue.s   IL_0062

      IL_0093:  ret

      IL_0094:  ldc.i8     0xa
      IL_009d:  conv.i
      IL_009e:  ldarg.0
      IL_009f:  bge.s      IL_00ae

      IL_00a1:  ldc.i8     0x0
      IL_00aa:  conv.i
      IL_00ab:  nop
      IL_00ac:  br.s       IL_00c6

      IL_00ae:  ldc.i8     0xa
      IL_00b7:  conv.i
      IL_00b8:  ldarg.0
      IL_00b9:  sub
      IL_00ba:  ldc.i8     0x1
      IL_00c3:  conv.i
      IL_00c4:  add.ovf.un
      IL_00c5:  nop
      IL_00c6:  stloc.2
      IL_00c7:  ldc.i8     0x0
      IL_00d0:  conv.i
      IL_00d1:  stloc.3
      IL_00d2:  ldarg.0
      IL_00d3:  stloc.s    V_4
      IL_00d5:  br.s       IL_00fa

      IL_00d7:  ldloc.s    V_4
      IL_00d9:  call       void assembly::set_c(native int)
      IL_00de:  ldloc.s    V_4
      IL_00e0:  ldc.i8     0x1
      IL_00e9:  conv.i
      IL_00ea:  add
      IL_00eb:  stloc.s    V_4
      IL_00ed:  ldloc.3
      IL_00ee:  ldc.i8     0x1
      IL_00f7:  conv.i
      IL_00f8:  add
      IL_00f9:  stloc.3
      IL_00fa:  ldloc.3
      IL_00fb:  ldloc.2
      IL_00fc:  blt.un.s   IL_00d7

      IL_00fe:  ret
    } 

    .method public static void  constStart(native int finish) cil managed
    {
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_004f:  brfalse.s  IL_009d

      IL_0051:  ldc.i4.1
      IL_0052:  stloc.1
      IL_0053:  ldc.i8     0x0
      IL_005c:  conv.i
      IL_005d:  stloc.2
      IL_005e:  ldc.i8     0x1
      IL_0067:  conv.i
      IL_0068:  stloc.3
      IL_0069:  br.s       IL_0099

      IL_006b:  ldloc.3
      IL_006c:  call       void assembly::set_c(native int)
      IL_0071:  ldloc.3
      IL_0072:  ldc.i8     0x1
      IL_007b:  conv.i
      IL_007c:  add
      IL_007d:  stloc.3
      IL_007e:  ldloc.2
      IL_007f:  ldc.i8     0x1
      IL_0088:  conv.i
      IL_0089:  add
      IL_008a:  stloc.2
      IL_008b:  ldloc.2
      IL_008c:  ldc.i8     0x0
      IL_0095:  conv.i
      IL_0096:  cgt.un
      IL_0098:  stloc.1
      IL_0099:  ldloc.1
      IL_009a:  brtrue.s   IL_006b

      IL_009c:  ret

      IL_009d:  ldarg.0
      IL_009e:  ldc.i8     0x1
      IL_00a7:  conv.i
      IL_00a8:  bge.s      IL_00b7

      IL_00aa:  ldc.i8     0x0
      IL_00b3:  conv.i
      IL_00b4:  nop
      IL_00b5:  br.s       IL_00cf

      IL_00b7:  ldarg.0
      IL_00b8:  ldc.i8     0x1
      IL_00c1:  conv.i
      IL_00c2:  sub
      IL_00c3:  ldc.i8     0x1
      IL_00cc:  conv.i
      IL_00cd:  add.ovf.un
      IL_00ce:  nop
      IL_00cf:  stloc.2
      IL_00d0:  ldc.i8     0x0
      IL_00d9:  conv.i
      IL_00da:  stloc.3
      IL_00db:  ldc.i8     0x1
      IL_00e4:  conv.i
      IL_00e5:  stloc.s    V_4
      IL_00e7:  br.s       IL_010c

      IL_00e9:  ldloc.s    V_4
      IL_00eb:  call       void assembly::set_c(native int)
      IL_00f0:  ldloc.s    V_4
      IL_00f2:  ldc.i8     0x1
      IL_00fb:  conv.i
      IL_00fc:  add
      IL_00fd:  stloc.s    V_4
      IL_00ff:  ldloc.3
      IL_0100:  ldc.i8     0x1
      IL_0109:  conv.i
      IL_010a:  add
      IL_010b:  stloc.3
      IL_010c:  ldloc.3
      IL_010d:  ldloc.2
      IL_010e:  blt.un.s   IL_00e9

      IL_0110:  ret
    } 

    .method public static void  annotatedStart(native int start,
                                               native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0x1
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.1
      IL_0083:  ldarg.0
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.1
      IL_0094:  ldarg.0
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0x1
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

    .method public static void  annotatedFinish(native int start,
                                                native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0x1
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.1
      IL_0083:  ldarg.0
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.1
      IL_0094:  ldarg.0
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0x1
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

    .method public static void  inferredStartAndFinish(native int start,
                                                       native int finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (native int V_0,
               bool V_1,
               native int V_2,
               native int V_3,
               native int V_4)
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
      IL_003d:  brfalse.s  IL_0082

      IL_003f:  ldc.i4.1
      IL_0040:  stloc.1
      IL_0041:  ldc.i8     0x0
      IL_004a:  conv.i
      IL_004b:  stloc.2
      IL_004c:  ldarg.0
      IL_004d:  stloc.3
      IL_004e:  br.s       IL_007e

      IL_0050:  ldloc.3
      IL_0051:  call       void assembly::set_c(native int)
      IL_0056:  ldloc.3
      IL_0057:  ldc.i8     0x1
      IL_0060:  conv.i
      IL_0061:  add
      IL_0062:  stloc.3
      IL_0063:  ldloc.2
      IL_0064:  ldc.i8     0x1
      IL_006d:  conv.i
      IL_006e:  add
      IL_006f:  stloc.2
      IL_0070:  ldloc.2
      IL_0071:  ldc.i8     0x0
      IL_007a:  conv.i
      IL_007b:  cgt.un
      IL_007d:  stloc.1
      IL_007e:  ldloc.1
      IL_007f:  brtrue.s   IL_0050

      IL_0081:  ret

      IL_0082:  ldarg.1
      IL_0083:  ldarg.0
      IL_0084:  bge.s      IL_0093

      IL_0086:  ldc.i8     0x0
      IL_008f:  conv.i
      IL_0090:  nop
      IL_0091:  br.s       IL_00a2

      IL_0093:  ldarg.1
      IL_0094:  ldarg.0
      IL_0095:  sub
      IL_0096:  ldc.i8     0x1
      IL_009f:  conv.i
      IL_00a0:  add.ovf.un
      IL_00a1:  nop
      IL_00a2:  stloc.2
      IL_00a3:  ldc.i8     0x0
      IL_00ac:  conv.i
      IL_00ad:  stloc.3
      IL_00ae:  ldarg.0
      IL_00af:  stloc.s    V_4
      IL_00b1:  br.s       IL_00d6

      IL_00b3:  ldloc.s    V_4
      IL_00b5:  call       void assembly::set_c(native int)
      IL_00ba:  ldloc.s    V_4
      IL_00bc:  ldc.i8     0x1
      IL_00c5:  conv.i
      IL_00c6:  add
      IL_00c7:  stloc.s    V_4
      IL_00c9:  ldloc.3
      IL_00ca:  ldc.i8     0x1
      IL_00d3:  conv.i
      IL_00d4:  add
      IL_00d5:  stloc.3
      IL_00d6:  ldloc.3
      IL_00d7:  ldloc.2
      IL_00d8:  blt.un.s   IL_00b3

      IL_00da:  ret
    } 

  } 

  .field static assembly native int c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static native int get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     native int assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(native int 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     native int assembly::c@1
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
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stsfld     native int assembly::c@1
    IL_000f:  ret
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






