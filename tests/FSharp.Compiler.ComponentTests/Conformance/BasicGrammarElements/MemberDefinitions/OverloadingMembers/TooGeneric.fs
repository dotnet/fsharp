// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:3388
// overloaded operators which are more generic than their enclosing class
//<Expects status="success"></Expects>
#light
type Foo<'a,'b>() =
#if TOO_GENERIC
    static member once(*<'a,'b,'c>*)(x:Foo<'a,'b>, y:Foo<'b,'c>) =
        new Foo<'a,'c>()
#else
    static member once(x:Foo<int,int>, y:Foo<int,int>) =
        new Foo<int,int>()
#endif

let inline (-->) (x : ^a) (y : ^b) = (^a: (static member once: ^a * ^b -> ^c) (x,y))

let x = new Foo<int,int>()
let y = new Foo<int,int>()
let z = x --> y
