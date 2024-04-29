




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
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0019

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.0
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  stelem.i4
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  ldlen
    IL_001c:  conv.i4
    IL_001d:  blt.s      IL_000d

    IL_001f:  ldloc.0
    IL_0020:  ret
  } 

  .method public static !!a[]  f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f,
                                     int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (!!a[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     !!a
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0023

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.1
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldarg.0
    IL_0014:  ldloc.2
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a>::Invoke(!0)
    IL_001a:  stelem     !!a
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldloc.0
    IL_0025:  ldlen
    IL_0026:  conv.i4
    IL_0027:  blt.s      IL_000d

    IL_0029:  ldloc.0
    IL_002a:  ret
  } 

  .method public static int32[]  f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0021

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.1
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldarg.0
    IL_0014:  ldnull
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001a:  pop
    IL_001b:  ldloc.2
    IL_001c:  stelem.i4
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldloc.0
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  blt.s      IL_000d

    IL_0027:  ldloc.0
    IL_0028:  ret
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
    IL_0000:  ldarg.2
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0029

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.2
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldarg.0
    IL_0014:  ldnull
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001a:  pop
    IL_001b:  ldarg.1
    IL_001c:  ldnull
    IL_001d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0022:  pop
    IL_0023:  ldloc.2
    IL_0024:  stelem.i4
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldloc.0
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  blt.s      IL_000d

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f5(int32[] 'array') cil managed
  {
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0019

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.0
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  stelem.i4
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  ldlen
    IL_001c:  conv.i4
    IL_001d:  blt.s      IL_000d

    IL_001f:  ldloc.0
    IL_0020:  ret
  } 

  .method public static int32[]  f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                    int32[] 'array') cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0021

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.1
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldarg.0
    IL_0014:  ldnull
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001a:  pop
    IL_001b:  ldloc.2
    IL_001c:  stelem.i4
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldloc.0
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  blt.s      IL_000d

    IL_0027:  ldloc.0
    IL_0028:  ret
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
    IL_0000:  ldarg.2
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0029

    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  ldarg.2
    IL_0010:  ldloc.1
    IL_0011:  ldelem.i4
    IL_0012:  stloc.2
    IL_0013:  ldarg.0
    IL_0014:  ldnull
    IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001a:  pop
    IL_001b:  ldarg.1
    IL_001c:  ldnull
    IL_001d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0022:  pop
    IL_0023:  ldloc.2
    IL_0024:  stelem.i4
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldloc.0
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  blt.s      IL_000d

    IL_002f:  ldloc.0
    IL_0030:  ret
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
    IL_0011:  ldarg.2
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.2
    IL_001a:  ldc.i4.0
    IL_001b:  stloc.3
    IL_001c:  br.s       IL_0030

    IL_001e:  ldloc.2
    IL_001f:  ldloc.3
    IL_0020:  ldarg.2
    IL_0021:  ldloc.3
    IL_0022:  ldelem.i4
    IL_0023:  stloc.s    V_4
    IL_0025:  ldloc.s    V_4
    IL_0027:  ldloc.0
    IL_0028:  add
    IL_0029:  ldloc.1
    IL_002a:  add
    IL_002b:  stelem.i4
    IL_002c:  ldloc.3
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  stloc.3
    IL_0030:  ldloc.3
    IL_0031:  ldloc.2
    IL_0032:  ldlen
    IL_0033:  conv.i4
    IL_0034:  blt.s      IL_001e

    IL_0036:  ldloc.2
    IL_0037:  ret
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
    IL_0011:  ldarg.2
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_002c

    IL_001e:  ldloc.1
    IL_001f:  ldloc.2
    IL_0020:  ldarg.2
    IL_0021:  ldloc.2
    IL_0022:  ldelem.i4
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  ldloc.0
    IL_0026:  add
    IL_0027:  stelem.i4
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.1
    IL_002e:  ldlen
    IL_002f:  conv.i4
    IL_0030:  blt.s      IL_001e

    IL_0032:  ldloc.1
    IL_0033:  ret
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
    IL_0011:  ldarg.2
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  stloc.1
    IL_001c:  br.s       IL_002a

    IL_001e:  ldloc.0
    IL_001f:  ldloc.1
    IL_0020:  ldarg.2
    IL_0021:  ldloc.1
    IL_0022:  ldelem.i4
    IL_0023:  stloc.2
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
    IL_002e:  blt.s      IL_001e

    IL_0030:  ldloc.0
    IL_0031:  ret
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
    IL_0011:  ldarg.2
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_002c

    IL_001e:  ldloc.1
    IL_001f:  ldloc.2
    IL_0020:  ldarg.2
    IL_0021:  ldloc.2
    IL_0022:  ldelem.i4
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  ldloc.0
    IL_0026:  add
    IL_0027:  stelem.i4
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.1
    IL_002e:  ldlen
    IL_002f:  conv.i4
    IL_0030:  blt.s      IL_001e

    IL_0032:  ldloc.1
    IL_0033:  ret
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






