namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell
open System.Collections.Concurrent

module internal SettingsPersistence =

    let cache = ConcurrentDictionary<Type, obj>()

    let getSettings() = 
        match cache.TryGetValue(typeof<'t>) with
        | true, value -> value :?> 't
        | _ -> new 't()

    [<Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")>]
    type SVsSettingsPersistenceManager = class end

    [<Export>]
    type SettingsStore
        [<ImportingConstructor>]
        (
            [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider
        ) =

        let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager
        let name settings = settings.GetType().FullName

        member this.LoadOrSetDefault(defaultSetting: 't) =
            let value = settingsManager.GetValueOrDefault<'t>(name defaultSetting, defaultSetting)
            cache.[typeof<'t>] <- value
            value

        member this.SaveSettings( settings ) =
            cache.[settings.GetType()] <- settings
            settingsManager.SetValueAsync(name settings, settings, false)
            |> Async.AwaitTask |> Async.StartImmediate

        member this.GetSettings() =
            match cache.TryGetValue(typeof<'t>) with
            | true, value -> value :?> 't
            | _ -> this.LoadOrSetDefault(new 't())

