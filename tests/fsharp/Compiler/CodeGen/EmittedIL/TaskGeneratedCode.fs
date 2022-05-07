// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open NUnit.Framework

open System
open System.Threading.Tasks

// These tests are sensitive to the fact that FSharp.Core is compiled release, not debug
// The code generated changes slightly in debug mode
#if !DEBUG 
// Check the exact code produced for tasks
[<TestFixture>]
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

    [<Test>]
    let ``check MoveNext of simple task debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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
                    int32 V_3,
                    class [runtime]System.Exception V_4,
                    class [runtime]System.Exception V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
    IL_0006:  stloc.0
    .try
    {
    IL_0007:  ldc.i4.1
    IL_0008:  stloc.3
    IL_0009:  ldarg.0
    IL_000a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_000f:  ldloc.3
    IL_0010:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.2
    IL_0017:  ldloc.2
    IL_0018:  brfalse.s  IL_0037
    
    IL_001a:  ldarg.0
    IL_001b:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0020:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0025:  ldarg.0
    IL_0026:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_002b:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0030:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_0035:  leave.s    IL_0045
    
    IL_0037:  leave.s    IL_0045
    
    }  
    catch [runtime]System.Object 
    {
    IL_0039:  castclass  [runtime]System.Exception
    IL_003e:  stloc.s    V_4
    IL_0040:  ldloc.s    V_4
    IL_0042:  stloc.1
    IL_0043:  leave.s    IL_0045
    
    }  
    IL_0045:  ldloc.1
    IL_0046:  stloc.s    V_5
    IL_0048:  ldloc.s    V_5
    IL_004a:  brtrue.s   IL_004d
    
    IL_004c:  ret
    
    IL_004d:  ldarg.0
    IL_004e:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0053:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0058:  ldloc.s    V_5
    IL_005a:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
    IL_005f:  ret
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

    [<Test>]
    let ``check MoveNext of simple task optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize+";"/debug:portable";"/tailcalls+" |],
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


    [<Test>]
    let ``check MoveNext of simple binding task debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview"; "/debug:portable"; "/optimize-"; "/tailcalls-" |],
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
                   class [runtime]System.Threading.Tasks.Task`1<int32> V_3,
                   bool V_4,
                   bool V_5,
                   int32 V_6,
                   int32 V_7,
                   int32 V_8,
                   int32 V_9,
                   valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_10,
                   class [runtime]System.Exception V_11,
                   class [runtime]System.Exception V_12)
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
          IL_0016:  br.s       IL_0019
    
          IL_0018:  nop
          .try
          {
            IL_0019:  ldloc.0
            IL_001a:  ldc.i4.1
            IL_001b:  sub
            IL_001c:  switch     ( 
                                  IL_0027)
            IL_0025:  br.s       IL_002a
    
            IL_0027:  nop
            IL_0028:  br.s       IL_0053
    
            IL_002a:  nop
            IL_002b:  ldarg.0
            IL_002c:  ldfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
            IL_0031:  stloc.3
            IL_0032:  ldarg.0
            IL_0033:  ldloc.3
            IL_0034:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
            IL_0039:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_003e:  ldc.i4.1
            IL_003f:  stloc.s    V_4
            IL_0041:  ldarg.0
            IL_0042:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_0047:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
            IL_004c:  brfalse.s  IL_0050
    
            IL_004e:  br.s       IL_0069
    
            IL_0050:  ldc.i4.0
            IL_0051:  brfalse.s  IL_0057
    
            IL_0053:  ldc.i4.1
            IL_0054:  nop
            IL_0055:  br.s       IL_0060
    
            IL_0057:  ldarg.0
            IL_0058:  ldc.i4.1
            IL_0059:  stfld      int32 Test/testTask@4::ResumptionPoint
            IL_005e:  ldc.i4.0
            IL_005f:  nop
            IL_0060:  stloc.s    V_5
            IL_0062:  ldloc.s    V_5
            IL_0064:  stloc.s    V_4
            IL_0066:  nop
            IL_0067:  br.s       IL_006a
    
            IL_0069:  nop
            IL_006a:  ldloc.s    V_4
            IL_006c:  brfalse.s  IL_009a
    
            IL_006e:  ldarg.0
            IL_006f:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_0074:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
            IL_0079:  stloc.s    V_6
            IL_007b:  ldloc.s    V_6
            IL_007d:  stloc.s    V_7
            IL_007f:  ldloc.s    V_7
            IL_0081:  stloc.s    V_8
            IL_0083:  ldloc.s    V_8
            IL_0085:  ldc.i4.1
            IL_0086:  add
            IL_0087:  stloc.s    V_9
            IL_0089:  ldarg.0
            IL_008a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_008f:  ldloc.s    V_9
            IL_0091:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
            IL_0096:  ldc.i4.1
            IL_0097:  nop
            IL_0098:  br.s       IL_00b3
    
            IL_009a:  ldarg.0
            IL_009b:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00a0:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
            IL_00a5:  ldarg.0
            IL_00a6:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_00ab:  ldarg.0
            IL_00ac:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype Test/testTask@4>(!!0&,
                                                                                                                                                                                                                                                                        !!1&)
            IL_00b1:  ldc.i4.0
            IL_00b2:  nop
            IL_00b3:  brfalse.s  IL_00c1
    
            IL_00b5:  ldarg.0
            IL_00b6:  ldloc.s    V_10
            IL_00b8:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
            IL_00bd:  ldc.i4.1
            IL_00be:  nop
            IL_00bf:  br.s       IL_00c3
    
            IL_00c1:  ldc.i4.0
            IL_00c2:  nop
            IL_00c3:  stloc.2
            IL_00c4:  ldloc.2
            IL_00c5:  brfalse.s  IL_00e4
    
            IL_00c7:  ldarg.0
            IL_00c8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00cd:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
            IL_00d2:  ldarg.0
            IL_00d3:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
            IL_00d8:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
            IL_00dd:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
            IL_00e2:  leave.s    IL_00f2
    
            IL_00e4:  leave.s    IL_00f2
    
          }  
          catch [runtime]System.Object 
          {
            IL_00e6:  castclass  [runtime]System.Exception
            IL_00eb:  stloc.s    V_11
            IL_00ed:  ldloc.s    V_11
            IL_00ef:  stloc.1
            IL_00f0:  leave.s    IL_00f2
    
          }  
          IL_00f2:  ldloc.1
          IL_00f3:  stloc.s    V_12
          IL_00f5:  ldloc.s    V_12
          IL_00f7:  brtrue.s   IL_00fa
    
          IL_00f9:  ret
    
          IL_00fa:  ldarg.0
          IL_00fb:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
          IL_0100:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
          IL_0105:  ldloc.s    V_12
          IL_0107:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
          IL_010c:  ret
        } 
                """
            ]))

module TaskTryFinallyGeneration =

    [<Test>]
    let ``check MoveNext of task try/finally optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize+";"/debug:portable";"/tailcalls+" |],
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


    [<Test>]
    let ``check MoveNext of task try/finally debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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
               class [runtime]System.Exception V_5,
               bool V_6,
               bool V_7,
               class [runtime]System.Exception V_8,
               class [runtime]System.Exception V_9)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldc.i4.0
        IL_0008:  stloc.3
        .try
        {
          IL_0009:  nop
          IL_000a:  ldc.i4.1
          IL_000b:  stloc.s    V_4
          IL_000d:  ldloc.s    V_4
          IL_000f:  stloc.3
          IL_0010:  leave.s    IL_0032

        }  
        catch [runtime]System.Object 
        {
          IL_0012:  castclass  [runtime]System.Exception
          IL_0017:  stloc.s    V_5
          IL_0019:  nop
          IL_001a:  ldstr      "finally"
          IL_001f:  call       void [runtime]System.Console::WriteLine(string)
          IL_0024:  ldc.i4.1
          IL_0025:  stloc.s    V_6
          IL_0027:  rethrow
          IL_0029:  ldnull
          IL_002a:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
          IL_002f:  pop
          IL_0030:  leave.s    IL_0032

        }  
        IL_0032:  ldloc.3
        IL_0033:  brfalse.s  IL_0046

        IL_0035:  nop
        IL_0036:  ldstr      "finally"
        IL_003b:  call       void [runtime]System.Console::WriteLine(string)
        IL_0040:  ldc.i4.1
        IL_0041:  stloc.s    V_7
        IL_0043:  nop
        IL_0044:  br.s       IL_0047

        IL_0046:  nop
        IL_0047:  ldloc.3
        IL_0048:  stloc.2
        IL_0049:  ldloc.2
        IL_004a:  brfalse.s  IL_0069

        IL_004c:  ldarg.0
        IL_004d:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0052:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_0057:  ldarg.0
        IL_0058:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_005d:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_0062:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_0067:  leave.s    IL_0077

        IL_0069:  leave.s    IL_0077

      }  
      catch [runtime]System.Object 
      {
        IL_006b:  castclass  [runtime]System.Exception
        IL_0070:  stloc.s    V_8
        IL_0072:  ldloc.s    V_8
        IL_0074:  stloc.1
        IL_0075:  leave.s    IL_0077

      }  
      IL_0077:  ldloc.1
      IL_0078:  stloc.s    V_9
      IL_007a:  ldloc.s    V_9
      IL_007c:  brtrue.s   IL_007f

      IL_007e:  ret

      IL_007f:  ldarg.0
      IL_0080:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_0085:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_008a:  ldloc.s    V_9
      IL_008c:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_0091:  ret
    } 
                """
            ]))

module TaskTryWithGeneration =

    [<Test>]
    let ``check MoveNext of task try/with optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize+";"/debug:portable";"/tailcalls+" |],
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


    [<Test>]
    let ``check MoveNext of task try/with debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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
               bool V_3,
               bool V_4,
               class [runtime]System.Exception V_5,
               bool V_6,
               class [runtime]System.Exception V_7,
               class [runtime]System.Exception V_8,
               class [runtime]System.Exception V_9,
               class [runtime]System.Exception V_10,
               class [runtime]System.Exception V_11)
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
          IL_000f:  nop
          IL_0010:  ldc.i4.1
          IL_0011:  stloc.s    V_6
          IL_0013:  ldloc.s    V_6
          IL_0015:  stloc.3
          IL_0016:  leave.s    IL_0028

        }  
        catch [runtime]System.Object 
        {
          IL_0018:  castclass  [runtime]System.Exception
          IL_001d:  stloc.s    V_7
          IL_001f:  ldc.i4.1
          IL_0020:  stloc.s    V_4
          IL_0022:  ldloc.s    V_7
          IL_0024:  stloc.s    V_5
          IL_0026:  leave.s    IL_0028

        }  
        IL_0028:  ldloc.s    V_4
        IL_002a:  brfalse.s  IL_0042

        IL_002c:  ldloc.s    V_5
        IL_002e:  stloc.s    V_8
        IL_0030:  ldloc.s    V_8
        IL_0032:  stloc.s    V_9
        IL_0034:  ldstr      "with"
        IL_0039:  call       void [runtime]System.Console::WriteLine(string)
        IL_003e:  ldc.i4.1
        IL_003f:  nop
        IL_0040:  br.s       IL_0044

        IL_0042:  ldloc.3
        IL_0043:  nop
        IL_0044:  stloc.2
        IL_0045:  ldloc.2
        IL_0046:  brfalse.s  IL_0065

        IL_0048:  ldarg.0
        IL_0049:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_004e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
        IL_0053:  ldarg.0
        IL_0054:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
        IL_0059:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Result
        IL_005e:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetResult(!0)
        IL_0063:  leave.s    IL_0073

        IL_0065:  leave.s    IL_0073

      }  
      catch [runtime]System.Object 
      {
        IL_0067:  castclass  [runtime]System.Exception
        IL_006c:  stloc.s    V_10
        IL_006e:  ldloc.s    V_10
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_0073

      }  
      IL_0073:  ldloc.1
      IL_0074:  stloc.s    V_11
      IL_0076:  ldloc.s    V_11
      IL_0078:  brtrue.s   IL_007b

      IL_007a:  ret

      IL_007b:  ldarg.0
      IL_007c:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@4::Data
      IL_0081:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0086:  ldloc.s    V_11
      IL_0088:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_008d:  ret
    } 
                """
            ]))

module TaskWhileLoopGeneration =

    [<Test>]
    let ``check MoveNext of task while loop optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize+";"/debug:portable";"/tailcalls+" |],
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


    [<Test>]
    let ``check MoveNext of task while loop debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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
               class [runtime]System.Exception V_5,
               class [runtime]System.Exception V_6)
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
      IL_0060:  stloc.s    V_6
      IL_0062:  ldloc.s    V_6
      IL_0064:  brtrue.s   IL_0067

      IL_0066:  ret

      IL_0067:  ldarg.0
      IL_0068:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test/testTask@5::Data
      IL_006d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::MethodBuilder
      IL_0072:  ldloc.s    V_6
      IL_0074:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::SetException(class [netstandard]System.Exception)
      IL_0079:  ret
    } 
                """
            ]))
#endif


module TaskTypeInference =
    // This tests the compilation of a case that hits corner cases in SRTP constraint processing.
    // See https://github.com/dotnet/fsharp/issues/12188
    [<Test>]
    let ``check initially ambiguous SRTP task code ``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "/langversion:preview"; "/optimize-"; "/debug:portable";"/tailcalls-" |],
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
    [<Test>]
    let ``check generic task code ``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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

    [<Test>]
    let ``check generic task exact code``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |],
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
    .method assembly hidebysig instance class [runtime]System.Threading.Tasks.Task`1<!!A> 
            run<A>(class [runtime]System.Threading.Tasks.Task`1<!!A> computation) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype Test/cctor@7<!!A> V_0,
               valuetype Test/cctor@7<!!A>& V_1)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    valuetype Test/cctor@7<!!A>
      IL_0008:  ldloca.s   V_0
      IL_000a:  stloc.1
      IL_000b:  ldloc.1
      IL_000c:  ldarg.1
      IL_000d:  stfld      class [runtime]System.Threading.Tasks.Task`1<!0> valuetype Test/cctor@7<!!A>::computation
      IL_0012:  ldloc.1
      IL_0013:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!!A>::Data
      IL_0018:  call       valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!A>::Create()
      IL_001d:  stfld      valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_0022:  ldloc.1
      IL_0023:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!!A>::Data
      IL_0028:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_002d:  ldloc.1
      IL_002e:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!0>::Start<valuetype Test/cctor@7<!!0>>(!!0&)
      IL_0033:  ldloc.1
      IL_0034:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!!A>::Data
      IL_0039:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!!A>::MethodBuilder
      IL_003e:  call       instance class [netstandard]System.Threading.Tasks.Task`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!A>::get_Task()
      IL_0043:  ret
    } 
                """
            """
    .method public strict virtual instance void 
            MoveNext() cil managed
    {
      .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      
      .maxstack  5
      .locals init (int32 V_0,
               class [runtime]System.Exception V_1,
               bool V_2,
               class [runtime]System.Threading.Tasks.Task`1<!A> V_3,
               bool V_4,
               bool V_5,
               !A V_6,
               !A V_7,
               valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!A> V_8,
               class [runtime]System.Exception V_9,
               class [runtime]System.Exception V_10)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 valuetype Test/cctor@7<!A>::ResumptionPoint
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
      IL_0019:  ldnull
      IL_001a:  stloc.1
      .try
      {
        IL_001b:  ldloc.0
        IL_001c:  ldc.i4.1
        IL_001d:  sub
        IL_001e:  switch     ( 
                              IL_0029)
        IL_0027:  br.s       IL_002c

        IL_0029:  nop
        IL_002a:  br.s       IL_0055

        IL_002c:  nop
        IL_002d:  ldarg.0
        IL_002e:  ldfld      class [runtime]System.Threading.Tasks.Task`1<!0> valuetype Test/cctor@7<!A>::computation
        IL_0033:  stloc.3
        IL_0034:  ldarg.0
        IL_0035:  ldloc.3
        IL_0036:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<!A>::GetAwaiter()
        IL_003b:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0> valuetype Test/cctor@7<!A>::awaiter
        IL_0040:  ldc.i4.1
        IL_0041:  stloc.s    V_4
        IL_0043:  ldarg.0
        IL_0044:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0> valuetype Test/cctor@7<!A>::awaiter
        IL_0049:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!A>::get_IsCompleted()
        IL_004e:  brfalse.s  IL_0052

        IL_0050:  br.s       IL_006b

        IL_0052:  ldc.i4.0
        IL_0053:  brfalse.s  IL_0059

        IL_0055:  ldc.i4.1
        IL_0056:  nop
        IL_0057:  br.s       IL_0062

        IL_0059:  ldarg.0
        IL_005a:  ldc.i4.1
        IL_005b:  stfld      int32 valuetype Test/cctor@7<!A>::ResumptionPoint
        IL_0060:  ldc.i4.0
        IL_0061:  nop
        IL_0062:  stloc.s    V_5
        IL_0064:  ldloc.s    V_5
        IL_0066:  stloc.s    V_4
        IL_0068:  nop
        IL_0069:  br.s       IL_006c

        IL_006b:  nop
        IL_006c:  ldloc.s    V_4
        IL_006e:  brfalse.s  IL_0092

        IL_0070:  ldarg.0
        IL_0071:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0> valuetype Test/cctor@7<!A>::awaiter
        IL_0076:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!A>::GetResult()
        IL_007b:  stloc.s    V_6
        IL_007d:  ldloc.s    V_6
        IL_007f:  stloc.s    V_7
        IL_0081:  ldarg.0
        IL_0082:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!A>::Data
        IL_0087:  ldloc.s    V_7
        IL_0089:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::Result
        IL_008e:  ldc.i4.1
        IL_008f:  nop
        IL_0090:  br.s       IL_00ab

        IL_0092:  ldarg.0
        IL_0093:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!A>::Data
        IL_0098:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
        IL_009d:  ldarg.0
        IL_009e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0> valuetype Test/cctor@7<!A>::awaiter
        IL_00a3:  ldarg.0
        IL_00a4:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0>,valuetype Test/cctor@7<!0>>(!!0&,
                                                                                                                                                                                                                                                             !!1&)
        IL_00a9:  ldc.i4.0
        IL_00aa:  nop
        IL_00ab:  brfalse.s  IL_00c1

        IL_00ad:  ldarg.0
        IL_00ae:  ldloca.s   V_8
        IL_00b0:  initobj    valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!A>
        IL_00b6:  ldloc.s    V_8
        IL_00b8:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<!0> valuetype Test/cctor@7<!A>::awaiter
        IL_00bd:  ldc.i4.1
        IL_00be:  nop
        IL_00bf:  br.s       IL_00c3

        IL_00c1:  ldc.i4.0
        IL_00c2:  nop
        IL_00c3:  stloc.2
        IL_00c4:  ldloc.2
        IL_00c5:  brfalse.s  IL_00e4

        IL_00c7:  ldarg.0
        IL_00c8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!A>::Data
        IL_00cd:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
        IL_00d2:  ldarg.0
        IL_00d3:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!A>::Data
        IL_00d8:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::Result
        IL_00dd:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::SetResult(!0)
        IL_00e2:  leave.s    IL_00f2

        IL_00e4:  leave.s    IL_00f2

      }  
      catch [runtime]System.Object 
      {
        IL_00e6:  castclass  [runtime]System.Exception
        IL_00eb:  stloc.s    V_9
        IL_00ed:  ldloc.s    V_9
        IL_00ef:  stloc.1
        IL_00f0:  leave.s    IL_00f2

      }  
      IL_00f2:  ldloc.1
      IL_00f3:  stloc.s    V_10
      IL_00f5:  ldloc.s    V_10
      IL_00f7:  brtrue.s   IL_00fa

      IL_00f9:  ret

      IL_00fa:  ldarg.0
      IL_00fb:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!0> valuetype Test/cctor@7<!A>::Data
      IL_0100:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<!A>::MethodBuilder
      IL_0105:  ldloc.s    V_10
      IL_0107:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!A>::SetException(class [netstandard]System.Exception)
      IL_010c:  ret
    } 
            """
            ]))
#endif


#if NETCOREAPP
[<TestFixture>]
module ``Check stack traces`` = 
    [<Test>]
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
           failwith "expected emit of CompilerGeneratedAtrtibute to suppress message in .NET Core stack walking "
#endif


// Test that the SRTP Bind on a ValueTask is still generic in the bind type
[<TestFixture>]
module ``Check return attributes`` = 
    let incr (x:int) : [<Experimental("a")>] (int -> int) = (fun a -> a + x)

    // Putting a return atribute of any kind on f function should be respected
    // If the function is curried its inferred arity should be no more than 
    // the declared arguments (the F# rule that infers additional arguments 
    // should not kick in).
    [<Test>]
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

