// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable
open System.ComponentModel.Composition
open System.IO
open System.Reflection
open System.Threading
open System.Threading.Tasks
open Microsoft.VisualStudio.ProjectSystem
open Microsoft.VisualStudio.ProjectSystem.Build

// We can't use well-known constants here because `string + string` isn't a valid constant expression in F#.
[<AppliesTo("FSharp&LanguageService")>]
[<ExportBuildGlobalPropertiesProvider>]
type internal SetGlobalPropertiesForSdkProjects
    [<ImportingConstructor>]
    (
        projectService: IProjectService
    ) =
    inherit StaticGlobalPropertiesProviderBase(projectService.Services)

    override __.GetGlobalPropertiesAsync(_cancellationToken: CancellationToken): Task<IImmutableDictionary<string, string>> =
        let editorDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        [ "FSharpPropsShim", "Microsoft.FSharp.NetSdk.props"
          "FSharpTargetsShim", "Microsoft.FSharp.NetSdk.targets"
          "FSharpOverridesTargetsShim", "Microsoft.FSharp.Overrides.NetSdk.targets" ]
        |> List.map (fun (key, value) -> (key, Path.Combine(editorDirectory, value)))
        |> List.fold (fun (map:ImmutableDictionary<string, string>) (key, value) -> map.Add(key, value)) (Empty.PropertiesMap)
        |> Task.FromResult<IImmutableDictionary<string, string>>
