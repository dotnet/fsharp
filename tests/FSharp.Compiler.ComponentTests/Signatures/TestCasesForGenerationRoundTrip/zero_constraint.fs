module Foo.Array
    
[<CompiledName("Average")>]
let inline average (array: 'T[] when ^T: (static member Zero: ^T) and ^T: (static member (+) :  ^T *  ^T ->  ^T) and ^T: (static member DivideByInt:  ^T * int ->  ^T)) : 'T =
    failwith "todo"