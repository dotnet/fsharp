[<AutoOpen>]
module Microsoft.VisualStudio.FSharp.Editor.Pervasive

open System
open System.IO
open System.Diagnostics

/// Checks if the filePath ends with ".fsi"
let isSignatureFile (filePath:string) = 
    String.Equals (Path.GetExtension filePath, ".fsi", StringComparison.OrdinalIgnoreCase)

/// Returns the corresponding signature file path for given implementation file path or vice versa
let getOtherFile (filePath: string) =
    if isSignatureFile filePath then
        Path.ChangeExtension(filePath, ".fs")
    else
        Path.ChangeExtension(filePath, ".fsi")

/// Checks if the file paht ends with '.fsx' or '.fsscript'
let isScriptFile (filePath:string) = 
    let ext = Path.GetExtension filePath 
    String.Equals (ext, ".fsx", StringComparison.OrdinalIgnoreCase) || String.Equals (ext, ".fsscript", StringComparison.OrdinalIgnoreCase)

type internal ISetThemeColors = abstract member SetColors: unit -> unit

[<Sealed>]
type MaybeBuilder () =
    // 'T -> M<'T>
    [<DebuggerStepThrough>]
    member inline _.Return value: 'T option =
        Some value

    // M<'T> -> M<'T>
    [<DebuggerStepThrough>]
    member inline _.ReturnFrom value: 'T option =
        value

    // unit -> M<'T>
    [<DebuggerStepThrough>]
    member inline _.Zero (): unit option =
        Some ()     // TODO: Should this be None?

    // (unit -> M<'T>) -> M<'T>
    [<DebuggerStepThrough>]
    member _.Delay (f: unit -> 'T option): 'T option =
        f ()

    // M<'T> -> M<'T> -> M<'T>
    // or
    // M<unit> -> M<'T> -> M<'T>
    [<DebuggerStepThrough>]
    member inline _.Combine (r1, r2: 'T option): 'T option =
        match r1 with
        | None ->
            None
        | Some () ->
            r2

    // M<'T> * ('T -> M<'U>) -> M<'U>
    [<DebuggerStepThrough>]
    member inline _.Bind (value, f: 'T -> 'U option): 'U option =
        Option.bind f value

    // 'T * ('T -> M<'U>) -> M<'U> when 'U :> IDisposable
    [<DebuggerStepThrough>]
    member _.Using (resource: ('T :> System.IDisposable), body: _ -> _ option): _ option =
        try body resource
        finally
            if not <| obj.ReferenceEquals (null, box resource) then
                resource.Dispose ()

    // (unit -> bool) * M<'T> -> M<'T>
    [<DebuggerStepThrough>]
    member x.While (guard, body: _ option): _ option =
        if guard () then
            // OPTIMIZE: This could be simplified so we don't need to make calls to Bind and While.
            x.Bind (body, (fun () -> x.While (guard, body)))
        else
            x.Zero ()

    // seq<'T> * ('T -> M<'U>) -> M<'U>
    // or
    // seq<'T> * ('T -> M<'U>) -> seq<M<'U>>
    [<DebuggerStepThrough>]
    member x.For (sequence: seq<_>, body: 'T -> unit option): _ option =
        // OPTIMIZE: This could be simplified so we don't need to make calls to Using, While, Delay.
        x.Using (sequence.GetEnumerator (), fun enum ->
            x.While (
                enum.MoveNext,
                x.Delay (fun () ->
                    body enum.Current)))

let maybe = MaybeBuilder()

[<Sealed>]
type AsyncMaybeBuilder () =
    [<DebuggerStepThrough>]
    member _.Return value : Async<'T option> = Some value |> async.Return

    [<DebuggerStepThrough>]
    member _.ReturnFrom value : Async<'T option> = value

    [<DebuggerStepThrough>]
    member _.ReturnFrom (value: 'T option) : Async<'T option> = async.Return value

    [<DebuggerStepThrough>]
    member _.Zero () : Async<unit option> =
        Some () |> async.Return

    [<DebuggerStepThrough>]
    member _.Delay (f : unit -> Async<'T option>) : Async<'T option> = async.Delay f

    [<DebuggerStepThrough>]
    member _.Combine (r1, r2 : Async<'T option>) : Async<'T option> =
        async {
            let! r1' = r1
            match r1' with
            | None -> return None
            | Some () -> return! r2
        }

    [<DebuggerStepThrough>]
    member _.Bind (value: Async<'T option>, f : 'T -> Async<'U option>) : Async<'U option> =
        async {
            let! value' = value
            match value' with
            | None -> return None
            | Some result -> return! f result
        }

    [<DebuggerStepThrough>]
    member _.Bind (value: System.Threading.Tasks.Task<'T>, f : 'T -> Async<'U option>) : Async<'U option> =
        async {
            let! value' = Async.AwaitTask value
            return! f value'
        }

    [<DebuggerStepThrough>]
    member _.Bind (value: 'T option, f : 'T -> Async<'U option>) : Async<'U option> =
        async {
            match value with
            | None -> return None
            | Some result -> return! f result
        }

    [<DebuggerStepThrough>]
    member _.Using (resource : ('T :> IDisposable), body : 'T -> Async<'U option>) : Async<'U option> =
        async {
            use resource = resource
            return! body resource
        }

    [<DebuggerStepThrough>]
    member x.While (guard, body : Async<_ option>) : Async<_ option> =
        if guard () then
            x.Bind (body, (fun () -> x.While (guard, body)))
        else
            x.Zero ()

    [<DebuggerStepThrough>]
    member x.For (sequence : seq<_>, body : 'T -> Async<unit option>) : Async<unit option> =
        x.Using (sequence.GetEnumerator (), fun enum ->
            x.While (enum.MoveNext, x.Delay (fun () -> body enum.Current)))

    [<DebuggerStepThrough>]
    member inline _.TryWith (computation : Async<'T option>, catchHandler : exn -> Async<'T option>) : Async<'T option> =
            async.TryWith (computation, catchHandler)

    [<DebuggerStepThrough>]
    member inline _.TryFinally (computation : Async<'T option>, compensation : unit -> unit) : Async<'T option> =
            async.TryFinally (computation, compensation)

let asyncMaybe = AsyncMaybeBuilder()

let inline liftAsync (computation : Async<'T>) : Async<'T option> =
    async {
        let! a = computation
        return Some a 
    }

let liftTaskAsync task = task |> Async.AwaitTask |> liftAsync

module Array =
    /// Returns a new array with an element replaced with a given value.
    let replace index value (array: _ []) =
        if index >= array.Length then raise (IndexOutOfRangeException "index")
        let res = Array.copy array
        res.[index] <- value
        res

module Async =
    let map (f: 'T -> 'U) (a: Async<'T>) : Async<'U> =
        async {
            let! a = a
            return f a
        }

    /// Creates an asynchronous workflow that runs the asynchronous workflow given as an argument at most once. 
    /// When the returned workflow is started for the second time, it reuses the result of the previous execution.
    let cache (input : Async<'T>) =
        let agent = MailboxProcessor<AsyncReplyChannel<_>>.Start <| fun agent ->
            async {
                let! replyCh = agent.Receive ()
                let! res = input
                replyCh.Reply res
                while true do
                    let! replyCh = agent.Receive ()
                    replyCh.Reply res 
            }
        async { return! agent.PostAndAsyncReply id }

let FSharpExperimentalFeaturesEnabledAutomatically =
    String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FSHARP_EXPERIMENTAL_FEATURES"))
    |> not