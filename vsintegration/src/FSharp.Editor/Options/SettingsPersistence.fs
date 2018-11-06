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
    // settings quallified type names are used as keys, this should be enough to avoid collisions
    let storageKey (typ: Type) = typ.Namespace + "." + typ.Name

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // We cache exactly one instance of each, treating them as immutable.
    // This cache is updated by the SettingsStore when the user changes an option.
    let cache = System.Collections.Concurrent.ConcurrentDictionary<Type, obj>()

    let read() =
        match cache.TryGetValue(typeof<'t>) with
        | true, value -> value :?> 't
        | _ -> failwithf "Settings %s are not registered." typeof<'t>.Name 

    let write settings = cache.[settings.GetType()] <- settings

    let updateFromStore settings =
        let result, json = settings.GetType() |> storageKey |> settingsManager.TryGetValue
        if result = GetValueResult.Success then
            // if it fails we just return what we got
            try JsonConvert.PopulateObject(json, settings) with _ -> () 
        settings
        
    member __.Read() = read()

    member __.Write settings =
        write settings
        // we replace default serialization with Newtonsoft.Json for easy schema evolution
        settingsManager.SetValueAsync(settings.GetType() |> storageKey, JsonConvert.SerializeObject settings, false)
        |> Async.AwaitTask |> Async.StartImmediate   

    member __.Register (defaultSettings : 'options) =                       
        defaultSettings |> updateFromStore |> write
        let subset = defaultSettings.GetType() |> storageKey |> settingsManager.GetSubset

        PropertyChangedAsyncEventHandler ( fun _ _ ->
            (read() :'options) |> updateFromStore |> write
            System.Threading.Tasks.Task.CompletedTask )
        |> subset.add_SettingChangedAsync
         