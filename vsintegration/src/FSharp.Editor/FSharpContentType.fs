// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis.Editor
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Utilities
open System.ComponentModel.Composition
open System.Composition

module FSharpStaticTypeDefinitions = 
    [<Export>]
    [<Name(FSharpCommonConstants.FSharpContentTypeName)>]
    [<BaseDefinition(ContentTypeNames.RoslynContentType)>]
    let FSharpContentTypeDefinition = ContentTypeDefinition()

[<Shared>]
[<ExportContentTypeLanguageService(FSharpCommonConstants.FSharpContentTypeName, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpContentTypeLanguageService [<ImportingConstructor>](contentTypeRegistry : IContentTypeRegistryService) =  
    member this.contentTypeRegistryService = contentTypeRegistry
 
    interface IContentTypeLanguageService with
        member this.GetDefaultContentType() = 
            this.contentTypeRegistryService.GetContentType(FSharpCommonConstants.FSharpContentTypeName);
