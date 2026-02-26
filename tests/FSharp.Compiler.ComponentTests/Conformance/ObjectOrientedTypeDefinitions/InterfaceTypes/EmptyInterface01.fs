// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify ability to define and implement an empty interface

type IDoNothing =
    interface
    end

type IDoSomething = 
    interface
        abstract DoStuff : unit -> unit
    end

type Foo() =
    interface IDoNothing
    interface IDoSomething with
        member this.DoStuff() = ()    

type Bar() =
    interface IDoSomething with
        member this.DoStuff() = ()
    interface IDoNothing


let t1 = new Foo() :> IDoNothing
let t2 = new Bar() :> IDoNothing

exit 0
