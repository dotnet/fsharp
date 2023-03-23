// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading

open FSharp.Compiler.Text.Range

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.GoToDefinition

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System.Collections.Immutable
open System.Threading.Tasks

[<Export(typeof<IFSharpFindDefinitionService>)>]
[<Export(typeof<FSharpFindDefinitionService>)>]
type internal FSharpFindDefinitionService [<ImportingConstructor>] (metadataAsSource: FSharpMetadataAsSourceService) =

    let statusBar =
        StatusBar(ServiceProvider.GlobalProvider.GetService<SVsStatusbar, IVsStatusbar>())

    interface IFSharpFindDefinitionService with
        member _.FindDefinitionsAsync(document: Document, position: int, cancellationToken: CancellationToken) =
            let navigation =
                FSharpNavigation(statusBar, metadataAsSource, document, rangeStartup)

            let definitions = navigation.FindDefinitions(position, cancellationToken)
            ImmutableArray.CreateRange(definitions) |> Task.FromResult
