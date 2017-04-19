// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.CodeAnalysis.Editor
open Microsoft.VisualStudio.Utilities

module FSharpStaticTypeDefinitions = 
    [<Export>]
    [<Name(FSharpConstants.FSharpContentTypeName)>]
    [<BaseDefinition(ContentTypeNames.RoslynContentType)>]
    let FSharpContentTypeDefinition = ContentTypeDefinition()

    [<Export>]
    [<Name(FSharpConstants.FSharpSignatureHelpContentTypeName)>]
    [<BaseDefinition("sighelp")>]
    let FSharpSignatureHelpContentTypeDefinition = ContentTypeDefinition()

[<ExportContentTypeLanguageService(FSharpConstants.FSharpContentTypeName, FSharpConstants.FSharpLanguageName)>]
type FSharpContentType [<System.Composition.ImportingConstructor>](contentTypeRegistry : IContentTypeRegistryService) =  
    member this.contentTypeRegistryService = contentTypeRegistry
 
    interface IContentTypeLanguageService with
        member this.GetDefaultContentType() = 
            this.contentTypeRegistryService.GetContentType(FSharpConstants.FSharpContentTypeName)
