// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6163
// [<RequiresExplicitTypeArgumentsAttribute>] wasn't working on type members, make sure it's inherited too
// <Expects status="error" id="FS0685" span="(20,5-20,10)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>

type C() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    abstract member Foo<'a> : 'a -> unit
    default x.Foo<'a>(y:'a) = printfn "first"

type D() =
    inherit C()
    override x.Foo<'a>(y:'a) = printfn "second"

type E() =
    inherit D()
    override x.Foo<'a>(y:'a) = printfn "third"
    
let f (x : #D) =
    x.Foo(42)

exit 1
    
