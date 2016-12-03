// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Controls
open System.Windows.Media

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor.Implementation.InlineRename

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

module InlineRenameInfo =
    let create () =
        { new IInlineRenameInfo with
            member __.CanRename = true
            member __.LocalizedErrorMessage = ""
            member __.TriggerSpan = Unchecked.defaultof<_>
            member __.HasOverloads = false
            member __.ForceRenameOverloads = true
            member __.DisplayName = ""
            member __.FullDisplayName = ""
            member __.Glyph = Glyph.MethodPublic
            member __.GetFinalSymbolName replacementText = ""
            member __.GetReferenceEditSpan(location, cancellationToken) = Unchecked.defaultof<_>
            member __.GetConflictEditSpan(location, replacementText, cancellationToken) = Nullable()
            member __.FindRenameLocationsAsync(optionSet, cancellationToken) = Task<IInlineRenameLocationSet>.FromResult null
            member __.TryOnBeforeGlobalSymbolRenamed(workspace, changedDocumentIDs, replacementText) = true
            member __.TryOnAfterGlobalSymbolRenamed(workspace, changedDocumentIDs, replacementText) = true
        }

[<ExportLanguageService(typeof<IEditorInlineRenameService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal InlineRenameService [<ImportingConstructor>]([<ImportMany>] _refactorNotifyServices: seq<IRefactorNotifyService>) =
    interface IEditorInlineRenameService with
        member __.GetRenameInfoAsync(_document: Document, _position: int, _cancellationToken: CancellationToken) : Task<IInlineRenameInfo> =
            Task.FromResult (InlineRenameInfo.create())