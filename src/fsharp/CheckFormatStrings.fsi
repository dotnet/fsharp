// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Parse "printf-style" format specifiers at compile time, producing
/// a list of items that specify the types of the things that follow.
///
/// Must be updated if the Printf runtime component is updated.

module internal FSharp.Compiler.CheckFormatStrings

open FSharp.Compiler 
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTree 
open FSharp.Compiler.TcGlobals

val ParseFormatString : Range.range -> TcGlobals -> formatStringCheckContext: FormatStringCheckContext option -> fmt: string -> bty: TType -> cty: TType -> dty: TType -> (TType * TType) * (Range.range * int) list

val TryCountFormatStringArguments : m:Range.range -> g:TcGlobals -> fmt:string -> bty:TType -> cty:TType -> int option
