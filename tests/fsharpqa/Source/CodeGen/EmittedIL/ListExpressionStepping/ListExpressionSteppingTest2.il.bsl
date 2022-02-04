
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly ListExpressionSteppingTest2
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest2
{
  // Offset: 0x00000000 Length: 0x000002C8
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest2
{
  // Offset: 0x000002D0 Length: 0x000000BC
}
.module ListExpressionSteppingTest2.exe
// MVID: {61FD32BA-D3DE-B780-A745-0383BA32FD61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06760000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest2
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest2
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 stage #2 at line 18@18'<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<!a,int32>,class [mscorlib]System.Tuple`2<!a,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a> @_instance
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<!a,int32>,class [mscorlib]System.Tuple`2<!a,int32>>::.ctor()
        IL_0006:  ret
      } // end of method 'Pipe #1 stage #2 at line 18@18'::.ctor

      .method public strict virtual instance class [mscorlib]System.Tuple`2<!a,int32> 
              Invoke(class [mscorlib]System.Tuple`2<!a,int32> tupledArg) cil managed
      {
        // Code size       24 (0x18)
        .maxstack  7
        .locals init ([0] !a a,
                 [1] int32 b)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest2.fs'
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<!a,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<!a,int32>::get_Item2()
        IL_000d:  stloc.1
        .line 18,18 : 38,44 ''
        IL_000e:  ldloc.0
        IL_000f:  ldloc.1
        IL_0010:  ldc.i4.1
        IL_0011:  add
        IL_0012:  newobj     instance void class [mscorlib]System.Tuple`2<!a,int32>::.ctor(!0,
                                                                                           !1)
        IL_0017:  ret
      } // end of method 'Pipe #1 stage #2 at line 18@18'::Invoke

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!a>::@_instance
        IL_000a:  ret
      } // end of method 'Pipe #1 stage #2 at line 18@18'::.cctor

    } // end of class 'Pipe #1 stage #2 at line 18@18'

    .class auto ansi serializable sealed nested assembly beforefieldinit xs1@19<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<!a,int32>,class [mscorlib]System.Tuple`2<!a,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a> @_instance
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<!a,int32>,class [mscorlib]System.Tuple`2<!a,int32>>::.ctor()
        IL_0006:  ret
      } // end of method xs1@19::.ctor

      .method public strict virtual instance class [mscorlib]System.Tuple`2<!a,int32> 
              Invoke(class [mscorlib]System.Tuple`2<!a,int32> tupledArg) cil managed
      {
        // Code size       24 (0x18)
        .maxstack  7
        .locals init ([0] !a a,
                 [1] int32 b)
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<!a,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<!a,int32>::get_Item2()
        IL_000d:  stloc.1
        .line 19,19 : 38,44 ''
        IL_000e:  ldloc.0
        IL_000f:  ldloc.1
        IL_0010:  ldc.i4.1
        IL_0011:  add
        IL_0012:  newobj     instance void class [mscorlib]System.Tuple`2<!a,int32>::.ctor(!0,
                                                                                           !1)
        IL_0017:  ret
      } // end of method xs1@19::Invoke

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!a>::@_instance
        IL_000a:  ret
      } // end of method xs1@19::.cctor

    } // end of class xs1@19

    .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 stage #2 at line 24@24'<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<!a,int32,int32>,class [mscorlib]System.Tuple`3<!a,int32,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a> @_instance
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<!a,int32,int32>,class [mscorlib]System.Tuple`3<!a,int32,int32>>::.ctor()
        IL_0006:  ret
      } // end of method 'Pipe #2 stage #2 at line 24@24'::.ctor

      .method public strict virtual instance class [mscorlib]System.Tuple`3<!a,int32,int32> 
              Invoke(class [mscorlib]System.Tuple`3<!a,int32,int32> tupledArg) cil managed
      {
        // Code size       32 (0x20)
        .maxstack  7
        .locals init ([0] !a a,
                 [1] int32 b,
                 [2] int32 c)
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldarg.1
        IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item3()
        IL_0014:  stloc.2
        .line 24,24 : 40,49 ''
        IL_0015:  ldloc.0
        IL_0016:  ldloc.1
        IL_0017:  ldc.i4.1
        IL_0018:  add
        IL_0019:  ldloc.2
        IL_001a:  newobj     instance void class [mscorlib]System.Tuple`3<!a,int32,int32>::.ctor(!0,
                                                                                                 !1,
                                                                                                 !2)
        IL_001f:  ret
      } // end of method 'Pipe #2 stage #2 at line 24@24'::Invoke

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!a>::@_instance
        IL_000a:  ret
      } // end of method 'Pipe #2 stage #2 at line 24@24'::.cctor

    } // end of class 'Pipe #2 stage #2 at line 24@24'

    .class auto ansi serializable sealed nested assembly beforefieldinit xs2@25<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<!a,int32,int32>,class [mscorlib]System.Tuple`3<!a,int32,int32>>
    {
      .field static assembly initonly class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a> @_instance
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<!a,int32,int32>,class [mscorlib]System.Tuple`3<!a,int32,int32>>::.ctor()
        IL_0006:  ret
      } // end of method xs2@25::.ctor

      .method public strict virtual instance class [mscorlib]System.Tuple`3<!a,int32,int32> 
              Invoke(class [mscorlib]System.Tuple`3<!a,int32,int32> tupledArg) cil managed
      {
        // Code size       32 (0x20)
        .maxstack  7
        .locals init ([0] !a a,
                 [1] int32 b,
                 [2] int32 c)
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.1
        IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item1()
        IL_0006:  stloc.0
        IL_0007:  ldarg.1
        IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item2()
        IL_000d:  stloc.1
        IL_000e:  ldarg.1
        IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<!a,int32,int32>::get_Item3()
        IL_0014:  stloc.2
        .line 25,25 : 40,49 ''
        IL_0015:  ldloc.0
        IL_0016:  ldloc.1
        IL_0017:  ldc.i4.1
        IL_0018:  add
        IL_0019:  ldloc.2
        IL_001a:  newobj     instance void class [mscorlib]System.Tuple`3<!a,int32,int32>::.ctor(!0,
                                                                                                 !1,
                                                                                                 !2)
        IL_001f:  ret
      } // end of method xs2@25::Invoke

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  10
        IL_0000:  newobj     instance void class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a>::.ctor()
        IL_0005:  stsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!a>::@_instance
        IL_000a:  ret
      } // end of method xs2@25::.cctor

    } // end of class xs2@25

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f1() cil managed
    {
      // Code size       59 (0x3b)
      .maxstack  4
      .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0)
      .line 6,9 : 9,19 ''
      IL_0000:  nop
      .line 6,6 : 11,26 ''
      IL_0001:  ldstr      "hello"
      IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0010:  pop
      IL_0011:  ldloca.s   V_0
      .line 7,7 : 17,18 ''
      IL_0013:  ldc.i4.1
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      .line 8,8 : 11,28 ''
      IL_001a:  ldstr      "goodbye"
      IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0029:  pop
      IL_002a:  ldloca.s   V_0
      .line 9,9 : 17,18 ''
      IL_002c:  ldc.i4.2
      IL_002d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0032:  nop
      IL_0033:  ldloca.s   V_0
      IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_003a:  ret
    } // end of method ListExpressionSteppingTest2::f1

    .method public static class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!a,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!a,int32,int32>>> 
            f2<a>(!!a x) cil managed
    {
      // Code size       192 (0xc0)
      .maxstack  6
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!a,int32>> xs1,
               [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 'Pipe #1 input #1 at line 16',
               [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'Pipe #1 input #2 at line 16',
               [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!a,int32>> 'Pipe #1 stage #1 at line 17',
               [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!a,int32>> 'Pipe #1 stage #2 at line 18',
               [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!a,int32,int32>> xs2,
               [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 'Pipe #2 input #1 at line 22',
               [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'Pipe #2 input #2 at line 22',
               [8] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'Pipe #2 input #3 at line 22',
               [9] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!a,int32,int32>> 'Pipe #2 stage #1 at line 23',
               [10] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!a,int32,int32>> 'Pipe #2 stage #2 at line 24')
      .line 16,16 : 13,20 ''
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
      .line 16,16 : 22,28 ''
      IL_0018:  ldc.i4.0
      IL_0019:  ldc.i4.1
      IL_001a:  ldc.i4.2
      IL_001b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0020:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_002a:  stloc.2
      .line 17,17 : 16,24 ''
      IL_002b:  ldloc.1
      IL_002c:  ldloc.2
      IL_002d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!0,!!1>> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Zip<!!0,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>,
                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1>)
      IL_0032:  stloc.3
      .line 18,18 : 15,45 ''
      IL_0033:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #1 stage #2 at line 18@18'<!!a>::@_instance
      IL_0038:  ldloc.3
      IL_0039:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [mscorlib]System.Tuple`2<!!0,int32>,class [mscorlib]System.Tuple`2<!!0,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_003e:  stloc.s    'Pipe #1 stage #2 at line 18'
      .line 19,19 : 15,45 ''
      IL_0040:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs1@19<!!a>::@_instance
      IL_0045:  ldloc.s    'Pipe #1 stage #2 at line 18'
      IL_0047:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [mscorlib]System.Tuple`2<!!0,int32>,class [mscorlib]System.Tuple`2<!!0,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_004c:  stloc.0
      .line 22,22 : 13,20 ''
      IL_004d:  ldarg.0
      IL_004e:  ldarg.0
      IL_004f:  ldarg.0
      IL_0050:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::get_Empty()
      IL_0055:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_005a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_005f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0064:  stloc.s    'Pipe #2 input #1 at line 22'
      .line 22,22 : 22,28 ''
      IL_0066:  ldc.i4.0
      IL_0067:  ldc.i4.1
      IL_0068:  ldc.i4.2
      IL_0069:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_006e:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0073:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0078:  stloc.s    'Pipe #2 input #2 at line 22'
      .line 22,22 : 30,36 ''
      IL_007a:  ldc.i4.0
      IL_007b:  ldc.i4.1
      IL_007c:  ldc.i4.2
      IL_007d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0082:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0087:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_008c:  stloc.s    'Pipe #2 input #3 at line 22'
      .line 23,23 : 17,26 ''
      IL_008e:  ldloc.s    'Pipe #2 input #1 at line 22'
      IL_0090:  ldloc.s    'Pipe #2 input #2 at line 22'
      IL_0092:  ldloc.s    'Pipe #2 input #3 at line 22'
      IL_0094:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!0,!!1,!!2>> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Zip3<!!0,int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>,
                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1>,
                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!2>)
      IL_0099:  stloc.s    'Pipe #2 stage #1 at line 23'
      .line 24,24 : 15,50 ''
      IL_009b:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/'Pipe #2 stage #2 at line 24@24'<!!a>::@_instance
      IL_00a0:  ldloc.s    'Pipe #2 stage #1 at line 23'
      IL_00a2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [mscorlib]System.Tuple`3<!!0,int32,int32>,class [mscorlib]System.Tuple`3<!!0,int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_00a7:  stloc.s    'Pipe #2 stage #2 at line 24'
      .line 25,25 : 15,50 ''
      IL_00a9:  ldsfld     class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!0> class ListExpressionSteppingTest2/ListExpressionSteppingTest2/xs2@25<!!a>::@_instance
      IL_00ae:  ldloc.s    'Pipe #2 stage #2 at line 24'
      IL_00b0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<class [mscorlib]System.Tuple`3<!!0,int32,int32>,class [mscorlib]System.Tuple`3<!!0,int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_00b5:  stloc.s    xs2
      .line 27,27 : 9,17 ''
      IL_00b7:  ldloc.0
      IL_00b8:  ldloc.s    xs2
      IL_00ba:  newobj     instance void class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!a,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!a,int32,int32>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                      !1)
      IL_00bf:  ret
    } // end of method ListExpressionSteppingTest2::f2

  } // end of class ListExpressionSteppingTest2

} // end of class ListExpressionSteppingTest2

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest2>'.$ListExpressionSteppingTest2
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       14 (0xe)
    .maxstack  8
    .line 11,11 : 13,17 ''
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest2/ListExpressionSteppingTest2::f1()
    IL_0005:  pop
    .line 29,29 : 13,17 ''
    IL_0006:  ldc.i4.5
    IL_0007:  call       class [mscorlib]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!0,int32>>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`3<!!0,int32,int32>>> ListExpressionSteppingTest2/ListExpressionSteppingTest2::f2<int32>(!!0)
    IL_000c:  pop
    IL_000d:  ret
  } // end of method $ListExpressionSteppingTest2::main@

} // end of class '<StartupCode$ListExpressionSteppingTest2>'.$ListExpressionSteppingTest2


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
