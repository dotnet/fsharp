// #Regression #Conformance #LexFilter 
#light

// FSB 1431, 'end' token ambiguity for interface/class: Incorrect and unactionable error messages when defining class which just implements an interface

type IFoo = interface
    abstract Meth1 : int -> int
    abstract Meth2 : string -> int -> int -> float
    abstract Meth3 : unit -> unit -> unit
end

type Foo_1() = class
    interface IFoo with
        member this.Meth1 arg1 = 42
        member this.Meth2 x y z = 3.14
        member this.Meth3 () () = ()
    // No interface 'end'
end

type Foo_2() = class
    interface IFoo with
        member this.Meth1 arg1 = 42
        member this.Meth2 x y z = 3.14
        member this.Meth3 () () = ()
    // Both interface & class 'end'
    end
end

type Foo_3() =
    interface IFoo with
        member this.Meth1 arg1 = 42
        member this.Meth2 x y z = 3.14
        member this.Meth3 () () = ()
    // no interface | class 'end'

let t1 = Foo_1()
if (t1 :> IFoo).Meth1 0 <> 42 then exit 1

let t2 = Foo_2()
if (t2 :> IFoo).Meth1 0 <> 42 then exit 1

let t3 = Foo_3()
if (t3 :> IFoo).Meth1 0 <> 42 then exit 1

// Previous issue was a compile time error
exit 0
