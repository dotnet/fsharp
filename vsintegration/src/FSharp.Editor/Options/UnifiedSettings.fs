namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Threading.Tasks

open Microsoft.VisualStudio.FSharp.Interactive
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Utilities.UnifiedSettings

module internal UnifiedSettings =
    [<Literal>]
    let ServiceId = "9c45f9ae-6c72-4b44-bf59-65d102a33d11"

    let read<'T> (reader: ISettingsReader) suffix =
        reader.GetValueOrThrow<'T>($"languages.fsharp.{suffix}")

    let readInteractive reader : Session.InteractiveOptions =
        {
            PreferAnyCpuVersion = read<bool> reader "interactive.preferAnyCpuVersion"
            CommandLineArgs = read<string> reader "interactive.commandLineArgs"
            ShadowCopy = read<bool> reader "interactive.shadowCopy"
            UseNetCore = read<bool> reader "interactive.useNetCore"
            DebugMode = read<bool> reader "interactive.debugMode"
            Preview = read<bool> reader "interactive.preview"
        }

    let subscribeToChanges (reader: ISettingsReader) (pattern: string) refresh =
        let gate = obj ()
        let refreshSerialized () = lock gate refresh

        let subscription =
            reader.SubscribeToChanges(
                (fun _ ->
                    try
                        refreshSerialized ()
                    with error ->
                        ActivityLog.LogError("F# Unified Settings", error.ToString())),
                pattern
            )

        try
            refreshSerialized ()
            subscription
        with _ ->
            subscription.Dispose()
            reraise ()

    let subscribeToInteractive (reader: ISettingsReader) =
        subscribeToChanges reader "languages.fsharp.interactive.*" (fun () -> Session.SessionsProperties.Update(readInteractive reader))

[<Guid(UnifiedSettings.ServiceId)>]
type internal FSharpSettingsProvider(readCompiler: unit -> bool, writeCompiler: bool -> unit, logError: exn -> unit) =

    let changed =
        Event<EventHandler<ExternalSettingsChangedEventArgs>, ExternalSettingsChangedEventArgs>()

    let enumChanged =
        Event<EventHandler<EnumSettingChoicesChangedEventArgs>, EnumSettingChoicesChangedEventArgs>()

    let messageChanged =
        Event<EventHandler<DynamicMessageTextChangedEventArgs>, DynamicMessageTextChangedEventArgs>()

    let errorResolved = Event<EventHandler, EventArgs>()

    let isSettingError (error: exn) =
        match error with
        | :? ArgumentException
        | :? InvalidOperationException
        | :? System.IO.IOException
        | :? UnauthorizedAccessException
        | :? System.Security.SecurityException -> true
        | _ -> false

    let reportError (error: exn) =
        logError error
        SR.UnifiedSettingsOperationFailed()

    interface ICachingExternalSettingsProvider with
        [<CLIEvent>]
        member _.SettingValuesChanged = changed.Publish

        [<CLIEvent>]
        member _.EnumSettingChoicesChanged = enumChanged.Publish

        [<CLIEvent>]
        member _.DynamicMessageTextChanged = messageChanged.Publish

        [<CLIEvent>]
        member _.ErrorConditionResolved = errorResolved.Publish

        member _.GetValueAsync<'T>(name, cancellationToken) =
            task {
                cancellationToken.ThrowIfCancellationRequested()

                try
                    if name <> "UseNetSdkCompiler" then
                        invalidArg (nameof name) "Unknown F# compiler setting."

                    return ExternalSettingOperationResult.ConvertSuccessResult<'T>(readCompiler ())
                with error when isSettingError error ->
                    return
                        ExternalSettingOperationResult<'T>.Failure(reportError error, ExternalSettingsErrorScope.SingleSettingOnly, false)
                        :> ExternalSettingOperationResult<'T>
            }

        member this.SetValueAsync<'T>(name, value: 'T, cancellationToken) =
            task {
                cancellationToken.ThrowIfCancellationRequested()

                try
                    if name <> "UseNetSdkCompiler" then
                        invalidArg (nameof name) "Unknown F# compiler setting."

                    if typeof<'T> <> typeof<bool> then
                        invalidArg (nameof value) "Expected a Boolean compiler setting."

                    writeCompiler (unbox<bool> value)
                    changed.Trigger(this, ExternalSettingsChangedEventArgs.SomeOrAll)
                    return ExternalSettingOperationResult.Success.Instance :> ExternalSettingOperationResult
                with error when isSettingError error ->
                    return
                        ExternalSettingOperationResult.Failure(reportError error, ExternalSettingsErrorScope.SingleSettingOnly, false)
                        :> ExternalSettingOperationResult
            }

        member _.GetEnumChoicesAsync(_, _) =
            Task.FromException<ExternalSettingOperationResult<IReadOnlyList<EnumChoice>>>(
                InvalidOperationException("F# enum choices are defined in the settings manifest.")
            )

        member _.GetMessageTextAsync(_, _) =
            Task.FromException<string>(InvalidOperationException("F# settings have no dynamic messages."))

        member _.OpenBackingStoreAsync _ =
            Task.FromException(InvalidOperationException("F# settings do not have an editable backing file."))

        member _.CommitPendingChangesAsync cancellationToken =
            cancellationToken.ThrowIfCancellationRequested()
            ExternalSettingOperationResult.SuccessResultTask()

        member this.RefreshCacheAsync cancellationToken =
            cancellationToken.ThrowIfCancellationRequested()
            changed.Trigger(this, ExternalSettingsChangedEventArgs.SomeOrAll)
            ExternalSettingOperationResult.SuccessResultTask()
