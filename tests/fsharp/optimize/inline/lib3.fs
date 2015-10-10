namespace ASecondLibrary

open ThisNamespaceHasToBeTheSame

type Bar () = 
    member x.BarMethod() =
       Foo.FooMethod()