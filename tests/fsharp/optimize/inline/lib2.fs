namespace ThisNamespaceHasToBeTheSame
module Factory = 
    let NewRecord () = { x = 0 } 
    let NewUnionA () = A 1
    let NewUnionB () = B "1"

type Bar () = 
    member x.BarMethod() =
       Foo.FooMethod()