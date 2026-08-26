




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname seq1@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<int32,int32>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Tuple`2<int32,int32> current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(int32 pc, class [runtime]System.Tuple`2<int32,int32> current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/seq1@9::pc
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_000e:  ldarg.0
      IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<int32,int32>>::.ctor()
      IL_0014:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>>& next) cil managed
    {
      
      .maxstack  7
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/seq1@9::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_003b

      IL_001e:  nop
      IL_001f:  br.s       IL_0051

      IL_0021:  nop
      IL_0022:  br.s       IL_0058

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.1
      IL_0027:  stfld      int32 assembly/seq1@9::pc
      IL_002c:  ldarg.0
      IL_002d:  ldc.i4.1
      IL_002e:  ldc.i4.1
      IL_002f:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_0034:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_0039:  ldc.i4.1
      IL_003a:  ret

      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.2
      IL_003d:  stfld      int32 assembly/seq1@9::pc
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.2
      IL_0044:  ldc.i4.2
      IL_0045:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_004a:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_004f:  ldc.i4.1
      IL_0050:  ret

      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.3
      IL_0053:  stfld      int32 assembly/seq1@9::pc
      IL_0058:  ldarg.0
      IL_0059:  ldnull
      IL_005a:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_005f:  ldc.i4.0
      IL_0060:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.3
      IL_0002:  stfld      int32 assembly/seq1@9::pc
      IL_0007:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/seq1@9::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.0
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,int32> get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<int32,int32>> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ldnull
      IL_0002:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                          class [runtime]System.Tuple`2<int32,int32>)
      IL_0007:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit clo@42
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,int32,int32>
  {
    .field static assembly initonly class assembly/clo@42 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 i, int32 j) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  add
      IL_0003:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/clo@42::.ctor()
      IL_0005:  stsfld     class assembly/clo@42 assembly/clo@42::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> alist@5
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] array@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<int32> aseq@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> list1@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> seq1@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<int32,int32>[] array1@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...] a3@11
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...,0...] array3D@12
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...,0...,0...] array4D@13
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] a1@25
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] a2@26
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 arg_0@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 arg_1@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 arg_2@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 arg_3@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_0@34-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_1@34-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_2@34-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_0@38-2'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_1@38-2'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_2@38-2'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'arg_3@38-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...] arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_alist() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::alist@5
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_array() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::array@6
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<int32> get_aseq() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::aseq@7
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> get_list1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::list1@8
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> get_seq1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::seq1@9
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<int32,int32>[] get_array1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::array1@10
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...] get_a3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...] assembly::a3@11
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...] get_array3D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...] assembly::array3D@12
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...,0...] get_array4D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...,0...] assembly::array4D@13
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_a1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::a1@25
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_a2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::a2@26
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_arg_0@30() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::arg_0@30
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_arg_1@30() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::arg_1@30
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_arg_2@30() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::arg_2@30
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_arg_3@30() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::arg_3@30
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_0@34-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_0@34-1'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_1@34-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_1@34-1'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_2@34-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_2@34-1'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_0@38-2'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_0@38-2'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_1@38-2'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_1@38-2'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_2@38-2'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_2@38-2'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_arg_3@38-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::'arg_3@38-1'
    IL_0005:  ret
  } 

  .method assembly specialname static int32[0...,0...] get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...] assembly::arg@1
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  12
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0019

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_000f:  nop
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  conv.i8
    IL_001d:  blt.un.s   IL_0007

    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0026:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::alist@5
    IL_002b:  ldc.i4.3
    IL_002c:  newarr     [runtime]System.Int32
    IL_0031:  dup
    IL_0032:  ldc.i4.0
    IL_0033:  ldc.i4.1
    IL_0034:  stelem.i4
    IL_0035:  dup
    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.2
    IL_0038:  stelem.i4
    IL_0039:  dup
    IL_003a:  ldc.i4.2
    IL_003b:  ldc.i4.3
    IL_003c:  stelem.i4
    IL_003d:  stsfld     int32[] assembly::array@6
    IL_0042:  ldc.i4.1
    IL_0043:  ldc.i4.1
    IL_0044:  ldc.i4.s   10
    IL_0046:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_004b:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0050:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::aseq@7
    IL_0055:  ldc.i4.1
    IL_0056:  ldc.i4.1
    IL_0057:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_005c:  ldc.i4.2
    IL_005d:  ldc.i4.2
    IL_005e:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0072:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::list1@8
    IL_0077:  ldc.i4.0
    IL_0078:  ldnull
    IL_0079:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                        class [runtime]System.Tuple`2<int32,int32>)
    IL_007e:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::seq1@9
    IL_0083:  ldc.i4.2
    IL_0084:  newarr     class [runtime]System.Tuple`2<int32,int32>
    IL_0089:  dup
    IL_008a:  ldc.i4.0
    IL_008b:  ldc.i4.1
    IL_008c:  ldc.i4.1
    IL_008d:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_0092:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_0097:  dup
    IL_0098:  ldc.i4.1
    IL_0099:  ldc.i4.2
    IL_009a:  ldc.i4.2
    IL_009b:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00a0:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00a5:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::array1@10
    IL_00aa:  ldc.i4.2
    IL_00ab:  ldc.i4.2
    IL_00ac:  ldc.i4.0
    IL_00ad:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00b2:  stsfld     int32[0...,0...] assembly::a3@11
    IL_00b7:  ldc.i4.3
    IL_00b8:  ldc.i4.3
    IL_00b9:  ldc.i4.3
    IL_00ba:  ldc.i4.0
    IL_00bb:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00c0:  stsfld     int32[0...,0...,0...] assembly::array3D@12
    IL_00c5:  ldc.i4.4
    IL_00c6:  ldc.i4.4
    IL_00c7:  ldc.i4.4
    IL_00c8:  ldc.i4.4
    IL_00c9:  ldc.i4.0
    IL_00ca:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_00cf:  stsfld     int32[0...,0...,0...,0...] assembly::array4D@13
    IL_00d4:  call       int32[] assembly::get_array()
    IL_00d9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_00de:  pop
    IL_00df:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00e9:  pop
    IL_00ea:  call       class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
    IL_00ef:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [runtime]System.Tuple`2<!!0,!!1>[])
    IL_00f4:  pop
    IL_00f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
    IL_00fa:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_00ff:  pop
    IL_0100:  call       class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
    IL_0105:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_010a:  pop
    IL_010b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
    IL_0110:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0115:  stsfld     int32[] assembly::a1@25
    IL_011a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_011f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0124:  stsfld     int32[] assembly::a2@26
    IL_0129:  call       int32[] assembly::get_a2()
    IL_012e:  ldc.i4.0
    IL_012f:  call       int32[] assembly::get_a1()
    IL_0134:  ldc.i4.0
    IL_0135:  ldelem.i4
    IL_0136:  stelem.i4
    IL_0137:  call       int32[0...,0...] assembly::get_a3()
    IL_013c:  ldc.i4.s   0
    IL_013e:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_0143:  stsfld     int32 assembly::arg_0@30
    IL_0148:  call       int32[0...,0...] assembly::get_a3()
    IL_014d:  ldc.i4.s   1
    IL_014f:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_0154:  stsfld     int32 assembly::arg_1@30
    IL_0159:  call       int32[0...,0...] assembly::get_a3()
    IL_015e:  ldc.i4.0
    IL_015f:  callvirt   instance int32 [netstandard]System.Array::GetLowerBound(int32)
    IL_0164:  stsfld     int32 assembly::arg_2@30
    IL_0169:  call       int32[0...,0...] assembly::get_a3()
    IL_016e:  ldc.i4.1
    IL_016f:  callvirt   instance int32 [netstandard]System.Array::GetLowerBound(int32)
    IL_0174:  stsfld     int32 assembly::arg_3@30
    IL_0179:  call       int32[0...,0...] assembly::get_a3()
    IL_017e:  ldc.i4.0
    IL_017f:  ldc.i4.0
    IL_0180:  call       int32[0...,0...] assembly::get_a3()
    IL_0185:  ldc.i4.0
    IL_0186:  ldc.i4.0
    IL_0187:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_018c:  call       instance void int32[0...,0...]::Set(int32,
                                                             int32,
                                                             int32)
    IL_0191:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_0196:  ldc.i4.s   0
    IL_0198:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_019d:  stsfld     int32 assembly::'arg_0@34-1'
    IL_01a2:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01a7:  ldc.i4.s   1
    IL_01a9:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_01ae:  stsfld     int32 assembly::'arg_1@34-1'
    IL_01b3:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01b8:  ldc.i4.s   2
    IL_01ba:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_01bf:  stsfld     int32 assembly::'arg_2@34-1'
    IL_01c4:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01c9:  ldc.i4.0
    IL_01ca:  ldc.i4.0
    IL_01cb:  ldc.i4.0
    IL_01cc:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01d1:  ldc.i4.0
    IL_01d2:  ldc.i4.0
    IL_01d3:  ldc.i4.0
    IL_01d4:  call       instance int32 int32[0...,0...,0...]::Get(int32,
                                                                   int32,
                                                                   int32)
    IL_01d9:  call       instance void int32[0...,0...,0...]::Set(int32,
                                                                  int32,
                                                                  int32,
                                                                  int32)
    IL_01de:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01e3:  ldc.i4.s   0
    IL_01e5:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_01ea:  stsfld     int32 assembly::'arg_0@38-2'
    IL_01ef:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01f4:  ldc.i4.s   1
    IL_01f6:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_01fb:  stsfld     int32 assembly::'arg_1@38-2'
    IL_0200:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0205:  ldc.i4.s   2
    IL_0207:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_020c:  stsfld     int32 assembly::'arg_2@38-2'
    IL_0211:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0216:  ldc.i4.s   3
    IL_0218:  call       instance int32 [runtime]System.Array::GetLength(int32)
    IL_021d:  stsfld     int32 assembly::'arg_3@38-1'
    IL_0222:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0227:  ldc.i4.0
    IL_0228:  ldc.i4.0
    IL_0229:  ldc.i4.0
    IL_022a:  ldc.i4.0
    IL_022b:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0230:  ldc.i4.0
    IL_0231:  ldc.i4.0
    IL_0232:  ldc.i4.0
    IL_0233:  ldc.i4.0
    IL_0234:  call       instance int32 int32[0...,0...,0...,0...]::Get(int32,
                                                                        int32,
                                                                        int32,
                                                                        int32)
    IL_0239:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_023e:  nop
    IL_023f:  ldc.i4.2
    IL_0240:  ldc.i4.2
    IL_0241:  ldsfld     class assembly/clo@42 assembly/clo@42::@_instance
    IL_0246:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Initialize<int32>(int32,
                                                                                                                   int32,
                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>>)
    IL_024b:  stsfld     int32[0...,0...] assembly::arg@1
    IL_0250:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          alist()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
  } 
  .property int32[] 'array'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_array()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerable`1<int32>
          aseq()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>
          list1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>>
          seq1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
  } 
  .property class [runtime]System.Tuple`2<int32,int32>[]
          array1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
  } 
  .property int32[0...,0...] a3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...] assembly::get_a3()
  } 
  .property int32[0...,0...,0...] array3D()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...,0...] assembly::get_array3D()
  } 
  .property int32[0...,0...,0...,0...] array4D()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...,0...,0...] assembly::get_array4D()
  } 
  .property int32[] a1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_a1()
  } 
  .property int32[] a2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_a2()
  } 
  .property int32 arg_0@30()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_arg_0@30()
  } 
  .property int32 arg_1@30()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_arg_1@30()
  } 
  .property int32 arg_2@30()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_arg_2@30()
  } 
  .property int32 arg_3@30()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_arg_3@30()
  } 
  .property int32 'arg_0@34-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_0@34-1'()
  } 
  .property int32 'arg_1@34-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_1@34-1'()
  } 
  .property int32 'arg_2@34-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_2@34-1'()
  } 
  .property int32 'arg_0@38-2'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_0@38-2'()
  } 
  .property int32 'arg_1@38-2'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_1@38-2'()
  } 
  .property int32 'arg_2@38-2'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_2@38-2'()
  } 
  .property int32 'arg_3@38-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::'get_arg_3@38-1'()
  } 
  .property int32[0...,0...] arg@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...] assembly::get_arg@1()
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





