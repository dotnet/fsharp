// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6163
// [<RequiresExplicitTypeArgumentsAttribute>] wasn't working on type members

type A() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a) = "first"
    member x.Foo<'a>(y:'a, ?z:int) = "second"

let a = new A()    
if a.Foo<int>(42) <> "first" then failwith "Failed: 1"
if a.Foo(42, 0) <> "second" then failwith "Failed: 2"
if a.Foo<int>(42, 0) <> "second" then failwith "Failed: 3"

type B() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a) = "first"
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a, ?z:int) = "second"

let b = new B()    
if b.Foo<int>(42) <> "first" then failwith "Failed: 4"
if b.Foo<int>(42, 0) <> "second" then failwith "Failed: 5"
