namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.ComponentModel.Composition
open System.Reflection
open System.Runtime.InteropServices

open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell

module internal SettingsPersistence =

    // Each group of settings is a value of some named type, for example 'IntelliSenseOptions', 'QuickInfoOptions'
    // We cache exactly one instance of each, treating them as immutable.
    // This cache is updated by the SettingsStore when the user changes an option.
    let private cache = ConcurrentDictionary<Type, obj>()

    let getSettings() =
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

        let ensureTrackingChanges (settings: 't) =
            if not (cache.ContainsKey typeof<'t>) then
                let key = storageKey typeof<'t>
                settingsManager.GetValueOrDefault(key, settings) |> setSettings
                let subset = settingsManager.GetSubset(key)
                subset.add_SettingChangedAsync 
                <| PropertyChangedAsyncEventHandler (fun _ _ ->
                    settingsManager.GetValueOrDefault<'t>(key, getSettings()) |> setSettings
                    System.Threading.Tasks.Task.CompletedTask )

        member this.LoadSettings() : 't =
            let result, (settings: 't) = settingsManager.TryGetValue(storageKey typeof<'t>)
            if result = GetValueResult.Success then
                setSettings settings
            getSettings()

        member this.SaveSettings(settings: 't) =
            settingsManager.SetValueAsync(storageKey typeof<'t>, settings, false)
            |> Async.AwaitTask |> Async.StartImmediate

        member __.RegisterDefault(defaultValue: 't) =
            ensureTrackingChanges defaultValue