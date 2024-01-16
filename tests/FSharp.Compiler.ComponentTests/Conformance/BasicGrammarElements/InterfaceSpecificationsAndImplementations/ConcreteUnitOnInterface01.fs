// #Regression #Conformance #DeclarationElements #InterfacesAndImplementations 
#light

// FS1 2504, Generic interface instatiated with unit

type IT<'state> =
    abstract Init: 'state -> unit

type state = unit
let dummyState:state = ()

type t() =
    interface IT<state> with
        member x.Init(a) = dummyState



let x = new t()
let itx = x :> IT<state>

let result = itx.Init(())
if result <> () then failwith "Failed: 1"
