// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading

open FSharp.Compiler.Text.Range

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.GoToDefinition

open CancellableTasks

[<Export(typeof<IFSharpFindDefinitionService>)>]
[<Export(typeof<FSharpFindDefinitionService>)>]
type internal FSharpFindDefinitionService [<ImportingConstructor>] (metadataAsSource: FSharpMetadataAsSourceService) =
    interface IFSharpFindDefinitionService with
        member _.FindDefinitionsAsync(document: Document, position: int, cancellationToken: CancellationToken) =
            cancellableTask {
                let navigation = FSharpNavigation(metadataAsSource, document, rangeStartup)
                return! navigation.FindDefinitionsAsync(position)
            }
            |> CancellableTask.start cancellationToken
