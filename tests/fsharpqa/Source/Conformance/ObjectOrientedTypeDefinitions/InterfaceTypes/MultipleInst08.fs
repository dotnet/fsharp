// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
type MyInt = int
[<Measure>] type kg

type IB<'a> =
    interface 
        abstract X : unit -> int
    end

type C() =
    interface IB<int<kg>> with
        member x.X() = 1
    interface IB<MyInt> with
        member x.X() = 2
    
exit 1