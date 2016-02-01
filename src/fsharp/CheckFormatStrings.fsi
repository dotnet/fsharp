// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Parse "printf-style" format specifiers at compile time, producing
/// a list of items that specify the types of the things that follow.
///
/// Must be updated if the Printf runtime component is updated.

module internal Microsoft.FSharp.Compiler.CheckFormatStrings

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Tast 
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.AbstractIL.Internal 

type FormatStringFragment = 
    | Text of string
    | Expr of string
 
type FormatString = FormatStringFragment list
 
val ParseFormatString : Range.range -> TcGlobals -> source: string option -> fmt: string -> bty: TType -> cty: TType -> dty: TType -> FormatString * (TType * TType) * Range.range list

val TryCountFormatStringArguments : m:Range.range -> g:TcGlobals -> fmt:string -> bty:TType -> cty:TType -> int option
