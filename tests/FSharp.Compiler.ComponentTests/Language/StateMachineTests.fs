// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module StateMachineTests =

    let verify3511AndRun code = 
        Fsx code
        |> withNoOptimize
        |> compile
        |> shouldFail
        |> withWarningCode 3511
        |> ignore

        Fsx code
        |> withNoOptimize
        |> withOptions ["--nowarn:3511"]
        |> compileExeAndRun

    [<Fact>] // https://github.com/dotnet/fsharp/issues/13067
    let ``Local function with a flexible type``() = 
        """
task {
    let m1 f s = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    let m2 f (s: #seq<_>) = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    return 1
}
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed

    [<Fact>] // https://github.com/dotnet/fsharp/issues/14806
    let ``Explicit returns types + constraints on generics``() = 
        """
module Foo

open System.Threading.Tasks

let run2(): Task =
    task {
        return ()
    }

let run() =
    task {
        let a = null
        do! run2()
    }

run()
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed
        

    [<Fact>] // https://github.com/dotnet/fsharp/issues/14807
    let ``let _ = null``() = 
        """
module TestProject1

let bar() = task {
    let! _ = async { return [| 1 |] } |> Async.StartAsTask
    ()
}

let foo() = task {
    let _ = null
    do! bar()
}

foo()
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>] // https://github.com/dotnet/fsharp/issues/13386
    let ``SkipLocalsInit does not cause an exception``() =
        FSharp """
module TestProject1

[<System.Runtime.CompilerServices.SkipLocalsInit>]
let compute () =
    task {
        try
            do! System.Threading.Tasks.Task.Delay 10
        with e ->
            printfn "%s" (e.ToString())
    }

// multiple invocations to trigger tiered compilation
for i in 1 .. 100 do
    compute().Wait ()
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>] // https://github.com/dotnet/fsharp/issues/16068
    let ``Decision tree with 32+ binds with nested expression is not getting splitted and state machine is successfully statically compiles``() = 
        FSharp """
module Testing

let test () =
    task {
        if true then
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            ()
    }

[<EntryPoint>]
let main _ =
    test () |> ignore
    printfn "Hello, World!"
    0
"""
        |> ignoreWarnings
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``State machine defined as top level value is statically compiled`` () =
        Fsx """
let test = task { return 42 }
if test.Result <> 42 then failwith "expected 42"

task { printfn "Hello, World!"; return 42 }
"""
        |> runFsi
        |> shouldSucceed

    [<Fact>]
    let ``State machine defined as top level has a generated MoveNext method`` () =
        FSharp """
module TestStateMachine
let test = task { return 42 }
"""
        |> compile
        |> verifyIL [ """
.method public strict virtual instance void MoveNext() cil managed
{
  .override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
  
  .maxstack  4
  .locals init (int32 V_0,
           class [runtime]System.Exception V_1,
           bool V_2,
           class [runtime]System.Exception V_3)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      int32 TestStateMachine/test@3::ResumptionPoint
  IL_0006:  stloc.0
  .try
  {
    IL_0007:  ldarg.0
    IL_0008:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> TestStateMachine/test@3::Data
    IL_000d:  ldc.i4.s   42
    IL_000f:  stfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  brfalse.s  IL_0036

    IL_0019:  ldarg.0
    IL_001a:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> TestStateMachine/test@3::Data
    IL_001f:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
    IL_0024:  ldarg.0
    IL_0025:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> TestStateMachine/test@3::Data
    IL_002a:  ldfld      !0 valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::Result
    IL_002f:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetResult(!0)
    IL_0034:  leave.s    IL_0042

    IL_0036:  leave.s    IL_0042

  }  
  catch [runtime]System.Object 
  {
    IL_0038:  castclass  [runtime]System.Exception
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  stloc.1
    IL_0040:  leave.s    IL_0042

  }  
  IL_0042:  ldloc.1
  IL_0043:  stloc.3
  IL_0044:  ldloc.3
  IL_0045:  brtrue.s   IL_0048

  IL_0047:  ret

  IL_0048:  ldarg.0
  IL_0049:  ldflda     valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32> TestStateMachine/test@3::Data
  IL_004e:  ldflda     valuetype [runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<int32>::MethodBuilder
  IL_0053:  ldloc.3
  IL_0054:  call       instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::SetException(class [netstandard]System.Exception)
  IL_0059:  ret
} 
""" ]
