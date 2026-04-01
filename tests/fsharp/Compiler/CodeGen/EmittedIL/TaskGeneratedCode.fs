// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open Xunit

open System
open System.Threading.Tasks

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
.method public strict virtual instance void MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
               int32 V_5,
               class [runtime]System.Exception V_6,
               class [runtime]System.Exception V_7)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_000d:  stloc.3
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_0014:  stloc.s    V_4
        IL_0016:  ldc.i4.1
        IL_0017:  stloc.s    V_5
        IL_0019:  ldarg.0
        IL_001a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
        IL_001f:  ldloc.s    V_5
        IL_0021:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
        IL_0026:  ldc.i4.1
        IL_0027:  stloc.2
        IL_0028:  ldloc.2
        IL_0029:  brfalse.s  IL_0048

        IL_002b:  ldarg.0
        IL_002c:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
        IL_0031:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
        IL_0036:  ldarg.0
        IL_0037:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
        IL_003c:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
        IL_0041:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
        IL_0046:  leave.s    IL_0056

        IL_0048:  leave.s    IL_0056

      }  
      catch [runtime]System.Object 
      {
        IL_004a:  castclass  [runtime]System.Exception
        IL_004f:  stloc.s    V_6
        IL_0051:  ldloc.s    V_6
        IL_0053:  stloc.1
        IL_0054:  leave.s    IL_0056

      }  
      IL_0056:  ldloc.1
      IL_0057:  stloc.s    V_7
      IL_0059:  ldloc.s    V_7
      IL_005b:  brtrue.s   IL_005e

      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
      IL_0064:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
      IL_0069:  ldloc.s    V_7
      IL_006b:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
      IL_0070:  ret
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
    .method public strict virtual instance void 
                MoveNext() cil managed
        {
          .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

          .maxstack  5
          .locals init (int32 V_0,
                   class [runtime]System.Exception V_1,
                   bool V_2,
                   class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
                   class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
                   class [runtime]System.Threading.Tasks.Task`1<int32> V_5,
                   bool V_6,
                   bool V_7,
                   int32 V_8,
                   int32 V_9,
                   int32 V_10,
                   class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_11,
                   int32 V_12,
                   valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_13,
                   class [runtime]System.Exception V_14,
                   class [runtime]System.Exception V_15)
          IL_0000:  ldarg.0
          IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
          IL_0006:  stloc.0
          IL_0007:  ldloc.0
          IL_0008:  ldc.i4.1
          IL_0009:  sub
          IL_000a:  switch     ( 
                                IL_0015)
          IL_0013:  br.s       IL_0018

          IL_0015:  nop
          IL_0016:  br.s       IL_001b

          IL_0018:  nop
          IL_0019:  br.s       IL_001b

          .try
          {
            IL_001b:  ldloc.0
            IL_001c:  ldc.i4.1
            IL_001d:  sub
            IL_001e:  switch     ( 
                                  IL_0029)
            IL_0027:  br.s       IL_002c

            IL_0029:  nop
            IL_002a:  br.s       IL_0068

            IL_002c:  nop
            IL_002d:  br.s       IL_002f

            IL_002f:  ldarg.0
            IL_0030:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
            IL_0035:  stloc.3
            IL_0036:  ldarg.0
            IL_0037:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
            IL_003c:  stloc.s    V_4
            IL_003e:  ldarg.0
            IL_003f:  ldfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
            IL_0044:  stloc.s    V_5
            IL_0046:  ldarg.0
            IL_0047:  ldloc.s    V_5
            IL_0049:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
            IL_004e:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_0053:  ldc.i4.1
            IL_0054:  stloc.s    V_6
            IL_0056:  ldarg.0
            IL_0057:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_005c:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
            IL_0061:  brfalse.s  IL_0065

            IL_0063:  br.s       IL_007e

            IL_0065:  ldc.i4.0
            IL_0066:  brfalse.s  IL_006c

            IL_0068:  ldc.i4.1
            IL_0069:  nop
            IL_006a:  br.s       IL_0075

            IL_006c:  ldarg.0
            IL_006d:  ldc.i4.1
            IL_006e:  stfld      int32 Test/testTask@4::ResumptionPoint
            IL_0073:  ldc.i4.0
            IL_0074:  nop
            IL_0075:  stloc.s    V_7
            IL_0077:  ldloc.s    V_7
            IL_0079:  stloc.s    V_6
            IL_007b:  nop
            IL_007c:  br.s       IL_007f

            IL_007e:  nop
            IL_007f:  ldloc.s    V_6
            IL_0081:  brfalse.s  IL_00b7

            IL_0083:  ldarg.0
            IL_0084:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_0089:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
            IL_008e:  stloc.s    V_8
            IL_0090:  ldloc.s    V_8
            IL_0092:  stloc.s    V_9
            IL_0094:  ldloc.s    V_9
            IL_0096:  stloc.s    V_10
            IL_0098:  ldarg.0
            IL_0099:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
            IL_009e:  stloc.s    V_11
            IL_00a0:  ldloc.s    V_10
            IL_00a2:  ldc.i4.1
            IL_00a3:  add
            IL_00a4:  stloc.s    V_12
            IL_00a6:  ldarg.0
            IL_00a7:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00ac:  ldloc.s    V_12
            IL_00ae:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
            IL_00b3:  ldc.i4.1
            IL_00b4:  nop
            IL_00b5:  br.s       IL_00d0

            IL_00b7:  ldarg.0
            IL_00b8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00bd:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
            IL_00c2:  ldarg.0
            IL_00c3:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_00c8:  ldarg.0
            IL_00c9:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype Test/testTask@4>(!!0&,
                                                                                                                                                                                                                                                               !!1&)
            IL_00ce:  ldc.i4.0
            IL_00cf:  nop
            IL_00d0:  brfalse.s  IL_00de

            IL_00d2:  ldarg.0
            IL_00d3:  ldloc.s    V_13
            IL_00d5:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_00da:  ldc.i4.1
            IL_00db:  nop
            IL_00dc:  br.s       IL_00e0

            IL_00de:  ldc.i4.0
            IL_00df:  nop
            IL_00e0:  stloc.2
            IL_00e1:  ldloc.2
            IL_00e2:  brfalse.s  IL_0101

            IL_00e4:  ldarg.0
            IL_00e5:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00ea:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
            IL_00ef:  ldarg.0
            IL_00f0:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00f5:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
            IL_00fa:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
            IL_00ff:  leave.s    IL_010f

            IL_0101:  leave.s    IL_010f

          }  
          catch [runtime]System.Object 
          {
            IL_0103:  castclass  [runtime]System.Exception
            IL_0108:  stloc.s    V_14
            IL_010a:  ldloc.s    V_14
            IL_010c:  stloc.1
            IL_010d:  leave.s    IL_010f

          }  
          IL_010f:  ldloc.1
          IL_0110:  stloc.s    V_15
          IL_0112:  ldloc.s    V_15
          IL_0114:  brtrue.s   IL_0117

          IL_0116:  ret

          IL_0117:  ldarg.0
          IL_0118:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
          IL_011d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
          IL_0122:  ldloc.s    V_15
          IL_0124:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
          IL_0129:  ret
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
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
               bool V_5,
               bool V_6,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_7,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_8,
               class [runtime]System.Exception V_9,
               bool V_10,
               bool V_11,
               class [runtime]System.Exception V_12,
               class [runtime]System.Exception V_13)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_000d:  stloc.3
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_0014:  stloc.s    V_4
        IL_0016:  ldc.i4.0
        IL_0017:  stloc.s    V_5
        .try
        {
          IL_0019:  ldarg.0
          IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
          IL_001f:  stloc.s    V_7
          IL_0021:  nop
          IL_0022:  ldarg.0
          IL_0023:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
          IL_0028:  stloc.s    V_8
          IL_002a:  ldc.i4.1
          IL_002b:  stloc.s    V_6
          IL_002d:  ldloc.s    V_6
          IL_002f:  stloc.s    V_5
          IL_0031:  leave.s    IL_0053

        }  
        catch [runtime]System.Object 
        {
          IL_0033:  castclass  [runtime]System.Exception
          IL_0038:  stloc.s    V_9
          IL_003a:  nop
          IL_003b:  ldstr      "finally"
          IL_0040:  call       void [runtime]System.Console::WriteLine(string)
          IL_0045:  ldc.i4.1
          IL_0046:  stloc.s    V_10
          IL_0048:  rethrow
          IL_004a:  ldnull
          IL_004b:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
          IL_0050:  pop
          IL_0051:  leave.s    IL_0053

        }  
        IL_0053:  ldloc.s    V_5
        IL_0055:  brfalse.s  IL_0068

        IL_0057:  nop
        IL_0058:  ldstr      "finally"
        IL_005d:  call       void [runtime]System.Console::WriteLine(string)
        IL_0062:  ldc.i4.1
        IL_0063:  stloc.s    V_11
        IL_0065:  nop
        IL_0066:  br.s       IL_0069

        IL_0068:  nop
        IL_0069:  ldloc.s    V_5
        IL_006b:  stloc.2
        IL_006c:  ldloc.2
        IL_006d:  brfalse.s  IL_008c

        IL_006f:  ldarg.0
        IL_0070:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0075:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_007a:  ldarg.0
        IL_007b:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0080:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_0085:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_008a:  leave.s    IL_009a

        IL_008c:  leave.s    IL_009a

      }  
      catch [runtime]System.Object 
      {
        IL_008e:  castclass  [runtime]System.Exception
        IL_0093:  stloc.s    V_12
        IL_0095:  ldloc.s    V_12
        IL_0097:  stloc.1
        IL_0098:  leave.s    IL_009a

      }  
      IL_009a:  ldloc.1
      IL_009b:  stloc.s    V_13
      IL_009d:  ldloc.s    V_13
      IL_009f:  brtrue.s   IL_00a2

      IL_00a1:  ret

      IL_00a2:  ldarg.0
      IL_00a3:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_00a8:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_00ad:  ldloc.s    V_13
      IL_00af:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_00b4:  ret
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
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
               bool V_5,
               bool V_6,
               class [runtime]System.Exception V_7,
               bool V_8,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_9,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_10,
               class [runtime]System.Exception V_11,
               class [runtime]System.Exception V_12,
               class [runtime]System.Exception V_13,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_14,
               class [runtime]System.Exception V_15,
               class [runtime]System.Exception V_16)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_000d:  stloc.3
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_0014:  stloc.s    V_4
        IL_0016:  ldc.i4.0
        IL_0017:  stloc.s    V_5
        IL_0019:  ldc.i4.0
        IL_001a:  stloc.s    V_6
        IL_001c:  ldnull
        IL_001d:  stloc.s    V_7
        .try
        {
          IL_001f:  ldarg.0
          IL_0020:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
          IL_0025:  stloc.s    V_9
          IL_0027:  nop
          IL_0028:  ldarg.0
          IL_0029:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
          IL_002e:  stloc.s    V_10
          IL_0030:  ldc.i4.1
          IL_0031:  stloc.s    V_8
          IL_0033:  ldloc.s    V_8
          IL_0035:  stloc.s    V_5
          IL_0037:  leave.s    IL_0049

        }  
        catch [runtime]System.Object 
        {
          IL_0039:  castclass  [runtime]System.Exception
          IL_003e:  stloc.s    V_11
          IL_0040:  ldc.i4.1
          IL_0041:  stloc.s    V_6
          IL_0043:  ldloc.s    V_11
          IL_0045:  stloc.s    V_7
          IL_0047:  leave.s    IL_0049

        }  
        IL_0049:  ldloc.s    V_6
        IL_004b:  brfalse.s  IL_006b

        IL_004d:  ldloc.s    V_7
        IL_004f:  stloc.s    V_12
        IL_0051:  ldloc.s    V_12
        IL_0053:  stloc.s    V_13
        IL_0055:  ldstr      "with"
        IL_005a:  call       void [runtime]System.Console::WriteLine(string)
        IL_005f:  ldarg.0
        IL_0060:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@4::builder@
        IL_0065:  stloc.s    V_14
        IL_0067:  ldc.i4.1
        IL_0068:  nop
        IL_0069:  br.s       IL_006e

        IL_006b:  ldloc.s    V_5
        IL_006d:  nop
        IL_006e:  stloc.2
        IL_006f:  ldloc.2
        IL_0070:  brfalse.s  IL_008f

        IL_0072:  ldarg.0
        IL_0073:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0078:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_007d:  ldarg.0
        IL_007e:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0083:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_0088:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_008d:  leave.s    IL_009d

        IL_008f:  leave.s    IL_009d

      }  
      catch [runtime]System.Object 
      {
        IL_0091:  castclass  [runtime]System.Exception
        IL_0096:  stloc.s    V_15
        IL_0098:  ldloc.s    V_15
        IL_009a:  stloc.1
        IL_009b:  leave.s    IL_009d

      }  
      IL_009d:  ldloc.1
      IL_009e:  stloc.s    V_16
      IL_00a0:  ldloc.s    V_16
      IL_00a2:  brtrue.s   IL_00a5

      IL_00a4:  ret

      IL_00a5:  ldarg.0
      IL_00a6:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_00ab:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_00b0:  ldloc.s    V_16
      IL_00b2:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_00b7:  ret
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
        .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

      .maxstack  4
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
               bool V_5,
               bool V_6,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_7,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_8,
               class [runtime]System.Exception V_9,
               class [runtime]System.Exception V_10)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@5::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
        IL_000d:  stloc.3
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
        IL_0014:  stloc.s    V_4
        IL_0016:  ldc.i4.1
        IL_0017:  stloc.s    V_5
        IL_0019:  br.s       IL_003e

        IL_001b:  ldarg.0
        IL_001c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
        IL_0021:  stloc.s    V_7
        IL_0023:  ldstr      "loop"
        IL_0028:  call       void [runtime]System.Console::WriteLine(string)
        IL_002d:  ldarg.0
        IL_002e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder Test/testTask@5::builder@
        IL_0033:  stloc.s    V_8
        IL_0035:  ldc.i4.1
        IL_0036:  stloc.s    V_6
        IL_0038:  ldloc.s    V_6
        IL_003a:  stloc.s    V_5
        IL_003c:  ldc.i4.0
        IL_003d:  stloc.0
        IL_003e:  ldloc.s    V_5
        IL_0040:  brfalse.s  IL_004d

        IL_0042:  call       int32 Test::get_x()
        IL_0047:  ldc.i4.4
        IL_0048:  cgt
        IL_004a:  nop
        IL_004b:  br.s       IL_004f

        IL_004d:  ldc.i4.0
        IL_004e:  nop
        IL_004f:  brtrue.s   IL_001b

        IL_0051:  ldloc.s    V_5
        IL_0053:  stloc.2
        IL_0054:  ldloc.2
        IL_0055:  brfalse.s  IL_0074

        IL_0057:  ldarg.0
        IL_0058:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
        IL_005d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_0062:  ldarg.0
        IL_0063:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
        IL_0068:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_006d:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_0072:  leave.s    IL_0082

        IL_0074:  leave.s    IL_0082

      }  
      catch [runtime]System.Object 
      {
        IL_0076:  castclass  [runtime]System.Exception
        IL_007b:  stloc.s    V_9
        IL_007d:  ldloc.s    V_9
        IL_007f:  stloc.1
        IL_0080:  leave.s    IL_0082

      }  
      IL_0082:  ldloc.1
      IL_0083:  stloc.s    V_10
      IL_0085:  ldloc.s    V_10
      IL_0087:  brtrue.s   IL_008a

      IL_0089:  ret

      IL_008a:  ldarg.0
      IL_008b:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
      IL_0090:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0095:  ldloc.s    V_10
      IL_0097:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_009c:  ret
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
    .class auto autochar sealed nested assembly beforefieldinit specialname clo@7<T,A>
           extends [runtime]System.ValueType
           implements [runtime]System.Runtime.CompilerServices.IAsyncStateMachine,
                      class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine`1<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>>
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A> Data
      .field public int32 ResumptionPoint
      .field public class [runtime]System.Threading.Tasks.Task`1<!A> computation
      .field public class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder builder@
      .field public valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!A> awaiter
      .method public strict virtual instance void MoveNext() cil managed
      {
        .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext

        .maxstack  5
        .locals init (int32 V_0,
                 class [runtime]System.Exception V_1,
                 bool V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_3,
                 class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderBase V_4,
                 class [runtime]System.Threading.Tasks.Task`1<!A> V_5,
                 bool V_6,
                 bool V_7,
                 !A V_8,
                 !A V_9,
                 valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!A> V_10,
                 class [runtime]System.Exception V_11,
                 class [runtime]System.Exception V_12)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::ResumptionPoint
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  ldc.i4.1
        IL_0009:  sub
        IL_000a:  switch     ( 
                              IL_0015)
        IL_0013:  br.s       IL_0018

        IL_0015:  nop
        IL_0016:  br.s       IL_001d

        IL_0018:  nop
        IL_0019:  br.s       IL_001b

        IL_001b:  ldnull
        IL_001c:  stloc.1
        .try
        {
          IL_001d:  ldloc.0
          IL_001e:  ldc.i4.1
          IL_001f:  sub
          IL_0020:  switch     ( 
                                IL_002b)
          IL_0029:  br.s       IL_002e

          IL_002b:  nop
          IL_002c:  br.s       IL_006a

          IL_002e:  nop
          IL_002f:  br.s       IL_0031

          IL_0031:  ldarg.0
          IL_0032:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::builder@
          IL_0037:  stloc.3
          IL_0038:  ldarg.0
          IL_0039:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::builder@
          IL_003e:  stloc.s    V_4
          IL_0040:  ldarg.0
          IL_0041:  ldfld      class [runtime]System.Threading.Tasks.Task`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::computation
          IL_0046:  stloc.s    V_5
          IL_0048:  ldarg.0
          IL_0049:  ldloc.s    V_5
          IL_004b:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<!A>::GetAwaiter()
          IL_0050:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::awaiter
          IL_0055:  ldc.i4.1
          IL_0056:  stloc.s    V_6
          IL_0058:  ldarg.0
          IL_0059:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::awaiter
          IL_005e:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!A>::get_IsCompleted()
          IL_0063:  brfalse.s  IL_0067

          IL_0065:  br.s       IL_0080

          IL_0067:  ldc.i4.0
          IL_0068:  brfalse.s  IL_006e

          IL_006a:  ldc.i4.1
          IL_006b:  nop
          IL_006c:  br.s       IL_0077

          IL_006e:  ldarg.0
          IL_006f:  ldc.i4.1
          IL_0070:  stfld      int32 valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::ResumptionPoint
          IL_0075:  ldc.i4.0
          IL_0076:  nop
          IL_0077:  stloc.s    V_7
          IL_0079:  ldloc.s    V_7
          IL_007b:  stloc.s    V_6
          IL_007d:  nop
          IL_007e:  br.s       IL_0081

          IL_0080:  nop
          IL_0081:  ldloc.s    V_6
          IL_0083:  brfalse.s  IL_00a7

          IL_0085:  ldarg.0
          IL_0086:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::awaiter
          IL_008b:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!A>::GetResult()
          IL_0090:  stloc.s    V_8
          IL_0092:  ldloc.s    V_8
          IL_0094:  stloc.s    V_9
          IL_0096:  ldarg.0
          IL_0097:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
          IL_009c:  ldloc.s    V_9
          IL_009e:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::Result
          IL_00a3:  ldc.i4.1
          IL_00a4:  nop
          IL_00a5:  br.s       IL_00c0

          IL_00a7:  ldarg.0
          IL_00a8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
          IL_00ad:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
          IL_00b2:  ldarg.0
          IL_00b3:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::awaiter
          IL_00b8:  ldarg.0
          IL_00b9:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1>,valuetype Test/Generic1InGeneric1`1/clo@7<!0,!1>>(!!0&,
                                                                                                                                                                                                                                                                                  !!1&)
          IL_00be:  ldc.i4.0
          IL_00bf:  nop
          IL_00c0:  brfalse.s  IL_00d6

          IL_00c2:  ldarg.0
          IL_00c3:  ldloca.s   V_10
          IL_00c5:  initobj    valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!A>
          IL_00cb:  ldloc.s    V_10
          IL_00cd:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::awaiter
          IL_00d2:  ldc.i4.1
          IL_00d3:  nop
          IL_00d4:  br.s       IL_00d8

          IL_00d6:  ldc.i4.0
          IL_00d7:  nop
          IL_00d8:  stloc.2
          IL_00d9:  ldloc.2
          IL_00da:  brfalse.s  IL_00f9

          IL_00dc:  ldarg.0
          IL_00dd:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
          IL_00e2:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
          IL_00e7:  ldarg.0
          IL_00e8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
          IL_00ed:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::Result
          IL_00f2:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::SetResult(!0)
          IL_00f7:  leave.s    IL_0107

          IL_00f9:  leave.s    IL_0107

        }  
        catch [runtime]System.Object 
        {
          IL_00fb:  castclass  [runtime]System.Exception
          IL_0100:  stloc.s    V_11
          IL_0102:  ldloc.s    V_11
          IL_0104:  stloc.1
          IL_0105:  leave.s    IL_0107

        }  
        IL_0107:  ldloc.1
        IL_0108:  stloc.s    V_12
        IL_010a:  ldloc.s    V_12
        IL_010c:  brtrue.s   IL_010f

        IL_010e:  ret

        IL_010f:  ldarg.0
        IL_0110:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
        IL_0115:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
        IL_011a:  ldloc.s    V_12
        IL_011c:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::SetException(class [netstandard]System.Exception)
        IL_0121:  ret
      } 

      .method public strict virtual instance void SetStateMachine(class [runtime]System.Runtime.CompilerServices.IAsyncStateMachine state) cil managed
      {
        .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine

        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
        IL_0006:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
        IL_000b:  ldarg.1
        IL_000c:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::SetStateMachine(class [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine)
        IL_0011:  ret
      } 

      .method public strict virtual instance int32 get_ResumptionPoint() cil managed
      {
        .override  method instance int32 class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine`1<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>>::get_ResumptionPoint()

        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::ResumptionPoint
        IL_0006:  ret
      } 

      .method public strict virtual instance valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A> get_Data() cil managed
      {
        .override  method instance !0 class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine`1<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>>::get_Data()

        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
        IL_0006:  ret
      } 

      .method public strict virtual instance void set_Data(valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A> 'value') cil managed
      {
        .override  method instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine`1<valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>>::set_Data(!0)

        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!A>::Data
        IL_0007:  ret
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

      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_0,
               class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder V_1,
               valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A> V_2,
               valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>& V_3)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder [FSharp.Core]Microsoft.FSharp.Control.TaskBuilderModule::get_task()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  stloc.1
      IL_0008:  ldloca.s   V_2
      IL_000a:  initobj    valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>
      IL_0010:  ldloca.s   V_2
      IL_0012:  stloc.3
      IL_0013:  ldloc.3
      IL_0014:  ldarg.1
      IL_0015:  stfld      class [runtime]System.Threading.Tasks.Task`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>::computation
      IL_001a:  ldloc.3
      IL_001b:  ldloc.0
      IL_001c:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.TaskBuilder valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>::builder@
      IL_0021:  ldloc.3
      IL_0022:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>::Data
      IL_0027:  call       valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!A>::Create()
      IL_002c:  stfld      valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_0031:  ldloc.3
      IL_0032:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>::Data
      IL_0037:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_003c:  ldloc.3
      IL_003d:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!0>::Start<valuetype Test/Generic1InGeneric1`1/clo@7<!0,!!0>>(!!0&)
      IL_0042:  ldloc.3
      IL_0043:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!1> valuetype Test/Generic1InGeneric1`1/clo@7<!T,!!A>::Data
      IL_0048:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_004d:  call       instance class [netstandard]System.Threading.Tasks.Task`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!A>::get_Task()
      IL_0052:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
} 
            """
            ]))
#endif


#if NETCOREAPP

module ``Check stack traces`` = 
    [<Fact>]
    let ``check stack trace of async exception from task``() =
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
        
        let stackTrace = try f(); failwith "huh?" with e -> e.ToString()

        if stackTrace.Contains("huh?") then
           failwith "unexpected - inner exception not generated correctly"
        
        if not (stackTrace.Contains("MoveNext()")) then
           failwith "expected MoveNext() on stack trace"

        if stackTrace.Contains("End of stack trace from previous location") then
           failwith "expected emit of CompilerGeneratedAttribute to suppress message in .NET Core stack walking "
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
