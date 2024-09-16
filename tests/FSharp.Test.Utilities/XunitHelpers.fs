namespace FSharp.Test

open System
open System.IO
open System.Threading
open System.Text
open Xunit
open Xunit.Sdk
open MessageSink

module MessageSink =
    type SinkWriter(sink: Abstractions.IMessageSink) =
        inherit StringWriter()
        member this.Send() =
            let sb = this.GetStringBuilder()
            sink.OnMessage(DiagnosticMessage(string sb)) |> ignore
            sb.Clear() |> ignore

        override _.Encoding = Encoding.UTF8

        // This depends on fprintfn implementation calling it
        // but should be good enough and fast.
        override this.WriteLine (): unit = this.Send()

    let installSink sink = sinkWriter <- new SinkWriter(sink)

module ParallelConsole =

    let private inHolder = new AsyncLocal<TextReader voption>()
    let private outHolder = new AsyncLocal<TextWriter voption>()
    let private errorHolder = new AsyncLocal<TextWriter voption>()

    /// Redirects reads performed on different threads or async execution contexts to the relevant TextReader held by AsyncLocal.
    type RedirectingTextReader(holder: AsyncLocal<TextReader voption>, defaultReader) =
        inherit TextReader()

        let getValue() = holder.Value |> ValueOption.defaultValue defaultReader

        override _.Peek() = getValue().Peek()
        override _.Read() = getValue().Read()
        member _.Set (reader: TextReader) = holder.Value <- ValueSome reader
        member _.Drop() = holder.Value <- ValueNone

    /// Redirects writes performed on different threads or async execution contexts to the relevant TextWriter held by AsyncLocal.
    type RedirectingTextWriter(holder: AsyncLocal<TextWriter voption>, defaultWriter) =
        inherit TextWriter()

        let getValue() = holder.Value |> ValueOption.defaultValue defaultWriter

        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = getValue().Write(value)
        override _.Write(value: string) = getValue().Write(value)
        override _.WriteLine(value: string) = getValue().WriteLine(value)
        member _.Value = getValue()
        member _.Set (writer: TextWriter) = holder.Value <- ValueSome writer
        member _.Drop() = holder.Value <- ValueNone

    let private localIn = new RedirectingTextReader(inHolder, TextReader.Null)
    let private localOut = new RedirectingTextWriter(outHolder, TextWriter.Null)
    let private localError = new RedirectingTextWriter(errorHolder, TextWriter.Null)

    let installParallelRedirections() =
        Console.SetIn localIn
        Console.SetOut localOut
        Console.SetError localError

    type Caputure(?input, ?error: TextWriter, ?output: TextWriter) =
        do
            input |> Option.iter localIn.Set
            defaultArg output (new StringWriter()) |> localOut.Set
            defaultArg error (new StringWriter()) |> localError.Set

        member _.OutText = string localOut.Value
        member _.ErrorText = string localError.Value

        interface IDisposable with
            member _.Dispose () =
                localIn.Drop()
                localOut.Drop()
                localError.Drop()

type ParallelConsoleTestFramework(sink) =
    inherit XunitTestFramework(sink)
    do
        // MessageSink.installSink sink
        ParallelConsole.installParallelRedirections()