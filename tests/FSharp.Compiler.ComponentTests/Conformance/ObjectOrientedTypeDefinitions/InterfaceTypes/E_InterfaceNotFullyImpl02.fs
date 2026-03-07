// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:3748
// Now we emit an error:
//<Expects id="FS0855" span="(14,22-14,25)" status="error">No abstract or interface member was found that corresponds to this override</Expects>
module WireOld = 
    [<AbstractClass>]
    type 'a wire =
      class
        abstract Send   : 'a -> unit
        abstract Listen : ('a -> unit) -> unit
        new () = {}
        member self.Event = self :> 'a IEvent
        interface IEvent<Handler<'a>,'a> with
            member x.Add(handler) = x.Listen(handler)
        end
      end
    let createWire() =
      let listeners = ref [] in
      {new wire<'a>() with
       member this.Send(x)   = List.iter (fun f -> f x) !listeners
       member this.Listen(f) = listeners := f :: !listeners
      }
