
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr07
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr07
{
  // Offset: 0x00000000 Length: 0x00000261
  // WARNING: managed resource file FSharpSignatureData.ComputationExpr07 created
}
.mresource public FSharpOptimizationData.ComputationExpr07
{
  // Offset: 0x00000268 Length: 0x00000073
  // WARNING: managed resource file FSharpOptimizationData.ComputationExpr07 created
}
.module ComputationExpr07.exe
// MVID: {6229B0E2-35BD-E566-A745-0383E2B02962}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00ED0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Program
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'res7@10-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res7@10-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Program/'res7@10-1'::x
      IL_0014:  ret
    } // end of method 'res7@10-1'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Program/'res7@10-1'::x
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Program/'res7@10-1'::x
      IL_000e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0013:  ldloc.0
      IL_0014:  sub
      IL_0015:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res7@10-1'::builder@
      IL_0020:  tail.
      IL_0022:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::Zero()
      IL_0027:  ret
    } // end of method 'res7@10-1'::Invoke

  } // end of class 'res7@10-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'res7@12-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res7@12-2'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Program/'res7@12-2'::x
      IL_0014:  ret
    } // end of method 'res7@12-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res7@12-2'::builder@
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Program/'res7@12-2'::x
      IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0011:  tail.
      IL_0013:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_0018:  ret
    } // end of method 'res7@12-2'::Invoke

  } // end of class 'res7@12-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit res7@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_000d:  ret
    } // end of method res7@9::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       85 (0x55)
      .maxstack  9
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0)
      IL_0000:  ldc.i4.1
      IL_0001:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_0013:  ldc.i4.0
      IL_0014:  ldc.i4.1
      IL_0015:  ldc.i4.3
      IL_0016:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_001b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0020:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0025:  ldarg.0
      IL_0026:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_002b:  ldloc.0
      IL_002c:  newobj     instance void Program/'res7@10-1'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder,
                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0031:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::For<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
      IL_0036:  ldarg.0
      IL_0037:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_003c:  ldarg.0
      IL_003d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res7@9::builder@
      IL_0042:  ldloc.0
      IL_0043:  newobj     instance void Program/'res7@12-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder,
                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0048:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_004d:  tail.
      IL_004f:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Combine<int32>(class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                                                                                                              class [ComputationExprLibrary]Library.Eventually`1<!!0>)
      IL_0054:  ret
    } // end of method res7@9::Invoke

  } // end of class res7@9

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res7() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr07>'.$Program::res7@7
    IL_0005:  ret
  } // end of method Program::get_res7

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res7()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res7()
  } // end of property Program::res7
} // end of class Program

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr07>'.$Program
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res7@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init (class [ComputationExprLibrary]Library.Eventually`1<int32> V_0,
             class [ComputationExprLibrary]Library.EventuallyBuilder V_1,
             class [ComputationExprLibrary]Library.Eventually`1<int32> V_2,
             int32 V_3,
             int32 V_4)
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void Program/res7@9::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr07>'.$Program::res7@7
    IL_0018:  stloc.0
    IL_0019:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res7()
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0025:  stloc.3
    IL_0026:  ldloc.3
    IL_0027:  stloc.s    V_4
    IL_0029:  ret
  } // end of method $Program::main@

} // end of class '<StartupCode$ComputationExpr07>'.$Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\ComputationExpressions\ComputationExpr07_fs\ComputationExpr07.res
