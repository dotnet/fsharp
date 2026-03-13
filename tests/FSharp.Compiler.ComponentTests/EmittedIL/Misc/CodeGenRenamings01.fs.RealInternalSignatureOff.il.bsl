




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
      IL_001c:  br.s       IL_003d

      IL_001e:  nop
      IL_001f:  br.s       IL_0053

      IL_0021:  nop
      IL_0022:  br.s       IL_005a

      IL_0024:  nop
      IL_0025:  br.s       IL_0027

      IL_0027:  ldarg.0
      IL_0028:  ldc.i4.1
      IL_0029:  stfld      int32 assembly/seq1@9::pc
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.1
      IL_0030:  ldc.i4.1
      IL_0031:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_0036:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_003b:  ldc.i4.1
      IL_003c:  ret

      IL_003d:  ldarg.0
      IL_003e:  ldc.i4.2
      IL_003f:  stfld      int32 assembly/seq1@9::pc
      IL_0044:  ldarg.0
      IL_0045:  ldc.i4.2
      IL_0046:  ldc.i4.2
      IL_0047:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_004c:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_0051:  ldc.i4.1
      IL_0052:  ret

      IL_0053:  ldarg.0
      IL_0054:  ldc.i4.3
      IL_0055:  stfld      int32 assembly/seq1@9::pc
      IL_005a:  ldarg.0
      IL_005b:  ldnull
      IL_005c:  stfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_0061:  ldc.i4.0
      IL_0062:  ret
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
      IL_001e:  br.s       IL_0030

      IL_0020:  nop
      IL_0021:  br.s       IL_002e

      IL_0023:  nop
      IL_0024:  br.s       IL_002c

      IL_0026:  nop
      IL_0027:  br.s       IL_0030

      IL_0029:  nop
      IL_002a:  br.s       IL_002c

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret
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

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_alist() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::alist@5
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_array() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::array@6
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<int32> get_aseq() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$assembly>'.$assembly::aseq@7
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> get_list1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::list1@8
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> get_seq1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::seq1@9
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<int32,int32>[] get_array1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<int32,int32>[] '<StartupCode$assembly>'.$assembly::array1@10
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...] get_a3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...] '<StartupCode$assembly>'.$assembly::a3@11
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...] get_array3D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...] '<StartupCode$assembly>'.$assembly::array3D@12
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...,0...] get_array4D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...,0...] '<StartupCode$assembly>'.$assembly::array4D@13
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_a1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::a1@25
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_a2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::a2@26
    IL_0005:  ret
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
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
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
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             int32[] V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> V_3,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> V_4,
             class [runtime]System.Tuple`2<int32,int32>[] V_5,
             int32[0...,0...] V_6,
             int32[0...,0...,0...] V_7,
             int32[0...,0...,0...,0...] V_8,
             int32[] V_9,
             int32[] V_10,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_11,
             uint64 V_12,
             int32 V_13,
             int32 V_14,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_15,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_16,
             int32 V_17,
             class [runtime]System.Tuple`3<int32,int32,int32> V_18,
             class [runtime]System.Tuple`3<int32,int32,int32> V_19,
             int32 V_20,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_21,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_22,
             int32 V_23)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.s    V_12
    IL_0004:  ldc.i4.1
    IL_0005:  stloc.s    V_13
    IL_0007:  br.s       IL_0020

    IL_0009:  ldloca.s   V_11
    IL_000b:  ldloc.s    V_13
    IL_000d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0012:  nop
    IL_0013:  ldloc.s    V_13
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.s    V_13
    IL_0019:  ldloc.s    V_12
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.s    V_12
    IL_0020:  ldloc.s    V_12
    IL_0022:  ldc.i4.s   10
    IL_0024:  conv.i8
    IL_0025:  blt.un.s   IL_0009

    IL_0027:  ldloca.s   V_11
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002e:  dup
    IL_002f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::alist@5
    IL_0034:  stloc.0
    IL_0035:  ldc.i4.3
    IL_0036:  newarr     [runtime]System.Int32
    IL_003b:  dup
    IL_003c:  ldc.i4.0
    IL_003d:  ldc.i4.1
    IL_003e:  stelem.i4
    IL_003f:  dup
    IL_0040:  ldc.i4.1
    IL_0041:  ldc.i4.2
    IL_0042:  stelem.i4
    IL_0043:  dup
    IL_0044:  ldc.i4.2
    IL_0045:  ldc.i4.3
    IL_0046:  stelem.i4
    IL_0047:  dup
    IL_0048:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::array@6
    IL_004d:  stloc.1
    IL_004e:  ldc.i4.1
    IL_004f:  ldc.i4.1
    IL_0050:  ldc.i4.s   10
    IL_0052:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0057:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_005c:  dup
    IL_005d:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$assembly>'.$assembly::aseq@7
    IL_0062:  stloc.2
    IL_0063:  ldc.i4.1
    IL_0064:  ldc.i4.1
    IL_0065:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_006a:  ldc.i4.2
    IL_006b:  ldc.i4.2
    IL_006c:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_0071:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0076:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0080:  dup
    IL_0081:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::list1@8
    IL_0086:  stloc.3
    IL_0087:  ldc.i4.0
    IL_0088:  ldnull
    IL_0089:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                        class [runtime]System.Tuple`2<int32,int32>)
    IL_008e:  dup
    IL_008f:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::seq1@9
    IL_0094:  stloc.s    V_4
    IL_0096:  ldc.i4.2
    IL_0097:  newarr     class [runtime]System.Tuple`2<int32,int32>
    IL_009c:  dup
    IL_009d:  ldc.i4.0
    IL_009e:  ldc.i4.1
    IL_009f:  ldc.i4.1
    IL_00a0:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00a5:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00aa:  dup
    IL_00ab:  ldc.i4.1
    IL_00ac:  ldc.i4.2
    IL_00ad:  ldc.i4.2
    IL_00ae:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00b3:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00b8:  dup
    IL_00b9:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] '<StartupCode$assembly>'.$assembly::array1@10
    IL_00be:  stloc.s    V_5
    IL_00c0:  ldc.i4.2
    IL_00c1:  ldc.i4.2
    IL_00c2:  ldc.i4.0
    IL_00c3:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00c8:  dup
    IL_00c9:  stsfld     int32[0...,0...] '<StartupCode$assembly>'.$assembly::a3@11
    IL_00ce:  stloc.s    V_6
    IL_00d0:  ldc.i4.3
    IL_00d1:  ldc.i4.3
    IL_00d2:  ldc.i4.3
    IL_00d3:  ldc.i4.0
    IL_00d4:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00d9:  dup
    IL_00da:  stsfld     int32[0...,0...,0...] '<StartupCode$assembly>'.$assembly::array3D@12
    IL_00df:  stloc.s    V_7
    IL_00e1:  ldc.i4.4
    IL_00e2:  ldc.i4.4
    IL_00e3:  ldc.i4.4
    IL_00e4:  ldc.i4.4
    IL_00e5:  ldc.i4.0
    IL_00e6:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_00eb:  dup
    IL_00ec:  stsfld     int32[0...,0...,0...,0...] '<StartupCode$assembly>'.$assembly::array4D@13
    IL_00f1:  stloc.s    V_8
    IL_00f3:  call       int32[] assembly::get_array()
    IL_00f8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_00fd:  pop
    IL_00fe:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_0103:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0108:  pop
    IL_0109:  call       class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
    IL_010e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [runtime]System.Tuple`2<!!0,!!1>[])
    IL_0113:  pop
    IL_0114:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
    IL_0119:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_011e:  pop
    IL_011f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
    IL_0124:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_0129:  pop
    IL_012a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
    IL_012f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0134:  dup
    IL_0135:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::a1@25
    IL_013a:  stloc.s    V_9
    IL_013c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_0141:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0146:  dup
    IL_0147:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::a2@26
    IL_014c:  stloc.s    V_10
    IL_014e:  call       int32[] assembly::get_a1()
    IL_0153:  ldc.i4.0
    IL_0154:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Get<int32>(!!0[],
                                                                                               int32)
    IL_0159:  stloc.s    V_14
    IL_015b:  call       int32[] assembly::get_a2()
    IL_0160:  ldc.i4.0
    IL_0161:  ldloc.s    V_14
    IL_0163:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Set<int32>(!!0[],
                                                                                                int32,
                                                                                                !!0)
    IL_0168:  nop
    IL_0169:  call       int32[0...,0...] assembly::get_a3()
    IL_016e:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length1<int32>(!!0[0...,0...])
    IL_0173:  call       int32[0...,0...] assembly::get_a3()
    IL_0178:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length2<int32>(!!0[0...,0...])
    IL_017d:  call       int32[0...,0...] assembly::get_a3()
    IL_0182:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base1<int32>(!!0[0...,0...])
    IL_0187:  call       int32[0...,0...] assembly::get_a3()
    IL_018c:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base2<int32>(!!0[0...,0...])
    IL_0191:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_0196:  stloc.s    V_15
    IL_0198:  ldloc.s    V_15
    IL_019a:  stloc.s    V_16
    IL_019c:  call       int32[0...,0...] assembly::get_a3()
    IL_01a1:  ldc.i4.0
    IL_01a2:  ldc.i4.0
    IL_01a3:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Get<int32>(!!0[0...,0...],
                                                                                                 int32,
                                                                                                 int32)
    IL_01a8:  stloc.s    V_17
    IL_01aa:  call       int32[0...,0...] assembly::get_a3()
    IL_01af:  ldc.i4.0
    IL_01b0:  ldc.i4.0
    IL_01b1:  ldloc.s    V_17
    IL_01b3:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Set<int32>(!!0[0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_01b8:  nop
    IL_01b9:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01be:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length1<int32>(!!0[0...,0...,0...])
    IL_01c3:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01c8:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length2<int32>(!!0[0...,0...,0...])
    IL_01cd:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01d2:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length3<int32>(!!0[0...,0...,0...])
    IL_01d7:  newobj     instance void class [runtime]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2)
    IL_01dc:  stloc.s    V_18
    IL_01de:  ldloc.s    V_18
    IL_01e0:  stloc.s    V_19
    IL_01e2:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01e7:  ldc.i4.0
    IL_01e8:  ldc.i4.0
    IL_01e9:  ldc.i4.0
    IL_01ea:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Get<int32>(!!0[0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_01ef:  stloc.s    V_20
    IL_01f1:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01f6:  ldc.i4.0
    IL_01f7:  ldc.i4.0
    IL_01f8:  ldc.i4.0
    IL_01f9:  ldloc.s    V_20
    IL_01fb:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Set<int32>(!!0[0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0200:  nop
    IL_0201:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0206:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length1<int32>(!!0[0...,0...,0...,0...])
    IL_020b:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0210:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length2<int32>(!!0[0...,0...,0...,0...])
    IL_0215:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_021a:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length3<int32>(!!0[0...,0...,0...,0...])
    IL_021f:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0224:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length4<int32>(!!0[0...,0...,0...,0...])
    IL_0229:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_022e:  stloc.s    V_21
    IL_0230:  ldloc.s    V_21
    IL_0232:  stloc.s    V_22
    IL_0234:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0239:  ldc.i4.0
    IL_023a:  ldc.i4.0
    IL_023b:  ldc.i4.0
    IL_023c:  ldc.i4.0
    IL_023d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Get<int32>(!!0[0...,0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_0242:  stloc.s    V_23
    IL_0244:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0249:  ldc.i4.0
    IL_024a:  ldc.i4.0
    IL_024b:  ldc.i4.0
    IL_024c:  ldc.i4.0
    IL_024d:  ldloc.s    V_23
    IL_024f:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0254:  nop
    IL_0255:  ret
  } 

} 






