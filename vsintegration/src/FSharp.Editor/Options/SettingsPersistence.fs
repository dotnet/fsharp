namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Runtime.InteropServices

open Microsoft.VisualStudio.Settings

open Newtonsoft.Json

type IPersistSettings =
    abstract member Read : unit -> 't
    abstract member Write : 't -> unit

[<Guid(Guids.svsSettingsPersistenceManagerIdString)>]
type SVsSettingsPersistenceManager = class end

type SettingsStore(serviceProvider: IServiceProvider) =

    let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager

    let storageKeyVersions (typ: Type) =
        [ "TextEditor.FSharp." + typ.Namespace + "." + typ.Name
          typ.Namespace + "." + typ.Name ]
        
    let storageKey (typ: Type) = storageKeyVersions typ |> List.head

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // We cache exactly one instance of each, treating them as immutable.
    // This cache is updated by the SettingsStore when the user changes an option.
    let cache = ConcurrentDictionary<Type, obj>()

    let getCached() =
        match cache.TryGetValue(typeof<'t>) with
        | true, (:? 't as value) -> value
        | _ -> failwithf "Settings %s are not registered." typeof<'t>.Name

    let keepInCache settings = cache.[settings.GetType()] <- settings

    let clone (v: 't) = JsonConvert.SerializeObject v |> JsonConvert.DeserializeObject<'t>

    let updateFromStore settings =
        let copy = clone settings
        settings.GetType() |> storageKeyVersions 
        |> Seq.map (settingsManager.TryGetValue)
        |> Seq.tryPick ( function GetValueResult.Success, json -> Some json | _ -> None )
        |> Option.iter (fun json -> try JsonConvert.PopulateObject(json, copy) with _ -> ())
        copy

    member __.Get() = getCached()
        
    member __.Read() = getCached() |> clone

    member __.Write settings =
        // we replace default serialization with Newtonsoft.Json for easy schema evolution
        settingsManager.SetValueAsync(settings.GetType() |> storageKey, JsonConvert.SerializeObject settings, false)
        |> Async.AwaitTask |> Async.Start

    member __.Register (defaultSettings : 'options) =
        defaultSettings |> updateFromStore |> keepInCache
        let subset = defaultSettings.GetType() |> storageKey |> settingsManager.GetSubset

        PropertyChangedAsyncEventHandler ( fun _ _ ->
            (getCached(): 'options) |> updateFromStore |> keepInCache
            System.Threading.Tasks.Task.CompletedTask )
        |> subset.add_SettingChangedAsync

         