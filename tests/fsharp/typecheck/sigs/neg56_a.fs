module Neg56

module Devdiv2_Bug_10649_repro3 = 
    open System.Collections
    
    type 'a list1 = 
        | One of 'a 
        | Many of 'a * 'a list1
        static member private toList = function
            | One x -> [x]
            | Many(x, xs) -> x :: list1.toList xs
        
        interface IEnumerable with
            member this.GetEnumerator() =
                (list1<_>.toList this :> IEnumerable).GetEnumerator()
        
        interface Generic.IEnumerable<'a> with
            member this.GetEnumerator() =
                (list1<_>.toList this :> Generic.IEnumerable<_>).GetEnumerator()
 
