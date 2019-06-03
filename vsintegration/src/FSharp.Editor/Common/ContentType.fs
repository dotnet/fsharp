// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.CodeAnalysis.Editor
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor

module FSharpStaticTypeDefinitions = 
    [<Export>]
    [<Name(FSharpConstants.FSharpContentTypeName)>]
    [<BaseDefinition(ContentTypeNames.RoslynContentType)>]
    let FSharpContentTypeDefinition = ContentTypeDefinition()

    [<Export>]
    [<Name(FSharpConstants.FSharpSignatureHelpContentTypeName)>]
    [<BaseDefinition("sighelp")>]
    let FSharpSignatureHelpContentTypeDefinition = ContentTypeDefinition()
