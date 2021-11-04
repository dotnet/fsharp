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
        [<CompiledName("Merge")>]
        val merge: event1:IEvent<'Del1,'T> -> event2:IEvent<'Del2,'T> -> IEvent<'T>

        /// <summary>Returns a new event that passes values transformed by the given function.</summary>
        ///
        /// <param name="mapping">The function to transform event values.</param>
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>An event that passes the transformed values.</returns>
        [<CompiledName("Map")>]
        val map: mapping:('T -> 'U) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U>

        /// <summary>Returns a new event that listens to the original event and triggers the resulting
        /// event only when the argument to the event passes the given function.</summary>
        ///
        /// <param name="predicate">The function to determine which triggers from the event to propagate.</param>
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>An event that only passes values that pass the predicate.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:('T -> bool) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'T>

        /// <summary>Returns a new event that listens to the original event and triggers the 
        /// first resulting event if the application of the predicate to the event arguments
        /// returned true, and the second event if it returned false.</summary>
        ///
        /// <param name="predicate">The function to determine which output event to trigger.</param>
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>A tuple of events.  The first is triggered when the predicate evaluates to true
        /// and the second when the predicate evaluates to false.</returns>
        [<CompiledName("Partition")>]
        val partition: predicate:('T -> bool) -> sourceEvent:IEvent<'Del,'T> -> (IEvent<'T> * IEvent<'T>)

        /// <summary>Returns a new event that listens to the original event and triggers the 
        /// first resulting event if the application of the function to the event arguments
        /// returned a Choice1Of2, and the second event if it returns a Choice2Of2.</summary>
        ///
        /// <param name="splitter">The function to transform event values into one of two types.</param>
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>A tuple of events.  The first fires whenever <c>splitter</c> evaluates to Choice1of1 and
        /// the second fires whenever <c>splitter</c> evaluates to Choice2of2.</returns>
        [<CompiledName("Split")>]
        val split: splitter:('T -> Choice<'U1,'U2>) -> sourceEvent:IEvent<'Del,'T> -> (IEvent<'U1> * IEvent<'U2>)

        /// <summary>Returns a new event which fires on a selection of messages from the original event.
        /// The selection function takes an original message to an optional new message.</summary>
        ///
        /// <param name="chooser">The function to select and transform event values to pass on.</param>
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>An event that fires only when the chooser returns Some.</returns>
        [<CompiledName("Choose")>]
        val choose: chooser:('T -> 'U option) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U>

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
        [<CompiledName("Scan")>]
        val scan: collector:('U -> 'T -> 'U) -> state:'U -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U> 

        /// <summary>Runs the given function each time the given event is triggered.</summary>
        ///
        /// <param name="callback">The function to call when the event is triggered.</param>
        /// <param name="sourceEvent">The input event.</param>
        [<CompiledName("Add")>]
        val add : callback:('T -> unit) -> sourceEvent:IEvent<'Del,'T> -> unit

        /// <summary>Returns a new event that triggers on the second and subsequent triggerings of the input event.
        /// The Nth triggering of the input event passes the arguments from the N-1th and Nth triggering as
        /// a pair. The argument passed to the N-1th triggering is held in hidden internal state until the 
        /// Nth triggering occurs.</summary>
        ///
        /// <param name="sourceEvent">The input event.</param>
        ///
        /// <returns>An event that triggers on pairs of consecutive values passed from the source event.</returns>
        [<CompiledName("Pairwise")>]
        val pairwise: sourceEvent:IEvent<'Del,'T> -> IEvent<'T * 'T>


