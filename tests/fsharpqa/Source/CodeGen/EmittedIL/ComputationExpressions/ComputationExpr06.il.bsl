
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
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr06
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr06
{
  // Offset: 0x00000000 Length: 0x00000237
}
.mresource public FSharpOptimizationData.ComputationExpr06
{
  // Offset: 0x00000240 Length: 0x0000007D
}
.module ComputationExpr06.exe
// MVID: {4DAC0626-35A8-E566-A745-03832606AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000260000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ComputationExpr06
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit 'res6@9-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr06/'res6@9-1'::x
      IL_000d:  ret
    } // end of method 'res6@9-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 9,9 : 15,21 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr06/'res6@9-1'::x
      IL_0007:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
      IL_000c:  ldc.i4.0
      IL_000d:  cgt
      IL_000f:  ret
    } // end of method 'res6@9-1'::Invoke

  } // end of class 'res6@9-1'

  .class auto ansi serializable nested assembly beforefieldinit 'res6@10-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/'res6@10-2'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr06/'res6@10-2'::x
      IL_0014:  ret
    } // end of method 'res6@10-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       107 (0x6b)
      .maxstack  5
      .line 10,10 : 13,28 
      IL_0000:  nop
      IL_0001:  ldstr      "hello"
      IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0010:  pop
      .line 11,11 : 13,28 
      IL_0011:  ldstr      "hello"
      IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_001b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0020:  pop
      .line 12,12 : 13,28 
      IL_0021:  ldstr      "hello"
      IL_0026:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_002b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0030:  pop
      .line 13,13 : 13,28 
      IL_0031:  ldstr      "hello"
      IL_0036:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_003b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0040:  pop
      .line 14,14 : 13,28 
      IL_0041:  ldstr      "hello"
      IL_0046:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_004b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0050:  pop
      .line 15,15 : 13,19 
      IL_0051:  ldarg.0
      IL_0052:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr06/'res6@10-2'::x
      IL_0057:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Decrement(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_005c:  nop
      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/'res6@10-2'::builder@
      IL_0063:  tail.
      IL_0065:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::Zero()
      IL_006a:  ret
    } // end of method 'res6@10-2'::Invoke

  } // end of class 'res6@10-2'

  .class auto ansi serializable nested assembly beforefieldinit 'res6@16-3'
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
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/'res6@16-3'::builder@
      IL_000d:  ret
    } // end of method 'res6@16-3'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      .line 16,16 : 9,17 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/'res6@16-3'::builder@
      IL_0007:  ldc.i4.1
      IL_0008:  tail.
      IL_000a:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_000f:  ret
    } // end of method 'res6@16-3'::Invoke

  } // end of class 'res6@16-3'

  .class auto ansi serializable nested assembly beforefieldinit res6@8
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
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_000d:  ret
    } // end of method res6@8::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       84 (0x54)
      .maxstack  10
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x)
      .line 8,8 : 9,22 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
      IL_0007:  stloc.0
      .line 9,9 : 9,21 
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_000e:  ldarg.0
      IL_000f:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_0014:  ldloc.0
      IL_0015:  newobj     instance void ComputationExpr06/'res6@9-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_0026:  ldloc.0
      IL_0027:  newobj     instance void ComputationExpr06/'res6@10-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder,
                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_002c:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_0031:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::While(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>,
                                                                                                                                                                                               class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0036:  ldarg.0
      IL_0037:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_003c:  ldarg.0
      IL_003d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr06/res6@8::builder@
      IL_0042:  newobj     instance void ComputationExpr06/'res6@16-3'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0047:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_004c:  tail.
      IL_004e:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Combine<int32>(class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                                                                                                              class [ComputationExprLibrary]Library.Eventually`1<!!0>)
      IL_0053:  ret
    } // end of method res6@8::Invoke

  } // end of class res6@8

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res6() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr06>'.$ComputationExpr06::res6@6
    IL_0005:  ret
  } // end of method ComputationExpr06::get_res6

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr06::get_res6()
  } // end of property ComputationExpr06::res6
} // end of class ComputationExpr06

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr06>'.$ComputationExpr06
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res6@6
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       38 (0x26)
    .maxstack  4
    .locals init ([0] class [ComputationExprLibrary]Library.Eventually`1<int32> res6,
             [1] class [ComputationExprLibrary]Library.EventuallyBuilder builder@)
    .line 18,18 : 1,25 
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void ComputationExpr06/res6@8::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr06>'.$ComputationExpr06::res6@6
    IL_0018:  stloc.0
    IL_0019:  nop
    IL_001a:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr06::get_res6()
    IL_001f:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0024:  pop
    IL_0025:  ret
  } // end of method $ComputationExpr06::main@

} // end of class '<StartupCode$ComputationExpr06>'.$ComputationExpr06


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
