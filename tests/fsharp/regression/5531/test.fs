type Base() =
    abstract member Foo : unit -> unit
    default this.Foo() = printfn "Base"
    
type Derived() =
    inherit Base()
    member this.Foo() = printfn "Derived"
    
let inline callFoo<^T when ^T : (member Foo: unit -> unit) > (t: ^T) =
    (^T : (member Foo: unit -> unit) (t))
    
let b = Base()
let d = Derived()
let bd = d :> Base

b.Foo()
bd.Foo()
d.Foo()

callFoo<Base> b
callFoo<Base> bd
callFoo<Base> d
callFoo<Derived> d