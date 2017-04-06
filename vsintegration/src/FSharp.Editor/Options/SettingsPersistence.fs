namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.ComponentModel
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

    let getCachedSettings() =
        match cache.TryGetValue(typeof<'t>) with
        | true, value -> value :?> 't
        | _ -> failwithf "Setting %s is not registered." typeof<'t>.Name


    [<Guid(Guids.svsSettingsPersistenceManagerIdString)>]
    type SVsSettingsPersistenceManager = class end

    type IRegisterSettings = abstract member RegisterSetting : 't -> unit
    type ISettingsToRegister = abstract member RegisterAll: IRegisterSettings -> unit

    [<Export>]
    type SettingsStore
        [<ImportingConstructor>]
        (
            [<Import(typeof<SVsServiceProvider>)>] 
            serviceProvider: IServiceProvider,
            settingsToRegister: ISettingsToRegister
        ) as this =

        let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager   

        // settings quallified type names are used as keys, this should be enough to avoid collisions
        let storageKey (typ: Type) = typ.Namespace + "." + typ.Name

        let register(defaultValue: 't) =
            let key = storageKey typeof<'t>
            let refresh () = cache.[typeof<'t>] <- settingsManager.GetValueOrDefault(key, defaultValue)
            let subset = settingsManager.GetSubset(key)
            subset.add_SettingChangedAsync 
            <| PropertyChangedAsyncEventHandler (fun _ _ ->
                    refresh()
                    System.Threading.Tasks.Task.CompletedTask )
            refresh()

        do  settingsToRegister.RegisterAll(this)

        member this.LoadSettings() : 't =
            let result, (value: 't) = settingsManager.TryGetValue(storageKey typeof<'t>)
            if result = GetValueResult.Success then cache.[typeof<'t>] <- value
            getCachedSettings()

        member this.SaveSettings( settings: 't) =
            cache.[typeof<'t>] <- settings
            settingsManager.SetValueAsync(storageKey typeof<'t>, settings, false)
            |> Async.AwaitTask |> Async.StartImmediate

        interface IRegisterSettings with
            member __.RegisterSetting(defaultValue: 't) = register defaultValue
            


