
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17376
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
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr03
{
  // Offset: 0x00000000 Length: 0x00000261
}
.mresource public FSharpOptimizationData.ComputationExpr03
{
  // Offset: 0x00000268 Length: 0x0000008C
}
.module ComputationExpr03.exe
// MVID: {4F31D660-3649-E566-A745-038360D6314F}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000390000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ComputationExpr03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit res2@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/res2@8::builder@
      IL_000d:  ret
    } // end of method res2@8::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       46 (0x2e)
      .maxstack  7
      .locals init ([0] int32 x)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 8,8 : 9,50 
      IL_0000:  nop
      IL_0001:  nop
      .line 8,8 : 18,33 
      IL_0002:  ldstr      "hello"
      IL_0007:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0011:  pop
      .line 8,8 : 35,49 
      IL_0012:  ldstr      "hello"
      IL_0017:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001c:  stloc.0
      .line 9,9 : 9,21 
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/res2@8::builder@
      IL_0023:  ldloc.0
      IL_0024:  ldloc.0
      IL_0025:  add
      IL_0026:  tail.
      IL_0028:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002d:  ret
    } // end of method res2@8::Invoke

  } // end of class res2@8

  .class auto ansi serializable nested assembly beforefieldinit 'res3@17-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@17-2'::builder@
      IL_000d:  ret
    } // end of method 'res3@17-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  6
      .locals init ([0] int32 x)
      .line 17,17 : 17,58 
      IL_0000:  nop
      IL_0001:  nop
      .line 17,17 : 26,41 
      IL_0002:  ldstr      "hello"
      IL_0007:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0011:  pop
      .line 17,17 : 43,57 
      IL_0012:  ldstr      "hello"
      IL_0017:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001c:  stloc.0
      .line 18,18 : 17,25 
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@17-2'::builder@
      IL_0023:  ldc.i4.1
      IL_0024:  tail.
      IL_0026:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002b:  ret
    } // end of method 'res3@17-2'::Invoke

  } // end of class 'res3@17-2'

  .class auto ansi serializable nested assembly beforefieldinit 'res3@20-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@20-3'::builder@
      IL_000d:  ret
    } // end of method 'res3@20-3'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(int32 _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] int32 x)
      .line 15,19 : 9,14 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 20,20 : 9,17 
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@20-3'::builder@
      IL_0009:  ldc.i4.1
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_0011:  ret
    } // end of method 'res3@20-3'::Invoke

  } // end of class 'res3@20-3'

  .class auto ansi serializable nested assembly beforefieldinit 'res3@15-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@15-1'::builder@
      IL_000d:  ret
    } // end of method 'res3@15-1'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       46 (0x2e)
      .maxstack  7
      .locals init ([0] int32 x,
               [1] class [ComputationExprLibrary]Library.EventuallyBuilder builder@)
      .line 14,14 : 9,23 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 15,19 : 9,14 
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@15-1'::builder@
      IL_0009:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
      IL_000e:  stloc.1
      IL_000f:  ldloc.1
      IL_0010:  ldloc.1
      IL_0011:  newobj     instance void ComputationExpr03/'res3@17-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0016:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/'res3@15-1'::builder@
      IL_0021:  newobj     instance void ComputationExpr03/'res3@20-3'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0026:  tail.
      IL_0028:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!1> [ComputationExprLibrary]Library.EventuallyBuilder::Bind<int32,int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>,
                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<!!1>>)
      IL_002d:  ret
    } // end of method 'res3@15-1'::Invoke

  } // end of class 'res3@15-1'

  .class auto ansi serializable nested assembly beforefieldinit res3@14
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/res3@14::builder@
      IL_000d:  ret
    } // end of method res3@14::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      .line 14,14 : 9,23 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/res3@14::builder@
      IL_0007:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr03::get_res2()
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr03/res3@14::builder@
      IL_0012:  newobj     instance void ComputationExpr03/'res3@15-1'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0017:  tail.
      IL_0019:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!1> [ComputationExprLibrary]Library.EventuallyBuilder::Bind<int32,int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>,
                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<!!1>>)
      IL_001e:  ret
    } // end of method res3@14::Invoke

  } // end of class res3@14

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$ComputationExpr03::res2@6
    IL_0005:  ret
  } // end of method ComputationExpr03::get_res2

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$ComputationExpr03::res3@12
    IL_0005:  ret
  } // end of method ComputationExpr03::get_res3

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr03::get_res2()
  } // end of property ComputationExpr03::res2
  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr03::get_res3()
  } // end of property ComputationExpr03::res3
} // end of class ComputationExpr03

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr03>'.$ComputationExpr03
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res2@6
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res3@12
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       74 (0x4a)
    .maxstack  4
    .locals init ([0] class [ComputationExprLibrary]Library.Eventually`1<int32> res2,
             [1] class [ComputationExprLibrary]Library.Eventually`1<int32> res3,
             [2] class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
             [3] class [ComputationExprLibrary]Library.EventuallyBuilder V_3)
    .line 10,10 : 1,25 
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.2
    IL_0006:  ldloc.2
    IL_0007:  ldloc.2
    IL_0008:  newobj     instance void ComputationExpr03/res2@8::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$ComputationExpr03::res2@6
    IL_0018:  stloc.0
    IL_0019:  nop
    IL_001a:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr03::get_res2()
    IL_001f:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0024:  pop
    IL_0025:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_002a:  stloc.3
    IL_002b:  ldloc.3
    IL_002c:  ldloc.3
    IL_002d:  newobj     instance void ComputationExpr03/res3@14::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_0032:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0037:  dup
    IL_0038:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$ComputationExpr03::res3@12
    IL_003d:  stloc.1
    .line 22,22 : 1,26 
    IL_003e:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr03::get_res3()
    IL_0043:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0048:  pop
    IL_0049:  ret
  } // end of method $ComputationExpr03::main@

} // end of class '<StartupCode$ComputationExpr03>'.$ComputationExpr03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
