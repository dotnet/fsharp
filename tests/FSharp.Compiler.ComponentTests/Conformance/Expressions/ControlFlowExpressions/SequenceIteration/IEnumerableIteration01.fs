// #Conformance #ControlFlow #Sequences #Regression #InterfacesAndImplementations
// Regression for Dev10:920236, used to be a compiler error thinking this didn't have a valid GetEnumerator for iterating over

open System.Collections

// Make sure it's fixed in the face of multiple interfaces too
type IOther =
    abstract member Test : int -> int

type Arr(a : int[]) =     
    interface IOther with
        member this.Test x = x + 1

    interface IEnumerable with         
        member this.GetEnumerator() =              
            let i = ref -1
            { new IEnumerator with                 
                  member this.Reset() = failwith "not supported"                 
                  member this.MoveNext() = incr i; !i < a.Length                 
                  member this.Current = box (a.[!i]) 
            }

let a = Arr([|1;2;3;4;5|]) 
let mutable x = 0
for i in a do     
    x <- x + unbox i
exit <| (if x = 15 then 0 else 1)