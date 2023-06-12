// #Conformance #DeclarationElements #LetBindings #TypeAnnotations #TypeInference #TypeConstraints 
let empty<'a when 'a : comparison> : ('a list * 'a Set) = ([], Set.empty)
let empties = empty<unit option list list option>  // optional list of lists of lists of optional units
printfn "Empties = %A" empties

let add x y : float = x + y
let r1 = add 1.0 2.0

type Foo() =
    member this.DoStuff with get() : unit list = []
    member this.DoStuff with set x : unit = ()

let r2 = (new Foo()).DoStuff
(new Foo()).DoStuff <- [();()]
