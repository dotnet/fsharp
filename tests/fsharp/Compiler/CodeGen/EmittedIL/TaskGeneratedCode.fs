// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

open System
open System.Threading.Tasks

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
    let ``check MoveNext of simple task optimized``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [| "/langversion:preview";"/optimize+";"/debug:portable";"/tailcalls+" |]
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
  IL_0044:  brfalse.s  IL_0048
    
  IL_0046:  br.s       IL_0049
    
  IL_0048:  ret
    
  IL_0049:  ldarg.0
  IL_004a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_004f:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0054:  ldloc.3
  IL_0055:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_005a:  ret
} 
                """
            ])


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
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [| "/langversion:preview";"/optimize-";"/debug:portable";"/tailcalls-" |]
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
  IL_004a:  brfalse.s  IL_004e
    
  IL_004c:  br.s       IL_004f
    
  IL_004e:  ret
    
  IL_004f:  ldarg.0
  IL_0050:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_0055:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_005a:  ldloc.s    V_5
  IL_005c:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_0061:  ret
} 
                """
            ])

    [<Test>]
    let ``check MoveNext of simple binding task debug``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [| "/langversion:preview"; "/debug:portable"; "/optimize-"; "/tailcalls-" |]
            """
module Test
open System.Threading.Tasks
let testTask(t: Task<int>) = task { let! res = t in return res+1 }
            """
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
  IL_00f7:  brfalse.s  IL_00fb
    
  IL_00f9:  br.s       IL_00fc
    
  IL_00fb:  ret
    
  IL_00fc:  ldarg.0
  IL_00fd:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_0102:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0107:  ldloc.s    V_12
  IL_0109:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_010e:  ret
} 
                """
            ])

#if NETCOREAPP
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
    
// Test that the SRTP Bind on a ValueTask is still generic in the bind type
module ``SRTP Bind on a ValueTask is still generic in the bind type`` =
    let FindAsync() = ValueTask<'T>(Unchecked.defaultof<'T>)
    let TryFindAsync() : Task<'T> = task {
            let! r = FindAsync()
            return r
        }
    let t1 : Task<int voption> = TryFindAsync() // test TryFindAsync is generic
    let t2 : Task<string voption> = TryFindAsync() // test TryFindAsync is generic
    

// 
module ``SRTP ReturnFrom on a ValueTask is still generic in the bind type`` =
    let FindAsync() = ValueTask<'T>(Unchecked.defaultof<'T>)
    let TryFindAsync() : Task<'T> = task {
            return! FindAsync()
        }
    
    let t1 : Task<int voption> = TryFindAsync() // test TryFindAsync is generic
    let t2 : Task<string voption> = TryFindAsync() // test TryFindAsync is generic

