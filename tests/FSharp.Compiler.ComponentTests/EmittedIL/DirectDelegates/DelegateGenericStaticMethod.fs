module DelegateGenericStaticMethod

open System

type G<'U> =
    static member SMc<'T> (x: 'T) (y: 'T) : unit = ()
    static member SMt<'T> (x: 'T, y: 'T) : unit = ()

// 7. non-eta-expanded generic static method (generic type + generic method)
let case7_nonEta () = Action<int, int>(G<string>.SMc<int>)

// 8. eta-expanded, curried application
let case8_etaCurried () = Action<int, int>(fun a b -> G<string>.SMc<int> a b)

// 9. eta-expanded, tupled application
let case9_etaTupled () = Action<int, int>(fun a b -> G<string>.SMt<int>(a, b))
