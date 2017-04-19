// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef

open Microsoft.VisualStudio.Shell

type internal FSharpCompletionService
    (
        workspace: Workspace,
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        assemblyContentProvider: AssemblyContentProvider
    ) =
    inherit CompletionServiceWithProviders(workspace)

    let builtInProviders = 
        ImmutableArray.Create<CompletionProvider>(
            FSharpCompletionProvider(workspace, serviceProvider, checkerProvider, projectInfoManager, assemblyContentProvider),
            HashDirectiveCompletionProvider(workspace, projectInfoManager,
                [ Completion.Create("""\s*#load\s+(@?"*(?<literal>"[^"]*"?))""", [".fs"; ".fsx"], useIncludeDirectives = true)
                  Completion.Create("""\s*#r\s+(@?"*(?<literal>"[^"]*"?))""", [".dll"; ".exe"], useIncludeDirectives = true)
                  Completion.Create("""\s*#I\s+(@?"*(?<literal>"[^"]*"?))""", ["\x00"], useIncludeDirectives = false) ]))

    let completionRules = 
        CompletionRules.Default
            .WithDismissIfEmpty(true)
            .WithDismissIfLastCharacterDeleted(true)
            .WithDefaultEnterKeyRule(EnterKeyRule.Never)

    override this.Language = FSharpConstants.FSharpLanguageName
    override this.GetBuiltInProviders() = builtInProviders
    override this.GetRules() = completionRules

[<Shared>]
[<ExportLanguageServiceFactory(typeof<CompletionService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpCompletionServiceFactory 
    [<ImportingConstructor>] 
    (
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        assemblyContentProvider: AssemblyContentProvider
    ) =
    interface ILanguageServiceFactory with
        member this.CreateLanguageService(hostLanguageServices: HostLanguageServices) : ILanguageService =
            upcast new FSharpCompletionService(hostLanguageServices.WorkspaceServices.Workspace, serviceProvider, checkerProvider, projectInfoManager, assemblyContentProvider)


