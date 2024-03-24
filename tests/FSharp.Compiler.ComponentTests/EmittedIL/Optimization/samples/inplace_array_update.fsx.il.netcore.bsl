




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Inplace_array_update
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public Foo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static !!a[]  bar<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>> f,
                                        !!a[] a,
                                        !!a[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  tail.
      IL_0005:  call       !!2[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Map2<!!0,!!0,!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>,
                                                                                                          !!0[],
                                                                                                          !!1[])
      IL_000a:  ret
    } 

    .method public static void  barInPlace<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>> f,
                                              !!a[] a,
                                              !!a[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0023

      IL_0004:  ldarg.2
      IL_0005:  ldloc.0
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  ldloc.0
      IL_0009:  ldelem     !!a
      IL_000e:  ldarg.2
      IL_000f:  ldloc.0
      IL_0010:  ldelem     !!a
      IL_0015:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!0>::InvokeFast<!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                               !0,
                                                                                                               !1)
      IL_001a:  stelem     !!a
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.1
      IL_0021:  add
      IL_0022:  stloc.0
      IL_0023:  ldloc.0
      IL_0024:  ldarg.2
      IL_0025:  ldlen
      IL_0026:  conv.i4
      IL_0027:  blt.s      IL_0004

      IL_0029:  ret
    } 

    .method public static void  barInPlaceInline<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>> f,
                                                    !!a[] a,
                                                    !!a[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0023

      IL_0004:  ldarg.2
      IL_0005:  ldloc.0
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  ldloc.0
      IL_0009:  ldelem     !!a
      IL_000e:  ldarg.2
      IL_000f:  ldloc.0
      IL_0010:  ldelem     !!a
      IL_0015:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!0>::InvokeFast<!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                               !0,
                                                                                                               !1)
      IL_001a:  stelem     !!a
      IL_001f:  ldloc.0
      IL_0020:  ldc.i4.1
      IL_0021:  add
      IL_0022:  stloc.0
      IL_0023:  ldloc.0
      IL_0024:  ldarg.2
      IL_0025:  ldlen
      IL_0026:  conv.i4
      IL_0027:  blt.s      IL_0004

      IL_0029:  ret
    } 

    .method public static void  barInPlaceOptClosure<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>> f,
                                                        !!a[] a,
                                                        !!a[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a> V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!0,!1,!2> class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a>::Adapt(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!2>>)
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.0
      IL_0008:  stloc.1
      IL_0009:  br.s       IL_002a

      IL_000b:  ldarg.2
      IL_000c:  ldloc.1
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  ldloc.1
      IL_0010:  ldelem     !!a
      IL_0015:  ldarg.2
      IL_0016:  ldloc.1
      IL_0017:  ldelem     !!a
      IL_001c:  callvirt   instance !2 class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a>::Invoke(!0,
                                                                                                                                    !1)
      IL_0021:  stelem     !!a
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  add
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  ldarg.2
      IL_002c:  ldlen
      IL_002d:  conv.i4
      IL_002e:  blt.s      IL_000b

      IL_0030:  ret
    } 

    .method public static void  barInPlaceOptClosureInline<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>> f,
                                                              !!a[] a,
                                                              !!a[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a> V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!0,!1,!2> class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a>::Adapt(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!2>>)
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.0
      IL_0008:  stloc.1
      IL_0009:  br.s       IL_002a

      IL_000b:  ldarg.2
      IL_000c:  ldloc.1
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  ldloc.1
      IL_0010:  ldelem     !!a
      IL_0015:  ldarg.2
      IL_0016:  ldloc.1
      IL_0017:  ldelem     !!a
      IL_001c:  callvirt   instance !2 class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!a,!!a,!!a>::Invoke(!0,
                                                                                                                                    !1)
      IL_0021:  stelem     !!a
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  add
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  ldarg.2
      IL_002c:  ldlen
      IL_002d:  conv.i4
      IL_002e:  blt.s      IL_000b

      IL_0030:  ret
    } 

    .method public static void  barFunInside(float32[] a,
                                             float32[] b) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               float32 V_1,
               float32 V_2)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_002e

      IL_0004:  ldarg.1
      IL_0005:  ldloc.0
      IL_0006:  ldarg.0
      IL_0007:  ldloc.0
      IL_0008:  ldelem     [runtime]System.Single
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  ldloc.0
      IL_0010:  ldelem     [runtime]System.Single
      IL_0015:  stloc.2
      IL_0016:  ldloc.1
      IL_0017:  ldc.r4     0.69999999
      IL_001c:  mul
      IL_001d:  ldloc.2
      IL_001e:  ldc.r4     0.30000001
      IL_0023:  mul
      IL_0024:  add
      IL_0025:  stelem     [runtime]System.Single
      IL_002a:  ldloc.0
      IL_002b:  ldc.i4.1
      IL_002c:  add
      IL_002d:  stloc.0
      IL_002e:  ldloc.0
      IL_002f:  ldarg.1
      IL_0030:  ldlen
      IL_0031:  conv.i4
      IL_0032:  blt.s      IL_0004

      IL_0034:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Inplace_array_update$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void [runtime]System.Console::WriteLine()
    IL_0005:  ret
  } 

} 





