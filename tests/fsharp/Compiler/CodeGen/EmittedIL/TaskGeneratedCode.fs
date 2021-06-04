// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

open System
open System.Threading.Tasks

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
           class [runtime]System.Exception V_1,
           class [FSharp.Core]Microsoft.FSharp.Core.Unit V_2,
           bool V_3,
           valuetype Test/testTask@4& V_4,
           valuetype Test/testTask@4& V_5,
           class [runtime]System.Exception V_6)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
  IL_0006:  stloc.0
  .try
  {
    IL_0007:  ldarg.0
    IL_0008:  stloc.s    V_4
    IL_000a:  ldloc.s    V_4
    IL_000c:  stloc.s    V_5
    IL_000e:  ldloc.s    V_5
    IL_0010:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0015:  ldc.i4.1
    IL_0016:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_001b:  ldc.i4.1
    IL_001c:  stloc.3
    IL_001d:  ldloc.3
    IL_001e:  brfalse.s  IL_003f
    
    IL_0020:  ldarg.0
    IL_0021:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0026:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_002b:  ldarg.0
    IL_002c:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0031:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0036:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_003b:  ldnull
    IL_003c:  stloc.2
    IL_003d:  leave.s    IL_0051
    
    IL_003f:  ldnull
    IL_0040:  stloc.2
    IL_0041:  leave.s    IL_0051
    
  }  
  catch [runtime]System.Object 
  {
    IL_0043:  castclass  [runtime]System.Exception
    IL_0048:  stloc.s    V_6
    IL_004a:  ldloc.s    V_6
    IL_004c:  stloc.1
    IL_004d:  ldnull
    IL_004e:  stloc.2
    IL_004f:  leave.s    IL_0051
    
  }  
  IL_0051:  ldloc.2
  IL_0052:  pop
  IL_0053:  ldloc.1
  IL_0054:  stloc.s    V_6
  IL_0056:  ldloc.s    V_6
  IL_0058:  brfalse.s  IL_006d
    
  IL_005a:  ldarg.0
  IL_005b:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_0060:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0065:  ldloc.s    V_6
  IL_0067:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_006c:  ret
    
  IL_006d:  ret
} 
                """
            ])


    [<Test>]
    let ``check MoveNext of simple binding task``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [| "/langversion:preview" |]
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
           class [FSharp.Core]Microsoft.FSharp.Core.Unit V_2,
           bool V_3,
           valuetype Test/testTask@4& V_4,
           bool V_5,
           bool V_6,
           int32 V_7,
           int32 V_8,
           int32 V_9,
           valuetype Test/testTask@4& V_10,
           valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_11,
           class [runtime]System.Exception V_12)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      int32 Test/testTask@4::ResumptionPoint
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  ldc.i4.1
  IL_0009:  sub
  IL_000a:  switch     ( 
                        IL_0015)
  IL_0013:  br.s       IL_0015
    
  .try
  {
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  sub
    IL_0018:  switch     ( 
                          IL_0023)
    IL_0021:  br.s       IL_0025
    
    IL_0023:  br.s       IL_004e
    
    IL_0025:  ldarg.0
    IL_0026:  stloc.s    V_4
    IL_0028:  ldarg.0
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
    IL_002f:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
    IL_0034:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_0039:  ldc.i4.1
    IL_003a:  stloc.s    V_5
    IL_003c:  ldarg.0
    IL_003d:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_0042:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
    IL_0047:  brfalse.s  IL_004b
    
    IL_0049:  br.s       IL_005f
    
    IL_004b:  ldc.i4.0
    IL_004c:  brfalse.s  IL_0051
    
    IL_004e:  ldc.i4.1
    IL_004f:  br.s       IL_0059
    
    IL_0051:  ldarg.0
    IL_0052:  ldc.i4.1
    IL_0053:  stfld      int32 Test/testTask@4::ResumptionPoint
    IL_0058:  ldc.i4.0
    IL_0059:  stloc.s    V_6
    IL_005b:  ldloc.s    V_6
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.s    V_5
    IL_0061:  brfalse.s  IL_008e
    
    IL_0063:  ldarg.0
    IL_0064:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_0069:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
    IL_006e:  stloc.s    V_7
    IL_0070:  ldloc.s    V_7
    IL_0072:  stloc.s    V_8
    IL_0074:  ldloc.s    V_8
    IL_0076:  ldc.i4.1
    IL_0077:  add
    IL_0078:  stloc.s    V_9
    IL_007a:  ldarg.0
    IL_007b:  stloc.s    V_10
    IL_007d:  ldloc.s    V_10
    IL_007f:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0084:  ldloc.s    V_9
    IL_0086:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_008b:  ldc.i4.1
    IL_008c:  br.s       IL_00a6
    
    IL_008e:  ldarg.0
    IL_008f:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0094:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0099:  ldarg.0
    IL_009a:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_009f:  ldarg.0
    IL_00a0:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype Test/testTask@4>(!!0&,
                                                                                                                                                                                                                                                                !!1&)
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldarg.0
    IL_00a7:  ldloc.s    V_11
    IL_00a9:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_00ae:  stloc.3
    IL_00af:  ldloc.3
    IL_00b0:  brfalse.s  IL_00d1
    
    IL_00b2:  ldarg.0
    IL_00b3:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_00b8:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_00bd:  ldarg.0
    IL_00be:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_00c3:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_00c8:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_00cd:  ldnull
    IL_00ce:  stloc.2
    IL_00cf:  leave.s    IL_00e3
    
    IL_00d1:  ldnull
    IL_00d2:  stloc.2
    IL_00d3:  leave.s    IL_00e3
    
  }  
  catch [runtime]System.Object 
  {
    IL_00d5:  castclass  [runtime]System.Exception
    IL_00da:  stloc.s    V_12
    IL_00dc:  ldloc.s    V_12
    IL_00de:  stloc.1
    IL_00df:  ldnull
    IL_00e0:  stloc.2
    IL_00e1:  leave.s    IL_00e3
    
  }  
  IL_00e3:  ldloc.2
  IL_00e4:  pop
  IL_00e5:  ldloc.1
  IL_00e6:  stloc.s    V_12
  IL_00e8:  ldloc.s    V_12
  IL_00ea:  brfalse.s  IL_00ff
    
  IL_00ec:  ldarg.0
  IL_00ed:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_00f2:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_00f7:  ldloc.s    V_12
  IL_00f9:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_00fe:  ret
    
  IL_00ff:  ret
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

