// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

open System

// Check the exact code produced for tasks
[<TestFixture>]
module TaskGeneratedCode =

    // This tests the exact code generated for the MoveNext for a trivial task - we expect 'MoveNext' to be there
    // because state machine compialtion succeeds
    //
    // The code is not perfect - because the MoveNext is generated late - but the JIT does a good job on it.
    //
    // The try/catch for the task still exists even though there is no chance of an exception
    //
    // The crucial code for "return 1" is really just
    //   IL_000e:  ldc.i4.1
    //   IL_000f:  stfld      int32 Test/testTask@4::Result

    [<Test>]
    let ``check MoveNext of simple task``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [| "/langversion:preview" |]
            """
module Test

let testTask() = task { return 1 }
            """
            (fun verifier -> verifier.VerifyIL [
            """
.method public strict virtual instance void 
        MoveNext() cil managed
{
    .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
          
    .maxstack  4
    .locals init (int32 V_0,
            class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
            bool V_2,
            valuetype Test/testTask@4& V_3,
            valuetype Test/testTask@4& V_4,
            class [runtime]System.Exception V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
    IL_0006:  stloc.0
    .try
    {
    IL_0007:  ldarg.0
    IL_0008:  stloc.3
    IL_0009:  ldloc.3
    IL_000a:  stloc.s    V_4
    IL_000c:  ldloc.s    V_4
    IL_000e:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0013:  ldc.i4.1
    IL_0014:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0019:  ldc.i4.1
    IL_001a:  stloc.2
    IL_001b:  ldloc.2
    IL_001c:  brfalse.s  IL_003d
    
    IL_001e:  ldarg.0
    IL_001f:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0024:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0029:  ldarg.0
    IL_002a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_002f:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0034:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_0039:  ldnull
    IL_003a:  stloc.1
    IL_003b:  leave.s    IL_005e
    
    IL_003d:  ldnull
    IL_003e:  stloc.1
    IL_003f:  leave.s    IL_005e
    
    }  
    catch [runtime]System.Object 
    {
    IL_0041:  castclass  [runtime]System.Exception
    IL_0046:  stloc.s    V_5
    IL_0048:  ldarg.0
    IL_0049:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_004e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0053:  ldloc.s    V_5
    IL_0055:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
    IL_005a:  ldnull
    IL_005b:  stloc.1
    IL_005c:  leave.s    IL_005e
    
    }  
    IL_005e:  ldloc.1
    IL_005f:  pop
    IL_0060:  ret
} 
                """
            ])

