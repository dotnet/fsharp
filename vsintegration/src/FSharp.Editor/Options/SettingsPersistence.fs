namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Runtime.InteropServices

open Microsoft.VisualStudio.Settings

open Newtonsoft.Json

type IPersistSettings =
    abstract member LoadSettings : unit -> 't
    abstract member SaveSettings : 't -> unit

[<Guid(Guids.svsSettingsPersistenceManagerIdString)>]
type SVsSettingsPersistenceManager = class end

type SettingsStore(serviceProvider: IServiceProvider) =

    let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager

    let storageKeyVersions (typ: Type) =
        // "TextEditor" prefix seems to be required for settings changes to be synced between IDE instances
        [ "TextEditor.FSharp." + typ.Namespace + "." + typ.Name
          // we keep this old storage key to upgrade without reverting user changes
          typ.Namespace + "." + typ.Name ]
        
    let storageKey (typ: Type) = storageKeyVersions typ |> List.head

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // and it is usually representing one separate option page in the UI.
    // We cache exactly one immutable value of each type.
    // This cache is updated by the SettingsStore when the user makes changes in the Options dialog
    // or when a change is propagated from another VS IDE instance by SVsSettingsPersistenceManager. 
    let cache = ConcurrentDictionary<Type, obj>()

    let getCached() =
        match cache.TryGetValue(typeof<'t>) with
        | true, (:? 't as value) -> value
        | _ -> failwithf "Settings %s are not registered." typeof<'t>.Name

    let keepInCache settings = cache.[settings.GetType()] <- settings

    // The settings record, even though immutable, is being effectively mutated in two instances:
    //   when it is passed to the UI (provided it is marked with CLIMutable attribute);
    //   when it is being populated from JSON using JsonConvert.PopulateObject;
    // We make a deep copy in these instances to isolate and contain the mutation
    let clone (v: 't) = JsonConvert.SerializeObject v |> JsonConvert.DeserializeObject<'t>

    let updateFromStore settings =
        // make a deep copy so that PopulateObject does not alter the original
        let copy = clone settings
        // if the new key is not found by ISettingsManager, we try the old keys
        // so that user settings are not lost
        settings.GetType() |> storageKeyVersions 
        |> Seq.map (settingsManager.TryGetValue)
        |> Seq.tryPick ( function GetValueResult.Success, json -> Some json | _ -> None )
        |> Option.iter (fun json -> try JsonConvert.PopulateObject(json, copy) with _ -> ())
        copy

    member __.Get() = getCached()

    // Used by the AbstractOptionPage to populate dialog controls.
    // We always have the latest value in the cache so we just return
    // cloned value here because it may be altered by the UI if declared with [<CLIMutable>]
    member __.LoadSettings() = getCached() |> clone

    member __.SaveSettings settings =
        // We replace default serialization with Newtonsoft.Json for easy schema evolution.
        // For example, if we add a new bool field to the record, representing another checkbox in Options dialog
        // deserialization will still work fine. When we pass default value to JsonConvert.PopulateObject it will
        // fill just the known fields.
        settingsManager.SetValueAsync(settings.GetType() |> storageKey, JsonConvert.SerializeObject settings, false)
        |> Async.AwaitTask |> Async.Start

    // This is the point we retrieve the initial value and subscribe to watch for changes
    member __.Register (defaultSettings : 'options) =
        defaultSettings |> updateFromStore |> keepInCache
        let subset = defaultSettings.GetType() |> storageKey |> settingsManager.GetSubset
        // this event is also raised when a setting change occurs in another VS instance, so we can keep everything in sync
        PropertyChangedAsyncEventHandler ( fun _ _ ->
            (getCached(): 'options) |> updateFromStore |> keepInCache
            System.Threading.Tasks.Task.CompletedTask )
        |> subset.add_SettingChangedAsync

         