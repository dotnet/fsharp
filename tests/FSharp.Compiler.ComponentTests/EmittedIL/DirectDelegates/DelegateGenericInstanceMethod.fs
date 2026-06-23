module DelegateGenericInstanceMethod

open System

type C() =
    member _.IMc<'T> (x: 'T) (y: 'T) : unit = ()
    member _.IMt<'T> (x: 'T, y: 'T) : unit = ()

// 13. eta-expanded, generic instance method, curried application
let case13_etaCurried (o: C) = Action<int, int>(fun a b -> o.IMc<int> a b)

// 14. eta-expanded, generic instance method, tupled application
let case14_etaTupled (o: C) = Action<int, int>(fun a b -> o.IMt<int>(a, b))
