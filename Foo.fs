module Meh

let bar (name:string) : 'a = failwith<'a> "todo"

#nowarn "40"

type Foo () =
    member __.Bar (name : string) : unit -> 'a =
            let rec prov : (unit -> 'a) ref = ref (fun () ->
                let p : (unit -> 'a) = bar name
                prov := p
                p())
            fun () -> (!prov)()