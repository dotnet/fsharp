// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading

open FSharp.Compiler.Text.Range

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor
open CancellableTasks
open System.Collections.Generic

[<Export(typeof<IFSharpGoToDefinitionService>)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService [<ImportingConstructor>] (metadataAsSource: FSharpMetadataAsSourceService) =

    interface IFSharpGoToDefinitionService with
        /// Invoked with Peek Definition.
        member _.FindDefinitionsAsync(document: Document, position: int, _cancellationToken: CancellationToken) =
            cancellableTask {
                let navigation = FSharpNavigation(metadataAsSource, document, rangeStartup)
                let! res = navigation.FindDefinitionsAsync(position)
                return (res :> IEnumerable<_>)
            }
            |> CancellableTask.startWithoutCancellation

        /// Invoked with Go to Definition.
        /// Try to navigate to the definition of the symbol at the symbolRange in the originDocument
        member _.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            let navigation = FSharpNavigation(metadataAsSource, document, rangeStartup)
            navigation.TryGoToDefinition(position, cancellationToken)
