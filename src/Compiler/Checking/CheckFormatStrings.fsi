// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Parse "printf-style" format specifiers at compile time, producing
/// a list of items that specify the types of the things that follow.
///
/// Must be updated if the Printf runtime component is updated.

module internal FSharp.Compiler.CheckFormatStrings

open FSharp.Compiler.NameResolution
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

/// A flexible type variable constrained to the integer types accepted by the '%d'/'%i'/'%u' specifiers.
val mkFlexibleIntFormatTypar: g: TcGlobals -> m: range -> TType

/// A flexible type variable constrained to 'decimal', as accepted by the '%M' specifier.
val mkFlexibleDecimalFormatTypar: g: TcGlobals -> m: range -> TType

val ParseFormatString:
    m: range ->
    fragmentRanges: range list ->
    g: TcGlobals ->
    isInterpolated: bool ->
    isFormattableString: bool ->
    formatStringCheckContext: FormatStringCheckContext option ->
    fmt: string ->
    printerArgTy: TType ->
    printerResidueTy: TType ->
    printerResultTy: TType ->
        TType list * TType * TType * TType[] * (range * int) list * string

val TryCountFormatStringArguments:
    m: range ->
    g: TcGlobals ->
    isInterpolated: bool ->
    fmt: string ->
    printerArgTy: TType ->
    printerResidueTy: TType ->
        int option
