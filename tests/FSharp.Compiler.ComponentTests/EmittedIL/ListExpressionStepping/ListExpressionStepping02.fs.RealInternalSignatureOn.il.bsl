




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





.class public abstract auto ansi sealed ListExpressionSteppingTest2
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest2
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 stage #2 at line 18@18'<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<!a,int32>,class [runtime]System.Tuple`2<!a,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a> @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<!a,int32>,class [runtime]System.Tuple`2<!a,int32>>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Tuple`2<!a,int32> Invoke(class [runtime]System.Tuple`2<!a,int32> tupledArg) cil managed
      {
        
        .maxstack  7
        .locals init (!a V_0,
                 int32 V_1)
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [runtime]System.Tuple`2<!a,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [runtime]System.Tuple`2<!a,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldloc.0
        IL_000f:  ldloc.1
        IL_0010:  ldc.i4.1
        IL_0011:  add
        IL_0012:  newobj     instance void class [runtime]System.Tuple`2<!a,int32>::.ctor(!0,
                                                                                                 !1)
        IL_0017:  ret
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a>::@_instance
        IL_000a:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit xs1@19<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<!a,int32>,class [runtime]System.Tuple`2<!a,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a> @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<!a,int32>,class [runtime]System.Tuple`2<!a,int32>>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Tuple`2<!a,int32> Invoke(class [runtime]System.Tuple`2<!a,int32> tupledArg) cil managed
      {
        
        .maxstack  7
        .locals init (!a V_0,
                 int32 V_1)
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [runtime]System.Tuple`2<!a,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [runtime]System.Tuple`2<!a,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldloc.0
        IL_000f:  ldloc.1
        IL_0010:  ldc.i4.1
        IL_0011:  add
        IL_0012:  newobj     instance void class [runtime]System.Tuple`2<!a,int32>::.ctor(!0,
                                                                                                 !1)
        IL_0017:  ret
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a>::@_instance
        IL_000a:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 stage #2 at line 24@24'<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<!a,int32,int32>,class [runtime]System.Tuple`3<!a,int32,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a> @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<!a,int32,int32>,class [runtime]System.Tuple`3<!a,int32,int32>>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Tuple`3<!a,int32,int32> Invoke(class [runtime]System.Tuple`3<!a,int32,int32> tupledArg) cil managed
      {
        
        .maxstack  7
        .locals init (!a V_0,
                 int32 V_1,
                 int32 V_2)
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldarg.1
        IL_000f:  call       instance !2 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item3()
        IL_0014:  stloc.2
        IL_0015:  ldloc.0
        IL_0016:  ldloc.1
        IL_0017:  ldc.i4.1
        IL_0018:  add
        IL_0019:  ldloc.2
        IL_001a:  newobj     instance void class [runtime]System.Tuple`3<!a,int32,int32>::.ctor(!0,
                                                                                                       !1,
                                                                                                       !2)
        IL_001f:  ret
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a>::@_instance
        IL_000a:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit xs2@25<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<!a,int32,int32>,class [runtime]System.Tuple`3<!a,int32,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a> @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<!a,int32,int32>,class [runtime]System.Tuple`3<!a,int32,int32>>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Tuple`3<!a,int32,int32> Invoke(class [runtime]System.Tuple`3<!a,int32,int32> tupledArg) cil managed
      {
        
        .maxstack  7
        .locals init (!a V_0,
                 int32 V_1,
                 int32 V_2)
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldarg.1
        IL_000f:  call       instance !2 class [runtime]System.Tuple`3<!a,int32,int32>::get_Item3()
        IL_0014:  stloc.2
        IL_0015:  ldloc.0
        IL_0016:  ldloc.1
        IL_0017:  ldc.i4.1
        IL_0018:  add
        IL_0019:  ldloc.2
        IL_001a:  newobj     instance void class [runtime]System.Tuple`3<!a,int32,int32>::.ctor(!0,
                                                                                                       !1,
                                                                                                       !2)
        IL_001f:  ret
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a>::@_instance
        IL_000a:  ret
      } 

    } 

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1() cil managed
    {
      
      .maxstack  4
      .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0)
      IL_0000:  nop
      IL_0001:  ldstr      "hello"
      IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0010:  pop
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldc.i4.1
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldstr      "goodbye"
      IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0029:  pop
      IL_002a:  ldloca.s   V_0
      IL_002c:  ldc.i4.2
      IL_002d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0032:  nop
      IL_0033:  ldloca.s   V_0
      IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_003a:  ret
    } 

    .method public static class [runtime]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!a,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!a,int32,int32>>> f2<a>(!!a x) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!a,int32>> V_0,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> V_1,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
               valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
               uint64 V_4,
               int32 V_5,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!a,int32>> V_6,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!a,int32>> V_7,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!a,int32,int32>> V_8,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> V_9,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_10,
               valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_11,
               uint64 V_12,
               int32 V_13,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_14,
               valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_15,
               uint64 V_16,
               int32 V_17,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!a,int32,int32>> V_18,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!a,int32,int32>> V_19)
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldarg.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::get_Empty()
      IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0017:  stloc.1
      IL_0018:  ldc.i4.0
      IL_0019:  conv.i8
      IL_001a:  stloc.s    V_4
      IL_001c:  ldc.i4.0
      IL_001d:  stloc.s    V_5
      IL_001f:  br.s       IL_0038

      IL_0021:  ldloca.s   V_3
      IL_0023:  ldloc.s    V_5
      IL_0025:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002a:  nop
      IL_002b:  ldloc.s    V_5
      IL_002d:  ldc.i4.1
      IL_002e:  add
      IL_002f:  stloc.s    V_5
      IL_0031:  ldloc.s    V_4
      IL_0033:  ldc.i4.1
      IL_0034:  conv.i8
      IL_0035:  add
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.s    V_4
      IL_003a:  ldc.i4.3
      IL_003b:  conv.i8
      IL_003c:  blt.un.s   IL_0021

      IL_003e:  ldloca.s   V_3
      IL_0040:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_0045:  stloc.2
      IL_0046:  ldloc.1
      IL_0047:  ldloc.2
      IL_0048:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,!!1>> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Zip<!!0,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>,
                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1>)
      IL_004d:  stloc.s    V_6
      IL_004f:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!!a>::@_instance
      IL_0054:  ldloc.s    V_6
      IL_0056:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [runtime]System.Tuple`2<!!0,int32>,class [runtime]System.Tuple`2<!!0,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_005b:  stloc.s    V_7
      IL_005d:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!!a>::@_instance
      IL_0062:  ldloc.s    V_7
      IL_0064:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [runtime]System.Tuple`2<!!0,int32>,class [runtime]System.Tuple`2<!!0,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_0069:  stloc.0
      IL_006a:  ldarg.0
      IL_006b:  ldarg.0
      IL_006c:  ldarg.0
      IL_006d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::get_Empty()
      IL_0072:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0077:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_007c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0081:  stloc.s    V_9
      IL_0083:  ldc.i4.0
      IL_0084:  conv.i8
      IL_0085:  stloc.s    V_12
      IL_0087:  ldc.i4.0
      IL_0088:  stloc.s    V_13
      IL_008a:  br.s       IL_00a3

      IL_008c:  ldloca.s   V_11
      IL_008e:  ldloc.s    V_13
      IL_0090:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0095:  nop
      IL_0096:  ldloc.s    V_13
      IL_0098:  ldc.i4.1
      IL_0099:  add
      IL_009a:  stloc.s    V_13
      IL_009c:  ldloc.s    V_12
      IL_009e:  ldc.i4.1
      IL_009f:  conv.i8
      IL_00a0:  add
      IL_00a1:  stloc.s    V_12
      IL_00a3:  ldloc.s    V_12
      IL_00a5:  ldc.i4.3
      IL_00a6:  conv.i8
      IL_00a7:  blt.un.s   IL_008c

      IL_00a9:  ldloca.s   V_11
      IL_00ab:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_00b0:  stloc.s    V_10
      IL_00b2:  ldc.i4.0
      IL_00b3:  conv.i8
      IL_00b4:  stloc.s    V_16
      IL_00b6:  ldc.i4.0
      IL_00b7:  stloc.s    V_17
      IL_00b9:  br.s       IL_00d2

      IL_00bb:  ldloca.s   V_15
      IL_00bd:  ldloc.s    V_17
      IL_00bf:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_00c4:  nop
      IL_00c5:  ldloc.s    V_17
      IL_00c7:  ldc.i4.1
      IL_00c8:  add
      IL_00c9:  stloc.s    V_17
      IL_00cb:  ldloc.s    V_16
      IL_00cd:  ldc.i4.1
      IL_00ce:  conv.i8
      IL_00cf:  add
      IL_00d0:  stloc.s    V_16
      IL_00d2:  ldloc.s    V_16
      IL_00d4:  ldc.i4.3
      IL_00d5:  conv.i8
      IL_00d6:  blt.un.s   IL_00bb

      IL_00d8:  ldloca.s   V_15
      IL_00da:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_00df:  stloc.s    V_14
      IL_00e1:  ldloc.s    V_9
      IL_00e3:  ldloc.s    V_10
      IL_00e5:  ldloc.s    V_14
      IL_00e7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!0,!!1,!!2>> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Zip3<!!0,int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!2>)
      IL_00ec:  stloc.s    V_18
      IL_00ee:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!!a>::@_instance
      IL_00f3:  ldloc.s    V_18
      IL_00f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [runtime]System.Tuple`3<!!0,int32,int32>,class [runtime]System.Tuple`3<!!0,int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_00fa:  stloc.s    V_19
      IL_00fc:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!!a>::@_instance
      IL_0101:  ldloc.s    V_19
      IL_0103:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [runtime]System.Tuple`3<!!0,int32,int32>,class [runtime]System.Tuple`3<!!0,int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_0108:  stloc.s    V_8
      IL_010a:  ldloc.0
      IL_010b:  ldloc.s    V_8
      IL_010d:  newobj     instance void class [runtime]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!a,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!a,int32,int32>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                        !1)
      IL_0112:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$ListExpressionSteppingTest2::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$ListExpressionSteppingTest2::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest2/ListExpressionSteppingTest2::f1()
      IL_0005:  pop
      IL_0006:  ldc.i4.5
      IL_0007:  call       class [runtime]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<!!0,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`3<!!0,int32,int32>>> ListExpressionSteppingTest2/ListExpressionSteppingTest2::f2<int32>(!!0)
      IL_000c:  pop
      IL_000d:  ret
    } 

  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$ListExpressionSteppingTest2::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$ListExpressionSteppingTest2::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void ListExpressionSteppingTest2/ListExpressionSteppingTest2::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ListExpressionSteppingTest2
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
    IL_0000:  call       void ListExpressionSteppingTest2::staticInitialization@()
    IL_0005:  ret
  } 

} 






