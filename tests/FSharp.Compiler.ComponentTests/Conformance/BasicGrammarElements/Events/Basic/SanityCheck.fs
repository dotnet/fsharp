// #Conformance #DeclarationElements #Events 
// Sanity check events.
type Action =
    | Squeeze
    | Poke
    | LeaveAlone

// Sanity check events can be defined, fired, and all subscribers notified.
type SqueakyToy() =
    let squeakEvent = new Event<_>()
    let triggerSqueakEvent = squeakEvent.Trigger
    
    // This event fire whenever the squeaky toy squeaks
    member this.Squeak with get ()  = squeakEvent
    
    member this.ApplyAction act = 
        match act with
        | Squeeze    -> triggerSqueakEvent(act)
        | Poke       -> triggerSqueakEvent(act)
        | LeaveAlone -> ()


// Test events

// Nothing wired up...
let pinkSqueakyToy = new SqueakyToy()
pinkSqueakyToy.ApplyAction(Squeeze)

// Hook up event handler, so when the event fires
// m_eventFlag is set to whatever the param was.
let mutable m_eventFlag1 : Action option = None
let eventHandler1 = new Handler<Action> (fun (_ : obj)  (arg : Action) -> m_eventFlag1 <- Some(arg))
pinkSqueakyToy.Squeak.Publish.AddHandler(eventHandler1)

// Poke the toy, which should cause it to squeak
m_eventFlag1 <- None
pinkSqueakyToy.ApplyAction(Poke)
if m_eventFlag1 <> Some(Poke) then failwith "Failed: 1"

// Now add another event handler
let mutable m_eventFlag2 : Action option = None
let eventHandler2 = new Handler<Action> (fun _ act -> m_eventFlag2 <- Some(act))
pinkSqueakyToy.Squeak.Publish.AddHandler(eventHandler2)

// Now squeeze the toy, verifying that both event handlers
// were called.
m_eventFlag1 <- None
m_eventFlag2 <- None

pinkSqueakyToy.ApplyAction(Squeeze)
if m_eventFlag1 <> Some(Squeeze) then failwith "Failed: 2"
if m_eventFlag2 <> Some(Squeeze) then failwith "Failed: 3"
 
// Remove one of the event handlers
pinkSqueakyToy.Squeak.Publish.RemoveHandler(eventHandler1)
m_eventFlag1 <- None
m_eventFlag2 <- None

// Event handler 1 should not have fired.
pinkSqueakyToy.ApplyAction(Squeeze)
if m_eventFlag1 <> None then failwith "Failed: 4"
if m_eventFlag2 <> Some(Squeeze) then failwith "Failed: 5"

// Remove the last event handler
pinkSqueakyToy.Squeak.Publish.RemoveHandler(eventHandler2)
m_eventFlag1 <- None
m_eventFlag2 <- None

// Event handler 1 should not have fired.
pinkSqueakyToy.ApplyAction(Squeeze)
if m_eventFlag1 <> None then failwith "Failed: 6"
if m_eventFlag2 <> None then failwith "Failed: 7"

