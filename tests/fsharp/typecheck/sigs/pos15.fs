[<Struct>]
type Enumerator =
    member this.MoveNext() = false
    member this.Current = 1
 
type Collection() = 
    member this.GetEnumerator() = Enumerator()
 
[for c in Collection() do yield c]