// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

[<Sealed>]
type NavigableTextRun(classificationTypeName:string, text:string, navigateAction:unit -> unit) =
    member __.ClassificationTypeName = classificationTypeName
    member __.Text = text
    member __.NavigateAction = navigateAction
