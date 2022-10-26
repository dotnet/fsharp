// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This wraps System.Runtime.ControlledExecution
// This class enables scripting engines such as Fsi to abort threads safely in the coreclr
// This functionality will be introduced in .net 7.0.
// because we continue to support older coreclrs and the windows desktop framework through netstandard2.0
// we access the features using reflection

namespace FSharp.Compiler.Interactive

open System
open System.Reflection
open System.Threading

open Internal.Utilities.FSharpEnvironment

open Unchecked

type internal ControlledExecution () =

    let mutable cts: CancellationTokenSource voption = ValueNone
    let mutable thread: Thread voption = ValueNone

    static let ceType: Type option =
        Option.ofObj (Type.GetType("System.Runtime.ControlledExecution, System.Private.CoreLib", false))

    static let threadType: Type option =
        Option.ofObj (typeof<Threading.Thread>)

    static let ceRun: MethodInfo option =
        match ceType with
        | None -> None
        | Some t -> Option.ofObj (t.GetMethod("Run", BindingFlags.Static ||| BindingFlags.Public, defaultof<Binder>, [|typeof<System.Action>; typeof<System.Threading.CancellationToken>|], [||] ))

    static let threadResetAbort: MethodInfo option =
        match isRunningOnCoreClr, threadType with
        | false, Some t -> Option.ofObj (t.GetMethod("ResetAbort", [||]))
        | _ -> None

    member _.Run (action: Action) =
        match ceRun with
        | Some run ->
            cts <- ValueSome (new CancellationTokenSource())
            run.Invoke(null, [|action; cts.Value.Token|]) |> ignore
        | _ ->
            thread <- ValueSome (Thread.CurrentThread)
            action.Invoke()

    member _.TryAbort(): unit =
        match isRunningOnCoreClr, cts, thread with
        | true, ValueSome cts, _ ->  cts.Cancel()
        | false, _, ValueSome thread -> thread.Abort(); ()
        | _ -> ()

    member _.ResetAbort() =
        match thread, threadResetAbort with
        | thread, Some threadResetAbort -> threadResetAbort.Invoke(thread, [||]) |> ignore
        | _ -> ()

    static member StripTargetInvocationException(exn: Exception) =
       match exn with
       | :? TargetInvocationException as e when not(isNull e.InnerException) ->
            ControlledExecution.StripTargetInvocationException(e.InnerException)
       | _ -> exn