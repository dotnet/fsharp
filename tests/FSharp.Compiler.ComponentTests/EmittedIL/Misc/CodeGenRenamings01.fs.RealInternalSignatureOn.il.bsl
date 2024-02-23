




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
    .method public specialname rtspecialname 
            instance void  .ctor(int32 pc,
                                 class [runtime]System.Tuple`2<int32,int32> current) cil managed
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
    .locals init (int32 V_0,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_1,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_2,
             int32 V_3,
             class [runtime]System.Tuple`3<int32,int32,int32> V_4,
             class [runtime]System.Tuple`3<int32,int32,int32> V_5,
             int32 V_6,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_7,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_8,
             int32 V_9)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0013:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::alist@5
    IL_0018:  ldc.i4.3
    IL_0019:  newarr     [runtime]System.Int32
    IL_001e:  dup
    IL_001f:  ldc.i4.0
    IL_0020:  ldc.i4.1
    IL_0021:  stelem     [runtime]System.Int32
    IL_0026:  dup
    IL_0027:  ldc.i4.1
    IL_0028:  ldc.i4.2
    IL_0029:  stelem     [runtime]System.Int32
    IL_002e:  dup
    IL_002f:  ldc.i4.2
    IL_0030:  ldc.i4.3
    IL_0031:  stelem     [runtime]System.Int32
    IL_0036:  stsfld     int32[] assembly::array@6
    IL_003b:  ldc.i4.1
    IL_003c:  ldc.i4.1
    IL_003d:  ldc.i4.s   10
    IL_003f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0044:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0049:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::aseq@7
    IL_004e:  ldc.i4.1
    IL_004f:  ldc.i4.1
    IL_0050:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0055:  ldc.i4.2
    IL_0056:  ldc.i4.2
    IL_0057:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0061:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0066:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::list1@8
    IL_0070:  ldc.i4.0
    IL_0071:  ldnull
    IL_0072:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                        class [runtime]System.Tuple`2<int32,int32>)
    IL_0077:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::seq1@9
    IL_007c:  ldc.i4.2
    IL_007d:  newarr     class [runtime]System.Tuple`2<int32,int32>
    IL_0082:  dup
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.1
    IL_0085:  ldc.i4.1
    IL_0086:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_008b:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_0090:  dup
    IL_0091:  ldc.i4.1
    IL_0092:  ldc.i4.2
    IL_0093:  ldc.i4.2
    IL_0094:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0099:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_009e:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::array1@10
    IL_00a3:  ldc.i4.2
    IL_00a4:  ldc.i4.2
    IL_00a5:  ldc.i4.0
    IL_00a6:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00ab:  stsfld     int32[0...,0...] assembly::a3@11
    IL_00b0:  ldc.i4.3
    IL_00b1:  ldc.i4.3
    IL_00b2:  ldc.i4.3
    IL_00b3:  ldc.i4.0
    IL_00b4:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00b9:  stsfld     int32[0...,0...,0...] assembly::array3D@12
    IL_00be:  ldc.i4.4
    IL_00bf:  ldc.i4.4
    IL_00c0:  ldc.i4.4
    IL_00c1:  ldc.i4.4
    IL_00c2:  ldc.i4.0
    IL_00c3:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_00c8:  stsfld     int32[0...,0...,0...,0...] assembly::array4D@13
    IL_00cd:  call       int32[] assembly::get_array()
    IL_00d2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_00d7:  pop
    IL_00d8:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_00dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00e2:  pop
    IL_00e3:  call       class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
    IL_00e8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [runtime]System.Tuple`2<!!0,!!1>[])
    IL_00ed:  pop
    IL_00ee:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
    IL_00f3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_00f8:  pop
    IL_00f9:  call       class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
    IL_00fe:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_0103:  pop
    IL_0104:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
    IL_0109:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_010e:  stsfld     int32[] assembly::a1@25
    IL_0113:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_0118:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_011d:  stsfld     int32[] assembly::a2@26
    IL_0122:  call       int32[] assembly::get_a1()
    IL_0127:  ldc.i4.0
    IL_0128:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Get<int32>(!!0[],
                                                                                               int32)
    IL_012d:  stloc.0
    IL_012e:  call       int32[] assembly::get_a2()
    IL_0133:  ldc.i4.0
    IL_0134:  ldloc.0
    IL_0135:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Set<int32>(!!0[],
                                                                                                int32,
                                                                                                !!0)
    IL_013a:  nop
    IL_013b:  call       int32[0...,0...] assembly::get_a3()
    IL_0140:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length1<int32>(!!0[0...,0...])
    IL_0145:  call       int32[0...,0...] assembly::get_a3()
    IL_014a:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length2<int32>(!!0[0...,0...])
    IL_014f:  call       int32[0...,0...] assembly::get_a3()
    IL_0154:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base1<int32>(!!0[0...,0...])
    IL_0159:  call       int32[0...,0...] assembly::get_a3()
    IL_015e:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base2<int32>(!!0[0...,0...])
    IL_0163:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_0168:  stloc.1
    IL_0169:  ldloc.1
    IL_016a:  stloc.2
    IL_016b:  call       int32[0...,0...] assembly::get_a3()
    IL_0170:  ldc.i4.0
    IL_0171:  ldc.i4.0
    IL_0172:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Get<int32>(!!0[0...,0...],
                                                                                                 int32,
                                                                                                 int32)
    IL_0177:  stloc.3
    IL_0178:  call       int32[0...,0...] assembly::get_a3()
    IL_017d:  ldc.i4.0
    IL_017e:  ldc.i4.0
    IL_017f:  ldloc.3
    IL_0180:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Set<int32>(!!0[0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0185:  nop
    IL_0186:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_018b:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length1<int32>(!!0[0...,0...,0...])
    IL_0190:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_0195:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length2<int32>(!!0[0...,0...,0...])
    IL_019a:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_019f:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length3<int32>(!!0[0...,0...,0...])
    IL_01a4:  newobj     instance void class [runtime]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_01a9:  stloc.s    V_4
    IL_01ab:  ldloc.s    V_4
    IL_01ad:  stloc.s    V_5
    IL_01af:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01b4:  ldc.i4.0
    IL_01b5:  ldc.i4.0
    IL_01b6:  ldc.i4.0
    IL_01b7:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Get<int32>(!!0[0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_01bc:  stloc.s    V_6
    IL_01be:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01c3:  ldc.i4.0
    IL_01c4:  ldc.i4.0
    IL_01c5:  ldc.i4.0
    IL_01c6:  ldloc.s    V_6
    IL_01c8:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Set<int32>(!!0[0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_01cd:  nop
    IL_01ce:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01d3:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length1<int32>(!!0[0...,0...,0...,0...])
    IL_01d8:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01dd:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length2<int32>(!!0[0...,0...,0...,0...])
    IL_01e2:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01e7:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length3<int32>(!!0[0...,0...,0...,0...])
    IL_01ec:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01f1:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length4<int32>(!!0[0...,0...,0...,0...])
    IL_01f6:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_01fb:  stloc.s    V_7
    IL_01fd:  ldloc.s    V_7
    IL_01ff:  stloc.s    V_8
    IL_0201:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0206:  ldc.i4.0
    IL_0207:  ldc.i4.0
    IL_0208:  ldc.i4.0
    IL_0209:  ldc.i4.0
    IL_020a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Get<int32>(!!0[0...,0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_020f:  stloc.s    V_9
    IL_0211:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0216:  ldc.i4.0
    IL_0217:  ldc.i4.0
    IL_0218:  ldc.i4.0
    IL_0219:  ldc.i4.0
    IL_021a:  ldloc.s    V_9
    IL_021c:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0221:  nop
    IL_0222:  ret
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






