// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.Ast
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Range

module public UnusedOpens =
    /// Get all unused open declarations in a file
    val getUnusedOpens : checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) -> Async<range list>

module public SimplifyNames = 
    /// Get all ranges that can be simplified in a file
    val getUnnecessaryRanges : checkFileResults: FSharpCheckFileResults * getSourceLineStr: (int -> string) * sleep: int option -> Async<(range*string) list>

module public UnusedDeclarations = 
    /// Get all unused declarations in a file
    val getUnusedDeclarations : checkFileResults: FSharpCheckFileResults * isScriptFile: bool -> Async<range list>