// #NoMT #CodeGen #Interop 
#light

// Verify events can be added to interfaces

namespace Test

    type MagicalResult = delegate of obj * string * int -> unit
    
    type IHaveMagicalProperties =

        abstract member Rub        : unit -> unit
        abstract member DropCoinIn : unit -> unit
        
        [<CLIEvent>]
        abstract member GoodThingHappened : IEvent<MagicalResult, string * int>
        
        [<CLIEvent>]
        abstract member BadThingHappened  : IEvent<MagicalResult, string * int>

    type MagicLamp =
    
        val m_goodResult : Event<MagicalResult, string * int>
    
        new() = { 
                    m_goodResult = new Event<_,_>()
                }
    
        interface IHaveMagicalProperties with
            member this.Rub() = this.m_goodResult.Trigger(this, ("A genie appears!", 3))
            member this.DropCoinIn() = ()
            
            [<CLIEvent>]
            member this.GoodThingHappened = this.m_goodResult.Publish
            
            // Make the event accessible, but will throw when you try to add or remove a handler
            [<CLIEvent>]
            member this.BadThingHappened  = Unchecked.defaultof< IEvent<MagicalResult, string * int> >

//// --------------------------------

    module Tester =

        open CodeGenHelper
        open System

        printfn "Testing..."

        try

            // Verify code gen
            System.Reflection.Assembly.GetExecutingAssembly()
            |> getType "Test.IHaveMagicalProperties"
            |> getEvent "GoodThingHappened"
            |> should useEventHandler typeof<MagicalResult>
       
        with
        | e -> printfn "Unhandled Exception: %s" e.Message 
               raise (Exception($"Oops: {e}"))
