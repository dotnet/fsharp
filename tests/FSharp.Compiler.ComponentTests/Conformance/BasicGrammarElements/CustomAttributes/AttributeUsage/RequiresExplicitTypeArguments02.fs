// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6163
// [<RequiresExplicitTypeArgumentsAttribute>] wasn't working on type members, make sure it's inherited too

type C() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    abstract member Foo<'a> : 'a -> string
    default x.Foo<'a>(y:'a) = "first"
    member x.Foo<'a>(y:'a, ?z:int) = "other"

type D() =
    inherit C()
    override x.Foo<'a>(y:'a) = "second"

type E() =
    inherit D()
    override x.Foo<'a>(y:'a) = "third"
    
let f (x : #D) =
    x.Foo<int>(42) + x.Foo(42, 1)
    
if f (E()) <> "thirdother" then failwith "Failed: 1" else ()