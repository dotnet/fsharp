module DelegateGenericStaticMethod

open System

type G<'U> =
    static member SMc<'T> (x: 'T) (y: 'T) : unit = ()
    static member SMt<'T> (x: 'T, y: 'T) : unit = ()

// 19. non-eta generic static method (generic type + generic method)
let case19_nonEta () = Action<int, int>(G<string>.SMc<int>)

// 3. eta generic static method (curried application)
let case3_etaCurried () = Action<int, int>(fun a b -> G<string>.SMc<int> a b)

// 33. eta generic static method, tupled application
let case33_etaTupled () = Action<int, int>(fun a b -> G<string>.SMt<int>(a, b))
