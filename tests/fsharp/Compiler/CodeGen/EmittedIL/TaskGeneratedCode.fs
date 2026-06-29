// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.Threading.Tasks
open FSharp.Test
open FSharp.Test.Compiler
open Xunit

// These tests are sensitive to the fact that FSharp.Core is compiled release, not debug
// The code generated changes slightly in debug mode
#if !DEBUG 
// Check the exact code produced for tasks

module TaskGeneratedCode =

    // This tests the exact optimized code generated for the MoveNext for a trivial task - we expect 'MoveNext' to be there
    // because state machine compilation succeeds
    //
    // The code is not perfect - because the MoveNext is generated late - but the JIT does a good job on it.
    //
    // The try/catch for the task still exists even though there is no chance of an exception
    //
    // The crucial code for "return 1" is really just
    //   IL_000e:  ldc.i4.1
    //   IL_000f:  stfld      int32 Test/testTask@4::Result

    [<Fact>]
    let ``check MoveNext of simple task debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

let testTask() = task { return 1 }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit testTask@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0006:  ldc.i4.1
      IL_0007:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Return<int32>(!!0)
      IL_000c:  ret
    } 

  } 

  .method public static class [runtime]System.Threading.Tasks.Task`1<int32> testTask() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void Test/testTask@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
    IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
    IL_0013:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<int32>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
    IL_0018:  ret
  } 

} 
                """
            ]))

    // This tests the exact optimized code generated for the MoveNext for a trivial task - we expect 'MoveNext' to be there
    // because state machine compilation succeeds
    //
    // The code is not perfect - because the MoveNext is generated late - but the JIT does a good job on it.
    //
    // The try/catch for the task still exists even though there is no chance of an exception
    //
    // The crucial code for "return 1" is really just
    //   IL_000e:  ldc.i4.1
    //   IL_000f:  stfld      int32 Test/testTask@4::Result

    [<Fact>]
    let ``check MoveNext of simple task optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize+"; "/debug:portable";"--realsig+"; "/tailcalls+" |],
            """
module Test

let testTask() = task { return 1 }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.method public strict virtual instance void 
        MoveNext() cil managed
{
  .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
  
  .maxstack  4
  .locals init (int32 V_0,
           class [runtime]System.Exception V_1,
           bool V_2,
           class [runtime]System.Exception V_3)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
  IL_0006:  stloc.0
  .try
  {
    IL_0007:  ldarg.0
    IL_0008:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_000d:  ldc.i4.1
    IL_000e:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0013:  ldc.i4.1
    IL_0014:  stloc.2
    IL_0015:  ldloc.2
    IL_0016:  brfalse.s  IL_0035
    
    IL_0018:  ldarg.0
    IL_0019:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_001e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0023:  ldarg.0
    IL_0024:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0029:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_002e:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_0033:  leave.s    IL_0041
    
    IL_0035:  leave.s    IL_0041
    
  }  
  catch [runtime]System.Object 
  {
    IL_0037:  castclass  [runtime]System.Exception
    IL_003c:  stloc.3
    IL_003d:  ldloc.3
    IL_003e:  stloc.1
    IL_003f:  leave.s    IL_0041
    
  }  
  IL_0041:  ldloc.1
  IL_0042:  stloc.3
  IL_0043:  ldloc.3
  IL_0044:  brtrue.s   IL_0047
    
  IL_0046:  ret
    
  IL_0047:  ldarg.0
  IL_0048:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_004d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0052:  ldloc.3
  IL_0053:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_0058:  ret
} 
                """
            ]))


    [<Fact>]
    let ``check MoveNext of simple binding task debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/debug:portable";"--realsig+"; "/optimize-"; "/tailcalls-" |],
            """
module Test
open System.Threading.Tasks
let testTask(t: Task<int>) = task { let! res = t in return res+1 }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@4-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_0008:  ldloc.0
      IL_0009:  ldc.i4.1
      IL_000a:  add
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Return<int32>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit testTask@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>
  {
    .field public class [runtime]System.Threading.Tasks.Task`1<int32> t
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [runtime]System.Threading.Tasks.Task`1<int32> t, class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>,int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0012:  newobj     instance void Test/'testTask@4-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!1>,!!2> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderExtensions.HighPriority::TaskBuilderBase.Bind<int32,int32,int32>(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase,
                                                                                                                                                                                                                                                                                                        class [runtime]System.Threading.Tasks.Task`1<!!0>,
                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!1>,!!2>>)
      IL_001c:  ret
    } 

  } 

  .method public static class [runtime]System.Threading.Tasks.Task`1<int32> testTask(class [runtime]System.Threading.Tasks.Task`1<int32> t) cil managed
  {
    
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void Test/testTask@4::.ctor(class [runtime]System.Threading.Tasks.Task`1<int32>,
                                                              class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
    IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
    IL_0014:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<int32>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
    IL_0019:  ret
  } 

} 
                """
            ]))

module TaskTryFinallyGeneration =

    [<Fact>]
    let ``check MoveNext of task try/finally optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize+"; "/debug:portable";"--realsig+"; "/tailcalls+" |],
            """
module Test

let testTask() = task { try 1+1 finally System.Console.WriteLine("finally") }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      
      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               bool V_3,
               bool V_4,
               class [runtime]System.Exception V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldc.i4.0
        IL_0008:  stloc.3
        .try
        {
          IL_0009:  ldc.i4.1
          IL_000a:  stloc.s    V_4
          IL_000c:  ldloc.s    V_4
          IL_000e:  stloc.3
          IL_000f:  leave.s    IL_0030

        }  
        catch [runtime]System.Object 
        {
          IL_0011:  castclass  [runtime]System.Exception
          IL_0016:  stloc.s    V_5
          IL_0018:  ldstr      "finally"
          IL_001d:  call       void [runtime]System.Console::WriteLine(string)
          IL_0022:  ldc.i4.1
          IL_0023:  stloc.s    V_4
          IL_0025:  rethrow
          IL_0027:  ldnull
          IL_0028:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
          IL_002d:  pop
          IL_002e:  leave.s    IL_0030

        }  
        IL_0030:  ldloc.3
        IL_0031:  brfalse.s  IL_0043

        IL_0033:  ldstr      "finally"
        IL_0038:  call       void [runtime]System.Console::WriteLine(string)
        IL_003d:  ldc.i4.1
        IL_003e:  stloc.s    V_4
        IL_0040:  nop
        IL_0041:  br.s       IL_0044

        IL_0043:  nop
        IL_0044:  ldloc.3
        IL_0045:  stloc.2
        IL_0046:  ldloc.2
        IL_0047:  brfalse.s  IL_0066

        IL_0049:  ldarg.0
        IL_004a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_004f:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_0054:  ldarg.0
        IL_0055:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_005a:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_005f:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_0064:  leave.s    IL_0074

        IL_0066:  leave.s    IL_0074

      }  
      catch [runtime]System.Object 
      {
        IL_0068:  castclass  [runtime]System.Exception
        IL_006d:  stloc.s    V_5
        IL_006f:  ldloc.s    V_5
        IL_0071:  stloc.1
        IL_0072:  leave.s    IL_0074

      }  
      IL_0074:  ldloc.1
      IL_0075:  stloc.s    V_5
      IL_0077:  ldloc.s    V_5
      IL_0079:  brtrue.s   IL_007c

      IL_007b:  ret

      IL_007c:  ldarg.0
      IL_007d:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_0082:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0087:  ldloc.s    V_5
      IL_0089:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_008e:  ret
    } 
                """
            ]))


    [<Fact>]
    let ``check MoveNext of task try/finally debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

let testTask() = task { try 1+1 finally System.Console.WriteLine("finally") }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit testTask@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0012:  newobj     instance void Test/'testTask@4-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_0017:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
      IL_001c:  ldsfld     class Test/'testTask@4-2' Test/'testTask@4-2'::@_instance
      IL_0021:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::TryFinally<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>,
                                                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0026:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@4-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_0007:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Zero<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
      IL_000c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@4-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class Test/'testTask@4-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldstr      "finally"
      IL_0006:  call       void [runtime]System.Console::WriteLine(string)
      IL_000b:  ldnull
      IL_000c:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Test/'testTask@4-2'::.ctor()
      IL_0005:  stsfld     class Test/'testTask@4-2' Test/'testTask@4-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static class [runtime]System.Threading.Tasks.Task`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> testTask() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void Test/testTask@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
    IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
    IL_0013:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
    IL_0018:  ret
  } 

} 
                """
            ]))

module TaskTryWithGeneration =

    [<Fact>]
    let ``check MoveNext of task try/with optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize+"; "/debug:portable";"--realsig+"; "/tailcalls+" |],
            """
module Test

let testTask() = task { try 1 with e -> System.Console.WriteLine("finally"); 2 }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      
      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               bool V_3,
               bool V_4,
               class [runtime]System.Exception V_5,
               bool V_6,
               class [runtime]System.Exception V_7)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldc.i4.0
        IL_0008:  stloc.3
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.s    V_4
        IL_000c:  ldnull
        IL_000d:  stloc.s    V_5
        .try
        {
          IL_000f:  ldc.i4.1
          IL_0010:  stloc.s    V_6
          IL_0012:  ldloc.s    V_6
          IL_0014:  stloc.3
          IL_0015:  leave.s    IL_0027

        }  
        catch [runtime]System.Object 
        {
          IL_0017:  castclass  [runtime]System.Exception
          IL_001c:  stloc.s    V_7
          IL_001e:  ldc.i4.1
          IL_001f:  stloc.s    V_4
          IL_0021:  ldloc.s    V_7
          IL_0023:  stloc.s    V_5
          IL_0025:  leave.s    IL_0027

        }  
        IL_0027:  ldloc.s    V_4
        IL_0029:  brfalse.s  IL_003e

        IL_002b:  ldloc.s    V_5
        IL_002d:  stloc.s    V_7
        IL_002f:  nop
        IL_0030:  ldstr      "finally"
        IL_0035:  call       void [runtime]System.Console::WriteLine(string)
        IL_003a:  ldc.i4.1
        IL_003b:  nop
        IL_003c:  br.s       IL_0040

        IL_003e:  ldloc.3
        IL_003f:  nop
        IL_0040:  stloc.2
        IL_0041:  ldloc.2
        IL_0042:  brfalse.s  IL_0061

        IL_0044:  ldarg.0
        IL_0045:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_004a:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_004f:  ldarg.0
        IL_0050:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0055:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_005a:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_005f:  leave.s    IL_006f

        IL_0061:  leave.s    IL_006f

      }  
      catch [runtime]System.Object 
      {
        IL_0063:  castclass  [runtime]System.Exception
        IL_0068:  stloc.s    V_5
        IL_006a:  ldloc.s    V_5
        IL_006c:  stloc.1
        IL_006d:  leave.s    IL_006f

      }  
      IL_006f:  ldloc.1
      IL_0070:  stloc.s    V_5
      IL_0072:  ldloc.s    V_5
      IL_0074:  brtrue.s   IL_0077

      IL_0076:  ret

      IL_0077:  ldarg.0
      IL_0078:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_007d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0082:  ldloc.s    V_5
      IL_0084:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_0089:  ret
    } 
                """
            ]))


    [<Fact>]
    let ``check MoveNext of task try/with debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

let testTask() = task { try 1 with e -> System.Console.WriteLine("with"); 2 }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@4-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Exception,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Exception,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-2'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [runtime]System.Exception _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (class [runtime]System.Exception V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldstr      "with"
      IL_0007:  call       void [runtime]System.Console::WriteLine(string)
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-2'::builder@
      IL_0012:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Zero<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
      IL_0017:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit testTask@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0012:  newobj     instance void Test/'testTask@4-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_0017:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
      IL_001c:  ldarg.0
      IL_001d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
      IL_0022:  newobj     instance void Test/'testTask@4-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::TryWith<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>,
                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Exception,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@4-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@4-1'::builder@
      IL_0007:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Zero<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
      IL_000c:  ret
    } 

  } 

  .method public static class [runtime]System.Threading.Tasks.Task`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> testTask() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void Test/testTask@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
    IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
    IL_0013:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
    IL_0018:  ret
  } 

} 
                """
            ]))

module TaskWhileLoopGeneration =

    [<Fact>]
    let ``check MoveNext of task while loop optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize+"; "/debug:portable";"--realsig+"; "/tailcalls+" |],
            """
module Test

let mutable x = 1
let testTask() = task { while x > 4 do System.Console.WriteLine("loop") }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      
      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               bool V_3,
               bool V_4,
               class [runtime]System.Exception V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@5::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldc.i4.1
        IL_0008:  stloc.3
        IL_0009:  br.s       IL_001d

        IL_000b:  ldstr      "loop"
        IL_0010:  call       void [runtime]System.Console::WriteLine(string)
        IL_0015:  ldc.i4.1
        IL_0016:  stloc.s    V_4
        IL_0018:  ldloc.s    V_4
        IL_001a:  stloc.3
        IL_001b:  ldc.i4.0
        IL_001c:  stloc.0
        IL_001d:  ldloc.3
        IL_001e:  brfalse.s  IL_002b

        IL_0020:  call       int32 Test::get_x()
        IL_0025:  ldc.i4.4
        IL_0026:  cgt
        IL_0028:  nop
        IL_0029:  br.s       IL_002d

        IL_002b:  ldc.i4.0
        IL_002c:  nop
        IL_002d:  brtrue.s   IL_000b

        IL_002f:  ldloc.3
        IL_0030:  stloc.2
        IL_0031:  ldloc.2
        IL_0032:  brfalse.s  IL_0051

        IL_0034:  ldarg.0
        IL_0035:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
        IL_003a:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_003f:  ldarg.0
        IL_0040:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
        IL_0045:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_004a:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_004f:  leave.s    IL_005f

        IL_0051:  leave.s    IL_005f

      }  
      catch [runtime]System.Object 
      {
        IL_0053:  castclass  [runtime]System.Exception
        IL_0058:  stloc.s    V_5
        IL_005a:  ldloc.s    V_5
        IL_005c:  stloc.1
        IL_005d:  leave.s    IL_005f

      }  
      IL_005f:  ldloc.1
      IL_0060:  stloc.s    V_5
      IL_0062:  ldloc.s    V_5
      IL_0064:  brtrue.s   IL_0067

      IL_0066:  ret

      IL_0067:  ldarg.0
      IL_0068:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
      IL_006d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0072:  ldloc.s    V_5
      IL_0074:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_0079:  ret
    } 
                """
            ]))


    [<Fact>]
    let ``check MoveNext of task while loop debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

let mutable x = 1 
let testTask() = task { while x > 4 do System.Console.WriteLine("loop") }
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit testTask@5
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
      IL_0006:  ldsfld     class Test/'testTask@5-1' Test/'testTask@5-1'::@_instance
      IL_000b:  ldarg.0
      IL_000c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
      IL_0011:  ldarg.0
      IL_0012:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
      IL_0017:  newobj     instance void Test/'testTask@5-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_001c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
      IL_0021:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::While<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>,
                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0026:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@5-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>
  {
    .field static assembly initonly class Test/'testTask@5-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  call       int32 Test::get_x()
      IL_0005:  ldc.i4.4
      IL_0006:  cgt
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Test/'testTask@5-1'::.ctor()
      IL_0005:  stsfld     class Test/'testTask@5-1' Test/'testTask@5-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'testTask@5-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@5-2'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldstr      "loop"
      IL_0005:  call       void [runtime]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/'testTask@5-2'::builder@
      IL_0010:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Zero<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
      IL_0015:  ret
    } 

  } 

  .field static assembly int32 x@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int32 get_x() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 Test::x@4
    IL_0005:  ret
  } 

  .method public specialname static void set_x(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 Test::x@4
    IL_0006:  ret
  } 

  .method public static class [runtime]System.Threading.Tasks.Task`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> testTask() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void Test/testTask@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
    IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
    IL_0013:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
    IL_0018:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  stsfld     int32 Test::x@4
    IL_0006:  ret
  } 

  .property int32 x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void Test::set_x(int32)
    .get int32 Test::get_x()
  } 
} 
                """
            ]))
#endif


module TaskTypeInference =
    // This tests the compilation of a case that hits corner cases in SRTP constraint processing.
    // See https://github.com/dotnet/fsharp/issues/12188
    [<Fact>]
    let ``check initially ambiguous SRTP task code ``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

open System.Threading.Tasks

let myFunction (f: string -> _, _i: 'T) =
    task {
        do! f ""
        return ()
    }

let myTuple : (string -> Task<unit>) * int = (fun (_s: string) -> Task.FromResult()), 1
(myFunction myTuple).Wait()
            """)

    // Test task code in generic position
    [<Fact>]
    let ``check generic task code ``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

open System.Threading.Tasks

type Generic1InGeneric1<'T>() =
    let run (computation: Task<'A>) =
        task { return! computation }

    member _.Run() = run (Task.FromResult 3)

type Generic2InGeneric1<'T1, 'T2>() =
    let run (computation: Task<'A>) =
        task { return! computation }

    member _.Run() = run (Task.FromResult 3)

let checkEquals s e a = if e = a then printfn $"test '{s}' passed" else failwith $"test '{s}' failed!, expected {e} got {a}"
Generic1InGeneric1<int>().Run().Result |> checkEquals "cwewe21" 3
Generic1InGeneric1<string>().Run().Result |> checkEquals "cwewe22" 3
Generic2InGeneric1<int, string>().Run().Result |> checkEquals "cwewe23" 3
Generic2InGeneric1<string, int>().Run().Result |> checkEquals "cwewe24" 3
printfn "test passed"

            """)


#if !DEBUG 

    [<Fact>]
    let ``check generic task exact code``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/optimize-"; "/debug:portable";"--realsig+"; "/tailcalls-" |],
            """
module Test

open System.Threading.Tasks
type Generic1InGeneric1<'T>() =
    let run (computation: Task<'A>) =
        task { return! computation }

    member _.Run() = run (Task.FromResult 3)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public beforefieldinit Generic1InGeneric1`1<T>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit clo@7<T,A>
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>,!A>>
    {
      .field public class [runtime]System.Threading.Tasks.Task`1<!A> computation
      .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class [runtime]System.Threading.Tasks.Task`1<!A> computation, class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>,!A>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [runtime]System.Threading.Tasks.Task`1<!1> class Test/Generic1InGeneric1`1/clo@7<!T,!A>::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder class Test/Generic1InGeneric1`1/clo@7<!T,!A>::builder@
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>,!A> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder class Test/Generic1InGeneric1`1/clo@7<!T,!A>::builder@
        IL_0006:  ldarg.0
        IL_0007:  ldfld      class [runtime]System.Threading.Tasks.Task`1<!1> class Test/Generic1InGeneric1`1/clo@7<!T,!A>::computation
        IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderExtensions.HighPriority::TaskBuilderBase.ReturnFrom<!A>(class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase,
                                                                                                                                                                                                                                                                                                 class [runtime]System.Threading.Tasks.Task`1<!!0>)
        IL_0011:  ret
      } 

    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public hidebysig instance class [runtime]System.Threading.Tasks.Task`1<int32> Run() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.3
      IL_0002:  call       class [runtime]System.Threading.Tasks.Task`1<!!0> [runtime]System.Threading.Tasks.Task::FromResult<int32>(!!0)
      IL_0007:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> class Test/Generic1InGeneric1`1<!T>::run<int32>(class [runtime]System.Threading.Tasks.Task`1<!!0>)
      IL_000c:  ret
    } 

    .method assembly hidebysig instance class [runtime]System.Threading.Tasks.Task`1<!!A> run<A>(class [runtime]System.Threading.Tasks.Task`1<!!A> computation) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldarg.1
      IL_0009:  ldloc.0
      IL_000a:  newobj     instance void class Test/Generic1InGeneric1`1/clo@7<!T,!!A>::.ctor(class [runtime]System.Threading.Tasks.Task`1<!1>,
                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder)
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase::Delay<!!0,!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!1>>)
      IL_0014:  callvirt   instance class [runtime]System.Threading.Tasks.Task`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder::Run<!!0>(class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ResumableCode`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!0>,!!0>)
      IL_0019:  ret
    } 

  } 

} 
                """
            ]))
#endif


#if NETCOREAPP

module ``Check stack traces`` =
    [<Fact>]
    let ``check stack trace of async exception from task``() =
        FSharp """
module Test

let t() =
    task {
        let! throws = task {
            do! System.Threading.Tasks.Task.Yield()
            failwith "Inner task exception"
            return ()
        }
        return throws
    }

let f() = t().Wait()

[<EntryPoint>]
let main _ =
    let stackTrace = try f(); failwith "huh?" with e -> e.ToString()

    if stackTrace.Contains("huh?") then
        failwith "unexpected - inner exception not generated correctly"

    if not (stackTrace.Contains("MoveNext()")) then
        failwith "expected MoveNext() on stack trace"

    if stackTrace.Contains("End of stack trace from previous location") then
        failwith "expected emit of CompilerGeneratedAttribute to suppress message in .NET Core stack walking "
    0
"""
        |> withOptimize
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
#endif


// Test that the SRTP Bind on a ValueTask is still generic in the bind type

module ``Check return attributes`` = 
    let incr (x:int) : [<Experimental("a")>] (int -> int) = (fun a -> a + x)

    // Putting a return attribute of any kind on f function should be respected
    // If the function is curried its inferred arity should be no more than 
    // the declared arguments (the F# rule that infers additional arguments 
    // should not kick in).
    [<Fact>]
    let ``check return attribute of curried function``() =
        match <@ incr 3 4 @> with
        | Quotations.Patterns.Application (Quotations.Patterns.Call(None, mi, [Quotations.DerivedPatterns.Int32 3]), Quotations.DerivedPatterns.Int32 4) ->
            if mi.GetParameters().Length <> 1 then  
               failwith "wrong number of parameters"
            if mi.ReturnTypeCustomAttributes.GetCustomAttributes(false).Length <> 1 then  
               failwith "wrong number of attributes"
            ()
        | _ -> failwith "curried function expected"


// Compilation test
// Test that the SRTP Bind on a ValueTask is still generic in the bind type
module ``SRTP Bind on a ValueTask is still generic in the bind type (nested)`` =
    let FindAsync() = ValueTask<'T>(Unchecked.defaultof<'T>)
    let TryFindAsync() : Task<'T voption> = task {
            let! r = FindAsync()
            if obj.ReferenceEquals(r, null) then return ValueNone
            else return ValueSome r
        }
    let t1 : Task<int voption> = TryFindAsync() // test TryFindAsync is generic
    let t2 : Task<string voption> = TryFindAsync() // test TryFindAsync is generic
    
// Compilation test
// Test that the SRTP Bind on a ValueTask is still generic in the bind type
module ``SRTP Bind on a ValueTask is still generic in the bind type`` =
    let FindAsync() = ValueTask<'T>(Unchecked.defaultof<'T>)
    let TryFindAsync() : Task<'T> = task {
            let! r = FindAsync()
            return r
        }
    
// Compilation test
module ``SRTP ReturnFrom on a ValueTask is still generic in the bind type`` =
    let FindAsync() = ValueTask<'T>(Unchecked.defaultof<'T>)
    let TryFindAsync() : Task<'T> = task {
            return! FindAsync()
        }
    
    let t1 : Task<int voption> = TryFindAsync() // test TryFindAsync is generic
    let t2 : Task<string voption> = TryFindAsync() // test TryFindAsync is generic
