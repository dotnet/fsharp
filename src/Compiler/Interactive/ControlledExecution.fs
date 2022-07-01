// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This wraps System.Runtime.CompilerServices.ControlledExecution
// This class enables scripting engines such as Fsi to abort threads safely in the coreclr
// This functionality will be introduced in .net 7.0.
// because we continue to dupport older coreclrs and the windows desktop framework through netstandard2.0
// we access the features using reflection

namespace FSharp.Compiler.Interactive

open System
open System.Reflection
open System.Threading

open Internal.Utilities.FSharpEnvironment

type ControlledExecution (thread:Thread) =

    static let ceType: Type option =
        Option.ofObj (Type.GetType("System.Runtime.CompilerServices.ControlledExecution, System.Private.CoreLib", false))

    static let threadType: Type option =
        Option.ofObj (typeof<Threading.Thread>)

    static let ceConstructor: ConstructorInfo option =
        match ceType with
        | None -> None
        | Some t -> Option.ofObj (t.GetConstructor([|typeof<Action>|]))

    static let ceRun: MethodInfo option =
        match ceType with
        | None -> None
        | Some t -> Option.ofObj (t.GetMethod("Run", [||]) )

    static let ceTryAbort: MethodInfo option =
        match ceType with
        | None -> None
        | Some t -> Option.ofObj (t.GetMethod("TryAbort", [|typeof<TimeSpan>|]))

    static let threadResetAbort: MethodInfo option =
        match isRunningOnCoreClr, threadType with
        | false, Some t -> Option.ofObj (t.GetMethod("ResetAbort", [||]))
        | _ -> None

    let newInstance (action: Action) =
        match ceConstructor with
        | None -> None
        | Some c -> Option.ofObj (c.Invoke([|action|]))

    let mutable instance = Unchecked.defaultof<obj option>

    member this.Run(action: Action) =
        let newinstance = newInstance(action)
        match newinstance, ceRun with
        | Some inst, Some ceRun ->
            instance <- newinstance
            ceRun.Invoke(inst, [||]) |> ignore
        | _ -> action.Invoke()

    member _.TryAbort(timeout: TimeSpan): bool =
        match isRunningOnCoreClr, instance, ceTryAbort with
        | _, Some instance, Some tryAbort -> tryAbort.Invoke(instance, [|timeout|]) :?> bool
        | false, _, _ -> thread.Abort(); true
        | true, _, _ -> true

    member _.ResetAbort() =
        match thread, threadResetAbort with
        | thread, Some threadResetAbort -> threadResetAbort.Invoke(thread, [||]) |> ignore
        | _ -> ()

    static member StripTargetInvocationException(exn: Exception) =
       match exn with
       | :? TargetInvocationException as e when not(isNull e.InnerException) ->
            ControlledExecution.StripTargetInvocationException(e.InnerException)
       | _ -> exn
