namespace FSharp.Test

open System
open System.IO
open System.Text
open System.Threading

open Xunit.Sdk

module internal ParallelConsole =
    /// Redirects reads performed on different threads or async execution contexts to the relevant TextReader held by AsyncLocal.
    type RedirectingTextReader(initial: TextReader) =
        inherit TextReader()
        let holder = AsyncLocal<_>()
        do holder.Value <- initial

        override _.Peek() = holder.Value.Peek()
        override _.Read() = holder.Value.Read()
        member _.Set (reader: TextReader) = holder.Value <- reader

    /// Redirects writes performed on different threads or async execution contexts to the relevant TextWriter held by AsyncLocal.
    type RedirectingTextWriter(initial: TextWriter) =
        inherit TextWriter()
        let holder = AsyncLocal<_>()
        do holder.Value <- initial

        override _.Encoding = Encoding.UTF8
        override _.Write(value: char) = holder.Value.Write(value)
        override _.Write(value: string) = holder.Value.Write(value)
        override _.WriteLine(value: string) = holder.Value.WriteLine(value)
        member _.Value = holder.Value
        member _.Set (writer: TextWriter) = holder.Value <- writer

    let localIn = new RedirectingTextReader(TextReader.Null)
    let localOut = new RedirectingTextWriter(TextWriter.Null)
    let localError = new RedirectingTextWriter(TextWriter.Null)

    let initStreamsCapture () = 
        Console.SetIn localIn
        Console.SetOut localOut
        Console.SetError localError

    let resetWriters() =
        new StringWriter() |> localOut.Set
        new StringWriter() |> localError.Set

type ParallelConsole =
    static member OutText =
        Console.Out.Flush()
        string ParallelConsole.localOut.Value

    static member ErrorText =
        Console.Error.Flush()
        string ParallelConsole.localError.Value

[<AttributeUsage(AttributeTargets.Assembly)>]
type ResetConsoleWriters() =
    inherit BeforeAfterTestAttribute()
    override _.Before (_methodUnderTest: Reflection.MethodInfo): unit =
        // Ensure fresh empty writers before each individual test.
        ParallelConsole.resetWriters()

type TestRun(sink) =
    inherit XunitTestFramework(sink)
    do
        MessageSink.sinkWriter |> ignore
        ParallelConsole.initStreamsCapture()

    static let testRunFinishedEvent = Event<unit>()
    static member Finished = testRunFinishedEvent.Publish
    interface IDisposable with
        member _.Dispose() =
            // Try to release files locked by AssemblyLoadContext, AppDomains etc.
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            testRunFinishedEvent.Trigger()
            base.Dispose()
