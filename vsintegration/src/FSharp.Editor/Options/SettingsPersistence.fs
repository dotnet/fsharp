namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent

open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell

open Newtonsoft.Json

module Settings =
    type IPersistSettings =
        abstract member LoadSettings : unit -> 't
        abstract member SaveSettings : 't -> unit

    type SettingsStore =
        inherit IPersistSettings
        abstract member Register : 't -> unit
        abstract member Get : unit -> 't

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // and it is usually representing one separate option page in the UI.
    // We cache exactly one immutable value of each type.
    // This cache is updated by the PersistentStore when the user makes changes in the Options dialog
    // or when a change is propagated from another VS IDE instance by ISettingsManager.
    type Cache = ConcurrentDictionary<string, obj>

    let storageKey (t: Type) = $"TextEditor.FSharp.{t.Namespace}.{t.Name}"

    let storageKeyOf settings = settings.GetType() |> storageKey

    let getValue (cache: Cache) =
        match cache.TryGetValue(storageKey (typeof<'t>)) with
        | true, (:? 't as value) -> value
        | _ -> failwithf "Settings %s are not registered." typeof<'t>.Name

    let setValue (cache: Cache) settings = cache.[storageKeyOf settings] <- settings

    // The settings record, even though immutable, is being effectively mutated in two instances:
    //   when it is passed to the UI (provided it is marked with CLIMutable attribute);
    //   when it is being populated from JSON using JsonConvert.PopulateObject;
    // We make a deep copy in these instances to isolate and contain the mutation.
    let clone (v: 't) = JsonConvert.SerializeObject v |> JsonConvert.DeserializeObject<'t>

    type TestStore() =
        // When running without Visual Studio, i.e. unit tests, we use only this in-memory store.
        let cache = Cache() 
        interface SettingsStore with
            member this.Get() = getValue cache
            member this.LoadSettings() = cache |> getValue |> clone
            member this.Register(defaultSettings) = defaultSettings |> setValue cache
            member this.SaveSettings(settings) = settings |> setValue cache

    type PersistentStore(settingsManager: ISettingsManager) =
        let cache = Cache()  
    
        let updateFromStore settings =
            // Make a deep copy so that PopulateObject does not alter the original.
            let copy = clone settings
            match settingsManager.TryGetValue(storageKeyOf settings) with
            | GetValueResult.Success, json ->
                // In case this fails, we just return current version.
                try JsonConvert.PopulateObject(json, copy) with _ -> ()
                copy
            | _ -> copy

        interface SettingsStore with
            member _.Get() = getValue cache

            // Used by the AbstractOptionPage to populate dialog controls.
            // We always have the latest value in the cache so we just return
            // cloned value here because it may be altered by the UI if declared with [<CLIMutable>]
            member _.LoadSettings() = cache |> getValue |> clone

            member _.SaveSettings settings =
                // We replace default serialization with Newtonsoft.Json for easy schema evolution.
                // For example, if we add a new bool field to the record, representing another checkbox in Options dialog
                // deserialization will still work fine. When we pass default value to JsonConvert.PopulateObject it will
                // fill just the known fields.
                settingsManager.SetValueAsync(storageKeyOf settings, JsonConvert.SerializeObject settings, false) |> ignore

            // This is the point we retrieve the initial value and subscribe to watch for changes.
            member _.Register defaultSettings =
                defaultSettings |> updateFromStore |> setValue cache
                let subset = storageKeyOf defaultSettings |> settingsManager.GetSubset
                // This event is also raised when a setting change occurs in another VS instance, so we can keep everything in sync.
                // Note: In experimental VS instance it only picks up local changes.
                subset.add_SettingChangedAsync(
                    PropertyChangedAsyncEventHandler( fun _ _ ->
                        backgroundTask { cache |> getValue |> updateFromStore |> setValue cache } )
                )

    let CreateStore() : SettingsStore =          
        match ServiceProvider.GlobalProvider.GetService(Guids.svsSettingsPersistenceManagerId) with
        | :? ISettingsManager as settingsManager -> PersistentStore(settingsManager)
        | _ -> TestStore()