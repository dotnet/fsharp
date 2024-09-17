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

[<CollectionDefinition(nameof DoNotRunInParallel, DisableParallelization = true)>]
type DoNotRunInParallel = class end

// Must by applied on every test assembly that uses ParallelConsole or CompilerAssertHelpers. 
[<AttributeUsage(AttributeTargets.Assembly)>]
type InitTestGlobals() =
    inherit Xunit.Sdk.BeforeAfterTestAttribute()
    override _.Before (_methodUnderTest: Reflection.MethodInfo): unit =
        // ensure static context is initialized
        ParallelConsole.Initialized |> ignore
        CompilerAssertHelpers.Initialized |> ignore
