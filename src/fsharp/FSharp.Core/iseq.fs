// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections
    open System
    open System.Diagnostics
    open System.Collections
    open System.Collections.Generic
    open System.Reflection
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics
    open Microsoft.FSharp.Collections.SeqComposition
    open Microsoft.FSharp.Collections.SeqComposition.Core

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module ISeq =
        open IEnumerator

        module Core =
            [<Struct; NoComparison; NoEquality>]
            type Value<'a> =
                val mutable _1 : 'a
                new (a:'a) = { _1 = a }

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b> =
                val mutable _1 : 'a
                val mutable _2 : 'b
                new (a:'a, b: 'b) = { _1 = a;  _2 = b }

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b,'c> =
                val mutable _1 : 'a
                val mutable _2 : 'b
                val mutable _3 : 'c
                new (a:'a, b:'b, c:'c) = { _1 = a; _2 = b; _3 = c }

            [<AbstractClass>]
            type Transform<'T,'U,'State> =
                inherit Activity<'T,'U>
                
                new (next:Activity, initState:'State) = {
                    inherit Activity<'T,'U> ()
                    State = initState
                    Next = next
                }

                val mutable State : 'State
                val Next : Activity
                
                override this.ChainComplete terminatingIdx =
                    this.Next.ChainComplete terminatingIdx
                override this.ChainDispose () =
                    this.Next.ChainDispose ()

            [<AbstractClass>]
            type TransformWithPostProcessing<'T,'U,'State>(next:Activity, initState:'State) =
                inherit Transform<'T,'U,'State>(next, initState)

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose  : unit -> unit

                override this.ChainComplete terminatingIdx =
                    this.OnComplete terminatingIdx
                    this.Next.ChainComplete terminatingIdx
                override this.ChainDispose ()  =
                    try     this.OnDispose ()
                    finally this.Next.ChainDispose ()

            [<AbstractClass>]
            type FolderWithState<'T,'Result,'State> =
                inherit Folder<'T,'Result>

                val mutable State : 'State

                new (initalResult,initState) = {
                    inherit Folder<'T,'Result>(initalResult)
                    State = initState
                }

            [<AbstractClass>]
            type FolderWithPostProcessing<'T,'Result,'State>(initResult,initState) =
                inherit FolderWithState<'T,'Result,'State>(initResult,initState)

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose : unit -> unit

                override this.ChainComplete terminatingIdx =
                    this.OnComplete terminatingIdx
                override this.ChainDispose () =
                    this.OnDispose ()

        open Core

        module internal TailCall =
            // used for performance reasons; these are not recursive calls, so should be safe
            // ** it should be noted that potential changes to the f# compiler may render this function
            // ineffictive **
            let inline avoid boolean = match boolean with true -> true | false -> false

        module internal Upcast =
            // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
            // is fixed with the compiler then these functions can be removed.
            let inline seq<'T,'seq when 'seq :> ISeq<'T> and 'seq : not struct> (t:'seq) : ISeq<'T> = (# "" t : ISeq<'T> #)
            let inline enumerableNonGeneric<'enumerable when 'enumerable :> IEnumerable and 'enumerable : not struct> (t:'enumerable) : IEnumerable = (# "" t : IEnumerable #)
            let inline outOfBand<'outOfBand when 'outOfBand :> IOutOfBand and 'outOfBand : not struct> (t:'outOfBand) : IOutOfBand = (# "" t : IOutOfBand #)

        let inline valueComparer<'T when 'T : equality> ()=
            let c = HashIdentity.Structural<'T>
            { new IEqualityComparer<Value<'T>> with
                    member __.GetHashCode o    = c.GetHashCode o._1
                    member __.Equals (lhs,rhs) = c.Equals (lhs._1, rhs._1) }

        [<CompiledName "Empty">]
        let empty<'T> = Microsoft.FSharp.Collections.SeqComposition.Core.EmptyEnumerable<'T>.Instance

        [<CompiledName("Singleton")>]
        let singleton x = Upcast.seq (new SingletonEnumerable<_>(x))

        /// wraps a ResizeArray in the ISeq framework. Care must be taken that the underlying ResizeArray
        /// is not modified whilst it can be accessed as the ISeq, so check on version is performed.
        /// i.e. usually iteration on calls the enumerator provied by GetEnumerator ensure that the
        /// list hasn't been modified (throwing an exception if it has), but such a check is not
        /// performed in this case. If you want this funcitonality, then use the ofSeq function instead.
        [<CompiledName "OfResizeArrayUnchecked">]
        let ofResizeArrayUnchecked (source:ResizeArray<'T>) : ISeq<'T> =
            Upcast.seq (ThinResizeArrayEnumerable<'T> source)

        [<CompiledName "OfArray">]
        let ofArray (source:array<'T>) : ISeq<'T> =
            checkNonNull "source" source
            Upcast.seq (ThinArrayEnumerable<'T> source)

        [<CompiledName "OfList">]
        let ofList (source:list<'T>) : ISeq<'T> =
            Upcast.seq source

        [<CompiledName "OfSeq">]
        let ofSeq (source:seq<'T>) : ISeq<'T> =
            match source with
            | :? ISeq<'T>  as seq   -> seq
            | :? array<'T> as array -> ofArray array
            | null                  -> nullArg "source"
            | _                     -> Upcast.seq (ThinEnumerable<'T> source)

        [<CompiledName "Average">]
        let inline average (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,int> (LanguagePrimitives.GenericZero, 0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'T> this.Result this.State 
                    override this.OnDispose () = () })

        [<CompiledName "AverageBy">]
        let inline averageBy (f:'T->'U) (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'U,int>(LanguagePrimitives.GenericZero,0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result (f value)
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'U> this.Result this.State
                    override this.OnDispose () = () })

        [<CompiledName "ExactlyOne">]
        let exactlyOne (source:ISeq<'T>) : 'T =
            source.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool, bool>>(Unchecked.defaultof<'T>, Values<bool,bool>(true, false)) with
                    override this.ProcessNext value =
                        if this.State._1 then
                            this.State._1 <- false
                            this.Result <- value
                        else
                            this.State._2 <- true
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        elif this.State._2 then
                            invalidArg "source" (SR.GetString SR.inputSequenceTooLong)
                    override this.OnDispose () = () })

        [<CompiledName "Fold">]
        let inline fold<'T,'State> (f:'State->'T->'State) (seed:'State) (source:ISeq<'T>) : 'State =
            source.Fold (fun _ ->
                { new Folder<'T,'State>(seed) with
                    override this.ProcessNext value =
                        this.Result <- f this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Fold2">]
        let inline fold2<'T1,'T2,'State> (folder:'State->'T1->'T2->'State) (state:'State) (source1:ISeq<'T1>) (source2: ISeq<'T2>) =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<_,'State,IEnumerator<'T2>>(state,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            this.Result <- folder this.Result value this.State.Current
                        else
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Unfold">]
        let unfold (generator:'State->option<'T * 'State>) (state:'State) : ISeq<'T> =
            Upcast.seq (new UnfoldEnumerable<'T,'T,'State>(generator, state, IdentityFactory.Instance, 1))

        [<CompiledName "InitializeInfinite">]
        let initInfinite<'T> (f:int->'T) : ISeq<'T> =
            Upcast.seq (new InitEnumerableDecider<'T>(Nullable (), f, 1))

        [<CompiledName "Initialize">]
        let init<'T> (count:int) (f:int->'T) : ISeq<'T> =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            elif count = 0 then empty else
            Upcast.seq (new InitEnumerableDecider<'T>(Nullable count, f, 1))

        [<CompiledName "Iterate">]
        let inline iter f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new Folder<'T,unit> (()) with
                    override this.ProcessNext value =
                        f value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Iterate2">]
        let inline iter2 (f:'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,unit,IEnumerator<'U>> ((),source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            f value this.State.Current
                        else
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "IterateIndexed2">]
        let inline iteri2 (f:int->'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,unit,Values<int,IEnumerator<'U>>>((),Values<_,_>(0,source2.GetEnumerator())) with
                    override this.ProcessNext value =
                        if this.State._2.MoveNext() then
                            f this.State._1 value this.State._2.Current
                            this.State._1 <- this.State._1 + 1
                            Unchecked.defaultof<_>
                        else
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                            Unchecked.defaultof<_>
                    override this.OnComplete _ = () 
                    override this.OnDispose () = this.State._2.Dispose () })

        [<CompiledName "TryHead">]
        let tryHead (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        this.Result <- Some value
                        (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Head">]
        let head (source:ISeq<_>) =
            match tryHead source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "IterateIndexed">]
        let inline iteri f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new FolderWithState<'T,unit,int> ((),0) with
                    override this.ProcessNext value =
                        f this.State value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Except">]
        let inline except (itemsToExclude: seq<'T>) (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            checkNonNull "itemsToExclude" itemsToExclude
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,Lazy<HashSet<'T>>>(next,lazy(HashSet<'T>(itemsToExclude,HashIdentity.Structural<'T>))) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Value.Add input && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Exists">]
        let inline exists f (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- true
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Exists2">]
        let inline exists2 (predicate:'T->'U->bool) (source1:ISeq<'T>) (source2: ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(false,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if predicate value this.State.Current then
                                this.Result <- true
                                (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        else
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Contains">]
        let inline contains element (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if element = value then
                            this.Result <- true
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll">]
        let inline forall predicate (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (true) with
                    override this.ProcessNext value =
                        if not (predicate value) then
                            this.Result <- false
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll2">]
        let inline forall2 predicate (source1:ISeq<'T>) (source2:ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(true,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if not (predicate value this.State.Current) then
                                this.Result <- false
                                (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        else
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Filter">]
        let inline filter<'T> (f:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            if f input then TailCall.avoid (next.ProcessNext input)
                            else false } }

        [<CompiledName "Map">]
        let inline map<'T,'U> (f:'T->'U) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            TailCall.avoid (next.ProcessNext (f input)) } }

        [<CompiledName "MapIndexed">]
        let inline mapi f (source:ISeq<_>) =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,int>(next, -1) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State <- this.State  + 1
                            TailCall.avoid (next.ProcessNext (f this.State input)) } }

        [<CompiledName "Map2">]
        let inline map2<'T,'U,'V> (map:'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new TransformFactory<'T,'V>() with
                override __.Compose outOfBand pipeIdx (next:Activity<'V,'W>) =
                    upcast { new TransformWithPostProcessing<'T,'W, IEnumerator<'U>>(next, (source2.GetEnumerator ())) with
                        override this.ProcessNext input =
                            if this.State.MoveNext () then
                                TailCall.avoid (next.ProcessNext (map input this.State.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnComplete _ = () 
                        override this.OnDispose () = this.State.Dispose () }}

        [<CompiledName "MapIndexed2">]
        let inline mapi2<'T,'U,'V> (map:int->'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new TransformFactory<'T,'V>() with
                override __.Compose<'W> outOfBand pipeIdx next =
                    upcast { new TransformWithPostProcessing<'T,'W, Values<int,IEnumerator<'U>>>(next, Values<_,_>(-1,source2.GetEnumerator ())) with
                        override this.ProcessNext t =
                            let idx : byref<_> = &this.State._1
                            let u = this.State._2
                            if u.MoveNext () then
                                idx <- idx + 1
                                TailCall.avoid (next.ProcessNext (map idx t u.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnDispose () = this.State._2.Dispose ()
                        override this.OnComplete _ = () }}

        [<CompiledName "Map3">]
        let inline map3<'T,'U,'V,'W>(map:'T->'U->'V->'W) (source1:ISeq<'T>) (source2:ISeq<'U>) (source3:ISeq<'V>) : ISeq<'W> =
            source1.PushTransform { new TransformFactory<'T,'W>() with
                override __.Compose<'X> outOfBand pipeIdx next =
                    upcast { new TransformWithPostProcessing<'T,'X,Values<IEnumerator<'U>,IEnumerator<'V>>>(next,Values<_,_>(source2.GetEnumerator(),source3.GetEnumerator())) with
                        override this.ProcessNext t =
                            let u = this.State._1
                            let v = this.State._2
                            if u.MoveNext() && v.MoveNext ()  then
                                TailCall.avoid (next.ProcessNext (map t u.Current v.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnComplete _ = () 
                        override this.OnDispose () = 
                            this.State._1.Dispose ()
                            this.State._2.Dispose () }}

        [<CompiledName "CompareWith">]
        let inline compareWith (f:'T->'T->int) (source1:ISeq<'T>) (source2:ISeq<'T>) : int =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,int,IEnumerator<'T>>(0,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if not (this.State.MoveNext()) then
                            this.Result <- 1
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        else
                            let c = f value this.State.Current
                            if c <> 0 then
                                this.Result <- c
                                (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        if this.Result = 0 && this.State.MoveNext() then
                            this.Result <- -1
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Choose">]
        let inline choose (f:'T->option<'U>) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            match f input with
                            | Some value -> TailCall.avoid (next.ProcessNext value)
                            | None       -> false } }

        [<CompiledName "Distinct">]
        let inline distinct (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,HashSet<'T>>(next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add input && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "DistinctBy">]
        let inline distinctBy (keyf:'T->'Key) (source:ISeq<'T>) :ISeq<'T>  when 'Key:equality =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,HashSet<'Key>> (next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add (keyf input) && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Max">]
        let inline max (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        elif value > this.Result then
                            this.Result <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "MaxBy">]
        let inline maxBy (f:'T->'U) (source:ISeq<'T>) : 'T when 'U:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof<'U>)) with
                    override this.ProcessNext value =
                        match this.State._1, f value with
                        | true, valueU ->
                            this.State._1 <- false
                            this.State._2 <- valueU
                            this.Result <- value
                        | false, valueU when valueU > this.State._2 ->
                            this.State._2 <- valueU
                            this.Result <- value
                        | _ -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "Min">]
        let inline min (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        elif value < this.Result then
                            this.Result <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "MinBy">]
        let inline minBy (f:'T->'U) (source:ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof< 'U>)) with
                    override this.ProcessNext value =
                        match this.State._1, f value with
                        | true, valueU ->
                            this.State._1 <- false
                            this.State._2 <- valueU
                            this.Result <- value
                        | false, valueU when valueU < this.State._2 ->
                            this.State._2 <- valueU
                            this.Result <- value
                        | _ -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "Pairwise">]
        let pairwise (source:ISeq<'T>) : ISeq<'T*'T> =
            source.PushTransform { new TransformFactory<'T,'T * 'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'U,Values<bool,'T>>(next, Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                            // member this.isFirst   = this.State._1
                            // member this.lastValue = this.State._2
                            override this.ProcessNext (input:'T) : bool =
                                if this.State._1  then
                                    this.State._2 <- input
                                    this.State._1 <- false
                                    false
                                else
                                    let currentPair = this.State._2, input
                                    this.State._2 <- input
                                    TailCall.avoid (next.ProcessNext currentPair) }}

        [<CompiledName "Reduce">]
        let inline reduce (f:'T->'T->'T) (source: ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        else
                            this.Result <- f this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName("Concat")>]
        let concat (sources:ISeq<#ISeq<'T>>) : ISeq<'T> =
            Upcast.seq (ThinConcatEnumerable (sources, id))

        [<CompiledName "Scan">]
        let inline scan (folder:'State->'T->'State) (initialState:'State) (source:ISeq<'T>) :ISeq<'State> =
            let head = singleton initialState
            let tail = 
                source.PushTransform { new TransformFactory<'T,'State>() with
                    override __.Compose _ _ next =
                        upcast { new Transform<'T,'V,'State>(next, initialState) with
                            override this.ProcessNext (input:'T) : bool =
                                this.State <- folder this.State input
                                TailCall.avoid (next.ProcessNext this.State) } }
            concat (ofSeq [| head ; tail |])

        [<CompiledName "Skip">]
        let skip (skipCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    let mutable this = Unchecked.defaultof<TransformWithPostProcessing<'T,'U,int>>
                    let skipper = 
                        { new TransformWithPostProcessing<'T,'U,int>(next,(*count*)0) with
                            // member this.count = this.State
                            override this.ProcessNext (input:'T) : bool =
                                if this.State < skipCount then
                                    this.State <- this.State + 1
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)

                            override this.OnComplete _ =
                                if this.State < skipCount then
                                    let x = skipCount - this.State
                                    invalidOpFmt "{0}\ntried to skip {1} {2} past the end of the seq"
                                        [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                            override this.OnDispose () = ()

                        interface ISkipable with
                            member __.CanSkip () =
                                if this.State < skipCount then
                                    this.State <- this.State + 1
                                    true
                                else
                                    false }
                    this <- skipper
                    upcast this }

        [<CompiledName "SkipWhile">]
        let inline skipWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,bool>(next,true) with
                        // member this.skip = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State then
                                this.State <- predicate input
                                if this.State then
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)
                            else
                                TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Sum">]
        let inline sum (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new Folder<'T,'T> (LanguagePrimitives.GenericZero) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "SumBy">]
        let inline sumBy (f:'T->'U) (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new Folder<'T,'U> (LanguagePrimitives.GenericZero<'U>) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result (f value)
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Take">]
        let take (takeCount:int) (source:ISeq<'T>) : ISeq<'T> =
            if takeCount < 0 then invalidArgInputMustBeNonNegative "count" takeCount
            elif takeCount = 0 then empty
            else
                source.PushTransform { new TransformFactory<'T,'T>() with
                    member __.Compose outOfBand pipelineIdx next =
                        if takeCount = 0 then
                            outOfBand.StopFurtherProcessing pipelineIdx

                        upcast { new TransformWithPostProcessing<'T,'U,int>(next,(*count*)0) with
                            // member this.count = this.State
                            override this.ProcessNext (input:'T) : bool =
                                if this.State < takeCount then
                                    this.State <- this.State + 1
                                    if this.State = takeCount then
                                        outOfBand.StopFurtherProcessing pipelineIdx
                                    TailCall.avoid (next.ProcessNext input)
                                else
                                    outOfBand.StopFurtherProcessing pipelineIdx
                                    false

                            override this.OnComplete terminatingIdx =
                                if terminatingIdx < pipelineIdx && this.State < takeCount then
                                    let x = takeCount - this.State
                                    invalidOpFmt "tried to take {0} {1} past the end of the seq"
                                        [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                            override this.OnDispose () = () }}

        [<CompiledName "TakeWhile">]
        let inline takeWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext (input:'T) : bool =
                            if predicate input then
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false }}

        [<CompiledName "Tail">]
        let tail (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose _ _ next =
                    upcast { new TransformWithPostProcessing<'T,'V,bool>(next,true) with
                        // member this.isFirst = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State then
                                this.State <- false
                                false
                            else
                                TailCall.avoid (next.ProcessNext input)
                        override this.OnComplete _ =
                            if this.State then
                                invalidArg "source" (SR.GetString SR.notEnoughElements) 
                        override this.OnDispose () = () }}

        [<CompiledName "Truncate">]
        let truncate (truncateCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast { new Transform<'T,'U,int>(next,(*count*)0) with
                        // member this.count = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State < truncateCount then
                                this.State <- this.State + 1
                                if this.State = truncateCount then
                                    outOfBand.StopFurtherProcessing pipeIdx
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false }}

        [<CompiledName "Indexed">]
        let indexed source =
            mapi (fun i x -> i,x) source

        [<CompiledName "TryItem">]
        let tryItem index (source:ISeq<'T>) =
            if index < 0 then None else
                source.PushTransform { new TransformFactory<'T,'T>() with
                    override __.Compose _ _ next =
                        let mutable this = Unchecked.defaultof<Transform<'T,'U,int>>
                        let skipper = 
                            { new Transform<'T,'U,int>(next,(*count*)0) with
                                // member this.count = this.State
                                override this.ProcessNext (input:'T) : bool =
                                    if this.State < index then
                                        this.State <- this.State + 1
                                        false
                                    else
                                        TailCall.avoid (next.ProcessNext input)

                            interface ISkipable with
                                member __.CanSkip () =
                                    if this.State < index then
                                        this.State <- this.State + 1
                                        true
                                    else
                                        false }
                        this <- skipper
                        upcast this }
                |> tryHead

        [<CompiledName "TryPick">]
        let inline tryPick f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'U>> (None) with
                    override this.ProcessNext value =
                        match f value with
                        | (Some _) as some ->
                            this.Result <- some
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        | None -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFind">]
        let inline tryFind f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- Some value
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFindIndex">]
        let inline tryFindIndex (predicate:'T->bool) (source:ISeq<'T>) : int option =
            source.Fold (fun pipeIdx ->
                upcast { new FolderWithState<'T, Option<int>, int>(None, 0) with
                    // member this.index = this.State
                    override this.ProcessNext value =
                        if predicate value then
                            this.Result <- Some this.State
                            (Upcast.outOfBand this).StopFurtherProcessing pipeIdx
                        else
                            this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryLast">]
        let tryLast (source:ISeq<'T>) : 'T option =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,option<'T>,Values<bool,'T>>(None,Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                    // member this.noItems = this.State._1
                    // memebr this.last    = this.State._2
                    override this.ProcessNext value =
                        if this.State._1 then
                            this.State._1 <- false
                        this.State._2 <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        if not this.State._1 then
                            this.Result <- Some this.State._2
                    override this.OnDispose () = () })

        [<CompiledName("Last")>]
        let last (source:ISeq<_>) =
            match tryLast source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "Windowed">]
        let windowed (windowSize:int) (source:ISeq<'T>) : ISeq<'T[]> =
            if windowSize <= 0 then
                invalidArgFmt "windowSize" "{0}\nwindowSize = {1}" [|SR.GetString SR.inputMustBePositive; windowSize|]

            source.PushTransform { new TransformFactory<'T,'T[]>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast {
                        new Transform<'T,'U,Values<'T[],int,int>>(next,Values<'T[],int,int>(Array.zeroCreateUnchecked windowSize, 0, windowSize-1)) with
                            override this.ProcessNext (input:'T) : bool =
                                let circularBuffer     =  this.State._1
                                let idx     : byref<_> = &this.State._2
                                let priming : byref<_> = &this.State._3

                                circularBuffer.[idx] <- input

                                idx <- idx + 1
                                if idx = windowSize then
                                    idx <- 0

                                if priming > 0 then
                                    priming <- priming - 1
                                    false
                                elif windowSize < 32 then
                                    let idx = idx
                                    let window :'T [] = Array.init windowSize (fun i -> circularBuffer.[(idx+i) % windowSize]: 'T)
                                    TailCall.avoid (next.ProcessNext window)
                                else
                                    let window = Array.zeroCreateUnchecked windowSize
                                    Array.Copy (circularBuffer, idx, window, 0, windowSize - idx)
                                    Array.Copy (circularBuffer, 0, window, windowSize - idx, idx)
                                    TailCall.avoid (next.ProcessNext window) }}

        [<CompiledName("Append")>]
        let append (source1:ISeq<'T>) (source2: ISeq<'T>) : ISeq<'T> =
            match source1 with
            | :? EnumerableBase<'T> as s -> s.Append source2
            | _ -> Upcast.seq (new AppendEnumerable<_>([source2; source1]))

        [<CompiledName "Delay">]
        let delay (delayed:unit->ISeq<'T>) =
            Upcast.seq (DelayedEnumerable (delayed, 1))

        module internal GroupBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    upcast { new FolderWithPostProcessing<'T,ISeq<'Key*ISeq<'T>>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
                        override this.ProcessNext v =
                            let safeKey = keyf v
                            match this.State.TryGetValue safeKey with
                            | false, _ ->
                                let prev = ResizeArray ()
                                this.State.[safeKey] <- prev
                                prev.Add v
                            | true, prev -> prev.Add v
                            Unchecked.defaultof<_> (* return value unused in Fold context *)

                        override this.OnComplete _ =
                            let maxWastage = 4
                            for value in this.State.Values do
                                if value.Capacity - value.Count > maxWastage then value.TrimExcess ()

                            this.Result <-
                                this.State
                                |> ofSeq
                                |> map (fun kv -> getKey kv.Key, ofResizeArrayUnchecked kv.Value)

                        override this.OnDispose () = () })

            let inline byVal (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> keyf id source) 

            let inline byRef (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (keyf >> Value) (fun v -> v._1) source)
        
        [<CompiledName("GroupByVal")>]
        let inline groupByVal<'T,'Key when 'Key : equality and 'Key : struct> (keyf:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byVal keyf source

        [<CompiledName("GroupByRef")>]
        let inline groupByRef<'T,'Key when 'Key : equality and 'Key : not struct> (keyf:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byRef keyf source

        module CountBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    upcast { new FolderWithPostProcessing<'T,ISeq<'Key*int>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
                        override this.ProcessNext v =
                            let safeKey = keyf v
                            this.State.[safeKey] <- 
                                match this.State.TryGetValue safeKey with
                                | true, prev -> prev + 1
                                | false, _   -> 1
                            Unchecked.defaultof<_> (* return value unused in Fold context *)

                        override this.OnComplete _ =
                            this.Result <-
                                this.State
                                |> ofSeq
                                |> map (fun group -> getKey group.Key, group.Value)

                        override this.OnDispose () = () })

            let inline byVal (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> keyf id source) 

            let inline byRef (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (keyf >> Value) (fun v -> v._1) source)
        
        [<CompiledName("CountByVal")>]
        let inline countByVal<'T,'Key when 'Key : equality and 'Key : struct>  (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byVal projection source 

        [<CompiledName("CountByRef")>]
        let inline countByRef<'T,'Key when 'Key : equality and 'Key : not struct> (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byRef projection source 

        [<CompiledName("Length")>]
        let length (source:ISeq<'T>)  =
            match source with
            | :? EnumerableBase<'T> as s -> s.Length ()
            | :? list<'T> as l -> l.Length
            | _ -> length source

        [<CompiledName("ToArray")>]
        let toArray (source:ISeq<'T>)  =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,array<'T>,_>(Unchecked.defaultof<_>,ResizeArray ()) with
                    override this.ProcessNext v =
                        this.State.Add v
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        this.Result <- this.State.ToArray ()
                    override this.OnDispose () = () })

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceBy keyf array
                ofArray array)

        [<CompiledName("Sort")>]
        let sort source =
            delay (fun () -> 
                let array = source |> toArray
                Array.stableSortInPlace array
                ofArray array)

        [<CompiledName("SortWith")>]
        let sortWith f source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceWith f array
                ofArray array)

        [<CompiledName("Reverse")>]
        let rev source =
            delay (fun () ->
                let array = source |> toArray
                Array.Reverse array
                ofArray array)

        [<CompiledName("Permute")>]
        let permute f (source:ISeq<_>) =
            delay (fun () ->
                source
                |> toArray
                |> Array.permute f
                |> ofArray)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> f (source:ISeq<'T>) (acc:'State) : ISeq<'State> =
            delay (fun () ->
                let array = source |> toArray
                Array.scanSubRight f array 0 (array.Length - 1) acc
                |> ofArray)

        let inline foldArraySubRight f (arr: 'T[]) start fin acc =
            let mutable state = acc
            for i = fin downto start do
                state <- f arr.[i] state
            state

        [<CompiledName("FoldBack")>]
        let inline foldBack<'T,'State> f (source: ISeq<'T>) (x:'State) =
            let arr = toArray source
            let len = arr.Length
            foldArraySubRight f arr 0 (len - 1) x

        [<CompiledName("Zip")>]
        let zip source1 source2 =
            map2 (fun x y -> x,y) source1 source2

        [<CompiledName("FoldBack2")>]
        let inline foldBack2<'T1,'T2,'State> f (source1:ISeq<'T1>) (source2:ISeq<'T2>) (x:'State) =
            let zipped = zip source1 source2
            foldBack ((<||) f) zipped x

        [<CompiledName("ReduceBack")>]
        let inline reduceBack f (source:ISeq<'T>) =
            let arr = toArray source
            match arr.Length with
            | 0 -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | len -> foldArraySubRight f arr 0 (len - 2) arr.[len - 1]

        [<Sealed>]
        type CachedSeq<'T>(source:ISeq<'T>) =
            let sync = obj ()

            // Wrap a seq to ensure that it is enumerated just once and only as far as is necessary.
            //
            // This code is required to be thread safe.
            // The necessary calls should be called at most once (include .MoveNext() = false).
            // The enumerator should be disposed (and dropped) when no longer required.
            //------
            // The state is (prefix,enumerator) with invariants:
            //   * the prefix followed by elts from the enumerator are the initial sequence.
            //   * the prefix contains only as many elements as the longest enumeration so far.

            let prefix = ResizeArray ()

            let mutable started = false
            let mutable enumeratorR = None : option<IEnumerator<'T>>
                
            // function should only be called from within the lock
            let oneStepTo i =
                // If possible, step the enumeration to prefix length i (at most one step).
                // Be speculative, since this could have already happened via another thread.
                if not (i < prefix.Count) then // is a step still required?
                    // If not yet started, start it (create enumerator).
                    if not started then
                        started <- true
                        enumeratorR <- Some (source.GetEnumerator())

                    match enumeratorR with
                    | Some enumerator when enumerator.MoveNext() ->
                        prefix.Add enumerator.Current
                    | Some enumerator ->
                        enumerator.Dispose () // Move failed, dispose enumerator,
                        enumeratorR <- None   // drop it and record finished.
                    | _ -> ()

            let unfolding i =
                // i being the next position to be returned
                // A lock is needed over the reads to prefix.Count since the list may be being resized
                // NOTE: we could change to a reader/writer lock here
                lock sync (fun () ->
                    if i < prefix.Count then
                        Some (prefix.[i], i+1)
                    else
                        oneStepTo i
                        if i < prefix.Count then
                            Some (prefix.[i], i+1)
                        else
                            None)

            let cached = Upcast.seq (new UnfoldEnumerable<'T,'T,int>(unfolding, 0, IdentityFactory.Instance, 1))

            interface System.IDisposable with
                member __.Dispose() =
                    lock sync (fun () ->
                       prefix.Clear()

                       enumeratorR
                       |> Option.iter IEnumerator.dispose

                       started <- false
                       enumeratorR <- None)

            interface System.Collections.Generic.IEnumerable<'T> with
                member __.GetEnumerator() = cached.GetEnumerator()

            interface System.Collections.IEnumerable with
                member __.GetEnumerator() = (Upcast.enumerableNonGeneric cached).GetEnumerator()

            interface ISeq<'T> with
                member __.PushTransform next = cached.PushTransform next
                member __.Fold f = cached.Fold f

            member this.Clear() = (this :> IDisposable).Dispose ()

        [<CompiledName("Cache")>]
        let cache (source:ISeq<'T>) : ISeq<'T> =
            Upcast.seq (new CachedSeq<_> (source))

        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

        [<CompiledName("AllPairs")>]
        let allPairs (source1:ISeq<'T1>) (source2:ISeq<'T2>) : ISeq<'T1 * 'T2> =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            let cached = cache source2
            source1 |> collect (fun x -> cached |> map (fun y -> x,y))

        [<CompiledName("ToList")>]
        let toList (source : ISeq<'T>) =
            match source with
            | :? list<'T> as lst -> lst
            | _ -> Microsoft.FSharp.Primitives.Basics.List.ofISeq source

        [<CompiledName("Replicate")>]
        let replicate count x =
            if count < 0 then raise (ArgumentOutOfRangeException "count")
            Upcast.seq (new InitEnumerable<'T,'T>(Nullable count, (fun _ -> x), IdentityFactory.Instance, 1))

        [<CompiledName("IsEmpty")>]
        let isEmpty (source : ISeq<'T>)  =
            match source with
            | :? list<'T> as lst -> lst.IsEmpty
            | _ ->
                use ie = source.GetEnumerator()
                not (ie.MoveNext())

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) : ISeq<'T> =
            match source with
            | :? ISeq<'T> as s -> s
            | :? ISeq<obj> as s -> s |> map unbox // covariant on ref types
            | _ -> 
                mkSeq (fun () -> IEnumerator.cast (source.GetEnumerator())) |> ofSeq

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : ISeq<'T>) : ISeq<'T[]> =
            if chunkSize <= 0 then invalidArgFmt "chunkSize" "{0}\nchunkSize = {1}"
                                    [|SR.GetString SR.inputMustBePositive; chunkSize|]

            source.PushTransform { new TransformFactory<'T,'T[]>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast {
                        new TransformWithPostProcessing<'T,'U,Values<'T[],int>>(next,Values<'T[],int>(Array.zeroCreateUnchecked chunkSize, 0)) with
                            override this.ProcessNext (input:'T) : bool =
                                this.State._1.[this.State._2] <- input
                                this.State._2 <- this.State._2 + 1
                                if this.State._2 <> chunkSize then false
                                else
                                    this.State._2 <- 0
                                    let tmp = this.State._1
                                    this.State._1 <- Array.zeroCreateUnchecked chunkSize
                                    TailCall.avoid (next.ProcessNext tmp)
                            override this.OnComplete _ =
                                if this.State._2 > 0 then
                                    System.Array.Resize (&this.State._1, this.State._2)
                                    next.ProcessNext this.State._1 |> ignore
                            override this.OnDispose () = () }}

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator()) |> ofSeq

        [<CompiledName("SplitInto")>]
        let splitInto count (source:ISeq<'T>) : ISeq<'T[]> =
            if count <= 0 then invalidArgFmt "count" "{0}\ncount = {1}"
                                [|SR.GetString SR.inputMustBePositive; count|]
            mkDelayedSeq (fun () ->
                source |> toArray |> Array.splitInto count :> seq<_>)

        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("Find")>]
        let find f source =
            match tryFind f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("FindIndex")>]
        let findIndex p (source:ISeq<_>) =
            use ie = source.GetEnumerator()
            let rec loop i =
                if ie.MoveNext() then
                    if p ie.Current then
                        i
                    else loop (i+1)
                else
                    indexNotFound()
            loop 0

        [<CompiledName("FindBack")>]
        let findBack f source =
            source |> toArray |> Array.findBack f

        [<CompiledName("FindIndexBack")>]
        let findIndexBack f source =
            source |> toArray |> Array.findIndexBack f

        [<CompiledName("Pick")>]
        let pick f source  =
            match tryPick f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (f: 'State -> 'T -> 'Result * 'State) acc source =
            let arr,state = source |> toArray |> Array.mapFold f acc
            ofArray arr, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (f: 'T -> 'State -> 'Result * 'State) source acc =
            let array = source |> toArray
            let arr,state = Array.mapFoldBack f array acc
            ofArray arr, state

        let rec nth index (e : IEnumerator<'T>) =
            if not (e.MoveNext()) then
              let shortBy = index + 1
              invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; shortBy; (if shortBy = 1 then "element" else "elements")|]
            if index = 0 then e.Current
            else nth (index-1) e

        [<CompiledName("Item")>]
        let item i (source : ISeq<'T>) =
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            nth i e

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            let inline compareDescending a b = compare b a
            sortWith compareDescending source

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending keyf source =
            let inline compareDescending a b = compare (keyf b) (keyf a)
            sortWith compareDescending source

        [<CompiledName("TryFindBack")>]
        let tryFindBack f (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindBack f

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack f (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindIndexBack f

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            map2 (fun x (y,z) -> x,y,z) source1 (zip source2 source3)
