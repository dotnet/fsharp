
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
  .ver 5:0:0:0
}
.assembly OptionalArg01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.OptionalArg01
{
  // Offset: 0x00000000 Length: 0x0000045A
}
.mresource public FSharpOptimizationData.OptionalArg01
{
  // Offset: 0x00000460 Length: 0x00000445
}
.module OptionalArg01.exe
// MVID: {6124063B-4F48-B5AF-A745-03833B062461}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04EE0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed OptionalArg01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public A
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 16707566,16707566 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Tuples\\OptionalArg01.fs'
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      .line 2,2 : 6,7 ''
      IL_0008:  ret
    } // end of method A::.ctor

  } // end of class A

  .class auto ansi serializable nested public C
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 16707566,16707566 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      .line 5,5 : 6,7 ''
      IL_0008:  ret
    } // end of method C::.ctor

    .method public static class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> 
            F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class OptionalArg01/A> x1,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class OptionalArg01/A> x2) cil managed
    {
      .param [1]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [2]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       93 (0x5d)
      .maxstack  4
      .locals init ([0] int32 'count (shadowed)',
               [1] int32 count,
               [2] class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> attribs,
               [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class OptionalArg01/A> V_3,
               [4] class OptionalArg01/A v2)
      .line 10,10 : 9,44 ''
      IL_0000:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_0001:  ldarg.0
      IL_0002:  brfalse.s  IL_0006

      IL_0004:  br.s       IL_000a

      .line 8,8 : 43,48 ''
      IL_0006:  ldc.i4.0
      .line 16707566,16707566 : 0,0 ''
      IL_0007:  nop
      IL_0008:  br.s       IL_000c

      .line 8,8 : 61,70 ''
      IL_000a:  ldc.i4.1
      .line 16707566,16707566 : 0,0 ''
      IL_000b:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_000c:  stloc.0
      .line 10,10 : 9,44 ''
      IL_000d:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_000e:  ldarg.1
      IL_000f:  brfalse.s  IL_0013

      IL_0011:  br.s       IL_0017

      .line 9,9 : 43,48 ''
      IL_0013:  ldloc.0
      .line 16707566,16707566 : 0,0 ''
      IL_0014:  nop
      IL_0015:  br.s       IL_001b

      .line 9,9 : 61,70 ''
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.1
      IL_0019:  add
      .line 16707566,16707566 : 0,0 ''
      IL_001a:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_001b:  stloc.1
      .line 10,10 : 9,44 ''
      IL_001c:  ldloc.1
      IL_001d:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::.ctor(int32)
      IL_0022:  stloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_0023:  ldarg.0
      IL_0024:  brfalse.s  IL_0028

      IL_0026:  br.s       IL_002c

      .line 11,11 : 31,33 ''
      IL_0028:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_0029:  nop
      IL_002a:  br.s       IL_003f

      .line 16707566,16707566 : 0,0 ''
      IL_002c:  ldarg.0
      IL_002d:  stloc.3
      IL_002e:  ldloc.3
      IL_002f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class OptionalArg01/A>::get_Value()
      IL_0034:  stloc.s    v2
      .line 11,11 : 47,62 ''
      IL_0036:  ldloc.2
      IL_0037:  ldloc.s    v2
      IL_0039:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
      .line 16707566,16707566 : 0,0 ''
      IL_003e:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_003f:  ldarg.1
      IL_0040:  brfalse.s  IL_0044

      IL_0042:  br.s       IL_0048

      .line 12,12 : 31,33 ''
      IL_0044:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_0045:  nop
      IL_0046:  br.s       IL_005b

      .line 16707566,16707566 : 0,0 ''
      IL_0048:  ldarg.1
      IL_0049:  stloc.3
      IL_004a:  ldloc.3
      IL_004b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class OptionalArg01/A>::get_Value()
      IL_0050:  stloc.s    v2
      .line 12,12 : 47,62 ''
      IL_0052:  ldloc.2
      IL_0053:  ldloc.s    v2
      IL_0055:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
      .line 16707566,16707566 : 0,0 ''
      IL_005a:  nop
      .line 13,13 : 9,16 ''
      IL_005b:  ldloc.2
      IL_005c:  ret
    } // end of method C::F

  } // end of class C

  .method public static class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> 
          test() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    .line 19,19 : 5,11 ''
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::.ctor(int32)
    IL_0006:  ret
  } // end of method OptionalArg01::test

  .method public static class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> 
          test2() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  4
    .locals init ([0] class OptionalArg01/A V_0,
             [1] class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> V_1)
    .line 27,27 : 5,17 ''
    IL_0000:  newobj     instance void OptionalArg01/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::.ctor(int32)
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.0
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
    IL_0014:  ldloc.1
    IL_0015:  ret
  } // end of method OptionalArg01::test2

  .method public static class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> 
          test3() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  4
    .locals init ([0] class OptionalArg01/A V_0,
             [1] class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> V_1)
    .line 35,35 : 5,17 ''
    IL_0000:  newobj     instance void OptionalArg01/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::.ctor(int32)
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.0
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
    IL_0014:  ldloc.1
    IL_0015:  ret
  } // end of method OptionalArg01::test3

  .method public static class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> 
          test4() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  4
    .locals init ([0] class OptionalArg01/A V_0,
             [1] class OptionalArg01/A V_1,
             [2] class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A> V_2)
    .line 45,45 : 5,25 ''
    IL_0000:  newobj     instance void OptionalArg01/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void OptionalArg01/A::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.2
    IL_000d:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::.ctor(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
    IL_001a:  ldloc.2
    IL_001b:  ldloc.1
    IL_001c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class OptionalArg01/A>::Add(!0)
    IL_0021:  ldloc.2
    IL_0022:  ret
  } // end of method OptionalArg01::test4

} // end of class OptionalArg01

.class private abstract auto ansi sealed '<StartupCode$OptionalArg01>'.$OptionalArg01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $OptionalArg01::main@

} // end of class '<StartupCode$OptionalArg01>'.$OptionalArg01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
