// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.IO
open Microsoft.FSharp.Core
open Microsoft.VisualStudio.Shell

type ProvideCodeExpansionsAttribute(
                                    languageGuidString: string,
                                    showRoots: bool,
                                    displayName: int16,
                                    languageStringId: string,
                                    indexPath: string) =
    inherit RegistrationAttribute()

    let languageName = @"Languages\CodeExpansions\" + languageStringId

    override this.Register(context: RegistrationAttribute.RegistrationContext) =
        if context <> null then
            let fullEscapedPath p =
                Path.Combine(context.ComponentPath, p)
                |> Path.GetFullPath
                |> context.EscapePath
            use childKey = context.CreateKey(languageName)            
            childKey.SetValue("", new Guid(languageGuidString))
            childKey.SetValue("DisplayName", displayName.ToString())
            childKey.SetValue("IndexPath", fullEscapedPath indexPath)
            childKey.SetValue("LangStringId", languageStringId.ToLowerInvariant())
            childKey.SetValue("Package", context.ComponentType.GUID.ToString("B"))
            childKey.SetValue("ShowRoots", if showRoots then 1 else 0)

    override this.Unregister(context: RegistrationAttribute.RegistrationContext) =
        if context <> null then
            context.RemoveKey(languageName)

type ProvideCodeExpansionPathAttribute(
                                       languageStringId: string,
                                       description: string,
                                       paths: string) =
    inherit RegistrationAttribute()

    let languageName = @"Languages\CodeExpansions\" + languageStringId

    override this.Register(context: RegistrationAttribute.RegistrationContext) =
        if context <> null then
            use childKey = context.CreateKey(languageName)
            let snippetPaths =
                Path.Combine(context.ComponentPath, paths)
                |> Path.GetFullPath
                |> context.EscapePath
            use pathsSubKey = childKey.CreateSubkey("Paths")
            pathsSubKey.SetValue(description, snippetPaths)

    override this.Unregister(_context: RegistrationAttribute.RegistrationContext) =
        ()
