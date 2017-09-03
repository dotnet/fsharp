// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting

open System.Diagnostics
open System
open System.IO
open System.Globalization
 
/// Provides a text writer that can indent new lines by a tabString token.
type internal IndentedTextWriter(writer: TextWriter, tabString: string) =
    inherit TextWriter (CultureInfo.InvariantCulture)
    let mutable indentLevel = 0
    let mutable tabsPending = false

    let outputTabs() =
        if tabsPending then
            for i = 0 to indentLevel - 1 do
                writer.Write(tabString)
            
            tabsPending <- false

    override __.Encoding = writer.Encoding
    override __.NewLine with get() = writer.NewLine and set value = writer.NewLine <- value
 
    /// Gets or sets the number of spaces to indent.
    member __.Indent with get() = indentLevel 
                      and set value =
                        Debug.Assert(value >= 0, "Bogus Indent... probably caused by mismatched Indent++ and Indent--")
                        indentLevel <- max 0 value
 
 /// Gets or sets the TextWriter to use.
    member __.InnerWriter = writer
    member __.TabString = tabString
 
    /// Closes the document being written to.
    override __.Close() = writer.Close()
    override __.Flush() = writer.Flush()
 
    /// Writes a string to the text stream.
    override __.Write(s: string) =
        outputTabs()
        writer.Write(s)
 
    /// Writes the text representation of a Boolean value to the text stream.
    override __.Write(value: bool) =
        outputTabs()
        writer.Write(value)
 
    /// Writes a character to the text stream.
    override __.Write(value: char) =
        outputTabs()
        writer.Write(value)
 
    /// Writes a character array to the text stream.
    override __.Write(buffer: char[]) =
        outputTabs()
        writer.Write(buffer)
 
    /// Writes a subarray of characters to the text stream.
    override __.Write(buffer: char[], index: int, count: int) =
        outputTabs()
        writer.Write(buffer, index, count)
 
    /// Writes the text representation of a Double to the text stream.
    override __.Write(value: float) =
        outputTabs()
        writer.Write(value)
 
    /// Writes the text representation of a Single to the text
    override __.Write(value: float32) =
        outputTabs()
        writer.Write(value)
    
    /// Writes the text representation of an integer to the text stream.
    override __.Write(value: int) =
        outputTabs()
        writer.Write(value)
 
    /// Writes the text representation of an 8-byte integer to the text stream.
    override __.Write(value: int64) =
        outputTabs()
        writer.Write(value)
 
    /// Writes the text representation of an object to the text stream.
    override __.Write(value: obj) =
        outputTabs()
        writer.Write(value)
 
    /// Writes out a formatted string, using the same semantics as specified.
    override __.Write(format: string, value: obj) =
        outputTabs()
        writer.Write(format, value)
 
    /// Writes out a formatted string, using the same semantics as specified.
    override __.Write(format: string, arg0: obj, arg1: obj) =
        outputTabs()
        writer.Write(format, arg0, arg1)
 
    /// Writes out a formatted string, using the same semantics as specified.
    override __.Write(format: string, [<ParamArray>] arg: obj[]) =
        outputTabs()
        writer.Write(format, arg)
 
    /// Writes the specified string to a line without tabs.
    member __.WriteLineNoTabs(s: string) = writer.WriteLine(s)
 
    /// Writes the specified string followed by a line terminator to the text stream.
    override __.WriteLine(s: string) =
        outputTabs()
        writer.WriteLine(s)
        tabsPending <- true
 
    /// Writes a line terminator.
    override __.WriteLine() =
        outputTabs()
        writer.WriteLine()
        tabsPending <- true
 
    /// Writes the text representation of a Boolean followed by a line terminator to the text stream.
    override __.WriteLine(value: bool) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(value: char) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(buffer: char[]) =
        outputTabs()
        writer.WriteLine(buffer)
        tabsPending <- true
 
    override __.WriteLine(buffer: char[], index: int, count: int) =
        outputTabs()
        writer.WriteLine(buffer, index, count)
        tabsPending <- true
 
    override __.WriteLine(value: float) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(value: float32) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(value: int) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(value: int64) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(value: obj) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    override __.WriteLine(format: string, arg0: obj) =
        outputTabs()
        writer.WriteLine(format, arg0)
        tabsPending <- true
 
    override __.WriteLine(format: string, arg0: obj, arg1: obj) =
        outputTabs()
        writer.WriteLine(format, arg0, arg1)
        tabsPending <- true
 
    override __.WriteLine(format: string, [<ParamArray>] arg: obj[]) =
        outputTabs()
        writer.WriteLine(format, arg)
        tabsPending <- true
 
    [<CLSCompliant(false)>]
    override __.WriteLine(value: UInt32) =
        outputTabs()
        writer.WriteLine(value)
        tabsPending <- true
 
    member internal __.InternalOutputTabs() =
        for i = 0 to indentLevel - 1 do
            writer.Write(tabString)