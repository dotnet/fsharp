
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17360
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
  .ver 4:3:0:0
}
.assembly extern mscorlib as mscorlib_2
{
  .publickey = (00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 ) 
  .ver 4:0:0:0
}
.assembly extern FSharp.Core as FSharp.Core_3
{
  .publickey = (00 24 00 00 04 80 00 00 94 00 00 00 06 02 00 00   // .$..............
                00 24 00 00 52 53 41 31 00 04 00 00 01 00 01 00   // .$..RSA1........
                07 D1 FA 57 C4 AE D9 F0 A3 2E 84 AA 0F AE FD 0D   // ...W............
                E9 E8 FD 6A EC 8F 87 FB 03 76 6C 83 4C 99 92 1E   // ...j.....vl.L...
                B2 3B E7 9A D9 D5 DC C1 DD 9A D2 36 13 21 02 90   // .;.........6.!..
                0B 72 3C F9 80 95 7F C4 E1 77 10 8F C6 07 77 4F   // .r<......w....wO
                29 E8 32 0E 92 EA 05 EC E4 E8 21 C0 A5 EF E8 F1   // ).2.......!.....
                64 5C 4C 0C 93 C1 AB 99 28 5D 62 2C AA 65 2C 1D   // d\L.....(]b,.e,.
                FA D6 3D 74 5D 6F 2D E5 F1 7E 5E AF 0F C4 96 3D   // ..=t]o-..~^....=
                26 1C 8A 12 43 65 18 20 6D C0 93 34 4D 5A D2 93 ) // &...Ce. m..4MZ..
  .ver 4:3:0:0
}
.assembly CallIntrinsics
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CallIntrinsics
{
  // Offset: 0x00000000 Length: 0x0000021E
}
.mresource public FSharpOptimizationData.CallIntrinsics
{
  // Offset: 0x00000228 Length: 0x00000081
}
.module CallIntrinsics.dll
// MVID: {4EBBAE0D-774B-AA86-A745-03830DAEBB4E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000000FF4C2A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CallIntrinsics
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit testIntrinsics@2
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method testIntrinsics@2::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method testIntrinsics@2::Invoke

  } // end of class testIntrinsics@2

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-1'::Invoke

  } // end of class 'testIntrinsics@2-1'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-2'::.ctor

    .method public strict virtual instance float64 
            Invoke(float64 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<float64>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-2'::Invoke

  } // end of class 'testIntrinsics@2-2'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-3'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-3'::Invoke

  } // end of class 'testIntrinsics@2-3'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-4'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-4'::Invoke

  } // end of class 'testIntrinsics@2-4'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-5'::.ctor

    .method public strict virtual instance float64 
            Invoke(float64 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<float64>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-5'::Invoke

  } // end of class 'testIntrinsics@2-5'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-6'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-6'::Invoke

  } // end of class 'testIntrinsics@2-6'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-7'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-7'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<int32>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-7'::Invoke

  } // end of class 'testIntrinsics@2-7'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-8'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-8'::.ctor

    .method public strict virtual instance float64 
            Invoke(float64 x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<float64>(!!0)
      IL_0009:  ret
    } // end of method 'testIntrinsics@2-8'::Invoke

  } // end of class 'testIntrinsics@2-8'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-9'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field public int32 start
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 step
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 start,
                                 int32 step) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 CallIntrinsics/'testIntrinsics@2-9'::start
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 CallIntrinsics/'testIntrinsics@2-9'::step
      IL_0014:  ret
    } // end of method 'testIntrinsics@2-9'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 stop) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-9'::start
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-9'::step
      IL_000d:  ldarg.1
      IL_000e:  tail.
      IL_0010:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0015:  ret
    } // end of method 'testIntrinsics@2-9'::Invoke

  } // end of class 'testIntrinsics@2-9'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-10'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`4<int32,int32,int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`4<int32,int32,int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-10'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 _arg3,
                   int32 _arg2,
                   int32 _arg1) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ret
    } // end of method 'testIntrinsics@2-10'::Invoke

  } // end of class 'testIntrinsics@2-10'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-11'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,int32,int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,int32,int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-11'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 _arg7,
                   int32 _arg6,
                   int32 _arg5,
                   int32 _arg4) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ret
    } // end of method 'testIntrinsics@2-11'::Invoke

  } // end of class 'testIntrinsics@2-11'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-12'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-12'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            Invoke(int32 x) cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0007:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_000c:  ret
    } // end of method 'testIntrinsics@2-12'::Invoke

  } // end of class 'testIntrinsics@2-12'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-13'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<object>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<object>>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-13'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<object> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit _arg8) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  tail.
      IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Empty<object>()
      IL_0008:  ret
    } // end of method 'testIntrinsics@2-13'::Invoke

  } // end of class 'testIntrinsics@2-13'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'testIntrinsics@2-14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 i
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<int32,int32> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 i,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`2<int32,int32> current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-14'::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>::.ctor()
      IL_0023:  ret
    } // end of method 'testIntrinsics@2-14'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>& next) cil managed
    {
      // Code size       194 (0xc2)
      .maxstack  8
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0098

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_008e

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br         IL_00b9

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002d:  nop
      .line 2,2 : 5,55 
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.0
      IL_0030:  ldc.i4.1
      IL_0031:  ldc.i4.s   10
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      .line 2,2 : 5,55 
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0098

      IL_0056:  ldarg.0
      IL_0057:  ldarg.0
      IL_0058:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_005d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0062:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.2
      IL_0069:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      .line 2,2 : 5,55 
      IL_006e:  ldarg.0
      IL_006f:  ldarg.0
      IL_0070:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      IL_0075:  ldarg.0
      IL_0076:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      IL_007b:  ldarg.0
      IL_007c:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      IL_0081:  mul
      IL_0082:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0087:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-14'::current
      IL_008c:  ldc.i4.1
      IL_008d:  ret

      .line 2,2 : 5,55 
      IL_008e:  ldarg.0
      IL_008f:  ldc.i4.0
      IL_0090:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::i
      .line 100001,100001 : 0,0 
      IL_0095:  nop
      IL_0096:  br.s       IL_0049

      IL_0098:  ldarg.0
      IL_0099:  ldc.i4.3
      IL_009a:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      .line 2,2 : 5,55 
      IL_009f:  ldarg.0
      IL_00a0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_00a5:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00aa:  nop
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
      IL_00b2:  ldarg.0
      IL_00b3:  ldc.i4.3
      IL_00b4:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      IL_00b9:  ldarg.0
      IL_00ba:  ldnull
      IL_00bb:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-14'::current
      IL_00c0:  ldc.i4.0
      IL_00c1:  ret
    } // end of method 'testIntrinsics@2-14'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004d:  nop
        .line 100001,100001 : 0,0 
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-14'::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-14'::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 2,2 : 5,55 
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 
      IL_0095:  ret
    } // end of method 'testIntrinsics@2-14'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-14'::pc
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
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method 'testIntrinsics@2-14'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-14'::current
      IL_0007:  ret
    } // end of method 'testIntrinsics@2-14'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<int32,int32>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void CallIntrinsics/'testIntrinsics@2-14'::.ctor(int32,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     class [mscorlib]System.Tuple`2<int32,int32>)
      IL_000a:  ret
    } // end of method 'testIntrinsics@2-14'::GetFreshEnumerator

  } // end of class 'testIntrinsics@2-14'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'testIntrinsics@2-15'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 i
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<int32,int32> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 i,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`2<int32,int32> current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-15'::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>::.ctor()
      IL_0023:  ret
    } // end of method 'testIntrinsics@2-15'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>& next) cil managed
    {
      // Code size       194 (0xc2)
      .maxstack  8
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0098

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_008e

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br         IL_00b9

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002d:  nop
      .line 2,2 : 5,55 
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.0
      IL_0030:  ldc.i4.1
      IL_0031:  ldc.i4.s   10
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      .line 2,2 : 5,55 
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0098

      IL_0056:  ldarg.0
      IL_0057:  ldarg.0
      IL_0058:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_005d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0062:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.2
      IL_0069:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      .line 2,2 : 5,55 
      IL_006e:  ldarg.0
      IL_006f:  ldarg.0
      IL_0070:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      IL_0075:  ldarg.0
      IL_0076:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      IL_007b:  ldarg.0
      IL_007c:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      IL_0081:  mul
      IL_0082:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0087:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-15'::current
      IL_008c:  ldc.i4.1
      IL_008d:  ret

      .line 2,2 : 5,55 
      IL_008e:  ldarg.0
      IL_008f:  ldc.i4.0
      IL_0090:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::i
      .line 100001,100001 : 0,0 
      IL_0095:  nop
      IL_0096:  br.s       IL_0049

      IL_0098:  ldarg.0
      IL_0099:  ldc.i4.3
      IL_009a:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      .line 2,2 : 5,55 
      IL_009f:  ldarg.0
      IL_00a0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_00a5:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00aa:  nop
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
      IL_00b2:  ldarg.0
      IL_00b3:  ldc.i4.3
      IL_00b4:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      IL_00b9:  ldarg.0
      IL_00ba:  ldnull
      IL_00bb:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-15'::current
      IL_00c0:  ldc.i4.0
      IL_00c1:  ret
    } // end of method 'testIntrinsics@2-15'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004d:  nop
        .line 100001,100001 : 0,0 
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-15'::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-15'::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 2,2 : 5,55 
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 
      IL_0095:  ret
    } // end of method 'testIntrinsics@2-15'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-15'::pc
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
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method 'testIntrinsics@2-15'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-15'::current
      IL_0007:  ret
    } // end of method 'testIntrinsics@2-15'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<int32,int32>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void CallIntrinsics/'testIntrinsics@2-15'::.ctor(int32,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     class [mscorlib]System.Tuple`2<int32,int32>)
      IL_000a:  ret
    } // end of method 'testIntrinsics@2-15'::GetFreshEnumerator

  } // end of class 'testIntrinsics@2-15'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'testIntrinsics@2-16'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 i
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<int32,int32> current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 i,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [mscorlib]System.Tuple`2<int32,int32> current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-16'::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [mscorlib]System.Tuple`2<int32,int32>>::.ctor()
      IL_0023:  ret
    } // end of method 'testIntrinsics@2-16'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>& next) cil managed
    {
      // Code size       194 (0xc2)
      .maxstack  8
      .line 100001,100001 : 0,0 
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0021:  nop
      IL_0022:  br.s       IL_0098

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0024:  nop
      IL_0025:  br.s       IL_008e

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0027:  nop
      IL_0028:  br         IL_00b9

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_002d:  nop
      .line 2,2 : 5,55 
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.0
      IL_0030:  ldc.i4.1
      IL_0031:  ldc.i4.s   10
      IL_0033:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0038:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      .line 2,2 : 5,55 
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_004f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0098

      IL_0056:  ldarg.0
      IL_0057:  ldarg.0
      IL_0058:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_005d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0062:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.2
      IL_0069:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      .line 2,2 : 5,55 
      IL_006e:  ldarg.0
      IL_006f:  ldarg.0
      IL_0070:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      IL_0075:  ldarg.0
      IL_0076:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      IL_007b:  ldarg.0
      IL_007c:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      IL_0081:  mul
      IL_0082:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0087:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-16'::current
      IL_008c:  ldc.i4.1
      IL_008d:  ret

      .line 2,2 : 5,55 
      IL_008e:  ldarg.0
      IL_008f:  ldc.i4.0
      IL_0090:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::i
      .line 100001,100001 : 0,0 
      IL_0095:  nop
      IL_0096:  br.s       IL_0049

      IL_0098:  ldarg.0
      IL_0099:  ldc.i4.3
      IL_009a:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      .line 2,2 : 5,55 
      IL_009f:  ldarg.0
      IL_00a0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_00a5:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00aa:  nop
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
      IL_00b2:  ldarg.0
      IL_00b3:  ldc.i4.3
      IL_00b4:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      IL_00b9:  ldarg.0
      IL_00ba:  ldnull
      IL_00bb:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-16'::current
      IL_00c0:  ldc.i4.0
      IL_00c1:  ret
    } // end of method 'testIntrinsics@2-16'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 
        .line 100001,100001 : 0,0 
        IL_004d:  nop
        .line 100001,100001 : 0,0 
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> CallIntrinsics/'testIntrinsics@2-16'::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-16'::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 2,2 : 5,55 
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 
      IL_0095:  ret
    } // end of method 'testIntrinsics@2-16'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 CallIntrinsics/'testIntrinsics@2-16'::pc
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
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method 'testIntrinsics@2-16'::get_CheckClose

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32> 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> CallIntrinsics/'testIntrinsics@2-16'::current
      IL_0007:  ret
    } // end of method 'testIntrinsics@2-16'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Tuple`2<int32,int32>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  ldc.i4.0
      IL_0004:  ldnull
      IL_0005:  newobj     instance void CallIntrinsics/'testIntrinsics@2-16'::.ctor(int32,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                     int32,
                                                                                     class [mscorlib]System.Tuple`2<int32,int32>)
      IL_000a:  ret
    } // end of method 'testIntrinsics@2-16'::GetFreshEnumerator

  } // end of class 'testIntrinsics@2-16'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-17'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string> clo1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string> clo1) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string> CallIntrinsics/'testIntrinsics@2-17'::clo1
      IL_000d:  ret
    } // end of method 'testIntrinsics@2-17'::.ctor

    .method public strict virtual instance string 
            Invoke(int32 arg10) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string> CallIntrinsics/'testIntrinsics@2-17'::clo1
      IL_0007:  ldarg.1
      IL_0008:  tail.
      IL_000a:  callvirt   instance !1 class [FSharp.Core_3]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::Invoke(!0)
      IL_000f:  ret
    } // end of method 'testIntrinsics@2-17'::Invoke

  } // end of class 'testIntrinsics@2-17'

  .class auto ansi serializable nested assembly beforefieldinit 'testIntrinsics@2-18'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'testIntrinsics@2-18'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 2,2 : 5,55 
      IL_0000:  nop
      IL_0001:  ldc.i4.7
      IL_0002:  ret
    } // end of method 'testIntrinsics@2-18'::Invoke

  } // end of class 'testIntrinsics@2-18'

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<object> 
          testIntrinsics() cil managed
  {
    // Code size       12462 (0x30ae)
    .maxstack  98
    .locals init ([0] bool V_0,
             [1] bool V_1,
             [2] bool V_2,
             [3] bool V_3,
             [4] bool V_4,
             [5] bool V_5,
             [6] bool V_6,
             [7] bool V_7,
             [8] int32 V_8,
             [9] bool V_9,
             [10] bool V_10,
             [11] int32 V_11,
             [12] bool V_12,
             [13] bool V_13,
             [14] valuetype [mscorlib]System.Nullable`1<int32> V_14,
             [15] int32 V_15,
             [16] valuetype [mscorlib]System.Nullable`1<int32> V_16,
             [17] valuetype [mscorlib]System.Nullable`1<int32> V_17,
             [18] valuetype [mscorlib]System.Nullable`1<int32> V_18,
             [19] valuetype [mscorlib]System.Nullable`1<int32> V_19,
             [20] valuetype [mscorlib]System.Nullable`1<int32> V_20,
             [21] int32 V_21,
             [22] valuetype [mscorlib]System.Nullable`1<int32> V_22,
             [23] valuetype [mscorlib]System.Nullable`1<int32> V_23,
             [24] valuetype [mscorlib]System.Nullable`1<int32> V_24,
             [25] int32 V_25,
             [26] valuetype [mscorlib]System.Nullable`1<int32> V_26,
             [27] valuetype [mscorlib]System.Nullable`1<int32> V_27,
             [28] valuetype [mscorlib]System.Nullable`1<int32> V_28,
             [29] valuetype [mscorlib]System.Nullable`1<int32> V_29,
             [30] valuetype [mscorlib]System.Nullable`1<int32> V_30,
             [31] int32 V_31,
             [32] valuetype [mscorlib]System.Nullable`1<int32> V_32,
             [33] valuetype [mscorlib]System.Nullable`1<int32> V_33,
             [34] bool V_34,
             [35] bool V_35,
             [36] bool V_36,
             [37] bool V_37,
             [38] bool V_38,
             [39] bool V_39,
             [40] bool V_40,
             [41] bool V_41,
             [42] bool V_42,
             [43] bool V_43,
             [44] bool V_44,
             [45] bool V_45,
             [46] bool V_46,
             [47] bool V_47,
             [48] bool V_48,
             [49] bool V_49,
             [50] int32 V_50,
             [51] class [mscorlib]System.Collections.IComparer V_51,
             [52] int32 V_52,
             [53] int32 V_53,
             [54] int32 V_54,
             [55] int32 V_55,
             [56] class [mscorlib]System.Collections.IEqualityComparer V_56,
             [57] class [mscorlib]System.Tuple`2<int32,int32> V_57,
             [58] int32 V_58,
             [59] int32 V_59,
             [60] int32 V_60,
             [61] int32 V_61,
             [62] int32 V_62,
             [63] class [mscorlib]System.Collections.IEqualityComparer V_63,
             [64] class [mscorlib]System.Tuple`3<int32,int32,int32> V_64,
             [65] int32 V_65,
             [66] int32 V_66,
             [67] int32 V_67,
             [68] int32 V_68,
             [69] int32 V_69,
             [70] int32 V_70,
             [71] int32 V_71,
             [72] class [mscorlib]System.Collections.IEqualityComparer V_72,
             [73] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_73,
             [74] int32 V_74,
             [75] int32 V_75,
             [76] int32 V_76,
             [77] int32 V_77,
             [78] int32 V_78,
             [79] int32 V_79,
             [80] int32 V_80,
             [81] int32 V_81,
             [82] int32 V_82,
             [83] class [mscorlib]System.Collections.IEqualityComparer V_83,
             [84] class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32> V_84,
             [85] int32 V_85,
             [86] int32 V_86,
             [87] int32 V_87,
             [88] int32 V_88,
             [89] int32 V_89,
             [90] int32 V_90,
             [91] int32 V_91,
             [92] int32 V_92,
             [93] int32 V_93,
             [94] int32 V_94,
             [95] bool V_95,
             [96] class [mscorlib]System.Collections.IEqualityComparer V_96,
             [97] class [mscorlib]System.Tuple`2<int32,int32> V_97,
             [98] class [mscorlib]System.Tuple`2<int32,int32> V_98,
             [99] int32 V_99,
             [100] int32 V_100,
             [101] int32 V_101,
             [102] int32 V_102,
             [103] bool V_103,
             [104] bool V_104,
             [105] class [mscorlib]System.Collections.IEqualityComparer V_105,
             [106] class [mscorlib]System.Tuple`3<int32,int32,int32> V_106,
             [107] class [mscorlib]System.Tuple`3<int32,int32,int32> V_107,
             [108] int32 V_108,
             [109] int32 V_109,
             [110] int32 V_110,
             [111] int32 V_111,
             [112] int32 V_112,
             [113] int32 V_113,
             [114] bool V_114,
             [115] bool V_115,
             [116] class [mscorlib]System.Collections.IEqualityComparer V_116,
             [117] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_117,
             [118] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_118,
             [119] int32 V_119,
             [120] int32 V_120,
             [121] int32 V_121,
             [122] int32 V_122,
             [123] int32 V_123,
             [124] int32 V_124,
             [125] int32 V_125,
             [126] int32 V_126,
             [127] bool V_127,
             [128] bool V_128,
             [129] class [mscorlib]System.Collections.IEqualityComparer V_129,
             [130] class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32> V_130,
             [131] class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32> V_131,
             [132] int32 V_132,
             [133] int32 V_133,
             [134] int32 V_134,
             [135] int32 V_135,
             [136] int32 V_136,
             [137] int32 V_137,
             [138] int32 V_138,
             [139] int32 V_139,
             [140] int32 V_140,
             [141] int32 V_141,
             [142] bool V_142,
             [143] int32 V_143,
             [144] class [mscorlib]System.Collections.IComparer V_144,
             [145] class [mscorlib]System.Tuple`2<int32,int32> V_145,
             [146] class [mscorlib]System.Tuple`2<int32,int32> V_146,
             [147] int32 V_147,
             [148] int32 V_148,
             [149] int32 V_149,
             [150] int32 V_150,
             [151] int32 V_151,
             [152] int32 V_152,
             [153] int32 V_153,
             [154] class [mscorlib]System.Collections.IComparer V_154,
             [155] class [mscorlib]System.Tuple`3<int32,int32,int32> V_155,
             [156] class [mscorlib]System.Tuple`3<int32,int32,int32> V_156,
             [157] int32 V_157,
             [158] int32 V_158,
             [159] int32 V_159,
             [160] int32 V_160,
             [161] int32 V_161,
             [162] int32 V_162,
             [163] int32 V_163,
             [164] int32 V_164,
             [165] int32 V_165,
             [166] int32 V_166,
             [167] class [mscorlib]System.Collections.IComparer V_167,
             [168] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_168,
             [169] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_169,
             [170] int32 V_170,
             [171] int32 V_171,
             [172] int32 V_172,
             [173] int32 V_173,
             [174] int32 V_174,
             [175] int32 V_175,
             [176] int32 V_176,
             [177] int32 V_177,
             [178] int32 V_178,
             [179] int32 V_179,
             [180] int32 V_180,
             [181] int32 V_181,
             [182] int32 V_182,
             [183] class [mscorlib]System.Collections.IComparer V_183,
             [184] class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32> V_184,
             [185] class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32> V_185,
             [186] int32 V_186,
             [187] int32 V_187,
             [188] int32 V_188,
             [189] int32 V_189,
             [190] int32 V_190,
             [191] int32 V_191,
             [192] int32 V_192,
             [193] int32 V_193,
             [194] int32 V_194,
             [195] int32 V_195,
             [196] int32 V_196,
             [197] int32 V_197,
             [198] int32 V_198,
             [199] int32 V_199,
             [200] int32 V_200,
             [201] int32 V_201,
             [202] int32 V_202,
             [203] int32 V_203,
             [204] int32 V_204,
             [205] int32 V_205,
             [206] int32 V_206,
             [207] int32 V_207,
             [208] int32 V_208,
             [209] int32 V_209,
             [210] int32 V_210,
             [211] int32 V_211,
             [212] int32 V_212,
             [213] int32 V_213,
             [214] int32 V_214,
             [215] int32 V_215,
             [216] int32 V_216,
             [217] int32 V_217,
             [218] int32 V_218,
             [219] int32 V_219,
             [220] int32 V_220,
             [221] int32 V_221,
             [222] int32 V_222,
             [223] int32 V_223,
             [224] int32 V_224,
             [225] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_225,
             [226] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_226,
             [227] int32 V_227,
             [228] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_228,
             [229] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_229,
             [230] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_230,
             [231] object V_231,
             [232] object V_232,
             [233] object V_233,
             [234] object V_234,
             [235] object V_235,
             [236] object V_236,
             [237] object V_237,
             [238] object V_238,
             [239] object V_239,
             [240] object V_240,
             [241] object V_241,
             [242] object V_242,
             [243] object V_243,
             [244] object V_244,
             [245] object V_245,
             [246] object V_246,
             [247] object V_247,
             [248] object V_248,
             [249] object V_249,
             [250] object V_250,
             [251] object V_251,
             [252] object V_252,
             [253] object V_253,
             [254] object V_254,
             [255] object V_255,
             [256] object V_256,
             [257] object V_257,
             [258] object V_258,
             [259] object V_259,
             [260] object V_260,
             [261] object V_261,
             [262] object V_262,
             [263] object V_263,
             [264] object V_264,
             [265] object V_265,
             [266] object V_266,
             [267] object V_267,
             [268] object V_268,
             [269] object V_269,
             [270] object V_270,
             [271] object V_271,
             [272] object V_272,
             [273] int32 V_273,
             [274] int32 V_274,
             [275] class [mscorlib]System.IDisposable V_275,
             [276] int32 V_276,
             [277] int32 V_277,
             [278] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_278,
             [279] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_279,
             [280] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_280,
             [281] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_281,
             [282] object V_282,
             [283] object V_283,
             [284] object V_284,
             [285] object V_285,
             [286] object V_286,
             [287] object V_287,
             [288] object V_288,
             [289] object V_289,
             [290] object V_290,
             [291] object V_291,
             [292] object V_292,
             [293] object V_293,
             [294] object V_294,
             [295] object V_295,
             [296] object V_296,
             [297] object V_297,
             [298] object V_298,
             [299] object V_299,
             [300] object V_300,
             [301] object V_301,
             [302] object V_302,
             [303] object V_303,
             [304] object V_304,
             [305] object V_305,
             [306] object V_306,
             [307] object V_307,
             [308] object V_308,
             [309] object V_309,
             [310] object V_310,
             [311] object V_311,
             [312] object V_312,
             [313] object V_313,
             [314] object V_314,
             [315] object V_315,
             [316] object V_316,
             [317] object V_317,
             [318] object V_318,
             [319] object V_319,
             [320] object V_320,
             [321] object V_321,
             [322] object V_322,
             [323] object V_323,
             [324] object V_324,
             [325] int32 V_325,
             [326] int32 V_326,
             [327] class [mscorlib]System.IDisposable V_327,
             [328] int32 V_328,
             [329] float64 V_329,
             [330] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> V_330,
             [331] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_331,
             [332] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_332,
             [333] object V_333,
             [334] object V_334,
             [335] object V_335,
             [336] object V_336,
             [337] object V_337,
             [338] object V_338,
             [339] object V_339,
             [340] object V_340,
             [341] object V_341,
             [342] object V_342,
             [343] object V_343,
             [344] object V_344,
             [345] object V_345,
             [346] object V_346,
             [347] object V_347,
             [348] object V_348,
             [349] object V_349,
             [350] object V_350,
             [351] object V_351,
             [352] object V_352,
             [353] object V_353,
             [354] object V_354,
             [355] object V_355,
             [356] object V_356,
             [357] object V_357,
             [358] object V_358,
             [359] object V_359,
             [360] object V_360,
             [361] object V_361,
             [362] object V_362,
             [363] object V_363,
             [364] object V_364,
             [365] object V_365,
             [366] object V_366,
             [367] object V_367,
             [368] object V_368,
             [369] object V_369,
             [370] object V_370,
             [371] object V_371,
             [372] object V_372,
             [373] object V_373,
             [374] object V_374,
             [375] object V_375,
             [376] object V_376,
             [377] float64 V_377,
             [378] float64 V_378,
             [379] int32 V_379,
             [380] float64 V_380,
             [381] int32 V_381,
             [382] class [mscorlib]System.IDisposable V_382,
             [383] float64 V_383,
             [384] float64 V_384,
             [385] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_385,
             [386] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> V_386,
             [387] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_387,
             [388] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_388,
             [389] object V_389,
             [390] object V_390,
             [391] object V_391,
             [392] object V_392,
             [393] object V_393,
             [394] object V_394,
             [395] object V_395,
             [396] object V_396,
             [397] object V_397,
             [398] object V_398,
             [399] object V_399,
             [400] object V_400,
             [401] object V_401,
             [402] object V_402,
             [403] object V_403,
             [404] object V_404,
             [405] object V_405,
             [406] object V_406,
             [407] object V_407,
             [408] object V_408,
             [409] object V_409,
             [410] object V_410,
             [411] object V_411,
             [412] object V_412,
             [413] object V_413,
             [414] object V_414,
             [415] object V_415,
             [416] object V_416,
             [417] object V_417,
             [418] object V_418,
             [419] object V_419,
             [420] object V_420,
             [421] object V_421,
             [422] object V_422,
             [423] object V_423,
             [424] object V_424,
             [425] object V_425,
             [426] object V_426,
             [427] object V_427,
             [428] object V_428,
             [429] object V_429,
             [430] object V_430,
             [431] object V_431,
             [432] object V_432,
             [433] object V_433,
             [434] float64 V_434,
             [435] float64 V_435,
             [436] int32 V_436,
             [437] float64 V_437,
             [438] int32 V_438,
             [439] class [mscorlib]System.IDisposable V_439,
             [440] float64 V_440,
             [441] int32[] V_441,
             [442] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_442,
             [443] int32[] V_443,
             [444] int32 V_444,
             [445] int32[] V_445,
             [446] object V_446,
             [447] object V_447,
             [448] object V_448,
             [449] object V_449,
             [450] object V_450,
             [451] object V_451,
             [452] object V_452,
             [453] object V_453,
             [454] object V_454,
             [455] object V_455,
             [456] object V_456,
             [457] object V_457,
             [458] object V_458,
             [459] object V_459,
             [460] object V_460,
             [461] object V_461,
             [462] object V_462,
             [463] object V_463,
             [464] object V_464,
             [465] object V_465,
             [466] object V_466,
             [467] object V_467,
             [468] object V_468,
             [469] object V_469,
             [470] object V_470,
             [471] object V_471,
             [472] object V_472,
             [473] object V_473,
             [474] object V_474,
             [475] object V_475,
             [476] object V_476,
             [477] object V_477,
             [478] object V_478,
             [479] object V_479,
             [480] object V_480,
             [481] object V_481,
             [482] object V_482,
             [483] object V_483,
             [484] object V_484,
             [485] object V_485,
             [486] object V_486,
             [487] object V_487,
             [488] object V_488,
             [489] object V_489,
             [490] object V_490,
             [491] object V_491,
             [492] int32 V_492,
             [493] int32 V_493,
             [494] int32[] V_494,
             [495] int32 V_495,
             [496] int32[] V_496,
             [497] int32 V_497,
             [498] object V_498,
             [499] object V_499,
             [500] object V_500,
             [501] object V_501,
             [502] object V_502,
             [503] object V_503,
             [504] object V_504,
             [505] object V_505,
             [506] object V_506,
             [507] object V_507,
             [508] object V_508,
             [509] object V_509,
             [510] object V_510,
             [511] object V_511,
             [512] object V_512,
             [513] object V_513,
             [514] object V_514,
             [515] object V_515,
             [516] object V_516,
             [517] object V_517,
             [518] object V_518,
             [519] object V_519,
             [520] object V_520,
             [521] object V_521,
             [522] object V_522,
             [523] object V_523,
             [524] object V_524,
             [525] object V_525,
             [526] object V_526,
             [527] object V_527,
             [528] object V_528,
             [529] object V_529,
             [530] object V_530,
             [531] object V_531,
             [532] object V_532,
             [533] object V_533,
             [534] object V_534,
             [535] object V_535,
             [536] object V_536,
             [537] object V_537,
             [538] object V_538,
             [539] object V_539,
             [540] object V_540,
             [541] object V_541,
             [542] object V_542,
             [543] object V_543,
             [544] object V_544,
             [545] int32 V_545,
             [546] int32 V_546,
             [547] int32 V_547,
             [548] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_548,
             [549] int32[] V_549,
             [550] int32 V_550,
             [551] object V_551,
             [552] object V_552,
             [553] object V_553,
             [554] object V_554,
             [555] object V_555,
             [556] object V_556,
             [557] object V_557,
             [558] object V_558,
             [559] object V_559,
             [560] object V_560,
             [561] object V_561,
             [562] object V_562,
             [563] object V_563,
             [564] object V_564,
             [565] object V_565,
             [566] object V_566,
             [567] object V_567,
             [568] object V_568,
             [569] object V_569,
             [570] object V_570,
             [571] object V_571,
             [572] object V_572,
             [573] object V_573,
             [574] object V_574,
             [575] object V_575,
             [576] object V_576,
             [577] object V_577,
             [578] object V_578,
             [579] object V_579,
             [580] object V_580,
             [581] object V_581,
             [582] object V_582,
             [583] object V_583,
             [584] object V_584,
             [585] object V_585,
             [586] object V_586,
             [587] object V_587,
             [588] object V_588,
             [589] object V_589,
             [590] object V_590,
             [591] object V_591,
             [592] object V_592,
             [593] object V_593,
             [594] object V_594,
             [595] object V_595,
             [596] object V_596,
             [597] object V_597,
             [598] object V_598,
             [599] int32 V_599,
             [600] int32 V_600,
             [601] float64 V_601,
             [602] float64[] V_602,
             [603] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_603,
             [604] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_604,
             [605] object V_605,
             [606] object V_606,
             [607] object V_607,
             [608] object V_608,
             [609] object V_609,
             [610] object V_610,
             [611] object V_611,
             [612] object V_612,
             [613] object V_613,
             [614] object V_614,
             [615] object V_615,
             [616] object V_616,
             [617] object V_617,
             [618] object V_618,
             [619] object V_619,
             [620] object V_620,
             [621] object V_621,
             [622] object V_622,
             [623] object V_623,
             [624] object V_624,
             [625] object V_625,
             [626] object V_626,
             [627] object V_627,
             [628] object V_628,
             [629] object V_629,
             [630] object V_630,
             [631] object V_631,
             [632] object V_632,
             [633] object V_633,
             [634] object V_634,
             [635] object V_635,
             [636] object V_636,
             [637] object V_637,
             [638] object V_638,
             [639] object V_639,
             [640] object V_640,
             [641] object V_641,
             [642] object V_642,
             [643] object V_643,
             [644] object V_644,
             [645] object V_645,
             [646] object V_646,
             [647] object V_647,
             [648] object V_648,
             [649] object V_649,
             [650] object V_650,
             [651] object V_651,
             [652] object V_652,
             [653] object V_653,
             [654] float64 V_654,
             [655] float64 V_655,
             [656] int32 V_656,
             [657] float64 V_657,
             [658] int32 V_658,
             [659] class [mscorlib]System.IDisposable V_659,
             [660] float64 V_660,
             [661] float64 V_661,
             [662] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_662,
             [663] float64[] V_663,
             [664] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_664,
             [665] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_665,
             [666] object V_666,
             [667] object V_667,
             [668] object V_668,
             [669] object V_669,
             [670] object V_670,
             [671] object V_671,
             [672] object V_672,
             [673] object V_673,
             [674] object V_674,
             [675] object V_675,
             [676] object V_676,
             [677] object V_677,
             [678] object V_678,
             [679] object V_679,
             [680] object V_680,
             [681] object V_681,
             [682] object V_682,
             [683] object V_683,
             [684] object V_684,
             [685] object V_685,
             [686] object V_686,
             [687] object V_687,
             [688] object V_688,
             [689] object V_689,
             [690] object V_690,
             [691] object V_691,
             [692] object V_692,
             [693] object V_693,
             [694] object V_694,
             [695] object V_695,
             [696] object V_696,
             [697] object V_697,
             [698] object V_698,
             [699] object V_699,
             [700] object V_700,
             [701] object V_701,
             [702] object V_702,
             [703] object V_703,
             [704] object V_704,
             [705] object V_705,
             [706] object V_706,
             [707] object V_707,
             [708] object V_708,
             [709] object V_709,
             [710] object V_710,
             [711] object V_711,
             [712] object V_712,
             [713] object V_713,
             [714] object V_714,
             [715] object V_715,
             [716] float64 V_716,
             [717] float64 V_717,
             [718] int32 V_718,
             [719] float64 V_719,
             [720] int32 V_720,
             [721] class [mscorlib]System.IDisposable V_721,
             [722] float64 V_722,
             [723] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_723,
             [724] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_724,
             [725] int32 V_725,
             [726] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_726,
             [727] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_727,
             [728] object V_728,
             [729] object V_729,
             [730] object V_730,
             [731] object V_731,
             [732] object V_732,
             [733] object V_733,
             [734] object V_734,
             [735] object V_735,
             [736] object V_736,
             [737] object V_737,
             [738] object V_738,
             [739] object V_739,
             [740] object V_740,
             [741] object V_741,
             [742] object V_742,
             [743] object V_743,
             [744] object V_744,
             [745] object V_745,
             [746] object V_746,
             [747] object V_747,
             [748] object V_748,
             [749] object V_749,
             [750] object V_750,
             [751] object V_751,
             [752] object V_752,
             [753] object V_753,
             [754] object V_754,
             [755] object V_755,
             [756] object V_756,
             [757] object V_757,
             [758] object V_758,
             [759] object V_759,
             [760] object V_760,
             [761] object V_761,
             [762] object V_762,
             [763] object V_763,
             [764] object V_764,
             [765] object V_765,
             [766] object V_766,
             [767] object V_767,
             [768] object V_768,
             [769] object V_769,
             [770] object V_770,
             [771] object V_771,
             [772] object V_772,
             [773] object V_773,
             [774] object V_774,
             [775] object V_775,
             [776] object V_776,
             [777] object V_777,
             [778] object V_778,
             [779] object V_779,
             [780] int32 V_780,
             [781] int32 V_781,
             [782] class [mscorlib]System.IDisposable V_782,
             [783] int32 V_783,
             [784] int32 V_784,
             [785] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_785,
             [786] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_786,
             [787] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_787,
             [788] object V_788,
             [789] object V_789,
             [790] object V_790,
             [791] object V_791,
             [792] object V_792,
             [793] object V_793,
             [794] object V_794,
             [795] object V_795,
             [796] object V_796,
             [797] object V_797,
             [798] object V_798,
             [799] object V_799,
             [800] object V_800,
             [801] object V_801,
             [802] object V_802,
             [803] object V_803,
             [804] object V_804,
             [805] object V_805,
             [806] object V_806,
             [807] object V_807,
             [808] object V_808,
             [809] object V_809,
             [810] object V_810,
             [811] object V_811,
             [812] object V_812,
             [813] object V_813,
             [814] object V_814,
             [815] object V_815,
             [816] object V_816,
             [817] object V_817,
             [818] object V_818,
             [819] object V_819,
             [820] object V_820,
             [821] object V_821,
             [822] object V_822,
             [823] object V_823,
             [824] object V_824,
             [825] object V_825,
             [826] object V_826,
             [827] object V_827,
             [828] object V_828,
             [829] object V_829,
             [830] object V_830,
             [831] object V_831,
             [832] object V_832,
             [833] object V_833,
             [834] object V_834,
             [835] object V_835,
             [836] object V_836,
             [837] object V_837,
             [838] object V_838,
             [839] object V_839,
             [840] object V_840,
             [841] int32 V_841,
             [842] int32 V_842,
             [843] class [mscorlib]System.IDisposable V_843,
             [844] int32 V_844,
             [845] float64 V_845,
             [846] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_846,
             [847] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_847,
             [848] object V_848,
             [849] object V_849,
             [850] object V_850,
             [851] object V_851,
             [852] object V_852,
             [853] object V_853,
             [854] object V_854,
             [855] object V_855,
             [856] object V_856,
             [857] object V_857,
             [858] object V_858,
             [859] object V_859,
             [860] object V_860,
             [861] object V_861,
             [862] object V_862,
             [863] object V_863,
             [864] object V_864,
             [865] object V_865,
             [866] object V_866,
             [867] object V_867,
             [868] object V_868,
             [869] object V_869,
             [870] object V_870,
             [871] object V_871,
             [872] object V_872,
             [873] object V_873,
             [874] object V_874,
             [875] object V_875,
             [876] object V_876,
             [877] object V_877,
             [878] object V_878,
             [879] object V_879,
             [880] object V_880,
             [881] object V_881,
             [882] object V_882,
             [883] object V_883,
             [884] object V_884,
             [885] object V_885,
             [886] object V_886,
             [887] object V_887,
             [888] object V_888,
             [889] object V_889,
             [890] object V_890,
             [891] object V_891,
             [892] object V_892,
             [893] object V_893,
             [894] object V_894,
             [895] object V_895,
             [896] object V_896,
             [897] object V_897,
             [898] object V_898,
             [899] object V_899,
             [900] object V_900,
             [901] object V_901,
             [902] float64 V_902,
             [903] float64 V_903,
             [904] int32 V_904,
             [905] float64 V_905,
             [906] int32 V_906,
             [907] class [mscorlib]System.IDisposable V_907,
             [908] float64 V_908,
             [909] float64 V_909,
             [910] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_910,
             [911] class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> V_911,
             [912] class [mscorlib]System.Collections.Generic.IEnumerator`1<float64> V_912,
             [913] object V_913,
             [914] object V_914,
             [915] object V_915,
             [916] object V_916,
             [917] object V_917,
             [918] object V_918,
             [919] object V_919,
             [920] object V_920,
             [921] object V_921,
             [922] object V_922,
             [923] object V_923,
             [924] object V_924,
             [925] object V_925,
             [926] object V_926,
             [927] object V_927,
             [928] object V_928,
             [929] object V_929,
             [930] object V_930,
             [931] object V_931,
             [932] object V_932,
             [933] object V_933,
             [934] object V_934,
             [935] object V_935,
             [936] object V_936,
             [937] object V_937,
             [938] object V_938,
             [939] object V_939,
             [940] object V_940,
             [941] object V_941,
             [942] object V_942,
             [943] object V_943,
             [944] object V_944,
             [945] object V_945,
             [946] object V_946,
             [947] object V_947,
             [948] object V_948,
             [949] object V_949,
             [950] object V_950,
             [951] object V_951,
             [952] object V_952,
             [953] object V_953,
             [954] object V_954,
             [955] object V_955,
             [956] object V_956,
             [957] object V_957,
             [958] object V_958,
             [959] object V_959,
             [960] object V_960,
             [961] object V_961,
             [962] object V_962,
             [963] object V_963,
             [964] object V_964,
             [965] object V_965,
             [966] object V_966,
             [967] object V_967,
             [968] float64 V_968,
             [969] float64 V_969,
             [970] int32 V_970,
             [971] float64 V_971,
             [972] int32 V_972,
             [973] class [mscorlib]System.IDisposable V_973,
             [974] float64 V_974,
             [975] int32 V_975,
             [976] int32 V_976,
             [977] float64 V_977,
             [978] float64 V_978,
             [979] float64 V_979,
             [980] float64 V_980,
             [981] float64 V_981,
             [982] float64 V_982,
             [983] float64 V_983,
             [984] float64 V_984,
             [985] float64 V_985,
             [986] float64 V_986,
             [987] float32 V_987,
             [988] float32 V_988,
             [989] float32 V_989,
             [990] float32 V_990,
             [991] float32 V_991,
             [992] float32 V_992,
             [993] float32 V_993,
             [994] float32 V_994,
             [995] float32 V_995,
             [996] float32 V_996,
             [997] int32 V_997,
             [998] int32 V_998,
             [999] int32 V_999,
             [1000] int32 V_1000,
             [1001] int32 V_1001,
             [1002] int32 V_1002,
             [1003] int32 V_1003,
             [1004] int32 V_1004,
             [1005] bool V_1005,
             [1006] bool V_1006,
             [1007] class [mscorlib]System.Type V_1007,
             [1008] class [mscorlib]System.Type V_1008,
             [1009] int32 V_1009,
             [1010] int32 V_1010,
             [1011] int32 V_1011,
             [1012] int32 V_1012,
             [1013] class [mscorlib]System.Type V_1013,
             [1014] class [mscorlib]System.Type V_1014,
             [1015] class [mscorlib]System.Type V_1015,
             [1016] valuetype [mscorlib]System.DayOfWeek V_1016,
             [1017] valuetype [mscorlib]System.DayOfWeek V_1017,
             [1018] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1018,
             [1019] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1019,
             [1020] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>> V_1020,
             [1021] int32 V_1021,
             [1022] int32 V_1022,
             [1023] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>> V_1023,
             [1024] int32 V_1024,
             [1025] int32 V_1025,
             [1026] int32 V_1026,
             [1027] int32 V_1027,
             [1028] int32 V_1028,
             [1029] int32 V_1029,
             [1030] int32 V_1030,
             [1031] int32 V_1031,
             [1032] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1032,
             [1033] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1033,
             [1034] class [mscorlib]System.Collections.Generic.IEnumerable`1<object> V_1034,
             [1035] class [mscorlib]System.Collections.Generic.IEnumerable`1<object> V_1035,
             [1036] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1036,
             [1037] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1037,
             [1038] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> V_1038,
             [1039] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>> V_1039,
             [1040] class [mscorlib]System.Tuple`2<int32,int32>[] V_1040,
             [1041] class [mscorlib]System.Tuple`2<int32,int32>[] V_1041,
             [1042] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> V_1042,
             [1043] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>> V_1043,
             [1044] string V_1044,
             [1045] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string> V_1045,
             [1046] string V_1046,
             [1047] class [mscorlib]System.Lazy`1<int32> V_1047,
             [1048] class [mscorlib]System.Lazy`1<int32> V_1048,
             [1049] valuetype [mscorlib]System.Decimal V_1049,
             [1050] valuetype [mscorlib]System.Decimal V_1050)
    .line 2,2 : 5,55 
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  brfalse.s  IL_0006

    IL_0004:  br.s       IL_0008

    IL_0006:  br.s       IL_000c

    .line 100001,100001 : 0,0 
    IL_0008:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0009:  nop
    IL_000a:  br.s       IL_000e

    .line 100001,100001 : 0,0 
    IL_000c:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_000d:  nop
    .line 100001,100001 : 0,0 
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  box        [mscorlib]System.Boolean
    IL_0017:  ldc.i4.1
    IL_0018:  brfalse.s  IL_001c

    IL_001a:  br.s       IL_001e

    IL_001c:  br.s       IL_0022

    .line 100001,100001 : 0,0 
    IL_001e:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_001f:  nop
    IL_0020:  br.s       IL_0024

    .line 100001,100001 : 0,0 
    IL_0022:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0023:  nop
    .line 100001,100001 : 0,0 
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  stloc.3
    IL_0027:  ldloc.3
    IL_0028:  box        [mscorlib]System.Boolean
    IL_002d:  ldc.i4.1
    .line 100001,100001 : 0,0 
    IL_002e:  nop
    .line 100001,100001 : 0,0 
    IL_002f:  stloc.s    V_4
    IL_0031:  ldloc.s    V_4
    IL_0033:  stloc.s    V_5
    IL_0035:  ldloc.s    V_5
    IL_0037:  box        [mscorlib]System.Boolean
    IL_003c:  ldc.i4.1
    .line 100001,100001 : 0,0 
    IL_003d:  nop
    .line 100001,100001 : 0,0 
    IL_003e:  stloc.s    V_6
    IL_0040:  ldloc.s    V_6
    IL_0042:  stloc.s    V_7
    IL_0044:  ldloc.s    V_7
    IL_0046:  box        [mscorlib]System.Boolean
    IL_004b:  ldc.i4.1
    IL_004c:  stloc.s    V_9
    IL_004e:  ldc.i4.0
    IL_004f:  stloc.s    V_10
    IL_0051:  ldloc.s    V_9
    IL_0053:  ldloc.s    V_10
    IL_0055:  bge.s      IL_0059

    IL_0057:  br.s       IL_005b

    IL_0059:  br.s       IL_005f

    .line 100001,100001 : 0,0 
    IL_005b:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_005c:  nop
    IL_005d:  br.s       IL_0066

    .line 100001,100001 : 0,0 
    IL_005f:  ldloc.s    V_9
    IL_0061:  ldloc.s    V_10
    IL_0063:  cgt
    .line 100001,100001 : 0,0 
    IL_0065:  nop
    .line 100001,100001 : 0,0 
    IL_0066:  stloc.s    V_8
    IL_0068:  ldloc.s    V_8
    IL_006a:  stloc.s    V_11
    IL_006c:  ldloc.s    V_11
    IL_006e:  box        [mscorlib]System.Int32
    IL_0073:  ldc.i4.0
    IL_0074:  stloc.s    V_12
    IL_0076:  ldloc.s    V_12
    IL_0078:  stloc.s    V_13
    IL_007a:  ldloc.s    V_13
    IL_007c:  box        [mscorlib]System.Boolean
    IL_0081:  ldc.i4.3
    IL_0082:  stloc.s    V_15
    IL_0084:  ldc.i4.4
    IL_0085:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<int32>::.ctor(!0)
    IL_008a:  stloc.s    V_16
    IL_008c:  ldloca.s   V_16
    IL_008e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0093:  brfalse.s  IL_0097

    IL_0095:  br.s       IL_0099

    IL_0097:  br.s       IL_00ab

    .line 100001,100001 : 0,0 
    IL_0099:  ldloc.s    V_15
    IL_009b:  ldloca.s   V_16
    IL_009d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::get_Value()
    IL_00a2:  add
    IL_00a3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    .line 100001,100001 : 0,0 
    IL_00a8:  nop
    IL_00a9:  br.s       IL_00b6

    .line 100001,100001 : 0,0 
    IL_00ab:  ldloca.s   V_17
    IL_00ad:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00b3:  ldloc.s    V_17
    .line 100001,100001 : 0,0 
    IL_00b5:  nop
    .line 100001,100001 : 0,0 
    IL_00b6:  stloc.s    V_14
    IL_00b8:  ldloc.s    V_14
    IL_00ba:  stloc.s    V_18
    IL_00bc:  ldloc.s    V_18
    IL_00be:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c3:  ldc.i4.3
    IL_00c4:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<int32>::.ctor(!0)
    IL_00c9:  stloc.s    V_20
    IL_00cb:  ldc.i4.4
    IL_00cc:  stloc.s    V_21
    IL_00ce:  ldloca.s   V_20
    IL_00d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d5:  brfalse.s  IL_00d9

    IL_00d7:  br.s       IL_00db

    IL_00d9:  br.s       IL_00ed

    .line 100001,100001 : 0,0 
    IL_00db:  ldloca.s   V_20
    IL_00dd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::get_Value()
    IL_00e2:  ldloc.s    V_21
    IL_00e4:  add
    IL_00e5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    .line 100001,100001 : 0,0 
    IL_00ea:  nop
    IL_00eb:  br.s       IL_00f8

    .line 100001,100001 : 0,0 
    IL_00ed:  ldloca.s   V_22
    IL_00ef:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f5:  ldloc.s    V_22
    .line 100001,100001 : 0,0 
    IL_00f7:  nop
    .line 100001,100001 : 0,0 
    IL_00f8:  stloc.s    V_19
    IL_00fa:  ldloc.s    V_19
    IL_00fc:  stloc.s    V_23
    IL_00fe:  ldloc.s    V_23
    IL_0100:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0105:  ldc.i4.3
    IL_0106:  stloc.s    V_25
    IL_0108:  ldc.i4.4
    IL_0109:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<int32>::.ctor(!0)
    IL_010e:  stloc.s    V_26
    IL_0110:  ldloca.s   V_26
    IL_0112:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0117:  brfalse.s  IL_011b

    IL_0119:  br.s       IL_011d

    IL_011b:  br.s       IL_012f

    .line 100001,100001 : 0,0 
    IL_011d:  ldloc.s    V_25
    IL_011f:  ldloca.s   V_26
    IL_0121:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::get_Value()
    IL_0126:  mul
    IL_0127:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    .line 100001,100001 : 0,0 
    IL_012c:  nop
    IL_012d:  br.s       IL_013a

    .line 100001,100001 : 0,0 
    IL_012f:  ldloca.s   V_27
    IL_0131:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0137:  ldloc.s    V_27
    .line 100001,100001 : 0,0 
    IL_0139:  nop
    .line 100001,100001 : 0,0 
    IL_013a:  stloc.s    V_24
    IL_013c:  ldloc.s    V_24
    IL_013e:  stloc.s    V_28
    IL_0140:  ldloc.s    V_28
    IL_0142:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0147:  ldc.i4.3
    IL_0148:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<int32>::.ctor(!0)
    IL_014d:  stloc.s    V_30
    IL_014f:  ldc.i4.4
    IL_0150:  stloc.s    V_31
    IL_0152:  ldloca.s   V_30
    IL_0154:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0159:  brfalse.s  IL_015d

    IL_015b:  br.s       IL_015f

    IL_015d:  br.s       IL_0171

    .line 100001,100001 : 0,0 
    IL_015f:  ldloca.s   V_30
    IL_0161:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::get_Value()
    IL_0166:  ldloc.s    V_31
    IL_0168:  mul
    IL_0169:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    .line 100001,100001 : 0,0 
    IL_016e:  nop
    IL_016f:  br.s       IL_017c

    .line 100001,100001 : 0,0 
    IL_0171:  ldloca.s   V_32
    IL_0173:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0179:  ldloc.s    V_32
    .line 100001,100001 : 0,0 
    IL_017b:  nop
    .line 100001,100001 : 0,0 
    IL_017c:  stloc.s    V_29
    IL_017e:  ldloc.s    V_29
    IL_0180:  stloc.s    V_33
    IL_0182:  ldloc.s    V_33
    IL_0184:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0189:  ldc.i4.1
    IL_018a:  ldc.i4.0
    IL_018b:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<bool>::.ctor(!0)
    IL_0190:  call       bool [FSharp.Core]Microsoft.FSharp.Linq.NullableOperators::op_EqualsQmark<bool>(!!0,
                                                                                                         valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0195:  stloc.s    V_34
    IL_0197:  ldloc.s    V_34
    IL_0199:  stloc.s    V_35
    IL_019b:  ldloc.s    V_35
    IL_019d:  box        [mscorlib]System.Boolean
    IL_01a2:  ldc.i4.1
    IL_01a3:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<bool>::.ctor(!0)
    IL_01a8:  ldc.i4.0
    IL_01a9:  call       bool [FSharp.Core]Microsoft.FSharp.Linq.NullableOperators::op_QmarkEquals<bool>(valuetype [mscorlib]System.Nullable`1<!!0>,
                                                                                                         !!0)
    IL_01ae:  stloc.s    V_36
    IL_01b0:  ldloc.s    V_36
    IL_01b2:  stloc.s    V_37
    IL_01b4:  ldloc.s    V_37
    IL_01b6:  box        [mscorlib]System.Boolean
    IL_01bb:  ldc.i4.1
    IL_01bc:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<bool>::.ctor(!0)
    IL_01c1:  ldc.i4.0
    IL_01c2:  newobj     instance void valuetype [mscorlib_2]System.Nullable`1<bool>::.ctor(!0)
    IL_01c7:  call       bool [FSharp.Core]Microsoft.FSharp.Linq.NullableOperators::op_QmarkEqualsQmark<bool>(valuetype [mscorlib]System.Nullable`1<!!0>,
                                                                                                              valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_01cc:  stloc.s    V_38
    IL_01ce:  ldloc.s    V_38
    IL_01d0:  stloc.s    V_39
    IL_01d2:  ldloc.s    V_39
    IL_01d4:  box        [mscorlib]System.Boolean
    IL_01d9:  ldc.i4.1
    IL_01da:  stloc.s    V_40
    IL_01dc:  ldloc.s    V_40
    IL_01de:  stloc.s    V_41
    IL_01e0:  ldloc.s    V_41
    IL_01e2:  box        [mscorlib]System.Boolean
    IL_01e7:  ldc.i4.1
    IL_01e8:  stloc.s    V_42
    IL_01ea:  ldloc.s    V_42
    IL_01ec:  stloc.s    V_43
    IL_01ee:  ldloc.s    V_43
    IL_01f0:  box        [mscorlib]System.Boolean
    IL_01f5:  ldc.i4.1
    IL_01f6:  stloc.s    V_44
    IL_01f8:  ldloc.s    V_44
    IL_01fa:  stloc.s    V_45
    IL_01fc:  ldloc.s    V_45
    IL_01fe:  box        [mscorlib]System.Boolean
    IL_0203:  ldc.i4.0
    IL_0204:  stloc.s    V_46
    IL_0206:  ldloc.s    V_46
    IL_0208:  stloc.s    V_47
    IL_020a:  ldloc.s    V_47
    IL_020c:  box        [mscorlib]System.Boolean
    IL_0211:  ldc.i4.0
    IL_0212:  stloc.s    V_48
    IL_0214:  ldloc.s    V_48
    IL_0216:  stloc.s    V_49
    IL_0218:  ldloc.s    V_49
    IL_021a:  box        [mscorlib]System.Boolean
    IL_021f:  call       class [mscorlib_2]System.Collections.IComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0224:  stloc.s    V_51
    IL_0226:  ldc.i4.3
    IL_0227:  stloc.s    V_52
    IL_0229:  ldc.i4.4
    IL_022a:  stloc.s    V_53
    IL_022c:  ldloc.s    V_52
    IL_022e:  ldloc.s    V_53
    IL_0230:  bge.s      IL_0234

    IL_0232:  br.s       IL_0236

    IL_0234:  br.s       IL_023a

    .line 100001,100001 : 0,0 
    IL_0236:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0237:  nop
    IL_0238:  br.s       IL_0241

    .line 100001,100001 : 0,0 
    IL_023a:  ldloc.s    V_52
    IL_023c:  ldloc.s    V_53
    IL_023e:  cgt
    .line 100001,100001 : 0,0 
    IL_0240:  nop
    .line 100001,100001 : 0,0 
    IL_0241:  stloc.s    V_50
    IL_0243:  ldloc.s    V_50
    IL_0245:  stloc.s    V_54
    IL_0247:  ldloc.s    V_54
    IL_0249:  box        [mscorlib]System.Int32
    IL_024e:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0253:  stloc.s    V_56
    IL_0255:  ldc.i4.1
    IL_0256:  ldc.i4.2
    IL_0257:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_025c:  stloc.s    V_57
    IL_025e:  ldloc.s    V_57
    IL_0260:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
    IL_0265:  stloc.s    V_58
    IL_0267:  ldloc.s    V_57
    IL_0269:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
    IL_026e:  stloc.s    V_59
    IL_0270:  ldloc.s    V_58
    IL_0272:  stloc.s    V_60
    IL_0274:  ldloc.s    V_60
    IL_0276:  ldc.i4.5
    IL_0277:  shl
    IL_0278:  ldloc.s    V_60
    IL_027a:  add
    IL_027b:  ldloc.s    V_59
    IL_027d:  xor
    IL_027e:  stloc.s    V_55
    IL_0280:  ldloc.s    V_55
    IL_0282:  stloc.s    V_61
    IL_0284:  ldloc.s    V_61
    IL_0286:  box        [mscorlib]System.Int32
    IL_028b:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0290:  stloc.s    V_63
    IL_0292:  ldc.i4.1
    IL_0293:  ldc.i4.2
    IL_0294:  ldc.i4.3
    IL_0295:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_029a:  stloc.s    V_64
    IL_029c:  ldloc.s    V_64
    IL_029e:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_02a3:  stloc.s    V_65
    IL_02a5:  ldloc.s    V_64
    IL_02a7:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_02ac:  stloc.s    V_66
    IL_02ae:  ldloc.s    V_64
    IL_02b0:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_02b5:  stloc.s    V_67
    IL_02b7:  ldloc.s    V_65
    IL_02b9:  stloc.s    V_69
    IL_02bb:  ldloc.s    V_69
    IL_02bd:  ldc.i4.5
    IL_02be:  shl
    IL_02bf:  ldloc.s    V_69
    IL_02c1:  add
    IL_02c2:  ldloc.s    V_66
    IL_02c4:  xor
    IL_02c5:  stloc.s    V_68
    IL_02c7:  ldloc.s    V_68
    IL_02c9:  ldc.i4.5
    IL_02ca:  shl
    IL_02cb:  ldloc.s    V_68
    IL_02cd:  add
    IL_02ce:  ldloc.s    V_67
    IL_02d0:  xor
    IL_02d1:  stloc.s    V_62
    IL_02d3:  ldloc.s    V_62
    IL_02d5:  stloc.s    V_70
    IL_02d7:  ldloc.s    V_70
    IL_02d9:  box        [mscorlib]System.Int32
    IL_02de:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_02e3:  stloc.s    V_72
    IL_02e5:  ldc.i4.1
    IL_02e6:  ldc.i4.2
    IL_02e7:  ldc.i4.3
    IL_02e8:  ldc.i4.4
    IL_02e9:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_02ee:  stloc.s    V_73
    IL_02f0:  ldloc.s    V_73
    IL_02f2:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_02f7:  stloc.s    V_74
    IL_02f9:  ldloc.s    V_73
    IL_02fb:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_0300:  stloc.s    V_75
    IL_0302:  ldloc.s    V_73
    IL_0304:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_0309:  stloc.s    V_76
    IL_030b:  ldloc.s    V_73
    IL_030d:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_0312:  stloc.s    V_77
    IL_0314:  ldloc.s    V_74
    IL_0316:  stloc.s    V_79
    IL_0318:  ldloc.s    V_79
    IL_031a:  ldc.i4.5
    IL_031b:  shl
    IL_031c:  ldloc.s    V_79
    IL_031e:  add
    IL_031f:  ldloc.s    V_75
    IL_0321:  xor
    IL_0322:  stloc.s    V_78
    IL_0324:  ldloc.s    V_78
    IL_0326:  ldc.i4.5
    IL_0327:  shl
    IL_0328:  ldloc.s    V_78
    IL_032a:  add
    IL_032b:  ldloc.s    V_76
    IL_032d:  stloc.s    V_80
    IL_032f:  ldloc.s    V_80
    IL_0331:  ldc.i4.5
    IL_0332:  shl
    IL_0333:  ldloc.s    V_80
    IL_0335:  add
    IL_0336:  ldloc.s    V_77
    IL_0338:  xor
    IL_0339:  xor
    IL_033a:  stloc.s    V_71
    IL_033c:  ldloc.s    V_71
    IL_033e:  stloc.s    V_81
    IL_0340:  ldloc.s    V_81
    IL_0342:  box        [mscorlib]System.Int32
    IL_0347:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_034c:  stloc.s    V_83
    IL_034e:  ldc.i4.1
    IL_034f:  ldc.i4.2
    IL_0350:  ldc.i4.3
    IL_0351:  ldc.i4.4
    IL_0352:  ldc.i4.5
    IL_0353:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4)
    IL_0358:  stloc.s    V_84
    IL_035a:  ldloc.s    V_84
    IL_035c:  call       instance !0 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item1()
    IL_0361:  stloc.s    V_85
    IL_0363:  ldloc.s    V_84
    IL_0365:  call       instance !1 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item2()
    IL_036a:  stloc.s    V_86
    IL_036c:  ldloc.s    V_84
    IL_036e:  call       instance !2 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item3()
    IL_0373:  stloc.s    V_87
    IL_0375:  ldloc.s    V_84
    IL_0377:  call       instance !3 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item4()
    IL_037c:  stloc.s    V_88
    IL_037e:  ldloc.s    V_84
    IL_0380:  call       instance !4 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item5()
    IL_0385:  stloc.s    V_89
    IL_0387:  ldloc.s    V_85
    IL_0389:  stloc.s    V_92
    IL_038b:  ldloc.s    V_92
    IL_038d:  ldc.i4.5
    IL_038e:  shl
    IL_038f:  ldloc.s    V_92
    IL_0391:  add
    IL_0392:  ldloc.s    V_86
    IL_0394:  xor
    IL_0395:  stloc.s    V_91
    IL_0397:  ldloc.s    V_91
    IL_0399:  ldc.i4.5
    IL_039a:  shl
    IL_039b:  ldloc.s    V_91
    IL_039d:  add
    IL_039e:  ldloc.s    V_87
    IL_03a0:  stloc.s    V_93
    IL_03a2:  ldloc.s    V_93
    IL_03a4:  ldc.i4.5
    IL_03a5:  shl
    IL_03a6:  ldloc.s    V_93
    IL_03a8:  add
    IL_03a9:  ldloc.s    V_88
    IL_03ab:  xor
    IL_03ac:  xor
    IL_03ad:  stloc.s    V_90
    IL_03af:  ldloc.s    V_90
    IL_03b1:  ldc.i4.5
    IL_03b2:  shl
    IL_03b3:  ldloc.s    V_90
    IL_03b5:  add
    IL_03b6:  ldloc.s    V_89
    IL_03b8:  xor
    IL_03b9:  stloc.s    V_82
    IL_03bb:  ldloc.s    V_82
    IL_03bd:  stloc.s    V_94
    IL_03bf:  ldloc.s    V_94
    IL_03c1:  box        [mscorlib]System.Int32
    IL_03c6:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_03cb:  stloc.s    V_96
    IL_03cd:  ldc.i4.1
    IL_03ce:  ldc.i4.2
    IL_03cf:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_03d4:  stloc.s    V_97
    IL_03d6:  ldc.i4.1
    IL_03d7:  ldc.i4.2
    IL_03d8:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_03dd:  stloc.s    V_98
    IL_03df:  ldloc.s    V_97
    IL_03e1:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
    IL_03e6:  stloc.s    V_99
    IL_03e8:  ldloc.s    V_97
    IL_03ea:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
    IL_03ef:  stloc.s    V_100
    IL_03f1:  ldloc.s    V_98
    IL_03f3:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
    IL_03f8:  stloc.s    V_101
    IL_03fa:  ldloc.s    V_98
    IL_03fc:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
    IL_0401:  stloc.s    V_102
    IL_0403:  ldloc.s    V_99
    IL_0405:  ldloc.s    V_101
    IL_0407:  bne.un.s   IL_040b

    IL_0409:  br.s       IL_040d

    IL_040b:  br.s       IL_0416

    .line 100001,100001 : 0,0 
    IL_040d:  ldloc.s    V_100
    IL_040f:  ldloc.s    V_102
    IL_0411:  ceq
    .line 100001,100001 : 0,0 
    IL_0413:  nop
    IL_0414:  br.s       IL_0418

    .line 100001,100001 : 0,0 
    IL_0416:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0417:  nop
    .line 100001,100001 : 0,0 
    IL_0418:  stloc.s    V_95
    IL_041a:  ldloc.s    V_95
    IL_041c:  stloc.s    V_103
    IL_041e:  ldloc.s    V_103
    IL_0420:  box        [mscorlib]System.Boolean
    IL_0425:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_042a:  stloc.s    V_105
    IL_042c:  ldc.i4.1
    IL_042d:  ldc.i4.2
    IL_042e:  ldc.i4.3
    IL_042f:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_0434:  stloc.s    V_106
    IL_0436:  ldc.i4.1
    IL_0437:  ldc.i4.2
    IL_0438:  ldc.i4.3
    IL_0439:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_043e:  stloc.s    V_107
    IL_0440:  ldloc.s    V_106
    IL_0442:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_0447:  stloc.s    V_108
    IL_0449:  ldloc.s    V_106
    IL_044b:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_0450:  stloc.s    V_109
    IL_0452:  ldloc.s    V_106
    IL_0454:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_0459:  stloc.s    V_110
    IL_045b:  ldloc.s    V_107
    IL_045d:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_0462:  stloc.s    V_111
    IL_0464:  ldloc.s    V_107
    IL_0466:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_046b:  stloc.s    V_112
    IL_046d:  ldloc.s    V_107
    IL_046f:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_0474:  stloc.s    V_113
    IL_0476:  ldloc.s    V_108
    IL_0478:  ldloc.s    V_111
    IL_047a:  bne.un.s   IL_047e

    IL_047c:  br.s       IL_0480

    IL_047e:  br.s       IL_0489

    .line 100001,100001 : 0,0 
    IL_0480:  ldloc.s    V_109
    IL_0482:  ldloc.s    V_112
    IL_0484:  ceq
    .line 100001,100001 : 0,0 
    IL_0486:  nop
    IL_0487:  br.s       IL_048b

    .line 100001,100001 : 0,0 
    IL_0489:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_048a:  nop
    .line 100001,100001 : 0,0 
    IL_048b:  brfalse.s  IL_048f

    IL_048d:  br.s       IL_0491

    IL_048f:  br.s       IL_049a

    .line 100001,100001 : 0,0 
    IL_0491:  ldloc.s    V_110
    IL_0493:  ldloc.s    V_113
    IL_0495:  ceq
    .line 100001,100001 : 0,0 
    IL_0497:  nop
    IL_0498:  br.s       IL_049c

    .line 100001,100001 : 0,0 
    IL_049a:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_049b:  nop
    .line 100001,100001 : 0,0 
    IL_049c:  stloc.s    V_104
    IL_049e:  ldloc.s    V_104
    IL_04a0:  stloc.s    V_114
    IL_04a2:  ldloc.s    V_114
    IL_04a4:  box        [mscorlib]System.Boolean
    IL_04a9:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_04ae:  stloc.s    V_116
    IL_04b0:  ldc.i4.1
    IL_04b1:  ldc.i4.2
    IL_04b2:  ldc.i4.3
    IL_04b3:  ldc.i4.4
    IL_04b4:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_04b9:  stloc.s    V_117
    IL_04bb:  ldc.i4.1
    IL_04bc:  ldc.i4.2
    IL_04bd:  ldc.i4.3
    IL_04be:  ldc.i4.4
    IL_04bf:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_04c4:  stloc.s    V_118
    IL_04c6:  ldloc.s    V_117
    IL_04c8:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_04cd:  stloc.s    V_119
    IL_04cf:  ldloc.s    V_117
    IL_04d1:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_04d6:  stloc.s    V_120
    IL_04d8:  ldloc.s    V_117
    IL_04da:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_04df:  stloc.s    V_121
    IL_04e1:  ldloc.s    V_117
    IL_04e3:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_04e8:  stloc.s    V_122
    IL_04ea:  ldloc.s    V_118
    IL_04ec:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_04f1:  stloc.s    V_123
    IL_04f3:  ldloc.s    V_118
    IL_04f5:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_04fa:  stloc.s    V_124
    IL_04fc:  ldloc.s    V_118
    IL_04fe:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_0503:  stloc.s    V_125
    IL_0505:  ldloc.s    V_118
    IL_0507:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_050c:  stloc.s    V_126
    IL_050e:  ldloc.s    V_119
    IL_0510:  ldloc.s    V_123
    IL_0512:  bne.un.s   IL_0516

    IL_0514:  br.s       IL_0518

    IL_0516:  br.s       IL_0521

    .line 100001,100001 : 0,0 
    IL_0518:  ldloc.s    V_120
    IL_051a:  ldloc.s    V_124
    IL_051c:  ceq
    .line 100001,100001 : 0,0 
    IL_051e:  nop
    IL_051f:  br.s       IL_0523

    .line 100001,100001 : 0,0 
    IL_0521:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0522:  nop
    .line 100001,100001 : 0,0 
    IL_0523:  brfalse.s  IL_0527

    IL_0525:  br.s       IL_0529

    IL_0527:  br.s       IL_0532

    .line 100001,100001 : 0,0 
    IL_0529:  ldloc.s    V_121
    IL_052b:  ldloc.s    V_125
    IL_052d:  ceq
    .line 100001,100001 : 0,0 
    IL_052f:  nop
    IL_0530:  br.s       IL_0534

    .line 100001,100001 : 0,0 
    IL_0532:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0533:  nop
    .line 100001,100001 : 0,0 
    IL_0534:  brfalse.s  IL_0538

    IL_0536:  br.s       IL_053a

    IL_0538:  br.s       IL_0543

    .line 100001,100001 : 0,0 
    IL_053a:  ldloc.s    V_122
    IL_053c:  ldloc.s    V_126
    IL_053e:  ceq
    .line 100001,100001 : 0,0 
    IL_0540:  nop
    IL_0541:  br.s       IL_0545

    .line 100001,100001 : 0,0 
    IL_0543:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0544:  nop
    .line 100001,100001 : 0,0 
    IL_0545:  stloc.s    V_115
    IL_0547:  ldloc.s    V_115
    IL_0549:  stloc.s    V_127
    IL_054b:  ldloc.s    V_127
    IL_054d:  box        [mscorlib]System.Boolean
    IL_0552:  call       class [mscorlib_2]System.Collections.IEqualityComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0557:  stloc.s    V_129
    IL_0559:  ldc.i4.1
    IL_055a:  ldc.i4.2
    IL_055b:  ldc.i4.3
    IL_055c:  ldc.i4.4
    IL_055d:  ldc.i4.5
    IL_055e:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4)
    IL_0563:  stloc.s    V_130
    IL_0565:  ldc.i4.1
    IL_0566:  ldc.i4.2
    IL_0567:  ldc.i4.3
    IL_0568:  ldc.i4.4
    IL_0569:  ldc.i4.5
    IL_056a:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4)
    IL_056f:  stloc.s    V_131
    IL_0571:  ldloc.s    V_130
    IL_0573:  call       instance !0 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item1()
    IL_0578:  stloc.s    V_132
    IL_057a:  ldloc.s    V_130
    IL_057c:  call       instance !1 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item2()
    IL_0581:  stloc.s    V_133
    IL_0583:  ldloc.s    V_130
    IL_0585:  call       instance !2 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item3()
    IL_058a:  stloc.s    V_134
    IL_058c:  ldloc.s    V_130
    IL_058e:  call       instance !3 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item4()
    IL_0593:  stloc.s    V_135
    IL_0595:  ldloc.s    V_130
    IL_0597:  call       instance !4 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item5()
    IL_059c:  stloc.s    V_136
    IL_059e:  ldloc.s    V_131
    IL_05a0:  call       instance !0 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item1()
    IL_05a5:  stloc.s    V_137
    IL_05a7:  ldloc.s    V_131
    IL_05a9:  call       instance !1 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item2()
    IL_05ae:  stloc.s    V_138
    IL_05b0:  ldloc.s    V_131
    IL_05b2:  call       instance !2 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item3()
    IL_05b7:  stloc.s    V_139
    IL_05b9:  ldloc.s    V_131
    IL_05bb:  call       instance !3 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item4()
    IL_05c0:  stloc.s    V_140
    IL_05c2:  ldloc.s    V_131
    IL_05c4:  call       instance !4 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item5()
    IL_05c9:  stloc.s    V_141
    IL_05cb:  ldloc.s    V_132
    IL_05cd:  ldloc.s    V_137
    IL_05cf:  bne.un.s   IL_05d3

    IL_05d1:  br.s       IL_05d5

    IL_05d3:  br.s       IL_05de

    .line 100001,100001 : 0,0 
    IL_05d5:  ldloc.s    V_133
    IL_05d7:  ldloc.s    V_138
    IL_05d9:  ceq
    .line 100001,100001 : 0,0 
    IL_05db:  nop
    IL_05dc:  br.s       IL_05e0

    .line 100001,100001 : 0,0 
    IL_05de:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_05df:  nop
    .line 100001,100001 : 0,0 
    IL_05e0:  brfalse.s  IL_05e4

    IL_05e2:  br.s       IL_05e6

    IL_05e4:  br.s       IL_05ef

    .line 100001,100001 : 0,0 
    IL_05e6:  ldloc.s    V_134
    IL_05e8:  ldloc.s    V_139
    IL_05ea:  ceq
    .line 100001,100001 : 0,0 
    IL_05ec:  nop
    IL_05ed:  br.s       IL_05f1

    .line 100001,100001 : 0,0 
    IL_05ef:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_05f0:  nop
    .line 100001,100001 : 0,0 
    IL_05f1:  brfalse.s  IL_05f5

    IL_05f3:  br.s       IL_05f7

    IL_05f5:  br.s       IL_0600

    .line 100001,100001 : 0,0 
    IL_05f7:  ldloc.s    V_135
    IL_05f9:  ldloc.s    V_140
    IL_05fb:  ceq
    .line 100001,100001 : 0,0 
    IL_05fd:  nop
    IL_05fe:  br.s       IL_0602

    .line 100001,100001 : 0,0 
    IL_0600:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0601:  nop
    .line 100001,100001 : 0,0 
    IL_0602:  brfalse.s  IL_0606

    IL_0604:  br.s       IL_0608

    IL_0606:  br.s       IL_0611

    .line 100001,100001 : 0,0 
    IL_0608:  ldloc.s    V_136
    IL_060a:  ldloc.s    V_141
    IL_060c:  ceq
    .line 100001,100001 : 0,0 
    IL_060e:  nop
    IL_060f:  br.s       IL_0613

    .line 100001,100001 : 0,0 
    IL_0611:  ldc.i4.0
    .line 100001,100001 : 0,0 
    IL_0612:  nop
    .line 100001,100001 : 0,0 
    IL_0613:  stloc.s    V_128
    IL_0615:  ldloc.s    V_128
    IL_0617:  stloc.s    V_142
    IL_0619:  ldloc.s    V_142
    IL_061b:  box        [mscorlib]System.Boolean
    IL_0620:  call       class [mscorlib_2]System.Collections.IComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0625:  stloc.s    V_144
    IL_0627:  ldc.i4.1
    IL_0628:  ldc.i4.2
    IL_0629:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_062e:  stloc.s    V_145
    IL_0630:  ldc.i4.1
    IL_0631:  ldc.i4.2
    IL_0632:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0637:  stloc.s    V_146
    IL_0639:  ldloc.s    V_145
    IL_063b:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
    IL_0640:  stloc.s    V_147
    IL_0642:  ldloc.s    V_145
    IL_0644:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
    IL_0649:  stloc.s    V_148
    IL_064b:  ldloc.s    V_146
    IL_064d:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
    IL_0652:  stloc.s    V_149
    IL_0654:  ldloc.s    V_146
    IL_0656:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
    IL_065b:  stloc.s    V_150
    IL_065d:  ldloc.s    V_147
    IL_065f:  ldloc.s    V_149
    IL_0661:  bge.s      IL_0665

    IL_0663:  br.s       IL_0667

    IL_0665:  br.s       IL_066b

    .line 100001,100001 : 0,0 
    IL_0667:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0668:  nop
    IL_0669:  br.s       IL_0672

    .line 100001,100001 : 0,0 
    IL_066b:  ldloc.s    V_147
    IL_066d:  ldloc.s    V_149
    IL_066f:  cgt
    .line 100001,100001 : 0,0 
    IL_0671:  nop
    .line 100001,100001 : 0,0 
    IL_0672:  stloc.s    V_151
    IL_0674:  ldloc.s    V_151
    IL_0676:  brfalse.s  IL_067a

    IL_0678:  br.s       IL_067c

    IL_067a:  br.s       IL_0681

    .line 100001,100001 : 0,0 
    IL_067c:  ldloc.s    V_151
    .line 100001,100001 : 0,0 
    IL_067e:  nop
    IL_067f:  br.s       IL_0696

    .line 100001,100001 : 0,0 
    IL_0681:  ldloc.s    V_148
    IL_0683:  ldloc.s    V_150
    IL_0685:  bge.s      IL_0689

    IL_0687:  br.s       IL_068b

    IL_0689:  br.s       IL_068f

    .line 100001,100001 : 0,0 
    IL_068b:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_068c:  nop
    IL_068d:  br.s       IL_0696

    .line 100001,100001 : 0,0 
    IL_068f:  ldloc.s    V_148
    IL_0691:  ldloc.s    V_150
    IL_0693:  cgt
    .line 100001,100001 : 0,0 
    IL_0695:  nop
    .line 100001,100001 : 0,0 
    IL_0696:  stloc.s    V_143
    IL_0698:  ldloc.s    V_143
    IL_069a:  stloc.s    V_152
    IL_069c:  ldloc.s    V_152
    IL_069e:  box        [mscorlib]System.Int32
    IL_06a3:  call       class [mscorlib_2]System.Collections.IComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_06a8:  stloc.s    V_154
    IL_06aa:  ldc.i4.1
    IL_06ab:  ldc.i4.2
    IL_06ac:  ldc.i4.3
    IL_06ad:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_06b2:  stloc.s    V_155
    IL_06b4:  ldc.i4.1
    IL_06b5:  ldc.i4.2
    IL_06b6:  ldc.i4.3
    IL_06b7:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_06bc:  stloc.s    V_156
    IL_06be:  ldloc.s    V_155
    IL_06c0:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_06c5:  stloc.s    V_157
    IL_06c7:  ldloc.s    V_155
    IL_06c9:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_06ce:  stloc.s    V_158
    IL_06d0:  ldloc.s    V_155
    IL_06d2:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_06d7:  stloc.s    V_159
    IL_06d9:  ldloc.s    V_156
    IL_06db:  call       instance !0 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item1()
    IL_06e0:  stloc.s    V_160
    IL_06e2:  ldloc.s    V_156
    IL_06e4:  call       instance !1 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item2()
    IL_06e9:  stloc.s    V_161
    IL_06eb:  ldloc.s    V_156
    IL_06ed:  call       instance !2 class [mscorlib]System.Tuple`3<int32,int32,int32>::get_Item3()
    IL_06f2:  stloc.s    V_162
    IL_06f4:  ldloc.s    V_157
    IL_06f6:  ldloc.s    V_160
    IL_06f8:  bge.s      IL_06fc

    IL_06fa:  br.s       IL_06fe

    IL_06fc:  br.s       IL_0702

    .line 100001,100001 : 0,0 
    IL_06fe:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_06ff:  nop
    IL_0700:  br.s       IL_0709

    .line 100001,100001 : 0,0 
    IL_0702:  ldloc.s    V_157
    IL_0704:  ldloc.s    V_160
    IL_0706:  cgt
    .line 100001,100001 : 0,0 
    IL_0708:  nop
    .line 100001,100001 : 0,0 
    IL_0709:  stloc.s    V_163
    IL_070b:  ldloc.s    V_163
    IL_070d:  brfalse.s  IL_0711

    IL_070f:  br.s       IL_0713

    IL_0711:  br.s       IL_0718

    .line 100001,100001 : 0,0 
    IL_0713:  ldloc.s    V_163
    .line 100001,100001 : 0,0 
    IL_0715:  nop
    IL_0716:  br.s       IL_0751

    .line 100001,100001 : 0,0 
    IL_0718:  ldloc.s    V_158
    IL_071a:  ldloc.s    V_161
    IL_071c:  bge.s      IL_0720

    IL_071e:  br.s       IL_0722

    IL_0720:  br.s       IL_0726

    .line 100001,100001 : 0,0 
    IL_0722:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0723:  nop
    IL_0724:  br.s       IL_072d

    .line 100001,100001 : 0,0 
    IL_0726:  ldloc.s    V_158
    IL_0728:  ldloc.s    V_161
    IL_072a:  cgt
    .line 100001,100001 : 0,0 
    IL_072c:  nop
    .line 100001,100001 : 0,0 
    IL_072d:  stloc.s    V_164
    IL_072f:  ldloc.s    V_164
    IL_0731:  brfalse.s  IL_0735

    IL_0733:  br.s       IL_0737

    IL_0735:  br.s       IL_073c

    .line 100001,100001 : 0,0 
    IL_0737:  ldloc.s    V_164
    .line 100001,100001 : 0,0 
    IL_0739:  nop
    IL_073a:  br.s       IL_0751

    .line 100001,100001 : 0,0 
    IL_073c:  ldloc.s    V_159
    IL_073e:  ldloc.s    V_162
    IL_0740:  bge.s      IL_0744

    IL_0742:  br.s       IL_0746

    IL_0744:  br.s       IL_074a

    .line 100001,100001 : 0,0 
    IL_0746:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0747:  nop
    IL_0748:  br.s       IL_0751

    .line 100001,100001 : 0,0 
    IL_074a:  ldloc.s    V_159
    IL_074c:  ldloc.s    V_162
    IL_074e:  cgt
    .line 100001,100001 : 0,0 
    IL_0750:  nop
    .line 100001,100001 : 0,0 
    IL_0751:  stloc.s    V_153
    IL_0753:  ldloc.s    V_153
    IL_0755:  stloc.s    V_165
    IL_0757:  ldloc.s    V_165
    IL_0759:  box        [mscorlib]System.Int32
    IL_075e:  call       class [mscorlib_2]System.Collections.IComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0763:  stloc.s    V_167
    IL_0765:  ldc.i4.1
    IL_0766:  ldc.i4.2
    IL_0767:  ldc.i4.3
    IL_0768:  ldc.i4.4
    IL_0769:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_076e:  stloc.s    V_168
    IL_0770:  ldc.i4.1
    IL_0771:  ldc.i4.2
    IL_0772:  ldc.i4.3
    IL_0773:  ldc.i4.4
    IL_0774:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_0779:  stloc.s    V_169
    IL_077b:  ldloc.s    V_168
    IL_077d:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_0782:  stloc.s    V_170
    IL_0784:  ldloc.s    V_168
    IL_0786:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_078b:  stloc.s    V_171
    IL_078d:  ldloc.s    V_168
    IL_078f:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_0794:  stloc.s    V_172
    IL_0796:  ldloc.s    V_168
    IL_0798:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_079d:  stloc.s    V_173
    IL_079f:  ldloc.s    V_169
    IL_07a1:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_07a6:  stloc.s    V_174
    IL_07a8:  ldloc.s    V_169
    IL_07aa:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_07af:  stloc.s    V_175
    IL_07b1:  ldloc.s    V_169
    IL_07b3:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_07b8:  stloc.s    V_176
    IL_07ba:  ldloc.s    V_169
    IL_07bc:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_07c1:  stloc.s    V_177
    IL_07c3:  ldloc.s    V_170
    IL_07c5:  ldloc.s    V_174
    IL_07c7:  bge.s      IL_07cb

    IL_07c9:  br.s       IL_07cd

    IL_07cb:  br.s       IL_07d1

    .line 100001,100001 : 0,0 
    IL_07cd:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_07ce:  nop
    IL_07cf:  br.s       IL_07d8

    .line 100001,100001 : 0,0 
    IL_07d1:  ldloc.s    V_170
    IL_07d3:  ldloc.s    V_174
    IL_07d5:  cgt
    .line 100001,100001 : 0,0 
    IL_07d7:  nop
    .line 100001,100001 : 0,0 
    IL_07d8:  stloc.s    V_178
    IL_07da:  ldloc.s    V_178
    IL_07dc:  brfalse.s  IL_07e0

    IL_07de:  br.s       IL_07e2

    IL_07e0:  br.s       IL_07ea

    .line 100001,100001 : 0,0 
    IL_07e2:  ldloc.s    V_178
    .line 100001,100001 : 0,0 
    IL_07e4:  nop
    IL_07e5:  br         IL_0847

    .line 100001,100001 : 0,0 
    IL_07ea:  ldloc.s    V_171
    IL_07ec:  ldloc.s    V_175
    IL_07ee:  bge.s      IL_07f2

    IL_07f0:  br.s       IL_07f4

    IL_07f2:  br.s       IL_07f8

    .line 100001,100001 : 0,0 
    IL_07f4:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_07f5:  nop
    IL_07f6:  br.s       IL_07ff

    .line 100001,100001 : 0,0 
    IL_07f8:  ldloc.s    V_171
    IL_07fa:  ldloc.s    V_175
    IL_07fc:  cgt
    .line 100001,100001 : 0,0 
    IL_07fe:  nop
    .line 100001,100001 : 0,0 
    IL_07ff:  stloc.s    V_179
    IL_0801:  ldloc.s    V_179
    IL_0803:  brfalse.s  IL_0807

    IL_0805:  br.s       IL_0809

    IL_0807:  br.s       IL_080e

    .line 100001,100001 : 0,0 
    IL_0809:  ldloc.s    V_179
    .line 100001,100001 : 0,0 
    IL_080b:  nop
    IL_080c:  br.s       IL_0847

    .line 100001,100001 : 0,0 
    IL_080e:  ldloc.s    V_172
    IL_0810:  ldloc.s    V_176
    IL_0812:  bge.s      IL_0816

    IL_0814:  br.s       IL_0818

    IL_0816:  br.s       IL_081c

    .line 100001,100001 : 0,0 
    IL_0818:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0819:  nop
    IL_081a:  br.s       IL_0823

    .line 100001,100001 : 0,0 
    IL_081c:  ldloc.s    V_172
    IL_081e:  ldloc.s    V_176
    IL_0820:  cgt
    .line 100001,100001 : 0,0 
    IL_0822:  nop
    .line 100001,100001 : 0,0 
    IL_0823:  stloc.s    V_180
    IL_0825:  ldloc.s    V_180
    IL_0827:  brfalse.s  IL_082b

    IL_0829:  br.s       IL_082d

    IL_082b:  br.s       IL_0832

    .line 100001,100001 : 0,0 
    IL_082d:  ldloc.s    V_180
    .line 100001,100001 : 0,0 
    IL_082f:  nop
    IL_0830:  br.s       IL_0847

    .line 100001,100001 : 0,0 
    IL_0832:  ldloc.s    V_173
    IL_0834:  ldloc.s    V_177
    IL_0836:  bge.s      IL_083a

    IL_0838:  br.s       IL_083c

    IL_083a:  br.s       IL_0840

    .line 100001,100001 : 0,0 
    IL_083c:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_083d:  nop
    IL_083e:  br.s       IL_0847

    .line 100001,100001 : 0,0 
    IL_0840:  ldloc.s    V_173
    IL_0842:  ldloc.s    V_177
    IL_0844:  cgt
    .line 100001,100001 : 0,0 
    IL_0846:  nop
    .line 100001,100001 : 0,0 
    IL_0847:  stloc.s    V_166
    IL_0849:  ldloc.s    V_166
    IL_084b:  stloc.s    V_181
    IL_084d:  ldloc.s    V_181
    IL_084f:  box        [mscorlib]System.Int32
    IL_0854:  call       class [mscorlib_2]System.Collections.IComparer [FSharp.Core_3]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0859:  stloc.s    V_183
    IL_085b:  ldc.i4.1
    IL_085c:  ldc.i4.2
    IL_085d:  ldc.i4.3
    IL_085e:  ldc.i4.4
    IL_085f:  ldc.i4.5
    IL_0860:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4)
    IL_0865:  stloc.s    V_184
    IL_0867:  ldc.i4.1
    IL_0868:  ldc.i4.2
    IL_0869:  ldc.i4.3
    IL_086a:  ldc.i4.4
    IL_086b:  ldc.i4.5
    IL_086c:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4)
    IL_0871:  stloc.s    V_185
    IL_0873:  ldloc.s    V_184
    IL_0875:  call       instance !0 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item1()
    IL_087a:  stloc.s    V_186
    IL_087c:  ldloc.s    V_184
    IL_087e:  call       instance !1 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item2()
    IL_0883:  stloc.s    V_187
    IL_0885:  ldloc.s    V_184
    IL_0887:  call       instance !2 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item3()
    IL_088c:  stloc.s    V_188
    IL_088e:  ldloc.s    V_184
    IL_0890:  call       instance !3 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item4()
    IL_0895:  stloc.s    V_189
    IL_0897:  ldloc.s    V_184
    IL_0899:  call       instance !4 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item5()
    IL_089e:  stloc.s    V_190
    IL_08a0:  ldloc.s    V_185
    IL_08a2:  call       instance !0 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item1()
    IL_08a7:  stloc.s    V_191
    IL_08a9:  ldloc.s    V_185
    IL_08ab:  call       instance !1 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item2()
    IL_08b0:  stloc.s    V_192
    IL_08b2:  ldloc.s    V_185
    IL_08b4:  call       instance !2 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item3()
    IL_08b9:  stloc.s    V_193
    IL_08bb:  ldloc.s    V_185
    IL_08bd:  call       instance !3 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item4()
    IL_08c2:  stloc.s    V_194
    IL_08c4:  ldloc.s    V_185
    IL_08c6:  call       instance !4 class [mscorlib]System.Tuple`5<int32,int32,int32,int32,int32>::get_Item5()
    IL_08cb:  stloc.s    V_195
    IL_08cd:  ldloc.s    V_186
    IL_08cf:  ldloc.s    V_191
    IL_08d1:  bge.s      IL_08d5

    IL_08d3:  br.s       IL_08d7

    IL_08d5:  br.s       IL_08db

    .line 100001,100001 : 0,0 
    IL_08d7:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_08d8:  nop
    IL_08d9:  br.s       IL_08e2

    .line 100001,100001 : 0,0 
    IL_08db:  ldloc.s    V_186
    IL_08dd:  ldloc.s    V_191
    IL_08df:  cgt
    .line 100001,100001 : 0,0 
    IL_08e1:  nop
    .line 100001,100001 : 0,0 
    IL_08e2:  stloc.s    V_196
    IL_08e4:  ldloc.s    V_196
    IL_08e6:  brfalse.s  IL_08ea

    IL_08e8:  br.s       IL_08ec

    IL_08ea:  br.s       IL_08f4

    .line 100001,100001 : 0,0 
    IL_08ec:  ldloc.s    V_196
    .line 100001,100001 : 0,0 
    IL_08ee:  nop
    IL_08ef:  br         IL_0978

    .line 100001,100001 : 0,0 
    IL_08f4:  ldloc.s    V_187
    IL_08f6:  ldloc.s    V_192
    IL_08f8:  bge.s      IL_08fc

    IL_08fa:  br.s       IL_08fe

    IL_08fc:  br.s       IL_0902

    .line 100001,100001 : 0,0 
    IL_08fe:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_08ff:  nop
    IL_0900:  br.s       IL_0909

    .line 100001,100001 : 0,0 
    IL_0902:  ldloc.s    V_187
    IL_0904:  ldloc.s    V_192
    IL_0906:  cgt
    .line 100001,100001 : 0,0 
    IL_0908:  nop
    .line 100001,100001 : 0,0 
    IL_0909:  stloc.s    V_197
    IL_090b:  ldloc.s    V_197
    IL_090d:  brfalse.s  IL_0911

    IL_090f:  br.s       IL_0913

    IL_0911:  br.s       IL_091b

    .line 100001,100001 : 0,0 
    IL_0913:  ldloc.s    V_197
    .line 100001,100001 : 0,0 
    IL_0915:  nop
    IL_0916:  br         IL_0978

    .line 100001,100001 : 0,0 
    IL_091b:  ldloc.s    V_188
    IL_091d:  ldloc.s    V_193
    IL_091f:  bge.s      IL_0923

    IL_0921:  br.s       IL_0925

    IL_0923:  br.s       IL_0929

    .line 100001,100001 : 0,0 
    IL_0925:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_0926:  nop
    IL_0927:  br.s       IL_0930

    .line 100001,100001 : 0,0 
    IL_0929:  ldloc.s    V_188
    IL_092b:  ldloc.s    V_193
    IL_092d:  cgt
    .line 100001,100001 : 0,0 
    IL_092f:  nop
    .line 100001,100001 : 0,0 
    IL_0930:  stloc.s    V_198
    IL_0932:  ldloc.s    V_198
    IL_0934:  brfalse.s  IL_0938

    IL_0936:  br.s       IL_093a

    IL_0938:  br.s       IL_093f

    .line 100001,100001 : 0,0 
    IL_093a:  ldloc.s    V_198
    .line 100001,100001 : 0,0 
    IL_093c:  nop
    IL_093d:  br.s       IL_0978

    .line 100001,100001 : 0,0 
    IL_093f:  ldloc.s    V_189
    IL_0941:  ldloc.s    V_194
    IL_0943:  bge.s      IL_0947

    IL_0945:  br.s       IL_0949

    IL_0947:  br.s       IL_094d

    .line 100001,100001 : 0,0 
    IL_0949:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_094a:  nop
    IL_094b:  br.s       IL_0954

    .line 100001,100001 : 0,0 
    IL_094d:  ldloc.s    V_189
    IL_094f:  ldloc.s    V_194
    IL_0951:  cgt
    .line 100001,100001 : 0,0 
    IL_0953:  nop
    .line 100001,100001 : 0,0 
    IL_0954:  stloc.s    V_199
    IL_0956:  ldloc.s    V_199
    IL_0958:  brfalse.s  IL_095c

    IL_095a:  br.s       IL_095e

    IL_095c:  br.s       IL_0963

    .line 100001,100001 : 0,0 
    IL_095e:  ldloc.s    V_199
    .line 100001,100001 : 0,0 
    IL_0960:  nop
    IL_0961:  br.s       IL_0978

    .line 100001,100001 : 0,0 
    IL_0963:  ldloc.s    V_190
    IL_0965:  ldloc.s    V_195
    IL_0967:  bge.s      IL_096b

    IL_0969:  br.s       IL_096d

    IL_096b:  br.s       IL_0971

    .line 100001,100001 : 0,0 
    IL_096d:  ldc.i4.m1
    .line 100001,100001 : 0,0 
    IL_096e:  nop
    IL_096f:  br.s       IL_0978

    .line 100001,100001 : 0,0 
    IL_0971:  ldloc.s    V_190
    IL_0973:  ldloc.s    V_195
    IL_0975:  cgt
    .line 100001,100001 : 0,0 
    IL_0977:  nop
    .line 100001,100001 : 0,0 
    IL_0978:  stloc.s    V_182
    IL_097a:  ldloc.s    V_182
    IL_097c:  stloc.s    V_200
    IL_097e:  ldloc.s    V_200
    IL_0980:  box        [mscorlib]System.Int32
    IL_0985:  ldc.i4.7
    IL_0986:  stloc.s    V_201
    IL_0988:  ldloc.s    V_201
    IL_098a:  stloc.s    V_202
    IL_098c:  ldloc.s    V_202
    IL_098e:  box        [mscorlib]System.Int32
    IL_0993:  ldc.i4.0
    IL_0994:  stloc.s    V_203
    IL_0996:  ldloc.s    V_203
    IL_0998:  stloc.s    V_204
    IL_099a:  ldloc.s    V_204
    IL_099c:  box        [mscorlib]System.Int32
    IL_09a1:  ldc.i4.7
    IL_09a2:  stloc.s    V_205
    IL_09a4:  ldloc.s    V_205
    IL_09a6:  stloc.s    V_206
    IL_09a8:  ldloc.s    V_206
    IL_09aa:  box        [mscorlib]System.Int32
    IL_09af:  ldc.i4.s   -4
    IL_09b1:  stloc.s    V_207
    IL_09b3:  ldloc.s    V_207
    IL_09b5:  stloc.s    V_208
    IL_09b7:  ldloc.s    V_208
    IL_09b9:  box        [mscorlib]System.Int32
    IL_09be:  ldc.i4.3
    IL_09bf:  stloc.s    V_210
    IL_09c1:  ldc.i4.1
    IL_09c2:  stloc.s    V_211
    IL_09c4:  ldloc.s    V_210
    IL_09c6:  ldloc.s    V_211
    IL_09c8:  ldc.i4.s   31
    IL_09ca:  and
    IL_09cb:  shl
    IL_09cc:  stloc.s    V_209
    IL_09ce:  ldloc.s    V_209
    IL_09d0:  stloc.s    V_212
    IL_09d2:  ldloc.s    V_212
    IL_09d4:  box        [mscorlib]System.Int32
    IL_09d9:  ldc.i4.3
    IL_09da:  stloc.s    V_214
    IL_09dc:  ldc.i4.1
    IL_09dd:  stloc.s    V_215
    IL_09df:  ldloc.s    V_214
    IL_09e1:  ldloc.s    V_215
    IL_09e3:  ldc.i4.s   31
    IL_09e5:  and
    IL_09e6:  shr
    IL_09e7:  stloc.s    V_213
    IL_09e9:  ldloc.s    V_213
    IL_09eb:  stloc.s    V_216
    IL_09ed:  ldloc.s    V_216
    IL_09ef:  box        [mscorlib]System.Int32
    IL_09f4:  ldc.i4.4
    IL_09f5:  stloc.s    V_217
    IL_09f7:  ldloc.s    V_217
    IL_09f9:  stloc.s    V_218
    IL_09fb:  ldloc.s    V_218
    IL_09fd:  box        [mscorlib]System.Int32
    IL_0a02:  ldc.i4.2
    IL_0a03:  stloc.s    V_219
    IL_0a05:  ldloc.s    V_219
    IL_0a07:  stloc.s    V_220
    IL_0a09:  ldloc.s    V_220
    IL_0a0b:  box        [mscorlib]System.Int32
    IL_0a10:  ldc.i4.3
    IL_0a11:  stloc.s    V_221
    IL_0a13:  ldloc.s    V_221
    IL_0a15:  stloc.s    V_222
    IL_0a17:  ldloc.s    V_222
    IL_0a19:  box        [mscorlib]System.Int32
    IL_0a1e:  ldc.i4.3
    IL_0a1f:  stloc.s    V_223
    IL_0a21:  ldloc.s    V_223
    IL_0a23:  stloc.s    V_224
    IL_0a25:  ldloc.s    V_224
    IL_0a27:  box        [mscorlib]System.Int32
    IL_0a2c:  newobj     instance void CallIntrinsics/testIntrinsics@2::.ctor()
    IL_0a31:  ldc.i4.1
    IL_0a32:  ldc.i4.2
    IL_0a33:  ldc.i4.3
    IL_0a34:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0a39:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a3e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a43:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a48:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0a4d:  stloc.s    V_225
    IL_0a4f:  ldloc.s    V_225
    IL_0a51:  stloc.s    V_226
    IL_0a53:  ldloc.s    V_226
    IL_0a55:  box        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
    IL_0a5a:  ldc.i4.1
    IL_0a5b:  ldc.i4.2
    IL_0a5c:  ldc.i4.3
    IL_0a5d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0a62:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a67:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a6c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0a71:  stloc.s    V_228
    IL_0a73:  ldloc.s    V_228
    IL_0a75:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_0a7a:  stloc.s    V_229
    IL_0a7c:  ldloc.s    V_229
    IL_0a7e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0a83:  stloc.s    V_230
    IL_0a85:  stloc.s    V_231
    IL_0a87:  stloc.s    V_232
    IL_0a89:  stloc.s    V_233
    IL_0a8b:  stloc.s    V_234
    IL_0a8d:  stloc.s    V_235
    IL_0a8f:  stloc.s    V_236
    IL_0a91:  stloc.s    V_237
    IL_0a93:  stloc.s    V_238
    IL_0a95:  stloc.s    V_239
    IL_0a97:  stloc.s    V_240
    IL_0a99:  stloc.s    V_241
    IL_0a9b:  stloc.s    V_242
    IL_0a9d:  stloc.s    V_243
    IL_0a9f:  stloc.s    V_244
    IL_0aa1:  stloc.s    V_245
    IL_0aa3:  stloc.s    V_246
    IL_0aa5:  stloc.s    V_247
    IL_0aa7:  stloc.s    V_248
    IL_0aa9:  stloc.s    V_249
    IL_0aab:  stloc.s    V_250
    IL_0aad:  stloc.s    V_251
    IL_0aaf:  stloc.s    V_252
    IL_0ab1:  stloc.s    V_253
    IL_0ab3:  stloc.s    V_254
    IL_0ab5:  stloc.s    V_255
    IL_0ab7:  stloc      V_256
    IL_0abb:  stloc      V_257
    IL_0abf:  stloc      V_258
    IL_0ac3:  stloc      V_259
    IL_0ac7:  stloc      V_260
    IL_0acb:  stloc      V_261
    IL_0acf:  stloc      V_262
    IL_0ad3:  stloc      V_263
    IL_0ad7:  stloc      V_264
    IL_0adb:  stloc      V_265
    IL_0adf:  stloc      V_266
    IL_0ae3:  stloc      V_267
    IL_0ae7:  stloc      V_268
    IL_0aeb:  stloc      V_269
    IL_0aef:  stloc      V_270
    IL_0af3:  stloc      V_271
    IL_0af7:  stloc      V_272
    .try
    {
      IL_0afb:  ldc.i4.0
      IL_0afc:  stloc      V_274
      IL_0b00:  ldloc.s    V_230
      IL_0b02:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0b07:  brfalse.s  IL_0b1c

      .line 2,2 : 5,55 
      IL_0b09:  ldloc      V_274
      IL_0b0d:  ldloc.s    V_230
      IL_0b0f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0b14:  add.ovf
      IL_0b15:  stloc      V_274
      .line 100001,100001 : 0,0 
      IL_0b19:  nop
      IL_0b1a:  br.s       IL_0b00

      IL_0b1c:  ldloc      V_274
      IL_0b20:  stloc      V_273
      IL_0b24:  leave.s    IL_0b4a

    }  // end .try
    finally
    {
      IL_0b26:  ldloc.s    V_230
      IL_0b28:  isinst     [mscorlib]System.IDisposable
      IL_0b2d:  stloc      V_275
      IL_0b31:  ldloc      V_275
      IL_0b35:  brfalse.s  IL_0b39

      IL_0b37:  br.s       IL_0b3b

      IL_0b39:  br.s       IL_0b47

      .line 100001,100001 : 0,0 
      IL_0b3b:  ldloc      V_275
      IL_0b3f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0b44:  ldnull
      IL_0b45:  pop
      IL_0b46:  endfinally
      .line 100001,100001 : 0,0 
      IL_0b47:  ldnull
      IL_0b48:  pop
      IL_0b49:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_0b4a:  ldloc      V_272
    IL_0b4e:  ldloc      V_271
    IL_0b52:  ldloc      V_270
    IL_0b56:  ldloc      V_269
    IL_0b5a:  ldloc      V_268
    IL_0b5e:  ldloc      V_267
    IL_0b62:  ldloc      V_266
    IL_0b66:  ldloc      V_265
    IL_0b6a:  ldloc      V_264
    IL_0b6e:  ldloc      V_263
    IL_0b72:  ldloc      V_262
    IL_0b76:  ldloc      V_261
    IL_0b7a:  ldloc      V_260
    IL_0b7e:  ldloc      V_259
    IL_0b82:  ldloc      V_258
    IL_0b86:  ldloc      V_257
    IL_0b8a:  ldloc      V_256
    IL_0b8e:  ldloc.s    V_255
    IL_0b90:  ldloc.s    V_254
    IL_0b92:  ldloc.s    V_253
    IL_0b94:  ldloc.s    V_252
    IL_0b96:  ldloc.s    V_251
    IL_0b98:  ldloc.s    V_250
    IL_0b9a:  ldloc.s    V_249
    IL_0b9c:  ldloc.s    V_248
    IL_0b9e:  ldloc.s    V_247
    IL_0ba0:  ldloc.s    V_246
    IL_0ba2:  ldloc.s    V_245
    IL_0ba4:  ldloc.s    V_244
    IL_0ba6:  ldloc.s    V_243
    IL_0ba8:  ldloc.s    V_242
    IL_0baa:  ldloc.s    V_241
    IL_0bac:  ldloc.s    V_240
    IL_0bae:  ldloc.s    V_239
    IL_0bb0:  ldloc.s    V_238
    IL_0bb2:  ldloc.s    V_237
    IL_0bb4:  ldloc.s    V_236
    IL_0bb6:  ldloc.s    V_235
    IL_0bb8:  ldloc.s    V_234
    IL_0bba:  ldloc.s    V_233
    IL_0bbc:  ldloc.s    V_232
    IL_0bbe:  ldloc.s    V_231
    IL_0bc0:  ldloc      V_273
    IL_0bc4:  stloc.s    V_227
    IL_0bc6:  ldloc.s    V_227
    IL_0bc8:  stloc      V_276
    IL_0bcc:  ldloc      V_276
    IL_0bd0:  box        [mscorlib]System.Int32
    IL_0bd5:  newobj     instance void CallIntrinsics/'testIntrinsics@2-1'::.ctor()
    IL_0bda:  stloc      V_278
    IL_0bde:  ldc.i4.1
    IL_0bdf:  ldc.i4.2
    IL_0be0:  ldc.i4.3
    IL_0be1:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0be6:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0beb:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0bf0:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0bf5:  stloc      V_279
    IL_0bf9:  ldloc      V_279
    IL_0bfd:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_0c02:  stloc      V_280
    IL_0c06:  ldloc      V_280
    IL_0c0a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0c0f:  stloc      V_281
    IL_0c13:  stloc      V_282
    IL_0c17:  stloc      V_283
    IL_0c1b:  stloc      V_284
    IL_0c1f:  stloc      V_285
    IL_0c23:  stloc      V_286
    IL_0c27:  stloc      V_287
    IL_0c2b:  stloc      V_288
    IL_0c2f:  stloc      V_289
    IL_0c33:  stloc      V_290
    IL_0c37:  stloc      V_291
    IL_0c3b:  stloc      V_292
    IL_0c3f:  stloc      V_293
    IL_0c43:  stloc      V_294
    IL_0c47:  stloc      V_295
    IL_0c4b:  stloc      V_296
    IL_0c4f:  stloc      V_297
    IL_0c53:  stloc      V_298
    IL_0c57:  stloc      V_299
    IL_0c5b:  stloc      V_300
    IL_0c5f:  stloc      V_301
    IL_0c63:  stloc      V_302
    IL_0c67:  stloc      V_303
    IL_0c6b:  stloc      V_304
    IL_0c6f:  stloc      V_305
    IL_0c73:  stloc      V_306
    IL_0c77:  stloc      V_307
    IL_0c7b:  stloc      V_308
    IL_0c7f:  stloc      V_309
    IL_0c83:  stloc      V_310
    IL_0c87:  stloc      V_311
    IL_0c8b:  stloc      V_312
    IL_0c8f:  stloc      V_313
    IL_0c93:  stloc      V_314
    IL_0c97:  stloc      V_315
    IL_0c9b:  stloc      V_316
    IL_0c9f:  stloc      V_317
    IL_0ca3:  stloc      V_318
    IL_0ca7:  stloc      V_319
    IL_0cab:  stloc      V_320
    IL_0caf:  stloc      V_321
    IL_0cb3:  stloc      V_322
    IL_0cb7:  stloc      V_323
    IL_0cbb:  stloc      V_324
    .try
    {
      IL_0cbf:  ldc.i4.0
      IL_0cc0:  stloc      V_326
      IL_0cc4:  ldloc      V_281
      IL_0cc8:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0ccd:  brfalse.s  IL_0ced

      .line 2,2 : 5,55 
      IL_0ccf:  ldloc      V_326
      IL_0cd3:  ldloc      V_278
      IL_0cd7:  ldloc      V_281
      IL_0cdb:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0ce0:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_0ce5:  add.ovf
      IL_0ce6:  stloc      V_326
      .line 100001,100001 : 0,0 
      IL_0cea:  nop
      IL_0ceb:  br.s       IL_0cc4

      IL_0ced:  ldloc      V_326
      IL_0cf1:  stloc      V_325
      IL_0cf5:  leave.s    IL_0d1d

    }  // end .try
    finally
    {
      IL_0cf7:  ldloc      V_281
      IL_0cfb:  isinst     [mscorlib]System.IDisposable
      IL_0d00:  stloc      V_327
      IL_0d04:  ldloc      V_327
      IL_0d08:  brfalse.s  IL_0d0c

      IL_0d0a:  br.s       IL_0d0e

      IL_0d0c:  br.s       IL_0d1a

      .line 100001,100001 : 0,0 
      IL_0d0e:  ldloc      V_327
      IL_0d12:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0d17:  ldnull
      IL_0d18:  pop
      IL_0d19:  endfinally
      .line 100001,100001 : 0,0 
      IL_0d1a:  ldnull
      IL_0d1b:  pop
      IL_0d1c:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_0d1d:  ldloc      V_324
    IL_0d21:  ldloc      V_323
    IL_0d25:  ldloc      V_322
    IL_0d29:  ldloc      V_321
    IL_0d2d:  ldloc      V_320
    IL_0d31:  ldloc      V_319
    IL_0d35:  ldloc      V_318
    IL_0d39:  ldloc      V_317
    IL_0d3d:  ldloc      V_316
    IL_0d41:  ldloc      V_315
    IL_0d45:  ldloc      V_314
    IL_0d49:  ldloc      V_313
    IL_0d4d:  ldloc      V_312
    IL_0d51:  ldloc      V_311
    IL_0d55:  ldloc      V_310
    IL_0d59:  ldloc      V_309
    IL_0d5d:  ldloc      V_308
    IL_0d61:  ldloc      V_307
    IL_0d65:  ldloc      V_306
    IL_0d69:  ldloc      V_305
    IL_0d6d:  ldloc      V_304
    IL_0d71:  ldloc      V_303
    IL_0d75:  ldloc      V_302
    IL_0d79:  ldloc      V_301
    IL_0d7d:  ldloc      V_300
    IL_0d81:  ldloc      V_299
    IL_0d85:  ldloc      V_298
    IL_0d89:  ldloc      V_297
    IL_0d8d:  ldloc      V_296
    IL_0d91:  ldloc      V_295
    IL_0d95:  ldloc      V_294
    IL_0d99:  ldloc      V_293
    IL_0d9d:  ldloc      V_292
    IL_0da1:  ldloc      V_291
    IL_0da5:  ldloc      V_290
    IL_0da9:  ldloc      V_289
    IL_0dad:  ldloc      V_288
    IL_0db1:  ldloc      V_287
    IL_0db5:  ldloc      V_286
    IL_0db9:  ldloc      V_285
    IL_0dbd:  ldloc      V_284
    IL_0dc1:  ldloc      V_283
    IL_0dc5:  ldloc      V_282
    IL_0dc9:  ldloc      V_325
    IL_0dcd:  stloc      V_277
    IL_0dd1:  ldloc      V_277
    IL_0dd5:  stloc      V_328
    IL_0dd9:  ldloc      V_328
    IL_0ddd:  box        [mscorlib]System.Int32
    IL_0de2:  ldc.r8     1.
    IL_0deb:  ldc.r8     2.
    IL_0df4:  ldc.r8     3.
    IL_0dfd:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_0e02:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0e07:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0e0c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0e11:  stloc      V_330
    IL_0e15:  ldloc      V_330
    IL_0e19:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_0e1e:  stloc      V_331
    IL_0e22:  ldloc      V_331
    IL_0e26:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_0e2b:  brfalse.s  IL_0e2f

    IL_0e2d:  br.s       IL_0e42

    .line 100001,100001 : 0,0 
    IL_0e2f:  ldstr      "source"
    IL_0e34:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_0e39:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_0e3e:  pop
    .line 100001,100001 : 0,0 
    IL_0e3f:  nop
    IL_0e40:  br.s       IL_0e43

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_0e42:  nop
    .line 100001,100001 : 0,0 
    IL_0e43:  ldloc      V_331
    IL_0e47:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_0e4c:  stloc      V_332
    IL_0e50:  stloc      V_333
    IL_0e54:  stloc      V_334
    IL_0e58:  stloc      V_335
    IL_0e5c:  stloc      V_336
    IL_0e60:  stloc      V_337
    IL_0e64:  stloc      V_338
    IL_0e68:  stloc      V_339
    IL_0e6c:  stloc      V_340
    IL_0e70:  stloc      V_341
    IL_0e74:  stloc      V_342
    IL_0e78:  stloc      V_343
    IL_0e7c:  stloc      V_344
    IL_0e80:  stloc      V_345
    IL_0e84:  stloc      V_346
    IL_0e88:  stloc      V_347
    IL_0e8c:  stloc      V_348
    IL_0e90:  stloc      V_349
    IL_0e94:  stloc      V_350
    IL_0e98:  stloc      V_351
    IL_0e9c:  stloc      V_352
    IL_0ea0:  stloc      V_353
    IL_0ea4:  stloc      V_354
    IL_0ea8:  stloc      V_355
    IL_0eac:  stloc      V_356
    IL_0eb0:  stloc      V_357
    IL_0eb4:  stloc      V_358
    IL_0eb8:  stloc      V_359
    IL_0ebc:  stloc      V_360
    IL_0ec0:  stloc      V_361
    IL_0ec4:  stloc      V_362
    IL_0ec8:  stloc      V_363
    IL_0ecc:  stloc      V_364
    IL_0ed0:  stloc      V_365
    IL_0ed4:  stloc      V_366
    IL_0ed8:  stloc      V_367
    IL_0edc:  stloc      V_368
    IL_0ee0:  stloc      V_369
    IL_0ee4:  stloc      V_370
    IL_0ee8:  stloc      V_371
    IL_0eec:  stloc      V_372
    IL_0ef0:  stloc      V_373
    IL_0ef4:  stloc      V_374
    IL_0ef8:  stloc      V_375
    IL_0efc:  stloc      V_376
    .try
    {
      IL_0f00:  ldc.r8     0.0
      IL_0f09:  stloc      V_378
      IL_0f0d:  ldc.i4.0
      IL_0f0e:  stloc      V_379
      IL_0f12:  ldloc      V_332
      IL_0f16:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0f1b:  brfalse.s  IL_0f3c

      IL_0f1d:  ldloc      V_378
      IL_0f21:  ldloc      V_332
      IL_0f25:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0f2a:  add
      IL_0f2b:  stloc      V_378
      .line 2,2 : 5,55 
      IL_0f2f:  ldloc      V_379
      IL_0f33:  ldc.i4.1
      IL_0f34:  add
      IL_0f35:  stloc      V_379
      .line 100001,100001 : 0,0 
      IL_0f39:  nop
      IL_0f3a:  br.s       IL_0f12

      IL_0f3c:  ldloc      V_379
      IL_0f40:  brtrue.s   IL_0f44

      IL_0f42:  br.s       IL_0f46

      IL_0f44:  br.s       IL_0f5e

      .line 100001,100001 : 0,0 
      IL_0f46:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_0f4b:  ldstr      "source"
      IL_0f50:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_0f55:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_0f5a:  pop
      .line 100001,100001 : 0,0 
      IL_0f5b:  nop
      IL_0f5c:  br.s       IL_0f5f

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_0f5e:  nop
      IL_0f5f:  ldloc      V_378
      IL_0f63:  stloc      V_380
      IL_0f67:  ldloc      V_379
      IL_0f6b:  stloc      V_381
      IL_0f6f:  ldloc      V_380
      IL_0f73:  ldloc      V_381
      IL_0f77:  conv.r8
      IL_0f78:  div
      IL_0f79:  stloc      V_377
      IL_0f7d:  leave.s    IL_0fa5

    }  // end .try
    finally
    {
      IL_0f7f:  ldloc      V_332
      IL_0f83:  isinst     [mscorlib]System.IDisposable
      IL_0f88:  stloc      V_382
      IL_0f8c:  ldloc      V_382
      IL_0f90:  brfalse.s  IL_0f94

      IL_0f92:  br.s       IL_0f96

      IL_0f94:  br.s       IL_0fa2

      .line 100001,100001 : 0,0 
      IL_0f96:  ldloc      V_382
      IL_0f9a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0f9f:  ldnull
      IL_0fa0:  pop
      IL_0fa1:  endfinally
      .line 100001,100001 : 0,0 
      IL_0fa2:  ldnull
      IL_0fa3:  pop
      IL_0fa4:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_0fa5:  ldloc      V_376
    IL_0fa9:  ldloc      V_375
    IL_0fad:  ldloc      V_374
    IL_0fb1:  ldloc      V_373
    IL_0fb5:  ldloc      V_372
    IL_0fb9:  ldloc      V_371
    IL_0fbd:  ldloc      V_370
    IL_0fc1:  ldloc      V_369
    IL_0fc5:  ldloc      V_368
    IL_0fc9:  ldloc      V_367
    IL_0fcd:  ldloc      V_366
    IL_0fd1:  ldloc      V_365
    IL_0fd5:  ldloc      V_364
    IL_0fd9:  ldloc      V_363
    IL_0fdd:  ldloc      V_362
    IL_0fe1:  ldloc      V_361
    IL_0fe5:  ldloc      V_360
    IL_0fe9:  ldloc      V_359
    IL_0fed:  ldloc      V_358
    IL_0ff1:  ldloc      V_357
    IL_0ff5:  ldloc      V_356
    IL_0ff9:  ldloc      V_355
    IL_0ffd:  ldloc      V_354
    IL_1001:  ldloc      V_353
    IL_1005:  ldloc      V_352
    IL_1009:  ldloc      V_351
    IL_100d:  ldloc      V_350
    IL_1011:  ldloc      V_349
    IL_1015:  ldloc      V_348
    IL_1019:  ldloc      V_347
    IL_101d:  ldloc      V_346
    IL_1021:  ldloc      V_345
    IL_1025:  ldloc      V_344
    IL_1029:  ldloc      V_343
    IL_102d:  ldloc      V_342
    IL_1031:  ldloc      V_341
    IL_1035:  ldloc      V_340
    IL_1039:  ldloc      V_339
    IL_103d:  ldloc      V_338
    IL_1041:  ldloc      V_337
    IL_1045:  ldloc      V_336
    IL_1049:  ldloc      V_335
    IL_104d:  ldloc      V_334
    IL_1051:  ldloc      V_333
    IL_1055:  ldloc      V_377
    IL_1059:  stloc      V_329
    IL_105d:  ldloc      V_329
    IL_1061:  stloc      V_383
    IL_1065:  ldloc      V_383
    IL_1069:  box        [mscorlib]System.Double
    IL_106e:  newobj     instance void CallIntrinsics/'testIntrinsics@2-2'::.ctor()
    IL_1073:  stloc      V_385
    IL_1077:  ldc.r8     1.
    IL_1080:  ldc.r8     2.
    IL_1089:  ldc.r8     3.
    IL_1092:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_1097:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_109c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_10a1:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_10a6:  stloc      V_386
    IL_10aa:  ldloc      V_386
    IL_10ae:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_10b3:  stloc      V_387
    IL_10b7:  ldloc      V_387
    IL_10bb:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_10c0:  brfalse.s  IL_10c4

    IL_10c2:  br.s       IL_10d7

    .line 100001,100001 : 0,0 
    IL_10c4:  ldstr      "source"
    IL_10c9:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_10ce:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_10d3:  pop
    .line 100001,100001 : 0,0 
    IL_10d4:  nop
    IL_10d5:  br.s       IL_10d8

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_10d7:  nop
    .line 100001,100001 : 0,0 
    IL_10d8:  ldloc      V_387
    IL_10dc:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_10e1:  stloc      V_388
    IL_10e5:  stloc      V_389
    IL_10e9:  stloc      V_390
    IL_10ed:  stloc      V_391
    IL_10f1:  stloc      V_392
    IL_10f5:  stloc      V_393
    IL_10f9:  stloc      V_394
    IL_10fd:  stloc      V_395
    IL_1101:  stloc      V_396
    IL_1105:  stloc      V_397
    IL_1109:  stloc      V_398
    IL_110d:  stloc      V_399
    IL_1111:  stloc      V_400
    IL_1115:  stloc      V_401
    IL_1119:  stloc      V_402
    IL_111d:  stloc      V_403
    IL_1121:  stloc      V_404
    IL_1125:  stloc      V_405
    IL_1129:  stloc      V_406
    IL_112d:  stloc      V_407
    IL_1131:  stloc      V_408
    IL_1135:  stloc      V_409
    IL_1139:  stloc      V_410
    IL_113d:  stloc      V_411
    IL_1141:  stloc      V_412
    IL_1145:  stloc      V_413
    IL_1149:  stloc      V_414
    IL_114d:  stloc      V_415
    IL_1151:  stloc      V_416
    IL_1155:  stloc      V_417
    IL_1159:  stloc      V_418
    IL_115d:  stloc      V_419
    IL_1161:  stloc      V_420
    IL_1165:  stloc      V_421
    IL_1169:  stloc      V_422
    IL_116d:  stloc      V_423
    IL_1171:  stloc      V_424
    IL_1175:  stloc      V_425
    IL_1179:  stloc      V_426
    IL_117d:  stloc      V_427
    IL_1181:  stloc      V_428
    IL_1185:  stloc      V_429
    IL_1189:  stloc      V_430
    IL_118d:  stloc      V_431
    IL_1191:  stloc      V_432
    IL_1195:  stloc      V_433
    .try
    {
      IL_1199:  ldc.r8     0.0
      IL_11a2:  stloc      V_435
      IL_11a6:  ldc.i4.0
      IL_11a7:  stloc      V_436
      IL_11ab:  ldloc      V_388
      IL_11af:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_11b4:  brfalse.s  IL_11de

      IL_11b6:  ldloc      V_435
      IL_11ba:  ldloc      V_385
      IL_11be:  ldloc      V_388
      IL_11c2:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_11c7:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_11cc:  add
      IL_11cd:  stloc      V_435
      .line 2,2 : 5,55 
      IL_11d1:  ldloc      V_436
      IL_11d5:  ldc.i4.1
      IL_11d6:  add
      IL_11d7:  stloc      V_436
      .line 100001,100001 : 0,0 
      IL_11db:  nop
      IL_11dc:  br.s       IL_11ab

      IL_11de:  ldloc      V_436
      IL_11e2:  brtrue.s   IL_11e6

      IL_11e4:  br.s       IL_11e8

      IL_11e6:  br.s       IL_1200

      .line 100001,100001 : 0,0 
      IL_11e8:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_11ed:  ldstr      "source"
      IL_11f2:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_11f7:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_11fc:  pop
      .line 100001,100001 : 0,0 
      IL_11fd:  nop
      IL_11fe:  br.s       IL_1201

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_1200:  nop
      IL_1201:  ldloc      V_435
      IL_1205:  stloc      V_437
      IL_1209:  ldloc      V_436
      IL_120d:  stloc      V_438
      IL_1211:  ldloc      V_437
      IL_1215:  ldloc      V_438
      IL_1219:  conv.r8
      IL_121a:  div
      IL_121b:  stloc      V_434
      IL_121f:  leave.s    IL_1247

    }  // end .try
    finally
    {
      IL_1221:  ldloc      V_388
      IL_1225:  isinst     [mscorlib]System.IDisposable
      IL_122a:  stloc      V_439
      IL_122e:  ldloc      V_439
      IL_1232:  brfalse.s  IL_1236

      IL_1234:  br.s       IL_1238

      IL_1236:  br.s       IL_1244

      .line 100001,100001 : 0,0 
      IL_1238:  ldloc      V_439
      IL_123c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_1241:  ldnull
      IL_1242:  pop
      IL_1243:  endfinally
      .line 100001,100001 : 0,0 
      IL_1244:  ldnull
      IL_1245:  pop
      IL_1246:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_1247:  ldloc      V_433
    IL_124b:  ldloc      V_432
    IL_124f:  ldloc      V_431
    IL_1253:  ldloc      V_430
    IL_1257:  ldloc      V_429
    IL_125b:  ldloc      V_428
    IL_125f:  ldloc      V_427
    IL_1263:  ldloc      V_426
    IL_1267:  ldloc      V_425
    IL_126b:  ldloc      V_424
    IL_126f:  ldloc      V_423
    IL_1273:  ldloc      V_422
    IL_1277:  ldloc      V_421
    IL_127b:  ldloc      V_420
    IL_127f:  ldloc      V_419
    IL_1283:  ldloc      V_418
    IL_1287:  ldloc      V_417
    IL_128b:  ldloc      V_416
    IL_128f:  ldloc      V_415
    IL_1293:  ldloc      V_414
    IL_1297:  ldloc      V_413
    IL_129b:  ldloc      V_412
    IL_129f:  ldloc      V_411
    IL_12a3:  ldloc      V_410
    IL_12a7:  ldloc      V_409
    IL_12ab:  ldloc      V_408
    IL_12af:  ldloc      V_407
    IL_12b3:  ldloc      V_406
    IL_12b7:  ldloc      V_405
    IL_12bb:  ldloc      V_404
    IL_12bf:  ldloc      V_403
    IL_12c3:  ldloc      V_402
    IL_12c7:  ldloc      V_401
    IL_12cb:  ldloc      V_400
    IL_12cf:  ldloc      V_399
    IL_12d3:  ldloc      V_398
    IL_12d7:  ldloc      V_397
    IL_12db:  ldloc      V_396
    IL_12df:  ldloc      V_395
    IL_12e3:  ldloc      V_394
    IL_12e7:  ldloc      V_393
    IL_12eb:  ldloc      V_392
    IL_12ef:  ldloc      V_391
    IL_12f3:  ldloc      V_390
    IL_12f7:  ldloc      V_389
    IL_12fb:  ldloc      V_434
    IL_12ff:  stloc      V_384
    IL_1303:  ldloc      V_384
    IL_1307:  stloc      V_440
    IL_130b:  ldloc      V_440
    IL_130f:  box        [mscorlib]System.Double
    IL_1314:  newobj     instance void CallIntrinsics/'testIntrinsics@2-3'::.ctor()
    IL_1319:  stloc      V_442
    IL_131d:  ldc.i4.3
    IL_131e:  newarr     [mscorlib]System.Int32
    IL_1323:  dup
    IL_1324:  ldc.i4.0
    IL_1325:  ldc.i4.1
    IL_1326:  stelem     [mscorlib]System.Int32
    IL_132b:  dup
    IL_132c:  ldc.i4.1
    IL_132d:  ldc.i4.2
    IL_132e:  stelem     [mscorlib]System.Int32
    IL_1333:  dup
    IL_1334:  ldc.i4.2
    IL_1335:  ldc.i4.3
    IL_1336:  stelem     [mscorlib]System.Int32
    IL_133b:  stloc      V_443
    IL_133f:  ldloc      V_443
    IL_1343:  box        int32[]
    IL_1348:  brfalse.s  IL_134c

    IL_134a:  br.s       IL_135f

    .line 100001,100001 : 0,0 
    IL_134c:  ldstr      "array"
    IL_1351:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_1356:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_135b:  pop
    .line 100001,100001 : 0,0 
    IL_135c:  nop
    IL_135d:  br.s       IL_1360

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_135f:  nop
    .line 100001,100001 : 0,0 
    IL_1360:  ldloc      V_443
    IL_1364:  ldlen
    IL_1365:  conv.i4
    IL_1366:  stloc      V_444
    IL_136a:  ldloc      V_444
    IL_136e:  newarr     [mscorlib]System.Int32
    IL_1373:  stloc      V_445
    IL_1377:  stloc      V_446
    IL_137b:  stloc      V_447
    IL_137f:  stloc      V_448
    IL_1383:  stloc      V_449
    IL_1387:  stloc      V_450
    IL_138b:  stloc      V_451
    IL_138f:  stloc      V_452
    IL_1393:  stloc      V_453
    IL_1397:  stloc      V_454
    IL_139b:  stloc      V_455
    IL_139f:  stloc      V_456
    IL_13a3:  stloc      V_457
    IL_13a7:  stloc      V_458
    IL_13ab:  stloc      V_459
    IL_13af:  stloc      V_460
    IL_13b3:  stloc      V_461
    IL_13b7:  stloc      V_462
    IL_13bb:  stloc      V_463
    IL_13bf:  stloc      V_464
    IL_13c3:  stloc      V_465
    IL_13c7:  stloc      V_466
    IL_13cb:  stloc      V_467
    IL_13cf:  stloc      V_468
    IL_13d3:  stloc      V_469
    IL_13d7:  stloc      V_470
    IL_13db:  stloc      V_471
    IL_13df:  stloc      V_472
    IL_13e3:  stloc      V_473
    IL_13e7:  stloc      V_474
    IL_13eb:  stloc      V_475
    IL_13ef:  stloc      V_476
    IL_13f3:  stloc      V_477
    IL_13f7:  stloc      V_478
    IL_13fb:  stloc      V_479
    IL_13ff:  stloc      V_480
    IL_1403:  stloc      V_481
    IL_1407:  stloc      V_482
    IL_140b:  stloc      V_483
    IL_140f:  stloc      V_484
    IL_1413:  stloc      V_485
    IL_1417:  stloc      V_486
    IL_141b:  stloc      V_487
    IL_141f:  stloc      V_488
    IL_1423:  stloc      V_489
    IL_1427:  stloc      V_490
    IL_142b:  stloc      V_491
    IL_142f:  ldc.i4.0
    IL_1430:  stloc      V_493
    IL_1434:  ldloc      V_444
    IL_1438:  ldc.i4.1
    IL_1439:  sub
    IL_143a:  stloc      V_492
    IL_143e:  ldloc      V_492
    IL_1442:  ldloc      V_493
    IL_1446:  blt.s      IL_1481

    .line 2,2 : 5,55 
    IL_1448:  ldloc      V_445
    IL_144c:  ldloc      V_493
    IL_1450:  ldloc      V_442
    IL_1454:  ldloc      V_443
    IL_1458:  ldloc      V_493
    IL_145c:  ldelem     [mscorlib]System.Int32
    IL_1461:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
    IL_1466:  stelem     [mscorlib]System.Int32
    IL_146b:  ldloc      V_493
    IL_146f:  ldc.i4.1
    IL_1470:  add
    IL_1471:  stloc      V_493
    .line 2,2 : 5,55 
    IL_1475:  ldloc      V_493
    IL_1479:  ldloc      V_492
    IL_147d:  ldc.i4.1
    IL_147e:  add
    IL_147f:  bne.un.s   IL_1448

    IL_1481:  ldloc      V_491
    IL_1485:  ldloc      V_490
    IL_1489:  ldloc      V_489
    IL_148d:  ldloc      V_488
    IL_1491:  ldloc      V_487
    IL_1495:  ldloc      V_486
    IL_1499:  ldloc      V_485
    IL_149d:  ldloc      V_484
    IL_14a1:  ldloc      V_483
    IL_14a5:  ldloc      V_482
    IL_14a9:  ldloc      V_481
    IL_14ad:  ldloc      V_480
    IL_14b1:  ldloc      V_479
    IL_14b5:  ldloc      V_478
    IL_14b9:  ldloc      V_477
    IL_14bd:  ldloc      V_476
    IL_14c1:  ldloc      V_475
    IL_14c5:  ldloc      V_474
    IL_14c9:  ldloc      V_473
    IL_14cd:  ldloc      V_472
    IL_14d1:  ldloc      V_471
    IL_14d5:  ldloc      V_470
    IL_14d9:  ldloc      V_469
    IL_14dd:  ldloc      V_468
    IL_14e1:  ldloc      V_467
    IL_14e5:  ldloc      V_466
    IL_14e9:  ldloc      V_465
    IL_14ed:  ldloc      V_464
    IL_14f1:  ldloc      V_463
    IL_14f5:  ldloc      V_462
    IL_14f9:  ldloc      V_461
    IL_14fd:  ldloc      V_460
    IL_1501:  ldloc      V_459
    IL_1505:  ldloc      V_458
    IL_1509:  ldloc      V_457
    IL_150d:  ldloc      V_456
    IL_1511:  ldloc      V_455
    IL_1515:  ldloc      V_454
    IL_1519:  ldloc      V_453
    IL_151d:  ldloc      V_452
    IL_1521:  ldloc      V_451
    IL_1525:  ldloc      V_450
    IL_1529:  ldloc      V_449
    IL_152d:  ldloc      V_448
    IL_1531:  ldloc      V_447
    IL_1535:  ldloc      V_446
    IL_1539:  ldloc      V_445
    IL_153d:  stloc      V_441
    IL_1541:  ldloc      V_441
    IL_1545:  stloc      V_494
    IL_1549:  ldloc      V_494
    IL_154d:  box        int32[]
    IL_1552:  ldc.i4.3
    IL_1553:  newarr     [mscorlib]System.Int32
    IL_1558:  dup
    IL_1559:  ldc.i4.0
    IL_155a:  ldc.i4.1
    IL_155b:  stelem     [mscorlib]System.Int32
    IL_1560:  dup
    IL_1561:  ldc.i4.1
    IL_1562:  ldc.i4.2
    IL_1563:  stelem     [mscorlib]System.Int32
    IL_1568:  dup
    IL_1569:  ldc.i4.2
    IL_156a:  ldc.i4.3
    IL_156b:  stelem     [mscorlib]System.Int32
    IL_1570:  stloc      V_496
    IL_1574:  ldloc      V_496
    IL_1578:  box        int32[]
    IL_157d:  brfalse.s  IL_1581

    IL_157f:  br.s       IL_1594

    .line 100001,100001 : 0,0 
    IL_1581:  ldstr      "array"
    IL_1586:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_158b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_1590:  pop
    .line 100001,100001 : 0,0 
    IL_1591:  nop
    IL_1592:  br.s       IL_1595

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_1594:  nop
    .line 100001,100001 : 0,0 
    IL_1595:  ldc.i4.0
    IL_1596:  stloc      V_497
    IL_159a:  stloc      V_498
    IL_159e:  stloc      V_499
    IL_15a2:  stloc      V_500
    IL_15a6:  stloc      V_501
    IL_15aa:  stloc      V_502
    IL_15ae:  stloc      V_503
    IL_15b2:  stloc      V_504
    IL_15b6:  stloc      V_505
    IL_15ba:  stloc      V_506
    IL_15be:  stloc      V_507
    IL_15c2:  stloc      V_508
    IL_15c6:  stloc      V_509
    IL_15ca:  stloc      V_510
    IL_15ce:  stloc      V_511
    IL_15d2:  stloc      V_512
    IL_15d6:  stloc      V_513
    IL_15da:  stloc      V_514
    IL_15de:  stloc      V_515
    IL_15e2:  stloc      V_516
    IL_15e6:  stloc      V_517
    IL_15ea:  stloc      V_518
    IL_15ee:  stloc      V_519
    IL_15f2:  stloc      V_520
    IL_15f6:  stloc      V_521
    IL_15fa:  stloc      V_522
    IL_15fe:  stloc      V_523
    IL_1602:  stloc      V_524
    IL_1606:  stloc      V_525
    IL_160a:  stloc      V_526
    IL_160e:  stloc      V_527
    IL_1612:  stloc      V_528
    IL_1616:  stloc      V_529
    IL_161a:  stloc      V_530
    IL_161e:  stloc      V_531
    IL_1622:  stloc      V_532
    IL_1626:  stloc      V_533
    IL_162a:  stloc      V_534
    IL_162e:  stloc      V_535
    IL_1632:  stloc      V_536
    IL_1636:  stloc      V_537
    IL_163a:  stloc      V_538
    IL_163e:  stloc      V_539
    IL_1642:  stloc      V_540
    IL_1646:  stloc      V_541
    IL_164a:  stloc      V_542
    IL_164e:  stloc      V_543
    IL_1652:  stloc      V_544
    IL_1656:  ldc.i4.0
    IL_1657:  stloc      V_545
    IL_165b:  br.s       IL_167d

    .line 2,2 : 5,55 
    IL_165d:  ldloc      V_497
    IL_1661:  ldloc      V_496
    IL_1665:  ldloc      V_545
    IL_1669:  ldelem     [mscorlib]System.Int32
    IL_166e:  add.ovf
    IL_166f:  stloc      V_497
    IL_1673:  ldloc      V_545
    IL_1677:  ldc.i4.1
    IL_1678:  add
    IL_1679:  stloc      V_545
    .line 2,2 : 5,55 
    IL_167d:  ldloc      V_545
    IL_1681:  ldloc      V_496
    IL_1685:  ldlen
    IL_1686:  conv.i4
    IL_1687:  blt.s      IL_165d

    IL_1689:  ldloc      V_544
    IL_168d:  ldloc      V_543
    IL_1691:  ldloc      V_542
    IL_1695:  ldloc      V_541
    IL_1699:  ldloc      V_540
    IL_169d:  ldloc      V_539
    IL_16a1:  ldloc      V_538
    IL_16a5:  ldloc      V_537
    IL_16a9:  ldloc      V_536
    IL_16ad:  ldloc      V_535
    IL_16b1:  ldloc      V_534
    IL_16b5:  ldloc      V_533
    IL_16b9:  ldloc      V_532
    IL_16bd:  ldloc      V_531
    IL_16c1:  ldloc      V_530
    IL_16c5:  ldloc      V_529
    IL_16c9:  ldloc      V_528
    IL_16cd:  ldloc      V_527
    IL_16d1:  ldloc      V_526
    IL_16d5:  ldloc      V_525
    IL_16d9:  ldloc      V_524
    IL_16dd:  ldloc      V_523
    IL_16e1:  ldloc      V_522
    IL_16e5:  ldloc      V_521
    IL_16e9:  ldloc      V_520
    IL_16ed:  ldloc      V_519
    IL_16f1:  ldloc      V_518
    IL_16f5:  ldloc      V_517
    IL_16f9:  ldloc      V_516
    IL_16fd:  ldloc      V_515
    IL_1701:  ldloc      V_514
    IL_1705:  ldloc      V_513
    IL_1709:  ldloc      V_512
    IL_170d:  ldloc      V_511
    IL_1711:  ldloc      V_510
    IL_1715:  ldloc      V_509
    IL_1719:  ldloc      V_508
    IL_171d:  ldloc      V_507
    IL_1721:  ldloc      V_506
    IL_1725:  ldloc      V_505
    IL_1729:  ldloc      V_504
    IL_172d:  ldloc      V_503
    IL_1731:  ldloc      V_502
    IL_1735:  ldloc      V_501
    IL_1739:  ldloc      V_500
    IL_173d:  ldloc      V_499
    IL_1741:  ldloc      V_498
    IL_1745:  ldloc      V_497
    IL_1749:  stloc      V_495
    IL_174d:  ldloc      V_495
    IL_1751:  stloc      V_546
    IL_1755:  ldloc      V_546
    IL_1759:  box        [mscorlib]System.Int32
    IL_175e:  newobj     instance void CallIntrinsics/'testIntrinsics@2-4'::.ctor()
    IL_1763:  stloc      V_548
    IL_1767:  ldc.i4.3
    IL_1768:  newarr     [mscorlib]System.Int32
    IL_176d:  dup
    IL_176e:  ldc.i4.0
    IL_176f:  ldc.i4.1
    IL_1770:  stelem     [mscorlib]System.Int32
    IL_1775:  dup
    IL_1776:  ldc.i4.1
    IL_1777:  ldc.i4.2
    IL_1778:  stelem     [mscorlib]System.Int32
    IL_177d:  dup
    IL_177e:  ldc.i4.2
    IL_177f:  ldc.i4.3
    IL_1780:  stelem     [mscorlib]System.Int32
    IL_1785:  stloc      V_549
    IL_1789:  ldloc      V_549
    IL_178d:  box        int32[]
    IL_1792:  brfalse.s  IL_1796

    IL_1794:  br.s       IL_17a9

    .line 100001,100001 : 0,0 
    IL_1796:  ldstr      "array"
    IL_179b:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_17a0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_17a5:  pop
    .line 100001,100001 : 0,0 
    IL_17a6:  nop
    IL_17a7:  br.s       IL_17aa

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_17a9:  nop
    .line 100001,100001 : 0,0 
    IL_17aa:  ldc.i4.0
    IL_17ab:  stloc      V_550
    IL_17af:  stloc      V_551
    IL_17b3:  stloc      V_552
    IL_17b7:  stloc      V_553
    IL_17bb:  stloc      V_554
    IL_17bf:  stloc      V_555
    IL_17c3:  stloc      V_556
    IL_17c7:  stloc      V_557
    IL_17cb:  stloc      V_558
    IL_17cf:  stloc      V_559
    IL_17d3:  stloc      V_560
    IL_17d7:  stloc      V_561
    IL_17db:  stloc      V_562
    IL_17df:  stloc      V_563
    IL_17e3:  stloc      V_564
    IL_17e7:  stloc      V_565
    IL_17eb:  stloc      V_566
    IL_17ef:  stloc      V_567
    IL_17f3:  stloc      V_568
    IL_17f7:  stloc      V_569
    IL_17fb:  stloc      V_570
    IL_17ff:  stloc      V_571
    IL_1803:  stloc      V_572
    IL_1807:  stloc      V_573
    IL_180b:  stloc      V_574
    IL_180f:  stloc      V_575
    IL_1813:  stloc      V_576
    IL_1817:  stloc      V_577
    IL_181b:  stloc      V_578
    IL_181f:  stloc      V_579
    IL_1823:  stloc      V_580
    IL_1827:  stloc      V_581
    IL_182b:  stloc      V_582
    IL_182f:  stloc      V_583
    IL_1833:  stloc      V_584
    IL_1837:  stloc      V_585
    IL_183b:  stloc      V_586
    IL_183f:  stloc      V_587
    IL_1843:  stloc      V_588
    IL_1847:  stloc      V_589
    IL_184b:  stloc      V_590
    IL_184f:  stloc      V_591
    IL_1853:  stloc      V_592
    IL_1857:  stloc      V_593
    IL_185b:  stloc      V_594
    IL_185f:  stloc      V_595
    IL_1863:  stloc      V_596
    IL_1867:  stloc      V_597
    IL_186b:  stloc      V_598
    IL_186f:  ldc.i4.0
    IL_1870:  stloc      V_599
    IL_1874:  br.s       IL_189f

    .line 2,2 : 5,55 
    IL_1876:  ldloc      V_550
    IL_187a:  ldloc      V_548
    IL_187e:  ldloc      V_549
    IL_1882:  ldloc      V_599
    IL_1886:  ldelem     [mscorlib]System.Int32
    IL_188b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
    IL_1890:  add.ovf
    IL_1891:  stloc      V_550
    IL_1895:  ldloc      V_599
    IL_1899:  ldc.i4.1
    IL_189a:  add
    IL_189b:  stloc      V_599
    .line 2,2 : 5,55 
    IL_189f:  ldloc      V_599
    IL_18a3:  ldloc      V_549
    IL_18a7:  ldlen
    IL_18a8:  conv.i4
    IL_18a9:  blt.s      IL_1876

    IL_18ab:  ldloc      V_598
    IL_18af:  ldloc      V_597
    IL_18b3:  ldloc      V_596
    IL_18b7:  ldloc      V_595
    IL_18bb:  ldloc      V_594
    IL_18bf:  ldloc      V_593
    IL_18c3:  ldloc      V_592
    IL_18c7:  ldloc      V_591
    IL_18cb:  ldloc      V_590
    IL_18cf:  ldloc      V_589
    IL_18d3:  ldloc      V_588
    IL_18d7:  ldloc      V_587
    IL_18db:  ldloc      V_586
    IL_18df:  ldloc      V_585
    IL_18e3:  ldloc      V_584
    IL_18e7:  ldloc      V_583
    IL_18eb:  ldloc      V_582
    IL_18ef:  ldloc      V_581
    IL_18f3:  ldloc      V_580
    IL_18f7:  ldloc      V_579
    IL_18fb:  ldloc      V_578
    IL_18ff:  ldloc      V_577
    IL_1903:  ldloc      V_576
    IL_1907:  ldloc      V_575
    IL_190b:  ldloc      V_574
    IL_190f:  ldloc      V_573
    IL_1913:  ldloc      V_572
    IL_1917:  ldloc      V_571
    IL_191b:  ldloc      V_570
    IL_191f:  ldloc      V_569
    IL_1923:  ldloc      V_568
    IL_1927:  ldloc      V_567
    IL_192b:  ldloc      V_566
    IL_192f:  ldloc      V_565
    IL_1933:  ldloc      V_564
    IL_1937:  ldloc      V_563
    IL_193b:  ldloc      V_562
    IL_193f:  ldloc      V_561
    IL_1943:  ldloc      V_560
    IL_1947:  ldloc      V_559
    IL_194b:  ldloc      V_558
    IL_194f:  ldloc      V_557
    IL_1953:  ldloc      V_556
    IL_1957:  ldloc      V_555
    IL_195b:  ldloc      V_554
    IL_195f:  ldloc      V_553
    IL_1963:  ldloc      V_552
    IL_1967:  ldloc      V_551
    IL_196b:  ldloc      V_550
    IL_196f:  stloc      V_547
    IL_1973:  ldloc      V_547
    IL_1977:  stloc      V_600
    IL_197b:  ldloc      V_600
    IL_197f:  box        [mscorlib]System.Int32
    IL_1984:  ldc.i4.3
    IL_1985:  newarr     [mscorlib]System.Double
    IL_198a:  dup
    IL_198b:  ldc.i4.0
    IL_198c:  ldc.r8     1.
    IL_1995:  stelem     [mscorlib]System.Double
    IL_199a:  dup
    IL_199b:  ldc.i4.1
    IL_199c:  ldc.r8     2.
    IL_19a5:  stelem     [mscorlib]System.Double
    IL_19aa:  dup
    IL_19ab:  ldc.i4.2
    IL_19ac:  ldc.r8     3.
    IL_19b5:  stelem     [mscorlib]System.Double
    IL_19ba:  stloc      V_602
    IL_19be:  ldloc      V_602
    IL_19c2:  box        float64[]
    IL_19c7:  brfalse.s  IL_19cb

    IL_19c9:  br.s       IL_19de

    .line 100001,100001 : 0,0 
    IL_19cb:  ldstr      "array"
    IL_19d0:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_19d5:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_19da:  pop
    .line 100001,100001 : 0,0 
    IL_19db:  nop
    IL_19dc:  br.s       IL_19df

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_19de:  nop
    .line 100001,100001 : 0,0 
    IL_19df:  ldloc      V_602
    IL_19e3:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_19e8:  stloc      V_603
    IL_19ec:  ldloc      V_603
    IL_19f0:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_19f5:  brfalse.s  IL_19f9

    IL_19f7:  br.s       IL_1a0c

    .line 100001,100001 : 0,0 
    IL_19f9:  ldstr      "source"
    IL_19fe:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_1a03:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_1a08:  pop
    .line 100001,100001 : 0,0 
    IL_1a09:  nop
    IL_1a0a:  br.s       IL_1a0d

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_1a0c:  nop
    .line 100001,100001 : 0,0 
    IL_1a0d:  ldloc      V_603
    IL_1a11:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_1a16:  stloc      V_604
    IL_1a1a:  stloc      V_605
    IL_1a1e:  stloc      V_606
    IL_1a22:  stloc      V_607
    IL_1a26:  stloc      V_608
    IL_1a2a:  stloc      V_609
    IL_1a2e:  stloc      V_610
    IL_1a32:  stloc      V_611
    IL_1a36:  stloc      V_612
    IL_1a3a:  stloc      V_613
    IL_1a3e:  stloc      V_614
    IL_1a42:  stloc      V_615
    IL_1a46:  stloc      V_616
    IL_1a4a:  stloc      V_617
    IL_1a4e:  stloc      V_618
    IL_1a52:  stloc      V_619
    IL_1a56:  stloc      V_620
    IL_1a5a:  stloc      V_621
    IL_1a5e:  stloc      V_622
    IL_1a62:  stloc      V_623
    IL_1a66:  stloc      V_624
    IL_1a6a:  stloc      V_625
    IL_1a6e:  stloc      V_626
    IL_1a72:  stloc      V_627
    IL_1a76:  stloc      V_628
    IL_1a7a:  stloc      V_629
    IL_1a7e:  stloc      V_630
    IL_1a82:  stloc      V_631
    IL_1a86:  stloc      V_632
    IL_1a8a:  stloc      V_633
    IL_1a8e:  stloc      V_634
    IL_1a92:  stloc      V_635
    IL_1a96:  stloc      V_636
    IL_1a9a:  stloc      V_637
    IL_1a9e:  stloc      V_638
    IL_1aa2:  stloc      V_639
    IL_1aa6:  stloc      V_640
    IL_1aaa:  stloc      V_641
    IL_1aae:  stloc      V_642
    IL_1ab2:  stloc      V_643
    IL_1ab6:  stloc      V_644
    IL_1aba:  stloc      V_645
    IL_1abe:  stloc      V_646
    IL_1ac2:  stloc      V_647
    IL_1ac6:  stloc      V_648
    IL_1aca:  stloc      V_649
    IL_1ace:  stloc      V_650
    IL_1ad2:  stloc      V_651
    IL_1ad6:  stloc      V_652
    IL_1ada:  stloc      V_653
    .try
    {
      IL_1ade:  ldc.r8     0.0
      IL_1ae7:  stloc      V_655
      IL_1aeb:  ldc.i4.0
      IL_1aec:  stloc      V_656
      IL_1af0:  ldloc      V_604
      IL_1af4:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_1af9:  brfalse.s  IL_1b1a

      IL_1afb:  ldloc      V_655
      IL_1aff:  ldloc      V_604
      IL_1b03:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_1b08:  add
      IL_1b09:  stloc      V_655
      .line 2,2 : 5,55 
      IL_1b0d:  ldloc      V_656
      IL_1b11:  ldc.i4.1
      IL_1b12:  add
      IL_1b13:  stloc      V_656
      .line 100001,100001 : 0,0 
      IL_1b17:  nop
      IL_1b18:  br.s       IL_1af0

      IL_1b1a:  ldloc      V_656
      IL_1b1e:  brtrue.s   IL_1b22

      IL_1b20:  br.s       IL_1b24

      IL_1b22:  br.s       IL_1b3c

      .line 100001,100001 : 0,0 
      IL_1b24:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_1b29:  ldstr      "source"
      IL_1b2e:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_1b33:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_1b38:  pop
      .line 100001,100001 : 0,0 
      IL_1b39:  nop
      IL_1b3a:  br.s       IL_1b3d

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_1b3c:  nop
      IL_1b3d:  ldloc      V_655
      IL_1b41:  stloc      V_657
      IL_1b45:  ldloc      V_656
      IL_1b49:  stloc      V_658
      IL_1b4d:  ldloc      V_657
      IL_1b51:  ldloc      V_658
      IL_1b55:  conv.r8
      IL_1b56:  div
      IL_1b57:  stloc      V_654
      IL_1b5b:  leave.s    IL_1b83

    }  // end .try
    finally
    {
      IL_1b5d:  ldloc      V_604
      IL_1b61:  isinst     [mscorlib]System.IDisposable
      IL_1b66:  stloc      V_659
      IL_1b6a:  ldloc      V_659
      IL_1b6e:  brfalse.s  IL_1b72

      IL_1b70:  br.s       IL_1b74

      IL_1b72:  br.s       IL_1b80

      .line 100001,100001 : 0,0 
      IL_1b74:  ldloc      V_659
      IL_1b78:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_1b7d:  ldnull
      IL_1b7e:  pop
      IL_1b7f:  endfinally
      .line 100001,100001 : 0,0 
      IL_1b80:  ldnull
      IL_1b81:  pop
      IL_1b82:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_1b83:  ldloc      V_653
    IL_1b87:  ldloc      V_652
    IL_1b8b:  ldloc      V_651
    IL_1b8f:  ldloc      V_650
    IL_1b93:  ldloc      V_649
    IL_1b97:  ldloc      V_648
    IL_1b9b:  ldloc      V_647
    IL_1b9f:  ldloc      V_646
    IL_1ba3:  ldloc      V_645
    IL_1ba7:  ldloc      V_644
    IL_1bab:  ldloc      V_643
    IL_1baf:  ldloc      V_642
    IL_1bb3:  ldloc      V_641
    IL_1bb7:  ldloc      V_640
    IL_1bbb:  ldloc      V_639
    IL_1bbf:  ldloc      V_638
    IL_1bc3:  ldloc      V_637
    IL_1bc7:  ldloc      V_636
    IL_1bcb:  ldloc      V_635
    IL_1bcf:  ldloc      V_634
    IL_1bd3:  ldloc      V_633
    IL_1bd7:  ldloc      V_632
    IL_1bdb:  ldloc      V_631
    IL_1bdf:  ldloc      V_630
    IL_1be3:  ldloc      V_629
    IL_1be7:  ldloc      V_628
    IL_1beb:  ldloc      V_627
    IL_1bef:  ldloc      V_626
    IL_1bf3:  ldloc      V_625
    IL_1bf7:  ldloc      V_624
    IL_1bfb:  ldloc      V_623
    IL_1bff:  ldloc      V_622
    IL_1c03:  ldloc      V_621
    IL_1c07:  ldloc      V_620
    IL_1c0b:  ldloc      V_619
    IL_1c0f:  ldloc      V_618
    IL_1c13:  ldloc      V_617
    IL_1c17:  ldloc      V_616
    IL_1c1b:  ldloc      V_615
    IL_1c1f:  ldloc      V_614
    IL_1c23:  ldloc      V_613
    IL_1c27:  ldloc      V_612
    IL_1c2b:  ldloc      V_611
    IL_1c2f:  ldloc      V_610
    IL_1c33:  ldloc      V_609
    IL_1c37:  ldloc      V_608
    IL_1c3b:  ldloc      V_607
    IL_1c3f:  ldloc      V_606
    IL_1c43:  ldloc      V_605
    IL_1c47:  ldloc      V_654
    IL_1c4b:  stloc      V_601
    IL_1c4f:  ldloc      V_601
    IL_1c53:  stloc      V_660
    IL_1c57:  ldloc      V_660
    IL_1c5b:  box        [mscorlib]System.Double
    IL_1c60:  newobj     instance void CallIntrinsics/'testIntrinsics@2-5'::.ctor()
    IL_1c65:  stloc      V_662
    IL_1c69:  ldc.i4.3
    IL_1c6a:  newarr     [mscorlib]System.Double
    IL_1c6f:  dup
    IL_1c70:  ldc.i4.0
    IL_1c71:  ldc.r8     1.
    IL_1c7a:  stelem     [mscorlib]System.Double
    IL_1c7f:  dup
    IL_1c80:  ldc.i4.1
    IL_1c81:  ldc.r8     2.
    IL_1c8a:  stelem     [mscorlib]System.Double
    IL_1c8f:  dup
    IL_1c90:  ldc.i4.2
    IL_1c91:  ldc.r8     3.
    IL_1c9a:  stelem     [mscorlib]System.Double
    IL_1c9f:  stloc      V_663
    IL_1ca3:  ldloc      V_663
    IL_1ca7:  box        float64[]
    IL_1cac:  brfalse.s  IL_1cb0

    IL_1cae:  br.s       IL_1cc3

    .line 100001,100001 : 0,0 
    IL_1cb0:  ldstr      "array"
    IL_1cb5:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_1cba:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_1cbf:  pop
    .line 100001,100001 : 0,0 
    IL_1cc0:  nop
    IL_1cc1:  br.s       IL_1cc4

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_1cc3:  nop
    .line 100001,100001 : 0,0 
    IL_1cc4:  ldloc      V_663
    IL_1cc8:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_1ccd:  stloc      V_664
    IL_1cd1:  ldloc      V_664
    IL_1cd5:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_1cda:  brfalse.s  IL_1cde

    IL_1cdc:  br.s       IL_1cf1

    .line 100001,100001 : 0,0 
    IL_1cde:  ldstr      "source"
    IL_1ce3:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_1ce8:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_1ced:  pop
    .line 100001,100001 : 0,0 
    IL_1cee:  nop
    IL_1cef:  br.s       IL_1cf2

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_1cf1:  nop
    .line 100001,100001 : 0,0 
    IL_1cf2:  ldloc      V_664
    IL_1cf6:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_1cfb:  stloc      V_665
    IL_1cff:  stloc      V_666
    IL_1d03:  stloc      V_667
    IL_1d07:  stloc      V_668
    IL_1d0b:  stloc      V_669
    IL_1d0f:  stloc      V_670
    IL_1d13:  stloc      V_671
    IL_1d17:  stloc      V_672
    IL_1d1b:  stloc      V_673
    IL_1d1f:  stloc      V_674
    IL_1d23:  stloc      V_675
    IL_1d27:  stloc      V_676
    IL_1d2b:  stloc      V_677
    IL_1d2f:  stloc      V_678
    IL_1d33:  stloc      V_679
    IL_1d37:  stloc      V_680
    IL_1d3b:  stloc      V_681
    IL_1d3f:  stloc      V_682
    IL_1d43:  stloc      V_683
    IL_1d47:  stloc      V_684
    IL_1d4b:  stloc      V_685
    IL_1d4f:  stloc      V_686
    IL_1d53:  stloc      V_687
    IL_1d57:  stloc      V_688
    IL_1d5b:  stloc      V_689
    IL_1d5f:  stloc      V_690
    IL_1d63:  stloc      V_691
    IL_1d67:  stloc      V_692
    IL_1d6b:  stloc      V_693
    IL_1d6f:  stloc      V_694
    IL_1d73:  stloc      V_695
    IL_1d77:  stloc      V_696
    IL_1d7b:  stloc      V_697
    IL_1d7f:  stloc      V_698
    IL_1d83:  stloc      V_699
    IL_1d87:  stloc      V_700
    IL_1d8b:  stloc      V_701
    IL_1d8f:  stloc      V_702
    IL_1d93:  stloc      V_703
    IL_1d97:  stloc      V_704
    IL_1d9b:  stloc      V_705
    IL_1d9f:  stloc      V_706
    IL_1da3:  stloc      V_707
    IL_1da7:  stloc      V_708
    IL_1dab:  stloc      V_709
    IL_1daf:  stloc      V_710
    IL_1db3:  stloc      V_711
    IL_1db7:  stloc      V_712
    IL_1dbb:  stloc      V_713
    IL_1dbf:  stloc      V_714
    IL_1dc3:  stloc      V_715
    .try
    {
      IL_1dc7:  ldc.r8     0.0
      IL_1dd0:  stloc      V_717
      IL_1dd4:  ldc.i4.0
      IL_1dd5:  stloc      V_718
      IL_1dd9:  ldloc      V_665
      IL_1ddd:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_1de2:  brfalse.s  IL_1e0c

      IL_1de4:  ldloc      V_717
      IL_1de8:  ldloc      V_662
      IL_1dec:  ldloc      V_665
      IL_1df0:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_1df5:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_1dfa:  add
      IL_1dfb:  stloc      V_717
      .line 2,2 : 5,55 
      IL_1dff:  ldloc      V_718
      IL_1e03:  ldc.i4.1
      IL_1e04:  add
      IL_1e05:  stloc      V_718
      .line 100001,100001 : 0,0 
      IL_1e09:  nop
      IL_1e0a:  br.s       IL_1dd9

      IL_1e0c:  ldloc      V_718
      IL_1e10:  brtrue.s   IL_1e14

      IL_1e12:  br.s       IL_1e16

      IL_1e14:  br.s       IL_1e2e

      .line 100001,100001 : 0,0 
      IL_1e16:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_1e1b:  ldstr      "source"
      IL_1e20:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_1e25:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_1e2a:  pop
      .line 100001,100001 : 0,0 
      IL_1e2b:  nop
      IL_1e2c:  br.s       IL_1e2f

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_1e2e:  nop
      IL_1e2f:  ldloc      V_717
      IL_1e33:  stloc      V_719
      IL_1e37:  ldloc      V_718
      IL_1e3b:  stloc      V_720
      IL_1e3f:  ldloc      V_719
      IL_1e43:  ldloc      V_720
      IL_1e47:  conv.r8
      IL_1e48:  div
      IL_1e49:  stloc      V_716
      IL_1e4d:  leave.s    IL_1e75

    }  // end .try
    finally
    {
      IL_1e4f:  ldloc      V_665
      IL_1e53:  isinst     [mscorlib]System.IDisposable
      IL_1e58:  stloc      V_721
      IL_1e5c:  ldloc      V_721
      IL_1e60:  brfalse.s  IL_1e64

      IL_1e62:  br.s       IL_1e66

      IL_1e64:  br.s       IL_1e72

      .line 100001,100001 : 0,0 
      IL_1e66:  ldloc      V_721
      IL_1e6a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_1e6f:  ldnull
      IL_1e70:  pop
      IL_1e71:  endfinally
      .line 100001,100001 : 0,0 
      IL_1e72:  ldnull
      IL_1e73:  pop
      IL_1e74:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_1e75:  ldloc      V_715
    IL_1e79:  ldloc      V_714
    IL_1e7d:  ldloc      V_713
    IL_1e81:  ldloc      V_712
    IL_1e85:  ldloc      V_711
    IL_1e89:  ldloc      V_710
    IL_1e8d:  ldloc      V_709
    IL_1e91:  ldloc      V_708
    IL_1e95:  ldloc      V_707
    IL_1e99:  ldloc      V_706
    IL_1e9d:  ldloc      V_705
    IL_1ea1:  ldloc      V_704
    IL_1ea5:  ldloc      V_703
    IL_1ea9:  ldloc      V_702
    IL_1ead:  ldloc      V_701
    IL_1eb1:  ldloc      V_700
    IL_1eb5:  ldloc      V_699
    IL_1eb9:  ldloc      V_698
    IL_1ebd:  ldloc      V_697
    IL_1ec1:  ldloc      V_696
    IL_1ec5:  ldloc      V_695
    IL_1ec9:  ldloc      V_694
    IL_1ecd:  ldloc      V_693
    IL_1ed1:  ldloc      V_692
    IL_1ed5:  ldloc      V_691
    IL_1ed9:  ldloc      V_690
    IL_1edd:  ldloc      V_689
    IL_1ee1:  ldloc      V_688
    IL_1ee5:  ldloc      V_687
    IL_1ee9:  ldloc      V_686
    IL_1eed:  ldloc      V_685
    IL_1ef1:  ldloc      V_684
    IL_1ef5:  ldloc      V_683
    IL_1ef9:  ldloc      V_682
    IL_1efd:  ldloc      V_681
    IL_1f01:  ldloc      V_680
    IL_1f05:  ldloc      V_679
    IL_1f09:  ldloc      V_678
    IL_1f0d:  ldloc      V_677
    IL_1f11:  ldloc      V_676
    IL_1f15:  ldloc      V_675
    IL_1f19:  ldloc      V_674
    IL_1f1d:  ldloc      V_673
    IL_1f21:  ldloc      V_672
    IL_1f25:  ldloc      V_671
    IL_1f29:  ldloc      V_670
    IL_1f2d:  ldloc      V_669
    IL_1f31:  ldloc      V_668
    IL_1f35:  ldloc      V_667
    IL_1f39:  ldloc      V_666
    IL_1f3d:  ldloc      V_716
    IL_1f41:  stloc      V_661
    IL_1f45:  ldloc      V_661
    IL_1f49:  stloc      V_722
    IL_1f4d:  ldloc      V_722
    IL_1f51:  box        [mscorlib]System.Double
    IL_1f56:  newobj     instance void CallIntrinsics/'testIntrinsics@2-6'::.ctor()
    IL_1f5b:  ldc.i4.1
    IL_1f5c:  ldc.i4.2
    IL_1f5d:  ldc.i4.3
    IL_1f5e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_1f63:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1f68:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1f6d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1f72:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_1f77:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_1f7c:  stloc      V_723
    IL_1f80:  ldloc      V_723
    IL_1f84:  stloc      V_724
    IL_1f88:  ldloc      V_724
    IL_1f8c:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_1f91:  ldc.i4.1
    IL_1f92:  ldc.i4.2
    IL_1f93:  ldc.i4.3
    IL_1f94:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_1f99:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1f9e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1fa3:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_1fa8:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_1fad:  stloc      V_726
    IL_1fb1:  ldloc      V_726
    IL_1fb5:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_1fba:  stloc      V_727
    IL_1fbe:  stloc      V_728
    IL_1fc2:  stloc      V_729
    IL_1fc6:  stloc      V_730
    IL_1fca:  stloc      V_731
    IL_1fce:  stloc      V_732
    IL_1fd2:  stloc      V_733
    IL_1fd6:  stloc      V_734
    IL_1fda:  stloc      V_735
    IL_1fde:  stloc      V_736
    IL_1fe2:  stloc      V_737
    IL_1fe6:  stloc      V_738
    IL_1fea:  stloc      V_739
    IL_1fee:  stloc      V_740
    IL_1ff2:  stloc      V_741
    IL_1ff6:  stloc      V_742
    IL_1ffa:  stloc      V_743
    IL_1ffe:  stloc      V_744
    IL_2002:  stloc      V_745
    IL_2006:  stloc      V_746
    IL_200a:  stloc      V_747
    IL_200e:  stloc      V_748
    IL_2012:  stloc      V_749
    IL_2016:  stloc      V_750
    IL_201a:  stloc      V_751
    IL_201e:  stloc      V_752
    IL_2022:  stloc      V_753
    IL_2026:  stloc      V_754
    IL_202a:  stloc      V_755
    IL_202e:  stloc      V_756
    IL_2032:  stloc      V_757
    IL_2036:  stloc      V_758
    IL_203a:  stloc      V_759
    IL_203e:  stloc      V_760
    IL_2042:  stloc      V_761
    IL_2046:  stloc      V_762
    IL_204a:  stloc      V_763
    IL_204e:  stloc      V_764
    IL_2052:  stloc      V_765
    IL_2056:  stloc      V_766
    IL_205a:  stloc      V_767
    IL_205e:  stloc      V_768
    IL_2062:  stloc      V_769
    IL_2066:  stloc      V_770
    IL_206a:  stloc      V_771
    IL_206e:  stloc      V_772
    IL_2072:  stloc      V_773
    IL_2076:  stloc      V_774
    IL_207a:  stloc      V_775
    IL_207e:  stloc      V_776
    IL_2082:  stloc      V_777
    IL_2086:  stloc      V_778
    IL_208a:  stloc      V_779
    .try
    {
      IL_208e:  ldc.i4.0
      IL_208f:  stloc      V_781
      IL_2093:  ldloc      V_727
      IL_2097:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_209c:  brfalse.s  IL_20b3

      .line 2,2 : 5,55 
      IL_209e:  ldloc      V_781
      IL_20a2:  ldloc      V_727
      IL_20a6:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_20ab:  add.ovf
      IL_20ac:  stloc      V_781
      .line 100001,100001 : 0,0 
      IL_20b0:  nop
      IL_20b1:  br.s       IL_2093

      IL_20b3:  ldloc      V_781
      IL_20b7:  stloc      V_780
      IL_20bb:  leave.s    IL_20e3

    }  // end .try
    finally
    {
      IL_20bd:  ldloc      V_727
      IL_20c1:  isinst     [mscorlib]System.IDisposable
      IL_20c6:  stloc      V_782
      IL_20ca:  ldloc      V_782
      IL_20ce:  brfalse.s  IL_20d2

      IL_20d0:  br.s       IL_20d4

      IL_20d2:  br.s       IL_20e0

      .line 100001,100001 : 0,0 
      IL_20d4:  ldloc      V_782
      IL_20d8:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_20dd:  ldnull
      IL_20de:  pop
      IL_20df:  endfinally
      .line 100001,100001 : 0,0 
      IL_20e0:  ldnull
      IL_20e1:  pop
      IL_20e2:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_20e3:  ldloc      V_779
    IL_20e7:  ldloc      V_778
    IL_20eb:  ldloc      V_777
    IL_20ef:  ldloc      V_776
    IL_20f3:  ldloc      V_775
    IL_20f7:  ldloc      V_774
    IL_20fb:  ldloc      V_773
    IL_20ff:  ldloc      V_772
    IL_2103:  ldloc      V_771
    IL_2107:  ldloc      V_770
    IL_210b:  ldloc      V_769
    IL_210f:  ldloc      V_768
    IL_2113:  ldloc      V_767
    IL_2117:  ldloc      V_766
    IL_211b:  ldloc      V_765
    IL_211f:  ldloc      V_764
    IL_2123:  ldloc      V_763
    IL_2127:  ldloc      V_762
    IL_212b:  ldloc      V_761
    IL_212f:  ldloc      V_760
    IL_2133:  ldloc      V_759
    IL_2137:  ldloc      V_758
    IL_213b:  ldloc      V_757
    IL_213f:  ldloc      V_756
    IL_2143:  ldloc      V_755
    IL_2147:  ldloc      V_754
    IL_214b:  ldloc      V_753
    IL_214f:  ldloc      V_752
    IL_2153:  ldloc      V_751
    IL_2157:  ldloc      V_750
    IL_215b:  ldloc      V_749
    IL_215f:  ldloc      V_748
    IL_2163:  ldloc      V_747
    IL_2167:  ldloc      V_746
    IL_216b:  ldloc      V_745
    IL_216f:  ldloc      V_744
    IL_2173:  ldloc      V_743
    IL_2177:  ldloc      V_742
    IL_217b:  ldloc      V_741
    IL_217f:  ldloc      V_740
    IL_2183:  ldloc      V_739
    IL_2187:  ldloc      V_738
    IL_218b:  ldloc      V_737
    IL_218f:  ldloc      V_736
    IL_2193:  ldloc      V_735
    IL_2197:  ldloc      V_734
    IL_219b:  ldloc      V_733
    IL_219f:  ldloc      V_732
    IL_21a3:  ldloc      V_731
    IL_21a7:  ldloc      V_730
    IL_21ab:  ldloc      V_729
    IL_21af:  ldloc      V_728
    IL_21b3:  ldloc      V_780
    IL_21b7:  stloc      V_725
    IL_21bb:  ldloc      V_725
    IL_21bf:  stloc      V_783
    IL_21c3:  ldloc      V_783
    IL_21c7:  box        [mscorlib]System.Int32
    IL_21cc:  newobj     instance void CallIntrinsics/'testIntrinsics@2-7'::.ctor()
    IL_21d1:  stloc      V_785
    IL_21d5:  ldc.i4.1
    IL_21d6:  ldc.i4.2
    IL_21d7:  ldc.i4.3
    IL_21d8:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_21dd:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_21e2:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_21e7:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_21ec:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_21f1:  stloc      V_786
    IL_21f5:  ldloc      V_786
    IL_21f9:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_21fe:  stloc      V_787
    IL_2202:  stloc      V_788
    IL_2206:  stloc      V_789
    IL_220a:  stloc      V_790
    IL_220e:  stloc      V_791
    IL_2212:  stloc      V_792
    IL_2216:  stloc      V_793
    IL_221a:  stloc      V_794
    IL_221e:  stloc      V_795
    IL_2222:  stloc      V_796
    IL_2226:  stloc      V_797
    IL_222a:  stloc      V_798
    IL_222e:  stloc      V_799
    IL_2232:  stloc      V_800
    IL_2236:  stloc      V_801
    IL_223a:  stloc      V_802
    IL_223e:  stloc      V_803
    IL_2242:  stloc      V_804
    IL_2246:  stloc      V_805
    IL_224a:  stloc      V_806
    IL_224e:  stloc      V_807
    IL_2252:  stloc      V_808
    IL_2256:  stloc      V_809
    IL_225a:  stloc      V_810
    IL_225e:  stloc      V_811
    IL_2262:  stloc      V_812
    IL_2266:  stloc      V_813
    IL_226a:  stloc      V_814
    IL_226e:  stloc      V_815
    IL_2272:  stloc      V_816
    IL_2276:  stloc      V_817
    IL_227a:  stloc      V_818
    IL_227e:  stloc      V_819
    IL_2282:  stloc      V_820
    IL_2286:  stloc      V_821
    IL_228a:  stloc      V_822
    IL_228e:  stloc      V_823
    IL_2292:  stloc      V_824
    IL_2296:  stloc      V_825
    IL_229a:  stloc      V_826
    IL_229e:  stloc      V_827
    IL_22a2:  stloc      V_828
    IL_22a6:  stloc      V_829
    IL_22aa:  stloc      V_830
    IL_22ae:  stloc      V_831
    IL_22b2:  stloc      V_832
    IL_22b6:  stloc      V_833
    IL_22ba:  stloc      V_834
    IL_22be:  stloc      V_835
    IL_22c2:  stloc      V_836
    IL_22c6:  stloc      V_837
    IL_22ca:  stloc      V_838
    IL_22ce:  stloc      V_839
    IL_22d2:  stloc      V_840
    .try
    {
      IL_22d6:  ldc.i4.0
      IL_22d7:  stloc      V_842
      IL_22db:  ldloc      V_787
      IL_22df:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_22e4:  brfalse.s  IL_2304

      .line 2,2 : 5,55 
      IL_22e6:  ldloc      V_842
      IL_22ea:  ldloc      V_785
      IL_22ee:  ldloc      V_787
      IL_22f2:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_22f7:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_22fc:  add.ovf
      IL_22fd:  stloc      V_842
      .line 100001,100001 : 0,0 
      IL_2301:  nop
      IL_2302:  br.s       IL_22db

      IL_2304:  ldloc      V_842
      IL_2308:  stloc      V_841
      IL_230c:  leave.s    IL_2334

    }  // end .try
    finally
    {
      IL_230e:  ldloc      V_787
      IL_2312:  isinst     [mscorlib]System.IDisposable
      IL_2317:  stloc      V_843
      IL_231b:  ldloc      V_843
      IL_231f:  brfalse.s  IL_2323

      IL_2321:  br.s       IL_2325

      IL_2323:  br.s       IL_2331

      .line 100001,100001 : 0,0 
      IL_2325:  ldloc      V_843
      IL_2329:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_232e:  ldnull
      IL_232f:  pop
      IL_2330:  endfinally
      .line 100001,100001 : 0,0 
      IL_2331:  ldnull
      IL_2332:  pop
      IL_2333:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_2334:  ldloc      V_840
    IL_2338:  ldloc      V_839
    IL_233c:  ldloc      V_838
    IL_2340:  ldloc      V_837
    IL_2344:  ldloc      V_836
    IL_2348:  ldloc      V_835
    IL_234c:  ldloc      V_834
    IL_2350:  ldloc      V_833
    IL_2354:  ldloc      V_832
    IL_2358:  ldloc      V_831
    IL_235c:  ldloc      V_830
    IL_2360:  ldloc      V_829
    IL_2364:  ldloc      V_828
    IL_2368:  ldloc      V_827
    IL_236c:  ldloc      V_826
    IL_2370:  ldloc      V_825
    IL_2374:  ldloc      V_824
    IL_2378:  ldloc      V_823
    IL_237c:  ldloc      V_822
    IL_2380:  ldloc      V_821
    IL_2384:  ldloc      V_820
    IL_2388:  ldloc      V_819
    IL_238c:  ldloc      V_818
    IL_2390:  ldloc      V_817
    IL_2394:  ldloc      V_816
    IL_2398:  ldloc      V_815
    IL_239c:  ldloc      V_814
    IL_23a0:  ldloc      V_813
    IL_23a4:  ldloc      V_812
    IL_23a8:  ldloc      V_811
    IL_23ac:  ldloc      V_810
    IL_23b0:  ldloc      V_809
    IL_23b4:  ldloc      V_808
    IL_23b8:  ldloc      V_807
    IL_23bc:  ldloc      V_806
    IL_23c0:  ldloc      V_805
    IL_23c4:  ldloc      V_804
    IL_23c8:  ldloc      V_803
    IL_23cc:  ldloc      V_802
    IL_23d0:  ldloc      V_801
    IL_23d4:  ldloc      V_800
    IL_23d8:  ldloc      V_799
    IL_23dc:  ldloc      V_798
    IL_23e0:  ldloc      V_797
    IL_23e4:  ldloc      V_796
    IL_23e8:  ldloc      V_795
    IL_23ec:  ldloc      V_794
    IL_23f0:  ldloc      V_793
    IL_23f4:  ldloc      V_792
    IL_23f8:  ldloc      V_791
    IL_23fc:  ldloc      V_790
    IL_2400:  ldloc      V_789
    IL_2404:  ldloc      V_788
    IL_2408:  ldloc      V_841
    IL_240c:  stloc      V_784
    IL_2410:  ldloc      V_784
    IL_2414:  stloc      V_844
    IL_2418:  ldloc      V_844
    IL_241c:  box        [mscorlib]System.Int32
    IL_2421:  ldc.r8     1.
    IL_242a:  ldc.r8     2.
    IL_2433:  ldc.r8     3.
    IL_243c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_2441:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2446:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_244b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2450:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_2455:  stloc      V_846
    IL_2459:  ldloc      V_846
    IL_245d:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_2462:  brfalse.s  IL_2466

    IL_2464:  br.s       IL_2479

    .line 100001,100001 : 0,0 
    IL_2466:  ldstr      "source"
    IL_246b:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_2470:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_2475:  pop
    .line 100001,100001 : 0,0 
    IL_2476:  nop
    IL_2477:  br.s       IL_247a

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_2479:  nop
    .line 100001,100001 : 0,0 
    IL_247a:  ldloc      V_846
    IL_247e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_2483:  stloc      V_847
    IL_2487:  stloc      V_848
    IL_248b:  stloc      V_849
    IL_248f:  stloc      V_850
    IL_2493:  stloc      V_851
    IL_2497:  stloc      V_852
    IL_249b:  stloc      V_853
    IL_249f:  stloc      V_854
    IL_24a3:  stloc      V_855
    IL_24a7:  stloc      V_856
    IL_24ab:  stloc      V_857
    IL_24af:  stloc      V_858
    IL_24b3:  stloc      V_859
    IL_24b7:  stloc      V_860
    IL_24bb:  stloc      V_861
    IL_24bf:  stloc      V_862
    IL_24c3:  stloc      V_863
    IL_24c7:  stloc      V_864
    IL_24cb:  stloc      V_865
    IL_24cf:  stloc      V_866
    IL_24d3:  stloc      V_867
    IL_24d7:  stloc      V_868
    IL_24db:  stloc      V_869
    IL_24df:  stloc      V_870
    IL_24e3:  stloc      V_871
    IL_24e7:  stloc      V_872
    IL_24eb:  stloc      V_873
    IL_24ef:  stloc      V_874
    IL_24f3:  stloc      V_875
    IL_24f7:  stloc      V_876
    IL_24fb:  stloc      V_877
    IL_24ff:  stloc      V_878
    IL_2503:  stloc      V_879
    IL_2507:  stloc      V_880
    IL_250b:  stloc      V_881
    IL_250f:  stloc      V_882
    IL_2513:  stloc      V_883
    IL_2517:  stloc      V_884
    IL_251b:  stloc      V_885
    IL_251f:  stloc      V_886
    IL_2523:  stloc      V_887
    IL_2527:  stloc      V_888
    IL_252b:  stloc      V_889
    IL_252f:  stloc      V_890
    IL_2533:  stloc      V_891
    IL_2537:  stloc      V_892
    IL_253b:  stloc      V_893
    IL_253f:  stloc      V_894
    IL_2543:  stloc      V_895
    IL_2547:  stloc      V_896
    IL_254b:  stloc      V_897
    IL_254f:  stloc      V_898
    IL_2553:  stloc      V_899
    IL_2557:  stloc      V_900
    IL_255b:  stloc      V_901
    .try
    {
      IL_255f:  ldc.r8     0.0
      IL_2568:  stloc      V_903
      IL_256c:  ldc.i4.0
      IL_256d:  stloc      V_904
      IL_2571:  ldloc      V_847
      IL_2575:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_257a:  brfalse.s  IL_259b

      IL_257c:  ldloc      V_903
      IL_2580:  ldloc      V_847
      IL_2584:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_2589:  add
      IL_258a:  stloc      V_903
      .line 2,2 : 5,55 
      IL_258e:  ldloc      V_904
      IL_2592:  ldc.i4.1
      IL_2593:  add
      IL_2594:  stloc      V_904
      .line 100001,100001 : 0,0 
      IL_2598:  nop
      IL_2599:  br.s       IL_2571

      IL_259b:  ldloc      V_904
      IL_259f:  brtrue.s   IL_25a3

      IL_25a1:  br.s       IL_25a5

      IL_25a3:  br.s       IL_25bd

      .line 100001,100001 : 0,0 
      IL_25a5:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_25aa:  ldstr      "source"
      IL_25af:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_25b4:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_25b9:  pop
      .line 100001,100001 : 0,0 
      IL_25ba:  nop
      IL_25bb:  br.s       IL_25be

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_25bd:  nop
      IL_25be:  ldloc      V_903
      IL_25c2:  stloc      V_905
      IL_25c6:  ldloc      V_904
      IL_25ca:  stloc      V_906
      IL_25ce:  ldloc      V_905
      IL_25d2:  ldloc      V_906
      IL_25d6:  conv.r8
      IL_25d7:  div
      IL_25d8:  stloc      V_902
      IL_25dc:  leave.s    IL_2604

    }  // end .try
    finally
    {
      IL_25de:  ldloc      V_847
      IL_25e2:  isinst     [mscorlib]System.IDisposable
      IL_25e7:  stloc      V_907
      IL_25eb:  ldloc      V_907
      IL_25ef:  brfalse.s  IL_25f3

      IL_25f1:  br.s       IL_25f5

      IL_25f3:  br.s       IL_2601

      .line 100001,100001 : 0,0 
      IL_25f5:  ldloc      V_907
      IL_25f9:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_25fe:  ldnull
      IL_25ff:  pop
      IL_2600:  endfinally
      .line 100001,100001 : 0,0 
      IL_2601:  ldnull
      IL_2602:  pop
      IL_2603:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_2604:  ldloc      V_901
    IL_2608:  ldloc      V_900
    IL_260c:  ldloc      V_899
    IL_2610:  ldloc      V_898
    IL_2614:  ldloc      V_897
    IL_2618:  ldloc      V_896
    IL_261c:  ldloc      V_895
    IL_2620:  ldloc      V_894
    IL_2624:  ldloc      V_893
    IL_2628:  ldloc      V_892
    IL_262c:  ldloc      V_891
    IL_2630:  ldloc      V_890
    IL_2634:  ldloc      V_889
    IL_2638:  ldloc      V_888
    IL_263c:  ldloc      V_887
    IL_2640:  ldloc      V_886
    IL_2644:  ldloc      V_885
    IL_2648:  ldloc      V_884
    IL_264c:  ldloc      V_883
    IL_2650:  ldloc      V_882
    IL_2654:  ldloc      V_881
    IL_2658:  ldloc      V_880
    IL_265c:  ldloc      V_879
    IL_2660:  ldloc      V_878
    IL_2664:  ldloc      V_877
    IL_2668:  ldloc      V_876
    IL_266c:  ldloc      V_875
    IL_2670:  ldloc      V_874
    IL_2674:  ldloc      V_873
    IL_2678:  ldloc      V_872
    IL_267c:  ldloc      V_871
    IL_2680:  ldloc      V_870
    IL_2684:  ldloc      V_869
    IL_2688:  ldloc      V_868
    IL_268c:  ldloc      V_867
    IL_2690:  ldloc      V_866
    IL_2694:  ldloc      V_865
    IL_2698:  ldloc      V_864
    IL_269c:  ldloc      V_863
    IL_26a0:  ldloc      V_862
    IL_26a4:  ldloc      V_861
    IL_26a8:  ldloc      V_860
    IL_26ac:  ldloc      V_859
    IL_26b0:  ldloc      V_858
    IL_26b4:  ldloc      V_857
    IL_26b8:  ldloc      V_856
    IL_26bc:  ldloc      V_855
    IL_26c0:  ldloc      V_854
    IL_26c4:  ldloc      V_853
    IL_26c8:  ldloc      V_852
    IL_26cc:  ldloc      V_851
    IL_26d0:  ldloc      V_850
    IL_26d4:  ldloc      V_849
    IL_26d8:  ldloc      V_848
    IL_26dc:  ldloc      V_902
    IL_26e0:  stloc      V_845
    IL_26e4:  ldloc      V_845
    IL_26e8:  stloc      V_908
    IL_26ec:  ldloc      V_908
    IL_26f0:  box        [mscorlib]System.Double
    IL_26f5:  newobj     instance void CallIntrinsics/'testIntrinsics@2-8'::.ctor()
    IL_26fa:  stloc      V_910
    IL_26fe:  ldc.r8     1.
    IL_2707:  ldc.r8     2.
    IL_2710:  ldc.r8     3.
    IL_2719:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_271e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2723:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2728:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                          class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_272d:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_2732:  stloc      V_911
    IL_2736:  ldloc      V_911
    IL_273a:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>
    IL_273f:  brfalse.s  IL_2743

    IL_2741:  br.s       IL_2756

    .line 100001,100001 : 0,0 
    IL_2743:  ldstr      "source"
    IL_2748:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_274d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
    IL_2752:  pop
    .line 100001,100001 : 0,0 
    IL_2753:  nop
    IL_2754:  br.s       IL_2757

    .line 100001,100001 : 0,0 
    .line 100001,100001 : 0,0 
    IL_2756:  nop
    .line 100001,100001 : 0,0 
    IL_2757:  ldloc      V_911
    IL_275b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_2760:  stloc      V_912
    IL_2764:  stloc      V_913
    IL_2768:  stloc      V_914
    IL_276c:  stloc      V_915
    IL_2770:  stloc      V_916
    IL_2774:  stloc      V_917
    IL_2778:  stloc      V_918
    IL_277c:  stloc      V_919
    IL_2780:  stloc      V_920
    IL_2784:  stloc      V_921
    IL_2788:  stloc      V_922
    IL_278c:  stloc      V_923
    IL_2790:  stloc      V_924
    IL_2794:  stloc      V_925
    IL_2798:  stloc      V_926
    IL_279c:  stloc      V_927
    IL_27a0:  stloc      V_928
    IL_27a4:  stloc      V_929
    IL_27a8:  stloc      V_930
    IL_27ac:  stloc      V_931
    IL_27b0:  stloc      V_932
    IL_27b4:  stloc      V_933
    IL_27b8:  stloc      V_934
    IL_27bc:  stloc      V_935
    IL_27c0:  stloc      V_936
    IL_27c4:  stloc      V_937
    IL_27c8:  stloc      V_938
    IL_27cc:  stloc      V_939
    IL_27d0:  stloc      V_940
    IL_27d4:  stloc      V_941
    IL_27d8:  stloc      V_942
    IL_27dc:  stloc      V_943
    IL_27e0:  stloc      V_944
    IL_27e4:  stloc      V_945
    IL_27e8:  stloc      V_946
    IL_27ec:  stloc      V_947
    IL_27f0:  stloc      V_948
    IL_27f4:  stloc      V_949
    IL_27f8:  stloc      V_950
    IL_27fc:  stloc      V_951
    IL_2800:  stloc      V_952
    IL_2804:  stloc      V_953
    IL_2808:  stloc      V_954
    IL_280c:  stloc      V_955
    IL_2810:  stloc      V_956
    IL_2814:  stloc      V_957
    IL_2818:  stloc      V_958
    IL_281c:  stloc      V_959
    IL_2820:  stloc      V_960
    IL_2824:  stloc      V_961
    IL_2828:  stloc      V_962
    IL_282c:  stloc      V_963
    IL_2830:  stloc      V_964
    IL_2834:  stloc      V_965
    IL_2838:  stloc      V_966
    IL_283c:  stloc      V_967
    .try
    {
      IL_2840:  ldc.r8     0.0
      IL_2849:  stloc      V_969
      IL_284d:  ldc.i4.0
      IL_284e:  stloc      V_970
      IL_2852:  ldloc      V_912
      IL_2856:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_285b:  brfalse.s  IL_2885

      IL_285d:  ldloc      V_969
      IL_2861:  ldloc      V_910
      IL_2865:  ldloc      V_912
      IL_2869:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_286e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_2873:  add
      IL_2874:  stloc      V_969
      .line 2,2 : 5,55 
      IL_2878:  ldloc      V_970
      IL_287c:  ldc.i4.1
      IL_287d:  add
      IL_287e:  stloc      V_970
      .line 100001,100001 : 0,0 
      IL_2882:  nop
      IL_2883:  br.s       IL_2852

      IL_2885:  ldloc      V_970
      IL_2889:  brtrue.s   IL_288d

      IL_288b:  br.s       IL_288f

      IL_288d:  br.s       IL_28a7

      .line 100001,100001 : 0,0 
      IL_288f:  call       string [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/ErrorStrings::get_InputSequenceEmptyString()
      IL_2894:  ldstr      "source"
      IL_2899:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                   string)
      IL_289e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Raise<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.Exception)
      IL_28a3:  pop
      .line 100001,100001 : 0,0 
      IL_28a4:  nop
      IL_28a5:  br.s       IL_28a8

      .line 100001,100001 : 0,0 
      .line 100001,100001 : 0,0 
      IL_28a7:  nop
      IL_28a8:  ldloc      V_969
      IL_28ac:  stloc      V_971
      IL_28b0:  ldloc      V_970
      IL_28b4:  stloc      V_972
      IL_28b8:  ldloc      V_971
      IL_28bc:  ldloc      V_972
      IL_28c0:  conv.r8
      IL_28c1:  div
      IL_28c2:  stloc      V_968
      IL_28c6:  leave.s    IL_28ee

    }  // end .try
    finally
    {
      IL_28c8:  ldloc      V_912
      IL_28cc:  isinst     [mscorlib]System.IDisposable
      IL_28d1:  stloc      V_973
      IL_28d5:  ldloc      V_973
      IL_28d9:  brfalse.s  IL_28dd

      IL_28db:  br.s       IL_28df

      IL_28dd:  br.s       IL_28eb

      .line 100001,100001 : 0,0 
      IL_28df:  ldloc      V_973
      IL_28e3:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_28e8:  ldnull
      IL_28e9:  pop
      IL_28ea:  endfinally
      .line 100001,100001 : 0,0 
      IL_28eb:  ldnull
      IL_28ec:  pop
      IL_28ed:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_28ee:  ldloc      V_967
    IL_28f2:  ldloc      V_966
    IL_28f6:  ldloc      V_965
    IL_28fa:  ldloc      V_964
    IL_28fe:  ldloc      V_963
    IL_2902:  ldloc      V_962
    IL_2906:  ldloc      V_961
    IL_290a:  ldloc      V_960
    IL_290e:  ldloc      V_959
    IL_2912:  ldloc      V_958
    IL_2916:  ldloc      V_957
    IL_291a:  ldloc      V_956
    IL_291e:  ldloc      V_955
    IL_2922:  ldloc      V_954
    IL_2926:  ldloc      V_953
    IL_292a:  ldloc      V_952
    IL_292e:  ldloc      V_951
    IL_2932:  ldloc      V_950
    IL_2936:  ldloc      V_949
    IL_293a:  ldloc      V_948
    IL_293e:  ldloc      V_947
    IL_2942:  ldloc      V_946
    IL_2946:  ldloc      V_945
    IL_294a:  ldloc      V_944
    IL_294e:  ldloc      V_943
    IL_2952:  ldloc      V_942
    IL_2956:  ldloc      V_941
    IL_295a:  ldloc      V_940
    IL_295e:  ldloc      V_939
    IL_2962:  ldloc      V_938
    IL_2966:  ldloc      V_937
    IL_296a:  ldloc      V_936
    IL_296e:  ldloc      V_935
    IL_2972:  ldloc      V_934
    IL_2976:  ldloc      V_933
    IL_297a:  ldloc      V_932
    IL_297e:  ldloc      V_931
    IL_2982:  ldloc      V_930
    IL_2986:  ldloc      V_929
    IL_298a:  ldloc      V_928
    IL_298e:  ldloc      V_927
    IL_2992:  ldloc      V_926
    IL_2996:  ldloc      V_925
    IL_299a:  ldloc      V_924
    IL_299e:  ldloc      V_923
    IL_29a2:  ldloc      V_922
    IL_29a6:  ldloc      V_921
    IL_29aa:  ldloc      V_920
    IL_29ae:  ldloc      V_919
    IL_29b2:  ldloc      V_918
    IL_29b6:  ldloc      V_917
    IL_29ba:  ldloc      V_916
    IL_29be:  ldloc      V_915
    IL_29c2:  ldloc      V_914
    IL_29c6:  ldloc      V_913
    IL_29ca:  ldloc      V_968
    IL_29ce:  stloc      V_909
    IL_29d2:  ldloc      V_909
    IL_29d6:  stloc      V_974
    IL_29da:  ldloc      V_974
    IL_29de:  box        [mscorlib]System.Double
    IL_29e3:  ldc.i4.s   -3
    IL_29e5:  stloc      V_975
    IL_29e9:  ldloc      V_975
    IL_29ed:  stloc      V_976
    IL_29f1:  ldloc      V_976
    IL_29f5:  box        [mscorlib]System.Int32
    IL_29fa:  ldc.r8     3.
    IL_2a03:  ldc.r8     1.
    IL_2a0c:  add
    IL_2a0d:  stloc      V_977
    IL_2a11:  ldloc      V_977
    IL_2a15:  stloc      V_978
    IL_2a19:  ldloc      V_978
    IL_2a1d:  box        [mscorlib]System.Double
    IL_2a22:  ldc.r8     3.
    IL_2a2b:  ldc.r8     1.
    IL_2a34:  sub
    IL_2a35:  stloc      V_979
    IL_2a39:  ldloc      V_979
    IL_2a3d:  stloc      V_980
    IL_2a41:  ldloc      V_980
    IL_2a45:  box        [mscorlib]System.Double
    IL_2a4a:  ldc.r8     3.
    IL_2a53:  ldc.r8     1.
    IL_2a5c:  mul
    IL_2a5d:  stloc      V_981
    IL_2a61:  ldloc      V_981
    IL_2a65:  stloc      V_982
    IL_2a69:  ldloc      V_982
    IL_2a6d:  box        [mscorlib]System.Double
    IL_2a72:  ldc.r8     3.
    IL_2a7b:  stloc      V_983
    IL_2a7f:  ldloc      V_983
    IL_2a83:  stloc      V_984
    IL_2a87:  ldloc      V_984
    IL_2a8b:  box        [mscorlib]System.Double
    IL_2a90:  ldc.r8     3.
    IL_2a99:  neg
    IL_2a9a:  stloc      V_985
    IL_2a9e:  ldloc      V_985
    IL_2aa2:  stloc      V_986
    IL_2aa6:  ldloc      V_986
    IL_2aaa:  box        [mscorlib]System.Double
    IL_2aaf:  ldc.r4     3.
    IL_2ab4:  ldc.r4     1.
    IL_2ab9:  add
    IL_2aba:  stloc      V_987
    IL_2abe:  ldloc      V_987
    IL_2ac2:  stloc      V_988
    IL_2ac6:  ldloc      V_988
    IL_2aca:  box        [mscorlib]System.Single
    IL_2acf:  ldc.r4     3.
    IL_2ad4:  ldc.r4     1.
    IL_2ad9:  sub
    IL_2ada:  stloc      V_989
    IL_2ade:  ldloc      V_989
    IL_2ae2:  stloc      V_990
    IL_2ae6:  ldloc      V_990
    IL_2aea:  box        [mscorlib]System.Single
    IL_2aef:  ldc.r4     3.
    IL_2af4:  ldc.r4     1.
    IL_2af9:  mul
    IL_2afa:  stloc      V_991
    IL_2afe:  ldloc      V_991
    IL_2b02:  stloc      V_992
    IL_2b06:  ldloc      V_992
    IL_2b0a:  box        [mscorlib]System.Single
    IL_2b0f:  ldc.r4     3.
    IL_2b14:  stloc      V_993
    IL_2b18:  ldloc      V_993
    IL_2b1c:  stloc      V_994
    IL_2b20:  ldloc      V_994
    IL_2b24:  box        [mscorlib]System.Single
    IL_2b29:  ldc.r4     3.
    IL_2b2e:  neg
    IL_2b2f:  stloc      V_995
    IL_2b33:  ldloc      V_995
    IL_2b37:  stloc      V_996
    IL_2b3b:  ldloc      V_996
    IL_2b3f:  box        [mscorlib]System.Single
    IL_2b44:  ldc.i4.3
    IL_2b45:  ldc.i4.4
    IL_2b46:  add.ovf
    IL_2b47:  stloc      V_997
    IL_2b4b:  ldloc      V_997
    IL_2b4f:  stloc      V_998
    IL_2b53:  ldloc      V_998
    IL_2b57:  box        [mscorlib]System.Int32
    IL_2b5c:  ldc.i4.3
    IL_2b5d:  ldc.i4.4
    IL_2b5e:  sub.ovf
    IL_2b5f:  stloc      V_999
    IL_2b63:  ldloc      V_999
    IL_2b67:  stloc      V_1000
    IL_2b6b:  ldloc      V_1000
    IL_2b6f:  box        [mscorlib]System.Int32
    IL_2b74:  ldc.i4.3
    IL_2b75:  ldc.i4.4
    IL_2b76:  mul.ovf
    IL_2b77:  stloc      V_1001
    IL_2b7b:  ldloc      V_1001
    IL_2b7f:  stloc      V_1002
    IL_2b83:  ldloc      V_1002
    IL_2b87:  box        [mscorlib]System.Int32
    IL_2b8c:  ldc.i4.0
    IL_2b8d:  ldc.i4.3
    IL_2b8e:  sub.ovf
    IL_2b8f:  stloc      V_1003
    IL_2b93:  ldloc      V_1003
    IL_2b97:  stloc      V_1004
    IL_2b9b:  ldloc      V_1004
    IL_2b9f:  box        [mscorlib]System.Int32
    IL_2ba4:  ldc.i4.0
    IL_2ba5:  stloc      V_1005
    IL_2ba9:  ldloc      V_1005
    IL_2bad:  stloc      V_1006
    IL_2bb1:  ldloc      V_1006
    IL_2bb5:  box        [mscorlib]System.Boolean
    IL_2bba:  ldtoken    [mscorlib]System.Int32
    IL_2bbf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_2bc4:  stloc      V_1007
    IL_2bc8:  ldloc      V_1007
    IL_2bcc:  stloc      V_1008
    IL_2bd0:  ldloc      V_1008
    IL_2bd4:  box        [mscorlib]System.Type
    IL_2bd9:  sizeof     [mscorlib]System.Int32
    IL_2bdf:  stloc      V_1009
    IL_2be3:  ldloc      V_1009
    IL_2be7:  stloc      V_1010
    IL_2beb:  ldloc      V_1010
    IL_2bef:  box        [mscorlib]System.Int32
    IL_2bf4:  ldc.i4.0
    IL_2bf5:  stloc      V_1011
    IL_2bf9:  ldloc      V_1011
    IL_2bfd:  stloc      V_1012
    IL_2c01:  ldloc      V_1012
    IL_2c05:  box        [mscorlib]System.Int32
    IL_2c0a:  ldtoken    [mscorlib]System.Int32
    IL_2c0f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_2c14:  stloc      V_1014
    IL_2c18:  ldloc      V_1014
    IL_2c1c:  callvirt   instance bool [mscorlib]System.Type::get_IsGenericType()
    IL_2c21:  brfalse.s  IL_2c25

    IL_2c23:  br.s       IL_2c27

    IL_2c25:  br.s       IL_2c33

    .line 100001,100001 : 0,0 
    IL_2c27:  ldloc      V_1014
    IL_2c2b:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Type::GetGenericTypeDefinition()
    .line 100001,100001 : 0,0 
    IL_2c30:  nop
    IL_2c31:  br.s       IL_2c38

    .line 100001,100001 : 0,0 
    IL_2c33:  ldloc      V_1014
    .line 100001,100001 : 0,0 
    IL_2c37:  nop
    .line 100001,100001 : 0,0 
    IL_2c38:  stloc      V_1013
    IL_2c3c:  ldloc      V_1013
    IL_2c40:  stloc      V_1015
    IL_2c44:  ldloc      V_1015
    IL_2c48:  box        [mscorlib]System.Type
    IL_2c4d:  ldc.i4.3
    IL_2c4e:  stloc      V_1016
    IL_2c52:  ldloc      V_1016
    IL_2c56:  stloc      V_1017
    IL_2c5a:  ldloc      V_1017
    IL_2c5e:  box        [mscorlib]System.DayOfWeek
    IL_2c63:  ldc.i4.3
    IL_2c64:  ldc.i4.1
    IL_2c65:  ldc.i4.4
    IL_2c66:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_2c6b:  stloc      V_1018
    IL_2c6f:  ldloc      V_1018
    IL_2c73:  stloc      V_1019
    IL_2c77:  ldloc      V_1019
    IL_2c7b:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2c80:  ldc.i4.3
    IL_2c81:  stloc      V_1021
    IL_2c85:  ldc.i4.4
    IL_2c86:  stloc      V_1022
    IL_2c8a:  ldloc      V_1021
    IL_2c8e:  ldloc      V_1022
    IL_2c92:  newobj     instance void CallIntrinsics/'testIntrinsics@2-9'::.ctor(int32,
                                                                                  int32)
    IL_2c97:  stloc      V_1020
    IL_2c9b:  ldloc      V_1020
    IL_2c9f:  stloc      V_1023
    IL_2ca3:  ldloc      V_1023
    IL_2ca7:  box        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
    IL_2cac:  ldc.i4.2
    IL_2cad:  newarr     [mscorlib]System.Int32
    IL_2cb2:  dup
    IL_2cb3:  ldc.i4.0
    IL_2cb4:  ldc.i4.1
    IL_2cb5:  stelem     [mscorlib]System.Int32
    IL_2cba:  dup
    IL_2cbb:  ldc.i4.1
    IL_2cbc:  ldc.i4.2
    IL_2cbd:  stelem     [mscorlib]System.Int32
    IL_2cc2:  ldc.i4.0
    IL_2cc3:  ldelem     [mscorlib]System.Int32
    IL_2cc8:  stloc      V_1024
    IL_2ccc:  ldloc      V_1024
    IL_2cd0:  stloc      V_1025
    IL_2cd4:  ldloc      V_1025
    IL_2cd8:  box        [mscorlib]System.Int32
    IL_2cdd:  ldc.i4.1
    IL_2cde:  ldc.i4.2
    IL_2cdf:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_2ce4:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ce9:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2cee:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::get_Empty()
    IL_2cf3:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Cons(!0,
                                                                                                                                                                                                                                      class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2cf8:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
    IL_2cfd:  call       !!1[0...,0...] [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::CreateArray2D<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_2d02:  ldc.i4.0
    IL_2d03:  ldc.i4.0
    IL_2d04:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_2d09:  stloc      V_1026
    IL_2d0d:  ldloc      V_1026
    IL_2d11:  stloc      V_1027
    IL_2d15:  ldloc      V_1027
    IL_2d19:  box        [mscorlib]System.Int32
    IL_2d1e:  ldc.i4.1
    IL_2d1f:  ldc.i4.1
    IL_2d20:  ldc.i4.1
    IL_2d21:  newobj     instance void CallIntrinsics/'testIntrinsics@2-10'::.ctor()
    IL_2d26:  call       !!0[0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array3DModule::Initialize<int32>(int32,
                                                                                                                        int32,
                                                                                                                        int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>>>)
    IL_2d2b:  ldc.i4.0
    IL_2d2c:  ldc.i4.0
    IL_2d2d:  ldc.i4.0
    IL_2d2e:  call       instance int32 int32[0...,0...,0...]::Get(int32,
                                                                   int32,
                                                                   int32)
    IL_2d33:  stloc      V_1028
    IL_2d37:  ldloc      V_1028
    IL_2d3b:  stloc      V_1029
    IL_2d3f:  ldloc      V_1029
    IL_2d43:  box        [mscorlib]System.Int32
    IL_2d48:  ldc.i4.1
    IL_2d49:  ldc.i4.1
    IL_2d4a:  ldc.i4.1
    IL_2d4b:  ldc.i4.1
    IL_2d4c:  newobj     instance void CallIntrinsics/'testIntrinsics@2-11'::.ctor()
    IL_2d51:  call       !!0[0...,0...,0...,0...] [FSharp.Core]Microsoft.FSharp.Collections.Array4DModule::Initialize<int32>(int32,
                                                                                                                             int32,
                                                                                                                             int32,
                                                                                                                             int32,
                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>>>>)
    IL_2d56:  ldc.i4.0
    IL_2d57:  ldc.i4.0
    IL_2d58:  ldc.i4.0
    IL_2d59:  ldc.i4.0
    IL_2d5a:  call       instance int32 int32[0...,0...,0...,0...]::Get(int32,
                                                                        int32,
                                                                        int32,
                                                                        int32)
    IL_2d5f:  stloc      V_1030
    IL_2d63:  ldloc      V_1030
    IL_2d67:  stloc      V_1031
    IL_2d6b:  ldloc      V_1031
    IL_2d6f:  box        [mscorlib]System.Int32
    IL_2d74:  newobj     instance void CallIntrinsics/'testIntrinsics@2-12'::.ctor()
    IL_2d79:  ldc.i4.1
    IL_2d7a:  ldc.i4.2
    IL_2d7b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_2d80:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2d85:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2d8a:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2d8f:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                     class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_2d94:  stloc      V_1032
    IL_2d98:  ldloc      V_1032
    IL_2d9c:  stloc      V_1033
    IL_2da0:  ldloc      V_1033
    IL_2da4:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2da9:  newobj     instance void CallIntrinsics/'testIntrinsics@2-13'::.ctor()
    IL_2dae:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Delay<object>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
    IL_2db3:  stloc      V_1034
    IL_2db7:  ldloc      V_1034
    IL_2dbb:  stloc      V_1035
    IL_2dbf:  ldloc      V_1035
    IL_2dc3:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<object>
    IL_2dc8:  ldc.i4.1
    IL_2dc9:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_2dce:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2dd3:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2dd8:  ldc.i4.2
    IL_2dd9:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_2dde:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                        class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2de3:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2de8:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Append<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_2ded:  stloc      V_1036
    IL_2df1:  ldloc      V_1036
    IL_2df5:  stloc      V_1037
    IL_2df9:  ldloc      V_1037
    IL_2dfd:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_2e02:  ldc.i4.0
    IL_2e03:  ldnull
    IL_2e04:  ldc.i4.0
    IL_2e05:  ldnull
    IL_2e06:  newobj     instance void CallIntrinsics/'testIntrinsics@2-14'::.ctor(int32,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                   int32,
                                                                                   class [mscorlib]System.Tuple`2<int32,int32>)
    IL_2e0b:  stloc      V_1038
    IL_2e0f:  ldloc      V_1038
    IL_2e13:  stloc      V_1039
    IL_2e17:  ldloc      V_1039
    IL_2e1b:  box        class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32>>
    IL_2e20:  ldc.i4.0
    IL_2e21:  ldnull
    IL_2e22:  ldc.i4.0
    IL_2e23:  ldnull
    IL_2e24:  newobj     instance void CallIntrinsics/'testIntrinsics@2-15'::.ctor(int32,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                   int32,
                                                                                   class [mscorlib]System.Tuple`2<int32,int32>)
    IL_2e29:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<int32,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_2e2e:  stloc      V_1040
    IL_2e32:  ldloc      V_1040
    IL_2e36:  stloc      V_1041
    IL_2e3a:  ldloc      V_1041
    IL_2e3e:  box        class [mscorlib]System.Tuple`2<int32,int32>[]
    IL_2e43:  ldc.i4.0
    IL_2e44:  ldnull
    IL_2e45:  ldc.i4.0
    IL_2e46:  ldnull
    IL_2e47:  newobj     instance void CallIntrinsics/'testIntrinsics@2-16'::.ctor(int32,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                   int32,
                                                                                   class [mscorlib]System.Tuple`2<int32,int32>)
    IL_2e4c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [mscorlib]System.Tuple`2<int32,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_2e51:  stloc      V_1042
    IL_2e55:  ldloc      V_1042
    IL_2e59:  stloc      V_1043
    IL_2e5d:  ldloc      V_1043
    IL_2e61:  box        class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [mscorlib]System.Tuple`2<int32,int32>>
    IL_2e66:  ldstr      "%d"
    IL_2e6b:  newobj     instance void class [FSharp.Core_3]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,int32>::.ctor(string)
    IL_2e70:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_2e75:  stloc      V_1045
    IL_2e79:  ldloc      V_1045
    IL_2e7d:  newobj     instance void CallIntrinsics/'testIntrinsics@2-17'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>)
    IL_2e82:  ldc.i4.1
    IL_2e83:  callvirt   instance !1 class [FSharp.Core_3]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::Invoke(!0)
    IL_2e88:  stloc      V_1044
    IL_2e8c:  ldloc      V_1044
    IL_2e90:  stloc      V_1046
    IL_2e94:  ldloc      V_1046
    IL_2e98:  box        [mscorlib]System.String
    IL_2e9d:  newobj     instance void CallIntrinsics/'testIntrinsics@2-18'::.ctor()
    IL_2ea2:  call       class [mscorlib]System.Lazy`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.LazyExtensions::Create<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_2ea7:  stloc      V_1047
    IL_2eab:  ldloc      V_1047
    IL_2eaf:  stloc      V_1048
    IL_2eb3:  ldloc      V_1048
    IL_2eb7:  box        class [mscorlib]System.Lazy`1<int32>
    IL_2ebc:  ldc.i4.s   10
    IL_2ebe:  ldc.i4.0
    IL_2ebf:  ldc.i4.0
    IL_2ec0:  ldc.i4.0
    IL_2ec1:  ldc.i4.1
    IL_2ec2:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_2ec7:  stloc      V_1049
    IL_2ecb:  ldloc      V_1049
    IL_2ecf:  stloc      V_1050
    IL_2ed3:  ldloc      V_1050
    IL_2ed7:  box        [mscorlib]System.Decimal
    IL_2edc:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::get_Empty()
    IL_2ee1:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ee6:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2eeb:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ef0:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ef5:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2efa:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2eff:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f04:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f09:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f0e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f13:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f18:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f1d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f22:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f27:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f2c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f31:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f36:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f3b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f40:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f45:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f4a:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f4f:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f54:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f59:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f5e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f63:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f68:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f6d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f72:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f77:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f7c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f81:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f86:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f8b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f90:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f95:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f9a:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2f9f:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fa4:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fa9:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fae:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fb3:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fb8:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fbd:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fc2:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fc7:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fcc:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fd1:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fd6:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fdb:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fe0:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fe5:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fea:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2fef:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ff4:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ff9:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_2ffe:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3003:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3008:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_300d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3012:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3017:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_301c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3021:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3026:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_302b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3030:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3035:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_303a:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_303f:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3044:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3049:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_304e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3053:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3058:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_305d:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3062:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3067:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_306c:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3071:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3076:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_307b:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3080:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3085:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_308a:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_308f:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3094:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_3099:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_309e:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_30a3:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_30a8:  call       class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<object>::Cons(!0,
                                                                                                                                                                         class [FSharp.Core_3]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_30ad:  ret
  } // end of method CallIntrinsics::testIntrinsics

} // end of class CallIntrinsics

.class private abstract auto ansi sealed '<StartupCode$CallIntrinsics>'.$CallIntrinsics$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$CallIntrinsics>'.$CallIntrinsics$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
