// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Parse "printf-style" format specifiers at compile time, producing
/// a list of items that specify the types of the things that follow.
///
/// Must be updated if the Printf runtime component is updated.

module internal Microsoft.FSharp.Compiler.CheckFormatStrings

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.Tast 
open Microsoft.FSharp.Compiler.TcGlobals

val ParseFormatString : Range.range -> TcGlobals -> formatStringCheckContext: FormatStringCheckContext option -> fmt: string -> bty: TType -> cty: TType -> dty: TType -> (TType * TType) * (Range.range * int) list

val TryCountFormatStringArguments : m:Range.range -> g:TcGlobals -> fmt:string -> bty:TType -> cty:TType -> int option
