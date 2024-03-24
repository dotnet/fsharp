




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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Array_of_function_no_alloc
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit runAll@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[],int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>
  {
    .field static assembly initonly class Array_of_function_no_alloc/runAll@9 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[],int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[] source,
                   int32 index) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  ldelem     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Array_of_function_no_alloc/runAll@9::.ctor()
      IL_0005:  stsfld     class Array_of_function_no_alloc/runAll@9 Array_of_function_no_alloc/runAll@9::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static int32  exampleFunc(int32 v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  add
    IL_0003:  ret
  } 

  .method public static int32  runAll(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[] fArr,
                                      int32 x) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_001a

    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class Array_of_function_no_alloc/runAll@9 Array_of_function_no_alloc/runAll@9::@_instance
    IL_000c:  ldarg.0
    IL_000d:  ldloc.1
    IL_000e:  ldarg.1
    IL_000f:  call       !!1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[],int32>::InvokeFast<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>>>,
                                                                                                                                                                                        !0,
                                                                                                                                                                                        !1,
                                                                                                                                                                                        !!0)
    IL_0014:  add
    IL_0015:  stloc.0
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  ldarg.0
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_0006

    IL_0020:  ldloc.0
    IL_0021:  ret
  } 

  .method public static int32  runAllNoAlloc(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>[] fArr,
                                             int32 x) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_001c

    IL_0006:  ldarg.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
    IL_000d:  stloc.2
    IL_000e:  ldloc.0
    IL_000f:  ldloc.2
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  ldarg.0
    IL_001e:  ldlen
    IL_001f:  conv.i4
    IL_0020:  blt.s      IL_0006

    IL_0022:  ldloc.0
    IL_0023:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Array_of_function_no_alloc$fsx
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






