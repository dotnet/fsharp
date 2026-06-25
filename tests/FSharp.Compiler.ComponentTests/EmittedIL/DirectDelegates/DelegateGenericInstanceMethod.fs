module DelegateGenericInstanceMethod

open System

type C() =
    member _.IMc<'T> (x: 'T) (y: 'T) : unit = ()
    member _.IMt<'T> (x: 'T, y: 'T) : unit = ()

// 5. eta generic instance method (curried application)
let case5_etaCurried (o: C) = Action<int, int>(fun a b -> o.IMc<int> a b)

// 37. eta generic instance method, tupled application
let case37_etaTupled (o: C) = Action<int, int>(fun a b -> o.IMt<int>(a, b))
