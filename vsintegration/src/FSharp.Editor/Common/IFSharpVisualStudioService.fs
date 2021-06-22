// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell

type internal IFSharpVisualStudioService =

    abstract Workspace: Workspace

[<System.Composition.Shared>]
[<System.ComponentModel.Composition.Export(typeof<IFSharpVisualStudioService>)>]
type internal FSharpVisualStudioService
    [<System.ComponentModel.Composition.ImportingConstructor>]
    (
        [<System.ComponentModel.Composition.Import(typeof<VisualStudioWorkspace>)>] workspace
    ) =

    interface IFSharpVisualStudioService with

        member _.Workspace = workspace