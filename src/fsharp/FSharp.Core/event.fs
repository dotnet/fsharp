// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Control
    open System.Reflection
    open System.Diagnostics

#if FX_RESHAPED_REFLECTION
    open ReflectionAdapters
    type BindingFlags = ReflectionAdapters.BindingFlags
#endif

#if FX_NO_DELEGATE_DYNAMIC_METHOD 
#else

#if FX_NO_DELEGATE_DYNAMIC_INVOKE 
    module Impl = 
        type System.Delegate with 
            member d.DynamicInvoke(args: obj[]) =
                d.Method.Invoke(d.Target, BindingFlags.Default |||  BindingFlags.Public ||| BindingFlags.NonPublic , null, args, null)

    open Impl
#endif
    
    [<CompiledName("FSharpDelegateEvent`1")>]
    type DelegateEvent<'Delegate when 'Delegate :> System.Delegate>() = 
        let mutable multicast : System.Delegate = null
        member x.Trigger(args:obj[]) = 
            match multicast with 
            | null -> ()
            | d -> d.DynamicInvoke(args) |> ignore
        member x.Publish = 
            { new IDelegateEvent<'Delegate> with 
                member x.AddHandler(d) =
                    multicast <- System.Delegate.Combine(multicast, d)
                member x.RemoveHandler(d) = 
                    multicast <- System.Delegate.Remove(multicast, d) }
    
    type EventDelegee<'Args>(observer: System.IObserver<'Args>) =
        static let makeTuple = 
            if Microsoft.FSharp.Reflection.FSharpType.IsTuple(typeof<'Args>) then
                Microsoft.FSharp.Reflection.FSharpValue.PreComputeTupleConstructor(typeof<'Args>)
            else
                fun _ -> assert false; null // should not be called, one-argument case don't use makeTuple function

        member x.Invoke(_sender:obj, args: 'Args) = 
            observer.OnNext args
        member x.Invoke(_sender:obj, a, b) = 
            let args = makeTuple([|a; b|]) :?> 'Args
            observer.OnNext args
        member x.Invoke(_sender:obj, a, b, c) = 
            let args = makeTuple([|a; b; c|]) :?> 'Args
            observer.OnNext args
        member x.Invoke(_sender:obj, a, b, c, d) = 
            let args = makeTuple([|a; b; c; d|]) :?> 'Args
            observer.OnNext args
        member x.Invoke(_sender:obj, a, b, c, d, e) = 
            let args = makeTuple([|a; b; c; d; e|]) :?> 'Args
            observer.OnNext args
        member x.Invoke(_sender:obj, a, b, c, d, e, f) = 
            let args = makeTuple([|a; b; c; d; e; f|]) :?> 'Args
            observer.OnNext args


    type EventWrapper<'Delegate,'Args> = delegate of 'Delegate * obj * 'Args -> unit

    [<CompiledName("FSharpEvent`2")>]
    type Event<'Delegate, 'Args when 'Delegate : delegate<'Args, unit> and 'Delegate :> System.Delegate>() =  

        let mutable multicast : 'Delegate = Unchecked.defaultof<_>     

#if FX_NO_DELEGATE_CREATE_DELEGATE_FROM_STATIC_METHOD
#else
        static let mi, argTypes = 
            let instanceBindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.DeclaredOnly
            let mi = typeof<'Delegate>.GetMethod("Invoke",instanceBindingFlags)
            let actualTypes = mi.GetParameters() |> Array.map (fun p -> p.ParameterType)
            mi, actualTypes.[1..]

        // For the one-argument case, use an optimization that allows a fast call. 
        // CreateDelegate creates a delegate that is fast to invoke.
        static let invoker = 
            if argTypes.Length = 1 then 
                (System.Delegate.CreateDelegate(typeof<EventWrapper<'Delegate,'Args>>, mi) :?> EventWrapper<'Delegate,'Args>)
            else
                null
#endif

        // For the multi-arg case, use a slower DynamicInvoke.
        static let invokeInfo =
            let instanceBindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.DeclaredOnly
#if FX_NO_DELEGATE_CREATE_DELEGATE_FROM_STATIC_METHOD
            typeof<EventDelegee<'Args>>.GetMethod("Invoke",instanceBindingFlags)
#else
            let mi = 
                typeof<EventDelegee<'Args>>.GetMethods(instanceBindingFlags)
                |> Seq.filter(fun mi -> mi.Name = "Invoke" && mi.GetParameters().Length = argTypes.Length + 1)
                |> Seq.exactlyOne
            if mi.IsGenericMethodDefinition then
                mi.MakeGenericMethod argTypes
            else 
                mi
#endif
                

        member x.Trigger(sender:obj,args: 'Args) = 
            match box multicast with 
            | null -> () 
            | _ -> 
#if FX_NO_DELEGATE_CREATE_DELEGATE_FROM_STATIC_METHOD
#else
                match invoker with 
                | null ->  
#endif
                    let args = Array.append [| sender |] (Microsoft.FSharp.Reflection.FSharpValue.GetTupleFields(box args))
                    multicast.DynamicInvoke(args) |> ignore
#if FX_NO_DELEGATE_CREATE_DELEGATE_FROM_STATIC_METHOD
#else
                | _ -> 
                    // For the one-argument case, use an optimization that allows a fast call. 
                    // CreateDelegate creates a delegate that is fast to invoke.
                    invoker.Invoke(multicast, sender, args) |> ignore
#endif

        member x.Publish = 
            // Note, we implement each interface explicitly: this works around a bug in the CLR 
            // implementation on CompactFramework 3.7, used on Windows Phone 7
            { new obj() with
                  member x.ToString() = "<published event>"
              interface IEvent<'Delegate,'Args> 
              interface IDelegateEvent<'Delegate> with 
                member e.AddHandler(d) =
                    multicast <- System.Delegate.Combine(multicast, d) :?> 'Delegate 
                member e.RemoveHandler(d) = 
                    multicast <- System.Delegate.Remove(multicast, d)  :?> 'Delegate 
              interface System.IObservable<'Args> with 
                member e.Subscribe(observer) = 
                   let obj = new EventDelegee<'Args>(observer)
                   let h = System.Delegate.CreateDelegate(typeof<'Delegate>, obj, invokeInfo) :?> 'Delegate
                   (e :?> IDelegateEvent<'Delegate>).AddHandler(h)
                   { new System.IDisposable with 
                        member x.Dispose() = (e :?> IDelegateEvent<'Delegate>).RemoveHandler(h) } } 

#endif

    [<CompiledName("FSharpEvent`1")>]
    type Event<'T> = 
        val mutable multicast : Handler<'T>
        new() = { multicast = null }

        member x.Trigger(arg:'T) = 
            match x.multicast with 
            | null -> ()
            | d -> d.Invoke(null,arg) |> ignore
        member x.Publish = 
            // Note, we implement each interface explicitly: this works around a bug in the CLR 
            // implementation on CompactFramework 3.7, used on Windows Phone 7
            { new obj() with
                  member x.ToString() = "<published event>"
              interface IEvent<'T> 
              interface IDelegateEvent<Handler<'T>> with 
                member e.AddHandler(d) =
                    x.multicast <- (System.Delegate.Combine(x.multicast, d) :?> Handler<'T>)
                member e.RemoveHandler(d) = 
                    x.multicast <- (System.Delegate.Remove(x.multicast, d) :?> Handler<'T>)
              interface System.IObservable<'T> with 
                member e.Subscribe(observer) = 
                   let h = new Handler<_>(fun sender args -> observer.OnNext(args))
                   (e :?> IEvent<_,_>).AddHandler(h)
                   { new System.IDisposable with 
                        member x.Dispose() = (e :?> IEvent<_,_>).RemoveHandler(h) } }


