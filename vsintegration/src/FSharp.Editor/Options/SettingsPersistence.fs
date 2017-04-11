namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.ComponentModel.Composition
open System.Reflection
open System.Runtime.InteropServices

open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell

open Newtonsoft.Json

module internal SettingsPersistence =

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // We cache exactly one instance of each, treating them as immutable.
    // This cache is updated by the SettingsStore when the user changes an option.
    let private cache = ConcurrentDictionary<Type, obj>()

    let getSettings() : 't =
        match cache.TryGetValue(typeof<'t>) with
        | true, value -> value :?> 't
        | _ -> failwithf "Settings %s are not registered." typeof<'t>.Name

    let setSettings( settings: 't) =
        cache.[typeof<'t>] <- settings

    [<Guid(Guids.svsSettingsPersistenceManagerIdString)>]
    type SVsSettingsPersistenceManager = class end

    // marker interface for default settings export
    type ISettings = interface end

    [<Export>]
    type SettingsStore
        [<ImportingConstructor>]
        (
            [<Import(typeof<SVsServiceProvider>)>] 
            serviceProvider: IServiceProvider
        ) =
        let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager

        // settings quallified type names are used as keys, this should be enough to avoid collisions
        let storageKey (typ: Type) = typ.Namespace + "." + typ.Name

        let save (settings: 't) =
            // we replace default serialization with Newtonsoft.Json for easy schema evolution
            settingsManager.SetValueAsync(storageKey typeof<'t>, JsonConvert.SerializeObject settings, false)
            |> Async.AwaitTask 

        let tryPopulate (settings: 't) =
            let result, json = settingsManager.TryGetValue(storageKey typeof<'t>)
            if result = GetValueResult.Success then
                // if it fails we just return what we got
                try JsonConvert.PopulateObject(json, settings) with _ -> () 
            settings       

        let ensureTrackingChanges (settings: 't) =                       
            settings |> tryPopulate |> setSettings
            let subset = settingsManager.GetSubset(storageKey typeof<'t>)
            subset.add_SettingChangedAsync 
            <| PropertyChangedAsyncEventHandler (fun _ _ ->
                (getSettings() : 't) |> tryPopulate |> setSettings
                System.Threading.Tasks.Task.CompletedTask )

        member this.LoadSettings() : 't =
            getSettings() |> tryPopulate
            
        member this.SaveSettings(settings: 't) =
            save settings 

        member __.RegisterDefault(defaultValue: 't) =
            ensureTrackingChanges defaultValue