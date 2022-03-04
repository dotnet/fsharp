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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// open System
    /// 
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     Observable.Create(fun observer -> timer.Elapsed.Subscribe(observer))
    /// 
    /// let observableFirstTimer = createTimer 1000
    /// let observableSecondTimer = createTimer 3000
    /// 
    /// let result = Observable.merge observableFirstTimer observableSecondTimer
    /// 
    /// result.Subscribe(fun output -> printfn $"Output - {output.SignalTime} ")
    /// |> ignore
    /// 
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will merge all events at a given interval and output it to the stream: <c>
    /// Output - 2/5/2022 3:49:37 AM
    /// Output - 2/5/2022 3:49:38 AM
    /// Output - 2/5/2022 3:49:39 AM
    /// Output - 2/5/2022 3:49:39 AM
    /// Output - 2/5/2022 3:49:40 AM
    /// Output - 2/5/2022 3:49:41 AM
    /// Output - 2/5/2022 3:49:42 AM
    /// Output - 2/5/2022 3:49:42 AM
    /// </c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    ///
    /// let multiplyByTwo = fun number -> number * 2
    /// let map = Observable.map multiplyByTwo observableNumbers
    ///
    /// map.Subscribe(fun x -> printf $"{x} ") |> ignore
    /// </code>
    /// The sample will output: <c>2 4 6 8 10</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    ///
    /// let getEvenNumbers = fun number -> number % 2 = 0
    /// let map = Observable.filter multiplyByTwo observableNumbers
    ///
    /// map.Subscribe(fun x -> printf $"{x} ") |> ignore
    /// </code>
    /// The sample will output: <c>2 4</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// 
    /// let isEvenNumber = fun number -> number % 2 = 0
    /// let initialState = 2
    /// 
    /// let leftPartition, rightPartition =
    ///     Observable.partition isEvenNumber observableNumbers
    /// 
    /// leftPartition.Subscribe(fun x -> printfn $"Left partition: {x}") |> ignore
    /// 
    /// rightPartition.Subscribe(fun x -> printfn $"Right partition: {x}") |> ignore
    /// </code>
    /// The sample evaluates to: <c>Left partition: 2, 4, Right partition: 1, 3, 5</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// 
    /// let getEvenNumbers number =
    ///     match number % 2 = 0 with
    ///     | true -> Choice1Of2 number
    ///     | false -> Choice2Of2 $"{number} is not an even number"
    /// 
    /// let evenSplit, printOddNumbers = Observable.split getEvenNumbers observableNumbers
    /// 
    /// let printOutput observable functionName =
    ///     use subscription =
    ///         Observable.subscribe
    ///             (fun output -> printfn $"{functionName} - Split output: {output}. Type: {output.GetType()}")
    ///             observable
    /// 
    ///     subscription
    /// 
    /// printOutput evenSplit (nameof evenSplit) |> ignore
    /// printOutput printOddNumbers (nameof printOddNumbers) |> ignore
    /// </code>
    /// The sample evaluates to: <c>evenSplit - Split output: 2. Type: System.Int32
    /// evenSplit - Split output: 4. Type: System.Int32
    /// printOddNumbers - Split output: 1 is not an even number. Type: System.String
    /// printOddNumbers - Split output: 3 is not an even number. Type: System.String
    /// printOddNumbers - Split output: 5 is not an even number. Type: System.String</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// 
    /// let getOddNumbers number =
    ///     match number with
    ///     | _ when number % 2 = 0 -> None
    ///     | _ -> Some number
    /// 
    /// let map = Observable.choose getOddNumbers observableNumbers
    /// 
    /// map.Subscribe(fun x -> printf $"{x} ") |> ignore
    /// </code>
    /// The sample will output: <c>1 3 5</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// 
    /// let multiplyBy number = fun y -> number * y
    /// let initialState = 2
    /// let scan = Observable.scan multiplyBy initialState observableNumbers
    /// 
    /// scan.Subscribe(fun x -> printf "%A " x) |> ignore
    /// </code>
    /// The sample evaluates to: <c>2 4 12 48 240</c>
    /// </example>
    [<CompiledName("Scan")>]
    val scan: collector:('U -> 'T -> 'U) -> state:'U -> source:IObservable<'T> -> IObservable<'U> 

    /// <summary>Creates an observer which permanently subscribes to the given observable and which calls
    /// the given function for each observation.</summary>
    ///
    /// <param name="callback">The function to be called on each observation.</param>
    /// <param name="source">The input Observable.</param>
    /// 
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// let multiplyByTwo = fun number -> printf $"{number * 2} "
    /// Observable.add multiplyByTwo observableNumbers
    /// </code>
    /// The sample evaluates to: <c>2 4 6 8 10</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// let numbers = seq { 1..3 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// let printOutput observable =
    ///     use subscription = Observable.subscribe (fun x -> printfn "%A" x) observable
    ///     subscription
    /// printOutput observableNumbers |> ignore
    /// </code>
    /// The sample evaluates to: <c>1, 2, 3</c>
    /// </example>
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
    /// <example>
    /// <code lang="fsharp">
    /// /// open System.Reactive.Linq
    /// let numbers = seq { 1..5 }
    /// let observableNumbers = Observable.ToObservable numbers
    /// 
    /// let pairWise = Observable.pairwise observableNumbers
    /// 
    /// pairWise.Subscribe(fun pair -> printf $"{pair} ")
    /// |> ignore
    /// </code>
    /// The sample evaluates to: <c>(1, 2), (2, 3), (3, 4), (4, 5)</c>
    /// </example>
    [<CompiledName("Pairwise")>]
    val pairwise: source:IObservable<'T> -> IObservable<'T * 'T>
