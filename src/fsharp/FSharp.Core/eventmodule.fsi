// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

open Microsoft.FSharp.Core
open Microsoft.FSharp.Control

/// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Control.IEvent`1"/>.</summary>
///
/// <category index="3">Events and Observables</category>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Event =

    /// <summary>Fires the output event when either of the input events fire.</summary>
    /// <param name="event1">The first input event.</param>
    /// <param name="event2">The second input event.</param>
    ///
    /// <returns>An event that fires when either of the input events fire.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System.Reactive.Linq
    /// open System
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let oneSecondTimer = createTimer 1000
    /// let fiveSecondsTimer = createTimer 5000
    ///
    /// let result = Event.merge oneSecondTimer fiveSecondsTimer
    ///
    /// result.Subscribe(fun output -> printfn $"Output - {output.SignalTime} ")
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will output: <c>Output - 2/15/2022 12:10:40 AM
    /// Output - 2/15/2022 12:10:41 AM
    /// Output - 2/15/2022 12:10:41 AM
    /// Output - 2/15/2022 12:10:42 AM
    /// Output - 2/15/2022 12:10:43 AM </c>
    /// </example>
    [<CompiledName("Merge")>]
    val merge: event1: IEvent<'Del1, 'T> -> event2: IEvent<'Del2, 'T> -> IEvent<'T>

    /// <summary>Returns a new event that passes values transformed by the given function.</summary>
    ///
    /// <param name="mapping">The function to transform event values.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>An event that passes the transformed values.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let transformSeconds (number: Timers.ElapsedEventArgs) =
    ///     match number with
    ///     | _ when number.SignalTime.Second % 2 = 0 -> 100
    ///     | _ -> -500
    ///
    /// let evenSecondsEvent = Event.map transformSeconds timer
    ///
    /// evenSecondsEvent.Subscribe(fun x -> printf $"{x} ")
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will transform the seconds if it's even or odd number and the output is: <c>-500 100 -500 100 -500 100 </c>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> sourceEvent: IEvent<'Del, 'T> -> IEvent<'U>

    /// <summary>Returns a new event that listens to the original event and triggers the resulting
    /// event only when the argument to the event passes the given function.</summary>
    ///
    /// <param name="predicate">The function to determine which triggers from the event to propagate.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>An event that only passes values that pass the predicate.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let getEvenSeconds (number: Timers.ElapsedEventArgs) =
    ///     match number with
    ///     | _ when number.SignalTime.Second % 2 = 0 -> true
    ///     | _ -> false
    ///
    /// let evenSecondsEvent = Event.filter getEvenSeconds timer
    ///
    /// evenSecondsEvent.Subscribe(fun x -> printfn $"{x} ")
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will only output even seconds: <c>2/15/2022 12:03:08 AM
    /// 2/15/2022 12:03:10 AM
    /// 2/15/2022 12:03:12 AM
    /// 2/15/2022 12:03:14 AM </c>
    /// </example>
    [<CompiledName("Filter")>]
    val filter: predicate: ('T -> bool) -> sourceEvent: IEvent<'Del, 'T> -> IEvent<'T>

    /// <summary>Returns a new event that listens to the original event and triggers the
    /// first resulting event if the application of the predicate to the event arguments
    /// returned true, and the second event if it returned false.</summary>
    ///
    /// <param name="predicate">The function to determine which output event to trigger.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>A tuple of events.  The first is triggered when the predicate evaluates to true
    /// and the second when the predicate evaluates to false.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let getEvenSeconds (number: Timers.ElapsedEventArgs) =
    ///     match number with
    ///     | _ when number.SignalTime.Second % 2 = 0 -> true
    ///     | _ -> false
    ///
    /// let leftPartition, rightPartition = Event.partition getEvenSeconds timer
    ///
    /// leftPartition.Subscribe(fun x -> printfn $"Left partition: {x.SignalTime}")
    /// |> ignore
    ///
    /// rightPartition.Subscribe(fun x -> printfn $"Right partition: {x.SignalTime}")
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will partition into two events if it is even or odd seconds: <c>
    /// Right partition: 2/15/2022 12:00:27 AM
    /// Left partition: 2/15/2022 12:00:28 AM
    /// Right partition: 2/15/2022 12:00:29 AM
    /// Left partition: 2/15/2022 12:00:30 AM
    /// Right partition: 2/15/2022 12:00:31 AM</c>
    /// </example>
    [<CompiledName("Partition")>]
    val partition: predicate: ('T -> bool) -> sourceEvent: IEvent<'Del, 'T> -> (IEvent<'T> * IEvent<'T>)

    /// <summary>Returns a new event that listens to the original event and triggers the
    /// first resulting event if the application of the function to the event arguments
    /// returned a Choice1Of2, and the second event if it returns a Choice2Of2.</summary>
    ///
    /// <param name="splitter">The function to transform event values into one of two types.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>A tuple of events.  The first fires whenever <c>splitter</c> evaluates to Choice1of1 and
    /// the second fires whenever <c>splitter</c> evaluates to Choice2of2.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let bySeconds (timerEvent: Timers.ElapsedEventArgs) =
    ///     match timerEvent.SignalTime.Second % 2 = 0 with
    ///     | true -> Choice1Of2 timerEvent.SignalTime.Second
    ///     | false -> Choice2Of2 $"{timerEvent.SignalTime.Second} is not an even num ber"
    ///
    /// let evenSplit, printOddNumbers = Event.split bySeconds timer
    ///
    /// let printOutput event functionName =
    ///     Event.add (fun output -> printfn $"{functionName} - Split output: {output}. /// Type: {output.GetType()}") event
    ///
    /// printOutput evenSplit (nameof evenSplit) |> ignore
    ///
    /// printOutput printOddNumbers (nameof printOddNumbers)
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will split the events by even or odd seconds: <c>evenSplit - Split output: 44. Type: System.Int32
    /// printOddNumbers - Split output: 45 is not an even number. Type: System.String
    /// evenSplit - Split output: 46. Type: System.Int32
    /// printOddNumbers - Split output: 47 is not an even number. Type: System.String
    /// evenSplit - Split output: 48. Type: System.Int32
    /// printOddNumbers - Split output: 49 is not an even number. Type: System.String</c>
    /// </example>
    [<CompiledName("Split")>]
    val split: splitter: ('T -> Choice<'U1, 'U2>) -> sourceEvent: IEvent<'Del, 'T> -> (IEvent<'U1> * IEvent<'U2>)

    /// <summary>Returns a new event which fires on a selection of messages from the original event.
    /// The selection function takes an original message to an optional new message.</summary>
    ///
    /// <param name="chooser">The function to select and transform event values to pass on.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>An event that fires only when the chooser returns Some.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let getEvenSeconds (number: Timers.ElapsedEventArgs) =
    ///     match number with
    ///     | _ when number.SignalTime.Second % 2 = 0 -> Some number.SignalTime
    ///     | _ -> None
    ///
    /// let evenSecondsEvent = Event.choose getEvenSeconds timer
    ///
    /// evenSecondsEvent.Subscribe(fun x -> printfn $"{x} ")
    /// |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will output: <c>2/15/2022 12:04:04 AM
    /// 2/15/2022 12:04:06 AM
    /// 2/15/2022 12:04:08 AM </c>
    /// </example>
    [<CompiledName("Choose")>]
    val choose: chooser: ('T -> 'U option) -> sourceEvent: IEvent<'Del, 'T> -> IEvent<'U>

    /// <summary>Returns a new event consisting of the results of applying the given accumulating function
    /// to successive values triggered on the input event.  An item of internal state
    /// records the current value of the state parameter.  The internal state is not locked during the
    /// execution of the accumulation function, so care should be taken that the
    /// input IEvent not triggered by multiple threads simultaneously.</summary>
    ///
    /// <param name="collector">The function to update the state with each event value.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>An event that fires on the updated state values.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let multiplyBy number =
    ///     fun (timerEvent: Timers.ElapsedEventArgs) -> number * timerEvent.SignalTime./// Second
    ///
    /// let initialState = 2
    ///
    /// let scan = Event.scan multiplyBy initialState timer
    ///
    /// scan.Subscribe(fun x -> printf "%A " x) |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will output depending on your timestamp. It will multiply the seconds with an initial state of 2: <c>106 5724 314820 17629920 1004905440 -1845026624 -1482388416</c>
    /// </example>
    [<CompiledName("Scan")>]
    val scan: collector: ('U -> 'T -> 'U) -> state: 'U -> sourceEvent: IEvent<'Del, 'T> -> IEvent<'U>

    /// <summary>Runs the given function each time the given event is triggered.</summary>
    ///
    /// <param name="callback">The function to call when the event is triggered.</param>
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// Event.add (fun (event: Timers.ElapsedEventArgs) -> printfn $"{event.SignalTime} ")  timer
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will output the timer event every second: <c>
    /// 2/14/2022 11:52:05 PM
    /// 2/14/2022 11:52:06 PM
    /// 2/14/2022 11:52:07 PM
    /// 2/14/2022 11:52:08 PM </c>
    /// </example>
    [<CompiledName("Add")>]
    val add: callback: ('T -> unit) -> sourceEvent: IEvent<'Del, 'T> -> unit

    /// <summary>Returns a new event that triggers on the second and subsequent triggerings of the input event.
    /// The Nth triggering of the input event passes the arguments from the N-1th and Nth triggering as
    /// a pair. The argument passed to the N-1th triggering is held in hidden internal state until the
    /// Nth triggering occurs.</summary>
    ///
    /// <param name="sourceEvent">The input event.</param>
    ///
    /// <returns>An event that triggers on pairs of consecutive values passed from the source event.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// open System
    ///
    /// let createTimer interval =
    ///     let timer = new Timers.Timer(interval)
    ///     timer.AutoReset &lt;- true
    ///     timer.Enabled &lt;- true
    ///     timer.Elapsed
    ///
    /// let timer = createTimer 1000
    ///
    /// let pairWise = Event.pairwise timer
    ///
    /// let extractPair (pair: Timers.ElapsedEventArgs * Timers.ElapsedEventArgs) =
    ///     let leftPair, rightPair = pair
    ///     printfn $"(Left): {leftPair.SignalTime} (Right): {rightPair.SignalTime}"
    ///
    /// pairWise.Subscribe(extractPair) |> ignore
    ///
    /// Console.ReadLine() |> ignore
    /// </code>
    /// The sample will output the timer event every second: <c>
    /// (Left): 2/14/2022 11:58:46 PM (Right): 2/14/2022 11:58:46 PM
    /// (Left): 2/14/2022 11:58:46 PM (Right): 2/14/2022 11:58:47 PM
    /// (Left): 2/14/2022 11:58:47 PM (Right): 2/14/2022 11:58:48 PM </c>
    /// </example>
    [<CompiledName("Pairwise")>]
    val pairwise: sourceEvent: IEvent<'Del, 'T> -> IEvent<'T * 'T>
