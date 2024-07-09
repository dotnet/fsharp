




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





.class public abstract auto ansi sealed Tailcall_last_expression_parens
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit impl@6
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/impl@6 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance uint64 
            Invoke(uint64 x,
                   uint64 acc) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brtrue.s   IL_0005

      IL_0003:  ldarg.2
      IL_0004:  ret

      IL_0005:  ldarg.1
      IL_0006:  ldc.i4.1
      IL_0007:  conv.i8
      IL_0008:  sub
      IL_0009:  ldarg.1
      IL_000a:  ldarg.2
      IL_000b:  mul
      IL_000c:  starg.s    acc
      IL_000e:  starg.s    x
      IL_0010:  br.s       IL_0000
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/impl@6::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/impl@6 Tailcall_last_expression_parens/impl@6::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'impl@14-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/'impl@14-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance uint64 
            Invoke(uint64 x,
                   uint64 acc) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brtrue.s   IL_0005

      IL_0003:  ldarg.2
      IL_0004:  ret

      IL_0005:  ldarg.1
      IL_0006:  ldc.i4.1
      IL_0007:  conv.i8
      IL_0008:  sub
      IL_0009:  ldarg.1
      IL_000a:  ldarg.2
      IL_000b:  mul
      IL_000c:  starg.s    acc
      IL_000e:  starg.s    x
      IL_0010:  br.s       IL_0000
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/'impl@14-1'::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/'impl@14-1' Tailcall_last_expression_parens/'impl@14-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'impl@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/'impl@22-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance uint64 
            Invoke(uint64 x,
                   uint64 acc) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brtrue.s   IL_0005

      IL_0003:  ldarg.2
      IL_0004:  ret

      IL_0005:  ldarg.0
      IL_0006:  ldarg.1
      IL_0007:  ldc.i4.1
      IL_0008:  conv.i8
      IL_0009:  sub
      IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>>::Invoke(!0)
      IL_000f:  ldarg.1
      IL_0010:  ldarg.2
      IL_0011:  mul
      IL_0012:  tail.
      IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::Invoke(!0)
      IL_0019:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/'impl@22-2'::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/'impl@22-2' Tailcall_last_expression_parens/'impl@22-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'impl@30-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/'impl@30-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance uint64 
            Invoke(uint64 x,
                   uint64 acc) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  brtrue.s   IL_0005

      IL_0003:  ldarg.2
      IL_0004:  ret

      IL_0005:  ldarg.0
      IL_0006:  ldarg.1
      IL_0007:  ldc.i4.1
      IL_0008:  conv.i8
      IL_0009:  sub
      IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>>::Invoke(!0)
      IL_000f:  ldarg.1
      IL_0010:  ldarg.2
      IL_0011:  mul
      IL_0012:  tail.
      IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::Invoke(!0)
      IL_0019:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/'impl@30-3'::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/'impl@30-3' Tailcall_last_expression_parens/'impl@30-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static uint64  factorialOk1(uint64 x) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/impl@6 Tailcall_last_expression_parens/impl@6::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  conv.i8
    IL_000a:  tail.
    IL_000c:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::InvokeFast<uint64>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                      !0,
                                                                                                                      !1)
    IL_0011:  ret
  } 

  .method public static uint64  factorialOk2(uint64 x) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/'impl@14-1' Tailcall_last_expression_parens/'impl@14-1'::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  conv.i8
    IL_000a:  tail.
    IL_000c:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::InvokeFast<uint64>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                      !0,
                                                                                                                      !1)
    IL_0011:  ret
  } 

  .method public static uint64  factorialNotOk1(uint64 x) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/'impl@22-2' Tailcall_last_expression_parens/'impl@22-2'::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  conv.i8
    IL_000a:  tail.
    IL_000c:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::InvokeFast<uint64>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                      !0,
                                                                                                                      !1)
    IL_0011:  ret
  } 

  .method public static uint64  factorialNotOk2(uint64 x) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/'impl@30-3' Tailcall_last_expression_parens/'impl@30-3'::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  conv.i8
    IL_000a:  tail.
    IL_000c:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::InvokeFast<uint64>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                      !0,
                                                                                                                      !1)
    IL_0011:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Tailcall_last_expression_parens$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Tailcall_last_expression_parens$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void [runtime]System.Console::WriteLine()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Tailcall_last_expression_parens$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void Tailcall_last_expression_parens::staticInitialization@()
    IL_0005:  ret
  } 

} 






