namespace FSharp.Test

open System
open System.IO
open System.Threading
open System.Text
open Xunit
open Xunit.Sdk

module internal ParallelConsole =

    let inHolder = new AsyncLocal<TextReader voption>()
    let outHolder = new AsyncLocal<TextWriter voption>()
    let errorHolder = new AsyncLocal<TextWriter voption>()

    /// Redirects reads performed on different threads or async execution contexts to the relevant TextReader held by AsyncLocal.
    type RedirectingTextReader(holder: AsyncLocal<TextReader voption>) =
        inherit TextReader()

        let getValue() = holder.Value |> ValueOption.defaultValue TextReader.Null

        override _.Peek() = getValue().Peek()
        override _.Read() = getValue().Read()
        member _.Set (reader: TextReader) = holder.Value <- ValueSome reader

    /// Redirects writes performed on different threads or async execution contexts to the relevant TextWriter held by AsyncLocal.
    type RedirectingTextWriter(holder: AsyncLocal<TextWriter voption>) =
        inherit TextWriter()

        let getValue() = holder.Value |> ValueOption.defaultValue TextWriter.Null

        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = getValue().Write(value)
        override _.Write(value: string) = getValue().Write(value)
        override _.WriteLine(value: string) = getValue().WriteLine(value)
        member _.Value = getValue()
        member _.Set (writer: TextWriter) = holder.Value <- ValueSome writer

    let localIn = new RedirectingTextReader(inHolder)
    let localOut = new RedirectingTextWriter(outHolder)
    let localError = new RedirectingTextWriter(errorHolder)

    do
        Console.SetIn localIn
        Console.SetOut localOut
        Console.SetError localError

    let reset() =
        new StringWriter() |> localOut.Set
        new StringWriter() |> localError.Set

type ParallelConsole =
    static member OutText =
        Console.Out.Flush()
        string ParallelConsole.localOut.Value

    static member ErrorText =
        Console.Error.Flush()
        string ParallelConsole.localError.Value

// Must by applied on every test assembly that uses ParallelConsole or CompilerAssertHelpers. 
[<AttributeUsage(AttributeTargets.Assembly)>]
type InitTestGlobals() =
    inherit Xunit.Sdk.BeforeAfterTestAttribute()
    override _.Before (_methodUnderTest: Reflection.MethodInfo): unit =
        // Ensure console capture is installed.
        ParallelConsole.reset()
