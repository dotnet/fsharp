




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
  .class auto ansi serializable sealed nested assembly beforefieldinit 'impl@21-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/'impl@21-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
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
      IL_0001:  ldarg.2
      IL_0002:  tail.
      IL_0004:  call       uint64 Tailcall_last_expression_parens::'impl@21-2'(uint64,
                                                                               uint64)
      IL_0009:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/'impl@21-3'::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/'impl@21-3' Tailcall_last_expression_parens/'impl@21-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'impl@29-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<uint64,uint64,uint64>
  {
    .field static assembly initonly class Tailcall_last_expression_parens/'impl@29-5' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
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
      IL_0001:  ldarg.2
      IL_0002:  call       uint64 Tailcall_last_expression_parens::'impl@29-4'(uint64,
                                                                               uint64)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Tailcall_last_expression_parens/'impl@29-5'::.ctor()
      IL_0005:  stsfld     class Tailcall_last_expression_parens/'impl@29-5' Tailcall_last_expression_parens/'impl@29-5'::@_instance
      IL_000a:  ret
    } 

  } 

  .method assembly static uint64  impl@5(uint64 x,
                                         uint64 acc) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldarg.1
    IL_0004:  ret

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.1
    IL_0007:  conv.i8
    IL_0008:  sub
    IL_0009:  ldarg.0
    IL_000a:  ldarg.1
    IL_000b:  mul
    IL_000c:  starg.s    acc
    IL_000e:  starg.s    x
    IL_0010:  br.s       IL_0000
  } 

  .method public static uint64  factorialOk1(uint64 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  call       uint64 Tailcall_last_expression_parens::impl@5(uint64,
                                                                        uint64)
    IL_0008:  ret
  } 

  .method assembly static uint64  'impl@13-1'(uint64 x,
                                              uint64 acc) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldarg.1
    IL_0004:  ret

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.1
    IL_0007:  conv.i8
    IL_0008:  sub
    IL_0009:  ldarg.0
    IL_000a:  ldarg.1
    IL_000b:  mul
    IL_000c:  starg.s    acc
    IL_000e:  starg.s    x
    IL_0010:  br.s       IL_0000
  } 

  .method public static uint64  factorialOk2(uint64 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  call       uint64 Tailcall_last_expression_parens::'impl@13-1'(uint64,
                                                                             uint64)
    IL_0008:  ret
  } 

  .method assembly static uint64  'impl@21-2'(uint64 x,
                                              uint64 acc) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/'impl@21-3' Tailcall_last_expression_parens/'impl@21-3'::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  ldarg.1
    IL_000a:  ret

    IL_000b:  ldloc.0
    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  sub
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>>::Invoke(!0)
    IL_0015:  ldarg.0
    IL_0016:  ldarg.1
    IL_0017:  mul
    IL_0018:  tail.
    IL_001a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::Invoke(!0)
    IL_001f:  ret
  } 

  .method public static uint64  factorialNotOk1(uint64 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  tail.
    IL_0005:  call       uint64 Tailcall_last_expression_parens::'impl@21-2'(uint64,
                                                                             uint64)
    IL_000a:  ret
  } 

  .method assembly static uint64  'impl@29-4'(uint64 x,
                                              uint64 acc) cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>> V_0)
    IL_0000:  ldsfld     class Tailcall_last_expression_parens/'impl@29-5' Tailcall_last_expression_parens/'impl@29-5'::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  ldarg.1
    IL_000a:  ret

    IL_000b:  ldloc.0
    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  sub
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>>::Invoke(!0)
    IL_0015:  ldarg.0
    IL_0016:  ldarg.1
    IL_0017:  mul
    IL_0018:  tail.
    IL_001a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<uint64,uint64>::Invoke(!0)
    IL_001f:  ret
  } 

  .method public static uint64  factorialNotOk2(uint64 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  call       uint64 Tailcall_last_expression_parens::'impl@29-4'(uint64,
                                                                             uint64)
    IL_0008:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Tailcall_last_expression_parens$fsx
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






