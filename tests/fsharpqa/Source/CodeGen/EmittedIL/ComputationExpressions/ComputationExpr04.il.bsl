
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
.assembly ComputationExpr04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr04
{
  // Offset: 0x00000000 Length: 0x00000237
}
.mresource public FSharpOptimizationData.ComputationExpr04
{
  // Offset: 0x00000240 Length: 0x0000007D
}
.module ComputationExpr04.exe
// MVID: {4F31D663-366A-E566-A745-038363D6314F}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000580000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ComputationExpr04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit 'res4@7-1'
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
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/'res4@7-1'::builder@
      IL_000d:  ret
    } // end of method 'res4@7-1'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       55 (0x37)
      .maxstack  6
      .locals init ([0] int32 x)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 7,7 : 13,54 
      IL_0000:  nop
      IL_0001:  nop
      .line 7,7 : 22,37 
      IL_0002:  ldstr      "hello"
      IL_0007:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0011:  pop
      .line 7,7 : 39,53 
      IL_0012:  ldstr      "hello"
      IL_0017:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001c:  stloc.0
      .line 8,8 : 13,28 
      IL_001d:  ldstr      "fail"
      IL_0022:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::FailWith<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(string)
      IL_0027:  pop
      .line 9,9 : 13,21 
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/'res4@7-1'::builder@
      IL_002e:  ldloc.0
      IL_002f:  tail.
      IL_0031:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_0036:  ret
    } // end of method 'res4@7-1'::Invoke

  } // end of class 'res4@7-1'

  .class auto ansi serializable nested assembly beforefieldinit 'res4@6-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Exception,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Exception,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/'res4@6-2'::builder@
      IL_000d:  ret
    } // end of method 'res4@6-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [mscorlib]System.Exception _arg1) cil managed
    {
      // Code size       46 (0x2e)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] int32 x)
      .line 6,12 : 9,21 
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 11,11 : 13,54 
      IL_0003:  nop
      .line 11,11 : 22,37 
      IL_0004:  ldstr      "hello"
      IL_0009:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  pop
      .line 11,11 : 39,53 
      IL_0014:  ldstr      "hello"
      IL_0019:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001e:  stloc.1
      .line 12,12 : 13,21 
      IL_001f:  ldarg.0
      IL_0020:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/'res4@6-2'::builder@
      IL_0025:  ldloc.1
      IL_0026:  tail.
      IL_0028:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002d:  ret
    } // end of method 'res4@6-2'::Invoke

  } // end of class 'res4@6-2'

  .class auto ansi serializable nested assembly beforefieldinit res4@6
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
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/res4@6::builder@
      IL_000d:  ret
    } // end of method res4@6::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 6,6 : 9,12 
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/res4@6::builder@
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/res4@6::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/res4@6::builder@
      IL_0013:  newobj     instance void ComputationExpr04/'res4@7-1'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0018:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr04/res4@6::builder@
      IL_0023:  newobj     instance void ComputationExpr04/'res4@6-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0028:  tail.
      IL_002a:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::TryWith<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Exception,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_002f:  ret
    } // end of method res4@6::Invoke

  } // end of class res4@6

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr04>'.$ComputationExpr04::res4@4
    IL_0005:  ret
  } // end of method ComputationExpr04::get_res4

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr04::get_res4()
  } // end of property ComputationExpr04::res4
} // end of class ComputationExpr04

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr04>'.$ComputationExpr04
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res4@4
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
    .locals init ([0] class [ComputationExprLibrary]Library.Eventually`1<int32> res4,
             [1] class [ComputationExprLibrary]Library.EventuallyBuilder builder@)
    .line 14,14 : 1,25 
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void ComputationExpr04/res4@6::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr04>'.$ComputationExpr04::res4@4
    IL_0018:  stloc.0
    IL_0019:  nop
    IL_001a:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr04::get_res4()
    IL_001f:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0024:  pop
    IL_0025:  ret
  } // end of method $ComputationExpr04::main@

} // end of class '<StartupCode$ComputationExpr04>'.$ComputationExpr04


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
