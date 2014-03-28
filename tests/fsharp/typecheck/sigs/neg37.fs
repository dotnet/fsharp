module Neg37


// This test should indeed give an error: the type variable 'T escapes its scope into the type of Bar
type AAA() = 
    static member Bar = (fun x -> ())
    static member Baz<'T> () =  AAA.Bar (failwith "" : list<'T>)

// This test should indeed give an error: the use of 'Bar' at a non-uniform instantiation before we know its
// complete type w.r.t. 'T is not permitted
// Moved to neg37a - since the error was masked by other errors
//type BBB<'T>() = 
//    static member Bar = (fun x -> ())
//    static member Baz =  BBB<string>.Bar (failwith "" : 'T)

// This test should indeed give an error: the type variable 'T escapes its scope into the type of Bar
type CCC<'U>() = 
    static member Bar = (fun x -> ())
    static member Baz<'T> () =  CCC.Bar (failwith "" : list<'T>)

type Bad1<'T>(v) = 
    member x.M() = 
        (v :> IEvent<'T> |> ignore)
        (v :> System.IComparable<'T> |> ignore); 

type Bad2<'T> = 
    member x.P 
      with get v = 
        (v :> IEvent<'T> |> ignore)
        (v :> System.IComparable<'T> |> ignore); 

type Bad3<'T>() = 
    member x.P 
      with get v = 
        (v :> IEvent<'T> |> ignore)
        (v :> System.IComparable<'T> |> ignore); 

type Bad4<'T> = 
    new () = { } 
    new v = 
       (v :> IEvent<'T> |> ignore); 
       (v :> System.IComparable<'T> |> ignore);  // this should give an ambiguity error
       Bad4<'T>()
