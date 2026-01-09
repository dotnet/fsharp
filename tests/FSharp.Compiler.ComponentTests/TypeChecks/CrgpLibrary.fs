module MyModule

type IFoo<'T when 'T :> IFoo<'T>> =
    abstract member Bar: other:'T -> unit

[<AbstractClass>]
type FooBase() =
    
    interface IFoo<FooBase> with
        member this.Bar (other: FooBase) = ()

[<Sealed>]
type FooDerived<'T>() =
    inherit FooBase()
    
    interface IFoo<FooDerived<'T>> with
        member this.Bar other = ()

type IFooContainer<'T> =
    abstract member Foo: FooDerived<'T>

let inline bar<'a when 'a :> IFoo<'a>> (x: 'a) (y: 'a) = x.Bar y
let inline takeSame<'a> (x: 'a) (y: 'a) = ()

// Successfully compiles under .NET 9 + F# 9
// Error under .NET 10 + F# 10: Program.fs(26,13): Error FS0193 : The type 'FooDerived<'TId>' does not match the type 'FooBase'
let callBar_NewlyBroken (foo1: IFooContainer<'TId>) (foo2: IFooContainer<'TId>) =
    bar foo1.Foo foo2.Foo
    
// Successfully compiles under both versions
let callBar (foo1: IFooContainer<'TId>) (foo2: IFooContainer<'TId>) =
    let id1 = foo1.Foo
    let id2 = foo2.Foo
    bar id1 id2