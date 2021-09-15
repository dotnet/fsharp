// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>This namespace contains types and modules for generating and formatting text with F#</summary>
namespace Microsoft.FSharp.Core

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open System
open System.IO
open System.Text

/// <summary>Type of a formatting expression.</summary>
///
/// <typeparam name="Printer">Function type generated by printf.</typeparam>
/// <typeparam name="State">Type argument passed to %a formatters</typeparam>
/// <typeparam name="Residue">Value generated by the overall printf action (e.g. sprint generates a string)</typeparam>
/// <typeparam name="Result">Value generated after post processing (e.g. failwithf generates a string internally then raises an exception)</typeparam>
///
/// <category>Language Primitives</category>
type PrintfFormat<'Printer,'State,'Residue,'Result> =

    /// <summary>Construct a format string </summary>
    /// <param name="value">The input string.</param>
    ///
    /// <returns>The PrintfFormat containing the formatted result.</returns>
    /// 
    /// <example-tbd></example-tbd>
    new : value:string -> PrintfFormat<'Printer,'State,'Residue,'Result>

    /// <summary>Construct a format string </summary>
    /// <param name="value">The input string.</param>
    /// <param name="captures">The captured expressions in an interpolated string.</param>
    /// <param name="captureTys">The types of expressions for %A holes in interpolated string.</param>
    /// <returns>The PrintfFormat containing the formatted result.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    new : value:string * captures: obj[] * captureTys: Type[] -> PrintfFormat<'Printer,'State,'Residue,'Result>

    /// <summary>The raw text of the format string.</summary>
    /// 
    /// <example-tbd></example-tbd>
    member Value : string

    /// <summary>The captures associated with an interpolated string.</summary>
    /// 
    /// <example-tbd></example-tbd>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member Captures: obj[]
    
    /// <summary>The capture types associated with an interpolated string.</summary>
    /// 
    /// <example-tbd></example-tbd>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member CaptureTypes: System.Type[]
    
/// <summary>Type of a formatting expression.</summary>
///
/// <typeparam name="Printer">Function type generated by printf.</typeparam>
/// <typeparam name="State">Type argument passed to %a formatters</typeparam>
/// <typeparam name="Residue">Value generated by the overall printf action (e.g. sprint generates a string)</typeparam>
/// <typeparam name="Result">Value generated after post processing (e.g. failwithf generates a string internally then raises an exception)</typeparam>
/// <typeparam name="Tuple">Tuple of values generated by scan or match.</typeparam>
///
/// <category>Language Primitives</category>
/// 
/// <example-tbd></example-tbd>
type PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple> = 

    inherit PrintfFormat<'Printer,'State,'Residue,'Result>

    /// <summary>Construct a format string</summary>
    ///
    /// <param name="value">The input string.</param>
    ///
    /// <returns>The created format string.</returns>
    /// 
    /// <example-tbd></example-tbd>
    new: value:string -> PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>

    /// <summary>Construct a format string</summary>
    /// 
    /// <param name="value">The input string.</param>
    /// <param name="captures">The captured expressions in an interpolated string.</param>
    /// <param name="captureTys">The types of expressions for %A holes in interpolated string.</param>
    /// 
    /// <returns>The created format string.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    new: value:string * captures: obj[] * captureTys: Type[] -> PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>

/// <summary>Type of a formatting expression.</summary>
/// <typeparam name="Printer">Function type generated by printf.</typeparam>
/// <typeparam name="State">Type argument passed to %a formatters</typeparam>
/// <typeparam name="Residue">Value generated by the overall printf action (e.g. sprint generates a string)</typeparam>
/// <typeparam name="Result">Value generated after post processing (e.g. failwithf generates a string internally then raises an exception)</typeparam>
///
/// <category>Language Primitives</category>
type Format<'Printer,'State,'Residue,'Result> = PrintfFormat<'Printer,'State,'Residue,'Result>

/// <summary>Type of a formatting expression.</summary>
/// <typeparam name="Printer">Function type generated by printf.</typeparam>
/// <typeparam name="State">Type argument passed to %a formatters</typeparam>
/// <typeparam name="Residue">Value generated by the overall printf action (e.g. sprint generates a string)</typeparam>
/// <typeparam name="Result">Value generated after post processing (e.g. failwithf generates a string internally then raises an exception)</typeparam>
/// <typeparam name="Tuple">Tuple of values generated by scan or match.</typeparam>
///
/// <category>Language Primitives</category>
type Format<'Printer,'State,'Residue,'Result,'Tuple> = PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>

/// <summary>Extensible printf-style formatting for numbers and other datatypes</summary>
///
/// <remarks>Format specifications are strings with "%" markers indicating format 
/// placeholders. Format placeholders consist of <c>%[flags][width][.precision][type]</c>.</remarks>
///
/// <category index="4">Strings and Text</category>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Printf = 

    /// <summary>Represents a statically-analyzed format associated with writing to a <see cref="T:System.Text.StringBuilder"/>. The first type parameter indicates the
    /// arguments of the format operation and the last the overall return type.</summary>
    type BuilderFormat<'T,'Result> = Format<'T, StringBuilder, unit, 'Result>

    /// <summary>Represents a statically-analyzed format when formatting builds a string. The first type parameter indicates the
    /// arguments of the format operation and the last the overall return type.</summary>
    type StringFormat<'T,'Result> = Format<'T, unit, string, 'Result>

    /// <summary>Represents a statically-analyzed format associated with writing to a <see cref="T:System.IO.TextWriter"/>. The first type parameter indicates the
    /// arguments of the format operation and the last the overall return type.</summary>
    type TextWriterFormat<'T,'Result> = Format<'T, TextWriter, unit, 'Result>

    /// <summary>Represents a statically-analyzed format associated with writing to a <see cref="T:System.Text.StringBuilder"/>. The type parameter indicates the
    /// arguments and return type of the format operation.</summary>
    type BuilderFormat<'T> = BuilderFormat<'T, unit>

    /// <summary>Represents a statically-analyzed format when formatting builds a string. The type parameter indicates the
    /// arguments and return type of the format operation.</summary>
    type StringFormat<'T> = StringFormat<'T,string>

    /// <summary>Represents a statically-analyzed format associated with writing to a <see cref="T:System.IO.TextWriter"/>. The type parameter indicates the
    /// arguments and return type of the format operation.</summary>
    type TextWriterFormat<'T> = TextWriterFormat<'T,unit>

    /// <summary>Print to a <see cref="T:System.Text.StringBuilder"/></summary>
    ///
    /// <param name="builder">The StringBuilder to print to.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToStringBuilder")>]
    val bprintf: builder:StringBuilder -> format:BuilderFormat<'T> -> 'T

    /// <summary>Print to a text writer.</summary>
    ///
    /// <param name="textWriter">The TextWriter to print to.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToTextWriter")>]
    val fprintf: textWriter:TextWriter -> format:TextWriterFormat<'T> -> 'T

    /// <summary>Print to a text writer, adding a newline</summary>
    ///
    /// <param name="textWriter">The TextWriter to print to.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatLineToTextWriter")>]
    val fprintfn: textWriter:TextWriter -> format:TextWriterFormat<'T> -> 'T

    /// <summary>Formatted printing to stderr</summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToError")>]
    val eprintf: format:TextWriterFormat<'T> -> 'T

    /// <summary>Formatted printing to stderr, adding a newline </summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatLineToError")>]
    val eprintfn: format:TextWriterFormat<'T> -> 'T

    /// <summary>Formatted printing to stdout</summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormat")>]
    val printf: format:TextWriterFormat<'T> -> 'T

    /// <summary>Formatted printing to stdout, adding a newline.</summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The return type and arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatLine")>]
    val printfn: format:TextWriterFormat<'T> -> 'T

    /// <summary>Print to a string via an internal string buffer and return 
    /// the result as a string. Helper printers must return strings.</summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The formatted string.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToStringThen")>]
    val sprintf: format:StringFormat<'T> -> 'T

    /// <summary>bprintf, but call the given 'final' function to generate the result.
    /// See <c>kprintf</c>.</summary>
    ///
    /// <param name="continuation">The function called after formatting to generate the format result.</param>
    /// <param name="builder">The input StringBuilder.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToStringBuilderThen")>]
    val kbprintf: continuation:(unit -> 'Result) -> builder:StringBuilder -> format:BuilderFormat<'T,'Result> -> 'T

    /// <summary>fprintf, but call the given 'final' function to generate the result.
    /// See <c>kprintf</c>.</summary>
    ///
    /// <param name="continuation">The function called after formatting to generate the format result.</param>
    /// <param name="textWriter">The input TextWriter.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToTextWriterThen")>]
    val kfprintf: continuation:(unit -> 'Result) -> textWriter:TextWriter -> format:TextWriterFormat<'T,'Result> -> 'T

    /// <summary>printf, but call the given 'final' function to generate the result.
    /// For example, these let the printing force a flush after all output has 
    /// been entered onto the channel, but not before. </summary>
    ///
    /// <param name="continuation">The function called after formatting to generate the format result.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatThen")>]
    val kprintf: continuation:(string -> 'Result) -> format:StringFormat<'T,'Result> -> 'T

    /// <summary>sprintf, but call the given 'final' function to generate the result.
    /// See <c>kprintf</c>.</summary>
    ///
    /// <param name="continuation">The function called to generate a result from the formatted string.</param>
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToStringThen")>]
    val ksprintf: continuation:(string -> 'Result) -> format:StringFormat<'T,'Result>  -> 'T

    /// <summary>Print to a string buffer and raise an exception with the given
    /// result. Helper printers must return strings.</summary>
    ///
    /// <param name="format">The input formatter.</param>
    ///
    /// <returns>The arguments of the formatter.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PrintFormatToStringThenFail")>]
    val failwithf: format:StringFormat<'T,'Result> -> 'T
