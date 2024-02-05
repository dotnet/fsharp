




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

    .method public strict virtual instance int32 
            GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>>& next) cil managed
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

    .method public strict virtual instance void 
            Close() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.3
      IL_0002:  stfld      int32 assembly/seq1@9::pc
      IL_0007:  ret
    } 

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
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

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,int32> 
            get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Tuple`2<int32,int32> assembly/seq1@9::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<int32,int32>> 
            GetFreshEnumerator() cil managed
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

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_alist() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::alist@5
    IL_0005:  ret
  } 

  .method public specialname static int32[] 
          get_array() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::array@6
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<int32> 
          get_aseq() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$assembly>'.$assembly::aseq@7
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> 
          get_list1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::list1@8
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> 
          get_seq1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::seq1@9
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<int32,int32>[] 
          get_array1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<int32,int32>[] '<StartupCode$assembly>'.$assembly::array1@10
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...] 
          get_a3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...] '<StartupCode$assembly>'.$assembly::a3@11
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...] 
          get_array3D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...] '<StartupCode$assembly>'.$assembly::array3D@12
    IL_0005:  ret
  } 

  .method public specialname static int32[0...,0...,0...,0...] 
          get_array4D() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...,0...] '<StartupCode$assembly>'.$assembly::array4D@13
    IL_0005:  ret
  } 

  .method public specialname static int32[] 
          get_a1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::a1@25
    IL_0005:  ret
  } 

  .method public specialname static int32[] 
          get_a2() cil managed
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
             int32 V_12,
             int32 V_13,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_14,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_15,
             int32 V_16,
             class [runtime]System.Tuple`3<int32,int32,int32> V_17,
             class [runtime]System.Tuple`3<int32,int32,int32> V_18,
             int32 V_19,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_20,
             class [runtime]System.Tuple`4<int32,int32,int32,int32> V_21,
             int32 V_22)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.s    V_12
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.s    V_12
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloca.s   V_11
    IL_000a:  ldloc.s    V_12
    IL_000c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0011:  nop
    IL_0012:  ldloc.s    V_12
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.s    V_12
    IL_0018:  ldloc.s    V_12
    IL_001a:  ldc.i4.s   10
    IL_001c:  bge.s      IL_002f

    IL_001e:  ldc.i4.1
    IL_001f:  ldloc.s    V_12
    IL_0021:  bge.s      IL_0027

    IL_0023:  ldc.i4.1
    IL_0024:  nop
    IL_0025:  br.s       IL_0036

    IL_0027:  ldc.i4.1
    IL_0028:  ldloc.s    V_12
    IL_002a:  ceq
    IL_002c:  nop
    IL_002d:  br.s       IL_0036

    IL_002f:  ldloc.s    V_12
    IL_0031:  ldc.i4.s   10
    IL_0033:  ceq
    IL_0035:  nop
    IL_0036:  brtrue.s   IL_0008

    IL_0038:  ldloca.s   V_11
    IL_003a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003f:  dup
    IL_0040:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::alist@5
    IL_0045:  stloc.0
    IL_0046:  ldc.i4.3
    IL_0047:  newarr     [runtime]System.Int32
    IL_004c:  dup
    IL_004d:  ldc.i4.0
    IL_004e:  ldc.i4.1
    IL_004f:  stelem     [runtime]System.Int32
    IL_0054:  dup
    IL_0055:  ldc.i4.1
    IL_0056:  ldc.i4.2
    IL_0057:  stelem     [runtime]System.Int32
    IL_005c:  dup
    IL_005d:  ldc.i4.2
    IL_005e:  ldc.i4.3
    IL_005f:  stelem     [runtime]System.Int32
    IL_0064:  dup
    IL_0065:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::array@6
    IL_006a:  stloc.1
    IL_006b:  ldc.i4.1
    IL_006c:  ldc.i4.1
    IL_006d:  ldc.i4.s   10
    IL_006f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0074:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0079:  dup
    IL_007a:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$assembly>'.$assembly::aseq@7
    IL_007f:  stloc.2
    IL_0080:  ldc.i4.1
    IL_0081:  ldc.i4.1
    IL_0082:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_0087:  ldc.i4.2
    IL_0088:  ldc.i4.2
    IL_0089:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_008e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0093:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0098:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009d:  dup
    IL_009e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::list1@8
    IL_00a3:  stloc.3
    IL_00a4:  ldc.i4.0
    IL_00a5:  ldnull
    IL_00a6:  newobj     instance void assembly/seq1@9::.ctor(int32,
                                                                        class [runtime]System.Tuple`2<int32,int32>)
    IL_00ab:  dup
    IL_00ac:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> '<StartupCode$assembly>'.$assembly::seq1@9
    IL_00b1:  stloc.s    V_4
    IL_00b3:  ldc.i4.2
    IL_00b4:  newarr     class [runtime]System.Tuple`2<int32,int32>
    IL_00b9:  dup
    IL_00ba:  ldc.i4.0
    IL_00bb:  ldc.i4.1
    IL_00bc:  ldc.i4.1
    IL_00bd:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00c2:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00c7:  dup
    IL_00c8:  ldc.i4.1
    IL_00c9:  ldc.i4.2
    IL_00ca:  ldc.i4.2
    IL_00cb:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                !1)
    IL_00d0:  stelem     class [runtime]System.Tuple`2<int32,int32>
    IL_00d5:  dup
    IL_00d6:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] '<StartupCode$assembly>'.$assembly::array1@10
    IL_00db:  stloc.s    V_5
    IL_00dd:  ldc.i4.2
    IL_00de:  ldc.i4.2
    IL_00df:  ldc.i4.0
    IL_00e0:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00e5:  dup
    IL_00e6:  stsfld     int32[0...,0...] '<StartupCode$assembly>'.$assembly::a3@11
    IL_00eb:  stloc.s    V_6
    IL_00ed:  ldc.i4.3
    IL_00ee:  ldc.i4.3
    IL_00ef:  ldc.i4.3
    IL_00f0:  ldc.i4.0
    IL_00f1:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00f6:  dup
    IL_00f7:  stsfld     int32[0...,0...,0...] '<StartupCode$assembly>'.$assembly::array3D@12
    IL_00fc:  stloc.s    V_7
    IL_00fe:  ldc.i4.4
    IL_00ff:  ldc.i4.4
    IL_0100:  ldc.i4.4
    IL_0101:  ldc.i4.4
    IL_0102:  ldc.i4.0
    IL_0103:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_0108:  dup
    IL_0109:  stsfld     int32[0...,0...,0...,0...] '<StartupCode$assembly>'.$assembly::array4D@13
    IL_010e:  stloc.s    V_8
    IL_0110:  call       int32[] assembly::get_array()
    IL_0115:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_011a:  pop
    IL_011b:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_0120:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0125:  pop
    IL_0126:  call       class [runtime]System.Tuple`2<int32,int32>[] assembly::get_array1()
    IL_012b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [runtime]System.Tuple`2<!!0,!!1>[])
    IL_0130:  pop
    IL_0131:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_list1()
    IL_0136:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_013b:  pop
    IL_013c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> assembly::get_seq1()
    IL_0141:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<!!0,!!1>>)
    IL_0146:  pop
    IL_0147:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_alist()
    IL_014c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0151:  dup
    IL_0152:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::a1@25
    IL_0157:  stloc.s    V_9
    IL_0159:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::get_aseq()
    IL_015e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0163:  dup
    IL_0164:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::a2@26
    IL_0169:  stloc.s    V_10
    IL_016b:  call       int32[] assembly::get_a1()
    IL_0170:  ldc.i4.0
    IL_0171:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Get<int32>(!!0[],
                                                                                               int32)
    IL_0176:  stloc.s    V_13
    IL_0178:  call       int32[] assembly::get_a2()
    IL_017d:  ldc.i4.0
    IL_017e:  ldloc.s    V_13
    IL_0180:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Set<int32>(!!0[],
                                                                                                int32,
                                                                                                !!0)
    IL_0185:  nop
    IL_0186:  call       int32[0...,0...] assembly::get_a3()
    IL_018b:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length1<int32>(!!0[0...,0...])
    IL_0190:  call       int32[0...,0...] assembly::get_a3()
    IL_0195:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length2<int32>(!!0[0...,0...])
    IL_019a:  call       int32[0...,0...] assembly::get_a3()
    IL_019f:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base1<int32>(!!0[0...,0...])
    IL_01a4:  call       int32[0...,0...] assembly::get_a3()
    IL_01a9:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base2<int32>(!!0[0...,0...])
    IL_01ae:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_01b3:  stloc.s    V_14
    IL_01b5:  ldloc.s    V_14
    IL_01b7:  stloc.s    V_15
    IL_01b9:  call       int32[0...,0...] assembly::get_a3()
    IL_01be:  ldc.i4.0
    IL_01bf:  ldc.i4.0
    IL_01c0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Get<int32>(!!0[0...,0...],
                                                                                                 int32,
                                                                                                 int32)
    IL_01c5:  stloc.s    V_16
    IL_01c7:  call       int32[0...,0...] assembly::get_a3()
    IL_01cc:  ldc.i4.0
    IL_01cd:  ldc.i4.0
    IL_01ce:  ldloc.s    V_16
    IL_01d0:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Set<int32>(!!0[0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_01d5:  nop
    IL_01d6:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01db:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length1<int32>(!!0[0...,0...,0...])
    IL_01e0:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01e5:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length2<int32>(!!0[0...,0...,0...])
    IL_01ea:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_01ef:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length3<int32>(!!0[0...,0...,0...])
    IL_01f4:  newobj     instance void class [runtime]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2)
    IL_01f9:  stloc.s    V_17
    IL_01fb:  ldloc.s    V_17
    IL_01fd:  stloc.s    V_18
    IL_01ff:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_0204:  ldc.i4.0
    IL_0205:  ldc.i4.0
    IL_0206:  ldc.i4.0
    IL_0207:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Get<int32>(!!0[0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_020c:  stloc.s    V_19
    IL_020e:  call       int32[0...,0...,0...] assembly::get_array3D()
    IL_0213:  ldc.i4.0
    IL_0214:  ldc.i4.0
    IL_0215:  ldc.i4.0
    IL_0216:  ldloc.s    V_19
    IL_0218:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Set<int32>(!!0[0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_021d:  nop
    IL_021e:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0223:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length1<int32>(!!0[0...,0...,0...,0...])
    IL_0228:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_022d:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length2<int32>(!!0[0...,0...,0...,0...])
    IL_0232:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0237:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length3<int32>(!!0[0...,0...,0...,0...])
    IL_023c:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0241:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length4<int32>(!!0[0...,0...,0...,0...])
    IL_0246:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_024b:  stloc.s    V_20
    IL_024d:  ldloc.s    V_20
    IL_024f:  stloc.s    V_21
    IL_0251:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0256:  ldc.i4.0
    IL_0257:  ldc.i4.0
    IL_0258:  ldc.i4.0
    IL_0259:  ldc.i4.0
    IL_025a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Get<int32>(!!0[0...,0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_025f:  stloc.s    V_22
    IL_0261:  call       int32[0...,0...,0...,0...] assembly::get_array4D()
    IL_0266:  ldc.i4.0
    IL_0267:  ldc.i4.0
    IL_0268:  ldc.i4.0
    IL_0269:  ldc.i4.0
    IL_026a:  ldloc.s    V_22
    IL_026c:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0271:  nop
    IL_0272:  ret
  } 

} 






