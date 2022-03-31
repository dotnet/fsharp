// #NoMT #CodeGen #Interop 
#light

// Verify delegates are generated correctly

namespace Test

    type ChannelChanged = delegate of obj * int -> unit
    
    type Television() as this =

        // Create an event channel changed.
        let channelChanged = new Event<ChannelChanged,_>()
        let triggerChanelChanged channelNumber = channelChanged.Trigger(this, channelNumber)
            
        // When TV constrcuted, fire chanel changed event.
        do triggerChanelChanged(3)
        
        // Static event
        static let defaultChannelChanged = new Event<ChannelChanged,_>()

        // Setting the chanel will fire our event.
        member this.SetChanel (x : int) = triggerChanelChanged(x)
        
        // By adding the [<CompileAsEvent>] on a property of type IEvent<_>
        // this will be compiled as a proper .NET event.
        [<CLIEvent>]
        member this.ChanelChangedEvent = channelChanged.Publish

        // Static event
        [<CLIEvent>]    
        static member DefaultChannelChanged = defaultChannelChanged.Publish

// --------------------------------

    module Tester =

        open CodeGenHelper
        open System

        printfn "Testing..."

        try

            // Take no parameters, return void
            System.Reflection.Assembly.GetExecutingAssembly()
            |> getType "Test.Television"
            |> getEvent "ChanelChangedEvent"
            |> should useEventHandler typeof<ChannelChanged>

            // Take no parameters, return void
            System.Reflection.Assembly.GetExecutingAssembly()
            |> getType "Test.Television"
            |> getEvent "DefaultChannelChanged"
            |> should useEventHandler typeof<ChannelChanged>
            
        with
        | e -> printfn "Unhandled Exception: %s" e.Message 
               raise (Exception($"Oops: {e}"))
