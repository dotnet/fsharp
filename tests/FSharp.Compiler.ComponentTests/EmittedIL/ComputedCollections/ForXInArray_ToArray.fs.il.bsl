




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
  .method public static int32[]  f1(int32[] 'array') cil managed
  {
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_001a

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.0
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldloc.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  ldloc.0
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000e

    IL_0020:  ldloc.0
    IL_0021:  ret
  } 

  .method public static !!a[]  f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f,
                                     int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (!!a[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     !!a
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_0024

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.1
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldarg.0
    IL_0015:  ldloc.2
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a>::Invoke(!0)
    IL_001b:  stelem     !!a
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  ldlen
    IL_0027:  conv.i4
    IL_0028:  blt.s      IL_000e

    IL_002a:  ldloc.0
    IL_002b:  ret
  } 

  .method public static int32[]  f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_0022

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.1
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldarg.0
    IL_0015:  ldnull
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001b:  pop
    IL_001c:  ldloc.2
    IL_001d:  stelem.i4
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  ldlen
    IL_0025:  conv.i4
    IL_0026:  blt.s      IL_000e

    IL_0028:  ldloc.0
    IL_0029:  ret
  } 

  .method public static int32[]  f4(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_002a

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldarg.0
    IL_0015:  ldnull
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001b:  pop
    IL_001c:  ldarg.1
    IL_001d:  ldnull
    IL_001e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0023:  pop
    IL_0024:  ldloc.2
    IL_0025:  stelem.i4
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  blt.s      IL_000e

    IL_0030:  ldloc.0
    IL_0031:  ret
  } 

  .method public static int32[]  f5(int32[] 'array') cil managed
  {
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_001a

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.0
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldloc.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  ldloc.0
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000e

    IL_0020:  ldloc.0
    IL_0021:  ret
  } 

  .method public static int32[]  f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_0022

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.1
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldarg.0
    IL_0015:  ldnull
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001b:  pop
    IL_001c:  ldloc.2
    IL_001d:  stelem.i4
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  ldlen
    IL_0025:  conv.i4
    IL_0026:  blt.s      IL_000e

    IL_0028:  ldloc.0
    IL_0029:  ret
  } 

  .method public static int32[]  f7(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.1
    IL_000c:  br.s       IL_002a

    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldloc.1
    IL_0012:  ldelem.i4
    IL_0013:  stloc.2
    IL_0014:  ldarg.0
    IL_0015:  ldnull
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001b:  pop
    IL_001c:  ldarg.1
    IL_001d:  ldnull
    IL_001e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0023:  pop
    IL_0024:  ldloc.2
    IL_0025:  stelem.i4
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  blt.s      IL_000e

    IL_0030:  ldloc.0
    IL_0031:  ret
  } 

  .method public static int32[]  f8(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             int32[] V_2,
             int32 V_3,
             int32 V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.1
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  newarr     [runtime]System.Int32
    IL_001a:  stloc.2
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.3
    IL_001d:  br.s       IL_0031

    IL_001f:  ldloc.2
    IL_0020:  ldloc.3
    IL_0021:  ldarg.2
    IL_0022:  ldloc.3
    IL_0023:  ldelem.i4
    IL_0024:  stloc.s    V_4
    IL_0026:  ldloc.s    V_4
    IL_0028:  ldloc.0
    IL_0029:  add
    IL_002a:  ldloc.1
    IL_002b:  add
    IL_002c:  stelem.i4
    IL_002d:  ldloc.3
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.3
    IL_0031:  ldloc.3
    IL_0032:  ldloc.2
    IL_0033:  ldlen
    IL_0034:  conv.i4
    IL_0035:  blt.s      IL_001f

    IL_0037:  ldloc.2
    IL_0038:  ret
  } 

  .method public static int32[]  f9(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32[] V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  newarr     [runtime]System.Int32
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_002d

    IL_001f:  ldloc.1
    IL_0020:  ldloc.2
    IL_0021:  ldarg.2
    IL_0022:  ldloc.2
    IL_0023:  ldelem.i4
    IL_0024:  stloc.3
    IL_0025:  ldloc.3
    IL_0026:  ldloc.0
    IL_0027:  add
    IL_0028:  stelem.i4
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  ldloc.1
    IL_002f:  ldlen
    IL_0030:  conv.i4
    IL_0031:  blt.s      IL_001f

    IL_0033:  ldloc.1
    IL_0034:  ret
  } 

  .method public static int32[]  f10(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
                                     int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  newarr     [runtime]System.Int32
    IL_001a:  stloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.1
    IL_001d:  br.s       IL_002b

    IL_001f:  ldloc.0
    IL_0020:  ldloc.1
    IL_0021:  ldarg.2
    IL_0022:  ldloc.1
    IL_0023:  ldelem.i4
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  stelem.i4
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldloc.0
    IL_002d:  ldlen
    IL_002e:  conv.i4
    IL_002f:  blt.s      IL_001f

    IL_0031:  ldloc.0
    IL_0032:  ret
  } 

  .method public static int32[]  f11(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
                                     int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32[] V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.0
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  newarr     [runtime]System.Int32
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.0
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_002d

    IL_001f:  ldloc.1
    IL_0020:  ldloc.2
    IL_0021:  ldarg.2
    IL_0022:  ldloc.2
    IL_0023:  ldelem.i4
    IL_0024:  stloc.3
    IL_0025:  ldloc.3
    IL_0026:  ldloc.0
    IL_0027:  add
    IL_0028:  stelem.i4
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  ldloc.1
    IL_002f:  ldlen
    IL_0030:  conv.i4
    IL_0031:  blt.s      IL_001f

    IL_0033:  ldloc.1
    IL_0034:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






