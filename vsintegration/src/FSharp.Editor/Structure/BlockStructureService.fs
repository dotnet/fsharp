// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

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
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Structure

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
open System.Windows.Documents

[<ExportLanguageServiceFactory(typeof<BlockStructureService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpBlockStructureServiceFactory() =
    interface ILanguageServiceFactory with
        member __.CreateLanguageService(_languageServices) =
            upcast FSharpBlockStructureService()
 
type internal FSharpBlockStructureService() =
    inherit BlockStructureService()
        override __.Language = FSharpCommonConstants.FSharpLanguageName
 
        override __.GetBlockStructureAsync(_document, cancellationToken) : Task<BlockStructure> =
            async {
                return BlockStructure([].ToImmutableArray())
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)