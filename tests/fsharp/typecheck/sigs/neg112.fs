module Something
type Tuple =
    static member inline TupleMap ((a, b),          _ : Tuple) = fun f -> (f a, f b)
    static member inline TupleMap ((a, b, c),       _ : Tuple) = fun f -> (f a, f b, f c)
    static member inline TupleMap ((a, b, c, d),    _ : Tuple) = fun f -> (f a, f b, f c, f d)
    
    static member inline Map f (x: 'Tin) : 'Tout =
        let inline call_2 (a: ^a, b : ^b) = ((^a or ^b) : (static member TupleMap : ^b * _ -> ^t) (b, a))
        call_2 (Unchecked.defaultof<Tuple>, x) f

type IOption<'T> =
    abstract member Value : 'T
    
let inline tupleMap f x = Tuple.Map f x

let inline addOptionValues<^value, ^options, ^values, 'item when 
                              'item :> IOption<^value>>
                              (addUp : ^values -> ^value, sourceOptions : ^options) =
    let getValue (i : 'item) = i.Value
    let allValues : ^values = tupleMap getValue sourceOptions
    let result : ^value = addUp allValues
    result
