// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

open System
open Microsoft.FSharp.Core

/// <summary>Contains operations for working with first class event and other observable objects.</summary>
///
/// <category index="3">Events and Observables</category>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Observable =

    /// <summary>Returns an observable for the merged observations from the sources. 
    /// The returned object propagates success and error values arising 
    /// from either source and completes when both the sources have completed.</summary>
    ///
    /// <remarks>For each observer, the registered intermediate observing object is not 
    /// thread safe. That is, observations arising from the sources must not 
    /// be triggered concurrently on different threads.</remarks>
    ///
    /// <param name="source1">The first Observable.</param>
    /// <param name="source2">The second Observable.</param>
    ///
    /// <returns>An Observable that propagates information from both sources.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Merge")>]
    val merge: source1:IObservable<'T> -> source2:IObservable<'T> -> IObservable<'T>

    /// <summary>Returns an observable which transforms the observations of the source by the 
    /// given function. The transformation function is executed once for each 
    /// subscribed observer. The returned object also propagates error observations 
    /// arising from the source and completes when the source completes.</summary>
    /// <param name="mapping">The function applied to observations from the source.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An Observable of the type specified by <c>mapping</c>.</returns> 
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Map")>]
    val map: mapping:('T -> 'U) -> source:IObservable<'T> -> IObservable<'U>

    /// <summary>Returns an observable which filters the observations of the source 
    /// by the given function. The observable will see only those observations
    /// for which the predicate returns true. The predicate is executed once for 
    /// each subscribed observer. The returned object also propagates error 
    /// observations arising from the source and completes when the source completes.</summary>
    ///
    /// <param name="predicate">The function to apply to observations to determine if it should
    /// be kept.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An Observable that filters observations based on <c>filter</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Filter")>]
    val filter: predicate:('T -> bool) -> source:IObservable<'T> -> IObservable<'T>

    /// <summary>Returns two observables which partition the observations of the source by 
    /// the given function. The first will trigger observations for those values 
    /// for which the predicate returns true. The second will trigger observations 
    /// for those values where the predicate returns false. The predicate is 
    /// executed once for each subscribed observer. Both also propagate all error 
    /// observations arising from the source and each completes when the source 
    /// completes.</summary>
    ///
    /// <param name="predicate">The function to determine which output Observable will trigger
    /// a particular observation.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>A tuple of Observables.  The first triggers when the predicate returns true, and
    /// the second triggers when the predicate returns false.</returns> 
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Partition")>]
    val partition: predicate:('T -> bool) -> source:IObservable<'T> -> (IObservable<'T> * IObservable<'T>)

    /// <summary>Returns two observables which split the observations of the source by the 
    /// given function. The first will trigger observations <c>x</c> for which the 
    /// splitter returns <c>Choice1Of2 x</c>. The second will trigger observations 
    /// <c>y</c> for which the splitter returns <c>Choice2Of2 y</c> The splitter is 
    /// executed once for each subscribed observer. Both also propagate error 
    /// observations arising from the source and each completes when the source 
    /// completes.</summary>
    ///
    /// <param name="splitter">The function that takes an observation an transforms
    /// it into one of the two output Choice types.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>A tuple of Observables.  The first triggers when <c>splitter</c> returns Choice1of2
    /// and the second triggers when <c>splitter</c> returns Choice2of2.</returns> 
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Split")>]
    val split: splitter:('T -> Choice<'U1,'U2>) -> source:IObservable<'T> -> (IObservable<'U1> * IObservable<'U2>)

    /// <summary>Returns an observable which chooses a projection of observations from the source 
    /// using the given function. The returned object will trigger observations <c>x</c>
    /// for which the splitter returns <c>Some x</c>. The returned object also propagates 
    /// all errors arising from the source and completes when the source completes.</summary>
    ///
    /// <param name="chooser">The function that returns Some for observations to be propagated
    /// and None for observations to ignore.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An Observable that only propagates some of the observations from the source.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Choose")>]
    val choose: chooser:('T -> 'U option) -> source:IObservable<'T> -> IObservable<'U>

    /// <summary>Returns an observable which, for each observer, allocates an item of state
    /// and applies the given accumulating function to successive values arising from
    /// the input. The returned object will trigger observations for each computed 
    /// state value, excluding the initial value. The returned object propagates 
    /// all errors arising from the source and completes when the source completes.</summary>
    ///
    /// <remarks>For each observer, the registered intermediate observing object is not thread safe.
    /// That is, observations arising from the source must not be triggered concurrently 
    /// on different threads.</remarks>
    /// <param name="collector">The function to update the state with each observation.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An Observable that triggers on the updated state values.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Scan")>]
    val scan: collector:('U -> 'T -> 'U) -> state:'U -> source:IObservable<'T> -> IObservable<'U> 

    /// <summary>Creates an observer which permanently subscribes to the given observable and which calls
    /// the given function for each observation.</summary>
    ///
    /// <param name="callback">The function to be called on each observation.</param>
    /// <param name="source">The input Observable.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Add")>]
    val add : callback:('T -> unit) -> source:IObservable<'T> -> unit

    /// <summary>Creates an observer which subscribes to the given observable and which calls
    /// the given function for each observation.</summary>
    ///
    /// <param name="callback">The function to be called on each observation.</param>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An object that will remove the callback if disposed.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Subscribe")>]
    val subscribe : callback:('T -> unit) -> source:IObservable<'T> -> System.IDisposable

    /// <summary>Returns a new observable that triggers on the second and subsequent triggerings of the input observable.
    /// The Nth triggering of the input observable passes the arguments from the N-1th and Nth triggering as
    /// a pair. The argument passed to the N-1th triggering is held in hidden internal state until the 
    /// Nth triggering occurs.</summary>
    ///
    /// <remarks>For each observer, the registered intermediate observing object is not thread safe.
    /// That is, observations arising from the source must not be triggered concurrently 
    /// on different threads.</remarks>
    /// <param name="source">The input Observable.</param>
    ///
    /// <returns>An Observable that triggers on successive pairs of observations from the input Observable.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Pairwise")>]
    val pairwise: source:IObservable<'T> -> IObservable<'T * 'T>
