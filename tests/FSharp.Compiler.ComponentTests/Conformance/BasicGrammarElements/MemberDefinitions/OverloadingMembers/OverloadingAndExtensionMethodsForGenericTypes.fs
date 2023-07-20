// #Regression 
// Regression for 353661 [FSharp] The code below shows an error when editing, but no error when compiled.
open System.Linq

type T = 
    abstract Count : int -> bool
    default this.Count(_ : int) = true

    interface System.Collections.Generic.IEnumerable<int> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<int> = failwith "not implemented"
    interface System.Collections.IEnumerable with
        member this.GetEnumerator() : System.Collections.IEnumerator = failwith "not implemented"

let g (t : T) = t.Count()