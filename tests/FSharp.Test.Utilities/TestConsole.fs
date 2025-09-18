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
        member _.Reader
            with get() = holder.Value |> ValueOption.defaultValue TextReader.Null
            and set v = holder.Value <- ValueSome v
        override this.Peek() = this.Reader.Peek()
        override this.Read() = this.Reader.Read()

    /// Redirects writes performed on different async execution contexts to the relevant TextWriter held by AsyncLocal.
    type private RedirectingTextWriter() =
        inherit TextWriter()
        let holder = AsyncLocal<_>()      
        member _.Writer
            with get() = holder.Value |> ValueOption.defaultValue TextWriter.Null
            and set v = holder.Value <- ValueSome v

        override _.Encoding = Encoding.UTF8
        override this.Write(value: char) = this.Writer.Write(value)

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
        let wrapped = redirecting.Writer
        do redirecting.Writer <- this
        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = wrapped.Write(value); base.Write(value)
        override _.Dispose (disposing: bool) =
            redirecting.Writer <- wrapped
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
        let oldIn = localIn.Reader
        do
            localIn.Reader <- new StringReader(input)

        interface IDisposable with
            member this.Dispose (): unit = localIn.Reader <- oldIn
