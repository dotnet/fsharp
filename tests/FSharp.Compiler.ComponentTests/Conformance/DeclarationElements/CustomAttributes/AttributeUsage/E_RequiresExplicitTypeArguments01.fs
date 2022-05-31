// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6163
// [<RequiresExplicitTypeArgumentsAttribute>] wasn't working on type members
// <Expects status="error" id="FS0685" span="(14,1-14,6)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>
// <Expects status="error" id="FS0685" span="(26,1-26,6)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>
// <Expects status="error" id="FS0685" span="(28,1-28,6)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>

type A() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a) = printfn "first"
    member x.Foo<'a>(y:'a, ?z:int) = printfn "second"

let a = new A()    
a.Foo(42)         // first
a.Foo<int>(42)    // first
a.Foo(42, 0)      // second
a.Foo<int>(42, 0) // second

type B() =
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a) = printfn "first"
    [<RequiresExplicitTypeArgumentsAttribute>]
    member x.Foo<'a>(y:'a, ?z:int) = printfn "second"

let b = new B()    
b.Foo(42)         // first
b.Foo<int>(42)    // first
b.Foo(42, 0)      // second
b.Foo<int>(42, 0) // second

exit 1
