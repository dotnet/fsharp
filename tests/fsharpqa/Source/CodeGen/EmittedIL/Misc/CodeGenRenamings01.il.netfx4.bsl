
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16774
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
  .ver 4:0:0:0
}
.assembly CodeGenRenamings01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CodeGenRenamings01
{
  // Offset: 0x00000000 Length: 0x000003F0
}
.mresource public FSharpOptimizationData.CodeGenRenamings01
{
  // Offset: 0x000003F8 Length: 0x0000011B
}
.module CodeGenRenamings01.exe
// MVID: {4DAC0DCC-8173-986B-A745-0383CC0DAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000180000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CodeGenRenamings01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname seq1@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<int32,int32> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 pc,
                                 class [mscorlib]System.Tuple`2<int32,int32> current) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 CodeGenRenamings01/seq1@9::pc
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CodeGenRenamings01/seq1@9::current
      IL_000e:  ldarg.0
      IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>::.ctor()
      IL_0014:  ret
    } // end of method seq1@9::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>& next) cil managed
    {
      // Code size       103 (0x67)
      .maxstack  7
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 CodeGenRenamings01/seq1@9::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002a

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0041

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_0057

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br.s       IL_005e

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.1
      IL_002d:  stfld      int32 CodeGenRenamings01/seq1@9::pc
      .line 9,9 : 18,30 
      IL_0032:  ldarg.0
      IL_0033:  ldc.i4.1
      IL_0034:  ldc.i4.1
      IL_0035:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_003a:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CodeGenRenamings01/seq1@9::current
      IL_003f:  ldc.i4.1
      IL_0040:  ret

      IL_0041:  ldarg.0
      IL_0042:  ldc.i4.2
      IL_0043:  stfld      int32 CodeGenRenamings01/seq1@9::pc
      .line 9,9 : 32,44 
      IL_0048:  ldarg.0
      IL_0049:  ldc.i4.2
      IL_004a:  ldc.i4.2
      IL_004b:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0050:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CodeGenRenamings01/seq1@9::current
      IL_0055:  ldc.i4.1
      IL_0056:  ret

      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.3
      IL_0059:  stfld      int32 CodeGenRenamings01/seq1@9::pc
      IL_005e:  ldarg.0
      IL_005f:  ldnull
      IL_0060:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CodeGenRenamings01/seq1@9::current
      IL_0065:  ldc.i4.0
      IL_0066:  ret
    } // end of method seq1@9::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.3
      IL_0003:  stfld      int32 CodeGenRenamings01/seq1@9::pc
      IL_0008:  ret
    } // end of method seq1@9::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 CodeGenRenamings01/seq1@9::pc
      IL_0007:  switch     ( 
                            IL_001e,
                            IL_0020,
                            IL_0022,
                            IL_0024)
      IL_001c:  br.s       IL_0032

      IL_001e:  br.s       IL_0026

      IL_0020:  br.s       IL_0029

      IL_0022:  br.s       IL_002c

      IL_0024:  br.s       IL_002f

      IL_0026:  nop
      IL_0027:  br.s       IL_0037

      IL_0029:  nop
      IL_002a:  br.s       IL_0035

      IL_002c:  nop
      IL_002d:  br.s       IL_0033

      IL_002f:  nop
      IL_0030:  br.s       IL_0037

      IL_0032:  nop
      IL_0033:  ldc.i4.0
      IL_0034:  ret

      IL_0035:  ldc.i4.0
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method seq1@9::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> CodeGenRenamings01/seq1@9::current
      IL_0007:  ret
    } // end of method seq1@9::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<int32,int32>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void CodeGenRenamings01/seq1@9::.ctor(int32,
                                                                          class [mscorlib]System.Tuple`2<int32,int32>)
      IL_0008:  ret
    } // end of method seq1@9::GetFreshEnumerator

  } // end of class seq1@9

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_alist() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::alist@5
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_alist

  .method public specialname static int32[] 
          get_array() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array@6
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_array

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          get_aseq() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::aseq@7
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_aseq

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> 
          get_list1() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::list1@8
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_list1

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> 
          get_seq1() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::seq1@9
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_seq1

  .method public specialname static class [mscorlib]System.Tuple`2<int32,int32>[] 
          get_array1() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<int32,int32>[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array1@10
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_array1

  .method public specialname static int32[0...,0...] 
          get_a3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a3@11
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_a3

  .method public specialname static int32[0...,0...,0...] 
          get_array3D() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array3D@12
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_array3D

  .method public specialname static int32[0...,0...,0...,0...] 
          get_array4D() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[0...,0...,0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array4D@13
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_array4D

  .method public specialname static int32[] 
          get_a1() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a1@25
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_a1

  .method public specialname static int32[] 
          get_a2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a2@26
    IL_0005:  ret
  } // end of method CodeGenRenamings01::get_a2

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          alist()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> CodeGenRenamings01::get_alist()
  } // end of property CodeGenRenamings01::alist
  .property int32[] 'array'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] CodeGenRenamings01::get_array()
  } // end of property CodeGenRenamings01::'array'
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
          aseq()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> CodeGenRenamings01::get_aseq()
  } // end of property CodeGenRenamings01::aseq
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>>
          list1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> CodeGenRenamings01::get_list1()
  } // end of property CodeGenRenamings01::list1
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>
          seq1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> CodeGenRenamings01::get_seq1()
  } // end of property CodeGenRenamings01::seq1
  .property class [mscorlib]System.Tuple`2<int32,int32>[]
          array1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<int32,int32>[] CodeGenRenamings01::get_array1()
  } // end of property CodeGenRenamings01::array1
  .property int32[0...,0...] a3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...] CodeGenRenamings01::get_a3()
  } // end of property CodeGenRenamings01::a3
  .property int32[0...,0...,0...] array3D()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
  } // end of property CodeGenRenamings01::array3D
  .property int32[0...,0...,0...,0...] array4D()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
  } // end of property CodeGenRenamings01::array4D
  .property int32[] a1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] CodeGenRenamings01::get_a1()
  } // end of property CodeGenRenamings01::a1
  .property int32[] a2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] CodeGenRenamings01::get_a2()
  } // end of property CodeGenRenamings01::a2
} // end of class CodeGenRenamings01

.class private abstract auto ansi sealed '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> alist@5
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] array@6
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> aseq@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> list1@8
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> seq1@9
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<int32,int32>[] array1@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...] a3@11
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...,0...] array3D@12
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[0...,0...,0...,0...] array4D@13
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] a1@25
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] a2@26
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       710 (0x2c6)
    .maxstack  12
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> alist,
             [1] int32[] 'array',
             [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> aseq,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> list1,
             [4] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> seq1,
             [5] class [mscorlib]System.Tuple`2<int32,int32>[] array1,
             [6] int32[0...,0...] a3,
             [7] int32[0...,0...,0...] array3D,
             [8] int32[0...,0...,0...,0...] array4D,
             [9] int32[] a1,
             [10] int32[] a2,
             [11] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_11,
             [12] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_12,
             [13] int32 V_13,
             [14] int32 V_14,
             [15] int32 V_15,
             [16] int32 V_16,
             [17] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_17,
             [18] class [mscorlib]System.Tuple`3<int32,int32,int32> V_18,
             [19] class [mscorlib]System.Tuple`3<int32,int32,int32> V_19,
             [20] int32 V_20,
             [21] int32 V_21,
             [22] int32 V_22,
             [23] class [mscorlib]System.Tuple`3<int32,int32,int32> V_23,
             [24] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_24,
             [25] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_25,
             [26] int32 V_26,
             [27] int32 V_27,
             [28] int32 V_28,
             [29] int32 V_29,
             [30] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_30)
    .line 5,5 : 1,24 
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_000a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0014:  dup
    IL_0015:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::alist@5
    IL_001a:  stloc.0
    .line 6,6 : 1,26 
    IL_001b:  ldc.i4.3
    IL_001c:  newarr     [mscorlib]System.Int32
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.1
    IL_0024:  stelem     [mscorlib]System.Int32
    IL_0029:  dup
    IL_002a:  ldc.i4.1
    IL_002b:  ldc.i4.2
    IL_002c:  stelem     [mscorlib]System.Int32
    IL_0031:  dup
    IL_0032:  ldc.i4.2
    IL_0033:  ldc.i4.3
    IL_0034:  stelem     [mscorlib]System.Int32
    IL_0039:  dup
    IL_003a:  stsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array@6
    IL_003f:  stloc.1
    .line 7,7 : 1,27 
    IL_0040:  ldc.i4.1
    IL_0041:  ldc.i4.1
    IL_0042:  ldc.i4.s   10
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0049:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_004e:  dup
    IL_004f:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::aseq@7
    IL_0054:  stloc.2
    .line 8,8 : 1,27 
    IL_0055:  ldc.i4.1
    IL_0056:  ldc.i4.1
    IL_0057:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_005c:  ldc.i4.2
    IL_005d:  ldc.i4.2
    IL_005e:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>>::get_Empty()
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>>::Cons(!0,
                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0072:  dup
    IL_0073:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::list1@8
    IL_0078:  stloc.3
    IL_0079:  ldc.i4.0
    IL_007a:  ldnull
    IL_007b:  newobj     instance void CodeGenRenamings01/seq1@9::.ctor(int32,
                                                                        class [mscorlib]System.Tuple`2<int32,int32>)
    IL_0080:  dup
    IL_0081:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::seq1@9
    IL_0086:  stloc.s    seq1
    .line 10,10 : 1,34 
    IL_0088:  ldc.i4.2
    IL_0089:  newarr     class [mscorlib]System.Tuple`2<int32,int32>
    IL_008e:  dup
    IL_008f:  ldc.i4.0
    IL_0090:  ldc.i4.1
    IL_0091:  ldc.i4.1
    IL_0092:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0097:  stelem     class [mscorlib]System.Tuple`2<int32,int32>
    IL_009c:  dup
    IL_009d:  ldc.i4.1
    IL_009e:  ldc.i4.2
    IL_009f:  ldc.i4.2
    IL_00a0:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_00a5:  stelem     class [mscorlib]System.Tuple`2<int32,int32>
    IL_00aa:  dup
    IL_00ab:  stsfld     class [mscorlib]System.Tuple`2<int32,int32>[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array1@10
    IL_00b0:  stloc.s    array1
    .line 11,11 : 1,30 
    IL_00b2:  ldc.i4.2
    IL_00b3:  ldc.i4.2
    IL_00b4:  ldc.i4.0
    IL_00b5:  call       !!0[0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Create<int32>(int32,
                                                                                                               int32,
                                                                                                               !!0)
    IL_00ba:  dup
    IL_00bb:  stsfld     int32[0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a3@11
    IL_00c0:  stloc.s    a3
    .line 12,12 : 1,37 
    IL_00c2:  ldc.i4.3
    IL_00c3:  ldc.i4.3
    IL_00c4:  ldc.i4.3
    IL_00c5:  ldc.i4.0
    IL_00c6:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Create<int32>(int32,
                                                                                                                    int32,
                                                                                                                    int32,
                                                                                                                    !!0)
    IL_00cb:  dup
    IL_00cc:  stsfld     int32[0...,0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array3D@12
    IL_00d1:  stloc.s    array3D
    .line 13,13 : 1,39 
    IL_00d3:  ldc.i4.4
    IL_00d4:  ldc.i4.4
    IL_00d5:  ldc.i4.4
    IL_00d6:  ldc.i4.4
    IL_00d7:  ldc.i4.0
    IL_00d8:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Create<int32>(int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         int32,
                                                                                                                         !!0)
    IL_00dd:  dup
    IL_00de:  stsfld     int32[0...,0...,0...,0...] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::array4D@13
    IL_00e3:  stloc.s    array4D
    .line 16,16 : 9,27 
    IL_00e5:  call       int32[] CodeGenRenamings01::get_array()
    IL_00ea:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfArray<int32>(!!0[])
    IL_00ef:  pop
    .line 17,17 : 9,24 
    IL_00f0:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> CodeGenRenamings01::get_aseq()
    IL_00f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00fa:  pop
    .line 20,20 : 9,27 
    IL_00fb:  call       class [mscorlib]System.Tuple`2<int32,int32>[] CodeGenRenamings01::get_array1()
    IL_0100:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfArray<int32,int32>(class [mscorlib]System.Tuple`2<!!0,!!1>[])
    IL_0105:  pop
    .line 21,21 : 9,25 
    IL_0106:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> CodeGenRenamings01::get_list1()
    IL_010b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfList<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<!!0,!!1>>)
    IL_0110:  pop
    .line 22,22 : 9,23 
    IL_0111:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> CodeGenRenamings01::get_seq1()
    IL_0116:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpMap`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Collections.MapModule::OfSeq<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<!!0,!!1>>)
    IL_011b:  pop
    .line 25,25 : 1,28 
    IL_011c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> CodeGenRenamings01::get_alist()
    IL_0121:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfList<int32>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0126:  dup
    IL_0127:  stsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a1@25
    IL_012c:  stloc.s    a1
    .line 26,26 : 1,27 
    IL_012e:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> CodeGenRenamings01::get_aseq()
    IL_0133:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::OfSeq<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0138:  dup
    IL_0139:  stsfld     int32[] '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01::a2@26
    IL_013e:  stloc.s    a2
    .line 27,27 : 1,33 
    IL_0140:  call       int32[] CodeGenRenamings01::get_a2()
    IL_0145:  ldc.i4.0
    IL_0146:  call       int32[] CodeGenRenamings01::get_a1()
    IL_014b:  ldc.i4.0
    IL_014c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Get<int32>(!!0[],
                                                                                               int32)
    IL_0151:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Set<int32>(!!0[],
                                                                                                int32,
                                                                                                !!0)
    IL_0156:  nop
    .line 30,30 : 1,87 
    IL_0157:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_015c:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length1<int32>(!!0[0...,0...])
    IL_0161:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_0166:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Length2<int32>(!!0[0...,0...])
    IL_016b:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_0170:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base1<int32>(!!0[0...,0...])
    IL_0175:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_017a:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Base2<int32>(!!0[0...,0...])
    IL_017f:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_0184:  stloc.s    V_11
    IL_0186:  ldloc.s    V_11
    IL_0188:  stloc.s    V_12
    IL_018a:  ldloc.s    V_12
    IL_018c:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_0191:  stloc.s    V_13
    IL_0193:  ldloc.s    V_12
    IL_0195:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_019a:  stloc.s    V_14
    IL_019c:  ldloc.s    V_12
    IL_019e:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_01a3:  stloc.s    V_15
    IL_01a5:  ldloc.s    V_12
    IL_01a7:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_01ac:  stloc.s    V_16
    IL_01ae:  ldloc.s    V_13
    IL_01b0:  ldloc.s    V_14
    IL_01b2:  ldloc.s    V_15
    IL_01b4:  ldloc.s    V_16
    IL_01b6:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_01bb:  stloc.s    V_17
    .line 31,31 : 1,41 
    IL_01bd:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_01c2:  ldc.i4.0
    IL_01c3:  ldc.i4.0
    IL_01c4:  call       int32[0...,0...] CodeGenRenamings01::get_a3()
    IL_01c9:  ldc.i4.0
    IL_01ca:  ldc.i4.0
    IL_01cb:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Get<int32>(!!0[0...,0...],
                                                                                                 int32,
                                                                                                 int32)
    IL_01d0:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array2DModule::Set<int32>(!!0[0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_01d5:  nop
    .line 34,34 : 1,86 
    IL_01d6:  call       int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
    IL_01db:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length1<int32>(!!0[0...,0...,0...])
    IL_01e0:  call       int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
    IL_01e5:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length2<int32>(!!0[0...,0...,0...])
    IL_01ea:  call       int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
    IL_01ef:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Length3<int32>(!!0[0...,0...,0...])
    IL_01f4:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_01f9:  stloc.s    V_18
    IL_01fb:  ldloc.s    V_18
    IL_01fd:  stloc.s    V_19
    IL_01ff:  ldloc.s    V_19
    IL_0201:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_0206:  stloc.s    V_20
    IL_0208:  ldloc.s    V_19
    IL_020a:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_020f:  stloc.s    V_21
    IL_0211:  ldloc.s    V_19
    IL_0213:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_0218:  stloc.s    V_22
    IL_021a:  ldloc.s    V_20
    IL_021c:  ldloc.s    V_21
    IL_021e:  ldloc.s    V_22
    IL_0220:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_0225:  stloc.s    V_23
    .line 35,35 : 1,55 
    IL_0227:  call       int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
    IL_022c:  ldc.i4.0
    IL_022d:  ldc.i4.0
    IL_022e:  ldc.i4.0
    IL_022f:  call       int32[0...,0...,0...] CodeGenRenamings01::get_array3D()
    IL_0234:  ldc.i4.0
    IL_0235:  ldc.i4.0
    IL_0236:  ldc.i4.0
    IL_0237:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Get<int32>(!!0[0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_023c:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Set<int32>(!!0[0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_0241:  nop
    .line 38,38 : 1,111 
    IL_0242:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_0247:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length1<int32>(!!0[0...,0...,0...,0...])
    IL_024c:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_0251:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length2<int32>(!!0[0...,0...,0...,0...])
    IL_0256:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_025b:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length3<int32>(!!0[0...,0...,0...,0...])
    IL_0260:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_0265:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Length4<int32>(!!0[0...,0...,0...,0...])
    IL_026a:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_026f:  stloc.s    V_24
    IL_0271:  ldloc.s    V_24
    IL_0273:  stloc.s    V_25
    IL_0275:  ldloc.s    V_25
    IL_0277:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_027c:  stloc.s    V_26
    IL_027e:  ldloc.s    V_25
    IL_0280:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_0285:  stloc.s    V_27
    IL_0287:  ldloc.s    V_25
    IL_0289:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_028e:  stloc.s    V_28
    IL_0290:  ldloc.s    V_25
    IL_0292:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_0297:  stloc.s    V_29
    IL_0299:  ldloc.s    V_26
    IL_029b:  ldloc.s    V_27
    IL_029d:  ldloc.s    V_28
    IL_029f:  ldloc.s    V_29
    IL_02a1:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_02a6:  stloc.s    V_30
    .line 39,39 : 1,59 
    IL_02a8:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_02ad:  ldc.i4.0
    IL_02ae:  ldc.i4.0
    IL_02af:  ldc.i4.0
    IL_02b0:  ldc.i4.0
    IL_02b1:  call       int32[0...,0...,0...,0...] CodeGenRenamings01::get_array4D()
    IL_02b6:  ldc.i4.0
    IL_02b7:  ldc.i4.0
    IL_02b8:  ldc.i4.0
    IL_02b9:  ldc.i4.0
    IL_02ba:  call       !!0 [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Get<int32>(!!0[0...,0...,0...,0...],
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32,
                                                                                                 int32)
    IL_02bf:  call       void [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Set<int32>(!!0[0...,0...,0...,0...],
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  int32,
                                                                                                  !!0)
    IL_02c4:  nop
    IL_02c5:  ret
  } // end of method $CodeGenRenamings01::main@

} // end of class '<StartupCode$CodeGenRenamings01>'.$CodeGenRenamings01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
