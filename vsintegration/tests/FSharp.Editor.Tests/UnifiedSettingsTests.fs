namespace FSharp.Editor.Tests

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open System.Resources
open System.Threading
open System.Threading.Tasks

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Interactive
open Microsoft.VisualStudio.Utilities.UnifiedSettings

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Xunit

module UnifiedSettingsTests =
    let private editorRecords =
        [|
            "intelliSense", box IntelliSenseOptions.Default
            "quickInfo", box QuickInfoOptions.Default
            "codeFixes", box CodeFixesOptions.Default
            "performance", box LanguageServicePerformanceOptions.Default
            "advanced", box AdvancedOptions.Default
            "formatting", box FormattingOptions.Default
        |]

    let private manifest () =
        JObject.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "fsharpSettings.registration.json")),
            JsonLoadSettings(DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error)
        )

    let private registrations () =
        (manifest ()).["properties"] :?> JObject

    let private editorSettings () =
        [|
            for category, defaults in editorRecords do
                for property in defaults.GetType().GetProperties(BindingFlags.Instance ||| BindingFlags.Public) do
                    let name = property.Name
                    yield $"languages.fsharp.{category}.{Char.ToLowerInvariant name[0]}{name.Substring(1)}", defaults, property
        |]

    type private NativeHost(?onRead: string -> obj -> unit) =
        let definitions = registrations ()
        let defaults = Dictionary<string, obj>(StringComparer.Ordinal)
        let values = Dictionary<string, obj>(StringComparer.Ordinal)
        let subscriptions = ResizeArray<Action<SettingsUpdate> * string[]>()
        let onRead = defaultArg onRead (fun _ _ -> ())
        let mutable reads = 0
        let mutable commits = 0
        let mutable commitOutcome = SettingCommitOutcome.Success
        let mutable rejectChanges = false

        do
            for property in definitions.Properties() do
                let setting = property.Value

                match setting["type"].Value<string>() with
                | "boolean" -> defaults.Add(property.Name, box (setting["default"].Value<bool>()))
                | "integer" -> defaults.Add(property.Name, box (setting["default"].Value<int>()))
                | "string" -> defaults.Add(property.Name, box (setting["default"].Value<string>()))
                | _ -> ()

        let get name =
            reads <- reads + 1

            let value =
                match values.TryGetValue name with
                | true, value -> value
                | _ -> defaults[name]

            onRead name value
            value

        let notify (names: seq<string>) =
            let update =
                SettingsUpdate(Guid.Empty, HashSet<string>(names, StringComparer.Ordinal))

            for callback, patterns in subscriptions.ToArray() do
                if
                    patterns
                    |> Array.exists (fun pattern ->
                        update.ChangedSettingMonikers
                        |> Seq.exists (fun name ->
                            if pattern.EndsWith("*", StringComparison.Ordinal) then
                                name.StartsWith(pattern.TrimEnd('*'), StringComparison.Ordinal)
                            else
                                name = pattern))
                then
                    callback.Invoke update

        let writer () =
            let pending = Dictionary<string, obj>(StringComparer.Ordinal)

            let enqueue name (value: obj) =
                let setting = definitions[name]
                let token = JToken.FromObject value

                let validType =
                    match setting["type"].Value<string>() with
                    | "boolean" -> value :? bool
                    | "integer" -> value :? int
                    | "string" -> value :? string
                    | _ -> false

                let valid =
                    validType
                    && (isNull setting["minimum"]
                        || token.Value<int>() >= setting["minimum"].Value<int>())
                    && (isNull setting["maximum"]
                        || token.Value<int>() <= setting["maximum"].Value<int>())
                    && (isNull setting["enum"]
                        || setting["enum"].Values<string>() |> Seq.contains (string value))

                if rejectChanges then
                    SettingChangeResult(SettingChangeOutcome.Denied, false, "Denied for test")
                elif not valid then
                    SettingChangeResult(SettingChangeOutcome.InvalidValue, false, "Invalid for test")
                else
                    pending[name] <- value
                    SettingChangeResult(SettingChangeOutcome.PendingCommit, true, null)

            let commit () =
                if pending.Count = 0 then
                    SettingCommitResult(SettingCommitOutcome.NoChangesQueued, null)
                elif commitOutcome <> SettingCommitOutcome.Success then
                    SettingCommitResult(commitOutcome, "Commit failed for test")
                else
                    for KeyValue(name, value) in pending do
                        values[name] <- value

                    commits <- commits + 1
                    notify pending.Keys
                    pending.Clear()
                    SettingCommitResult(SettingCommitOutcome.Success, null)

            { new ISettingsWriter with
                member _.SubscribeToChanges(callback, patterns) =
                    let subscription = callback, patterns
                    subscriptions.Add subscription

                    { new IDisposable with
                        member _.Dispose() =
                            subscriptions.Remove subscription |> ignore
                    }

                member _.GetValueOrThrow<'T>(name) = unbox<'T> (get name)
                member _.GetValueOrThrow(name, _) = get name

                member _.GetValue<'T>(name, _) =
                    SettingRetrieval<'T>(SettingRetrievalOutcome.Success, null, unbox<'T> (get name))

                member _.GetValue(name, _, _) =
                    SettingRetrieval<obj>(SettingRetrievalOutcome.Success, null, get name)

                member _.GetArray<'T>(_, _) : SettingRetrieval<IReadOnlyList<'T>> = invalidOp "Unexpected array read."
                member _.GetArrayOrThrow<'T>(_) : IReadOnlyList<'T> = invalidOp "Unexpected array read."
                member _.EnqueueChange<'T>(name, value: 'T) = enqueue name (box value)
                member _.EnqueueChange<'T>(name, value: 'T, _) = enqueue name (box value)
                member _.EnqueueArrayChange<'T>(_, _: IReadOnlyList<'T>) = invalidOp "Unexpected array write."
                member _.RequestCommit _ = commit ()
                member _.Commit _ = commit ()
            }

        let manager =
            { new ISettingsManager with
                member _.GetReader() = writer () :> ISettingsReader
                member _.GetWriter _ = writer ()
                member _.GetWriter(_, _) = writer ()
            }

        member _.Manager = manager
        member _.Reader = manager.GetReader()
        member _.Values = values
        member _.ReadCount = reads
        member _.CommitCount = commits
        member _.SubscriptionCount = subscriptions.Count

        member _.CommitOutcome
            with get () = commitOutcome
            and set value = commitOutcome <- value

        member _.RejectChanges
            with get () = rejectChanges
            and set value = rejectChanges <- value

        member _.Set(name, value: obj) =
            values[name] <- value
            notify [| name |]

        member _.Reset name =
            values.Remove name |> ignore
            notify [| name |]

    type private ImportFixture() =
        let native = NativeHost()
        let legacy = Dictionary<string, obj>(StringComparer.Ordinal)
        let errors = ResizeArray<exn>()
        let checkpoints = ResizeArray<string>()
        let mutable failCheckpoint = false
        member _.Native = native
        member _.Legacy = legacy
        member _.Errors = errors
        member _.Checkpoints = checkpoints

        member _.FailCheckpoint
            with get () = failCheckpoint
            and set value = failCheckpoint <- value

        member _.Run(?cancellationToken) =
            UnifiedSettingsMigration.runAsync
                native.Manager
                (fun name ->
                    match legacy.TryGetValue name with
                    | true, value -> ValueSome value
                    | _ -> ValueNone)
                (fun name ->
                    if failCheckpoint then
                        Task.FromException(IOException("Checkpoint failed for test"))
                    else
                        legacy[name] <- true
                        checkpoints.Add name
                        Task.CompletedTask)
                (fun name -> native.Values.ContainsKey name || legacy.ContainsKey("UnifiedSettings." + name))
                errors.Add
                (defaultArg cancellationToken CancellationToken.None)

    [<Fact>]
    let ``All 43 native settings roam individually and only Compiler is external`` () =
        let settings = registrations ()
        Assert.Equal(44, settings.Count)

        let native =
            settings.Properties()
            |> Seq.filter (fun p ->
                let setting = p.Value
                setting["type"].Value<string>() <> "external")
            |> Seq.toArray

        Assert.Equal(43, native.Length)

        for property in native do
            let setting = property.Value
            Assert.True(setting.SelectToken("migration.roamingOnly").Value<bool>(), property.Name)
            Assert.False(setting["transientScopesSupported"].Value<bool>())
            Assert.Equal(1, (setting["migration"] :?> JObject).Count)

        let compiler = settings["languages.fsharp.compiler.settings"]
        Assert.Equal("external", compiler["type"].Value<string>())
        Assert.Null(compiler["migration"])
        Assert.Equal(UnifiedSettings.ServiceId, compiler.SelectToken("callback.serviceId").Value<string>())
        Assert.Equal("UseNetSdkCompiler", (Assert.Single((compiler["properties"] :?> JObject).Properties())).Name)

        for category in ((manifest ()).["categories"] :?> JObject).Properties() do
            let category = category.Value
            Assert.Null(category["legacyOptionPageId"])

    [<Fact>]
    let ``Native defaults match the typed editor defaults`` () =
        let host = NativeHost()
        use options = new EditorOptions(host.Reader)
        Assert.Equal(IntelliSenseOptions.Default, options.IntelliSense)
        Assert.Equal(QuickInfoOptions.Default, options.QuickInfo)
        Assert.Equal(CodeFixesOptions.Default, options.CodeFixes)
        Assert.Equal(LanguageServicePerformanceOptions.Default, options.LanguageServicePerformance)
        Assert.Equal(AdvancedOptions.Default, options.Advanced)
        Assert.Equal(FormattingOptions.Default, options.Formatting)
        Assert.Equal(1, host.SubscriptionCount)
        let reads = host.ReadCount
        options.QuickInfo |> ignore
        options.IntelliSense |> ignore
        Assert.Equal(reads, host.ReadCount)

    [<Fact>]
    let ``Native changes and resets update cached records without legacy storage`` () =
        let host = NativeHost()
        use options = new EditorOptions(host.Reader)
        host.Set("languages.fsharp.codeFixes.simplifyName", true)
        host.Set("languages.fsharp.intelliSense.enterKeySetting", "AlwaysNewline")
        host.Set("languages.fsharp.advanced.solutionBackgroundAnalysis", true)
        host.Set("languages.fsharp.performance.transparentCompilerCacheFactor", 123)
        host.Set("languages.fsharp.formatting.formatOnPaste", true)
        Assert.True(options.CodeFixes.SimplifyName)
        Assert.Equal(AlwaysNewline, options.IntelliSense.EnterKeySetting)
        Assert.True(options.Advanced.SolutionBackgroundAnalysis)
        Assert.Equal(123, options.LanguageServicePerformance.TransparentCompilerCacheFactor)
        Assert.True(options.Formatting.FormatOnPaste)
        host.Reset "languages.fsharp.codeFixes.simplifyName"
        Assert.False(options.CodeFixes.SimplifyName)
        (options :> IDisposable).Dispose()
        Assert.Equal(0, host.SubscriptionCount)
        host.Set("languages.fsharp.formatting.formatOnPaste", false)
        Assert.True(options.Formatting.FormatOnPaste)

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Refreshes serialize initialization and concurrent notifications`` (interactive: bool, initializing: bool) =
        let timeout = TimeSpan.FromSeconds 10.
        use firstRead = new ManualResetEventSlim()
        use releaseRead = new ManualResetEventSlim()
        let mutable blockNextRead = 0

        let moniker =
            if interactive then
                "languages.fsharp.interactive.preferAnyCpuVersion"
            else
                "languages.fsharp.intelliSense.showAfterCharIsTyped"

        let host =
            NativeHost(fun name value ->
                if name = moniker && Interlocked.CompareExchange(&blockNextRead, 0, 1) = 1 then
                    Assert.Equal(box true, value)
                    firstRead.Set()
                    Assert.True(releaseRead.Wait timeout, "The first refresh was not released."))

        let original = Session.SessionsProperties.Current
        let mutable subscription = None

        let mutable currentValue =
            fun () -> Session.SessionsProperties.Current.PreferAnyCpuVersion

        let initialize () =
            if interactive then
                UnifiedSettings.subscribeToInteractive host.Reader
            else
                let options = new EditorOptions(host.Reader)
                currentValue <- fun () -> options.IntelliSense.ShowAfterCharIsTyped
                options :> IDisposable

        try
            if not initializing then
                subscription <- Some(initialize ())

            Volatile.Write(&blockNextRead, 1)

            let first =
                Task.Run(
                    Action(fun () ->
                        if initializing then
                            subscription <- Some(initialize ())
                        else
                            host.Set(moniker, true))
                )

            let mutable second = Task.CompletedTask

            try
                Assert.True(firstRead.Wait timeout, "The first refresh did not read the old value.")
                let callbackThread = TaskCompletionSource<Thread>()

                second <-
                    Task.Factory.StartNew(
                        Action(fun () ->
                            callbackThread.SetResult Thread.CurrentThread
                            host.Set(moniker, false)),
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    )

                Assert.True(callbackThread.Task.Wait timeout, "The second refresh did not start.")

                Assert.True(
                    SpinWait.SpinUntil(
                        (fun () ->
                            second.IsCompleted
                            || callbackThread.Task.Result.ThreadState.HasFlag ThreadState.WaitSleepJoin),
                        timeout
                    ),
                    "The second refresh neither completed nor waited for the first refresh."
                )
            finally
                releaseRead.Set()
                Assert.True(Task.WaitAll([| first; second |], timeout), "The refreshes did not complete.")

            Assert.False(currentValue (), "The older refresh overwrote the latest setting value.")
        finally
            subscription |> Option.iter _.Dispose()
            Session.SessionsProperties.Update original

    [<Fact>]
    let ``Headless options remain isolated and support typed overrides`` () =
        use first = new EditorOptions()
        use second = new EditorOptions()

        first.With
            { QuickInfoOptions.Default with
                DescriptionWidth = Some 80
            }

        Assert.Equal(Some 80, first.QuickInfo.DescriptionWidth)
        Assert.Equal(None, second.QuickInfo.DescriptionWidth)

    [<Fact>]
    let ``Every editor field reads changes from its own native setting`` () =
        let host = NativeHost()
        use options = new EditorOptions(host.Reader)

        for name, defaults, property in editorSettings () do
            let value = property.GetValue defaults

            if property.PropertyType = typeof<bool> then
                host.Set(name, not (unbox<bool> value))
            elif property.PropertyType = typeof<int> then
                host.Set(name, unbox<int> value + 1)
            elif property.PropertyType = typeof<EnterKeySetting> then
                host.Set(name, "NewlineOnCompleteWord")
            elif property.PropertyType = typeof<QuickInfoUnderlineStyle> then
                host.Set(name, "Dash")
            else
                host.Set(name + "InCharacters", 91)
                host.Set(name, true)

        let actual =
            dict
                [
                    "intelliSense", box options.IntelliSense
                    "quickInfo", box options.QuickInfo
                    "codeFixes", box options.CodeFixes
                    "performance", box options.LanguageServicePerformance
                    "advanced", box options.Advanced
                    "formatting", box options.Formatting
                ]

        for category, defaults in editorRecords do
            for property in defaults.GetType().GetProperties(BindingFlags.Instance ||| BindingFlags.Public) do
                let name =
                    $"languages.fsharp.{category}.{Char.ToLowerInvariant property.Name[0]}{property.Name.Substring(1)}"

                let value = property.GetValue actual[category]

                if property.PropertyType = typeof<int option> then
                    Assert.Equal(box (Some 91), value)
                elif property.PropertyType = typeof<bool> || property.PropertyType = typeof<int> then
                    Assert.Equal(host.Values[name], value)
                else
                    Assert.Equal(host.Values[name], box (string value))

    [<Fact>]
    let ``Description width is a Boolean with a dependent integer and remembers the integer`` () =
        let settings = registrations ()
        let width = settings["languages.fsharp.quickInfo.descriptionWidth"]
        let characters = settings["languages.fsharp.quickInfo.descriptionWidthInCharacters"]
        Assert.Equal("boolean", width["type"].Value<string>())
        Assert.False(width["default"].Value<bool>())
        Assert.Equal("integer", characters["type"].Value<string>())
        Assert.Equal(120, characters["default"].Value<int>())
        Assert.Equal(20, characters["minimum"].Value<int>())
        Assert.Equal(Int32.MaxValue, characters["maximum"].Value<int>())
        Assert.Equal("${config:languages.fsharp.quickInfo.descriptionWidth} == 'true'", characters["enableWhen"].Value<string>())
        let host = NativeHost()
        use options = new EditorOptions(host.Reader)
        host.Set("languages.fsharp.quickInfo.descriptionWidthInCharacters", 140)
        Assert.Equal(None, options.QuickInfo.DescriptionWidth)
        host.Set("languages.fsharp.quickInfo.descriptionWidth", true)
        Assert.Equal(Some 140, options.QuickInfo.DescriptionWidth)
        host.Set("languages.fsharp.quickInfo.descriptionWidth", false)
        Assert.Equal(None, options.QuickInfo.DescriptionWidth)
        host.Set("languages.fsharp.quickInfo.descriptionWidth", true)
        Assert.Equal(Some 140, options.QuickInfo.DescriptionWidth)

    [<Fact>]
    let ``Interactive configuration reads all six native settings`` () =
        let host = NativeHost()
        let defaults = UnifiedSettings.readInteractive host.Reader
        Assert.True(defaults.PreferAnyCpuVersion)
        Assert.True(defaults.UseNetCore)
        Assert.Equal("--optimize", defaults.CommandLineArgs)
        Assert.True(defaults.ShadowCopy)
        Assert.False(defaults.DebugMode)
        Assert.False(defaults.Preview)
        host.Set("languages.fsharp.interactive.preferAnyCpuVersion", false)
        host.Set("languages.fsharp.interactive.useNetCore", false)
        host.Set("languages.fsharp.interactive.commandLineArgs", "--warn:5")
        host.Set("languages.fsharp.interactive.shadowCopy", false)
        host.Set("languages.fsharp.interactive.debugMode", true)
        host.Set("languages.fsharp.interactive.preview", true)
        let changed = UnifiedSettings.readInteractive host.Reader
        Assert.False(changed.PreferAnyCpuVersion)
        Assert.False(changed.UseNetCore)
        Assert.Equal("--warn:5", changed.CommandLineArgs)
        Assert.False(changed.ShadowCopy)
        Assert.True(changed.DebugMode)
        Assert.True(changed.Preview)

    [<Fact>]
    let ``Fresh profiles remain uncustomized and can import data arriving after startup`` () =
        task {
            let fixture = ImportFixture()
            use options = new EditorOptions(fixture.Native.Reader)
            do! fixture.Run()
            Assert.Empty fixture.Native.Values
            Assert.Empty fixture.Checkpoints
            fixture.Legacy[UnifiedSettingsMigration.editorKey "FormattingOptions"] <- "{\"FormatOnPaste\":true}"
            do! fixture.Run()
            Assert.Equal(box true, fixture.Native.Values["languages.fsharp.formatting.formatOnPaste"])
            Assert.True(options.Formatting.FormatOnPaste)
            Assert.Single(fixture.Checkpoints) |> ignore
            Assert.Empty fixture.Errors
        }

    [<Fact>]
    let ``Interactive sessions receive native changes and release their subscription`` () =
        let host = NativeHost()
        let original = Session.SessionsProperties.Current

        try
            use subscription = UnifiedSettings.subscribeToInteractive host.Reader
            host.Set("languages.fsharp.interactive.preferAnyCpuVersion", false)
            host.Set("languages.fsharp.interactive.useNetCore", false)
            host.Set("languages.fsharp.interactive.commandLineArgs", "--warn:5")
            host.Set("languages.fsharp.interactive.shadowCopy", false)
            host.Set("languages.fsharp.interactive.debugMode", true)
            host.Set("languages.fsharp.interactive.preview", true)
            Assert.False(Session.SessionsProperties.useAnyCpuVersion)
            Assert.False(Session.SessionsProperties.fsiUseNetCore)
            Assert.Equal("--warn:5", Session.SessionsProperties.fsiArgs)
            Assert.False(Session.SessionsProperties.fsiShadowCopy)
            Assert.True(Session.SessionsProperties.fsiDebugMode)
            Assert.True(Session.SessionsProperties.fsiPreview)
            subscription.Dispose()
            Assert.Equal(0, host.SubscriptionCount)
            host.Reset "languages.fsharp.interactive.preview"
            Assert.True(Session.SessionsProperties.fsiPreview)
        finally
            Session.SessionsProperties.Update original

    [<Fact>]
    let ``Every legacy editor and Interactive field imports once without rewriting its source`` () =
        task {
            let fixture = ImportFixture()

            for _, defaults in editorRecords do
                fixture.Legacy[UnifiedSettingsMigration.editorKey (defaults.GetType().Name)] <- JsonConvert.SerializeObject defaults

            for suffix, legacyName in UnifiedSettingsMigration.interactiveSettings do
                fixture.Legacy[UnifiedSettingsMigration.interactiveKey legacyName] <-
                    if suffix = "commandLineArgs" then
                        box "--warn:5"
                    else
                        box false

            let source = fixture.Legacy |> Seq.map (fun (KeyValue(k, v)) -> k, v) |> Seq.toArray
            do! fixture.Run()
            Assert.Empty fixture.Errors
            Assert.Equal(42, fixture.Native.Values.Count)
            Assert.Equal(12, fixture.Checkpoints.Count)

            for name, defaults, property in editorSettings () do
                if property.PropertyType = typeof<int option> then
                    Assert.Equal(box false, fixture.Native.Values[name])
                else
                    let expected = property.GetValue defaults

                    Assert.Equal(
                        (if property.PropertyType = typeof<bool> || property.PropertyType = typeof<int> then
                             expected
                         else
                             box (string expected)),
                        fixture.Native.Values[name]
                    )

            for suffix, _ in UnifiedSettingsMigration.interactiveSettings do
                Assert.Equal(
                    (if suffix = "commandLineArgs" then
                         box "--warn:5"
                     else
                         box false),
                    fixture.Native.Values[$"languages.fsharp.interactive.{suffix}"]
                )

            for key, value in source do
                Assert.Equal(value, fixture.Legacy[key])

            fixture.Native.Reset "languages.fsharp.quickInfo.showRemarks"
            let commits = fixture.Native.CommitCount
            do! fixture.Run()
            Assert.Equal(commits, fixture.Native.CommitCount)
            Assert.False(fixture.Native.Values.ContainsKey "languages.fsharp.quickInfo.showRemarks")
            Assert.Equal(12, fixture.Checkpoints.Count)
        }

    [<Theory>]
    [<InlineData("null", false, 0)>]
    [<InlineData("{\"Case\":\"Some\",\"Fields\":[20]}", true, 20)>]
    [<InlineData("{\"Case\":\"Some\",\"Fields\":[120]}", true, 120)>]
    [<InlineData("{\"Case\":\"Some\",\"Fields\":[2147483647]}", true, Int32.MaxValue)>]
    let ``Optional width imports as a Boolean and integer`` value enabled width =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <- $"{{\"DescriptionWidth\":{value}}}"
            do! fixture.Run()
            Assert.Empty fixture.Errors
            Assert.Equal(box enabled, fixture.Native.Values["languages.fsharp.quickInfo.descriptionWidth"])

            if enabled then
                Assert.Equal(box width, fixture.Native.Values["languages.fsharp.quickInfo.descriptionWidthInCharacters"])
            else
                Assert.False(fixture.Native.Values.ContainsKey "languages.fsharp.quickInfo.descriptionWidthInCharacters")
        }

    [<Theory>]
    [<InlineData("languages.fsharp.quickInfo.descriptionWidth", false)>]
    [<InlineData("languages.fsharp.quickInfo.descriptionWidthInCharacters", 80)>]
    let ``A customized width pair is not partially overwritten`` protectedName (value: obj) =
        task {
            let fixture = ImportFixture()
            fixture.Native.Set(protectedName, value)

            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <-
                "{\"DescriptionWidth\":{\"Case\":\"Some\",\"Fields\":[120]}}"

            do! fixture.Run()
            Assert.Empty fixture.Errors
            Assert.Equal(value, fixture.Native.Values[protectedName])
            Assert.Single(fixture.Native.Values) |> ignore
        }

    [<Fact>]
    let ``Existing native customizations including explicit defaults and roamed keys win`` () =
        task {
            let fixture = ImportFixture()
            fixture.Native.Set("languages.fsharp.quickInfo.showRemarks", true)
            fixture.Legacy["UnifiedSettings.languages.fsharp.quickInfo.displayLinks"] <- false
            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <- "{\"ShowRemarks\":false,\"DisplayLinks\":true}"
            do! fixture.Run()
            Assert.Empty fixture.Errors
            Assert.Equal(box true, fixture.Native.Values["languages.fsharp.quickInfo.showRemarks"])
            Assert.False(fixture.Native.Values.ContainsKey "languages.fsharp.quickInfo.displayLinks")
        }

    [<Theory>]
    [<InlineData("not json")>]
    [<InlineData("{\"DescriptionWidth\":{\"Case\":\"Some\",\"Fields\":[19]}}")>]
    [<InlineData("{\"DescriptionWidth\":{\"Case\":\"Some\",\"Fields\":[2147483648]}}")>]
    [<InlineData("{\"UnderlineStyle\":{\"Case\":\"Unknown\"}}")>]
    [<InlineData("{\"ShowRemarks\":\"false\"}")>]
    [<InlineData("{\"ShowRemarks\":true,\"ShowRemarks\":false}")>]
    let ``Invalid groups are reported while independent groups still import`` json =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <- json
            fixture.Legacy[UnifiedSettingsMigration.editorKey "FormattingOptions"] <- "{\"FormatOnPaste\":true}"
            do! fixture.Run()
            Assert.Single(fixture.Errors) |> ignore
            Assert.Equal(box true, fixture.Native.Values["languages.fsharp.formatting.formatOnPaste"])
            Assert.False(fixture.Legacy.ContainsKey(UnifiedSettingsMigration.checkpointKey "quickInfo"))
            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <- "{\"ShowRemarks\":false}"
            do! fixture.Run()
            Assert.Equal(box false, fixture.Native.Values["languages.fsharp.quickInfo.showRemarks"])
            Assert.Equal(2, fixture.Checkpoints.Count)
        }

    [<Theory>]
    [<InlineData(SettingCommitOutcome.InternalError)>]
    [<InlineData(SettingCommitOutcome.PendingApproval)>]
    let ``Unsuccessful commits never mark migration complete and can be retried`` outcome =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "FormattingOptions"] <- "{\"FormatOnPaste\":true}"
            fixture.Native.CommitOutcome <- outcome
            do! fixture.Run()
            Assert.Empty fixture.Native.Values
            Assert.Empty fixture.Checkpoints
            Assert.Single(fixture.Errors) |> ignore
            fixture.Native.CommitOutcome <- SettingCommitOutcome.Success
            do! fixture.Run()
            Assert.Equal(box true, fixture.Native.Values["languages.fsharp.formatting.formatOnPaste"])
            Assert.Single(fixture.Checkpoints) |> ignore
        }

    [<Fact>]
    let ``Denied writes and failed checkpoint persistence remain retryable`` () =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "FormattingOptions"] <- "{\"FormatOnPaste\":true}"
            fixture.Native.RejectChanges <- true
            do! fixture.Run()
            Assert.Empty fixture.Native.Values
            Assert.Empty fixture.Checkpoints
            fixture.Native.RejectChanges <- false
            fixture.FailCheckpoint <- true
            do! fixture.Run()
            Assert.Equal(box true, fixture.Native.Values["languages.fsharp.formatting.formatOnPaste"])
            Assert.Empty fixture.Checkpoints
            fixture.FailCheckpoint <- false
            do! fixture.Run()
            Assert.Single(fixture.Checkpoints) |> ignore
            Assert.Equal(1, fixture.Native.CommitCount)
            Assert.Equal(2, fixture.Errors.Count)
        }

    [<Fact>]
    let ``Cancellation does not import or checkpoint values`` () =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "FormattingOptions"] <- "{\"FormatOnPaste\":true}"
            use cancellation = new CancellationTokenSource()
            cancellation.Cancel()
            let! _ = Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> fixture.Run(cancellation.Token))
            Assert.Empty fixture.Native.Values
            Assert.Empty fixture.Checkpoints
        }

    [<Fact>]
    let ``Native settings no longer share a legacy record writer`` () =
        task {
            let fixture = ImportFixture()
            fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"] <- "{\"ShowRemarks\":false,\"DisplayLinks\":false}"
            do! fixture.Run()
            fixture.Native.Set("languages.fsharp.quickInfo.showRemarks", true)
            Assert.Equal(box false, fixture.Native.Values["languages.fsharp.quickInfo.displayLinks"])

            Assert.Equal(
                "{\"ShowRemarks\":false,\"DisplayLinks\":false}",
                unbox<string> fixture.Legacy[UnifiedSettingsMigration.editorKey "QuickInfoOptions"]
            )
        }

    [<Fact>]
    let ``Compiler provider reads writes notifies and rejects migrated settings`` () =
        task {
            let mutable compiler = true
            let errors = ResizeArray<exn>()

            let provider =
                FSharpSettingsProvider((fun () -> compiler), (fun value -> compiler <- value), errors.Add)
                :> ICachingExternalSettingsProvider

            let mutable notifications = 0
            provider.SettingValuesChanged.Add(fun _ -> notifications <- notifications + 1)
            let! read = provider.GetValueAsync<bool>("UseNetSdkCompiler", CancellationToken.None)
            Assert.True(Assert.IsType<ExternalSettingOperationResult<bool>.Success>(read).Value)
            let! write = provider.SetValueAsync("UseNetSdkCompiler", false, CancellationToken.None)
            Assert.IsType<ExternalSettingOperationResult.Success>(write) |> ignore
            Assert.False compiler
            Assert.Equal(1, notifications)
            let! rejected = provider.SetValueAsync("UseNetSdkCompiler", "false", CancellationToken.None)
            Assert.IsType<ExternalSettingOperationResult.Failure>(rejected) |> ignore

            for name, _, _ in editorSettings () do
                let! rejected = provider.GetValueAsync<obj>(name, CancellationToken.None)
                Assert.IsType<ExternalSettingOperationResult<obj>.Failure>(rejected) |> ignore

            Assert.Equal(37, errors.Count)
            use cancellation = new CancellationTokenSource()
            cancellation.Cancel()

            let! _ =
                Assert.ThrowsAnyAsync<OperationCanceledException>(fun () ->
                    provider.SetValueAsync("UseNetSdkCompiler", true, cancellation.Token) :> Task)

            Assert.False compiler
        }

    [<Fact>]
    let ``The package has no remaining legacy options page registrations`` () =
        use stream = File.OpenRead(typeof<EditorOptions>.Assembly.Location)
        use pe = new PEReader(stream)
        let reader = pe.GetMetadataReader()

        let package =
            reader.TypeDefinitions
            |> Seq.map reader.GetTypeDefinition
            |> Seq.find (fun definition -> reader.GetString(definition.Name) = "FSharpPackage")

        for handle in package.GetCustomAttributes() do
            let attribute = reader.GetCustomAttribute handle

            if attribute.Constructor.Kind = HandleKind.MemberReference then
                let constructor =
                    reader.GetMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor)

                let owner =
                    reader.GetTypeReference(TypeReferenceHandle.op_Explicit constructor.Parent)

                Assert.DoesNotContain(
                    reader.GetString(owner.Name),
                    [| "ProvideOptionPageAttribute"; "ProvideLanguageEditorOptionPageAttribute" |]
                )

    [<Fact>]
    let ``Localized resources resolve in their owning shipped assembly`` () =
        let assemblies =
            dict
                [
                    "FSharp.Editor.dll", typeof<EditorOptions>.Assembly
                    "FSharp.VS.FSI.dll", typeof<FSharpCompilerSettings>.Assembly
                    "FSharp.UIResources.dll", Assembly.Load("FSharp.UIResources")
                ]

        for value in (manifest ()).Descendants() do
            if value.Type = JTokenType.String then
                let resource = value.Value<string>()

                if resource.StartsWith("@", StringComparison.Ordinal) then
                    let separator = resource.IndexOf(';')
                    let colon = resource.IndexOf(':')

                    let manager =
                        ResourceManager(resource.Substring(1, colon - 1), assemblies[resource.Substring(separator + 1)])

                    Assert.False(String.IsNullOrEmpty(manager.GetString(resource.Substring(colon + 1, separator - colon - 1))), resource)
