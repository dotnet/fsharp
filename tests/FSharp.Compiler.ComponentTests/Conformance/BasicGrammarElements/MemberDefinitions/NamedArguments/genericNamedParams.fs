// #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

type Foo() =
    static member DoStuff (a, b, c, d:string) = a + b + c + d.Length
    static member DoStuff2 (a:'a) : 'a list = []
    
let r = Foo.DoStuff(a=1, b=2, c=3, d="foo")
if r <> 9 then exit 1

let r2 = Foo.DoStuff2(a=[Some("string"); None])
// Can we verify the type of r2 is a string option list list?
