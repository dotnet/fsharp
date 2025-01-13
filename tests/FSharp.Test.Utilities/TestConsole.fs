namespace FSharp.Test

open System
open System.IO
open System.Text
open System.Threading

module TestConsole =

    /// Redirects reads performed on different async execution contexts to the relevant TextReader held by AsyncLocal.
    type private RedirectingTextReader() =
        inherit TextReader()
        let holder = AsyncLocal<_>()
        do holder.Value <- TextReader.Null

        override _.Peek() = holder.Value.Peek()
        override _.Read() = holder.Value.Read()
        member _.Value = holder.Value
        member _.Set (reader: TextReader) = holder.Value <- reader

    /// Redirects writes performed on different async execution contexts to the relevant TextWriter held by AsyncLocal.
    type private RedirectingTextWriter() =
        inherit TextWriter()
        let holder = AsyncLocal<TextWriter>()
        do holder.Value <- TextWriter.Null

        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = holder.Value.Write(value)
        member _.Value = holder.Value
        member _.Set (writer: TextWriter) = holder.Value <- writer

    let private localIn = new RedirectingTextReader()
    let private localOut = new RedirectingTextWriter()
    let private localError = new RedirectingTextWriter()

    let install () = 
        Console.SetIn localIn
        Console.SetOut localOut
        Console.SetError localError
    
    // Taps into the redirected console stream.
    type private CapturingWriter(redirecting: RedirectingTextWriter) as this =
        inherit StringWriter()
        let wrapped = redirecting.Value
        do redirecting.Set this
        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = wrapped.Write(value); base.Write(value)
        override _.Dispose (disposing: bool) =
            redirecting.Set wrapped
            base.Dispose(disposing: bool)

    /// Captures console streams for the current async execution context.
    /// Each simultaneously executing test case runs in a separate async execution context.
    ///
    /// Can be used to capture just a single compilation or eval as well as the whole test case execution output.
    type ExecutionCapture() =
        do
            Console.Out.Flush()
            Console.Error.Flush()

        let output = new CapturingWriter(localOut)
        let error = new CapturingWriter(localError)

        member _.Dispose() =
            output.Dispose()
            error.Dispose()

        interface IDisposable with
            member this.Dispose (): unit = this.Dispose()

        member _.OutText =
            Console.Out.Flush()
            string output

        member _.ErrorText =
            Console.Error.Flush()
            string error

    type ProvideInput(input: string) =
        let oldIn = localIn.Value
        do
            new StringReader(input) |> localIn.Set

        interface IDisposable with
            member this.Dispose (): unit = localIn.Set oldIn
