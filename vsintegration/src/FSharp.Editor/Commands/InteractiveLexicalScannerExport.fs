// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.VisualStudio.FSharp.Interactive

[<Export(typeof<ILexicalScannerFactory>)>]
type internal FSharpLexicalScannerFactoryExport() =
    inherit FSharpLexicalScannerFactory()
