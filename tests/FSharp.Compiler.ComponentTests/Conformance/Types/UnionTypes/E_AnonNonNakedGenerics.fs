// Anonymous Union Types
// Anonymous union type consisting of non-naked generics - ambiguous function application

let id<'T, 'U>(x: (list<'T> | list<'U>)): (list<'T> | list<'U>) =
    x

id<int, string> [] |> ignore