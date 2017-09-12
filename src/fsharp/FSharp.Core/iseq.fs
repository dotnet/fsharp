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

        /// Base classes and helpers for object expressions that represent the Activitys in
        /// the ISeq chain
        module Core =
            /// Single value Tuple.
            /// Allows null reference type values as key in Dictionary.
            [<Struct; NoComparison; NoEquality>]
            type internal Value<'T> =
                val mutable _1 : 'T
                new (a:'T) = { _1 = a }

            /// 2-Value Tuple.
            [<Struct; NoComparison; NoEquality>]
            type Values<'T1,'T2> =
                val mutable _1 : 'T1
                val mutable _2 : 'T2
                new (a:'T1, b: 'T2) = { _1 = a;  _2 = b }

            /// 3-Value Tuple.
            [<Struct; NoComparison; NoEquality>]
            type internal Values<'T1,'T2,'T3> =
                val mutable _1 : 'T1
                val mutable _2 : 'T2
                val mutable _3 : 'T3
                new (a:'T1, b:'T2, c:'T3) = { _1 = a; _2 = b; _3 = c }

            /// Base class for chaining mapping or filtering operation within the ISeq pipeline
            [<AbstractClass>]
            type internal Transform<'T,'U,'State> =
                inherit Activity<'T,'U>

                new (next:Activity, initState:'State) = {
                    State = initState
                    Next = next
                }
                
                val mutable State : 'State 
                val Next : Activity 
                
                override this.ChainComplete terminatingIdx =
                    this.Next.ChainComplete terminatingIdx
                override this.ChainDispose () =
                    this.Next.ChainDispose ()

            /// Base class for chaining mapping or filtering operation that require some post processing
            /// (i.e. Disposal and/or post conditional checks) within the ISeq pipeline
            [<AbstractClass>]
            type internal TransformWithPostProcessing<'T,'U,'State>(next:Activity, initState:'State) =
                inherit Transform<'T,'U,'State>(next, initState)

                abstract OnComplete : pipeIdx:PipeIdx -> unit
                abstract OnDispose  : unit -> unit

                override this.ChainComplete terminatingIdx =
                    this.OnComplete terminatingIdx
                    this.Next.ChainComplete terminatingIdx
                override this.ChainDispose ()  =
                    try     this.OnDispose ()
                    finally this.Next.ChainDispose ()

            /// Base class for folding operation within the ISeq pipeline
            [<AbstractClass>]
            type FolderWithState<'T,'Result,'State> =
                inherit Folder<'T,'Result>

                val mutable State : 'State

                new (initalResult,initState) = {
                    inherit Folder<'T,'Result>(initalResult)
                    State = initState
                }

            /// Base class for folding operation that require some post processing
            /// (i.e. Disposal and/or post conditional checks) within the ISeq pipeline
            [<AbstractClass>]
            type FolderWithPostProcessing<'T,'Result,'State>(initResult,initState) =
                inherit FolderWithState<'T,'Result,'State>(initResult,initState)

                abstract OnComplete : pipeIdx:PipeIdx -> unit
                abstract OnDispose : unit -> unit

                override this.ChainComplete terminatingIdx =
                    this.OnComplete terminatingIdx
                override this.ChainDispose () =
                    this.OnDispose ()

        open Core

        module internal TailCall =
            // used for performance reasons; these are not recursive calls, so should be safe
            // ** it should be noted that potential changes to the f# compiler may render this function
            // ineffective - i.e. this function shouldn't do anything, it just happens to be complex
            // enough to fool the compiler into erasing it's behaviour, but making the calling
            // function invalid for tail call optimization (No invesigation beyond observing behaviour) **
            let inline avoid boolean = match boolean with true -> true | false -> false

        let inline internal valueComparer<'T when 'T : equality> () =
            let c = HashIdentity.Structural<'T>
            { new IEqualityComparer<Value<'T>> with
                    member __.GetHashCode o    = c.GetHashCode o._1
                    member __.Equals (lhs,rhs) = c.Equals (lhs._1, rhs._1) }

        /// Usually the implementation of GetEnumerator is handled by the ISeq<'T>.Fold operation,
        /// but there can be cases where it is more efficent to provide a specific IEnumerator, and
        /// only defer to the ISeq<'T> interface for its specific operations (this is the case with
        /// the scan function)
        [<AbstractClass>]
        type internal PreferGetEnumerator<'T>() =
            inherit EnumerableBase<'T>()

            abstract GetEnumerator: unit -> IEnumerator<'T>
            abstract GetSeq : unit -> ISeq<'T>

            interface IEnumerable<'T> with
                member this.GetEnumerator () : IEnumerator<'T> = this.GetEnumerator ()

            interface ISeq<'T> with
                member this.PushTransform<'U> (next:ITransformFactory<'T,'U>) : ISeq<'U> = (this.GetSeq()).PushTransform next
                member this.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) : 'Result = (this.GetSeq()).Fold f

        [<CompiledName "Empty">]
        let internal empty<'T> = Microsoft.FSharp.Collections.SeqComposition.Core.EmptyEnumerable<'T>.Instance

        [<CompiledName("Singleton")>]
        let internal singleton<'T> (value:'T) : ISeq<'T> = new SingletonEnumerable<_>(value) :> _

        /// wraps a ResizeArray in the ISeq framework. Care must be taken that the underlying ResizeArray
        /// is not modified whilst it can be accessed as the ISeq, so check on version is performed.
        /// i.e. usually iteration on calls the enumerator provied by GetEnumerator ensure that the
        /// list hasn't been modified (throwing an exception if it has), but such a check is not
        /// performed in this case. If you want this functionality, then use the ofSeq function instead.
        [<CompiledName "OfResizeArrayUnchecked">]
        let internal ofResizeArrayUnchecked (source:ResizeArray<'T>) : ISeq<'T> =
            ThinResizeArrayEnumerable<'T> source :> _

        [<CompiledName "OfArray">]
        let internal ofArray (source:array<'T>) : ISeq<'T> =
            checkNonNull "source" source
            ThinArrayEnumerable<'T> source :> _

        [<CompiledName "OfList">]
        let internal ofList (source:list<'T>) : ISeq<'T> =
            source :> _

        [<CompiledName "OfSeq">]
        let ofSeq (source:seq<'T>) : ISeq<'T> =
            match source with
            | :? ISeq<'T>  as seq   -> seq
            | :? array<'T> as array -> ofArray array
            | null                  -> nullArg "source"
            | _                     -> ThinEnumerable<'T> source :> _

        [<CompiledName "Average">]
        let inline average (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,int> (LanguagePrimitives.GenericZero, 0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'T> this.Result this.State 
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "AverageBy">]
        let inline averageBy (f:'T->'U) (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'U,int>(LanguagePrimitives.GenericZero,0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result (f value)
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'U> this.Result this.State
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "ExactlyOne">]
        let internal exactlyOne (source:ISeq<'T>) : 'T =
            source.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,'T,Values<bool, bool>>(Unchecked.defaultof<'T>, Values<bool,bool>(true, false)) with
                    override this.ProcessNext value =
                        if this.State._1 then
                            this.State._1 <- false
                            this.Result <- value
                        else
                            this.State._2 <- true
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        elif this.State._2 then
                            invalidArg "source" (SR.GetString SR.inputSequenceTooLong)
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "Fold">]
        let inline internal fold<'T,'State> (f:'State->'T->'State) (seed:'State) (source:ISeq<'T>) : 'State =
            source.Fold (fun _ ->
                { new Folder<'T,'State>(seed) with
                    override this.ProcessNext value =
                        this.Result <- f this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Fold2">]
        let inline internal fold2<'T1,'T2,'State> (folder:'State->'T1->'T2->'State) (state:'State) (source1:ISeq<'T1>) (source2: ISeq<'T2>) =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<_,'State,IEnumerator<'T2>>(state,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            this.Result <- folder this.Result value this.State.Current
                        else
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    
                    override this.OnDispose () = this.State.Dispose () } :> _)

        [<CompiledName "Unfold">]
        let internal unfold (generator:'State->option<'T * 'State>) (state:'State) : ISeq<'T> =
            new UnfoldEnumerable<'T,'T,'State>(generator, state, IdentityFactory.Instance, 1) :> _

        [<CompiledName "InitializeInfinite">]
        let internal initInfinite<'T> (f:int->'T) : ISeq<'T> =
            new InitEnumerableDecider<'T>(Nullable (), f, 1) :> _

        [<CompiledName "Initialize">]
        let internal init<'T> (count:int) (f:int->'T) : ISeq<'T> =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            elif count = 0 then empty else
            new InitEnumerableDecider<'T>(Nullable count, f, 1) :> _

        [<CompiledName "Iterate">]
        let inline internal iter f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new Folder<'T,unit> (()) with
                    override this.ProcessNext value =
                        f value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Iterate2">]
        let inline internal iter2 (f:'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,unit,IEnumerator<'U>> ((),source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            f value this.State.Current
                        else
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    
                    override this.OnDispose () = this.State.Dispose () } :> _)

        [<CompiledName "IterateIndexed2">]
        let inline internal iteri2 (f:int->'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,unit,Values<int,IEnumerator<'U>>>((),Values<_,_>(0,source2.GetEnumerator())) with
                    override this.ProcessNext value =
                        if this.State._2.MoveNext() then
                            f this.State._1 value this.State._2.Current
                            this.State._1 <- this.State._1 + 1
                            Unchecked.defaultof<_>
                        else
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                            Unchecked.defaultof<_>
                    
                    override this.OnComplete _ = () 
                    
                    override this.OnDispose () = this.State._2.Dispose () } :> _)

        [<CompiledName "TryHead">]
        let internal tryHead (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        this.Result <- Some value
                        (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Head">]
        let internal head (source:ISeq<_>) =
            match tryHead source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "IterateIndexed">]
        let inline internal iteri f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new FolderWithState<'T,unit,int> ((),0) with
                    override this.ProcessNext value =
                        f this.State value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) } :> _)

        [<CompiledName "Except">]
        let inline internal except (itemsToExclude: seq<'T>) (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            checkNonNull "itemsToExclude" itemsToExclude
            source.PushTransform { new ITransformFactory<'T,'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,Lazy<HashSet<'T>>>(next,lazy(HashSet<'T>(itemsToExclude,HashIdentity.Structural<'T>))) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Value.Add input && TailCall.avoid (next.ProcessNext input) } :> _}

        [<CompiledName "Exists">]
        let inline internal exists f (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- true
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Exists2">]
        let inline internal exists2 (predicate:'T->'U->bool) (source1:ISeq<'T>) (source2: ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(false,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if predicate value this.State.Current then
                                this.Result <- true
                                (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        else
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    
                    override this.OnDispose () = this.State.Dispose () } :> _)

        [<CompiledName "Contains">]
        let inline contains element (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if element = value then
                            this.Result <- true
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll">]
        let inline internal forall predicate (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, bool> (true) with
                    override this.ProcessNext value =
                        if not (predicate value) then
                            this.Result <- false
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll2">]
        let inline internal forall2 predicate (source1:ISeq<'T>) (source2:ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(true,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if not (predicate value this.State.Current) then
                                this.Result <- false
                                (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        else
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    
                    override this.OnDispose () = this.State.Dispose () } :> _)

        [<CompiledName "Filter">]
        let inline internal filter<'T> (f:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            if f input then TailCall.avoid (next.ProcessNext input)
                            else false } :> _}

        [<CompiledName "Map">]
        let inline internal map<'T,'U> (f:'T->'U) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new ITransformFactory<'T,'U> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            TailCall.avoid (next.ProcessNext (f input)) } :> _ }

        [<CompiledName "MapIndexed">]
        let inline internal mapi f (source:ISeq<_>) =
            source.PushTransform { new ITransformFactory<'T,'U> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,int>(next, -1) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State <- this.State  + 1
                            TailCall.avoid (next.ProcessNext (f this.State input)) } :> _ }

        [<CompiledName "Map2">]
        let inline internal map2<'T,'U,'V> (map:'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new ITransformFactory<'T,'V> with
                override __.Compose outOfBand pipeIdx (next:Activity<'V,'W>) =
                    { new TransformWithPostProcessing<'T,'W, IEnumerator<'U>>(next, (source2.GetEnumerator ())) with
                        override this.ProcessNext input =
                            if this.State.MoveNext () then
                                TailCall.avoid (next.ProcessNext (map input this.State.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        
                        override this.OnComplete _ = () 
                        
                        override this.OnDispose () = this.State.Dispose () } :> _ }

        [<CompiledName "MapIndexed2">]
        let inline internal mapi2<'T,'U,'V> (map:int->'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new ITransformFactory<'T,'V> with
                override __.Compose<'W> outOfBand pipeIdx next =
                    { new TransformWithPostProcessing<'T,'W, Values<int,IEnumerator<'U>>>(next, Values<_,_>(-1,source2.GetEnumerator ())) with
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
                        
                        override this.OnComplete _ = () } :> _ }

        [<CompiledName "Map3">]
        let inline internal map3<'T,'U,'V,'W>(map:'T->'U->'V->'W) (source1:ISeq<'T>) (source2:ISeq<'U>) (source3:ISeq<'V>) : ISeq<'W> =
            source1.PushTransform { new ITransformFactory<'T,'W> with
                override __.Compose<'X> outOfBand pipeIdx next =
                    { new TransformWithPostProcessing<'T,'X,Values<IEnumerator<'U>,IEnumerator<'V>>>(next,Values<_,_>(source2.GetEnumerator(),source3.GetEnumerator())) with
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
                            this.State._2.Dispose () } :> _ }

        [<CompiledName "CompareWith">]
        let inline internal compareWith (f:'T->'T->int) (source1:ISeq<'T>) (source2:ISeq<'T>) : int =
            source1.Fold (fun pipeIdx ->
                { new FolderWithPostProcessing<'T,int,IEnumerator<'T>>(0,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if not (this.State.MoveNext()) then
                            this.Result <- 1
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        else
                            let c = f value this.State.Current
                            if c <> 0 then
                                this.Result <- c
                                (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    
                    override this.OnComplete _ =
                        if this.Result = 0 && this.State.MoveNext() then
                            this.Result <- -1
                    
                    override this.OnDispose () = this.State.Dispose () } :> _)

        [<CompiledName "Choose">]
        let inline internal choose (f:'T->option<'U>) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new ITransformFactory<'T,'U> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            match f input with
                            | Some value -> TailCall.avoid (next.ProcessNext value)
                            | None       -> false } :> _ }

        [<CompiledName "Distinct">]
        let inline internal distinct (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            source.PushTransform { new ITransformFactory<'T,'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,HashSet<'T>>(next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add input && TailCall.avoid (next.ProcessNext input) } :> _}

        [<CompiledName "DistinctBy">]
        let inline internal distinctBy (keyf:'T->'Key) (source:ISeq<'T>) :ISeq<'T>  when 'Key:equality =
            source.PushTransform { new ITransformFactory<'T,'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,HashSet<'Key>> (next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add (keyf input) && TailCall.avoid (next.ProcessNext input) } :> _}

        [<CompiledName "Max">]
        let inline max (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "MaxBy">]
        let inline maxBy (f:'T->'U) (source:ISeq<'T>) : 'T when 'U:comparison =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof<'U>)) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "Min">]
        let inline min (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "MinBy">]
        let inline minBy (f:'T->'U) (source:ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof< 'U>)) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName "Pairwise">]
        let internal pairwise (source:ISeq<'T>) : ISeq<'T*'T> =
            source.PushTransform { new ITransformFactory<'T,'T*'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'U,Values<bool,'T>>(next, Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
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
                                    TailCall.avoid (next.ProcessNext currentPair) } :> _}

        [<CompiledName "Reduce">]
        let inline internal reduce (f:'T->'T->'T) (source: ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName("Concat")>]
        let internal concat (sources:ISeq<#ISeq<'T>>) : ISeq<'T> =
            ThinConcatEnumerable (sources, id) :> _

        (*
            Represents the following seq comprehension, but they don't work at this level

            seq {
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder 
                let mutable state = initialState
                yield state
                for item in enumerable do
                    state <- f.Invoke (state, item)
                    yield state }
        *)
        type internal ScanEnumerator<'T,'State>(folder:'State->'T->'State, initialState:'State, enumerable:seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder

            let mutable state      = 0 (*Pre-start*)
            let mutable enumerator = Unchecked.defaultof<IEnumerator<'T>>
            let mutable current    = initialState

            interface IEnumerator<'State> with
                member this.Current: 'State =
                    match state with
                    | 0(*PreStart*)      -> notStarted()
                    | 1(*GetEnumerator*) -> current
                    | 2(*MoveNext*)      -> current
                    | _(*Finished*)      -> alreadyFinished()

            interface IEnumerator with
                member this.Current : obj =
                    box (this:>IEnumerator<'State>).Current
                
                member this.MoveNext () : bool = 
                    match state with
                    | 0(*PreStart*) ->
                        state <- 1(*GetEnumerator*)
                        true
                    | 1(*GetEnumerator*) ->
                        enumerator <- enumerable.GetEnumerator ()
                        state <- 2(*MoveNext*)
                        (this:>IEnumerator).MoveNext ()
                    | 2(*MoveNext*) ->
                        if enumerator.MoveNext () then
                            current <- f.Invoke (current, enumerator.Current)
                            true
                        else
                            current <- Unchecked.defaultof<_>
                            state <- 3(*Finished*)
                            false
                    | _(*Finished*) -> alreadyFinished()
                    
                member this.Reset () : unit = noReset ()

            interface IDisposable with
                member this.Dispose(): unit = 
                    if isNotNull enumerator then
                        enumerator.Dispose ()

        [<CompiledName "Scan">]
        let internal scan (folder:'State->'T->'State) (initialState:'State) (source:ISeq<'T>) : ISeq<'State> =
            { new PreferGetEnumerator<'State>() with
                member this.GetEnumerator () =
                    new ScanEnumerator<'T,'State>(folder, initialState, source) :> _

                member this.GetSeq () = 
                    let head = singleton initialState
                    let tail = 
                        source.PushTransform { new ITransformFactory<'T,'State> with
                            override __.Compose _ _ next =
                                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder
                                { new Transform<'T,'V,'State>(next, initialState) with
                                    override this.ProcessNext (input:'T) : bool =
                                        this.State <- f.Invoke (this.State, input)
                                        TailCall.avoid (next.ProcessNext this.State) } :> _ }
                    concat (ofList [ head ; tail ]) } :> _

        [<CompiledName "Skip">]
        let internal skip (skipCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
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
                    this :> _ }

        [<CompiledName "SkipWhile">]
        let inline internal skipWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
                override __.Compose _ _ next =
                    { new Transform<'T,'V,bool>(next,true) with
                        // member this.skip = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State then
                                this.State <- predicate input
                                if this.State then
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)
                            else
                                TailCall.avoid (next.ProcessNext input) } :> _}

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
        let internal take (takeCount:int) (source:ISeq<'T>) : ISeq<'T> =
            if takeCount < 0 then invalidArgInputMustBeNonNegative "count" takeCount
            elif takeCount = 0 then empty
            else
                source.PushTransform { new ITransformFactory<'T,'T> with
                    member __.Compose outOfBand pipelineIdx next =
                        if takeCount = 0 then
                            outOfBand.StopFurtherProcessing pipelineIdx

                        { new TransformWithPostProcessing<'T,'U,int>(next,(*count*)0) with
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
                            
                            override this.OnDispose () = () } :> _}

        [<CompiledName "TakeWhile">]
        let inline internal takeWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
                member __.Compose outOfBand pipeIdx next =
                    { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext (input:'T) : bool =
                            if predicate input then
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false } :> _ }

        [<CompiledName "Tail">]
        let internal tail (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
                member __.Compose _ _ next =
                    { new TransformWithPostProcessing<'T,'V,bool>(next,true) with
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
                        
                        override this.OnDispose () = () } :> _ }

        [<CompiledName "Truncate">]
        let internal truncate (truncateCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new ITransformFactory<'T,'T> with
                member __.Compose outOfBand pipeIdx next =
                    { new Transform<'T,'U,int>(next,(*count*)0) with
                        // member this.count = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State < truncateCount then
                                this.State <- this.State + 1
                                if this.State = truncateCount then
                                    outOfBand.StopFurtherProcessing pipeIdx
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false } :> _ }

        [<CompiledName "Indexed">]
        let internal indexed source =
            mapi (fun i x -> i,x) source

        [<CompiledName "TryItem">]
        let internal tryItem index (source:ISeq<'T>) =
            if index < 0 then None else
                source.PushTransform { new ITransformFactory<'T,'T> with
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
                        this :> _ }
                |> tryHead

        [<CompiledName "TryPick">]
        let inline internal tryPick f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'U>> (None) with
                    override this.ProcessNext value =
                        match f value with
                        | (Some _) as some ->
                            this.Result <- some
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        | None -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFind">]
        let inline internal tryFind f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- Some value
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFindIndex">]
        let inline internal tryFindIndex (predicate:'T->bool) (source:ISeq<'T>) : int option =
            source.Fold (fun pipeIdx ->
                { new FolderWithState<'T, Option<int>, int>(None, 0) with
                    // member this.index = this.State
                    override this.ProcessNext value =
                        if predicate value then
                            this.Result <- Some this.State
                            (this :> IOutOfBand).StopFurtherProcessing pipeIdx
                        else
                            this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) } :> _)

        [<CompiledName "TryLast">]
        let internal tryLast (source:ISeq<'T>) : 'T option =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,option<'T>,Values<bool,'T>>(None,Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
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
                    
                    override this.OnDispose () = () } :> _)

        [<CompiledName("Last")>]
        let internal last (source:ISeq<_>) =
            match tryLast source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "Windowed">]
        let internal windowed (windowSize:int) (source:ISeq<'T>) : ISeq<'T[]> =
            if windowSize <= 0 then
                invalidArgFmt "windowSize" "{0}\nwindowSize = {1}" [|SR.GetString SR.inputMustBePositive; windowSize|]

            source.PushTransform { new ITransformFactory<'T,'T[]> with
                member __.Compose outOfBand pipeIdx next =
                    {
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
                                    TailCall.avoid (next.ProcessNext window) } :> _}

        [<CompiledName("Append")>]
        let internal append (source1:ISeq<'T>) (source2: ISeq<'T>) : ISeq<'T> =
            match source1 with
            | :? EnumerableBase<'T> as s -> s.Append source2
            | _ -> new AppendEnumerable<_>([source2; source1]) :> _

        [<CompiledName "Delay">]
        let internal delay (delayed:unit->ISeq<'T>) : ISeq<'T> =
            DelayedEnumerable (delayed, 1) :> _

        module internal GroupBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    { new FolderWithPostProcessing<'T,ISeq<'Key*ISeq<'T>>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
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

                        override this.OnDispose () = () } :> _ )

            let inline byVal (projection:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> projection id source) 

            let inline byRef (projection:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (projection >> Value) (fun v -> v._1) source)
        
        [<CompiledName("GroupByVal")>]
        let inline internal groupByVal<'T,'Key when 'Key : equality and 'Key : struct> (projection:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byVal projection source

        [<CompiledName("GroupByRef")>]
        let inline internal groupByRef<'T,'Key when 'Key : equality and 'Key : not struct> (projection:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byRef projection source

        module CountBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    { new FolderWithPostProcessing<'T,ISeq<'Key*int>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
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

                        override this.OnDispose () = () } :> _ )

            let inline byVal (projection:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> projection id source) 

            let inline byRef (projection:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (projection >> Value) (fun v -> v._1) source)
        
        [<CompiledName("CountByVal")>]
        let inline internal countByVal<'T,'Key when 'Key : equality and 'Key : struct>  (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byVal projection source 

        [<CompiledName("CountByRef")>]
        let inline internal countByRef<'T,'Key when 'Key : equality and 'Key : not struct> (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byRef projection source 

        [<CompiledName("Length")>]
        let internal length (source:ISeq<'T>)  =
            match source with
            | :? EnumerableBase<'T> as s -> s.Length ()
            | :? list<'T> as l -> l.Length
            | _ -> Microsoft.FSharp.Primitives.Basics.ISeq.length source

        [<CompiledName("ToArray")>]
        let internal toArray (source:ISeq<'T>)  =
            source.Fold (fun _ ->
                { new FolderWithPostProcessing<'T,array<'T>,_>(Unchecked.defaultof<_>,ResizeArray ()) with
                    override this.ProcessNext v =
                        this.State.Add v
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    
                    override this.OnComplete _ =
                        this.Result <- this.State.ToArray ()
                    
                    override this.OnDispose () = () } :> _ )

        [<CompiledName("SortBy")>]
        let internal sortBy projection source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceBy projection array
                ofArray array)

        [<CompiledName("Sort")>]
        let internal sort source =
            delay (fun () -> 
                let array = source |> toArray
                Array.stableSortInPlace array
                ofArray array)

        [<CompiledName("SortWith")>]
        let sortWith comparer source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceWith comparer array
                ofArray array)

        [<CompiledName("Reverse")>]
        let internal rev source =
            delay (fun () ->
                let array = source |> toArray
                Array.Reverse array
                ofArray array)

        [<CompiledName("Permute")>]
        let internal permute indexMap (source:ISeq<_>) =
            delay (fun () ->
                source
                |> toArray
                |> Array.permute indexMap
                |> ofArray)

        [<CompiledName("ScanBack")>]
        let internal scanBack<'T,'State> folder (source:ISeq<'T>) (state:'State) : ISeq<'State> =
            delay (fun () ->
                let array = source |> toArray
                Array.scanSubRight folder array 0 (array.Length - 1) state
                |> ofArray)

        let inline internal foldArraySubRight f (arr: 'T[]) start fin acc =
            let mutable state = acc
            for i = fin downto start do
                state <- f arr.[i] state
            state

        [<CompiledName("FoldBack")>]
        let inline internal foldBack<'T,'State> folder (source: ISeq<'T>) (state:'State) =
            let arr = toArray source
            let len = arr.Length
            foldArraySubRight folder arr 0 (len - 1) state

        [<CompiledName("Zip")>]
        let internal zip source1 source2 =
            map2 (fun x y -> x,y) source1 source2

        [<CompiledName("FoldBack2")>]
        let inline internal foldBack2<'T1,'T2,'State> folder (source1:ISeq<'T1>) (source2:ISeq<'T2>) (state:'State) =
            let zipped = zip source1 source2
            foldBack ((<||) folder) zipped state

        [<CompiledName("ReduceBack")>]
        let inline internal reduceBack reduction (source:ISeq<'T>) =
            let arr = toArray source
            match arr.Length with
            | 0 -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | len -> foldArraySubRight reduction arr 0 (len - 2) arr.[len - 1]

        [<Sealed>]
        type internal CachedSeq<'T>(source:ISeq<'T>) =
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

            let cached : ISeq<'T> = new UnfoldEnumerable<'T,'T,int>(unfolding, 0, IdentityFactory.Instance, 1) :> _

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
                member __.GetEnumerator() = (cached:>IEnumerable).GetEnumerator()

            interface ISeq<'T> with
                member __.PushTransform next = cached.PushTransform next
                member __.Fold f = cached.Fold f

            member this.Clear() = (this :> IDisposable).Dispose ()

        [<CompiledName("Cache")>]
        let internal cache (source:ISeq<'T>) : ISeq<'T> =
            new CachedSeq<_> (source) :> _

        [<CompiledName("Collect")>]
        let internal collect mapping source = map mapping source |> concat

        [<CompiledName("AllPairs")>]
        let internal allPairs (source1:ISeq<'T1>) (source2:ISeq<'T2>) : ISeq<'T1 * 'T2> =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            let cached = cache source2
            source1 |> collect (fun x -> cached |> map (fun y -> x,y))

        [<CompiledName("ToList")>]
        let internal toList (source : ISeq<'T>) =
            match source with
            | :? list<'T> as lst -> lst
            | _ -> Microsoft.FSharp.Primitives.Basics.List.ofISeq source

        [<CompiledName("Replicate")>]
        let replicate<'T> count (initial:'T) : ISeq<'T> =
            if count < 0 then raise (ArgumentOutOfRangeException "count")
            new InitEnumerable<'T,'T>(Nullable count, (fun _ -> initial), IdentityFactory.Instance, 1) :> _

        [<CompiledName("IsEmpty")>]
        let internal isEmpty (source : ISeq<'T>)  =
            match source with
            | :? list<'T> as lst -> lst.IsEmpty
            | _ ->
                use ie = source.GetEnumerator()
                not (ie.MoveNext())

        [<CompiledName("Cast")>]
        let internal cast (source: IEnumerable) : ISeq<'T> =
            match source with
            | :? ISeq<'T> as s -> s
            | :? ISeq<obj> as s -> s |> map unbox // covariant on ref types
            | _ -> 
                mkSeq (fun () -> IEnumerator.cast (source.GetEnumerator())) |> ofSeq

        [<CompiledName("ChunkBySize")>]
        let internal chunkBySize chunkSize (source : ISeq<'T>) : ISeq<'T[]> =
            if chunkSize <= 0 then invalidArgFmt "chunkSize" "{0}\nchunkSize = {1}"
                                    [|SR.GetString SR.inputMustBePositive; chunkSize|]

            source.PushTransform { new ITransformFactory<'T,'T[]> with
                member __.Compose outOfBand pipeIdx next =
                    {
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
                            
                            override this.OnDispose () = () } :> _ }

        let internal mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator()) |> ofSeq

        [<CompiledName("SplitInto")>]
        let internal splitInto count (source:ISeq<'T>) : ISeq<'T[]> =
            if count <= 0 then invalidArgFmt "count" "{0}\ncount = {1}"
                                [|SR.GetString SR.inputMustBePositive; count|]
            mkDelayedSeq (fun () ->
                source |> toArray |> Array.splitInto count :> seq<_>)

        let inline internal indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("Find")>]
        let internal find predicate source =
            match tryFind predicate source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("FindIndex")>]
        let internal findIndex predicate (source:ISeq<_>) =
            use ie = source.GetEnumerator()
            let rec loop i =
                if ie.MoveNext() then
                    if predicate ie.Current then
                        i
                    else loop (i+1)
                else
                    indexNotFound()
            loop 0

        [<CompiledName("FindBack")>]
        let internal findBack predicate source =
            source |> toArray |> Array.findBack predicate

        [<CompiledName("FindIndexBack")>]
        let internal findIndexBack predicate source =
            source |> toArray |> Array.findIndexBack predicate

        [<CompiledName("Pick")>]
        let internal pick chooser source  =
            match tryPick chooser source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("MapFold")>]
        let internal mapFold<'T,'State,'Result> (mapping:'State->'T->'Result*'State) state source =
            let arr,state = source |> toArray |> Array.mapFold mapping state
            ofArray arr, state

        [<CompiledName("MapFoldBack")>]
        let internal mapFoldBack<'T,'State,'Result> (mapping:'T->'State->'Result*'State) source state =
            let array = source |> toArray
            let arr,state = Array.mapFoldBack mapping array state
            ofArray arr, state

        let rec internal nth index (e : IEnumerator<'T>) =
            if not (e.MoveNext()) then
              let shortBy = index + 1
              invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; shortBy; (if shortBy = 1 then "element" else "elements")|]
            if index = 0 then e.Current
            else nth (index-1) e

        [<CompiledName("Item")>]
        let internal item index (source : ISeq<'T>) =
            if index < 0 then invalidArgInputMustBeNonNegative "index" index
            use e = source.GetEnumerator()
            nth index e

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            let inline compareDescending a b = compare b a
            sortWith compareDescending source

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending projection source =
            let inline compareDescending a b = compare (projection b) (projection a)
            sortWith compareDescending source

        [<CompiledName("TryFindBack")>]
        let internal tryFindBack predicate (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindBack predicate

        [<CompiledName("TryFindIndexBack")>]
        let internal tryFindIndexBack predicate (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindIndexBack predicate

        [<CompiledName("Zip3")>]
        let internal zip3 source1 source2  source3 =
            map2 (fun x (y,z) -> x,y,z) source1 (zip source2 source3)