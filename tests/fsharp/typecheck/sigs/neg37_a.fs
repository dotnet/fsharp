module Neg37a

// This test should indeed give an error: the use of 'Bar' at a non-uniform instantiation before we know its
// complete type w.r.t. 'T is not permitted
type BBB<'T>() = 
    static member Bar = (fun x -> ())
    static member Baz =  BBB<string>.Bar (failwith "" : 'T)

