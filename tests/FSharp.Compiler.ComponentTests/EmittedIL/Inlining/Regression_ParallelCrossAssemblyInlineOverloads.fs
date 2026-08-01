namespace EmittedIL.Inlining

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_ParallelCrossAssemblyInlineOverloads =

    [<Fact>]
    let ``Cross-assembly overloaded inline Source members compile and run`` () =
       let library =
           (
               FSharp """
module LibraryImpl

open System.Threading.Tasks
open Microsoft.FSharp.Control

module Result =
   let ofChoice choice =
       match choice with
       | Choice1Of2 value -> Ok value
       | Choice2Of2 error -> Error error

module Async =
   let singleton value = async.Return value

type Validation<'ok, 'error> = Result<'ok, 'error>
type TaskValidation<'ok, 'error> = Task<Validation<'ok, 'error>>

type TaskValidationBuilderBase() =
   member inline _.Return(value: 'ok) : TaskValidation<'ok, 'error> =
       task { return Ok value }

   member inline _.ReturnFrom(taskValidation: TaskValidation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       taskValidation

   member inline _.Bind
       (source: TaskValidation<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline this.Bind
       (source: Validation<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline this.Bind
       (source: Choice<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline this.Bind
       (source: Async<'okInput>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline _.Delay(generator: unit -> TaskValidation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       generator ()

   member inline this.Source(result: Validation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       task { return result }

   member inline this.Source(choice: Choice<'ok, 'error>) : TaskValidation<'ok, 'error> =
       task {
           return
               choice
               |> Result.ofChoice
       }

   member inline this.Source(asyncComputation: Async<'ok>) : TaskValidation<'ok, 'error> =
       task {
           let! value = asyncComputation
           return Ok value
       }

type TaskValidationBuilder() =
   inherit TaskValidationBuilderBase()

let taskValidation = TaskValidationBuilder()
"""
               |> withAdditionalSourceFile (SourceCodeFileKind.Create("Library.Support.fs", """
module LibraryImplSupport

let taskValidation = LibraryImpl.taskValidation
"""))
               |> withOutputType CompileOutput.Library
               |> withName "Library"
               |> withOptimize
               |> withOptions ["--parallelcompilation+"; "--nowarn:75"]
               |> ignoreWarnings
           )

       let consumerSource =
           "module ConsumerImpl\n\nopen LibraryImpl\nopen LibraryImplSupport\n\nlet run () =\n    taskValidation.Bind(\n        Async.singleton 42,\n        fun asyncValue ->\n            taskValidation.Bind(\n                Ok 42,\n                fun resultValue ->\n                    taskValidation.Bind(\n                        Choice1Of2 42,\n                        fun choiceValue ->\n                            taskValidation.Return(asyncValue + resultValue + choiceValue))))\n"

       let consumerAdditionalSources =
           Array.init 12 (fun i ->
               let source =
                   "module Consumer"
                   + string i
                   + "\n\nopen LibraryImpl\nopen LibraryImplSupport\n\nlet run () =\n    taskValidation.Bind(\n        Async.singleton 42,\n        fun asyncValue ->\n            taskValidation.Bind(\n                Ok 42,\n                fun resultValue ->\n                    taskValidation.Bind(\n                        Choice1Of2 42,\n                        fun choiceValue ->\n                            taskValidation.Return(asyncValue + resultValue + choiceValue))))\n"

               SourceCodeFileKind.Create(sprintf "Consumer.%d.fs" i, source))
           |> Array.toList

       let consumer =
           (
               FSharp consumerSource
               |> withAdditionalSourceFiles consumerAdditionalSources
               |> withAdditionalSourceFile (SourceCodeFileKind.Create("Consumer.Support.fs", """
module ConsumerSupport

[<EntryPoint>]
let main _ =
   match (ConsumerImpl.run ()).GetAwaiter().GetResult() with
   | Ok value -> if value = 126 then 0 else 1
   | Error _ -> 1
"""))
               |> withOutputType CompileOutput.Exe
               |> withReferences [ library ]
               |> withOptimize
               |> withOptions ["--parallelcompilation+"; "--nowarn:75"]
               |> ignoreWarnings
           )

       for _i = 1 to 30 do
           consumer
           |> compile
           |> shouldSucceed
           |> ignore
