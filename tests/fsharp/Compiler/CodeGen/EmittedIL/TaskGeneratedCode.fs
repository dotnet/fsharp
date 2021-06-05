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
  IL_0013:  br.s       IL_0017
    
  IL_0015:  br.s       IL_0017
    
  .try
  {
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  switch     ( 
                          IL_0025)
    IL_0023:  br.s       IL_0027
    
    IL_0025:  br.s       IL_0050
    
    IL_0027:  ldarg.0
    IL_0028:  stloc.s    V_4
    IL_002a:  ldarg.0
    IL_002b:  ldarg.0
    IL_002c:  ldfld      class [runtime]System.Threading.Tasks.Task`1<int32> Test/testTask@4::t
    IL_0031:  callvirt   instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
    IL_0036:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_003b:  ldc.i4.1
    IL_003c:  stloc.s    V_5
    IL_003e:  ldarg.0
    IL_003f:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_0044:  call       instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
    IL_0049:  brfalse.s  IL_004d
    
    IL_004b:  br.s       IL_0063
    
    IL_004d:  ldc.i4.0
    IL_004e:  brfalse.s  IL_0053
    
    IL_0050:  ldc.i4.1
    IL_0051:  br.s       IL_005b
    
    IL_0053:  ldarg.0
    IL_0054:  ldc.i4.1
    IL_0055:  stfld      int32 Test/testTask@4::ResumptionPoint
    IL_005a:  ldc.i4.0
    IL_005b:  stloc.s    V_6
    IL_005d:  ldloc.s    V_6
    IL_005f:  stloc.s    V_5
    IL_0061:  br.s       IL_0063
    
    IL_0063:  ldloc.s    V_5
    IL_0065:  brfalse.s  IL_0092
    
    IL_0067:  ldarg.0
    IL_0068:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_006d:  call       instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
    IL_0072:  stloc.s    V_7
    IL_0074:  ldloc.s    V_7
    IL_0076:  stloc.s    V_8
    IL_0078:  ldloc.s    V_8
    IL_007a:  ldc.i4.1
    IL_007b:  add
    IL_007c:  stloc.s    V_9
    IL_007e:  ldarg.0
    IL_007f:  stloc.s    V_10
    IL_0081:  ldloc.s    V_10
    IL_0083:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0088:  ldloc.s    V_9
    IL_008a:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_008f:  ldc.i4.1
    IL_0090:  br.s       IL_00aa
    
    IL_0092:  ldarg.0
    IL_0093:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_0098:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_009d:  ldarg.0
    IL_009e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_00a3:  ldarg.0
    IL_00a4:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::AwaitUnsafeOnCompleted<valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype Test/testTask@4>(!!0&,
                                                                                                                                                                                                                                                                !!1&)
    IL_00a9:  ldc.i4.0
    IL_00aa:  brfalse.s  IL_00b7
    
    IL_00ac:  ldarg.0
    IL_00ad:  ldloc.s    V_11
    IL_00af:  stfld      valuetype [runtime]System.Runtime.CompilerServices.TaskAwaiter`1<int32> Test/testTask@4::awaiter
    IL_00b4:  ldnull
    IL_00b5:  br.s       IL_00b8
    
    IL_00b7:  ldnull
    IL_00b8:  stloc.3
    IL_00b9:  ldloc.3
    IL_00ba:  brfalse.s  IL_00db
    
    IL_00bc:  ldarg.0
    IL_00bd:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_00c2:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_00c7:  ldarg.0
    IL_00c8:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
    IL_00cd:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_00d2:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_00d7:  ldnull
    IL_00d8:  stloc.2
    IL_00d9:  leave.s    IL_00ed
    
    IL_00db:  ldnull
    IL_00dc:  stloc.2
    IL_00dd:  leave.s    IL_00ed
    
  }  
  catch [runtime]System.Object 
  {
    IL_00df:  castclass  [runtime]System.Exception
    IL_00e4:  stloc.s    V_12
    IL_00e6:  ldloc.s    V_12
    IL_00e8:  stloc.1
    IL_00e9:  ldnull
    IL_00ea:  stloc.2
    IL_00eb:  leave.s    IL_00ed
    
  }  
  IL_00ed:  ldloc.2
  IL_00ee:  pop
  IL_00ef:  ldloc.1
  IL_00f0:  stloc.s    V_12
  IL_00f2:  ldloc.s    V_12
  IL_00f4:  brfalse.s  IL_00f8
    
  IL_00f6:  br.s       IL_00f9
    
  IL_00f8:  ret
    
  IL_00f9:  ldarg.0
  IL_00fa:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> Test/testTask@4::Data
  IL_00ff:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0104:  ldloc.s    V_12
  IL_0106:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_010b:  ret
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

