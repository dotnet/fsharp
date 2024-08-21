




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
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
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_4,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_5,
             int32 V_6,
             class [runtime]System.Tuple`3<int32,int32,int32> V_7,
             class [runtime]System.Tuple`3<int32,int32,int32> V_8,
             int32 V_9,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_10,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_11,
             int32 V_12)
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
    IL_0034:  stelem     [runtime]System.Int32
    IL_0039:  dup
    IL_003a:  ldc.i4.1
    IL_003b:  ldc.i4.2
    IL_003c:  stelem     [runtime]System.Int32
    IL_0041:  dup
    IL_0042:  ldc.i4.2
    IL_0043:  ldc.i4.3
    IL_0044:  stelem     [runtime]System.Int32
    IL_0049:  stsfld     int32[] assembly::array@6
    IL_004e:  ldc.i4.1
    IL_004f:  ldc.i4.1
    IL_0050:  ldc.i4.s   10
    IL_0052:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0057:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_005c:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::aseq@7
    IL_0061:  ldc.i4.1
    IL_0062:  ldc.i4.1
    IL_0063:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_0068:  ldc.i4.2
    IL_0069:  ldc.i4.2
    IL_006a:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_006f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0074:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0079:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::list1@8
    IL_0083:  ldc.i4.0
    IL_0084:  ldnull
    IL_0085:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                        class [runtime]System.Tuple`2<int32,int32>)
    IL_008a:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::seq1@9
    IL_008f:  ldc.i4.2
    IL_0090:  newarr     class [runtime]System.Tuple`2<int32,int32>
    IL_0095:  dup
    IL_0096:  ldc.i4.0
    IL_0097:  ldc.i4.1
    IL_0098:  ldc.i4.1
    IL_0099:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_009e:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00a3:  dup
    IL_00a4:  ldc.i4.1
    IL_00a5:  ldc.i4.2
    IL_00a6:  ldc.i4.2
    IL_00a7:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00ac:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00b1:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::array1@10
    IL_00b6:  ldc.i4.2
    IL_00b7:  ldc.i4.2
    IL_00b8:  ldc.i4.0
    IL_00b9:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00be:  stsfld     int32[0...,0...] assembly::a3@11
    IL_00c3:  ldc.i4.3
    IL_00c4:  ldc.i4.3
    IL_00c5:  ldc.i4.3
    IL_00c6:  ldc.i4.0
    IL_00c7:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00cc:  stsfld     int32[0...,0...,0...] assembly::array3D@12
    IL_00d1:  ldc.i4.4
    IL_00d2:  ldc.i4.4
    IL_00d3:  ldc.i4.4
    IL_00d4:  ldc.i4.4
    IL_00d5:  ldc.i4.0
    IL_00d6:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_00db:  stsfld     int32[0...,0...,0...,0...] assembly::array4D@13
    IL_00e0:  call       int32[] assembly::get_array()
    IL_00e5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_00ea:  pop
    IL_00eb:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_00f0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00f5:  pop
    IL_00f6:  call       class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
    IL_00fb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [runtime]System.Tuple`2<!!0,!!1>[])
    IL_0100:  pop
    IL_0101:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
    IL_0106:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_010b:  pop
    IL_010c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
    IL_0111:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_0116:  pop
    IL_0117:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
    IL_011c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0121:  stsfld     int32[] assembly::a1@25
    IL_0126:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_012b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0130:  stsfld     int32[] assembly::a2@26
    IL_0135:  call       int32[] assembly::get_a1()
    IL_013a:  ldc.i4.0
    IL_013b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Get<int32>(!!0[],
                                                                                               int32)
    IL_0140:  stloc.3
    IL_0141:  call       int32[] assembly::get_a2()
    IL_0146:  ldc.i4.0
    IL_0147:  ldloc.3
    IL_0148:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Set<int32>(!!0[],
                                                                                                int32,
                                                                                                !!0)
    IL_014d:  nop
    IL_014e:  call       int32[0...,0...] assembly::get_a3()
    IL_0153:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length1<int32>(!!0[0...,0...])
    IL_0158:  call       int32[0...,0...] assembly::get_a3()
    IL_015d:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length2<int32>(!!0[0...,0...])
    IL_0162:  call       int32[0...,0...] assembly::get_a3()
    IL_0167:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base1<int32>(!!0[0...,0...])
    IL_016c:  call       int32[0...,0...] assembly::get_a3()
    IL_0171:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base2<int32>(!!0[0...,0...])
    IL_0176:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_017b:  stloc.s    V_4
    IL_017d:  ldloc.s    V_4
    IL_017f:  stloc.s    V_5
    IL_0181:  call       int32[0...,0...] assembly::get_a3()
    IL_0186:  ldc.i4.0
    IL_0187:  ldc.i4.0
    IL_0188:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Get<int32>(!!0[0...,0...],
                                                                                                 int32,
                                                                                                 int32)
    IL_018d:  stloc.s    V_6
    IL_018f:  call       int32[0...,0...] assembly::get_a3()
    IL_0194:  ldc.i4.0
    IL_0195:  ldc.i4.0
    IL_0196:  ldloc.s    V_6
    IL_0198:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Set<int32>(!!0[0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_019d:  nop
    IL_019e:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01a3:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length1<int32>(!!0[0...,0...,0...])
    IL_01a8:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01ad:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length2<int32>(!!0[0...,0...,0...])
    IL_01b2:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01b7:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length3<int32>(!!0[0...,0...,0...])
    IL_01bc:  newobj     instance void class [runtime]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2)
    IL_01c1:  stloc.s    V_7
    IL_01c3:  ldloc.s    V_7
    IL_01c5:  stloc.s    V_8
    IL_01c7:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01cc:  ldc.i4.0
    IL_01cd:  ldc.i4.0
    IL_01ce:  ldc.i4.0
    IL_01cf:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Get<int32>(!!0[0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_01d4:  stloc.s    V_9
    IL_01d6:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01db:  ldc.i4.0
    IL_01dc:  ldc.i4.0
    IL_01dd:  ldc.i4.0
    IL_01de:  ldloc.s    V_9
    IL_01e0:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Set<int32>(!!0[0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_01e5:  nop
    IL_01e6:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01eb:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length1<int32>(!!0[0...,0...,0...,0...])
    IL_01f0:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01f5:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length2<int32>(!!0[0...,0...,0...,0...])
    IL_01fa:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_01ff:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length3<int32>(!!0[0...,0...,0...,0...])
    IL_0204:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0209:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length4<int32>(!!0[0...,0...,0...,0...])
    IL_020e:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_0213:  stloc.s    V_10
    IL_0215:  ldloc.s    V_10
    IL_0217:  stloc.s    V_11
    IL_0219:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_021e:  ldc.i4.0
    IL_021f:  ldc.i4.0
    IL_0220:  ldc.i4.0
    IL_0221:  ldc.i4.0
    IL_0222:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Get<int32>(!!0[0...,0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_0227:  stloc.s    V_12
    IL_0229:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_022e:  ldc.i4.0
    IL_022f:  ldc.i4.0
    IL_0230:  ldc.i4.0
    IL_0231:  ldc.i4.0
    IL_0232:  ldloc.s    V_12
    IL_0234:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0239:  nop
    IL_023a:  ret
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






