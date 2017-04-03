namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.Reflection
open System.ComponentModel

open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell
open System.Collections.Concurrent

module internal SettingsPersistence =

    let private cache = ConcurrentDictionary<Type, obj>()

    let getCachedSettings() =
        match cache.TryGetValue(typeof<'t>) with
        | true, value -> value :?> 't
        | _ -> failwith "Setting is not registered."

    let registerSetting (defaultValue : 't) = cache.[typeof<'t>] <- defaultValue

    [<Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")>]
    type SVsSettingsPersistenceManager = class end

    type ISettings = abstract member RegisterAll : unit -> unit

    [<Export>]
    type SettingsStore
        [<ImportingConstructor>]
        (
            [<Import(typeof<SVsServiceProvider>)>] 
            serviceProvider: IServiceProvider,
            settings: ISettings
        ) =

        let settingsManager = serviceProvider.GetService(typeof<SVsSettingsPersistenceManager>) :?> ISettingsManager

        let subset = settingsManager.GetSubset("Microsoft.VisualStudio.FSharp.Editor.*")

        let refresh typ = cache.[typ] <- settingsManager.GetValueOrDefault(typ.FullName, cache.[typ])

        let changeHandler _ (args: PropertyChangedEventArgs) =
            Assembly.GetAssembly(typeof<SettingsStore>).DefinedTypes 
            |> Seq.tryFind (fun typ -> typ.FullName = args.PropertyName)
            |> Option.iter refresh
            System.Threading.Tasks.Task.CompletedTask
        do
            settings.RegisterAll()
            subset.add_SettingChangedAsync <| PropertyChangedAsyncEventHandler(changeHandler)
            cache.Keys |> Seq.iter refresh

        member this.LoadSettings() : 't =
            let result, (value: 't) = settingsManager.TryGetValue(typeof<'t>.FullName)
            if result = GetValueResult.Success then cache.[typeof<'t>] <- value
            getCachedSettings()

        member this.SaveSettings( settings: 't) =
            cache.[typeof<'t>] <- settings
            settingsManager.SetValueAsync(typeof<'t>.FullName, settings, false)
            |> Async.AwaitTask |> Async.StartImmediate


