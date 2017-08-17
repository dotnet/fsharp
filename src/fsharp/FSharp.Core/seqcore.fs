// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections
    #nowarn "52" // The value has been copied to ensure the original is not mutated by this operation

    open System
    open System.Diagnostics
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

    module internal IEnumerator =

      let noReset() = raise (new System.NotSupportedException(SR.GetString(SR.resetNotSupported)))
      let notStarted() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
      let alreadyFinished() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))
      let check started = if not started then notStarted()
      let dispose (r : System.IDisposable) = r.Dispose()

      let cast (e : IEnumerator) : IEnumerator<'T> =
          { new IEnumerator<'T> with
                member x.Current = unbox<'T> e.Current
            interface IEnumerator with
                member x.Current = unbox<'T> e.Current :> obj
                member x.MoveNext() = e.MoveNext()
                member x.Reset() = noReset()
            interface System.IDisposable with
                member x.Dispose() =
                    match e with
                    | :? System.IDisposable as e -> e.Dispose()
                    | _ -> ()   }

      /// A concrete implementation of an enumerator that returns no values
      [<Sealed>]
      type EmptyEnumerator<'T>() =
          let mutable started = false
          interface IEnumerator<'T> with
                member x.Current =
                  check started
                  (alreadyFinished() : 'T)

          interface System.Collections.IEnumerator with
              member x.Current =
                  check started
                  (alreadyFinished() : obj)
              member x.MoveNext() =
                  if not started then started <- true
                  false
              member x.Reset() = noReset()
          interface System.IDisposable with
                member x.Dispose() = ()
                
      let Empty<'T> () = (new EmptyEnumerator<'T>() :> IEnumerator<'T>)

      [<NoEquality; NoComparison>]
      type EmptyEnumerable<'T> =
            | EmptyEnumerable
            interface IEnumerable<'T> with
                member x.GetEnumerator() = Empty<'T>()
            interface IEnumerable with
                member x.GetEnumerator() = (Empty<'T>() :> IEnumerator)

      let readAndClear r =
          lock r (fun () -> match !r with None -> None | Some _ as res -> r := None; res)

      let generateWhileSome openf compute closef : IEnumerator<'U> =
          let started = ref false
          let curr = ref None
          let state = ref (Some(openf()))
          let getCurr() =
              check !started
              match !curr with None -> alreadyFinished() | Some x -> x
          let start() = if not !started then (started := true)

          let dispose() = readAndClear state |> Option.iter closef
          let finish() = (try dispose() finally curr := None)
          {  new IEnumerator<'U> with
                 member x.Current = getCurr()
             interface IEnumerator with
                 member x.Current = box (getCurr())
                 member x.MoveNext() =
                     start()
                     match !state with
                     | None -> false (* we started, then reached the end, then got another MoveNext *)
                     | Some s ->
                         match (try compute s with e -> finish(); reraise()) with
                         | None -> finish(); false
                         | Some _ as x -> curr := x; true

                 member x.Reset() = noReset()
             interface System.IDisposable with
                 member x.Dispose() = dispose() }

      [<Sealed>]
      type Singleton<'T>(v:'T) =
          let mutable started = false
          interface IEnumerator<'T> with
                member x.Current = v
          interface IEnumerator with
              member x.Current = box v
              member x.MoveNext() = if started then false else (started <- true; true)
              member x.Reset() = noReset()
          interface System.IDisposable with
              member x.Dispose() = ()

      let Singleton x = (new Singleton<'T>(x) :> IEnumerator<'T>)

      let EnumerateThenFinally f (e : IEnumerator<'T>) =
          { new IEnumerator<'T> with
                member x.Current = e.Current
            interface IEnumerator with
                member x.Current = (e :> IEnumerator).Current
                member x.MoveNext() = e.MoveNext()
                member x.Reset() = noReset()
            interface System.IDisposable with
                member x.Dispose() =
                    try
                        e.Dispose()
                    finally
                        f()
          }

      let inline checkNonNull argName arg =
            match box arg with
            | null -> nullArg argName
            | _ -> ()

      let mkSeq f =
            { new IEnumerable<'U> with
                member x.GetEnumerator() = f()
              interface IEnumerable with
                member x.GetEnumerator() = (f() :> IEnumerator) }

namespace Microsoft.FSharp.Collections.SeqComposition
  open System
  open System.Collections
  open System.Collections.Generic
  open Microsoft.FSharp.Core
  open Microsoft.FSharp.Collections
  open Microsoft.FSharp.Collections.IEnumerator
  open Microsoft.FSharp.Collections.SeqComposition
  open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
  open Microsoft.FSharp.Primitives.Basics
  open Microsoft.FSharp.Control

  module Core = 
    [<Struct; NoComparison; NoEquality>]
    type NoValue = struct end

    module internal TailCall =
        // used for performance reasons; these are not recursive calls, so should be safe
        // ** it should be noted that potential changes to the f# compiler may render this function
        // ineffictive **
        let inline avoid boolean = match boolean with true -> true | false -> false

    module internal Closure =
        // F# inlines simple functions, which can mean that it some case you keep creating closures when
        // a single funciton object would have done. This forces the compiler to create the object
        let inline forceCapture<'a,'b> (f:'a->'b) : 'a->'b = (# "" f : 'a->'b #)

    type IdentityFactory<'T> private () =
        static let singleton : ITransformFactory<'T,'T> = upcast (IdentityFactory<'T>())
        static member Instance = singleton
        interface ITransformFactory<'T,'T> with
            member __.Compose<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Activity<'T,'V>) : Activity<'T,'V> = next

    type ComposedFactory<'T,'U,'V> private (first:ITransformFactory<'T,'U>, second:ITransformFactory<'U,'V>) =
        interface ITransformFactory<'T,'V> with
            member this.Compose<'W> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Activity<'V,'W>) : Activity<'T,'W> =
                first.Compose outOfBand (pipeIdx-1) (second.Compose outOfBand pipeIdx next)

        static member Combine (first:ITransformFactory<'T,'U>) (second:ITransformFactory<'U,'V>) : ITransformFactory<'T,'V> =
            upcast ComposedFactory(first, second)

    type ISkipable =
        // Seq.init(Infinite)? lazily uses Current. The only ISeq component that can do that is Skip
        // and it can only do it at the start of a sequence
        abstract CanSkip : unit -> bool

    type SeqProcessNextStates =
    | InProcess  = 0
    | NotStarted = 1
    | Finished   = 2

    type EmptyEnumerator<'T>() =
        let current () = failwith "library implementation error: Current should never be called"

        interface IEnumerator<'T> with
            member __.Current = current ()

        interface IEnumerator with
            member __.Current = current ()
            member __.MoveNext () = false
            member __.Reset (): unit = noReset ()

        interface IDisposable with
            member __.Dispose () = ()

    type EmptyEnumerators<'T>() =
        static let element : IEnumerator<'T> = upcast (new EmptyEnumerator<'T> ())
        static member Element = element

    [<AbstractClass>]
    type EnumerableBase<'T> () =
        let derivedClassShouldImplement () =
            failwith "library implementation error: derived class should implement (should be abstract)"

        abstract member Append : ISeq<'T> -> ISeq<'T>
        abstract member Length : unit -> int
        abstract member GetRaw : unit -> seq<'T>

        default this.Append source = upcast (AppendEnumerable [source; this])
        default this.Length () = Microsoft.FSharp.Primitives.Basics.ISeq.length this
        default this.GetRaw () = upcast this

        interface IEnumerable with
            member this.GetEnumerator () : IEnumerator =
                upcast ((this:>IEnumerable<'T>).GetEnumerator ())

        interface IEnumerable<'T> with
            // fsharp doesn't allow abstract interface methods
            member this.GetEnumerator () : IEnumerator<'T> = derivedClassShouldImplement ()

        interface ISeq<'T> with
            // fsharp doesn't allow abstract interface methods
            member __.PushTransform _ = derivedClassShouldImplement ()
            member __.Fold _ = derivedClassShouldImplement ()

    and [<AbstractClass>] SeqFactoryBase<'T,'U>(transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        member __.CreateActivityPipeline<'Result> (folder:Folder<'U,'Result>) : Activity<'T,'U> =
            transformFactory.Compose (folder :> IOutOfBand) pipeIdx folder

        member this.CreatePipeline<'Result> (createFolder:PipeIdx->Folder<'U,'Result>, [<System.Runtime.InteropServices.Out>] activity:byref<Activity<'T,'U>>) =
            let folder = createFolder (pipeIdx+1)
            activity <- this.CreateActivityPipeline folder
            folder

        member __.Compose next = ComposedFactory.Combine transformFactory next

        member __.PipeIdx = pipeIdx
    
    and [<AbstractClass>] EnumeratorBase<'T>() =
        inherit Folder<'T,'T>(Unchecked.defaultof<'T>)

        member val SeqState = SeqProcessNextStates.NotStarted with get, set
        abstract Activity : Activity

        override this.ProcessNext (input:'T) : bool =
            this.Result <- input
            true

        interface IDisposable with
            member this.Dispose () : unit =
                this.Activity.ChainDispose ()

        interface IEnumerator with
            member this.Current : obj = box (this :> IEnumerator<'T>).Current
            member __.MoveNext () = failwith "library implementation error: derived class should implement (should be abstract)"
            member __.Reset () : unit = noReset ()

        interface IEnumerator<'T> with
            member this.Current =
                if this.SeqState = SeqProcessNextStates.InProcess then this.Result
                else
                    match this.SeqState with
                    | SeqProcessNextStates.NotStarted -> notStarted()
                    | SeqProcessNextStates.Finished -> alreadyFinished()
                    | _ -> failwith "library implementation error: all states should have been handled"

    and VanillaEnumerator<'T,'U> private (source:IEnumerator<'T>) =
        inherit EnumeratorBase<'U>()

        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

        override this.Activity = upcast activity

        member private this.MoveNext () =
            if this.HaltedIdx = 0 && source.MoveNext () then
                if activity.ProcessNext source.Current then
                    true
                else
                    this.MoveNext ()
            else
                this.SeqState <- SeqProcessNextStates.Finished
                activity.ChainComplete this.HaltedIdx
                false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        interface IDisposable with
            member __.Dispose () =
                try
                    source.Dispose ()
                finally
                    activity.ChainDispose ()

        static member Construct (source:IEnumerator<'T>) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new VanillaEnumerator<'T,'U>(source)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    and VanillaEnumerable<'T,'U>(enumerable:IEnumerable<'T>, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = VanillaEnumerator<'T,'U>.Construct (enumerable.GetEnumerator()) this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new VanillaEnumerable<'T,'V>(enumerable, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result, consumer = this.CreatePipeline createFolder
                try
                    use enumerator = enumerable.GetEnumerator ()
                    while result.HaltedIdx = 0 && enumerator.MoveNext () do
                        consumer.ProcessNext enumerator.Current |> ignore

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

    and ConcatCommon<'T>(consumer:Activity<'T>) =
        inherit Folder<'T,NoValue>(Unchecked.defaultof<_>)

        member __.Consumer = consumer

        override me.ProcessNext value = TailCall.avoid (consumer.ProcessNext value)

    and ConcatFold<'T,'U,'Collection,'Result when 'Collection :> ISeq<'T>>(result:Folder<'U,'Result>, consumer:Activity<'T,'U>, common:Folder<'T,NoValue>) as this =
        inherit Folder<'Collection, 'Result>(Unchecked.defaultof<_>)

        do
            (this :> IOutOfBand).ListenForStopFurtherProcessing (fun idx ->
                (result :> IOutOfBand).StopFurtherProcessing idx
                (common :> IOutOfBand).StopFurtherProcessing PipeIdx.MinValue)

            (result :> IOutOfBand).ListenForStopFurtherProcessing (fun idx ->
                (this :> IOutOfBand).StopFurtherProcessing idx
                (common :> IOutOfBand).StopFurtherProcessing PipeIdx.MaxValue)

        let getCommonFolder = Closure.forceCapture (fun (_:PipeIdx) ->
            (common :> IOutOfBand).StopFurtherProcessing 0
            common)

        override __.ProcessNext value =
            value.Fold getCommonFolder |> ignore
            Unchecked.defaultof<_> (* return value unused in Fold context *)

        override this.ChainComplete _ =
            consumer.ChainComplete result.HaltedIdx
            this.Result <- result.Result

        override this.ChainDispose () =
            consumer.ChainDispose ()

    and ConcatEnumerator<'T,'U,'Collection when 'Collection :> IEnumerable<'T>> private (sources:IEnumerable<'Collection>) =
        inherit EnumeratorBase<'U>()

        let main = sources.GetEnumerator ()
        let mutable active = EmptyEnumerators.Element
        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>

        override this.Activity = upcast activity

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

        member private this.MoveNext () =
            if this.HaltedIdx <> 0 then false
            else
                if active.MoveNext () then
                    if activity.ProcessNext active.Current then
                        true
                    else
                        this.MoveNext ()
                elif main.MoveNext () then
                    active.Dispose ()
                    active <- main.Current.GetEnumerator ()
                    this.MoveNext ()
                else
                    this.SeqState <- SeqProcessNextStates.Finished
                    activity.ChainComplete this.HaltedIdx
                    false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        interface IDisposable with
            member __.Dispose () =
                try
                    main.Dispose ()
                    active.Dispose ()
                finally
                    activity.ChainDispose ()

        static member Construct (sources:IEnumerable<'Collection>) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new ConcatEnumerator<'T,'U,'Collection>(sources)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    and ConcatEnumerable<'T,'U,'Collection when 'Collection :> ISeq<'T>> (sources:ISeq<'Collection>, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = ConcatEnumerator<'T,'U,'Collection>.Construct sources this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new ConcatEnumerable<'T,'V,'Collection>(sources, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                sources.Fold (fun lowerPipeIdx ->
                    let thisPipeIdx = lowerPipeIdx + pipeIdx

                    let result = createFolder (thisPipeIdx+1)

                    let outOfBand = result :> IOutOfBand

                    let consumer = transformFactory.Compose outOfBand thisPipeIdx result 
                         
                    let common =
                        match box consumer with
                        | :? ConcatCommon<'T> as c -> ConcatCommon c.Consumer
                        | _ -> ConcatCommon consumer

                    upcast ConcatFold (result, consumer, common))

    and ThinConcatEnumerable<'T, 'Sources, 'Collection when 'Collection :> ISeq<'T>> (sources:'Sources, preEnumerate:'Sources->ISeq<'Collection>) =
        inherit EnumerableBase<'T>()

        member private __.Fatten : ISeq<'T> = upcast (ConcatEnumerable<'T,'T,'Collection>(preEnumerate sources, IdentityFactory.Instance, 1))

        interface IEnumerable<'T> with
            member this.GetEnumerator () = this.Fatten.GetEnumerator ()

        interface ISeq<'T> with
            member this.PushTransform (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                upcast (ConcatEnumerable<'T,'V,'Collection>(preEnumerate sources, next, 1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                this.Fatten.Fold createFolder

    and AppendEnumerable<'T> (sources:list<ISeq<'T>>) =
        inherit ThinConcatEnumerable<'T, list<ISeq<'T>>, ISeq<'T>>(sources, fun sources -> upcast (List.rev sources))

        override this.Append source =
            upcast (AppendEnumerable (source::sources))

    /// ThinEnumerable is used when the IEnumerable provided to ofSeq is neither an array or a list
    type ThinEnumerable<'T>(enumerable:IEnumerable<'T>) =
        inherit VanillaEnumerable<'T,'T>(enumerable, IdentityFactory.Instance, 0)

        override __.Length () =
            match enumerable with
            | :? ICollection<'T> as a -> a.Count
#if !FSCORE_PORTABLE_OLD
            | :? IReadOnlyCollection<'T> as a -> a.Count
#endif
            | _ ->
                use e = enumerable.GetEnumerator ()
                let mutable count = 0
                while e.MoveNext () do
                    count <- count + 1
                count

        interface IEnumerable<'T> with
            member __.GetEnumerator () = enumerable.GetEnumerator ()

    type DelayedEnumerable<'T>(delayed:unit->ISeq<'T>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'T>()

        override __.Length () =
            match delayed() with
            | :? EnumerableBase<'T> as s -> s.Length ()
            | s -> Microsoft.FSharp.Primitives.Basics.ISeq.length s

        override __.GetRaw () = 
            match delayed() with
            | :? EnumerableBase<'T> as s -> s.GetRaw ()
            | s -> upcast s

        interface IEnumerable<'T> with
            member this.GetEnumerator () : IEnumerator<'T> = (delayed()).GetEnumerator ()

        interface ISeq<'T> with
            member __.PushTransform (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                upcast (new DelayedEnumerable<'U>((fun () -> (delayed()).PushTransform next), pipeIdx+1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                (delayed()).Fold f

    type EmptyEnumerable<'T> private () =
        inherit EnumerableBase<'T>()

        static let singleton = EmptyEnumerable<'T>() :> ISeq<'T>
        static member Instance = singleton

        override __.Length () = 0

        interface IEnumerable<'T> with
            member this.GetEnumerator () : IEnumerator<'T> = IEnumerator.Empty<'T>()

        override this.Append source = source

        interface ISeq<'T> with
            member this.PushTransform (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                upcast (VanillaEnumerable<'T,'V>(this, next, 1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                let result = createFolder 1
                try
                    result.ChainComplete result.HaltedIdx
                finally
                    result.ChainDispose ()
                result.Result

    type ArrayEnumerator<'T,'U> private (array:array<'T>) =
        inherit EnumeratorBase<'U>()

        let mutable idx = 0
        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

        override this.Activity = upcast activity

        member private this.MoveNext () =
            if this.HaltedIdx = 0 && idx < array.Length then
                idx <- idx+1
                if activity.ProcessNext array.[idx-1] then
                    true
                else
                    this.MoveNext ()
            else
                this.SeqState <- SeqProcessNextStates.Finished
                activity.ChainComplete this.HaltedIdx
                false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        static member Construct (array:array<'T>) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new ArrayEnumerator<'T,'U>(array)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    type ArrayEnumerable<'T,'U>(array:array<'T>, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = ArrayEnumerator<'T,'U>.Construct array this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new ArrayEnumerable<'T,'V>(array, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result, consumer = this.CreatePipeline createFolder
                try
                    let array = array
                    let mutable idx = 0
                    while result.HaltedIdx = 0 && idx < array.Length do
                        consumer.ProcessNext array.[idx] |> ignore
                        idx <- idx + 1

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

    type ThinArrayEnumerable<'T>(array:array<'T>) =
        inherit ArrayEnumerable<'T,'T>(array, IdentityFactory.Instance, 0)

        override __.Length () = array.Length
        override __.GetRaw () = upcast array

        interface IEnumerable<'T> with
            member __.GetEnumerator () = (array:>IEnumerable<'T>).GetEnumerator ()

    type SingletonEnumerable<'T>(item:'T) =
        inherit EnumerableBase<'T>()

        override __.Length () = 1

        interface IEnumerable<'T> with
            member this.GetEnumerator () = (new Singleton<'T>(item)) :> IEnumerator<'T>

        interface ISeq<'T> with
            member __.PushTransform (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                ([item] :> ISeq<'T>).PushTransform next

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                let result = createFolder 1
                try
                    if result.HaltedIdx = 0 then
                        result.ProcessNext item |> ignore

                    result.ChainComplete result.HaltedIdx
                finally
                    result.ChainDispose ()
                result.Result

    type ResizeArrayEnumerator<'T,'U> private (array:ResizeArray<'T>) =
        inherit EnumeratorBase<'U>()

        let mutable idx = 0
        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

        override this.Activity = upcast activity

        member private this.MoveNext () =
            if this.HaltedIdx = 0 && idx < array.Count then
                idx <- idx+1
                if activity.ProcessNext array.[idx-1] then
                    true
                else
                    this.MoveNext ()
            else
                this.SeqState <- SeqProcessNextStates.Finished
                activity.ChainComplete this.HaltedIdx
                false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        static member Construct (array:ResizeArray<'T>) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new ResizeArrayEnumerator<'T,'U>(array)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    type ResizeArrayEnumerable<'T,'U>(resizeArray:ResizeArray<'T>, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = ResizeArrayEnumerator<'T,'U>.Construct resizeArray this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new ResizeArrayEnumerable<'T,'V>(resizeArray, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result, consumer = this.CreatePipeline createFolder
                try
                    let array = resizeArray
                    let mutable idx = 0
                    while result.HaltedIdx = 0 && idx < array.Count do  
                        consumer.ProcessNext array.[idx] |> ignore
                        idx <- idx + 1

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

    type ThinResizeArrayEnumerable<'T>(resizeArray:ResizeArray<'T>) =
        inherit ResizeArrayEnumerable<'T,'T>(resizeArray, IdentityFactory.Instance, 0)

        override __.Length () = resizeArray.Count

    type UnfoldEnumerator<'T,'U,'State> private (generator:'State->option<'T*'State>, state:'State) =
        inherit EnumeratorBase<'U>()

        let mutable current = state

        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

        override this.Activity = upcast activity

        member private this.MoveNext () =
            if this.HaltedIdx <> 0 then
                false
            else
                match generator current with
                | Some (item, nextState) ->
                    current <- nextState
                    if activity.ProcessNext item then
                        true
                    else
                        this.MoveNext ()
                | _ -> false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        static member Construct (generator:'State->option<'T*'State>) (state:'State) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new UnfoldEnumerator<'T,'U,'State>(generator, state)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    type UnfoldEnumerable<'T,'U,'GeneratorState>(generator:'GeneratorState->option<'T*'GeneratorState>, state:'GeneratorState, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = UnfoldEnumerator<'T,'U,'GeneratorState>.Construct generator state this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new UnfoldEnumerable<'T,'V,'GeneratorState>(generator, state, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result, consumer = this.CreatePipeline createFolder
                try
                    let generator = generator
                    let rec iterate current =
                        if result.HaltedIdx <> 0 then ()
                        else
                            match generator current with
                            | Some (item, next) ->
                                consumer.ProcessNext item |> ignore
                                iterate next
                            | _ -> ()
                    iterate state

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

    let getInitTerminatingIdx (count:Nullable<int>) =
        // we are offset by 1 to allow for values going up to System.Int32.MaxValue
        // System.Int32.MaxValue is an illegal value for the "infinite" sequence
        if count.HasValue then
            count.Value - 1
        else
            System.Int32.MaxValue

    type InitEnumerator<'T,'U>(count:Nullable<int>, f:int->'T) =
        inherit EnumeratorBase<'U>()

        let terminatingIdx =
            getInitTerminatingIdx count

        let mutable maybeSkipping = true
        let mutable idx = -1

        let mutable activity = Unchecked.defaultof<Activity<'T,'U>>
        let mutable isSkipping = Unchecked.defaultof<unit->bool>

        member __.FinaliseConstruct(activity':Activity<'T,'U>) =
            activity <- activity'

            isSkipping <- 
                match box activity with
                | :? ISkipable as skip -> skip.CanSkip
                | _ -> fun () -> false


        override this.Activity = upcast activity

        member private this.MoveNext () =
            if this.HaltedIdx = 0 && idx < terminatingIdx then
                idx <- idx + 1

                if maybeSkipping then
                    // Skip can only is only checked at the start of the sequence, so once
                    // triggered, we stay triggered.
                    maybeSkipping <- isSkipping ()

                if maybeSkipping then
                    this.MoveNext ()
                elif activity.ProcessNext (f idx) then
                    true
                else
                    this.MoveNext ()
            elif this.HaltedIdx = 0 && idx = System.Int32.MaxValue then
                raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
            else
                this.SeqState <- SeqProcessNextStates.Finished
                activity.ChainComplete this.HaltedIdx
                false

        interface IEnumerator with
            member this.MoveNext () =
                this.SeqState <- SeqProcessNextStates.InProcess
                this.MoveNext ()

        static member Construct (count:Nullable<int>) (f:int->'T) (factory:SeqFactoryBase<'T,'U>) : IEnumerator<'U> =
            let enumerator = new InitEnumerator<'T,'U>(count, f)
            enumerator.FinaliseConstruct (factory.CreateActivityPipeline enumerator)
            upcast enumerator

    type InitEnumerable<'T,'U>(count:Nullable<int>, f:int->'T, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = InitEnumerator<'T,'U>.Construct count f this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new InitEnumerable<'T,'V>(count, f, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let terminatingIdx = getInitTerminatingIdx count
                let result, consumer = this.CreatePipeline createFolder
                try
                    let firstIdx = 
                        match box consumer with
                        | :? ISkipable as skipping ->
                            let rec skip idx =
                                if idx = terminatingIdx || result.HaltedIdx <> 0 then
                                    terminatingIdx
                                elif skipping.CanSkip () then
                                    skip (idx+1)
                                else
                                    idx
                            skip -1
                        | _ -> -1

                    let rec iterate idx =
                        if idx < terminatingIdx then
                            consumer.ProcessNext (f (idx+1)) |> ignore
                            if result.HaltedIdx = 0 then
                                iterate (idx+1)
                            else
                                idx
                        else
                            idx

                    let finalIdx = iterate firstIdx
                    if result.HaltedIdx = 0 && finalIdx = System.Int32.MaxValue then
                        raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

    // The original implementation of "init" delayed the calculation of Current, and so it was possible
    // to do MoveNext without it's value being calculated.
    // I can imagine only two scenerios where that is possibly sane, although a simple solution is readily
    // at hand in both cases. The first is that of an expensive generator function, where you skip the
    // first n elements. The simple solution would have just been to have a map ((+) n) as the first operation
    // instead. The second case would be counting elements, but that is only of use if you're not filtering
    // or mapping or doing anything else (as that would cause Current to be evaluated!) and
    // so you already know what the count is!! Anyway, someone thought it was a good idea, so
    // I have had to add an extra function that is used in Skip to determine if we are touching
    // Current or not.
    // InitEnumerableDecider returns the original implementation of init when GetEnumerator is called
    // If any Activites are added to the pipeline then the original implementation is ignored, as special
    // handling has been added to Current isn't calculated whilst skipping enumerated items.
    type InitEnumerableDecider<'T>(count:Nullable<int>, f:int->'T, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'T>()

        let upto lastOption f =
            match lastOption with
            | Some b when b<0 -> failwith "library implementation error: upto can never be called with a negative value"
            | _ ->
                let unstarted   = -1  // index value means unstarted (and no valid index)
                let completed   = -2  // index value means completed (and no valid index)
                let unreachable = -3  // index is unreachable from 0,1,2,3,...
                let finalIndex  = match lastOption with
                                    | Some b -> b             // here b>=0, a valid end value.
                                    | None   -> unreachable   // run "forever", well as far as Int32.MaxValue since indexing with a bounded type.
                // The Current value for a valid index is "f i".
                // Lazy<_> values are used as caches, to store either the result or an exception if thrown.
                // These "Lazy<_>" caches are created only on the first call to current and forced immediately.
                // The lazy creation of the cache nodes means enumerations that skip many Current values are not delayed by GC.
                // For example, the full enumeration of Seq.initInfinite in the tests.
                // state
                let index   = ref unstarted
                // a Lazy node to cache the result/exception
                let current = ref (Unchecked.defaultof<_>)
                let setIndex i = index := i; current := (Unchecked.defaultof<_>) // cache node unprimed, initialised on demand.
                let getCurrent() =
                    if !index = unstarted then notStarted()
                    if !index = completed then alreadyFinished()
                    match box !current with
                    | null -> current := Lazy<_>.Create(fun () -> f !index)
                    | _ ->  ()
                    // forced or re-forced immediately.
                    (!current).Force()
                { new IEnumerator<'U> with
                        member x.Current = getCurrent()
                    interface IEnumerator with
                        member x.Current = box (getCurrent())
                        member x.MoveNext() =
                            if !index = completed then
                                false
                            elif !index = unstarted then
                                setIndex 0
                                true
                            else (
                                if !index = System.Int32.MaxValue then raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
                                if !index = finalIndex then
                                    false
                                else
                                    setIndex (!index + 1)
                                    true
                            )
                        member this.Reset() = noReset()
                    interface System.IDisposable with
                        member x.Dispose () = () }

        override this.Length () =
            if count.HasValue then
                count.Value
            else
                raise (System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue)))

        interface IEnumerable<'T> with
            member this.GetEnumerator () : IEnumerator<'T> =
                // we defer back to the original implementation as, as it's quite idiomatic in it's decision
                // to calculate Current in a lazy fashion. I doubt anyone is really using this functionality
                // in the way presented, but it's possible.
                upto (if count.HasValue then Some (count.Value-1) else None) f

        interface ISeq<'T> with
            member this.PushTransform (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                upcast (InitEnumerable<'T,'V>(count, f, next, pipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                let result = createFolder 1
                try
                    use enumerator = (this:>IEnumerable<'T>).GetEnumerator ()
                    while result.HaltedIdx = 0 && enumerator.MoveNext () do
                        result.ProcessNext enumerator.Current |> ignore

                    result.ChainComplete result.HaltedIdx
                finally
                    result.ChainDispose ()
                result.Result

namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Collections.IEnumerator
    open Microsoft.FSharp.Primitives.Basics
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Collections.SeqComposition
    open Microsoft.FSharp.Collections.SeqComposition.Core

    module RuntimeHelpers =
        let Generate openf compute closef =
            mkSeq (fun () -> IEnumerator.generateWhileSome openf compute closef)

        let GenerateUsing (openf : unit -> ('U :> System.IDisposable)) compute =
            Generate openf compute (fun (s:'U) -> s.Dispose())

        let EnumerateFromFunctions opener moveNext current =
            Generate
                opener
                (fun x -> if moveNext x then Some(current x) else None)
                (fun x -> match box(x) with :? System.IDisposable as id -> id.Dispose() | _ -> ())

        // A family of enumerators that can have additional 'finally' actions added to the enumerator through
        // the use of mutation. This is used to 'push' the disposal action for a 'use' into the next enumerator.
        // For example,
        //    seq { use x = ...
        //          while ... }
        // results in the 'while' loop giving an adjustable enumerator. This is then adjusted by adding the disposal action
        // from the 'use' into the enumerator. This means that we avoid constructing a two-deep enumerator chain in this
        // common case.
        type IFinallyEnumerator =
            abstract AppendFinallyAction : (unit -> unit) -> unit

        /// A concrete implementation of IEnumerable that adds the given compensation to the "Dispose" chain of any
        /// enumerators returned by the enumerable.
        [<Sealed>]
        type FinallyEnumerable<'T>(compensation: unit -> unit, restf: unit -> seq<'T>) =
            interface IEnumerable<'T> with
                member x.GetEnumerator() =
                    try
                        let ie = restf().GetEnumerator()
                        match ie with
                        | :? IFinallyEnumerator as a ->
                            a.AppendFinallyAction(compensation)
                            ie
                        | _ ->
                            IEnumerator.EnumerateThenFinally compensation ie
                    with e ->
                        compensation()
                        reraise()
            interface IEnumerable with
                member x.GetEnumerator() = ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

        /// An optimized object for concatenating a sequence of enumerables
        [<Sealed>]
        type ConcatEnumerator<'T,'U when 'U :> seq<'T>>(sources: seq<'U>) =
            let mutable outerEnum = sources.GetEnumerator()
            let mutable currInnerEnum = IEnumerator.Empty()

            let mutable started = false
            let mutable finished = false
            let mutable compensations = []

            [<DefaultValue(false)>] // false = unchecked
            val mutable private currElement : 'T

            member x.Finish() =
                finished <- true
                try
                    match currInnerEnum with
                    | null -> ()
                    | _ ->
                        try
                            currInnerEnum.Dispose()
                        finally
                            currInnerEnum <- null
                finally
                    try
                        match outerEnum with
                        | null -> ()
                        | _ ->
                            try
                                outerEnum.Dispose()
                            finally
                                outerEnum <- null
                    finally
                        let rec iter comps =
                            match comps with
                            |   [] -> ()
                            |   h::t ->
                                    try h() finally iter t
                        try
                            compensations |> List.rev |> iter
                        finally
                            compensations <- []

            member x.GetCurrent() =
                IEnumerator.check started
                if finished then IEnumerator.alreadyFinished() else x.currElement

            interface IFinallyEnumerator with
                member x.AppendFinallyAction(f) =
                    compensations <- f :: compensations

            interface IEnumerator<'T> with
                member x.Current = x.GetCurrent()

            interface IEnumerator with
                member x.Current = box (x.GetCurrent())

                member x.MoveNext() =
                   if not started then (started <- true)
                   if finished then false
                   else
                      let rec takeInner () =
                        // check the inner list
                        if currInnerEnum.MoveNext() then
                            x.currElement <- currInnerEnum.Current
                            true
                        else
                            // check the outer list
                            let rec takeOuter() =
                                if outerEnum.MoveNext() then
                                    let ie = outerEnum.Current
                                    // Optimization to detect the statically-allocated empty IEnumerables
                                    match box ie with
                                    | :? EmptyEnumerable<'T> ->
                                         // This one is empty, just skip, don't call GetEnumerator, try again
                                         takeOuter()
                                    | _ ->
                                         // OK, this one may not be empty.
                                         // Don't forget to dispose of the enumerator for the inner list now we're done with it
                                         currInnerEnum.Dispose()
                                         currInnerEnum <- ie.GetEnumerator()
                                         takeInner ()
                                else
                                    // We're done
                                    x.Finish()
                                    false
                            takeOuter()
                      takeInner ()

                member x.Reset() = IEnumerator.noReset()

            interface System.IDisposable with
                member x.Dispose() =
                    if not finished then
                        x.Finish()

        let EnumerateUsing (resource : 'T :> System.IDisposable) (rest: 'T -> #seq<'U>) =
            (FinallyEnumerable((fun () -> match box resource with null -> () | _ -> resource.Dispose()),
                               (fun () -> rest resource :> seq<_>)) :> seq<_>)

        let mkConcatSeq (sources: seq<'U :> seq<'T>>) =
            mkSeq (fun () -> new ConcatEnumerator<_,_>(sources) :> IEnumerator<'T>)

        let EnumerateWhile (g : unit -> bool) (b: seq<'T>) : seq<'T> =
            let started = ref false
            let curr = ref None
            let getCurr() =
                IEnumerator.check !started
                match !curr with None -> IEnumerator.alreadyFinished() | Some x -> x
            let start() = if not !started then (started := true)

            let finish() = (curr := None)
            mkConcatSeq
               (mkSeq (fun () ->
                    { new IEnumerator<_> with
                          member x.Current = getCurr()
                       interface IEnumerator with
                          member x.Current = box (getCurr())
                          member x.MoveNext() =
                               start()
                               let keepGoing = (try g() with e -> finish (); reraise ()) in
                               if keepGoing then
                                   curr := Some(b); true
                               else
                                   finish(); false
                          member x.Reset() = IEnumerator.noReset()
                       interface System.IDisposable with
                          member x.Dispose() = () }))

        let EnumerateThenFinally (rest : seq<'T>) (compensation : unit -> unit)  =
            (FinallyEnumerable(compensation, (fun () -> rest)) :> seq<_>)

        let CreateEvent (add : 'Delegate -> unit) (remove : 'Delegate -> unit) (create : (obj -> 'Args -> unit) -> 'Delegate ) :IEvent<'Delegate,'Args> =
            // Note, we implement each interface explicitly: this works around a bug in the CLR
            // implementation on CompactFramework 3.7, used on Windows Phone 7
            { new obj() with
                  member x.ToString() = "<published event>"
              interface IEvent<'Delegate,'Args>
              interface IDelegateEvent<'Delegate> with
                 member x.AddHandler(h) = add h
                 member x.RemoveHandler(h) = remove h
              interface System.IObservable<'Args> with
                 member x.Subscribe(r:IObserver<'Args>) =
                     let h = create (fun _ args -> r.OnNext(args))
                     add h
                     { new System.IDisposable with
                          member x.Dispose() = remove h } }


    [<AbstractClass>]
    type GeneratedSequenceBase<'T>() =
        inherit EnumerableBase<'T>()

        let mutable redirectTo : GeneratedSequenceBase<'T> = Unchecked.defaultof<_>
        let mutable redirect : bool = false

        member internal x.GetCurrent () = if redirect then redirectTo.LastGenerated else x.LastGenerated

        abstract GetFreshEnumerator : unit -> IEnumerator<'T>
        abstract GenerateNext : next:byref<IEnumerable<'T>> -> int // 0 = Stop, 1 = Yield, 2 = Goto
        abstract Close: unit -> unit
        abstract CheckClose: bool
        abstract LastGenerated : 'T

        //[<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
        member x.MoveNextImpl() =
             let active =
                 if redirect then redirectTo
                 else x
             let mutable target = null
             match active.GenerateNext(&target) with
             | 1 ->
                 true
             | 2 ->
                 match target.GetEnumerator() with
                 | :? GeneratedSequenceBase<'T> as g when not active.CheckClose ->
                     redirectTo <- g
                 | e ->
                     redirectTo <-
                           { new GeneratedSequenceBase<'T>() with
                                 member x.GetFreshEnumerator() = e
                                 member x.GenerateNext(_) = if e.MoveNext() then 1 else 0
                                 member x.Close() = try e.Dispose() finally active.Close()
                                 member x.CheckClose = true
                                 member x.LastGenerated = e.Current }
                 redirect <- true
                 x.MoveNextImpl()
             | _ (* 0 *)  ->
                 false

        interface IEnumerable<'T> with
            member x.GetEnumerator() = x.GetFreshEnumerator()
        interface IEnumerable with
            member x.GetEnumerator() = (x.GetFreshEnumerator() :> IEnumerator)
        interface IEnumerator<'T> with
            member x.Current = x.GetCurrent ()
            member x.Dispose() = if redirect then redirectTo.Close() else x.Close()
        interface IEnumerator with
            member x.Current = box (if redirect then redirectTo.LastGenerated else x.LastGenerated)

            //[<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
            member x.MoveNext() = x.MoveNextImpl()

            member x.Reset() = raise <| new System.NotSupportedException()

        interface ISeq<'T> with
            member this.PushTransform<'U> (next:ITransformFactory<'T,'U>) : ISeq<'U> =
                upcast (new GeneratedSequenceBaseEnumerable<'T,'U>(this, next, 1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                let result = createFolder 1
                try
                    use maybeGeneratedSequenceBase = this.GetFreshEnumerator ()
                    match maybeGeneratedSequenceBase with
                    | :? GeneratedSequenceBase<'T> as e -> // avoids two virtual function calls
                        while result.HaltedIdx = 0 && e.MoveNextImpl () do
                            result.ProcessNext (e.GetCurrent ()) |> ignore 
                    | e ->
                        while result.HaltedIdx = 0 && e.MoveNext () do
                            result.ProcessNext e.Current |> ignore

                    result.ChainComplete result.HaltedIdx
                finally
                    result.ChainDispose ()
                result.Result

        override this.Length () =
            use maybeGeneratedSequenceBase = this.GetFreshEnumerator ()
            let mutable count = 0
            match maybeGeneratedSequenceBase with
            | :? GeneratedSequenceBase<'T> as e ->
                while e.MoveNextImpl () do
                    count <- count + 1
            | e ->
                while e.MoveNext () do
                    count <- count + 1
            count

    and GeneratedSequenceBaseEnumerable<'T,'U>(generatedSequence:GeneratedSequenceBase<'T>, transformFactory:ITransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit SeqFactoryBase<'T,'U>(transformFactory, pipeIdx)

        interface IEnumerable<'U> with
            member this.GetEnumerator () = VanillaEnumerator<'T,'U>.Construct (generatedSequence.GetFreshEnumerator()) this

        interface ISeq<'U> with
            member this.PushTransform (next:ITransformFactory<'U,'V>) : ISeq<'V> =
                upcast (new GeneratedSequenceBaseEnumerable<'T,'V>(generatedSequence, this.Compose next, this.PipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result, consumer = this.CreatePipeline createFolder
                try
                    use maybeGeneratedSequenceBase = generatedSequence.GetFreshEnumerator ()
                    match maybeGeneratedSequenceBase with
                    | :? GeneratedSequenceBase<'T> as e ->
                        while result.HaltedIdx = 0 && e.MoveNextImpl () do
                            consumer.ProcessNext (e.GetCurrent ()) |> ignore
                    | e ->
                        while result.HaltedIdx = 0 && e.MoveNext () do
                            consumer.ProcessNext e.Current |> ignore

                    consumer.ChainComplete result.HaltedIdx
                finally
                    consumer.ChainDispose ()
                result.Result

