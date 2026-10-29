namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.Internal.VisualStudio.Services.UnifiedSettings
open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Utilities.UnifiedSettings

open Newtonsoft.Json.Linq

module internal UnifiedSettingsMigration =
    let editorGroups =
        [|
            "intelliSense",
            "IntelliSenseOptions",
            [|
                "ShowAfterCharIsTyped"
                "ShowAfterCharIsDeleted"
                "IncludeSymbolsFromUnopenedNamespacesOrModules"
                "EnterKeySetting"
                "GenerateBodyForOverriddenMethod"
            |]
            "quickInfo", "QuickInfoOptions", [| "DisplayLinks"; "UnderlineStyle"; "DescriptionWidth"; "ShowRemarks" |]
            "codeFixes",
            "CodeFixesOptions",
            [|
                "SimplifyName"
                "AlwaysPlaceOpensAtTopLevel"
                "UnusedOpens"
                "UnusedDeclarations"
                "SuggestNamesForErrors"
                "RemoveParens"
            |]
            "performance",
            "LanguageServicePerformanceOptions",
            [|
                "EnableInMemoryCrossProjectReferences"
                "TransparentCompilerCacheFactor"
                "AllowStaleCompletionResults"
                "TimeUntilStaleCompletion"
                "EnableParallelReferenceResolution"
                "EnableFastFindReferencesAndRename"
                "EnablePartialTypeChecking"
                "KeepAllBackgroundResolutions"
                "KeepAllBackgroundSymbolUses"
                "EnableBackgroundItemKeyStoreAndSemanticClassification"
            |]
            "advanced",
            "AdvancedOptions",
            [|
                "IsBlockStructureEnabled"
                "IsOutliningEnabled"
                "IsInlayTypeHintsEnabled"
                "IsInlayParameterNameHintsEnabled"
                "IsInlayReturnTypeHintsEnabled"
                "IsUseLiveBuffersEnabled"
                "UseTransparentCompiler"
                "TransparentCompilerSnapshotReuse"
                "SendAdditionalTelemetry"
                "SolutionBackgroundAnalysis"
            |]
            "formatting", "FormattingOptions", [| "FormatOnPaste" |]
        |]

    let interactiveSettings =
        [|
            "preferAnyCpuVersion", "FsiPreferAnyCPUVersion"
            "commandLineArgs", "FsiCommandLineArgs"
            "shadowCopy", "FsiShadowCopy"
            "useNetCore", "FsiUseNetCore"
            "debugMode", "FsiDebugMode"
            "preview", "FsiPreview"
        |]

    let editorKey name =
        $"TextEditor.FSharp.Microsoft.VisualStudio.FSharp.Editor.{name}"

    let interactiveKey name =
        $"Microsoft.VisualStudio.FSharp.Interactive.FsiPropertyPage.{name}"

    [<Literal>]
    let CompletionKey = "FSharp.UnifiedSettingsMigration.v1.Completed"

    let private enqueue (writer: ISettingsWriter) moniker (value: JToken) =
        let options = SettingWriteOptions.CustomizeUnconditionally

        let result =
            match value.Type with
            | JTokenType.Boolean -> writer.EnqueueChange(moniker, value.Value<bool>(), options)
            | JTokenType.Integer -> writer.EnqueueChange(moniker, value.Value<int>(), options)
            | JTokenType.String -> writer.EnqueueChange(moniker, value.Value<string>(), options)
            | _ -> invalidOp $"Invalid legacy value for {moniker}."

        if result.Outcome <> SettingChangeOutcome.PendingCommit then
            invalidOp $"Cannot import {moniker}: {result.Outcome}. {result.Message}"

    let importEditor (writer: ISettingsWriter) category (fields: string[]) (json: string) =
        let source =
            JObject.Parse(json, JsonLoadSettings(DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error))

        for field in fields do
            let moniker =
                $"languages.fsharp.{category}.{Char.ToLowerInvariant field[0]}{field.Substring(1)}"

            let value = source.GetValue(field, StringComparison.Ordinal)

            if not (isNull value) then
                if category = "quickInfo" && field = "DescriptionWidth" then
                    let widthMoniker = moniker + "InCharacters"

                    match value.Type with
                    | JTokenType.Null -> enqueue writer moniker (JValue(false))
                    | JTokenType.Object when value["Case"].Value<string>() = "Some" ->
                        let values = value["Fields"] :?> JArray

                        if
                            values.Count <> 1
                            || values[0].Type <> JTokenType.Integer
                            || values[0].Value<int>() < 20
                        then
                            invalidOp "Invalid legacy F# description width."

                        enqueue writer widthMoniker values[0]
                        enqueue writer moniker (JValue(true))
                    | _ -> invalidOp "Invalid legacy F# description width."
                else
                    let value =
                        if value.Type = JTokenType.Object then
                            value["Case"]
                        else
                            value

                    enqueue writer moniker value

    let runAsync
        (manager: Microsoft.VisualStudio.Utilities.UnifiedSettings.ISettingsManager)
        (tryRead: string -> obj voption)
        (saveCompleted: unit -> Task)
        (hasNativeCustomizations: unit -> bool)
        (logError: exn -> unit)
        (cancellationToken: CancellationToken)
        : Task =
        task {
            cancellationToken.ThrowIfCancellationRequested()

            try
                match tryRead CompletionKey with
                | ValueSome(:? bool as completed) when completed -> ()
                | _ when hasNativeCustomizations () -> do! saveCompleted ()
                | _ ->
                    let writer = manager.GetWriter("F# settings migration")
                    let mutable hasLegacyValues = false

                    for category, legacyType, fields in editorGroups do
                        match tryRead (editorKey legacyType) with
                        | ValueNone -> ()
                        | ValueSome(:? string as json) ->
                            hasLegacyValues <- true
                            importEditor writer category fields json
                        | _ -> invalidOp $"Invalid legacy F# settings record: {legacyType}."

                    for suffix, legacyName in interactiveSettings do
                        match tryRead (interactiveKey legacyName) with
                        | ValueNone -> ()
                        | ValueSome value ->
                            hasLegacyValues <- true
                            enqueue writer $"languages.fsharp.interactive.{suffix}" (JToken.FromObject value)

                    if hasLegacyValues then
                        cancellationToken.ThrowIfCancellationRequested()
                        let result = writer.RequestCommit(SR.UnifiedSettingsMigrationDescription())

                        match result.Outcome with
                        | SettingCommitOutcome.Success
                        | SettingCommitOutcome.NoChangesQueued -> do! saveCompleted ()
                        | _ -> invalidOp $"Cannot complete F# settings import: {result.Outcome}. {result.Message}"
            with
            | :? OperationCanceledException as error when cancellationToken.IsCancellationRequested ->
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw()
            | error -> logError error
        }

    #nowarn "57"

    let startAsync
        (manager: Microsoft.VisualStudio.Utilities.UnifiedSettings.ISettingsManager)
        (legacy: ISettingsManager5)
        (metadata: IUnifiedSettingsMetadata)
        (cancellationToken: CancellationToken)
        =
        task {
            let legacyBase = legacy :> Microsoft.VisualStudio.Settings.ISettingsManager
            let gate = new SemaphoreSlim(1, 1)

            let logError (error: exn) =
                ActivityLog.LogError("F# Unified Settings migration", error.ToString())

            let tryRead name =
                match legacyBase.TryGetValue<obj>(name) with
                | GetValueResult.Success, value -> ValueSome value
                | GetValueResult.Missing, _ -> ValueNone
                | result, _ -> invalidOp $"Cannot read legacy F# setting {name}: {result}."

            let hasNativeCustomizations () =
                legacyBase.NamesStartingWith("UnifiedSettings.languages.fsharp.").Length > 0
                || metadata.GetMonikersMatchingPattern("languages.fsharp.*")
                   |> Seq.exists (fun moniker -> metadata.GetProvenance(moniker).Source = SettingValueSource.UserCustomization)

            let run () : Task =
                task {
                    try
                        do! gate.WaitAsync(cancellationToken)

                        try
                            do! metadata.WaitForRefreshAsync(cancellationToken)

                            do!
                                runAsync
                                    manager
                                    tryRead
                                    (fun () -> legacyBase.SetValueAsync(CompletionKey, true, true))
                                    hasNativeCustomizations
                                    logError
                                    cancellationToken
                        finally
                            gate.Release() |> ignore
                    with
                    | :? OperationCanceledException when cancellationToken.IsCancellationRequested -> ()
                    | error -> logError error
                }

            let mutable handler = Unchecked.defaultof<EventHandler>

            handler <-
                EventHandler(fun _ _ ->
                    legacy.RoamingSyncComplete.RemoveHandler handler
                    ThreadHelper.JoinableTaskFactory.RunAsync(fun () -> run ()) |> ignore)

            legacy.RoamingSyncComplete.AddHandler handler
            do! run ()

            return
                { new IDisposable with
                    member _.Dispose() =
                        legacy.RoamingSyncComplete.RemoveHandler handler
                }
        }

    #warnon "57"
