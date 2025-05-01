// #Conformance #DeclarationElements #Events 
#light

// Verify ability to implement events on an interface

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


// Verify it works
let mutable setArgs = "", 0
        
let lamp = new MagicLamp()
let magicalLamp = lamp :> IHaveMagicalProperties
        
let eventHandler = MagicalResult(fun sender msg res -> setArgs <- (msg, res))
magicalLamp.GoodThingHappened.AddHandler eventHandler

magicalLamp.Rub()
        
if setArgs <> ("A genie appears!", 3) then failwith "Failed: 1"
