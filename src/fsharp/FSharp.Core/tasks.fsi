namespace Mirosoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the state of a computation: either awaiting something with a continuation, or completed with a return value.
[<Sealed; NoComparison; NoEquality>]
type TaskStep<'T>

module TaskHelpers = 

    val unwrapException: agg: AggregateException -> exn
    
    val zero: TaskStep<unit>
    
    val ret: x: 'T -> TaskStep<'T>

    val bindTask: task: Task<'T> -> continuation: ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>

    val bindTaskConfigureFalse: task: Task<'T> -> continuation: ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>

    val combine: step: TaskStep<unit> -> continuation: (unit -> TaskStep<'TResult>) -> TaskStep<'TResult>

    val whileLoop: cond: (unit -> bool) -> body: (unit -> TaskStep<unit>) -> TaskStep<unit>

    val tryWith: step: (unit -> TaskStep<'T>) -> catch: (exn -> TaskStep<'T>) -> TaskStep<'T>

    val tryFinally: step: (unit -> TaskStep<'T>) -> fin: (unit -> unit) -> TaskStep<'T>

    val using: disp: 'Resource -> body: ('Resource -> TaskStep<'T>) -> TaskStep<'T> when 'Resource :> IDisposable

    val forLoop: sequence: seq<'T> -> body: ('T -> TaskStep<unit>) -> TaskStep<unit>

    val run: firstStep: (unit -> TaskStep<'T>) -> Task<'T>

    type Priority3 = obj

    type Priority2 = IComparable

    [<Sealed; NoComparison; NoEquality>]
    type BindSensitive =
        //static member inline ( >>= ): Priority2 * taskLike: ^TaskLike -> (( ^TResult -> TaskStep<'TResult>) -> TaskStep<'TResult>)
        //              when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
        //              and ^Awaiter :> ICriticalNotifyCompletion
        //              and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
        //              and ^Awaiter: (member GetResult:  ^Awaiter ->  ^TResult)
        static member ( >>= ): Priority1: BindSensitive * task: Task<'T> -> (('T -> TaskStep<'TResult>) -> TaskStep<'TResult>)
        //static member ( >>= ): Priority1: BindSensitive * computation: Async<'T1> -> (('T1 -> TaskStep<'TResult1>) -> TaskStep<'TResult1>)

    [<Sealed; NoComparison; NoEquality>]
    type ReturnFromSensitive =
        static member inline ( $ ): Priority1: ReturnFromSensitive * taskLike: ^TaskLike -> TaskStep< ^T >
                    when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
                    and ^Awaiter :> ICriticalNotifyCompletion
                    and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
                    and ^Awaiter: (member GetResult:  ^Awaiter ->  ^T)
        //static member ( $ ): Priority1: ReturnFromSensitive * computation: Async<'T> -> TaskStep<'T>

    [<Sealed; NoComparison; NoEquality>]
    type BindInsensitive =
        //static member inline ( >>= ): Priority3 * taskLike: ^TaskLike -> (( ^TResult -> TaskStep<'TResult>) -> TaskStep<'TResult>)
        //    when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
        //    and ^Awaiter :> ICriticalNotifyCompletion
        //    and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
        //    and ^Awaiter: (member GetResult:  ^Awaiter ->  ^TResult)

        //static member inline ( >>= ): Priority2 * configurableTaskLike: ^TaskLike -> (( ^TResult -> TaskStep<'TResult>) -> TaskStep<'TResult>)
        //    when  ^TaskLike: (member ConfigureAwait:  ^TaskLike * bool ->  ^Awaitable)
        //    and ^Awaitable: (member GetAwaiter:  ^Awaitable ->  ^Awaiter)
        //    and ^Awaiter :> ICriticalNotifyCompletion
        //    and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
        //    and ^Awaiter: (member GetResult:  ^Awaiter ->  ^TResult)

        static member ( >>= ): Priority1: BindInsensitive * task: Task<'T> -> (('T -> TaskStep<'TResult>) -> TaskStep<'TResult>)
        //static member ( >>= ): Priority1: BindInsensitive * computation: Async<'T> -> (('T -> TaskStep<'TResult>) -> TaskStep<'TResult>)

    [<Sealed; NoComparison; NoEquality>]
    type ReturnFromInsensitive =
        static member inline ( $ ): Priority2 * taskLike: ^TaskLike -> TaskStep< ^T>
                    when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
                    and ^Awaiter :> ICriticalNotifyCompletion
                    and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
                    and ^Awaiter: (member GetResult:  ^Awaiter ->  ^T)

        //static member inline ( $ ): Priority1: ReturnFromInsensitive * configurableTaskLike: ^TaskLike -> TaskStep< ^T>
        //            when  ^TaskLike: (member ConfigureAwait:  ^TaskLike * bool ->  ^Awaitable)
        //            and ^Awaitable: (member GetAwaiter:  ^Awaitable ->  ^Awaiter)
        //            and ^Awaiter :> ICriticalNotifyCompletion
        //            and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
        //            and ^Awaiter: (member GetResult:  ^Awaiter ->  ^T)

        //static member ( $ ): Priority1: ReturnFromInsensitive * computation: Async<'T> -> TaskStep<'T>


type TaskBuilder =
    new: unit -> TaskBuilder
    member Combine: step: TaskStep<unit> * continuation: (unit -> TaskStep<'T>) -> TaskStep<'T>
    member Delay: f: (unit -> TaskStep<'T>) -> (unit -> TaskStep<'T>)
    member For: sequence: seq<'T> * body: ('T -> TaskStep<unit>) -> TaskStep<unit>
    member Return: x: 'c -> TaskStep<'c>
    member ReturnFrom: task: Task<'T> -> TaskStep<'T>
    member Run: f: (unit -> TaskStep<'T>) -> Task<'T>
    member TryFinally: body: (unit -> TaskStep<'T>) * fin: (unit -> unit) -> TaskStep<'T>
    member TryWith: body: (unit -> TaskStep<'T>) * catch: (exn -> TaskStep<'T>) -> TaskStep<'T>
    member Using: disp: 'Resource * body: ('Resource -> TaskStep<'T>) -> TaskStep<'T> when 'Resource :> IDisposable
    member While: condition: (unit -> bool) * body: (unit -> TaskStep<unit>) -> TaskStep<unit>
    member Zero: unit -> TaskStep<unit>

[<AutoOpen>]
module ContextSensitiveTasks = 
    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method.
    /// Use this like `task { let! taskResult = someTask(); return taskResult.ToString(); }`.
    val task: TaskBuilder

    type TaskBuilder with
      member inline Bind: task: ^TaskLike * continuation: ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>
          when (TaskHelpers.BindSensitive or  ^TaskLike): (static member ( >>= ): TaskHelpers.BindSensitive * ^TaskLike -> ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>)

      member inline ReturnFrom: a: ^TaskLike -> TaskStep< ^TResult >
          //when (TaskHelpers.ReturnFromSensitive or  ^TaskLike): (static member ( $ ): TaskHelpers.ReturnFromSensitive * ^TaskLike -> TaskStep<'TResult>)
                    when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
                    and ^Awaiter :> ICriticalNotifyCompletion
                    and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
                    and ^Awaiter: (member GetResult:  ^Awaiter ->  ^TResult)

module ContextInsensitiveTasks = 
    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method, but with
    /// all awaited tasks automatically configured *not* to resume on the captured context.
    /// This is often preferable when writing library code that is not context-aware, but undesirable when writing
    /// e.g. code that must interact with user interface controls on the same thread as its caller.
    val task: TaskBuilder

    type TaskBuilder with
      member inline Bind: task: ^TaskLike * continuation: ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>
          when (TaskHelpers.BindInsensitive or  ^TaskLike): (static member ( >>= ): TaskHelpers.BindInsensitive * ^TaskLike -> ('T -> TaskStep<'TResult>) -> TaskStep<'TResult>)

//      member inline ReturnFrom: a: ^TaskLike -> TaskStep<'TResult>
//          when (TaskHelpers.ReturnFromInsensitive or  ^TaskLike): (static member ( $ ): TaskHelpers.ReturnFromInsensitive * ^TaskLike -> TaskStep<'TResult>)
