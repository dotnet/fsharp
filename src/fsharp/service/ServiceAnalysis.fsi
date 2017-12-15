// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.Range

module public UnusedOpens =
    /// Get all unused open declarations in a file
    val getUnusedOpens : checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) -> Async<range list>